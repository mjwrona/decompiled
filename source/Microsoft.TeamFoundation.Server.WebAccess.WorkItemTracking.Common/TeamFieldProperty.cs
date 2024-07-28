// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.TeamFieldProperty
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8CB058E6-FA40-46B0-B867-53112DFAAB81
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.dll

namespace Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common
{
  public class TeamFieldProperty
  {
    public string TeamName { get; set; }

    public bool IncludeChildren { get; set; }

    public bool ParentIncludeChildren { get; set; }

    public TeamFieldProperty(string teamName, bool includeChildren, bool parentIncludeChildren = false)
    {
      this.TeamName = teamName;
      this.IncludeChildren = includeChildren;
      this.ParentIncludeChildren = parentIncludeChildren;
    }
  }
}
