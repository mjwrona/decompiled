// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.BlobStore.WebApi.DedupStoreHttpClientFactory
// Assembly: Microsoft.VisualStudio.Services.BlobStore.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4500AC57-FBCC-4F18-B11F-F661A75E4A46
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.BlobStore.WebApi.dll

using Microsoft.VisualStudio.Services.Content.Common;
using System;

namespace Microsoft.VisualStudio.Services.BlobStore.WebApi
{
  public static class DedupStoreHttpClientFactory
  {
    public static IDedupStoreHttpClient GetClient(
      Uri dedupServiceUri,
      ArtifactHttpClientFactory clientFactory)
    {
      return (IDedupStoreHttpClient) clientFactory.CreateVssHttpClient(typeof (IDedupStoreHttpClient), typeof (DedupStoreHttpClient), dedupServiceUri);
    }

    public static IDomainDedupStoreHttpClient GetDomainClient(
      Uri dedupServiceUri,
      ArtifactHttpClientFactory clientFactory)
    {
      return (IDomainDedupStoreHttpClient) clientFactory.CreateVssHttpClient(typeof (IDomainDedupStoreHttpClient), typeof (DomainDedupStoreHttpClient), dedupServiceUri);
    }

    public static IHostDomainHttpClient GetHostDomainClient(
      Uri dedupServiceUri,
      ArtifactHttpClientFactory clientFactory)
    {
      return (IHostDomainHttpClient) clientFactory.CreateVssHttpClient(typeof (IHostDomainHttpClient), typeof (HostDomainHttpClient), dedupServiceUri);
    }
  }
}
