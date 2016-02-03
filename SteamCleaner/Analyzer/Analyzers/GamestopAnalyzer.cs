﻿#region

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Win32;

#endregion

namespace SteamCleaner.Analyzer.Analyzers
{
    public class GamestopAnalyzer : IAnalyzer
    {
        public string Name => "Gamestop";

        public bool CheckExists()
        {
            using (var rootKey = GetRootKey())
            {
                return rootKey != null;
            }
        }

        public IEnumerable<string> FindPaths()
        {
            using (var rootKey = GetRootKey())
            {
                if (rootKey == null)
                {
                    return null;
                }
                var is64Bit = Environment.Is64BitOperatingSystem;
                var regPath = is64Bit
                    ? @"Software\Wow6432Node\Microsoft\\Windows\CurrentVersion\Uninstall"
                    : @"Software\Microsoft\Windows\CurrentVersion\Uninstall";
                var paths = new List<string>();
                paths.AddRange(from subkeyName in rootKey.GetSubKeyNames()
                    where subkeyName.StartsWith("Gamestop_", StringComparison.Ordinal)
                    select Registry.LocalMachine.OpenSubKey(regPath + "\\" + subkeyName)
                    into subKey
                    select subKey?.GetValue("Install_local").ToString());
                return paths;
            }
        }

        public RegistryKey GetRootKey()
        {
            var is64Bit = Environment.Is64BitOperatingSystem;
            var regPath = is64Bit
                ? @"Software\Wow6432Node\Microsoft\\Windows\CurrentVersion\Uninstall"
                : @"Software\Microsoft\Windows\CurrentVersion\Uninstall";
            var root = Registry.LocalMachine.OpenSubKey(regPath);
            return root;
        }
    }
}