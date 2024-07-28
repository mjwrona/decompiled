// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Npm.Server.Registry.NpmUpstreamRegistryResponse
// Assembly: Microsoft.VisualStudio.Services.Npm.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 2F4F0262-1C1B-42F0-BCA7-1385424A0D51
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Npm.Server.dll

using Microsoft.VisualStudio.Services.Feed.WebApi;
using System;

namespace Microsoft.VisualStudio.Services.Npm.Server.Registry
{
  public class NpmUpstreamRegistryResponse
  {
    public NpmUpstreamRegistryResponse(UpstreamSource upstream, string json)
    {
      this.Upstream = upstream;
      this.Json = json;
    }

    public NpmUpstreamRegistryResponse(UpstreamSource upstream, Exception exception)
    {
      this.Upstream = upstream;
      this.Exception = exception;
    }

    public UpstreamSource Upstream { get; }

    public string Json { get; }

    public Exception Exception { get; set; }

    public bool IsCriticalFailure => this.Exception != null && !(this.Exception is Microsoft.VisualStudio.Services.Packaging.Shared.WebApi.Exceptions.PackageNotFoundException);
  }
}
