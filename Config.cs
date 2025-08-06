
using System.Reflection.Metadata;
using System.Text.Json;
using System.Text.Json.Serialization;
using VirtualOperatorServer.Facade;

namespace VirtualOperatorServer.Configuration
{

    public class StaticConfig
    {
        private static readonly StaticConfig _instance = new();
        private readonly string _stepperConfigFile;
        private readonly string _timerConfigFile;
        static readonly JsonSerializerOptions _serializerOption = new()
        {
            Converters = { new JsonStringEnumConverter() },
            IncludeFields = true
        };


        private void LoadStepperConfig()
        {
            try
            {
                if (!File.Exists(_stepperConfigFile))
                    throw new FileNotFoundException("The file does not exist.", _stepperConfigFile);

                string jsonContent = File.ReadAllText(_stepperConfigFile);
                var tmpConfigs = JsonSerializer.Deserialize<StatusFacade.Facade.Stepper.Configuration[]>(jsonContent, _serializerOption);

                if (tmpConfigs == null)
                    throw new Exception($"Failed to deserialize '{_stepperConfigFile}'");
                else
                    StepperConfigs = tmpConfigs;
            }
            catch (FileNotFoundException ex)
            {
                Console.WriteLine($"File not found: {ex.FileName}");
            }
            catch (JsonException ex)
            {
                Console.WriteLine($"Invalid JSON: {ex.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Unexpected error: {ex.Message}");
            }
        }

        private void LoadTimerConfig()
        {
            try
            {
                if (!File.Exists(_timerConfigFile))
                    throw new FileNotFoundException("The file does not exist.", _timerConfigFile);

                string jsonContent = File.ReadAllText(_timerConfigFile);
                var tmpConfigs = JsonSerializer.Deserialize<ushort[]>(jsonContent, _serializerOption);

                if (tmpConfigs == null)
                    throw new Exception($"Failed to deserialize '{_timerConfigFile}'");
                else
                    TimerConfigs = tmpConfigs;
            }
            catch (FileNotFoundException ex)
            {
                Console.WriteLine($"File not found: {ex.FileName}");
            }
            catch (JsonException ex)
            {
                Console.WriteLine($"Invalid JSON: {ex.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Unexpected error: {ex.Message}");
            }
        }

        private void SaveFile(string pathName, string content)
        {
            try
            {
                Directory.CreateDirectory(Path.GetDirectoryName(pathName)!);
                File.WriteAllText(pathName, content);
            }
            catch (IOException ex)
            {
                Console.WriteLine("I/O error: " + ex.Message);
            }
            catch (UnauthorizedAccessException ex)
            {
                Console.WriteLine("Access denied: " + ex.Message);
            }
        }

        private StaticConfig()
        {
            _stepperConfigFile = Path.Join(Directory.GetCurrentDirectory(), "configs", "stepperConfig.json");
            _timerConfigFile = Path.Join(Directory.GetCurrentDirectory(), "configs", "timerConfig.json");

            LoadStepperConfig();
            LoadTimerConfig();
        }

        public static StaticConfig Instance => _instance;

        public StatusFacade.Facade.Stepper.Configuration[] StepperConfigs { get; private set; } = new StatusFacade.Facade.Stepper.Configuration[StatusFacade.Facade.StepperCount];
        public ushort[] TimerConfigs { get; private set; } = new ushort[StatusFacade.Facade.FlexTimerCount + 1];

        public void SaveStepperConfigs()
        {
            string jsonStr = JsonSerializer.Serialize(StepperConfigs, _serializerOption);
            SaveFile(_stepperConfigFile, jsonStr);
        }

        public void SaveTimerConfigs()
        {
            string jsonStr = JsonSerializer.Serialize(TimerConfigs);
            SaveFile(_timerConfigFile, jsonStr);
        }
    }

    public class DynamicConfig
    {
        public static StatusFacade.Facade.Stepper.Configuration[] StepperConfigs = StaticConfig.Instance.StepperConfigs;
        public static ushort[] TimerConfigs = StaticConfig.Instance.TimerConfigs;
    }
}


