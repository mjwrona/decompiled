// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.Promote.TeamProjectPromoteJobData
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8CB058E6-FA40-46B0-B867-53112DFAAB81
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.dll

using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.Promote
{
  public class TeamProjectPromoteJobData
  {
    private const int c_retries = 2;

    public TeamProjectPromoteJobData()
      : this(Guid.Empty)
    {
    }

    public TeamProjectPromoteJobData(
      Guid processTemplateTypeId,
      int retries = 2,
      bool isXmlToInheritedPromote = false)
    {
      this.ProcessTemplateTypeId = processTemplateTypeId;
      this.isXmlToInheritedPromote = isXmlToInheritedPromote;
      this.RemainingRetries = retries;
    }

    [XmlAttribute("processTemplateTypeId")]
    public Guid ProcessTemplateTypeId { get; set; }

    public List<PromoteProjectInfo> Projects { get; set; }

    [XmlAttribute("isXmlToInheritedPromote")]
    public bool isXmlToInheritedPromote { get; set; }

    [XmlAttribute("remainingRetries")]
    public int RemainingRetries { get; set; }

    public bool IsSuccessful()
    {
      foreach (PromoteProjectInfo project in this.Projects)
      {
        if (project.State == ProjectPromoteState.Failed || project.State == ProjectPromoteState.NotProcessed)
          return false;
      }
      return true;
    }
  }
}
