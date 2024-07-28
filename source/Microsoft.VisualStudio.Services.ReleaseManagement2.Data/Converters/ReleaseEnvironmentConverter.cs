// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Data.Converters.ReleaseEnvironmentConverter
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Data, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A69B0FB7-3028-4162-BA38-794D80D7A49A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Data.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.ReleaseManagement.Common.Helpers;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Exceptions;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Enumerations;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Properties;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Utilities;
using Microsoft.VisualStudio.Services.ReleaseManagement.WebApi;
using Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts;
using Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts.Conditions;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Data.Converters
{
  public static class ReleaseEnvironmentConverter
  {
    private static readonly IDictionary<ReleaseEnvironmentStepStatus, EnvironmentStatus> CurrentStepStatusToEnvironmentStatusMap = (IDictionary<ReleaseEnvironmentStepStatus, EnvironmentStatus>) new Dictionary<ReleaseEnvironmentStepStatus, EnvironmentStatus>()
    {
      {
        ReleaseEnvironmentStepStatus.Done,
        EnvironmentStatus.InProgress
      },
      {
        ReleaseEnvironmentStepStatus.Pending,
        EnvironmentStatus.InProgress
      },
      {
        ReleaseEnvironmentStepStatus.Reassigned,
        EnvironmentStatus.InProgress
      },
      {
        ReleaseEnvironmentStepStatus.Stopped,
        EnvironmentStatus.Rejected
      },
      {
        ReleaseEnvironmentStepStatus.Abandoned,
        EnvironmentStatus.Rejected
      },
      {
        ReleaseEnvironmentStepStatus.Rejected,
        EnvironmentStatus.Rejected
      },
      {
        ReleaseEnvironmentStepStatus.Canceled,
        EnvironmentStatus.Canceled
      },
      {
        ReleaseEnvironmentStepStatus.Skipped,
        EnvironmentStatus.InProgress
      },
      {
        ReleaseEnvironmentStepStatus.PartiallySucceeded,
        EnvironmentStatus.InProgress
      }
    };

    [SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity", Justification = "environment is bulky by design")]
    public static Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ReleaseEnvironment ToWebApi(
      this Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ReleaseEnvironment serverEnvironment,
      Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Release release,
      IVssRequestContext context,
      Guid projectId)
    {
      if (serverEnvironment == null)
        throw new ArgumentNullException(nameof (serverEnvironment));
      if (context == null)
        throw new ArgumentNullException(nameof (context));
      if (release == null)
        throw new ArgumentNullException(nameof (release));
      Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ReleaseEnvironment webApi = new Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ReleaseEnvironment()
      {
        Id = serverEnvironment.Id,
        Name = serverEnvironment.Name,
        ReleaseId = serverEnvironment.ReleaseId,
        Rank = serverEnvironment.Rank,
        DefinitionEnvironmentId = serverEnvironment.DefinitionEnvironmentId,
        ScheduledDeploymentTime = serverEnvironment.ScheduledDeploymentTime,
        EnvironmentOptions = EnvironmentOptionsConverter.ToWebApiEnvironmentOptions(serverEnvironment.EnvironmentOptions),
        ReleaseReference = new ReleaseShallowReference()
        {
          Id = release.Id,
          Name = release.Name
        },
        ReleaseDefinitionReference = new Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts.ReleaseDefinitionShallowReference()
        {
          Id = release.ReleaseDefinitionId,
          Name = release.ReleaseDefinitionName,
          Path = release.ReleaseDefinitionPath
        },
        ReleaseCreatedBy = new IdentityRef()
        {
          Id = release.CreatedBy.ToString()
        },
        ProcessParameters = serverEnvironment.ProcessParameters,
        PreDeploymentGatesSnapshot = serverEnvironment.PreDeploymentGates,
        PostDeploymentGatesSnapshot = serverEnvironment.PostDeploymentGates
      };
      webApi.Status = ReleaseEnvironmentConverter.GetEnvironmentStatus(release, serverEnvironment);
      webApi.Owner = new IdentityRef()
      {
        Id = serverEnvironment.OwnerId.ToString()
      };
      ReleaseEnvironmentConverter.PopulateEnvironmentConditions(webApi, serverEnvironment);
      ReleaseEnvironmentConverter.PopulateEnvironmentSchedules(context, webApi, serverEnvironment);
      ReleaseEnvironmentConverter.SetEnvironmentDateTimeFields(serverEnvironment, webApi);
      webApi.FillDeploySteps(serverEnvironment, context, projectId);
      foreach (DeployPhaseSnapshot deployPhaseSnapshot in (IEnumerable<DeployPhaseSnapshot>) serverEnvironment.GetDesignerDeployPhaseSnapshots())
        webApi.DeployPhasesSnapshot.Add(deployPhaseSnapshot.ToWebApiDeployPhaseSnapshot());
      List<Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts.DeployPhase> list = webApi.DeployPhasesSnapshot.OrderBy<Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts.DeployPhase, int>((Func<Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts.DeployPhase, int>) (dps => dps.Rank)).ToList<Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts.DeployPhase>();
      webApi.DeployPhasesSnapshot.Clear();
      webApi.DeployPhasesSnapshot.AddRange<Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts.DeployPhase, IList<Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts.DeployPhase>>((IEnumerable<Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts.DeployPhase>) list);
      IEnumerable<ReleaseApproval> approvalContract1 = ReleaseEnvironmentConverter.ConvertStepsToApprovalContract(serverEnvironment.GetStepsForTests.Where<ReleaseEnvironmentStep>((Func<ReleaseEnvironmentStep, bool>) (step => step.StepType == EnvironmentStepType.PreDeploy)), release.DefinitionSnapshot);
      IEnumerable<ReleaseApproval> approvalContract2 = ReleaseEnvironmentConverter.ConvertStepsToApprovalContract(serverEnvironment.GetStepsForTests.Where<ReleaseEnvironmentStep>((Func<ReleaseEnvironmentStep, bool>) (step => step.StepType == EnvironmentStepType.PostDeploy)), release.DefinitionSnapshot);
      foreach (ReleaseApproval approval in approvalContract1)
      {
        approval.PopulateShallowReferences(context, release, serverEnvironment, projectId);
        webApi.PreDeployApprovals.Add(approval);
      }
      foreach (ReleaseApproval approval in approvalContract2)
      {
        approval.PopulateShallowReferences(context, release, serverEnvironment, projectId);
        webApi.PostDeployApprovals.Add(approval);
      }
      foreach (KeyValuePair<string, Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ConfigurationVariableValue> variable in (IEnumerable<KeyValuePair<string, Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ConfigurationVariableValue>>) serverEnvironment.Variables)
        webApi.Variables[variable.Key] = variable.Value.ToWebApiConfigurationVariableValue();
      foreach (Microsoft.TeamFoundation.DistributedTask.WebApi.VariableGroup variableGroup in (IEnumerable<Microsoft.TeamFoundation.DistributedTask.WebApi.VariableGroup>) serverEnvironment.VariableGroups)
      {
        Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts.VariableGroup apiVariableGroup = VariableGroupConverter.ToWebApiVariableGroup(variableGroup);
        if (apiVariableGroup != null)
          webApi.VariableGroups.Add(apiVariableGroup);
      }
      string restUrlForRelease = RestUrlHelper.GetRestUrlForRelease(context, projectId, release.Id);
      webApi.ReleaseReference = new ReleaseShallowReference()
      {
        Id = release.Id,
        Name = release.Name,
        Url = restUrlForRelease
      };
      string releaseWebAccessUri = WebAccessUrlBuilder.GetReleaseWebAccessUri(context, projectId.ToString(), release.Id);
      webApi.ReleaseReference.Links.AddLink("web", releaseWebAccessUri);
      webApi.ReleaseReference.Links.AddLink("self", restUrlForRelease);
      string releaseDefinition = RestUrlHelper.GetRestUrlForReleaseDefinition(context, projectId, release.ReleaseDefinitionId);
      webApi.ReleaseDefinitionReference = new Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts.ReleaseDefinitionShallowReference()
      {
        Id = release.ReleaseDefinitionId,
        Name = release.ReleaseDefinitionName,
        Path = release.ReleaseDefinitionPath,
        Url = releaseDefinition
      };
      string definitionWebAccessUri = WebAccessUrlBuilder.GetReleaseDefinitionWebAccessUri(context, projectId.ToString(), release.ReleaseDefinitionId);
      webApi.ReleaseDefinitionReference.Links.AddLink("web", definitionWebAccessUri);
      webApi.ReleaseDefinitionReference.Links.AddLink("self", releaseDefinition);
      webApi.ReleaseCreatedBy = new IdentityRef()
      {
        Id = release.CreatedBy.ToString()
      };
      ReleaseEnvironmentStep latestDeployStep = serverEnvironment.GetLatestDeployStep();
      if (latestDeployStep != null)
      {
        TimeSpan timeSpan = latestDeployStep.ModifiedOn - latestDeployStep.CreatedOn;
        webApi.TimeToDeploy = timeSpan.TotalMinutes;
      }
      if (webApi.Conditions.Any<ReleaseCondition>())
      {
        ReleaseCondition condition = webApi.Conditions[0];
        if (condition.ConditionType == ConditionType.Event)
        {
          webApi.TriggerReason = condition.Name;
        }
        else
        {
          IEnumerable<string> values = webApi.Conditions.Where<ReleaseCondition>((Func<ReleaseCondition, bool>) (c => c.ConditionType == ConditionType.EnvironmentState)).Select<ReleaseCondition, string>((Func<ReleaseCondition, string>) (c => c.Name));
          webApi.TriggerReason = string.Format((IFormatProvider) CultureInfo.InvariantCulture, Resources.TriggerReasonOnSuccesfulDeployment, (object) string.Join(",", values));
        }
      }
      else
        webApi.TriggerReason = "Manual";
      return webApi;
    }

    [SuppressMessage("Microsoft.Maintainability", "CA1506:AvoidExcessiveClassCoupling", Justification = "release object has multiple layers")]
    public static Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ReleaseEnvironment FromWebApi(
      this Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ReleaseEnvironment webApiEnvironment)
    {
      if (webApiEnvironment == null)
        throw new ArgumentNullException(nameof (webApiEnvironment));
      Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ReleaseEnvironment releaseEnvironment = new Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ReleaseEnvironment()
      {
        Id = webApiEnvironment.Id,
        Name = webApiEnvironment.Name,
        ReleaseId = webApiEnvironment.ReleaseId,
        Rank = webApiEnvironment.Rank,
        DefinitionEnvironmentId = webApiEnvironment.DefinitionEnvironmentId,
        ProcessParameters = webApiEnvironment.ProcessParameters,
        PreDeploymentGates = webApiEnvironment.PreDeploymentGatesSnapshot,
        PostDeploymentGates = webApiEnvironment.PostDeploymentGatesSnapshot
      };
      releaseEnvironment.EnvironmentOptions = EnvironmentOptionsConverter.ToServerEnvironmentOptions(webApiEnvironment.EnvironmentOptions);
      foreach (Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts.DeployPhase deployPhaseSnapshot1 in (IEnumerable<Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts.DeployPhase>) webApiEnvironment.DeployPhasesSnapshot)
      {
        DeployPhaseSnapshot deployPhaseSnapshot2 = deployPhaseSnapshot1.ToServerDeployPhaseSnapshot();
        releaseEnvironment.AddDesignerDeployPhaseSnapshot(deployPhaseSnapshot2);
      }
      Guid result;
      if (webApiEnvironment.Owner != null && Guid.TryParse(webApiEnvironment.Owner.Id, out result))
        releaseEnvironment.OwnerId = result;
      releaseEnvironment.Conditions = (IList<ReleaseCondition>) new List<ReleaseCondition>();
      if (webApiEnvironment.Conditions != null)
      {
        foreach (ReleaseCondition condition in webApiEnvironment.Conditions)
          releaseEnvironment.Conditions.Add(condition);
      }
      if (webApiEnvironment.Schedules != null)
      {
        foreach (Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ReleaseSchedule schedule in webApiEnvironment.Schedules)
          releaseEnvironment.Schedules.Add(schedule.FromWebApi());
      }
      foreach (KeyValuePair<string, Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ConfigurationVariableValue> variable in (IEnumerable<KeyValuePair<string, Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ConfigurationVariableValue>>) webApiEnvironment.Variables)
        releaseEnvironment.Variables[variable.Key] = variable.Value.ToServerConfigurationVariableValue();
      foreach (Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts.VariableGroup variableGroup in (IEnumerable<Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts.VariableGroup>) webApiEnvironment.VariableGroups)
      {
        Microsoft.TeamFoundation.DistributedTask.WebApi.VariableGroup serverVariableGroup = VariableGroupConverter.ToServerVariableGroup(variableGroup);
        if (serverVariableGroup != null)
          releaseEnvironment.VariableGroups.Add(serverVariableGroup);
      }
      releaseEnvironment.PreApprovalOptions = webApiEnvironment.PreApprovalsSnapshot.ApprovalOptions.FromWebApiApprovalOptions();
      releaseEnvironment.PostApprovalOptions = webApiEnvironment.PostApprovalsSnapshot.ApprovalOptions.FromWebApiApprovalOptions();
      return releaseEnvironment;
    }

    private static void PopulateShallowReferences(
      this ReleaseApproval approval,
      IVssRequestContext context,
      Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Release release,
      Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ReleaseEnvironment releaseEnvironment,
      Guid projectId)
    {
      approval.ReleaseReference = ShallowReferencesHelper.CreateReleaseShallowReference(context, projectId, release.Id, release.Name);
      approval.ReleaseDefinitionReference = ShallowReferencesHelper.CreateDefinitionShallowReference(context, projectId, release.ReleaseDefinitionId, release.ReleaseDefinitionName, release.ReleaseDefinitionPath);
      approval.ReleaseEnvironmentReference = ShallowReferencesHelper.CreateReleaseEnvironmentShallowReference(context, projectId, release.Id, releaseEnvironment.Id, releaseEnvironment.Name);
      approval.Url = WebAccessUrlBuilder.GetReleaseApprovalRestUri(context, projectId, approval.Id);
    }

    public static EnvironmentStatus GetEnvironmentStatus(
      Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Release release,
      Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ReleaseEnvironment environment)
    {
      if (release == null)
        throw new ArgumentNullException(nameof (release));
      if (environment == null)
        throw new ArgumentNullException(nameof (environment));
      if (environment.Status != ReleaseEnvironmentStatus.Undefined)
        return environment.Status.ToWebApi();
      if (!environment.GetStepsForTests.Any<ReleaseEnvironmentStep>())
        return EnvironmentStatus.NotStarted;
      if (ReleaseEnvironmentConverter.IsTargetEnvironmentReached(release, environment) || ReleaseEnvironmentConverter.IsNextEnvironmentExecutionStarted(release, environment))
        return EnvironmentStatus.Succeeded;
      ReleaseEnvironmentStep releaseEnvironmentStep = environment.GetStepsForTests.OrderBy<ReleaseEnvironmentStep, DateTime>((Func<ReleaseEnvironmentStep, DateTime>) (e => e.CreatedOn)).Last<ReleaseEnvironmentStep>();
      if (releaseEnvironmentStep.StepType != EnvironmentStepType.PreDeploy)
        return ReleaseEnvironmentConverter.CurrentStepStatusToEnvironmentStatusMap[releaseEnvironmentStep.Status];
      switch (releaseEnvironmentStep.Status)
      {
        case ReleaseEnvironmentStepStatus.Rejected:
          return EnvironmentStatus.Rejected;
        case ReleaseEnvironmentStepStatus.Abandoned:
          return EnvironmentStatus.Rejected;
        case ReleaseEnvironmentStepStatus.Canceled:
          return EnvironmentStatus.Canceled;
        case ReleaseEnvironmentStepStatus.PartiallySucceeded:
          return EnvironmentStatus.PartiallySucceeded;
        default:
          return EnvironmentStatus.InProgress;
      }
    }

    public static IDictionary<string, Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ApprovalOptions> GetPreAndPostApprovalOptionsFromReleaseEnvironment(
      this Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ReleaseEnvironment releaseEnvironment)
    {
      if (releaseEnvironment == null)
        throw new ArgumentNullException(nameof (releaseEnvironment));
      Dictionary<string, Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ApprovalOptions> releaseEnvironment1 = new Dictionary<string, Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ApprovalOptions>();
      if (releaseEnvironment.PreApprovalOptions != null)
        releaseEnvironment1.Add("pre", releaseEnvironment.PreApprovalOptions);
      if (releaseEnvironment.PostApprovalOptions != null)
        releaseEnvironment1.Add("post", releaseEnvironment.PostApprovalOptions);
      return (IDictionary<string, Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ApprovalOptions>) releaseEnvironment1;
    }

    public static void PopulateReleaseEnvironmentPreAndPostApprovalOptions(
      this Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ReleaseEnvironment releaseEnvironment,
      IDictionary<string, Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ApprovalOptions> approvalOptions)
    {
      if (releaseEnvironment == null)
        throw new ArgumentNullException(nameof (releaseEnvironment));
      if (approvalOptions == null)
        return;
      Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ApprovalOptions approvalOptions1;
      if (approvalOptions.TryGetValue("pre", out approvalOptions1))
        releaseEnvironment.PreApprovalOptions = approvalOptions1;
      if (!approvalOptions.TryGetValue("post", out approvalOptions1))
        return;
      releaseEnvironment.PostApprovalOptions = approvalOptions1;
    }

    public static IDictionary<string, ReleaseDefinitionGatesStep> GetDefinitionGates(
      this Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ReleaseEnvironment releaseEnvironment)
    {
      if (releaseEnvironment == null)
        throw new ArgumentNullException(nameof (releaseEnvironment));
      Dictionary<string, ReleaseDefinitionGatesStep> definitionGates = new Dictionary<string, ReleaseDefinitionGatesStep>();
      if (releaseEnvironment.PreDeploymentGates != null)
        definitionGates.Add("pre", releaseEnvironment.PreDeploymentGates);
      if (releaseEnvironment.PostDeploymentGates != null)
        definitionGates.Add("post", releaseEnvironment.PostDeploymentGates);
      return (IDictionary<string, ReleaseDefinitionGatesStep>) definitionGates;
    }

    public static void PopulateDefinitionGates(
      this Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ReleaseEnvironment releaseEnvironment,
      IDictionary<string, ReleaseDefinitionGatesStep> deploymentGates)
    {
      if (releaseEnvironment == null)
        throw new ArgumentNullException(nameof (releaseEnvironment));
      if (deploymentGates == null)
        return;
      ReleaseDefinitionGatesStep definitionGatesStep;
      if (deploymentGates.TryGetValue("pre", out definitionGatesStep))
        releaseEnvironment.PreDeploymentGates = definitionGatesStep;
      if (!deploymentGates.TryGetValue("post", out definitionGatesStep))
        return;
      releaseEnvironment.PostDeploymentGates = definitionGatesStep;
    }

    public static IEnumerable<ReleaseApproval> ConvertStepsToApprovalContract(
      IEnumerable<ReleaseEnvironmentStep> steps,
      ReleaseDefinitionEnvironmentsSnapshot definitionSnapshot)
    {
      List<ReleaseEnvironmentStep> list = steps.Where<ReleaseEnvironmentStep>((Func<ReleaseEnvironmentStep, bool>) (step => step.Status != ReleaseEnvironmentStepStatus.Abandoned && step.Status != ReleaseEnvironmentStepStatus.Stopped)).ToList<ReleaseEnvironmentStep>();
      IEnumerable<ReleaseEnvironmentStep> source = list.Where<ReleaseEnvironmentStep>((Func<ReleaseEnvironmentStep, bool>) (step => step.Status != ReleaseEnvironmentStepStatus.Reassigned));
      IEnumerable<ReleaseEnvironmentStep> approvalHistorySteps = list.Where<ReleaseEnvironmentStep>((Func<ReleaseEnvironmentStep, bool>) (step => step.Status == ReleaseEnvironmentStepStatus.Reassigned));
      Func<ReleaseEnvironmentStep, ReleaseApproval> selector = (Func<ReleaseEnvironmentStep, ReleaseApproval>) (step => step.ToWebApiApprovalWithHistory(approvalHistorySteps, definitionSnapshot));
      return source.Select<ReleaseEnvironmentStep, ReleaseApproval>(selector);
    }

    private static void PopulateEnvironmentConditions(
      Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ReleaseEnvironment webApiEnvironment,
      Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ReleaseEnvironment serverEnvironment)
    {
      foreach (ReleaseCondition releaseCondition in (IEnumerable<ReleaseCondition>) (serverEnvironment.Conditions ?? (IList<ReleaseCondition>) new List<ReleaseCondition>()))
      {
        ConditionsUtility.ConvertToWebApiCondition((Condition) releaseCondition);
        webApiEnvironment.Conditions.Add(releaseCondition);
      }
    }

    private static void PopulateEnvironmentSchedules(
      IVssRequestContext requestContext,
      Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ReleaseEnvironment webApiEnvironment,
      Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ReleaseEnvironment serverEnvironment)
    {
      foreach (Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ReleaseSchedule releaseSchedule in (IEnumerable<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ReleaseSchedule>) (serverEnvironment.Schedules ?? (IList<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ReleaseSchedule>) new List<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ReleaseSchedule>()))
        webApiEnvironment.Schedules.Add(releaseSchedule.ToWebApi());
      if (webApiEnvironment.Status != EnvironmentStatus.Scheduled || !webApiEnvironment.Schedules.Any<Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ReleaseSchedule>())
        return;
      webApiEnvironment.NextScheduledUtcTime = new DateTime?(ReleaseEnvironmentConverter.GetNextScheduledUtcTime(requestContext, webApiEnvironment.Schedules.First<Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ReleaseSchedule>()));
    }

    private static Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ScheduleDays ConvertDayOfWeek(
      DayOfWeek dayOfWeek)
    {
      switch (dayOfWeek)
      {
        case DayOfWeek.Sunday:
          return Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ScheduleDays.Sunday;
        case DayOfWeek.Monday:
          return Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ScheduleDays.Monday;
        case DayOfWeek.Tuesday:
          return Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ScheduleDays.Tuesday;
        case DayOfWeek.Wednesday:
          return Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ScheduleDays.Wednesday;
        case DayOfWeek.Thursday:
          return Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ScheduleDays.Thursday;
        case DayOfWeek.Friday:
          return Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ScheduleDays.Friday;
        case DayOfWeek.Saturday:
          return Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ScheduleDays.Saturday;
        default:
          return Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ScheduleDays.None;
      }
    }

    private static Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ScheduleDays ConvertNextDayOfWeek(
      DayOfWeek dayOfWeek)
    {
      switch (dayOfWeek)
      {
        case DayOfWeek.Sunday:
          return Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ScheduleDays.Monday;
        case DayOfWeek.Monday:
          return Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ScheduleDays.Tuesday;
        case DayOfWeek.Tuesday:
          return Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ScheduleDays.Wednesday;
        case DayOfWeek.Wednesday:
          return Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ScheduleDays.Thursday;
        case DayOfWeek.Thursday:
          return Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ScheduleDays.Friday;
        case DayOfWeek.Friday:
          return Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ScheduleDays.Saturday;
        case DayOfWeek.Saturday:
          return Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ScheduleDays.Sunday;
        default:
          return Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ScheduleDays.None;
      }
    }

    [SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", Justification = "This is not shown to the user and is for tracing")]
    [SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", Justification = "This is not shown to the user and is for tracing")]
    [SuppressMessage("Microsoft.Globalization", "CA1305:SpecifyIFormatProvider", MessageId = "System.String.Format(System.String,System.Object[])", Justification = "Error message")]
    private static DateTime GetNextScheduledUtcTime(
      IVssRequestContext requestContext,
      Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ReleaseSchedule schedule)
    {
      DateTime dateTime1 = DateTime.MinValue;
      try
      {
        DateTime dateTime2 = TimeZoneInfo.ConvertTimeBySystemTimeZoneId(DateTime.UtcNow, schedule.TimeZoneId);
        try
        {
          dateTime1 = new DateTime(dateTime2.Year, dateTime2.Month, dateTime2.Day, schedule.StartHours, schedule.StartMinutes, 0, 0);
        }
        catch (ArgumentOutOfRangeException ex)
        {
          throw new InvalidRequestException(string.Format("Time Now: {0}, Time Zone Id: {1}, Year: {2}, Month: {3}, Day: {4}, Start Hours: {5}, Start Minutes: {6}", (object) DateTime.UtcNow, (object) schedule.TimeZoneId, (object) dateTime2.Year, (object) dateTime2.Month, (object) dateTime2.Day, (object) schedule.StartHours, (object) schedule.StartMinutes));
        }
        Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ScheduleDays scheduleDays1 = ReleaseEnvironmentConverter.ConvertDayOfWeek(dateTime1.DayOfWeek);
        if ((schedule.DaysToRelease & scheduleDays1) != Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ScheduleDays.None && dateTime1 > dateTime2)
          return TimeZoneInfo.ConvertTimeToUtc(dateTime1, TimeZoneInfo.FindSystemTimeZoneById(schedule.TimeZoneId));
        Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ScheduleDays scheduleDays2;
        do
        {
          scheduleDays2 = ReleaseEnvironmentConverter.ConvertNextDayOfWeek(dateTime1.DayOfWeek);
          dateTime1 = dateTime1.AddDays(1.0);
        }
        while ((schedule.DaysToRelease & scheduleDays2) == Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ScheduleDays.None);
        return TimeZoneInfo.ConvertTimeToUtc(dateTime1, TimeZoneInfo.FindSystemTimeZoneById(schedule.TimeZoneId));
      }
      catch (ArgumentException ex)
      {
        requestContext.Trace(1971006, TraceLevel.Warning, "ReleaseManagementService", "Service", "Got error in converting computed next local schedule time : {0} to UTC time for timezone : {1}, exception: {2}", (object) dateTime1, (object) schedule.TimeZoneId, (object) ex);
        return DateTime.MinValue;
      }
    }

    private static bool IsNextEnvironmentExecutionStarted(
      Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Release release,
      Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ReleaseEnvironment environment)
    {
      Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ReleaseEnvironment releaseEnvironment = release.Environments.SingleOrDefault<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ReleaseEnvironment>((Func<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ReleaseEnvironment, bool>) (env => env.Rank == environment.Rank + 1));
      return releaseEnvironment != null && releaseEnvironment.GetStepsForTests.Any<ReleaseEnvironmentStep>();
    }

    private static bool IsTargetEnvironmentReached(Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Release release, Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ReleaseEnvironment environment)
    {
      List<ReleaseEnvironmentStep> list = environment.GetStepsForTests.Where<ReleaseEnvironmentStep>((Func<ReleaseEnvironmentStep, bool>) (step => step.StepType == EnvironmentStepType.PostDeploy)).ToList<ReleaseEnvironmentStep>();
      if (!list.Any<ReleaseEnvironmentStep>() || !list.All<ReleaseEnvironmentStep>((Func<ReleaseEnvironmentStep, bool>) (step => step.IsAutomated || step.Status == ReleaseEnvironmentStepStatus.Done || step.Status == ReleaseEnvironmentStepStatus.Reassigned)))
        return false;
      Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ReleaseEnvironment releaseEnvironment = release.Environments.SingleOrDefault<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ReleaseEnvironment>((Func<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ReleaseEnvironment, bool>) (env => env.DefinitionEnvironmentId == release.TargetEnvironmentId));
      return releaseEnvironment == null || releaseEnvironment.Id == environment.Id;
    }

    private static void SetEnvironmentDateTimeFields(
      Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ReleaseEnvironment serverEnvironment,
      Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ReleaseEnvironment webApiEnvironment)
    {
      if (!serverEnvironment.GetStepsForTests.Any<ReleaseEnvironmentStep>())
      {
        webApiEnvironment.CreatedOn = new DateTime?();
        webApiEnvironment.ModifiedOn = new DateTime?();
      }
      else
      {
        webApiEnvironment.CreatedOn = new DateTime?(serverEnvironment.DeploymentAttempts.Any<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Deployment>() ? serverEnvironment.DeploymentAttempts.OrderBy<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Deployment, int>((Func<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Deployment, int>) (d => d.Attempt)).First<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Deployment>().QueuedOn : serverEnvironment.GetStepsForTests.OrderBy<ReleaseEnvironmentStep, DateTime>((Func<ReleaseEnvironmentStep, DateTime>) (s => s.CreatedOn)).First<ReleaseEnvironmentStep>().CreatedOn);
        webApiEnvironment.ModifiedOn = new DateTime?(serverEnvironment.GetStepsForTests.Select<ReleaseEnvironmentStep, DateTime>((Func<ReleaseEnvironmentStep, DateTime>) (step => step.ModifiedOn)).Max<DateTime>());
      }
    }
  }
}
