// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.ProcessComparer.Declarations.ProcessConfigurationDeclaration
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

using Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.DataModels;
using System;
using System.Xml.Serialization;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server.ProcessComparer.Declarations
{
  [XmlRoot("ProjectProcessConfiguration")]
  [Serializable]
  public class ProcessConfigurationDeclaration
  {
    public WorkItemColor[] WorkItemColors { get; set; }

    public TypeField[] TypeFields { get; set; }

    [XmlArray("PortfolioBacklogs")]
    [XmlArrayItem("PortfolioBacklog", typeof (BacklogCategoryConfiguration))]
    public BacklogCategoryConfiguration[] PortfolioBacklogs { get; set; }

    [XmlElement("RequirementBacklog")]
    public BacklogCategoryConfiguration RequirementBacklog { get; set; }

    [XmlElement("TaskBacklog")]
    public BacklogCategoryConfiguration TaskBacklog { get; set; }

    [XmlElement("FeedbackRequestWorkItems")]
    public CategoryConfiguration FeedbackRequestWorkItems { get; set; }

    [XmlElement("FeedbackResponseWorkItems")]
    public CategoryConfiguration FeedbackResponseWorkItems { get; set; }

    [XmlElement("FeedbackWorkItems")]
    public CategoryConfiguration FeedbackWorkItems { get; set; }

    [XmlElement("TestPlanWorkItems")]
    public CategoryConfiguration TestPlanWorkItems { get; set; }

    [XmlElement("TestSuiteWorkItems")]
    public CategoryConfiguration TestSuiteWorkItems { get; set; }

    [XmlElement("BugWorkItems")]
    public CategoryConfiguration BugWorkItems { get; set; }

    [XmlElement("ReleaseWorkItems")]
    public CategoryConfiguration ReleaseWorkItems { get; set; }

    [XmlElement("ReleaseStageWorkItems")]
    public CategoryConfiguration ReleaseStageWorkItems { get; set; }

    [XmlElement("StageSignoffTaskWorkItems")]
    public CategoryConfiguration StageSignoffTaskWorkItems { get; set; }

    public DayOfWeek[] Weekends { get; set; }

    public Property[] Properties { get; set; }
  }
}
