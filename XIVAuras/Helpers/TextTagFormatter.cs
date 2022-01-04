using System.Reflection;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System;
using System.Globalization;

namespace XIVAuras.Helpers
{
    public class TextTagFormatter
    {
        public static Regex TextTagRegex { get; } = new Regex(@"\[(\w*)(:k)?\.?(\d+)?\]", RegexOptions.Compiled);

        private string _format;
        private Dictionary<string, FieldInfo> _fields;
        private object _source;

        public TextTagFormatter(
            object source,
            string format,
            Dictionary<string, FieldInfo> fields)
        {
            _source = source;
            _format = format;
            _fields = fields;
        }

        public string Evaluate(Match m)
        {
            if (m.Groups.Count != 4)
            {
                return m.Value;
            }

            string format = string.IsNullOrEmpty(m.Groups[3].Value)
                ? $"{_format}0"
                : $"{_format}{m.Groups[3].Value}";
            
            string? value = null;
            string key = m.Groups[1].Value;
            
            if (_fields.ContainsKey(key))
            {
                object? propValue = _fields[m.Groups[1].Value].GetValue(_source);

                if (propValue is null)
                {
                    return string.Empty;
                }

                if (propValue is float f)
                {
                    value = !string.IsNullOrEmpty(m.Groups[2].Value)
                        ? KiloFormat(f, format) ?? m.Value
                        : f.ToString(format);
                }
                else
                {
                    value = propValue?.ToString();
                    if (!string.IsNullOrEmpty(value) &&
                        int.TryParse(m.Groups[3].Value, out int trim) &&
                        trim < value.Length)
                    {
                        value = propValue?.ToString().AsSpan(0, trim).ToString();
                    }
                }
            }

            return value ?? m.Value;
        }

        private static string KiloFormat(float num, string format) => num switch
        {
            >= 1000000 => (num / 1000000f).ToString(format, CultureInfo.InvariantCulture) + "M",
            >= 1000 => (num / 1000f).ToString(format, CultureInfo.InvariantCulture) + "K",
            _ => num.ToString(format, CultureInfo.InvariantCulture)
        };
    }
}