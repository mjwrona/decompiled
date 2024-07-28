// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Common.CommandLine.ExtensionsMethods
// Assembly: Microsoft.VisualStudio.Services.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F9C46641-2F1F-44C4-9C2E-4328CD5FFC63
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Common.dll

using Microsoft.VisualStudio.Services.Common.CommandLine.Validation;
using Microsoft.VisualStudio.Services.Common.Internal;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Text;

namespace Microsoft.VisualStudio.Services.Common.CommandLine
{
  public static class ExtensionsMethods
  {
    public static bool Contains(this IEnumerable<Option> options, params string[] optionNames)
    {
      bool flag = false;
      if (options != null && optionNames != null)
      {
        flag = true;
        foreach (string optionName in optionNames)
        {
          if (options.Get(optionName) == null)
          {
            flag = false;
            break;
          }
        }
      }
      return flag;
    }

    public static bool Contains(this IEnumerable<Argument> arguments, params string[] optionNames)
    {
      bool flag = false;
      if (arguments != null && optionNames != null)
      {
        flag = true;
        foreach (string optionName in optionNames)
        {
          if (arguments.Get(optionName) == null)
          {
            flag = false;
            break;
          }
        }
      }
      return flag;
    }

    public static bool Contains(this IEnumerable<Argument> arguments, params Option[] options)
    {
      bool flag = false;
      if (arguments != null && options != null)
      {
        flag = true;
        foreach (Option option in options)
        {
          if (arguments.Get(option) == null)
          {
            flag = false;
            break;
          }
        }
      }
      return flag;
    }

    public static bool ContainsAny(
      this IEnumerable<Argument> arguments,
      params string[] optionNames)
    {
      bool flag = false;
      if (arguments != null && optionNames != null)
      {
        foreach (string optionName in optionNames)
        {
          if (arguments.Get(optionName) != null)
          {
            flag = true;
            break;
          }
        }
      }
      return flag;
    }

    public static bool ContainsAny(this IEnumerable<Argument> arguments, params Option[] options)
    {
      bool flag = false;
      if (arguments != null && options != null)
      {
        foreach (Option option in options)
        {
          if (arguments.Get(option) != null)
          {
            flag = true;
            break;
          }
        }
      }
      return flag;
    }

    public static bool ContainsHelp(this IEnumerable<Argument> arguments)
    {
      bool flag = false;
      if (arguments != null)
      {
        foreach (Argument obj in arguments)
        {
          if (obj != null && obj.Option != null && obj.Option.IsHelp)
          {
            flag = true;
            break;
          }
        }
      }
      return flag;
    }

    public static Option Get(this IEnumerable<Option> options, string optionName)
    {
      Option option = (Option) null;
      if (options != null && optionName != null)
        option = options.FirstOrDefault<Option>((Func<Option, bool>) (opt =>
        {
          if (string.Equals(opt.Name, optionName, opt.CaseSensitivity))
            return true;
          return opt.HasShortName && string.Equals(opt.ShortNameString, optionName, opt.CaseSensitivity);
        }));
      return option;
    }

    public static Option GetByShortName(this IEnumerable<Option> options, string shortName)
    {
      Option byShortName = (Option) null;
      if (options != null && shortName != null)
        byShortName = options.FirstOrDefault<Option>((Func<Option, bool>) (opt => opt.HasShortName && string.Equals(opt.ShortNameString, shortName, opt.CaseSensitivity)));
      return byShortName;
    }

    public static Option GetByShortName(this IEnumerable<Option> options, char shortName) => options.GetByShortName(shortName.ToString());

    public static Argument Get(this IEnumerable<Argument> arguments, string optionName)
    {
      Argument obj = (Argument) null;
      if (arguments != null && optionName != null)
        obj = arguments.FirstOrDefault<Argument>((Func<Argument, bool>) (arg =>
        {
          if (arg.Option == null)
            return false;
          if (string.Equals(arg.Option.Name, optionName, arg.Option.CaseSensitivity))
            return true;
          return arg.Option.HasShortName && string.Equals(arg.Option.ShortNameString, optionName, arg.Option.CaseSensitivity);
        }));
      return obj;
    }

    public static Argument Get(this IEnumerable<Argument> arguments, Option option)
    {
      Argument obj = (Argument) null;
      if (option != null)
        obj = arguments.Get(option.Name);
      return obj;
    }

    public static IEnumerable<Argument> GetAll(
      this IEnumerable<Argument> arguments,
      string optionName)
    {
      List<Argument> all = (List<Argument>) null;
      if (arguments != null && optionName != null)
      {
        IEnumerable<Argument> collection = arguments.Where<Argument>((Func<Argument, bool>) (arg =>
        {
          if (arg.Option == null)
            return false;
          if (string.Equals(arg.Option.Name, optionName, arg.Option.CaseSensitivity))
            return true;
          return arg.Option.HasShortName && string.Equals(arg.Option.ShortNameString, optionName, arg.Option.CaseSensitivity);
        }));
        if (collection != null)
          all = new List<Argument>(collection);
      }
      return (IEnumerable<Argument>) all;
    }

    public static IEnumerable<Argument> GetAll(this IEnumerable<Argument> arguments, Option option)
    {
      IEnumerable<Argument> all = (IEnumerable<Argument>) null;
      if (option != null)
        all = arguments.GetAll(option.Name);
      return all;
    }

    public static IEnumerable<Argument> GetPositionalArguments(this IEnumerable<Argument> arguments)
    {
      IEnumerable<Argument> positionalArguments = (IEnumerable<Argument>) null;
      if (arguments != null && arguments.Any<Argument>())
        positionalArguments = arguments.Where<Argument>((Func<Argument, bool>) (arg => (arg.Metadata & OptionMetadata.Positional) == OptionMetadata.Positional));
      return positionalArguments;
    }

    public static string GetUsage(
      this IEnumerable<Option> options,
      string applicationName = null,
      Version applicationVersion = null)
    {
      StringBuilder stringBuilder = new StringBuilder();
      stringBuilder.Append(ExtensionsMethods.GetUsageHeader(options, applicationName, applicationVersion));
      stringBuilder.AppendLine();
      stringBuilder.Append(ExtensionsMethods.GetUsageBody(options));
      return stringBuilder.ToString();
    }

    public static void Validate(this IEnumerable<Option> options, IEnumerable<Argument> arguments) => options.Validate(arguments, (IEnumerable<IOptionValidation>) new Collection<IOptionValidation>()
    {
      (IOptionValidation) OptionValidation.Default
    });

    public static void Validate(
      this IEnumerable<Option> options,
      IEnumerable<Argument> arguments,
      IEnumerable<IOptionValidation> dependencies)
    {
      if (options == null || dependencies == null)
        return;
      foreach (IOptionValidation dependency in dependencies)
        dependency?.Validate(options, arguments);
    }

    public static object ValueOf(this IEnumerable<Argument> arguments, string optionName)
    {
      if (arguments == null)
        throw new ArgumentNullException(nameof (arguments));
      return ((optionName != null ? arguments.Get(optionName) : throw new ArgumentNullException(nameof (optionName))) ?? throw new OptionNotFoundException(CommonResources.ErrorOptionNotFound((object) optionName))).Value;
    }

    public static object ValueOf(this IEnumerable<Argument> arguments, Option option)
    {
      if (option == null)
        throw new ArgumentNullException(nameof (option));
      return arguments.ValueOf(option.Name);
    }

    [CLSCompliant(false)]
    public static T ValueOf<T>(this IEnumerable<Argument> arguments, string optionName) where T : IConvertible
    {
      T obj1 = default (T);
      object obj2 = arguments.ValueOf(optionName);
      if (obj2 != null)
        obj1 = (T) Convert.ChangeType(obj2, typeof (T), (IFormatProvider) CultureInfo.CurrentCulture);
      return obj1;
    }

    [CLSCompliant(false)]
    public static T ValueOf<T>(this IEnumerable<Argument> arguments, Option option) where T : IConvertible
    {
      if (option == null)
        throw new ArgumentNullException(nameof (option));
      return arguments.ValueOf<T>(option.Name);
    }

    public static IEnumerable<object> ValueOfAll(
      this IEnumerable<Argument> arguments,
      string optionName)
    {
      if (arguments == null)
        throw new ArgumentNullException(nameof (arguments));
      IEnumerable<Argument> source = optionName != null ? arguments.GetAll(optionName) : throw new ArgumentNullException(nameof (optionName));
      if (source == null || !source.Any<Argument>())
        throw new OptionNotFoundException(CommonResources.ErrorOptionNotFound((object) optionName));
      List<object> objectList = new List<object>();
      foreach (Argument obj in source)
        objectList.Add(obj.Value);
      return (IEnumerable<object>) objectList;
    }

    public static IEnumerable<object> ValueOfAll(
      this IEnumerable<Argument> arguments,
      Option option)
    {
      if (option == null)
        throw new ArgumentNullException(nameof (option));
      return arguments.ValueOfAll(option.Name);
    }

    [CLSCompliant(false)]
    public static IEnumerable<T> ValueOfAll<T>(
      this IEnumerable<Argument> arguments,
      string optionName)
      where T : IConvertible
    {
      Collection<T> collection = (Collection<T>) null;
      IEnumerable<object> objects = arguments.ValueOfAll(optionName);
      if (objects != null)
      {
        collection = new Collection<T>();
        foreach (object obj in objects)
          collection.Add((T) Convert.ChangeType(obj, typeof (T), (IFormatProvider) CultureInfo.CurrentCulture));
      }
      return (IEnumerable<T>) collection;
    }

    [CLSCompliant(false)]
    public static IEnumerable<T> ValueOfAll<T>(this IEnumerable<Argument> arguments, Option option) where T : IConvertible
    {
      if (option == null)
        throw new ArgumentNullException(nameof (option));
      return arguments.ValueOfAll<T>(option.Name);
    }

    private static string GetSeparatorLine(int length)
    {
      StringBuilder stringBuilder = new StringBuilder();
      for (int index = 0; index < length; ++index)
        stringBuilder.Append("-");
      return stringBuilder.ToString();
    }

    private static string GetUsageHeader(
      IEnumerable<Option> options,
      string applicationName = null,
      Version applicationVersion = null)
    {
      StringBuilder stringBuilder = new StringBuilder();
      if (applicationName != null)
      {
        string separatorLine = ExtensionsMethods.GetSeparatorLine(50);
        stringBuilder.AppendFormat((IFormatProvider) CultureInfo.CurrentCulture, " {0}", (object) separatorLine);
        stringBuilder.AppendLine();
        if (applicationVersion == (Version) null)
          stringBuilder.Append(applicationName);
        else
          stringBuilder.AppendFormat((IFormatProvider) CultureInfo.CurrentCulture, " {0}  v{1}", (object) applicationName, (object) applicationVersion);
        stringBuilder.AppendLine();
        stringBuilder.Append(" " + separatorLine);
        stringBuilder.AppendLine();
        stringBuilder.AppendLine();
      }
      stringBuilder.Append("  Usage:");
      stringBuilder.AppendLine();
      stringBuilder.AppendLine();
      stringBuilder.AppendFormat((IFormatProvider) CultureInfo.CurrentCulture, "  {0}", (object) applicationName);
      stringBuilder.Append("  ");
      foreach (Option option in options)
      {
        if (option.OptionType == OptionType.Required)
        {
          if (!option.HasShortName)
            stringBuilder.AppendFormat((IFormatProvider) CultureInfo.CurrentCulture, "-{0}", (object) option.Name);
          else
            stringBuilder.AppendFormat((IFormatProvider) CultureInfo.CurrentCulture, "(-{0}|{1})", (object) option.ShortNameString, (object) option.Name);
          if (option.ArgumentType == OptionArgumentType.Required)
            stringBuilder.Append(" <value>");
        }
        else
        {
          if (!option.HasShortName)
            stringBuilder.AppendFormat((IFormatProvider) CultureInfo.CurrentCulture, "[-{0}", (object) option.Name);
          else
            stringBuilder.AppendFormat((IFormatProvider) CultureInfo.CurrentCulture, "[(-{0}|{1})", (object) option.ShortNameString, (object) option.Name);
          if (option.ArgumentType == OptionArgumentType.Required)
            stringBuilder.Append(" <value>");
          stringBuilder.Append("]");
        }
        stringBuilder.Append(" ");
      }
      return stringBuilder.ToString();
    }

    private static string GetUsageBody(IEnumerable<Option> options)
    {
      StringBuilder stringBuilder1 = new StringBuilder();
      int num = 10;
      Collection<string> collection = new Collection<string>();
      try
      {
        StringBuilder stringBuilder2 = new StringBuilder();
        foreach (Option option in options)
        {
          stringBuilder2.Clear();
          stringBuilder2.Append("/");
          stringBuilder2.Append(option.Name);
          if (stringBuilder2.Length > num)
            num = stringBuilder2.Length;
          collection.Add(stringBuilder2.ToString());
        }
        string format = string.Format((IFormatProvider) CultureInfo.CurrentCulture, "{{0,-{0}}}", (object) (num + 2));
        for (int index = 0; index < options.Count<Option>(); ++index)
        {
          if (index != 0)
            stringBuilder1.AppendLine();
          Option option = options.ElementAt<Option>(index);
          stringBuilder1.AppendLine();
          stringBuilder1.Append("  ");
          string str1 = string.Format((IFormatProvider) CultureInfo.CurrentCulture, format, (object) collection[index]);
          stringBuilder1.Append(str1);
          string str2 = string.Format((IFormatProvider) CultureInfo.CurrentCulture, "- {0}", (object) option.Description);
          if (option.DefaultValue != null)
            str2 = string.Format((IFormatProvider) CultureInfo.CurrentCulture, "{0}.  (Default value = {1})", (object) str2, option.DefaultValue);
          stringBuilder1.Append(str2);
        }
      }
      finally
      {
        collection.Clear();
      }
      return stringBuilder1.ToString();
    }
  }
}
