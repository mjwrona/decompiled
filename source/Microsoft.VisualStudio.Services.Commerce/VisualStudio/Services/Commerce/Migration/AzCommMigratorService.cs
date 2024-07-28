// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Commerce.Migration.AzCommMigratorService
// Assembly: Microsoft.VisualStudio.Services.Commerce, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1A903DA-646E-45AC-891B-7946BA20E824
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Commerce.dll

using Microsoft.TeamFoundation.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.AzComm.Migration.Rest.Contracts;
using Microsoft.VisualStudio.Services.AzComm.WebApi;
using Microsoft.VisualStudio.Services.Commerce.Migration.Managers;
using Microsoft.VisualStudio.Services.Commerce.Migration.Utilities;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Commerce.Migration
{
  internal class AzCommMigratorService : IMigratorService, IVssFrameworkService
  {
    private static readonly Guid UpdatedByIdentifier = Guid.Parse("88977C8C-E5AA-4533-8C2F-A46453E6C79A");
    private const string Layer = "AzCommMigratorService";
    private const string Area = "Commerce";

    public void MigrateData(IVssRequestContext requestContext, ServicingContext servicingContext)
    {
      servicingContext.LogInfo(string.Format("Executing {0} for host {1}", (object) nameof (MigrateData), (object) requestContext.ServiceHost.InstanceId));
      try
      {
        requestContext.CheckProjectCollectionRequestContext();
        if (!MigrationUtilities.ShouldMigrate(requestContext))
        {
          servicingContext.LogInfo(string.Format("Resource Migration for the host {0} is either disabled or completed.", (object) requestContext.ServiceHost.InstanceId));
        }
        else
        {
          this.PreMigrationSetup(requestContext);
          IEnumerable<Microsoft.VisualStudio.Services.Commerce.SubscriptionResourceUsage> resourceUsages = this.GetResourceUsages(requestContext);
          Microsoft.VisualStudio.Services.Commerce.AzureResourceAccount resourceAccount = this.GetResourceAccount(requestContext);
          if (!resourceUsages.IsNullOrEmpty<Microsoft.VisualStudio.Services.Commerce.SubscriptionResourceUsage>() || resourceAccount != null)
          {
            Microsoft.VisualStudio.Services.Commerce.AzureSubscriptionInternal subscriptionInternal = this.GetSubscriptionInternal(requestContext, resourceAccount);
            DataMigrationRequest request = MigrationPayloadManager.BuildMigrationRequest(resourceAccount, resourceUsages, subscriptionInternal, requestContext.ServiceHost.InstanceId);
            IVssRequestContext context = requestContext.To(TeamFoundationHostType.Deployment);
            context.GetClient<AzCommMigrationHttpClient>().MigrateResourcesAsync(request, (object) context.Elevate()).SyncResult();
          }
          this.PostMigrationSetup(requestContext);
        }
      }
      catch (Exception ex)
      {
        requestContext.TraceException(0, "Commerce", nameof (AzCommMigratorService), ex);
        servicingContext.LogInfo(string.Format("Resetting the Feature flags for the host {0}", (object) requestContext.ServiceHost.InstanceId));
        MigrationUtilities.ResetMigrationState(requestContext);
        throw;
      }
    }

    public void DualWriteResourceUsage(
      IVssRequestContext requestContext,
      IEnumerable<Microsoft.VisualStudio.Services.Commerce.SubscriptionResourceUsage> resourceUsages,
      Guid hostId)
    {
      requestContext.CheckProjectCollectionRequestContext();
      DualWriteSRURequest request = MigrationPayloadManager.BuildDualWriteSRURequest(resourceUsages, hostId);
      IVssRequestContext vssRequestContext = requestContext.Elevate().To(TeamFoundationHostType.Deployment);
      vssRequestContext.GetClient<AzCommMigrationHttpClient>().DualWriteOrgMeterResourcesAsync(request, (object) vssRequestContext).SyncResult();
    }

    public void DualWriteResourceAccounts(
      IVssRequestContext requestContext,
      Microsoft.VisualStudio.Services.Commerce.AzureResourceAccount resourceAccount,
      Guid hostId,
      bool isLinkOperation = true)
    {
      DualWriteARARequest request = MigrationPayloadManager.BuildDualWriteARARequest(resourceAccount, hostId, isLinkOperation);
      IVssRequestContext vssRequestContext = requestContext.Elevate().To(TeamFoundationHostType.Deployment);
      vssRequestContext.GetClient<AzCommMigrationHttpClient>().DualWriteOrgResourcesAsync(request, (object) vssRequestContext).SyncResult();
    }

    public void DualWriteSubscription(
      IVssRequestContext requestContext,
      Microsoft.VisualStudio.Services.Commerce.AzureSubscriptionInternal subscriptionInternal,
      Guid subscriptionId)
    {
      DualWriteSubscriptionRequest request = MigrationPayloadManager.BuildDualWriteSubscriptionRequest(subscriptionInternal, subscriptionId);
      IVssRequestContext vssRequestContext = requestContext.Elevate().To(TeamFoundationHostType.Deployment);
      vssRequestContext.GetClient<AzCommMigrationHttpClient>().DualWriteSubResourcesAsync(request, (object) vssRequestContext).SyncResult();
    }

    public void DualWriteAccountTags(
      IVssRequestContext requestContext,
      Guid organizationId,
      Dictionary<string, string> tags)
    {
      DualWriteTagsRequest request = new DualWriteTagsRequest()
      {
        HostId = organizationId,
        Tags = tags
      };
      IVssRequestContext vssRequestContext = requestContext.Elevate().To(TeamFoundationHostType.Deployment);
      vssRequestContext.GetClient<AzCommMigrationHttpClient>().DualWriteOrgTagsResourcesAsync(request, (object) vssRequestContext).SyncResult();
    }

    public void DualWriteDefaultAccessLevel(
      IVssRequestContext requestContext,
      Guid organizationId,
      int accessLevel)
    {
      DualWriteDefaultAccessLevelRequest request = new DualWriteDefaultAccessLevelRequest()
      {
        hostId = organizationId,
        accessLevel = accessLevel
      };
      IVssRequestContext vssRequestContext = requestContext.Elevate().To(TeamFoundationHostType.Deployment);
      vssRequestContext.GetClient<AzCommMigrationHttpClient>().DualWriteDefaultAccessLevelResourcesAsync(request, (object) vssRequestContext).SyncResult();
    }

    public void SetStaleOrganization(
      IVssRequestContext requestContext,
      Guid organizationId,
      IEnumerable<int> meterIds)
    {
      requestContext.CheckProjectCollectionRequestContext();
      IVssRequestContext vssRequestContext = requestContext.Elevate().To(TeamFoundationHostType.Deployment);
      StaleOrganizationRequest request = MigrationPayloadManager.BuildStaleOrganizationRequest(organizationId, meterIds);
      vssRequestContext.GetClient<AzCommMigrationHttpClient>().SetStaleOrganizationAsync(request, (object) vssRequestContext).SyncResult();
    }

    public void RemoveStaleOrganization(
      IVssRequestContext requestContext,
      Guid organizationId,
      IEnumerable<int> meterIds)
    {
      requestContext.CheckProjectCollectionRequestContext();
      IVssRequestContext vssRequestContext = requestContext.Elevate().To(TeamFoundationHostType.Deployment);
      StaleOrganizationRequest request = MigrationPayloadManager.BuildStaleOrganizationRequest(organizationId, meterIds);
      vssRequestContext.GetClient<AzCommMigrationHttpClient>().ClearStaleOrganizationAsync(request, (object) vssRequestContext).SyncResult();
    }

    public void ServiceEnd(IVssRequestContext systemRequestContext)
    {
    }

    public void ServiceStart(IVssRequestContext systemRequestContext)
    {
    }

    private IEnumerable<Microsoft.VisualStudio.Services.Commerce.SubscriptionResourceUsage> GetResourceUsages(
      IVssRequestContext requestContext)
    {
      return requestContext.GetService<PlatformSourceResourceMigrationService>().GetResourceUsages(requestContext, AzCommMigratorService.UpdatedByIdentifier);
    }

    private Microsoft.VisualStudio.Services.Commerce.AzureResourceAccount GetResourceAccount(
      IVssRequestContext requestContext)
    {
      IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Deployment);
      return vssRequestContext.GetService<PlatformSubscriptionService>().GetAzureResourceAccountByCollectionId(vssRequestContext, requestContext.ServiceHost.InstanceId);
    }

    private Microsoft.VisualStudio.Services.Commerce.AzureSubscriptionInternal GetSubscriptionInternal(
      IVssRequestContext requestContext,
      Microsoft.VisualStudio.Services.Commerce.AzureResourceAccount resourceAccount)
    {
      IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Deployment);
      PlatformSubscriptionService service = vssRequestContext.GetService<PlatformSubscriptionService>();
      return resourceAccount == null ? (Microsoft.VisualStudio.Services.Commerce.AzureSubscriptionInternal) null : service.GetAzureSubscription(vssRequestContext, resourceAccount.AzureSubscriptionId);
    }

    private void PreMigrationSetup(IVssRequestContext requestContext) => MigrationUtilities.SetMigrationInProgress(requestContext);

    private void PostMigrationSetup(IVssRequestContext requestContext)
    {
      MigrationUtilities.SetMigrationCompleted(requestContext);
      MigrationUtilities.ResetMigrationInProgress(requestContext);
    }
  }
}
