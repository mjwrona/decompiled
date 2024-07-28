// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Server.Validations.ReleaseDefinitionApproverValidations
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 134B1041-BFA6-49C6-8C6D-CA5ADF31AF54
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.ReleaseManagement.Common.Diagnostics;
using Microsoft.VisualStudio.Services.ReleaseManagement.Common.Security;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Exceptions;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Enumerations;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Utilities;
using Microsoft.VisualStudio.Services.ReleaseManagement.Server.Properties;
using Microsoft.VisualStudio.Services.ReleaseManagement.Server.Security;
using Microsoft.VisualStudio.Services.ReleaseManagement.Server.Services;
using Microsoft.VisualStudio.Services.ReleaseManagement.WebApi;
using Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts;
using Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Security;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Server.Validations
{
  public class ReleaseDefinitionApproverValidations
  {
    private readonly Func<IVssRequestContext, Guid, int, Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ReleaseDefinition> getReleaseDefinition;

    internal ReleaseDefinitionApproverValidations(
      Func<IVssRequestContext, Guid, int, Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ReleaseDefinition> getReleaseDefinition)
    {
      this.getReleaseDefinition = getReleaseDefinition;
    }

    public ReleaseDefinitionApproverValidations()
      : this(ReleaseDefinitionApproverValidations.\u003C\u003EO.\u003C0\u003E__GetReleaseDefinition ?? (ReleaseDefinitionApproverValidations.\u003C\u003EO.\u003C0\u003E__GetReleaseDefinition = new Func<IVssRequestContext, Guid, int, Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ReleaseDefinition>(ReleaseDefinitionApproverValidations.GetReleaseDefinition)))
    {
      // ISSUE: reference to a compiler-generated field (out of statement scope)
      // ISSUE: reference to a compiler-generated field (out of statement scope)
    }

    private static Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ReleaseDefinition GetReleaseDefinition(
      IVssRequestContext context,
      Guid projectId,
      int id)
    {
      return Microsoft.VisualStudio.Services.ReleaseManagement.Server.Extensions.ReleaseDefinitionExtensions.GetReleaseDefinition(context.GetService<ReleaseDefinitionsService>(), context, projectId, id, (IEnumerable<string>) null);
    }

    public void CheckManagePermission(
      IVssRequestContext context,
      Guid projectId,
      Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ReleaseDefinition releaseDefinition)
    {
      if (context == null)
        throw new ArgumentNullException(nameof (context));
      if (releaseDefinition == null)
        throw new ArgumentNullException(nameof (releaseDefinition));
      if (!ReleaseDefinitionApproverValidations.DoesUserHasManageApproverPermission(context, projectId, releaseDefinition) && this.HasApproverChangedInReleaseDefinition(context, projectId, releaseDefinition, 1961022))
      {
        ResourceAccessException innerException = new ResourceAccessException(context.RootContext.GetUserId().ToString(), ReleaseManagementSecurityPermissions.ManageReleaseApprovers);
        throw new UnauthorizedRequestException(innerException.Message, (Exception) innerException);
      }
    }

    private static bool DoesUserHasManageApproverPermission(
      IVssRequestContext context,
      Guid projectId,
      Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ReleaseDefinition releaseDefinition)
    {
      return context.HasPermission(projectId, releaseDefinition.Path, releaseDefinition.Id, ReleaseManagementSecurityPermissions.ManageReleaseApprovers);
    }

    public static bool CompareDefinitionEnvironmentApprovals(
      ReleaseDefinitionEnvironment updatedEnvironment,
      ReleaseDefinitionEnvironment existingEnvironment)
    {
      if (updatedEnvironment == null)
        throw new ArgumentNullException(nameof (updatedEnvironment));
      if (existingEnvironment == null)
        throw new ArgumentNullException(nameof (existingEnvironment));
      IList<ReleaseDefinitionApprovalStep> environmentApprovals1 = ReleaseDefinitionEnvironmentExtensions.GetDefinitionEnvironmentApprovals(updatedEnvironment, EnvironmentStepType.PreDeploy);
      IList<ReleaseDefinitionApprovalStep> environmentApprovals2 = ReleaseDefinitionEnvironmentExtensions.GetDefinitionEnvironmentApprovals(updatedEnvironment, EnvironmentStepType.PostDeploy);
      IList<ReleaseDefinitionApprovalStep> environmentApprovals3 = ReleaseDefinitionEnvironmentExtensions.GetDefinitionEnvironmentApprovals(existingEnvironment, EnvironmentStepType.PreDeploy);
      IList<ReleaseDefinitionApprovalStep> environmentApprovals4 = ReleaseDefinitionEnvironmentExtensions.GetDefinitionEnvironmentApprovals(existingEnvironment, EnvironmentStepType.PostDeploy);
      Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts.ApprovalOptions environmentApprovalOptions1 = ReleaseDefinitionEnvironmentExtensions.GetDefinitionEnvironmentApprovalOptions(updatedEnvironment, EnvironmentStepType.PreDeploy);
      Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts.ApprovalOptions environmentApprovalOptions2 = ReleaseDefinitionEnvironmentExtensions.GetDefinitionEnvironmentApprovalOptions(updatedEnvironment, EnvironmentStepType.PostDeploy);
      Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts.ApprovalOptions environmentApprovalOptions3 = ReleaseDefinitionEnvironmentExtensions.GetDefinitionEnvironmentApprovalOptions(existingEnvironment, EnvironmentStepType.PreDeploy);
      Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts.ApprovalOptions environmentApprovalOptions4 = ReleaseDefinitionEnvironmentExtensions.GetDefinitionEnvironmentApprovalOptions(existingEnvironment, EnvironmentStepType.PostDeploy);
      bool isNewApprovalAutomated1 = environmentApprovals1.Any<ReleaseDefinitionApprovalStep>() && environmentApprovals1.First<ReleaseDefinitionApprovalStep>().IsAutomated;
      bool isExistingApprovalAutomated1 = environmentApprovals3.Any<ReleaseDefinitionApprovalStep>() && environmentApprovals3.First<ReleaseDefinitionApprovalStep>().IsAutomated;
      bool isNewApprovalAutomated2 = environmentApprovals2.Any<ReleaseDefinitionApprovalStep>() && environmentApprovals2.First<ReleaseDefinitionApprovalStep>().IsAutomated;
      bool isExistingApprovalAutomated2 = environmentApprovals4.Any<ReleaseDefinitionApprovalStep>() && environmentApprovals4.First<ReleaseDefinitionApprovalStep>().IsAutomated;
      return ReleaseDefinitionApproverValidations.CompareReleaseApprovers((IEnumerable<ReleaseDefinitionApprovalStep>) environmentApprovals1, (IEnumerable<ReleaseDefinitionApprovalStep>) environmentApprovals3) || ReleaseDefinitionApproverValidations.CompareReleaseApprovers((IEnumerable<ReleaseDefinitionApprovalStep>) environmentApprovals2, (IEnumerable<ReleaseDefinitionApprovalStep>) environmentApprovals4) || ReleaseDefinitionApproverValidations.CompareApprovalOptions(environmentApprovalOptions1, environmentApprovalOptions3, isNewApprovalAutomated1, isExistingApprovalAutomated1) || ReleaseDefinitionApproverValidations.CompareApprovalOptions(environmentApprovalOptions2, environmentApprovalOptions4, isNewApprovalAutomated2, isExistingApprovalAutomated2);
    }

    public static bool CompareDefinitionEnvironmentGates(
      ReleaseDefinitionEnvironment updatedEnvironment,
      ReleaseDefinitionEnvironment existingEnvironment)
    {
      if (updatedEnvironment == null)
        throw new ArgumentNullException(nameof (updatedEnvironment));
      if (existingEnvironment == null)
        throw new ArgumentNullException(nameof (existingEnvironment));
      return !updatedEnvironment.PreDeploymentGates.AreReleaseDefinitionGatesEqual(existingEnvironment.PreDeploymentGates) || !updatedEnvironment.PostDeploymentGates.AreReleaseDefinitionGatesEqual(existingEnvironment.PostDeploymentGates);
    }

    public static bool CompareApprovalOptions(
      Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts.ApprovalOptions newApprovalOptions,
      Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts.ApprovalOptions existingApprovalOptions,
      bool isNewApprovalAutomated,
      bool isExistingApprovalAutomated)
    {
      if (newApprovalOptions == null && existingApprovalOptions == null)
        return false;
      if (newApprovalOptions != null && existingApprovalOptions != null)
        return !existingApprovalOptions.Equals((object) newApprovalOptions);
      return !(isNewApprovalAutomated & isExistingApprovalAutomated);
    }

    public static bool CompareReleaseApprovers(
      IEnumerable<ReleaseDefinitionApprovalStep> newApprovalSteps,
      IEnumerable<ReleaseDefinitionApprovalStep> existingApprovalSteps)
    {
      if (newApprovalSteps == null)
        throw new ArgumentNullException(nameof (newApprovalSteps));
      if (existingApprovalSteps == null)
        throw new ArgumentNullException(nameof (existingApprovalSteps));
      return newApprovalSteps.Count<ReleaseDefinitionApprovalStep>() != existingApprovalSteps.Count<ReleaseDefinitionApprovalStep>() || newApprovalSteps.Any<ReleaseDefinitionApprovalStep>((Func<ReleaseDefinitionApprovalStep, bool>) (newApprovalStep => !existingApprovalSteps.Contains<ReleaseDefinitionApprovalStep>(newApprovalStep))) || existingApprovalSteps.Any<ReleaseDefinitionApprovalStep>((Func<ReleaseDefinitionApprovalStep, bool>) (existingApprovalStep => !newApprovalSteps.Contains<ReleaseDefinitionApprovalStep>(existingApprovalStep)));
    }

    public static void ValidateApprovalOptions(
      Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts.ApprovalOptions approvalOptions,
      int numberOfApprovals,
      ApprovalType approvalType,
      string environmentName)
    {
      if (approvalOptions == null)
        return;
      string str = approvalType == ApprovalType.PreDeploy ? Resources.PreApprovals : Resources.PostApprovals;
      int? requiredApproverCount = approvalOptions.RequiredApproverCount;
      if (requiredApproverCount.HasValue)
      {
        int? nullable1 = requiredApproverCount;
        int num1 = numberOfApprovals;
        if (!(nullable1.GetValueOrDefault() > num1 & nullable1.HasValue))
        {
          int? nullable2 = requiredApproverCount;
          int num2 = 0;
          if (!(nullable2.GetValueOrDefault() < num2 & nullable2.HasValue))
            goto label_6;
        }
        throw new InvalidRequestException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.InvalidReqiredApproversCount, (object) str, (object) environmentName));
      }
label_6:
      if (approvalOptions.TimeoutInMinutes < Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ApprovalOptions.ApprovalMinTimeoutInMinutes || approvalOptions.TimeoutInMinutes > Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ApprovalOptions.ApprovalMaxTimeoutInMinutes)
        throw new InvalidRequestException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.InvalidReqiredApprovalTimeout, (object) str, (object) environmentName, (object) Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ApprovalOptions.ApprovalMinTimeoutInMinutes, (object) Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ApprovalOptions.ApprovalMaxTimeoutInMinutes));
    }

    private bool HasApproverChangedInReleaseDefinition(
      IVssRequestContext context,
      Guid projectId,
      Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ReleaseDefinition releaseDefinition,
      int detectApproverChangeTracePoint)
    {
      using (ReleaseManagementTimer.Create(context, "Service", "Service.HasApproverChangedInReleaseDefinition", detectApproverChangeTracePoint))
      {
        if (releaseDefinition.Id > 0)
        {
          Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ReleaseDefinition releaseDefinition1 = this.getReleaseDefinition(context, projectId, releaseDefinition.Id);
          foreach (ReleaseDefinitionEnvironment environment in (IEnumerable<ReleaseDefinitionEnvironment>) releaseDefinition.Environments)
          {
            ReleaseDefinitionEnvironment environment1 = environment;
            ReleaseDefinitionEnvironment existingEnvironment = releaseDefinition1.Environments.FirstOrDefault<ReleaseDefinitionEnvironment>((Func<ReleaseDefinitionEnvironment, bool>) (x => x.Id == environment1.Id));
            if (existingEnvironment != null && (ReleaseDefinitionApproverValidations.CompareDefinitionEnvironmentApprovals(environment, existingEnvironment) || ReleaseDefinitionApproverValidations.CompareDefinitionEnvironmentGates(environment, existingEnvironment)))
              return true;
          }
        }
        return false;
      }
    }
  }
}
