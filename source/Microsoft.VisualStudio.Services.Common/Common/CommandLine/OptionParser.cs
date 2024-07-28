// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Common.CommandLine.OptionParser
// Assembly: Microsoft.VisualStudio.Services.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F9C46641-2F1F-44C4-9C2E-4328CD5FFC63
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Common.dll

using Microsoft.VisualStudio.Services.Common.CommandLine.Validation;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Common.CommandLine
{
  public abstract class OptionParser
  {
    public abstract IEnumerable<Argument> Parse(
      IEnumerable<string> commandLine,
      IEnumerable<Option> options);

    public abstract IEnumerable<Argument> Parse(
      IEnumerable<string> commandLine,
      IEnumerable<Option> options,
      IEnumerable<IOptionValidation> optionValidators);

    public static OptionParser CreateParser() => (OptionParser) new BasicParser();

    public static OptionParser CreateParser(OptionReader responseFileRetriever) => (OptionParser) new BasicParser(responseFileRetriever);

    protected static HashSet<string> GetOptionNames(IEnumerable<Option> options)
    {
      HashSet<string> optionNames = new HashSet<string>();
      foreach (Option option in options)
      {
        optionNames.Add(option.Name);
        if (!string.IsNullOrEmpty(option.ShortNameString))
          optionNames.Add(option.ShortNameString);
      }
      return optionNames;
    }

    protected bool IsOptionRun(
      string value,
      IEnumerable<Option> options,
      out Collection<Option> optionRun)
    {
      optionRun = (Collection<Option>) null;
      bool flag = false;
      if (value != null && value.StartsWith("--") && value.Length > "--".Length)
        return false;
      if (value != null && options != null)
      {
        string option = this.ParseOption(value);
        if (option != null)
        {
          char[] charArray = option.ToCharArray();
          if (charArray != null && charArray.Length > 1)
          {
            foreach (char ch in charArray)
            {
              Option byShortName = options.GetByShortName(ch.ToString((IFormatProvider) CultureInfo.InvariantCulture));
              if (byShortName == null)
              {
                flag = false;
                if (optionRun != null)
                {
                  optionRun.Clear();
                  optionRun = (Collection<Option>) null;
                  break;
                }
                break;
              }
              if (optionRun == null)
                optionRun = new Collection<Option>();
              optionRun.Add(byShortName);
              flag = true;
            }
          }
        }
      }
      return flag;
    }

    protected bool IsOption(string value, IEnumerable<Option> options)
    {
      bool flag = false;
      if (value != null && Option.HasSwitch(value))
      {
        string option = this.ParseOption(value);
        if (options.Contains(option) || this.IsOptionRun(value, options, out Collection<Option> _))
          flag = true;
      }
      return flag;
    }

    protected bool IsResponseFileOption(string value)
    {
      bool flag = false;
      if (value != null && value.StartsWith("@", StringComparison.OrdinalIgnoreCase))
        flag = true;
      return flag;
    }

    protected string ParseOption(string value, bool includeSwitch = false)
    {
      string option = (string) null;
      if (value != null)
      {
        IOrderedEnumerable<string> orderedEnumerable = ((IEnumerable<string>) Option.StandardSwitches).OrderByDescending<string, int>((Func<string, int>) (s => s.Length));
        if (!includeSwitch)
        {
          foreach (string str in (IEnumerable<string>) orderedEnumerable)
          {
            if (value.StartsWith(str, StringComparison.OrdinalIgnoreCase))
            {
              option = value.Substring(str.Length);
              break;
            }
          }
        }
        else
          option = value;
        if (option != null)
        {
          int length = option.IndexOfAny(Option.StandardArgumentDelimiters);
          if (length >= 0)
            option = option.Substring(0, length);
        }
      }
      return option;
    }

    protected string ParseOptionArgument(string value)
    {
      string optionArgument = (string) null;
      if (value != null)
      {
        int num = value.IndexOfAny(Option.StandardArgumentDelimiters);
        if (num != -1)
          optionArgument = value.Substring(num + 1);
      }
      return optionArgument;
    }
  }
}
