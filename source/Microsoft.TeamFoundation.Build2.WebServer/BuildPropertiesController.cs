// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build2.WebServer.BuildPropertiesController
// Assembly: Microsoft.TeamFoundation.Build2.WebServer, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: FDDA87C8-3548-4A75-AA18-4FB488450659
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build2.WebServer.dll

using Microsoft.TeamFoundation.Build2.Server;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.WebApi;
using Microsoft.VisualStudio.Services.WebApi.Patch;
using Microsoft.VisualStudio.Services.WebApi.Patch.Json;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Microsoft.TeamFoundation.Build2.WebServer
{
  [VersionedApiControllerCustomName(Area = "build", ResourceName = "properties")]
  public class BuildPropertiesController : BuildApiController
  {
    [HttpGet]
    [ClientResponseType(typeof (PropertiesCollection), null, null)]
    [ClientLocationId("0A6312E9-0627-49B7-8083-7D74A64849C9")]
    [CustomerIntelligence("Build", "Properties")]
    public HttpResponseMessage GetBuildProperties(int buildId, [ClientParameterAsIEnumerable(typeof (string), ',')] string filter = "*")
    {
      if (string.IsNullOrWhiteSpace(filter))
        filter = "*";
      BuildData buildById = this.InternalBuildService.GetBuildById(this.TfsRequestContext, buildId, ArtifactPropertyKinds.AsPropertyFilters(filter), true);
      if (buildById == null)
        return this.Request.CreateErrorResponse(HttpStatusCode.NotFound, Resources.BuildNotFound((object) buildId));
      this.TfsRequestContext.AddCIEntry("BuildId", (object) buildId);
      this.TfsRequestContext.AddCIEntry("PropertyCount", (object) buildById.Properties.Count);
      return this.Request.CreateResponse<PropertiesCollection>(HttpStatusCode.OK, buildById.Properties);
    }

    [HttpPatch]
    [ClientResponseType(typeof (PropertiesCollection), null, null)]
    [ClientRequestContentMediaType("application/json-patch+json")]
    [RequestContentTypeRestriction(AllowJson = false)]
    [CustomerIntelligence("Build", "Properties")]
    public HttpResponseMessage UpdateBuildProperties(
      int buildId,
      [ClientParameterType(typeof (JsonPatchDocument), false)] PatchDocument<IDictionary<string, object>> document)
    {
      PropertiesCollection properties = PropertiesCollectionPatchHelper.ReadPatchDocument((IPatchDocument<IDictionary<string, object>>) document);
      PropertiesCollection propertiesCollection = this.BuildService.UpdateProperties(this.TfsRequestContext, this.ProjectId, buildId, properties);
      this.TfsRequestContext.AddCIEntry("BuildId", (object) buildId);
      this.TfsRequestContext.AddCIEntry("PropertyCount", (object) propertiesCollection.Count);
      return this.Request.CreateResponse<PropertiesCollection>(HttpStatusCode.OK, propertiesCollection);
    }

    [HttpGet]
    [ClientResponseType(typeof (PropertiesCollection), null, null)]
    [ClientLocationId("D9826AD7-2A68-46A9-A6E9-677698777895")]
    [CustomerIntelligence("Build", "Properties")]
    public HttpResponseMessage GetDefinitionProperties(int definitionId, [ClientParameterAsIEnumerable(typeof (string), ',')] string filter = "*")
    {
      if (string.IsNullOrWhiteSpace(filter))
        filter = "*";
      BuildDefinition definition = this.DefinitionService.GetDefinition(this.TfsRequestContext, this.ProjectId, definitionId, propertyFilters: ArtifactPropertyKinds.AsPropertyFilters(filter), includeDeleted: true);
      if (definition == null)
        return this.Request.CreateErrorResponse(HttpStatusCode.NotFound, Resources.DefinitionNotFound((object) definitionId));
      this.TfsRequestContext.AddCIEntry("DefinitionId", (object) definitionId);
      this.TfsRequestContext.AddCIEntry("PropertyCount", (object) definition.Properties.Count);
      return this.Request.CreateResponse<PropertiesCollection>(HttpStatusCode.OK, definition.Properties);
    }

    [HttpPatch]
    [ClientResponseType(typeof (PropertiesCollection), null, null)]
    [ClientRequestContentMediaType("application/json-patch+json")]
    [RequestContentTypeRestriction(AllowJson = false)]
    [CustomerIntelligence("Build", "Properties")]
    [ClientExample("PATCH_build_updateDefinitionProperties.json", null, null, null)]
    public HttpResponseMessage UpdateDefinitionProperties(
      int definitionId,
      [ClientParameterType(typeof (JsonPatchDocument), false)] PatchDocument<IDictionary<string, object>> document)
    {
      PropertiesCollection properties = PropertiesCollectionPatchHelper.ReadPatchDocument((IPatchDocument<IDictionary<string, object>>) document);
      PropertiesCollection propertiesCollection = this.DefinitionService.UpdateProperties(this.TfsRequestContext, this.ProjectId, definitionId, properties);
      this.TfsRequestContext.AddCIEntry("DefinitionId", (object) definitionId);
      this.TfsRequestContext.AddCIEntry("PropertyCount", (object) propertiesCollection.Count);
      return this.Request.CreateResponse<PropertiesCollection>(HttpStatusCode.OK, propertiesCollection);
    }
  }
}
