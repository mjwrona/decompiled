// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server2010.TestResultAttachment
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
  public class TestResultAttachment
  {
    private int m_id;
    private string m_fileName;
    private string m_comment;
    private DateTime m_creationDate;
    private long m_length;
    private int m_iterationId;
    private string m_actionPath;
    private string m_attachmentType;
    private int m_testRunId;
    private int m_testResultId;
    private bool m_isComplete = true;

    [XmlAttribute]
    [ClientProperty(ClientVisibility.Internal)]
    public int Id
    {
      get => this.m_id;
      set => this.m_id = value;
    }

    [XmlAttribute]
    [ClientProperty(ClientVisibility.Internal)]
    public string FileName
    {
      get => this.m_fileName;
      set => this.m_fileName = value;
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
    public string AttachmentType
    {
      get => this.m_attachmentType;
      set => this.m_attachmentType = value;
    }

    [XmlAttribute]
    [ClientProperty(ClientVisibility.Internal)]
    public int TestRunId
    {
      get => this.m_testRunId;
      set => this.m_testRunId = value;
    }

    [XmlAttribute]
    [ClientProperty(ClientVisibility.Internal)]
    public int TestResultId
    {
      get => this.m_testResultId;
      set => this.m_testResultId = value;
    }

    [XmlAttribute]
    [ClientProperty(ClientVisibility.Internal)]
    public DateTime CreationDate
    {
      get => this.m_creationDate;
      set => this.m_creationDate = value;
    }

    [XmlAttribute]
    [ClientProperty(ClientVisibility.Internal)]
    public long Length
    {
      get => this.m_length;
      set => this.m_length = value;
    }

    [XmlAttribute]
    [DefaultValue(0)]
    [ClientProperty(ClientVisibility.Internal)]
    public int IterationId
    {
      get => this.m_iterationId;
      set => this.m_iterationId = value;
    }

    [XmlAttribute]
    [ClientProperty(ClientVisibility.Internal)]
    public string ActionPath
    {
      get => this.m_actionPath;
      set => this.m_actionPath = value;
    }

    [XmlAttribute]
    [ClientProperty(ClientVisibility.Internal)]
    [DefaultValue(true)]
    public bool IsComplete
    {
      get => this.m_isComplete;
      set => this.m_isComplete = value;
    }

    [XmlAttribute]
    [ClientProperty(ClientVisibility.Internal)]
    public Guid TmiRunId { get; set; }
  }
}
