// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Server.Utilities.DeploymentsHelper
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 134B1041-BFA6-49C6-8C6D-CA5ADF31AF54
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Enumerations;
using Microsoft.VisualStudio.Services.ReleaseManagement.Server.Services;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Text;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Server.Utilities
{
  public sealed class DeploymentsHelper
  {
    private DeploymentsHelper()
    {
    }

    public static bool CancelDeployments(
      IVssRequestContext requestContext,
      ReleasesService releasesService,
      DeploymentsService deploymentsService,
      Guid projectId,
      int releaseDefinitionId,
      int releaseId,
      string comment,
      bool addCommentAsDeploymentIssue,
      bool isDeleted)
    {
      return DeploymentsHelper.CancelTopDeployments(requestContext, releasesService, deploymentsService, projectId, releaseDefinitionId, releaseId, comment, addCommentAsDeploymentIssue, int.MaxValue, isDeleted) > 0;
    }

    public static int CancelTopDeployments(
      IVssRequestContext requestContext,
      ReleasesService releasesService,
      DeploymentsService deploymentsService,
      Guid projectId,
      int releaseDefinitionId,
      int releaseId,
      string comment,
      bool addCommentAsDeploymentIssue,
      int top,
      bool isDeleted = true)
    {
      if (releasesService == null)
        throw new ArgumentNullException(nameof (releasesService));
      if (deploymentsService == null)
        throw new ArgumentNullException(nameof (deploymentsService));
      IEnumerable<Deployment> cancellableDeployments = DeploymentsHelper.GetCancellableDeployments(requestContext, deploymentsService, projectId, releaseDefinitionId, releaseId, isDeleted, top);
      return DeploymentsHelper.CancelDeployments(requestContext, releasesService, deploymentsService, projectId, releaseDefinitionId, cancellableDeployments, comment, addCommentAsDeploymentIssue, isDeleted, (StringBuilder) null);
    }

    [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Safe handle exception")]
    public static int CancelDeployments(
      IVssRequestContext requestContext,
      ReleasesService releasesService,
      DeploymentsService deploymentsService,
      Guid projectId,
      int releaseDefinitionId,
      IEnumerable<Deployment> cancellableDeployments,
      string comment,
      bool addCommentAsDeploymentIssue,
      bool isDeleted,
      StringBuilder messageBuilder)
    {
      if (releasesService == null)
        throw new ArgumentNullException(nameof (releasesService));
      if (deploymentsService == null)
        throw new ArgumentNullException(nameof (deploymentsService));
      IEnumerable<IGrouping<int, Deployment>> groupings = cancellableDeployments.GroupBy<Deployment, int>((Func<Deployment, int>) (d => d.ReleaseId));
      int num = 0;
      foreach (IGrouping<int, Deployment> grouping in groupings)
      {
        Release release = releasesService.GetRelease(requestContext, projectId, grouping.Key, isDeleted);
        foreach (Deployment deployment1 in (IEnumerable<Deployment>) grouping)
        {
          Deployment deployment = deployment1;
          try
          {
            ReleaseEnvironment environment = release.Environments.Single<ReleaseEnvironment>((Func<ReleaseEnvironment, bool>) (e => e.Id == deployment.ReleaseEnvironmentId));
            if (environment.Status != ReleaseEnvironmentStatus.Rejected)
            {
              if (environment.Status != ReleaseEnvironmentStatus.Succeeded)
              {
                if (environment.Status != ReleaseEnvironmentStatus.Canceled)
                {
                  requestContext.Trace(1976390, TraceLevel.Info, "ReleaseManagementService", "JobLayer", "Cancelling environment - projectId: {0}, releaseDefinitionId: {1}, releaseId: {2}, environmentId: {3}, deploymentId: {4}, deploymentOperationStatus: {5}, deploymentStatus: {6}, releaseEnvironmentStatus: {7}", (object) projectId, (object) releaseDefinitionId, (object) deployment.ReleaseId, (object) deployment.ReleaseEnvironmentId, (object) deployment.Id, (object) deployment.OperationStatus, (object) deployment.Status, (object) environment.Status);
                  release = releasesService.UpdateReleaseEnvironment(requestContext, projectId, release, environment, new ReleaseEnvironmentUpdateMetadata()
                  {
                    Status = ReleaseEnvironmentStatus.Canceled,
                    Comment = comment,
                    AddCommentAsDeploymentIssue = addCommentAsDeploymentIssue
                  }, true);
                  messageBuilder?.AppendLine(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Deployment cancelled for  projectId: {0}, releaseDefinitionId: {1}, Release Id: {2}, DeploymentId: {3}", (object) projectId, (object) releaseDefinitionId, (object) release.Id, (object) deployment.Id));
                  ++num;
                }
              }
            }
          }
          catch (Exception ex)
          {
            requestContext.Trace(1976391, TraceLevel.Error, "ReleaseManagementService", "JobLayer", "CancelDeployments - for projectId: {0}, releaseDefinitionId: {1}, releaseId: {2}, environmentId: {3}, deploymentId:: {4}, error: {5}, stackTrace: {6}", (object) projectId, (object) releaseDefinitionId, (object) deployment.ReleaseId, (object) deployment.ReleaseEnvironmentId, (object) deployment.Id, (object) ex.Message, (object) ex.StackTrace);
          }
        }
      }
      return num;
    }

    public static bool CancelDeployments(
      IVssRequestContext requestContext,
      ReleasesService releasesService,
      DeploymentsService deploymentsService,
      Guid projectId,
      IEnumerable<int> releaseIds,
      string comment,
      bool addCommentAsDeploymentIssue,
      bool isDeleted)
    {
      if (releaseIds == null)
        throw new ArgumentNullException(nameof (releaseIds));
      bool flag = false;
      foreach (int releaseId in releaseIds)
        DeploymentsHelper.CancelDeployments(requestContext, releasesService, deploymentsService, projectId, 0, releaseId, comment, addCommentAsDeploymentIssue, isDeleted);
      return flag;
    }

    public static Deployment GetLastSuccessfulDeployment(
      IVssRequestContext requestContext,
      Guid projectId,
      int releaseDefinitionId,
      int definitionEnvironmentId,
      int currentDeploymentId)
    {
      List<Deployment> list = requestContext.GetService<DeploymentsService>().ListDeployments(requestContext, projectId, releaseDefinitionId, definitionEnvironmentId, 0, (string) null, DeploymentOperationStatus.Undefined, DeploymentStatus.Succeeded | DeploymentStatus.PartiallySucceeded, false, ReleaseQueryOrder.IdDescending, 2, currentDeploymentId, new DateTime?(), new DateTime?(), createdFor: (string) null).Where<Deployment>((Func<Deployment, bool>) (dep => dep.Id != currentDeploymentId)).ToList<Deployment>();
      return !list.Any<Deployment>() ? (Deployment) null : list[0];
    }

    public static IEnumerable<Deployment> GetCancellablePullRequestDeployments(
      IVssRequestContext requestContext,
      DeploymentsService deploymentsService,
      Guid projectId,
      int releaseDefinitionId,
      string artifactTypeId,
      string sourceId,
      string branchName,
      int top)
    {
      List<Deployment> requestDeployments = new List<Deployment>();
      List<Deployment> deploymentList = new List<Deployment>();
      do
      {
        int deploymentContinuationToken = deploymentList.Count<Deployment>() > 0 ? deploymentList.Max<Deployment>((Func<Deployment, int>) (d => d.Id)) + 1 : 0;
        deploymentList = DeploymentsHelper.GetCancellableDeploymentsBatch(requestContext, deploymentsService, projectId, releaseDefinitionId, 0, deploymentContinuationToken, false, Math.Min(1000, top), artifactTypeId, sourceId, branchName, ReleaseReason.PullRequest).ToList<Deployment>();
        if (deploymentList.Count != 0)
        {
          requestDeployments.AddRange((IEnumerable<Deployment>) deploymentList);
          top -= deploymentList.Count<Deployment>();
        }
        else
          break;
      }
      while (top > 0);
      return (IEnumerable<Deployment>) requestDeployments;
    }

    private static IEnumerable<Deployment> GetCancellableDeployments(
      IVssRequestContext requestContext,
      DeploymentsService deploymentsService,
      Guid projectId,
      int releaseDefinitionId,
      int releaseId,
      bool isDeleted,
      int top)
    {
      List<Deployment> cancellableDeployments = new List<Deployment>();
      List<Deployment> deploymentList = new List<Deployment>();
      do
      {
        int deploymentContinuationToken = deploymentList.Count<Deployment>() > 0 ? deploymentList.Max<Deployment>((Func<Deployment, int>) (d => d.Id)) + 1 : 0;
        deploymentList = DeploymentsHelper.GetCancellableDeploymentsBatch(requestContext, deploymentsService, projectId, releaseDefinitionId, releaseId, deploymentContinuationToken, isDeleted, Math.Min(1000, top)).ToList<Deployment>();
        if (deploymentList.Count != 0)
        {
          cancellableDeployments.AddRange((IEnumerable<Deployment>) deploymentList);
          top -= deploymentList.Count<Deployment>();
        }
        else
          break;
      }
      while (top > 0);
      return (IEnumerable<Deployment>) cancellableDeployments;
    }

    private static IList<Deployment> GetCancellableDeploymentsBatch(
      IVssRequestContext requestContext,
      DeploymentsService deploymentsService,
      Guid projectId,
      int releaseDefinitionId,
      int releaseId,
      int deploymentContinuationToken,
      bool isDeleted,
      int batchSize,
      string artifactTypeId = "",
      string sourceId = "",
      string banchName = "",
      ReleaseReason releaseReason = ReleaseReason.None)
    {
      DeploymentsService deploymentsService1 = deploymentsService;
      IVssRequestContext context = requestContext;
      Guid projectId1 = projectId;
      int definitionId = releaseDefinitionId;
      int releaseId1 = releaseId;
      int deploymentOperationStatus = (int) DeploymentsHelper.GetCancellableDeploymentOperationStatus();
      int deploymentStatus = (int) DeploymentsHelper.GetCancellableDeploymentStatus();
      int top = batchSize;
      int deploymentContinuationToken1 = deploymentContinuationToken;
      DateTime? minModifiedTime = new DateTime?();
      DateTime? maxModifiedTime = new DateTime?();
      int num1 = isDeleted ? 1 : 0;
      string str1 = artifactTypeId;
      string str2 = sourceId;
      string branchName = banchName;
      ReleaseReason releaseReason1 = releaseReason;
      DateTime? minStartedTime = new DateTime?();
      DateTime? maxStartedTime = new DateTime?();
      string artifactTypeId1 = str1;
      string sourceId1 = str2;
      int num2 = (int) releaseReason1;
      IEnumerable<Deployment> source = deploymentsService1.ListDeployments(context, projectId1, definitionId, 0, releaseId1, (string) null, (DeploymentOperationStatus) deploymentOperationStatus, (DeploymentStatus) deploymentStatus, false, ReleaseQueryOrder.IdAscending, top, deploymentContinuationToken1, minModifiedTime, maxModifiedTime, num1 != 0, (string) null, branchName, minStartedTime, maxStartedTime, artifactTypeId1, sourceId1, (ReleaseReason) num2);
      return source is IList<Deployment> deploymentList ? deploymentList : (IList<Deployment>) source.ToList<Deployment>();
    }

    private static DeploymentStatus GetCancellableDeploymentStatus() => DeploymentStatus.NotDeployed | DeploymentStatus.InProgress;

    private static DeploymentOperationStatus GetCancellableDeploymentOperationStatus() => DeploymentOperationStatus.Queued | DeploymentOperationStatus.Scheduled | DeploymentOperationStatus.Pending | DeploymentOperationStatus.Approved | DeploymentOperationStatus.Deferred | DeploymentOperationStatus.QueuedForAgent | DeploymentOperationStatus.PhaseInProgress | DeploymentOperationStatus.ManualInterventionPending | DeploymentOperationStatus.QueuedForPipeline;
  }
}
