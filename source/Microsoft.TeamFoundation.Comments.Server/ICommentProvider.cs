// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Comments.Server.ICommentProvider
// Assembly: Microsoft.TeamFoundation.Comments.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: CBA40CC5-9694-4582-97B5-1660FA9D4307
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Comments.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;

namespace Microsoft.TeamFoundation.Comments.Server
{
  [InheritedExport]
  public interface ICommentProvider
  {
    Guid ArtifactKind { get; }

    string ArtifactFriendlyName { get; }

    bool SupportsThreading { get; }

    bool SupportsMentions { get; }

    bool ShouldFireReactionChangedEvent { get; }

    void CheckReadPermission(
      IVssRequestContext requestContext,
      Guid projectId,
      ISet<string> artifactIds,
      out IDictionary<string, ISecuredObject> securedObjects);

    bool CanReadArtifact(
      IVssRequestContext requestContext,
      Microsoft.VisualStudio.Services.Identity.Identity identity,
      Guid projectId,
      string artifactId);

    void CheckAddPermission(
      IVssRequestContext requestContext,
      Guid projectId,
      ISet<string> artifactIds,
      out IDictionary<string, ISecuredObject> securedObjects);

    void CheckDeletePermission(
      IVssRequestContext requestContext,
      Guid projectId,
      IEnumerable<Comment> comments,
      out IDictionary<int, ISecuredObject> securedObjects);

    void CheckDestroyPermission(
      IVssRequestContext requestContext,
      Guid projectId,
      ISet<string> artifactIds);

    void CheckUpdatePermission(
      IVssRequestContext requestContext,
      Guid projectId,
      IEnumerable<Comment> comments,
      out IDictionary<int, ISecuredObject> securedObjects);

    CommentFormat GetCommentFormat(IVssRequestContext requestContext, Guid projectId);

    void CheckReactPermission(
      IVssRequestContext requestContext,
      Guid projectId,
      Comment comment,
      out ISecuredObject securedObject);

    ArtifactInfo GetArtifactInfo(
      IVssRequestContext requestContext,
      Guid projectId,
      string artifactId);

    string GetAttachmentsUrl(IVssRequestContext requestContext, Guid projectId, string artifactId);
  }
}
