// Decompiled with JetBrains decompiler
// Type: Nest.AllocationDecision
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System.Runtime.Serialization;

namespace Nest
{
  [DataContract]
  public class AllocationDecision
  {
    [DataMember(Name = "decider")]
    public string Decider { get; set; }

    [DataMember(Name = "decision")]
    public AllocationExplainDecision Decision { get; set; }

    [DataMember(Name = "explanation")]
    public string Explanation { get; set; }
  }
}
