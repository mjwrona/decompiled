// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Npm.WebApi.NpmWebApiExtensions
// Assembly: Microsoft.VisualStudio.Services.Npm.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 639B57A1-1338-429F-9659-38C0A0394E05
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Npm.WebApi.dll

using Microsoft.VisualStudio.Services.Npm.WebApi.Types;
using System;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Npm.WebApi
{
  [Obsolete]
  public static class NpmWebApiExtensions
  {
    [Obsolete]
    public static VersionMetadata VerifyCompliance(this VersionMetadata versionMetadata)
    {
      if (versionMetadata.Binaries != null)
      {
        Dictionary<string, string> dictionary = new Dictionary<string, string>();
        foreach (string key1 in versionMetadata.Binaries.Keys)
        {
          string key2 = string.IsNullOrWhiteSpace(key1) ? versionMetadata.Name : key1;
          string binary = versionMetadata.Binaries[key1];
          dictionary.Add(key2, binary);
        }
        versionMetadata.Binaries = dictionary;
      }
      return versionMetadata;
    }
  }
}
