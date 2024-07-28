// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.DevOps.ServiceEndpoints.Sdk.Server.IdentityServiceExtensions
// Assembly: Microsoft.Azure.DevOps.ServiceEndpoints.Sdk.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 002C83BC-B53E-470A-8038-76E47B5E5BF3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.Azure.DevOps.ServiceEndpoints.Sdk.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Identity;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.Azure.DevOps.ServiceEndpoints.Sdk.Server
{
  public static class IdentityServiceExtensions
  {
    public static bool TryGetScope(
      this IdentityService identityService,
      IVssRequestContext requestContext,
      string scopeName,
      out IdentityScope scope)
    {
      try
      {
        scope = identityService.GetScope(requestContext, scopeName);
      }
      catch (GroupScopeDoesNotExistException ex)
      {
        scope = (IdentityScope) null;
      }
      return scope != null;
    }

    public static bool TryGetScope(
      this IdentityService identityService,
      IVssRequestContext requestContext,
      Guid scopeId,
      out IdentityScope scope)
    {
      try
      {
        scope = identityService.GetScope(requestContext, scopeId);
      }
      catch (GroupScopeDoesNotExistException ex)
      {
        scope = (IdentityScope) null;
      }
      return scope != null;
    }

    public static IEnumerable<Microsoft.VisualStudio.Services.Identity.Identity> GetGroups(
      this IdentityService identityService,
      IVssRequestContext requestContext,
      Guid scopeId,
      IEnumerable<IdentityDescriptor> descriptors,
      bool insertNullForUnmatchedValues = false)
    {
      List<Microsoft.VisualStudio.Services.Identity.Identity> groups = new List<Microsoft.VisualStudio.Services.Identity.Identity>();
      IList<Microsoft.VisualStudio.Services.Identity.Identity> source = identityService.ListGroups(requestContext, new Guid[1]
      {
        scopeId
      }, false, (IEnumerable<string>) null);
      foreach (IdentityDescriptor descriptor1 in descriptors)
      {
        IdentityDescriptor descriptor = descriptor1;
        Microsoft.VisualStudio.Services.Identity.Identity identity = source.FirstOrDefault<Microsoft.VisualStudio.Services.Identity.Identity>((Func<Microsoft.VisualStudio.Services.Identity.Identity, bool>) (x => IdentityHelper.IsWellKnownGroup(x.Descriptor, descriptor)));
        if (identity != null | insertNullForUnmatchedValues)
          groups.Add(identity);
      }
      return (IEnumerable<Microsoft.VisualStudio.Services.Identity.Identity>) groups;
    }

    public static IEnumerable<Microsoft.VisualStudio.Services.Identity.Identity> GetGroups(
      this IdentityService identityService,
      IVssRequestContext requestContext,
      Guid scopeId,
      IList<string> accountNames)
    {
      List<Microsoft.VisualStudio.Services.Identity.Identity> groups = new List<Microsoft.VisualStudio.Services.Identity.Identity>();
      IList<Microsoft.VisualStudio.Services.Identity.Identity> source = identityService.ListGroups(requestContext, new Guid[1]
      {
        scopeId
      }, false, (IEnumerable<string>) null);
      foreach (string accountName1 in (IEnumerable<string>) accountNames)
      {
        string accountName = accountName1;
        Microsoft.VisualStudio.Services.Identity.Identity identity = source.FirstOrDefault<Microsoft.VisualStudio.Services.Identity.Identity>((Func<Microsoft.VisualStudio.Services.Identity.Identity, bool>) (group => string.Equals(accountName, group.GetProperty<string>("Account", string.Empty), StringComparison.OrdinalIgnoreCase)));
        if (identity != null)
          groups.Add(identity);
      }
      return (IEnumerable<Microsoft.VisualStudio.Services.Identity.Identity>) groups;
    }

    public static Microsoft.VisualStudio.Services.Identity.Identity GetIdentity(
      this IdentityService identityService,
      IVssRequestContext requestContext,
      Guid identifier,
      QueryMembership queryMembership = QueryMembership.None)
    {
      return identityService.ReadIdentities(requestContext, (IList<Guid>) new Guid[1]
      {
        identifier
      }, queryMembership, (IEnumerable<string>) null).FirstOrDefault<Microsoft.VisualStudio.Services.Identity.Identity>();
    }

    public static Microsoft.VisualStudio.Services.Identity.Identity GetIdentity(
      this IdentityService identityService,
      IVssRequestContext requestContext,
      string identifier,
      QueryMembership queryMembership = QueryMembership.None)
    {
      Microsoft.VisualStudio.Services.Identity.Identity identity = (Microsoft.VisualStudio.Services.Identity.Identity) null;
      Guid result;
      if (Guid.TryParse(identifier, out result))
        identity = identityService.ReadIdentities(requestContext, (IList<Guid>) new Guid[1]
        {
          result
        }, queryMembership, (IEnumerable<string>) null).FirstOrDefault<Microsoft.VisualStudio.Services.Identity.Identity>();
      if (identity == null)
        identity = identityService.ReadIdentities(requestContext, IdentitySearchFilter.General, identifier, queryMembership, (IEnumerable<string>) null).FirstOrDefault<Microsoft.VisualStudio.Services.Identity.Identity>();
      return identity;
    }

    public static Microsoft.VisualStudio.Services.Identity.Identity GetIdentity(
      this IdentityService identityService,
      IVssRequestContext requestContext,
      IdentityDescriptor identifier,
      QueryMembership queryMembership = QueryMembership.None)
    {
      return identityService.GetIdentity(requestContext, requestContext.ServiceHost.InstanceId, identifier, queryMembership);
    }

    public static Microsoft.VisualStudio.Services.Identity.Identity GetIdentity(
      this IdentityService identityService,
      IVssRequestContext requestContext,
      Guid scopeId,
      IdentityDescriptor identifier,
      QueryMembership queryMembership = QueryMembership.None)
    {
      return identityService.GetIdentities(requestContext, scopeId, (IEnumerable<IdentityDescriptor>) new IdentityDescriptor[1]
      {
        identifier
      }, queryMembership).FirstOrDefault<Microsoft.VisualStudio.Services.Identity.Identity>();
    }

    public static Microsoft.VisualStudio.Services.Identity.Identity GetIdentity(
      this IdentityService identityService,
      IVssRequestContext requestContext,
      IdentityRef identityRef,
      QueryMembership queryMembership = QueryMembership.None)
    {
      Microsoft.VisualStudio.Services.Identity.Identity identity = (Microsoft.VisualStudio.Services.Identity.Identity) null;
      if (identityRef != null)
      {
        if (identityRef.Descriptor != new SubjectDescriptor())
          identity = identityService.ReadIdentities(requestContext, (IList<SubjectDescriptor>) new SubjectDescriptor[1]
          {
            identityRef.Descriptor
          }, queryMembership, (IEnumerable<string>) null).FirstOrDefault<Microsoft.VisualStudio.Services.Identity.Identity>();
        if (identity == null && !string.IsNullOrEmpty(identityRef.Id))
          identity = identityService.GetIdentity(requestContext, identityRef.Id, queryMembership);
      }
      return identity;
    }

    public static IEnumerable<Microsoft.VisualStudio.Services.Identity.Identity> GetIdentities(
      this IdentityService identityService,
      IVssRequestContext requestContext,
      IEnumerable<Guid> identityIds,
      QueryMembership queryMembership = QueryMembership.None)
    {
      List<Guid> list = identityIds.Where<Guid>((Func<Guid, bool>) (x => x != Guid.Empty)).ToList<Guid>();
      return (IEnumerable<Microsoft.VisualStudio.Services.Identity.Identity>) identityService.ReadIdentities(requestContext, (IList<Guid>) list, QueryMembership.None, (IEnumerable<string>) null);
    }

    public static IEnumerable<Microsoft.VisualStudio.Services.Identity.Identity> GetIdentities(
      this IdentityService identityService,
      IVssRequestContext requestContext,
      Guid scopeId,
      IEnumerable<IdentityDescriptor> identifiers,
      QueryMembership queryMembership = QueryMembership.None)
    {
      List<Microsoft.VisualStudio.Services.Identity.Identity> identityList = new List<Microsoft.VisualStudio.Services.Identity.Identity>();
      IList<IdentityDescriptor> descriptors = (IList<IdentityDescriptor>) new List<IdentityDescriptor>();
      foreach (IdentityDescriptor identifier in identifiers)
        descriptors.Add(IdentityDomain.MapFromWellKnownIdentifier(scopeId, identifier));
      return (IEnumerable<Microsoft.VisualStudio.Services.Identity.Identity>) identityService.ReadIdentities(requestContext, descriptors, queryMembership, (IEnumerable<string>) null);
    }
  }
}
