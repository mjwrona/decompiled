// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Graph.GraphUsersController
// Assembly: Microsoft.VisualStudio.Services.Graph, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 00390AA0-D8BB-45EB-AEF5-70DC8BFC765D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Graph.dll

using Microsoft.CSharp.RuntimeBinder;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Framework.Server.ConfigFramework;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Graph.Client;
using Microsoft.VisualStudio.Services.Graph.Services;
using Microsoft.VisualStudio.Services.Identity;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Web.Http;

namespace Microsoft.VisualStudio.Services.Graph
{
  [ControllerApiVersion(3.2)]
  [VersionedApiControllerCustomName(Area = "Graph", ResourceName = "Users")]
  public class GraphUsersController : GraphMembersControllerBase<GraphUser>
  {
    private readonly IAadServicePrincipalConfigurationHelper aadServicePrincipalConfigurationHelper;
    protected static readonly IConfigPrototype<bool> UseIPagedScopedIdentityReaderPrototype = ConfigPrototype.Create<bool>("Identity.UseIPagedScopedIdentityReaderInterface.M209", false);
    private readonly IConfigQueryable<bool> UseIPagedScopedIdentityReaderConfig;

    protected override IReadOnlyCollection<string> AllowedSubjectTypes => GraphMembersControllerBase<GraphUser>.UserSubjectTypes;

    protected override string Layer => nameof (GraphUsersController);

    public GraphUsersController()
      : this(ConfigProxy.Create<bool>(GraphUsersController.UseIPagedScopedIdentityReaderPrototype), AadServicePrincipalConfigurationHelper.Instance)
    {
    }

    public GraphUsersController(
      IConfigQueryable<bool> useIPagedScopedIdentityReaderConfig,
      IAadServicePrincipalConfigurationHelper aadServicePrincipalConfigurationHelper)
    {
      this.UseIPagedScopedIdentityReaderConfig = useIPagedScopedIdentityReaderConfig;
      this.aadServicePrincipalConfigurationHelper = aadServicePrincipalConfigurationHelper;
    }

    [HttpGet]
    [TraceFilter(6307611, 6307612)]
    [ClientExample("GetUserAAD.json", null, null, null)]
    public GraphUser GetUser([ClientParameterType(typeof (string), false)] SubjectDescriptor userDescriptor)
    {
      GraphUser graphSubject = !this.aadServicePrincipalConfigurationHelper.IsAllowedSubjectTypesEnabled(this.TfsRequestContext) ? GraphSubjectHelper.FetchGraphUser(this.TfsRequestContext, userDescriptor) : this.GetMember(this.TfsRequestContext, userDescriptor);
      graphSubject.FillSerializeInternalsField(this.TfsRequestContext);
      return graphSubject;
    }

    [HttpGet]
    [TraceFilter(6307610, 6307619)]
    [ClientResponseType(typeof (PagedGraphUsers), null, null)]
    [ClientExample("GetAllUsers.json", null, null, null)]
    public HttpResponseMessage ListUsers(
      [ClientQueryParameter, ClientParameterAsIEnumerable(typeof (string), ',')] string subjectTypes = null,
      [FromUri] string continuationToken = null,
      [ClientParameterType(typeof (string), false), FromUri] SubjectDescriptor? scopeDescriptor = null)
    {
      if (this.UseIPagedScopedIdentityReaderConfig.QueryByCtx<bool>(this.TfsRequestContext))
      {
        this.TfsRequestContext.Trace(6307661, TraceLevel.Info, this.TraceArea, nameof (GraphUsersController), "UseIPagedScopedIdentityReaderInterface FF enabled - using IPagedScopedIdentityReader interface");
        return this.ListMembersInternal("user", subjectTypes, continuationToken, scopeDescriptor);
      }
      PlatformIdentityService service = this.TfsRequestContext.GetService<PlatformIdentityService>();
      Guid scopeId = !scopeDescriptor.HasValue ? this.TfsRequestContext.ServiceHost.InstanceId : scopeDescriptor.Value.GetLocalScopeId(this.TfsRequestContext);
      ScopePagingContext scopePagingContext1;
      if (continuationToken != null)
      {
        scopePagingContext1 = ScopePagingContext.FromContinuationToken(continuationToken);
      }
      else
      {
        int pageSize = PagingHelper.GetPageSize(this.TfsRequestContext);
        scopePagingContext1 = new ScopePagingContext(scopeId, pageSize, false, true);
      }
      IVssRequestContext tfsRequestContext = this.TfsRequestContext;
      ScopePagingContext scopePagingContext2 = scopePagingContext1;
      int num = this.ShouldForceFilterIdentities() ? 1 : 0;
      IdentitiesPage identitiesPage = service.ReadIdentitiesByScopeByPage(tfsRequestContext, scopePagingContext2, num != 0);
      List<GraphUser> list = QueryFilters.ApplySubjectTypeFilter(QueryFilters.ApplySubjectKindFilter(identitiesPage.Identities.Where<Microsoft.VisualStudio.Services.Identity.Identity>((Func<Microsoft.VisualStudio.Services.Identity.Identity, bool>) (x => !x.IsContainer)).Select<Microsoft.VisualStudio.Services.Identity.Identity, GraphSubject>((Func<Microsoft.VisualStudio.Services.Identity.Identity, GraphSubject>) (x => x.ToGraphSubjectClient(this.TfsRequestContext))), "user"), subjectTypes).Take<GraphSubject>(scopePagingContext1.PageSize).OfType<GraphUser>().ToList<GraphUser>();
      list.ForEach((Action<GraphUser>) (x => x.FillSerializeInternalsField(this.TfsRequestContext)));
      HttpResponseMessage response = this.Request.CreateResponse<List<GraphUser>>(HttpStatusCode.OK, list);
      if (identitiesPage.ContinuationToken != null)
        response.Headers.Add("X-MS-ContinuationToken", identitiesPage.ContinuationToken);
      return response;
    }

    [HttpPost]
    [TraceFilter(6307620, 6307629)]
    [ClientResponseType(typeof (GraphUser), null, null)]
    [RequestContentTypeRestriction(AllowJsonPatch = false)]
    [ClientExample("CreateUserAAD.json", "Add an AAD user by UPN", null, null)]
    [ClientExample("CreateUserMSA.json", "Add a MSA user by UPN", null, null)]
    [ClientExample("MaterializeAADUserByOID.json", "Add an AAD user by OID", null, null)]
    [ClientExample("MaterializeAADUserByOIDAsMember.json", "Add an AAD user as member of a group", null, null)]
    [ClientExample("MaterializeAADUserByOIDWithStorageKey.json", "Add an AAD user with a custom storage key", null, null)]
    public HttpResponseMessage CreateUser(
      GraphUserCreationContext creationContext,
      [ClientParameterAsIEnumerable(typeof (SubjectDescriptor), ',')] string groupDescriptors = null)
    {
      if (creationContext == null)
        throw new GraphBadRequestException(Resources.GraphUserMissingRequiredFields());
      IEnumerable<string> strings = GraphSubjectHelper.CreateDedupedListOfSubjectDescriptorsFromString(groupDescriptors).Select<SubjectDescriptor, string>((Func<SubjectDescriptor, string>) (x => x.Identifier));
      // ISSUE: reference to a compiler-generated field
      if (GraphUsersController.\u003C\u003Eo__11.\u003C\u003Ep__1 == null)
      {
        // ISSUE: reference to a compiler-generated field
        GraphUsersController.\u003C\u003Eo__11.\u003C\u003Ep__1 = CallSite<Func<CallSite, object, HttpResponseMessage>>.Create(Binder.Convert(CSharpBinderFlags.None, typeof (HttpResponseMessage), typeof (GraphUsersController)));
      }
      // ISSUE: reference to a compiler-generated field
      Func<CallSite, object, HttpResponseMessage> target = GraphUsersController.\u003C\u003Eo__11.\u003C\u003Ep__1.Target;
      // ISSUE: reference to a compiler-generated field
      CallSite<Func<CallSite, object, HttpResponseMessage>> p1 = GraphUsersController.\u003C\u003Eo__11.\u003C\u003Ep__1;
      // ISSUE: reference to a compiler-generated field
      if (GraphUsersController.\u003C\u003Eo__11.\u003C\u003Ep__0 == null)
      {
        // ISSUE: reference to a compiler-generated field
        GraphUsersController.\u003C\u003Eo__11.\u003C\u003Ep__0 = CallSite<Func<CallSite, GraphUsersController, object, IEnumerable<string>, object>>.Create(Binder.InvokeMember(CSharpBinderFlags.InvokeSimpleName, "AddMemberInternal", (IEnumerable<Type>) null, typeof (GraphUsersController), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[3]
        {
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType, (string) null),
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null),
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType, (string) null)
        }));
      }
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      object obj = GraphUsersController.\u003C\u003Eo__11.\u003C\u003Ep__0.Target((CallSite) GraphUsersController.\u003C\u003Eo__11.\u003C\u003Ep__0, this, (object) creationContext, strings);
      return target((CallSite) p1, obj);
    }

    [HttpPatch]
    [TraceFilter(6307650, 6307659)]
    [ClientResponseType(typeof (GraphUser), null, null)]
    public HttpResponseMessage UpdateUser(
      [ClientParameterType(typeof (string), false)] SubjectDescriptor userDescriptor,
      [FromBody] GraphUserUpdateContext updateContext)
    {
      if (updateContext == null)
        throw new GraphBadRequestException(Resources.GraphUserMissingRequiredFields());
      this.ValidateSubjectDescriptorType(userDescriptor);
      Microsoft.VisualStudio.Services.Identity.Identity currentIdentity = this.TfsRequestContext.GetService<IdentityService>().ReadIdentities(this.TfsRequestContext, (IList<SubjectDescriptor>) new SubjectDescriptor[1]
      {
        userDescriptor
      }, QueryMembership.None, (IEnumerable<string>) null).SingleOrDefault<Microsoft.VisualStudio.Services.Identity.Identity>();
      if (currentIdentity == null)
        throw new GraphSubjectNotFoundException(userDescriptor);
      this.CheckPermissionsToTransferIdentity(this.TfsRequestContext);
      if (this.TfsRequestContext.IsFeatureEnabled("VisualStudio.Services.Graph.UpdateUserInternal.Refactored.Enable"))
        this.TfsRequestContext.GetService<GraphUsersService>().UpdateUserInternal(this.TfsRequestContext, (GraphUserOriginIdUpdateContext) updateContext, currentIdentity);
      // ISSUE: reference to a compiler-generated field
      if (GraphUsersController.\u003C\u003Eo__12.\u003C\u003Ep__1 == null)
      {
        // ISSUE: reference to a compiler-generated field
        GraphUsersController.\u003C\u003Eo__12.\u003C\u003Ep__1 = CallSite<Func<CallSite, object, HttpResponseMessage>>.Create(Binder.Convert(CSharpBinderFlags.None, typeof (HttpResponseMessage), typeof (GraphUsersController)));
      }
      // ISSUE: reference to a compiler-generated field
      Func<CallSite, object, HttpResponseMessage> target = GraphUsersController.\u003C\u003Eo__12.\u003C\u003Ep__1.Target;
      // ISSUE: reference to a compiler-generated field
      CallSite<Func<CallSite, object, HttpResponseMessage>> p1 = GraphUsersController.\u003C\u003Eo__12.\u003C\u003Ep__1;
      // ISSUE: reference to a compiler-generated field
      if (GraphUsersController.\u003C\u003Eo__12.\u003C\u003Ep__0 == null)
      {
        // ISSUE: reference to a compiler-generated field
        GraphUsersController.\u003C\u003Eo__12.\u003C\u003Ep__0 = CallSite<Func<CallSite, GraphUsersController, IVssRequestContext, object, Microsoft.VisualStudio.Services.Identity.Identity, object>>.Create(Binder.InvokeMember(CSharpBinderFlags.InvokeSimpleName, "UpdateMemberInternal", (IEnumerable<Type>) null, typeof (GraphUsersController), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[4]
        {
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType, (string) null),
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType, (string) null),
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null),
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType, (string) null)
        }));
      }
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      object obj = GraphUsersController.\u003C\u003Eo__12.\u003C\u003Ep__0.Target((CallSite) GraphUsersController.\u003C\u003Eo__12.\u003C\u003Ep__0, this, this.TfsRequestContext, (object) updateContext, currentIdentity);
      return target((CallSite) p1, obj);
    }

    [HttpDelete]
    [TraceFilter(6307630, 6307639)]
    [ClientResponseType(typeof (void), null, null)]
    [RequestContentTypeRestriction(AllowJsonPatch = false)]
    [ClientExample("DeleteUserAAD.json", null, null, null)]
    public HttpResponseMessage DeleteUser([ClientParameterType(typeof (string), false)] SubjectDescriptor userDescriptor) => this.DeleteMemberInternal(userDescriptor);

    protected override void ValidateSubjectDescriptorType(SubjectDescriptor subjectDescriptor)
    {
      if (subjectDescriptor.IsAadServicePrincipalType())
        throw new GraphBadRequestException(Resources.GraphUserServicePrincipalsMappingNotAllowed());
    }
  }
}
