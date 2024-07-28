// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Git.Server.ITeamFoundationGitAttachmentService
// Assembly: Microsoft.TeamFoundation.Git.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1F0714E-7EF5-4D28-9AF2-C8D8620EA079
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Git.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.SourceControl.WebApi;
using System.Collections.Generic;
using System.IO;

namespace Microsoft.TeamFoundation.Git.Server
{
  [DefaultServiceImplementation(typeof (TeamFoundationGitAttachmentService))]
  public interface ITeamFoundationGitAttachmentService : IVssFrameworkService
  {
    Attachment CreateAttachment(
      Stream content,
      long offsetFrom,
      CompressionType compressionType,
      long calculatedLength,
      IVssRequestContext requestContext,
      RepoKey repoScope,
      TfsGitPullRequest pullRequest,
      string fileName,
      out bool completedUpload);

    IEnumerable<Attachment> GetAttachments(
      IVssRequestContext requestContext,
      RepoKey repoScope,
      TfsGitPullRequest pullRequest);

    Attachment GetAttachment(
      IVssRequestContext requestContext,
      RepoKey repoScope,
      TfsGitPullRequest pullRequest,
      string fileName);

    void DeleteAttachment(
      IVssRequestContext requestContext,
      RepoKey repoScope,
      TfsGitPullRequest pullRequest,
      string fileName);

    Stream GetAttachmentContent(
      IVssRequestContext requestContext,
      RepoKey repoScope,
      TfsGitPullRequest pullRequest,
      string fileName,
      out Attachment pullRequestAttachment,
      out CompressionType compressionType);
  }
}
