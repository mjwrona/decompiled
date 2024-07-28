// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server2010.TestCaseResult
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
  [SoapInclude(typeof (TestResult))]
  public class TestCaseResult : TestResult
  {
    private byte m_state;
    private int m_resolutionStateId;
    private string m_computerName;
    private byte m_priority = byte.MaxValue;
    private string m_testCaseTitle;
    private string m_testCaseArea;
    private string m_testCaseAreaUri;
    private int m_testCaseRevision;
    private int m_afnStripId;
    private int m_resetCount;
    private int m_testCaseId;
    private int m_configurationId;
    private int m_testPointId = int.MinValue;
    private byte m_failureType;
    private string m_automatedTestName;
    private string m_automatedTestStorage;
    private string m_automatedTestType;
    private string m_automatedTestTypeId;
    private string m_automatedTestId;
    private int m_revision;
    private const byte UnspecifiedPriority = 255;

    [XmlAttribute]
    [DefaultValue(0)]
    [ClientProperty(ClientVisibility.Internal)]
    public int TestCaseId
    {
      get => this.m_testCaseId;
      set => this.m_testCaseId = value;
    }

    [XmlAttribute]
    [DefaultValue(0)]
    [ClientProperty(ClientVisibility.Internal)]
    public int ConfigurationId
    {
      get => this.m_configurationId;
      set => this.m_configurationId = value;
    }

    [XmlAttribute]
    [ClientProperty(ClientVisibility.Internal)]
    public string ConfigurationName { get; set; }

    [XmlAttribute]
    [DefaultValue(-2147483648)]
    [ClientProperty(ClientVisibility.Internal)]
    public int TestPointId
    {
      get => this.m_testPointId;
      set => this.m_testPointId = value;
    }

    [XmlAttribute]
    [DefaultValue(0)]
    [ClientProperty(ClientVisibility.Internal)]
    public byte State
    {
      get => this.m_state;
      set => this.m_state = value;
    }

    [XmlAttribute]
    [DefaultValue(0)]
    [ClientProperty(ClientVisibility.Internal)]
    public byte FailureType
    {
      get => this.m_failureType;
      set => this.m_failureType = value;
    }

    [XmlAttribute]
    [DefaultValue(0)]
    [ClientProperty(ClientVisibility.Internal)]
    public int ResolutionStateId
    {
      get => this.m_resolutionStateId;
      set => this.m_resolutionStateId = value;
    }

    [XmlAttribute]
    [ClientProperty(ClientVisibility.Internal)]
    public string ComputerName
    {
      get => this.m_computerName;
      set => this.m_computerName = value;
    }

    [XmlAttribute]
    [ClientProperty(ClientVisibility.Internal)]
    public Guid Owner { get; set; }

    [XmlAttribute]
    [ClientProperty(ClientVisibility.Internal)]
    public Guid RunBy { get; set; }

    [XmlAttribute]
    [DefaultValue(255)]
    [ClientProperty(ClientVisibility.Internal)]
    public byte Priority
    {
      get => this.m_priority;
      set => this.m_priority = value;
    }

    [XmlAttribute]
    [ClientProperty(ClientVisibility.Internal)]
    public string TestCaseTitle
    {
      get => this.m_testCaseTitle;
      set => this.m_testCaseTitle = value;
    }

    [XmlAttribute]
    [ClientProperty(ClientVisibility.Internal)]
    public string TestCaseArea
    {
      get => this.m_testCaseArea;
      set => this.m_testCaseArea = value;
    }

    [XmlAttribute]
    [ClientProperty(ClientVisibility.Internal)]
    public string TestCaseAreaUri
    {
      get => this.m_testCaseAreaUri;
      set => this.m_testCaseAreaUri = value;
    }

    [XmlAttribute]
    [DefaultValue(0)]
    [ClientProperty(ClientVisibility.Internal)]
    public int TestCaseRevision
    {
      get => this.m_testCaseRevision;
      set => this.m_testCaseRevision = value;
    }

    [ClientProperty(ClientVisibility.Internal)]
    public int AfnStripId
    {
      get => this.m_afnStripId;
      set => this.m_afnStripId = value;
    }

    [XmlAttribute]
    [ClientProperty(ClientVisibility.Internal)]
    public int ResetCount
    {
      get => this.m_resetCount;
      set => this.m_resetCount = value;
    }

    [XmlAttribute]
    [ClientProperty(ClientVisibility.Internal)]
    public string AutomatedTestName
    {
      get => this.m_automatedTestName;
      set => this.m_automatedTestName = value;
    }

    [XmlAttribute]
    [ClientProperty(ClientVisibility.Internal)]
    public string AutomatedTestStorage
    {
      get => this.m_automatedTestStorage;
      set => this.m_automatedTestStorage = value;
    }

    [XmlAttribute]
    [ClientProperty(ClientVisibility.Internal)]
    public string AutomatedTestType
    {
      get => this.m_automatedTestType;
      set => this.m_automatedTestType = value;
    }

    [XmlAttribute]
    [ClientProperty(ClientVisibility.Internal)]
    public string AutomatedTestTypeId
    {
      get => this.m_automatedTestTypeId;
      set => this.m_automatedTestTypeId = value;
    }

    [XmlAttribute]
    [ClientProperty(ClientVisibility.Internal)]
    public string AutomatedTestId
    {
      get => this.m_automatedTestId;
      set => this.m_automatedTestId = value;
    }

    [XmlAttribute]
    [ClientProperty(ClientVisibility.Internal)]
    public int Revision
    {
      get => this.m_revision;
      set => this.m_revision = value;
    }

    public override string ToString() => string.Format((IFormatProvider) CultureInfo.InvariantCulture, "TestCaseResult TestCaseId={0} State={1}", (object) this.TestCaseId, (object) this.State);
  }
}
