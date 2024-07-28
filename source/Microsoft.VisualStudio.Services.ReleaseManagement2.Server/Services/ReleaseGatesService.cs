// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Server.Services.ReleaseGatesService
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 134B1041-BFA6-49C6-8C6D-CA5ADF31AF54
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.ReleaseManagement.Common.Diagnostics;
using Microsoft.VisualStudio.Services.ReleaseManagement.Common.Security;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.DataAccess;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Exceptions;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Enumerations;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Utilities;
using Microsoft.VisualStudio.Services.ReleaseManagement.Server.DistributedTask;
using Microsoft.VisualStudio.Services.ReleaseManagement.Server.Security;
using Microsoft.VisualStudio.Services.ReleaseManagement.WebApi;
using Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts;
using Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Security;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Server.Services
{
  public class ReleaseGatesService : ReleaseManagement2ServiceBase
  {
    private readonly Func<IVssRequestContext, Guid, IDataAccessLayer> getDataAccessLayer;
    private readonly Func<IVssRequestContext, Guid, GreenlightingOrchestrator> getGatesOrchestrator;

    protected ReleaseGatesService(
      Func<IVssRequestContext, Guid, IDataAccessLayer> getDataAccessLayer,
      Func<IVssRequestContext, Guid, GreenlightingOrchestrator> getGatesOrchestrator)
    {
      this.getDataAccessLayer = getDataAccessLayer;
      this.getGatesOrchestrator = getGatesOrchestrator;
    }

    public ReleaseGatesService()
      : this(ReleaseGatesService.\u003C\u003EO.\u003C0\u003E__GetDataAccessLayer ?? (ReleaseGatesService.\u003C\u003EO.\u003C0\u003E__GetDataAccessLayer = new Func<IVssRequestContext, Guid, IDataAccessLayer>(ReleaseGatesService.GetDataAccessLayer)), ReleaseGatesService.\u003C\u003EO.\u003C1\u003E__GetGatesOrchestrator ?? (ReleaseGatesService.\u003C\u003EO.\u003C1\u003E__GetGatesOrchestrator = new Func<IVssRequestContext, Guid, GreenlightingOrchestrator>(ReleaseGatesService.GetGatesOrchestrator)))
    {
      // ISSUE: reference to a compiler-generated field (out of statement scope)
      // ISSUE: reference to a compiler-generated field (out of statement scope)
      // ISSUE: reference to a compiler-generated field (out of statement scope)
      // ISSUE: reference to a compiler-generated field (out of statement scope)
    }

    [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "Vssf needs it to be non-static")]
    public ReleaseGates UpdateGate(
      IVssRequestContext requestContext,
      Guid projectId,
      int gateStepId,
      GateUpdateMetadata gateUpdateMetadata)
    {
      if (requestContext == null)
        throw new ArgumentNullException(nameof (requestContext));
      ReleaseGatesService.ValidateInput(gateUpdateMetadata);
      using (ReleaseManagementTimer releaseManagementTimer = ReleaseManagementTimer.Create(requestContext, "Service", "Service.UpdateGate", 1971069))
      {
        IDataAccessLayer dataAccessLayer = this.getDataAccessLayer(requestContext, projectId);
        ReleaseEnvironmentStep gateStep = dataAccessLayer.GetReleaseStep(gateStepId);
        releaseManagementTimer.RecordLap("DataAccessLayer", "ReleaseGatesService.UpdateGate.GetReleaseStep", 1971069);
        if (gateStep.Status != ReleaseEnvironmentStepStatus.Pending)
          throw new InvalidRequestException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Microsoft.VisualStudio.Services.ReleaseManagement.Data.Properties.Resources.GatesAreNotInProgress, (object) gateStep.ReleaseId, (object) gateStep.Id));
        if (!gateStep.IsGateStep())
          throw new InvalidRequestException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Microsoft.VisualStudio.Services.ReleaseManagement.Data.Properties.Resources.InvalidGateStepType, (object) gateStep.Id));
        ReleaseGatesService.ValidateUserHasManageReleaseApproversPermission(requestContext, projectId, gateStep);
        releaseManagementTimer.RecordLap("Service", "ReleaseGatesService.UpdateGate.ValidatePermission", 1971069);
        Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Release release = dataAccessLayer.GetRelease(gateStep.ReleaseId);
        releaseManagementTimer.RecordLap("DataAccessLayer", "ReleaseGatesService.UpdateGate.GetRelease", 1971069);
        Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ReleaseEnvironment environment = release.GetEnvironment(gateStep.ReleaseEnvironmentId);
        if (release.Status != Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Enumerations.ReleaseStatus.Active)
          throw new InvalidRequestException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Microsoft.VisualStudio.Services.ReleaseManagement.Server.Properties.Resources.CannotUpdateEnvironmentAsReleaseIsNotActive, (object) environment.Name, (object) release.Name, (object) Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Enumerations.ReleaseStatus.Active));
        DeploymentGate deploymentGate1 = environment.GetDeploymentByAttempt(gateStep.TrialNumber).DeploymentGates.FirstOrDefault<DeploymentGate>((Func<DeploymentGate, bool>) (g => g.GateType == gateStep.StepType));
        if (requestContext.IsFeatureEnabled("AzureDevOps.ReleaseManagement.HonorIsGeneratedBitForIgnoredGates"))
          ReleaseGatesService.ValidateGivenGatesCanBeIgnored(gateUpdateMetadata, gateStep, environment);
        else
          ReleaseGatesService.ValidateGivenIgnoreGatesArePresentInDeploymentGates(gateUpdateMetadata, gateStep, environment);
        ReleaseGatesService.ValidateDeploymentGateIsInprogress(deploymentGate1, gateStep);
        bool needToUpdateDb = false;
        ReleaseGatesService.HasAnyGateToUpdateInDb(deploymentGate1, gateUpdateMetadata, out needToUpdateDb);
        DeploymentGate deploymentGate2 = deploymentGate1;
        if (needToUpdateDb)
        {
          deploymentGate2 = ReleaseGatesService.IgnoreGate(dataAccessLayer, release.Id, gateStepId, deploymentGate2, (IEnumerable<string>) gateUpdateMetadata.GatesToIgnore, gateUpdateMetadata.Comment, false);
          releaseManagementTimer.RecordLap("DataAccessLayer", "ReleaseGatesService.UpdateGate.IgnoreGateProcessing", 1971069);
        }
        this.getGatesOrchestrator(requestContext, projectId).UpdateGate(deploymentGate1.RunPlanId.Value, (IList<string>) gateUpdateMetadata.GatesToIgnore);
        releaseManagementTimer.RecordLap("DistributedTask", "ReleaseGatesService.UpdateGate.SendOrchestratorMessage", 1971069);
        DeploymentGate deploymentGate3 = ReleaseGatesService.MarkGatesIgnored(requestContext, release.Id, environment.Id, gateStepId, gateUpdateMetadata, dataAccessLayer, deploymentGate2);
        releaseManagementTimer.RecordLap("DataAccessLayer", "ReleaseGatesService.UpdateGate.IgnoreGateProcessed", 1971069);
        return deploymentGate3.ToReleaseGates();
      }
    }

    private static DeploymentGate IgnoreGate(
      IDataAccessLayer dataAccessLayer,
      int releaseId,
      int gateStepId,
      DeploymentGate existingDeploymentGate,
      IEnumerable<string> newGatesToIgnore,
      string comment,
      bool markProcessed)
    {
      string ignoredGatesJson = IgnoredGatesExtension.ToIgnoredGatesJson(existingDeploymentGate.IgnoredGates);
      List<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.IgnoredGate> source = new List<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.IgnoredGate>();
      if (existingDeploymentGate.IgnoredGates != null)
        source = new List<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.IgnoredGate>((IEnumerable<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.IgnoredGate>) existingDeploymentGate.IgnoredGates);
      DateTime utcNow = DateTime.UtcNow;
      List<string> stringList = new List<string>();
      foreach (string str in newGatesToIgnore)
      {
        string gate = str;
        Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.IgnoredGate ignoredGate = source.FirstOrDefault<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.IgnoredGate>((Func<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.IgnoredGate, bool>) (g => string.Equals(g.Name, gate, StringComparison.OrdinalIgnoreCase)));
        if (ignoredGate == null)
        {
          source.Add(new Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.IgnoredGate()
          {
            Name = gate,
            IsProcessed = markProcessed,
            LastModifiedOn = utcNow
          });
          stringList.Add(gate);
        }
        else
        {
          if (markProcessed && !ignoredGate.IsProcessed)
            stringList.Add(gate);
          ignoredGate.IsProcessed = markProcessed;
        }
      }
      string afterGatesIgnored = ServerModelUtility.ToString((object) source);
      return !stringList.Any<string>() ? existingDeploymentGate : dataAccessLayer.IgnoreGates(releaseId, gateStepId, (IEnumerable<string>) stringList, ignoredGatesJson, afterGatesIgnored, comment, markProcessed);
    }

    private static DeploymentGate MarkGatesIgnored(
      IVssRequestContext requestContext,
      int releaseId,
      int releaseEnvironmentId,
      int gateStepId,
      GateUpdateMetadata gateUpdateMetadata,
      IDataAccessLayer dataAccessLayer,
      DeploymentGate updatedDeploymentGate)
    {
      int num = 2;
      for (int index = 0; index < num; ++index)
      {
        try
        {
          updatedDeploymentGate = ReleaseGatesService.IgnoreGate(dataAccessLayer, releaseId, gateStepId, updatedDeploymentGate, (IEnumerable<string>) gateUpdateMetadata.GatesToIgnore, gateUpdateMetadata.Comment, true);
          break;
        }
        catch (GateUpdateFailedException ex)
        {
          ReleaseGatesService.TraceInformationMessage(requestContext, TraceLevel.Error, 1971069, "GateService: IgnoreGate marking processed failed for the gates {0}. ReleaseId: {1}, StepId: {2}, Exception {3}", (object) string.Join(",", (IEnumerable<string>) gateUpdateMetadata.GatesToIgnore), (object) releaseId, (object) gateStepId, (object) ex.Message);
          updatedDeploymentGate = dataAccessLayer.GetDeploymentGate(releaseId, releaseEnvironmentId, gateStepId);
        }
      }
      return updatedDeploymentGate;
    }

    private static void ValidateDeploymentGateIsInprogress(
      DeploymentGate deploymentGate,
      ReleaseEnvironmentStep gateStep)
    {
      if (deploymentGate == null || !deploymentGate.RunPlanId.HasValue || !deploymentGate.RunPlanId.HasValue)
        throw new InvalidRequestException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Microsoft.VisualStudio.Services.ReleaseManagement.Server.Properties.Resources.NoGreenlightingPlan, (object) gateStep.ReleaseId, (object) gateStep.ReleaseEnvironmentId, (object) gateStep.Id));
      if (deploymentGate.Status != Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Enumerations.GateStatus.InProgress)
        throw new InvalidRequestException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Microsoft.VisualStudio.Services.ReleaseManagement.Data.Properties.Resources.GatesAreNotInProgress, (object) gateStep.ReleaseId, (object) gateStep.Id));
    }

    private static void ValidateInput(GateUpdateMetadata gateUpdateMetadata)
    {
      if (gateUpdateMetadata == null)
        throw new ArgumentNullException(nameof (gateUpdateMetadata));
      if (gateUpdateMetadata.GatesToIgnore == null)
        throw new InvalidRequestException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Microsoft.VisualStudio.Services.ReleaseManagement.Server.Properties.Resources.InvalidGateUpdateMetaDataName));
      gateUpdateMetadata.GatesToIgnore = gateUpdateMetadata.GatesToIgnore.Where<string>((Func<string, bool>) (g => !string.IsNullOrWhiteSpace(g))).ToList<string>();
      if (!gateUpdateMetadata.GatesToIgnore.Any<string>())
        throw new InvalidRequestException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Microsoft.VisualStudio.Services.ReleaseManagement.Server.Properties.Resources.InvalidGateUpdateMetaDataName));
    }

    [SuppressMessage("Microsoft.Maintainability", "CA1502: Avoid excessive complexity", Justification = "Need to override RM build rule")]
    private static void ValidateGivenGatesCanBeIgnored(
      GateUpdateMetadata gateUpdateMetadata,
      ReleaseEnvironmentStep gateStep,
      Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ReleaseEnvironment releaseEnvironment)
    {
      ReleaseDefinitionGatesStep definitionGatesStep = gateStep.StepType == EnvironmentStepType.PreGate ? releaseEnvironment.PreDeploymentGates : releaseEnvironment.PostDeploymentGates;
      if (definitionGatesStep == null || definitionGatesStep.Gates == null)
        throw new InvalidRequestException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Microsoft.VisualStudio.Services.ReleaseManagement.Server.Properties.Resources.GatesNotExist, (object) string.Join(", ", (IEnumerable<string>) gateUpdateMetadata.GatesToIgnore)));
      IEnumerable<\u003C\u003Ef__AnonymousType1<bool, string>> inner = definitionGatesStep.Gates.Where<ReleaseDefinitionGate>((Func<ReleaseDefinitionGate, bool>) (g => g != null && g.Tasks != null)).SelectMany((Func<ReleaseDefinitionGate, IEnumerable<WorkflowTask>>) (g => g.Tasks.Where<WorkflowTask>((Func<WorkflowTask, bool>) (t => t != null))), (g, t) => new
      {
        IsGenerated = g.IsGenerated,
        Name = t.Name
      }).Where(t => !string.IsNullOrEmpty(t.Name));
      IEnumerable<\u003C\u003Ef__AnonymousType2<string, bool, bool>> source = gateUpdateMetadata.GatesToIgnore.GroupJoin(inner, (Func<string, string>) (g => g), g => g.Name, (ignored, existing) => new
      {
        GateToIgnore = ignored,
        Exists = existing.Any(),
        IsGenerated = existing.Any(g => g.IsGenerated)
      }, (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
      IEnumerable<string> strings1 = source.Where(g => !g.Exists).Select(g => g.GateToIgnore);
      if (strings1.Any<string>())
        throw new InvalidRequestException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Microsoft.VisualStudio.Services.ReleaseManagement.Server.Properties.Resources.GatesNotExist, (object) string.Join(", ", strings1)));
      IEnumerable<string> strings2 = source.Where(g => g.IsGenerated).Select(g => g.GateToIgnore);
      if (strings2.Any<string>())
        throw new InvalidRequestException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Microsoft.VisualStudio.Services.ReleaseManagement.Server.Properties.Resources.GeneratedGates, (object) string.Join(", ", strings2)));
    }

    private static void ValidateGivenIgnoreGatesArePresentInDeploymentGates(
      GateUpdateMetadata gateUpdateMetadata,
      ReleaseEnvironmentStep gateStep,
      Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ReleaseEnvironment releaseEnvironment)
    {
      ReleaseDefinitionGatesStep definitionGatesStep = gateStep.StepType == EnvironmentStepType.PreGate ? releaseEnvironment.PreDeploymentGates : releaseEnvironment.PostDeploymentGates;
      if (definitionGatesStep == null || definitionGatesStep.Gates == null)
        throw new InvalidRequestException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Microsoft.VisualStudio.Services.ReleaseManagement.Server.Properties.Resources.GatesNotExist, (object) string.Join(",", (IEnumerable<string>) gateUpdateMetadata.GatesToIgnore)));
      IEnumerable<string> second = definitionGatesStep.Gates.Where<ReleaseDefinitionGate>((Func<ReleaseDefinitionGate, bool>) (g => g != null && g.Tasks != null)).SelectMany<ReleaseDefinitionGate, WorkflowTask>((Func<ReleaseDefinitionGate, IEnumerable<WorkflowTask>>) (g => (IEnumerable<WorkflowTask>) g.Tasks)).Where<WorkflowTask>((Func<WorkflowTask, bool>) (t => t != null)).Select<WorkflowTask, string>((Func<WorkflowTask, string>) (t => t.Name));
      IEnumerable<string> strings = gateUpdateMetadata.GatesToIgnore.Except<string>(second, (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
      if (strings.Any<string>())
        throw new InvalidRequestException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Microsoft.VisualStudio.Services.ReleaseManagement.Server.Properties.Resources.GatesNotExist, (object) string.Join(",", strings)));
    }

    private static void HasAnyGateToUpdateInDb(
      DeploymentGate deploymentGate,
      GateUpdateMetadata gateUpdateMetadata,
      out bool needToUpdateDb)
    {
      needToUpdateDb = false;
      if (deploymentGate.IgnoredGates == null)
      {
        needToUpdateDb = true;
      }
      else
      {
        foreach (string str in gateUpdateMetadata.GatesToIgnore)
        {
          string gate = str;
          Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.IgnoredGate ignoredGate = deploymentGate.IgnoredGates.FirstOrDefault<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.IgnoredGate>((Func<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.IgnoredGate, bool>) (g => string.Equals(g.Name, gate, StringComparison.OrdinalIgnoreCase)));
          if (ignoredGate != null && ignoredGate.IsProcessed)
            throw new InvalidRequestException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Microsoft.VisualStudio.Services.ReleaseManagement.Data.Properties.Resources.DeploymentGateAlreadyIgnored, (object) gate));
          needToUpdateDb = true;
        }
      }
    }

    private static void ValidateUserHasManageReleaseApproversPermission(
      IVssRequestContext requestContext,
      Guid projectId,
      ReleaseEnvironmentStep gateStep)
    {
      string folderPath = ReleaseManagementSecurityProcessor.GetFolderPath(requestContext, projectId, gateStep.ReleaseDefinitionId);
      if (!requestContext.HasPermission(projectId, folderPath, gateStep.ReleaseDefinitionId, gateStep.DefinitionEnvironmentId, ReleaseManagementSecurityPermissions.ManageReleaseApprovers))
      {
        ResourceAccessException innerException = new ResourceAccessException(requestContext.RootContext.GetUserId().ToString(), ReleaseManagementSecurityPermissions.ManageReleaseApprovers);
        throw new UnauthorizedRequestException(innerException.Message, (Exception) innerException);
      }
    }

    private static IDataAccessLayer GetDataAccessLayer(IVssRequestContext context, Guid projectId) => (IDataAccessLayer) new DataAccessLayer(context, projectId);

    private static GreenlightingOrchestrator GetGatesOrchestrator(
      IVssRequestContext context,
      Guid projectId)
    {
      return new GreenlightingOrchestrator(context, projectId);
    }

    private static void TraceInformationMessage(
      IVssRequestContext context,
      TraceLevel traceLevel,
      int tracePoint,
      string format,
      params object[] args)
    {
      VssRequestContextExtensions.Trace(context, tracePoint, traceLevel, "ReleaseManagementService", "Service", format, args);
    }
  }
}
