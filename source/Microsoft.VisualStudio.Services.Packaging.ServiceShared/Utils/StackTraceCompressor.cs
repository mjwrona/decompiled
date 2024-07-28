// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.Utils.StackTraceCompressor
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;


#nullable enable
namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared.Utils
{
  public static class StackTraceCompressor
  {
    private static readonly IList<Tuple<Regex, string>> StackTraceRegexReplacements = (IList<Tuple<Regex, string>>) new List<Tuple<Regex, string>>()
    {
      Tuple.Create<Regex, string>(new Regex("\\r"), string.Empty),
      Tuple.Create<Regex, string>(new Regex("--- End of stack trace from previous location where exception was thrown ---\\n[ ]+at System.Runtime.ExceptionServices.ExceptionDispatchInfo.Throw\\(\\)\\n[ ]+at System.Runtime.CompilerServices.TaskAwaiter.HandleNonSuccessAndDebuggerNotification\\(Task task\\)\\n[ ]+at "), "   at (async a) "),
      Tuple.Create<Regex, string>(new Regex("--- End of stack trace from previous location where exception was thrown ---\\n[ ]+at System.Runtime.ExceptionServices.ExceptionDispatchInfo.Throw\\(\\)\\n[ ]+at "), "   at (async b) ")
    };

    public static string ToStringWithCompressedStackTrace(this Exception exception) => StackTraceCompressor.CompressStackTrace(exception.ToString());

    public static string CompressStackTrace(string stackTrace) => StackTraceCompressor.RemoveFilePaths(StackTraceCompressor.ShortenNamespaces(StackTraceCompressor.RemoveAsyncJunk(stackTrace)));

    private static string RemoveAsyncJunk(string stackTrace)
    {
      foreach (Tuple<Regex, string> regexReplacement in (IEnumerable<Tuple<Regex, string>>) StackTraceCompressor.StackTraceRegexReplacements)
        stackTrace = regexReplacement.Item1.Replace(stackTrace, regexReplacement.Item2);
      return stackTrace;
    }

    private static string ShortenNamespaces(string stackTrace) => stackTrace.Replace("Microsoft.VisualStudio.Services.Packaging.ServiceShared", "$MVSSPSS").Replace("Microsoft.VisualStudio.Services.NuGet.Server", "$MVSSNGS").Replace("Microsoft.VisualStudio.Services", "$MVSS").Replace("Microsoft.TeamFoundation.Framework", "$MTFF");

    private static string RemoveFilePaths(string stackTrace) => Regex.Replace(stackTrace, " in (?:.+[\\\\/])?(?<filename>[^:\\\\/]+):line (?<line>\\d+)", " in ${filename}:line ${line}");
  }
}
