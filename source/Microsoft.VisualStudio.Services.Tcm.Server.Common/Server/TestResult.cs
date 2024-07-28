// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.TestResult
// Assembly: Microsoft.VisualStudio.Services.Tcm.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7631C286-897C-44D1-A133-A0BB6CC047F3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Tcm.Server.Common.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.TestManagement.Client;
using System;
using System.ComponentModel;
using System.Xml.Serialization;

namespace Microsoft.TeamFoundation.TestManagement.Server
{
  [ClassVisibility(ClientVisibility.Internal)]
  public abstract class TestResult
  {
    private TestCaseResultIdentifier m_id = new TestCaseResultIdentifier();
    private DateTime m_creationDate;
    private byte m_outcome;
    private string m_errorMessage;
    private string m_comment;
    private DateTime m_dateStarted;
    private DateTime m_dateCompleted;
    private long m_duration;

    [ClientProperty(ClientVisibility.Private, UseClientDefinedProperty = true)]
    public TestCaseResultIdentifier Id
    {
      get => this.m_id;
      set => this.m_id = value;
    }

    [XmlIgnore]
    [QueryMapping]
    public int TestRunId
    {
      get => this.m_id.TestRunId;
      set => this.m_id.TestRunId = value;
    }

    [XmlIgnore]
    [QueryMapping]
    public int TestResultId
    {
      get => this.m_id.TestResultId;
      set => this.m_id.TestResultId = value;
    }

    [XmlAttribute]
    [DefaultValue(typeof (DateTime), "1-1-0001")]
    [ClientProperty(ClientVisibility.Private)]
    [QueryMapping]
    public DateTime CreationDate
    {
      get => this.m_creationDate;
      set => this.m_creationDate = value;
    }

    [XmlAttribute]
    [DefaultValue(0)]
    [ClientProperty(ClientVisibility.Private)]
    [QueryMapping(EnumType = typeof (TestOutcome))]
    public byte Outcome
    {
      get => this.m_outcome;
      set => this.m_outcome = value;
    }

    [XmlElement]
    [ClientProperty(ClientVisibility.Private)]
    [QueryMapping]
    public string ErrorMessage
    {
      get => this.m_errorMessage;
      set => this.m_errorMessage = value;
    }

    [XmlElement]
    [ClientProperty(ClientVisibility.Private)]
    [QueryMapping]
    public string Comment
    {
      get => this.m_comment;
      set => this.m_comment = value;
    }

    [XmlAttribute]
    [ClientProperty(ClientVisibility.Private)]
    [QueryMapping]
    public Guid LastUpdatedBy { get; set; }

    [XmlAttribute]
    [ClientProperty(ClientVisibility.Private)]
    public string LastUpdatedByName { get; set; }

    [XmlAttribute]
    [DefaultValue(typeof (DateTime), "1-1-0001")]
    [ClientProperty(ClientVisibility.Private)]
    [QueryMapping]
    public DateTime LastUpdated { get; set; }

    [XmlAttribute]
    [DefaultValue(typeof (DateTime), "1-1-0001")]
    [ClientProperty(ClientVisibility.Private)]
    [QueryMapping]
    public DateTime DateStarted
    {
      get => this.m_dateStarted;
      set => this.m_dateStarted = value;
    }

    [XmlAttribute]
    [DefaultValue(typeof (DateTime), "1-1-0001")]
    [ClientProperty(ClientVisibility.Private)]
    [QueryMapping]
    public DateTime DateCompleted
    {
      get => this.m_dateCompleted;
      set => this.m_dateCompleted = value;
    }

    [XmlAttribute]
    [DefaultValue(0)]
    [ClientProperty(ClientVisibility.Private)]
    [QueryMapping]
    public long Duration
    {
      get => this.m_duration;
      set => this.m_duration = value;
    }

    internal static byte ToPreDev12QU2Outcome(byte outcome)
    {
      if (outcome == (byte) 11)
        return 2;
      return outcome == (byte) 12 ? (byte) 4 : outcome;
    }

    internal static byte FromPreDev12QU2Outcome(byte outcome) => outcome == (byte) 4 ? (byte) 12 : outcome;
  }
}
