// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.SourceControl.WebServer.GitAnnotatedTagsController
// Assembly: Microsoft.TeamFoundation.SourceControl.WebServer, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 47C05AF5-4412-4EF0-BF63-D475D8EECD03
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.SourceControl.WebServer.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Git.Server;
using Microsoft.TeamFoundation.Git.Server.Utils;
using Microsoft.TeamFoundation.SourceControl.WebApi;
using Microsoft.VisualStudio.Services.WebApi.Internal;
using System;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Controllers;

namespace Microsoft.TeamFoundation.SourceControl.WebServer
{
  [VersionedApiControllerCustomName(Area = "git", ResourceName = "annotatedTags")]
  public class GitAnnotatedTagsController : GitApiController
  {
    [HttpPost]
    [ClientLocationId("5E8A8081-3851-4626-B677-9891CC04102E")]
    [ClientResponseType(typeof (GitAnnotatedTag), null, null)]
    [ClientExample("POST__git_repositories__annotated__tags.json", null, null, null)]
    public HttpResponseMessage CreateAnnotatedTag(
      [ClientParameterType(typeof (Guid), true)] string repositoryId,
      [FromBody] GitAnnotatedTag tagObject,
      [ClientIgnore] string projectId = null)
    {
      if (tagObject == null || tagObject.Name == null || tagObject.TaggedObject == null || tagObject.TaggedObject.ObjectId == null || tagObject.Message == null)
        throw new InvalidArgumentValueException(nameof (tagObject), Resources.Get("InvalidParameters"));
      Sha1Id id;
      if (!Sha1Id.TryParse(tagObject.TaggedObject.ObjectId, out id))
        throw new InvalidArgumentValueException("ObjectId");
      Sha1Id tag;
      using (ITfsGitRepository tfsGitRepository = this.GetTfsGitRepository(repositoryId, projectId))
      {
        if (tfsGitRepository.IsInMaintenance)
          throw new GitRepoInMaintenanceException(MaintenanceMessageUtils.GetSanitizedMaintenanceMessage(this.TfsRequestContext));
        tag = GitAnnotatedTagsHelper.CreateTag(this.TfsRequestContext, tfsGitRepository, tagObject.Name, id, tagObject.Message);
      }
      using (ITfsGitRepository tfsGitRepository = this.GetTfsGitRepository(repositoryId, projectId))
        return this.Request.CreateResponse<GitAnnotatedTag>(HttpStatusCode.Created, tfsGitRepository.LookupObject<TfsGitTag>(tag).ToGitAnnotatedTags(this.TfsRequestContext, tfsGitRepository.Key, false));
    }

    [HttpGet]
    [ClientLocationId("5E8A8081-3851-4626-B677-9891CC04102E")]
    [ClientResponseType(typeof (GitAnnotatedTag), null, null)]
    [ClientExample("GET__git_repositories__annotated__tags.json", null, null, null)]
    public HttpResponseMessage GetAnnotatedTag(
      [ClientParameterType(typeof (Guid), true)] string repositoryId,
      string objectId,
      [ClientIgnore] string projectId = null)
    {
      Sha1Id id;
      if (string.IsNullOrEmpty(objectId) || !Sha1Id.TryParse(objectId, out id) || id.Equals(Sha1Id.Empty))
        throw new InvalidArgumentValueException(nameof (objectId));
      using (ITfsGitRepository tfsGitRepository = this.GetTfsGitRepository(repositoryId))
      {
        TfsGitTag gitTag;
        try
        {
          gitTag = tfsGitRepository.LookupObject<TfsGitTag>(id);
        }
        catch (GitUnexpectedObjectTypeException ex)
        {
          return this.Request.CreateErrorResponse(HttpStatusCode.NotFound, (Exception) ex, (IHttpController) this);
        }
        catch (GitObjectDoesNotExistException ex)
        {
          return this.Request.CreateErrorResponse(HttpStatusCode.NotFound, (Exception) ex, (IHttpController) this);
        }
        return this.Request.CreateResponse<GitAnnotatedTag>(HttpStatusCode.OK, gitTag.ToGitAnnotatedTags(this.TfsRequestContext, tfsGitRepository.Key, false));
      }
    }
  }
}
