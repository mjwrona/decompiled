// Decompiled with JetBrains decompiler
// Type: Nest.IConditionContainer
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net.Utf8Json;
using System.Runtime.Serialization;

namespace Nest
{
  [InterfaceDataContract]
  [ReadAs(typeof (ConditionContainer))]
  public interface IConditionContainer
  {
    [DataMember(Name = "always")]
    IAlwaysCondition Always { get; set; }

    [DataMember(Name = "array_compare")]
    IArrayCompareCondition ArrayCompare { get; set; }

    [DataMember(Name = "compare")]
    ICompareCondition Compare { get; set; }

    [DataMember(Name = "never")]
    INeverCondition Never { get; set; }

    [DataMember(Name = "script")]
    IScriptCondition Script { get; set; }
  }
}
