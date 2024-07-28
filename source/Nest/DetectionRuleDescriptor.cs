// Decompiled with JetBrains decompiler
// Type: Nest.DetectionRuleDescriptor
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;
using System.Collections.Generic;

namespace Nest
{
  public class DetectionRuleDescriptor : 
    DescriptorBase<DetectionRuleDescriptor, IDetectionRule>,
    IDetectionRule
  {
    IEnumerable<RuleAction> IDetectionRule.Actions { get; set; }

    IEnumerable<IRuleCondition> IDetectionRule.Conditions { get; set; }

    IReadOnlyDictionary<Field, FilterRef> IDetectionRule.Scope { get; set; }

    public DetectionRuleDescriptor Actions(IEnumerable<RuleAction> actions) => this.Assign<IEnumerable<RuleAction>>(actions, (Action<IDetectionRule, IEnumerable<RuleAction>>) ((a, v) => a.Actions = v));

    public DetectionRuleDescriptor Actions(params RuleAction[] actions) => this.Assign<RuleAction[]>(actions, (Action<IDetectionRule, RuleAction[]>) ((a, v) => a.Actions = (IEnumerable<RuleAction>) v));

    public DetectionRuleDescriptor Scope<T>(
      Func<ScopeDescriptor<T>, IPromise<IReadOnlyDictionary<Field, FilterRef>>> selector)
      where T : class
    {
      return this.Assign<IReadOnlyDictionary<Field, FilterRef>>(selector(new ScopeDescriptor<T>()).Value, (Action<IDetectionRule, IReadOnlyDictionary<Field, FilterRef>>) ((a, v) => a.Scope = v));
    }

    public DetectionRuleDescriptor Conditions(
      Func<RuleConditionsDescriptor, IPromise<List<IRuleCondition>>> selector)
    {
      return this.Assign<List<IRuleCondition>>(selector != null ? selector(new RuleConditionsDescriptor())?.Value : (List<IRuleCondition>) null, (Action<IDetectionRule, List<IRuleCondition>>) ((a, v) => a.Conditions = (IEnumerable<IRuleCondition>) v));
    }
  }
}
