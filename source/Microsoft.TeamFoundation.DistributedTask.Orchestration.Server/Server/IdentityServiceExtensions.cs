// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Server.IdentityServiceExtensions
// Assembly: Microsoft.TeamFoundation.DistributedTask.Orchestration.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07FD5059-3D25-415E-AA3A-5372051D7E71
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.dll

using Microsoft.TeamFoundation.DistributedTask.Orchestration.Server;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Identity;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamFoundation.DistributedTask.Server
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
      IdentityScope scope;
      IList<IdentityDescriptor> descriptors1 = !identityService.TryGetScope(requestContext, scopeId, out scope) ? (IList<IdentityDescriptor>) descriptors.ToList<IdentityDescriptor>() : (IList<IdentityDescriptor>) descriptors.Select<IdentityDescriptor, IdentityDescriptor>((Func<IdentityDescriptor, IdentityDescriptor>) (x => IdentityDomain.MapFromWellKnownIdentifier(scope.Id, x))).ToList<IdentityDescriptor>();
      IList<Microsoft.VisualStudio.Services.Identity.Identity> source = identityService.ReadIdentities(requestContext, descriptors1, QueryMembership.None, (IEnumerable<string>) null);
      foreach (IdentityDescriptor descriptor1 in descriptors)
      {
        IdentityDescriptor descriptor = descriptor1;
        Microsoft.VisualStudio.Services.Identity.Identity identity = source.FirstOrDefault<Microsoft.VisualStudio.Services.Identity.Identity>((Func<Microsoft.VisualStudio.Services.Identity.Identity, bool>) (x => x != null && IdentityHelper.IsWellKnownGroup(x.Descriptor, descriptor)));
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
      IdentityScope scope = identityService.GetScope(requestContext, scopeId);
      foreach (string accountName in (IEnumerable<string>) accountNames)
      {
        Microsoft.VisualStudio.Services.Identity.Identity identity = identityService.ReadIdentities(requestContext, IdentitySearchFilter.LocalGroupName, "[" + scope.Name + "]\\" + accountName, QueryMembership.None, (IEnumerable<string>) null).FirstOrDefault<Microsoft.VisualStudio.Services.Identity.Identity>();
        if (identity != null)
          groups.Add(identity);
      }
      return (IEnumerable<Microsoft.VisualStudio.Services.Identity.Identity>) groups;
    }

    public static IEnumerable<Microsoft.VisualStudio.Services.Identity.Identity> GetDeletedGroups(
      this IdentityService identityService,
      IVssRequestContext requestContext,
      Guid scopeId,
      IEnumerable<IdentityDescriptor> descriptors)
    {
      List<Microsoft.VisualStudio.Services.Identity.Identity> deletedGroups = new List<Microsoft.VisualStudio.Services.Identity.Identity>();
      IList<Microsoft.VisualStudio.Services.Identity.Identity> source = identityService.ListDeletedGroups(requestContext, new Guid[1]
      {
        scopeId
      }, false, (IEnumerable<string>) null);
      foreach (IdentityDescriptor descriptor1 in descriptors)
      {
        IdentityDescriptor descriptor = descriptor1;
        Microsoft.VisualStudio.Services.Identity.Identity identity = source.FirstOrDefault<Microsoft.VisualStudio.Services.Identity.Identity>((Func<Microsoft.VisualStudio.Services.Identity.Identity, bool>) (x => x != null && IdentityHelper.IsWellKnownGroup(x.Descriptor, descriptor)));
        if (identity != null)
          deletedGroups.Add(identity);
      }
      return (IEnumerable<Microsoft.VisualStudio.Services.Identity.Identity>) deletedGroups;
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
        {
          requestContext.TraceVerbose("Orchestration", "Trying identity lookup from descriptor {0}", (object) identityRef.Descriptor);
          identity = identityService.ReadIdentities(requestContext, (IList<SubjectDescriptor>) new SubjectDescriptor[1]
          {
            identityRef.Descriptor
          }, queryMembership, (IEnumerable<string>) null).FirstOrDefault<Microsoft.VisualStudio.Services.Identity.Identity>();
        }
        if (identity == null && !string.IsNullOrEmpty(identityRef.Id))
        {
          requestContext.TraceVerbose("Orchestration", "Trying identity lookup from ID {0}", (object) identityRef.Id);
          identity = identityService.GetIdentity(requestContext, identityRef.Id, queryMembership);
        }
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
