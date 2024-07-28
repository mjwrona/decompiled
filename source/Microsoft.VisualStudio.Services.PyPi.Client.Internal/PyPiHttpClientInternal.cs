// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.PyPi.Client.Internal.PyPiHttpClientInternal
// Assembly: Microsoft.VisualStudio.Services.PyPi.Client.Internal, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 2072801D-0EB4-49B3-8929-AFF365507D86
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.PyPi.Client.Internal.dll

using Microsoft.VisualStudio.Services.BlobStore.Common;
using Microsoft.VisualStudio.Services.BlobStore.WebApi.Contracts;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.PyPi.WebApi;
using Microsoft.VisualStudio.Services.PyPi.WebApi.Generated.Api;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.PyPi.Client.Internal
{
  [ResourceArea("92F0314B-06C5-46E0-ABE7-15FD9D13276A")]
  public class PyPiHttpClientInternal : PyPiApiHttpClient
  {
    private static readonly ApiResourceVersion CurrentApiVersion = new ApiResourceVersion("7.1-preview.1");

    public PyPiHttpClientInternal(Uri baseUrl, VssCredentials credentials)
      : base(baseUrl, credentials)
    {
    }

    public PyPiHttpClientInternal(
      Uri baseUrl,
      VssCredentials credentials,
      VssHttpRequestSettings settings)
      : base(baseUrl, credentials, settings)
    {
    }

    public PyPiHttpClientInternal(
      Uri baseUrl,
      VssCredentials credentials,
      params DelegatingHandler[] handlers)
      : base(baseUrl, credentials, handlers)
    {
    }

    public PyPiHttpClientInternal(Uri baseUrl, HttpMessageHandler pipeline, bool disposeHandler)
      : base(baseUrl, pipeline, disposeHandler)
    {
    }

    public PyPiHttpClientInternal(
      Uri baseUrl,
      VssCredentials credentials,
      VssHttpRequestSettings settings,
      params DelegatingHandler[] handlers)
      : base(baseUrl, credentials, settings, handlers)
    {
    }

    public virtual Task AddPackageFromBlobStoreAsync(
      string project,
      string feedId,
      BlobIdentifierWithBlocks blob,
      string fileName,
      long length,
      Dictionary<string, string[]> packageMetadata,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      object obj = (object) new
      {
        project = project,
        feedId = feedId
      };
      AddPackageFromBlobRequestInternal blobRequestInternal = new AddPackageFromBlobRequestInternal()
      {
        Blob = new Blob(blob),
        PackageMetadata = packageMetadata,
        FileName = fileName,
        Length = length
      };
      HttpMethod method = httpMethod;
      Guid packagesLocationId = ResourceIds.PyPiLargePackagesLocationId;
      object routeValues = obj;
      HttpContent httpContent = (HttpContent) new ObjectContent<AddPackageFromBlobRequestInternal>(blobRequestInternal, (MediaTypeFormatter) new VssJsonMediaTypeFormatter());
      ApiResourceVersion currentApiVersion = PyPiHttpClientInternal.CurrentApiVersion;
      HttpContent content = httpContent;
      object userState1 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      return (Task) this.SendAsync(method, packagesLocationId, routeValues, currentApiVersion, content, userState: userState1, cancellationToken: cancellationToken1);
    }
  }
}
