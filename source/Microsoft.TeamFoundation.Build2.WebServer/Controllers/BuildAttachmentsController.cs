// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build2.WebServer.Controllers.BuildAttachmentsController
// Assembly: Microsoft.TeamFoundation.Build2.WebServer, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: FDDA87C8-3548-4A75-AA18-4FB488450659
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build2.WebServer.dll

using Microsoft.TeamFoundation.Build.WebApi;
using Microsoft.TeamFoundation.Build2.Routes;
using Microsoft.TeamFoundation.Build2.Server;
using Microsoft.TeamFoundation.DistributedTask.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.Types;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web.Http;

namespace Microsoft.TeamFoundation.Build2.WebServer.Controllers
{
  [ControllerApiVersion(4.1)]
  [VersionedApiControllerCustomName(Area = "build", ResourceName = "attachments", ResourceVersion = 1)]
  public class BuildAttachmentsController : BuildApiController
  {
    [HttpGet]
    [ClientResponseType(typeof (Stream), null, null, MethodName = "GetAttachment", MediaType = "application/octet-stream")]
    [PublicProjectRequestRestrictions]
    [ClientLocationId("AF5122D3-3438-485E-A25A-2DBBFDE84EE6")]
    public HttpResponseMessage GetAttachment(
      int buildId,
      Guid timelineId,
      Guid recordId,
      string type,
      string name)
    {
      ArgumentUtility.CheckForEmptyGuid(timelineId, nameof (timelineId));
      ArgumentUtility.CheckForEmptyGuid(recordId, nameof (recordId));
      ArgumentUtility.CheckStringForNullOrEmpty(type, nameof (type));
      ArgumentUtility.CheckStringForNullOrEmpty(name, nameof (name));
      BuildData buildById = this.InternalBuildService.GetBuildById(this.TfsRequestContext, buildId);
      Guid? projectId1 = buildById?.ProjectId;
      Guid projectId2 = this.ProjectId;
      if ((projectId1.HasValue ? (projectId1.HasValue ? (projectId1.GetValueOrDefault() != projectId2 ? 1 : 0) : 0) : 1) != 0)
        throw new BuildNotFoundException(Resources.BuildNotFound((object) buildId));
      TaskAttachment attachment = new TaskAttachment(type, name)
      {
        TimelineId = timelineId,
        RecordId = recordId
      };
      Stream contentStream = this.TfsRequestContext.GetService<IBuildOrchestrator>().GetAttachment(this.TfsRequestContext, buildById.ProjectId, buildById.OrchestrationPlan.PlanId, attachment);
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
      }), new MediaTypeHeaderValue("application/octet-stream"), (object) buildById.ToSecuredObject());
      VssPerformanceCounter performanceCounter = VssPerformanceCounterManager.GetPerformanceCounter("Microsoft.TeamFoundation.Framework.Server.Perf_FCS_TotalDownloads");
      performanceCounter.Increment();
      performanceCounter = VssPerformanceCounterManager.GetPerformanceCounter("Microsoft.TeamFoundation.Framework.Server.Perf_FCS_DownloadSec");
      performanceCounter.Increment();
      return response;
    }

    [HttpGet]
    [PublicProjectRequestRestrictions]
    [ClientLocationId("F2192269-89FA-4F94-BAF6-8FB128C55159")]
    public List<Attachment> GetAttachments(int buildId, string type)
    {
      ArgumentUtility.CheckStringForNullOrEmpty(type, nameof (type));
      BuildData build = this.InternalBuildService.GetBuildById(this.TfsRequestContext, buildId);
      Guid? projectId1 = build?.ProjectId;
      Guid projectId2 = this.ProjectId;
      if ((projectId1.HasValue ? (projectId1.HasValue ? (projectId1.GetValueOrDefault() != projectId2 ? 1 : 0) : 0) : 1) != 0)
        throw new BuildNotFoundException(Resources.BuildNotFound((object) buildId));
      IEnumerable<TaskAttachment> attachments = this.TfsRequestContext.GetService<IBuildOrchestrator>().GetAttachments(this.TfsRequestContext, build.ProjectId, build.OrchestrationPlan.PlanId, type);
      ISecuredObject securedObject = build.ToSecuredObject();
      Func<TaskAttachment, Attachment> selector = (Func<TaskAttachment, Attachment>) (attachment => this.ToBuildAttachment(build, attachment, securedObject));
      return attachments.Select<TaskAttachment, Attachment>(selector).ToList<Attachment>();
    }

    private Attachment ToBuildAttachment(
      BuildData build,
      TaskAttachment taskAttachment,
      ISecuredObject securedObject)
    {
      Attachment buildAttachment = new Attachment(securedObject);
      buildAttachment.Name = taskAttachment.Name;
      buildAttachment.Links.AddLink("self", this.TfsRequestContext.GetService<IBuildRouteService>().GetBuildAttachmentRestUrl(this.TfsRequestContext, this.ProjectId, build.Id, taskAttachment.TimelineId, taskAttachment.RecordId, taskAttachment.Type, taskAttachment.Name), securedObject);
      return buildAttachment;
    }
  }
}
