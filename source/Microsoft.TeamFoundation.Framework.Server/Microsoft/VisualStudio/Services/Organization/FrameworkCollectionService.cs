// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Organization.FrameworkCollectionService
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
using System.Diagnostics;
using System.Threading;

namespace Microsoft.VisualStudio.Services.Organization
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  internal class FrameworkCollectionService : ICollectionService, IVssFrameworkService
  {
    private Guid m_serviceHostId;
    private const string c_area = "Organization";
    private const string c_layer = "FrameworkCollectionService";
    private const short c_forceUpdateReasonMaxLength = 256;
    private static readonly PerformanceTracer s_tracer = new PerformanceTracer(OrganizationPerfCounters.StandardSet, "Organization", nameof (FrameworkCollectionService));

    public void ServiceStart(IVssRequestContext context)
    {
      this.m_serviceHostId = context.ServiceHost.InstanceId;
      this.ValidateRequestContext(context);
    }

    public void ServiceEnd(IVssRequestContext context)
    {
    }

    public Collection GetCollection(IVssRequestContext context, IEnumerable<string> propertyNames)
    {
      this.ValidateRequestContext(context);
      using (FrameworkCollectionService.s_tracer.TraceTimedAction(context, FrameworkCollectionService.TracePoints.GetCollection.Slow, actionName: nameof (GetCollection)))
        return FrameworkCollectionService.s_tracer.TraceAction<Collection>(context, (ActionTracePoints) FrameworkCollectionService.TracePoints.GetCollection, (Func<Collection>) (() =>
        {
          Guid instanceId = context.ServiceHost.InstanceId;
          IVssRequestContext context1 = context.To(TeamFoundationHostType.Deployment);
          CollectionCacheService service = context1.GetService<CollectionCacheService>();
          Collection collection = service.Get(context1, instanceId, propertyNames);
          bool flag;
          if ((!context.Items.TryGetValue<bool>("Organization.Cache.Bypass", out flag) || !flag) && collection != null)
            return collection;
          OrganizationHttpClient organizationHttpClient = this.GetOrganizationHttpClient(context);
          HashSet<string> propertyNames1 = propertyNames.EnsureWellKnownCollectionPropertiesExists();
          HashSet<string> propertyNames2 = propertyNames1;
          CancellationToken cancellationToken = new CancellationToken();
          Collection server = organizationHttpClient.GetCollectionAsync("Me", (IEnumerable<string>) propertyNames2, cancellationToken: cancellationToken).SyncResult<Microsoft.VisualStudio.Services.Organization.Client.Collection>().ToServer();
          service.Update(context1, server, (IEnumerable<string>) propertyNames1);
          return server;
        }), nameof (GetCollection));
    }

    public bool UpdateCollectionOwner(IVssRequestContext context, Guid newOwnerId)
    {
      this.ValidateRequestContext(context);
      ArgumentUtility.CheckForEmptyGuid(newOwnerId, nameof (newOwnerId));
      using (FrameworkCollectionService.s_tracer.TraceTimedAction(context, FrameworkCollectionService.TracePoints.UpdateCollectionOwner.Slow, 2000, nameof (UpdateCollectionOwner)))
        return FrameworkCollectionService.s_tracer.TraceAction<bool>(context, (ActionTracePoints) FrameworkCollectionService.TracePoints.UpdateCollectionOwner, (Func<bool>) (() =>
        {
          JsonPatchDocument patchDocument = JsonPatchDocumentHelper.ConstructJsonPatchDocument(Operation.Replace, "Owner", (object) newOwnerId);
          Microsoft.VisualStudio.Services.Organization.Client.Collection collection = this.GetOrganizationHttpClient(context).UpdateCollectionAsync(patchDocument, "Me").SyncResult<Microsoft.VisualStudio.Services.Organization.Client.Collection>();
          OrganizationMessagePublisher.Publish(context, new AccountChangedData()
          {
            AccountId = context.ServiceHost.InstanceId
          }, ChangePublisherKind.SqlNotification);
          return collection.Owner == newOwnerId;
        }), nameof (UpdateCollectionOwner));
    }

    public bool RenameCollection(IVssRequestContext context, string newName)
    {
      this.ValidateRequestContext(context);
      ArgumentUtility.CheckStringForNullOrWhiteSpace(newName, nameof (newName));
      using (FrameworkCollectionService.s_tracer.TraceTimedAction(context, FrameworkCollectionService.TracePoints.RenameCollection.Slow, 5000, nameof (RenameCollection)))
        return FrameworkCollectionService.s_tracer.TraceAction<bool>(context, (ActionTracePoints) FrameworkCollectionService.TracePoints.RenameCollection, (Func<bool>) (() =>
        {
          JsonPatchDocument patchDocument = JsonPatchDocumentHelper.ConstructJsonPatchDocument(Operation.Replace, "Name", (object) newName);
          Microsoft.VisualStudio.Services.Organization.Client.Collection collection = this.GetOrganizationHttpClient(context).UpdateCollectionAsync(patchDocument, "Me").SyncResult<Microsoft.VisualStudio.Services.Organization.Client.Collection>();
          OrganizationMessagePublisher.Publish(context, new AccountChangedData()
          {
            AccountId = context.ServiceHost.InstanceId
          }, ChangePublisherKind.SqlNotification);
          return newName.Equals(collection.Name);
        }), nameof (RenameCollection));
    }

    public bool UpdateProperties(IVssRequestContext context, PropertyBag properties)
    {
      this.ValidateRequestContext(context);
      using (FrameworkCollectionService.s_tracer.TraceTimedAction(context, FrameworkCollectionService.TracePoints.UpdateProperties.Slow, 2000, nameof (UpdateProperties)))
        return FrameworkCollectionService.s_tracer.TraceAction<bool>(context, (ActionTracePoints) FrameworkCollectionService.TracePoints.UpdateProperties, (Func<bool>) (() =>
        {
          if (properties == null || properties.UpdatedProperties.IsNullOrEmpty<KeyValuePair<string, object>>())
            return true;
          object obj;
          if (properties.UpdatedProperties.TryGetValue("Policy.AllowAnonymousAccess", out obj) && obj != null)
          {
            string str = obj.ToString();
            if (!bool.TryParse(str, out bool _))
            {
              context.Trace(654321, TraceLevel.Error, "Organization", nameof (FrameworkCollectionService), "Someone fatfingered the value of Policy.AllowAnonymousAccess policy. Value: '" + str + "'. Setting value to false.");
              properties.UpdatedProperties["Policy.AllowAnonymousAccess"] = (object) bool.FalseString;
            }
          }
          JsonPatchDocument patchDocument = JsonPatchDocumentHelper.ConstructJsonPatchDocument(Operation.Replace, (IDictionary<string, object>) properties.UpdatedProperties);
          int num = this.GetOrganizationHttpClient(context).UpdateCollectionPropertiesAsync(context.ServiceHost.InstanceId, patchDocument).SyncResult<bool>() ? 1 : 0;
          OrganizationMessagePublisher.Publish(context, new AccountChangedData()
          {
            AccountId = context.ServiceHost.InstanceId
          }, ChangePublisherKind.SqlNotification);
          return num != 0;
        }), nameof (UpdateProperties));
    }

    public bool DeleteProperties(IVssRequestContext context, IEnumerable<string> propertyNames)
    {
      this.ValidateRequestContext(context);
      using (FrameworkCollectionService.s_tracer.TraceTimedAction(context, FrameworkCollectionService.TracePoints.DeleteProperties.Slow, 2000, nameof (DeleteProperties)))
        return FrameworkCollectionService.s_tracer.TraceAction<bool>(context, (ActionTracePoints) FrameworkCollectionService.TracePoints.DeleteProperties, (Func<bool>) (() =>
        {
          if (propertyNames.IsNullOrEmpty<string>())
            return true;
          JsonPatchDocument patchDocument = JsonPatchDocumentHelper.ConstructJsonPatchDocument(Operation.Remove, propertyNames);
          int num = this.GetOrganizationHttpClient(context).UpdateCollectionPropertiesAsync(context.ServiceHost.InstanceId, patchDocument).SyncResult<bool>() ? 1 : 0;
          OrganizationMessagePublisher.Publish(context, new AccountChangedData()
          {
            AccountId = context.ServiceHost.InstanceId
          }, ChangePublisherKind.SqlNotification);
          return num != 0;
        }), nameof (DeleteProperties));
    }

    public bool ForceUpdateCollectionOwner(
      IVssRequestContext context,
      Guid newOwnerId,
      string forceUpdateReason)
    {
      this.ValidateRequestContext(context);
      ArgumentUtility.CheckForEmptyGuid(newOwnerId, nameof (newOwnerId));
      ArgumentUtility.CheckStringLength(forceUpdateReason ?? string.Empty, nameof (forceUpdateReason), 256);
      using (FrameworkCollectionService.s_tracer.TraceTimedAction(context, FrameworkCollectionService.TracePoints.ForceUpdateCollectionOwner.Slow, 2000, nameof (ForceUpdateCollectionOwner)))
        return FrameworkCollectionService.s_tracer.TraceAction<bool>(context, (ActionTracePoints) FrameworkCollectionService.TracePoints.ForceUpdateCollectionOwner, (Func<bool>) (() =>
        {
          Microsoft.VisualStudio.Services.Organization.Client.Collection collection = this.GetOrganizationHttpClient(context).ForceUpdateCollectionOwnerAsync(forceUpdateReason, context.ServiceHost.InstanceId, newOwnerId).SyncResult<Microsoft.VisualStudio.Services.Organization.Client.Collection>();
          OrganizationMessagePublisher.Publish(context, new AccountChangedData()
          {
            AccountId = context.ServiceHost.InstanceId
          }, ChangePublisherKind.SqlNotification);
          return collection.Owner == newOwnerId;
        }), nameof (ForceUpdateCollectionOwner));
    }

    public bool IsEligibleForTakeOver(IVssRequestContext context)
    {
      this.ValidateRequestContext(context);
      using (FrameworkCollectionService.s_tracer.TraceTimedAction(context, FrameworkCollectionService.TracePoints.IsEligibleForTakeOver.Slow, 2000, nameof (IsEligibleForTakeOver)))
        return FrameworkCollectionService.s_tracer.TraceAction<bool>(context, (ActionTracePoints) FrameworkCollectionService.TracePoints.IsEligibleForTakeOver, (Func<bool>) (() => this.GetOrganizationHttpClient(context).IsEligibleForTakeOverAsync(context.ServiceHost.InstanceId).SyncResult<bool>()), nameof (IsEligibleForTakeOver));
    }

    public bool DeleteAvatar(IVssRequestContext context)
    {
      this.ValidateRequestContext(context);
      using (FrameworkCollectionService.s_tracer.TraceTimedAction(context, FrameworkCollectionService.TracePoints.DeleteAvatar.Slow, 2000, nameof (DeleteAvatar)))
        return FrameworkCollectionService.s_tracer.TraceAction<bool>(context, (ActionTracePoints) FrameworkCollectionService.TracePoints.DeleteAvatar, (Func<bool>) (() =>
        {
          this.GetOrganizationHttpClient(context).DeleteCollectionAvatarAsync(context.ServiceHost.InstanceId).SyncResult();
          OrganizationMessagePublisher.Publish(context, new AccountChangedData()
          {
            AccountId = context.ServiceHost.InstanceId
          }, ChangePublisherKind.SqlNotification);
          return true;
        }), nameof (DeleteAvatar));
    }

    public Collection BackfillPreferredGeography(
      IVssRequestContext collectionContext,
      string geographyCode)
    {
      this.ValidateRequestContext(collectionContext);
      using (FrameworkCollectionService.s_tracer.TraceTimedAction(collectionContext, FrameworkCollectionService.TracePoints.BackfillPreferredGeography.Slow, 5000, nameof (BackfillPreferredGeography)))
        return FrameworkCollectionService.s_tracer.TraceAction<Collection>(collectionContext, (ActionTracePoints) FrameworkCollectionService.TracePoints.BackfillPreferredGeography, (Func<Collection>) (() =>
        {
          OrganizationHttpClient organizationHttpClient = this.GetOrganizationHttpClient(collectionContext);
          Guid instanceId = collectionContext.ServiceHost.InstanceId;
          Guid collectionId = instanceId;
          string geographyName = geographyCode;
          CancellationToken cancellationToken = new CancellationToken();
          Microsoft.VisualStudio.Services.Organization.Client.Collection x = organizationHttpClient.BackfillPreferredGeographyAsync(collectionId, geographyName, cancellationToken: cancellationToken).SyncResult<Microsoft.VisualStudio.Services.Organization.Client.Collection>();
          OrganizationMessagePublisher.Publish(collectionContext, new AccountChangedData()
          {
            AccountId = instanceId
          }, ChangePublisherKind.SqlNotification);
          return x.ToServer();
        }), nameof (BackfillPreferredGeography));
    }

    private void ValidateRequestContext(IVssRequestContext context)
    {
      context.CheckHostedDeployment();
      context.CheckServiceHostType(TeamFoundationHostType.ProjectCollection);
      context.CheckServiceHostId(this.m_serviceHostId, (IVssFrameworkService) this);
    }

    private OrganizationHttpClient GetOrganizationHttpClient(IVssRequestContext requestContext) => requestContext.GetClient<OrganizationHttpClient>();

    private static class TracePoints
    {
      internal static readonly TimedActionTracePoints GetCollection = new TimedActionTracePoints(7730010, 7730017, 7730018, 7730019);
      internal static readonly TimedActionTracePoints UpdateCollectionOwner = new TimedActionTracePoints(7730020, 7730027, 7730028, 7730029);
      internal static readonly TimedActionTracePoints UpdateProperties = new TimedActionTracePoints(7730030, 7730037, 7730038, 7730039);
      internal static readonly TimedActionTracePoints DeleteProperties = new TimedActionTracePoints(7730040, 7730047, 7730048, 7730049);
      internal static readonly TimedActionTracePoints RenameCollection = new TimedActionTracePoints(7730050, 7730057, 7730058, 7730059);
      internal static readonly TimedActionTracePoints ForceUpdateCollectionOwner = new TimedActionTracePoints(7730060, 7730067, 7730068, 7730069);
      internal static readonly TimedActionTracePoints IsEligibleForTakeOver = new TimedActionTracePoints(7730070, 7730077, 7730078, 7730079);
      internal static readonly TimedActionTracePoints DeleteAvatar = new TimedActionTracePoints(7790140, 7790147, 7790148, 7790149);
      internal static readonly TimedActionTracePoints BackfillPreferredGeography = new TimedActionTracePoints(7790150, 7790157, 7790158, 7790159);
    }
  }
}
