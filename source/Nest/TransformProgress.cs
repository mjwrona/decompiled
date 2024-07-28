// Decompiled with JetBrains decompiler
// Type: Nest.TransformProgress
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System.Runtime.Serialization;

namespace Nest
{
  public class TransformProgress
  {
    [DataMember(Name = "total_docs")]
    public long TotalDocs { get; internal set; }

    [DataMember(Name = "docs_remaining")]
    public long DocsRemaining { get; internal set; }

    [DataMember(Name = "percent_complete")]
    public double PercentComplete { get; internal set; }

    [DataMember(Name = "docs_processed")]
    public long DocsProcessed { get; internal set; }

    [DataMember(Name = "docs_indexed")]
    public long DocsIndexed { get; internal set; }
  }
}
