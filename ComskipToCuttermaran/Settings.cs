using System;
using System.IO;
using System.Collections.Generic;
using System.Drawing;
using ComskipToCuttermaran.Statistics;

namespace ComskipToCuttermaran
{
    internal class Settings
    {
        private static object _lock = new object();
        private static SettingsData _current = new SettingsData();
        private static bool _currentLoaded = false;
        public static SettingsData Current
        {
            get
            {
                if (!_currentLoaded)
                {
                    lock (_lock)
                    {
                        if (!_currentLoaded)
                        {
                            if (File.Exists(Path))
                            {
                                var x = new Utils.XMLSerializer();
                                try
                                {
                                    _current = x.ReadObjectFromFile(Path) as SettingsData;
                                }
                                catch (Exception)
                                {
                                }
                            }
                            _currentLoaded = true;
                        }
                    }
                }
                return _current;
            }
        }

        public static void Save()
        {
            lock (_lock)
            {
                var x = new Utils.XMLSerializer();
                x.WriteObjectToFile(Current, Path);
            }
        }

        private static string Path
        {
            get
            {
                var path = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
                path += @"\SqlUpdate\settings.xml";
                return path;
            }
        }

    }

    public class SettingsData
    {
        public string Username { get; set; }

        public Utils.DiffPrograms.DiffProgram DiffProgram { get; set; }

        public bool MinimiseToTray { get; set; }

        #region SavedWindows
        public class SavedWindowSettings
        {
            public string Name;
            public Point Location;
            public Size Size;
            public bool Maximised;
            public bool RememberLocation = false;
            public bool RememberSize = false;

            public Dictionary<string, int> ExtraPosition = new Dictionary<string, int>();
        }
        public class SavedWindowSettingsList : List<SavedWindowSettings>
        {
            public SavedWindowSettings this[string windowName]
            {
                get
                {
                    foreach (var settings in this)
                    {
                        if (settings.Name == windowName)
                            return settings;
                    }
                    var newSettings = new SavedWindowSettings();
                    newSettings.Name = windowName;
                    this.Add(newSettings);
                    return newSettings;
                }
            }
        }
        public SavedWindowSettingsList SavedWindows { get; set; }
        #endregion

        public List<string> RecentFiles { get; set; }

        public Settings_info DetectionSettings { get; set; }

        public SettingsData()
        {
            Username = "";
            MinimiseToTray = false;
            DiffProgram = Utils.DiffPrograms.DiffProgram.AutoDetect;
            SavedWindows = new SavedWindowSettingsList();
            RecentFiles = new List<string>();
            DetectionSettings = new Settings_info();
        }

    }
}
