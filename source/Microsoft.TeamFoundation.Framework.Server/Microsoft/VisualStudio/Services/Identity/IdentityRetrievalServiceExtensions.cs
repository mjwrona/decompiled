// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Identity.IdentityRetrievalServiceExtensions
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Identity
{
  public static class IdentityRetrievalServiceExtensions
  {
    private const string Area = "IdentityService";
    private const string Layer = "IdentityRetrievalService";

    public static Microsoft.VisualStudio.Services.Identity.Identity GetActiveUser(
      this IVssIdentityRetrievalService identityRetrievalService,
      IVssRequestContext collectionOrOrganizationContext,
      IdentityDescriptor userDescriptor)
    {
      return identityRetrievalService.GetActiveUsers(collectionOrOrganizationContext, (IEnumerable<IdentityDescriptor>) new IdentityDescriptor[1]
      {
        userDescriptor
      })[userDescriptor];
    }

    public static Microsoft.VisualStudio.Services.Identity.Identity GetActiveGroup(
      this IVssIdentityRetrievalService identityRetrievalService,
      IVssRequestContext collectionOrOrganizationContext,
      IdentityDescriptor groupDescriptor)
    {
      return identityRetrievalService.GetActiveGroups(collectionOrOrganizationContext, (IEnumerable<IdentityDescriptor>) new IdentityDescriptor[1]
      {
        groupDescriptor
      })[groupDescriptor];
    }

    public static Microsoft.VisualStudio.Services.Identity.Identity GetActiveMember(
      this IVssIdentityRetrievalService identityRetrievalService,
      IVssRequestContext collectionOrOrganizationContext,
      IdentityDescriptor memberDescriptor)
    {
      return identityRetrievalService.GetActiveMembers(collectionOrOrganizationContext, (IEnumerable<IdentityDescriptor>) new IdentityDescriptor[1]
      {
        memberDescriptor
      })[memberDescriptor];
    }

    public static Microsoft.VisualStudio.Services.Identity.Identity GetActiveOrHistoricalUser(
      this IVssIdentityRetrievalService identityRetrievalService,
      IVssRequestContext collectionOrOrganizationContext,
      IdentityDescriptor userDescriptor)
    {
      return identityRetrievalService.GetActiveOrHistoricalUsers(collectionOrOrganizationContext, (IEnumerable<IdentityDescriptor>) new IdentityDescriptor[1]
      {
        userDescriptor
      })[userDescriptor];
    }

    public static Microsoft.VisualStudio.Services.Identity.Identity GetActiveOrHistoricalGroup(
      this IVssIdentityRetrievalService identityRetrievalService,
      IVssRequestContext collectionOrOrganizationContext,
      IdentityDescriptor groupDescriptor)
    {
      return identityRetrievalService.GetActiveOrHistoricalGroups(collectionOrOrganizationContext, (IEnumerable<IdentityDescriptor>) new IdentityDescriptor[1]
      {
        groupDescriptor
      })[groupDescriptor];
    }

    public static Microsoft.VisualStudio.Services.Identity.Identity GetActiveOrHistoricalMember(
      this IVssIdentityRetrievalService identityRetrievalService,
      IVssRequestContext collectionOrOrganizationContext,
      IdentityDescriptor memberDescriptor)
    {
      return identityRetrievalService.GetActiveOrHistoricalMembers(collectionOrOrganizationContext, (IEnumerable<IdentityDescriptor>) new IdentityDescriptor[1]
      {
        memberDescriptor
      })[memberDescriptor];
    }

    public static Microsoft.VisualStudio.Services.Identity.Identity GetActiveUser(
      this IVssIdentityRetrievalService identityRetrievalService,
      IVssRequestContext collectionOrOrganizationContext,
      Guid userid)
    {
      return identityRetrievalService.GetActiveUsers(collectionOrOrganizationContext, (IEnumerable<Guid>) new Guid[1]
      {
        userid
      })[userid];
    }

    public static Microsoft.VisualStudio.Services.Identity.Identity GetActiveGroup(
      this IVssIdentityRetrievalService identityRetrievalService,
      IVssRequestContext collectionOrOrganizationContext,
      Guid groupid)
    {
      return identityRetrievalService.GetActiveGroups(collectionOrOrganizationContext, (IEnumerable<Guid>) new Guid[1]
      {
        groupid
      })[groupid];
    }

    public static Microsoft.VisualStudio.Services.Identity.Identity GetActiveMember(
      this IVssIdentityRetrievalService identityRetrievalService,
      IVssRequestContext collectionOrOrganizationContext,
      Guid memberid)
    {
      return identityRetrievalService.GetActiveMembers(collectionOrOrganizationContext, (IEnumerable<Guid>) new Guid[1]
      {
        memberid
      })[memberid];
    }

    public static Microsoft.VisualStudio.Services.Identity.Identity GetActiveOrHistoricalUser(
      this IVssIdentityRetrievalService identityRetrievalService,
      IVssRequestContext collectionOrOrganizationContext,
      Guid userid)
    {
      return identityRetrievalService.GetActiveOrHistoricalUsers(collectionOrOrganizationContext, (IEnumerable<Guid>) new Guid[1]
      {
        userid
      })[userid];
    }

    public static Microsoft.VisualStudio.Services.Identity.Identity GetActiveOrHistoricalGroup(
      this IVssIdentityRetrievalService identityRetrievalService,
      IVssRequestContext collectionOrOrganizationContext,
      Guid groupid)
    {
      return identityRetrievalService.GetActiveOrHistoricalGroups(collectionOrOrganizationContext, (IEnumerable<Guid>) new Guid[1]
      {
        groupid
      })[groupid];
    }

    public static Microsoft.VisualStudio.Services.Identity.Identity GetActiveOrHistoricalMember(
      this IVssIdentityRetrievalService identityRetrievalService,
      IVssRequestContext collectionOrOrganizationContext,
      Guid memberid)
    {
      return identityRetrievalService.GetActiveOrHistoricalMembers(collectionOrOrganizationContext, (IEnumerable<Guid>) new Guid[1]
      {
        memberid
      })[memberid];
    }

    public static Microsoft.VisualStudio.Services.Identity.Identity ResolveEligibleActorByMasterId(
      this IVssIdentityRetrievalService identityRetrievalService,
      IVssRequestContext collectionOrOrganizationContext,
      Guid masterId)
    {
      IVssRequestContext vssRequestContext1 = collectionOrOrganizationContext.Elevate();
      IdentityDescriptor identityDescriptor = vssRequestContext1.GetService<IIdentityIdentifierConversionService>().GetDescriptorByMasterId(vssRequestContext1, masterId);
      collectionOrOrganizationContext.TraceDataConditionally(82007, TraceLevel.Verbose, "IdentityService", "IdentityRetrievalService", "Results from identity identifier conversion service", (Func<object>) (() => (object) new
      {
        masterId = masterId,
        identityDescriptor = identityDescriptor
      }), nameof (ResolveEligibleActorByMasterId));
      if (identityDescriptor != (IdentityDescriptor) null)
      {
        Microsoft.VisualStudio.Services.Identity.Identity identity = identityRetrievalService.ResolveEligibleActor(collectionOrOrganizationContext, identityDescriptor);
        if (identity != null)
          return identity;
        collectionOrOrganizationContext.TraceDataConditionally(1007073, TraceLevel.Error, "IdentityService", "IdentityRetrievalService", "ResolveEligibleActorByMasterId: resolvedEligibleActor is null", (Func<object>) (() => (object) new
        {
          identityDescriptor = identityDescriptor
        }), nameof (ResolveEligibleActorByMasterId));
        return identity;
      }
      if (ServicePrincipals.IsInternalServicePrincipalId(masterId))
      {
        IVssRequestContext vssRequestContext2 = vssRequestContext1.To(TeamFoundationHostType.Deployment);
        Microsoft.VisualStudio.Services.Identity.Identity readIdentity = vssRequestContext2.GetService<IdentityService>().ReadIdentities(vssRequestContext2, (IList<Guid>) new Guid[1]
        {
          masterId
        }, QueryMembership.None, (IEnumerable<string>) null)[0];
        if (readIdentity != null)
          return readIdentity;
        collectionOrOrganizationContext.Trace(1007070, TraceLevel.Error, "IdentityService", "IdentityRetrievalService", "ResolveEligibleActorByMasterId: identity is null");
        return readIdentity;
      }
      if (identityDescriptor == (IdentityDescriptor) null)
        collectionOrOrganizationContext.TraceAlways(1007071, TraceLevel.Info, "IdentityService", "IdentityRetrievalService", string.Format("ResolveEligibleActorByMasterId: identityDescriptor is null; masterId is {0}", (object) masterId));
      return (Microsoft.VisualStudio.Services.Identity.Identity) null;
    }

    public static Microsoft.VisualStudio.Services.Identity.Identity ResolveEligibleActorByDeploymentId(
      this IVssIdentityRetrievalService identityRetrievalService,
      IVssRequestContext requestContext,
      Guid deploymentId)
    {
      Microsoft.VisualStudio.Services.Identity.Identity identity1 = (Microsoft.VisualStudio.Services.Identity.Identity) null;
      IVssRequestContext vssRequestContext = requestContext.Elevate().To(TeamFoundationHostType.Deployment);
      Microsoft.VisualStudio.Services.Identity.Identity identity2 = vssRequestContext.GetService<IdentityService>().ReadIdentities(vssRequestContext, (IList<Guid>) new Guid[1]
      {
        deploymentId
      }, QueryMembership.None, (IEnumerable<string>) null).First<Microsoft.VisualStudio.Services.Identity.Identity>();
      if (requestContext.ServiceHost.Is(TeamFoundationHostType.Deployment))
        identity1 = identity2;
      else if (identity2 != null)
      {
        identity1 = requestContext.GetService<IdentityService>().ReadIdentities(requestContext, (IList<IdentityDescriptor>) new IdentityDescriptor[1]
        {
          identity2.Descriptor
        }, QueryMembership.None, (IEnumerable<string>) null).FirstOrDefault<Microsoft.VisualStudio.Services.Identity.Identity>();
        requestContext.Trace(82008, TraceLevel.Info, "IdentityService", "IdentityRetrievalService", string.Format("Translate from deployment identity {0} to enterprise identity {1}", (object) 0, (object) 1), (object) identity2, (object) identity1);
      }
      requestContext.Trace(82009, TraceLevel.Info, "IdentityService", "IdentityRetrievalService", string.Format("Identity id {0} is resolved to {1}", (object) 0, (object) 1), (object) deploymentId, (object) identity1);
      return identity1;
    }

    public static Microsoft.VisualStudio.Services.Identity.Identity ResolveEligibleActorByBasicAuthAlias(
      this IVssIdentityRetrievalService identityRetrievalService,
      IVssRequestContext collectionOrOrganizationContext,
      string basicAuthAlias)
    {
      IVssRequestContext collectionOrOrganizationContext1 = collectionOrOrganizationContext.Elevate();
      IVssRequestContext vssRequestContext = collectionOrOrganizationContext1.To(TeamFoundationHostType.Application);
      IdentitySearchFilter searchFactor = basicAuthAlias.IndexOf('@') >= 0 ? IdentitySearchFilter.AccountName : IdentitySearchFilter.Alias;
      if (searchFactor == IdentitySearchFilter.AccountName)
      {
        IdentityValidation.CheckAccountName(ref basicAuthAlias);
        basicAuthAlias = IdentityHelper.QualifyAccountName(collectionOrOrganizationContext, basicAuthAlias);
      }
      IList<Microsoft.VisualStudio.Services.Identity.Identity> source = vssRequestContext.GetService<IdentityService>().ReadIdentities(vssRequestContext, searchFactor, basicAuthAlias, QueryMembership.None, (IEnumerable<string>) null);
      if (source.Count == 0)
      {
        collectionOrOrganizationContext.Trace(1007064, TraceLevel.Info, "IdentityService", "IdentityRetrievalService", "Couldn't find any identities with provided username {1}", (object) basicAuthAlias);
        return (Microsoft.VisualStudio.Services.Identity.Identity) null;
      }
      IList<Microsoft.VisualStudio.Services.Identity.Identity> list = (IList<Microsoft.VisualStudio.Services.Identity.Identity>) identityRetrievalService.GetActiveUsers(collectionOrOrganizationContext1, (IEnumerable<IdentityDescriptor>) source.Select<Microsoft.VisualStudio.Services.Identity.Identity, IdentityDescriptor>((Func<Microsoft.VisualStudio.Services.Identity.Identity, IdentityDescriptor>) (x => x.Descriptor)).ToList<IdentityDescriptor>()).Values.Where<Microsoft.VisualStudio.Services.Identity.Identity>((Func<Microsoft.VisualStudio.Services.Identity.Identity, bool>) (x => x != null)).ToList<Microsoft.VisualStudio.Services.Identity.Identity>();
      Microsoft.VisualStudio.Services.Identity.Identity identity = (Microsoft.VisualStudio.Services.Identity.Identity) null;
      if (list.Count == 1)
        identity = list[0];
      else
        collectionOrOrganizationContext.Trace(1007061, TraceLevel.Error, "IdentityService", "IdentityRetrievalService", "Find {0} identities of username {1}", (object) list.Count, (object) basicAuthAlias);
      return identity;
    }
  }
}
