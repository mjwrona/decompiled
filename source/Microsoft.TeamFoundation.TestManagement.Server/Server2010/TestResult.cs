// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server2010.TestResult
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

    [ClientProperty(ClientVisibility.Internal)]
    public TestCaseResultIdentifier Id
    {
      get => this.m_id;
      set => this.m_id = value;
    }

    [XmlAttribute]
    [DefaultValue(typeof (DateTime), "1-1-0001")]
    [ClientProperty(ClientVisibility.Internal)]
    public DateTime CreationDate
    {
      get => this.m_creationDate;
      set => this.m_creationDate = value;
    }

    [XmlAttribute]
    [DefaultValue(0)]
    [ClientProperty(ClientVisibility.Internal)]
    public byte Outcome
    {
      get => this.m_outcome;
      set => this.m_outcome = value;
    }

    [XmlElement]
    [ClientProperty(ClientVisibility.Internal)]
    public string ErrorMessage
    {
      get => this.m_errorMessage;
      set => this.m_errorMessage = value;
    }

    [XmlElement]
    [ClientProperty(ClientVisibility.Internal)]
    public string Comment
    {
      get => this.m_comment;
      set => this.m_comment = value;
    }

    [XmlAttribute]
    [ClientProperty(ClientVisibility.Internal)]
    public Guid LastUpdatedBy { get; set; }

    [XmlAttribute]
    [DefaultValue(typeof (DateTime), "1-1-0001")]
    [ClientProperty(ClientVisibility.Internal)]
    public DateTime LastUpdated { get; set; }

    [XmlAttribute]
    [DefaultValue(typeof (DateTime), "1-1-0001")]
    [ClientProperty(ClientVisibility.Internal)]
    public DateTime DateStarted
    {
      get => this.m_dateStarted;
      set => this.m_dateStarted = value;
    }

    [XmlAttribute]
    [DefaultValue(typeof (DateTime), "1-1-0001")]
    [ClientProperty(ClientVisibility.Internal)]
    public DateTime DateCompleted
    {
      get => this.m_dateCompleted;
      set => this.m_dateCompleted = value;
    }

    [XmlAttribute]
    [DefaultValue(0)]
    [ClientProperty(ClientVisibility.Internal)]
    public long Duration
    {
      get => this.m_duration;
      set => this.m_duration = value;
    }
  }
}
