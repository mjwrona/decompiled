// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Cloud.IdentityPickerIdentitiesController
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.IdentityPicker;
using System.Collections.Generic;
using System.Net.Http;
using System.Web.Http;

namespace Microsoft.VisualStudio.Services.Cloud
{
  [ControllerApiVersion(1.0)]
  [VersionedApiControllerCustomName(Area = "IdentityPicker", ResourceName = "Identities")]
  [FeatureEnabled("VisualStudio.Services.IdentityPicker")]
  public class IdentityPickerIdentitiesController : AbstractIdentityPickerIdentitiesController
  {
    [HttpPost]
    [ClientResponseType(typeof (IdentitiesSearchResponseModel), null, null)]
    [PublicCollectionRequestRestrictions(false, true, null)]
    public override HttpResponseMessage GetIdentities([FromBody] IdentitiesSearchRequestModel identitiesRequest) => base.GetIdentities(identitiesRequest);

    [HttpGet]
    [ActionName("GetAvatar")]
    [PublicCollectionRequestRestrictions(false, true, null)]
    public override HttpResponseMessage GetAvatar(string objectId) => base.GetAvatar(objectId);

    [HttpGet]
    [ActionName("GetConnections")]
    [ClientResponseType(typeof (IdentitiesGetConnectionsResponseModel), null, null)]
    public override HttpResponseMessage GetConnections(
      string objectId,
      [FromUri] IdentitiesGetConnectionsRequestModel getRequestParams)
    {
      return base.GetConnections(objectId, getRequestParams);
    }

    [HttpGet]
    [ClientResponseType(typeof (IdentitiesGetMruResponseModel), null, null)]
    public override HttpResponseMessage GetMru(
      string objectId,
      string featureId,
      [FromUri] IdentitiesGetMruRequestModel getRequestParams)
    {
      return base.GetMru(objectId, featureId, getRequestParams);
    }

    [HttpPatch]
    [ClientResponseType(typeof (IdentitiesPatchMruResponseModel), null, null)]
    public override HttpResponseMessage PatchMru(
      string objectId,
      string featureId,
      [FromBody] IList<IdentitiesPatchMruRequestModel> patchRequestbody)
    {
      return base.PatchMru(objectId, featureId, patchRequestbody);
    }
  }
}
