// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.BlobStore.WebApi.BlobStoreHttpClientFactory
// Assembly: Microsoft.VisualStudio.Services.BlobStore.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4500AC57-FBCC-4F18-B11F-F661A75E4A46
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.BlobStore.WebApi.dll

using Microsoft.VisualStudio.Services.Content.Common;
using System;

namespace Microsoft.VisualStudio.Services.BlobStore.WebApi
{
  public static class BlobStoreHttpClientFactory
  {
    public static IBlobStoreHttpClient GetClient(
      Uri blobServiceUri,
      ArtifactHttpClientFactory clientFactory)
    {
      return clientFactory.CreateVssHttpClient<IBlobStoreHttpClient, BlobStore2HttpClient>(blobServiceUri);
    }

    public static IDomainBlobStoreHttpClient GetDomainClient(
      Uri blobServiceUri,
      ArtifactHttpClientFactory clientFactory)
    {
      return clientFactory.CreateVssHttpClient<IDomainBlobStoreHttpClient, DomainBlobstoreHttpClient>(blobServiceUri);
    }

    public static IHostDomainHttpClient GetHostDomainClient(
      Uri blobServiceUri,
      ArtifactHttpClientFactory clientFactory)
    {
      return clientFactory.CreateVssHttpClient<IHostDomainHttpClient, HostDomainHttpClient>(blobServiceUri);
    }
  }
}
