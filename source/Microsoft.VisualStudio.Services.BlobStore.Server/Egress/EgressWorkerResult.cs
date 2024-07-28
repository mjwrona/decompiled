// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.BlobStore.Server.Egress.EgressWorkerResult
// Assembly: Microsoft.VisualStudio.Services.BlobStore.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: D3AB5C9B-EB54-4477-A304-63BB297414A3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.BlobStore.Server.dll

using System;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.BlobStore.Server.Egress
{
  [Serializable]
  public class EgressWorkerResult
  {
    public HashSet<string> ExceptionSet = new HashSet<string>();
    public Dictionary<string, Dictionary<string, EgressParserResult>> ShardMetricMap = new Dictionary<string, Dictionary<string, EgressParserResult>>();

    public int PartitionId { get; set; }

    public string JobId { get; set; }

    public DateTimeOffset LifeSpan { get; set; }
  }
}
