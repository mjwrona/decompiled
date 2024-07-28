// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Npm.Server.Registry.UpstreamProvider
// Assembly: Microsoft.VisualStudio.Services.Npm.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 2F4F0262-1C1B-42F0-BCA7-1385424A0D51
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Npm.Server.dll

using Microsoft.VisualStudio.Services.Feed.WebApi;
using Microsoft.VisualStudio.Services.Npm.Server.Exceptions;
using System;

namespace Microsoft.VisualStudio.Services.Npm.Server.Registry
{
  public class UpstreamProvider
  {
    private const string DistTagPath = "/-/package/{0}/dist-tags";

    public UpstreamProvider(UpstreamSource source)
    {
      if (!"npm".Equals(source.Protocol, StringComparison.OrdinalIgnoreCase))
        throw new InvalidUpstreamSourceProtocolException(Microsoft.VisualStudio.Services.Npm.Server.Resources.Error_UpstreamSourceDoesNotSupportNpm());
      this.UpstreamRegistryUri = source.UpstreamSourceType.Equals((object) UpstreamSourceType.Public) ? new Uri(source.Location) : throw new Microsoft.VisualStudio.Services.Npm.Server.Exceptions.InvalidUpstreamSourceException(Microsoft.VisualStudio.Services.Npm.Server.Resources.Error_UpstreamSourceTypeNotSupported((object) source.UpstreamSourceType));
      this.UpstreamDistTagUrl = new Uri(this.UpstreamRegistryUri, "/-/package/{0}/dist-tags").ToString();
    }

    public Uri UpstreamRegistryUri { get; private set; }

    public string UpstreamDistTagUrl { get; private set; }
  }
}
