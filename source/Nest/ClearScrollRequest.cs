// Decompiled with JetBrains decompiler
// Type: Nest.ClearScrollRequest
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net;
using System.Collections.Generic;

namespace Nest
{
  public class ClearScrollRequest : 
    PlainRequestBase<ClearScrollRequestParameters>,
    IClearScrollRequest,
    IRequest<ClearScrollRequestParameters>,
    IRequest
  {
    protected IClearScrollRequest Self => (IClearScrollRequest) this;

    internal override ApiUrls ApiUrls => ApiUrlsLookups.NoNamespaceClearScroll;

    public ClearScrollRequest(IEnumerable<string> scrollIds) => this.ScrollIds = scrollIds;

    public ClearScrollRequest(string scrollId) => this.ScrollIds = (IEnumerable<string>) new string[1]
    {
      scrollId
    };

    public IEnumerable<string> ScrollIds { get; set; }
  }
}
