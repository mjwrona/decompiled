// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Graph.Services.GraphUsersService
// Assembly: Microsoft.VisualStudio.Services.Graph, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 00390AA0-D8BB-45EB-AEF5-70DC8BFC765D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Graph.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Directories;
using Microsoft.VisualStudio.Services.Directories.DirectoryService;
using Microsoft.VisualStudio.Services.Directories.DirectoryService.Components;
using Microsoft.VisualStudio.Services.Graph.Client;
using Microsoft.VisualStudio.Services.Identity;
using Microsoft.VisualStudio.Services.Identity.Internal;
using Microsoft.VisualStudio.Services.Licensing;
using Microsoft.VisualStudio.Services.Organization;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;

namespace Microsoft.VisualStudio.Services.Graph.Services
{
  public class GraphUsersService : IVssFrameworkService
  {
    private const string Area = "Graph";
    private const string Layer = "GraphUsersService";

    public void ServiceStart(IVssRequestContext systemRequestContext) => systemRequestContext.CheckProjectCollectionRequestContext();

    public void ServiceEnd(IVssRequestContext systemRequestContext) => systemRequestContext.CheckProjectCollectionRequestContext();

    public virtual void UpdateUserInternal(
      IVssRequestContext requestContext,
      GraphUserPrincipalNameUpdateContext updateContext,
      Microsoft.VisualStudio.Services.Identity.Identity currentIdentity)
    {
      requestContext.TraceBlock(6307651, 6307658, "Graph", nameof (GraphUsersService), nameof (UpdateUserInternal), (Action) (() =>
      {
        BusinessRulesValidator.ValidateGraphMemberCreationContext(requestContext, updateContext);
        Guid organizationAadTenantId = requestContext.GetOrganizationAadTenantId();
        Microsoft.VisualStudio.Services.Identity.Identity identity = requestContext.To(TeamFoundationHostType.Application).GetService<IdentityService>().ReadIdentities(requestContext, (IList<IdentityDescriptor>) new List<IdentityDescriptor>()
        {
          IdentityHelper.CreateDescriptorFromAccountName(requestContext.GetTenantId().ToString(), updateContext.PrincipalName)
        }, QueryMembership.None, (IEnumerable<string>) null).SingleOrDefault<Microsoft.VisualStudio.Services.Identity.Identity>();
        if (identity == null)
        {
          requestContext.TraceAlways(6307652, TraceLevel.Info, "Graph", nameof (GraphUsersService), string.Format("Target identity not found by UPN {0} and TenantId {1} at enterprise level", (object) updateContext.PrincipalName, (object) organizationAadTenantId));
          this.ResolveIdentity(updateContext.ToDirectoryEntityDescriptor(), requestContext, updateContext.PrincipalName, currentIdentity);
        }
        else if (requestContext.IsFeatureEnabled("VisualStudio.Services.Graph.UpdateUserInternal.ScrubBeforeTransfer.Disable"))
        {
          requestContext.TraceAlways(6307652, TraceLevel.Info, "Graph", nameof (GraphUsersService), "VisualStudio.Services.Graph.UpdateUserInternal.ScrubBeforeTransfer.Disable enabled");
          this.EnsureNoExistingMembershipsInCollection(requestContext, identity);
          IdentityRightsTransferHelper.TransferIdentityRights(requestContext.Elevate(), currentIdentity, identity);
          requestContext.TraceAlways(6307652, TraceLevel.Info, "Graph", nameof (GraphUsersService), string.Format("Identity transfer successful: {0} -> {1}", (object) currentIdentity.Id, (object) identity.Id));
        }
        else
        {
          requestContext.TraceAlways(6307652, TraceLevel.Warning, "Graph", nameof (GraphUsersService), "VisualStudio.Services.Graph.UpdateUserInternal.ScrubBeforeTransfer.Disable disabled");
          requestContext.TraceAlways(6307652, TraceLevel.Info, "Graph", nameof (GraphUsersService), string.Format("Target identity found by UPN {0} and TenantId {1} at enterprise level: {2}", (object) updateContext.PrincipalName, (object) organizationAadTenantId, (object) identity.Id));
          if (currentIdentity.MasterId == identity.Id)
          {
            requestContext.TraceAlways(6307652, TraceLevel.Error, "Graph", nameof (GraphUsersService), string.Format("toIdentity is already currentIdentity's masterId: [{0} -> {1}]", (object) currentIdentity.Id, (object) currentIdentity.MasterId));
            throw new GraphBadRequestException("Cannot transfer identity to itself");
          }
          if (this.DetermineAndRemoveExistingMembershipsInCollection(identity.Clone(), requestContext))
          {
            this.ResolveIdentity(updateContext.ToDirectoryEntityDescriptor(), requestContext, updateContext.PrincipalName, currentIdentity);
          }
          else
          {
            requestContext.TraceAlways(6307652, TraceLevel.Error, "Graph", nameof (GraphUsersService), string.Format("Failed to scrub {0}", (object) identity.Id));
            throw new GraphAccountNameCollisionRepairFailedException("Failed to scrub " + identity.Descriptor.Identifier);
          }
        }
      }));
    }

    public void UpdateUserInternal(
      IVssRequestContext requestContext,
      GraphUserOriginIdUpdateContext updateContext,
      Microsoft.VisualStudio.Services.Identity.Identity currentIdentity)
    {
      requestContext.TraceBlock(6307651, 6307658, "Graph", nameof (GraphUsersService), nameof (UpdateUserInternal), (Action) (() =>
      {
        BusinessRulesValidator.ValidateGraphMemberUpdateContext(requestContext, (IGraphMemberOriginIdUpdateContext) updateContext);
        Guid organizationAadTenantId = requestContext.GetOrganizationAadTenantId();
        IVssRequestContext context = requestContext.To(TeamFoundationHostType.Application);
        IdentityService service = context.GetService<IdentityService>();
        Guid result;
        if (!Guid.TryParse(updateContext.OriginId, out result))
          throw new GraphBadRequestException(Resources.CannotParseParameter((object) updateContext.OriginId, (object) "OriginId"));
        IVssRequestContext vssRequestContext = context;
        Guid guid1 = organizationAadTenantId;
        Guid guid2 = result;
        Microsoft.VisualStudio.Services.Identity.Identity identity = ReadIdentitiesByAadTenantIdOidExtension.ReadIdentityByTenantIdAndOid(service, vssRequestContext, guid1, guid2);
        if (identity == null)
        {
          requestContext.TraceAlways(6307652, TraceLevel.Info, "Graph", nameof (GraphUsersService), string.Format("Target identity not found by OID {0} and TenantId {1} at enterprise level", (object) result, (object) organizationAadTenantId));
          this.ResolveIdentity(updateContext.ToDirectoryEntityDescriptor((string) null), requestContext, updateContext.OriginId, currentIdentity);
        }
        else
        {
          this.ValidateSubjectDescriptorType(identity.SubjectDescriptor);
          if (requestContext.IsFeatureEnabled("VisualStudio.Services.Graph.UpdateUserInternal.ScrubBeforeTransfer.Disable"))
          {
            requestContext.TraceAlways(6307652, TraceLevel.Warning, "Graph", nameof (GraphUsersService), "VisualStudio.Services.Graph.UpdateUserInternal.ScrubBeforeTransfer.Disable enabled");
            this.EnsureNoExistingMembershipsInCollection(requestContext, identity);
            IdentityRightsTransferHelper.TransferIdentityRights(requestContext.Elevate(), currentIdentity, identity);
            requestContext.TraceAlways(6307652, TraceLevel.Info, "Graph", nameof (GraphUsersService), string.Format("Identity transfer successful: {0} -> {1}", (object) currentIdentity.Id, (object) identity.Id));
          }
          else
          {
            requestContext.TraceAlways(6307652, TraceLevel.Warning, "Graph", nameof (GraphUsersService), "VisualStudio.Services.Graph.UpdateUserInternal.ScrubBeforeTransfer.Disable disabled");
            requestContext.TraceAlways(6307652, TraceLevel.Info, "Graph", nameof (GraphUsersService), string.Format("Target identity found by OID {0} and TenantId {1} at enterprise level: {2}", (object) result, (object) organizationAadTenantId, (object) identity.Id));
            if (currentIdentity.MasterId == identity.Id)
            {
              requestContext.TraceAlways(6307652, TraceLevel.Error, "Graph", nameof (GraphUsersService), string.Format("toIdentity is already currentIdentity's masterId: [{0} -> {1}]", (object) currentIdentity.Id, (object) currentIdentity.MasterId));
              throw new GraphBadRequestException("Cannot transfer identity to itself");
            }
            if (this.DetermineAndRemoveExistingMembershipsInCollection(identity.Clone(), requestContext))
            {
              this.ResolveIdentity(updateContext.ToDirectoryEntityDescriptor(Guid.NewGuid().ToString()), requestContext, updateContext.OriginId, currentIdentity);
            }
            else
            {
              requestContext.TraceAlways(6307652, TraceLevel.Error, "Graph", nameof (GraphUsersService), string.Format("Failed to scrub {0}", (object) identity.Id));
              throw new GraphAccountNameCollisionRepairFailedException("Failed to scrub " + identity.Descriptor.Identifier);
            }
          }
        }
      }));
    }

    private IdentityDirectoryEntityResult<Microsoft.VisualStudio.Services.Identity.Identity> MaterializeExternalUser(
      IVssRequestContext context,
      IDirectoryEntityDescriptor directoryEntityDescriptor,
      IEnumerable<string> localGroups)
    {
      IdentityDirectoryWrapperService<Microsoft.VisualStudio.Services.Identity.Identity> directoryWrapperService = context.GetService<IDirectoryService>().IncludeIdentities(context);
      IVssRequestContext requestContext = context;
      IDirectoryEntityDescriptor member = directoryEntityDescriptor;
      IEnumerable<string> strings = (IEnumerable<string>) new string[1]
      {
        "LocalDescriptor"
      };
      IEnumerable<string> localGroups1 = localGroups;
      IEnumerable<string> propertiesToReturn = strings;
      return directoryWrapperService.AddMember(requestContext, member, license: "None", localGroups: localGroups1, propertiesToReturn: propertiesToReturn);
    }

    private bool DetermineAndRemoveExistingMembershipsInCollection(
      Microsoft.VisualStudio.Services.Identity.Identity targetIdentity,
      IVssRequestContext requestContext)
    {
      IdentityService collectionIdentityService = requestContext.GetService<IdentityService>();
      Microsoft.VisualStudio.Services.Identity.Identity identityInAccount = collectionIdentityService.ReadIdentities(requestContext, (IList<Guid>) new Guid[1]
      {
        targetIdentity.Id
      }, QueryMembership.Direct, (IEnumerable<string>) null).FirstOrDefault<Microsoft.VisualStudio.Services.Identity.Identity>();
      if (identityInAccount == null)
      {
        requestContext.TraceAlways(6307654, TraceLevel.Info, "Graph", nameof (GraphUsersService), string.Format("Target is not readable at collection level by enterprise level id {0}", (object) targetIdentity.Id));
        return MungeIdentityAtEnterprise();
      }
      requestContext.TraceAlways(6307654, TraceLevel.Info, "Graph", nameof (GraphUsersService), string.Format("Target is readable at collection level by enterprise level id {0}: [{1} -> {2}]", (object) targetIdentity.Id, (object) identityInAccount.Id, (object) identityInAccount.MasterId));
      if (identityInAccount.Id != identityInAccount.MasterId)
      {
        requestContext.TraceAlways(6307654, TraceLevel.Info, "Graph", nameof (GraphUsersService), "Translation found for targetIdentity");
        if (targetIdentity.Id == identityInAccount.Id)
        {
          requestContext.TraceAlways(6307654, TraceLevel.Info, "Graph", nameof (GraphUsersService), "targetIdentity is already a localId");
          return MungeIdentityAtEnterprise();
        }
        if (targetIdentity.Id == identityInAccount.MasterId)
        {
          requestContext.TraceAlways(6307654, TraceLevel.Info, "Graph", nameof (GraphUsersService), "targetIdentity is already a masterId");
          RemoveIdentityAtCollection();
          return MungeIdentityAtEnterprise();
        }
        requestContext.TraceAlways(6307654, TraceLevel.Error, "Graph", nameof (GraphUsersService), string.Format("Read {0} and got [{1} -> {2}]", (object) targetIdentity.Id, (object) identityInAccount.Id, (object) identityInAccount.MasterId));
        throw new GraphBadRequestException("Invalid translation for " + targetIdentity.Descriptor.Identifier);
      }
      requestContext.TraceAlways(6307654, TraceLevel.Info, "Graph", nameof (GraphUsersService), "No translations found for targetIdentity");
      RemoveIdentityAtCollection();
      return MungeIdentityAtEnterprise();

      bool MungeIdentityAtEnterprise()
      {
        requestContext.TraceAlways(6307655, TraceLevel.Info, "Graph", nameof (GraphUsersService), string.Format("Munging target identity at enterprise level: {0}", (object) targetIdentity.Id));
        IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Application);
        IdentityService service = vssRequestContext.GetService<IdentityService>();
        IdentityHelper.MungeIdentity(vssRequestContext, targetIdentity, string.Format("AccountTransfer_{0}", (object) Guid.NewGuid()));
        requestContext.TraceAlways(6307655, TraceLevel.Info, "Graph", nameof (GraphUsersService), string.Format("Munging {0} to [{1}]", (object) targetIdentity.Id, (object) targetIdentity.Descriptor.Identifier));
        IVssRequestContext requestContext = vssRequestContext.Elevate();
        Microsoft.VisualStudio.Services.Identity.Identity[] identities = new Microsoft.VisualStudio.Services.Identity.Identity[1]
        {
          targetIdentity
        };
        bool flag = service.UpdateIdentities(requestContext, (IList<Microsoft.VisualStudio.Services.Identity.Identity>) identities, true);
        requestContext.TraceAlways(6307655, TraceLevel.Info, "Graph", nameof (GraphUsersService), string.Format("Munging target identity {0} at enterprise level result: [{1}]", (object) targetIdentity.Id, (object) flag));
        return flag;
      }

      void RemoveIdentityAtCollection()
      {
        requestContext.TraceAlways(6307656, TraceLevel.Info, "Graph", nameof (GraphUsersService), string.Format("Removing target identity at collection level: {0}", (object) identityInAccount.Id));
        if (!identityInAccount.IsActive)
        {
          requestContext.TraceAlways(6307656, TraceLevel.Info, "Graph", nameof (GraphUsersService), string.Format("Target identity is not active at collection level: {0}", (object) identityInAccount.Id));
        }
        else
        {
          requestContext.TraceAlways(6307656, TraceLevel.Info, "Graph", nameof (GraphUsersService), string.Format("Target identity is active at collection level: {0}", (object) identityInAccount.Id));
          requestContext.GetService<ILicensingEntitlementService>().DeleteAccountEntitlement(requestContext, identityInAccount.Id);
          collectionIdentityService.RemoveMemberFromGroup(requestContext, GroupWellKnownIdentityDescriptors.SecurityServiceGroup, identityInAccount.Descriptor);
          foreach (IdentityDescriptor groupDescriptor in (IEnumerable<IdentityDescriptor>) identityInAccount.MemberOf)
            collectionIdentityService.RemoveMemberFromGroup(requestContext, groupDescriptor, identityInAccount.Descriptor);
        }
      }
    }

    private static void TransferIdentityRights(
      HttpResponseMessage message,
      Microsoft.VisualStudio.Services.Identity.Identity currentIdentity,
      IdentityDirectoryEntityResult<Microsoft.VisualStudio.Services.Identity.Identity> result,
      IVssRequestContext requestContext)
    {
      if (message.StatusCode == HttpStatusCode.Created)
      {
        IdentityRightsTransferHelper.TransferIdentityRights(requestContext.Elevate(), currentIdentity, result.Identity);
        requestContext.TraceAlways(6307652, TraceLevel.Info, "Graph", nameof (GraphUsersService), string.Format("Identity transfer successful: {0} -> {1}", (object) currentIdentity.Id, (object) result.Identity.Id));
      }
      else
        requestContext.TraceAlways(6307652, TraceLevel.Error, "Graph", nameof (GraphUsersService), string.Format("Target identity materialization failed: [{0}]", (object) result.Exception));
    }

    private void EnsureNoExistingMembershipsInCollection(
      IVssRequestContext requestContext,
      Microsoft.VisualStudio.Services.Identity.Identity identity)
    {
      IList<Microsoft.VisualStudio.Services.Identity.Identity> source = requestContext.GetService<IdentityService>().ReadIdentities(requestContext, (IList<Guid>) new List<Guid>()
      {
        identity.MasterId
      }, QueryMembership.None, (IEnumerable<string>) null);
      if ((source != null ? source.FirstOrDefault<Microsoft.VisualStudio.Services.Identity.Identity>() : (Microsoft.VisualStudio.Services.Identity.Identity) null) != null)
        throw new InvalidTransferIdentityRightsRequestException(string.Format("Identity with VSID {0} has active or inactive memberships in the account", (object) identity.MasterId));
      requestContext.TraceAlways(6307653, TraceLevel.Info, "Graph", nameof (GraphUsersService), string.Format("Target identity not found at collection: {0}", (object) identity.MasterId));
    }

    private void ResolveIdentity(
      IDirectoryEntityDescriptor directoryEntityDescriptor,
      IVssRequestContext requestContext,
      string descriptor,
      Microsoft.VisualStudio.Services.Identity.Identity currentIdentity)
    {
      IdentityDirectoryEntityResult<Microsoft.VisualStudio.Services.Identity.Identity> directoryEntityResult = this.MaterializeExternalUser(requestContext, directoryEntityDescriptor, Enumerable.Empty<string>());
      if (directoryEntityResult.Status == "Success")
      {
        IdentityRightsTransferHelper.TransferIdentityRights(requestContext.Elevate(), currentIdentity, directoryEntityResult.Identity);
        requestContext.TraceAlways(6307652, TraceLevel.Info, "Graph", nameof (GraphUsersService), string.Format("Identity transfer successful: {0} -> {1}", (object) currentIdentity.Id, (object) directoryEntityResult.Identity.Id));
      }
      else
      {
        requestContext.TraceAlways(6307652, TraceLevel.Error, "Graph", nameof (GraphUsersService), string.Format("Target identity materialization failed: [{0}]", (object) directoryEntityResult.Exception));
        throw new GraphException(string.Format("Target identity materialization failed: [{0}]", (object) directoryEntityResult.Exception));
      }
    }

    private void ValidateSubjectDescriptorType(SubjectDescriptor subjectDescriptor)
    {
      if (subjectDescriptor.IsAadServicePrincipalType())
        throw new GraphBadRequestException(Resources.GraphUserServicePrincipalsMappingNotAllowed());
    }
  }
}
