// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Graph.GraphGroupsController
// Assembly: Microsoft.VisualStudio.Services.Graph, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 00390AA0-D8BB-45EB-AEF5-70DC8BFC765D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Graph.dll

using Microsoft.CSharp.RuntimeBinder;
using Microsoft.TeamFoundation.Common;
using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Directories;
using Microsoft.VisualStudio.Services.Directories.DirectoryService;
using Microsoft.VisualStudio.Services.Directories.DirectoryService.Components;
using Microsoft.VisualStudio.Services.Graph.Client;
using Microsoft.VisualStudio.Services.Identity;
using Microsoft.VisualStudio.Services.WebApi.Patch;
using Microsoft.VisualStudio.Services.WebApi.Patch.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Security.Principal;
using System.Web.Http;

namespace Microsoft.VisualStudio.Services.Graph
{
  [ControllerApiVersion(3.2)]
  [VersionedApiControllerCustomName(Area = "Graph", ResourceName = "Groups")]
  public class GraphGroupsController : GraphControllerBase
  {
    protected readonly HashSet<string> ValidPatchPaths = new HashSet<string>()
    {
      "displayName",
      "description"
    };

    [HttpGet]
    [TraceFilter(6307000, 6307009)]
    [ClientExample("GetGroup-AddRemoveAADGroupByOID.json", null, null, null)]
    public GraphGroup GetGroup([ClientParameterType(typeof (string), false)] SubjectDescriptor groupDescriptor)
    {
      Microsoft.VisualStudio.Services.Identity.Identity identity = GraphSubjectHelper.FetchSingleIdentityByDescriptor(this.TfsRequestContext, groupDescriptor);
      if (!((identity != null ? identity.ToGraphSubjectClient(this.TfsRequestContext) : (GraphSubject) null) is GraphGroup graphSubjectClient))
        throw new GraphSubjectNotFoundException(groupDescriptor);
      graphSubjectClient.FillSerializeInternalsField(this.TfsRequestContext);
      return graphSubjectClient;
    }

    [HttpGet]
    [TraceFilter(6307010, 6307019)]
    [ClientResponseType(typeof (PagedGraphGroups), null, null)]
    [ClientExample("GetAllGroups.json", null, null, null)]
    public HttpResponseMessage ListGroups(
      [ClientParameterType(typeof (string), false), FromUri] SubjectDescriptor? scopeDescriptor = null,
      [ClientParameterAsIEnumerable(typeof (string), ',')] string subjectTypes = null,
      [FromUri] string continuationToken = null)
    {
      Guid scopeId = !scopeDescriptor.HasValue ? this.TfsRequestContext.ServiceHost.InstanceId : scopeDescriptor.Value.GetLocalScopeId(this.TfsRequestContext);
      ScopePagingContext scopePagingContext;
      if (continuationToken != null)
      {
        scopePagingContext = ScopePagingContext.FromContinuationToken(continuationToken);
      }
      else
      {
        int pageSize = PagingHelper.GetPageSize(this.TfsRequestContext);
        scopePagingContext = new ScopePagingContext(scopeId, pageSize, true, false);
      }
      IdentitiesPage identitiesPage = this.TfsRequestContext.GetService<PlatformIdentityService>().ReadIdentitiesByScopeByPage(this.TfsRequestContext, scopePagingContext, this.ShouldForceFilterIdentities());
      List<GraphGroup> list = GraphGroupsController.ApplyFilters(identitiesPage.Identities.Select<Microsoft.VisualStudio.Services.Identity.Identity, GraphSubject>((Func<Microsoft.VisualStudio.Services.Identity.Identity, GraphSubject>) (x => x.ToGraphSubjectClient(this.TfsRequestContext))), "group", subjectTypes).Take<GraphSubject>(scopePagingContext.PageSize).OfType<GraphGroup>().ToList<GraphGroup>();
      list.ForEach((Action<GraphGroup>) (x => x.FillSerializeInternalsField(this.TfsRequestContext)));
      HttpResponseMessage response = this.Request.CreateResponse<List<GraphGroup>>(HttpStatusCode.OK, list);
      if (identitiesPage.ContinuationToken != null)
        response.Headers.Add("X-MS-ContinuationToken", identitiesPage.ContinuationToken);
      return response;
    }

    [HttpPost]
    [TraceFilter(6307020, 6307029)]
    [ClientResponseType(typeof (GraphGroup), null, null)]
    [RequestContentTypeRestriction(AllowJsonPatch = false)]
    [ClientExample("CreateGroup.json", "Create a Group at the account level", null, null)]
    [ClientExample("CreateGroupInProject.json", "Create a Group at the project level", null, null)]
    [ClientExample("MaterializeAADGroupByOID.json", "Add an AAD Group by OID", null, null)]
    [ClientExample("MaterializeAADGroupByOIDAsMember.json", "Add an AAD Group as member of a group", null, null)]
    [ClientExample("MaterializeAADGroupByOIDWithStorageKey.json", "Add an AAD Group with a custom storage key", null, null)]
    public HttpResponseMessage CreateGroup(
      GraphGroupCreationContext creationContext,
      [ClientParameterType(typeof (string), false), FromUri] SubjectDescriptor? scopeDescriptor = null,
      [ClientParameterAsIEnumerable(typeof (SubjectDescriptor), ',')] string groupDescriptors = null)
    {
      if (creationContext == null)
        throw new GraphBadRequestException(Resources.GraphGroupMissingRequiredFields());
      IEnumerable<string> strings = GraphSubjectHelper.CreateDedupedListOfSubjectDescriptorsFromString(groupDescriptors).Select<SubjectDescriptor, string>((Func<SubjectDescriptor, string>) (x => x.Identifier));
      // ISSUE: reference to a compiler-generated field
      if (GraphGroupsController.\u003C\u003Eo__2.\u003C\u003Ep__1 == null)
      {
        // ISSUE: reference to a compiler-generated field
        GraphGroupsController.\u003C\u003Eo__2.\u003C\u003Ep__1 = CallSite<Func<CallSite, object, HttpResponseMessage>>.Create(Binder.Convert(CSharpBinderFlags.None, typeof (HttpResponseMessage), typeof (GraphGroupsController)));
      }
      // ISSUE: reference to a compiler-generated field
      Func<CallSite, object, HttpResponseMessage> target = GraphGroupsController.\u003C\u003Eo__2.\u003C\u003Ep__1.Target;
      // ISSUE: reference to a compiler-generated field
      CallSite<Func<CallSite, object, HttpResponseMessage>> p1 = GraphGroupsController.\u003C\u003Eo__2.\u003C\u003Ep__1;
      // ISSUE: reference to a compiler-generated field
      if (GraphGroupsController.\u003C\u003Eo__2.\u003C\u003Ep__0 == null)
      {
        // ISSUE: reference to a compiler-generated field
        GraphGroupsController.\u003C\u003Eo__2.\u003C\u003Ep__0 = CallSite<Func<CallSite, GraphGroupsController, object, SubjectDescriptor?, IEnumerable<string>, object>>.Create(Binder.InvokeMember(CSharpBinderFlags.InvokeSimpleName, "CreateGroupInternal", (IEnumerable<System.Type>) null, typeof (GraphGroupsController), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[4]
        {
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType, (string) null),
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null),
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType, (string) null),
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType, (string) null)
        }));
      }
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      object obj = GraphGroupsController.\u003C\u003Eo__2.\u003C\u003Ep__0.Target((CallSite) GraphGroupsController.\u003C\u003Eo__2.\u003C\u003Ep__0, this, (object) creationContext, scopeDescriptor, strings);
      return target((CallSite) p1, obj);
    }

    [HttpPatch]
    [ClientRequestContentMediaType("application/json-patch+json")]
    [RequestContentTypeRestriction(AllowJson = false)]
    [ClientResponseType(typeof (GraphGroup), null, null)]
    [ClientEnforceBodyParameterToComeBeforeQueryParameters(false)]
    [TraceFilter(6307030, 6307039)]
    [ClientExample("UpdateGroup.json", null, null, null)]
    public HttpResponseMessage UpdateGroup(
      [ClientParameterType(typeof (string), false)] SubjectDescriptor groupDescriptor,
      [ClientParameterType(typeof (JsonPatchDocument), false)] PatchDocument<IDictionary<string, object>> patchDocument)
    {
      JsonPatchDocumentHelper.ValidateUpdatePatchDocument<IDictionary<string, object>>(patchDocument, this.ValidPatchPaths);
      Microsoft.VisualStudio.Services.Identity.Identity identity = GraphSubjectHelper.FetchSingleIdentityByDescriptor(this.TfsRequestContext, groupDescriptor, throwIfNotFound: true);
      if (!identity.IsContainer)
        throw new GraphBadRequestException(Resources.SubjectDoesNotMatchType((object) groupDescriptor, (object) "group"));
      if (!GraphGroupsController.IsTeamFoundationGroup(identity))
        throw new GraphBadRequestException(Resources.UpdateSourceTenant((object) groupDescriptor));
      if (SidIdentityHelper.IsWellKnownSid(identity.Descriptor.Identifier))
        throw new CannotUpdateWellKnownGraphGroupException(Resources.GroupCannotBeModified());
      IDictionary<string, string> propertiesToUpdate;
      JsonPatchDocumentHelper.ParseUpdatePropertiesPatchDocument(patchDocument, out propertiesToUpdate);
      if (propertiesToUpdate.ContainsKey("displayName"))
      {
        if (this.TfsRequestContext.IsFeatureEnabled("VisualStudio.Services.Identity.EnableGraphEnhancedValidation"))
        {
          string groupName = propertiesToUpdate["displayName"];
          TFCommonUtil.CheckGroupName(ref groupName);
        }
        identity.SetProperty("Account", (object) propertiesToUpdate["displayName"]);
      }
      if (propertiesToUpdate.ContainsKey("Description"))
      {
        if (this.TfsRequestContext.IsFeatureEnabled("VisualStudio.Services.Identity.EnableGraphEnhancedValidation"))
        {
          string groupDescription = propertiesToUpdate["description"];
          TFCommonUtil.CheckGroupDescription(ref groupDescription);
        }
        identity.SetProperty("Description", (object) propertiesToUpdate["description"]);
      }
      this.TfsRequestContext.GetService<IdentityService>().UpdateIdentities(this.TfsRequestContext, (IList<Microsoft.VisualStudio.Services.Identity.Identity>) new List<Microsoft.VisualStudio.Services.Identity.Identity>()
      {
        identity
      });
      GraphGroup graphGroupClient = GraphSubjectHelper.FetchSingleIdentityByDescriptor(this.TfsRequestContext, groupDescriptor, throwIfNotFound: true).ToGraphGroupClient(this.TfsRequestContext);
      graphGroupClient.FillSerializeInternalsField(this.TfsRequestContext);
      return this.Request.CreateResponse<GraphGroup>(HttpStatusCode.OK, graphGroupClient);
    }

    [HttpDelete]
    [TraceFilter(6307040, 6307049)]
    [ClientResponseType(typeof (void), null, null)]
    [RequestContentTypeRestriction(AllowJsonPatch = false)]
    [ClientExample("DeleteGroup.json", null, null, null)]
    public HttpResponseMessage DeleteGroup([ClientParameterType(typeof (string), false)] SubjectDescriptor groupDescriptor)
    {
      this.TfsRequestContext.GetService<IdentityService>().DeleteGroup(this.TfsRequestContext, groupDescriptor);
      return new HttpResponseMessage(HttpStatusCode.NoContent);
    }

    private static IEnumerable<GraphSubject> ApplyFilters(
      IEnumerable<GraphSubject> graphSubjects,
      string subjectKind,
      string subjectTypes)
    {
      graphSubjects = QueryFilters.ApplySubjectKindFilter(graphSubjects, subjectKind);
      graphSubjects = QueryFilters.ApplySubjectTypeFilter(graphSubjects, subjectTypes);
      return graphSubjects;
    }

    private HttpResponseMessage CreateGroupInternal(
      GraphGroupOriginIdCreationContext creationContext,
      SubjectDescriptor? scopeDescriptor,
      IEnumerable<string> localGroups)
    {
      BusinessRulesValidator.ValidateGraphGroupCreationContext(this.TfsRequestContext, creationContext);
      if (scopeDescriptor.HasValue)
        throw new GraphBadRequestException(Resources.OnlyVstsGroupsInCustomScope());
      string lowerCase = "OriginId".FirstLetterToLowerCase();
      return this.ParseMaterializeGroupResult(this.MaterializeExternalGroup(this.TfsRequestContext, creationContext.ToDirectoryEntityDescriptor(), localGroups), lowerCase, creationContext.OriginId);
    }

    private HttpResponseMessage CreateGroupInternal(
      GraphGroupMailAddressCreationContext creationContext,
      SubjectDescriptor? scopeDescriptor,
      IEnumerable<string> localGroups)
    {
      BusinessRulesValidator.ValidateGraphGroupCreationContext(this.TfsRequestContext, creationContext);
      if (scopeDescriptor.HasValue)
        throw new GraphBadRequestException(Resources.OnlyVstsGroupsInCustomScope());
      string lowerCase = "MailAddress".FirstLetterToLowerCase();
      return this.ParseMaterializeGroupResult(this.MaterializeExternalGroup(this.TfsRequestContext, creationContext.ToDirectoryEntityDescriptor(), localGroups), lowerCase, creationContext.MailAddress);
    }

    private IdentityDirectoryEntityResult<Microsoft.VisualStudio.Services.Identity.Identity> MaterializeExternalGroup(
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

    private HttpResponseMessage ParseMaterializeGroupResult(
      IdentityDirectoryEntityResult<Microsoft.VisualStudio.Services.Identity.Identity> result,
      string parameterName,
      string parameterValue)
    {
      string status = result.Status;
      HttpResponseMessage response;
      if (status != null)
      {
        switch (status.Length)
        {
          case 7:
            if (status == "Success")
            {
              if (result.Identity == null)
              {
                response = this.Request.CreateResponse<string>(HttpStatusCode.InternalServerError, result.Exception?.ToString());
                goto label_23;
              }
              else
              {
                GraphSubject graphSubjectClient = result.Identity.ToGraphSubjectClient(this.TfsRequestContext);
                graphSubjectClient.FillSerializeInternalsField(this.TfsRequestContext);
                response = this.Request.CreateResponse<GraphSubject>(HttpStatusCode.Created, graphSubjectClient);
                response.Headers.Add("Location", graphSubjectClient.Url);
                goto label_23;
              }
            }
            else
              goto label_22;
          case 9:
            if (status == "NoResults")
            {
              response = this.Request.CreateResponse<string>(HttpStatusCode.BadRequest, Resources.CannotMaterializeGraphSubject((object) parameterValue, (object) parameterName));
              goto label_23;
            }
            else
              goto label_22;
          case 13:
            switch (status[0])
            {
              case 'A':
                if (status == "AadReadFailed")
                  break;
                goto label_22;
              case 'C':
                if (status == "ConvertFailed")
                  goto label_19;
                else
                  goto label_22;
              case 'V':
                if (status == "VsdReadFailed")
                  break;
                goto label_22;
              default:
                goto label_22;
            }
            break;
          case 14:
            switch (status[0])
            {
              case 'T':
                if (status == "TooManyResults")
                  goto label_20;
                else
                  goto label_22;
              case 'V':
                if (status == "VsdWriteFailed")
                  break;
                goto label_22;
              default:
                goto label_22;
            }
            break;
          case 21:
            if (status == "IdTranslationConflict")
              goto label_19;
            else
              goto label_22;
          case 22:
            if (status == "InvalidLocalDescriptor")
              goto label_20;
            else
              goto label_22;
          case 25:
            if (status == "ProtectedIdentityConflict")
              goto label_19;
            else
              goto label_22;
          case 26:
            if (status == "InvalidPermissionsProperty")
              goto label_20;
            else
              goto label_22;
          default:
            goto label_22;
        }
        response = this.Request.CreateResponse<string>(HttpStatusCode.ServiceUnavailable, result.Exception?.ToString());
        goto label_23;
label_19:
        response = this.Request.CreateResponse<string>(HttpStatusCode.Conflict, result.Exception?.ToString());
        goto label_23;
label_20:
        response = this.Request.CreateResponse<string>(HttpStatusCode.BadRequest, result.Exception?.ToString());
        goto label_23;
      }
label_22:
      response = this.Request.CreateResponse<string>(HttpStatusCode.InternalServerError, result.Exception?.ToString());
label_23:
      return response;
    }

    private HttpResponseMessage CreateGroupInternal(
      GraphGroupVstsCreationContext creationContext,
      SubjectDescriptor? scopeDescriptor,
      IEnumerable<string> localGroups)
    {
      BusinessRulesValidator.ValidateGraphGroupCreationContext(this.TfsRequestContext, creationContext);
      Guid parentScopeId = Guid.Empty;
      if (scopeDescriptor.HasValue)
      {
        SubjectDescriptor subjectDescriptor = scopeDescriptor.Value;
        ArgumentValidator.CheckDescriptorIsScopeSubjectKind(subjectDescriptor);
        parentScopeId = subjectDescriptor.GetLocalScopeId(this.TfsRequestContext);
      }
      SpecialGroupType specialType = SpecialGroupType.Generic;
      if (creationContext.SpecialGroupType != null)
        specialType = EnumUtility.Parse<SpecialGroupType>(creationContext.SpecialGroupType, true);
      string groupSid = (string) null;
      SubjectDescriptor descriptor = creationContext.Descriptor;
      if (descriptor.Identifier != null)
      {
        try
        {
          descriptor = creationContext.Descriptor;
          SecurityIdentifier securityIdentifier = new SecurityIdentifier(descriptor.Identifier);
          descriptor = creationContext.Descriptor;
          groupSid = descriptor.Identifier;
        }
        catch (Exception ex)
        {
          throw new GraphBadRequestException(Resources.IllegalSid((object) creationContext.Descriptor.Identifier), ex);
        }
      }
      Microsoft.VisualStudio.Services.Identity.Identity identity = (Microsoft.VisualStudio.Services.Identity.Identity) null;
      Guid groupId = creationContext.StorageKey != Guid.Empty ? creationContext.StorageKey : Guid.NewGuid();
      IdentityService service = this.TfsRequestContext.GetService<IdentityService>();
      if (parentScopeId.Equals(Guid.Empty))
        parentScopeId = service.DomainId;
      try
      {
        identity = service.CreateGroup(this.TfsRequestContext, parentScopeId, groupId, groupSid, creationContext.DisplayName, creationContext.Description, specialType, !creationContext.CrossProject, creationContext.RestrictedVisibility);
      }
      catch (GroupCreationException ex)
      {
        if (groupSid != null)
          identity = service.ReadIdentities(this.TfsRequestContext, (IList<SubjectDescriptor>) new SubjectDescriptor[1]
          {
            creationContext.Descriptor
          }, QueryMembership.None, (IEnumerable<string>) null).SingleOrDefault<Microsoft.VisualStudio.Services.Identity.Identity>();
        else if (!string.IsNullOrEmpty(creationContext.DisplayName))
          identity = service.ReadIdentities(this.TfsRequestContext, IdentitySearchFilter.DisplayName, creationContext.DisplayName, QueryMembership.None, (IEnumerable<string>) null).Where<Microsoft.VisualStudio.Services.Identity.Identity>((Func<Microsoft.VisualStudio.Services.Identity.Identity, bool>) (group =>
          {
            if (group == null)
              return false;
            return group.LocalScopeId.Equals(parentScopeId) || group.GetProperty<Guid>("ScopeId", Guid.Empty).Equals(parentScopeId);
          })).SingleOrDefault<Microsoft.VisualStudio.Services.Identity.Identity>();
        if (identity == null)
          throw;
      }
      foreach (string identifier in localGroups.AsEmptyIfNull<string>())
      {
        SubjectDescriptor groupDescriptor = new SubjectDescriptor("vssgp", identifier);
        service.AddMemberToGroup(this.TfsRequestContext, groupDescriptor, identity.SubjectDescriptor);
      }
      GraphSubject graphSubjectClient = identity.ToGraphSubjectClient(this.TfsRequestContext);
      graphSubjectClient.FillSerializeInternalsField(this.TfsRequestContext);
      HttpResponseMessage response = this.Request.CreateResponse<GraphSubject>(HttpStatusCode.Created, graphSubjectClient);
      response.Headers.Add("Location", graphSubjectClient.Url);
      return response;
    }

    private HttpResponseMessage CreateGroupInternal(
      GraphGroupCreationContext creationContext,
      string scopeDescriptor,
      IEnumerable<string> localGroups)
    {
      throw new InvalidOperationException();
    }

    private static bool IsTeamFoundationGroup(Microsoft.VisualStudio.Services.Identity.Identity identity) => identity?.Descriptor != (IdentityDescriptor) null && identity.IsContainer && string.Equals(identity.Descriptor.IdentityType, "Microsoft.TeamFoundation.Identity", StringComparison.OrdinalIgnoreCase) && string.Equals(identity.GetProperty<string>("SchemaClassName", (string) null), "Group");
  }
}
