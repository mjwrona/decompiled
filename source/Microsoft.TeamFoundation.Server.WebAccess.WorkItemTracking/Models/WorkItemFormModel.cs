// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Models.WorkItemFormModel
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 74AD14A4-225D-46D2-B154-945941A2D167
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.dll

using System.Collections.Generic;

namespace Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Models
{
  public class WorkItemFormModel
  {
    public string SourceView { get; set; }

    public string Action { get; set; }

    public int Id { get; set; }

    public IDictionary<string, object> InitialValues { get; set; }

    public string WorkItemType { get; set; }
  }
}
