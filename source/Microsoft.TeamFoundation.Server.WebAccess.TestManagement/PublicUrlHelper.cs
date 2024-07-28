// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.TestManagement.PublicUrlHelper
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.TestManagement, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 2E4165D5-898A-42D9-B816-9FABF135E4DA
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.TestManagement.dll

namespace Microsoft.TeamFoundation.Server.WebAccess.TestManagement
{
  public static class PublicUrlHelper
  {
    private const string c_projectAndTeamNameString = "{0}/{1}";
    private const string c_viewSuitePublicUrl = "{0}{1}/_testManagement?planId={2}&suiteId={3}";
    private const string c_getAttachmentPublicUrl = "{0}{1}/_apis/wit/attachments/{2}?fileName={3}&download=true";
    private const string c_viewPlanPublicUrl = "{0}{1}/_testManagement?planId={2}";
    private const string c_viewTestsPublicUrl = "{0}{1}/_testManagement?planId={2}&suiteId={3}&_a=filterByTester";
    private const string c_editWorkItemPublicUrl = "{0}{1}/_workitems?_a=edit&id={2}";
    private const string c_runPublicUrl = "{0}{1}/_testManagement/Runs#_a=resultSummary&runId={2}&resultId={3}";
    private const string c_viewSharedParameterUrl = "{0}{1}/_testManagement/sharedParameters#sharedParameterId={2}&_a=values";

    public static string GetTestSuiteUrl(
      TestManagerRequestContext testContext,
      int suiteId,
      int planId)
    {
      return string.Format("{0}{1}/_testManagement?planId={2}&suiteId={3}", (object) TestManagementUrlHelper.GetTfsBaseUrl(testContext.TfsRequestContext), (object) PublicUrlHelper.GetProjectAndTeamName(testContext), (object) planId, (object) suiteId);
    }

    public static string GetAttachmentUrl(
      TestManagerRequestContext testContext,
      string attachmentName,
      string locationId)
    {
      return string.Format("{0}{1}/_apis/wit/attachments/{2}?fileName={3}&download=true", (object) TestManagementUrlHelper.GetTfsBaseUrl(testContext.TfsRequestContext), (object) testContext.ProjectName, (object) locationId, (object) attachmentName);
    }

    public static string GetTestPlanUrl(TestManagerRequestContext testContext, int planId) => string.Format("{0}{1}/_testManagement?planId={2}", (object) TestManagementUrlHelper.GetTfsBaseUrl(testContext.TfsRequestContext), (object) PublicUrlHelper.GetProjectAndTeamName(testContext), (object) planId);

    public static string GetViewTestsLink(
      TestManagerRequestContext testContext,
      int planId,
      int suiteId)
    {
      return string.Format("{0}{1}/_testManagement?planId={2}&suiteId={3}&_a=filterByTester", (object) TestManagementUrlHelper.GetTfsBaseUrl(testContext.TfsRequestContext), (object) PublicUrlHelper.GetProjectAndTeamName(testContext), (object) planId, (object) suiteId);
    }

    public static string GetWorkItemEditUrl(TestManagerRequestContext testContext, int workItemId) => string.Format("{0}{1}/_workitems?_a=edit&id={2}", (object) TestManagementUrlHelper.GetTfsBaseUrl(testContext.TfsRequestContext), (object) PublicUrlHelper.GetProjectAndTeamName(testContext), (object) workItemId);

    public static string GetTestRunUrl(
      TestManagerRequestContext testContext,
      string runId,
      string resultId)
    {
      return string.Format("{0}{1}/_testManagement/Runs#_a=resultSummary&runId={2}&resultId={3}", (object) TestManagementUrlHelper.GetTfsBaseUrl(testContext.TfsRequestContext), (object) PublicUrlHelper.GetProjectAndTeamName(testContext), (object) runId, (object) resultId);
    }

    public static string GetSharedParameterUrl(
      TestManagerRequestContext testContext,
      string sharedParameterId)
    {
      return string.Format("{0}{1}/_testManagement/sharedParameters#sharedParameterId={2}&_a=values", (object) TestManagementUrlHelper.GetTfsBaseUrl(testContext.TfsRequestContext), (object) PublicUrlHelper.GetProjectAndTeamName(testContext), (object) sharedParameterId);
    }

    private static string GetProjectAndTeamName(TestManagerRequestContext testContext) => testContext.Team == null || string.IsNullOrEmpty(testContext.Team.Name) ? testContext.ProjectName : string.Format("{0}/{1}", (object) testContext.ProjectName, (object) testContext.Team.Name);
  }
}
