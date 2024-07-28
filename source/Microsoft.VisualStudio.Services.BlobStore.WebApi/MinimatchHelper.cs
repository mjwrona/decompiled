// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.BlobStore.WebApi.MinimatchHelper
// Assembly: Microsoft.VisualStudio.Services.BlobStore.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4500AC57-FBCC-4F18-B11F-F661A75E4A46
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.BlobStore.WebApi.dll

using Microsoft.VisualStudio.Services.BlobStore.Common;
using Microsoft.VisualStudio.Services.Content.Common.Tracing;
using Minimatch;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.VisualStudio.Services.BlobStore.WebApi
{
  public static class MinimatchHelper
  {
    private static readonly bool isWindows = Helpers.IsWindowsPlatform(Environment.OSVersion);
    private static readonly Options defaultMinimatchOptions = new Options()
    {
      Dot = true,
      NoBrace = true,
      NoCase = MinimatchHelper.isWindows,
      AllowWindowsPaths = MinimatchHelper.isWindows
    };

    public static IEnumerable<Func<string, bool>> GetMinimatchFuncs(
      IEnumerable<string> minimatchPatterns,
      IAppTraceSource tracer,
      Options customMinimatchOptions = null)
    {
      Options options = MinimatchHelper.defaultMinimatchOptions;
      if (customMinimatchOptions != null)
        options = customMinimatchOptions;
      IEnumerable<Func<string, bool>> minimatchFuncs;
      if (minimatchPatterns != null && minimatchPatterns.Count<string>() != 0)
      {
        string format = "Minimatch patterns: [" + string.Join(",", minimatchPatterns) + "]";
        tracer.Info(format);
        minimatchFuncs = minimatchPatterns.Where<string>((Func<string, bool>) (pattern => !string.IsNullOrEmpty(pattern))).Select<string, Func<string, bool>>((Func<string, Func<string, bool>>) (pattern => Minimatcher.CreateFilter(pattern, options)));
      }
      else
        minimatchFuncs = (IEnumerable<Func<string, bool>>) null;
      return minimatchFuncs;
    }
  }
}
