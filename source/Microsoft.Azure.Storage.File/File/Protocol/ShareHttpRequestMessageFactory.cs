// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Storage.File.Protocol.ShareHttpRequestMessageFactory
// Assembly: Microsoft.Azure.Storage.File, Version=11.2.3.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: C68E95B0-8DFB-410C-8E70-706406D1A279
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Storage.File.dll

using Microsoft.Azure.Storage.Auth;
using Microsoft.Azure.Storage.Core;
using Microsoft.Azure.Storage.Core.Auth;
using Microsoft.Azure.Storage.Core.Util;
using Microsoft.Azure.Storage.Shared.Protocol;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;

namespace Microsoft.Azure.Storage.File.Protocol
{
  internal static class ShareHttpRequestMessageFactory
  {
    public static StorageRequestMessage Create(
      Uri uri,
      FileShareProperties properties,
      int? timeout,
      HttpContent content,
      OperationContext operationContext,
      ICanonicalizer canonicalizer,
      StorageCredentials credentials)
    {
      UriQueryBuilder shareUriQueryBuilder = ShareHttpRequestMessageFactory.GetShareUriQueryBuilder();
      StorageRequestMessage storageRequestMessage = HttpRequestMessageFactory.Create(uri, timeout, shareUriQueryBuilder, content, operationContext, canonicalizer, credentials);
      if (properties != null)
      {
        int? quota = properties.Quota;
        if (quota.HasValue)
        {
          StorageRequestMessage request = storageRequestMessage;
          quota = properties.Quota;
          int? nullable = new int?(quota.Value);
          request.AddOptionalHeader("x-ms-share-quota", nullable);
        }
      }
      return storageRequestMessage;
    }

    public static StorageRequestMessage Delete(
      Uri uri,
      int? timeout,
      DateTimeOffset? snapshot,
      DeleteShareSnapshotsOption deleteSnapshotsOption,
      AccessCondition accessCondition,
      HttpContent content,
      OperationContext operationContext,
      ICanonicalizer canonicalizer,
      StorageCredentials credentials)
    {
      UriQueryBuilder shareUriQueryBuilder = ShareHttpRequestMessageFactory.GetShareUriQueryBuilder();
      ShareHttpRequestMessageFactory.AddShareSnapshot(shareUriQueryBuilder, snapshot);
      StorageRequestMessage request = HttpRequestMessageFactory.Delete(uri, timeout, shareUriQueryBuilder, content, operationContext, canonicalizer, credentials);
      if (deleteSnapshotsOption != DeleteShareSnapshotsOption.None && deleteSnapshotsOption == DeleteShareSnapshotsOption.IncludeSnapshots)
        request.Headers.Add("x-ms-delete-snapshots", "include");
      request.ApplyAccessCondition(accessCondition);
      return request;
    }

    public static StorageRequestMessage GetMetadata(
      Uri uri,
      int? timeout,
      DateTimeOffset? snapshot,
      AccessCondition accessCondition,
      HttpContent content,
      OperationContext operationContext,
      ICanonicalizer canonicalizer,
      StorageCredentials credentials)
    {
      UriQueryBuilder shareUriQueryBuilder = ShareHttpRequestMessageFactory.GetShareUriQueryBuilder();
      ShareHttpRequestMessageFactory.AddShareSnapshot(shareUriQueryBuilder, snapshot);
      StorageRequestMessage metadata = HttpRequestMessageFactory.GetMetadata(uri, timeout, shareUriQueryBuilder, content, operationContext, canonicalizer, credentials);
      metadata.ApplyAccessCondition(accessCondition);
      return metadata;
    }

    public static StorageRequestMessage GetProperties(
      Uri uri,
      int? timeout,
      DateTimeOffset? snapshot,
      AccessCondition accessCondition,
      HttpContent content,
      OperationContext operationContext,
      ICanonicalizer canonicalizer,
      StorageCredentials credentials)
    {
      UriQueryBuilder shareUriQueryBuilder = ShareHttpRequestMessageFactory.GetShareUriQueryBuilder();
      ShareHttpRequestMessageFactory.AddShareSnapshot(shareUriQueryBuilder, snapshot);
      StorageRequestMessage properties = HttpRequestMessageFactory.GetProperties(uri, timeout, shareUriQueryBuilder, content, operationContext, canonicalizer, credentials);
      properties.ApplyAccessCondition(accessCondition);
      return properties;
    }

    public static StorageRequestMessage SetMetadata(
      Uri uri,
      int? timeout,
      AccessCondition accessCondition,
      HttpContent content,
      OperationContext operationContext,
      ICanonicalizer canonicalizer,
      StorageCredentials credentials)
    {
      UriQueryBuilder shareUriQueryBuilder = ShareHttpRequestMessageFactory.GetShareUriQueryBuilder();
      StorageRequestMessage request = HttpRequestMessageFactory.SetMetadata(uri, timeout, shareUriQueryBuilder, content, operationContext, canonicalizer, credentials);
      request.ApplyAccessCondition(accessCondition);
      return request;
    }

    public static StorageRequestMessage SetProperties(
      Uri uri,
      int? timeout,
      FileShareProperties properties,
      AccessCondition accessCondition,
      HttpContent content,
      OperationContext operationContext,
      ICanonicalizer canonicalizer,
      StorageCredentials credentials)
    {
      CommonUtility.AssertNotNull(nameof (properties), (object) properties);
      UriQueryBuilder shareUriQueryBuilder = ShareHttpRequestMessageFactory.GetShareUriQueryBuilder();
      shareUriQueryBuilder.Add("comp", nameof (properties));
      StorageRequestMessage requestMessage = HttpRequestMessageFactory.CreateRequestMessage(HttpMethod.Put, uri, timeout, shareUriQueryBuilder, content, operationContext, canonicalizer, credentials);
      if (properties.Quota.HasValue)
        requestMessage.AddOptionalHeader("x-ms-share-quota", new int?(properties.Quota.Value));
      requestMessage.ApplyAccessCondition(accessCondition);
      return requestMessage;
    }

    public static void AddMetadata(
      StorageRequestMessage request,
      IDictionary<string, string> metadata)
    {
      HttpRequestMessageFactory.AddMetadata(request, metadata);
    }

    public static void AddMetadata(StorageRequestMessage request, string name, string value) => HttpRequestMessageFactory.AddMetadata(request, name, value);

    public static StorageRequestMessage List(
      Uri uri,
      int? timeout,
      ListingContext listingContext,
      ShareListingDetails detailsIncluded,
      HttpContent content,
      OperationContext operationContext,
      ICanonicalizer canonicalizer,
      StorageCredentials credentials)
    {
      UriQueryBuilder builder = new UriQueryBuilder();
      builder.Add("comp", "list");
      if (listingContext != null)
      {
        if (listingContext.Prefix != null)
          builder.Add("prefix", listingContext.Prefix);
        if (listingContext.Marker != null)
          builder.Add("marker", listingContext.Marker);
        if (listingContext.MaxResults.HasValue)
          builder.Add("maxresults", listingContext.MaxResults.ToString());
      }
      if (detailsIncluded != ShareListingDetails.None)
      {
        StringBuilder stringBuilder = new StringBuilder();
        bool flag = false;
        if ((detailsIncluded & ShareListingDetails.Snapshots) == ShareListingDetails.Snapshots)
        {
          if (!flag)
            flag = true;
          else
            stringBuilder.Append(",");
          stringBuilder.Append("snapshots");
        }
        if ((detailsIncluded & ShareListingDetails.Metadata) == ShareListingDetails.Metadata)
        {
          if (flag)
            stringBuilder.Append(",");
          stringBuilder.Append("metadata");
        }
        builder.Add("include", stringBuilder.ToString());
      }
      return HttpRequestMessageFactory.CreateRequestMessage(HttpMethod.Get, uri, timeout, builder, content, operationContext, canonicalizer, credentials);
    }

    public static StorageRequestMessage GetAcl(
      Uri uri,
      int? timeout,
      AccessCondition accessCondition,
      HttpContent content,
      OperationContext operationContext,
      ICanonicalizer canonicalizer,
      StorageCredentials credentials)
    {
      StorageRequestMessage acl = HttpRequestMessageFactory.GetAcl(uri, timeout, ShareHttpRequestMessageFactory.GetShareUriQueryBuilder(), content, operationContext, canonicalizer, credentials);
      acl.ApplyAccessCondition(accessCondition);
      return acl;
    }

    public static StorageRequestMessage SetAcl(
      Uri uri,
      int? timeout,
      FileSharePublicAccessType publicAccess,
      AccessCondition accessCondition,
      HttpContent content,
      OperationContext operationContext,
      ICanonicalizer canonicalizer,
      StorageCredentials credentials)
    {
      StorageRequestMessage request = HttpRequestMessageFactory.SetAcl(uri, timeout, ShareHttpRequestMessageFactory.GetShareUriQueryBuilder(), content, operationContext, canonicalizer, credentials);
      request.ApplyAccessCondition(accessCondition);
      return request;
    }

    public static StorageRequestMessage GetStats(
      Uri uri,
      int? timeout,
      OperationContext operationContext,
      ICanonicalizer canonicalizer,
      StorageCredentials credentials)
    {
      UriQueryBuilder shareUriQueryBuilder = ShareHttpRequestMessageFactory.GetShareUriQueryBuilder();
      shareUriQueryBuilder.Add("comp", "stats");
      return HttpRequestMessageFactory.CreateRequestMessage(HttpMethod.Get, uri, timeout, shareUriQueryBuilder, (HttpContent) null, operationContext, canonicalizer, credentials);
    }

    public static StorageRequestMessage Snapshot(
      Uri uri,
      int? timeout,
      AccessCondition accessCondition,
      HttpContent content,
      OperationContext operationContext,
      ICanonicalizer canonicalizer,
      StorageCredentials credentials)
    {
      UriQueryBuilder builder = new UriQueryBuilder();
      builder.Add("restype", "share");
      builder.Add("comp", "snapshot");
      StorageRequestMessage requestMessage = HttpRequestMessageFactory.CreateRequestMessage(HttpMethod.Put, uri, timeout, builder, content, operationContext, canonicalizer, credentials);
      requestMessage.ApplyAccessCondition(accessCondition);
      return requestMessage;
    }

    public static StorageRequestMessage CreateFilePermission(
      Uri uri,
      int? timeout,
      HttpContent content,
      OperationContext operationContext,
      ICanonicalizer canonicalizer,
      StorageCredentials credentials)
    {
      UriQueryBuilder builder = new UriQueryBuilder();
      builder.Add("restype", "share");
      builder.Add("comp", "filepermission");
      return HttpRequestMessageFactory.CreateRequestMessage(HttpMethod.Put, uri, timeout, builder, content, operationContext, canonicalizer, credentials);
    }

    public static StorageRequestMessage GetFilePermission(
      Uri uri,
      int? timeout,
      string filePermissionKey,
      HttpContent content,
      OperationContext operationContext,
      ICanonicalizer canonicalizer,
      StorageCredentials credentials)
    {
      UriQueryBuilder builder = new UriQueryBuilder();
      builder.Add("restype", "share");
      builder.Add("comp", "filepermission");
      StorageRequestMessage requestMessage = HttpRequestMessageFactory.CreateRequestMessage(HttpMethod.Get, uri, timeout, builder, content, operationContext, canonicalizer, credentials);
      requestMessage.Headers.Add("x-ms-file-permission-key", filePermissionKey);
      return requestMessage;
    }

    private static void AddShareSnapshot(UriQueryBuilder builder, DateTimeOffset? snapshot)
    {
      if (!snapshot.HasValue)
        return;
      builder.Add("sharesnapshot", Request.ConvertDateTimeToSnapshotString(snapshot.Value));
    }

    internal static UriQueryBuilder GetShareUriQueryBuilder()
    {
      UriQueryBuilder shareUriQueryBuilder = new UriQueryBuilder();
      shareUriQueryBuilder.Add("restype", "share");
      return shareUriQueryBuilder;
    }
  }
}
