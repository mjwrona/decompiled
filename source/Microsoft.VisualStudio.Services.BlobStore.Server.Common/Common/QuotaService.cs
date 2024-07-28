// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.BlobStore.Server.Common.QuotaService
// Assembly: Microsoft.VisualStudio.Services.BlobStore.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: CB48D0BF-32A2-483C-A1D4-2F10DEBB3D56
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.BlobStore.Server.Common.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.AzComm.SharedContracts;
using Microsoft.VisualStudio.Services.AzComm.WebApi.Contracts;
using Microsoft.VisualStudio.Services.AzComm.WebApi.HttpClients;
using Microsoft.VisualStudio.Services.Commerce;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Content.Common;
using Microsoft.VisualStudio.Services.Content.Server.Common;
using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.BlobStore.Server.Common
{
  public class QuotaService : IQuotaService, IVssFrameworkService
  {
    private readonly string QuotaEvaluationDueInSeconds = "/Configuration/BlobStore/QuotaCapping/QuotaEvaluationDueInSeconds";
    private readonly string QuotaHardBlockDueInSeconds = "/Configuration/BlobStore/QuotaCapping/QuotaHardBlockDueInSeconds";
    private readonly string QuotaHardBlock = "/Configuration/BlobStore/QuotaCapping/QuotaHardBlock";
    private IVssRegistryService registryService;
    private const int DefaultQuotaHardBlockDueInSeconds = 600;
    private const int DefaultQuotaEvaluationDueInSeconds = 3600;
    private QuotaCacheService quotaCacheService;
    private readonly ConcurrencyConsolidator<Guid, bool> quotaServiceConcurrencyConsolidator;
    private bool enforceHardBlock;
    private bool disallowRequest;
    private const int TracePoint = 5701995;
    private const string ProductTraceArea = "QuotaService";
    private const string ProductTraceLayer = "Service";
    protected TraceData ProductTraceData = new TraceData()
    {
      Area = nameof (QuotaService),
      Layer = "Service"
    };

    private TimeSpan QuotaEvaluationDuration { get; set; }

    private TimeSpan QuotaHardBlockDuration { get; set; }

    public QuotaService() => this.quotaServiceConcurrencyConsolidator = new ConcurrencyConsolidator<Guid, bool>(true, 1);

    public void ServiceStart(IVssRequestContext reqContext)
    {
      using (Microsoft.VisualStudio.Services.Content.Server.Common.Tracer tracer = Microsoft.VisualStudio.Services.Content.Server.Common.Tracer.Enter(reqContext, this.ProductTraceData, 5701995, nameof (ServiceStart)))
      {
        tracer.TraceAlways(string.Format("[{0}]:Starting for Host: {1}.", (object) nameof (QuotaService), (object) reqContext.ServiceHost.InstanceId));
        this.registryService = reqContext.GetService<IVssRegistryService>();
        this.quotaCacheService = reqContext.GetService<QuotaCacheService>();
        this.QuotaEvaluationDuration = TimeSpan.FromSeconds((double) this.registryService.GetValue<int>(reqContext, (RegistryQuery) this.QuotaEvaluationDueInSeconds, true, 3600));
        this.enforceHardBlock = this.registryService.GetValue<bool>(reqContext, (RegistryQuery) this.QuotaHardBlock, true, false);
        this.QuotaHardBlockDuration = TimeSpan.FromSeconds((double) this.registryService.GetValue<int>(reqContext, (RegistryQuery) this.QuotaHardBlockDueInSeconds, true, 600));
        tracer.TraceAlways("[QuotaService]: " + string.Format("Details for Host: {0} ", (object) reqContext.ServiceHost.InstanceId) + string.Format("{0}: {1} ", (object) "QuotaEvaluationDuration", (object) this.QuotaEvaluationDuration) + string.Format("{0}: {1}.", (object) "QuotaHardBlockDuration", (object) this.QuotaHardBlockDuration));
      }
    }

    public void ServiceEnd(IVssRequestContext reqContext) => this.MostRecentQuotaEvaluationTime = new DateTimeOffset();

    public Task<bool> ValidateQuotaAsync(
      IVssRequestContext requestContext,
      string scope,
      ResourceName targetResource = ResourceName.Artifacts)
    {
      return this.quotaServiceConcurrencyConsolidator.RunOnceAsync(requestContext.ServiceHost.InstanceId, (Func<Task<bool>>) (() => this.EvaluateQuotaHelper(requestContext, targetResource, scope)));
    }

    public bool IsQuotaExceededV2(IVssRequestContext requestContext)
    {
      Guid instanceId = requestContext.ServiceHost.InstanceId;
      MeterUsage2GetResponse hostMeterInfo = requestContext.To(TeamFoundationHostType.Deployment).GetClient<MeterUsage2HttpClient>().GetMeterUsageAsync(instanceId, AzCommMeterIds.ArtifactsMeterId).SyncResult<MeterUsage2GetResponse>();
      return !this.IsUseable(requestContext, hostMeterInfo);
    }

    public DateTimeOffset MostRecentQuotaEvaluationTime { get; private set; }

    public DateTimeOffset MostRecentQuotaHardBlockEval { get; private set; }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool IsQuotaEvaluationDue(IVssRequestContext requestContext)
    {
      this.QuotaEvaluationDuration = TimeSpan.FromSeconds((double) this.registryService.GetValue<int>(requestContext, (RegistryQuery) this.QuotaEvaluationDueInSeconds, true, 3600));
      if (!(DateTimeOffset.UtcNow.Subtract(this.MostRecentQuotaEvaluationTime) >= this.QuotaEvaluationDuration))
        return false;
      return this.MostRecentQuotaEvaluationTime == new DateTimeOffset() || DateTimeOffset.UtcNow.Subtract(this.MostRecentQuotaEvaluationTime) > this.QuotaEvaluationDuration;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool IsHostHardBlockEvalDue(IVssRequestContext requestContext)
    {
      this.QuotaHardBlockDuration = TimeSpan.FromSeconds((double) this.registryService.GetValue<int>(requestContext, (RegistryQuery) this.QuotaHardBlockDueInSeconds, true, 600));
      if (DateTimeOffset.UtcNow.Subtract(this.MostRecentQuotaHardBlockEval) < this.QuotaHardBlockDuration)
        return false;
      return this.MostRecentQuotaHardBlockEval == new DateTimeOffset() || DateTimeOffset.UtcNow.Subtract(this.MostRecentQuotaHardBlockEval) > this.QuotaHardBlockDuration;
    }

    private bool EvaluateHardBlock(IVssRequestContext reqContext)
    {
      if (!this.IsHostHardBlockEvalDue(reqContext))
        return this.enforceHardBlock;
      this.enforceHardBlock = this.registryService.GetValue<bool>(reqContext, (RegistryQuery) this.QuotaHardBlock, true, false);
      this.disallowRequest = this.enforceHardBlock;
      this.MostRecentQuotaHardBlockEval = DateTimeOffset.UtcNow;
      return this.enforceHardBlock;
    }

    private bool IsExemptedFromQuota(string scope)
    {
      if (string.IsNullOrWhiteSpace(scope))
        return true;
      bool flag;
      switch (this.quotaCacheService.HostDesignation)
      {
        case BlobStoreUtils.HostAccountType.Internal:
          flag = scope.Equals("buildartifacts", StringComparison.OrdinalIgnoreCase) || scope.Equals("buildlogs", StringComparison.OrdinalIgnoreCase) || scope.Equals("cargo", StringComparison.OrdinalIgnoreCase);
          break;
        case BlobStoreUtils.HostAccountType.External:
          flag = scope.Equals("pipelineartifact", StringComparison.OrdinalIgnoreCase) || scope.Equals("pipelinecache", StringComparison.OrdinalIgnoreCase) || scope.Equals("symbol", StringComparison.OrdinalIgnoreCase) || scope.Equals("buildartifacts", StringComparison.OrdinalIgnoreCase) || scope.Equals("buildlogs", StringComparison.OrdinalIgnoreCase) || scope.Equals("cargo", StringComparison.OrdinalIgnoreCase);
          break;
        default:
          flag = false;
          break;
      }
      return flag;
    }

    private Task<bool> EvaluateQuotaHelper(
      IVssRequestContext requestContext,
      ResourceName targetResource,
      string scope)
    {
      if (this.IsExemptedFromQuota(scope))
        return Task.FromResult<bool>(true);
      if (this.EvaluateHardBlock(requestContext))
        throw new ArtifactBillingException();
      if (!this.IsQuotaEvaluationDue(requestContext))
      {
        if (!this.disallowRequest)
          return Task.FromResult<bool>(true);
        throw new ArtifactBillingException();
      }
      using (Microsoft.VisualStudio.Services.Content.Server.Common.Tracer tracer = Microsoft.VisualStudio.Services.Content.Server.Common.Tracer.Enter(requestContext, this.ProductTraceData, 5701995, nameof (EvaluateQuotaHelper)))
      {
        string name = Enum.GetName(typeof (ResourceName), (object) targetResource);
        MeterUsage2GetResponse hostMeterInfo;
        try
        {
          hostMeterInfo = this.quotaCacheService.PerformCacheLookupV2(requestContext);
        }
        catch (Exception ex)
        {
          tracer.TraceAlways("[QuotaService]: Couldn't retrieve metric from e-commerce for " + name + ".");
          tracer.TraceException(ex);
          return Task.FromResult<bool>(false);
        }
        this.MostRecentQuotaEvaluationTime = DateTimeOffset.UtcNow;
        if (this.IsUseable(requestContext, hostMeterInfo))
        {
          this.disallowRequest = false;
          return Task.FromResult<bool>(true);
        }
        this.disallowRequest = true;
        throw new ArtifactBillingException();
      }
    }

    private bool IsUseable(IVssRequestContext requestContext, MeterUsage2GetResponse hostMeterInfo)
    {
      if (hostMeterInfo != null)
        return hostMeterInfo.AvailableQuantity > 0.0;
      using (Microsoft.VisualStudio.Services.Content.Server.Common.Tracer tracer = Microsoft.VisualStudio.Services.Content.Server.Common.Tracer.Enter(requestContext, this.ProductTraceData, 5701995, nameof (IsUseable)))
      {
        tracer.TraceAlways(string.Format("[{0}]: meter info retrieved from Commerce is null for host {1}.", (object) nameof (QuotaService), (object) requestContext.ServiceHost.InstanceId));
        return true;
      }
    }
  }
}
