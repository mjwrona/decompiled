// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.Legacy.TestActionResultService
// Assembly: Microsoft.VisualStudio.Services.Tcm.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7631C286-897C-44D1-A133-A0BB6CC047F3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Tcm.Server.Common.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.TestManagement.WebApi.Legacy;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamFoundation.TestManagement.Server.Legacy
{
  internal class TestActionResultService : 
    TeamFoundationTestManagementService,
    ITestActionResultService,
    IVssFrameworkService
  {
    public TestActionResultService() => this.QueryById = TestActionResultService.\u003C\u003EO.\u003C0\u003E__QueryById ?? (TestActionResultService.\u003C\u003EO.\u003C0\u003E__QueryById = new QueryByIdDelegate(Microsoft.TeamFoundation.TestManagement.Server.TestActionResult.QueryById));

    public TestActionResultService(TestManagementRequestContext context)
      : base(context)
    {
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      this.QueryById = TestActionResultService.\u003C\u003EO.\u003C0\u003E__QueryById ?? (TestActionResultService.\u003C\u003EO.\u003C0\u003E__QueryById = new QueryByIdDelegate(Microsoft.TeamFoundation.TestManagement.Server.TestActionResult.QueryById));
    }

    public QueryTestActionResultResponse QueryTestActionResults(
      TestManagementRequestContext context,
      string projectName,
      LegacyTestCaseResultIdentifier testResultId)
    {
      List<Microsoft.TeamFoundation.TestManagement.Server.TestActionResult> actionsList;
      List<Microsoft.TeamFoundation.TestManagement.Server.TestResultParameter> parametersList;
      List<Microsoft.TeamFoundation.TestManagement.Server.TestResultAttachment> attachmentsList;
      this.QueryById(context, TestCaseResultIdentifierConverter.Convert(testResultId), projectName, out actionsList, out parametersList, out attachmentsList);
      QueryTestActionResultResponse actionResultResponse = new QueryTestActionResultResponse();
      IEnumerable<Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.TestActionResult> source1 = TestActionResultUtils.Convert((IEnumerable<Microsoft.TeamFoundation.TestManagement.Server.TestActionResult>) actionsList);
      actionResultResponse.TestActionResults = source1 != null ? source1.ToList<Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.TestActionResult>() : (List<Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.TestActionResult>) null;
      IEnumerable<Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.TestResultParameter> source2 = TestActionResultUtils.Convert((IEnumerable<Microsoft.TeamFoundation.TestManagement.Server.TestResultParameter>) parametersList);
      actionResultResponse.TestResultParameters = source2 != null ? source2.ToList<Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.TestResultParameter>() : (List<Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.TestResultParameter>) null;
      IEnumerable<Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.TestResultAttachment> source3 = AttachmentContractConverter.Convert((IEnumerable<Microsoft.TeamFoundation.TestManagement.Server.TestResultAttachment>) attachmentsList);
      actionResultResponse.TestAttachments = source3 != null ? source3.ToList<Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.TestResultAttachment>() : (List<Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.TestResultAttachment>) null;
      return actionResultResponse;
    }

    public QueryByIdDelegate QueryById { get; }
  }
}
