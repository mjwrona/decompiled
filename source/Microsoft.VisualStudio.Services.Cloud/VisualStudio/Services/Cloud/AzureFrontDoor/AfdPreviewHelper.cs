// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Cloud.AzureFrontDoor.AfdPreviewHelper
// Assembly: Microsoft.VisualStudio.Services.Cloud, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F2D48A0E-C4C9-4233-BA34-E8461823F6F2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cloud.dll

using Microsoft.Rest;
using Microsoft.TeamFoundation.Common;
using Microsoft.VisualStudio.Services.AfdClient;
using Microsoft.VisualStudio.Services.AfdClient.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.Cloud.AzureFrontDoor
{
  public static class AfdPreviewHelper
  {
    public static bool GetPreviewEnabled(
      Microsoft.VisualStudio.Services.AfdClient.AfdClient client,
      string tenant,
      string partner,
      ITFLogger logger)
    {
      return AfdRetryHelper.ExecuteWithRetries<OperationsSettings>((Func<OperationsSettings>) (() => OperationsSettingsOperationsExtensions.Read(client.OperationsSettings, tenant, partner)), logger).PreviewEnabled;
    }

    public static List<UrlReferenceOperationReferenceInfo> GetOperations(
      Microsoft.VisualStudio.Services.AfdClient.AfdClient client,
      List<Guid> changes,
      string tenant,
      string partner,
      ITFLogger logger)
    {
      List<UrlReferenceOperationReferenceInfo> operations = new List<UrlReferenceOperationReferenceInfo>(changes.Count);
      foreach (Guid change1 in changes)
      {
        Guid change = change1;
        operations.AddRange((IEnumerable<UrlReferenceOperationReferenceInfo>) (AfdRetryHelper.ExecuteWithRetries<DeploymentStatusForChange>((Func<DeploymentStatusForChange>) (() => DeploymentQueryExtensions.GetDeploymentStatusForChange(client.DeploymentQuery, change, (string) null)), logger) ?? throw new InvalidOperationException(string.Format("Deployment status could not be found for change ID {0}.", (object) change))).Operations);
      }
      return operations;
    }

    public static IList<UrlReferenceOperationReferenceInfo> GetLatestOperations(
      Microsoft.VisualStudio.Services.AfdClient.AfdClient client,
      string tenant,
      string partner,
      ITFLogger logger)
    {
      LatestDeployment latestDeployment;
      try
      {
        latestDeployment = AfdRetryHelper.ExecuteWithRetries<LatestDeployment>((Func<LatestDeployment>) (() => LatestDeploymentsExtensions.Read(client.LatestDeployments, "Routes", "Preview", tenant, partner)), logger);
      }
      catch (AfdErrorResponseException ex) when (ex.ResponseCode == HttpStatusCode.NotFound)
      {
        return (IList<UrlReferenceOperationReferenceInfo>) Array.Empty<UrlReferenceOperationReferenceInfo>();
      }
      ExtendedInfo info = latestDeployment.Deployment.Info;
      logger.Info(string.Format("The latest preview deployment is {0}.", (object) info.ObjectId));
      return AfdRetryHelper.ExecuteWithRetries<IList<UrlReferenceOperationReferenceInfo>>((Func<IList<UrlReferenceOperationReferenceInfo>>) (() => InitiatedDeploymentsExtensions.Read3(client.InitiatedDeployments, info.ObjectId, info.ResourceGroup, info.TenantId, info.PartnerId, (string) null).PreviewOperations), logger);
    }

    public static IList<UrlReferenceOperationReferenceInfo> GetPendingOperations(
      Microsoft.VisualStudio.Services.AfdClient.AfdClient client,
      string tenant,
      string partner,
      ITFLogger logger)
    {
      IList<UrlReferenceOperationReferenceInfo> pendingOperations = (IList<UrlReferenceOperationReferenceInfo>) new List<UrlReferenceOperationReferenceInfo>();
      foreach (UrlReferenceOperationReferenceInfo latestOperation in (IEnumerable<UrlReferenceOperationReferenceInfo>) AfdPreviewHelper.GetLatestOperations(client, tenant, partner, logger))
      {
        UrlReferenceOperationReferenceInfo operation = latestOperation;
        if (!AfdRetryHelper.ExecuteWithRetries<OperationInfo>((Func<OperationInfo>) (() => OperationsExtensions.ReadPartnerOperation(client.Operations, operation.Info.ResourceGroup, operation.Info.ObjectId, operation.Info.TenantId, operation.Info.PartnerId, new bool?())), logger).SignOffTime.HasValue)
          pendingOperations.Add(operation);
      }
      return pendingOperations;
    }

    public static void ApprovePendingDeployments(
      Microsoft.VisualStudio.Services.AfdClient.AfdClient client,
      string tenant,
      string partner,
      ITFLogger logger,
      bool validateOnly = false)
    {
      IList<UrlReferenceOperationReferenceInfo> pendingOperations = AfdPreviewHelper.GetPendingOperations(client, tenant, partner, logger);
      AfdPreviewHelper.SignOffForProductionDeployment(client, pendingOperations, logger, validateOnly);
    }

    public static void WaitForDeployment(
      Microsoft.VisualStudio.Services.AfdClient.AfdClient client,
      List<UrlReferenceOperationReferenceInfo> operations,
      AzureFrontDoorDeploymentType deploymentType,
      ITFLogger logger,
      CancellationToken cancel)
    {
      Stopwatch stopwatch = new Stopwatch();
      stopwatch.Restart();
      TimeSpan timeSpan = deploymentType == AzureFrontDoorDeploymentType.Production ? TimeSpan.FromHours(4.0) : TimeSpan.FromMinutes(30.0);
      foreach (UrlReferenceOperationReferenceInfo operation1 in operations)
      {
        UrlReferenceOperationReferenceInfo operation = operation1;
        OperationInfo operationInfo;
        while (true)
        {
          cancel.ThrowIfCancellationRequested();
          if (!(stopwatch.Elapsed > timeSpan))
          {
            operationInfo = AfdRetryHelper.ExecuteWithRetries<OperationInfo>((Func<OperationInfo>) (() => OperationsExtensions.ReadPartnerOperation(client.Operations, operation.Info.ResourceGroup, operation.Info.ObjectId, operation.Info.TenantId, operation.Info.PartnerId, new bool?())), logger);
            if (deploymentType == AzureFrontDoorDeploymentType.Preview)
            {
              if ("NotStarted".Equals(operationInfo.ProdStatus.Status, StringComparison.OrdinalIgnoreCase))
              {
                if ("RolledOut".Equals(operationInfo.PreviewStatus.Status, StringComparison.OrdinalIgnoreCase))
                  goto label_9;
              }
              else
                goto label_7;
            }
            else if ("RolledOut".Equals(operationInfo.ProdStatus.Status, StringComparison.OrdinalIgnoreCase))
              goto label_11;
            Task.Delay(1000).Wait(cancel);
          }
          else
            break;
        }
        throw new TimeoutException(string.Format("AFD deployment timed out, with {0} operations remaining.", (object) operations.Count));
label_7:
        throw new InvalidOperationException("Prod rollout started before preview was complete.");
label_9:
        logger.Info("Operation " + operationInfo.Action + " has been deployed to preview environment.");
        continue;
label_11:
        logger.Info("Operation " + operationInfo.Action + " has been deployed to production environment.");
      }
    }

    public static void SignOffForProductionDeployment(
      Microsoft.VisualStudio.Services.AfdClient.AfdClient client,
      IList<UrlReferenceOperationReferenceInfo> operations,
      ITFLogger logger,
      bool validateOnly = false)
    {
      foreach (UrlReferenceOperationReferenceInfo operationReferenceInfo in operations.Reverse<UrlReferenceOperationReferenceInfo>())
      {
        UrlReferenceOperationReferenceInfo operation = operationReferenceInfo;
        logger.Info(string.Format("Confirming operation {0} for production deployment.", (object) operation.Info.ObjectId));
        try
        {
          OperationInfo operationData = AfdRetryHelper.ExecuteWithRetries<OperationInfo>((Func<OperationInfo>) (() => OperationsExtensions.ReadPartnerOperation(client.Operations, operation.Info.ResourceGroup, operation.Info.ObjectId, operation.Info.TenantId, operation.Info.PartnerId, new bool?())), logger);
          if (operationData.SignOffTime.HasValue)
          {
            logger.Info(string.Format("Operation {0} was already signed off.", (object) operation.Info.ObjectId));
            continue;
          }
          if (validateOnly)
            throw new Exception(string.Format("AFD Configuration Invalid: Operation {0} is not signed off.", (object) operation.Info.ObjectId));
          AfdRetryHelper.ExecuteWithRetries<bool>((Func<bool>) (() =>
          {
            InitiatedDeploymentsExtensions.SignOff2(client.InitiatedDeployments, operationData.PreviewDeployment.Info.ResourceGroup, operationData.PreviewDeployment.Info.ObjectId, operationData.PreviewDeployment.Info.TenantId, operationData.PreviewDeployment.Info.PartnerId);
            return true;
          }), logger);
        }
        catch (HttpOperationException ex)
        {
          if (ex.Response != null)
          {
            if (ex.Response.StatusCode == HttpStatusCode.NoContent)
              goto label_14;
          }
          throw;
        }
        catch (AfdErrorResponseException ex)
        {
          if (ex.ResponseCode != HttpStatusCode.BadRequest)
            throw;
          else
            logger.Warning("Partner sign off returned HTTP Bad Request. Message: " + ((Exception) ex).Message);
        }
label_14:
        if (!AfdRetryHelper.ExecuteWithRetries<OperationInfo>((Func<OperationInfo>) (() => OperationsExtensions.ReadPartnerOperation(client.Operations, operation.Info.ResourceGroup, operation.Info.ObjectId, operation.Info.TenantId, operation.Info.PartnerId, new bool?())), logger).SignOffTime.HasValue)
          throw new InvalidOperationException(string.Format("Sign off failed for operation {0}.", (object) operation.Info.ObjectId));
      }
    }

    private enum DeploymentStatus
    {
      NotStarted,
      InRollout,
      RolledOut,
    }

    private enum ResourceGroup
    {
      Routes,
    }
  }
}
