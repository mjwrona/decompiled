// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.TestActionResult
// Assembly: Microsoft.VisualStudio.Services.Tcm.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7631C286-897C-44D1-A133-A0BB6CC047F3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Tcm.Server.Common.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Xml.Serialization;

namespace Microsoft.TeamFoundation.TestManagement.Server
{
  [XmlInclude(typeof (TestIterationResult))]
  [XmlInclude(typeof (TestStepResult))]
  [XmlInclude(typeof (SharedStepResult))]
  [ClassVisibility(ClientVisibility.Internal)]
  public class TestActionResult : TestResult
  {
    private string m_actionPath;
    private int m_sharedStepId;
    private int m_sharedStepRevision;
    private int m_iterationId;

    [XmlAttribute]
    [ClientProperty(ClientVisibility.Public, ClientVisibility.Private)]
    public string ActionPath
    {
      get => this.m_actionPath;
      set => this.m_actionPath = value;
    }

    [XmlAttribute]
    [DefaultValue(0)]
    [ClientProperty(ClientVisibility.Public, ClientVisibility.Private)]
    public int SetId
    {
      get => this.m_sharedStepId;
      set => this.m_sharedStepId = value;
    }

    [XmlAttribute]
    [DefaultValue(0)]
    [ClientProperty(ClientVisibility.Public, ClientVisibility.Private)]
    public int SetRevision
    {
      get => this.m_sharedStepRevision;
      set => this.m_sharedStepRevision = value;
    }

    [XmlAttribute]
    [DefaultValue(0)]
    [ClientProperty(ClientVisibility.Public, ClientVisibility.Private)]
    public int IterationId
    {
      get => this.m_iterationId;
      set => this.m_iterationId = value;
    }

    public override string ToString() => string.Format((IFormatProvider) CultureInfo.InvariantCulture, "TestActionResult ({0}, {1}, {2}, {3})", (object) this.TestRunId, (object) this.TestResultId, (object) this.IterationId, (object) this.ActionPath);

    private static void QueryById(
      TestManagementRequestContext context,
      TestCaseResultIdentifier identifier,
      GuidAndString projectId,
      out List<TestActionResult> actions,
      out List<TestResultParameter> parameters,
      out List<TestResultAttachment> attachments)
    {
      if (!context.SecurityManager.HasViewTestResultsPermission(context, projectId.String))
      {
        actions = new List<TestActionResult>();
        parameters = new List<TestResultParameter>();
        attachments = new List<TestResultAttachment>();
      }
      else
      {
        string areaUri = string.Empty;
        using (TestManagementDatabase managementDatabase = TestManagementDatabase.Create(context))
          managementDatabase.QueryTestActionResults(context, identifier.TestRunId, identifier.TestResultId, projectId.GuidId, out actions, out parameters, out attachments, out areaUri);
        if (!context.SecurityManager.CanViewTestResult(context, areaUri))
        {
          actions = new List<TestActionResult>();
          parameters = new List<TestResultParameter>();
          attachments = new List<TestResultAttachment>();
        }
        else
        {
          if (attachments == null || !attachments.Any<TestResultAttachment>())
            return;
          TestResultAttachment.SignAttachmentObjects(context.RequestContext, attachments);
        }
      }
    }

    internal static void QueryById(
      TestManagementRequestContext context,
      TestCaseResultIdentifier identifier,
      string projectName,
      out List<TestActionResult> actions,
      out List<TestResultParameter> parameters,
      out List<TestResultAttachment> attachments)
    {
      GuidAndString projectFromName = Validator.CheckAndGetProjectFromName(context, projectName);
      TestActionResult.QueryById(context, identifier, projectFromName, out actions, out parameters, out attachments);
    }
  }
}
