// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Wiki.Server.WikiCommentProvider
// Assembly: Microsoft.TeamFoundation.Wiki.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B3E52AF1-8928-4A06-8693-F7E4A258A64E
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Wiki.Server.dll

using Microsoft.TeamFoundation.Comments.Server;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.SourceControl.WebApi;
using Microsoft.TeamFoundation.Wiki.WebApi;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.Wiki.Server
{
  public class WikiCommentProvider : ICommentProvider
  {
    Guid ICommentProvider.ArtifactKind => WikiConstants.WikiPageArtifactKind;

    bool ICommentProvider.SupportsThreading => true;

    bool ICommentProvider.SupportsMentions => true;

    bool ICommentProvider.ShouldFireReactionChangedEvent => false;

    string ICommentProvider.ArtifactFriendlyName => Resources.WikiCommentArtifactFriendlyName;

    void ICommentProvider.CheckReadPermission(
      IVssRequestContext requestContext,
      Guid projectId,
      ISet<string> artifactIds,
      out IDictionary<string, ISecuredObject> securedObjects)
    {
      this.CheckPermissionForArtifacts(requestContext, projectId, artifactIds, GitRepositoryPermissions.GenericRead, out securedObjects);
    }

    bool ICommentProvider.CanReadArtifact(
      IVssRequestContext requestContext,
      Microsoft.VisualStudio.Services.Identity.Identity identity,
      Guid projectId,
      string artifactId)
    {
      Guid projectId1;
      Guid wikiId;
      WikiPageIdHelper.GetIDsFromArtifactiId(artifactId, out projectId1, out wikiId, out int _);
      WikiV2 wikiById = WikiV2Helper.GetWikiById(requestContext, projectId1, wikiId);
      return WikiV2Helper.CanUserAccessWiki(requestContext, wikiById, identity.Descriptor);
    }

    void ICommentProvider.CheckAddPermission(
      IVssRequestContext requestContext,
      Guid projectId,
      ISet<string> artifactIds,
      out IDictionary<string, ISecuredObject> securedObjects)
    {
      this.CheckPermissionForArtifacts(requestContext, projectId, artifactIds, GitRepositoryPermissions.GenericContribute, out securedObjects);
    }

    void ICommentProvider.CheckDeletePermission(
      IVssRequestContext requestContext,
      Guid projectId,
      IEnumerable<Microsoft.TeamFoundation.Comments.Server.Comment> comments,
      out IDictionary<int, ISecuredObject> securedObjects)
    {
      this.CheckPermissionForArtifacts(requestContext, projectId, comments, GitRepositoryPermissions.GenericContribute, false, out securedObjects);
    }

    void ICommentProvider.CheckUpdatePermission(
      IVssRequestContext requestContext,
      Guid projectId,
      IEnumerable<Microsoft.TeamFoundation.Comments.Server.Comment> comments,
      out IDictionary<int, ISecuredObject> securedObjects)
    {
      this.CheckPermissionForArtifacts(requestContext, projectId, comments, GitRepositoryPermissions.GenericContribute, true, out securedObjects);
    }

    void ICommentProvider.CheckReactPermission(
      IVssRequestContext requestContext,
      Guid projectId,
      Microsoft.TeamFoundation.Comments.Server.Comment comment,
      out ISecuredObject securedObject)
    {
      this.CheckPermissionForArtifact(requestContext, projectId, comment.ArtifactId, GitRepositoryPermissions.GenericRead, out securedObject);
    }

    void ICommentProvider.CheckDestroyPermission(
      IVssRequestContext requestContext,
      Guid projectId,
      ISet<string> artifactIds)
    {
    }

    CommentFormat ICommentProvider.GetCommentFormat(
      IVssRequestContext requestContext,
      Guid projectId)
    {
      return CommentFormat.Markdown;
    }

    ArtifactInfo ICommentProvider.GetArtifactInfo(
      IVssRequestContext requestContext,
      Guid projectId,
      string artifactId)
    {
      Guid projectId1;
      Guid wikiId;
      int pageId;
      WikiPageIdHelper.GetIDsFromArtifactiId(artifactId, out projectId1, out wikiId, out pageId);
      WikiPage pageById = new WikiPagesProvider().GetPageById(requestContext, projectId1, pageId);
      WikiV2 wikiById = WikiV2Helper.GetWikiById(requestContext, projectId1, wikiId);
      string fromReadablePath = PathHelper.GetPageTitleFromReadablePath(pageById.Path);
      return new ArtifactInfo()
      {
        ArtifactTitle = fromReadablePath + " (" + wikiById.Name + ")",
        ArtifactUri = pageById.RemoteUrl + "&shouldScrollToComments=true"
      };
    }

    string ICommentProvider.GetAttachmentsUrl(
      IVssRequestContext requestContext,
      Guid projectId,
      string artifactId)
    {
      Guid projectId1;
      Guid wikiId;
      int pageId;
      WikiPageIdHelper.GetIDsFromArtifactiId(artifactId, out projectId1, out wikiId, out pageId);
      return WikiUrlHelper.GetWikiPageCommentAttachmentsResourceUrl(requestContext, projectId1, wikiId, pageId);
    }

    private void CheckIfProjectIdIsValid(Guid projectId, Guid artifactProjectId, string artifactId)
    {
      if (!projectId.Equals(artifactProjectId))
        throw new ArgumentException(string.Format(Resources.WikiCommentInvalidProjectId, (object) projectId, (object) artifactId));
    }

    private void CheckPermissionForArtifacts(
      IVssRequestContext requestContext,
      Guid projectId,
      ISet<string> artifactIds,
      GitRepositoryPermissions gitRepositoryPermissions,
      out IDictionary<string, ISecuredObject> securedObjects)
    {
      securedObjects = (IDictionary<string, ISecuredObject>) new Dictionary<string, ISecuredObject>();
      foreach (string artifactId in (IEnumerable<string>) artifactIds)
      {
        ISecuredObject wikiSecuredObject;
        this.CheckPermissionForArtifact(requestContext, projectId, artifactId, gitRepositoryPermissions, out wikiSecuredObject);
        securedObjects.Add(artifactId, wikiSecuredObject);
      }
    }

    private void CheckPermissionForArtifacts(
      IVssRequestContext requestContext,
      Guid projectId,
      IEnumerable<Microsoft.TeamFoundation.Comments.Server.Comment> comments,
      GitRepositoryPermissions gitRepositoryPermissions,
      bool isUpdate,
      out IDictionary<int, ISecuredObject> securedObjects)
    {
      securedObjects = (IDictionary<int, ISecuredObject>) new Dictionary<int, ISecuredObject>();
      foreach (Microsoft.TeamFoundation.Comments.Server.Comment comment in comments)
      {
        if (!comment.CreatedBy.Equals(requestContext.GetUserId()))
        {
          if (isUpdate)
            throw new CommentUpdateException(comment.CommentId);
          throw new CommentDeleteException(comment.CommentId);
        }
        ISecuredObject wikiSecuredObject;
        this.CheckPermissionForArtifact(requestContext, projectId, comment.ArtifactId, GitRepositoryPermissions.GenericContribute, out wikiSecuredObject);
        securedObjects.Add(comment.CommentId, wikiSecuredObject);
      }
    }

    private void CheckPermissionForArtifact(
      IVssRequestContext requestContext,
      Guid projectId,
      string artifactId,
      GitRepositoryPermissions gitRepositoryPermissions,
      out ISecuredObject wikiSecuredObject)
    {
      Guid projectId1;
      Guid wikiId;
      int pageId;
      WikiPageIdHelper.GetIDsFromArtifactiId(artifactId, out projectId1, out wikiId, out pageId);
      this.CheckIfProjectIdIsValid(projectId, projectId1, artifactId);
      WikiV2Helper.HasWikiPermissions(requestContext, projectId1, wikiId, pageId, gitRepositoryPermissions, out wikiSecuredObject);
    }
  }
}
