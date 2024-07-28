// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Server.Services.ApprovalsService
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 134B1041-BFA6-49C6-8C6D-CA5ADF31AF54
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.ReleaseManagement.Common.Diagnostics;
using Microsoft.VisualStudio.Services.ReleaseManagement.Common.Extensions;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Components;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Enumerations;
using Microsoft.VisualStudio.Services.ReleaseManagement.Server.Processors;
using Microsoft.VisualStudio.Services.ReleaseManagement.Server.Utilities;
using Microsoft.VisualStudio.Services.ReleaseManagement.WebApi;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Server.Services
{
  public class ApprovalsService : ReleaseManagement2ServiceBase
  {
    [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "Vssf needs it to be non-static")]
    public IEnumerable<ReleaseEnvironmentStep> GetApprovals(
      IVssRequestContext requestContext,
      Guid projectId,
      ApprovalQueryParameters approvalQueryParameters)
    {
      if (requestContext == null)
        throw new ArgumentNullException(nameof (requestContext));
      if (approvalQueryParameters == null)
        throw new ArgumentNullException(nameof (approvalQueryParameters));
      using (ReleaseManagementTimer releaseManagementTimer = ReleaseManagementTimer.Create(requestContext, "Service", "ApprovalsService.GetApprovals", 1961024))
      {
        ApprovalQueryParameters sqlFilters = approvalQueryParameters.GetSqlFilters();
        Func<ReleaseEnvironmentStepSqlComponent, IEnumerable<ReleaseEnvironmentStep>> action = (Func<ReleaseEnvironmentStepSqlComponent, IEnumerable<ReleaseEnvironmentStep>>) (component => ApprovalsService.GetApprovalsFromDb(component, projectId, sqlFilters));
        IEnumerable<ReleaseEnvironmentStep> approvals1 = requestContext.ExecuteWithinUsingWithComponent<ReleaseEnvironmentStepSqlComponent, IEnumerable<ReleaseEnvironmentStep>>(action);
        releaseManagementTimer.RecordLap("DataAccessLayer", "GetReleasesFetchFromSql", 1961025);
        IEnumerable<ReleaseEnvironmentStep> approvals2 = new ApprovalFilterHelper(requestContext).ApplyFiltersPostSql(approvals1, approvalQueryParameters);
        releaseManagementTimer.RecordLap("DataAccessLayer", "GetApprovalsPostSqlFilter", 1961072);
        return approvals2;
      }
    }

    [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "Vssf needs it to be non-static")]
    public IEnumerable<ReleaseEnvironmentStep> UpdateApprovalsStatus(
      IVssRequestContext context,
      Guid projectId,
      IEnumerable<ReleaseApproval> approvals,
      IList<DeploymentAuthorizationInfo> deploymentAuthorizationInfo)
    {
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      return new ApprovalsProcessor(context, projectId).UpdateApprovalsStatus(approvals, deploymentAuthorizationInfo, ApprovalsService.\u003C\u003EO.\u003C0\u003E__IsAzureActiveDirectoryAccount ?? (ApprovalsService.\u003C\u003EO.\u003C0\u003E__IsAzureActiveDirectoryAccount = new Func<IVssRequestContext, bool>(ServiceEndpointHelper.IsAzureActiveDirectoryAccount)));
    }

    [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "Vssf needs it to be non-static")]
    public IEnumerable<ReleaseEnvironmentStep> GetApproval(
      IVssRequestContext requestContext,
      Guid projectId,
      int approvalId,
      bool includeHistory)
    {
      return new ApprovalsProcessor(requestContext, projectId).GetApproval(approvalId, includeHistory);
    }

    [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "Vssf needs it to be non-static")]
    public IEnumerable<ReleaseEnvironmentStep> GetApprovalHistory(
      IVssRequestContext requestContext,
      Guid projectId,
      IEnumerable<int> approvalStepIds)
    {
      if (requestContext == null)
        throw new ArgumentNullException(nameof (requestContext));
      using (ReleaseManagementTimer releaseManagementTimer = ReleaseManagementTimer.Create(requestContext, "Service", "ApprovalsService.GetApprovalHistory", 1961029))
      {
        Func<ReleaseEnvironmentStepSqlComponent, IEnumerable<ReleaseEnvironmentStep>> action = (Func<ReleaseEnvironmentStepSqlComponent, IEnumerable<ReleaseEnvironmentStep>>) (component => component.GetApprovalHistory(projectId, approvalStepIds));
        IEnumerable<ReleaseEnvironmentStep> approvalHistory = requestContext.ExecuteWithinUsingWithComponent<ReleaseEnvironmentStepSqlComponent, IEnumerable<ReleaseEnvironmentStep>>(action);
        releaseManagementTimer.RecordLap("DataAccessLayer", "ApprovalsService.GetApprovalHistorySql", 1961029);
        return approvalHistory;
      }
    }

    [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "Vssf needs it to be non-static")]
    public virtual IEnumerable<ReleaseEnvironmentStep> GetPendingApprovalsForReleaseDefinitions(
      IVssRequestContext requestContext,
      Guid projectId,
      IEnumerable<int> releaseDefinitionIds,
      DateTime minModifiedTime,
      DateTime maxModifiedTime)
    {
      if (requestContext == null)
        throw new ArgumentNullException(nameof (requestContext));
      if (releaseDefinitionIds == null)
        throw new ArgumentNullException(nameof (releaseDefinitionIds));
      using (ReleaseManagementTimer.Create(requestContext, "Service", "ApprovalsService.GetPendingApprovalsForReleaseDefinitions", 1961032))
      {
        Func<ReleaseEnvironmentStepSqlComponent, IEnumerable<ReleaseEnvironmentStep>> action = (Func<ReleaseEnvironmentStepSqlComponent, IEnumerable<ReleaseEnvironmentStep>>) (component => component.GetPendingApprovalsForReleaseDefinitions(projectId, releaseDefinitionIds, minModifiedTime, maxModifiedTime));
        return requestContext.ExecuteWithinUsingWithComponent<ReleaseEnvironmentStepSqlComponent, IEnumerable<ReleaseEnvironmentStep>>(action);
      }
    }

    [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "Vssf needs it to be non-static")]
    public virtual IEnumerable<ReleaseEnvironmentStep> GetApprovalsForAnIdentity(
      IVssRequestContext requestContext,
      Guid projectId,
      Guid approverId,
      ReleaseEnvironmentStepStatus status,
      int maxApprovals,
      DateTime minModifiedTime,
      DateTime maxModifiedTime)
    {
      Func<ReleaseEnvironmentStepSqlComponent, IEnumerable<ReleaseEnvironmentStep>> action = (Func<ReleaseEnvironmentStepSqlComponent, IEnumerable<ReleaseEnvironmentStep>>) (component => component.ListApprovalsForAnIdentity(projectId, approverId, status, maxApprovals, minModifiedTime, maxModifiedTime));
      return requestContext.ExecuteWithinUsingWithComponent<ReleaseEnvironmentStepSqlComponent, IEnumerable<ReleaseEnvironmentStep>>(action);
    }

    private static IEnumerable<ReleaseEnvironmentStep> GetApprovalsFromDb(
      ReleaseEnvironmentStepSqlComponent sqlComponent,
      Guid projectId,
      ApprovalQueryParameters sqlFilters)
    {
      return sqlComponent.ListReleaseApprovalSteps(projectId, (IEnumerable<int>) sqlFilters.ReleaseIdsFilter, new ReleaseEnvironmentStepStatus?(sqlFilters.StatusFilter), sqlFilters.ApproverIdFilter, sqlFilters.TypeFilter, sqlFilters.ActualApproverIdFilter, sqlFilters.Top, sqlFilters.ContinuationToken, sqlFilters.QueryOrder, sqlFilters.MinModifiedTime, sqlFilters.MaxModifiedTime);
    }
  }
}
