// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.IdentityRuleTransformer
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

using Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.Rules;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata
{
  public abstract class IdentityRuleTransformer
  {
    protected virtual IReadOnlyCollection<ProcessTypelet> ReplaceRules(
      IEnumerable<ProcessTypelet> typelets,
      Dictionary<string, IEnumerable<WorkItemFieldRule>> fieldRulesMap)
    {
      List<ProcessTypelet> processTypeletList = new List<ProcessTypelet>();
      foreach (ProcessTypelet typelet in typelets)
      {
        if (fieldRulesMap.ContainsKey(typelet.ReferenceName))
          processTypeletList.Add((ProcessTypelet) ProcessWorkItemType.Create((ProcessWorkItemType) typelet, fieldRulesMap[typelet.ReferenceName]));
        else
          processTypeletList.Add(typelet);
      }
      return (IReadOnlyCollection<ProcessTypelet>) processTypeletList;
    }
  }
}
