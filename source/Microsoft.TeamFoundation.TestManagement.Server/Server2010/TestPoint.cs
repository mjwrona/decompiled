// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server2010.TestPoint
// Assembly: Microsoft.TeamFoundation.TestManagement.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F9B71993-88CC-4B0D-89B6-4ADDEEAB3DE1
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.TestManagement.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.ComponentModel;
using System.Xml.Serialization;

namespace Microsoft.TeamFoundation.TestManagement.Server2010
{
  [ClassVisibility(ClientVisibility.Internal, ClientVisibility.Internal)]
  public class TestPoint
  {
    [XmlAttribute]
    [ClientProperty(ClientVisibility.Internal, PropertyName = "Id")]
    public int PointId { get; set; }

    [XmlAttribute]
    [ClientProperty(ClientVisibility.Internal)]
    public byte State { get; set; }

    [XmlAttribute]
    public byte FailureType { get; set; }

    [XmlAttribute]
    [ClientProperty(ClientVisibility.Internal)]
    public int PlanId { get; set; }

    [XmlAttribute]
    [ClientProperty(ClientVisibility.Internal)]
    public int SuiteId { get; set; }

    [XmlAttribute]
    [ClientProperty(ClientVisibility.Internal)]
    public int ConfigurationId { get; set; }

    [XmlAttribute]
    [ClientProperty(ClientVisibility.Internal)]
    public string ConfigurationName { get; set; }

    [XmlAttribute]
    [ClientProperty(ClientVisibility.Internal)]
    public int LastTestRunId { get; set; }

    [XmlAttribute]
    [ClientProperty(ClientVisibility.Internal)]
    public int LastTestResultId { get; set; }

    [XmlAttribute]
    [ClientProperty(ClientVisibility.Internal)]
    public byte LastResultState { get; set; }

    [XmlAttribute]
    [ClientProperty(ClientVisibility.Internal)]
    public byte LastResultOutcome { get; set; }

    [XmlAttribute]
    [ClientProperty(ClientVisibility.Internal)]
    public int LastResolutionStateId { get; set; }

    [XmlAttribute]
    [ClientProperty(ClientVisibility.Internal)]
    public int TestCaseId { get; set; }

    [XmlAttribute]
    [ClientProperty(ClientVisibility.Internal)]
    public string Comment { get; set; }

    [XmlAttribute]
    [ClientProperty(ClientVisibility.Internal)]
    public Guid LastUpdatedBy { get; set; }

    [XmlAttribute]
    [DefaultValue(typeof (DateTime), "1-1-0001")]
    [ClientProperty(ClientVisibility.Internal)]
    public DateTime LastUpdated { get; set; }

    [XmlAttribute]
    [ClientProperty(ClientVisibility.Internal)]
    public Guid AssignedTo { get; set; }

    [XmlAttribute]
    [ClientProperty(ClientVisibility.Internal)]
    [DefaultValue(0)]
    public int Revision { get; set; }
  }
}
