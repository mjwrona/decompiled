// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.BlobStore.Server.Common.UsageInfoService
// Assembly: Microsoft.VisualStudio.Services.BlobStore.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: D3AB5C9B-EB54-4477-A304-63BB297414A3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.BlobStore.Server.dll

using Microsoft.TeamFoundation.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.BlobStore.Common;
using Microsoft.VisualStudio.Services.BlobStore.Server.Billing;
using Microsoft.VisualStudio.Services.BlobStore.Server.Billing.PackagingBreakdown;
using Microsoft.VisualStudio.Services.Content.Common;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.BlobStore.Server.Common
{
  public class UsageInfoService : IUsageInfoService, IVssFrameworkService
  {
    private const int Tracepoint = 5701993;
    private const string ProductTraceArea = "BlobStore";
    private const string ProductTraceLayer = "UsageInfoService";
    private const double DefaultUsageMultiplier = 1.0;
    private const double DefaultDedupMultiplier = 1.0;
    private const string UsageMultiplierRegKey = "/UsageMultiplier";
    private const string DedupMultiplierRegBaseKey = "/DedupMultiplier";
    private readonly string FileDedupMultiplierRegKey = "/DedupMultiplier/File";
    private readonly string ChunkDedupMultiplierRegKey = "/DedupMultiplier/Chunk";
    private readonly string RegistryBasePath = ServiceRegistryConstants.BillingConfigRootPath;
    private readonly Func<DateTimeOffset, DateTimeOffset, DateTimeOffset> GetMaxDate = (Func<DateTimeOffset, DateTimeOffset, DateTimeOffset>) ((f, s) => !(f > s) ? s : f);
    private IDictionary<string, DataContracts.RawStorageBreakdownInfo> breakdownByDomain;

    protected double UsageMultiplier { get; set; } = 1.0;

    protected double DedupMultiplier { get; set; } = 1.0;

    protected double FileDedupMultiplier { get; set; } = 1.0;

    protected double ChunkDedupMultiplier { get; set; } = 1.0;

    protected bool IsHostInternal { get; set; }

    protected bool IsMultiDomainEnabled { get; set; }

    public Task<DataContracts.MeterUsageInfo> GetUsedOverMaxQtyUsageInfo(
      IVssRequestContext requestContext,
      DateTimeOffset maxTime)
    {
      StorageMetric storageMetric1 = this.FetchStorageMetric<StorageVolumeMeterJobResult>(requestContext, StorageVolumeMeterJob.StorageVolumeMeterJobId, (Func<StorageVolumeMeterJobResult, double>) (job => job.TotalLogicalVolumeInGiB), maxTime);
      StorageMetric storageMetric2 = this.FetchStorageMetric<StorageVolumeMeterJobResult>(requestContext, StorageVolumeMeterJob.StorageVolumeMeterJobId, (Func<StorageVolumeMeterJobResult, double>) (job => job.TotalMaxVolumeInGiB), maxTime);
      return Task.FromResult<DataContracts.MeterUsageInfo>(new DataContracts.MeterUsageInfo()
      {
        UsedQty = storageMetric1 != null ? storageMetric1.TotalBytes : 0.0,
        MaxQty = storageMetric2 != null ? storageMetric2.TotalBytes : 0.0
      });
    }

    public Task<DataContracts.UsageInfo> GetStorageUsageInfo(
      IVssRequestContext requestContext,
      DateTimeOffset maxTime)
    {
      this.FetchConfig(requestContext);
      StorageMetric storageMetric1 = this.IsMultiDomainEnabled ? this.AggregateMetricAcrossDomains<ParentFileStorageJobInfo>(requestContext, FileStorageSizeJob.FileStorageSizeJobId, (Func<ParentFileStorageJobInfo, double>) (job => (double) job.Result.TotalLogicalBytes), (Func<ParentFileStorageJobInfo, string>) (job => job.DomainId), maxTime) : this.FetchStorageMetric<ParentFileStorageJobInfo>(requestContext, FileStorageSizeJob.FileStorageSizeJobId, (Func<ParentFileStorageJobInfo, double>) (job => (double) job.Result.TotalLogicalBytes), maxTime);
      StorageMetric storageMetric2 = this.IsMultiDomainEnabled ? this.AggregateMetricAcrossDomains<DedupLogicalDataJobPartitionedInfo>(requestContext, ChunkDedupLogicalSizeJob.ChunkLogicalUsageInfoJobId, (Func<DedupLogicalDataJobPartitionedInfo, double>) (job => (double) job.Result.TotalBytes), (Func<DedupLogicalDataJobPartitionedInfo, string>) (job => job.DomainId), maxTime) : this.FetchStorageMetric<DedupLogicalDataJobPartitionedInfo>(requestContext, ChunkDedupLogicalSizeJob.ChunkLogicalUsageInfoJobId, (Func<DedupLogicalDataJobPartitionedInfo, double>) (job => (double) job.Result.TotalBytes), maxTime);
      StorageMetric storageMetric3 = this.IsMultiDomainEnabled ? this.AggregateMetricAcrossDomains<ParentFileStorageJobInfo>(requestContext, FileStorageSizeJob.FileStorageSizeJobId, (Func<ParentFileStorageJobInfo, double>) (job => (double) job.Result.TotalBytes), (Func<ParentFileStorageJobInfo, string>) (job => job.DomainId), maxTime) : this.FetchStorageMetric<ParentFileStorageJobInfo>(requestContext, FileStorageSizeJob.FileStorageSizeJobId, (Func<ParentFileStorageJobInfo, double>) (job => (double) job.Result.TotalBytes), maxTime);
      StorageMetric storageMetric4 = this.IsMultiDomainEnabled ? this.AggregateMetricAcrossDomains<ParentDedupDataJobInfo>(requestContext, ChunkDedupPhysicalSizeJob.ChunkPhysicalSizeInfoJobId, (Func<ParentDedupDataJobInfo, double>) (job => (double) job.Result.TotalBytes), (Func<ParentDedupDataJobInfo, string>) (job => job.DomainId), maxTime) : this.FetchStorageMetric<ParentDedupDataJobInfo>(requestContext, ChunkDedupPhysicalSizeJob.ChunkPhysicalSizeInfoJobId, (Func<ParentDedupDataJobInfo, double>) (job => (double) job.Result.TotalBytes), maxTime);
      if (this.IsHostInternal)
      {
        StorageMetric storageMetric5 = this.IsMultiDomainEnabled ? this.AggregateMetricAcrossDomains<DedupLogicalDataJobPartitionedInfo>(requestContext, ChunkDedupLogicalSizeJob.ChunkLogicalUsageInfoJobId, (Func<DedupLogicalDataJobPartitionedInfo, double>) (job => (double) job.Result.TotalBytesOutOfScope), (Func<DedupLogicalDataJobPartitionedInfo, string>) (job => job.DomainId), maxTime) : this.FetchStorageMetric<DedupLogicalDataJobPartitionedInfo>(requestContext, ChunkDedupLogicalSizeJob.ChunkLogicalUsageInfoJobId, (Func<DedupLogicalDataJobPartitionedInfo, double>) (job => (double) job.Result.TotalBytesOutOfScope), maxTime);
        if (storageMetric2 != null && storageMetric5 != null)
          storageMetric2.TotalBytes += storageMetric5.TotalBytes;
      }
      StorageMetric storageMetric6 = this.IsMultiDomainEnabled ? this.AggregateMetricAcrossDomains<ParentFileStorageJobInfo>(requestContext, FileStorageSizeJob.FileStorageSizeJobId, (Func<ParentFileStorageJobInfo, double>) (job => UsageInfoService.GetLogicalStorageSizeByScope(ArtifactScopeType.Drop, job.Result)), (Func<ParentFileStorageJobInfo, string>) (job => job.DomainId), maxTime) : this.FetchStorageMetric<ParentFileStorageJobInfo>(requestContext, FileStorageSizeJob.FileStorageSizeJobId, (Func<ParentFileStorageJobInfo, double>) (job => UsageInfoService.GetLogicalStorageSizeByScope(ArtifactScopeType.Drop, job.Result)), maxTime);
      StorageMetric storageMetric7 = this.IsMultiDomainEnabled ? this.AggregateMetricAcrossDomains<ParentFileStorageJobInfo>(requestContext, FileStorageSizeJob.FileStorageSizeJobId, (Func<ParentFileStorageJobInfo, double>) (job => UsageInfoService.GetLogicalStorageSizeByScope(ArtifactScopeType.Symbol, job.Result)), (Func<ParentFileStorageJobInfo, string>) (job => job.DomainId), maxTime) : this.FetchStorageMetric<ParentFileStorageJobInfo>(requestContext, FileStorageSizeJob.FileStorageSizeJobId, (Func<ParentFileStorageJobInfo, double>) (job => UsageInfoService.GetLogicalStorageSizeByScope(ArtifactScopeType.Symbol, job.Result)), maxTime);
      StorageMetric storageMetric8 = this.IsMultiDomainEnabled ? this.AggregateMetricAcrossDomains<ParentFileStorageJobInfo>(requestContext, FileStorageSizeJob.FileStorageSizeJobId, (Func<ParentFileStorageJobInfo, double>) (job => UsageInfoService.GetLogicalStorageSizeByScope(ArtifactScopeType.PipelineArtifact, job.Result)), (Func<ParentFileStorageJobInfo, string>) (job => job.DomainId), maxTime) : this.FetchStorageMetric<ParentFileStorageJobInfo>(requestContext, FileStorageSizeJob.FileStorageSizeJobId, (Func<ParentFileStorageJobInfo, double>) (job => UsageInfoService.GetLogicalStorageSizeByScope(ArtifactScopeType.PipelineArtifact, job.Result)), maxTime);
      StorageMetric storageMetric9 = this.IsMultiDomainEnabled ? this.AggregateMetricAcrossDomains<ParentFileStorageJobInfo>(requestContext, FileStorageSizeJob.FileStorageSizeJobId, (Func<ParentFileStorageJobInfo, double>) (job => UsageInfoService.GetLogicalStorageSizeByScope(ArtifactScopeType.PipelineCache, job.Result)), (Func<ParentFileStorageJobInfo, string>) (job => job.DomainId), maxTime) : this.FetchStorageMetric<ParentFileStorageJobInfo>(requestContext, FileStorageSizeJob.FileStorageSizeJobId, (Func<ParentFileStorageJobInfo, double>) (job => UsageInfoService.GetLogicalStorageSizeByScope(ArtifactScopeType.PipelineCache, job.Result)), maxTime);
      StorageMetric storageMetric10 = this.IsMultiDomainEnabled ? this.AggregateMetricAcrossDomains<ParentFileStorageJobInfo>(requestContext, FileStorageSizeJob.FileStorageSizeJobId, (Func<ParentFileStorageJobInfo, double>) (job => UsageInfoService.GetLogicalStorageSizeByScope(ArtifactScopeType.Packaging, job.Result)), (Func<ParentFileStorageJobInfo, string>) (job => job.DomainId), maxTime) : this.FetchStorageMetric<ParentFileStorageJobInfo>(requestContext, FileStorageSizeJob.FileStorageSizeJobId, (Func<ParentFileStorageJobInfo, double>) (job => UsageInfoService.GetLogicalStorageSizeByScope(ArtifactScopeType.Packaging, job.Result)), maxTime);
      StorageMetric storageMetric11 = this.IsMultiDomainEnabled ? this.AggregateMetricAcrossDomains<ParentFileStorageJobInfo>(requestContext, FileStorageSizeJob.FileStorageSizeJobId, (Func<ParentFileStorageJobInfo, double>) (job => UsageInfoService.GetLogicalStorageSizeByScope(ArtifactScopeType.Npm, job.Result)), (Func<ParentFileStorageJobInfo, string>) (job => job.DomainId), maxTime) : this.FetchStorageMetric<ParentFileStorageJobInfo>(requestContext, FileStorageSizeJob.FileStorageSizeJobId, (Func<ParentFileStorageJobInfo, double>) (job => UsageInfoService.GetLogicalStorageSizeByScope(ArtifactScopeType.Npm, job.Result)), maxTime);
      StorageMetric storageMetric12 = this.IsMultiDomainEnabled ? this.AggregateMetricAcrossDomains<ParentFileStorageJobInfo>(requestContext, FileStorageSizeJob.FileStorageSizeJobId, (Func<ParentFileStorageJobInfo, double>) (job => UsageInfoService.GetLogicalStorageSizeByScope(ArtifactScopeType.UPack, job.Result)), (Func<ParentFileStorageJobInfo, string>) (job => job.DomainId), maxTime) : this.FetchStorageMetric<ParentFileStorageJobInfo>(requestContext, FileStorageSizeJob.FileStorageSizeJobId, (Func<ParentFileStorageJobInfo, double>) (job => UsageInfoService.GetLogicalStorageSizeByScope(ArtifactScopeType.UPack, job.Result)), maxTime);
      StorageMetric storageMetric13 = this.IsMultiDomainEnabled ? this.AggregateMetricAcrossDomains<ParentFileStorageJobInfo>(requestContext, FileStorageSizeJob.FileStorageSizeJobId, (Func<ParentFileStorageJobInfo, double>) (job => UsageInfoService.GetLogicalStorageSizeByScope(ArtifactScopeType.Ivy, job.Result)), (Func<ParentFileStorageJobInfo, string>) (job => job.DomainId), maxTime) : this.FetchStorageMetric<ParentFileStorageJobInfo>(requestContext, FileStorageSizeJob.FileStorageSizeJobId, (Func<ParentFileStorageJobInfo, double>) (job => UsageInfoService.GetLogicalStorageSizeByScope(ArtifactScopeType.Ivy, job.Result)), maxTime);
      StorageMetric storageMetric14 = this.IsMultiDomainEnabled ? this.AggregateMetricAcrossDomains<ParentFileStorageJobInfo>(requestContext, FileStorageSizeJob.FileStorageSizeJobId, (Func<ParentFileStorageJobInfo, double>) (job => UsageInfoService.GetLogicalStorageSizeByScope(ArtifactScopeType.Maven, job.Result)), (Func<ParentFileStorageJobInfo, string>) (job => job.DomainId), maxTime) : this.FetchStorageMetric<ParentFileStorageJobInfo>(requestContext, FileStorageSizeJob.FileStorageSizeJobId, (Func<ParentFileStorageJobInfo, double>) (job => UsageInfoService.GetLogicalStorageSizeByScope(ArtifactScopeType.Maven, job.Result)), maxTime);
      StorageMetric storageMetric15 = this.IsMultiDomainEnabled ? this.AggregateMetricAcrossDomains<ParentFileStorageJobInfo>(requestContext, FileStorageSizeJob.FileStorageSizeJobId, (Func<ParentFileStorageJobInfo, double>) (job => UsageInfoService.GetLogicalStorageSizeByScope(ArtifactScopeType.Cargo, job.Result)), (Func<ParentFileStorageJobInfo, string>) (job => job.DomainId), maxTime) : this.FetchStorageMetric<ParentFileStorageJobInfo>(requestContext, FileStorageSizeJob.FileStorageSizeJobId, (Func<ParentFileStorageJobInfo, double>) (job => UsageInfoService.GetLogicalStorageSizeByScope(ArtifactScopeType.Cargo, job.Result)), maxTime);
      StorageMetric storageMetric16 = this.IsMultiDomainEnabled ? this.AggregateMetricAcrossDomains<ParentFileStorageJobInfo>(requestContext, FileStorageSizeJob.FileStorageSizeJobId, (Func<ParentFileStorageJobInfo, double>) (job => UsageInfoService.GetLogicalStorageSizeByScope(ArtifactScopeType.NuGet, job.Result)), (Func<ParentFileStorageJobInfo, string>) (job => job.DomainId), maxTime) : this.FetchStorageMetric<ParentFileStorageJobInfo>(requestContext, FileStorageSizeJob.FileStorageSizeJobId, (Func<ParentFileStorageJobInfo, double>) (job => UsageInfoService.GetLogicalStorageSizeByScope(ArtifactScopeType.NuGet, job.Result)), maxTime);
      StorageMetric storageMetric17 = this.IsMultiDomainEnabled ? this.AggregateMetricAcrossDomains<ParentFileStorageJobInfo>(requestContext, FileStorageSizeJob.FileStorageSizeJobId, (Func<ParentFileStorageJobInfo, double>) (job => UsageInfoService.GetLogicalStorageSizeByScope(ArtifactScopeType.PyPi, job.Result)), (Func<ParentFileStorageJobInfo, string>) (job => job.DomainId), maxTime) : this.FetchStorageMetric<ParentFileStorageJobInfo>(requestContext, FileStorageSizeJob.FileStorageSizeJobId, (Func<ParentFileStorageJobInfo, double>) (job => UsageInfoService.GetLogicalStorageSizeByScope(ArtifactScopeType.PyPi, job.Result)), maxTime);
      StorageMetric storageMetric18 = this.IsMultiDomainEnabled ? this.AggregateMetricAcrossDomains<ParentFileStorageJobInfo>(requestContext, FileStorageSizeJob.FileStorageSizeJobId, (Func<ParentFileStorageJobInfo, double>) (job => UsageInfoService.GetLogicalStorageSizeByScope(ArtifactScopeType.Others, job.Result)), (Func<ParentFileStorageJobInfo, string>) (job => job.DomainId), maxTime) : this.FetchStorageMetric<ParentFileStorageJobInfo>(requestContext, FileStorageSizeJob.FileStorageSizeJobId, (Func<ParentFileStorageJobInfo, double>) (job => UsageInfoService.GetLogicalStorageSizeByScope(ArtifactScopeType.Others, job.Result)), maxTime);
      StorageMetric storageMetric19 = this.IsMultiDomainEnabled ? this.AggregateMetricAcrossDomains<ParentFileStorageJobInfo>(requestContext, FileStorageSizeJob.FileStorageSizeJobId, (Func<ParentFileStorageJobInfo, double>) (job => UsageInfoService.GetLogicalStorageSizeByScope(ArtifactScopeType.None, job.Result)), (Func<ParentFileStorageJobInfo, string>) (job => job.DomainId), maxTime) : this.FetchStorageMetric<ParentFileStorageJobInfo>(requestContext, FileStorageSizeJob.FileStorageSizeJobId, (Func<ParentFileStorageJobInfo, double>) (job => UsageInfoService.GetLogicalStorageSizeByScope(ArtifactScopeType.None, job.Result)), maxTime);
      StorageMetric storageMetric20 = this.IsMultiDomainEnabled ? this.AggregateMetricAcrossDomains<ParentFileStorageJobInfo>(requestContext, FileStorageSizeJob.FileStorageSizeJobId, (Func<ParentFileStorageJobInfo, double>) (job => UsageInfoService.GetLogicalStorageSizeByScope(ArtifactScopeType.BuildArtifacts, job.Result)), (Func<ParentFileStorageJobInfo, string>) (job => job.DomainId), maxTime) : this.FetchStorageMetric<ParentFileStorageJobInfo>(requestContext, FileStorageSizeJob.FileStorageSizeJobId, (Func<ParentFileStorageJobInfo, double>) (job => UsageInfoService.GetLogicalStorageSizeByScope(ArtifactScopeType.BuildArtifacts, job.Result)), maxTime);
      StorageMetric storageMetric21 = this.IsMultiDomainEnabled ? this.AggregateMetricAcrossDomains<ParentFileStorageJobInfo>(requestContext, FileStorageSizeJob.FileStorageSizeJobId, (Func<ParentFileStorageJobInfo, double>) (job => UsageInfoService.GetLogicalStorageSizeByScope(ArtifactScopeType.BuildLogs, job.Result)), (Func<ParentFileStorageJobInfo, string>) (job => job.DomainId), maxTime) : this.FetchStorageMetric<ParentFileStorageJobInfo>(requestContext, FileStorageSizeJob.FileStorageSizeJobId, (Func<ParentFileStorageJobInfo, double>) (job => UsageInfoService.GetLogicalStorageSizeByScope(ArtifactScopeType.BuildLogs, job.Result)), maxTime);
      StorageMetric storageMetric22 = this.IsMultiDomainEnabled ? this.AggregateMetricAcrossDomains<ParentFileStorageJobInfo>(requestContext, FileStorageSizeJob.FileStorageSizeJobId, (Func<ParentFileStorageJobInfo, double>) (job => UsageInfoService.GetPhysicalStorageSizeByScope(ArtifactScopeType.Drop, job.Result)), (Func<ParentFileStorageJobInfo, string>) (job => job.DomainId), maxTime) : this.FetchStorageMetric<ParentFileStorageJobInfo>(requestContext, FileStorageSizeJob.FileStorageSizeJobId, (Func<ParentFileStorageJobInfo, double>) (job => UsageInfoService.GetPhysicalStorageSizeByScope(ArtifactScopeType.Drop, job.Result)), maxTime);
      StorageMetric storageMetric23 = this.IsMultiDomainEnabled ? this.AggregateMetricAcrossDomains<ParentFileStorageJobInfo>(requestContext, FileStorageSizeJob.FileStorageSizeJobId, (Func<ParentFileStorageJobInfo, double>) (job => UsageInfoService.GetPhysicalStorageSizeByScope(ArtifactScopeType.Symbol, job.Result)), (Func<ParentFileStorageJobInfo, string>) (job => job.DomainId), maxTime) : this.FetchStorageMetric<ParentFileStorageJobInfo>(requestContext, FileStorageSizeJob.FileStorageSizeJobId, (Func<ParentFileStorageJobInfo, double>) (job => UsageInfoService.GetPhysicalStorageSizeByScope(ArtifactScopeType.Symbol, job.Result)), maxTime);
      StorageMetric storageMetric24 = this.IsMultiDomainEnabled ? this.AggregateMetricAcrossDomains<ParentFileStorageJobInfo>(requestContext, FileStorageSizeJob.FileStorageSizeJobId, (Func<ParentFileStorageJobInfo, double>) (job => UsageInfoService.GetPhysicalStorageSizeByScope(ArtifactScopeType.PipelineArtifact, job.Result)), (Func<ParentFileStorageJobInfo, string>) (job => job.DomainId), maxTime) : this.FetchStorageMetric<ParentFileStorageJobInfo>(requestContext, FileStorageSizeJob.FileStorageSizeJobId, (Func<ParentFileStorageJobInfo, double>) (job => UsageInfoService.GetPhysicalStorageSizeByScope(ArtifactScopeType.PipelineArtifact, job.Result)), maxTime);
      StorageMetric storageMetric25 = this.IsMultiDomainEnabled ? this.AggregateMetricAcrossDomains<ParentFileStorageJobInfo>(requestContext, FileStorageSizeJob.FileStorageSizeJobId, (Func<ParentFileStorageJobInfo, double>) (job => UsageInfoService.GetPhysicalStorageSizeByScope(ArtifactScopeType.PipelineCache, job.Result)), (Func<ParentFileStorageJobInfo, string>) (job => job.DomainId), maxTime) : this.FetchStorageMetric<ParentFileStorageJobInfo>(requestContext, FileStorageSizeJob.FileStorageSizeJobId, (Func<ParentFileStorageJobInfo, double>) (job => UsageInfoService.GetPhysicalStorageSizeByScope(ArtifactScopeType.PipelineCache, job.Result)), maxTime);
      StorageMetric storageMetric26 = this.IsMultiDomainEnabled ? this.AggregateMetricAcrossDomains<ParentFileStorageJobInfo>(requestContext, FileStorageSizeJob.FileStorageSizeJobId, (Func<ParentFileStorageJobInfo, double>) (job => UsageInfoService.GetPhysicalStorageSizeByScope(ArtifactScopeType.Packaging, job.Result)), (Func<ParentFileStorageJobInfo, string>) (job => job.DomainId), maxTime) : this.FetchStorageMetric<ParentFileStorageJobInfo>(requestContext, FileStorageSizeJob.FileStorageSizeJobId, (Func<ParentFileStorageJobInfo, double>) (job => UsageInfoService.GetPhysicalStorageSizeByScope(ArtifactScopeType.Packaging, job.Result)), maxTime);
      StorageMetric storageMetric27 = this.IsMultiDomainEnabled ? this.AggregateMetricAcrossDomains<ParentFileStorageJobInfo>(requestContext, FileStorageSizeJob.FileStorageSizeJobId, (Func<ParentFileStorageJobInfo, double>) (job => UsageInfoService.GetPhysicalStorageSizeByScope(ArtifactScopeType.Npm, job.Result)), (Func<ParentFileStorageJobInfo, string>) (job => job.DomainId), maxTime) : this.FetchStorageMetric<ParentFileStorageJobInfo>(requestContext, FileStorageSizeJob.FileStorageSizeJobId, (Func<ParentFileStorageJobInfo, double>) (job => UsageInfoService.GetPhysicalStorageSizeByScope(ArtifactScopeType.Npm, job.Result)), maxTime);
      StorageMetric storageMetric28 = this.IsMultiDomainEnabled ? this.AggregateMetricAcrossDomains<ParentFileStorageJobInfo>(requestContext, FileStorageSizeJob.FileStorageSizeJobId, (Func<ParentFileStorageJobInfo, double>) (job => UsageInfoService.GetPhysicalStorageSizeByScope(ArtifactScopeType.UPack, job.Result)), (Func<ParentFileStorageJobInfo, string>) (job => job.DomainId), maxTime) : this.FetchStorageMetric<ParentFileStorageJobInfo>(requestContext, FileStorageSizeJob.FileStorageSizeJobId, (Func<ParentFileStorageJobInfo, double>) (job => UsageInfoService.GetPhysicalStorageSizeByScope(ArtifactScopeType.UPack, job.Result)), maxTime);
      StorageMetric storageMetric29 = this.IsMultiDomainEnabled ? this.AggregateMetricAcrossDomains<ParentFileStorageJobInfo>(requestContext, FileStorageSizeJob.FileStorageSizeJobId, (Func<ParentFileStorageJobInfo, double>) (job => UsageInfoService.GetPhysicalStorageSizeByScope(ArtifactScopeType.Ivy, job.Result)), (Func<ParentFileStorageJobInfo, string>) (job => job.DomainId), maxTime) : this.FetchStorageMetric<ParentFileStorageJobInfo>(requestContext, FileStorageSizeJob.FileStorageSizeJobId, (Func<ParentFileStorageJobInfo, double>) (job => UsageInfoService.GetPhysicalStorageSizeByScope(ArtifactScopeType.Ivy, job.Result)), maxTime);
      StorageMetric storageMetric30 = this.IsMultiDomainEnabled ? this.AggregateMetricAcrossDomains<ParentFileStorageJobInfo>(requestContext, FileStorageSizeJob.FileStorageSizeJobId, (Func<ParentFileStorageJobInfo, double>) (job => UsageInfoService.GetPhysicalStorageSizeByScope(ArtifactScopeType.Maven, job.Result)), (Func<ParentFileStorageJobInfo, string>) (job => job.DomainId), maxTime) : this.FetchStorageMetric<ParentFileStorageJobInfo>(requestContext, FileStorageSizeJob.FileStorageSizeJobId, (Func<ParentFileStorageJobInfo, double>) (job => UsageInfoService.GetPhysicalStorageSizeByScope(ArtifactScopeType.Maven, job.Result)), maxTime);
      StorageMetric storageMetric31 = this.IsMultiDomainEnabled ? this.AggregateMetricAcrossDomains<ParentFileStorageJobInfo>(requestContext, FileStorageSizeJob.FileStorageSizeJobId, (Func<ParentFileStorageJobInfo, double>) (job => UsageInfoService.GetPhysicalStorageSizeByScope(ArtifactScopeType.Cargo, job.Result)), (Func<ParentFileStorageJobInfo, string>) (job => job.DomainId), maxTime) : this.FetchStorageMetric<ParentFileStorageJobInfo>(requestContext, FileStorageSizeJob.FileStorageSizeJobId, (Func<ParentFileStorageJobInfo, double>) (job => UsageInfoService.GetPhysicalStorageSizeByScope(ArtifactScopeType.Cargo, job.Result)), maxTime);
      StorageMetric storageMetric32 = this.IsMultiDomainEnabled ? this.AggregateMetricAcrossDomains<ParentFileStorageJobInfo>(requestContext, FileStorageSizeJob.FileStorageSizeJobId, (Func<ParentFileStorageJobInfo, double>) (job => UsageInfoService.GetPhysicalStorageSizeByScope(ArtifactScopeType.NuGet, job.Result)), (Func<ParentFileStorageJobInfo, string>) (job => job.DomainId), maxTime) : this.FetchStorageMetric<ParentFileStorageJobInfo>(requestContext, FileStorageSizeJob.FileStorageSizeJobId, (Func<ParentFileStorageJobInfo, double>) (job => UsageInfoService.GetPhysicalStorageSizeByScope(ArtifactScopeType.NuGet, job.Result)), maxTime);
      StorageMetric storageMetric33 = this.IsMultiDomainEnabled ? this.AggregateMetricAcrossDomains<ParentFileStorageJobInfo>(requestContext, FileStorageSizeJob.FileStorageSizeJobId, (Func<ParentFileStorageJobInfo, double>) (job => UsageInfoService.GetPhysicalStorageSizeByScope(ArtifactScopeType.PyPi, job.Result)), (Func<ParentFileStorageJobInfo, string>) (job => job.DomainId), maxTime) : this.FetchStorageMetric<ParentFileStorageJobInfo>(requestContext, FileStorageSizeJob.FileStorageSizeJobId, (Func<ParentFileStorageJobInfo, double>) (job => UsageInfoService.GetPhysicalStorageSizeByScope(ArtifactScopeType.PyPi, job.Result)), maxTime);
      StorageMetric storageMetric34 = this.IsMultiDomainEnabled ? this.AggregateMetricAcrossDomains<ParentFileStorageJobInfo>(requestContext, FileStorageSizeJob.FileStorageSizeJobId, (Func<ParentFileStorageJobInfo, double>) (job => UsageInfoService.GetPhysicalStorageSizeByScope(ArtifactScopeType.Others, job.Result)), (Func<ParentFileStorageJobInfo, string>) (job => job.DomainId), maxTime) : this.FetchStorageMetric<ParentFileStorageJobInfo>(requestContext, FileStorageSizeJob.FileStorageSizeJobId, (Func<ParentFileStorageJobInfo, double>) (job => UsageInfoService.GetPhysicalStorageSizeByScope(ArtifactScopeType.Others, job.Result)), maxTime);
      StorageMetric storageMetric35 = this.IsMultiDomainEnabled ? this.AggregateMetricAcrossDomains<ParentFileStorageJobInfo>(requestContext, FileStorageSizeJob.FileStorageSizeJobId, (Func<ParentFileStorageJobInfo, double>) (job => UsageInfoService.GetPhysicalStorageSizeByScope(ArtifactScopeType.None, job.Result)), (Func<ParentFileStorageJobInfo, string>) (job => job.DomainId), maxTime) : this.FetchStorageMetric<ParentFileStorageJobInfo>(requestContext, FileStorageSizeJob.FileStorageSizeJobId, (Func<ParentFileStorageJobInfo, double>) (job => UsageInfoService.GetPhysicalStorageSizeByScope(ArtifactScopeType.None, job.Result)), maxTime);
      StorageMetric storageMetric36 = this.IsMultiDomainEnabled ? this.AggregateMetricAcrossDomains<ParentFileStorageJobInfo>(requestContext, FileStorageSizeJob.FileStorageSizeJobId, (Func<ParentFileStorageJobInfo, double>) (job => UsageInfoService.GetPhysicalStorageSizeByScope(ArtifactScopeType.BuildArtifacts, job.Result)), (Func<ParentFileStorageJobInfo, string>) (job => job.DomainId), maxTime) : this.FetchStorageMetric<ParentFileStorageJobInfo>(requestContext, FileStorageSizeJob.FileStorageSizeJobId, (Func<ParentFileStorageJobInfo, double>) (job => UsageInfoService.GetPhysicalStorageSizeByScope(ArtifactScopeType.BuildArtifacts, job.Result)), maxTime);
      StorageMetric storageMetric37 = this.IsMultiDomainEnabled ? this.AggregateMetricAcrossDomains<ParentFileStorageJobInfo>(requestContext, FileStorageSizeJob.FileStorageSizeJobId, (Func<ParentFileStorageJobInfo, double>) (job => UsageInfoService.GetPhysicalStorageSizeByScope(ArtifactScopeType.BuildLogs, job.Result)), (Func<ParentFileStorageJobInfo, string>) (job => job.DomainId), maxTime) : this.FetchStorageMetric<ParentFileStorageJobInfo>(requestContext, FileStorageSizeJob.FileStorageSizeJobId, (Func<ParentFileStorageJobInfo, double>) (job => UsageInfoService.GetPhysicalStorageSizeByScope(ArtifactScopeType.BuildLogs, job.Result)), maxTime);
      StorageMetric storageMetric38 = this.IsMultiDomainEnabled ? this.AggregateMetricAcrossDomains<DedupLogicalDataJobPartitionedInfo>(requestContext, ChunkDedupLogicalSizeJob.ChunkLogicalUsageInfoJobId, (Func<DedupLogicalDataJobPartitionedInfo, double>) (job => UsageInfoService.GetChunkLogicalStorageSizeByScope(ArtifactScopeType.Drop, job.Result)), (Func<DedupLogicalDataJobPartitionedInfo, string>) (job => job.DomainId), maxTime) : this.FetchStorageMetric<DedupLogicalDataJobPartitionedInfo>(requestContext, ChunkDedupLogicalSizeJob.ChunkLogicalUsageInfoJobId, (Func<DedupLogicalDataJobPartitionedInfo, double>) (job => UsageInfoService.GetChunkLogicalStorageSizeByScope(ArtifactScopeType.Drop, job.Result)), maxTime);
      StorageMetric storageMetric39 = this.IsMultiDomainEnabled ? this.AggregateMetricAcrossDomains<DedupLogicalDataJobPartitionedInfo>(requestContext, ChunkDedupLogicalSizeJob.ChunkLogicalUsageInfoJobId, (Func<DedupLogicalDataJobPartitionedInfo, double>) (job => UsageInfoService.GetChunkLogicalStorageSizeByScope(ArtifactScopeType.Symbol, job.Result)), (Func<DedupLogicalDataJobPartitionedInfo, string>) (job => job.DomainId), maxTime) : this.FetchStorageMetric<DedupLogicalDataJobPartitionedInfo>(requestContext, ChunkDedupLogicalSizeJob.ChunkLogicalUsageInfoJobId, (Func<DedupLogicalDataJobPartitionedInfo, double>) (job => UsageInfoService.GetChunkLogicalStorageSizeByScope(ArtifactScopeType.Symbol, job.Result)), maxTime);
      StorageMetric storageMetric40 = this.IsMultiDomainEnabled ? this.AggregateMetricAcrossDomains<DedupLogicalDataJobPartitionedInfo>(requestContext, ChunkDedupLogicalSizeJob.ChunkLogicalUsageInfoJobId, (Func<DedupLogicalDataJobPartitionedInfo, double>) (job => UsageInfoService.GetChunkLogicalStorageSizeByScope(ArtifactScopeType.PipelineArtifact, job.Result)), (Func<DedupLogicalDataJobPartitionedInfo, string>) (job => job.DomainId), maxTime) : this.FetchStorageMetric<DedupLogicalDataJobPartitionedInfo>(requestContext, ChunkDedupLogicalSizeJob.ChunkLogicalUsageInfoJobId, (Func<DedupLogicalDataJobPartitionedInfo, double>) (job => UsageInfoService.GetChunkLogicalStorageSizeByScope(ArtifactScopeType.PipelineArtifact, job.Result)), maxTime);
      StorageMetric storageMetric41 = this.IsMultiDomainEnabled ? this.AggregateMetricAcrossDomains<DedupLogicalDataJobPartitionedInfo>(requestContext, ChunkDedupLogicalSizeJob.ChunkLogicalUsageInfoJobId, (Func<DedupLogicalDataJobPartitionedInfo, double>) (job => UsageInfoService.GetChunkLogicalStorageSizeByScope(ArtifactScopeType.PipelineCache, job.Result)), (Func<DedupLogicalDataJobPartitionedInfo, string>) (job => job.DomainId), maxTime) : this.FetchStorageMetric<DedupLogicalDataJobPartitionedInfo>(requestContext, ChunkDedupLogicalSizeJob.ChunkLogicalUsageInfoJobId, (Func<DedupLogicalDataJobPartitionedInfo, double>) (job => UsageInfoService.GetChunkLogicalStorageSizeByScope(ArtifactScopeType.PipelineCache, job.Result)), maxTime);
      StorageMetric storageMetric42 = this.IsMultiDomainEnabled ? this.AggregateMetricAcrossDomains<DedupLogicalDataJobPartitionedInfo>(requestContext, ChunkDedupLogicalSizeJob.ChunkLogicalUsageInfoJobId, (Func<DedupLogicalDataJobPartitionedInfo, double>) (job => UsageInfoService.GetChunkLogicalStorageSizeByScope(ArtifactScopeType.Packaging, job.Result)), (Func<DedupLogicalDataJobPartitionedInfo, string>) (job => job.DomainId), maxTime) : this.FetchStorageMetric<DedupLogicalDataJobPartitionedInfo>(requestContext, ChunkDedupLogicalSizeJob.ChunkLogicalUsageInfoJobId, (Func<DedupLogicalDataJobPartitionedInfo, double>) (job => UsageInfoService.GetChunkLogicalStorageSizeByScope(ArtifactScopeType.Packaging, job.Result)), maxTime);
      StorageMetric storageMetric43 = this.IsMultiDomainEnabled ? this.AggregateMetricAcrossDomains<DedupLogicalDataJobPartitionedInfo>(requestContext, ChunkDedupLogicalSizeJob.ChunkLogicalUsageInfoJobId, (Func<DedupLogicalDataJobPartitionedInfo, double>) (job => UsageInfoService.GetChunkLogicalStorageSizeByScope(ArtifactScopeType.Npm, job.Result)), (Func<DedupLogicalDataJobPartitionedInfo, string>) (job => job.DomainId), maxTime) : this.FetchStorageMetric<DedupLogicalDataJobPartitionedInfo>(requestContext, ChunkDedupLogicalSizeJob.ChunkLogicalUsageInfoJobId, (Func<DedupLogicalDataJobPartitionedInfo, double>) (job => UsageInfoService.GetChunkLogicalStorageSizeByScope(ArtifactScopeType.Npm, job.Result)), maxTime);
      StorageMetric storageMetric44 = this.IsMultiDomainEnabled ? this.AggregateMetricAcrossDomains<DedupLogicalDataJobPartitionedInfo>(requestContext, ChunkDedupLogicalSizeJob.ChunkLogicalUsageInfoJobId, (Func<DedupLogicalDataJobPartitionedInfo, double>) (job => UsageInfoService.GetChunkLogicalStorageSizeByScope(ArtifactScopeType.UPack, job.Result)), (Func<DedupLogicalDataJobPartitionedInfo, string>) (job => job.DomainId), maxTime) : this.FetchStorageMetric<DedupLogicalDataJobPartitionedInfo>(requestContext, ChunkDedupLogicalSizeJob.ChunkLogicalUsageInfoJobId, (Func<DedupLogicalDataJobPartitionedInfo, double>) (job => UsageInfoService.GetChunkLogicalStorageSizeByScope(ArtifactScopeType.UPack, job.Result)), maxTime);
      StorageMetric storageMetric45 = this.IsMultiDomainEnabled ? this.AggregateMetricAcrossDomains<DedupLogicalDataJobPartitionedInfo>(requestContext, ChunkDedupLogicalSizeJob.ChunkLogicalUsageInfoJobId, (Func<DedupLogicalDataJobPartitionedInfo, double>) (job => UsageInfoService.GetChunkLogicalStorageSizeByScope(ArtifactScopeType.Ivy, job.Result)), (Func<DedupLogicalDataJobPartitionedInfo, string>) (job => job.DomainId), maxTime) : this.FetchStorageMetric<DedupLogicalDataJobPartitionedInfo>(requestContext, ChunkDedupLogicalSizeJob.ChunkLogicalUsageInfoJobId, (Func<DedupLogicalDataJobPartitionedInfo, double>) (job => UsageInfoService.GetChunkLogicalStorageSizeByScope(ArtifactScopeType.Ivy, job.Result)), maxTime);
      StorageMetric storageMetric46 = this.IsMultiDomainEnabled ? this.AggregateMetricAcrossDomains<DedupLogicalDataJobPartitionedInfo>(requestContext, ChunkDedupLogicalSizeJob.ChunkLogicalUsageInfoJobId, (Func<DedupLogicalDataJobPartitionedInfo, double>) (job => UsageInfoService.GetChunkLogicalStorageSizeByScope(ArtifactScopeType.Maven, job.Result)), (Func<DedupLogicalDataJobPartitionedInfo, string>) (job => job.DomainId), maxTime) : this.FetchStorageMetric<DedupLogicalDataJobPartitionedInfo>(requestContext, ChunkDedupLogicalSizeJob.ChunkLogicalUsageInfoJobId, (Func<DedupLogicalDataJobPartitionedInfo, double>) (job => UsageInfoService.GetChunkLogicalStorageSizeByScope(ArtifactScopeType.Maven, job.Result)), maxTime);
      StorageMetric storageMetric47 = this.IsMultiDomainEnabled ? this.AggregateMetricAcrossDomains<DedupLogicalDataJobPartitionedInfo>(requestContext, ChunkDedupLogicalSizeJob.ChunkLogicalUsageInfoJobId, (Func<DedupLogicalDataJobPartitionedInfo, double>) (job => UsageInfoService.GetChunkLogicalStorageSizeByScope(ArtifactScopeType.Cargo, job.Result)), (Func<DedupLogicalDataJobPartitionedInfo, string>) (job => job.DomainId), maxTime) : this.FetchStorageMetric<DedupLogicalDataJobPartitionedInfo>(requestContext, ChunkDedupLogicalSizeJob.ChunkLogicalUsageInfoJobId, (Func<DedupLogicalDataJobPartitionedInfo, double>) (job => UsageInfoService.GetChunkLogicalStorageSizeByScope(ArtifactScopeType.Cargo, job.Result)), maxTime);
      StorageMetric storageMetric48 = this.IsMultiDomainEnabled ? this.AggregateMetricAcrossDomains<DedupLogicalDataJobPartitionedInfo>(requestContext, ChunkDedupLogicalSizeJob.ChunkLogicalUsageInfoJobId, (Func<DedupLogicalDataJobPartitionedInfo, double>) (job => UsageInfoService.GetChunkLogicalStorageSizeByScope(ArtifactScopeType.NuGet, job.Result)), (Func<DedupLogicalDataJobPartitionedInfo, string>) (job => job.DomainId), maxTime) : this.FetchStorageMetric<DedupLogicalDataJobPartitionedInfo>(requestContext, ChunkDedupLogicalSizeJob.ChunkLogicalUsageInfoJobId, (Func<DedupLogicalDataJobPartitionedInfo, double>) (job => UsageInfoService.GetChunkLogicalStorageSizeByScope(ArtifactScopeType.NuGet, job.Result)), maxTime);
      StorageMetric storageMetric49 = this.IsMultiDomainEnabled ? this.AggregateMetricAcrossDomains<DedupLogicalDataJobPartitionedInfo>(requestContext, ChunkDedupLogicalSizeJob.ChunkLogicalUsageInfoJobId, (Func<DedupLogicalDataJobPartitionedInfo, double>) (job => UsageInfoService.GetChunkLogicalStorageSizeByScope(ArtifactScopeType.PyPi, job.Result)), (Func<DedupLogicalDataJobPartitionedInfo, string>) (job => job.DomainId), maxTime) : this.FetchStorageMetric<DedupLogicalDataJobPartitionedInfo>(requestContext, ChunkDedupLogicalSizeJob.ChunkLogicalUsageInfoJobId, (Func<DedupLogicalDataJobPartitionedInfo, double>) (job => UsageInfoService.GetChunkLogicalStorageSizeByScope(ArtifactScopeType.PyPi, job.Result)), maxTime);
      StorageMetric storageMetric50 = this.IsMultiDomainEnabled ? this.AggregateMetricAcrossDomains<DedupLogicalDataJobPartitionedInfo>(requestContext, ChunkDedupLogicalSizeJob.ChunkLogicalUsageInfoJobId, (Func<DedupLogicalDataJobPartitionedInfo, double>) (job => UsageInfoService.GetChunkLogicalStorageSizeByScope(ArtifactScopeType.Others, job.Result)), (Func<DedupLogicalDataJobPartitionedInfo, string>) (job => job.DomainId), maxTime) : this.FetchStorageMetric<DedupLogicalDataJobPartitionedInfo>(requestContext, ChunkDedupLogicalSizeJob.ChunkLogicalUsageInfoJobId, (Func<DedupLogicalDataJobPartitionedInfo, double>) (job => UsageInfoService.GetChunkLogicalStorageSizeByScope(ArtifactScopeType.Others, job.Result)), maxTime);
      StorageMetric storageMetric51 = this.IsMultiDomainEnabled ? this.AggregateMetricAcrossDomains<DedupLogicalDataJobPartitionedInfo>(requestContext, ChunkDedupLogicalSizeJob.ChunkLogicalUsageInfoJobId, (Func<DedupLogicalDataJobPartitionedInfo, double>) (job => UsageInfoService.GetChunkLogicalStorageSizeByScope(ArtifactScopeType.None, job.Result)), (Func<DedupLogicalDataJobPartitionedInfo, string>) (job => job.DomainId), maxTime) : this.FetchStorageMetric<DedupLogicalDataJobPartitionedInfo>(requestContext, ChunkDedupLogicalSizeJob.ChunkLogicalUsageInfoJobId, (Func<DedupLogicalDataJobPartitionedInfo, double>) (job => UsageInfoService.GetChunkLogicalStorageSizeByScope(ArtifactScopeType.None, job.Result)), maxTime);
      StorageMetric storageMetric52 = this.IsMultiDomainEnabled ? this.AggregateMetricAcrossDomains<DedupLogicalDataJobPartitionedInfo>(requestContext, ChunkDedupLogicalSizeJob.ChunkLogicalUsageInfoJobId, (Func<DedupLogicalDataJobPartitionedInfo, double>) (job => UsageInfoService.GetChunkLogicalStorageSizeByScope(ArtifactScopeType.BuildArtifacts, job.Result)), (Func<DedupLogicalDataJobPartitionedInfo, string>) (job => job.DomainId), maxTime) : this.FetchStorageMetric<DedupLogicalDataJobPartitionedInfo>(requestContext, ChunkDedupLogicalSizeJob.ChunkLogicalUsageInfoJobId, (Func<DedupLogicalDataJobPartitionedInfo, double>) (job => UsageInfoService.GetChunkLogicalStorageSizeByScope(ArtifactScopeType.BuildArtifacts, job.Result)), maxTime);
      StorageMetric storageMetric53 = this.IsMultiDomainEnabled ? this.AggregateMetricAcrossDomains<DedupLogicalDataJobPartitionedInfo>(requestContext, ChunkDedupLogicalSizeJob.ChunkLogicalUsageInfoJobId, (Func<DedupLogicalDataJobPartitionedInfo, double>) (job => UsageInfoService.GetChunkLogicalStorageSizeByScope(ArtifactScopeType.BuildLogs, job.Result)), (Func<DedupLogicalDataJobPartitionedInfo, string>) (job => job.DomainId), maxTime) : this.FetchStorageMetric<DedupLogicalDataJobPartitionedInfo>(requestContext, ChunkDedupLogicalSizeJob.ChunkLogicalUsageInfoJobId, (Func<DedupLogicalDataJobPartitionedInfo, double>) (job => UsageInfoService.GetChunkLogicalStorageSizeByScope(ArtifactScopeType.BuildLogs, job.Result)), maxTime);
      DataContracts.UsageInfo usageInfo = new DataContracts.UsageInfo();
      usageInfo.LogicalFileStorageSize = storageMetric1 != null ? storageMetric1.TotalBytes : 0.0;
      usageInfo.LogicalFileStorageNonExemptedSize = storageMetric1 != null ? storageMetric1.TotalBytes : 0.0;
      usageInfo.PhysicalFileStorageSize = storageMetric3 != null ? storageMetric3.TotalBytes : 0.0;
      usageInfo.LogicalChunkStorageSize = storageMetric2 != null ? storageMetric2.TotalBytes : 0.0;
      usageInfo.LogicalChunkStorageNonExemptedSize = storageMetric2 != null ? storageMetric2.TotalBytes : 0.0;
      usageInfo.PhysicalChunkStorageSize = storageMetric4 != null ? storageMetric4.TotalBytes : 0.0;
      DataContracts.LogicalFileUsageBreakdownInfo usageBreakdownInfo1 = new DataContracts.LogicalFileUsageBreakdownInfo();
      usageBreakdownInfo1.LogicalDropStorageSize = storageMetric6 != null ? storageMetric6.TotalBytes : 0.0;
      usageBreakdownInfo1.LogicalSymbolStorageSize = storageMetric7 != null ? storageMetric7.TotalBytes : 0.0;
      usageBreakdownInfo1.LogicalPipelineArtifactStorageSize = storageMetric8 != null ? storageMetric8.TotalBytes : 0.0;
      usageBreakdownInfo1.LogicalPipelineCacheStorageSize = storageMetric9 != null ? storageMetric9.TotalBytes : 0.0;
      double num1;
      if (storageMetric18 == null)
      {
        double? totalBytes = storageMetric19?.TotalBytes;
        num1 = (totalBytes.HasValue ? new double?(0.0 + totalBytes.GetValueOrDefault()) : new double?()).GetValueOrDefault();
      }
      else
        num1 = storageMetric18.TotalBytes;
      usageBreakdownInfo1.LogicalOthersStorageSize = num1;
      usageBreakdownInfo1.LogicalBuildArtifactsStorageSize = storageMetric20 != null ? storageMetric20.TotalBytes : 0.0;
      usageBreakdownInfo1.LogicalBuildLogsStorageSize = storageMetric21 != null ? storageMetric21.TotalBytes : 0.0;
      usageBreakdownInfo1.LogicalFilePackagingUsageBreakdownInfo = new DataContracts.LogicalFilePackagingUsageBreakdownInfo()
      {
        LogicalIvyStorageSize = storageMetric13 != null ? storageMetric13.TotalBytes : 0.0,
        LogicalMavenStorageSize = storageMetric14 != null ? storageMetric14.TotalBytes : 0.0,
        LogicalNpmStorageSize = storageMetric11 != null ? storageMetric11.TotalBytes : 0.0,
        LogicalNuGetStorageSize = storageMetric16 != null ? storageMetric16.TotalBytes : 0.0,
        LogicalPackagingStorageSize = storageMetric10 != null ? storageMetric10.TotalBytes : 0.0,
        LogicalPyPiStorageSize = storageMetric17 != null ? storageMetric17.TotalBytes : 0.0,
        LogicalCargoStorageSize = storageMetric15 != null ? storageMetric15.TotalBytes : 0.0,
        LogicalUPackStorageSize = storageMetric12 != null ? storageMetric12.TotalBytes : 0.0
      };
      usageInfo.LogicalFileUsageBreakdownInfo = usageBreakdownInfo1;
      DataContracts.PhysicalFileUsageBreakdownInfo usageBreakdownInfo2 = new DataContracts.PhysicalFileUsageBreakdownInfo();
      usageBreakdownInfo2.PhysicalDropStorageSize = storageMetric22 != null ? storageMetric22.TotalBytes : 0.0;
      usageBreakdownInfo2.PhysicalSymbolStorageSize = storageMetric23 != null ? storageMetric23.TotalBytes : 0.0;
      usageBreakdownInfo2.PhysicalPipelineArtifactStorageSize = storageMetric24 != null ? storageMetric24.TotalBytes : 0.0;
      usageBreakdownInfo2.PhysicalPipelineCacheStorageSize = storageMetric25 != null ? storageMetric25.TotalBytes : 0.0;
      double num2;
      if (storageMetric34 == null)
      {
        double? totalBytes = storageMetric35?.TotalBytes;
        num2 = (totalBytes.HasValue ? new double?(0.0 + totalBytes.GetValueOrDefault()) : new double?()).GetValueOrDefault();
      }
      else
        num2 = storageMetric34.TotalBytes;
      usageBreakdownInfo2.PhysicalOthersStorageSize = num2;
      usageBreakdownInfo2.PhysicalBuildArtifactsStorageSize = storageMetric36 != null ? storageMetric36.TotalBytes : 0.0;
      usageBreakdownInfo2.PhysicalBuildLogsStorageSize = storageMetric37 != null ? storageMetric37.TotalBytes : 0.0;
      usageBreakdownInfo2.PhysicalFilePackagingUsageBreakdownInfo = new DataContracts.PhysicalFilePackagingUsageBreakdownInfo()
      {
        PhysicalIvyStorageSize = storageMetric29 != null ? storageMetric29.TotalBytes : 0.0,
        PhysicalMavenStorageSize = storageMetric30 != null ? storageMetric30.TotalBytes : 0.0,
        PhysicalNpmStorageSize = storageMetric27 != null ? storageMetric27.TotalBytes : 0.0,
        PhysicalNuGetStorageSize = storageMetric32 != null ? storageMetric32.TotalBytes : 0.0,
        PhysicalPackagingStorageSize = storageMetric26 != null ? storageMetric26.TotalBytes : 0.0,
        PhysicalPyPiStorageSize = storageMetric33 != null ? storageMetric33.TotalBytes : 0.0,
        PhysicalCargoStorageSize = storageMetric31 != null ? storageMetric31.TotalBytes : 0.0,
        PhysicalUPackStorageSize = storageMetric28 != null ? storageMetric28.TotalBytes : 0.0
      };
      usageInfo.PhysicalFileUsageBreakdownInfo = usageBreakdownInfo2;
      DataContracts.LogicalChunkUsageBreakdownInfo usageBreakdownInfo3 = new DataContracts.LogicalChunkUsageBreakdownInfo();
      usageBreakdownInfo3.LogicalDropStorageSize = storageMetric38 != null ? storageMetric38.TotalBytes : 0.0;
      usageBreakdownInfo3.LogicalSymbolStorageSize = storageMetric39 != null ? storageMetric39.TotalBytes : 0.0;
      usageBreakdownInfo3.LogicalPipelineArtifactStorageSize = storageMetric40 != null ? storageMetric40.TotalBytes : 0.0;
      usageBreakdownInfo3.LogicalPipelineCacheStorageSize = storageMetric41 != null ? storageMetric41.TotalBytes : 0.0;
      double num3;
      if (storageMetric50 == null)
      {
        double? totalBytes = storageMetric51?.TotalBytes;
        num3 = (totalBytes.HasValue ? new double?(0.0 + totalBytes.GetValueOrDefault()) : new double?()).GetValueOrDefault();
      }
      else
        num3 = storageMetric50.TotalBytes;
      usageBreakdownInfo3.LogicalOthersStorageSize = num3;
      usageBreakdownInfo3.LogicalBuildArtifactsStorageSize = storageMetric52 != null ? storageMetric52.TotalBytes : 0.0;
      usageBreakdownInfo3.LogicalBuildLogsStorageSize = storageMetric53 != null ? storageMetric53.TotalBytes : 0.0;
      usageBreakdownInfo3.LogicalChunkPackagingUsageBreakdownInfo = new DataContracts.LogicalChunkPackagingUsageBreakdownInfo()
      {
        LogicalIvyStorageSize = storageMetric45 != null ? storageMetric45.TotalBytes : 0.0,
        LogicalMavenStorageSize = storageMetric46 != null ? storageMetric46.TotalBytes : 0.0,
        LogicalNpmStorageSize = storageMetric43 != null ? storageMetric43.TotalBytes : 0.0,
        LogicalNuGetStorageSize = storageMetric48 != null ? storageMetric48.TotalBytes : 0.0,
        LogicalPackagingStorageSize = storageMetric42 != null ? storageMetric42.TotalBytes : 0.0,
        LogicalPyPiStorageSize = storageMetric49 != null ? storageMetric49.TotalBytes : 0.0,
        LogicalCargoStorageSize = storageMetric47 != null ? storageMetric47.TotalBytes : 0.0,
        LogicalUPackStorageSize = storageMetric44 != null ? storageMetric44.TotalBytes : 0.0
      };
      usageInfo.LogicalChunkUsageBreakdownInfo = usageBreakdownInfo3;
      usageInfo.PhysicalChunkUsageBreakdownInfo = new DataContracts.PhysicalChunkUsageBreakdownInfo()
      {
        PhysicalDropStorageSize = 0.0,
        PhysicalSymbolStorageSize = 0.0,
        PhysicalPipelineArtifactStorageSize = 0.0,
        PhysicalPipelineCacheStorageSize = 0.0,
        PhysicalOthersStorageSize = 0.0,
        PhysicalBuildArtifactsStorageSize = 0.0,
        PhysicalBuildLogsStorageSize = 0.0,
        PhysicalChunkPackagingUsageBreakdownInfo = new DataContracts.PhysicalChunkPackagingUsageBreakdownInfo()
        {
          PhysicalIvyStorageSize = 0.0,
          PhysicalMavenStorageSize = 0.0,
          PhysicalNpmStorageSize = 0.0,
          PhysicalNuGetStorageSize = 0.0,
          PhysicalPackagingStorageSize = 0.0,
          PhysicalPyPiStorageSize = 0.0,
          PhysicalCargoStorageSize = 0.0,
          PhysicalUPackStorageSize = 0.0
        }
      };
      DataContracts.UsageInfo result = usageInfo;
      if (!this.IsHostInternal)
      {
        result.LogicalFileStorageSize -= result.LogicalFileUsageBreakdownInfo.LogicalSymbolStorageSize;
        result.LogicalChunkStorageSize -= result.LogicalChunkUsageBreakdownInfo.LogicalSymbolStorageSize;
      }
      if (this.IsHostInternal && result.LogicalChunkUsageBreakdownInfo.LogicalBuildArtifactsStorageSize > 0.0)
        result.LogicalChunkStorageSize -= result.LogicalChunkUsageBreakdownInfo.LogicalBuildArtifactsStorageSize;
      if (result.LogicalFileUsageBreakdownInfo.LogicalBuildArtifactsStorageSize > 0.0)
        result.LogicalFileStorageSize -= result.LogicalFileUsageBreakdownInfo.LogicalBuildArtifactsStorageSize;
      if (this.IsHostInternal && result.LogicalChunkUsageBreakdownInfo.LogicalBuildLogsStorageSize > 0.0)
        result.LogicalChunkStorageSize -= result.LogicalChunkUsageBreakdownInfo.LogicalBuildLogsStorageSize;
      if (result.LogicalFileUsageBreakdownInfo.LogicalBuildLogsStorageSize > 0.0)
        result.LogicalFileStorageSize -= result.LogicalFileUsageBreakdownInfo.LogicalBuildLogsStorageSize;
      if (!requestContext.IsFeatureEnabled("BlobStore.Features.EnableCargoBilling"))
      {
        if (result.LogicalFileUsageBreakdownInfo.LogicalFilePackagingUsageBreakdownInfo.LogicalCargoStorageSize > 0.0)
          result.LogicalFileStorageSize -= result.LogicalFileUsageBreakdownInfo.LogicalFilePackagingUsageBreakdownInfo.LogicalCargoStorageSize;
        if (result.PhysicalFileUsageBreakdownInfo.PhysicalFilePackagingUsageBreakdownInfo.PhysicalCargoStorageSize > 0.0)
          result.PhysicalFileStorageSize -= result.PhysicalFileUsageBreakdownInfo.PhysicalFilePackagingUsageBreakdownInfo.PhysicalCargoStorageSize;
      }
      return Task.FromResult<DataContracts.UsageInfo>(result);
    }

    public Task<DataContracts.RawStorageBreakdownInfo> GetStorageBreakdownUsageInfo(
      IVssRequestContext requestContext,
      DateTimeOffset maxTime)
    {
      DataContracts.UsageInfo result = this.GetStorageUsageInfo(requestContext, maxTime).Result;
      return Task.FromResult<DataContracts.RawStorageBreakdownInfo>(new DataContracts.RawStorageBreakdownInfo()
      {
        LogicalDropStorageSize = result.LogicalFileUsageBreakdownInfo.LogicalDropStorageSize + result.LogicalChunkUsageBreakdownInfo.LogicalDropStorageSize,
        LogicalSymbolStorageSize = result.LogicalFileUsageBreakdownInfo.LogicalSymbolStorageSize + result.LogicalChunkUsageBreakdownInfo.LogicalSymbolStorageSize,
        LogicalPipelineArtifactStorageSize = result.LogicalFileUsageBreakdownInfo.LogicalPipelineArtifactStorageSize + result.LogicalChunkUsageBreakdownInfo.LogicalPipelineArtifactStorageSize,
        LogicalPipelineCacheStorageSize = result.LogicalFileUsageBreakdownInfo.LogicalPipelineCacheStorageSize + result.LogicalChunkUsageBreakdownInfo.LogicalPipelineCacheStorageSize,
        LogicalIvyStorageSize = result.LogicalFileUsageBreakdownInfo.LogicalFilePackagingUsageBreakdownInfo.LogicalIvyStorageSize + result.LogicalChunkUsageBreakdownInfo.LogicalChunkPackagingUsageBreakdownInfo.LogicalIvyStorageSize,
        LogicalMavenStorageSize = result.LogicalFileUsageBreakdownInfo.LogicalFilePackagingUsageBreakdownInfo.LogicalMavenStorageSize + result.LogicalChunkUsageBreakdownInfo.LogicalChunkPackagingUsageBreakdownInfo.LogicalMavenStorageSize,
        LogicalNpmStorageSize = result.LogicalFileUsageBreakdownInfo.LogicalFilePackagingUsageBreakdownInfo.LogicalNpmStorageSize + result.LogicalChunkUsageBreakdownInfo.LogicalChunkPackagingUsageBreakdownInfo.LogicalNpmStorageSize,
        LogicalNuGetStorageSize = result.LogicalFileUsageBreakdownInfo.LogicalFilePackagingUsageBreakdownInfo.LogicalNuGetStorageSize + result.LogicalChunkUsageBreakdownInfo.LogicalChunkPackagingUsageBreakdownInfo.LogicalNuGetStorageSize,
        LogicalPackagingStorageSize = result.LogicalFileUsageBreakdownInfo.LogicalFilePackagingUsageBreakdownInfo.LogicalPackagingStorageSize + result.LogicalChunkUsageBreakdownInfo.LogicalChunkPackagingUsageBreakdownInfo.LogicalPackagingStorageSize,
        LogicalPyPiStorageSize = result.LogicalFileUsageBreakdownInfo.LogicalFilePackagingUsageBreakdownInfo.LogicalPyPiStorageSize + result.LogicalChunkUsageBreakdownInfo.LogicalChunkPackagingUsageBreakdownInfo.LogicalPyPiStorageSize,
        LogicalCargoStorageSize = result.LogicalFileUsageBreakdownInfo.LogicalFilePackagingUsageBreakdownInfo.LogicalCargoStorageSize + result.LogicalChunkUsageBreakdownInfo.LogicalChunkPackagingUsageBreakdownInfo.LogicalCargoStorageSize,
        LogicalUPackStorageSize = result.LogicalFileUsageBreakdownInfo.LogicalFilePackagingUsageBreakdownInfo.LogicalUPackStorageSize + result.LogicalChunkUsageBreakdownInfo.LogicalChunkPackagingUsageBreakdownInfo.LogicalUPackStorageSize,
        LogicalOthersStorageSize = result.LogicalFileUsageBreakdownInfo.LogicalOthersStorageSize + result.LogicalChunkUsageBreakdownInfo.LogicalOthersStorageSize,
        LogicalBuildArtifactsStorageSize = result.LogicalFileUsageBreakdownInfo.LogicalBuildArtifactsStorageSize + result.LogicalChunkUsageBreakdownInfo.LogicalBuildArtifactsStorageSize
      });
    }

    public Task<IDictionary<string, DataContracts.RawStorageBreakdownInfo>> GetStorageBreakdownUsageInfoByDomain(
      IVssRequestContext requestContext,
      DateTimeOffset maxTime)
    {
      this.breakdownByDomain = (IDictionary<string, DataContracts.RawStorageBreakdownInfo>) new Dictionary<string, DataContracts.RawStorageBreakdownInfo>();
      foreach (object obj in Enum.GetValues(typeof (ArtifactScopeType)))
      {
        object scope = obj;
        IDictionary<string, StorageMetric> scopeBasedUsageByDomain1 = this.FetchStorageMetricAcrossDomains<ParentFileStorageJobInfo>(requestContext, FileStorageSizeJob.FileStorageSizeJobId, (Func<ParentFileStorageJobInfo, double>) (job => UsageInfoService.GetLogicalStorageSizeByScope((ArtifactScopeType) scope, job.Result)), (Func<ParentFileStorageJobInfo, string>) (job => job.DomainId), maxTime);
        this.UpdateStorageBreakdown((ArtifactScopeType) scope, scopeBasedUsageByDomain1);
        IDictionary<string, StorageMetric> scopeBasedUsageByDomain2 = this.FetchStorageMetricAcrossDomains<DedupLogicalDataJobPartitionedInfo>(requestContext, ChunkDedupLogicalSizeJob.ChunkLogicalUsageInfoJobId, (Func<DedupLogicalDataJobPartitionedInfo, double>) (job => UsageInfoService.GetChunkLogicalStorageSizeByScope((ArtifactScopeType) scope, job.Result)), (Func<DedupLogicalDataJobPartitionedInfo, string>) (job => job.DomainId), maxTime);
        this.UpdateStorageBreakdown((ArtifactScopeType) scope, scopeBasedUsageByDomain2);
      }
      return Task.FromResult<IDictionary<string, DataContracts.RawStorageBreakdownInfo>>(this.breakdownByDomain);
    }

    public Task<FeedMetric> GetFeedLevelPackageUsageInfo(
      IVssRequestContext requestContext,
      DateTimeOffset maxTime)
    {
      FeedMetric fileUsageInfo = this.FetchPackageStorageMetricByFeed<FileVolumeByFeedAggregatedResult>(requestContext, (Func<FileVolumeByFeedAggregatedResult, ConcurrentDictionary<string, ulong>>) (result => result.LogicalSizeByFeed), maxTime, FileVolumeByFeedAggregatorJob.JobId, "LogicalSizeByFeed");
      FeedMetric chunkUsageInfo = this.FetchPackageStorageMetricByFeed<ChunkVolumeByFeedAggregatedResult>(requestContext, (Func<ChunkVolumeByFeedAggregatedResult, ConcurrentDictionary<string, ulong>>) (result => result.LogicalSizeByFeed), maxTime, ChunkVolumeByFeedAggregatorJob.JobId, "LogicalSizeByFeed");
      return Task.FromResult<FeedMetric>(this.GetAggregatedFeedLevelPackageUsageInfo(requestContext, fileUsageInfo, chunkUsageInfo));
    }

    public void ServiceEnd(IVssRequestContext systemRequestContext)
    {
    }

    public void ServiceStart(IVssRequestContext systemRequestContext)
    {
    }

    private void FetchConfig(IVssRequestContext requestContext)
    {
      IVssRegistryService service = requestContext.GetService<IVssRegistryService>();
      this.IsHostInternal = BlobStoreUtils.IsInternalHost(requestContext);
      this.IsMultiDomainEnabled = UsageInfoServiceExtensions.IsBillingEnabledForDomainsAsync(requestContext);
      this.UsageMultiplier = service.GetValue<double>(requestContext, (RegistryQuery) (this.RegistryBasePath + "/UsageMultiplier"), true, 1.0);
      this.DedupMultiplier = service.GetValue<double>(requestContext, (RegistryQuery) (this.RegistryBasePath + "/DedupMultiplier"), true, 1.0);
      this.FileDedupMultiplier = service.GetValue<double>(requestContext, (RegistryQuery) (this.RegistryBasePath + this.FileDedupMultiplierRegKey), true, 1.0);
      this.ChunkDedupMultiplier = service.GetValue<double>(requestContext, (RegistryQuery) (this.RegistryBasePath + this.ChunkDedupMultiplierRegKey), true, 1.0);
    }

    private static double GetChunkLogicalStorageSizeByScope(
      ArtifactScopeType scopeType,
      DedupLogicalDataJobResult jobResult)
    {
      ulong num;
      return jobResult.SizeByScope.TryGetValue(scopeType, out num) ? (double) num : 0.0;
    }

    private static double GetChunkPhysicalStorageSizeByScope(
      ArtifactScopeType scopeType,
      DedupPhysicalStorageData jobResult)
    {
      throw new NotSupportedException("Chunk physical breakdown by scope is not supported by design.");
    }

    private static double GetLogicalStorageSizeByScope(
      ArtifactScopeType scopeType,
      FileStorageSizeJobResult jobResult)
    {
      long num;
      return !jobResult.LogicalSizeByScope.TryGetValue(scopeType, out num) ? 0.0 : (double) num;
    }

    private static double GetPhysicalStorageSizeByScope(
      ArtifactScopeType scopeType,
      FileStorageSizeJobResult jobResult)
    {
      long num;
      return !jobResult.PhysicalSizeByScope.TryGetValue(scopeType, out num) ? 0.0 : (double) num;
    }

    private void UpdateStorageBreakdown(
      ArtifactScopeType scope,
      IDictionary<string, StorageMetric> scopeBasedUsageByDomain)
    {
      foreach (KeyValuePair<string, StorageMetric> keyValuePair in (IEnumerable<KeyValuePair<string, StorageMetric>>) scopeBasedUsageByDomain)
      {
        DataContracts.RawStorageBreakdownInfo storageBreakdownInfo1 = (DataContracts.RawStorageBreakdownInfo) null;
        switch (scope)
        {
          case ArtifactScopeType.None:
            continue;
          case ArtifactScopeType.Others:
            if (this.breakdownByDomain.TryGetValue(keyValuePair.Key, out storageBreakdownInfo1))
            {
              DataContracts.RawStorageBreakdownInfo storageBreakdownInfo2 = storageBreakdownInfo1;
              double othersStorageSize = storageBreakdownInfo2.LogicalOthersStorageSize;
              StorageMetric storageMetric = keyValuePair.Value;
              double num = storageMetric != null ? storageMetric.TotalBytes : 0.0;
              storageBreakdownInfo2.LogicalOthersStorageSize = othersStorageSize + num;
              this.breakdownByDomain[keyValuePair.Key] = storageBreakdownInfo1;
              continue;
            }
            IDictionary<string, DataContracts.RawStorageBreakdownInfo> breakdownByDomain1 = this.breakdownByDomain;
            string key1 = keyValuePair.Key;
            DataContracts.RawStorageBreakdownInfo storageBreakdownInfo3 = new DataContracts.RawStorageBreakdownInfo();
            StorageMetric storageMetric1 = keyValuePair.Value;
            storageBreakdownInfo3.LogicalOthersStorageSize = storageMetric1 != null ? storageMetric1.TotalBytes : 0.0;
            breakdownByDomain1.Add(key1, storageBreakdownInfo3);
            continue;
          case ArtifactScopeType.Drop:
            if (this.breakdownByDomain.TryGetValue(keyValuePair.Key, out storageBreakdownInfo1))
            {
              DataContracts.RawStorageBreakdownInfo storageBreakdownInfo4 = storageBreakdownInfo1;
              double logicalDropStorageSize = storageBreakdownInfo4.LogicalDropStorageSize;
              StorageMetric storageMetric2 = keyValuePair.Value;
              double num = storageMetric2 != null ? storageMetric2.TotalBytes : 0.0;
              storageBreakdownInfo4.LogicalDropStorageSize = logicalDropStorageSize + num;
              this.breakdownByDomain[keyValuePair.Key] = storageBreakdownInfo1;
              continue;
            }
            IDictionary<string, DataContracts.RawStorageBreakdownInfo> breakdownByDomain2 = this.breakdownByDomain;
            string key2 = keyValuePair.Key;
            DataContracts.RawStorageBreakdownInfo storageBreakdownInfo5 = new DataContracts.RawStorageBreakdownInfo();
            StorageMetric storageMetric3 = keyValuePair.Value;
            storageBreakdownInfo5.LogicalDropStorageSize = storageMetric3 != null ? storageMetric3.TotalBytes : 0.0;
            breakdownByDomain2.Add(key2, storageBreakdownInfo5);
            continue;
          case ArtifactScopeType.Symbol:
            if (this.breakdownByDomain.TryGetValue(keyValuePair.Key, out storageBreakdownInfo1))
            {
              DataContracts.RawStorageBreakdownInfo storageBreakdownInfo6 = storageBreakdownInfo1;
              double symbolStorageSize = storageBreakdownInfo6.LogicalSymbolStorageSize;
              StorageMetric storageMetric4 = keyValuePair.Value;
              double num = storageMetric4 != null ? storageMetric4.TotalBytes : 0.0;
              storageBreakdownInfo6.LogicalSymbolStorageSize = symbolStorageSize + num;
              this.breakdownByDomain[keyValuePair.Key] = storageBreakdownInfo1;
              continue;
            }
            IDictionary<string, DataContracts.RawStorageBreakdownInfo> breakdownByDomain3 = this.breakdownByDomain;
            string key3 = keyValuePair.Key;
            DataContracts.RawStorageBreakdownInfo storageBreakdownInfo7 = new DataContracts.RawStorageBreakdownInfo();
            StorageMetric storageMetric5 = keyValuePair.Value;
            storageBreakdownInfo7.LogicalSymbolStorageSize = storageMetric5 != null ? storageMetric5.TotalBytes : 0.0;
            breakdownByDomain3.Add(key3, storageBreakdownInfo7);
            continue;
          case ArtifactScopeType.PipelineArtifact:
            if (this.breakdownByDomain.TryGetValue(keyValuePair.Key, out storageBreakdownInfo1))
            {
              DataContracts.RawStorageBreakdownInfo storageBreakdownInfo8 = storageBreakdownInfo1;
              double artifactStorageSize = storageBreakdownInfo8.LogicalPipelineArtifactStorageSize;
              StorageMetric storageMetric6 = keyValuePair.Value;
              double num = storageMetric6 != null ? storageMetric6.TotalBytes : 0.0;
              storageBreakdownInfo8.LogicalPipelineArtifactStorageSize = artifactStorageSize + num;
              this.breakdownByDomain[keyValuePair.Key] = storageBreakdownInfo1;
              continue;
            }
            IDictionary<string, DataContracts.RawStorageBreakdownInfo> breakdownByDomain4 = this.breakdownByDomain;
            string key4 = keyValuePair.Key;
            DataContracts.RawStorageBreakdownInfo storageBreakdownInfo9 = new DataContracts.RawStorageBreakdownInfo();
            StorageMetric storageMetric7 = keyValuePair.Value;
            storageBreakdownInfo9.LogicalPipelineArtifactStorageSize = storageMetric7 != null ? storageMetric7.TotalBytes : 0.0;
            breakdownByDomain4.Add(key4, storageBreakdownInfo9);
            continue;
          case ArtifactScopeType.PipelineCache:
            if (this.breakdownByDomain.TryGetValue(keyValuePair.Key, out storageBreakdownInfo1))
            {
              DataContracts.RawStorageBreakdownInfo storageBreakdownInfo10 = storageBreakdownInfo1;
              double cacheStorageSize = storageBreakdownInfo10.LogicalPipelineCacheStorageSize;
              StorageMetric storageMetric8 = keyValuePair.Value;
              double num = storageMetric8 != null ? storageMetric8.TotalBytes : 0.0;
              storageBreakdownInfo10.LogicalPipelineCacheStorageSize = cacheStorageSize + num;
              this.breakdownByDomain[keyValuePair.Key] = storageBreakdownInfo1;
              continue;
            }
            IDictionary<string, DataContracts.RawStorageBreakdownInfo> breakdownByDomain5 = this.breakdownByDomain;
            string key5 = keyValuePair.Key;
            DataContracts.RawStorageBreakdownInfo storageBreakdownInfo11 = new DataContracts.RawStorageBreakdownInfo();
            StorageMetric storageMetric9 = keyValuePair.Value;
            storageBreakdownInfo11.LogicalPipelineCacheStorageSize = storageMetric9 != null ? storageMetric9.TotalBytes : 0.0;
            breakdownByDomain5.Add(key5, storageBreakdownInfo11);
            continue;
          case ArtifactScopeType.Packaging:
            if (this.breakdownByDomain.TryGetValue(keyValuePair.Key, out storageBreakdownInfo1))
            {
              DataContracts.RawStorageBreakdownInfo storageBreakdownInfo12 = storageBreakdownInfo1;
              double packagingStorageSize = storageBreakdownInfo12.LogicalPackagingStorageSize;
              StorageMetric storageMetric10 = keyValuePair.Value;
              double num = storageMetric10 != null ? storageMetric10.TotalBytes : 0.0;
              storageBreakdownInfo12.LogicalPackagingStorageSize = packagingStorageSize + num;
              this.breakdownByDomain[keyValuePair.Key] = storageBreakdownInfo1;
              continue;
            }
            IDictionary<string, DataContracts.RawStorageBreakdownInfo> breakdownByDomain6 = this.breakdownByDomain;
            string key6 = keyValuePair.Key;
            DataContracts.RawStorageBreakdownInfo storageBreakdownInfo13 = new DataContracts.RawStorageBreakdownInfo();
            StorageMetric storageMetric11 = keyValuePair.Value;
            storageBreakdownInfo13.LogicalPackagingStorageSize = storageMetric11 != null ? storageMetric11.TotalBytes : 0.0;
            breakdownByDomain6.Add(key6, storageBreakdownInfo13);
            continue;
          case ArtifactScopeType.UPack:
            if (this.breakdownByDomain.TryGetValue(keyValuePair.Key, out storageBreakdownInfo1))
            {
              DataContracts.RawStorageBreakdownInfo storageBreakdownInfo14 = storageBreakdownInfo1;
              double upackStorageSize = storageBreakdownInfo14.LogicalUPackStorageSize;
              StorageMetric storageMetric12 = keyValuePair.Value;
              double num = storageMetric12 != null ? storageMetric12.TotalBytes : 0.0;
              storageBreakdownInfo14.LogicalUPackStorageSize = upackStorageSize + num;
              this.breakdownByDomain[keyValuePair.Key] = storageBreakdownInfo1;
              continue;
            }
            IDictionary<string, DataContracts.RawStorageBreakdownInfo> breakdownByDomain7 = this.breakdownByDomain;
            string key7 = keyValuePair.Key;
            DataContracts.RawStorageBreakdownInfo storageBreakdownInfo15 = new DataContracts.RawStorageBreakdownInfo();
            StorageMetric storageMetric13 = keyValuePair.Value;
            storageBreakdownInfo15.LogicalUPackStorageSize = storageMetric13 != null ? storageMetric13.TotalBytes : 0.0;
            breakdownByDomain7.Add(key7, storageBreakdownInfo15);
            continue;
          case ArtifactScopeType.Npm:
            if (this.breakdownByDomain.TryGetValue(keyValuePair.Key, out storageBreakdownInfo1))
            {
              DataContracts.RawStorageBreakdownInfo storageBreakdownInfo16 = storageBreakdownInfo1;
              double logicalNpmStorageSize = storageBreakdownInfo16.LogicalNpmStorageSize;
              StorageMetric storageMetric14 = keyValuePair.Value;
              double num = storageMetric14 != null ? storageMetric14.TotalBytes : 0.0;
              storageBreakdownInfo16.LogicalNpmStorageSize = logicalNpmStorageSize + num;
              this.breakdownByDomain[keyValuePair.Key] = storageBreakdownInfo1;
              continue;
            }
            IDictionary<string, DataContracts.RawStorageBreakdownInfo> breakdownByDomain8 = this.breakdownByDomain;
            string key8 = keyValuePair.Key;
            DataContracts.RawStorageBreakdownInfo storageBreakdownInfo17 = new DataContracts.RawStorageBreakdownInfo();
            StorageMetric storageMetric15 = keyValuePair.Value;
            storageBreakdownInfo17.LogicalNpmStorageSize = storageMetric15 != null ? storageMetric15.TotalBytes : 0.0;
            breakdownByDomain8.Add(key8, storageBreakdownInfo17);
            continue;
          case ArtifactScopeType.PyPi:
            if (this.breakdownByDomain.TryGetValue(keyValuePair.Key, out storageBreakdownInfo1))
            {
              DataContracts.RawStorageBreakdownInfo storageBreakdownInfo18 = storageBreakdownInfo1;
              double logicalPyPiStorageSize = storageBreakdownInfo18.LogicalPyPiStorageSize;
              StorageMetric storageMetric16 = keyValuePair.Value;
              double num = storageMetric16 != null ? storageMetric16.TotalBytes : 0.0;
              storageBreakdownInfo18.LogicalPyPiStorageSize = logicalPyPiStorageSize + num;
              this.breakdownByDomain[keyValuePair.Key] = storageBreakdownInfo1;
              continue;
            }
            IDictionary<string, DataContracts.RawStorageBreakdownInfo> breakdownByDomain9 = this.breakdownByDomain;
            string key9 = keyValuePair.Key;
            DataContracts.RawStorageBreakdownInfo storageBreakdownInfo19 = new DataContracts.RawStorageBreakdownInfo();
            StorageMetric storageMetric17 = keyValuePair.Value;
            storageBreakdownInfo19.LogicalPyPiStorageSize = storageMetric17 != null ? storageMetric17.TotalBytes : 0.0;
            breakdownByDomain9.Add(key9, storageBreakdownInfo19);
            continue;
          case ArtifactScopeType.NuGet:
            if (this.breakdownByDomain.TryGetValue(keyValuePair.Key, out storageBreakdownInfo1))
            {
              DataContracts.RawStorageBreakdownInfo storageBreakdownInfo20 = storageBreakdownInfo1;
              double nuGetStorageSize = storageBreakdownInfo20.LogicalNuGetStorageSize;
              StorageMetric storageMetric18 = keyValuePair.Value;
              double num = storageMetric18 != null ? storageMetric18.TotalBytes : 0.0;
              storageBreakdownInfo20.LogicalNuGetStorageSize = nuGetStorageSize + num;
              this.breakdownByDomain[keyValuePair.Key] = storageBreakdownInfo1;
              continue;
            }
            IDictionary<string, DataContracts.RawStorageBreakdownInfo> breakdownByDomain10 = this.breakdownByDomain;
            string key10 = keyValuePair.Key;
            DataContracts.RawStorageBreakdownInfo storageBreakdownInfo21 = new DataContracts.RawStorageBreakdownInfo();
            StorageMetric storageMetric19 = keyValuePair.Value;
            storageBreakdownInfo21.LogicalNuGetStorageSize = storageMetric19 != null ? storageMetric19.TotalBytes : 0.0;
            breakdownByDomain10.Add(key10, storageBreakdownInfo21);
            continue;
          case ArtifactScopeType.Ivy:
            if (this.breakdownByDomain.TryGetValue(keyValuePair.Key, out storageBreakdownInfo1))
            {
              DataContracts.RawStorageBreakdownInfo storageBreakdownInfo22 = storageBreakdownInfo1;
              double logicalIvyStorageSize = storageBreakdownInfo22.LogicalIvyStorageSize;
              StorageMetric storageMetric20 = keyValuePair.Value;
              double num = storageMetric20 != null ? storageMetric20.TotalBytes : 0.0;
              storageBreakdownInfo22.LogicalIvyStorageSize = logicalIvyStorageSize + num;
              this.breakdownByDomain[keyValuePair.Key] = storageBreakdownInfo1;
              continue;
            }
            IDictionary<string, DataContracts.RawStorageBreakdownInfo> breakdownByDomain11 = this.breakdownByDomain;
            string key11 = keyValuePair.Key;
            DataContracts.RawStorageBreakdownInfo storageBreakdownInfo23 = new DataContracts.RawStorageBreakdownInfo();
            StorageMetric storageMetric21 = keyValuePair.Value;
            storageBreakdownInfo23.LogicalIvyStorageSize = storageMetric21 != null ? storageMetric21.TotalBytes : 0.0;
            breakdownByDomain11.Add(key11, storageBreakdownInfo23);
            continue;
          case ArtifactScopeType.Maven:
            if (this.breakdownByDomain.TryGetValue(keyValuePair.Key, out storageBreakdownInfo1))
            {
              DataContracts.RawStorageBreakdownInfo storageBreakdownInfo24 = storageBreakdownInfo1;
              double mavenStorageSize = storageBreakdownInfo24.LogicalMavenStorageSize;
              StorageMetric storageMetric22 = keyValuePair.Value;
              double num = storageMetric22 != null ? storageMetric22.TotalBytes : 0.0;
              storageBreakdownInfo24.LogicalMavenStorageSize = mavenStorageSize + num;
              this.breakdownByDomain[keyValuePair.Key] = storageBreakdownInfo1;
              continue;
            }
            IDictionary<string, DataContracts.RawStorageBreakdownInfo> breakdownByDomain12 = this.breakdownByDomain;
            string key12 = keyValuePair.Key;
            DataContracts.RawStorageBreakdownInfo storageBreakdownInfo25 = new DataContracts.RawStorageBreakdownInfo();
            StorageMetric storageMetric23 = keyValuePair.Value;
            storageBreakdownInfo25.LogicalMavenStorageSize = storageMetric23 != null ? storageMetric23.TotalBytes : 0.0;
            breakdownByDomain12.Add(key12, storageBreakdownInfo25);
            continue;
          case ArtifactScopeType.Cargo:
            if (this.breakdownByDomain.TryGetValue(keyValuePair.Key, out storageBreakdownInfo1))
            {
              DataContracts.RawStorageBreakdownInfo storageBreakdownInfo26 = storageBreakdownInfo1;
              double cargoStorageSize = storageBreakdownInfo26.LogicalCargoStorageSize;
              StorageMetric storageMetric24 = keyValuePair.Value;
              double num = storageMetric24 != null ? storageMetric24.TotalBytes : 0.0;
              storageBreakdownInfo26.LogicalCargoStorageSize = cargoStorageSize + num;
              this.breakdownByDomain[keyValuePair.Key] = storageBreakdownInfo1;
              continue;
            }
            IDictionary<string, DataContracts.RawStorageBreakdownInfo> breakdownByDomain13 = this.breakdownByDomain;
            string key13 = keyValuePair.Key;
            DataContracts.RawStorageBreakdownInfo storageBreakdownInfo27 = new DataContracts.RawStorageBreakdownInfo();
            StorageMetric storageMetric25 = keyValuePair.Value;
            storageBreakdownInfo27.LogicalCargoStorageSize = storageMetric25 != null ? storageMetric25.TotalBytes : 0.0;
            breakdownByDomain13.Add(key13, storageBreakdownInfo27);
            continue;
          case ArtifactScopeType.BuildArtifacts:
            if (this.breakdownByDomain.TryGetValue(keyValuePair.Key, out storageBreakdownInfo1))
            {
              DataContracts.RawStorageBreakdownInfo storageBreakdownInfo28 = storageBreakdownInfo1;
              double artifactsStorageSize = storageBreakdownInfo28.LogicalBuildArtifactsStorageSize;
              StorageMetric storageMetric26 = keyValuePair.Value;
              double num = storageMetric26 != null ? storageMetric26.TotalBytes : 0.0;
              storageBreakdownInfo28.LogicalBuildArtifactsStorageSize = artifactsStorageSize + num;
              this.breakdownByDomain[keyValuePair.Key] = storageBreakdownInfo1;
              continue;
            }
            IDictionary<string, DataContracts.RawStorageBreakdownInfo> breakdownByDomain14 = this.breakdownByDomain;
            string key14 = keyValuePair.Key;
            DataContracts.RawStorageBreakdownInfo storageBreakdownInfo29 = new DataContracts.RawStorageBreakdownInfo();
            StorageMetric storageMetric27 = keyValuePair.Value;
            storageBreakdownInfo29.LogicalBuildArtifactsStorageSize = storageMetric27 != null ? storageMetric27.TotalBytes : 0.0;
            breakdownByDomain14.Add(key14, storageBreakdownInfo29);
            continue;
          case ArtifactScopeType.BuildLogs:
            if (this.breakdownByDomain.TryGetValue(keyValuePair.Key, out storageBreakdownInfo1))
            {
              DataContracts.RawStorageBreakdownInfo storageBreakdownInfo30 = storageBreakdownInfo1;
              double buildLogsStorageSize = storageBreakdownInfo30.LogicalBuildLogsStorageSize;
              StorageMetric storageMetric28 = keyValuePair.Value;
              double num = storageMetric28 != null ? storageMetric28.TotalBytes : 0.0;
              storageBreakdownInfo30.LogicalBuildLogsStorageSize = buildLogsStorageSize + num;
              this.breakdownByDomain[keyValuePair.Key] = storageBreakdownInfo1;
              continue;
            }
            IDictionary<string, DataContracts.RawStorageBreakdownInfo> breakdownByDomain15 = this.breakdownByDomain;
            string key15 = keyValuePair.Key;
            DataContracts.RawStorageBreakdownInfo storageBreakdownInfo31 = new DataContracts.RawStorageBreakdownInfo();
            StorageMetric storageMetric29 = keyValuePair.Value;
            storageBreakdownInfo31.LogicalBuildLogsStorageSize = storageMetric29 != null ? storageMetric29.TotalBytes : 0.0;
            breakdownByDomain15.Add(key15, storageBreakdownInfo31);
            continue;
          default:
            throw new InvalidEnumArgumentException(string.Format("Unknown scope {0} encountered.", (object) scope));
        }
      }
    }

    private StorageMetric AggregateMetricAcrossDomains<T>(
      IVssRequestContext requestContext,
      Guid jobId,
      Func<T, double> getUsage,
      Func<T, string> getDomainId,
      DateTimeOffset maxTime)
    {
      StorageMetric storageMetric = new StorageMetric();
      foreach (KeyValuePair<string, StorageMetric> metricAcrossDomain in (IEnumerable<KeyValuePair<string, StorageMetric>>) this.FetchStorageMetricAcrossDomains<T>(requestContext, jobId, getUsage, getDomainId, maxTime))
      {
        if (DomainIdFactory.Create(metricAcrossDomain.Key).Equals(WellKnownDomainIds.DefaultDomainId))
          storageMetric.TotalBytes += metricAcrossDomain.Value.TotalBytes;
        else
          storageMetric.TotalBytes += metricAcrossDomain.Value.TotalBytes * this.UsageMultiplier * this.DedupMultiplier;
        storageMetric.TimeStamp = maxTime > storageMetric.TimeStamp ? maxTime : storageMetric.TimeStamp;
      }
      return storageMetric;
    }

    private IDictionary<string, StorageMetric> FetchStorageMetricAcrossDomains<T>(
      IVssRequestContext requestContext,
      Guid jobId,
      Func<T, double> getUsage,
      Func<T, string> getDomainId,
      DateTimeOffset maxTime)
    {
      IOrderedEnumerable<Tuple<DateTimeOffset, string>> source = UsageInfoServiceExtensions.GetJobResults(requestContext, jobId, maxTime, "TotalBytes").OrderByDescending<Tuple<DateTimeOffset, string>, DateTimeOffset>((Func<Tuple<DateTimeOffset, string>, DateTimeOffset>) (jr => jr.Item1));
      Dictionary<string, StorageMetric> enumerable = new Dictionary<string, StorageMetric>();
      foreach (Tuple<DateTimeOffset, string> tuple in (IEnumerable<Tuple<DateTimeOffset, string>>) source)
      {
        bool flag;
        try
        {
          foreach (T obj in JsonSerializer.Deserialize<List<T>>(tuple.Item2))
          {
            double num = getUsage(obj);
            string key = getDomainId(obj);
            enumerable.Add(key, new StorageMetric()
            {
              TimeStamp = tuple.Item1,
              TotalBytes = num,
              DomainId = key
            });
          }
          flag = true;
        }
        catch
        {
          requestContext.Trace(5701993, TraceLevel.Error, "BlobStore", nameof (UsageInfoService), string.Format("{0} couldn't retrieve metric bytes from JobId: {1}. Investigate the result message at: {2}", (object) nameof (FetchStorageMetricAcrossDomains), (object) jobId, (object) maxTime));
          continue;
        }
        if (flag)
          break;
      }
      if (enumerable.IsNullOrEmpty<KeyValuePair<string, StorageMetric>>())
      {
        int num = 0;
        if (source != null)
          num = source.Count<Tuple<DateTimeOffset, string>>();
        requestContext.Trace(5701993, TraceLevel.Error, "BlobStore", nameof (UsageInfoService), string.Format("{0} could not be deserialized from all {1} job results", (object) nameof (FetchStorageMetricAcrossDomains), (object) num));
      }
      return (IDictionary<string, StorageMetric>) enumerable;
    }

    private StorageMetric FetchStorageMetric<T>(
      IVssRequestContext requestContext,
      Guid jobId,
      Func<T, double> getUsage,
      DateTimeOffset maxTime)
    {
      IOrderedEnumerable<Tuple<DateTimeOffset, string>> source = UsageInfoServiceExtensions.GetJobResults(requestContext, jobId, maxTime, "TotalBytes").OrderByDescending<Tuple<DateTimeOffset, string>, DateTimeOffset>((Func<Tuple<DateTimeOffset, string>, DateTimeOffset>) (jr => jr.Item1));
      StorageMetric storageMetric = (StorageMetric) null;
      foreach (Tuple<DateTimeOffset, string> tuple in (IEnumerable<Tuple<DateTimeOffset, string>>) source)
      {
        try
        {
          T obj = JsonSerializer.Deserialize<T>(tuple.Item2);
          double num = getUsage(obj);
          storageMetric = new StorageMetric()
          {
            TimeStamp = tuple.Item1,
            TotalBytes = num
          };
        }
        catch
        {
          continue;
        }
        if (storageMetric != null)
          break;
      }
      if (storageMetric == null)
      {
        int num = 0;
        if (source != null)
          num = source.Count<Tuple<DateTimeOffset, string>>();
        requestContext.Trace(5701993, TraceLevel.Verbose, "BlobStore", nameof (UsageInfoService), string.Format("{0} could not be deserialized from all {1} job results", (object) "latestUsage", (object) num));
      }
      return storageMetric;
    }

    private FeedMetric FetchPackageStorageMetricByFeed<T>(
      IVssRequestContext requestContext,
      Func<T, ConcurrentDictionary<string, ulong>> getLogicalSizeByFeed,
      DateTimeOffset maxTime,
      Guid jobId,
      string resultFieldName)
    {
      IOrderedEnumerable<Tuple<DateTimeOffset, string>> source = UsageInfoServiceExtensions.GetJobResults(requestContext, jobId, maxTime, resultFieldName).OrderByDescending<Tuple<DateTimeOffset, string>, DateTimeOffset>((Func<Tuple<DateTimeOffset, string>, DateTimeOffset>) (jr => jr.Item1));
      FeedMetric feedMetric = (FeedMetric) null;
      foreach (Tuple<DateTimeOffset, string> tuple in (IEnumerable<Tuple<DateTimeOffset, string>>) source)
      {
        try
        {
          T obj = JsonSerializer.Deserialize<T>(tuple.Item2);
          ConcurrentDictionary<string, ulong> concurrentDictionary = getLogicalSizeByFeed(obj);
          feedMetric = new FeedMetric()
          {
            TimeStamp = tuple.Item1,
            LogicalSizeByFeed = concurrentDictionary
          };
        }
        catch
        {
          continue;
        }
        if (feedMetric != null)
          break;
      }
      if (feedMetric == null)
      {
        int num = 0;
        if (source != null)
          num = source.Count<Tuple<DateTimeOffset, string>>();
        requestContext.Trace(5701993, TraceLevel.Verbose, "BlobStore", nameof (UsageInfoService), string.Format("{0} could not be deserialized from all {1} job results", (object) "feedLevelPackageUsageInfo", (object) num));
      }
      return feedMetric;
    }

    private FeedMetric GetAggregatedFeedLevelPackageUsageInfo(
      IVssRequestContext requestContext,
      FeedMetric fileUsageInfo,
      FeedMetric chunkUsageInfo)
    {
      FeedMetric packageUsageInfo = new FeedMetric()
      {
        TimeStamp = this.GetFeedLevelPackageAggregatedUsageTimeStamp(fileUsageInfo, chunkUsageInfo)
      };
      if (fileUsageInfo != null)
        this.AddOrUpdateDictionary(requestContext, fileUsageInfo.LogicalSizeByFeed, packageUsageInfo.LogicalSizeByFeed, "FileStorageSizeJobResult");
      if (chunkUsageInfo != null)
        this.AddOrUpdateDictionary(requestContext, chunkUsageInfo.LogicalSizeByFeed, packageUsageInfo.LogicalSizeByFeed, "DedupLogicalDataJobResult");
      packageUsageInfo.LogicalSizeByFeed = UsageInfoServiceExtensions.GetTopLogicalSizeByFeed(packageUsageInfo.LogicalSizeByFeed);
      return packageUsageInfo;
    }

    private void AddOrUpdateDictionary(
      IVssRequestContext requestContext,
      ConcurrentDictionary<string, ulong> fromDict,
      ConcurrentDictionary<string, ulong> toDict,
      string jobResultName)
    {
      if (!fromDict.IsNullOrEmpty<KeyValuePair<string, ulong>>())
      {
        foreach (KeyValuePair<string, ulong> keyValuePair in fromDict)
        {
          KeyValuePair<string, ulong> kvp = keyValuePair;
          long num = (long) toDict.AddOrUpdate(kvp.Key, kvp.Value, (Func<string, ulong, ulong>) ((key, existing) => existing + kvp.Value));
        }
      }
      else
        requestContext.Trace(5701993, TraceLevel.Warning, "BlobStore", nameof (UsageInfoService), jobResultName + "'s LogicalSizeByFeed dictionary returned null or empty");
    }

    private DateTimeOffset GetFeedLevelPackageAggregatedUsageTimeStamp(
      FeedMetric fileInfo,
      FeedMetric chunkInfo)
    {
      if (fileInfo != null && chunkInfo != null)
        return this.GetMaxDate(fileInfo.TimeStamp, chunkInfo.TimeStamp);
      if (fileInfo != null)
        return fileInfo.TimeStamp;
      return chunkInfo != null ? chunkInfo.TimeStamp : DateTimeOffset.UtcNow;
    }
  }
}
