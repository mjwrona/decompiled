// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.HostAcquisition.FrameworkHostAcquisitionService
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Graph;
using Microsoft.VisualStudio.Services.HostAcquisition.Client;
using Microsoft.VisualStudio.Services.Identity;
using Microsoft.VisualStudio.Services.Organization;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace Microsoft.VisualStudio.Services.HostAcquisition
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  public class FrameworkHostAcquisitionService : IHostAcquisitionService, IVssFrameworkService
  {
    private Guid m_serviceHostId;
    private const string c_area = "HostAcquisition";
    private const string c_layer = "FrameworkHostAcquisitionService";
    private static readonly PerformanceTracer s_tracer = new PerformanceTracer(OrganizationPerfCounters.StandardSet, "HostAcquisition", nameof (FrameworkHostAcquisitionService));

    public void ServiceStart(IVssRequestContext context)
    {
      this.m_serviceHostId = context.ServiceHost.InstanceId;
      FrameworkHostAcquisitionService.ValidateRequestContext(context);
    }

    public void ServiceEnd(IVssRequestContext context)
    {
    }

    public CollectionRef CreateCollection(
      IVssRequestContext context,
      CollectionCreationContext creationContext)
    {
      FrameworkHostAcquisitionService.ValidateRequestContext(context);
      Microsoft.VisualStudio.Services.Organization.ArgumentValidator.ValidateCreateContext(creationContext);
      using (FrameworkHostAcquisitionService.s_tracer.TraceTimedAction(context, FrameworkHostAcquisitionService.TracePoints.CreateCollection.Slow, 5000, nameof (CreateCollection)))
        return FrameworkHostAcquisitionService.s_tracer.TraceAction<CollectionRef>(context, (ActionTracePoints) FrameworkHostAcquisitionService.TracePoints.CreateCollection, (Func<CollectionRef>) (() =>
        {
          SubjectDescriptor descriptorByStorageKey = context.GetService<IGraphIdentifierConversionService>().GetDescriptorByStorageKey(context, creationContext.OwnerId);
          if (descriptorByStorageKey == new SubjectDescriptor())
            throw new IdentityNotFoundException(creationContext.OwnerId);
          Microsoft.VisualStudio.Services.Organization.Client.Collection x1 = this.GetHttpClient(context).CreateCollectionAsync((IDictionary<string, string>) creationContext.Data.ToDictionary<KeyValuePair<string, object>, string, string>((Func<KeyValuePair<string, object>, string>) (x => x.Key), (Func<KeyValuePair<string, object>, string>) (x => x.Value.ToString())), creationContext.Name, creationContext.PreferredRegion, creationContext.PreferredGeography, descriptorByStorageKey.ToString()).SyncResult<Microsoft.VisualStudio.Services.Organization.Client.Collection>();
          FrameworkHostAcquisitionService.SyncHost(context, x1.Id);
          return x1.ToRef();
        }), nameof (CreateCollection));
    }

    public IEnumerable<Region> GetRegions(IVssRequestContext context)
    {
      FrameworkHostAcquisitionService.ValidateRequestContext(context);
      using (FrameworkHostAcquisitionService.s_tracer.TraceTimedAction(context, FrameworkHostAcquisitionService.TracePoints.GetRegions.Slow, 500, nameof (GetRegions)))
        return (IEnumerable<Region>) FrameworkHostAcquisitionService.s_tracer.TraceAction<List<Region>>(context, (ActionTracePoints) FrameworkHostAcquisitionService.TracePoints.GetRegions, (Func<List<Region>>) (() => this.GetHttpClient(context).GetRegionsAsync().SyncResult<List<Region>>()), nameof (GetRegions));
    }

    public IEnumerable<Geography> GetGeographies(IVssRequestContext context)
    {
      FrameworkHostAcquisitionService.ValidateRequestContext(context);
      using (FrameworkHostAcquisitionService.s_tracer.TraceTimedAction(context, FrameworkHostAcquisitionService.TracePoints.GetGeographies.Slow, 500, nameof (GetGeographies)))
        return (IEnumerable<Geography>) FrameworkHostAcquisitionService.s_tracer.TraceAction<List<Geography>>(context, (ActionTracePoints) FrameworkHostAcquisitionService.TracePoints.GetGeographies, (Func<List<Geography>>) (() => this.GetHttpClient(context).GetGeographiesAsync().SyncResult<List<Geography>>()), nameof (GetGeographies));
    }

    private static void ValidateRequestContext(IVssRequestContext context)
    {
      context.CheckHostedDeployment();
      context.CheckDeploymentRequestContext();
    }

    private HostAcquisitionHttpClient GetHttpClient(IVssRequestContext requestContext) => requestContext.GetClient<HostAcquisitionHttpClient>();

    private static void SyncHost(IVssRequestContext context, Guid hostId) => context.GetService<IHostSyncService>().EnsureHostUpdated(context, hostId);

    private static class TracePoints
    {
      internal static readonly TimedActionTracePoints CreateCollection = new TimedActionTracePoints(7620110, 7620117, 7620118, 7620119);
      internal static readonly TimedActionTracePoints GetRegions = new TimedActionTracePoints(7620120, 7620127, 7620128, 7620129);
      internal static readonly TimedActionTracePoints GetGeographies = new TimedActionTracePoints(7620130, 7620137, 7620138, 7620139);
    }
  }
}
