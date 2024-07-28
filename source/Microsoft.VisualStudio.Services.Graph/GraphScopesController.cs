// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Graph.GraphScopesController
// Assembly: Microsoft.VisualStudio.Services.Graph, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 00390AA0-D8BB-45EB-AEF5-70DC8BFC765D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Graph.dll

using Microsoft.TeamFoundation.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Graph.Client;
using Microsoft.VisualStudio.Services.Identity;
using Microsoft.VisualStudio.Services.WebApi;
using Microsoft.VisualStudio.Services.WebApi.Patch;
using Microsoft.VisualStudio.Services.WebApi.Patch.Json;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Microsoft.VisualStudio.Services.Graph
{
  [ControllerApiVersion(3.2)]
  [ClientInternalUseOnly(true)]
  [RestrictInternalGraphEndpoints]
  [VersionedApiControllerCustomName(Area = "Graph", ResourceName = "Scopes")]
  public class GraphScopesController : GraphControllerBase
  {
    protected readonly HashSet<string> ValidPatchPaths = new HashSet<string>()
    {
      "name"
    };

    [HttpGet]
    [TraceFilter(6307400, 6307409)]
    public GraphScope GetScope([ClientParameterType(typeof (string), false)] SubjectDescriptor scopeDescriptor)
    {
      ArgumentValidator.CheckSubjectType(scopeDescriptor, "scp");
      GraphScope graphScopeClient = (GraphSubjectHelper.FetchIdentityScope(this.TfsRequestContext, scopeDescriptor) ?? throw new GraphSubjectNotFoundException(scopeDescriptor)).ToGraphScopeClient(this.TfsRequestContext);
      graphScopeClient.FillSerializeInternalsField(this.TfsRequestContext);
      return graphScopeClient;
    }

    [HttpPost]
    [TraceFilter(6307410, 6307419)]
    [ClientResponseType(typeof (GraphScope), null, null)]
    [RequestContentTypeRestriction(AllowJsonPatch = false)]
    public HttpResponseMessage CreateScope(
      GraphScopeCreationContext creationContext,
      [ClientParameterType(typeof (string), false)] SubjectDescriptor? scopeDescriptor = null)
    {
      BusinessRulesValidator.ValidateGraphScopeCreationContext(this.TfsRequestContext, creationContext);
      IdentityService service = this.TfsRequestContext.GetService<IdentityService>();
      IdentityScope identityScope;
      if (scopeDescriptor.HasValue)
      {
        SubjectDescriptor subjectDescriptor = scopeDescriptor.Value;
        ArgumentValidator.CheckDescriptorIsScopeSubjectKind(subjectDescriptor);
        identityScope = GraphSubjectHelper.FetchIdentityScope(this.TfsRequestContext, subjectDescriptor);
        if (identityScope == null)
          throw new GraphSubjectNotFoundException(subjectDescriptor);
      }
      else
      {
        Guid instanceId = this.TfsRequestContext.ServiceHost.InstanceId;
        identityScope = service.GetScope(this.TfsRequestContext, instanceId);
        if (identityScope == null)
          throw new GroupScopeDoesNotExistException(instanceId);
      }
      Guid scopeId = creationContext.StorageKey == Guid.Empty ? Guid.NewGuid() : creationContext.StorageKey;
      GraphScope graphScopeClient = service.CreateScope(this.TfsRequestContext, scopeId, identityScope.Id, creationContext.ScopeType, creationContext.Name, creationContext.AdminGroupName ?? creationContext.Name + " Administrators Group", creationContext.AdminGroupDescription, creationContext.CreatorId).ToGraphScopeClient(this.TfsRequestContext);
      graphScopeClient.FillSerializeInternalsField(this.TfsRequestContext);
      return this.Request.CreateResponse<GraphSubject>(HttpStatusCode.Created, (GraphSubject) graphScopeClient);
    }

    [HttpPatch]
    [TraceFilter(6307420, 6307429)]
    [ClientRequestContentMediaType("application/json-patch+json")]
    [RequestContentTypeRestriction(AllowJson = false)]
    [ClientResponseType(typeof (void), null, null)]
    [ClientEnforceBodyParameterToComeBeforeQueryParameters(false)]
    public HttpResponseMessage UpdateScope(
      [ClientParameterType(typeof (string), false)] SubjectDescriptor scopeDescriptor,
      [ClientParameterType(typeof (JsonPatchDocument), false)] PatchDocument<IDictionary<string, object>> patchDocument)
    {
      ArgumentValidator.CheckSubjectType(scopeDescriptor, "scp");
      JsonPatchDocumentHelper.ValidateUpdatePatchDocument<IDictionary<string, object>>(patchDocument, this.ValidPatchPaths);
      IdentityScope identityScope = GraphSubjectHelper.FetchIdentityScope(this.TfsRequestContext, scopeDescriptor);
      if (identityScope == null)
        throw new GraphSubjectNotFoundException(scopeDescriptor);
      IDictionary<string, string> propertiesToUpdate;
      JsonPatchDocumentHelper.ParseUpdatePropertiesPatchDocument(patchDocument, out propertiesToUpdate);
      if (propertiesToUpdate.ContainsKey("name"))
      {
        if (this.TfsRequestContext.IsFeatureEnabled("VisualStudio.Services.Identity.EnableGraphEnhancedValidation"))
        {
          string groupName = propertiesToUpdate["name"];
          TFCommonUtil.CheckGroupName(ref groupName);
        }
        this.TfsRequestContext.GetService<IdentityService>().RenameScope(this.TfsRequestContext, identityScope.Id, propertiesToUpdate["name"]);
      }
      return new HttpResponseMessage(HttpStatusCode.OK);
    }

    [HttpDelete]
    [TraceFilter(630730, 6307439)]
    [ClientResponseType(typeof (void), null, null)]
    [RequestContentTypeRestriction(AllowJsonPatch = false)]
    public HttpResponseMessage DeleteScope([ClientParameterType(typeof (string), false)] SubjectDescriptor scopeDescriptor)
    {
      ArgumentValidator.CheckSubjectType(scopeDescriptor, "scp");
      Guid localScopeId = scopeDescriptor.GetLocalScopeId(this.TfsRequestContext);
      this.TfsRequestContext.GetService<IdentityService>().DeleteScope(this.TfsRequestContext, localScopeId);
      return new HttpResponseMessage(HttpStatusCode.NoContent);
    }
  }
}
