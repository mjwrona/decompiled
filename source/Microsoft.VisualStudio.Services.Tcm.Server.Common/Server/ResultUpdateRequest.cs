// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.ResultUpdateRequest
// Assembly: Microsoft.VisualStudio.Services.Tcm.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7631C286-897C-44D1-A133-A0BB6CC047F3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Tcm.Server.Common.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Globalization;
using System.Xml.Serialization;

namespace Microsoft.TeamFoundation.TestManagement.Server
{
  [ClassVisibility(ClientVisibility.Internal)]
  [CLSCompliant(false)]
  public class ResultUpdateRequest
  {
    private int m_testRunId;
    private int m_testResultId;
    private TestCaseResult m_testCaseResult;
    private TestActionResult[] m_actionResults;
    private TestActionResult[] m_actionResultDeletes;
    private TestResultParameter[] m_parameters;
    private TestResultParameter[] m_parameterDeletes;
    private TestResultAttachment[] m_attachments;
    private TestResultAttachmentIdentity[] m_attachmentDeletes;

    [XmlAttribute]
    public int TestRunId
    {
      get => this.m_testRunId;
      set => this.m_testRunId = value;
    }

    [XmlAttribute]
    public int TestResultId
    {
      get => this.m_testResultId;
      set => this.m_testResultId = value;
    }

    [XmlElement]
    public TestCaseResult TestCaseResult
    {
      get => this.m_testCaseResult;
      set => this.m_testCaseResult = value;
    }

    [XmlArrayItem(typeof (TestActionResult))]
    public TestActionResult[] ActionResults
    {
      get => this.m_actionResults;
      set => this.m_actionResults = value;
    }

    [XmlArrayItem(typeof (TestActionResult))]
    public TestActionResult[] ActionResultDeletes
    {
      get => this.m_actionResultDeletes;
      set => this.m_actionResultDeletes = value;
    }

    [XmlArrayItem(typeof (TestResultParameter))]
    public TestResultParameter[] Parameters
    {
      get => this.m_parameters;
      set => this.m_parameters = value;
    }

    [XmlArrayItem(typeof (TestResultParameter))]
    public TestResultParameter[] ParameterDeletes
    {
      get => this.m_parameterDeletes;
      set => this.m_parameterDeletes = value;
    }

    [XmlArrayItem(typeof (TestResultAttachment))]
    public TestResultAttachment[] Attachments
    {
      get => this.m_attachments;
      set => this.m_attachments = value;
    }

    [XmlArrayItem(typeof (TestResultAttachmentIdentity))]
    public TestResultAttachmentIdentity[] AttachmentDeletes
    {
      get => this.m_attachmentDeletes;
      set => this.m_attachmentDeletes = value;
    }

    public override string ToString() => string.Format((IFormatProvider) CultureInfo.InvariantCulture, "ResultUpdateRequest Run={0} Result={1}", (object) this.m_testRunId, (object) this.m_testResultId);
  }
}
