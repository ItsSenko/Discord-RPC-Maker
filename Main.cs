using DiscordRPC;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace DiscordRPCMaker
{
    internal class Discord
    {
        public static void LoadIcon()
        {
            if (!File.Exists($"{Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData)}Low/Senko/Icon.ico"))
            {
                using Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("DiscordRPCMakerDotNet.SimpleFoxLogo.ico");
                if (stream != null)
                {
                    using FileStream fs = new FileStream($"{Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData)}Low/Senko/Icon.ico", FileMode.Create);
                    stream.CopyTo(fs);
                    fs.Close();
                    stream.Close();
                }
                IconChanger.SetConsoleIcon($"{Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData)}Low/Senko/Icon.ico");
            }
        }
        public static void Log(Type type, string txt, bool color = false, bool NewLine = true)
        {
            if (type != Type.Empty)
            {
                var time = DateTime.Now.ToString("[hh:mm.ss] ");
                Console.ForegroundColor = ConsoleColor.DarkMagenta;
                Console.Write(time);
            }
            switch (type)
            {
                case Type.Info:
                    {
                        Console.ForegroundColor = ConsoleColor.Gray;
                        Console.Write($"[{type}] ");
                        Console.ForegroundColor = ConsoleColor.DarkGray;
                        if (NewLine)
                        {
                            Console.WriteLine(txt);
                        }
                        else
                        {
                            Console.Write(txt);
                        }
                    }
                    break;
                case Type.Warn:
                    {
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        Console.WriteLine($"[{type}] {txt}");
                    }
                    break;
                case Type.Error:
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine($"[{type}] {txt}");
                    }
                    break;
                case Type.Discord:
                    {
                        Console.ForegroundColor = ConsoleColor.DarkCyan;
                        Console.Write($"[{type}] ");
                        Console.ForegroundColor = ConsoleColor.DarkGray;
                        if (NewLine)
                        {
                            Console.WriteLine(txt);
                        }
                        else
                        {
                            Console.Write(txt);
                        }
                    }
                    break;
                case Type.Success:
                    {
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine($"[{type}] {txt}");
                    }
                    break;
                case Type.Empty:
                    {
                        Console.ForegroundColor = color ? ConsoleColor.DarkMagenta : ConsoleColor.DarkGray;
                        Console.WriteLine(txt);
                    }
                    break;

            }
        }
        public static void Space()
        {
            Console.WriteLine();
        }
        public static bool GetAnswer()
        {
            var answer = Console.ReadLine().ToLower();
            switch (answer)
            {
                case "y":
                    return true;
                case "n":
                    return false;
                default:
                    {
                        if (string.IsNullOrEmpty(answer))
                        {
                            return true;
                        }
                        Log(Type.Error, "Invalid Response");
                        return false;
                    }
            }
        }
        [STAThread]
        static void Main(string[] args)
        {
            Console.Title = "Discord RPC Maker - By Senko";
            Log(Type.Empty, ASCII.Logo, true);
            Log(Type.Info, "Welcome to Senko's Simple Discord RPC Maker!");
            Presets.Clear();
            ReadPresets();
            SavePreset();
            LoadIcon();
            Log(Type.Empty, "[S] Start");
            Log(Type.Empty, "[?] Settings");
            var answer = Console.ReadLine().ToLower();
            if (answer != "s" && answer != "?" && !string.IsNullOrEmpty(answer))
            {
                Log(Type.Error, "Invalid Response, Resetting...");
                Console.Clear();
                Main(new string[] { answer });
                return;
            }
            if (answer == "?")
            {
                Settings();
            }
            else if (answer == "s")
            {
                Start();
            }
            else if (string.IsNullOrEmpty(answer))
            {
                Start();
            }
        }
        public static void Settings()
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.DarkMagenta;
            Log(Type.Empty, ASCII.Cogwheel, true);
            Log(Type.Info, "Welcome to the settings menu, please select a option");
            Space();
            Log(Type.Empty, "[D] Delete a Preset");
            Log(Type.Empty, "[E] Edit a Preset");
            Log(Type.Empty, "[B] Back to Main Menu");
            var answer = Console.ReadLine().ToLower();
            switch (answer)
            {
                case "d":
                    {
                        if (Presets.Count == 0)
                        {
                            Settings();
                            break;
                        }
                        Log(Type.Info, "Please select a preset to delete");
                        Space();
                        for (int i = 0; i < Presets.Count; i++)
                        {
                            Log(Type.Empty, $"[{i}] {Presets[i].Name}");
                        }
                        var response = Console.ReadLine();
                        if (!int.TryParse(response, out var value))
                        {
                            Log(Type.Error, "Invalid Response, Failed to parse, Resetting...");
                            Settings();
                            break;
                        }
                        if (Presets[value] == null)
                        {
                            Log(Type.Error, "Invalid Response, Preset does not exist, Resetting...");
                            Settings();
                            break;
                        }
                        Log(Type.Warn, "Are you sure you want to DELETE this preset?");
                        Space();
                        Log(Type.Empty, $"[Y] Yes");
                        Log(Type.Empty, $"[N] No");
                        if (GetAnswer())
                        {
                            Log(Type.Info, $"Deleting {Presets[value].Name}");
                            Presets.Remove(Presets[value]);
                            SavePreset();
                            Log(Type.Success, "Preset Deleted, Press enter to go back to the Settings Menu");
                            var key = Console.ReadKey();
                            Settings();
                            break;
                        }
                        else
                        {
                            Settings();
                            break;
                        }
                    }
                case "e":
                    {
                        if (Presets.Count == 0)
                        {
                            Settings();
                            break;
                        }
                        Log(Type.Info, "Please select a preset to edit");
                        Space();
                        for (int i = 0; i < Presets.Count; i++)
                        {
                            Log(Type.Empty, $"[{i}] {Presets[i].Name}");
                        }
                        var response = Console.ReadLine();
                        var value = 0;
                        if (!string.IsNullOrEmpty(response))
                        {
                            if (!int.TryParse(response, out value))
                            {
                                Log(Type.Error, "Invalid Response, Failed to parse, Resetting...");
                                Settings();
                                break;
                            }
                        }
                        if (Presets[value] == null)
                        {
                            Log(Type.Error, "Invalid Response, Preset does not exist, Resetting...");
                            Settings();
                            break;
                        }
                        var preset = Presets[value];
                        Log(Type.Info, $"What would you like to edit in the {preset.Name} Preset?");
                        Space();
                        Log(Type.Empty, $"[N] Name: {preset.Name}");
                        Log(Type.Empty, $"[C] Client ID: {preset.clientId}");
                        Log(Type.Empty, $"[S] State: {preset.state}");
                        Log(Type.Empty, $"[D] Details: {preset.details}");
                        Log(Type.Empty, $"[LI] Large Image Key: {preset.largeImageKey}");
                        Log(Type.Empty, $"[SI] Small Image Key: {preset.smallImageKey}");
                        Log(Type.Empty, $"[LT] Large Image Text: {preset.largeImageText}");
                        Log(Type.Empty, $"[ST] Small Image Text: {preset.smallImageText}");
                        Log(Type.Empty, $"[BN1] Button 1 Name: {preset.button1Name}");
                        Log(Type.Empty, $"[BU1] Button 1 Url: {preset.button1Url}");
                        Log(Type.Empty, $"[BN2] Button 2 Name: {preset.button2Name}");
                        Log(Type.Empty, $"[BU2] Button 2 Url: {preset.button2Url}");
                        var a = Console.ReadLine().ToLower();
                        switch (a)
                        {
                            case "n":
                                {
                                    Log(Type.Info, $"Please set the new Name for this preset: ", false, false);
                                    var n = Console.ReadLine();
                                    Log(Type.Info, $"Your new preset Name will be {n}, Continue?");
                                    Space();
                                    Log(Type.Empty, "[Y] Yes");
                                    Log(Type.Empty, "[N] No");
                                    if (GetAnswer())
                                    {
                                        preset.Name = n;
                                        SavePreset();
                                        Log(Type.Success, $"Preset Name changed to {n}, Press enter to return to the Settings Menu");
                                        var key = Console.ReadKey();
                                        Settings();
                                        break;
                                    }
                                    else
                                    {
                                        Log(Type.Error, "Invalid Response");
                                        Settings();
                                        break;
                                    }
                                }
                            case "c":
                                {
                                    Log(Type.Info, $"Please set the new Client ID for this preset: ", false, false);
                                    var n = Console.ReadLine();
                                    Log(Type.Info, $"Your new preset Client ID will be {n}, Continue?");
                                    Space();
                                    Log(Type.Empty, "[Y] Yes");
                                    Log(Type.Empty, "[N] No");
                                    if (GetAnswer())
                                    {
                                        preset.clientId = n;
                                        SavePreset();
                                        Log(Type.Success, $"Preset Client ID changed to {n}, Press enter to return to the Settings Menu");
                                        var key = Console.ReadKey();
                                        Settings();
                                        break;
                                    }
                                    else
                                    {
                                        Log(Type.Error, "Invalid Response");
                                        Settings();
                                        break;
                                    }
                                }
                            case "s":
                                {
                                    Log(Type.Info, $"Please set the new State for this preset: ", false, false);
                                    var n = Console.ReadLine();
                                    Log(Type.Info, $"Your new preset State will be {n}, Continue?");
                                    Space();
                                    Log(Type.Empty, "[Y] Yes");
                                    Log(Type.Empty, "[N] No");
                                    if (GetAnswer())
                                    {
                                        preset.state = n;
                                        SavePreset();
                                        Log(Type.Success, $"Preset State changed to {n}, Press enter to return to the Settings Menu");
                                        var key = Console.ReadKey();
                                        Settings();
                                        break;
                                    }
                                    else
                                    {
                                        Log(Type.Error, "Invalid Response");
                                        Settings();
                                        break;
                                    }
                                }
                            case "d":
                                {
                                    Log(Type.Info, $"Please set the new Details for this preset: ", false, false);
                                    var n = Console.ReadLine();
                                    Log(Type.Info, $"Your new preset Details will be {n}, Continue?");
                                    Space();
                                    Log(Type.Empty, "[Y] Yes");
                                    Log(Type.Empty, "[N] No");
                                    if (GetAnswer())
                                    {
                                        preset.details = n;
                                        SavePreset();
                                        Log(Type.Success, $"Preset Details changed to {n}, Press enter to return to the Settings Menu");
                                        var key = Console.ReadKey();
                                        Settings();
                                        break;
                                    }
                                    else
                                    {
                                        Log(Type.Error, "Invalid Response");
                                        Settings();
                                        break;
                                    }
                                }
                            case "li":
                                {
                                    Log(Type.Info, $"Please set the new Large Image Key for this preset: ", false, false);
                                    var n = Console.ReadLine();
                                    Log(Type.Info, $"Your new preset Large Image Key will be {n}, Continue?");
                                    Space();
                                    Log(Type.Empty, "[Y] Yes");
                                    Log(Type.Empty, "[N] No");
                                    if (GetAnswer())
                                    {
                                        preset.largeImageKey = n;
                                        SavePreset();
                                        Log(Type.Success, $"Preset Large Image Key changed to {n}, Press enter to return to the Settings Menu");
                                        var key = Console.ReadKey();
                                        Settings();
                                        break;
                                    }
                                    else
                                    {
                                        Log(Type.Error, "Invalid Response");
                                        Settings();
                                        break;
                                    }
                                }
                            case "si":
                                {
                                    Log(Type.Info, $"Please set the new Small Image Key for this preset: ", false, false);
                                    var n = Console.ReadLine();
                                    Log(Type.Info, $"Your new preset Small Image Key will be {n}, Continue?");
                                    Space();
                                    Log(Type.Empty, "[Y] Yes");
                                    Log(Type.Empty, "[N] No");
                                    if (GetAnswer())
                                    {
                                        preset.smallImageKey = n;
                                        SavePreset();
                                        Log(Type.Success, $"Preset Small Image Key changed to {n}, Press enter to return to the Settings Menu");
                                        var key = Console.ReadKey();
                                        Settings();
                                        break;
                                    }
                                    else
                                    {
                                        Log(Type.Error, "Invalid Response");
                                        Settings();
                                        break;
                                    }
                                }
                            case "lt":
                                {
                                    Log(Type.Info, $"Please set the new Large Image Text for this preset: ", false, false);
                                    var n = Console.ReadLine();
                                    Log(Type.Info, $"Your new preset Large Image Text will be {n}, Continue?");
                                    Space();
                                    Log(Type.Empty, "[Y] Yes");
                                    Log(Type.Empty, "[N] No");
                                    if (GetAnswer())
                                    {
                                        preset.largeImageText = n;
                                        SavePreset();
                                        Log(Type.Success, $"Preset Large Image Text changed to {n}, Press enter to return to the Settings Menu");
                                        var key = Console.ReadKey();
                                        Settings();
                                        break;
                                    }
                                    else
                                    {
                                        Log(Type.Error, "Invalid Response");
                                        Settings();
                                        break;
                                    }
                                }
                            case "st":
                                {
                                    Log(Type.Info, $"Please set the new Small Image Text for this preset: ", false, false);
                                    var n = Console.ReadLine();
                                    Log(Type.Info, $"Your new preset Small Image Text will be {n}, Continue?");
                                    Space();
                                    Log(Type.Empty, "[Y] Yes");
                                    Log(Type.Empty, "[N] No");
                                    if (GetAnswer())
                                    {
                                        preset.smallImageText = n;
                                        SavePreset();
                                        Log(Type.Success, $"Preset Small Image Text changed to {n}, Press enter to return to the Settings Menu");
                                        var key = Console.ReadKey();
                                        Settings();
                                        break;
                                    }
                                    else
                                    {
                                        Log(Type.Error, "Invalid Response");
                                        Settings();
                                        break;
                                    }
                                }
                            case "bn1":
                                {
                                    Log(Type.Info, $"Please set the new Button Name for this preset: ", false, false);
                                    var n = Console.ReadLine();
                                    Log(Type.Info, $"Your new preset Button Name will be {n}, Continue?");
                                    Space();
                                    Log(Type.Empty, "[Y] Yes");
                                    Log(Type.Empty, "[N] No");
                                    if (GetAnswer())
                                    {
                                        preset.button1Name = n;
                                        SavePreset();
                                        Log(Type.Success, $"Preset Button Name changed to {n}, Press enter to return to the Settings Menu");
                                        var key = Console.ReadKey();
                                        Settings();
                                        break;
                                    }
                                    else
                                    {
                                        Log(Type.Error, "Invalid Response");
                                        Settings();
                                        break;
                                    }
                                }
                                break;
                            case "bu1":
                                {
                                    Log(Type.Info, $"Please set the new Button Url for this preset: ", false, false);
                                    var n = Console.ReadLine();
                                    Log(Type.Info, $"Your new preset Button Url will be {n}, Continue?");
                                    Space();
                                    Log(Type.Empty, "[Y] Yes");
                                    Log(Type.Empty, "[N] No");
                                    if (GetAnswer())
                                    {
                                        preset.button1Url = n;
                                        SavePreset();
                                        Log(Type.Success, $"Preset Button Url changed to {n}, Press enter to return to the Settings Menu");
                                        var key = Console.ReadKey();
                                        Settings();
                                        break;
                                    }
                                    else
                                    {
                                        Log(Type.Error, "Invalid Response");
                                        Settings();
                                        break;
                                    }
                                }
                                break;
                            case "bn2":
                                {
                                    Log(Type.Info, $"Please set the new Button Name for this preset: ", false, false);
                                    var n = Console.ReadLine();
                                    Log(Type.Info, $"Your new preset Button Name will be {n}, Continue?");
                                    Space();
                                    Log(Type.Empty, "[Y] Yes");
                                    Log(Type.Empty, "[N] No");
                                    if (GetAnswer())
                                    {
                                        preset.button2Name = n;
                                        SavePreset();
                                        Log(Type.Success, $"Preset Button Name changed to {n}, Press enter to return to the Settings Menu");
                                        var key = Console.ReadKey();
                                        Settings();
                                        break;
                                    }
                                    else
                                    {
                                        Log(Type.Error, "Invalid Response");
                                        Settings();
                                        break;
                                    }
                                }
                                break;
                            case "bu2":
                                {
                                    Log(Type.Info, $"Please set the new Button Url for this preset: ", false, false);
                                    var n = Console.ReadLine();
                                    Log(Type.Info, $"Your new preset Button Url will be {n}, Continue?");
                                    Space();
                                    Log(Type.Empty, "[Y] Yes");
                                    Log(Type.Empty, "[N] No");
                                    if (GetAnswer())
                                    {
                                        preset.button2Url = n;
                                        SavePreset();
                                        Log(Type.Success, $"Preset Button Url changed to {n}, Press enter to return to the Settings Menu");
                                        var key = Console.ReadKey();
                                        Settings();
                                        break;
                                    }
                                    else
                                    {
                                        Log(Type.Error, "Invalid Response");
                                        Settings();
                                        break;
                                    }
                                }
                                break;
                            default:
                                {
                                    Log(Type.Error, "Invalid Response, Resetting...");
                                    Settings();
                                }
                                break;
                        }
                    }
                    break;
                case "b":
                    Console.Clear();
                    Main(new string[0]);
                    break;
                default:
                    Log(Type.Error, "Invalid Response, Option does not exist, Resetting...");
                    Settings();
                    break;
            }
        }
        public enum Type
        {
            Info,
            Warn,
            Error,
            Discord,
            Success,
            Empty
        }
        internal class Preset
        {
            public string Name { get; set; }
            public string clientId { get; set; }
            public string state { get; set; }
            public string details { get; set; }
            public string largeImageKey { get; set; }
            public string smallImageKey { get; set; }
            public string largeImageText { get; set; }
            public string smallImageText { get; set; }
            public string button1Name { get; set; }
            public string button1Url { get; set; }
            public string button2Name { get; set; }
            public string button2Url { get; set; }
            public Preset(string name, string clientid, string State, string Details, string LargeImageKey, string SmallImageKey, string LargeImageText, string SmallImageText, string Button1Name, string Button1Url, string Button2Name, string Button2Url)
            {
                Name = name;
                clientId = clientid;
                state = State;
                details = Details;
                largeImageKey = LargeImageKey;
                smallImageKey = SmallImageKey;
                largeImageText = LargeImageText;
                smallImageText = SmallImageText;
                button1Name = Button1Name;
                button1Url = Button1Url;
                button2Name = Button2Name;
                button2Url = Button2Url;
            }
            public Preset()
            {

            }
            public Preset(Preset preset)
            {
                this.clientId = preset.clientId;
                this.state = preset.state;
                this.details = preset.details;
                this.largeImageText = preset.largeImageText;
                this.smallImageText = preset.smallImageText;
                this.largeImageKey = preset.largeImageKey;
                this.smallImageKey = preset.smallImageKey;
                this.button1Name = preset.button1Name;
                this.button1Url = preset.button1Url;
                this.button2Name = preset.button2Name;
                this.button2Url = preset.button2Url;
            }
        }
        public static List<Preset> Presets = new List<Preset>();
        public static void SavePreset()
        {
            using (StreamWriter file = new StreamWriter($"{Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData)}Low/Senko/Presets.json"))
            {
                file.Write(JsonConvert.SerializeObject(Presets, Formatting.Indented));
            }
        }
        public static void ReadPresets()
        {
            if (!Directory.Exists($"{Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData)}Low/Senko"))
                Directory.CreateDirectory($"{Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData)}Low/Senko");
            if (File.Exists($"{Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData)}Low/Senko/Presets.json"))
            {
                string presetstring = File.ReadAllText($"{Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData)}Low/Senko/Presets.json");
                if (!string.IsNullOrEmpty(presetstring))
                {
                    JArray PresetArray = JArray.Parse(presetstring);
                    Log(Type.Info, $"Found {PresetArray.Count} presets");
                    foreach (JObject preset in PresetArray)
                    {
                        Preset getpreset = new Preset();
                        getpreset.Name = preset["Name"].ToObject<string>();
                        getpreset.clientId = preset["clientId"].ToObject<string>();
                        getpreset.state = preset["state"].ToObject<string>();
                        getpreset.details = preset["details"].ToObject<string>();
                        getpreset.largeImageKey = preset["largeImageKey"].ToObject<string>();
                        getpreset.smallImageKey = preset["smallImageKey"].ToObject<string>();
                        getpreset.largeImageText = preset["largeImageText"].ToObject<string>();
                        getpreset.smallImageText = preset["smallImageText"].ToObject<string>();
                        if (preset["button1Name"] == null)
                        {
                            getpreset.button1Name = null;
                            getpreset.button1Url = null;
                            getpreset.button2Name = null;
                            getpreset.button2Url = null;
                        }
                        else
                        {
                            getpreset.button1Name = preset["button1Name"].ToObject<string>();
                            getpreset.button1Url = preset["button1Url"].ToObject<string>();
                            getpreset.button2Name = preset["button2Name"].ToObject<string>();
                            getpreset.button2Url = preset["button2Url"].ToObject<string>();
                        }
                        Presets.Add(getpreset);
                    }
                }
            }
            else
            {
                FileStream file = File.Create($"{Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData)}Low/Senko/Presets.json");
                file.Close();
            }
        }
        public static void Start()
        {
            string largeImageKey = null;
            string smallImageKey = null;
            string largeImageText = "Senko's RPC Maker";
            string smallImageText = "https://github.com/ItsSenko";
            string button1Name = null;
            string button1Url = null;
            string button2Name = null;
            string button2Url = null;
            Log(Type.Info, "Alright Lets get started!");
            if (Presets.Count > 0)
            {
                Log(Type.Info, "Would you like to load a preset?");
                Space();
                Log(Type.Empty, "[Y] Yes");
                Log(Type.Empty, "[N] No");
                if (GetAnswer())
                {
                    Log(Type.Info, "Please Select a Preset");
                    for (int i = 0; i < Presets.Count; i++)
                    {
                        Log(Type.Empty, $"[{i}] {Presets[i].Name}");
                    }
                    var selectedpreset = Console.ReadLine();
                    if (string.IsNullOrEmpty(selectedpreset))
                    {
                        var presett = Presets[0];
                        Log(Type.Success, $"Preset {presett.Name} Loaded!");
                        Log(Type.Success, "All Info is Setup!");
                        Log(Type.Info, "Would you like to Launch the RPC?");
                        Space();
                        Log(Type.Empty, "[Y] Yes");
                        Log(Type.Empty, "[N] No");
                        if (GetAnswer())
                        {
                            Log(Type.Discord, "Launching RPC...");
                            Init(presett.clientId, presett.state, presett.details, presett.largeImageKey, presett.smallImageKey, presett.largeImageText, presett.smallImageText, presett.button1Name, presett.button1Url, presett.button2Name, presett.button2Url);
                            return;
                        }
                    }
                    if (!int.TryParse(selectedpreset, out var presetnum))
                    {
                        Log(Type.Error, "Invalid Response, Could Not Parse, Resetting...");
                        Start();
                        return;
                    }
                    if (Presets[presetnum] == null)
                    {
                        Log(Type.Error, "Invalid Response, Preset does not exist, Resetting...");
                        Start();
                        return;
                    }
                    var preset = Presets[presetnum];
                    Log(Type.Success, $"Preset {preset.Name} Loaded!");
                    Log(Type.Success, "All Info is Setup!");
                    Log(Type.Info, "Would you like to Launch the RPC?");
                    Space();
                    Log(Type.Empty, "[Y] Yes");
                    Log(Type.Empty, "[N] No");
                    if (GetAnswer())
                    {
                        Log(Type.Discord, "Launching RPC...");
                        Init(preset.clientId, preset.state, preset.details, preset.largeImageKey, preset.smallImageKey, preset.largeImageText, preset.smallImageText);
                        return;
                    }
                }
            }
            Log(Type.Info, "Please enter your Client ID: ", false, false);
            var clientID = Console.ReadLine();
            Log(Type.Info, $"Your current ID is set to: {clientID}");
            Log(Type.Info, "Please enter your Rich Presence State: ", false, false);
            var state = Console.ReadLine();
            Log(Type.Info, $"Your current Rich Presence State is set to: {state}");
            Log(Type.Info, "Please enter your Rich Presence Details: ", false, false);
            var details = Console.ReadLine();
            Log(Type.Info, $"Your current Rich Presence Details is set to: {details}");
            Log(Type.Info, "Would you like to set a Large Image?");
            Space();
            Log(Type.Empty, "[Y] Yes");
            Log(Type.Empty, "[N] No");
            if (GetAnswer())
            {
                Log(Type.Info, "Set your Large Image Key: ", false, false);
                largeImageKey = Console.ReadLine();
                Log(Type.Info, $"Your current Large Image Key is set to: {largeImageKey}");
            }
            Log(Type.Info, "Would you like to set a Small Image?");
            Space();
            Log(Type.Empty, "[Y] Yes");
            Log(Type.Empty, "[N] No");
            if (GetAnswer())
            {
                Log(Type.Info, "Set your Small Image Key: ", false, false);
                smallImageKey = Console.ReadLine();
                Log(Type.Info, $"Your current Small Image Key is set to: {smallImageKey}");
            }
            Log(Type.Info, "Would you like to set the Large Image Text?", false, false);
            Space();
            Log(Type.Empty, "[Y] Yes");
            Log(Type.Empty, "[N] No");
            if (GetAnswer())
            {
                Log(Type.Info, "Set your Large Image Text: ", false, false);
                largeImageText = Console.ReadLine();
                Log(Type.Info, $"Your current Large Image Text is set to: {largeImageText}");
            }
            Log(Type.Info, "Would you like to set the Small Image Text?", false, false);
            Space();
            Log(Type.Empty, "[Y] Yes");
            Log(Type.Empty, "[N] No");
            if (GetAnswer())
            {
                Log(Type.Info, "Set your Small Image Text: ", false, false);
                smallImageText = Console.ReadLine();
                Log(Type.Info, $"Your current Small Image Text is set to: {smallImageText}");
            }
            Log(Type.Info, "Would you like to set Button 1?", false, false);
            Space();
            Log(Type.Empty, "[Y] Yes");
            Log(Type.Empty, "[N] No");
            if (GetAnswer())
            {
                Log(Type.Info, "Set your Button Name: ", false, false);
                button1Name = Console.ReadLine();
                Log(Type.Info, $"Your current Button Name is set to: {button1Name}");
                Log(Type.Info, "Set your Button URL: ", false, false);
                button1Url = Console.ReadLine();
                Log(Type.Info, $"Your current Button URL is set to: {button1Url}");
            }
            Log(Type.Info, "Would you like to set Button 2?", false, false);
            Space();
            Log(Type.Empty, "[Y] Yes");
            Log(Type.Empty, "[N] No");
            if (GetAnswer())
            {
                Log(Type.Info, "Set your Button Name: ", false, false);
                button2Name = Console.ReadLine();
                Log(Type.Info, $"Your current Button Name is set to: {button2Name}");
                Log(Type.Info, "Set your Button URL: ", false, false);
                button1Url = Console.ReadLine();
                Log(Type.Info, $"Your current Button URL is set to: {button2Url}");
            }
            Log(Type.Success, "All Info is Setup!");
            Log(Type.Info, "Would you like to save this RPC as a preset?");
            Space();
            Log(Type.Empty, "[Y] Yes");
            Log(Type.Empty, "[N] No");
            if (GetAnswer())
            {
                Log(Type.Info, $"Please enter a name for this preset: ", false, false);
                var name = Console.ReadLine();
                Log(Type.Info, $"Saving preset as {name}");
                Preset preset = new Preset(name, clientID, state, details, largeImageKey, smallImageKey, largeImageText, smallImageText, button1Name, button1Url, button2Name, button2Url);
                Presets.Add(preset);
                SavePreset();
            }
            Log(Type.Info, "Would you like to Launch the RPC?");
            Space();
            Log(Type.Empty, "[Y] Yes");
            Log(Type.Empty, "[N] No");
            if (GetAnswer())
            {
                Log(Type.Discord, "Launching RPC...");
                Init(clientID, state, details, largeImageKey, smallImageKey, largeImageText, smallImageText);
            }

            
        }
        public static void Init(string clientID, string state, string details, string largeImageKey = null, string smallImageKey = null, string largeImageText = "Senko's RPC Maker", string smallImageText = "https://github.com/ItsSenko", string button1Name = null, string button1Url = "https://github.com/ItsSenko", string button2Name = null, string button2Url = "https://github.com/ItsSenko")
        {
            try
            {
                
                var client = new DiscordRpcClient(clientID);
                RichPresence rp = new RichPresence();
                Assets assets = new Assets();
                {
                    if (largeImageKey != null)
                    {
                        assets.LargeImageKey = largeImageKey;
                        assets.LargeImageText = largeImageText;
                    }
                    if (smallImageKey != null)
                    {
                        assets.SmallImageKey = smallImageKey;
                        assets.SmallImageText = smallImageText;
                    }
                }
                rp.Assets = assets;
                Button[] btns = new Button[2];
                if (button1Name != null)
                {
                    Button btn1 = new Button();
                    Uri.TryCreate(button1Url, UriKind.Absolute, out var uri);
                    btn1.Url = uri.AbsoluteUri;
                    btn1.Label = button1Name;
                    btns[0] = btn1;
                }
                if (button2Name != null)
                {
                    Button btn2 = new Button();
                    Uri.TryCreate(button2Url, UriKind.Absolute, out var uri);
                    btn2.Url = uri.AbsoluteUri;
                    btn2.Label = button2Name;
                    btns[1] = btn2;
                }
                if (button1Name != null && button2Name != null)
                { rp.Buttons = btns; }
                rp.State = state;
                rp.Details = details;
                client.SetPresence(rp);
                client.Initialize();
                

                Log(Type.Success, "RPC Launched!");
                Console.ReadLine();
            }
            catch (Exception ex)
            {
                Log(Type.Error, $"Failed Launching RPC: {ex.Message}");
                Console.ReadLine();
            }
        }
    }
    class IconChanger
    {
        public static void SetConsoleIcon(string iconFilePath)
        {
            if (Environment.OSVersion.Platform == PlatformID.Win32NT)
            {
                if (!string.IsNullOrEmpty(iconFilePath))
                {
                    Icon icon = new Icon(iconFilePath);
                    SetWindowIcon(icon);
                }
            }
        }
        public enum WinMessages : uint
        {
            /// <summary>
            /// An application sends the WM_SETICON message to associate a new large or small icon with a window. 
            /// The system displays the large icon in the ALT+TAB dialog box, and the small icon in the window caption. 
            /// </summary>
            SETICON = 0x0080,
        }

        [System.Runtime.InteropServices.DllImport("user32.dll", CharSet = System.Runtime.InteropServices.CharSet.Auto)]
        private static extern IntPtr SendMessage(IntPtr hWnd, int Msg, int wParam, IntPtr lParam);


        private static void SetWindowIcon(System.Drawing.Icon icon)
        {
            IntPtr mwHandle = System.Diagnostics.Process.GetCurrentProcess().MainWindowHandle;
            IntPtr result01 = SendMessage(mwHandle, (int)WinMessages.SETICON, 0, icon.Handle);
            IntPtr result02 = SendMessage(mwHandle, (int)WinMessages.SETICON, 1, icon.Handle);
        }// SetWindowIcon()
    }
}
