// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.DataAccess.BoardCardRuleRow
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8CB058E6-FA40-46B0-B867-53112DFAAB81
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.dll

using Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.Models;
using Newtonsoft.Json;
using System;

namespace Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.DataAccess
{
  public class BoardCardRuleRow
  {
    public BoardCardRuleRow()
    {
    }

    public BoardCardRuleRow(
      string name,
      string type,
      bool on,
      int order,
      DateTime revisedDate,
      string filter,
      FilterModel filterExpression)
    {
      this.Name = name;
      this.Type = type;
      this.IsEnabled = on;
      this.Order = order;
      this.RevisedDate = revisedDate;
      this.Filter = filter;
      if (filterExpression != null)
        this.FilterExpression = JsonConvert.SerializeObject((object) filterExpression);
      else
        this.FilterExpression = string.Empty;
    }

    public int Id { get; set; }

    public string Name { get; set; }

    public string Type { get; set; }

    public bool IsEnabled { get; set; }

    public int Order { get; set; }

    public DateTime RevisedDate { get; set; }

    public string Filter { get; set; }

    public string FilterExpression { get; set; }
  }
}
