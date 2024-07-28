// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Wiki.Web.Controllers.WikiAttachmentsController
// Assembly: Microsoft.TeamFoundation.Wiki.Web, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: D56D2437-BFF5-4193-B3F8-AC19F31B2530
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Wiki.Web.dll

using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Git.Server;
using Microsoft.TeamFoundation.SourceControl.WebApi;
using Microsoft.TeamFoundation.SourceControl.WebServer;
using Microsoft.TeamFoundation.Wiki.Server;
using Microsoft.TeamFoundation.Wiki.WebApi;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web.Http;
using System.Web.Http.ModelBinding;

namespace Microsoft.TeamFoundation.Wiki.Web.Controllers
{
  [ApiTelemetry(true, false)]
  [VersionedApiControllerCustomName(Area = "wiki", ResourceName = "attachments")]
  [RequestContentTypeRestriction(AllowStream = true)]
  public class WikiAttachmentsController : WikiApiController
  {
    private const string WebAccessWikiRichCodeWikiEditing = "WebAccess.Wiki.RichCodeWikiEditing";

    [HttpPut]
    [ClientResourceOperation(ClientResourceOperationName.Create)]
    [ClientResponseCode(HttpStatusCode.Created, "Attachment created. Attachment's version is populated in the ETag response header.", false)]
    [ClientResponseType(typeof (WikiAttachmentResponse), null, null)]
    [ClientLocationId("C4382D8D-FEFC-40E0-92C5-49852E9E17C0")]
    [ClientRequestContentMediaType("application/octet-stream")]
    [ClientRequestContentIsRawData]
    [ClientRequestBodyIsStream]
    [TraceFilter(15250300, 15250399)]
    [ClientExample("PUT_attachments.json", null, null, null)]
    public HttpResponseMessage CreateAttachment(
      [FromUri, ClientParameterType(typeof (Guid), true)] string wikiIdentifier,
      [ClientQueryParameter] string name,
      [ModelBinder(typeof (GitVersionDescriptorModelBinder))] GitVersionDescriptor versionDescriptor = null)
    {
      Microsoft.TeamFoundation.Wiki.Server.PathHelper.ValidateAttachmentName(name, this.TfsRequestContext.IsFeatureEnabled("Wiki.AttachSvgFile"));
      string end;
      using (RestrictedStream restrictedStream = new RestrictedStream(this.Request.Content.ReadAsStreamAsync().Result, 0L, 26214401L, true))
      {
        try
        {
          end = new StreamReader((Stream) restrictedStream).ReadToEnd();
          if (end == null)
            throw new ArgumentNullException("content", Microsoft.TeamFoundation.Wiki.Web.Resources.WikiAttachmentContentNullMessage);
        }
        catch (Exception ex)
        {
          if (restrictedStream.Position >= 26214400L)
            throw new InvalidArgumentValueException("contentStream", string.Format(Microsoft.TeamFoundation.Wiki.Web.Resources.RequestMaxSizeExceeded, (object) 26214400));
          throw new InvalidArgumentValueException("contentStream", ex.Message);
        }
      }
      WikiV2 wikiByIdentifier = WikiV2Helper.GetWikiByIdentifier(this.TfsRequestContext, this.ProjectId, wikiIdentifier);
      versionDescriptor = wikiByIdentifier != null ? this.ValidateAndGetWikiVersion(wikiByIdentifier, versionDescriptor) : throw new WikiNotFoundException(Microsoft.TeamFoundation.Wiki.Web.Resources.WikiNotFound);
      WikiAttachmentChange attachmentChange = this.GetWikiAttachmentChange(name, end, wikiByIdentifier);
      using (ITfsGitRepository wikiRepository = this.GetWikiRepository(wikiByIdentifier))
      {
        IList<GitChange> changes = new AttachmentsGitChangesProvider().GetChanges(this.TfsRequestContext, wikiRepository, versionDescriptor, wikiByIdentifier.MappedPath, attachmentChange);
        TfsGitRef versionDescriptor1 = this.GetRefFromVersionDescriptor(wikiRepository, versionDescriptor);
        if (versionDescriptor1 == null)
          throw new InvalidArgumentValueException("Version", string.Format(Microsoft.TeamFoundation.Wiki.Web.Resources.WikiVersionInvalidOrDoesNotExist, (object) versionDescriptor.Version));
        TfsGitRefUpdateResultSet refUpdateResultSet;
        try
        {
          refUpdateResultSet = wikiRepository.ModifyPaths(versionDescriptor1.Name, versionDescriptor1.ObjectId, string.Format(Microsoft.TeamFoundation.Wiki.Web.Resources.WikiAttachmentCommitMessage, (object) attachmentChange.Name), (IEnumerable<GitChange>) changes, (GitUserDate) null, (GitUserDate) null);
        }
        catch (Exception ex)
        {
          this.TfsRequestContext.TraceException(15250301, "Wiki", "Service", ex);
          if (!(ex is GitObjectRejectedException))
            throw new WikiCreateAttachmentFailedException(string.Format(Microsoft.TeamFoundation.Wiki.Web.Resources.CreateAttachmentFailedWithMessage, (object) ex.Message));
          throw;
        }
        TfsGitRefUpdateResult gitRefUpdateResult = refUpdateResultSet.Results.Single<TfsGitRefUpdateResult>();
        if (!gitRefUpdateResult.Succeeded)
        {
          switch (gitRefUpdateResult.Status)
          {
            case GitRefUpdateStatus.WritePermissionRequired:
              throw new WikiCreateAttachmentFailedException(string.Format(Microsoft.TeamFoundation.Wiki.Web.Resources.CreateAttachmentFailedWithMessage, (object) Microsoft.TeamFoundation.Wiki.Web.Resources.ErrorMessageWritePermissionRequired));
            case GitRefUpdateStatus.RejectedByPolicy:
              throw new WikiCreateAttachmentFailedException(string.Format(Microsoft.TeamFoundation.Wiki.Web.Resources.CreateAttachmentFailedWithMessage, (object) gitRefUpdateResult.CustomMessage));
            default:
              throw new WikiCreateAttachmentFailedException(string.Format(Microsoft.TeamFoundation.Wiki.Web.Resources.CreateAttachmentFailedWithMessage, (object) gitRefUpdateResult.Status));
          }
        }
        else
        {
          HttpResponseMessage response = this.Request.CreateResponse<WikiAttachment>(HttpStatusCode.Created, new WikiAttachment(attachmentChange.Name, attachmentChange.Path));
          response.Headers.ETag = new EntityTagHeaderValue("\"" + WikiGitHelper.GetWikiItemGitObjectId(this.TfsRequestContext, wikiRepository, versionDescriptor, attachmentChange.Path) + "\"");
          return response;
        }
      }
    }

    private WikiAttachmentChange GetWikiAttachmentChange(string name, string content, WikiV2 wiki)
    {
      string fileName = Path.GetFileName(name);
      string attachmentFilePath = Microsoft.TeamFoundation.Wiki.Server.PathHelper.GetAttachmentFilePath(fileName);
      if (wiki.Type == WikiType.CodeWiki)
        attachmentFilePath = Microsoft.TeamFoundation.Wiki.Server.PathHelper.GetCodeWikiAttachmentFilePath(fileName, wiki.MappedPath);
      return new WikiAttachmentChange()
      {
        Name = fileName,
        Path = attachmentFilePath,
        Content = content
      };
    }
  }
}
