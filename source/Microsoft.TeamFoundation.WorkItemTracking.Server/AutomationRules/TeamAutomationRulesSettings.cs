// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.AutomationRules.TeamAutomationRulesSettings
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server.AutomationRules
{
  public class TeamAutomationRulesSettings
  {
    public TeamAutomationRulesSettings(
      Guid teamId,
      string backlogLevelId,
      IDictionary<string, bool> rulesStates)
    {
      this.TeamId = teamId;
      this.BacklogLevelId = backlogLevelId;
      this.RulesStates = rulesStates;
    }

    public Guid TeamId { get; set; }

    public string BacklogLevelId { get; set; }

    public IDictionary<string, bool> RulesStates { get; set; }
  }
}
