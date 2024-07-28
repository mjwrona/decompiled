// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Documents.IClientSideRequestStatistics
// Assembly: Microsoft.Azure.DocumentDB.Core, Version=2.10.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 20BB0EB7-1465-494C-8F4E-F898448B85D9
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.DocumentDB.Core.dll

using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.Azure.Documents
{
  internal interface IClientSideRequestStatistics
  {
    List<Uri> ContactedReplicas { get; set; }

    HashSet<Uri> FailedReplicas { get; }

    HashSet<Uri> RegionsContacted { get; }

    bool IsCpuOverloaded { get; }

    void RecordRequest(DocumentServiceRequest request);

    void RecordResponse(DocumentServiceRequest request, StoreResult storeResult);

    string RecordAddressResolutionStart(Uri targetEndpoint);

    void RecordAddressResolutionEnd(string identifier);

    TimeSpan RequestLatency { get; }

    void AppendToBuilder(StringBuilder stringBuilder);
  }
}
