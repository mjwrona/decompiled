// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.BlobStore.Server.Egress.EgressParserResult
// Assembly: Microsoft.VisualStudio.Services.BlobStore.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: D3AB5C9B-EB54-4477-A304-63BB297414A3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.BlobStore.Server.dll

using System;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.BlobStore.Server.Egress
{
  [Serializable]
  public class EgressParserResult
  {
    public EgressParserResult()
    {
    }

    public EgressParserResult(string shardName) => this.ShardName = shardName;

    public string ShardName { get; set; }

    public int LogBlobProcessedCount { get; set; }

    public string Exception { get; set; }

    public Dictionary<string, long> EgressMetricPerHost { get; set; } = new Dictionary<string, long>();
  }
}
