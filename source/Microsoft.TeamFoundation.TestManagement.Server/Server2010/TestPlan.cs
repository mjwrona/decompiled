// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server2010.TestPlan
// Assembly: Microsoft.TeamFoundation.TestManagement.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F9B71993-88CC-4B0D-89B6-4ADDEEAB3DE1
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.TestManagement.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.ComponentModel;
using System.Globalization;
using System.Xml.Serialization;

namespace Microsoft.TeamFoundation.TestManagement.Server2010
{
  [ClassVisibility(ClientVisibility.Internal, ClientVisibility.Internal)]
  public class TestPlan
  {
    private string m_name;
    private string m_areaPath;
    private string m_iteration;
    private int m_revision;
    private string m_buildDefinition;
    private string m_buildQuality;
    private string m_buildUri;

    [XmlAttribute]
    [ClientProperty(ClientVisibility.Internal, PropertyName = "Id")]
    public int PlanId { get; set; }

    [XmlAttribute]
    [ClientProperty(ClientVisibility.Internal)]
    public string Name
    {
      get => this.m_name;
      set => this.m_name = value;
    }

    [XmlElement]
    [ClientProperty(ClientVisibility.Internal)]
    public string Description { get; set; }

    [XmlAttribute]
    [ClientProperty(ClientVisibility.Internal)]
    public Guid Owner { get; set; }

    [XmlAttribute]
    [ClientProperty(ClientVisibility.Internal)]
    public byte State { get; set; }

    [XmlAttribute(DataType = "date")]
    [ClientProperty(ClientVisibility.Internal)]
    public DateTime StartDate { get; set; }

    [XmlAttribute(DataType = "date")]
    [ClientProperty(ClientVisibility.Internal)]
    public DateTime EndDate { get; set; }

    [XmlAttribute]
    [ClientProperty(ClientVisibility.Internal)]
    public string AreaPath
    {
      get => this.m_areaPath;
      set => this.m_areaPath = value;
    }

    [XmlAttribute]
    [ClientProperty(ClientVisibility.Internal)]
    public string Iteration
    {
      get => this.m_iteration;
      set => this.m_iteration = value;
    }

    [XmlAttribute]
    [ClientProperty(ClientVisibility.Internal)]
    [DefaultValue(0)]
    public int TestSettingsId { get; set; }

    [XmlAttribute]
    [ClientProperty(ClientVisibility.Internal)]
    [DefaultValue(0)]
    public int AutomatedTestSettingsId { get; set; }

    [XmlAttribute]
    [ClientProperty(ClientVisibility.Internal)]
    public Guid ManualTestEnvironmentId { get; set; }

    [XmlAttribute]
    [ClientProperty(ClientVisibility.Internal)]
    public Guid AutomatedTestEnvironmentId { get; set; }

    [XmlAttribute]
    [ClientProperty(ClientVisibility.Internal)]
    public Guid LastUpdatedBy { get; set; }

    [XmlAttribute]
    [DefaultValue(typeof (DateTime), "1-1-0001")]
    [ClientProperty(ClientVisibility.Internal)]
    public DateTime LastUpdated { get; set; }

    [XmlAttribute]
    [ClientProperty(ClientVisibility.Internal)]
    public int RootSuiteId { get; set; }

    [XmlAttribute]
    [ClientProperty(ClientVisibility.Internal)]
    public int Revision
    {
      get => this.m_revision;
      set => this.m_revision = value;
    }

    [XmlAttribute]
    [ClientProperty(ClientVisibility.Internal)]
    public string BuildDefinition
    {
      get => this.m_buildDefinition;
      set => this.m_buildDefinition = value;
    }

    [XmlAttribute]
    [ClientProperty(ClientVisibility.Internal)]
    public string BuildQuality
    {
      get => this.m_buildQuality;
      set => this.m_buildQuality = value;
    }

    [XmlAttribute]
    [ClientProperty(ClientVisibility.Internal)]
    public string BuildUri
    {
      get => this.m_buildUri;
      set => this.m_buildUri = value;
    }

    [XmlAttribute]
    [ClientProperty(ClientVisibility.Internal)]
    public string PreviousBuildUri { get; set; }

    [XmlAttribute]
    [ClientProperty(ClientVisibility.Internal)]
    public DateTime BuildTakenDate { get; set; }

    public override string ToString() => string.Format((IFormatProvider) CultureInfo.InvariantCulture, "TestPlan Id={0} Name={1} Description={2}", (object) this.PlanId, (object) this.Name, (object) this.Description);
  }
}
