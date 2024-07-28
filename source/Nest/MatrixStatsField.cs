// Decompiled with JetBrains decompiler
// Type: Nest.MatrixStatsField
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Nest
{
  [DataContract]
  public class MatrixStatsField
  {
    [DataMember(Name = "correlation")]
    public Dictionary<string, double> Correlation { get; set; }

    [DataMember(Name = "count")]
    public int Count { get; set; }

    [DataMember(Name = "covariance")]
    public Dictionary<string, double> Covariance { get; set; }

    [DataMember(Name = "kurtosis")]
    public double Kurtosis { get; set; }

    [DataMember(Name = "mean")]
    public double Mean { get; set; }

    [DataMember(Name = "name")]
    public string Name { get; set; }

    [DataMember(Name = "skewness")]
    public double Skewness { get; set; }

    [DataMember(Name = "variance")]
    public double Variance { get; set; }
  }
}
