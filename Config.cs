
using System.Reflection.Metadata;
using System.Text.Json;
using System.Text.Json.Serialization;
using VirtualOperatorServer.Facade;

namespace VirtualOperatorServer.Config
{

    public class Config
    {
        private static readonly Config _instance = new();
        private readonly string _stepperConfigFile;
        private readonly string _timerConfigFile;
        static readonly JsonSerializerOptions _serializerOption = new()
        {
            Converters = { new JsonStringEnumConverter() },
            IncludeFields = true
        };


        private void LoadStepperConfig()
        {
            StatusFacade.Facade.Stepper.Configuration[]? tmpConfigs = null;

            try
            {
                if (!File.Exists(_stepperConfigFile))
                    throw new FileNotFoundException("The file does not exist.", _stepperConfigFile);

                string jsonContent = File.ReadAllText(_stepperConfigFile);
                tmpConfigs = JsonSerializer.Deserialize<StatusFacade.Facade.Stepper.Configuration[]>(jsonContent);

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
            ushort[]? tmpConfigs = null;

            try
            {
                if (!File.Exists(_timerConfigFile))
                    throw new FileNotFoundException("The file does not exist.", _timerConfigFile);

                string jsonContent = File.ReadAllText(_timerConfigFile);
                tmpConfigs = JsonSerializer.Deserialize<ushort[]>(jsonContent);

                if (tmpConfigs == null)
                    throw new Exception($"Failed to deserialize '{_timerConfigFile}'");
                else
                    TimerPrescalers = tmpConfigs;
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

        private Config()
        {
            _stepperConfigFile = Path.Join(Directory.GetCurrentDirectory(), "configs", "stepperConfig.json");
            _timerConfigFile = Path.Join(Directory.GetCurrentDirectory(), "configs", "timerConfig.json");

            LoadStepperConfig();
            LoadTimerConfig();
        }

        public static Config Instance => _instance;

        public StatusFacade.Facade.Stepper.Configuration[] StepperConfigs { get; private set; } = new StatusFacade.Facade.Stepper.Configuration[StatusFacade.Facade.StepperCount];
        public ushort[] TimerPrescalers { get; private set; } = new ushort[StatusFacade.Facade.FlexTimerCount + 1];

        public void SaveStepperConfigs(in StatusFacade.Facade.Stepper.Configuration[] configs)
        {
            if (configs.Length != StepperConfigs.Length)
            {
                Console.WriteLine("Error: invalid stepper configurations");
                return;
            }

            StepperConfigs = configs;

            string jsonStr = JsonSerializer.Serialize(StepperConfigs, _serializerOption);
            SaveFile(_stepperConfigFile, jsonStr);
        }

        public void SaveStepperConfig(uint index, in StatusFacade.Facade.Stepper.Configuration config)
        {
            var configs = StepperConfigs;

            if (index >= configs.Length)
            {
                Console.WriteLine($"Error: invalid stepper index {index}");
                return;
            }

            configs[index] = config;

            SaveStepperConfigs(configs);
        }

        public void SaveTimerConfigs(in ushort[] configs)
        {
            if (configs.Length != TimerPrescalers.Length)
            {
                Console.WriteLine("Error: invalid Timer configurations");
                return;
            }

            TimerPrescalers = configs;
            string jsonStr = JsonSerializer.Serialize(TimerPrescalers);
            SaveFile(_timerConfigFile, jsonStr);
        }

        public void SaveTimerConfig(uint index, ushort config)
        {
            var configs = TimerPrescalers;

            if (index >= configs.Length)
            {
                Console.WriteLine($"Error: invalid timer index {index}");
                return;
            }

            configs[index] = config;

            SaveTimerConfigs(configs);
        }
    }
}


