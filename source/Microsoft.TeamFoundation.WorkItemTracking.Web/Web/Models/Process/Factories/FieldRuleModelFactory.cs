// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Web.Models.Process.Factories.FieldRuleModelFactory
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Web, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: CCDEBCB9-DB6B-41A2-8797-78C2455AE321
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Web.dll

using Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.Rules;
using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.WorkItemTracking.Web.Models.Process.Factories
{
  internal static class FieldRuleModelFactory
  {
    public static IDictionary<Guid, FieldRuleModel> CreateIdToFieldRuleModelDictionary(
      IEnumerable<WorkItemFieldRule> allFieldRules)
    {
      FieldRuleModelDictionary ruleModelDictionary = new FieldRuleModelDictionary();
      foreach (WorkItemFieldRule allFieldRule in allFieldRules)
        ruleModelDictionary.MergeWorkItemFieldRule(allFieldRule);
      return (IDictionary<Guid, FieldRuleModel>) ruleModelDictionary;
    }
  }
}
