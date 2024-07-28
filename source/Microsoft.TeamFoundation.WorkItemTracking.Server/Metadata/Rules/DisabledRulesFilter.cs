// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.Rules.DisabledRulesFilter
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.Rules
{
  public static class DisabledRulesFilter
  {
    public static List<WorkItemFieldRule> RunFilter(
      IReadOnlyCollection<WorkItemFieldRule> rulesList,
      HashSet<Guid> disabledRules)
    {
      if (rulesList == null)
        return (List<WorkItemFieldRule>) null;
      if (disabledRules == null)
        return new List<WorkItemFieldRule>((IEnumerable<WorkItemFieldRule>) rulesList);
      List<WorkItemFieldRule> workItemFieldRuleList = new List<WorkItemFieldRule>();
      foreach (WorkItemFieldRule rules in (IEnumerable<WorkItemFieldRule>) rulesList)
      {
        if (rules.ContainsAny(disabledRules))
        {
          WorkItemFieldRule workItemFieldRule = rules.CanEditRule() ? rules : rules.Clone() as WorkItemFieldRule;
          workItemFieldRule.RemoveRules((ISet<Guid>) disabledRules);
          workItemFieldRuleList.Add(workItemFieldRule);
        }
        else
          workItemFieldRuleList.Add(rules);
      }
      return workItemFieldRuleList;
    }
  }
}
