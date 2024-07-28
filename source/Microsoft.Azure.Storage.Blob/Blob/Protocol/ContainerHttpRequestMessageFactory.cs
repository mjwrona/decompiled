// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Storage.Blob.Protocol.ContainerHttpRequestMessageFactory
// Assembly: Microsoft.Azure.Storage.Blob, Version=11.2.3.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: A04A3512-352A-442F-A95B-BC1B94EF8840
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Storage.Blob.dll

using Microsoft.Azure.Storage.Auth;
using Microsoft.Azure.Storage.Core;
using Microsoft.Azure.Storage.Core.Auth;
using Microsoft.Azure.Storage.Shared.Protocol;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;

namespace Microsoft.Azure.Storage.Blob.Protocol
{
  internal static class ContainerHttpRequestMessageFactory
  {
    public static StorageRequestMessage GetAccountProperties(
      Uri uri,
      UriQueryBuilder builder,
      int? timeout,
      HttpContent content,
      OperationContext operationContext,
      ICanonicalizer canonicalizer,
      StorageCredentials credentials)
    {
      return HttpRequestMessageFactory.GetAccountProperties(uri, builder, timeout, content, operationContext, canonicalizer, credentials);
    }

    public static StorageRequestMessage Create(
      Uri uri,
      int? timeout,
      HttpContent content,
      OperationContext operationContext,
      ICanonicalizer canonicalizer,
      StorageCredentials credentials)
    {
      return ContainerHttpRequestMessageFactory.Create(uri, timeout, content, operationContext, BlobContainerPublicAccessType.Off, (BlobContainerEncryptionScopeOptions) null, canonicalizer, credentials);
    }

    public static StorageRequestMessage Create(
      Uri uri,
      int? timeout,
      HttpContent content,
      OperationContext operationContext,
      BlobContainerPublicAccessType accessType,
      BlobContainerEncryptionScopeOptions encryptionScopeOptions,
      ICanonicalizer canonicalizer,
      StorageCredentials credentials)
    {
      UriQueryBuilder containerUriQueryBuilder = ContainerHttpRequestMessageFactory.GetContainerUriQueryBuilder();
      StorageRequestMessage storageRequestMessage = HttpRequestMessageFactory.Create(uri, timeout, containerUriQueryBuilder, content, operationContext, canonicalizer, credentials);
      if (accessType != BlobContainerPublicAccessType.Off)
        storageRequestMessage.Headers.Add("x-ms-blob-public-access", accessType.ToString().ToLower());
      if (encryptionScopeOptions != null)
      {
        storageRequestMessage.Headers.Add("x-ms-default-encryption-scope", encryptionScopeOptions.DefaultEncryptionScope);
        storageRequestMessage.Headers.Add("x-ms-deny-encryption-scope-override", encryptionScopeOptions.PreventEncryptionScopeOverride ? "true" : "false");
      }
      return storageRequestMessage;
    }

    public static StorageRequestMessage Delete(
      Uri uri,
      int? timeout,
      AccessCondition accessCondition,
      HttpContent content,
      OperationContext operationContext,
      ICanonicalizer canonicalizer,
      StorageCredentials credentials)
    {
      UriQueryBuilder containerUriQueryBuilder = ContainerHttpRequestMessageFactory.GetContainerUriQueryBuilder();
      StorageRequestMessage request = HttpRequestMessageFactory.Delete(uri, timeout, containerUriQueryBuilder, content, operationContext, canonicalizer, credentials);
      request.ApplyAccessCondition(accessCondition);
      return request;
    }

    public static StorageRequestMessage GetMetadata(
      Uri uri,
      int? timeout,
      AccessCondition accessCondition,
      HttpContent content,
      OperationContext operationContext,
      ICanonicalizer canonicalizer,
      StorageCredentials credentials)
    {
      UriQueryBuilder containerUriQueryBuilder = ContainerHttpRequestMessageFactory.GetContainerUriQueryBuilder();
      StorageRequestMessage metadata = HttpRequestMessageFactory.GetMetadata(uri, timeout, containerUriQueryBuilder, content, operationContext, canonicalizer, credentials);
      metadata.ApplyAccessCondition(accessCondition);
      return metadata;
    }

    public static StorageRequestMessage GetProperties(
      Uri uri,
      int? timeout,
      AccessCondition accessCondition,
      HttpContent content,
      OperationContext operationContext,
      ICanonicalizer canonicalizer,
      StorageCredentials credentials)
    {
      UriQueryBuilder containerUriQueryBuilder = ContainerHttpRequestMessageFactory.GetContainerUriQueryBuilder();
      StorageRequestMessage properties = HttpRequestMessageFactory.GetProperties(uri, timeout, containerUriQueryBuilder, content, operationContext, canonicalizer, credentials);
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
      UriQueryBuilder containerUriQueryBuilder = ContainerHttpRequestMessageFactory.GetContainerUriQueryBuilder();
      StorageRequestMessage request = HttpRequestMessageFactory.SetMetadata(uri, timeout, containerUriQueryBuilder, content, operationContext, canonicalizer, credentials);
      request.ApplyAccessCondition(accessCondition);
      return request;
    }

    public static StorageRequestMessage Lease(
      Uri uri,
      int? timeout,
      LeaseAction action,
      string proposedLeaseId,
      int? leaseDuration,
      int? leaseBreakPeriod,
      AccessCondition accessCondition,
      HttpContent content,
      OperationContext operationContext,
      ICanonicalizer canonicalizer,
      StorageCredentials credentials)
    {
      UriQueryBuilder containerUriQueryBuilder = ContainerHttpRequestMessageFactory.GetContainerUriQueryBuilder();
      containerUriQueryBuilder.Add("comp", "lease");
      StorageRequestMessage requestMessage = HttpRequestMessageFactory.CreateRequestMessage(HttpMethod.Put, uri, timeout, containerUriQueryBuilder, content, operationContext, canonicalizer, credentials);
      BlobHttpRequestMessageFactory.AddLeaseAction(requestMessage, action);
      BlobHttpRequestMessageFactory.AddLeaseDuration(requestMessage, leaseDuration);
      BlobHttpRequestMessageFactory.AddProposedLeaseId(requestMessage, proposedLeaseId);
      BlobHttpRequestMessageFactory.AddLeaseBreakPeriod(requestMessage, leaseBreakPeriod);
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
      ContainerListingDetails detailsIncluded,
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
      if ((detailsIncluded & ContainerListingDetails.Metadata) != ContainerListingDetails.None)
        builder.Add("include", "metadata");
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
      StorageRequestMessage acl = HttpRequestMessageFactory.GetAcl(uri, timeout, ContainerHttpRequestMessageFactory.GetContainerUriQueryBuilder(), content, operationContext, canonicalizer, credentials);
      acl.ApplyAccessCondition(accessCondition);
      return acl;
    }

    public static StorageRequestMessage SetAcl(
      Uri uri,
      int? timeout,
      BlobContainerPublicAccessType publicAccess,
      AccessCondition accessCondition,
      HttpContent content,
      OperationContext operationContext,
      ICanonicalizer canonicalizer,
      StorageCredentials credentials)
    {
      StorageRequestMessage request = HttpRequestMessageFactory.SetAcl(uri, timeout, ContainerHttpRequestMessageFactory.GetContainerUriQueryBuilder(), content, operationContext, canonicalizer, credentials);
      if (publicAccess != BlobContainerPublicAccessType.Off)
        request.Headers.Add("x-ms-blob-public-access", publicAccess.ToString().ToLower());
      request.ApplyAccessCondition(accessCondition);
      return request;
    }

    public static StorageRequestMessage ListBlobs(
      Uri uri,
      int? timeout,
      BlobListingContext listingContext,
      HttpContent content,
      OperationContext operationContext,
      ICanonicalizer canonicalizer,
      StorageCredentials credentials)
    {
      UriQueryBuilder containerUriQueryBuilder = ContainerHttpRequestMessageFactory.GetContainerUriQueryBuilder();
      containerUriQueryBuilder.Add("comp", "list");
      if (listingContext != null)
      {
        if (listingContext.Prefix != null)
          containerUriQueryBuilder.Add("prefix", listingContext.Prefix);
        if (listingContext.Delimiter != null)
          containerUriQueryBuilder.Add("delimiter", listingContext.Delimiter);
        if (listingContext.Marker != null)
          containerUriQueryBuilder.Add("marker", listingContext.Marker);
        if (listingContext.MaxResults.HasValue)
          containerUriQueryBuilder.Add("maxresults", listingContext.MaxResults.ToString());
        if (listingContext.Details != BlobListingDetails.None)
        {
          StringBuilder stringBuilder = new StringBuilder();
          bool flag = false;
          if ((listingContext.Details & BlobListingDetails.Snapshots) == BlobListingDetails.Snapshots)
          {
            if (!flag)
              flag = true;
            else
              stringBuilder.Append(",");
            stringBuilder.Append("snapshots");
          }
          if ((listingContext.Details & BlobListingDetails.UncommittedBlobs) == BlobListingDetails.UncommittedBlobs)
          {
            if (!flag)
              flag = true;
            else
              stringBuilder.Append(",");
            stringBuilder.Append("uncommittedblobs");
          }
          if ((listingContext.Details & BlobListingDetails.Metadata) == BlobListingDetails.Metadata)
          {
            if (!flag)
              flag = true;
            else
              stringBuilder.Append(",");
            stringBuilder.Append("metadata");
          }
          if ((listingContext.Details & BlobListingDetails.Copy) == BlobListingDetails.Copy)
          {
            if (!flag)
              flag = true;
            else
              stringBuilder.Append(",");
            stringBuilder.Append("copy");
          }
          if ((listingContext.Details & BlobListingDetails.Deleted) == BlobListingDetails.Deleted)
          {
            if (flag)
              stringBuilder.Append(",");
            stringBuilder.Append("deleted");
          }
          containerUriQueryBuilder.Add("include", stringBuilder.ToString());
        }
      }
      return HttpRequestMessageFactory.CreateRequestMessage(HttpMethod.Get, uri, timeout, containerUriQueryBuilder, content, operationContext, canonicalizer, credentials);
    }

    internal static UriQueryBuilder GetContainerUriQueryBuilder()
    {
      UriQueryBuilder containerUriQueryBuilder = new UriQueryBuilder();
      containerUriQueryBuilder.Add("restype", "container");
      return containerUriQueryBuilder;
    }
  }
}
