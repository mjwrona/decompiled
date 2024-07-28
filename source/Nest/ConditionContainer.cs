// Decompiled with JetBrains decompiler
// Type: Nest.ConditionContainer
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System.Runtime.Serialization;

namespace Nest
{
  [DataContract]
  public class ConditionContainer : IConditionContainer, IDescriptor
  {
    internal ConditionContainer()
    {
    }

    public ConditionContainer(ConditionBase condition)
    {
      condition.ThrowIfNull<ConditionBase>(nameof (condition));
      condition.WrapInContainer((IConditionContainer) this);
    }

    IAlwaysCondition IConditionContainer.Always { get; set; }

    IArrayCompareCondition IConditionContainer.ArrayCompare { get; set; }

    ICompareCondition IConditionContainer.Compare { get; set; }

    INeverCondition IConditionContainer.Never { get; set; }

    IScriptCondition IConditionContainer.Script { get; set; }

    public static implicit operator ConditionContainer(ConditionBase condition) => condition != null ? new ConditionContainer(condition) : (ConditionContainer) null;
  }
}
