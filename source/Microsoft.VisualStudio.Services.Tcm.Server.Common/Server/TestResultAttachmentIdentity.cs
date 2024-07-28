// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.TestResultAttachmentIdentity
// Assembly: Microsoft.VisualStudio.Services.Tcm.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7631C286-897C-44D1-A133-A0BB6CC047F3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Tcm.Server.Common.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.ComponentModel;
using System.Globalization;
using System.Xml.Serialization;

namespace Microsoft.TeamFoundation.TestManagement.Server
{
  [ClassVisibility(ClientVisibility.Internal)]
  public class TestResultAttachmentIdentity
  {
    private int m_testRunId;
    private int m_testResultId;
    private int m_attachmentId;
    private int m_sessionId;

    [XmlAttribute]
    [DefaultValue(0)]
    public int TestRunId
    {
      get => this.m_testRunId;
      set => this.m_testRunId = value;
    }

    [XmlAttribute]
    [DefaultValue(0)]
    public int TestResultId
    {
      get => this.m_testResultId;
      set => this.m_testResultId = value;
    }

    [XmlAttribute]
    [DefaultValue(0)]
    public int SessionId
    {
      get => this.m_sessionId;
      set => this.m_sessionId = value;
    }

    [XmlAttribute]
    public int AttachmentId
    {
      get => this.m_attachmentId;
      set => this.m_attachmentId = value;
    }

    public override string ToString() => string.Format((IFormatProvider) CultureInfo.InvariantCulture, "TestResultAttachmentIdentity Run={0} Result={1} Attachment={2} Session={3}", (object) this.m_testRunId, (object) this.m_testResultId, (object) this.m_attachmentId, (object) this.m_sessionId);
  }
}
