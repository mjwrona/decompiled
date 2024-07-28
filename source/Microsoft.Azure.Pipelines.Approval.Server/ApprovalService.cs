// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Pipelines.Approval.Server.ApprovalService
// Assembly: Microsoft.Azure.Pipelines.Approval.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 03DD9218-2C79-49A0-9EA7-F497B1327A4F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Pipelines.Approval.Server.dll

using Microsoft.Azure.Pipelines.Approval.Server.DataAccess;
using Microsoft.Azure.Pipelines.Approval.Server.DataAccess.Model;
using Microsoft.Azure.Pipelines.Approval.WebApi;
using Microsoft.Azure.Pipelines.Checks.Common;
using Microsoft.TeamFoundation.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.Core;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Identity;
using Microsoft.VisualStudio.Services.Location.Server;
using Microsoft.VisualStudio.Services.WebApi;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Web;

namespace Microsoft.Azure.Pipelines.Approval.Server
{
  public class ApprovalService : IApprovalService, IVssFrameworkService
  {
    private static readonly HashSet<Guid> s_emptyGuidSet = new HashSet<Guid>();
    private const string c_layer = "ApprovalService";
    private const int m_defaultQueryApprovalResultCount = 250;
    private const int m_QueryApprovalsGroupsCountThreshold = 20;

    public IEnumerable<Microsoft.Azure.Pipelines.Approval.WebApi.Approval> CreateApprovals(
      IVssRequestContext requestContext,
      Guid projectId,
      IEnumerable<ApprovalRequest> createParameters)
    {
      ArgumentValidation.ValidateServiceBasicData(requestContext, projectId);
      using (new MethodScope(requestContext, nameof (ApprovalService), nameof (CreateApprovals)))
      {
        ArgumentValidation.ValidateApprovalsCreateParameters(requestContext, createParameters);
        List<CreateApprovalParameter> createApprovalParameters = new List<CreateApprovalParameter>();
        Guid approvalCreator = requestContext.GetUserIdentity().Id;
        createParameters.ForEach<ApprovalRequest>((Action<ApprovalRequest>) (createParameter =>
        {
          Guid guid = Guid.NewGuid();
          ApprovalConfig approvalConfig = this.FillConfigDefaults(createParameter.Config);
          createApprovalParameters.Add(new CreateApprovalParameter()
          {
            ApprovalId = createParameter.ApprovalId,
            Config = approvalConfig,
            TimeoutJobId = guid,
            CreatedBy = approvalCreator,
            Owner = createParameter.Owner,
            Context = createParameter.Context
          });
        }));
        IList<Microsoft.Azure.Pipelines.Approval.WebApi.Approval> approvals;
        using (ApprovalComponent component = requestContext.CreateComponent<ApprovalComponent>())
          approvals = component.CreateApprovals(projectId, (IList<CreateApprovalParameter>) createApprovalParameters);
        IDictionary<string, IdentityRef> identities = this.FetchRequiredIdentities(requestContext, (IEnumerable<Microsoft.Azure.Pipelines.Approval.WebApi.Approval>) approvals);
        ISet<Guid> approvalIdsWithAdminPermissions = this.FilterApprovalsWithRequiredPermissions(requestContext, projectId, (IEnumerable<Microsoft.Azure.Pipelines.Approval.WebApi.Approval>) approvals, ApprovalPermissions.ResourceAdmin);
        approvals.ForEach<Microsoft.Azure.Pipelines.Approval.WebApi.Approval>((Action<Microsoft.Azure.Pipelines.Approval.WebApi.Approval>) (approval =>
        {
          this.FillComputedValues(requestContext, projectId, approval, identities: identities, hasAdminPermission: approvalIdsWithAdminPermissions.Contains(approval.Id));
          this.SendApprovalPendingNotificationEvent(requestContext, projectId, approval);
        }));
        string feature = "ApprovalService.Add";
        CustomerIntelligenceData properties = new CustomerIntelligenceData();
        properties.Add("parameterCount", createParameters.Count<ApprovalRequest>().ToString());
        requestContext.GetService<CustomerIntelligenceService>().Publish(requestContext, "PipelinePolicy.Approval", feature, properties);
        return (IEnumerable<Microsoft.Azure.Pipelines.Approval.WebApi.Approval>) approvals;
      }
    }

    public Microsoft.Azure.Pipelines.Approval.WebApi.Approval GetApproval(
      IVssRequestContext requestContext,
      Guid projectId,
      Guid approvalId,
      ApprovalDetailsExpandParameter expand = ApprovalDetailsExpandParameter.None)
    {
      ArgumentValidation.ValidateServiceBasicData(requestContext, projectId);
      using (new MethodScope(requestContext, nameof (ApprovalService), nameof (GetApproval)))
      {
        ArgumentUtility.CheckForEmptyGuid(approvalId, nameof (approvalId), "PipelinePolicy.Approval");
        Microsoft.Azure.Pipelines.Approval.WebApi.Approval approval1 = (Microsoft.Azure.Pipelines.Approval.WebApi.Approval) null;
        using (ApprovalComponent component = requestContext.CreateComponent<ApprovalComponent>())
          approval1 = component.GetApproval(projectId, approvalId);
        if (approval1 == null)
          throw new ApprovalNotFoundException(ApprovalResources.ApprovalNotFoundExceptionMessage());
        IVssRequestContext requestContext1 = requestContext;
        Guid projectId1 = projectId;
        List<Microsoft.Azure.Pipelines.Approval.WebApi.Approval> approvalList = new List<Microsoft.Azure.Pipelines.Approval.WebApi.Approval>();
        approvalList.Add(approval1);
        int expand1 = (int) expand;
        Microsoft.Azure.Pipelines.Approval.WebApi.Approval approval2 = this.GetPopulatedApprovals(requestContext1, projectId1, (IEnumerable<Microsoft.Azure.Pipelines.Approval.WebApi.Approval>) approvalList, (ApprovalDetailsExpandParameter) expand1).FirstOrDefault<Microsoft.Azure.Pipelines.Approval.WebApi.Approval>();
        string feature = "ApprovalService.Get";
        CustomerIntelligenceData properties = new CustomerIntelligenceData();
        properties.Add("includeSteps", (object) (expand & ApprovalDetailsExpandParameter.Steps));
        requestContext.GetService<CustomerIntelligenceService>().Publish(requestContext, "PipelinePolicy.Approval", feature, properties);
        return approval2;
      }
    }

    public IEnumerable<Microsoft.Azure.Pipelines.Approval.WebApi.Approval> UpdateApprovals(
      IVssRequestContext requestContext,
      Guid projectId,
      IEnumerable<ApprovalUpdateParameters> updateParameters,
      Guid? userId = null)
    {
      ArgumentValidation.ValidateServiceBasicData(requestContext, projectId);
      using (new MethodScope(requestContext, nameof (ApprovalService), nameof (UpdateApprovals)))
      {
        ArgumentValidation.ValidateApprovalsUpdateParameters(requestContext, updateParameters);
        IList<ApprovalUpdateParameters> updateParametersList1 = (IList<ApprovalUpdateParameters>) new List<ApprovalUpdateParameters>();
        IList<ApprovalUpdateParameters> updateParametersList2 = (IList<ApprovalUpdateParameters>) new List<ApprovalUpdateParameters>();
        IList<ApprovalUpdateParameters> values = (IList<ApprovalUpdateParameters>) new List<ApprovalUpdateParameters>();
        IDictionary<string, IdentityRef> identities = (IDictionary<string, IdentityRef>) new Dictionary<string, IdentityRef>();
        Guid currentUserId = (requestContext.IsFeatureEnabled("Pipelines.Approvals.EnableDeferredApprovals") ? userId : new Guid?()) ?? requestContext.GetUserIdentity().Id;
        IList<string> stringList = (IList<string>) new List<string>()
        {
          currentUserId.ToString("D")
        };
        updateParameters = updateParameters.Distinct<ApprovalUpdateParameters>((IEqualityComparer<ApprovalUpdateParameters>) new ApprovalUpdateParametersComparer());
        List<Guid> list = updateParameters.Select<ApprovalUpdateParameters, Guid>((Func<ApprovalUpdateParameters, Guid>) (x => x.ApprovalId)).Distinct<Guid>().ToList<Guid>();
        IList<Microsoft.Azure.Pipelines.Approval.WebApi.Approval> approvalList;
        using (ApprovalComponent component = requestContext.CreateComponent<ApprovalComponent>())
          approvalList = component.QueryApprovals(projectId, (IList<Guid>) list);
        if (approvalList.Count != list.Count || !this.AreUpdateParametersInDatabase(updateParameters, (IEnumerable<Microsoft.Azure.Pipelines.Approval.WebApi.Approval>) approvalList))
        {
          requestContext.TraceError(34001711, nameof (ApprovalService), "No approvals found for the specified query.");
          throw new ApprovalNotFoundException(ApprovalResources.ApprovalNotFoundExceptionMessage());
        }
        IDictionary<Guid, Microsoft.Azure.Pipelines.Approval.WebApi.Approval> approvalIdToApprovals = (IDictionary<Guid, Microsoft.Azure.Pipelines.Approval.WebApi.Approval>) new Dictionary<Guid, Microsoft.Azure.Pipelines.Approval.WebApi.Approval>(approvalList.Count);
        approvalList.ForEach<Microsoft.Azure.Pipelines.Approval.WebApi.Approval>((Action<Microsoft.Azure.Pipelines.Approval.WebApi.Approval>) (approval =>
        {
          this.ThrowIfApprovalCompleted(requestContext, projectId, approval);
          approvalIdToApprovals[approval.Id] = approval;
        }));
        IEnumerable<ApprovalUpdateParameters> reassignParameters = updateParameters.Where<ApprovalUpdateParameters>((Func<ApprovalUpdateParameters, bool>) (x => x.ReassignTo != null && x.ReassignTo.Id != null));
        IEnumerable<ApprovalUpdateParameters> updateParameterses = updateParameters.Where<ApprovalUpdateParameters>((Func<ApprovalUpdateParameters, bool>) (x =>
        {
          ApprovalStatus? status = x.Status;
          ApprovalStatus approvalStatus = ApprovalStatus.Skipped;
          return status.GetValueOrDefault() == approvalStatus & status.HasValue;
        }));
        updateParameters = updateParameters.Except<ApprovalUpdateParameters>(reassignParameters).Except<ApprovalUpdateParameters>(updateParameterses);
        if (reassignParameters.Any<ApprovalUpdateParameters>())
          updateParametersList2 = this.ValidateReassignParameters(requestContext, currentUserId, reassignParameters, approvalIdToApprovals, stringList);
        identities = this.FetchRequiredIdentities(requestContext, (IEnumerable<Microsoft.Azure.Pipelines.Approval.WebApi.Approval>) approvalList, additionalIds: stringList);
        ISet<Guid> approvalIdsWithApprovalUpdatePermissions = this.FilterApprovalsWithRequiredPermissions(requestContext, projectId, (IEnumerable<Microsoft.Azure.Pipelines.Approval.WebApi.Approval>) approvalList, ApprovalPermissions.QueueBuild);
        if (updateParameters.Any<ApprovalUpdateParameters>())
          updateParametersList1 = this.ValidateUpdateParameters(requestContext, currentUserId, updateParameters, approvalIdToApprovals, identities, approvalIdsWithApprovalUpdatePermissions);
        ISet<Guid> approvalIdsWithAdminPermissions = this.FilterApprovalsWithRequiredPermissions(requestContext, projectId, (IEnumerable<Microsoft.Azure.Pipelines.Approval.WebApi.Approval>) approvalList, ApprovalPermissions.ResourceAdmin);
        if (updateParametersList2.Any<ApprovalUpdateParameters>())
          this.CheckResourceAdminPermissions(requestContext, (IEnumerable<ApprovalUpdateParameters>) updateParametersList2, identities, approvalIdsWithAdminPermissions);
        ISet<Guid> hashSet = (ISet<Guid>) updateParameterses.Select<ApprovalUpdateParameters, Guid>((Func<ApprovalUpdateParameters, Guid>) (x => x.ApprovalId)).ToHashSet<Guid>();
        if (requestContext.IsFeatureEnabled("Pipelines.Checks.EnableBypassApprovals") && updateParameterses.Any<ApprovalUpdateParameters>())
        {
          values = this.ValidateUpdateParameters(requestContext, currentUserId, updateParameterses, approvalIdToApprovals, identities, hashSet);
          this.CheckResourceAdminPermissions(requestContext, updateParameterses, identities, approvalIdsWithAdminPermissions);
        }
        requestContext.TraceInfo(34001709, nameof (ApprovalService), "Parameters validation completed.");
        updateParametersList1.AddRange<ApprovalUpdateParameters, IList<ApprovalUpdateParameters>>((IEnumerable<ApprovalUpdateParameters>) updateParametersList2).AddRange<ApprovalUpdateParameters, IList<ApprovalUpdateParameters>>((IEnumerable<ApprovalUpdateParameters>) values);
        using (ApprovalComponent component = requestContext.CreateComponent<ApprovalComponent>())
          approvalList = component.UpdateApprovals(projectId, currentUserId, (IEnumerable<ApprovalUpdateParameters>) updateParametersList1);
        ApprovalService.AuditApproverReassignments(requestContext, projectId, updateParametersList1);
        ApprovalService.AuditSkippedApprovals(requestContext, projectId, approvalList, hashSet);
        requestContext.TraceInfo(34001710, nameof (ApprovalService), "Update succeeded, {0} approvals updated.", (object) approvalList.Count<Microsoft.Azure.Pipelines.Approval.WebApi.Approval>());
        this.PopulateMissingIdentitiesData(requestContext, (IEnumerable<Microsoft.Azure.Pipelines.Approval.WebApi.Approval>) approvalList, identities);
        approvalList.ForEach<Microsoft.Azure.Pipelines.Approval.WebApi.Approval>((Action<Microsoft.Azure.Pipelines.Approval.WebApi.Approval>) (approval =>
        {
          this.FillComputedValues(requestContext, projectId, approval, identities: identities, hasAdminPermission: approvalIdsWithAdminPermissions.Contains(approval.Id));
          if (approval.IsCompleted || approval.Status == ApprovalStatus.Deferred)
          {
            IdentityRef skippedBy;
            if (approval.Status == ApprovalStatus.Skipped && identities.TryGetValue(currentUserId.ToString(), out skippedBy))
              this.SendApprovalSkippedNotificationEvent(requestContext, projectId, approval, skippedBy);
            else
              this.SendApprovalCompletedNotificationEvent(requestContext, projectId, approval);
          }
          else if (approval.ExecutionOrder == ApprovalExecutionOrder.InSequence)
          {
            this.SendApprovalPendingNotificationEvent(requestContext, projectId, approval);
          }
          else
          {
            ApprovalUpdateParameters reassignParameter = reassignParameters.ToList<ApprovalUpdateParameters>().Find((Predicate<ApprovalUpdateParameters>) (param => param.ApprovalId == approval.Id));
            if (reassignParameter != null && reassignParameter.ReassignTo != null)
            {
              approval.Steps.Where<ApprovalStep>((Func<ApprovalStep, bool>) (step => step.AssignedApprover.Id != reassignParameter.ReassignTo.Id)).ForEach<ApprovalStep>((Action<ApprovalStep>) (step => step.History.Clear()));
              this.SendApprovalPendingNotificationEvent(requestContext, projectId, approval);
            }
          }
          foreach (ApprovalUpdateParameters updateParameter in updateParameters.Where<ApprovalUpdateParameters>((Func<ApprovalUpdateParameters, bool>) (u =>
          {
            ApprovalStatus? status = u.Status;
            ApprovalStatus approvalStatus = ApprovalStatus.Deferred;
            return status.GetValueOrDefault() == approvalStatus & status.HasValue && u.ApprovalId == approval.Id;
          })))
            requestContext.GetExtension<IDeferredApprovalPlugin>().ScheduleDeferredApprovalJob(requestContext, projectId, currentUserId, updateParameter);
        }));
        return (IEnumerable<Microsoft.Azure.Pipelines.Approval.WebApi.Approval>) approvalList;
      }
    }

    private static void AuditApproverReassignments(
      IVssRequestContext requestContext,
      Guid projectId,
      IList<ApprovalUpdateParameters> approvalUpdateParameters)
    {
      if (!requestContext.IsFeatureEnabled("Pipelines.Checks.ApproverReassignmentAudit"))
        return;
      foreach (ApprovalUpdateParameters updateParameters in approvalUpdateParameters.Where<ApprovalUpdateParameters>((Func<ApprovalUpdateParameters, bool>) (x => x.ReassignTo?.Id != null)))
      {
        try
        {
          Microsoft.VisualStudio.Services.Identity.Identity identity = requestContext.GetService<IdentityService>().ReadIdentities(requestContext, (IList<Guid>) new List<Guid>()
          {
            new Guid(updateParameters.AssignedApprover.Id)
          }, QueryMembership.None, (IEnumerable<string>) null).FirstOrDefault<Microsoft.VisualStudio.Services.Identity.Identity>();
          Dictionary<string, object> dictionary = new Dictionary<string, object>()
          {
            {
              "ApprovalId",
              (object) updateParameters.ApprovalId
            },
            {
              "OldApproverUserId",
              (object) updateParameters.AssignedApprover.Id
            },
            {
              "OldApproverDisplayName",
              (object) (identity?.DisplayName ?? "")
            },
            {
              "NewApproverUserId",
              (object) updateParameters.ReassignTo.Id
            },
            {
              "NewApproverDisplayName",
              (object) updateParameters.ReassignTo.DisplayName
            },
            {
              "Comment",
              (object) updateParameters.Comment
            }
          };
          IVssRequestContext requestContext1 = requestContext;
          Dictionary<string, object> data = dictionary;
          Guid guid = projectId;
          Guid targetHostId = new Guid();
          Guid projectId1 = guid;
          requestContext1.LogAuditEvent("ApproverReassigned", data, targetHostId, projectId1);
          requestContext.TraceAlways(34001712, nameof (ApprovalService), string.Format("Reassigned approver of {0} from {1} to {2}", (object) updateParameters.ApprovalId, (object) updateParameters.AssignedApprover.Id, (object) updateParameters.ReassignTo.Id));
        }
        catch (Exception ex)
        {
          requestContext.TraceException(34001713, nameof (ApprovalService), ex);
        }
      }
    }

    private static void AuditSkippedApprovals(
      IVssRequestContext requestContext,
      Guid projectId,
      IList<Microsoft.Azure.Pipelines.Approval.WebApi.Approval> updatedApprovals,
      ISet<Guid> skippedApprovalIds)
    {
      if (!requestContext.IsFeatureEnabled("Pipelines.Checks.EnableBypassApprovals"))
        return;
      foreach (ApprovalStep approvalStep in updatedApprovals.Where<Microsoft.Azure.Pipelines.Approval.WebApi.Approval>((Func<Microsoft.Azure.Pipelines.Approval.WebApi.Approval, bool>) (approval => approval.Status == ApprovalStatus.Skipped && skippedApprovalIds.Contains(approval.Id))).SelectMany<Microsoft.Azure.Pipelines.Approval.WebApi.Approval, ApprovalStep>((Func<Microsoft.Azure.Pipelines.Approval.WebApi.Approval, IEnumerable<ApprovalStep>>) (approval => (IEnumerable<ApprovalStep>) approval.Steps)).Where<ApprovalStep>((Func<ApprovalStep, bool>) (step => step.Status == ApprovalStatus.Skipped)))
      {
        try
        {
          Dictionary<string, object> dictionary = new Dictionary<string, object>()
          {
            {
              "ApprovalId",
              (object) approvalStep.ApprovalId
            },
            {
              "AssignedApproverUserId",
              (object) approvalStep.AssignedApprover.Id
            },
            {
              "AssignedApproverDisplayName",
              (object) (approvalStep.AssignedApprover?.DisplayName ?? "")
            },
            {
              "SkippedByUserId",
              (object) approvalStep.ActualApprover.Id
            },
            {
              "SkippedByDisplayName",
              (object) approvalStep.ActualApprover?.DisplayName
            },
            {
              "Comment",
              (object) approvalStep.Comment
            }
          };
          IVssRequestContext requestContext1 = requestContext;
          Dictionary<string, object> data = dictionary;
          Guid guid = projectId;
          Guid targetHostId = new Guid();
          Guid projectId1 = guid;
          requestContext1.LogAuditEvent("ApprovalSkipped", data, targetHostId, projectId1);
          requestContext.TraceAlways(34001716, nameof (ApprovalService), string.Format("Skipped {0} approval by {1}", (object) approvalStep.ApprovalId, (object) approvalStep.ActualApprover.Id));
        }
        catch (Exception ex)
        {
          requestContext.TraceException(34001718, nameof (ApprovalService), ex);
        }
      }
    }

    public IEnumerable<Microsoft.Azure.Pipelines.Approval.WebApi.Approval> QueryApprovals(
      IVssRequestContext requestContext,
      Guid projectId,
      ApprovalsQueryParameters queryParameters,
      ApprovalDetailsExpandParameter expand = ApprovalDetailsExpandParameter.None)
    {
      ArgumentValidation.ValidateServiceBasicData(requestContext, projectId);
      using (new MethodScope(requestContext, nameof (ApprovalService), nameof (QueryApprovals)))
      {
        IEnumerable<Microsoft.Azure.Pipelines.Approval.WebApi.Approval> approvalsByApprovalIds = this.GetApprovalsByApprovalIds(requestContext, projectId, queryParameters);
        return this.GetPopulatedApprovals(requestContext, projectId, approvalsByApprovalIds, expand);
      }
    }

    public IEnumerable<Microsoft.Azure.Pipelines.Approval.WebApi.Approval> QueryApprovalsExtended(
      IVssRequestContext requestContext,
      Guid projectId,
      ApprovalsQueryParameters queryParameters,
      ApprovalDetailsExpandParameter expand = ApprovalDetailsExpandParameter.None)
    {
      ArgumentValidation.ValidateServiceBasicData(requestContext, projectId);
      using (new MethodScope(requestContext, nameof (ApprovalService), nameof (QueryApprovalsExtended)))
      {
        IEnumerable<Microsoft.Azure.Pipelines.Approval.WebApi.Approval> approvals;
        if (requestContext.IsFeatureEnabled("Pipelines.Policy.UseEnhancedQueryApprovalsApi"))
        {
          this.MergeUserIdentifiersAndGroupsIntoUserIds(requestContext, queryParameters);
          approvals = this.GetApprovals(requestContext, projectId, queryParameters);
        }
        else
          approvals = this.GetApprovalsByApprovalIds(requestContext, projectId, queryParameters);
        if (approvals == null || approvals.Count<Microsoft.Azure.Pipelines.Approval.WebApi.Approval>() == 0)
          return approvals;
        IEnumerable<Microsoft.Azure.Pipelines.Approval.WebApi.Approval> source = this.GetPopulatedApprovals(requestContext, projectId, approvals, expand);
        if (queryParameters.ApproverStatus != ApprovalStatus.All)
          source = source.Where<Microsoft.Azure.Pipelines.Approval.WebApi.Approval>((Func<Microsoft.Azure.Pipelines.Approval.WebApi.Approval, bool>) (a => (a.Status & queryParameters.ApproverStatus) > ApprovalStatus.Undefined));
        return source;
      }
    }

    public void DeleteApprovals(
      IVssRequestContext requestContext,
      Guid projectId,
      IList<Guid> approvalIds)
    {
      ArgumentValidation.ValidateServiceBasicData(requestContext, projectId);
      using (new MethodScope(requestContext, nameof (ApprovalService), nameof (DeleteApprovals)))
      {
        ArgumentUtility.CheckForNull<IList<Guid>>(approvalIds, nameof (approvalIds), "PipelinePolicy.Approval");
        using (ApprovalComponent component = requestContext.CreateComponent<ApprovalComponent>())
          component.DeleteApprovals(projectId, approvalIds);
      }
    }

    public void ServiceStart(IVssRequestContext requestContext)
    {
    }

    public void ServiceEnd(IVssRequestContext requestContext)
    {
    }

    internal ApprovalConfig FillConfigDefaults(ApprovalConfig config)
    {
      if (config.MinRequiredApprovers.HasValue)
      {
        int? requiredApprovers = config.MinRequiredApprovers;
        int num = 0;
        if (!(requiredApprovers.GetValueOrDefault() == num & requiredApprovers.HasValue))
          goto label_3;
      }
      config.MinRequiredApprovers = new int?(config.Approvers.Count);
label_3:
      if (config.ExecutionOrder != ApprovalExecutionOrder.AnyOrder && config.ExecutionOrder != ApprovalExecutionOrder.InSequence)
        config.ExecutionOrder = ApprovalExecutionOrder.AnyOrder;
      return config;
    }

    internal IList<string> GetIdentityIdsFromApproval(Microsoft.Azure.Pipelines.Approval.WebApi.Approval approval, bool includeSteps = true)
    {
      List<string> collection = new List<string>();
      if (approval.BlockedApprovers != null && approval.BlockedApprovers.Count > 0)
        collection.AddRangeIfRangeNotNull<string, List<string>>(approval.BlockedApprovers.Select<IdentityRef, string>((Func<IdentityRef, string>) (blockedApprover => blockedApprover.Id)));
      if (includeSteps && approval.Steps != null && approval.Steps.Count > 0)
      {
        foreach (ApprovalStep step in approval.Steps)
        {
          if (step.AssignedApprover != null && step.AssignedApprover.Id != null)
            collection.Add(step.AssignedApprover.Id);
          if (step.ActualApprover != null && step.ActualApprover.Id != null)
            collection.Add(step.ActualApprover.Id);
          if (step.LastModifiedBy != null && step.LastModifiedBy.Id != null)
            collection.Add(step.LastModifiedBy.Id);
          foreach (ApprovalStepHistory approvalStepHistory in (IEnumerable<ApprovalStepHistory>) step.History)
          {
            if (approvalStepHistory.AssignedTo != null && approvalStepHistory.AssignedTo.Id != null)
              collection.Add(approvalStepHistory.AssignedTo.Id);
            if (approvalStepHistory.CreatedBy != null && approvalStepHistory.CreatedBy.Id != null)
              collection.Add(approvalStepHistory.CreatedBy.Id);
          }
        }
      }
      return (IList<string>) collection;
    }

    private IEnumerable<Microsoft.Azure.Pipelines.Approval.WebApi.Approval> GetApprovalsByApprovalIds(
      IVssRequestContext requestContext,
      Guid projectId,
      ApprovalsQueryParameters queryParameters)
    {
      ArgumentValidation.ValidateApprovalsQueryParameters(queryParameters);
      List<Guid> list = queryParameters.ApprovalIds.Distinct<Guid>().ToList<Guid>();
      using (ApprovalComponent component = requestContext.CreateComponent<ApprovalComponent>())
        return (IEnumerable<Microsoft.Azure.Pipelines.Approval.WebApi.Approval>) component.QueryApprovals(projectId, (IList<Guid>) list);
    }

    private IEnumerable<Microsoft.Azure.Pipelines.Approval.WebApi.Approval> GetApprovals(
      IVssRequestContext requestContext,
      Guid projectId,
      ApprovalsQueryParameters queryParameters)
    {
      ArgumentValidation.ValidateApprovalsQueryAllParameters(queryParameters);
      IEnumerable<Microsoft.Azure.Pipelines.Approval.WebApi.Approval> approvals = (IEnumerable<Microsoft.Azure.Pipelines.Approval.WebApi.Approval>) null;
      List<Guid> list1 = queryParameters.ApprovalIds.Distinct<Guid>().ToList<Guid>();
      List<Guid> list2 = queryParameters.UserIds.Where<Guid>((Func<Guid, bool>) (uid => uid != Guid.Empty)).Distinct<Guid>().ToList<Guid>();
      int val2 = requestContext.GetService<IVssRegistryService>().GetValue<int>(requestContext, (RegistryQuery) RegistryKeys.QueryApprovalsCount, 250);
      using (ApprovalComponent component = requestContext.CreateComponent<ApprovalComponent>())
      {
        if (component is ApprovalComponent6 approvalComponent6)
        {
          Guid projectId1 = projectId;
          List<Guid> approvalIds = list1;
          List<Guid> approverIds = list2;
          int approverStatus = (int) queryParameters.ApproverStatus;
          int rowCount = queryParameters.Top > 0 ? Math.Min(queryParameters.Top, val2) : val2;
          approvals = (IEnumerable<Microsoft.Azure.Pipelines.Approval.WebApi.Approval>) approvalComponent6.QueryApprovals(projectId1, (IList<Guid>) approvalIds, (IList<Guid>) approverIds, (ApprovalStatus) approverStatus, rowCount);
        }
      }
      return approvals;
    }

    private void MergeUserIdentifiersAndGroupsIntoUserIds(
      IVssRequestContext requestContext,
      ApprovalsQueryParameters queryParameters)
    {
      List<Microsoft.VisualStudio.Services.Identity.Identity> fromDifferentTypes = this.ParseAndMergeIdentitiesFromDifferentTypes(requestContext, queryParameters);
      if (!requestContext.IsFeatureEnabled("Pipelines.Policy.EnableSearchInGroupsForQueryApprovalsApi"))
      {
        queryParameters.UserIds.AddRange(fromDifferentTypes.Select<Microsoft.VisualStudio.Services.Identity.Identity, Guid>((Func<Microsoft.VisualStudio.Services.Identity.Identity, Guid>) (i => i.Id)));
      }
      else
      {
        IdentityService service = requestContext.GetService<IdentityService>();
        IList<Microsoft.VisualStudio.Services.Identity.Identity> collection = (IList<Microsoft.VisualStudio.Services.Identity.Identity>) new List<Microsoft.VisualStudio.Services.Identity.Identity>();
        if (queryParameters.UserIds != null && queryParameters.UserIds.Any<Guid>())
          collection = service.ReadIdentities(requestContext, (IList<Guid>) queryParameters.UserIds, QueryMembership.Expanded, (IEnumerable<string>) null);
        queryParameters.UserIds.AddRange(fromDifferentTypes.Select<Microsoft.VisualStudio.Services.Identity.Identity, Guid>((Func<Microsoft.VisualStudio.Services.Identity.Identity, Guid>) (i => i.Id)));
        collection.AddRange<Microsoft.VisualStudio.Services.Identity.Identity, IList<Microsoft.VisualStudio.Services.Identity.Identity>>((IEnumerable<Microsoft.VisualStudio.Services.Identity.Identity>) fromDifferentTypes);
        if (collection.Count == 0)
          return;
        HashSet<SubjectDescriptor> source1 = new HashSet<SubjectDescriptor>();
        foreach (IdentityBase identityBase in (IEnumerable<Microsoft.VisualStudio.Services.Identity.Identity>) collection)
        {
          foreach (IdentityDescriptor identityDescriptor in (IEnumerable<IdentityDescriptor>) identityBase.MemberOf)
            source1.Add(identityDescriptor.ToSubjectDescriptor(requestContext));
        }
        if (source1.Count > 20)
          requestContext.TraceAlways(34001714, nameof (ApprovalService), string.Format("We are trying to query for approvals assigned to {0} groups - this could slow down the query", (object) source1.Count));
        IList<Microsoft.VisualStudio.Services.Identity.Identity> source2 = service.ReadIdentities(requestContext, (IList<SubjectDescriptor>) source1.ToList<SubjectDescriptor>(), QueryMembership.None, (IEnumerable<string>) null);
        queryParameters.UserIds.AddRange(source2.Select<Microsoft.VisualStudio.Services.Identity.Identity, Guid>((Func<Microsoft.VisualStudio.Services.Identity.Identity, Guid>) (i => i.Id)));
      }
    }

    private List<Microsoft.VisualStudio.Services.Identity.Identity> ParseAndMergeIdentitiesFromDifferentTypes(
      IVssRequestContext requestContext,
      ApprovalsQueryParameters queryParameters)
    {
      List<SubjectDescriptor> subjectDescriptors;
      List<string> displayNames;
      this.ParseAssignedToIntoTypedIdentities(requestContext, queryParameters, out subjectDescriptors, out displayNames);
      IdentityService service = requestContext.GetService<IdentityService>();
      List<Microsoft.VisualStudio.Services.Identity.Identity> source = new List<Microsoft.VisualStudio.Services.Identity.Identity>();
      if (subjectDescriptors.Count != 0)
        source.AddRange((IEnumerable<Microsoft.VisualStudio.Services.Identity.Identity>) service.ReadIdentities(requestContext, (IList<SubjectDescriptor>) subjectDescriptors, QueryMembership.Expanded, (IEnumerable<string>) null));
      if (displayNames.Count != 0 && requestContext.IsFeatureEnabled("Pipelines.Policy.EnableSearchByEmailForQueryApprovalsApi"))
      {
        foreach (string factorValue in displayNames)
        {
          IList<Microsoft.VisualStudio.Services.Identity.Identity> collection = service.ReadIdentities(requestContext, IdentitySearchFilter.MailAddress, factorValue, QueryMembership.Expanded, (IEnumerable<string>) null);
          if (collection == null || collection.Count == 0)
            throw new ArgumentException("We were not able to find an identity for " + factorValue);
          source.AddRange((IEnumerable<Microsoft.VisualStudio.Services.Identity.Identity>) collection);
        }
      }
      return source.Where<Microsoft.VisualStudio.Services.Identity.Identity>((Func<Microsoft.VisualStudio.Services.Identity.Identity, bool>) (i => i != null)).ToList<Microsoft.VisualStudio.Services.Identity.Identity>();
    }

    private void ParseAssignedToIntoTypedIdentities(
      IVssRequestContext requestContext,
      ApprovalsQueryParameters queryParameters,
      out List<SubjectDescriptor> subjectDescriptors,
      out List<string> displayNames)
    {
      displayNames = new List<string>();
      subjectDescriptors = new List<SubjectDescriptor>();
      foreach (string str in queryParameters.AssignedTo)
      {
        Guid result;
        if (Guid.TryParse(str, out result))
        {
          queryParameters.UserIds.Add(result);
        }
        else
        {
          SubjectDescriptor subjectDescriptor = SubjectDescriptor.FromString(str);
          if (subjectDescriptor.SubjectType == "ukn" && subjectDescriptor.Identifier.Equals(str, StringComparison.OrdinalIgnoreCase))
          {
            if (!ArgumentUtility.IsValidEmailAddress(str))
            {
              requestContext.TraceError(34001715, nameof (ApprovalService), "AssignedTo contains invalid identity)");
              throw new ArgumentException("Invalid identity: " + str);
            }
            displayNames.Add(str);
          }
          else
            subjectDescriptors.Add(subjectDescriptor);
        }
      }
    }

    private IList<ApprovalUpdateParameters> ValidateUpdateParameters(
      IVssRequestContext requestContext,
      Guid currentUserId,
      IEnumerable<ApprovalUpdateParameters> updateParameters,
      IDictionary<Guid, Microsoft.Azure.Pipelines.Approval.WebApi.Approval> approvalIdToApprovals,
      IDictionary<string, IdentityRef> identities,
      ISet<Guid> approvalIdsWithApprovalUpdatePermissions)
    {
      IList<string> stringList = (IList<string>) new List<string>();
      IList<Microsoft.Azure.Pipelines.Approval.WebApi.Approval> approvalList = (IList<Microsoft.Azure.Pipelines.Approval.WebApi.Approval>) new List<Microsoft.Azure.Pipelines.Approval.WebApi.Approval>();
      IList<ApprovalUpdateParameters> approvalUpdateParameters = (IList<ApprovalUpdateParameters>) new List<ApprovalUpdateParameters>();
      bool flag = updateParameters.Any<ApprovalUpdateParameters>((Func<ApprovalUpdateParameters, bool>) (x =>
      {
        ApprovalStatus? status = x.Status;
        ApprovalStatus approvalStatus = ApprovalStatus.Canceled;
        return status.GetValueOrDefault() == approvalStatus & status.HasValue;
      }));
      foreach (ApprovalUpdateParameters updateParameter in updateParameters)
      {
        Microsoft.Azure.Pipelines.Approval.WebApi.Approval approvalIdToApproval = approvalIdToApprovals[updateParameter.ApprovalId];
        if (!flag)
        {
          if (approvalIdToApproval.BlockedApprovers.Where<IdentityRef>((Func<IdentityRef, bool>) (blockedApprover => blockedApprover.Id == currentUserId.ToString("D"))).Count<IdentityRef>() > 0)
            throw new ApprovalBlockedException(ApprovalResources.ApprovalBlockedExceptionMessage((object) currentUserId, (object) approvalIdToApproval.Id));
          stringList.AddRange<string, IList<string>>(approvalIdToApproval.BlockedApprovers.Select<IdentityRef, string>((Func<IdentityRef, string>) (blockedApprover => blockedApprover.Id)));
        }
        approvalList.Add(approvalIdToApproval);
      }
      if (flag)
      {
        approvalUpdateParameters = (IList<ApprovalUpdateParameters>) updateParameters.ToList<ApprovalUpdateParameters>();
      }
      else
      {
        IDictionary<Guid, IList<int>> approvalIdToStepIds;
        this.VerifyAssignedAndBlockedApprovers(requestContext, identities, stringList, (IEnumerable<Microsoft.Azure.Pipelines.Approval.WebApi.Approval>) approvalList, approvalIdsWithApprovalUpdatePermissions, currentUserId, out approvalIdToStepIds);
        this.ThrowIfNoStepsCanBeUpdated((IEnumerable<Microsoft.Azure.Pipelines.Approval.WebApi.Approval>) approvalList, approvalIdToStepIds);
        updateParameters.ForEach<ApprovalUpdateParameters>((Action<ApprovalUpdateParameters>) (updateParameter =>
        {
          IList<int> intList = approvalIdToStepIds[updateParameter.ApprovalId];
          for (int index = 0; index < intList.Count; ++index)
            approvalUpdateParameters.Add(new ApprovalUpdateParameters()
            {
              ApprovalId = updateParameter.ApprovalId,
              Comment = updateParameter.Comment,
              Status = updateParameter.Status,
              DeferredTo = updateParameter.DeferredTo,
              StepId = new int?(intList[index])
            });
        }));
      }
      return approvalUpdateParameters;
    }

    private IList<ApprovalUpdateParameters> ValidateReassignParameters(
      IVssRequestContext requestContext,
      Guid currentUserId,
      IEnumerable<ApprovalUpdateParameters> approvalReassignParameters,
      IDictionary<Guid, Microsoft.Azure.Pipelines.Approval.WebApi.Approval> approvalIdToApprovals,
      IList<string> additionalIdsToFetch)
    {
      approvalReassignParameters.ForEach<ApprovalUpdateParameters>((Action<ApprovalUpdateParameters>) (reassignParameter =>
      {
        Microsoft.Azure.Pipelines.Approval.WebApi.Approval approvalIdToApproval = approvalIdToApprovals[reassignParameter.ApprovalId];
        ApprovalStep approvalStep = reassignParameter.AssignedApprover == null || reassignParameter.AssignedApprover.Id == null ? approvalIdToApproval.Steps.Where<ApprovalStep>((Func<ApprovalStep, bool>) (step => step.AssignedApprover.Id.Equals(currentUserId.ToString(), StringComparison.OrdinalIgnoreCase))).FirstOrDefault<ApprovalStep>() : approvalIdToApproval.Steps.Where<ApprovalStep>((Func<ApprovalStep, bool>) (step => step.AssignedApprover.Id.Equals(reassignParameter.AssignedApprover.Id, StringComparison.OrdinalIgnoreCase))).FirstOrDefault<ApprovalStep>();
        if (approvalStep == null || approvalStep.Status != ApprovalStatus.Uninitiated && approvalStep.Status != ApprovalStatus.Pending)
        {
          requestContext.TraceError(34001706, nameof (ApprovalService), "The reassign request for approval id '{0}' is invalid. Could not find step with provided assigned approver or with the current user as assigned approver.", (object) approvalIdToApproval.Id);
          throw new ApprovalStateChangeException(ApprovalResources.ApprovalReassignRequestFailed((object) approvalIdToApproval.Id));
        }
        reassignParameter.StepId = new int?(approvalStep.StepId);
        reassignParameter.AssignedApprover = approvalStep.AssignedApprover;
        additionalIdsToFetch.Add(reassignParameter.ReassignTo.Id);
      }));
      return (IList<ApprovalUpdateParameters>) approvalReassignParameters.ToList<ApprovalUpdateParameters>();
    }

    private void CheckResourceAdminPermissions(
      IVssRequestContext requestContext,
      IEnumerable<ApprovalUpdateParameters> approvalRequestParameters,
      IDictionary<string, IdentityRef> identities,
      ISet<Guid> approvalIdsWithAdminPermissions)
    {
      IdentityRef currentUser = requestContext.GetUserIdentity().ToIdentityRef(requestContext, false);
      IdentityService identityService = requestContext.GetService<IdentityService>();
      approvalRequestParameters.ForEach<ApprovalUpdateParameters>((Action<ApprovalUpdateParameters>) (requestParameter =>
      {
        ApprovalStatus? status = requestParameter.Status;
        ApprovalStatus approvalStatus = ApprovalStatus.Skipped;
        if (status.GetValueOrDefault() == approvalStatus & status.HasValue)
        {
          if (!approvalIdsWithAdminPermissions.Contains(requestParameter.ApprovalId))
          {
            requestContext.TraceError(34001717, nameof (ApprovalService), "The skipped request for approval id '{0}' is invalid. User does not have permissions to skip the approval.", (object) requestParameter.ApprovalId);
            throw new ApprovalUnauthorizedException(ApprovalResources.ApprovalSkippedUnauthorizedExceptionMessage((object) currentUser.Id, (object) requestParameter.ApprovalId));
          }
        }
        else if (!this.IsUserOrMember(requestContext, identityService, identities[requestParameter.AssignedApprover.Id], currentUser) && !approvalIdsWithAdminPermissions.Contains(requestParameter.ApprovalId))
        {
          requestContext.TraceError(34001707, nameof (ApprovalService), "The reassign request for approval id '{0}' is invalid. User does not have permissions to reassign approval step.", (object) requestParameter.ApprovalId);
          throw new ApprovalUnauthorizedException(ApprovalResources.ApprovalReassignUnauthorizedExceptionMessage((object) currentUser.Id, (object) requestParameter.AssignedApprover.Id));
        }
      }));
    }

    private ISet<Guid> FilterApprovalsWithRequiredPermissions(
      IVssRequestContext requestContext,
      Guid projectId,
      IEnumerable<Microsoft.Azure.Pipelines.Approval.WebApi.Approval> approvals,
      ApprovalPermissions permission)
    {
      IDictionary<Guid, JObject> approvalIdsToContext = (IDictionary<Guid, JObject>) new Dictionary<Guid, JObject>();
      approvals.ForEach<Microsoft.Azure.Pipelines.Approval.WebApi.Approval>((Action<Microsoft.Azure.Pipelines.Approval.WebApi.Approval>) (approval => approvalIdsToContext.Add(approval.Id, approval.Context)));
      ApprovalOwner? nullable1 = !approvals.IsNullOrEmpty<Microsoft.Azure.Pipelines.Approval.WebApi.Approval>() ? new ApprovalOwner?(approvals.First<Microsoft.Azure.Pipelines.Approval.WebApi.Approval>().Owner) : new ApprovalOwner?();
      using (IDisposableReadOnlyList<IApprovalDetailsProvider> extensions = requestContext.GetExtensions<IApprovalDetailsProvider>())
      {
        foreach (IApprovalDetailsProvider approvalDetailsProvider in (IEnumerable<IApprovalDetailsProvider>) extensions)
        {
          int approvalOwner = (int) approvalDetailsProvider.GetApprovalOwner();
          ApprovalOwner? nullable2 = nullable1;
          int valueOrDefault = (int) nullable2.GetValueOrDefault();
          if (approvalOwner == valueOrDefault & nullable2.HasValue)
            return (ISet<Guid>) approvalDetailsProvider.FilterApprovalsWithRequiredPermissions(requestContext, projectId, approvalIdsToContext, permission).ToHashSet<Guid>();
        }
        return (ISet<Guid>) ApprovalService.s_emptyGuidSet;
      }
    }

    private IDictionary<string, IdentityRef> FetchRequiredIdentities(
      IVssRequestContext requestContext,
      IEnumerable<Microsoft.Azure.Pipelines.Approval.WebApi.Approval> approvals,
      bool includeSteps = true,
      IList<string> additionalIds = null)
    {
      List<string> identitiesToFetch = new List<string>();
      approvals.ForEach<Microsoft.Azure.Pipelines.Approval.WebApi.Approval>((Action<Microsoft.Azure.Pipelines.Approval.WebApi.Approval>) (approval => identitiesToFetch.AddRangeIfRangeNotNull<string, List<string>>((IEnumerable<string>) this.GetIdentityIdsFromApproval(approval, includeSteps))));
      identitiesToFetch.AddRangeIfRangeNotNull<string, List<string>>((IEnumerable<string>) additionalIds);
      identitiesToFetch = identitiesToFetch.Distinct<string>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase).ToList<string>();
      IDictionary<string, IdentityRef> dictionary = (IDictionary<string, IdentityRef>) new Dictionary<string, IdentityRef>();
      if (identitiesToFetch != null && identitiesToFetch.Count > 0)
        dictionary = requestContext.GetService<IdentityService>().QueryIdentities(requestContext, (IList<string>) identitiesToFetch);
      return dictionary;
    }

    private void FillComputedValues(
      IVssRequestContext requestContext,
      Guid projectId,
      Microsoft.Azure.Pipelines.Approval.WebApi.Approval approval,
      bool includeSteps = true,
      IDictionary<string, IdentityRef> identities = null,
      bool includePermissions = false,
      IdentityRef currentApproverFilterIdentity = null,
      IdentityService identityService = null,
      bool hasAdminPermission = false,
      bool hasUpdatePermission = false,
      ApprovalDetailsExpandParameter expand = ApprovalDetailsExpandParameter.None)
    {
      DateTime dateTime1 = DateTime.MinValue;
      IDictionary<ApprovalStatus, int> approvalStatusToCountMap = (IDictionary<ApprovalStatus, int>) new Dictionary<ApprovalStatus, int>();
      foreach (ApprovalStep step in approval.Steps)
      {
        if (step.LastModifiedOn > dateTime1)
          dateTime1 = step.LastModifiedOn;
        if (approvalStatusToCountMap.ContainsKey(step.Status))
          approvalStatusToCountMap[step.Status]++;
        else
          approvalStatusToCountMap.Add(step.Status, 1);
      }
      DateTime initiatedOn = (DateTime?) approval.Steps.FirstOrDefault<ApprovalStep>()?.InitiatedOn ?? DateTime.MinValue;
      approval.ExecutionOrder = !approval.Steps.All<ApprovalStep>((Func<ApprovalStep, bool>) (step =>
      {
        DateTime? initiatedOn1 = step.InitiatedOn;
        DateTime dateTime2 = initiatedOn;
        if (!initiatedOn1.HasValue)
          return false;
        return !initiatedOn1.HasValue || initiatedOn1.GetValueOrDefault() == dateTime2;
      })) ? ApprovalExecutionOrder.InSequence : ApprovalExecutionOrder.AnyOrder;
      approval.LastModifiedOn = dateTime1;
      IDictionary<string, string> parameters = (IDictionary<string, string>) new Dictionary<string, string>();
      if (expand != ApprovalDetailsExpandParameter.None)
        parameters.Add("$expand", expand.ToString("D"));
      string approvalRestUri = this.GetApprovalRestUri(requestContext, projectId, approval.Id, parameters);
      approval.Links = new ReferenceLinks();
      approval.Links.AddLink("self", approvalRestUri);
      JToken jtoken1 = approval?.Context?.SelectToken("EvaluationContext.Pipeline");
      if (jtoken1 != null)
      {
        JToken jtoken2 = this.DeserializeWithLowerCasePropertyNames(jtoken1?.ToString());
        approval.Pipeline = jtoken2?.ToObject<JObject>();
      }
      approval.Status = this.GetOverallApprovalStatus(approval.MinRequiredApprovers.Value, approvalStatusToCountMap);
      if (includePermissions)
        approval = this.PopulateIfUserCanApproveStepField(requestContext, identityService, identities, approval, currentApproverFilterIdentity, hasAdminPermission, hasUpdatePermission);
      approval.Steps = includeSteps ? approval.Steps : (List<ApprovalStep>) null;
      if (identities == null)
        return;
      if (approval.BlockedApprovers != null && approval.BlockedApprovers.Count > 0)
      {
        for (int index = 0; index < approval.BlockedApprovers.Count; ++index)
        {
          if (approval.BlockedApprovers[index] != null && !string.IsNullOrWhiteSpace(approval.BlockedApprovers[index].Id))
            approval.BlockedApprovers[index] = this.GetIdentityFromId(identities, approval.BlockedApprovers[index].Id);
        }
      }
      if (approval.Steps == null || approval.Steps.Count <= 0)
        return;
      for (int index = 0; index < approval.Steps.Count; ++index)
      {
        if (approval.Steps[index].AssignedApprover != null && !string.IsNullOrWhiteSpace(approval.Steps[index].AssignedApprover.Id))
          approval.Steps[index].AssignedApprover = this.GetIdentityFromId(identities, approval.Steps[index].AssignedApprover.Id);
        if (approval.Steps[index].ActualApprover != null && !string.IsNullOrWhiteSpace(approval.Steps[index].ActualApprover.Id))
          approval.Steps[index].ActualApprover = this.GetIdentityFromId(identities, approval.Steps[index].ActualApprover.Id);
        if (approval.Steps[index].LastModifiedBy != null && !string.IsNullOrWhiteSpace(approval.Steps[index].LastModifiedBy.Id))
          approval.Steps[index].LastModifiedBy = this.GetIdentityFromId(identities, approval.Steps[index].LastModifiedBy.Id);
        foreach (ApprovalStepHistory approvalStepHistory in (IEnumerable<ApprovalStepHistory>) approval.Steps[index].History)
        {
          if (approvalStepHistory.AssignedTo != null && !string.IsNullOrWhiteSpace(approvalStepHistory.AssignedTo.Id))
            approvalStepHistory.AssignedTo = this.GetIdentityFromId(identities, approvalStepHistory.AssignedTo.Id);
          if (approvalStepHistory.CreatedBy != null && !string.IsNullOrWhiteSpace(approvalStepHistory.CreatedBy.Id))
            approvalStepHistory.CreatedBy = this.GetIdentityFromId(identities, approvalStepHistory.CreatedBy.Id);
        }
      }
    }

    private IdentityRef GetIdentityFromId(
      IDictionary<string, IdentityRef> identities,
      string identityId)
    {
      if (identities.ContainsKey(identityId) && identities[identityId] != null)
        return identities[identityId];
      return new IdentityRef() { Id = identityId };
    }

    private ApprovalStatus GetOverallApprovalStatus(
      int MinRequiredApprovers,
      IDictionary<ApprovalStatus, int> approvalStatusToCountMap)
    {
      ApprovalStatus overallApprovalStatus = ApprovalStatus.Pending;
      if (approvalStatusToCountMap.ContainsKey(ApprovalStatus.Canceled))
        overallApprovalStatus = ApprovalStatus.Canceled;
      else if (approvalStatusToCountMap.ContainsKey(ApprovalStatus.Rejected))
        overallApprovalStatus = ApprovalStatus.Rejected;
      else if (approvalStatusToCountMap.ContainsKey(ApprovalStatus.TimedOut))
        overallApprovalStatus = ApprovalStatus.TimedOut;
      else if (approvalStatusToCountMap.ContainsKey(ApprovalStatus.Skipped))
        overallApprovalStatus = ApprovalStatus.Skipped;
      else if (approvalStatusToCountMap.ContainsKey(ApprovalStatus.Approved) && approvalStatusToCountMap[ApprovalStatus.Approved] >= MinRequiredApprovers)
        overallApprovalStatus = ApprovalStatus.Approved;
      if (overallApprovalStatus == ApprovalStatus.Pending && approvalStatusToCountMap.ContainsKey(ApprovalStatus.Deferred))
      {
        int num;
        approvalStatusToCountMap.TryGetValue(ApprovalStatus.Approved, out num);
        if (approvalStatusToCountMap[ApprovalStatus.Deferred] + num >= MinRequiredApprovers)
          overallApprovalStatus = ApprovalStatus.Deferred;
      }
      return overallApprovalStatus;
    }

    private void SendApprovalPendingNotificationEvent(
      IVssRequestContext requestContext,
      Guid projectId,
      Microsoft.Azure.Pipelines.Approval.WebApi.Approval approval)
    {
      requestContext.GetService<IApprovalEventPublisherService>().NotifyApprovalPendingEvent(requestContext, projectId, approval);
    }

    private void SendApprovalCompletedNotificationEvent(
      IVssRequestContext requestContext,
      Guid projectId,
      Microsoft.Azure.Pipelines.Approval.WebApi.Approval approval)
    {
      requestContext.GetService<IApprovalEventPublisherService>().NotifyApprovalCompletedEvent(requestContext, projectId, approval);
    }

    private void SendApprovalSkippedNotificationEvent(
      IVssRequestContext requestContext,
      Guid projectId,
      Microsoft.Azure.Pipelines.Approval.WebApi.Approval approval,
      IdentityRef skippedBy)
    {
      requestContext.GetService<IApprovalEventPublisherService>().NotifyApprovalSkippedEvent(requestContext, projectId, approval, skippedBy);
    }

    private void ThrowIfApprovalCompleted(
      IVssRequestContext requestContext,
      Guid projectId,
      Microsoft.Azure.Pipelines.Approval.WebApi.Approval approval)
    {
      this.FillComputedValues(requestContext, projectId, approval);
      if (approval.IsCompleted)
        throw new ApprovalAlreadyCompletedException(ApprovalResources.ApprovalAlreadyCompletedExceptionMessage());
    }

    private void VerifyAssignedAndBlockedApprovers(
      IVssRequestContext requestContext,
      IDictionary<string, IdentityRef> identities,
      IList<string> blockedApprovers,
      IEnumerable<Microsoft.Azure.Pipelines.Approval.WebApi.Approval> approvals,
      ISet<Guid> approvalIdsWithApprovalUpdatePermissions,
      Guid approverId,
      out IDictionary<Guid, IList<int>> approvalIdToStepIds)
    {
      IdentityRef currentApprover = identities[approverId.ToString()];
      approvalIdToStepIds = (IDictionary<Guid, IList<int>>) new Dictionary<Guid, IList<int>>();
      IdentityService identityService = requestContext.GetService<IdentityService>();
      IList<string> list = (IList<string>) blockedApprovers.Where<string>((Func<string, bool>) (blockedApprover => identities[blockedApprover].IsContainer)).ToList<string>();
      if (!requestContext.IsSystemContext)
      {
        foreach (string str in (IEnumerable<string>) list)
        {
          string blockedApprover = str;
          if (!identityService.IsMember(requestContext, identities[blockedApprover].Descriptor, currentApprover.Descriptor))
          {
            Guid id = approvals.Where<Microsoft.Azure.Pipelines.Approval.WebApi.Approval>((Func<Microsoft.Azure.Pipelines.Approval.WebApi.Approval, bool>) (approval => approval.BlockedApprovers.Select<IdentityRef, string>((Func<IdentityRef, string>) (approver => approver.Id)).Contains<string>(blockedApprover))).First<Microsoft.Azure.Pipelines.Approval.WebApi.Approval>().Id;
            throw new ApprovalBlockedException(ApprovalResources.ApprovalBlockedExceptionMessage((object) approverId.ToString("D"), (object) id));
          }
        }
      }
      foreach (Microsoft.Azure.Pipelines.Approval.WebApi.Approval approval in approvals)
      {
        bool authorizedForThisApproval = false;
        IList<int> currentAuthorizedSteps = (IList<int>) new List<int>();
        bool hasUpdatePermission = approvalIdsWithApprovalUpdatePermissions.Contains(approval.Id);
        approval.Steps.ForEach((Action<ApprovalStep>) (approvalStep =>
        {
          string id = approvalStep.AssignedApprover.Id;
          if (approvalStep.Status != ApprovalStatus.Pending && approvalStep.Status != ApprovalStatus.Deferred || ((requestContext.IsSystemContext ? 1 : (this.IsUserOrMember(requestContext, identityService, identities[id], currentApprover) ? 1 : 0)) | (hasUpdatePermission ? 1 : 0)) == 0)
            return;
          authorizedForThisApproval = true;
          currentAuthorizedSteps.Add(approvalStep.StepId);
        }));
        if (!authorizedForThisApproval)
          throw new ApprovalUnauthorizedException(ApprovalResources.ApprovalUnauthorizedExceptionMessage((object) approverId, (object) approval.Id));
        approvalIdToStepIds[approval.Id] = currentAuthorizedSteps;
      }
    }

    private bool IsUserOrMember(
      IVssRequestContext requestContext,
      IdentityService identityService,
      IdentityRef identityToVerify,
      IdentityRef currentApprover)
    {
      return identityToVerify.Id.Equals(currentApprover.Id) || identityToVerify.IsContainer && identityService.IsMember(requestContext, identityToVerify.Descriptor, currentApprover.Descriptor);
    }

    private void ThrowIfNoStepsCanBeUpdated(
      IEnumerable<Microsoft.Azure.Pipelines.Approval.WebApi.Approval> approvals,
      IDictionary<Guid, IList<int>> approvalIdToAuthorizedStepIds)
    {
      approvals.ForEach<Microsoft.Azure.Pipelines.Approval.WebApi.Approval>((Action<Microsoft.Azure.Pipelines.Approval.WebApi.Approval>) (approval =>
      {
        IList<int> authorizedStepIds = approvalIdToAuthorizedStepIds[approval.Id];
        if (approval.Steps.Where<ApprovalStep>((Func<ApprovalStep, bool>) (step => authorizedStepIds.Contains(step.StepId))).Where<ApprovalStep>((Func<ApprovalStep, bool>) (step => step.Status == ApprovalStatus.Pending || step.Status == ApprovalStatus.Deferred)).Count<ApprovalStep>() == 0)
          throw new ApprovalStateChangeException(ApprovalResources.ApprovalUpdateRequestFailed((object) approval.Id));
      }));
    }

    private bool AreUpdateParametersInDatabase(
      IEnumerable<ApprovalUpdateParameters> approvalUpdateParameters,
      IEnumerable<Microsoft.Azure.Pipelines.Approval.WebApi.Approval> queriedApprovals)
    {
      bool flag = true;
      foreach (ApprovalUpdateParameters approvalUpdateParameter1 in approvalUpdateParameters)
      {
        ApprovalUpdateParameters approvalUpdateParameter = approvalUpdateParameter1;
        if (queriedApprovals.Where<Microsoft.Azure.Pipelines.Approval.WebApi.Approval>((Func<Microsoft.Azure.Pipelines.Approval.WebApi.Approval, bool>) (queriedApproval => queriedApproval.Id.Equals(approvalUpdateParameter.ApprovalId))).Count<Microsoft.Azure.Pipelines.Approval.WebApi.Approval>() != 1)
        {
          flag = false;
          break;
        }
      }
      return flag;
    }

    private static Uri GetUrlWithParameters(Uri uri, IDictionary<string, string> parameters)
    {
      if (parameters == null || !parameters.Any<KeyValuePair<string, string>>())
        return uri;
      UriBuilder uriBuilder = new UriBuilder(uri);
      NameValueCollection queryString = HttpUtility.ParseQueryString(uri.Query);
      foreach (KeyValuePair<string, string> parameter in (IEnumerable<KeyValuePair<string, string>>) parameters)
        queryString.Add(parameter.Key, parameter.Value);
      uriBuilder.Query = queryString.ToString();
      return uriBuilder.Uri;
    }

    private string GetApprovalRestUri(
      IVssRequestContext requestContext,
      Guid projectId,
      Guid approvalId,
      IDictionary<string, string> parameters = null)
    {
      ILocationService service = requestContext.GetService<ILocationService>();
      object obj = (object) new{ approvalId = approvalId };
      IVssRequestContext requestContext1 = requestContext;
      Guid approvals = Microsoft.Azure.Pipelines.Approval.WebApi.Constants.Approvals;
      Guid projectId1 = projectId;
      object routeValues = obj;
      Guid serviceOwner = new Guid();
      return ApprovalService.GetUrlWithParameters(service.GetResourceUri(requestContext1, "approval", approvals, projectId1, routeValues, serviceOwner), parameters).AbsoluteUri;
    }

    private Microsoft.Azure.Pipelines.Approval.WebApi.Approval PopulateIfUserCanApproveStepField(
      IVssRequestContext requestContext,
      IdentityService identityService,
      IDictionary<string, IdentityRef> identities,
      Microsoft.Azure.Pipelines.Approval.WebApi.Approval approval,
      IdentityRef currentApproverFilterIdentity,
      bool hasAdminPermission,
      bool hasUpdatePermission)
    {
      bool flag = false;
      for (int index = 0; index < approval.BlockedApprovers.Count; ++index)
      {
        if (this.IsUserOrMember(requestContext, identityService, identities[approval.BlockedApprovers.ElementAt<IdentityRef>(index).Id], currentApproverFilterIdentity))
        {
          flag = true;
          break;
        }
      }
      approval.Permissions = new ApprovalPermissions?(ApprovalPermissions.View);
      for (int index = 0; index < approval.Steps.Count; ++index)
      {
        ApprovalStep approvalStep = approval.Steps.ElementAt<ApprovalStep>(index);
        if (approvalStep.ActualApprover == null)
        {
          approvalStep.Permissions = new ApprovalPermissions?(ApprovalPermissions.View);
          if (approval.Status == ApprovalStatus.Pending)
          {
            if (this.IsUserOrMember(requestContext, identityService, identities[approvalStep.AssignedApprover.Id], currentApproverFilterIdentity))
            {
              if (approvalStep.Status == ApprovalStatus.Pending)
              {
                approvalStep.Permissions = new ApprovalPermissions?(flag ? ApprovalPermissions.Reassign : ApprovalPermissions.Update);
                approval.Permissions = new ApprovalPermissions?(flag ? ApprovalPermissions.View : ApprovalPermissions.Update);
              }
              else if (approvalStep.Status == ApprovalStatus.Uninitiated)
                approvalStep.Permissions = new ApprovalPermissions?(ApprovalPermissions.Reassign);
            }
            else if (hasAdminPermission && (approvalStep.Status == ApprovalStatus.Pending || approvalStep.Status == ApprovalStatus.Uninitiated))
              approvalStep.Permissions = new ApprovalPermissions?(ApprovalPermissions.Reassign);
            else if (hasUpdatePermission)
            {
              approvalStep.Permissions = new ApprovalPermissions?(ApprovalPermissions.Update);
              approval.Permissions = new ApprovalPermissions?(ApprovalPermissions.Update);
            }
          }
        }
        else if (approval.Status == ApprovalStatus.Deferred && !flag && this.IsUserOrMember(requestContext, identityService, identities[approvalStep.AssignedApprover.Id], currentApproverFilterIdentity))
          approvalStep.Permissions = new ApprovalPermissions?(ApprovalPermissions.Update);
      }
      return approval;
    }

    private IEnumerable<Microsoft.Azure.Pipelines.Approval.WebApi.Approval> GetPopulatedApprovals(
      IVssRequestContext requestContext,
      Guid projectId,
      IEnumerable<Microsoft.Azure.Pipelines.Approval.WebApi.Approval> approvals,
      ApprovalDetailsExpandParameter expand)
    {
      bool includeSteps = (expand & ApprovalDetailsExpandParameter.Steps) != 0;
      bool includePermissions = (expand & ApprovalDetailsExpandParameter.Permissions) != 0;
      string key = requestContext.GetUserIdentity().Id.ToString("D");
      IList<string> additionalIds = (IList<string>) new List<string>()
      {
        key
      };
      IDictionary<string, IdentityRef> identities = this.FetchRequiredIdentities(requestContext, approvals, includeSteps | includePermissions, additionalIds);
      IdentityRef currentApproverFilterIdentity = (IdentityRef) null;
      IdentityService identityService = requestContext.GetService<IdentityService>();
      if (includePermissions)
        identities.TryGetValue(key, out currentApproverFilterIdentity);
      ISet<Guid> approvalIdsWithAdminPermissions = this.FilterApprovalsWithRequiredPermissions(requestContext, projectId, approvals, ApprovalPermissions.ResourceAdmin);
      ISet<Guid> approvalIdsWithUpdatePermissions = this.FilterApprovalsWithRequiredPermissions(requestContext, projectId, approvals, ApprovalPermissions.QueueBuild);
      approvals.ForEach<Microsoft.Azure.Pipelines.Approval.WebApi.Approval>((Action<Microsoft.Azure.Pipelines.Approval.WebApi.Approval>) (approval => this.FillComputedValues(requestContext, projectId, approval, includeSteps, identities, includePermissions, currentApproverFilterIdentity, identityService, approvalIdsWithAdminPermissions.Contains(approval.Id), approvalIdsWithUpdatePermissions.Contains(approval.Id), expand)));
      return approvals;
    }

    private void PopulateMissingIdentitiesData(
      IVssRequestContext requestContext,
      IEnumerable<Microsoft.Azure.Pipelines.Approval.WebApi.Approval> approvals,
      IDictionary<string, IdentityRef> identities)
    {
      List<string> missingApproverIdentities = new List<string>();
      approvals.ForEach<Microsoft.Azure.Pipelines.Approval.WebApi.Approval>((Action<Microsoft.Azure.Pipelines.Approval.WebApi.Approval>) (approval => approval.Steps.ForEach((Action<ApprovalStep>) (approvalStep =>
      {
        if (approvalStep.ActualApprover != null && !string.IsNullOrWhiteSpace(approvalStep.ActualApprover.Id) && !identities.ContainsKey(approvalStep.ActualApprover.Id))
          missingApproverIdentities.Add(approvalStep.ActualApprover.Id);
        if (approvalStep.LastModifiedBy == null || string.IsNullOrWhiteSpace(approvalStep.LastModifiedBy.Id) || identities.ContainsKey(approvalStep.LastModifiedBy.Id))
          return;
        missingApproverIdentities.Add(approvalStep.LastModifiedBy.Id);
      }))));
      if (missingApproverIdentities.Count <= 0)
        return;
      IDictionary<string, IdentityRef> values = requestContext.GetService<IdentityService>().QueryIdentities(requestContext, (IList<string>) missingApproverIdentities);
      identities.AddRange<KeyValuePair<string, IdentityRef>, IDictionary<string, IdentityRef>>((IEnumerable<KeyValuePair<string, IdentityRef>>) values);
    }

    private JToken DeserializeWithLowerCasePropertyNames(string json)
    {
      using (TextReader textReader = (TextReader) new StringReader(json))
      {
        using (JsonReader reader = (JsonReader) new ApprovalService.LowerCasePropertyNameJsonReader(textReader))
          return new JsonSerializer().Deserialize<JToken>(reader);
      }
    }

    private class LowerCasePropertyNameJsonReader : JsonTextReader
    {
      public LowerCasePropertyNameJsonReader(TextReader textReader)
        : base(textReader)
      {
      }

      public override object Value => this.TokenType == JsonToken.PropertyName ? (object) ((string) base.Value).ToLowerInvariant() : base.Value;
    }
  }
}
