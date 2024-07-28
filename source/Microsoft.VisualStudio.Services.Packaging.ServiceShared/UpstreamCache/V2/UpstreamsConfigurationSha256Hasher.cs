// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.UpstreamCache.V2.UpstreamsConfigurationSha256Hasher
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Feed.WebApi;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.DataContracts;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Upstream;
using Microsoft.VisualStudio.Services.Packaging.Shared.WebApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;

namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared.UpstreamCache.V2
{
  public class UpstreamsConfigurationSha256Hasher : IUpstreamsConfigurationHasher
  {
    private HashAlgorithm algo;
    private IPackageUpstreamBehaviorFacade upstreamBehaviorFacade;

    public UpstreamsConfigurationSha256Hasher(
      IPackageUpstreamBehaviorFacade upstreamBehaviorFacade)
    {
      this.algo = (HashAlgorithm) SHA256.Create();
      this.upstreamBehaviorFacade = upstreamBehaviorFacade;
    }

    public string GetHash(UpstreamsConfiguration config, IPackageNameRequest packageNameRequest)
    {
      if (config == null)
        throw new ArgumentNullException(nameof (config));
      if (!config.UpstreamEnabled)
        return (string) null;
      IEnumerable<byte> bytes = config.ProtocolUpstreamSources.SelectMany<UpstreamSource, byte>((Func<UpstreamSource, IEnumerable<byte>>) (x => (IEnumerable<byte>) x.Id.ToByteArray()));
      Microsoft.VisualStudio.Services.Packaging.ServiceShared.Upstream.UpstreamingBehavior behavior = this.upstreamBehaviorFacade.GetBehavior(packageNameRequest.Feed, packageNameRequest.PackageName);
      if (behavior.VersionsFromExternalUpstreams != UpstreamVersionVisibility.Auto)
        bytes = bytes.Concat<byte>((IEnumerable<byte>) new byte[3]
        {
          (byte) 95,
          (byte) 85,
          (byte) behavior.VersionsFromExternalUpstreams
        });
      return HexConverter.ToString(this.algo.ComputeHash(bytes.ToArray<byte>()));
    }

    public static IUpstreamsConfigurationHasher Bootstrap(IVssRequestContext requestContext) => (IUpstreamsConfigurationHasher) new UpstreamsConfigurationSha256Hasher((IPackageUpstreamBehaviorFacade) new PackageUpstreamBehaviorServiceFacade(requestContext));
  }
}
