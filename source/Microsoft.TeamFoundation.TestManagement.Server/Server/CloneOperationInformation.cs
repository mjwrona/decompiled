// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.CloneOperationInformation
// Assembly: Microsoft.TeamFoundation.TestManagement.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F9B71993-88CC-4B0D-89B6-4ADDEEAB3DE1
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.TestManagement.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace Microsoft.TeamFoundation.TestManagement.Server
{
  [ClassVisibility(ClientVisibility.Public)]
  public class CloneOperationInformation
  {
    private Dictionary<string, string> m_editFieldDetails;

    public int OpId { get; set; }

    public DateTime CreationDate { get; set; }

    public DateTime CompletionDate { get; set; }

    public CloneOperationState State { get; set; }

    public string Message { get; set; }

    public int TotalTestCaseCount { get; set; }

    public int ClonedTestCaseCount { get; set; }

    public int ClonedSharedStepCount { get; set; }

    public int TotalRequirementsCount { get; set; }

    public int ClonedRequirementsCount { get; set; }

    public ResultObjectType ResultObjectType { get; set; }

    public string TargetPlanName { get; set; }

    public int TargetPlanId { get; set; }

    public string SourcePlanName { get; set; }

    public int SourcePlanId { get; set; }

    public string ResultObjectName { get; set; }

    public int ResultObjectId { get; set; }

    public string SourceObjectName { get; set; }

    public int SourceObjectId { get; set; }

    [ClientProperty(ClientVisibility.Private)]
    public Guid TeamFoundationUserId { get; set; }

    [XmlAttribute]
    [ClientProperty(ClientVisibility.Private)]
    public string TeamFoundationUserName { get; set; }

    public string SourceProjectName { get; set; }

    public string DestinationProjectName { get; set; }

    [XmlIgnore]
    internal string DestinationWorkItemType { get; set; }

    [XmlIgnore]
    internal string LinkComment { get; set; }

    [XmlIgnore]
    internal int EditFieldId { get; set; }

    [XmlIgnore]
    internal string EditFieldValue { get; set; }

    [XmlIgnore]
    internal Dictionary<string, string> EditFieldDetails
    {
      get
      {
        if (this.m_editFieldDetails == null)
          this.m_editFieldDetails = new Dictionary<string, string>();
        return this.m_editFieldDetails;
      }
      set => this.m_editFieldDetails = value;
    }
  }
}
