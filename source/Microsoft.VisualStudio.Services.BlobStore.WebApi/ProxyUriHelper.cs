// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.BlobStore.WebApi.ProxyUriHelper
// Assembly: Microsoft.VisualStudio.Services.BlobStore.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4500AC57-FBCC-4F18-B11F-F661A75E4A46
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.BlobStore.WebApi.dll

using BuildXL.Cache.ContentStore.Hashing;
using Microsoft.VisualStudio.Services.BlobStore.Common;
using System;
using System.Collections.Concurrent;
using System.Collections.Specialized;
using System.Web;

namespace Microsoft.VisualStudio.Services.BlobStore.WebApi
{
  public static class ProxyUriHelper
  {
    public static Uri GetProxyDownloadUri(BlobIdentifier blobId, Uri proxyUri, Uri blobServiceUri) => ProxyUriHelper.GetProxyDownloadUri(blobId, (Uri) null, proxyUri, blobServiceUri);

    public static Uri GetProxyDownloadUri(
      IDomainId domainId,
      BlobIdentifier blobId,
      Uri proxyUri,
      Uri blobServiceUri)
    {
      return ProxyUriHelper.GetProxyDownloadUri(domainId, blobId, (Uri) null, proxyUri, blobServiceUri);
    }

    public static Uri GetProxyDownloadUri(
      BlobIdentifier blobId,
      ConcurrentDictionary<BlobIdentifier, Uri> blobsToUris,
      Uri proxyUri,
      Uri blobServiceUri)
    {
      return ProxyUriHelper.GetProxyDownloadUri(blobId, blobsToUris[blobId], proxyUri, blobServiceUri);
    }

    public static Uri GetProxyDownloadUri(
      BlobIdentifier blobId,
      Uri sasUri,
      Uri proxyUri,
      Uri blobServiceUri)
    {
      return ProxyUriHelper.GetProxyDownloadUri((IDomainId) null, blobId, sasUri, proxyUri, blobServiceUri);
    }

    public static Uri GetProxyDownloadUri(
      IDomainId domainId,
      BlobIdentifier blobId,
      Uri sasUri,
      Uri proxyUri,
      Uri blobServiceUri)
    {
      UriBuilder uriBuilder = new UriBuilder(proxyUri);
      uriBuilder.Path = "blob/objects";
      NameValueCollection queryString = HttpUtility.ParseQueryString(uriBuilder.Query);
      queryString[Enum.GetName(typeof (ProxyRouteConstructs), (object) ProxyRouteConstructs.blobId)] = blobId.ValueString;
      if (sasUri != (Uri) null)
        queryString[Enum.GetName(typeof (ProxyRouteConstructs), (object) ProxyRouteConstructs.sasUri)] = sasUri.ToString();
      if (domainId != (IDomainId) null)
        queryString[Enum.GetName(typeof (ProxyRouteConstructs), (object) ProxyRouteConstructs.domainId)] = domainId.Serialize();
      queryString[Enum.GetName(typeof (ProxyRouteConstructs), (object) ProxyRouteConstructs.serviceUri)] = blobServiceUri.ToString();
      uriBuilder.Query = queryString.ToString();
      return uriBuilder.Uri;
    }
  }
}
