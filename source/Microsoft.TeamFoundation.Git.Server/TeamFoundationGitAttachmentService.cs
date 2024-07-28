// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Git.Server.TeamFoundationGitAttachmentService
// Assembly: Microsoft.TeamFoundation.Git.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1F0714E-7EF5-4D28-9AF2-C8D8620EA079
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Git.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.CodeReview.Server;
using Microsoft.VisualStudio.Services.CodeReview.WebApi;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Microsoft.TeamFoundation.Git.Server
{
  internal class TeamFoundationGitAttachmentService : 
    ITeamFoundationGitAttachmentService,
    IVssFrameworkService
  {
    public Microsoft.TeamFoundation.SourceControl.WebApi.Attachment CreateAttachment(
      Stream content,
      long offsetFrom,
      CompressionType compressionType,
      long calculatedLength,
      IVssRequestContext requestContext,
      RepoKey repoScope,
      TfsGitPullRequest pullRequest,
      string fileName,
      out bool completedUpload)
    {
      this.VerifyAttachementsSupported(pullRequest, (string) null);
      ICodeReviewAttachmentService service1 = requestContext.GetService<ICodeReviewAttachmentService>();
      ICodeReviewService service2 = requestContext.GetService<ICodeReviewService>();
      try
      {
        ReviewFileContentInfo reviewFileContentInfo = service2.UploadFile(requestContext, repoScope.ProjectId, pullRequest.CodeReviewId, content, calculatedLength, offsetFrom, compressionType, ReviewFileType.Attachment, fileName);
        Microsoft.VisualStudio.Services.CodeReview.WebApi.Attachment attachment = service1.SaveAttachment(requestContext, repoScope.ProjectId, pullRequest.CodeReviewId, new Microsoft.VisualStudio.Services.CodeReview.WebApi.Attachment()
        {
          ContentHash = reviewFileContentInfo.SHA1Hash,
          DisplayName = fileName
        });
        completedUpload = reviewFileContentInfo.FileUploadComplete;
        return PullRequestCodeReviewConverter.ToGitAttachment(attachment, requestContext, pullRequest, repoScope);
      }
      catch (CodeReviewMaxAttachmentCountException ex)
      {
        throw new GitPullRequestMaxAttachmentCountException(ex.Message);
      }
    }

    public void DeleteAttachment(
      IVssRequestContext requestContext,
      RepoKey repoScope,
      TfsGitPullRequest pullRequest,
      string fileName)
    {
      this.VerifyAttachementsSupported(pullRequest, fileName);
      ICodeReviewAttachmentService service = requestContext.GetService<ICodeReviewAttachmentService>();
      foreach (Microsoft.VisualStudio.Services.CodeReview.WebApi.Attachment attachment in service.GetAttachments(requestContext, repoScope.ProjectId, pullRequest.CodeReviewId).Where<Microsoft.VisualStudio.Services.CodeReview.WebApi.Attachment>((Func<Microsoft.VisualStudio.Services.CodeReview.WebApi.Attachment, bool>) (a => string.Equals(a.DisplayName, fileName, StringComparison.OrdinalIgnoreCase))))
        service.DeleteAttachment(requestContext, repoScope.ProjectId, pullRequest.CodeReviewId, attachment.Id, true);
    }

    public Microsoft.TeamFoundation.SourceControl.WebApi.Attachment GetAttachment(
      IVssRequestContext requestContext,
      RepoKey repoScope,
      TfsGitPullRequest pullRequest,
      string fileName)
    {
      this.VerifyAttachementsSupported(pullRequest, fileName);
      return PullRequestCodeReviewConverter.ToGitAttachment(requestContext.GetService<ICodeReviewAttachmentService>().GetAttachments(requestContext, repoScope.ProjectId, pullRequest.CodeReviewId).Where<Microsoft.VisualStudio.Services.CodeReview.WebApi.Attachment>((Func<Microsoft.VisualStudio.Services.CodeReview.WebApi.Attachment, bool>) (a => string.Equals(a.DisplayName, fileName, StringComparison.OrdinalIgnoreCase))).FirstOrDefault<Microsoft.VisualStudio.Services.CodeReview.WebApi.Attachment>() ?? throw new GitPullRequestAttachmentNotFoundException(fileName), requestContext, pullRequest, repoScope);
    }

    public Stream GetAttachmentContent(
      IVssRequestContext requestContext,
      RepoKey repoScope,
      TfsGitPullRequest pullRequest,
      string fileName,
      out Microsoft.TeamFoundation.SourceControl.WebApi.Attachment pullRequestAttachment,
      out CompressionType compressionType)
    {
      this.VerifyAttachementsSupported(pullRequest, fileName);
      ICodeReviewService service = requestContext.GetService<ICodeReviewService>();
      Microsoft.VisualStudio.Services.CodeReview.WebApi.Attachment attachment = requestContext.GetService<ICodeReviewAttachmentService>().GetAttachments(requestContext, repoScope.ProjectId, pullRequest.CodeReviewId).Where<Microsoft.VisualStudio.Services.CodeReview.WebApi.Attachment>((Func<Microsoft.VisualStudio.Services.CodeReview.WebApi.Attachment, bool>) (a => string.Equals(a.DisplayName, fileName, StringComparison.OrdinalIgnoreCase))).FirstOrDefault<Microsoft.VisualStudio.Services.CodeReview.WebApi.Attachment>();
      byte[] numArray = attachment != null ? ReviewFileContentExtensions.ToSha1HashBytes(attachment.ContentHash) : throw new GitPullRequestAttachmentNotFoundException(fileName);
      IVssRequestContext requestContext1 = requestContext;
      Guid projectId = repoScope.ProjectId;
      int codeReviewId = pullRequest.CodeReviewId;
      byte[] contentHash = numArray;
      ChangeEntryStream changeEntryStream = service.DownloadFile(requestContext1, projectId, codeReviewId, contentHash, ReviewFileType.Attachment);
      compressionType = changeEntryStream.CompressionType;
      pullRequestAttachment = PullRequestCodeReviewConverter.ToGitAttachment(attachment, requestContext, pullRequest, repoScope);
      return changeEntryStream.FileStream;
    }

    public IEnumerable<Microsoft.TeamFoundation.SourceControl.WebApi.Attachment> GetAttachments(
      IVssRequestContext requestContext,
      RepoKey repoScope,
      TfsGitPullRequest pullRequest)
    {
      return pullRequest.CodeReviewId <= 0 ? (IEnumerable<Microsoft.TeamFoundation.SourceControl.WebApi.Attachment>) new List<Microsoft.TeamFoundation.SourceControl.WebApi.Attachment>() : (IEnumerable<Microsoft.TeamFoundation.SourceControl.WebApi.Attachment>) requestContext.GetService<ICodeReviewAttachmentService>().GetAttachments(requestContext, repoScope.ProjectId, pullRequest.CodeReviewId).Select<Microsoft.VisualStudio.Services.CodeReview.WebApi.Attachment, Microsoft.TeamFoundation.SourceControl.WebApi.Attachment>((Func<Microsoft.VisualStudio.Services.CodeReview.WebApi.Attachment, Microsoft.TeamFoundation.SourceControl.WebApi.Attachment>) (a => PullRequestCodeReviewConverter.ToGitAttachment(a, requestContext, pullRequest, repoScope))).ToList<Microsoft.TeamFoundation.SourceControl.WebApi.Attachment>();
    }

    public void ServiceEnd(IVssRequestContext systemRequestContext)
    {
    }

    public void ServiceStart(IVssRequestContext systemRequestContext)
    {
    }

    private void VerifyAttachementsSupported(TfsGitPullRequest pullRequest, string fileName)
    {
      if (pullRequest.CodeReviewId > 0)
        return;
      if (fileName != null)
        throw new GitPullRequestAttachmentNotFoundException(fileName);
      throw new GitPullRequestAttachmentsNotSupported();
    }
  }
}
