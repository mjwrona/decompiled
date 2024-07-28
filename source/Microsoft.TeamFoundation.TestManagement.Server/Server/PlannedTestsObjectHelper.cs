// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.PlannedTestsObjectHelper
// Assembly: Microsoft.TeamFoundation.TestManagement.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F9B71993-88CC-4B0D-89B6-4ADDEEAB3DE1
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.TestManagement.Server.dll

using Microsoft.Azure.Boards.CssNodes;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Integration.Server;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamFoundation.TestManagement.Server
{
  public class PlannedTestsObjectHelper : 
    TestManagementObjectHelper,
    IPlannedTestsObjectHelper,
    ITestManagementObjectHelper
  {
    public virtual List<TestPoint> FetchTestPointsFromIds(
      TestManagementRequestContext requestContext,
      string projectName,
      int planId,
      IdAndRev[] idsToFetch,
      string[] testCaseProperties,
      List<int> deletedIds)
    {
      return this.ExecuteAction<List<TestPoint>>(requestContext.RequestContext, "TestManagementObjectHelper.FetchTestPointsFromIds", (Func<List<TestPoint>>) (() => TestPoint.Fetch(requestContext, projectName, planId, idsToFetch, testCaseProperties, deletedIds)), 1015080, "TestManagement", "BusinessLayer");
    }

    public virtual List<TestPlan> FetchTestPlans(
      TfsTestManagementRequestContext requestContext,
      IEnumerable<int> testPlanIds,
      string projectName,
      bool includeDetails = true)
    {
      return this.ExecuteAction<List<TestPlan>>(requestContext.RequestContext, "TestManagementObjectHelper.FetchTestPlans", (Func<List<TestPlan>>) (() => TestPlan.Fetch((TestManagementRequestContext) requestContext, testPlanIds.Select<int, IdAndRev>((Func<int, IdAndRev>) (planId => new IdAndRev()
      {
        Id = planId
      })).ToArray<IdAndRev>(), new List<int>(), projectName, includeDetails: includeDetails)), 1015080, "TestManagement", "BusinessLayer");
    }

    public virtual void CheckWorkItemDeletePermission(
      string projectName,
      TestManagementRequestContext context)
    {
      using (PerfManager.Measure(context.RequestContext, "CrossService", TraceUtils.GetActionName(nameof (CheckWorkItemDeletePermission), "Security")))
      {
        IVssSecurityNamespace securityNamespace = context.RequestContext.GetService<ITeamFoundationSecurityService>().GetSecurityNamespace(context.RequestContext, AuthorizationSecurityConstants.ProjectSecurityGuid);
        string projectUriFromName = Validator.CheckAndGetProjectUriFromName(context, projectName);
        int num = securityNamespace.HasPermission(context.RequestContext, securityNamespace.NamespaceExtension.HandleIncomingToken(context.RequestContext, securityNamespace, projectUriFromName), AuthorizationProjectPermissions.WorkItemSoftDelete) ? 1 : 0;
        bool flag = securityNamespace.HasPermission(context.RequestContext, securityNamespace.NamespaceExtension.HandleIncomingToken(context.RequestContext, securityNamespace, projectUriFromName), AuthorizationProjectPermissions.WorkItemPermanentlyDelete);
        if (num == 0 || !flag)
          throw new Microsoft.TeamFoundation.TestManagement.WebApi.AccessDeniedException(ServerResources.WorkItemDeletePermissionError);
      }
    }
  }
}
