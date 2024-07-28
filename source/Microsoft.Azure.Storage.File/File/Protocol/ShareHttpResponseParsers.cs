// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Storage.File.Protocol.ShareHttpResponseParsers
// Assembly: Microsoft.Azure.Storage.File, Version=11.2.3.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: C68E95B0-8DFB-410C-8E70-706406D1A279
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Storage.File.dll

using Microsoft.Azure.Storage.Core.Util;
using Microsoft.Azure.Storage.Shared.Protocol;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;

namespace Microsoft.Azure.Storage.File.Protocol
{
  public static class ShareHttpResponseParsers
  {
    public static FileShareProperties GetProperties(HttpResponseMessage response)
    {
      CommonUtility.AssertNotNull(nameof (response), (object) response);
      FileShareProperties properties = new FileShareProperties();
      properties.ETag = response.Headers.ETag == null ? (string) null : response.Headers.ETag.ToString();
      properties.LastModified = response.Content == null ? new DateTimeOffset?() : response.Content.Headers.LastModified;
      string singleValueOrDefault = response.Headers.GetHeaderSingleValueOrDefault("x-ms-share-quota");
      if (!string.IsNullOrEmpty(singleValueOrDefault))
        properties.Quota = new int?(int.Parse(singleValueOrDefault, (IFormatProvider) CultureInfo.InvariantCulture));
      return properties;
    }

    public static IDictionary<string, string> GetMetadata(HttpResponseMessage response) => HttpResponseParsers.GetMetadata(response);

    public static Task ReadSharedAccessIdentifiersAsync(
      Stream inputStream,
      FileSharePermissions permissions,
      CancellationToken token)
    {
      CommonUtility.AssertNotNull(nameof (permissions), (object) permissions);
      return Response.ReadSharedAccessIdentifiersAsync<SharedAccessFilePolicy>((IDictionary<string, SharedAccessFilePolicy>) permissions.SharedAccessPolicies, (AccessPolicyResponseBase<SharedAccessFilePolicy>) new FileAccessPolicyResponse(inputStream), token);
    }

    public static string GetSnapshotTime(HttpResponseMessage response)
    {
      CommonUtility.AssertNotNull(nameof (response), (object) response);
      return response.Headers.GetHeaderSingleValueOrDefault("x-ms-snapshot");
    }

    public static Task<ShareStats> ReadShareStatsAsync(Stream inputStream, CancellationToken token) => Task.Run<ShareStats>((Func<ShareStats>) (() =>
    {
      using (XmlReader reader = XmlReader.Create(inputStream))
        return ShareStats.FromServiceXml(XDocument.Load(reader));
    }), token);
  }
}
