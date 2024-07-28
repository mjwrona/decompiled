// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.Rules.WorkItemFieldRuleExtensions
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

using System.Collections.Generic;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.Rules
{
  public static class WorkItemFieldRuleExtensions
  {
    public static void MergeIntoDictionary(
      this WorkItemFieldRule workItemFieldRule,
      IDictionary<int, WorkItemFieldRule> fieldRulesDictionary)
    {
      WorkItemFieldRule workItemFieldRule1;
      if (fieldRulesDictionary.TryGetValue(workItemFieldRule.FieldId, out workItemFieldRule1))
      {
        WorkItemRule[] subRules = workItemFieldRule.SubRules;
        if (subRules.Length != 0)
        {
          workItemFieldRule1 = workItemFieldRule1.Clone() as WorkItemFieldRule;
          fieldRulesDictionary[workItemFieldRule.FieldId] = workItemFieldRule1;
        }
        foreach (WorkItemRule rule in subRules)
          workItemFieldRule1.AddRule<WorkItemRule>(rule);
      }
      else
        fieldRulesDictionary[workItemFieldRule.FieldId] = workItemFieldRule;
    }

    public static void MergeIntoDictionary(
      this IEnumerable<WorkItemFieldRule> workItemFieldRules,
      IDictionary<int, WorkItemFieldRule> fieldRulesDictionary)
    {
      foreach (WorkItemFieldRule workItemFieldRule in workItemFieldRules)
        workItemFieldRule.MergeIntoDictionary(fieldRulesDictionary);
    }
  }
}
