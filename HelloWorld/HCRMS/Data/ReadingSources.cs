using System.Globalization;

namespace HCRMS.Data
{
    // Gives the next set of readings for every (RoomID, DeviceID)
    // A missing or null value means the sensor sent no data
    public interface IReadingSource
    {
        string Description { get; }
        Dictionary<(string RoomID, string DeviceID), double?> NextReadings();
    }

    public static class ReadingSources
    {
        // Uses the CSV files in the folder if there are any,
        // otherwise falls back to the built-in simulator.
        public static IReadingSource Create(string csvFolder, IEnumerable<Room> rooms)
        {
            return (IReadingSource?)CsvReadingSource.FromFolder(csvFolder)
                   ?? new SimulatedReadingSource(rooms);
        }
    }

    // Reads CSV files in the IR-03 format, one reading per line:
    //     timestamp,roomId,sensorId,value
    //     2026-11-20 08:00:00,ICU-01,TEMP-01,22.5
    // Rows with the same timestamp are read together, and the files loop when they end
    // A bad value only makes that one sensor Failed (NFR-03)
    public class CsvReadingSource : IReadingSource
    {
        private readonly List<Dictionary<(string RoomID, string DeviceID), double?>> frames = new();
        private int nextFrame;

        public string Description { get; }
        public int FrameCount => frames.Count;

        public CsvReadingSource(IEnumerable<string> files)
        {
            var byTime = new SortedDictionary<DateTime, Dictionary<(string, string), double?>>();
            int fileCount = 0;

            foreach (string file in files)
            {
                string[] lines;
                try
                {
                    lines = File.ReadAllLines(file);
                    fileCount++;
                }
                catch (Exception ex) when (ex is IOException || ex is UnauthorizedAccessException)
                {
                    continue; // An unreadable file only affects its own sensors.
                }

                foreach (string line in lines)
                {
                    if (string.IsNullOrWhiteSpace(line))
                        continue;

                    string[] fields = line.Split(',');
                    if (fields[0].Trim().Equals("timestamp", StringComparison.OrdinalIgnoreCase))
                        continue; // header row

                    // Skip lines without four fields or a valid timestamp
                    if (fields.Length < 4 ||
                        !DateTime.TryParse(fields[0].Trim(), CultureInfo.InvariantCulture,
                                           DateTimeStyles.None, out DateTime timestamp))
                        continue;

                    string roomId = fields[1].Trim().ToUpperInvariant();
                    string deviceId = fields[2].Trim().ToUpperInvariant();
                    double? value = double.TryParse(fields[3].Trim(), NumberStyles.Float,
                                                    CultureInfo.InvariantCulture, out double v)
                        ? v
                        : null;

                    if (!byTime.TryGetValue(timestamp, out var frame))
                    {
                        frame = new Dictionary<(string, string), double?>();
                        byTime[timestamp] = frame;
                    }
                    frame[(roomId, deviceId)] = value;
                }
            }

            frames.AddRange(byTime.Values);
            Description = $"CSV files ({fileCount} file(s), {frames.Count} time steps)";
        }

        // Returns null when the folder is missing or has no usable rows.
        public static CsvReadingSource? FromFolder(string folder)
        {
            if (!Directory.Exists(folder))
                return null;

            string[] files = Directory.GetFiles(folder, "*.csv");
            Array.Sort(files, StringComparer.OrdinalIgnoreCase);
            if (files.Length == 0)
                return null;

            var source = new CsvReadingSource(files);
            return source.FrameCount > 0 ? source : null;
        }

        public Dictionary<(string RoomID, string DeviceID), double?> NextReadings()
        {
            if (frames.Count == 0)
                return new Dictionary<(string, string), double?>();

            var frame = frames[nextFrame];
            nextFrame = (nextFrame + 1) % frames.Count;
            return new Dictionary<(string, string), double?>(frame);
        }
    }

    // Built-in simulator, used when there are no CSV files
    // Each sensor stays near a normal value and sometimes drifts into
    // warning or critical for a while, so alerts can be demonstrated
    public class SimulatedReadingSource : IReadingSource
    {
        private const double ExcursionChance = 0.003;

        private sealed class Channel
        {
            public double Baseline;
            public double Noise;
            public double ExcursionTarget;
            public int Decimals;
            public double Value;
            public int ExcursionTicksLeft;
        }

        private readonly Dictionary<(string RoomID, string DeviceID), Channel> channels = new();
        private readonly Random random;

        public string Description => "Built-in simulator";

        public SimulatedReadingSource(IEnumerable<Room> rooms, int? seed = null)
        {
            random = seed.HasValue ? new Random(seed.Value) : new Random();

            foreach (Room room in rooms)
                foreach (var sensor in room.Sensors)
                    channels[(room.RoomID, sensor.DeviceID)] = ProfileFor(sensor.DeviceID);
        }

        public Dictionary<(string RoomID, string DeviceID), double?> NextReadings()
        {
            var readings = new Dictionary<(string, string), double?>();

            foreach (var (key, channel) in channels)
            {
                if (channel.ExcursionTicksLeft == 0 && random.NextDouble() < ExcursionChance)
                    channel.ExcursionTicksLeft = random.Next(8, 20);

                double target = channel.ExcursionTicksLeft > 0 ? channel.ExcursionTarget : channel.Baseline;
                if (channel.ExcursionTicksLeft > 0)
                    channel.ExcursionTicksLeft--;

                channel.Value += (target - channel.Value) * 0.25
                               + (random.NextDouble() * 2 - 1) * channel.Noise;
                if (channel.Value < 0)
                    channel.Value = 0;

                readings[key] = Math.Round(channel.Value, channel.Decimals);
            }
            return readings;
        }

        // Normal value, noise and excursion target per sensor type (by ID prefix).
        private static Channel ProfileFor(string deviceId)
        {
            string prefix = deviceId.Split('-')[0].ToUpperInvariant();
            (double baseline, double noise, double target, int decimals) = prefix switch
            {
                "TEMP" => (21.5, 0.15, 31.5, 1),
                "HUM" => (45.0, 0.6, 74.0, 1),
                "OXY" => (20.9, 0.05, 23.8, 2),
                "AQI" => (40.0, 2.0, 165.0, 0),
                "PRESS" => (5.0, 0.3, 22.0, 1),
                "SMOKE" => (1.0, 0.2, 30.0, 1),
                "PWR" => (220.0, 1.0, 196.0, 0),
                _ => (10.0, 0.5, 100.0, 1)
            };

            return new Channel
            {
                Baseline = baseline,
                Noise = noise,
                ExcursionTarget = target,
                Decimals = decimals,
                Value = baseline
            };
        }
    }
}
