// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.BacklogCategoryConfiguration
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8CB058E6-FA40-46B0-B867-53112DFAAB81
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.dll

using System.Xml.Serialization;

namespace Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common
{
  public class BacklogCategoryConfiguration : CategoryConfiguration
  {
    internal const int WorkItemCountLimitDefault = 1000;
    internal const int UnlimitedWorkItemCount = -1;

    public BacklogCategoryConfiguration() => this.WorkItemCountLimit = 1000;

    [XmlAttribute(AttributeName = "parent")]
    public string ParentCategoryReferenceName { get; set; }

    [XmlAttribute("workItemCountLimit")]
    public int WorkItemCountLimit { get; set; }

    public AddPanelConfiguration AddPanel { get; set; }

    public Column[] Columns { get; set; }
  }
}
