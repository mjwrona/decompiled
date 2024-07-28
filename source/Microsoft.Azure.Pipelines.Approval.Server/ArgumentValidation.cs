// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Pipelines.Approval.Server.ArgumentValidation
// Assembly: Microsoft.Azure.Pipelines.Approval.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 03DD9218-2C79-49A0-9EA7-F497B1327A4F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Pipelines.Approval.Server.dll

using Microsoft.Azure.Pipelines.Approval.WebApi;
using Microsoft.Azure.Pipelines.Checks.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Identity;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.Azure.Pipelines.Approval.Server
{
  public static class ArgumentValidation
  {
    private static int MAX_APPROVAL_PARAMETER_COUNT = 50;
    private static int MAX_APPROVAL_QUERY_PARAMETER_COUNT = 500;
    private const string TraceLayer = "ApprovalArgumentValidation";

    public static void ValidateServiceBasicData(IVssRequestContext requestContext, Guid projectId)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext), "PipelinePolicy.Approval");
      ArgumentUtility.CheckForEmptyGuid(projectId, nameof (projectId), "PipelinePolicy.Approval");
    }

    public static void ValidateApprovalsCreateParameters(
      IVssRequestContext requestContext,
      IEnumerable<ApprovalRequest> createParameters)
    {
      ArgumentUtility.CheckEnumerableForNullOrEmpty((IEnumerable) createParameters, nameof (createParameters), "PipelinePolicy.Approval");
      ArgumentValidation.ValidateApprovalParameterCount(createParameters.Count<ApprovalRequest>(), ArgumentValidation.MAX_APPROVAL_PARAMETER_COUNT);
      List<Guid> approvalsIds = new List<Guid>();
      createParameters.ForEach<ApprovalRequest>((Action<ApprovalRequest>) (createParameter =>
      {
        ArgumentUtility.CheckForNull<ApprovalRequest>(createParameter, nameof (createParameter), "PipelinePolicy.Approval");
        ArgumentUtility.CheckForEmptyGuid(createParameter.ApprovalId, "ApprovalId", "PipelinePolicy.Approval");
        if (approvalsIds.Contains(createParameter.ApprovalId))
          throw new ArgumentException(ApprovalResources.DuplicateApprovalCreateRequests(), "ApprovalId").Expected("PipelinePolicy.Approval");
        approvalsIds.Add(createParameter.ApprovalId);
        ArgumentValidation.ValidateOwner(createParameter.Owner);
        ArgumentValidation.ValidateApprovalConfig(requestContext, createParameter.Config, createParameter.Owner);
      }));
    }

    public static void ValidateApprovalConfig(
      IVssRequestContext requestContext,
      ApprovalConfig config,
      ApprovalOwner owner,
      bool verifyIdentities = false)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext), "PipelinePolicy.Approval");
      ArgumentUtility.CheckForNull<ApprovalConfig>(config, nameof (config), "PipelinePolicy.Approval");
      ArgumentUtility.CheckEnumerableForNullOrEmpty((IEnumerable) config.Approvers, "config.Approvers", "PipelinePolicy.Approval");
      ArgumentValidation.ValidateApproversAndBlockedApprovers(requestContext, config, verifyIdentities);
      if (config.MinRequiredApprovers.HasValue)
      {
        int? nullable;
        if (config.ExecutionOrder == ApprovalExecutionOrder.InSequence)
        {
          nullable = config.MinRequiredApprovers;
          if (nullable.Value != 0)
          {
            nullable = config.MinRequiredApprovers;
            if (nullable.Value != config.Approvers.Count)
              throw new ArgumentException(ApprovalResources.InvalidApprovalConfigInSequenceMinRequiredApproversInput(), "config.MinRequiredApprovers").Expected("PipelinePolicy.Approval");
          }
        }
        nullable = config.MinRequiredApprovers;
        nullable = nullable.Value >= 0 ? config.MinRequiredApprovers : throw new ArgumentException(ApprovalResources.InvalidApprovalConfigMinRequiredApproversInput(), "config.MinRequiredApprovers").Expected("PipelinePolicy.Approval");
        if (nullable.Value <= config.Approvers.Count)
          ;
      }
    }

    public static void ValidateOwner(ApprovalOwner Owner)
    {
      if (!Enum.IsDefined(typeof (ApprovalOwner), (object) Owner))
        throw new ArgumentException(ApprovalResources.InvalidApprovalOwner(), nameof (Owner)).Expected("PipelinePolicy.Approval");
    }

    public static void ValidateApprovalsUpdateParameters(
      IVssRequestContext requestContext,
      IEnumerable<ApprovalUpdateParameters> updateParameters)
    {
      ArgumentUtility.CheckEnumerableForNullOrEmpty((IEnumerable) updateParameters, nameof (updateParameters), "PipelinePolicy.Approval");
      ArgumentValidation.ValidateApprovalParameterCount(updateParameters.Count<ApprovalUpdateParameters>(), ArgumentValidation.MAX_APPROVAL_PARAMETER_COUNT);
      Dictionary<Guid, ApprovalUpdateParameters> approvalIdToUpdateParametersMap = new Dictionary<Guid, ApprovalUpdateParameters>();
      IList<IdentityRef> assignedApprovers = (IList<IdentityRef>) new List<IdentityRef>();
      IList<IdentityRef> reassignedApprovers = (IList<IdentityRef>) new List<IdentityRef>();
      ApprovalUpdateParametersComparer equalityComparer = new ApprovalUpdateParametersComparer();
      updateParameters.ForEach<ApprovalUpdateParameters>((Action<ApprovalUpdateParameters>) (updateParameter =>
      {
        Guid approvalId = updateParameter.ApprovalId;
        if (updateParameter.ApprovalId.Equals(Guid.Empty))
          throw new ArgumentException(ApprovalResources.InvalidApprovalId(), "updateParameters.ApprovalId").Expected("PipelinePolicy.Approval");
        if (approvalIdToUpdateParametersMap.ContainsKey(updateParameter.ApprovalId) && !equalityComparer.Equals(approvalIdToUpdateParametersMap[updateParameter.ApprovalId], updateParameter))
          throw new ArgumentException(ApprovalResources.ApprovalUpdateRequestFailed((object) updateParameter.ApprovalId), "updateParameters.ApprovalId").Expected("PipelinePolicy.Approval");
        if (updateParameter.ReassignTo != null && updateParameter.ReassignTo.Id != null)
        {
          if (updateParameter.AssignedApprover != null && updateParameter.AssignedApprover.Id.Equals(updateParameter.ReassignTo.Id) || updateParameter.AssignedApprover == null && updateParameter.ReassignTo.Id.Equals(requestContext.GetUserIdentity().Id.ToString("D")))
          {
            requestContext.TraceError(34001708, "ApprovalArgumentValidation", "Invalid reassign request. Assigned approver and reassign identity cannot be same.");
            throw new ArgumentException(ApprovalResources.InvalidApprovalReassignRequest((object) updateParameter.ApprovalId)).Expected("PipelinePolicy.Approval");
          }
          if (updateParameter.AssignedApprover != null && updateParameter.AssignedApprover.Id != null)
            assignedApprovers.Add(updateParameter.AssignedApprover);
          reassignedApprovers.Add(updateParameter.ReassignTo);
        }
        else
          ArgumentValidation.ValidateApprovalUpdateParameters(requestContext, updateParameter);
        approvalIdToUpdateParametersMap[updateParameter.ApprovalId] = updateParameter;
      }));
      ArgumentValidation.ValidateReassignIdentities(requestContext, assignedApprovers, reassignedApprovers);
    }

    private static void ValidateApprovalUpdateParameters(
      IVssRequestContext requestContext,
      ApprovalUpdateParameters updateParameter)
    {
      ArgumentUtility.CheckForNull<ApprovalStatus>(updateParameter.Status, "Status", "PipelinePolicy.Approval");
      if (!requestContext.IsFeatureEnabled("Pipelines.Approvals.EnableDeferredApprovals"))
      {
        ApprovalStatus? status = updateParameter.Status;
        ApprovalStatus approvalStatus = ApprovalStatus.Deferred;
        if (status.GetValueOrDefault() == approvalStatus & status.HasValue)
          throw new ArgumentException(ApprovalResources.DeferredApprovalNotSupported()).Expected("PipelinePolicy.Approval");
      }
      DateTime utcNow = DateTime.UtcNow;
      ApprovalStatus? nullable = updateParameter.Status;
      ApprovalStatus approvalStatus1 = ApprovalStatus.Deferred;
      if (nullable.GetValueOrDefault() == approvalStatus1 & nullable.HasValue && (!updateParameter.DeferredTo.HasValue || updateParameter.DeferredTo.Value < utcNow.AddMinutes(-5.0) || updateParameter.DeferredTo.Value > utcNow.AddDays(30.0)))
        throw new ArgumentException(ApprovalResources.DeferredApprovalsMustHaveValidDeferredTo()).Expected("PipelinePolicy.Approval");
      ApprovalStatus? status1 = updateParameter.Status;
      nullable = status1.HasValue ? new ApprovalStatus?(status1.GetValueOrDefault() & (ApprovalStatus.Completed | ApprovalStatus.Deferred)) : new ApprovalStatus?();
      ApprovalStatus? status2 = updateParameter.Status;
      if (!(nullable.GetValueOrDefault() == status2.GetValueOrDefault() & nullable.HasValue == status2.HasValue))
        throw new ArgumentException(ApprovalResources.ApprovalUpdateRequestFailed((object) updateParameter.ApprovalId), "updateParameter.Status").Expected("PipelinePolicy.Approval");
      status2 = updateParameter.Status;
      ApprovalStatus approvalStatus2 = ApprovalStatus.Canceled;
      if (!(status2.GetValueOrDefault() == approvalStatus2 & status2.HasValue))
      {
        status2 = updateParameter.Status;
        ApprovalStatus approvalStatus3 = ApprovalStatus.TimedOut;
        if (!(status2.GetValueOrDefault() == approvalStatus3 & status2.HasValue))
          return;
      }
      if (!requestContext.IsSystemContext)
        throw new ArgumentException(ApprovalResources.ApprovalUpdateRequestFailed((object) updateParameter.ApprovalId), "updateParameter.Status").Expected("PipelinePolicy.Approval");
    }

    public static void ValidateApprovalParameterCount(int parameterCount, int maxCount)
    {
      if (parameterCount > maxCount)
        throw new ApprovalInvalidParametersException(ApprovalResources.ApprovalParametersMaxLengthExceptionMessage((object) maxCount));
    }

    public static void ValidateApprovalsQueryParameters(ApprovalsQueryParameters queryParameters)
    {
      ArgumentUtility.CheckForNull<ApprovalsQueryParameters>(queryParameters, nameof (queryParameters), "PipelinePolicy.Approval");
      if (queryParameters.ApprovalIds == null || queryParameters.ApprovalIds.Count < 1)
        throw new ArgumentException(ApprovalResources.InvalidApprovalsQueryParameters(), nameof (queryParameters)).Expected("PipelinePolicy.Approval");
      ArgumentValidation.ValidateApprovalParameterCount(queryParameters.ApprovalIds.Count<Guid>(), ArgumentValidation.MAX_APPROVAL_QUERY_PARAMETER_COUNT);
    }

    public static void ValidateApprovalsQueryAllParameters(ApprovalsQueryParameters queryParameters)
    {
      ArgumentUtility.CheckForNull<ApprovalsQueryParameters>(queryParameters, nameof (queryParameters), "PipelinePolicy.Approval");
      ArgumentValidation.ValidateApprovalParameterCount(queryParameters.ApprovalIds.Count<Guid>() + queryParameters.UserIds.Count<Guid>(), ArgumentValidation.MAX_APPROVAL_QUERY_PARAMETER_COUNT);
    }

    private static void ValidateApproversAndBlockedApprovers(
      IVssRequestContext requestContext,
      ApprovalConfig config,
      bool verifyIdentities)
    {
      IList<string> identitiesToVerify = (IList<string>) new List<string>();
      ArgumentValidation.ValidateAndPopulateIdentityIds((IList<IdentityRef>) config.Approvers, identitiesToVerify, "approver");
      ArgumentValidation.ValidateAndPopulateIdentityIds((IList<IdentityRef>) config.BlockedApprovers, identitiesToVerify, "blockedApprover");
      if (!verifyIdentities)
        return;
      ArgumentValidation.ValidateIdentities(requestContext, identitiesToVerify);
    }

    private static void ValidateReassignIdentities(
      IVssRequestContext requestContext,
      IList<IdentityRef> assignedApprover,
      IList<IdentityRef> reassignedApprovers)
    {
      IList<string> identitiesToVerify = (IList<string>) new List<string>();
      ArgumentValidation.ValidateAndPopulateIdentityIds(assignedApprover, identitiesToVerify, nameof (assignedApprover));
      ArgumentValidation.ValidateAndPopulateIdentityIds(reassignedApprovers, identitiesToVerify, "reassignTo");
      ArgumentValidation.ValidateIdentities(requestContext, identitiesToVerify);
    }

    private static void ValidateIdentities(
      IVssRequestContext requestContext,
      IList<string> identitiesToVerify)
    {
      if (identitiesToVerify == null || identitiesToVerify.Count <= 0)
        return;
      IEnumerable<Guid> identityIds = identitiesToVerify.Select<string, Guid>((Func<string, Guid>) (x => Guid.Parse(x))).Distinct<Guid>();
      IEnumerable<Guid> guids = identityIds;
      List<Microsoft.VisualStudio.Services.Identity.Identity> list = requestContext.GetService<IdentityService>().GetIdentities(requestContext, identityIds).ToList<Microsoft.VisualStudio.Services.Identity.Identity>();
      if (list != null)
      {
        IEnumerable<Guid> second = list.Select<Microsoft.VisualStudio.Services.Identity.Identity, Guid>((Func<Microsoft.VisualStudio.Services.Identity.Identity, Guid>) (identity => identity == null ? Guid.Empty : identity.Id));
        guids = guids.Except<Guid>(second);
      }
      if (guids != null && guids.Count<Guid>() > 0)
        throw new ArgumentException(ApprovalResources.InvalidIdentities((object) string.Join<Guid>(ApprovalResources.StringValueCommaSeparator(), guids)), "identities").Expected("PipelinePolicy.Approval");
    }

    private static void ValidateAndPopulateIdentityIds(
      IList<IdentityRef> identities,
      IList<string> identitiesToVerify,
      string varName)
    {
      if (identities == null || identities.Count <= 0)
        return;
      identities.ForEach<IdentityRef>((Action<IdentityRef>) (identity =>
      {
        ArgumentUtility.CheckForNull<IdentityRef>(identity, varName, "PipelinePolicy.Approval");
        if (!Guid.TryParse(identity.Id, out Guid _))
          throw new ArgumentException(ApprovalResources.InvalidIdentities(identity.DisplayName == null || identity.DisplayName.Length <= 0 ? (object) identity.Id : (object) identity.DisplayName), varName).Expected("PipelinePolicy.Approval");
        identitiesToVerify.Add(identity.Id);
      }));
    }
  }
}
