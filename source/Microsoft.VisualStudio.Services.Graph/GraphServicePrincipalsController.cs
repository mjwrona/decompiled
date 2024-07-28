// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Graph.GraphServicePrincipalsController
// Assembly: Microsoft.VisualStudio.Services.Graph, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 00390AA0-D8BB-45EB-AEF5-70DC8BFC765D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Graph.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Framework.Server.WebApis;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Graph.Client;
using Microsoft.VisualStudio.Services.Identity;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web.Http;

namespace Microsoft.VisualStudio.Services.Graph
{
  [ControllerApiVersion(7.1)]
  [VersionedApiControllerCustomName(Area = "Graph", ResourceName = "ServicePrincipals", ResourceVersion = 1)]
  [RequireAadBackedOrg]
  public class GraphServicePrincipalsController : GraphMembersControllerBase<GraphServicePrincipal>
  {
    private readonly IAadServicePrincipalConfigurationHelper aadServicePrincipalConfigurationHelper;

    protected override IReadOnlyCollection<string> AllowedSubjectTypes => GraphMembersControllerBase<GraphServicePrincipal>.ServicePrincipalSubjectTypes;

    protected override string Layer => nameof (GraphServicePrincipalsController);

    public GraphServicePrincipalsController()
      : this(AadServicePrincipalConfigurationHelper.Instance)
    {
    }

    public GraphServicePrincipalsController(
      IAadServicePrincipalConfigurationHelper aadServicePrincipalConfigurationHelper)
    {
      this.aadServicePrincipalConfigurationHelper = aadServicePrincipalConfigurationHelper;
    }

    [HttpGet]
    [TraceFilter(6307670, 6307679)]
    [ClientExample("GetServicePrincipal.json", null, null, null)]
    public GraphServicePrincipal GetServicePrincipal([ClientParameterType(typeof (string), false)] SubjectDescriptor servicePrincipalDescriptor)
    {
      this.CheckIfGraphOperationsAreEnabled();
      return this.aadServicePrincipalConfigurationHelper.IsAllowedSubjectTypesEnabled(this.TfsRequestContext) ? this.GetMember(this.TfsRequestContext, servicePrincipalDescriptor) : GraphSubjectHelper.FetchGraphServicePrincipal(this.TfsRequestContext, servicePrincipalDescriptor);
    }

    [HttpGet]
    [ClientResponseType(typeof (PagedGraphServicePrincipals), null, null)]
    [TraceFilter(6307660, 6307669)]
    [ClientExample("GetAllServicePrincipals.json", null, null, null)]
    public HttpResponseMessage ListServicePrincipals(
      [FromUri] string continuationToken = null,
      [ClientParameterType(typeof (string), false), FromUri] SubjectDescriptor? scopeDescriptor = null)
    {
      this.CheckIfGraphOperationsAreEnabled();
      return this.ListMembersInternal("servicePrincipal", continuationToken: continuationToken, scopeDescriptor: scopeDescriptor);
    }

    [HttpPost]
    [ClientResponseType(typeof (GraphServicePrincipal), null, null)]
    [RequestContentTypeRestriction(AllowJsonPatch = false)]
    [ClientExample("CreateServicePrincipal.json", null, null, null)]
    public HttpResponseMessage CreateServicePrincipal(
      GraphServicePrincipalCreationContext creationContext,
      [ClientParameterAsIEnumerable(typeof (SubjectDescriptor), ',')] string groupDescriptors = null)
    {
      this.CheckIfGraphOperationsAreEnabled();
      if (creationContext == null)
        throw new GraphBadRequestException(Resources.GraphServicePrincipalMissingCreateContext());
      IEnumerable<string> localGroups = GraphSubjectHelper.CreateDedupedListOfSubjectDescriptorsFromString(groupDescriptors).Select<SubjectDescriptor, string>((Func<SubjectDescriptor, string>) (x => x.Identifier));
      return this.AddMemberInternal((IGraphMemberOriginIdCreationContext) creationContext, localGroups);
    }

    [HttpPatch]
    [ClientResponseType(typeof (GraphServicePrincipal), null, null)]
    [ClientExample("UpdateServicePrincipal.json", null, null, null)]
    [ClientInternalUseOnly(true)]
    public HttpResponseMessage UpdateServicePrincipal(
      [ClientParameterType(typeof (string), false)] SubjectDescriptor servicePrincipalDescriptor,
      [FromBody] GraphServicePrincipalUpdateContext updateContext)
    {
      this.CheckIfGraphOperationsAreEnabled();
      if (updateContext == null)
        throw new GraphBadRequestException(Resources.GraphServicePrincipalMissingUpdateContext());
      this.ValidateSubjectDescriptorType(servicePrincipalDescriptor);
      Microsoft.VisualStudio.Services.Identity.Identity currentIdentity = this.TfsRequestContext.GetService<IdentityService>().ReadIdentities(this.TfsRequestContext, (IList<SubjectDescriptor>) new SubjectDescriptor[1]
      {
        servicePrincipalDescriptor
      }, QueryMembership.None, (IEnumerable<string>) null).SingleOrDefault<Microsoft.VisualStudio.Services.Identity.Identity>();
      if (currentIdentity == null)
        throw new GraphSubjectNotFoundException(servicePrincipalDescriptor);
      this.CheckPermissionsToTransferIdentity(this.TfsRequestContext);
      return this.UpdateMemberInternal(this.TfsRequestContext, (IGraphMemberOriginIdUpdateContext) updateContext, currentIdentity);
    }

    [HttpDelete]
    [TraceFilter(6307640, 6307640)]
    [ClientResponseType(typeof (void), null, null)]
    [RequestContentTypeRestriction(AllowJsonPatch = false)]
    [ClientExample("DeleteServicePrincipal.json", null, null, null)]
    public HttpResponseMessage DeleteServicePrincipal([ClientParameterType(typeof (string), false)] SubjectDescriptor servicePrincipalDescriptor)
    {
      this.CheckIfGraphOperationsAreEnabled();
      return this.DeleteMemberInternal(servicePrincipalDescriptor);
    }

    private void CheckIfGraphOperationsAreEnabled()
    {
      if (!this.aadServicePrincipalConfigurationHelper.IsAdoGraphApiEnabled(this.TfsRequestContext))
        throw new InvalidAccessException("You are not allowed to access this feature.");
    }

    protected override void ValidateSubjectDescriptorType(SubjectDescriptor subjectDescriptor)
    {
      if (!subjectDescriptor.IsAadServicePrincipalType())
        throw new GraphBadRequestException(Resources.GraphServicePrincipalMappingToServicePrincipalsAllowedOnly());
    }
  }
}
