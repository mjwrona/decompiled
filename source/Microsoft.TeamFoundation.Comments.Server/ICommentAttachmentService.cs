// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Comments.Server.ICommentAttachmentService
// Assembly: Microsoft.TeamFoundation.Comments.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: CBA40CC5-9694-4582-97B5-1660FA9D4307
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Comments.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.IO;

namespace Microsoft.TeamFoundation.Comments.Server
{
  [DefaultServiceImplementation(typeof (CommentAttachmentService))]
  public interface ICommentAttachmentService : IVssFrameworkService
  {
    CommentAttachment UploadAttachment(
      IVssRequestContext requestContext,
      Guid projectId,
      Guid artifactKind,
      string artifactId,
      Stream attachentStream);

    (CommentAttachment, Stream) DownloadAttachment(
      IVssRequestContext requestContext,
      Guid projectId,
      Guid artifactKind,
      string artifactId,
      Guid attachmentId);
  }
}
