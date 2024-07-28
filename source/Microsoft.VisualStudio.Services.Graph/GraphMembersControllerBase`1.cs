// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Graph.GraphMembersControllerBase`1
// Assembly: Microsoft.VisualStudio.Services.Graph, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 00390AA0-D8BB-45EB-AEF5-70DC8BFC765D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Graph.dll

using Microsoft.TeamFoundation.Common;
using Microsoft.TeamFoundation.Framework.Common;
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

namespace Microsoft.VisualStudio.Services.Graph
{
  public abstract class GraphMembersControllerBase<T> : GraphControllerBase where T : AadGraphMember
  {
    protected static readonly IReadOnlyCollection<string> ServicePrincipalSubjectTypes = (IReadOnlyCollection<string>) new HashSet<string>()
    {
      "aadsp"
    };
    protected static readonly IReadOnlyCollection<string> UserSubjectTypes = (IReadOnlyCollection<string>) new HashSet<string>()
    {
      "aad",
      "msa",
      "svc",
      "bnd",
      "imp",
      "acs",
      "unusr",
      "win",
      "agg",
      "csp"
    };
    protected static readonly IReadOnlyCollection<string> MemberSubjectTypes = (IReadOnlyCollection<string>) new HashSet<string>((IEnumerable<string>) GraphMembersControllerBase<T>.UserSubjectTypes)
    {
      "aadsp"
    };

    protected abstract IReadOnlyCollection<string> AllowedSubjectTypes { get; }

    protected abstract string Layer { get; }

    protected HttpResponseMessage UpdateMemberInternal(
      IVssRequestContext requestContext,
      IGraphMemberOriginIdUpdateContext updateContext,
      Microsoft.VisualStudio.Services.Identity.Identity currentIdentity)
    {
      return requestContext.TraceBlock<HttpResponseMessage>(6307651, 6307658, this.TraceArea, this.Layer, nameof (UpdateMemberInternal), (Func<HttpResponseMessage>) (() =>
      {
        BusinessRulesValidator.ValidateGraphMemberUpdateContext(this.TfsRequestContext, updateContext);
        string lowerCase = "OriginId".FirstLetterToLowerCase();
        Guid organizationAadTenantId = requestContext.GetOrganizationAadTenantId();
        IVssRequestContext context = requestContext.To(TeamFoundationHostType.Application);
        IdentityService service = context.GetService<IdentityService>();
        Guid result1;
        if (!Guid.TryParse(updateContext.OriginId, out result1))
          throw new GraphBadRequestException(Resources.CannotParseParameter((object) updateContext.OriginId, (object) "OriginId"));
        Microsoft.VisualStudio.Services.Identity.Identity identity = ReadIdentitiesByAadTenantIdOidExtension.ReadIdentityByTenantIdAndOid(service, context, organizationAadTenantId, result1);
        if (identity == null)
        {
          requestContext.TraceAlways(6307652, TraceLevel.Info, this.TraceArea, this.Layer, string.Format("Target identity not found by OID {0} and TenantId {1} at enterprise level", (object) result1, (object) organizationAadTenantId));
          IdentityDirectoryEntityResult<Microsoft.VisualStudio.Services.Identity.Identity> result2 = this.MaterializeExternalMember(this.TfsRequestContext, updateContext.ToDirectoryEntityDescriptor(), Enumerable.Empty<string>());
          HttpResponseMessage materializeMemberResult = this.ParseMaterializeMemberResult(result2, lowerCase, updateContext.OriginId);
          if (materializeMemberResult.StatusCode == HttpStatusCode.Created)
          {
            IdentityRightsTransferHelper.TransferIdentityRights(requestContext.Elevate(), currentIdentity, result2.Identity);
            requestContext.TraceAlways(6307652, TraceLevel.Info, this.TraceArea, this.Layer, string.Format("Identity transfer successful: {0} -> {1}", (object) currentIdentity.Id, (object) result2.Identity.Id));
            return materializeMemberResult;
          }
          requestContext.TraceAlways(6307652, TraceLevel.Error, this.TraceArea, this.Layer, string.Format("Target identity materialization failed: [{0}]", (object) result2.Exception));
          return materializeMemberResult;
        }
        this.ValidateSubjectDescriptorType(identity.SubjectDescriptor);
        if (requestContext.IsFeatureEnabled("VisualStudio.Services.Graph.UpdateUserInternal.ScrubBeforeTransfer.Disable"))
        {
          requestContext.TraceAlways(6307652, TraceLevel.Warning, this.TraceArea, this.Layer, "VisualStudio.Services.Graph.UpdateUserInternal.ScrubBeforeTransfer.Disable enabled");
          this.EnsureNoExistingMembershipsInCollection(requestContext, identity);
          IdentityRightsTransferHelper.TransferIdentityRights(requestContext.Elevate(), currentIdentity, identity);
          requestContext.TraceAlways(6307652, TraceLevel.Info, this.TraceArea, this.Layer, string.Format("Identity transfer successful: {0} -> {1}", (object) currentIdentity.Id, (object) identity.Id));
          return this.CreateSuccessResponse(identity);
        }
        requestContext.TraceAlways(6307652, TraceLevel.Warning, this.TraceArea, this.Layer, "VisualStudio.Services.Graph.UpdateUserInternal.ScrubBeforeTransfer.Disable disabled");
        requestContext.TraceAlways(6307652, TraceLevel.Info, this.TraceArea, this.Layer, string.Format("Target identity found by OID {0} and TenantId {1} at enterprise level: {2}", (object) result1, (object) organizationAadTenantId, (object) identity.Id));
        if (currentIdentity.MasterId == identity.Id)
        {
          requestContext.TraceAlways(6307652, TraceLevel.Error, this.TraceArea, this.Layer, string.Format("toIdentity is already currentIdentity's masterId: [{0} -> {1}]", (object) currentIdentity.Id, (object) currentIdentity.MasterId));
          throw new GraphBadRequestException("Cannot transfer identity to itself");
        }
        if (this.DetermineAndRemoveExistingMembershipsInCollection(requestContext, identity.Clone()))
        {
          IdentityDirectoryEntityResult<Microsoft.VisualStudio.Services.Identity.Identity> result3 = this.MaterializeExternalMember(this.TfsRequestContext, updateContext.ToDirectoryEntityDescriptor(Guid.NewGuid().ToString()), Enumerable.Empty<string>());
          HttpResponseMessage materializeMemberResult = this.ParseMaterializeMemberResult(result3, lowerCase, updateContext.OriginId);
          if (materializeMemberResult.StatusCode == HttpStatusCode.Created)
          {
            IdentityRightsTransferHelper.TransferIdentityRights(requestContext.Elevate(), currentIdentity, result3.Identity);
            requestContext.TraceAlways(6307652, TraceLevel.Info, this.TraceArea, this.Layer, string.Format("Identity transfer successful: {0} -> {1}", (object) currentIdentity.Id, (object) result3.Identity.Id));
            return materializeMemberResult;
          }
          requestContext.TraceAlways(6307652, TraceLevel.Error, this.TraceArea, this.Layer, string.Format("Target identity materialization failed: [{0}]", (object) result3.Exception));
          return materializeMemberResult;
        }
        requestContext.TraceAlways(6307652, TraceLevel.Error, this.TraceArea, this.Layer, string.Format("Failed to scrub {0}", (object) identity.Id));
        throw new GraphAccountNameCollisionRepairFailedException("Failed to scrub " + identity.Descriptor.Identifier);
      }));
    }

    private bool DetermineAndRemoveExistingMembershipsInCollection(
      IVssRequestContext requestContext,
      Microsoft.VisualStudio.Services.Identity.Identity targetIdentity)
    {
      IdentityService collectionIdentityService = requestContext.GetService<IdentityService>();
      Microsoft.VisualStudio.Services.Identity.Identity identityInAccount = collectionIdentityService.ReadIdentities(requestContext, (IList<Guid>) new Guid[1]
      {
        targetIdentity.Id
      }, QueryMembership.Direct, (IEnumerable<string>) null).FirstOrDefault<Microsoft.VisualStudio.Services.Identity.Identity>();
      if (identityInAccount == null)
      {
        requestContext.TraceAlways(6307654, TraceLevel.Info, this.TraceArea, this.Layer, string.Format("Target is not readable at collection level by enterprise level id {0}", (object) targetIdentity.Id));
        return MungIdentityAtEnterprise();
      }
      requestContext.TraceAlways(6307654, TraceLevel.Info, this.TraceArea, this.Layer, string.Format("Target is readable at collection level by enterprise level id {0}: [{1} -> {2}]", (object) targetIdentity.Id, (object) identityInAccount.Id, (object) identityInAccount.MasterId));
      if (identityInAccount.Id != identityInAccount.MasterId)
      {
        requestContext.TraceAlways(6307654, TraceLevel.Info, this.TraceArea, this.Layer, "Translation found for targetIdentity");
        if (targetIdentity.Id == identityInAccount.Id)
        {
          requestContext.TraceAlways(6307654, TraceLevel.Info, this.TraceArea, this.Layer, "targetIdentity is already a localId");
          return MungIdentityAtEnterprise();
        }
        if (targetIdentity.Id == identityInAccount.MasterId)
        {
          requestContext.TraceAlways(6307654, TraceLevel.Info, this.TraceArea, this.Layer, "targetIdentity is already a masterId");
          RemoveIdentityAtCollection();
          return MungIdentityAtEnterprise();
        }
        requestContext.TraceAlways(6307654, TraceLevel.Error, this.TraceArea, this.Layer, string.Format("Read {0} and got [{1} -> {2}]", (object) targetIdentity.Id, (object) identityInAccount.Id, (object) identityInAccount.MasterId));
        throw new GraphBadRequestException("Invalid translation for " + targetIdentity.Descriptor.Identifier);
      }
      requestContext.TraceAlways(6307654, TraceLevel.Info, this.TraceArea, this.Layer, "No translations found for targetIdentity");
      RemoveIdentityAtCollection();
      return MungIdentityAtEnterprise();

      bool MungIdentityAtEnterprise()
      {
        requestContext.TraceAlways(6307655, TraceLevel.Info, this.TraceArea, this.Layer, string.Format("Munging target identity at enterprise level: {0}", (object) targetIdentity.Id));
        IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Application);
        IdentityService service = vssRequestContext.GetService<IdentityService>();
        IdentityHelper.MungeIdentity(vssRequestContext, targetIdentity, string.Format("AccountTransfer_{0}", (object) Guid.NewGuid()));
        requestContext.TraceAlways(6307655, TraceLevel.Info, this.TraceArea, this.Layer, string.Format("Munging {0} to [{1}]", (object) targetIdentity.Id, (object) targetIdentity.Descriptor.Identifier));
        IVssRequestContext requestContext = vssRequestContext.Elevate();
        Microsoft.VisualStudio.Services.Identity.Identity[] identities = new Microsoft.VisualStudio.Services.Identity.Identity[1]
        {
          targetIdentity
        };
        bool flag = service.UpdateIdentities(requestContext, (IList<Microsoft.VisualStudio.Services.Identity.Identity>) identities, true);
        requestContext.TraceAlways(6307655, TraceLevel.Info, this.TraceArea, this.Layer, string.Format("Munging target identity {0} at enterprise level result: [{1}]", (object) targetIdentity.Id, (object) flag));
        return flag;
      }

      void RemoveIdentityAtCollection()
      {
        requestContext.TraceAlways(6307656, TraceLevel.Info, this.TraceArea, this.Layer, string.Format("Removing target identity at collection level: {0}", (object) identityInAccount.Id));
        if (!identityInAccount.IsActive)
        {
          requestContext.TraceAlways(6307656, TraceLevel.Info, this.TraceArea, this.Layer, string.Format("Target identity is not active at collection level: {0}", (object) identityInAccount.Id));
        }
        else
        {
          requestContext.TraceAlways(6307656, TraceLevel.Info, this.TraceArea, this.Layer, string.Format("Target identity is active at collection level: {0}", (object) identityInAccount.Id));
          requestContext.GetService<ILicensingEntitlementService>().DeleteAccountEntitlement(requestContext, identityInAccount.Id);
          collectionIdentityService.RemoveMemberFromGroup(requestContext, GroupWellKnownIdentityDescriptors.SecurityServiceGroup, identityInAccount.Descriptor);
          foreach (IdentityDescriptor groupDescriptor in (IEnumerable<IdentityDescriptor>) identityInAccount.MemberOf)
            collectionIdentityService.RemoveMemberFromGroup(requestContext, groupDescriptor, identityInAccount.Descriptor);
        }
      }
    }

    protected abstract void ValidateSubjectDescriptorType(SubjectDescriptor subjectDescriptor);

    private void EnsureNoExistingMembershipsInCollection(
      IVssRequestContext requestContext,
      Microsoft.VisualStudio.Services.Identity.Identity identity)
    {
      IList<Microsoft.VisualStudio.Services.Identity.Identity> source = this.TfsRequestContext.GetService<IdentityService>().ReadIdentities(this.TfsRequestContext, (IList<Guid>) new List<Guid>()
      {
        identity.MasterId
      }, QueryMembership.None, (IEnumerable<string>) null);
      if ((source != null ? source.FirstOrDefault<Microsoft.VisualStudio.Services.Identity.Identity>() : (Microsoft.VisualStudio.Services.Identity.Identity) null) != null)
        throw new InvalidTransferIdentityRightsRequestException(string.Format("Identity with VSID {0} has active or inactive memberships in the account", (object) identity.MasterId));
      requestContext.TraceAlways(6307653, TraceLevel.Info, this.TraceArea, this.Layer, string.Format("Target identity not found at collection: {0}", (object) identity.MasterId));
    }

    protected void CheckPermissionsToTransferIdentity(IVssRequestContext requestContext)
    {
      IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Application);
      vssRequestContext.GetService<ITeamFoundationSecurityService>().GetSecurityNamespace(vssRequestContext, FrameworkSecurity.IdentitiesNamespaceId).CheckPermission(vssRequestContext, requestContext.ServiceHost.InstanceId.ToString(), 31);
    }

    protected HttpResponseMessage AddMemberInternal(
      IGraphMemberOriginIdCreationContext creationContext,
      IEnumerable<string> localGroups)
    {
      BusinessRulesValidator.ValidateGraphMemberCreationContext(this.TfsRequestContext, creationContext);
      string lowerCase = "OriginId".FirstLetterToLowerCase();
      return this.ParseMaterializeMemberResult(this.MaterializeExternalMember(this.TfsRequestContext, creationContext.ToDirectoryEntityDescriptor(), localGroups), lowerCase, creationContext.OriginId);
    }

    protected HttpResponseMessage AddMemberInternal(
      IGraphMemberPrincipalNameCreationContext creationContext,
      IEnumerable<string> localGroups)
    {
      BusinessRulesValidator.ValidateGraphMemberCreationContext(this.TfsRequestContext, creationContext);
      string lowerCase = "PrincipalName".FirstLetterToLowerCase();
      return this.ParseMaterializeMemberResult(this.MaterializeExternalMember(this.TfsRequestContext, creationContext.ToDirectoryEntityDescriptor(), localGroups), lowerCase, creationContext.PrincipalName);
    }

    protected HttpResponseMessage AddMemberInternal(
      IGraphMemberMailAddressCreationContext creationContext,
      IEnumerable<string> localGroups)
    {
      BusinessRulesValidator.ValidateGraphMemberCreationContext(this.TfsRequestContext, creationContext);
      string lowerCase = "MailAddress".FirstLetterToLowerCase();
      return this.ParseMaterializeMemberResult(this.MaterializeExternalMember(this.TfsRequestContext, creationContext.ToDirectoryEntityDescriptor(), localGroups), lowerCase, creationContext.MailAddress);
    }

    protected HttpResponseMessage DeleteMemberInternal(SubjectDescriptor memberDescriptor)
    {
      IdentityService service = this.TfsRequestContext.GetService<IdentityService>();
      Microsoft.VisualStudio.Services.Identity.Identity identity = service.ReadIdentities(this.TfsRequestContext, (IList<SubjectDescriptor>) new SubjectDescriptor[1]
      {
        memberDescriptor
      }, QueryMembership.Direct, (IEnumerable<string>) null, true).SingleOrDefault<Microsoft.VisualStudio.Services.Identity.Identity>();
      if (identity == null)
        throw new GraphSubjectNotFoundException(memberDescriptor);
      service.RemoveMemberFromGroup(this.TfsRequestContext, GroupWellKnownIdentityDescriptors.SecurityServiceGroup, identity.Descriptor);
      foreach (IdentityDescriptor groupDescriptor in (IEnumerable<IdentityDescriptor>) identity.MemberOf)
        service.RemoveMemberFromGroup(this.TfsRequestContext, groupDescriptor, identity.Descriptor);
      return new HttpResponseMessage(HttpStatusCode.NoContent);
    }

    protected T GetMember(IVssRequestContext requestContext, SubjectDescriptor memberDescriptor)
    {
      if (this.AllowedSubjectTypes.Contains<string>(memberDescriptor.SubjectType))
        return GraphSubjectHelper.FetchGraphMember<T>(requestContext, memberDescriptor);
      throw new GraphBadRequestException(Resources.GraphMemberSubjectTypeIsNotAllowed((object) memberDescriptor.SubjectType));
    }

    private IdentityDirectoryEntityResult<Microsoft.VisualStudio.Services.Identity.Identity> MaterializeExternalMember(
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

    private HttpResponseMessage ParseMaterializeMemberResult(
      IdentityDirectoryEntityResult<Microsoft.VisualStudio.Services.Identity.Identity> result,
      string parameterName,
      string parameterValue)
    {
      string status = result.Status;
      HttpResponseMessage materializeMemberResult;
      if (status != null)
      {
        switch (status.Length)
        {
          case 7:
            if (status == "Success")
            {
              materializeMemberResult = result.Identity != null ? this.CreateSuccessResponse(result.Identity) : this.Request.CreateResponse<string>(HttpStatusCode.InternalServerError, result.Exception?.ToString());
              goto label_24;
            }
            else
              goto label_23;
          case 9:
            if (status == "NoResults")
            {
              materializeMemberResult = this.Request.CreateResponse<string>(HttpStatusCode.BadRequest, Resources.CannotMaterializeGraphSubject((object) parameterValue, (object) parameterName));
              goto label_24;
            }
            else
              goto label_23;
          case 10:
            if (status == "IdConflict")
              goto label_20;
            else
              goto label_23;
          case 13:
            switch (status[0])
            {
              case 'A':
                if (status == "AadReadFailed")
                  break;
                goto label_23;
              case 'C':
                if (status == "ConvertFailed")
                  goto label_20;
                else
                  goto label_23;
              case 'V':
                if (status == "VsdReadFailed")
                  break;
                goto label_23;
              default:
                goto label_23;
            }
            break;
          case 14:
            switch (status[0])
            {
              case 'T':
                if (status == "TooManyResults")
                  goto label_21;
                else
                  goto label_23;
              case 'V':
                if (status == "VsdWriteFailed")
                  break;
                goto label_23;
              default:
                goto label_23;
            }
            break;
          case 17:
            if (status == "AadGetTokenFailed")
            {
              materializeMemberResult = this.Request.CreateResponse<string>(HttpStatusCode.Unauthorized, Resources.GraphAadGetAccessTokenFailed());
              goto label_24;
            }
            else
              goto label_23;
          case 21:
            if (status == "IdTranslationConflict")
              goto label_20;
            else
              goto label_23;
          case 22:
            if (status == "InvalidLocalDescriptor")
              goto label_21;
            else
              goto label_23;
          case 25:
            if (status == "ProtectedIdentityConflict")
              goto label_20;
            else
              goto label_23;
          case 26:
            if (status == "InvalidPermissionsProperty")
              goto label_21;
            else
              goto label_23;
          default:
            goto label_23;
        }
        materializeMemberResult = this.Request.CreateResponse<string>(HttpStatusCode.ServiceUnavailable, result.Exception?.ToString());
        goto label_24;
label_20:
        materializeMemberResult = this.Request.CreateResponse<string>(HttpStatusCode.Conflict, result.Exception?.ToString());
        goto label_24;
label_21:
        materializeMemberResult = this.Request.CreateResponse<string>(HttpStatusCode.BadRequest, result.Exception?.ToString());
        goto label_24;
      }
label_23:
      materializeMemberResult = this.Request.CreateResponse<string>(HttpStatusCode.InternalServerError, result.Exception?.ToString());
label_24:
      return materializeMemberResult;
    }

    private HttpResponseMessage CreateSuccessResponse(Microsoft.VisualStudio.Services.Identity.Identity identity)
    {
      GraphSubject graphSubjectClient = identity.ToGraphSubjectClient(this.TfsRequestContext);
      graphSubjectClient.FillSerializeInternalsField(this.TfsRequestContext);
      HttpResponseMessage response = this.Request.CreateResponse<GraphSubject>(HttpStatusCode.Created, graphSubjectClient);
      response.Headers.Add("Location", graphSubjectClient.Url);
      return response;
    }

    protected HttpResponseMessage ListMembersInternal(
      string subjectKinds,
      string subjectTypes = null,
      string continuationToken = null,
      SubjectDescriptor? scopeDescriptor = null,
      string metaTypes = null)
    {
      ScopePagingContext pagingContext = this.GetOrCreatePagingContext(continuationToken, scopeDescriptor);
      IdentitiesPage pagedIdentities = this.TfsRequestContext.GetService<IPagedScopedIdentityReader>().ReadIdentitiesByScopeByPage(this.TfsRequestContext, pagingContext, this.ShouldForceFilterIdentities());
      return this.CreateGraphSubjectResponse(this.FilterGraphSubject(pagedIdentities, pagingContext.PageSize, subjectKinds, subjectTypes, metaTypes), pagedIdentities.ContinuationToken);
    }

    private ScopePagingContext GetOrCreatePagingContext(
      string continuationToken = null,
      SubjectDescriptor? scopeDescriptor = null)
    {
      Guid scopeId = !scopeDescriptor.HasValue ? this.TfsRequestContext.ServiceHost.InstanceId : scopeDescriptor.Value.GetLocalScopeId(this.TfsRequestContext);
      if (continuationToken != null)
        return ScopePagingContext.FromContinuationToken(continuationToken);
      int pageSize = PagingHelper.GetPageSize(this.TfsRequestContext);
      return new ScopePagingContext(scopeId, pageSize, false, true);
    }

    private List<T> FilterGraphSubject(
      IdentitiesPage pagedIdentities,
      int pageSize,
      string subjectKinds,
      string subjectTypes,
      string metaTypes)
    {
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      ISet<IdentityMetaType> hashSet = (ISet<IdentityMetaType>) QueryFilters.ParseFilter(metaTypes).Select<string, IdentityMetaType?>(GraphMembersControllerBase<T>.\u003C\u003EO.\u003C0\u003E__ConvertToIdentityMetaTypeIgnoreNull ?? (GraphMembersControllerBase<T>.\u003C\u003EO.\u003C0\u003E__ConvertToIdentityMetaTypeIgnoreNull = new Func<string, IdentityMetaType?>(GraphObjectExtensionHelpers.ConvertToIdentityMetaTypeIgnoreNull))).Where<IdentityMetaType?>((Func<IdentityMetaType?, bool>) (x => x.HasValue)).Select<IdentityMetaType?, IdentityMetaType>((Func<IdentityMetaType?, IdentityMetaType>) (x => x.Value)).ToHashSet<IdentityMetaType>();
      ISet<string> filter1 = QueryFilters.ParseFilter(subjectTypes);
      ISet<string> filter2 = QueryFilters.ParseFilter(subjectKinds);
      IEnumerable<T> objs = pagedIdentities.Identities.Where<Microsoft.VisualStudio.Services.Identity.Identity>((Func<Microsoft.VisualStudio.Services.Identity.Identity, bool>) (x => !x.IsContainer)).Where<Microsoft.VisualStudio.Services.Identity.Identity>(QueryFilters.ApplyMetaTypeFilter(hashSet, metaTypes.IsNullOrEmpty<char>())).Where<Microsoft.VisualStudio.Services.Identity.Identity>(QueryFilters.ApplySubjectTypeFilter(filter1)).Select<Microsoft.VisualStudio.Services.Identity.Identity, GraphSubject>((Func<Microsoft.VisualStudio.Services.Identity.Identity, GraphSubject>) (x => x.ToGraphSubjectClient(this.TfsRequestContext))).Where<GraphSubject>(QueryFilters.ApplySubjectKindFilter(filter2)).Take<GraphSubject>(pageSize).OfType<T>();
      objs.ForEach<T>((Action<T>) (x => x.FillSerializeInternalsField(this.TfsRequestContext)));
      return objs.ToList<T>();
    }

    private HttpResponseMessage CreateGraphSubjectResponse(
      List<T> graphSubjects,
      string continuationToken)
    {
      HttpResponseMessage response = this.Request.CreateResponse<List<T>>(HttpStatusCode.OK, graphSubjects);
      if (continuationToken != null)
        response.Headers.Add("X-MS-ContinuationToken", continuationToken);
      return response;
    }
  }
}
