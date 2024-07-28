// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Storage.Shared.Protocol.HttpRequestMessageFactory
// Assembly: Microsoft.Azure.Storage.Common, Version=11.2.3.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 0978DA65-6954-4A99-9ACB-2EF3D979A5D5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Storage.Common.dll

using Microsoft.Azure.Storage.Auth;
using Microsoft.Azure.Storage.Core;
using Microsoft.Azure.Storage.Core.Auth;
using Microsoft.Azure.Storage.Core.Util;
using System;
using System.Collections.Generic;
using System.Net.Http;

namespace Microsoft.Azure.Storage.Shared.Protocol
{
  internal static class HttpRequestMessageFactory
  {
    internal static StorageRequestMessage CreateRequestMessage(
      HttpMethod method,
      Uri uri,
      int? timeout,
      UriQueryBuilder builder,
      HttpContent content,
      OperationContext operationContext,
      ICanonicalizer canonicalizer,
      StorageCredentials credentials)
    {
      if (builder == null)
        builder = new UriQueryBuilder();
      if (timeout.HasValue && timeout.Value > 0)
        builder.Add(nameof (timeout), timeout.ToString());
      Uri uri1 = builder.AddToUri(uri);
      StorageRequestMessage requestMessage = new StorageRequestMessage(method, uri1, canonicalizer, credentials, credentials?.AccountName);
      requestMessage.Content = content;
      return requestMessage;
    }

    internal static StorageRequestMessage Create(
      Uri uri,
      int? timeout,
      UriQueryBuilder builder,
      HttpContent content,
      OperationContext operationContext,
      ICanonicalizer canonicalizer,
      StorageCredentials credentials)
    {
      return HttpRequestMessageFactory.CreateRequestMessage(HttpMethod.Put, uri, timeout, builder, content, operationContext, canonicalizer, credentials);
    }

    internal static StorageRequestMessage GetAcl(
      Uri uri,
      int? timeout,
      UriQueryBuilder builder,
      HttpContent content,
      OperationContext operationContext,
      ICanonicalizer canonicalizer,
      StorageCredentials credentials)
    {
      if (builder == null)
        builder = new UriQueryBuilder();
      builder.Add("comp", "acl");
      return HttpRequestMessageFactory.CreateRequestMessage(HttpMethod.Get, uri, timeout, builder, content, operationContext, canonicalizer, credentials);
    }

    internal static StorageRequestMessage SetAcl(
      Uri uri,
      int? timeout,
      UriQueryBuilder builder,
      HttpContent content,
      OperationContext operationContext,
      ICanonicalizer canonicalizer,
      StorageCredentials credentials)
    {
      if (builder == null)
        builder = new UriQueryBuilder();
      builder.Add("comp", "acl");
      return HttpRequestMessageFactory.CreateRequestMessage(HttpMethod.Put, uri, timeout, builder, content, operationContext, canonicalizer, credentials);
    }

    internal static StorageRequestMessage GetProperties(
      Uri uri,
      int? timeout,
      UriQueryBuilder builder,
      HttpContent content,
      OperationContext operationContext,
      ICanonicalizer canonicalizer,
      StorageCredentials credentials)
    {
      return HttpRequestMessageFactory.CreateRequestMessage(HttpMethod.Head, uri, timeout, builder, content, operationContext, canonicalizer, credentials);
    }

    internal static StorageRequestMessage GetMetadata(
      Uri uri,
      int? timeout,
      UriQueryBuilder builder,
      HttpContent content,
      OperationContext operationContext,
      ICanonicalizer canonicalizer,
      StorageCredentials credentials)
    {
      if (builder == null)
        builder = new UriQueryBuilder();
      builder.Add("comp", "metadata");
      return HttpRequestMessageFactory.CreateRequestMessage(HttpMethod.Head, uri, timeout, builder, content, operationContext, canonicalizer, credentials);
    }

    internal static StorageRequestMessage SetMetadata(
      Uri uri,
      int? timeout,
      UriQueryBuilder builder,
      HttpContent content,
      OperationContext operationContext,
      ICanonicalizer canonicalizer,
      StorageCredentials credentials)
    {
      if (builder == null)
        builder = new UriQueryBuilder();
      builder.Add("comp", "metadata");
      return HttpRequestMessageFactory.CreateRequestMessage(HttpMethod.Put, uri, timeout, builder, content, operationContext, canonicalizer, credentials);
    }

    internal static void AddMetadata(
      StorageRequestMessage request,
      IDictionary<string, string> metadata)
    {
      if (metadata == null)
        return;
      foreach (KeyValuePair<string, string> keyValuePair in (IEnumerable<KeyValuePair<string, string>>) metadata)
        HttpRequestMessageFactory.AddMetadata(request, keyValuePair.Key, keyValuePair.Value);
    }

    internal static void AddMetadata(StorageRequestMessage request, string name, string value)
    {
      CommonUtility.AssertNotNull(nameof (value), (object) value);
      if (string.IsNullOrWhiteSpace(value))
        throw new ArgumentException("The argument must not be empty string.", value);
      request.Headers.Add("x-ms-meta-" + name, value);
    }

    internal static StorageRequestMessage Delete(
      Uri uri,
      int? timeout,
      UriQueryBuilder builder,
      HttpContent content,
      OperationContext operationContext,
      ICanonicalizer canonicalizer,
      StorageCredentials credentials)
    {
      return HttpRequestMessageFactory.CreateRequestMessage(HttpMethod.Delete, uri, timeout, builder, content, operationContext, canonicalizer, credentials);
    }

    internal static StorageRequestMessage Undelete(
      Uri uri,
      int? timeout,
      UriQueryBuilder builder,
      HttpContent content,
      OperationContext operationContext,
      ICanonicalizer canonicalizer,
      StorageCredentials credentials)
    {
      if (builder == null)
        builder = new UriQueryBuilder();
      builder.Add("comp", "undelete");
      return HttpRequestMessageFactory.CreateRequestMessage(HttpMethod.Put, uri, timeout, builder, content, operationContext, canonicalizer, credentials);
    }

    internal static StorageRequestMessage GetAccountProperties(
      Uri uri,
      UriQueryBuilder builder,
      int? timeout,
      HttpContent content,
      OperationContext operationContext,
      ICanonicalizer canonicalizer,
      StorageCredentials credentials)
    {
      if (builder == null)
        builder = new UriQueryBuilder();
      builder.Add("comp", "properties");
      builder.Add("restype", "account");
      return HttpRequestMessageFactory.CreateRequestMessage(HttpMethod.Head, uri, timeout, builder, content, operationContext, canonicalizer, credentials);
    }

    internal static StorageRequestMessage GetServiceProperties(
      Uri uri,
      int? timeout,
      OperationContext operationContext,
      ICanonicalizer canonicalizer,
      StorageCredentials credentials)
    {
      UriQueryBuilder serviceUriQueryBuilder = HttpRequestMessageFactory.GetServiceUriQueryBuilder();
      serviceUriQueryBuilder.Add("comp", "properties");
      return HttpRequestMessageFactory.CreateRequestMessage(HttpMethod.Get, uri, timeout, serviceUriQueryBuilder, (HttpContent) null, operationContext, canonicalizer, credentials);
    }

    internal static StorageRequestMessage SetServiceProperties(
      Uri uri,
      int? timeout,
      HttpContent content,
      OperationContext operationContext,
      ICanonicalizer canonicalizer,
      StorageCredentials credentials)
    {
      UriQueryBuilder serviceUriQueryBuilder = HttpRequestMessageFactory.GetServiceUriQueryBuilder();
      serviceUriQueryBuilder.Add("comp", "properties");
      return HttpRequestMessageFactory.CreateRequestMessage(HttpMethod.Put, uri, timeout, serviceUriQueryBuilder, content, operationContext, canonicalizer, credentials);
    }

    internal static StorageRequestMessage GetServiceStats(
      Uri uri,
      int? timeout,
      OperationContext operationContext,
      ICanonicalizer canonicalizer,
      StorageCredentials credentials)
    {
      UriQueryBuilder serviceUriQueryBuilder = HttpRequestMessageFactory.GetServiceUriQueryBuilder();
      serviceUriQueryBuilder.Add("comp", "stats");
      return HttpRequestMessageFactory.CreateRequestMessage(HttpMethod.Get, uri, timeout, serviceUriQueryBuilder, (HttpContent) null, operationContext, canonicalizer, credentials);
    }

    internal static UriQueryBuilder GetServiceUriQueryBuilder()
    {
      UriQueryBuilder serviceUriQueryBuilder = new UriQueryBuilder();
      serviceUriQueryBuilder.Add("restype", "service");
      return serviceUriQueryBuilder;
    }
  }
}
