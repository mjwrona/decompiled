// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Storage.Queue.Protocol.QueueHttpRequestMessageFactory
// Assembly: Microsoft.Azure.Storage.Queue, Version=11.2.3.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 3D35BFA0-638A-4C3C-8E74-B592D3B60EFD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Storage.Queue.dll

using Microsoft.Azure.Storage.Auth;
using Microsoft.Azure.Storage.Core;
using Microsoft.Azure.Storage.Core.Auth;
using Microsoft.Azure.Storage.Core.Util;
using Microsoft.Azure.Storage.Shared.Protocol;
using System;
using System.Collections.Generic;
using System.Net.Http;

namespace Microsoft.Azure.Storage.Queue.Protocol
{
  internal static class QueueHttpRequestMessageFactory
  {
    public static StorageRequestMessage Create(
      Uri uri,
      int? timeout,
      HttpContent content,
      OperationContext operationContext,
      ICanonicalizer canonicalizer,
      StorageCredentials credentials)
    {
      return HttpRequestMessageFactory.Create(uri, timeout, (UriQueryBuilder) null, content, operationContext, canonicalizer, credentials);
    }

    public static StorageRequestMessage Delete(
      Uri uri,
      int? timeout,
      HttpContent content,
      OperationContext operationContext,
      ICanonicalizer canonicalizer,
      StorageCredentials credentials)
    {
      return HttpRequestMessageFactory.Delete(uri, timeout, (UriQueryBuilder) null, content, operationContext, canonicalizer, credentials);
    }

    public static StorageRequestMessage ClearMessages(
      Uri uri,
      int? timeout,
      HttpContent content,
      OperationContext operationContext,
      ICanonicalizer canonicalizer,
      StorageCredentials credentials)
    {
      return HttpRequestMessageFactory.Delete(uri, timeout, (UriQueryBuilder) null, content, operationContext, canonicalizer, credentials);
    }

    public static StorageRequestMessage GetMetadata(
      Uri uri,
      int? timeout,
      HttpContent content,
      OperationContext operationContext,
      ICanonicalizer canonicalizer,
      StorageCredentials credentials)
    {
      return HttpRequestMessageFactory.GetMetadata(uri, timeout, (UriQueryBuilder) null, content, operationContext, canonicalizer, credentials);
    }

    public static StorageRequestMessage SetMetadata(
      Uri uri,
      int? timeout,
      HttpContent content,
      OperationContext operationContext,
      ICanonicalizer canonicalizer,
      StorageCredentials credentials)
    {
      return HttpRequestMessageFactory.SetMetadata(uri, timeout, (UriQueryBuilder) null, content, operationContext, canonicalizer, credentials);
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
      QueueListingDetails detailsIncluded,
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
      if ((detailsIncluded & QueueListingDetails.Metadata) != QueueListingDetails.None)
        builder.Add("include", "metadata");
      return HttpRequestMessageFactory.CreateRequestMessage(HttpMethod.Get, uri, timeout, builder, content, operationContext, canonicalizer, credentials);
    }

    public static StorageRequestMessage GetAcl(
      Uri uri,
      int? timeout,
      HttpContent content,
      OperationContext operationContext,
      ICanonicalizer canonicalizer,
      StorageCredentials credentials)
    {
      return HttpRequestMessageFactory.GetAcl(uri, timeout, (UriQueryBuilder) null, content, operationContext, canonicalizer, credentials);
    }

    public static StorageRequestMessage SetAcl(
      Uri uri,
      int? timeout,
      HttpContent content,
      OperationContext operationContext,
      ICanonicalizer canonicalizer,
      StorageCredentials credentials)
    {
      return HttpRequestMessageFactory.SetAcl(uri, timeout, (UriQueryBuilder) null, content, operationContext, canonicalizer, credentials);
    }

    public static StorageRequestMessage AddMessage(
      Uri uri,
      int? timeout,
      long? timeToLiveInSeconds,
      int? visibilityTimeoutInSeconds,
      HttpContent content,
      OperationContext operationContext,
      ICanonicalizer canonicalizer,
      StorageCredentials credentials)
    {
      UriQueryBuilder builder = new UriQueryBuilder();
      if (timeToLiveInSeconds.HasValue)
        builder.Add("messagettl", timeToLiveInSeconds.Value.ToString());
      if (visibilityTimeoutInSeconds.HasValue)
        builder.Add("visibilitytimeout", visibilityTimeoutInSeconds.Value.ToString());
      return HttpRequestMessageFactory.CreateRequestMessage(HttpMethod.Post, uri, timeout, builder, content, operationContext, canonicalizer, credentials);
    }

    public static StorageRequestMessage UpdateMessage(
      Uri uri,
      int? timeout,
      string popReceipt,
      TimeSpan? visibilityTimeout,
      HttpContent content,
      OperationContext operationContext,
      ICanonicalizer canonicalizer,
      StorageCredentials credentials)
    {
      UriQueryBuilder builder = new UriQueryBuilder();
      builder.Add("popreceipt", popReceipt);
      if (visibilityTimeout.HasValue)
        builder.Add("visibilitytimeout", visibilityTimeout.Value.TotalSeconds.ToString());
      else
        builder.Add("visibilitytimeout", "0");
      return HttpRequestMessageFactory.CreateRequestMessage(HttpMethod.Put, uri, timeout, builder, content, operationContext, canonicalizer, credentials);
    }

    public static StorageRequestMessage DeleteMessage(
      Uri uri,
      int? timeout,
      string popReceipt,
      HttpContent content,
      OperationContext operationContext,
      ICanonicalizer canonicalizer,
      StorageCredentials credentials)
    {
      UriQueryBuilder builder = new UriQueryBuilder();
      builder.Add("popreceipt", popReceipt);
      return HttpRequestMessageFactory.Delete(uri, timeout, builder, content, operationContext, canonicalizer, credentials);
    }

    public static StorageRequestMessage GetMessages(
      Uri uri,
      int? timeout,
      int numberOfMessages,
      TimeSpan? visibilityTimeout,
      HttpContent content,
      OperationContext operationContext,
      ICanonicalizer canonicalizer,
      StorageCredentials credentials)
    {
      UriQueryBuilder builder = new UriQueryBuilder();
      builder.Add("numofmessages", numberOfMessages.ToString());
      if (visibilityTimeout.HasValue)
        builder.Add("visibilitytimeout", visibilityTimeout.Value.RoundUpToSeconds().ToString());
      return HttpRequestMessageFactory.CreateRequestMessage(HttpMethod.Get, uri, timeout, builder, content, operationContext, canonicalizer, credentials);
    }

    public static StorageRequestMessage PeekMessages(
      Uri uri,
      int? timeout,
      int numberOfMessages,
      HttpContent content,
      OperationContext operationContext,
      ICanonicalizer canonicalizer,
      StorageCredentials credentials)
    {
      UriQueryBuilder builder = new UriQueryBuilder();
      builder.Add("peekonly", "true");
      builder.Add("numofmessages", numberOfMessages.ToString());
      return HttpRequestMessageFactory.CreateRequestMessage(HttpMethod.Get, uri, timeout, builder, content, operationContext, canonicalizer, credentials);
    }

    public static StorageRequestMessage GetServiceProperties(
      Uri uri,
      int? timeout,
      OperationContext operationContext,
      ICanonicalizer canonicalizer,
      StorageCredentials credentials)
    {
      return HttpRequestMessageFactory.GetServiceProperties(uri, timeout, operationContext, canonicalizer, credentials);
    }

    internal static StorageRequestMessage SetServiceProperties(
      Uri uri,
      int? timeout,
      HttpContent content,
      OperationContext operationContext,
      ICanonicalizer canonicalizer,
      StorageCredentials credentials)
    {
      return HttpRequestMessageFactory.SetServiceProperties(uri, timeout, content, operationContext, canonicalizer, credentials);
    }

    public static StorageRequestMessage GetServiceStats(
      Uri uri,
      int? timeout,
      OperationContext operationContext,
      ICanonicalizer canonicalizer,
      StorageCredentials credentials)
    {
      return HttpRequestMessageFactory.GetServiceStats(uri, timeout, operationContext, canonicalizer, credentials);
    }
  }
}
