// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Documents.IClientSideRequestStatistics
// Assembly: Microsoft.Azure.Cosmos.Direct, Version=3.29.4.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: FFE3C00D-4333-4294-8947-B1C93A852E2F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Direct.dll

using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;

namespace Microsoft.Azure.Documents
{
  internal interface IClientSideRequestStatistics
  {
    List<TransportAddressUri> ContactedReplicas { get; set; }

    HashSet<TransportAddressUri> FailedReplicas { get; }

    HashSet<(string, Uri)> RegionsContacted { get; }

    bool? IsCpuHigh { get; }

    bool? IsCpuThreadStarvation { get; }

    void RecordRequest(DocumentServiceRequest request);

    void RecordResponse(
      DocumentServiceRequest request,
      StoreResult storeResult,
      DateTime startTimeUtc,
      DateTime endTimeUtc);

    void RecordException(
      DocumentServiceRequest request,
      Exception exception,
      DateTime startTimeUtc,
      DateTime endTimeUtc);

    string RecordAddressResolutionStart(Uri targetEndpoint);

    void RecordAddressResolutionEnd(string identifier);

    TimeSpan? RequestLatency { get; }

    void AppendToBuilder(StringBuilder stringBuilder);

    void RecordHttpResponse(
      HttpRequestMessage request,
      HttpResponseMessage response,
      ResourceType resourceType,
      DateTime requestStartTimeUtc);

    void RecordHttpException(
      HttpRequestMessage request,
      Exception exception,
      ResourceType resourceType,
      DateTime requestStartTimeUtc);
  }
}
