// Decompiled with JetBrains decompiler
// Type: Nest.ClosePointInTimeDescriptor
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net;
using System;

namespace Nest
{
  public class ClosePointInTimeDescriptor : 
    RequestDescriptorBase<ClosePointInTimeDescriptor, ClosePointInTimeRequestParameters, IClosePointInTimeRequest>,
    IClosePointInTimeRequest,
    IRequest<ClosePointInTimeRequestParameters>,
    IRequest
  {
    internal override ApiUrls ApiUrls => ApiUrlsLookups.NoNamespaceClosePointInTime;

    string IClosePointInTimeRequest.Id { get; set; }

    public ClosePointInTimeDescriptor Id(string id) => this.Assign<string>(id, (Action<IClosePointInTimeRequest, string>) ((a, v) => a.Id = v));
  }
}
