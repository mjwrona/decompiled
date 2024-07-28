// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Gallery.Server.SoapService.Galleries.ServiceHelper
// Assembly: Microsoft.VisualStudio.Services.Gallery.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B9EBBED5-135E-45CD-B0B4-F747360599CD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Gallery.Server.dll

using System;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Gallery.Server.SoapService.Galleries
{
  internal static class ServiceHelper
  {
    public const string VisualStudioDev11VersionNumber = "11.0";
    public const string VisualStudioDev12VersionNumber = "12.0";
    public const string VisualStudioDev14VersionNumber = "14.0";
    public const string VisualStudioDev15VersionNumber = "15.0";
    public const string VisualStudioDev15VersionNumberLatest = "15.6";
    public const string VisualStudioDev16VersionNumber = "16.0";
    public const string VisualStudioDev17VersionNumber = "17.0";
    public static readonly IDictionary<string, VisualStudioIdeVersion> VisualStudioVersionNumbersMap = (IDictionary<string, VisualStudioIdeVersion>) new Dictionary<string, VisualStudioIdeVersion>()
    {
      {
        "11.0",
        VisualStudioIdeVersion.Dev11
      },
      {
        "12.0",
        VisualStudioIdeVersion.Dev12
      },
      {
        "14.0",
        VisualStudioIdeVersion.Dev14
      },
      {
        "15.0",
        VisualStudioIdeVersion.Dev15
      },
      {
        "15.6",
        VisualStudioIdeVersion.Dev15
      },
      {
        "16.0",
        VisualStudioIdeVersion.Dev16
      },
      {
        "17.0",
        VisualStudioIdeVersion.Dev17
      }
    };

    public static VisualStudioIdeVersion GetVisualStudioIdeVersion(
      string productVersion,
      VisualStudioIdeVersion noMatchDefaultVersion)
    {
      foreach (KeyValuePair<string, VisualStudioIdeVersion> studioVersionNumbers in (IEnumerable<KeyValuePair<string, VisualStudioIdeVersion>>) ServiceHelper.VisualStudioVersionNumbersMap)
      {
        if (productVersion.StartsWith(studioVersionNumbers.Key, StringComparison.OrdinalIgnoreCase))
          return studioVersionNumbers.Value;
      }
      return noMatchDefaultVersion;
    }
  }
}
