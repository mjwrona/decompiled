// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Organization.FrameworkOrganizationService
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Organization.Client;
using Microsoft.VisualStudio.Services.WebApi;
using Microsoft.VisualStudio.Services.WebApi.Patch;
using Microsoft.VisualStudio.Services.WebApi.Patch.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading;

namespace Microsoft.VisualStudio.Services.Organization
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  internal class FrameworkOrganizationService : IOrganizationService, IVssFrameworkService
  {
    private Guid m_serviceHostId;
    private const string c_area = "Organization";
    private const string c_layer = "FrameworkOrganizationService";
    private static readonly PerformanceTracer s_tracer = new PerformanceTracer(OrganizationPerfCounters.StandardSet, "Organization", nameof (FrameworkOrganizationService));

    public void ServiceStart(IVssRequestContext context)
    {
      this.m_serviceHostId = context.ServiceHost.InstanceId;
      this.ValidateRequestContext(context);
    }

    public void ServiceEnd(IVssRequestContext context)
    {
    }

    public CollectionRef CreateCollection(
      IVssRequestContext context,
      CollectionCreationContext creationContext)
    {
      this.ValidateRequestContext(context);
      ArgumentValidator.ValidateCreateContext(creationContext);
      using (FrameworkOrganizationService.s_tracer.TraceTimedAction(context, FrameworkOrganizationService.TracePoints.CreateCollection.Slow, 5000, nameof (CreateCollection)))
        return FrameworkOrganizationService.s_tracer.TraceAction<CollectionRef>(context, (ActionTracePoints) FrameworkOrganizationService.TracePoints.CreateCollection, (Func<CollectionRef>) (() =>
        {
          Microsoft.VisualStudio.Services.Organization.Client.Collection x = this.GetOrganizationHttpClient(context).CreateCollectionAsync(creationContext.ToClient()).SyncResult<Microsoft.VisualStudio.Services.Organization.Client.Collection>();
          FrameworkOrganizationService.SyncHost(context, x.Id);
          OrganizationMessagePublisher.Publish(context, new OrganizationChangedData()
          {
            OrganizationId = context.ServiceHost.InstanceId
          }, ChangePublisherKind.SqlNotification);
          return x.ToRef();
        }), nameof (CreateCollection));
    }

    public IList<CollectionRef> GetCollections(IVssRequestContext context)
    {
      this.ValidateRequestContext(context);
      using (FrameworkOrganizationService.s_tracer.TraceTimedAction(context, FrameworkOrganizationService.TracePoints.GetCollections.Slow, actionName: nameof (GetCollections)))
        return (IList<CollectionRef>) FrameworkOrganizationService.s_tracer.TraceAction<List<CollectionRef>>(context, (ActionTracePoints) FrameworkOrganizationService.TracePoints.GetCollections, (Func<List<CollectionRef>>) (() => this.GetOrganization(context, (IEnumerable<string>) null).Collections.ToList<CollectionRef>()), nameof (GetCollections));
    }

    public bool DeleteCollection(
      IVssRequestContext context,
      Guid collectionId,
      Dictionary<string, object> deletionContext = null)
    {
      this.ValidateRequestContext(context);
      using (FrameworkOrganizationService.s_tracer.TraceTimedAction(context, FrameworkOrganizationService.TracePoints.DeleteCollection.Slow, 4000, nameof (DeleteCollection)))
        return FrameworkOrganizationService.s_tracer.TraceAction<bool>(context, (ActionTracePoints) FrameworkOrganizationService.TracePoints.DeleteCollection, (Func<bool>) (() =>
        {
          OrganizationHttpClient organizationHttpClient = this.GetOrganizationHttpClient(context);
          int? nullable = new int?();
          bool flag = false;
          Dictionary<string, object> dictionary1 = deletionContext;
          if (dictionary1 != null)
            dictionary1.TryGetValue<int?>("GracePeriod", out nullable);
          Dictionary<string, object> dictionary2 = deletionContext;
          if (dictionary2 != null)
            dictionary2.TryGetValue<bool>("ViolatedTerms", out flag);
          Guid collectionId1 = collectionId;
          int? gracePeriodToRestoreInHours = nullable;
          bool? violatedTerms = new bool?(flag);
          CancellationToken cancellationToken = new CancellationToken();
          int num = organizationHttpClient.DeleteCollectionAsync(collectionId1, gracePeriodToRestoreInHours, violatedTerms, cancellationToken: cancellationToken).SyncResult<bool>() ? 1 : 0;
          OrganizationMessagePublisher.Publish(context, new OrganizationChangedData()
          {
            OrganizationId = context.ServiceHost.InstanceId
          }, ChangePublisherKind.SqlNotification);
          return num != 0;
        }), nameof (DeleteCollection));
    }

    public bool RestoreCollection(
      IVssRequestContext context,
      Guid collectionId,
      string collectionName = null)
    {
      this.ValidateRequestContext(context);
      using (FrameworkOrganizationService.s_tracer.TraceTimedAction(context, FrameworkOrganizationService.TracePoints.RestoreCollection.Slow, 4000, nameof (RestoreCollection)))
        return FrameworkOrganizationService.s_tracer.TraceAction<bool>(context, (ActionTracePoints) FrameworkOrganizationService.TracePoints.RestoreCollection, (Func<bool>) (() =>
        {
          int num = this.GetOrganizationHttpClient(context).RestoreCollectionAsync(collectionId, collectionName).SyncResult<bool>() ? 1 : 0;
          OrganizationMessagePublisher.Publish(context, new OrganizationChangedData()
          {
            OrganizationId = context.ServiceHost.InstanceId
          }, ChangePublisherKind.SqlNotification);
          return num != 0;
        }), nameof (RestoreCollection));
    }

    public Microsoft.VisualStudio.Services.Organization.Organization GetOrganization(
      IVssRequestContext context,
      IEnumerable<string> propertyNames)
    {
      this.ValidateRequestContext(context);
      using (FrameworkOrganizationService.s_tracer.TraceTimedAction(context, FrameworkOrganizationService.TracePoints.GetOrganization.Slow, actionName: nameof (GetOrganization)))
        return FrameworkOrganizationService.s_tracer.TraceAction<Microsoft.VisualStudio.Services.Organization.Organization>(context, (ActionTracePoints) FrameworkOrganizationService.TracePoints.GetOrganization, (Func<Microsoft.VisualStudio.Services.Organization.Organization>) (() =>
        {
          Guid instanceId = context.ServiceHost.InstanceId;
          IVssRequestContext context1 = context.To(TeamFoundationHostType.Deployment);
          OrganizationCacheService service = context1.GetService<OrganizationCacheService>();
          Microsoft.VisualStudio.Services.Organization.Organization organization = service.Get(context1, instanceId, propertyNames);
          if (organization != null)
            return organization;
          HashSet<string> propertyNames1 = propertyNames.EnsureWellKnownOrganizationPropertiesExists();
          Microsoft.VisualStudio.Services.Organization.Organization server = this.GetOrganizationHttpClient(context).GetOrganizationAsync("Me", (IEnumerable<string>) propertyNames1).SyncResult<Microsoft.VisualStudio.Services.Organization.Client.Organization>().ToServer();
          service.Update(context1, server, (IEnumerable<string>) propertyNames1);
          return server;
        }), nameof (GetOrganization));
    }

    public bool RenameOrganization(IVssRequestContext context, string newName)
    {
      this.ValidateRequestContext(context);
      using (FrameworkOrganizationService.s_tracer.TraceTimedAction(context, FrameworkOrganizationService.TracePoints.RenameOrganization.Slow, 4000, nameof (RenameOrganization)))
      {
        ArgumentUtility.CheckStringForNullOrWhiteSpace(newName, nameof (newName));
        return FrameworkOrganizationService.s_tracer.TraceAction<bool>(context, (ActionTracePoints) FrameworkOrganizationService.TracePoints.RenameOrganization, (Func<bool>) (() =>
        {
          JsonPatchDocument patchDocument = JsonPatchDocumentHelper.ConstructJsonPatchDocument(Operation.Replace, "Name", (object) newName);
          Microsoft.VisualStudio.Services.Organization.Client.Organization organization = this.GetOrganizationHttpClient(context).UpdateOrganizationAsync(patchDocument, "Me").SyncResult<Microsoft.VisualStudio.Services.Organization.Client.Organization>();
          OrganizationMessagePublisher.Publish(context, new OrganizationChangedData()
          {
            OrganizationId = context.ServiceHost.InstanceId
          }, ChangePublisherKind.SqlNotification);
          return newName.Equals(organization.Name);
        }), nameof (RenameOrganization));
      }
    }

    public bool ActivateOrganization(IVssRequestContext context)
    {
      this.ValidateRequestContext(context);
      using (FrameworkOrganizationService.s_tracer.TraceTimedAction(context, FrameworkOrganizationService.TracePoints.ActivateOrganization.Slow, 4000, nameof (ActivateOrganization)))
        return FrameworkOrganizationService.s_tracer.TraceAction<bool>(context, (ActionTracePoints) FrameworkOrganizationService.TracePoints.ActivateOrganization, (Func<bool>) (() =>
        {
          JsonPatchDocument patchDocument = JsonPatchDocumentHelper.ConstructJsonPatchDocument(Operation.Replace, "IsActivated", (object) true.ToString());
          Microsoft.VisualStudio.Services.Organization.Client.Organization organization = this.GetOrganizationHttpClient(context).UpdateOrganizationAsync(patchDocument, "Me").SyncResult<Microsoft.VisualStudio.Services.Organization.Client.Organization>();
          OrganizationMessagePublisher.Publish(context, new OrganizationChangedData()
          {
            OrganizationId = context.ServiceHost.InstanceId
          }, ChangePublisherKind.SqlNotification);
          return organization.IsActivated;
        }), nameof (ActivateOrganization));
    }

    public bool UpdateProperties(IVssRequestContext context, PropertyBag properties)
    {
      this.ValidateRequestContext(context);
      using (FrameworkOrganizationService.s_tracer.TraceTimedAction(context, FrameworkOrganizationService.TracePoints.UpdateProperties.Slow, 2000, nameof (UpdateProperties)))
        return FrameworkOrganizationService.s_tracer.TraceAction<bool>(context, (ActionTracePoints) FrameworkOrganizationService.TracePoints.UpdateProperties, (Func<bool>) (() =>
        {
          if (properties == null || properties.UpdatedProperties.IsNullOrEmpty<KeyValuePair<string, object>>())
            return true;
          JsonPatchDocument jsonPatchDocument = JsonPatchDocumentHelper.ConstructJsonPatchDocument(Operation.Replace, (IDictionary<string, object>) properties.UpdatedProperties);
          OrganizationHttpClient organizationHttpClient = this.GetOrganizationHttpClient(context);
          Guid instanceId = context.ServiceHost.InstanceId;
          Guid organizationId = instanceId;
          JsonPatchDocument patchDocument = jsonPatchDocument;
          CancellationToken cancellationToken = new CancellationToken();
          int num = organizationHttpClient.UpdateOrganizationPropertiesAsync(organizationId, patchDocument, cancellationToken: cancellationToken).SyncResult<bool>() ? 1 : 0;
          OrganizationMessagePublisher.Publish(context, new OrganizationChangedData()
          {
            OrganizationId = instanceId
          }, ChangePublisherKind.SqlNotification);
          return num != 0;
        }), nameof (UpdateProperties));
    }

    public bool DeleteProperties(IVssRequestContext context, IEnumerable<string> propertyNames)
    {
      this.ValidateRequestContext(context);
      using (FrameworkOrganizationService.s_tracer.TraceTimedAction(context, FrameworkOrganizationService.TracePoints.DeleteProperties.Slow, 2000, nameof (DeleteProperties)))
        return FrameworkOrganizationService.s_tracer.TraceAction<bool>(context, (ActionTracePoints) FrameworkOrganizationService.TracePoints.DeleteProperties, (Func<bool>) (() =>
        {
          if (propertyNames.IsNullOrEmpty<string>())
            return true;
          JsonPatchDocument jsonPatchDocument = JsonPatchDocumentHelper.ConstructJsonPatchDocument(Operation.Remove, propertyNames);
          OrganizationHttpClient organizationHttpClient = this.GetOrganizationHttpClient(context);
          Guid instanceId = context.ServiceHost.InstanceId;
          Guid organizationId = instanceId;
          JsonPatchDocument patchDocument = jsonPatchDocument;
          CancellationToken cancellationToken = new CancellationToken();
          int num = organizationHttpClient.UpdateOrganizationPropertiesAsync(organizationId, patchDocument, cancellationToken: cancellationToken).SyncResult<bool>() ? 1 : 0;
          OrganizationMessagePublisher.Publish(context, new OrganizationChangedData()
          {
            OrganizationId = instanceId
          }, ChangePublisherKind.SqlNotification);
          return num != 0;
        }), nameof (DeleteProperties));
    }

    public bool UpdateLogo(IVssRequestContext context, Logo logo) => throw new NotImplementedException();

    private void ValidateRequestContext(IVssRequestContext context)
    {
      context.CheckHostedDeployment();
      context.CheckOrganizationOnlyRequestContext();
      context.CheckServiceHostId(this.m_serviceHostId, (IVssFrameworkService) this);
    }

    private OrganizationHttpClient GetOrganizationHttpClient(IVssRequestContext requestContext) => requestContext.GetClient<OrganizationHttpClient>();

    private static void SyncHost(IVssRequestContext context, Guid hostId)
    {
      IVssRequestContext vssRequestContext = context.To(TeamFoundationHostType.Deployment);
      vssRequestContext.GetService<IHostSyncService>().EnsureHostUpdated(vssRequestContext, hostId);
    }

    private static class TracePoints
    {
      internal static readonly TimedActionTracePoints CreateCollection = new TimedActionTracePoints(7720010, 7720017, 7720018, 7720019);
      internal static readonly TimedActionTracePoints GetCollections = new TimedActionTracePoints(7720030, 7720037, 7720038, 7720039);
      internal static readonly TimedActionTracePoints GetOrganization = new TimedActionTracePoints(7720040, 7720047, 7720048, 7720049);
      internal static readonly TimedActionTracePoints UpdateProperties = new TimedActionTracePoints(7720050, 7720057, 7720058, 7720059);
      internal static readonly TimedActionTracePoints DeleteProperties = new TimedActionTracePoints(7720060, 7720067, 7720068, 7720069);
      internal static readonly TimedActionTracePoints DeleteCollection = new TimedActionTracePoints(7720070, 7720077, 7720078, 7720079);
      internal static readonly TimedActionTracePoints RestoreCollection = new TimedActionTracePoints(7720080, 7720087, 7720088, 7720089);
      internal static readonly TimedActionTracePoints RenameOrganization = new TimedActionTracePoints(7720090, 7720097, 7720098, 7720099);
      internal static readonly TimedActionTracePoints ActivateOrganization = new TimedActionTracePoints(7720100, 7720107, 7720108, 7720109);
    }
  }
}
