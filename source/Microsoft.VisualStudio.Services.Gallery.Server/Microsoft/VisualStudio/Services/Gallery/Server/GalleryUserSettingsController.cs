// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Gallery.Server.GalleryUserSettingsController
// Assembly: Microsoft.VisualStudio.Services.Gallery.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B9EBBED5-135E-45CD-B0B4-F747360599CD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Gallery.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Gallery.WebApi;
using System.Collections.Generic;
using System.Net;
using System.Web.Http;

namespace Microsoft.VisualStudio.Services.Gallery.Server
{
  [VersionedApiControllerCustomName(Area = "gallery", ResourceName = "settings")]
  public class GalleryUserSettingsController : GalleryController
  {
    protected override void InitializeExceptionMap(ApiExceptionMapping exceptionMap)
    {
      base.InitializeExceptionMap(exceptionMap);
      exceptionMap.AddStatusCode<InvalidSettingKeyException>(HttpStatusCode.BadRequest);
    }

    [HttpGet]
    [ClientLocationId("9B75ECE3-7960-401C-848B-148AC01CA350")]
    public IDictionary<string, object> GetGalleryUserSettings(string userScope) => this.GetGalleryUserSettings(userScope, (string) null);

    [HttpGet]
    [ClientLocationId("9B75ECE3-7960-401C-848B-148AC01CA350")]
    public IDictionary<string, object> GetGalleryUserSettings(string userScope, string key) => this.TfsRequestContext.GetService<IGalleryUserSettingsService>().GetGalleryUserSettings(this.TfsRequestContext, userScope, key);

    [HttpPatch]
    [ClientLocationId("9B75ECE3-7960-401C-848B-148AC01CA350")]
    public void SetGalleryUserSettings([FromBody] IDictionary<string, object> entries, string userScope) => this.TfsRequestContext.GetService<IGalleryUserSettingsService>().SetGalleryUserSettings(this.TfsRequestContext, entries, userScope);
  }
}
