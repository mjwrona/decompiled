// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Gallery.Server.CVS.CVSController
// Assembly: Microsoft.VisualStudio.Services.Gallery.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B9EBBED5-135E-45CD-B0B4-F747360599CD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Gallery.Server.dll

using Microsoft.Ops.Cvs.Client;
using Microsoft.Ops.Cvs.Client.DataContracts;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.WebApi;
using Microsoft.VisualStudio.Services.WebApi.Internal;
using System;
using System.Diagnostics;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Microsoft.VisualStudio.Services.Gallery.Server.CVS
{
  [ControllerApiVersion(4.1)]
  [VersionedApiControllerCustomName(Area = "gallery", ResourceName = "cvsscanitem")]
  [ClientIgnore]
  [RequestContentTypeRestriction(AllowJsonAPIContent = true)]
  public class CVSController : GalleryController
  {
    private const string s_layer = "CVSController";

    [HttpPost]
    [ClientResponseType(typeof (void), null, null)]
    public HttpResponseMessage UpdateScanStatus(Guid scanId, Guid itemId)
    {
      string str = this.Request.Content.ReadAsStringAsync().SyncResult<string>();
      if (!string.IsNullOrWhiteSpace(str))
      {
        try
        {
          this.TfsRequestContext.GetService<ICVSService>().UpdateItemScanStatus(this.TfsRequestContext, scanId, itemId, new CVSScanResponse(CvsConvert.DeserializeObject<Job>(str)));
        }
        catch (Exception ex)
        {
          this.TfsRequestContext.TraceException(12061123, "gallery", nameof (CVSController), ex);
        }
      }
      else
        this.TfsRequestContext.TraceAlways(12061123, TraceLevel.Error, "gallery", nameof (CVSController), "Null Jobdata in controller call ScanId: {0}, ItemId: {1}", (object) scanId, (object) itemId);
      return this.Request.CreateResponse(HttpStatusCode.Accepted);
    }
  }
}
