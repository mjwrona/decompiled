// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Server.Processors.ReleaseEnvironmentStatusResolver
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 134B1041-BFA6-49C6-8C6D-CA5ADF31AF54
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Server.dll

using Microsoft.TeamFoundation.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Exceptions;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Enumerations;
using Microsoft.VisualStudio.Services.ReleaseManagement.Server.Properties;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Server.Processors
{
  public static class ReleaseEnvironmentStatusResolver
  {
    private static readonly Dictionary<ReleaseEnvironmentStatus, List<ReleaseEnvironmentStatus>> ReleaseEnvironmentStateTransitionMap = new Dictionary<ReleaseEnvironmentStatus, List<ReleaseEnvironmentStatus>>()
    {
      {
        ReleaseEnvironmentStatus.InProgress,
        new List<ReleaseEnvironmentStatus>()
        {
          ReleaseEnvironmentStatus.Canceled
        }
      },
      {
        ReleaseEnvironmentStatus.Canceled,
        new List<ReleaseEnvironmentStatus>()
        {
          ReleaseEnvironmentStatus.InProgress
        }
      },
      {
        ReleaseEnvironmentStatus.NotStarted,
        new List<ReleaseEnvironmentStatus>()
        {
          ReleaseEnvironmentStatus.InProgress
        }
      },
      {
        ReleaseEnvironmentStatus.Queued,
        new List<ReleaseEnvironmentStatus>()
        {
          ReleaseEnvironmentStatus.Canceled
        }
      },
      {
        ReleaseEnvironmentStatus.Rejected,
        new List<ReleaseEnvironmentStatus>()
        {
          ReleaseEnvironmentStatus.InProgress
        }
      },
      {
        ReleaseEnvironmentStatus.Scheduled,
        new List<ReleaseEnvironmentStatus>()
        {
          ReleaseEnvironmentStatus.InProgress,
          ReleaseEnvironmentStatus.Canceled
        }
      },
      {
        ReleaseEnvironmentStatus.PartiallySucceeded,
        new List<ReleaseEnvironmentStatus>()
        {
          ReleaseEnvironmentStatus.InProgress
        }
      },
      {
        ReleaseEnvironmentStatus.Succeeded,
        new List<ReleaseEnvironmentStatus>()
        {
          ReleaseEnvironmentStatus.InProgress
        }
      }
    };

    [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "This is to isolate state knowledge to this module")]
    private static Dictionary<Tuple<ReleaseEnvironmentStatus, ReleaseEnvironmentStatus>, Func<Release, int, ReleaseEnvironmentUpdateMetadata, bool, Release>> GetReleaseEnvironmentOperationsMap(
      IVssRequestContext requestContext,
      Guid projectId)
    {
      OrchestratorServiceProcessorV2 orchestratorServiceProcessor = new OrchestratorServiceProcessorV2(requestContext, projectId);
      return new Dictionary<Tuple<ReleaseEnvironmentStatus, ReleaseEnvironmentStatus>, Func<Release, int, ReleaseEnvironmentUpdateMetadata, bool, Release>>()
      {
        {
          Tuple.Create<ReleaseEnvironmentStatus, ReleaseEnvironmentStatus>(ReleaseEnvironmentStatus.InProgress, ReleaseEnvironmentStatus.Canceled),
          (Func<Release, int, ReleaseEnvironmentUpdateMetadata, bool, Release>) ((release, envId, environmentUpdateData, forceUpdate) => orchestratorServiceProcessor.CancelDeploymentOnEnvironment(release, envId, environmentUpdateData.Comment, environmentUpdateData.AddCommentAsDeploymentIssue, forceUpdate))
        },
        {
          Tuple.Create<ReleaseEnvironmentStatus, ReleaseEnvironmentStatus>(ReleaseEnvironmentStatus.Canceled, ReleaseEnvironmentStatus.InProgress),
          (Func<Release, int, ReleaseEnvironmentUpdateMetadata, bool, Release>) ((release, envId, environmentUpdateData, forceUpdate) => orchestratorServiceProcessor.RetryDeploymentOnEnvironment(release, envId, environmentUpdateData))
        },
        {
          Tuple.Create<ReleaseEnvironmentStatus, ReleaseEnvironmentStatus>(ReleaseEnvironmentStatus.Rejected, ReleaseEnvironmentStatus.InProgress),
          (Func<Release, int, ReleaseEnvironmentUpdateMetadata, bool, Release>) ((release, envId, environmentUpdateData, forceUpdate) => orchestratorServiceProcessor.RetryDeploymentOnEnvironment(release, envId, environmentUpdateData))
        },
        {
          Tuple.Create<ReleaseEnvironmentStatus, ReleaseEnvironmentStatus>(ReleaseEnvironmentStatus.NotStarted, ReleaseEnvironmentStatus.InProgress),
          (Func<Release, int, ReleaseEnvironmentUpdateMetadata, bool, Release>) ((release, envId, environmentUpdateData, forceUpdate) => ReleaseEnvironmentStatusResolver.HandleQueueReleaseEnvironmentJob(requestContext, projectId, release, envId, environmentUpdateData))
        },
        {
          Tuple.Create<ReleaseEnvironmentStatus, ReleaseEnvironmentStatus>(ReleaseEnvironmentStatus.Queued, ReleaseEnvironmentStatus.Canceled),
          (Func<Release, int, ReleaseEnvironmentUpdateMetadata, bool, Release>) ((release, envId, environmentUpdateData, forceUpdate) => orchestratorServiceProcessor.CancelDeploymentOnEnvironment(release, envId, environmentUpdateData.Comment, environmentUpdateData.AddCommentAsDeploymentIssue, forceUpdate))
        },
        {
          Tuple.Create<ReleaseEnvironmentStatus, ReleaseEnvironmentStatus>(ReleaseEnvironmentStatus.Scheduled, ReleaseEnvironmentStatus.InProgress),
          (Func<Release, int, ReleaseEnvironmentUpdateMetadata, bool, Release>) ((release, envId, environmentUpdateData, forceUpdate) => ReleaseEnvironmentStatusResolver.HandleQueueReleaseEnvironmentJob(requestContext, projectId, release, envId, environmentUpdateData))
        },
        {
          Tuple.Create<ReleaseEnvironmentStatus, ReleaseEnvironmentStatus>(ReleaseEnvironmentStatus.Scheduled, ReleaseEnvironmentStatus.Canceled),
          (Func<Release, int, ReleaseEnvironmentUpdateMetadata, bool, Release>) ((release, envId, environmentUpdateData, forceUpdate) => orchestratorServiceProcessor.CancelDeploymentOnEnvironment(release, envId, environmentUpdateData.Comment, environmentUpdateData.AddCommentAsDeploymentIssue, forceUpdate))
        },
        {
          Tuple.Create<ReleaseEnvironmentStatus, ReleaseEnvironmentStatus>(ReleaseEnvironmentStatus.PartiallySucceeded, ReleaseEnvironmentStatus.InProgress),
          (Func<Release, int, ReleaseEnvironmentUpdateMetadata, bool, Release>) ((release, envId, environmentUpdateData, forceUpdate) => orchestratorServiceProcessor.RetryDeploymentOnEnvironment(release, envId, environmentUpdateData))
        },
        {
          Tuple.Create<ReleaseEnvironmentStatus, ReleaseEnvironmentStatus>(ReleaseEnvironmentStatus.Succeeded, ReleaseEnvironmentStatus.InProgress),
          (Func<Release, int, ReleaseEnvironmentUpdateMetadata, bool, Release>) ((release, envId, environmentUpdateData, forceUpdate) => orchestratorServiceProcessor.RetryDeploymentOnEnvironment(release, envId, environmentUpdateData))
        }
      };
    }

    [SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed", Justification = "These are optional parameters")]
    public static Release UpdateReleaseEnvironmentStatus(
      IVssRequestContext requestContext,
      Guid projectId,
      Release release,
      ReleaseEnvironment releaseEnvironment,
      ReleaseEnvironmentUpdateMetadata environmentUpdateData,
      bool forceUpdate = false)
    {
      if (release == null)
        throw new ArgumentNullException(nameof (release));
      if (releaseEnvironment == null)
        throw new ArgumentNullException(nameof (releaseEnvironment));
      if (environmentUpdateData == null)
        throw new ArgumentNullException(nameof (environmentUpdateData));
      if (!forceUpdate && release.Status == ReleaseStatus.Abandoned)
        throw new InvalidRequestException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.EnvironmentStatusCannotBeUpdated, (object) releaseEnvironment.Name));
      if (!ReleaseEnvironmentStatusResolver.IsEnvironmentStatusUpdateAllowed(releaseEnvironment.Status, environmentUpdateData.Status))
        throw new InvalidRequestException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.ReleaseEnvironmentStatusUpdateNotAllowed, (object) releaseEnvironment.Status, (object) environmentUpdateData.Status));
      return ReleaseEnvironmentStatusResolver.GetReleaseEnvironmentOperationsMap(requestContext, projectId)[Tuple.Create<ReleaseEnvironmentStatus, ReleaseEnvironmentStatus>(releaseEnvironment.Status, environmentUpdateData.Status)](release, releaseEnvironment.Id, environmentUpdateData, forceUpdate);
    }

    private static Release HandleQueueReleaseEnvironmentJob(
      IVssRequestContext requestContext,
      Guid projectId,
      Release release,
      int releaseEnvironmentId,
      ReleaseEnvironmentUpdateMetadata environmentUpdateData)
    {
      ReleaseEnvironment releaseEnvironment = release.Environments.Single<ReleaseEnvironment>((Func<ReleaseEnvironment, bool>) (env => env.Id == releaseEnvironmentId));
      Guid userId = requestContext.GetUserId(true);
      ReleaseEnvironmentSnapshotDelta deploymentDelta = (ReleaseEnvironmentSnapshotDelta) null;
      if (!environmentUpdateData.Variables.IsNullOrEmpty<KeyValuePair<string, ConfigurationVariableValue>>())
        deploymentDelta = new ReleaseEnvironmentSnapshotDelta(release.Id, releaseEnvironmentId, environmentUpdateData.Variables);
      new OrchestratorServiceProcessorV2(requestContext, projectId).QueueOnStartEnvironmentJob(release.Id, release.ReleaseDefinitionId, releaseEnvironment, userId, userId, DeploymentReason.Manual, environmentUpdateData.Comment, true, deploymentDelta);
      releaseEnvironment.Status = ReleaseEnvironmentStatus.Queued;
      return release;
    }

    private static bool IsEnvironmentStatusUpdateAllowed(
      ReleaseEnvironmentStatus currentStatus,
      ReleaseEnvironmentStatus desiredStatus)
    {
      return ReleaseEnvironmentStatusResolver.ReleaseEnvironmentStateTransitionMap.ContainsKey(currentStatus) && ReleaseEnvironmentStatusResolver.ReleaseEnvironmentStateTransitionMap[currentStatus].Contains(desiredStatus);
    }
  }
}
