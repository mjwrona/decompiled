// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Telemetry.TelemetrySessionCommandLineExtensions
// Assembly: Microsoft.VisualStudio.Telemetry, Version=16.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0E58FD5B-7E43-40D6-A963-E92D0F67BACC
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Telemetry.dll

using Microsoft.VisualStudio.Utilities.Internal;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.VisualStudio.Telemetry
{
  public static class TelemetrySessionCommandLineExtensions
  {
    internal const string CommandLineFlagsEventName = "vs/telemetryapi/commandlineflags";
    internal const string CommandLineFlagsPropertiesPrefix = "VS.TelemetryApi.CommandLineFlags.";
    internal const string CommandLineFlagsArgumentsCountProperty = "VS.TelemetryApi.CommandLineFlags.ArgsCount";
    internal const string CommandLineFlagsFlagsCountProperty = "VS.TelemetryApi.CommandLineFlags.FlagsCount";
    internal const string CommandLineFlagsFirstFlagProperty = "VS.TelemetryApi.CommandLineFlags.FirstFlag";
    internal const string CommandLineFlagsFlagsProperty = "VS.TelemetryApi.CommandLineFlags.Flags";

    public static void PostCommandLineFlags(
      this TelemetrySession session,
      params string[] flagPrefixes)
    {
      session.PostCommandLineFlags((IEnumerable<string>) flagPrefixes, (IDictionary<string, object>) null);
    }

    public static void PostCommandLineFlags(
      this TelemetrySession session,
      IEnumerable<string> flagPrefixes,
      IDictionary<string, object> additionalProperties)
    {
      session.RequiresArgumentNotNull<TelemetrySession>(nameof (session));
      TelemetrySessionCommandLineExtensions.CommandLineFlagsInfo commandLineFlags = TelemetrySessionCommandLineExtensions.ComputeCommandLineFlags(flagPrefixes);
      TelemetryEvent telemetryEvent = new TelemetryEvent("vs/telemetryapi/commandlineflags");
      telemetryEvent.Properties["VS.TelemetryApi.CommandLineFlags.ArgsCount"] = (object) commandLineFlags.ArgsCount;
      telemetryEvent.Properties["VS.TelemetryApi.CommandLineFlags.FlagsCount"] = (object) commandLineFlags.Flags.Length;
      telemetryEvent.Properties["VS.TelemetryApi.CommandLineFlags.FirstFlag"] = commandLineFlags.Flags.Length != 0 ? (object) commandLineFlags.Flags[0] : (object) string.Empty;
      telemetryEvent.Properties["VS.TelemetryApi.CommandLineFlags.Flags"] = (object) new TelemetryComplexProperty((object) commandLineFlags.Flags);
      if (additionalProperties != null)
      {
        foreach (KeyValuePair<string, object> additionalProperty in (IEnumerable<KeyValuePair<string, object>>) additionalProperties)
          telemetryEvent.Properties[additionalProperty.Key] = additionalProperty.Value;
      }
      session.PostEvent(telemetryEvent);
    }

    internal static Func<string[]> GetCommandLineArgsFunc { get; set; } = new Func<string[]>(Environment.GetCommandLineArgs);

    private static TelemetrySessionCommandLineExtensions.CommandLineFlagsInfo ComputeCommandLineFlags(
      IEnumerable<string> flagPrefixes)
    {
      flagPrefixes.RequiresArgumentNotNull<IEnumerable<string>>(nameof (flagPrefixes));
      string[] prefixes = flagPrefixes.Where<string>((Func<string, bool>) (fp => !string.IsNullOrWhiteSpace(fp))).Select<string, string>((Func<string, string>) (fp => fp.Trim())).ToArray<string>();
      if (prefixes.Length == 0)
        throw new ArgumentException("All flag prefixes are invalid (null, empty or white space).", nameof (flagPrefixes));
      string[] array1 = ((IEnumerable<string>) TelemetrySessionCommandLineExtensions.GetCommandLineArgs()).Skip<string>(1).ToArray<string>();
      string[] array2 = ((IEnumerable<string>) array1).Select<string, Tuple<bool, string>>((Func<string, Tuple<bool, string>>) (arg => arg.TryGetValidFlag(prefixes))).Where<Tuple<bool, string>>((Func<Tuple<bool, string>, bool>) (r => r.Item1)).Select<Tuple<bool, string>, string>((Func<Tuple<bool, string>, string>) (r => r.Item2)).ToArray<string>();
      return new TelemetrySessionCommandLineExtensions.CommandLineFlagsInfo()
      {
        ArgsCount = array1.Length,
        Flags = array2
      };
    }

    private static string[] GetCommandLineArgs()
    {
      string[] commandLineArgs = (string[]) null;
      try
      {
        commandLineArgs = TelemetrySessionCommandLineExtensions.GetCommandLineArgsFunc();
      }
      catch
      {
      }
      if (commandLineArgs == null)
        commandLineArgs = new string[0];
      return commandLineArgs;
    }

    private static Tuple<bool, string> TryGetValidFlag(this string argument, string[] flagPrefixes)
    {
      Tuple<bool, string> validFlag = Tuple.Create<bool, string>(false, string.Empty);
      if (string.IsNullOrWhiteSpace(argument))
        return validFlag;
      argument = argument.Trim();
      foreach (string flagPrefix in flagPrefixes)
      {
        if (argument.StartsWith(flagPrefix, StringComparison.OrdinalIgnoreCase))
        {
          argument = argument.Substring(flagPrefix.Length);
          argument = ((IEnumerable<string>) argument.Split(':', ' ')).FirstOrDefault<string>();
          validFlag = Tuple.Create<bool, string>(true, argument);
          break;
        }
      }
      return validFlag;
    }

    private struct CommandLineFlagsInfo
    {
      public int ArgsCount;
      public string[] Flags;
    }
  }
}
