// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.AutomationRules.WorkItemAutomationRule
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

using Microsoft.TeamFoundation.WorkItemTracking.Server.Common;
using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server.AutomationRules
{
  public class WorkItemAutomationRule : WorkItemSecuredObject
  {
    public WorkItemAutomationRule()
    {
    }

    public WorkItemAutomationRule(
      Guid teamId,
      string backlogLevelId,
      string witName,
      IDictionary<string, bool> rulesStates)
    {
      this.TeamId = teamId;
      this.BacklogLevelId = backlogLevelId;
      this.WorkItemTypeName = witName;
      this.RulesStates = rulesStates;
    }

    public Guid TeamId { get; set; }

    public string BacklogLevelId { get; set; }

    public string WorkItemTypeName { get; set; }

    public IDictionary<string, bool> RulesStates { get; set; }
  }
}
