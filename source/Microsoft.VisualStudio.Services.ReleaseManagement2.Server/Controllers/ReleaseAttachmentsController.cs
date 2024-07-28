// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Server.Controllers.ReleaseAttachmentsController
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 134B1041-BFA6-49C6-8C6D-CA5ADF31AF54
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Server.dll

using Microsoft.TeamFoundation.DistributedTask.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.Types;
using Microsoft.VisualStudio.Services.ReleaseManagement.Common.Security;
using Microsoft.VisualStudio.Services.ReleaseManagement.Server.Extensions;
using Microsoft.VisualStudio.Services.ReleaseManagement.Server.Security;
using Microsoft.VisualStudio.Services.ReleaseManagement.Server.Services;
using Microsoft.VisualStudio.Services.ReleaseManagement.WebApi;
using Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Security;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web.Http;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Server.Controllers
{
  [ControllerApiVersion(4.1)]
  [VersionedApiControllerCustomName(Area = "Release", ResourceName = "attachments")]
  public class ReleaseAttachmentsController : ReleaseManagementProjectControllerBase
  {
    [HttpGet]
    [PublicProjectRequestRestrictions]
    [ClientLocationId("a4d06688-0dfa-4895-82a5-f43ec9452306")]
    [ReleaseManagementSecurityPermission("releaseId", ReleaseManagementSecurityArgumentType.ReleaseId, ReleaseManagementSecurityPermissions.ViewReleases)]
    [ClientExample("GET__GetReleaseAttachment.json", "Get release task attachments", null, null)]
    public IEnumerable<ReleaseTaskAttachment> GetReleaseTaskAttachments(
      int releaseId,
      int environmentId,
      int attemptId,
      Guid planId,
      string type)
    {
      IEnumerable<ReleaseTaskAttachment> releaseTaskAttachments = this.TfsRequestContext.GetService<ReleaseAttachmentsService>().GetAttachments(this.TfsRequestContext, this.ProjectId, releaseId, environmentId, attemptId, planId, type).ToList<TaskAttachment>().ToReleaseTaskAttachments(this.TfsRequestContext);
      this.TfsRequestContext.SetSecuredObjects<ReleaseTaskAttachment>(releaseTaskAttachments);
      return releaseTaskAttachments;
    }

    [SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope", Justification = "Stream dispose should be sufficient")]
    [HttpGet]
    [PublicProjectRequestRestrictions]
    [ClientResponseType(typeof (Stream), null, null, MediaType = "application/octet-stream")]
    [ClientLocationId("60b86efb-7b8c-4853-8f9f-aa142b77b479")]
    [ReleaseManagementSecurityPermission("releaseId", ReleaseManagementSecurityArgumentType.ReleaseId, ReleaseManagementSecurityPermissions.ViewReleases)]
    [ClientExample("GET__GetReleaseTaskAttachmentContent.json", "Get release task attachment content", null, null)]
    public HttpResponseMessage GetReleaseTaskAttachmentContent(
      int releaseId,
      int environmentId,
      int attemptId,
      Guid planId,
      Guid timelineId,
      Guid recordId,
      string type,
      string name)
    {
      Stream contentStream = this.TfsRequestContext.GetService<ReleaseAttachmentsService>().GetAttachment(this.TfsRequestContext, this.ProjectId, releaseId, environmentId, attemptId, planId, timelineId, recordId, type, name);
      if (contentStream == null)
        return this.Request.CreateResponse(HttpStatusCode.NotFound);
      HttpResponseMessage response = this.Request.CreateResponse(HttpStatusCode.OK);
      response.Content = (HttpContent) new VssServerPushStreamContent((Action<Stream, HttpContent, TransportContext>) ((stream, httpContent, transportContext) =>
      {
        try
        {
          this.TfsRequestContext.UpdateTimeToFirstPage();
          using (contentStream)
            contentStream.CopyTo(stream);
        }
        finally
        {
          stream?.Dispose();
        }
      }), new MediaTypeHeaderValue("application/octet-stream"), (object) this.TfsRequestContext.GetSecuredObject());
      return response;
    }

    [HttpGet]
    [Obsolete("GetTaskAttachments API is deprecated. Use GetReleaseTaskAttachments API instead.")]
    [ClientLocationId("214111EE-2415-4DF2-8ED2-74417F7D61F9")]
    [ReleaseManagementSecurityPermission("releaseId", ReleaseManagementSecurityArgumentType.ReleaseId, ReleaseManagementSecurityPermissions.ViewReleases)]
    public IEnumerable<ReleaseTaskAttachment> GetTaskAttachments(
      int releaseId,
      int environmentId,
      int attemptId,
      Guid timelineId,
      string type)
    {
      IEnumerable<ReleaseTaskAttachment> releaseTaskAttachments = this.TfsRequestContext.GetService<ReleaseAttachmentsService>().GetAttachments(this.TfsRequestContext, this.ProjectId, releaseId, environmentId, attemptId, timelineId, type).ToList<TaskAttachment>().ToReleaseTaskAttachments(this.TfsRequestContext);
      this.TfsRequestContext.SetSecuredObjects<ReleaseTaskAttachment>(releaseTaskAttachments);
      return releaseTaskAttachments;
    }

    [SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope", Justification = "Stream dispose should be sufficient")]
    [HttpGet]
    [Obsolete("GetTaskAttachmentContent API is deprecated. Use GetReleaseTaskAttachmentContent API instead.")]
    [ClientResponseType(typeof (Stream), null, null, MediaType = "application/octet-stream")]
    [ClientLocationId("C4071F6D-3697-46CA-858E-8B10FF09E52F")]
    [ReleaseManagementSecurityPermission("releaseId", ReleaseManagementSecurityArgumentType.ReleaseId, ReleaseManagementSecurityPermissions.ViewReleases)]
    public HttpResponseMessage GetTaskAttachmentContent(
      int releaseId,
      int environmentId,
      int attemptId,
      Guid timelineId,
      Guid recordId,
      string type,
      string name)
    {
      Stream contentStream = this.TfsRequestContext.GetService<ReleaseAttachmentsService>().GetAttachment(this.TfsRequestContext, this.ProjectId, releaseId, environmentId, attemptId, timelineId, timelineId, recordId, type, name);
      if (contentStream == null)
        return this.Request.CreateResponse(HttpStatusCode.NotFound);
      HttpResponseMessage response = this.Request.CreateResponse(HttpStatusCode.OK);
      response.Content = (HttpContent) new VssServerPushStreamContent((Action<Stream, HttpContent, TransportContext>) ((stream, httpContent, transportContext) =>
      {
        try
        {
          this.TfsRequestContext.UpdateTimeToFirstPage();
          using (contentStream)
            contentStream.CopyTo(stream);
        }
        finally
        {
          stream?.Dispose();
        }
      }), new MediaTypeHeaderValue("application/octet-stream"), (object) this.TfsRequestContext.GetSecuredObject());
      return response;
    }
  }
}
