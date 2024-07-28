// Decompiled with JetBrains decompiler
// Type: Nest.RuleConditionDescriptor
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;

namespace Nest
{
  public class RuleConditionDescriptor : 
    DescriptorBase<RuleConditionDescriptor, IRuleCondition>,
    IRuleCondition
  {
    Nest.AppliesTo IRuleCondition.AppliesTo { get; set; }

    ConditionOperator IRuleCondition.Operator { get; set; }

    double IRuleCondition.Value { get; set; }

    public RuleConditionDescriptor AppliesTo(Nest.AppliesTo appliesTo) => this.Assign<Nest.AppliesTo>(appliesTo, (Action<IRuleCondition, Nest.AppliesTo>) ((a, v) => a.AppliesTo = v));

    public RuleConditionDescriptor Operator(ConditionOperator @operator) => this.Assign<ConditionOperator>(@operator, (Action<IRuleCondition, ConditionOperator>) ((a, v) => a.Operator = v));

    public RuleConditionDescriptor Value(double value) => this.Assign<double>(value, (Action<IRuleCondition, double>) ((a, v) => a.Value = v));
  }
}
