// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Common.CommandLine.Option
// Assembly: Microsoft.VisualStudio.Services.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F9C46641-2F1F-44C4-9C2E-4328CD5FFC63
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Common.dll

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;

namespace Microsoft.VisualStudio.Services.Common.CommandLine
{
  [DebuggerDisplay("{Name,nq}")]
  [Serializable]
  public class Option
  {
    internal const string DoubleDashSwitch = "--";
    internal const string SingleDashSwitch = "-";
    internal const string SlashSwitch = "/";
    public const OptionArgumentType DefaultArgumentType = OptionArgumentType.Required;
    public const StringComparison DefaultCaseSensitivity = StringComparison.OrdinalIgnoreCase;
    public const OptionType DefaultOptionType = OptionType.Required;
    public const string HelpOptionName = "Help";
    public static readonly string[] StandardSwitches = new string[3]
    {
      "--",
      "-",
      "/"
    };
    public static readonly string[] StandardHelpOptions = new string[3]
    {
      "?",
      "h",
      "help"
    };
    public static readonly char[] StandardArgumentDelimiters = new char[2]
    {
      ':',
      '='
    };
    private static readonly string hasStandardSwitchPattern;

    static Option()
    {
      IOrderedEnumerable<string> values = ((IEnumerable<string>) Option.StandardSwitches).OrderByDescending<string, int>((Func<string, int>) (s => s.Length));
      Option.hasStandardSwitchPattern = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "^({0}){1}[^{2}].*$", (object) string.Join("|", (IEnumerable<string>) values), (object) "{1}", (object) string.Join<char>(string.Empty, ((IEnumerable<char>) string.Join(string.Empty, (IEnumerable<string>) values).ToCharArray()).Distinct<char>()));
    }

    public Option()
    {
      this.AllowMultiple = false;
      this.ArgumentType = OptionArgumentType.Required;
      this.CaseSensitivity = StringComparison.OrdinalIgnoreCase;
      this.Converter = ValueConverter.None;
      this.OptionType = OptionType.Required;
    }

    public Option(string name)
      : this()
    {
      this.Name = name;
    }

    public Option(string name, char shortName)
      : this(name)
    {
      this.ShortName = shortName;
    }

    public bool AllowMultiple { get; set; }

    public OptionArgumentType ArgumentType { get; set; }

    public StringComparison CaseSensitivity { get; set; }

    public IValueConvertible Converter { get; set; }

    public object DefaultValue { get; set; }

    public string Description { get; set; }

    public bool HasShortName
    {
      get
      {
        bool hasShortName = false;
        if (this.ShortName != char.MinValue)
          hasShortName = true;
        return hasShortName;
      }
    }

    public bool IsHelp => Option.IsHelpOption(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}{1}", (object) ((IEnumerable<string>) Option.StandardSwitches).Last<string>(), (object) this.Name));

    public string Name { get; set; }

    public OptionType OptionType { get; set; }

    public char ShortName { get; set; }

    public string ShortNameString => this.HasShortName ? this.ShortName.ToString() : (string) null;

    public static Option CreateHelpOption() => new Option("Help", '?')
    {
      CaseSensitivity = StringComparison.OrdinalIgnoreCase,
      Description = "Display usage/help",
      ArgumentType = OptionArgumentType.None,
      OptionType = OptionType.Optional,
      Converter = ValueConverter.None
    };

    public static bool HasHelpOption(IEnumerable<string> args)
    {
      bool flag = false;
      if (args != null)
      {
        foreach (string option in args)
        {
          if (Option.IsHelpOption(option))
          {
            flag = true;
            break;
          }
        }
      }
      return flag;
    }

    public static bool HasOption(IEnumerable<string> args, string option) => Option.HasOption(args, option, StringComparison.OrdinalIgnoreCase);

    public static bool HasOption(
      IEnumerable<string> args,
      string option,
      StringComparison comparisonType)
    {
      bool flag = false;
      if (option != null && args != null)
      {
        string str1 = Option.RemoveSwitch(option);
        foreach (string option1 in args)
        {
          string str2 = Option.RemoveSwitch(option1);
          if (str1.Equals(str2, comparisonType))
          {
            flag = true;
            break;
          }
        }
      }
      return flag;
    }

    public static bool HasSwitch(string value)
    {
      bool flag = false;
      if (!string.IsNullOrEmpty(value) && Regex.IsMatch(value, Option.hasStandardSwitchPattern, RegexOptions.IgnoreCase))
        flag = true;
      return flag;
    }

    public static bool IsHelpOption(string option)
    {
      bool flag = false;
      if (option != null && Option.HasSwitch(option))
      {
        string str = Option.RemoveSwitch(option);
        foreach (string standardHelpOption in Option.StandardHelpOptions)
        {
          if (str.Equals(standardHelpOption, StringComparison.OrdinalIgnoreCase))
          {
            flag = true;
            break;
          }
        }
      }
      return flag;
    }

    public static string RemoveSwitch(string option)
    {
      string str = (string) null;
      if (option != null)
      {
        string pattern = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "^({0}){1}", (object) string.Join("|", (IEnumerable<string>) ((IEnumerable<string>) Option.StandardSwitches).OrderByDescending<string, int>((Func<string, int>) (s => s.Length))), (object) "{1}");
        str = Regex.Replace(option, pattern, string.Empty);
      }
      return str;
    }
  }
}
