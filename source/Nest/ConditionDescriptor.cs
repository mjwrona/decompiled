// Decompiled with JetBrains decompiler
// Type: Nest.ConditionDescriptor
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;

namespace Nest
{
  public class ConditionDescriptor : ConditionContainer
  {
    private ConditionDescriptor Assign<TValue>(
      TValue value,
      Action<IConditionContainer, TValue> assigner)
    {
      return Fluent.Assign<ConditionDescriptor, IConditionContainer, TValue>(this, value, assigner);
    }

    public ConditionDescriptor Always() => this.Assign<AlwaysCondition>(new AlwaysCondition(), (Action<IConditionContainer, AlwaysCondition>) ((a, v) => a.Always = (IAlwaysCondition) v));

    public ConditionDescriptor Never() => this.Assign<NeverCondition>(new NeverCondition(), (Action<IConditionContainer, NeverCondition>) ((a, v) => a.Never = (INeverCondition) v));

    public ConditionDescriptor Compare(
      Func<CompareConditionDescriptor, ICompareCondition> selector)
    {
      return this.Assign<Func<CompareConditionDescriptor, ICompareCondition>>(selector, (Action<IConditionContainer, Func<CompareConditionDescriptor, ICompareCondition>>) ((a, v) => a.Compare = v != null ? v(new CompareConditionDescriptor()) : (ICompareCondition) null));
    }

    public ConditionDescriptor ArrayCompare(
      Func<ArrayCompareConditionDescriptor, IArrayCompareCondition> selector)
    {
      return this.Assign<Func<ArrayCompareConditionDescriptor, IArrayCompareCondition>>(selector, (Action<IConditionContainer, Func<ArrayCompareConditionDescriptor, IArrayCompareCondition>>) ((a, v) => a.ArrayCompare = v != null ? v(new ArrayCompareConditionDescriptor()) : (IArrayCompareCondition) null));
    }

    public ConditionDescriptor Script(
      Func<ScriptConditionDescriptor, IScriptCondition> selector)
    {
      return this.Assign<Func<ScriptConditionDescriptor, IScriptCondition>>(selector, (Action<IConditionContainer, Func<ScriptConditionDescriptor, IScriptCondition>>) ((a, v) => a.Script = v != null ? v(new ScriptConditionDescriptor()) : (IScriptCondition) null));
    }
  }
}
