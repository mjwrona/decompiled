// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Identity.IdentityRetrievalService
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Identity
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  internal class IdentityRetrievalService : IVssIdentityRetrievalService, IVssFrameworkService
  {
    private const string Area = "IdentityService";
    private const string Layer = "IdentityRetrievalService";

    public void ServiceStart(IVssRequestContext systemRequestContext)
    {
    }

    public void ServiceEnd(IVssRequestContext systemRequestContext)
    {
    }

    public IReadOnlyDictionary<IdentityDescriptor, Microsoft.VisualStudio.Services.Identity.Identity> GetMaterializedUsers(
      IVssRequestContext collectionOrOrganizationContext,
      IEnumerable<IdentityDescriptor> userDescriptors)
    {
      collectionOrOrganizationContext.CheckProjectCollectionOrOrganizationRequestContext();
      Dictionary<IdentityDescriptor, Microsoft.VisualStudio.Services.Identity.Identity> source;
      List<IdentityDescriptor> list;
      if (collectionOrOrganizationContext.IsDeploymentFallbackIdentityReadAllowed())
      {
        IVssRequestContext context = collectionOrOrganizationContext.To(TeamFoundationHostType.Deployment);
        IdentityService service = context.GetService<IdentityService>();
        if (!(userDescriptors is IList<IdentityDescriptor> identityDescriptorList))
          identityDescriptorList = (IList<IdentityDescriptor>) userDescriptors.ToList<IdentityDescriptor>();
        IList<IdentityDescriptor> second = identityDescriptorList;
        IVssRequestContext requestContext = context;
        IList<IdentityDescriptor> descriptors = second;
        source = service.ReadIdentities(requestContext, descriptors, QueryMembership.None, (IEnumerable<string>) null).Zip((IEnumerable<IdentityDescriptor>) second, (identity, descriptor) => new
        {
          descriptor = descriptor,
          identity = identity
        }).ToDedupedDictionary(item => item.descriptor, item => item.identity);
        list = source.Where<KeyValuePair<IdentityDescriptor, Microsoft.VisualStudio.Services.Identity.Identity>>((Func<KeyValuePair<IdentityDescriptor, Microsoft.VisualStudio.Services.Identity.Identity>, bool>) (r => r.Value == null)).Select<KeyValuePair<IdentityDescriptor, Microsoft.VisualStudio.Services.Identity.Identity>, IdentityDescriptor>((Func<KeyValuePair<IdentityDescriptor, Microsoft.VisualStudio.Services.Identity.Identity>, IdentityDescriptor>) (r => r.Key)).ToList<IdentityDescriptor>();
        if (list.Count == 0)
          return (IReadOnlyDictionary<IdentityDescriptor, Microsoft.VisualStudio.Services.Identity.Identity>) source;
      }
      else
      {
        source = new Dictionary<IdentityDescriptor, Microsoft.VisualStudio.Services.Identity.Identity>();
        list = userDescriptors.ToList<IdentityDescriptor>();
      }
      IVssRequestContext vssRequestContext = collectionOrOrganizationContext.To(TeamFoundationHostType.Application);
      foreach (var data in vssRequestContext.GetService<IdentityService>().ReadIdentities(vssRequestContext, (IList<IdentityDescriptor>) list, QueryMembership.None, (IEnumerable<string>) null).Zip((IEnumerable<IdentityDescriptor>) list, (identity, descriptor) => new
      {
        descriptor = descriptor,
        identity = identity
      }))
        source[data.descriptor] = data.identity;
      return (IReadOnlyDictionary<IdentityDescriptor, Microsoft.VisualStudio.Services.Identity.Identity>) source;
    }

    public Microsoft.VisualStudio.Services.Identity.Identity ResolveEligibleActor(
      IVssRequestContext requestContext,
      IdentityDescriptor actorDescriptor)
    {
      if (actorDescriptor == (IdentityDescriptor) null)
        return (Microsoft.VisualStudio.Services.Identity.Identity) null;
      Microsoft.VisualStudio.Services.Identity.Identity identity = requestContext.GetService<IdentityService>().ReadIdentities(requestContext, (IList<IdentityDescriptor>) new IdentityDescriptor[1]
      {
        actorDescriptor
      }, QueryMembership.None, (IEnumerable<string>) null).FirstOrDefault<Microsoft.VisualStudio.Services.Identity.Identity>();
      if (identity != null && !identity.Descriptor.Equals(actorDescriptor))
        requestContext.Trace(82000, TraceLevel.Verbose, "IdentityService", nameof (IdentityRetrievalService), string.Format("ResolveEligibleActor: While tring to resolve the descriptor [{0}], we got back the descriptor [{1}].", (object) actorDescriptor, (object) identity.Descriptor));
      if ((identity == null || !identity.Descriptor.Equals(actorDescriptor)) && actorDescriptor.IsCuidBased() && !requestContext.ServiceHost.Is(TeamFoundationHostType.Deployment))
      {
        IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Deployment);
        identity = vssRequestContext.GetService<IdentityService>().ReadIdentities(vssRequestContext, (IList<IdentityDescriptor>) new IdentityDescriptor[1]
        {
          actorDescriptor
        }, QueryMembership.None, (IEnumerable<string>) null).FirstOrDefault<Microsoft.VisualStudio.Services.Identity.Identity>();
      }
      return identity;
    }

    public IReadOnlyDictionary<IdentityDescriptor, Microsoft.VisualStudio.Services.Identity.Identity> GetActiveUsers(
      IVssRequestContext collectionOrOrganizationContext,
      IEnumerable<IdentityDescriptor> userDescriptors)
    {
      return this.GetItems(collectionOrOrganizationContext, userDescriptors, (Func<Microsoft.VisualStudio.Services.Identity.Identity, bool>) (item => item.IsActive && !item.IsContainer));
    }

    public IReadOnlyDictionary<IdentityDescriptor, Microsoft.VisualStudio.Services.Identity.Identity> GetActiveGroups(
      IVssRequestContext collectionOrOrganizationContext,
      IEnumerable<IdentityDescriptor> groupDescriptors)
    {
      return this.GetItems(collectionOrOrganizationContext, groupDescriptors, (Func<Microsoft.VisualStudio.Services.Identity.Identity, bool>) (item => item.IsActive && item.IsContainer));
    }

    public IReadOnlyDictionary<IdentityDescriptor, Microsoft.VisualStudio.Services.Identity.Identity> GetActiveMembers(
      IVssRequestContext collectionOrOrganizationContext,
      IEnumerable<IdentityDescriptor> memberDescriptors)
    {
      return this.GetItems(collectionOrOrganizationContext, memberDescriptors, (Func<Microsoft.VisualStudio.Services.Identity.Identity, bool>) (item => item.IsActive));
    }

    public IReadOnlyDictionary<IdentityDescriptor, Microsoft.VisualStudio.Services.Identity.Identity> GetActiveOrHistoricalUsers(
      IVssRequestContext collectionOrOrganizationContext,
      IEnumerable<IdentityDescriptor> userDescriptors)
    {
      return this.GetItems(collectionOrOrganizationContext, userDescriptors, (Func<Microsoft.VisualStudio.Services.Identity.Identity, bool>) (item => !item.IsContainer));
    }

    public IReadOnlyDictionary<IdentityDescriptor, Microsoft.VisualStudio.Services.Identity.Identity> GetActiveOrHistoricalGroups(
      IVssRequestContext collectionOrOrganizationContext,
      IEnumerable<IdentityDescriptor> groupDescriptors)
    {
      return this.GetItems(collectionOrOrganizationContext, groupDescriptors, (Func<Microsoft.VisualStudio.Services.Identity.Identity, bool>) (item => item.IsContainer));
    }

    public IReadOnlyDictionary<IdentityDescriptor, Microsoft.VisualStudio.Services.Identity.Identity> GetActiveOrHistoricalMembers(
      IVssRequestContext collectionOrOrganizationContext,
      IEnumerable<IdentityDescriptor> memberDescriptors)
    {
      return this.GetItems(collectionOrOrganizationContext, memberDescriptors);
    }

    public IReadOnlyDictionary<Guid, Microsoft.VisualStudio.Services.Identity.Identity> GetActiveUsers(
      IVssRequestContext collectionOrOrganizationContext,
      IEnumerable<Guid> userIds)
    {
      return this.GetItems(collectionOrOrganizationContext, userIds, (Func<Microsoft.VisualStudio.Services.Identity.Identity, bool>) (item => item.IsActive && !item.IsContainer));
    }

    public IReadOnlyDictionary<Guid, Microsoft.VisualStudio.Services.Identity.Identity> GetActiveGroups(
      IVssRequestContext collectionOrOrganizationContext,
      IEnumerable<Guid> groupIds)
    {
      return this.GetItems(collectionOrOrganizationContext, groupIds, (Func<Microsoft.VisualStudio.Services.Identity.Identity, bool>) (item => item.IsActive && item.IsContainer));
    }

    public IReadOnlyDictionary<Guid, Microsoft.VisualStudio.Services.Identity.Identity> GetActiveMembers(
      IVssRequestContext collectionOrOrganizationContext,
      IEnumerable<Guid> memberIds)
    {
      return this.GetItems(collectionOrOrganizationContext, memberIds, (Func<Microsoft.VisualStudio.Services.Identity.Identity, bool>) (item => item.IsActive));
    }

    public IReadOnlyDictionary<Guid, Microsoft.VisualStudio.Services.Identity.Identity> GetActiveOrHistoricalUsers(
      IVssRequestContext collectionOrOrganizationContext,
      IEnumerable<Guid> userIds)
    {
      return this.GetItems(collectionOrOrganizationContext, userIds, (Func<Microsoft.VisualStudio.Services.Identity.Identity, bool>) (item => !item.IsContainer));
    }

    public IReadOnlyDictionary<Guid, Microsoft.VisualStudio.Services.Identity.Identity> GetActiveOrHistoricalGroups(
      IVssRequestContext collectionOrOrganizationContext,
      IEnumerable<Guid> groupIds)
    {
      return this.GetItems(collectionOrOrganizationContext, groupIds, (Func<Microsoft.VisualStudio.Services.Identity.Identity, bool>) (item => item.IsContainer));
    }

    public IReadOnlyDictionary<Guid, Microsoft.VisualStudio.Services.Identity.Identity> GetActiveOrHistoricalMembers(
      IVssRequestContext collectionOrOrganizationContext,
      IEnumerable<Guid> memberIds)
    {
      return this.GetItems(collectionOrOrganizationContext, memberIds);
    }

    private IReadOnlyDictionary<IdentityDescriptor, Microsoft.VisualStudio.Services.Identity.Identity> GetItems(
      IVssRequestContext collectionOrOrganizationContext,
      IEnumerable<IdentityDescriptor> itemDescriptors,
      Func<Microsoft.VisualStudio.Services.Identity.Identity, bool> filter = null)
    {
      collectionOrOrganizationContext.CheckProjectCollectionOrOrganizationRequestContext();
      ArgumentUtility.CheckForNull<IEnumerable<IdentityDescriptor>>(itemDescriptors, nameof (itemDescriptors));
      filter = filter ?? (Func<Microsoft.VisualStudio.Services.Identity.Identity, bool>) (item => true);
      if (!(itemDescriptors is IList<IdentityDescriptor> identityDescriptorList1))
        identityDescriptorList1 = (IList<IdentityDescriptor>) itemDescriptors.ToList<IdentityDescriptor>();
      IList<IdentityDescriptor> identityDescriptorList2 = identityDescriptorList1;
      return (IReadOnlyDictionary<IdentityDescriptor, Microsoft.VisualStudio.Services.Identity.Identity>) collectionOrOrganizationContext.GetService<IdentityService>().ReadIdentities(collectionOrOrganizationContext, identityDescriptorList2, QueryMembership.None, (IEnumerable<string>) null).Select<Microsoft.VisualStudio.Services.Identity.Identity, Microsoft.VisualStudio.Services.Identity.Identity>((Func<Microsoft.VisualStudio.Services.Identity.Identity, Microsoft.VisualStudio.Services.Identity.Identity>) (identity => identity != null && !filter(identity) ? (Microsoft.VisualStudio.Services.Identity.Identity) null : identity)).Zip((IEnumerable<IdentityDescriptor>) identityDescriptorList2, (identity, descriptor) => new
      {
        descriptor = descriptor,
        identity = identity
      }).ToDedupedDictionary(item => item.descriptor, item => item.identity, (IEqualityComparer<IdentityDescriptor>) IdentityDescriptorComparer.Instance);
    }

    private IReadOnlyDictionary<Guid, Microsoft.VisualStudio.Services.Identity.Identity> GetItems(
      IVssRequestContext collectionOrOrganizationContext,
      IEnumerable<Guid> itemId,
      Func<Microsoft.VisualStudio.Services.Identity.Identity, bool> filter = null)
    {
      collectionOrOrganizationContext.CheckProjectCollectionOrOrganizationRequestContext();
      ArgumentUtility.CheckForNull<IEnumerable<Guid>>(itemId, nameof (itemId));
      filter = filter ?? (Func<Microsoft.VisualStudio.Services.Identity.Identity, bool>) (item => true);
      if (!(itemId is IList<Guid> guidList1))
        guidList1 = (IList<Guid>) itemId.ToList<Guid>();
      IList<Guid> guidList2 = guidList1;
      return (IReadOnlyDictionary<Guid, Microsoft.VisualStudio.Services.Identity.Identity>) collectionOrOrganizationContext.GetService<IdentityService>().ReadIdentities(collectionOrOrganizationContext, guidList2, QueryMembership.None, (IEnumerable<string>) null).Select<Microsoft.VisualStudio.Services.Identity.Identity, Microsoft.VisualStudio.Services.Identity.Identity>((Func<Microsoft.VisualStudio.Services.Identity.Identity, Microsoft.VisualStudio.Services.Identity.Identity>) (identity => identity != null && !filter(identity) ? (Microsoft.VisualStudio.Services.Identity.Identity) null : identity)).Zip((IEnumerable<Guid>) guidList2, (identity, id) => new
      {
        id = id,
        identity = identity
      }).ToDedupedDictionary(item => item.id, item => item.identity);
    }
  }
}
