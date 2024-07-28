// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Maven.Server.Upstreams.IUpstreamMavenClient
// Assembly: Microsoft.VisualStudio.Services.Maven.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3AEBE02E-FDD2-41D8-89F7-5C54445DBFA7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Maven.Server.dll

using Microsoft.VisualStudio.Services.Maven.Server.Implementations;
using Microsoft.VisualStudio.Services.Maven.Server.Models;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype;
using Microsoft.VisualStudio.Services.Packaging.Shared.WebApi;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.Maven.Server.Upstreams
{
  public interface IUpstreamMavenClient
  {
    Task<Stream> GetFileAsync(IMavenArtifactFilePath filePath);

    Task<IReadOnlyList<VersionWithSourceChain<MavenPackageVersion>>> GetPackageVersionsAsync(
      MavenPackageName packageName);

    Task<IEnumerable<UpstreamSourceInfo>> GetSourceChainAsync(IMavenFullyQualifiedFilePath filePath);
  }
}
