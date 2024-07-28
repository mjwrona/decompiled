// Decompiled with JetBrains decompiler
// Type: Nest.IGraphExploreControls
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System.Runtime.Serialization;

namespace Nest
{
  public interface IGraphExploreControls
  {
    [DataMember(Name = "sample_diversity")]
    SampleDiversity SampleDiversity { get; set; }

    [DataMember(Name = "sample_size")]
    int? SampleSize { get; set; }

    [DataMember(Name = "timeout")]
    Time Timeout { get; set; }

    [DataMember(Name = "use_significance")]
    bool? UseSignificance { get; set; }
  }
}
