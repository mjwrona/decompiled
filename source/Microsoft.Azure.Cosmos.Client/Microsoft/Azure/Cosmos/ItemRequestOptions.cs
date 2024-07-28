// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.ItemRequestOptions
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using System.Collections.Generic;
using System.Linq;

namespace Microsoft.Azure.Cosmos
{
  public class ItemRequestOptions : RequestOptions
  {
    public IEnumerable<string> PreTriggers { get; set; }

    public IEnumerable<string> PostTriggers { get; set; }

    public Microsoft.Azure.Cosmos.IndexingDirective? IndexingDirective { get; set; }

    public string SessionToken { get; set; }

    public Microsoft.Azure.Cosmos.ConsistencyLevel? ConsistencyLevel
    {
      get => this.BaseConsistencyLevel;
      set => this.BaseConsistencyLevel = value;
    }

    public bool? EnableContentResponseOnWrite { get; set; }

    public DedicatedGatewayRequestOptions DedicatedGatewayRequestOptions { get; set; }

    internal override void PopulateRequestOptions(RequestMessage request)
    {
      if (this.PreTriggers != null && this.PreTriggers.Any<string>())
        request.Headers.Add("x-ms-documentdb-pre-trigger-include", this.PreTriggers);
      if (this.PostTriggers != null && this.PostTriggers.Any<string>())
        request.Headers.Add("x-ms-documentdb-post-trigger-include", this.PostTriggers);
      Microsoft.Azure.Cosmos.IndexingDirective? indexingDirective = this.IndexingDirective;
      if (indexingDirective.HasValue)
      {
        indexingDirective = this.IndexingDirective;
        if (indexingDirective.HasValue)
        {
          Headers headers = request.Headers;
          indexingDirective = this.IndexingDirective;
          string str = IndexingDirectiveStrings.FromIndexingDirective(indexingDirective.Value);
          headers.Add("x-ms-indexing-directive", str);
        }
      }
      DedicatedGatewayRequestOptions.PopulateMaxIntegratedCacheStalenessOption(this.DedicatedGatewayRequestOptions, request);
      RequestOptions.SetSessionToken(request, this.SessionToken);
      base.PopulateRequestOptions(request);
    }
  }
}
