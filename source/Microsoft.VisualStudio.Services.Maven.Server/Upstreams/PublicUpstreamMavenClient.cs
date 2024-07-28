// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Maven.Server.Upstreams.PublicUpstreamMavenClient
// Assembly: Microsoft.VisualStudio.Services.Maven.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3AEBE02E-FDD2-41D8-89F7-5C54445DBFA7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Maven.Server.dll

using Microsoft.VisualStudio.Services.Maven.Server.Implementations;
using Microsoft.VisualStudio.Services.Maven.Server.Implementations.Internal;
using Microsoft.VisualStudio.Services.Maven.Server.Models;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.PackageMetadata;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Upstream;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Utils;
using Microsoft.VisualStudio.Services.Packaging.Shared.WebApi;
using Microsoft.VisualStudio.Services.Packaging.Shared.WebApi.Exceptions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;


#nullable enable
namespace Microsoft.VisualStudio.Services.Maven.Server.Upstreams
{
  public class PublicUpstreamMavenClient : IUpstreamMavenClient
  {
    private readonly 
    #nullable disable
    Uri packageSourceUri;
    private readonly IHttpClient httpClient;
    private readonly IConverter<Stream, IList<string>> upstreamXmlMetadataToVersionListConverter;
    private readonly bool treatHttp401AsNotFound;
    private readonly Action<Uri, HttpStatusCode> errorCodeHandlingOverride;

    private static void Treat401AsNotFound(Uri requestUri, HttpStatusCode statusCode)
    {
      if (statusCode == HttpStatusCode.Unauthorized || statusCode == HttpStatusCode.Forbidden)
        throw new PackageNotFoundException(Microsoft.VisualStudio.Services.Maven.Server.Resources.Error_UpstreamReturnedUnauthorized((object) requestUri.AbsoluteUri));
    }

    public PublicUpstreamMavenClient(
      Uri packageSourceUri,
      IHttpClient httpClient,
      IConverter<Stream, IList<string>> upstreamXmlMetadataToVersionListConverter,
      bool treatHttp401AsNotFound)
    {
      this.packageSourceUri = packageSourceUri;
      this.httpClient = httpClient;
      this.upstreamXmlMetadataToVersionListConverter = upstreamXmlMetadataToVersionListConverter;
      this.treatHttp401AsNotFound = treatHttp401AsNotFound;
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      this.errorCodeHandlingOverride = treatHttp401AsNotFound ? PublicUpstreamMavenClient.\u003C\u003EO.\u003C0\u003E__Treat401AsNotFound ?? (PublicUpstreamMavenClient.\u003C\u003EO.\u003C0\u003E__Treat401AsNotFound = new Action<Uri, HttpStatusCode>(PublicUpstreamMavenClient.Treat401AsNotFound)) : (Action<Uri, HttpStatusCode>) null;
    }

    public async Task<Stream> GetFileAsync(IMavenArtifactFilePath filePath)
    {
      Uri uri = new Uri(this.packageSourceUri, filePath.FullName);
      return await PublicUpstreamHttpClientHelper.GetStreamWithErrorHandlingAsync(this.httpClient, this.packageSourceUri.AbsoluteUri, PackageIdentifierForMessages.From((IPackageIdentity) new MavenPackageIdentity(filePath.PackageName, filePath.PackageVersion), (IPackageFileName) filePath), uri, HttpCompletionOption.ResponseHeadersRead, this.errorCodeHandlingOverride);
    }

    public async Task<IReadOnlyList<VersionWithSourceChain<MavenPackageVersion>>> GetPackageVersionsAsync(
      MavenPackageName packageName)
    {
      PublicUpstreamMavenClient upstreamMavenClient = this;
      MavenArtifactIdLevelMetadataFilePath metadataFilePath = new MavenArtifactIdLevelMetadataFilePath(packageName);
      Uri uri = new Uri(upstreamMavenClient.packageSourceUri, metadataFilePath.FullName);
      // ISSUE: reference to a compiler-generated method
      return (IReadOnlyList<VersionWithSourceChain<MavenPackageVersion>>) await PublicUpstreamHttpClientHelper.GetWithErrorHandlingAsync<List<VersionWithSourceChain<MavenPackageVersion>>>(upstreamMavenClient.httpClient, upstreamMavenClient.packageSourceUri.AbsoluteUri, PackageIdentifierForMessages.From((IPackageName) packageName), uri, HttpCompletionOption.ResponseContentRead, new Func<HttpResponseMessage, Task<List<VersionWithSourceChain<MavenPackageVersion>>>>(upstreamMavenClient.\u003CGetPackageVersionsAsync\u003Eb__8_1), upstreamMavenClient.errorCodeHandlingOverride);
    }

    public Task<IEnumerable<UpstreamSourceInfo>> GetSourceChainAsync(
      IMavenFullyQualifiedFilePath filePath)
    {
      return Task.FromResult<IEnumerable<UpstreamSourceInfo>>(Enumerable.Empty<UpstreamSourceInfo>());
    }
  }
}
