// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Boards.WebApi.Common.Converters.ProcessRulesModelFactory
// Assembly: Microsoft.Azure.Boards.WebApi.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: FC99C479-6852-4E74-BCA4-2660760F9D83
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Boards.WebApi.Common.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.WorkItemTracking.Process.WebApi.Models;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.Rules;
using System;
using System.Collections.Generic;

namespace Microsoft.Azure.Boards.WebApi.Common.Converters
{
  public static class ProcessRulesModelFactory
  {
    public static IDictionary<Guid, ProcessRule> CreateIdToProcessRuleDictionary(
      IVssRequestContext requestContext,
      Guid processId,
      string witRefName,
      IEnumerable<WorkItemFieldRule> allFieldRules)
    {
      ProcessRuleModelDictionary processRuleDictionary = new ProcessRuleModelDictionary();
      foreach (WorkItemFieldRule allFieldRule in allFieldRules)
        processRuleDictionary.MergeWorkItemFieldRule(requestContext, processId, witRefName, allFieldRule);
      return (IDictionary<Guid, ProcessRule>) processRuleDictionary;
    }
  }
}
