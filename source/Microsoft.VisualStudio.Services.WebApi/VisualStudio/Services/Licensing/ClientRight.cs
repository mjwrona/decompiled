// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Licensing.ClientRight
// Assembly: Microsoft.VisualStudio.Services.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7B264323-C592-4F23-AB6B-55AEDC85864F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.WebApi.dll

using System;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Licensing
{
  public class ClientRight : IClientRight, IUsageRight
  {
    public Dictionary<string, object> Attributes { get; set; }

    public string AuthorizedVSEdition { get; set; }

    public Version ClientVersion { get; set; }

    public DateTimeOffset ExpirationDate { get; set; }

    public string LicenseDescriptionId { get; set; }

    public string LicenseFallbackDescription { get; set; }

    public string LicenseUrl { get; set; }

    public string LicenseSourceName { get; set; }

    public string Name { get; set; }

    public Version Version { get; set; }
  }
}
