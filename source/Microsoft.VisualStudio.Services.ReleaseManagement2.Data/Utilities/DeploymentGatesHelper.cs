// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Data.Utilities.DeploymentGatesHelper
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Data, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A69B0FB7-3028-4162-BA38-794D80D7A49A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Data.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Converters;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Exceptions;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Properties;
using Microsoft.VisualStudio.Services.ReleaseManagement.WebApi;
using Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Data.Utilities
{
  public static class DeploymentGatesHelper
  {
    public static int GetDeploymentGatesStepRank(
      this ReleaseDefinitionGatesStep gates,
      IEnumerable<DefinitionEnvironmentStep> approvals,
      Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ApprovalOptions approvalOptions,
      int offset)
    {
      ApprovalExecutionOrder executionOrder = approvalOptions != null ? approvalOptions.ExecutionOrder : ApprovalExecutionOrder.BeforeGates;
      int maxApprovalRank = approvals.Any<DefinitionEnvironmentStep>() ? approvals.Max<DefinitionEnvironmentStep>((Func<DefinitionEnvironmentStep, int>) (a => a.Rank)) : 0;
      return DeploymentGatesHelper.GetDeploymentGatesStepRank(gates, executionOrder, offset, maxApprovalRank);
    }

    public static int GetDeploymentGatesStepRank(
      this ReleaseDefinitionGatesStep gates,
      ReleaseDefinitionApprovals approvals,
      int offset)
    {
      Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ApprovalOptions approvalOptions = approvals.ApprovalOptions.FromWebApiApprovalOptions();
      ApprovalExecutionOrder executionOrder = approvalOptions != null ? approvalOptions.ExecutionOrder : ApprovalExecutionOrder.BeforeGates;
      int maxApprovalRank = approvals.Approvals.Any<ReleaseDefinitionApprovalStep>() ? approvals.Approvals.Max<ReleaseDefinitionApprovalStep>((Func<ReleaseDefinitionApprovalStep, int>) (a => a.Rank)) : 0;
      return DeploymentGatesHelper.GetDeploymentGatesStepRank(gates, executionOrder, offset, maxApprovalRank);
    }

    [SuppressMessage("Microsoft.Usage", "CA2233:OperationsShouldNotOverflow", MessageId = "offset+1", Justification = "will fix in next iteration.")]
    public static int GetDeploymentGatesStepRank(
      ReleaseDefinitionGatesStep gates,
      ApprovalExecutionOrder executionOrder,
      int offset,
      int maxApprovalRank)
    {
      if (!gates.AreGatesEnabled())
        return offset;
      return executionOrder == ApprovalExecutionOrder.BeforeGates ? offset + maxApprovalRank + 1 : offset + 1;
    }

    public static bool AreGatesEnabled(this ReleaseDefinitionGatesStep gates) => gates != null && gates.GatesOptions != null && gates.GatesOptions.IsEnabled;

    public static int GetReleaseDefinitionGateStepId(this ReleaseDefinitionGatesStep gates) => gates != null ? gates.Id : -1;

    public static bool AnyChangeInGatesOptionsAndApprovalsExecutionOrder(
      this ReleaseDefinitionGatesStep existingGates,
      ReleaseDefinitionGatesStep newGates,
      Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts.ApprovalOptions webApiApprovalOptions,
      Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ApprovalOptions serverApprovalOptions)
    {
      if (existingGates == null)
        throw new ArgumentNullException(nameof (existingGates));
      if (newGates == null)
        throw new ArgumentNullException(nameof (newGates));
      return (existingGates.GatesOptions != null || newGates.GatesOptions != null) && (existingGates.GatesOptions == null || newGates.GatesOptions == null || existingGates.GatesOptions.IsEnabled != newGates.GatesOptions.IsEnabled || (webApiApprovalOptions != null || serverApprovalOptions != null) && (webApiApprovalOptions == null || serverApprovalOptions == null || webApiApprovalOptions.ExecutionOrder != serverApprovalOptions.ExecutionOrder));
    }

    public static bool AreReleaseDefinitionGatesEqual(
      this ReleaseDefinitionGatesStep existingGates,
      ReleaseDefinitionGatesStep newGates)
    {
      if (existingGates == null && newGates == null)
        return true;
      return existingGates != null && newGates != null && existingGates.Equals((object) newGates);
    }

    [SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity", Justification = "Over all we are okay with this complexity.")]
    public static void ValidateGateStep(
      this ReleaseDefinitionGatesStep gateStep,
      IVssRequestContext requestContext,
      string gateTypeText,
      string environmentName)
    {
      if (gateStep == null || gateStep.GatesOptions == null || !gateStep.GatesOptions.IsEnabled)
        return;
      if (string.IsNullOrWhiteSpace(gateTypeText))
        throw new ArgumentNullException(nameof (gateTypeText));
      if (string.IsNullOrWhiteSpace(environmentName))
        throw new ArgumentNullException(nameof (environmentName));
      ReleaseDefinitionGatesOptions gatesOptions = gateStep.GatesOptions;
      if (gatesOptions.StabilizationTime < 0 || gatesOptions.StabilizationTime > 2880)
        throw new InvalidRequestException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.InvalidGateStabilizationTime, (object) gateTypeText, (object) environmentName, (object) 0, (object) 2880));
      if (gateStep.Gates == null || !gateStep.Gates.Any<ReleaseDefinitionGate>())
        return;
      IEnumerable<WorkflowTask> workflowTasks = gateStep.Gates.Where<ReleaseDefinitionGate>((Func<ReleaseDefinitionGate, bool>) (g => g != null && g.Tasks != null)).SelectMany<ReleaseDefinitionGate, WorkflowTask>((Func<ReleaseDefinitionGate, IEnumerable<WorkflowTask>>) (g => (IEnumerable<WorkflowTask>) g.Tasks)).Where<WorkflowTask>((Func<WorkflowTask, bool>) (t => t != null && t.Enabled));
      if (!workflowTasks.Any<WorkflowTask>())
        return;
      if (gatesOptions.Timeout < 6 || gatesOptions.Timeout > 21600)
        throw new InvalidRequestException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.InvalidGateTimeout, (object) gateTypeText, (object) environmentName, (object) 6, (object) 21600));
      int intervalInMinutes = DeploymentGatesHelper.GetMinimalSamplingIntervalInMinutes(requestContext, new int?(gatesOptions.Timeout));
      if (gatesOptions.SamplingInterval < intervalInMinutes || gatesOptions.SamplingInterval > 1440 || gatesOptions.SamplingInterval >= gatesOptions.Timeout)
        throw new InvalidRequestException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.InvalidGateSamplingIntervalTime, (object) gateTypeText, (object) environmentName, (object) intervalInMinutes, (object) 1440));
      if (gatesOptions.MinimumSuccessDuration < 0 || gatesOptions.MinimumSuccessDuration > 2880 || gatesOptions.MinimumSuccessDuration >= gatesOptions.Timeout)
        throw new InvalidRequestException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.InvalidGatesMinimumSuccessWindow, (object) gateTypeText, (object) environmentName, (object) 0, (object) 2880));
      DeploymentGatesHelper.ValidateGateTasks(workflowTasks, gateTypeText.ToLower(CultureInfo.CurrentCulture), environmentName);
    }

    public static ReleaseGates ToReleaseGates(this DeploymentGate deploymentGate)
    {
      if (deploymentGate == null)
        return (ReleaseGates) null;
      ReleaseGates releaseGates = new ReleaseGates();
      releaseGates.Id = deploymentGate.ReleaseEnvironmentStepId;
      releaseGates.Status = (GateStatus) deploymentGate.Status;
      releaseGates.RunPlanId = deploymentGate.RunPlanId;
      releaseGates.StartedOn = deploymentGate.StartedOn;
      releaseGates.LastModifiedOn = deploymentGate.LastModifiedOn;
      releaseGates.StabilizationCompletedOn = deploymentGate.StabilizationCompletedOn;
      IList<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.IgnoredGate> ignoredGates = deploymentGate.IgnoredGates;
      releaseGates.IgnoredGates = ignoredGates != null ? ignoredGates.ToWebApi() : (IList<Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.IgnoredGate>) null;
      releaseGates.SucceedingSince = deploymentGate.SucceedingSince;
      return releaseGates;
    }

    public static void ValidateGateTasks(
      IEnumerable<WorkflowTask> gateWorkflow,
      string gateTypeText,
      string environmentName)
    {
      if (string.IsNullOrWhiteSpace(gateTypeText))
        throw new ArgumentNullException(nameof (gateTypeText));
      if (string.IsNullOrWhiteSpace(environmentName))
        throw new ArgumentNullException(nameof (environmentName));
      if (gateWorkflow == null || gateWorkflow.Any<WorkflowTask>((Func<WorkflowTask, bool>) (t => t.TaskId == Guid.Empty || string.IsNullOrEmpty(t.Version))))
        throw new InvalidRequestException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.GateWorkflowCannotBeEmpty, (object) gateTypeText, (object) environmentName));
      if (gateWorkflow.Count<WorkflowTask>() > 10)
        throw new InvalidRequestException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.NumberOfGatesMoreThanLimit, (object) gateTypeText, (object) environmentName, (object) 10));
      foreach (WorkflowTask workflowTask in gateWorkflow)
      {
        if (workflowTask.TimeoutInMinutes < 0)
          throw new InvalidRequestException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.InvalidGateTaskTimeout, (object) workflowTask.Name, (object) gateTypeText, (object) environmentName));
      }
      if (gateWorkflow.Select<WorkflowTask, string>((Func<WorkflowTask, string>) (t => t.Name.Trim())).GroupBy<string, string>((Func<string, string>) (x => x), (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase).Any<IGrouping<string, string>>((Func<IGrouping<string, string>, bool>) (x => x.Count<string>() > 1)))
        throw new InvalidRequestException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.DuplicateGateNames, (object) gateTypeText, (object) environmentName));
    }

    public static int GetMinimalSamplingIntervalInMinutes(
      IVssRequestContext requestContext,
      int? gateTimeout)
    {
      if (requestContext == null)
        throw new ArgumentNullException(nameof (requestContext));
      if (!requestContext.ExecutionEnvironment.IsOnPremisesDeployment)
      {
        if (gateTimeout.HasValue)
        {
          int? nullable = gateTimeout;
          int num = 4320;
          if (nullable.GetValueOrDefault() > num & nullable.HasValue)
            return 30;
        }
        return 5;
      }
      int intervalInMinutes = DeploymentGatesHelper.GetMinimalSamplingIntervalFromRegistry(requestContext);
      if (gateTimeout.HasValue)
      {
        int? nullable = gateTimeout;
        int num = 4320;
        if (nullable.GetValueOrDefault() > num & nullable.HasValue && intervalInMinutes < 30)
          intervalInMinutes = 30;
      }
      return intervalInMinutes;
    }

    [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "This is for logging purpose.")]
    private static int GetMinimalSamplingIntervalFromRegistry(IVssRequestContext requestContext)
    {
      try
      {
        int num = requestContext.GetService<IVssRegistryService>().GetValue<int>(requestContext, (RegistryQuery) "/Service/ReleaseManagement/Settings/DeploymentGateReevaluationTimeout", true, 5);
        return num > 0 ? num : 5;
      }
      catch (Exception ex)
      {
        requestContext.TraceException(1971090, "PipelineWorkflow", "Service", ex);
        return 5;
      }
    }
  }
}
