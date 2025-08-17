
using System.Reflection.Metadata;
using System.Text.Json;
using System.Text.Json.Serialization;
using VirtualOperatorServer.Facade;

namespace VirtualOperatorServer.Configuration
{
    /// <summary>
    /// StaticConfig is singleton of steppers and timers configurations.
    /// It loads configurations from files when constructed, and provides
    /// APIs to save stepper and timer configurations respectively.
    /// </summary>        
    public class StaticConfig
    {
        private static readonly StaticConfig _instance = new();
        private readonly string _stepperConfigFile;
        private readonly string _timerConfigFile;

        private void LoadStepperConfig()
        {
            try
            {
                if (!File.Exists(_stepperConfigFile))
                    throw new FileNotFoundException("The file does not exist.", _stepperConfigFile);

                string jsonContent = File.ReadAllText(_stepperConfigFile);
                var tmpConfigs = JsonSerializer.Deserialize<StatusFacade.Facade.Stepper.Configuration[]>(jsonContent);

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

            TimerConfigs = new ushort[StatusFacade.Facade.FlexTimerCount + 1];

            StepperConfigs = new StatusFacade.Facade.Stepper.Configuration[StatusFacade.Facade.StepperCount];
            for (int i = 0; i < StepperConfigs.Length; i++)
            {
                StepperConfigs[i] = new StatusFacade.Facade.Stepper.Configuration();
            }

            LoadStepperConfig();
            LoadTimerConfig();
        }

        public static StaticConfig Instance => _instance;

        public StatusFacade.Facade.Stepper.Configuration[] StepperConfigs { get; private set; }
        public ushort[] TimerConfigs { get; private set; } 

        public void SaveStepperConfigs()
        {
            string jsonStr = JsonSerializer.Serialize<StatusFacade.Facade.Stepper.Configuration[]>(StepperConfigs);
            SaveFile(_stepperConfigFile, jsonStr);
        }

        public void SaveTimerConfigs()
        {
            string jsonStr = JsonSerializer.Serialize(TimerConfigs);
            SaveFile(_timerConfigFile, jsonStr);
        }
    }
}


