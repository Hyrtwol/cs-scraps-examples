using System;
using System.Configuration;

namespace Scraps
{
    public abstract class AppSettings
    {
        protected static string GetValue(string name)
        {
            if (string.IsNullOrEmpty(name)) throw new ArgumentNullException("name");
            return ConfigurationManager.AppSettings[name];
        }

        protected static string GetString(string name, string defaultVal = null)
        {
            var val = GetValue(name);
            if (string.IsNullOrEmpty(val))
            {
                if (!string.IsNullOrEmpty(defaultVal)) return defaultVal;
                throw new ConfigurationErrorsException("Missing '" + name + "' in appsettings.");
            }
            return val;
        }
		
        protected static string GetPath(string name, string defaultVal = null)
        {
            var path = GetString(name, defaultVal);
            if (path.Contains("%"))
            {
                path = Environment.ExpandEnvironmentVariables(path);
            }
            return path;
        }

        protected static TimeSpan GetTimeSpan(string name, TimeSpan defaultVal)
        {
            var val = GetValue(name);
            TimeSpan res;
            if (!string.IsNullOrEmpty(val) && TimeSpan.TryParse(val, out res)) return res;
            return defaultVal;
        }

        protected static int GetInt32(string name, int defaultVal)
        {
            var val = GetValue(name);
            int res;
            if (!string.IsNullOrEmpty(val) && int.TryParse(val, out res)) return res;
            return defaultVal;
        }

        protected static ushort GetUInt16(string name, ushort defaultVal)
        {
            var val = GetValue(name);
            ushort res;
            if (!string.IsNullOrEmpty(val) && ushort.TryParse(val, out res)) return res;
            return defaultVal;
        }

        protected static bool GetBool(string name, bool defaultVal)
        {
            var val = GetValue(name);
            bool res;
            if (!string.IsNullOrEmpty(val) && bool.TryParse(val, out res)) return res;
            return defaultVal;
        }
    }
}