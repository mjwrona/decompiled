// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.Promote.PromoteProjectInfo
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8CB058E6-FA40-46B0-B867-53112DFAAB81
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.dll

using System;
using System.Xml.Serialization;

namespace Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.Promote
{
  public class PromoteProjectInfo
  {
    public PromoteProjectInfo()
    {
    }

    public PromoteProjectInfo(Guid id, ProjectPromoteState state)
    {
      this.Id = id;
      this.State = state;
    }

    [XmlAttribute("ProjectId")]
    public Guid Id { get; set; }

    [XmlAttribute("State")]
    public ProjectPromoteState State { get; set; }

    [XmlAttribute("CompletedSteps")]
    public int CompletedSteps { get; set; }
  }
}
