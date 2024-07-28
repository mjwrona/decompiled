// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Server.Processors.ApprovalsProcessor
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 134B1041-BFA6-49C6-8C6D-CA5ADF31AF54
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Server.dll

using Microsoft.TeamFoundation.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.ReleaseManagement.Common.Extensions;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Components;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Converters;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Exceptions;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Enumerations;
using Microsoft.VisualStudio.Services.ReleaseManagement.Server.Analytics;
using Microsoft.VisualStudio.Services.ReleaseManagement.Server.Properties;
using Microsoft.VisualStudio.Services.ReleaseManagement.Server.Services;
using Microsoft.VisualStudio.Services.ReleaseManagement.Server.Utilities;
using Microsoft.VisualStudio.Services.ReleaseManagement.Server.Validations;
using Microsoft.VisualStudio.Services.ReleaseManagement.WebApi;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Server.Processors
{
  public class ApprovalsProcessor
  {
    private readonly Guid projectId;
    private readonly IVssRequestContext requestContext;
    private readonly ApprovalValidatorCalls validatorCalls;
    private readonly OrchestratorCalls orchestratorCalls;
    private readonly Func<int, bool, IEnumerable<ReleaseEnvironmentStep>> loadApprovalStep;

    public ApprovalsProcessor(IVssRequestContext context, Guid projectId)
      : this(context, ApprovalsProcessor.GetValidatorCalls(context, projectId), ApprovalsProcessor.GetOrhcestratorCalls(context, projectId), ApprovalsProcessor.LoadApproval(context, projectId))
    {
      this.projectId = projectId;
    }

    protected ApprovalsProcessor(
      IVssRequestContext context,
      ApprovalValidatorCalls validatorCalls,
      OrchestratorCalls orchestratorCalls,
      Func<int, bool, IEnumerable<ReleaseEnvironmentStep>> loadApprovalStep)
    {
      this.requestContext = context;
      this.validatorCalls = validatorCalls;
      this.orchestratorCalls = orchestratorCalls;
      this.loadApprovalStep = loadApprovalStep;
    }

    public IEnumerable<ReleaseEnvironmentStep> GetApproval(int approvalId, bool includeHistory) => this.loadApprovalStep(approvalId, includeHistory);

    private static Guid GetReassignedGuid(
      IVssRequestContext requestContext,
      ReleaseApproval approval)
    {
      if (approval.Approver == null)
        throw new InvalidRequestException(Resources.ReassignedApproverCannotBeEmpty);
      string str = !string.IsNullOrWhiteSpace(approval.Approver.UniqueName) ? approval.Approver.UniqueName : approval.Approver.Id;
      if (string.IsNullOrWhiteSpace(str))
        throw new InvalidRequestException(Resources.ReassignedApproverCannotBeEmpty);
      if (str == approval.Approver.Id)
        return new Guid(str);
      IList<Microsoft.VisualStudio.Services.Identity.Identity> identitiesIdFromName = Microsoft.VisualStudio.Services.ReleaseManagement.Common.Extensions.IdentityExtensions.GetIdentitiesIdFromName(requestContext, str);
      return identitiesIdFromName != null && identitiesIdFromName.Count != 0 ? identitiesIdFromName.First<Microsoft.VisualStudio.Services.Identity.Identity>().Id : throw new InvalidRequestException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.ReassignedApproverCannotBeEmpty));
    }

    [SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity", Justification = "Required to validate user input")]
    public IEnumerable<ReleaseEnvironmentStep> UpdateApprovalsStatus(
      IEnumerable<ReleaseApproval> approvals,
      IList<DeploymentAuthorizationInfo> deploymentAuthorizationInfo,
      Func<IVssRequestContext, bool> isAzureActiveDirectoryAccount)
    {
      ReleaseApproval releaseApproval = approvals.First<ReleaseApproval>();
      ApprovalStatus approvalStatus = releaseApproval.Status;
      if (approvalStatus != ApprovalStatus.Approved && approvalStatus != ApprovalStatus.Rejected && approvalStatus != ApprovalStatus.Reassigned)
        throw new InvalidRequestException(Resources.ProvideValidApprovalStatus);
      if (approvals.Any<ReleaseApproval>((Func<ReleaseApproval, bool>) (a => a.Status != approvalStatus)))
        throw new InvalidRequestException(Resources.AllApprovalsAreNotOfSameStatus);
      ReleaseEnvironmentStep approvalStep = this.loadApprovalStep(releaseApproval.Id, false).First<ReleaseEnvironmentStep>();
      if (!approvalStep.IsApprovalStep())
        throw new InvalidRequestException(Resources.StepTypeMismatch);
      Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Release release = this.requestContext.GetService<ReleasesService>().GetRelease(this.requestContext, approvalStep.ProjectId, approvalStep.ReleaseId);
      IEnumerable<ReleaseEnvironmentStep> allSteps = release.Environments.SelectMany<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ReleaseEnvironment, ReleaseEnvironmentStep>((Func<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ReleaseEnvironment, IEnumerable<ReleaseEnvironmentStep>>) (e => (IEnumerable<ReleaseEnvironmentStep>) e.GetStepsForTests));
      IEnumerable<int> ints = approvals.Select<ReleaseApproval, int>((Func<ReleaseApproval, int>) (a => a.Id)).Except<int>(allSteps.Select<ReleaseEnvironmentStep, int>((Func<ReleaseEnvironmentStep, int>) (a => a.Id)));
      if (ints.Any<int>())
        throw new InvalidRequestException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.ApprovalsNotBelongsToSameRelease, (object) string.Join<int>(",", (IEnumerable<int>) ints.ToArray<int>()), (object) release.Id));
      IEnumerable<ReleaseEnvironmentStep> releaseEnvironmentSteps = approvals.Select<ReleaseApproval, ReleaseEnvironmentStep>((Func<ReleaseApproval, ReleaseEnvironmentStep>) (a => allSteps.First<ReleaseEnvironmentStep>((Func<ReleaseEnvironmentStep, bool>) (s => s.Id == a.Id))));
      if (releaseEnvironmentSteps.Where<ReleaseEnvironmentStep>((Func<ReleaseEnvironmentStep, bool>) (a => a.StepType != approvalStep.StepType)).Any<ReleaseEnvironmentStep>())
        throw new InvalidRequestException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.MismatchApprovalSteps, (object) string.Join<int>(",", ints)));
      IEnumerable<ReleaseEnvironmentStep> source1 = releaseEnvironmentSteps.Where<ReleaseEnvironmentStep>((Func<ReleaseEnvironmentStep, bool>) (a => a.IsAutomated));
      if (source1.Any<ReleaseEnvironmentStep>())
        throw new InvalidRequestException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.AutomatedStepsUpdateNotAllowed, (object) string.Join<int>(",", source1.Select<ReleaseEnvironmentStep, int>((Func<ReleaseEnvironmentStep, int>) (a => a.Id)))));
      IEnumerable<ReleaseEnvironmentStep> source2 = releaseEnvironmentSteps.Where<ReleaseEnvironmentStep>((Func<ReleaseEnvironmentStep, bool>) (a => a.Status != ReleaseEnvironmentStepStatus.Pending));
      if (source2.Any<ReleaseEnvironmentStep>())
        throw new InvalidRequestException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.InvalidApprovalUpdate, (object) string.Join<int>(",", source2.Select<ReleaseEnvironmentStep, int>((Func<ReleaseEnvironmentStep, int>) (a => a.Id)))));
      this.PerformValidations(releaseEnvironmentSteps, deploymentAuthorizationInfo, isAzureActiveDirectoryAccount, release);
      ApprovalsProcessor.FillApprovalStepsWithUserData(releaseEnvironmentSteps, approvals, this.requestContext);
      RmTelemetryFactory.GetLogger(this.requestContext).PublishApprovalUpdated(this.requestContext, release, approvalStep.ProjectId, releaseEnvironmentSteps);
      Enumerable.Empty<ReleaseEnvironmentStep>();
      IEnumerable<ReleaseEnvironmentStep> steps;
      switch (approvalStatus.FromWebApi())
      {
        case ReleaseEnvironmentStepStatus.Rejected:
          steps = this.orchestratorCalls.RejectStep(releaseEnvironmentSteps, (IList<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Release>) new List<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Release>()
          {
            release
          });
          break;
        case ReleaseEnvironmentStepStatus.Reassigned:
          steps = this.orchestratorCalls.ReassignStep(releaseEnvironmentSteps);
          break;
        case ReleaseEnvironmentStepStatus.Done:
          steps = this.orchestratorCalls.AcceptStep(releaseEnvironmentSteps, (IEnumerable<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Release>) new List<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Release>()
          {
            release
          });
          break;
        default:
          throw new InvalidRequestException(Resources.ProvideValidApprovalStatus);
      }
      this.SendReleaseEnvironmentUpdateEvents(steps);
      return steps;
    }

    private static void FillApprovalStepsWithUserData(
      IEnumerable<ReleaseEnvironmentStep> approvalSteps,
      IEnumerable<ReleaseApproval> releaseApprovals,
      IVssRequestContext requestContext)
    {
      Microsoft.VisualStudio.Services.Identity.Identity userIdentity = requestContext.GetUserIdentity();
      foreach (ReleaseEnvironmentStep approvalStep in approvalSteps)
      {
        ReleaseEnvironmentStep step = approvalStep;
        ReleaseApproval approval = releaseApprovals.First<ReleaseApproval>((Func<ReleaseApproval, bool>) (a => a.Id == step.Id));
        if (approval.Status == ApprovalStatus.Reassigned)
          step.ApproverId = ApprovalsProcessor.GetReassignedGuid(requestContext, approval);
        step.ActualApproverId = userIdentity.Id;
        step.ApproverComment = approval.Comments;
        step.Status = approval.Status.FromWebApi();
      }
    }

    private static ApprovalValidatorCalls GetValidatorCalls(
      IVssRequestContext context,
      Guid projectId)
    {
      ApprovalsValidator approvalsValidator = new ApprovalsValidator(context, projectId);
      return new ApprovalValidatorCalls()
      {
        IsRequestorApprover = new ApprovalValidatorCalls.Validator(approvalsValidator.IsRequestorApprover),
        IsRequestorAuthorizedGroupMember = new Func<ReleaseEnvironmentStep, bool>(approvalsValidator.IsRequestorAuthorizedGroupMember),
        IsRequestorReleaseManager = new Func<int, bool>(approvalsValidator.IsRequestorReleaseManager)
      };
    }

    private static OrchestratorCalls GetOrhcestratorCalls(
      IVssRequestContext context,
      Guid projectId)
    {
      OrchestratorServiceProcessorV2 serviceProcessorV2 = new OrchestratorServiceProcessorV2(context, projectId);
      return new OrchestratorCalls()
      {
        AcceptStep = new Func<IEnumerable<ReleaseEnvironmentStep>, IEnumerable<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Release>, IEnumerable<ReleaseEnvironmentStep>>(serviceProcessorV2.AcceptStep),
        RejectStep = new Func<IEnumerable<ReleaseEnvironmentStep>, IList<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Release>, IEnumerable<ReleaseEnvironmentStep>>(serviceProcessorV2.RejectStep),
        ReassignStep = new Func<IEnumerable<ReleaseEnvironmentStep>, IEnumerable<ReleaseEnvironmentStep>>(serviceProcessorV2.ReassignApprovalRequest)
      };
    }

    private static Func<int, bool, IEnumerable<ReleaseEnvironmentStep>> LoadApproval(
      IVssRequestContext context,
      Guid projectId)
    {
      return (Func<int, bool, IEnumerable<ReleaseEnvironmentStep>>) ((id, includeHistory) => context.ExecuteWithinUsingWithComponent<ReleaseEnvironmentStepSqlComponent, IEnumerable<ReleaseEnvironmentStep>>((Func<ReleaseEnvironmentStepSqlComponent, IEnumerable<ReleaseEnvironmentStep>>) (component => component.GetReleaseEnvironmentStep(projectId, id, includeHistory))));
    }

    private static IEnumerable<int> IdentityRevalidationEnabledEnvironments(
      IEnumerable<ReleaseEnvironmentStep> approvals,
      Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Release release)
    {
      List<int> intList = new List<int>();
      foreach (int releaseEnvironmentId in approvals.Select<ReleaseEnvironmentStep, int>((Func<ReleaseEnvironmentStep, int>) (a => a.ReleaseEnvironmentId)).Distinct<int>())
      {
        Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ReleaseEnvironment environment = release.GetEnvironment(releaseEnvironmentId);
        if (!environment.CompareEnforceIdentityRevalidation(EnvironmentStepType.PreDeploy, false) || !environment.CompareEnforceIdentityRevalidation(EnvironmentStepType.PostDeploy, false))
          intList.Add(environment.Id);
      }
      return (IEnumerable<int>) intList;
    }

    private void SendReleaseEnvironmentUpdateEvents(IEnumerable<ReleaseEnvironmentStep> steps)
    {
      if (steps == null)
        return;
      Dictionary<int, Tuple<int, int>> dictionary1 = new Dictionary<int, Tuple<int, int>>();
      Dictionary<int, int> dictionary2 = new Dictionary<int, int>();
      foreach (ReleaseEnvironmentStep step in steps)
      {
        int releaseId = step.ReleaseId;
        int releaseDefinitionId = step.ReleaseDefinitionId;
        if (!dictionary2.ContainsKey(releaseId))
        {
          if (dictionary1.ContainsKey(releaseId))
          {
            dictionary2.Add(releaseId, releaseDefinitionId);
            dictionary1.Remove(releaseId);
          }
          else
            dictionary1.Add(releaseId, new Tuple<int, int>(step.ReleaseEnvironmentId, releaseDefinitionId));
        }
      }
      foreach (int key in dictionary2.Keys)
        this.requestContext.SendReleaseUpdatedEvent(this.projectId, dictionary2[key], key);
      foreach (int key in dictionary1.Keys)
      {
        int definitionId = dictionary1[key].Item2;
        int releaseEnvironmentId = dictionary1[key].Item1;
        this.requestContext.SendReleaseEnvironmentUpdatedEvent(this.projectId, definitionId, key, releaseEnvironmentId);
      }
    }

    private void PerformValidations(
      IEnumerable<ReleaseEnvironmentStep> approvals,
      IList<DeploymentAuthorizationInfo> deploymentAuthorizationInfo,
      Func<IVssRequestContext, bool> isAadAccount,
      Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Release release)
    {
      List<int> intList = new List<int>();
      foreach (ReleaseEnvironmentStep approval in approvals)
      {
        if (!this.validatorCalls.IsRequestorApprover(approval.Id, approval) && !this.validatorCalls.IsRequestorAuthorizedGroupMember(approval) && !intList.Contains(release.ReleaseDefinitionId))
        {
          if (!this.validatorCalls.IsRequestorReleaseManager(release.ReleaseDefinitionId))
            throw new ReleaseManagementUnauthorizedException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.UnauthorizedApprover, (object) approval.Id));
          intList.Add(release.ReleaseDefinitionId);
        }
      }
      this.ValidateDeploymentAuthorization(approvals, deploymentAuthorizationInfo, isAadAccount, release);
    }

    private void ValidateDeploymentAuthorization(
      IEnumerable<ReleaseEnvironmentStep> releaseEnvironmentSteps,
      IList<DeploymentAuthorizationInfo> deploymentAuthorizationInfo,
      Func<IVssRequestContext, bool> isAadAccount,
      Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Release release)
    {
      if (this.requestContext.ExecutionEnvironment.IsOnPremisesDeployment || !isAadAccount(this.requestContext))
        return;
      IEnumerable<int> source = ApprovalsProcessor.IdentityRevalidationEnabledEnvironments(releaseEnvironmentSteps, release);
      if (!source.Any<int>())
        return;
      Guid organizationAadTenantId = this.requestContext.GetOrganizationAadTenantId();
      List<DeploymentAuthorizationInfo> approverByTenantId = ServiceEndpointHelper.GetDeploymentAuthorizationInfoForRevalidateApproverByTenantId((IEnumerable<DeploymentAuthorizationInfo>) deploymentAuthorizationInfo, organizationAadTenantId.ToString());
      DeploymentAuthorizationInfo approverInfo = !approverByTenantId.IsNullOrEmpty<DeploymentAuthorizationInfo>() ? approverByTenantId.FirstOrDefault<DeploymentAuthorizationInfo>() : throw new InvalidRequestException(Resources.UpdateApprovalRevalidateIdentityEnabledButHeaderNotPresent);
      JwtSecurityToken jsonWebToken = new JwtSecurityToken();
      DeploymentAuthorizationValidations.PerformValidation(this.requestContext, approverInfo.OAuthParameters.IdentificationToken, approverInfo.OAuthParameters.Nonce, jsonWebToken);
      RmTelemetryFactory.GetLogger(this.requestContext).PublishRevalidateApprovalIdentity(this.requestContext, release, this.projectId, source.FirstOrDefault<int>(), approverInfo);
    }
  }
}
