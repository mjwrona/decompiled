// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Operations.JobOperationsUtility
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Location.Server;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Operations
{
  public static class JobOperationsUtility
  {
    internal static Dictionary<TeamFoundationJobResult, OperationStatus> s_jobResultToStatus = new Dictionary<TeamFoundationJobResult, OperationStatus>()
    {
      {
        TeamFoundationJobResult.Succeeded,
        OperationStatus.Succeeded
      },
      {
        TeamFoundationJobResult.PartiallySucceeded,
        OperationStatus.Succeeded
      },
      {
        TeamFoundationJobResult.Stopped,
        OperationStatus.Cancelled
      },
      {
        TeamFoundationJobResult.Killed,
        OperationStatus.Failed
      },
      {
        TeamFoundationJobResult.Disabled,
        OperationStatus.Failed
      },
      {
        TeamFoundationJobResult.Blocked,
        OperationStatus.Failed
      },
      {
        TeamFoundationJobResult.Inactive,
        OperationStatus.Failed
      },
      {
        TeamFoundationJobResult.Failed,
        OperationStatus.Failed
      },
      {
        TeamFoundationJobResult.ExtensionNotFound,
        OperationStatus.Failed
      },
      {
        TeamFoundationJobResult.JobInitializationError,
        OperationStatus.Failed
      },
      {
        TeamFoundationJobResult.BlockedByUpgrade,
        OperationStatus.Failed
      },
      {
        TeamFoundationJobResult.HostShutdown,
        OperationStatus.Failed
      },
      {
        TeamFoundationJobResult.HostNotFound,
        OperationStatus.Failed
      },
      {
        TeamFoundationJobResult.JobDefinitionNotFound,
        OperationStatus.Failed
      },
      {
        TeamFoundationJobResult.None,
        OperationStatus.NotSet
      },
      {
        TeamFoundationJobResult.Last,
        OperationStatus.NotSet
      }
    };
    internal static Dictionary<TeamFoundationJobState, OperationStatus> s_jobStateToStatus = new Dictionary<TeamFoundationJobState, OperationStatus>()
    {
      {
        TeamFoundationJobState.QueuedScheduled,
        OperationStatus.Queued
      },
      {
        TeamFoundationJobState.Dormant,
        OperationStatus.Queued
      },
      {
        TeamFoundationJobState.Paused,
        OperationStatus.InProgress
      },
      {
        TeamFoundationJobState.Pausing,
        OperationStatus.InProgress
      },
      {
        TeamFoundationJobState.Resuming,
        OperationStatus.InProgress
      },
      {
        TeamFoundationJobState.Running,
        OperationStatus.InProgress
      },
      {
        TeamFoundationJobState.Stopping,
        OperationStatus.Cancelled
      }
    };

    public static Uri GetOperationUrl(IVssRequestContext requestContext, Guid jobId) => requestContext.GetService<ILocationService>().GetResourceUri(requestContext, "operations", OperationsResourceIds.OperationsLocationId, (object) new
    {
      operationId = jobId
    });

    public static OperationReference GetOperationReference(
      IVssRequestContext requestContext,
      Guid jobId)
    {
      return new OperationReference()
      {
        Id = jobId,
        Url = JobOperationsUtility.GetOperationUrl(requestContext, jobId).ToString()
      };
    }

    public static Operation GetOperation(IVssRequestContext requestContext, Guid jobId)
    {
      if (requestContext.ServiceHost.Is(TeamFoundationHostType.Deployment))
        requestContext.GetService<ITeamFoundationSecurityService>().GetSecurityNamespace(requestContext, FrameworkSecurity.FrameworkNamespaceId).CheckPermission(requestContext, FrameworkSecurity.FrameworkNamespaceToken, 2);
      OperationStatus status;
      string resultMessage;
      if (JobOperationsUtility.TryQueryOperation(requestContext, jobId, out status, out resultMessage))
      {
        Operation operation = new Operation(JobOperationsUtility.GetOperationReference(requestContext, jobId));
        operation.Status = status;
        operation.ResultMessage = resultMessage;
        return operation;
      }
      if (requestContext.ExecutionEnvironment.IsHostedDeployment && requestContext.ServiceHost.IsOnly(TeamFoundationHostType.Application))
      {
        IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Deployment);
        TeamFoundationHostManagementService service = vssRequestContext.GetService<TeamFoundationHostManagementService>();
        foreach (TeamFoundationServiceHostProperties child in service.QueryServiceHostProperties(vssRequestContext, requestContext.ServiceHost.InstanceId, ServiceHostFilterFlags.IncludeChildren).Children)
        {
          try
          {
            using (IVssRequestContext requestContext1 = service.BeginRequest(vssRequestContext, child.Id, RequestContextType.SystemContext, true, true))
            {
              if (JobOperationsUtility.TryQueryOperation(requestContext1, jobId, out status, out resultMessage))
              {
                Operation operation = new Operation(JobOperationsUtility.GetOperationReference(requestContext1, jobId));
                operation.Status = status;
                operation.ResultMessage = resultMessage;
                return operation;
              }
            }
          }
          catch (HostShutdownException ex)
          {
          }
        }
      }
      throw new OperationNotFoundException(jobId);
    }

    internal static OperationStatus MapStatus(TeamFoundationJobResult jobResult)
    {
      OperationStatus operationStatus = OperationStatus.NotSet;
      JobOperationsUtility.s_jobResultToStatus.TryGetValue(jobResult, out operationStatus);
      return operationStatus;
    }

    internal static OperationStatus MapStatus(TeamFoundationJobState jobState)
    {
      OperationStatus operationStatus = OperationStatus.NotSet;
      JobOperationsUtility.s_jobStateToStatus.TryGetValue(jobState, out operationStatus);
      return operationStatus;
    }

    private static bool TryQueryOperation(
      IVssRequestContext requestContext,
      Guid jobId,
      out OperationStatus status,
      out string resultMessage)
    {
      status = OperationStatus.NotSet;
      resultMessage = (string) null;
      ITeamFoundationJobService service = requestContext.GetService<ITeamFoundationJobService>();
      Guid[] jobIds = new Guid[1]{ jobId };
      TeamFoundationJobDefinition foundationJobDefinition = service.QueryJobDefinition(requestContext, jobId);
      TeamFoundationJobQueueEntry foundationJobQueueEntry = service.QueryJobQueue(requestContext, (IEnumerable<Guid>) jobIds).FirstOrDefault<TeamFoundationJobQueueEntry>();
      if (foundationJobQueueEntry != null)
      {
        status = JobOperationsUtility.MapStatus(foundationJobQueueEntry.State);
      }
      else
      {
        TeamFoundationJobHistoryEntry foundationJobHistoryEntry = service.QueryLatestJobHistory(requestContext, (IEnumerable<Guid>) jobIds).FirstOrDefault<TeamFoundationJobHistoryEntry>();
        if (foundationJobHistoryEntry != null)
        {
          if (!requestContext.ServiceHost.IsProduction)
            resultMessage = foundationJobHistoryEntry.ResultMessage;
          status = JobOperationsUtility.MapStatus(foundationJobHistoryEntry.Result);
        }
        else
        {
          if (foundationJobDefinition == null)
            return false;
          status = OperationStatus.Queued;
        }
      }
      return true;
    }
  }
}
