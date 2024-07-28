// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.BlobStore.Server.DeleteRetentionConfigurationJob
// Assembly: Microsoft.VisualStudio.Services.BlobStore.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: D3AB5C9B-EB54-4477-A304-63BB297414A3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.BlobStore.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.BlobStore.Common;
using Microsoft.VisualStudio.Services.BlobStore.Server.Common;
using Microsoft.VisualStudio.Services.BlobStore.Server.Domain;
using Microsoft.VisualStudio.Services.Content.Common;
using Microsoft.VisualStudio.Services.Content.Server.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.BlobStore.Server
{
  public class DeleteRetentionConfigurationJob : VssAsyncJobExtension
  {
    private const int SoftDeleteRetentionAccountingTracepoint = 5701991;
    private const string RunModeRegistryPath = "/RunMode";
    private const string RetentionInDaysRegistryPath = "/RetentionInDays";
    private const string EnableMultiDomainMode = "/EnableMultiDomain";
    private const int LegacyDomainDeleteRetentionInDays = 14;
    private const int EphemeralDomainDeleteRetentionInDays = 3;
    private const int DefaultDeleteRetentionInDays = 14;
    private readonly DeleteRetentionOperationMode DefaultRunMode;
    private readonly TraceData traceData = new TraceData()
    {
      Area = "BlobStore",
      Layer = "Job"
    };

    private string RegistryBasePath => "/Configuration/BlobStore/DeleteRetentionConfigurationJob";

    private int GetDefaultRetentionForDomain(IDomainId domainId) => !domainId.Equals(WellKnownDomainIds.DefaultDomainId) ? 3 : 14;

    public override async Task<VssJobResult> RunAsync(
      IVssRequestContext requestContext,
      TeamFoundationJobDefinition jobDefinition,
      DateTime queueTime)
    {
      List<DeleteRetentionConfigurationJobInfo> configurationJobInfoList = new List<DeleteRetentionConfigurationJobInfo>();
      VssJobResult vssJobResult;
      using (Microsoft.VisualStudio.Services.Content.Server.Common.Tracer tracer = Microsoft.VisualStudio.Services.Content.Server.Common.Tracer.Enter(requestContext, this.traceData, 5701991, nameof (RunAsync)))
      {
        AdminPlatformBlobStore adminBlobStore = requestContext.GetService<AdminPlatformBlobStore>();
        IVssRegistryService registryService = requestContext.To(TeamFoundationHostType.Deployment).GetService<IVssRegistryService>();
        int num1 = registryService.GetValue<bool>(requestContext, (RegistryQuery) (this.RegistryBasePath + "/EnableMultiDomain"), false) ? 1 : 0;
        AggregatedDeleteRetentionConfigurationJobInfo jobInfo = new AggregatedDeleteRetentionConfigurationJobInfo();
        DeleteRetentionConfigurationJobInfo jobSubInfo;
        if (num1 != 0)
        {
          IEnumerable<PhysicalDomainInfo> source = await this.GetDomains(requestContext).ConfigureAwait(false);
          jobInfo.TotalDomainsDiscovered = source != null ? source.Count<PhysicalDomainInfo>() : throw new Exception("Aborting. Multi-domain environment isn't provisioned but the job is enabled for multi-domain.");
          tracer.TraceAlways(string.Format("Discovered {0} domains. Domain Id(s): {1}.", (object) jobInfo.TotalDomainsDiscovered, (object) string.Join<IDomainId>(",", source.Select<PhysicalDomainInfo, IDomainId>((Func<PhysicalDomainInfo, IDomainId>) (dom => dom.DomainId)))));
          foreach ((PhysicalDomainInfo physicalDomainInfo, DeleteRetentionOperationMode retentionOperationMode, int num2) in source.Select(adminDomainInfo => new
          {
            adminDomainInfo = adminDomainInfo,
            runMode = registryService.GetValue<DeleteRetentionOperationMode>(requestContext, (RegistryQuery) string.Format("{0}/{1}{2}", (object) this.RegistryBasePath, (object) adminDomainInfo.DomainId, (object) "/RunMode"), this.DefaultRunMode)
          }).Select(_param1 => new
          {
            \u003C\u003Eh__TransparentIdentifier0 = _param1,
            deleteRetention = registryService.GetValue<int>(requestContext, (RegistryQuery) string.Format("{0}/{1}{2}", (object) this.RegistryBasePath, (object) _param1.adminDomainInfo.DomainId, (object) "/RetentionInDays"), this.GetDefaultRetentionForDomain(_param1.adminDomainInfo.DomainId))
          }).Select(_param1 => (_param1.\u003C\u003Eh__TransparentIdentifier0.adminDomainInfo, _param1.\u003C\u003Eh__TransparentIdentifier0.runMode, _param1.deleteRetention)))
          {
            jobSubInfo = new DeleteRetentionConfigurationJobInfo()
            {
              DomainId = physicalDomainInfo.DomainId.ToString(),
              JobRunMode = retentionOperationMode,
              DeleteRetentionRequestedInDays = retentionOperationMode == DeleteRetentionOperationMode.Disable ? 0 : num2
            };
            await adminBlobStore.ServiceDeleteRetentionOnStorageAccounts(requestContext, jobSubInfo, physicalDomainInfo).ConfigureAwait(true);
            ++jobInfo.TotalDomainsServiced;
            jobInfo.Results.Add(jobSubInfo);
            jobSubInfo = (DeleteRetentionConfigurationJobInfo) null;
          }
        }
        else
        {
          tracer.TraceAlways("Executing job in non-multi-domain mode. Only legacy accounts will be serviced.");
          DeleteRetentionOperationMode retentionOperationMode = registryService.GetValue<DeleteRetentionOperationMode>(requestContext, (RegistryQuery) (this.RegistryBasePath + "/RunMode"), this.DefaultRunMode);
          int num3 = registryService.GetValue<int>(requestContext, (RegistryQuery) (this.RegistryBasePath + "/RetentionInDays"), 14);
          jobSubInfo = new DeleteRetentionConfigurationJobInfo()
          {
            DomainId = "-1",
            JobRunMode = retentionOperationMode,
            DeleteRetentionRequestedInDays = retentionOperationMode == DeleteRetentionOperationMode.Disable ? 0 : num3
          };
          jobInfo.TotalDomainsDiscovered = 1;
          await adminBlobStore.ServiceDeleteRetentionOnStorageAccounts(requestContext, jobSubInfo).ConfigureAwait(true);
          ++jobInfo.TotalDomainsServiced;
          jobInfo.Results.Add(jobSubInfo);
          jobSubInfo = (DeleteRetentionConfigurationJobInfo) null;
        }
        vssJobResult = new VssJobResult(TeamFoundationJobExecutionResult.Succeeded, JsonSerializer.Serialize<AggregatedDeleteRetentionConfigurationJobInfo>(jobInfo));
      }
      return vssJobResult;
    }

    private async Task<IEnumerable<PhysicalDomainInfo>> GetDomains(IVssRequestContext requestContext) => !requestContext.AllowHostDomainAdminOperations() ? (IEnumerable<PhysicalDomainInfo>) null : await requestContext.GetService<AdminHostDomainStoreService>().GetPhysicalDomainsForOrganizationForAdminAsync(requestContext).ConfigureAwait(true);
  }
}
