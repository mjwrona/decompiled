// Decompiled with JetBrains decompiler
// Type: Nest.ClearScrollDescriptor
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net;
using System;
using System.Collections.Generic;

namespace Nest
{
  public class ClearScrollDescriptor : 
    RequestDescriptorBase<ClearScrollDescriptor, ClearScrollRequestParameters, IClearScrollRequest>,
    IClearScrollRequest,
    IRequest<ClearScrollRequestParameters>,
    IRequest
  {
    internal override ApiUrls ApiUrls => ApiUrlsLookups.NoNamespaceClearScroll;

    IEnumerable<string> IClearScrollRequest.ScrollIds { get; set; }

    public ClearScrollDescriptor ScrollId(params string[] scrollIds) => this.Assign<string[]>(scrollIds, (Action<IClearScrollRequest, string[]>) ((a, v) => a.ScrollIds = (IEnumerable<string>) v));
  }
}
