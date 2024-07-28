// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Server.HostedParallelismService
// Assembly: Microsoft.TeamFoundation.DistributedTask.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: EDA28B25-3B93-49F7-A194-C85AAF98B83A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Server.dll

using Microsoft.TeamFoundation.DistributedTask.Orchestration.Server;
using Microsoft.TeamFoundation.DistributedTask.Server.DataAccess;
using Microsoft.TeamFoundation.DistributedTask.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Organization;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace Microsoft.TeamFoundation.DistributedTask.Server
{
  internal class HostedParallelismService : IHostedParallelismService, IVssFrameworkService
  {
    private static readonly DateTime PublicParallelismCutoff = new DateTime(2021, 3, 1);
    private static readonly DateTime PrivateParallelismCutoff = new DateTime(2021, 4, 1);

    public async Task<HostedParallelism> GetHostedParallelismAsync(IVssRequestContext requestContext)
    {
      HostedParallelism parallelismAsync;
      using (new MethodScope(requestContext, nameof (HostedParallelismService), nameof (GetHostedParallelismAsync)))
      {
        using (HostedParallelismComponent component = requestContext.CreateComponent<HostedParallelismComponent>())
          parallelismAsync = await component.GetHostedParallelismAsync();
      }
      return parallelismAsync;
    }

    public async Task<HostedParallelism> GetOrCreateHostedParallelismAsync(
      IVssRequestContext requestContext)
    {
      using (new MethodScope(requestContext, nameof (HostedParallelismService), nameof (GetOrCreateHostedParallelismAsync)))
      {
        HostedParallelism parallelismAsync1 = await this.GetHostedParallelismAsync(requestContext);
        if (parallelismAsync1 != null)
          return parallelismAsync1;
        HostedParallelism defaultParallelism = new HostedParallelism(HostedParallelismLevel.PaidOnly, HostedParallelismSource.Default);
        try
        {
          HostedParallelism parallelismAsync2 = this.NewHostedParallelism(requestContext);
          if (parallelismAsync2 != null)
          {
            using (HostedParallelismComponent component = requestContext.CreateComponent<HostedParallelismComponent>())
              parallelismAsync2 = await component.UpdateHostedParallelismAsync(parallelismAsync2.Level, parallelismAsync2.Source, parallelismAsync2.Description);
            requestContext.TraceAlways(10015230, TraceLevel.Info, "DistributedTask", nameof (HostedParallelismService), "Created initial HostedParallelism level {0} from source {1}", (object) parallelismAsync2.Level, (object) parallelismAsync2.Source);
            this.AuditParallelism(requestContext, parallelismAsync2.Level);
            return parallelismAsync2;
          }
        }
        catch (Exception ex)
        {
          requestContext.TraceException(nameof (HostedParallelismService), ex);
        }
        return defaultParallelism;
      }
    }

    public async Task<HostedParallelism> UpdateHostedParallelismAsync(
      IVssRequestContext requestContext,
      HostedParallelismLevel level,
      HostedParallelismSource source,
      string description = null)
    {
      HostedParallelism hostedParallelism;
      using (new MethodScope(requestContext, nameof (HostedParallelismService), nameof (UpdateHostedParallelismAsync)))
      {
        HostedParallelism parallelism;
        using (HostedParallelismComponent component = requestContext.CreateComponent<HostedParallelismComponent>())
          parallelism = await component.UpdateHostedParallelismAsync(level, source, description);
        requestContext.TraceAlways(10015231, TraceLevel.Info, "DistributedTask", nameof (HostedParallelismService), "Updated HostedParallelism level to {0} from source {1}", (object) parallelism.Level, (object) parallelism.Source);
        this.AuditParallelism(requestContext, parallelism.Level);
        if (parallelism.Level == HostedParallelismLevel.None)
        {
          IDistributedTaskResourceService resourceService = requestContext.GetService<IDistributedTaskResourceService>();
          IEnumerable<TaskAgentPool> taskAgentPools = (await resourceService.GetAgentPoolsAsync(requestContext)).Where<TaskAgentPool>((Func<TaskAgentPool, bool>) (p => p.IsHosted));
          requestContext.TraceInfo(nameof (HostedParallelismService), "Canceling hosted agent requests");
          foreach (TaskAgentPool pool in taskAgentPools)
          {
            IList<TaskAgentJobRequest> taskAgentJobRequestList = await resourceService.CancelAgentRequestsForPoolAsync(requestContext, pool.Id, TaskResources.NoHostedParallelismAvailable());
            requestContext.TraceInfo(nameof (HostedParallelismService), "{0} cancelations queued for pool {1} ('{2}')", (object) taskAgentJobRequestList.Count, (object) pool.Id, (object) pool.Name);
          }
          resourceService = (IDistributedTaskResourceService) null;
        }
        hostedParallelism = parallelism;
      }
      return hostedParallelism;
    }

    public void ServiceStart(IVssRequestContext systemRequestContext) => systemRequestContext.CheckProjectCollectionRequestContext();

    public void ServiceEnd(IVssRequestContext systemRequestContext)
    {
    }

    private void AuditParallelism(IVssRequestContext requestContext, HostedParallelismLevel level)
    {
      switch (level)
      {
        case HostedParallelismLevel.None:
          requestContext.LogAuditEvent("Pipelines.HostedParallelismZero", new Dictionary<string, object>());
          break;
        case HostedParallelismLevel.PaidOnly:
          requestContext.LogAuditEvent("Pipelines.HostedParallelismPaid", new Dictionary<string, object>());
          break;
        case HostedParallelismLevel.Private:
          requestContext.LogAuditEvent("Pipelines.HostedParallelismPrivate", new Dictionary<string, object>());
          break;
        case HostedParallelismLevel.Public:
          requestContext.LogAuditEvent("Pipelines.HostedParallelismPublic", new Dictionary<string, object>());
          break;
      }
    }

    private HostedParallelism NewHostedParallelism(IVssRequestContext requestContext)
    {
      IVssRequestContext context = requestContext.Elevate();
      Collection collection = context.GetService<ICollectionService>().GetCollection(context, (IEnumerable<string>) new string[2]
      {
        "SystemProperty.AssignmentStatus",
        "SystemProperty.CreationVerification"
      });
      if (collection == null)
      {
        requestContext.TraceAlways(10015234, TraceLevel.Info, "DistributedTask", nameof (HostedParallelismService), "Skipped lazy creation of HostedParallelism level due to missing collection");
        return (HostedParallelism) null;
      }
      AssignmentStatus assignmentStatus;
      if (collection.Properties.TryGetValue<AssignmentStatus>("SystemProperty.AssignmentStatus", out assignmentStatus) && assignmentStatus != AssignmentStatus.Assigned)
      {
        requestContext.TraceAlways(10015234, TraceLevel.Info, "DistributedTask", nameof (HostedParallelismService), "Skipped lazy creation of HostedParallelism level for collection with assigment status {0}", (object) assignmentStatus);
        return (HostedParallelism) null;
      }
      if (collection.DateCreated < HostedParallelismService.PublicParallelismCutoff)
      {
        requestContext.TraceAlways(10015237, TraceLevel.Info, "DistributedTask", nameof (HostedParallelismService), "Defaulting to Public parallelism level based on collection creation date {0}", (object) collection.DateCreated);
        return new HostedParallelism(HostedParallelismLevel.Public, HostedParallelismSource.Default);
      }
      if (collection.DateCreated >= HostedParallelismService.PublicParallelismCutoff && collection.DateCreated < HostedParallelismService.PrivateParallelismCutoff)
      {
        requestContext.TraceAlways(10015237, TraceLevel.Info, "DistributedTask", nameof (HostedParallelismService), "Defaulting to Private parallelism level based on collection creation date {0}", (object) collection.DateCreated);
        return new HostedParallelism(HostedParallelismLevel.Private, HostedParallelismSource.Default);
      }
      HostedParallelism hostedParallelism1 = this.TenantDefaultHostedParallelism(requestContext);
      if (hostedParallelism1 != null)
      {
        requestContext.TraceAlways(10015233, TraceLevel.Info, "DistributedTask", nameof (HostedParallelismService), "Using tenant default for initial HostedParallelism");
        return hostedParallelism1;
      }
      HostedParallelism hostedParallelism2 = new HostedParallelism(HostedParallelismLevel.PaidOnly, HostedParallelismSource.Default);
      if (requestContext.IsFeatureEnabled("DistributedTask.TrustedTenantFreeParallelism") || requestContext.IsFeatureEnabled("DistributedTask.AllowedListFreeParallelism") || requestContext.IsFeatureEnabled("DistributedTask.RiskEngineFreeParallelism"))
      {
        CollectionCreationVerification creationVerification;
        if (collection.Properties.TryGetValue<CollectionCreationVerification>("SystemProperty.CreationVerification", out creationVerification))
        {
          HostedParallelism hostedParallelism3 = new HostedParallelism(HostedParallelismLevel.PaidOnly, HostedParallelismSource.Sps);
          if (requestContext.IsFeatureEnabled("DistributedTask.TrustedTenantFreeParallelism") && creationVerification.HasFlag((Enum) CollectionCreationVerification.TrustedTenant) && hostedParallelism3.Level < HostedParallelismLevel.Private)
            hostedParallelism3.Level = HostedParallelismLevel.Private;
          if (requestContext.IsFeatureEnabled("DistributedTask.AllowedListFreeParallelism") && creationVerification.HasFlag((Enum) CollectionCreationVerification.AllowedList) && hostedParallelism3.Level < HostedParallelismLevel.Public)
            hostedParallelism3.Level = HostedParallelismLevel.Public;
          if (requestContext.IsFeatureEnabled("DistributedTask.RiskEngineFreeParallelism") && creationVerification.HasFlag((Enum) CollectionCreationVerification.RiskEngine) && hostedParallelism3.Level < HostedParallelismLevel.Private)
            hostedParallelism3.Level = HostedParallelismLevel.Private;
          if (hostedParallelism3.Level >= hostedParallelism2.Level)
          {
            hostedParallelism2 = hostedParallelism3;
            requestContext.TraceAlways(10015236, TraceLevel.Info, "DistributedTask", nameof (HostedParallelismService), "Using HostedParallelism level {0} based on verification value {1}", (object) hostedParallelism3.Level, (object) creationVerification);
          }
        }
        else
        {
          if (collection.DateCreated > DateTime.UtcNow.AddDays(-7.0))
          {
            requestContext.TraceAlways(10015234, TraceLevel.Info, "DistributedTask", nameof (HostedParallelismService), "Skipped lazy creation of HostedParallelism level for a collection without sign-up verification");
            return (HostedParallelism) null;
          }
          requestContext.TraceAlways(10015235, TraceLevel.Info, "DistributedTask", nameof (HostedParallelismService), "CreationVerification property not found on the collection. Using level {0} from source {1}.", (object) hostedParallelism2.Level, (object) hostedParallelism2.Source);
        }
      }
      return hostedParallelism2;
    }

    private HostedParallelism TenantDefaultHostedParallelism(IVssRequestContext requestContext)
    {
      IVssRegistryService service = requestContext.GetService<IVssRegistryService>();
      IVssRequestContext context = requestContext.To(TeamFoundationHostType.Application);
      Microsoft.VisualStudio.Services.Organization.Organization organization = context.GetService<IOrganizationService>().GetOrganization(context, (IEnumerable<string>) new List<string>());
      if (organization == null)
      {
        requestContext.TraceException(nameof (HostedParallelismService), new Exception("Organization lookup failed"));
        return (HostedParallelism) null;
      }
      HostedParallelismLevel result;
      return Enum.TryParse<HostedParallelismLevel>(service.GetValue(requestContext, (RegistryQuery) string.Format("{0}/{1}", (object) RegistryKeys.DefaultHostedParallelismKey, (object) organization.TenantId), true, ""), out result) ? new HostedParallelism(result, HostedParallelismSource.Default) : (HostedParallelism) null;
    }
  }
}
