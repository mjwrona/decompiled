// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.DevOps.Comments.Web.Controllers.CommentAttachmentsBaseController
// Assembly: Microsoft.Azure.DevOps.Comments.Web, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A6538262-E3F2-45F5-B799-587642D68EAC
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.DevOps.Comments.Web.dll

using Microsoft.Azure.DevOps.Comments.Web.Factories;
using Microsoft.TeamFoundation.Comments.Server;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.Types;
using System;
using System.IO;

namespace Microsoft.Azure.DevOps.Comments.Web.Controllers
{
  [ApiTelemetry(true, false)]
  public abstract class CommentAttachmentsBaseController : TfsProjectApiController
  {
    protected virtual ICommentAttachmentService CommentAttachmentService => this.TfsRequestContext.GetService<ICommentAttachmentService>();

    protected T CreateAttachment<T>(Guid artifactKind, string artifactId, Stream contentStream) where T : Microsoft.Azure.DevOps.Comments.WebApi.CommentAttachment, new()
    {
      T commentAttachment = CommentFactory.Create<T>(this.TfsRequestContext, this.TfsRequestContext.GetService<ICommentAttachmentService>().UploadAttachment(this.TfsRequestContext, this.ProjectId, artifactKind, artifactId, contentStream));
      this.AddUrlToAttachment<T>(this.ProjectId, artifactId, commentAttachment);
      return commentAttachment;
    }

    protected (Microsoft.TeamFoundation.Comments.Server.CommentAttachment, Stream) GetAttachment(
      Guid artifactKind,
      string artifactId,
      Guid attachmentId)
    {
      (Microsoft.TeamFoundation.Comments.Server.CommentAttachment, Stream) tuple = this.TfsRequestContext.GetService<ICommentAttachmentService>().DownloadAttachment(this.TfsRequestContext, this.ProjectId, artifactKind, artifactId, attachmentId);
      return (tuple.Item1, tuple.Item2);
    }

    protected abstract void AddUrlToAttachment<T>(
      Guid projectId,
      string artifactId,
      T commentAttachment)
      where T : Microsoft.Azure.DevOps.Comments.WebApi.CommentAttachment, new();
  }
}
