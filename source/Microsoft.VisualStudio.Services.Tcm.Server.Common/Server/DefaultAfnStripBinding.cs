// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.DefaultAfnStripBinding
// Assembly: Microsoft.VisualStudio.Services.Tcm.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7631C286-897C-44D1-A133-A0BB6CC047F3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Tcm.Server.Common.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Globalization;
using System.Xml.Serialization;

namespace Microsoft.TeamFoundation.TestManagement.Server
{
  [ClassVisibility(ClientVisibility.Internal)]
  public class DefaultAfnStripBinding
  {
    private int m_testCaseId;
    private int m_testRunId;
    private int m_testResultId;

    [XmlAttribute]
    [ClientProperty(ClientVisibility.Private)]
    public int TestCaseId
    {
      get => this.m_testCaseId;
      set => this.m_testCaseId = value;
    }

    [XmlAttribute]
    [ClientProperty(ClientVisibility.Private)]
    public int TestRunId
    {
      get => this.m_testRunId;
      set => this.m_testRunId = value;
    }

    [XmlAttribute]
    [ClientProperty(ClientVisibility.Private)]
    public int TestResultId
    {
      get => this.m_testResultId;
      set => this.m_testResultId = value;
    }

    public override string ToString() => string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Afn Strip - Case = {0} Run = {1} Result = {2}", (object) this.m_testCaseId, (object) this.m_testRunId, (object) this.m_testResultId);

    internal static void UpdateAfnStripBindingList(
      TestManagementRequestContext context,
      DefaultAfnStripBinding[] bindings,
      GuidAndString project)
    {
      ArgumentUtility.CheckForNull<DefaultAfnStripBinding[]>(bindings, nameof (bindings), "Test Results");
      context.SecurityManager.CheckPublishTestResultsPermission(context, project.String);
      using (TestManagementDatabase managementDatabase = TestManagementDatabase.Create(context))
      {
        foreach (DefaultAfnStripBinding binding in bindings)
          managementDatabase.UpdateDefaultStrip(project.GuidId, binding.TestRunId, binding.TestResultId, binding.TestCaseId);
      }
    }

    internal static void UpdateAfnStripBindingList(
      TestManagementRequestContext context,
      DefaultAfnStripBinding[] bindings,
      string projectName)
    {
      GuidAndString projectFromName = Validator.CheckAndGetProjectFromName(context, projectName);
      DefaultAfnStripBinding.UpdateAfnStripBindingList(context, bindings, projectFromName);
    }

    internal static void UpdateAfnStripBindingList(
      TestManagementRequestContext context,
      DefaultAfnStripBinding[] bindings,
      Guid projectId)
    {
      string projectUriFromId = Validator.CheckAndGetProjectUriFromId(context, projectId);
      DefaultAfnStripBinding.UpdateAfnStripBindingList(context, bindings, new GuidAndString(projectUriFromId, projectId));
    }
  }
}
