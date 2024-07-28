// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Storage.Queue.Protocol.QueueHttpResponseParsers
// Assembly: Microsoft.Azure.Storage.Queue, Version=11.2.3.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 3D35BFA0-638A-4C3C-8E74-B592D3B60EFD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Storage.Queue.dll

using Microsoft.Azure.Storage.Core.Util;
using Microsoft.Azure.Storage.Shared.Protocol;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.Azure.Storage.Queue.Protocol
{
  public static class QueueHttpResponseParsers
  {
    public static string GetApproximateMessageCount(HttpResponseMessage response)
    {
      CommonUtility.AssertNotNull(nameof (response), (object) response);
      return response.Headers.GetHeaderSingleValueOrDefault("x-ms-approximate-messages-count");
    }

    public static IDictionary<string, string> GetMetadata(HttpResponseMessage response) => HttpResponseParsers.GetMetadata(response);

    public static string GetPopReceipt(HttpResponseMessage response)
    {
      CommonUtility.AssertNotNull(nameof (response), (object) response);
      return HttpResponseParsers.GetHeader(response, "x-ms-popreceipt");
    }

    public static DateTime GetNextVisibleTime(HttpResponseMessage response)
    {
      CommonUtility.AssertNotNull(nameof (response), (object) response);
      return DateTime.Parse(HttpResponseParsers.GetHeader(response, "x-ms-time-next-visible"), (IFormatProvider) DateTimeFormatInfo.InvariantInfo, DateTimeStyles.AdjustToUniversal);
    }

    public static Task<ServiceProperties> ReadServicePropertiesAsync(
      Stream inputStream,
      CancellationToken token)
    {
      return HttpResponseParsers.ReadServicePropertiesAsync(inputStream, token);
    }

    public static Task<ServiceStats> ReadServiceStatsAsync(
      Stream inputStream,
      CancellationToken token)
    {
      return HttpResponseParsers.ReadServiceStatsAsync(inputStream, token);
    }

    public static Task ReadSharedAccessIdentifiersAsync(
      Stream inputStream,
      QueuePermissions permissions,
      CancellationToken token)
    {
      CommonUtility.AssertNotNull(nameof (permissions), (object) permissions);
      return Response.ReadSharedAccessIdentifiersAsync<SharedAccessQueuePolicy>((IDictionary<string, SharedAccessQueuePolicy>) permissions.SharedAccessPolicies, (AccessPolicyResponseBase<SharedAccessQueuePolicy>) new QueueAccessPolicyResponse(inputStream), token);
    }
  }
}
