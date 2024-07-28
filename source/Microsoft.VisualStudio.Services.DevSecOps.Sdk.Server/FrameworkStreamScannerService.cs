// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.DevSecOps.Sdk.Server.FrameworkStreamScannerService
// Assembly: Microsoft.VisualStudio.Services.DevSecOps.Sdk.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: AEA81E2B-AB47-44C0-8043-66C0E1018997
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.DevSecOps.Sdk.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.CircuitBreaker;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.DevSecOps.WebApi;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.DevSecOps.Sdk.Server
{
  public class FrameworkStreamScannerService : IStreamScannerService, IVssFrameworkService
  {
    private const string c_layer = "FrameworkStreamScannerService";

    public ScanResult ScanPipelineDefinitionStream(
      IVssRequestContext requestContext,
      Stream inputStream,
      Guid projectId,
      PipelineType pipelineType,
      string pipelineId,
      int? revision = null,
      string resourceName = "DefaultResource.json",
      CancellationToken cancellationToken = default (CancellationToken))
    {
      if (requestContext.ExecutionEnvironment.IsOnPremisesDeployment)
        return new ScanResult()
        {
          Violations = (IList<Violation>) Array.Empty<Violation>()
        };
      using (new MethodScope(requestContext, nameof (FrameworkStreamScannerService), nameof (ScanPipelineDefinitionStream)))
      {
        ArgumentUtility.CheckForEmptyGuid(projectId, nameof (projectId));
        ArgumentUtility.CheckForNull<string>(pipelineId, nameof (pipelineId));
        string empty = string.Empty;
        if (revision.HasValue)
        {
          ArgumentUtility.CheckGreaterThanOrEqualToZero((float) revision.Value, nameof (revision));
          empty = revision.ToString();
        }
        try
        {
          string instanceId1 = string.IsNullOrEmpty(empty) ? pipelineId : pipelineId + "/" + empty;
          string instanceId2 = FrameworkStreamScannerService.CreateInstanceId(pipelineType, requestContext.ServiceHost.InstanceId, projectId, instanceId1);
          return this.GetScanResult(requestContext.To(TeamFoundationHostType.Deployment), inputStream, resourceName, instanceId2, cancellationToken);
        }
        catch (SecretsScanTimeoutException ex)
        {
          requestContext.TraceException(27009016, nameof (FrameworkStreamScannerService), (Exception) ex);
        }
        catch (CircuitBreakerException ex)
        {
          requestContext.TraceException(27009026, nameof (FrameworkStreamScannerService), (Exception) ex);
        }
        catch (Exception ex) when (cancellationToken.IsCancellationRequested)
        {
          requestContext.TraceException(27009030, nameof (FrameworkStreamScannerService), ex);
        }
        catch (Exception ex)
        {
          requestContext.TraceException(27009025, nameof (FrameworkStreamScannerService), ex);
        }
      }
      return new ScanResult()
      {
        Violations = (IList<Violation>) Array.Empty<Violation>()
      };
    }

    public ScanResult ScanContentStream(
      IVssRequestContext requestContext,
      Stream inputStream,
      Guid projectId,
      string repositoryId,
      string commitId,
      string resourceName,
      bool isInternalExperience = true,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      if (requestContext.ExecutionEnvironment.IsOnPremisesDeployment)
        return new ScanResult()
        {
          Violations = (IList<Violation>) Array.Empty<Violation>()
        };
      using (new MethodScope(requestContext, nameof (FrameworkStreamScannerService), nameof (ScanContentStream)))
      {
        ArgumentUtility.CheckForEmptyGuid(projectId, nameof (projectId));
        ArgumentUtility.CheckForNull<string>(repositoryId, nameof (repositoryId));
        ArgumentUtility.CheckStringForNullOrEmpty(resourceName, nameof (resourceName));
        ArgumentUtility.CheckStringForNullOrEmpty(commitId, nameof (commitId));
        string instanceId = FrameworkStreamScannerService.CreateInstanceId(PipelineType.Git, requestContext.ServiceHost.InstanceId, projectId, repositoryId + resourceName + "/" + commitId, isInternalExperience);
        return this.GetScanResult(requestContext.To(TeamFoundationHostType.Deployment), inputStream, resourceName, instanceId, cancellationToken);
      }
    }

    public ScanResult ScanStream(
      IVssRequestContext requestContext,
      Stream inputStream,
      string instanceId,
      string resourceName,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      if (requestContext.ExecutionEnvironment.IsOnPremisesDeployment)
        return new ScanResult()
        {
          Violations = (IList<Violation>) Array.Empty<Violation>()
        };
      using (new MethodScope(requestContext, nameof (FrameworkStreamScannerService), nameof (ScanStream)))
        return this.GetScanResult(requestContext.To(TeamFoundationHostType.Deployment), inputStream, resourceName, instanceId, cancellationToken);
    }

    public async Task<BatchScanResult> ScanContainer(
      IVssRequestContext requestContext,
      Container container,
      BatchScanOptions batchScanOptions = null,
      CompressionLevel batchScanTargetCompressionLevel = CompressionLevel.NoCompression,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      BatchScanResult batchScanResult;
      using (new MethodScope(requestContext, nameof (FrameworkStreamScannerService), nameof (ScanContainer)))
        batchScanResult = await this.GetBatchScanResult(requestContext, container, batchScanOptions, batchScanTargetCompressionLevel, cancellationToken);
      return batchScanResult;
    }

    public async Task<ScanConfiguration> GetScanConfigurationAsync(
      IVssRequestContext requestContext,
      CancellationToken cancellationToken)
    {
      using (new MethodScope(requestContext, nameof (FrameworkStreamScannerService), nameof (GetScanConfigurationAsync)))
      {
        DevSecOpsHttpClient client = requestContext.To(TeamFoundationHostType.Deployment).GetClient<DevSecOpsHttpClient>();
        if (cancellationToken == new CancellationToken())
          return await client.GetCurrentConfigurationAsync((object) cancellationToken);
        using (CancellationTokenSource cts = CancellationTokenSource.CreateLinkedTokenSource(requestContext.CancellationToken, cancellationToken))
          return await client.GetCurrentConfigurationAsync((object) cts);
      }
    }

    public ScanConfiguration GetScanConfiguration(
      IVssRequestContext requestContext,
      CancellationToken cancellationToken)
    {
      using (new MethodScope(requestContext, nameof (FrameworkStreamScannerService), nameof (GetScanConfiguration)))
      {
        DevSecOpsHttpClient client = requestContext.To(TeamFoundationHostType.Deployment).Elevate().GetClient<DevSecOpsHttpClient>();
        if (cancellationToken == new CancellationToken())
          return client.GetCurrentConfigurationAsync((object) cancellationToken).SyncResult<ScanConfiguration>();
        using (CancellationTokenSource linkedTokenSource = CancellationTokenSource.CreateLinkedTokenSource(requestContext.CancellationToken, cancellationToken))
          return client.GetCurrentConfigurationAsync((object) linkedTokenSource).SyncResult<ScanConfiguration>();
      }
    }

    protected virtual ScanResult GetScanResult(
      IVssRequestContext deploymentContext,
      Stream inputStream,
      string resourceName,
      string instanceId,
      CancellationToken cancellationToken)
    {
      ArgumentUtility.CheckForNull<Stream>(inputStream, nameof (inputStream));
      DevSecOpsHttpClient client = deploymentContext.Elevate().GetClient<DevSecOpsHttpClient>();
      if (cancellationToken == new CancellationToken())
        return client.ScanStreamAsync(inputStream, instanceId, resourceName).SyncResult<ScanResult>();
      using (CancellationTokenSource linkedTokenSource = CancellationTokenSource.CreateLinkedTokenSource(deploymentContext.CancellationToken, cancellationToken))
        return client.ScanStreamAsync(inputStream, instanceId, resourceName, cancellationToken: linkedTokenSource.Token).SyncResult<ScanResult>();
    }

    protected virtual async Task<BatchScanResult> GetBatchScanResult(
      IVssRequestContext requestContext,
      Container container,
      BatchScanOptions options,
      CompressionLevel batchScanTargetCompressionLevel,
      CancellationToken cancellationToken)
    {
      DevSecOpsHttpClient client = requestContext.To(TeamFoundationHostType.Deployment).GetClient<DevSecOpsHttpClient>();
      if (cancellationToken == new CancellationToken())
        return await client.ScanBatchAsync(container.Metadata, (IEnumerable<ScanTarget>) container.ScanTargets, options, container.CorrelationId, batchScanTargetCompressionLevel, cancellationToken: cancellationToken);
      using (CancellationTokenSource cts = CancellationTokenSource.CreateLinkedTokenSource(requestContext.CancellationToken, cancellationToken))
        return await client.ScanBatchAsync(container.Metadata, (IEnumerable<ScanTarget>) container.ScanTargets, options, container.CorrelationId, batchScanTargetCompressionLevel, cancellationToken: cts.Token);
    }

    internal static string CreateInstanceId(
      PipelineType pipelineType,
      Guid hostId,
      Guid projectId,
      string instanceId,
      bool isInternalExperience = true)
    {
      return string.Format("{0}{1}/{2}/{3}/", (object) string.Format((IFormatProvider) CultureInfo.InvariantCulture, isInternalExperience ? "ado-int/{0}/" : "ado/{0}/", (object) pipelineType.ToString()), (object) hostId, (object) projectId, (object) instanceId);
    }

    public void ServiceStart(IVssRequestContext systemRequestContext)
    {
    }

    public void ServiceEnd(IVssRequestContext systemRequestContext)
    {
    }
  }
}
