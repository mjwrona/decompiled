// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server2010.TestRun
// Assembly: Microsoft.TeamFoundation.TestManagement.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F9B71993-88CC-4B0D-89B6-4ADDEEAB3DE1
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.TestManagement.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.TestManagement.Client;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Xml.Serialization;

namespace Microsoft.TeamFoundation.TestManagement.Server2010
{
  [ClassVisibility(ClientVisibility.Internal, ClientVisibility.Internal)]
  public class TestRun
  {
    private int m_testRunId;
    private string m_title;
    private byte m_state;
    private string m_buildUri;
    private string m_dropLocation;
    private string m_buildNumber;
    private string m_buildPlatform;
    private string m_buildFlavor;
    private DateTime m_creationDate;
    private DateTime m_startDate;
    private DateTime m_completeDate;
    private byte m_postProcessState;
    private DateTime m_dueDate;
    private string m_iteration;
    private string m_controller;
    private string m_teamProject;
    private int m_testPlanId;
    private int m_testMessageLogId;
    private int m_revision;

    [XmlAttribute]
    [ClientProperty(ClientVisibility.Internal, PropertyName = "Id")]
    public int TestRunId
    {
      get => this.m_testRunId;
      set => this.m_testRunId = value;
    }

    [XmlAttribute]
    [ClientProperty(ClientVisibility.Internal)]
    public string Title
    {
      get => this.m_title;
      set => this.m_title = value;
    }

    [XmlAttribute]
    [ClientProperty(ClientVisibility.Internal)]
    public Guid Owner { get; set; }

    [XmlAttribute]
    [ClientProperty(ClientVisibility.Internal)]
    public byte State
    {
      get => this.m_state;
      set => this.m_state = value;
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
    public string DropLocation
    {
      get => this.m_dropLocation;
      set => this.m_dropLocation = value;
    }

    [XmlAttribute]
    [ClientProperty(ClientVisibility.Internal)]
    public string BuildNumber
    {
      get => this.m_buildNumber;
      set => this.m_buildNumber = value;
    }

    [XmlAttribute]
    [ClientProperty(ClientVisibility.Internal)]
    public string BuildPlatform
    {
      get => this.m_buildPlatform;
      set => this.m_buildPlatform = value;
    }

    [XmlAttribute]
    [ClientProperty(ClientVisibility.Internal)]
    public string BuildFlavor
    {
      get => this.m_buildFlavor;
      set => this.m_buildFlavor = value;
    }

    [XmlAttribute]
    [ClientProperty(ClientVisibility.Internal)]
    public DateTime CreationDate
    {
      get => this.m_creationDate;
      set => this.m_creationDate = value;
    }

    [XmlAttribute]
    [DefaultValue(typeof (DateTime), "1-1-0001")]
    [ClientProperty(ClientVisibility.Internal)]
    public DateTime LastUpdated { get; set; }

    [XmlAttribute]
    [ClientProperty(ClientVisibility.Internal)]
    public DateTime StartDate
    {
      get => this.m_startDate;
      set => this.m_startDate = value;
    }

    [XmlAttribute]
    [ClientProperty(ClientVisibility.Internal)]
    public DateTime CompleteDate
    {
      get => this.m_completeDate;
      set => this.m_completeDate = value;
    }

    [XmlAttribute]
    [ClientProperty(ClientVisibility.Internal)]
    public byte PostProcessState
    {
      get => this.m_postProcessState;
      set => this.m_postProcessState = value;
    }

    [XmlAttribute]
    [ClientProperty(ClientVisibility.Internal)]
    public DateTime DueDate
    {
      get => this.m_dueDate;
      set => this.m_dueDate = value;
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
    public string Controller
    {
      get => this.m_controller;
      set => this.m_controller = value;
    }

    [XmlAttribute]
    [ClientProperty(ClientVisibility.Internal)]
    public string TeamProject
    {
      get => this.m_teamProject;
      set => this.m_teamProject = value;
    }

    [XmlAttribute]
    [ClientProperty(ClientVisibility.Internal)]
    public int TestPlanId
    {
      get => this.m_testPlanId;
      set => this.m_testPlanId = value;
    }

    [XmlAttribute]
    [ClientProperty(ClientVisibility.Internal)]
    public int BuildConfigurationId { get; set; }

    [XmlAttribute]
    [ClientProperty(ClientVisibility.Internal)]
    [DefaultValue(0)]
    public int TestSettingsId { get; set; }

    [XmlAttribute]
    [ClientProperty(ClientVisibility.Internal)]
    [DefaultValue(0)]
    public int PublicTestSettingsId { get; set; }

    [XmlAttribute]
    [ClientProperty(ClientVisibility.Internal)]
    public Guid TestEnvironmentId { get; set; }

    [XmlAttribute]
    [ClientProperty(ClientVisibility.Internal)]
    public int Revision
    {
      get => this.m_revision;
      set => this.m_revision = value;
    }

    [XmlAttribute]
    [ClientProperty(ClientVisibility.Internal)]
    public int TestMessageLogId
    {
      get => this.m_testMessageLogId;
      set => this.m_testMessageLogId = value;
    }

    [XmlAttribute]
    [ClientProperty(ClientVisibility.Internal)]
    public string LegacySharePath { get; set; }

    [XmlAttribute]
    [ClientProperty(ClientVisibility.Internal)]
    public Guid LastUpdatedBy { get; set; }

    [XmlElement]
    [ClientProperty(ClientVisibility.Internal)]
    public string ErrorMessage { get; set; }

    [XmlAttribute]
    [ClientProperty(ClientVisibility.Internal)]
    public byte Type { get; set; }

    [XmlAttribute]
    [ClientProperty(ClientVisibility.Internal)]
    public bool IsAutomated { get; set; }

    [XmlAttribute]
    [ClientProperty(ClientVisibility.Internal)]
    public int Version { get; set; }

    [XmlAttribute]
    [ClientProperty(ClientVisibility.Internal)]
    public bool IsBvt { get; set; }

    [XmlAttribute]
    [ClientProperty(ClientVisibility.Internal)]
    public string Comment { get; set; }

    public override string ToString() => string.Format((IFormatProvider) CultureInfo.InvariantCulture, "TestRun Id={0} Title={1} Build={2}", (object) this.m_testRunId, (object) this.m_title, (object) this.m_buildUri);

    internal static List<TestRun> FilterNotOfType(IEnumerable<TestRun> runs, TestRunType type) => runs.Where<TestRun>((Func<TestRun, bool>) (r => ((int) r.Type & (int) (byte) type) == 0)).ToList<TestRun>();
  }
}
