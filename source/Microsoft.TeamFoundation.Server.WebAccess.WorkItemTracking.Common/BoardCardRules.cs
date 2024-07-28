// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.BoardCardRules
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8CB058E6-FA40-46B0-B867-53112DFAAB81
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.dll

using Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.DataAccess;
using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common
{
  public class BoardCardRules
  {
    public string Scope { get; set; }

    public Guid ScopeId { get; set; }

    public DateTime RevisedDate { get; set; }

    public List<RuleAttributeRow> Attributes { get; set; }

    public List<BoardCardRuleRow> Rules { get; set; }

    public BoardCardRules()
    {
      this.Rules = new List<BoardCardRuleRow>();
      this.Attributes = new List<RuleAttributeRow>();
    }
  }
}
