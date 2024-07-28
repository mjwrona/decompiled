// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Comments.Server.CommentAttachmentService
// Assembly: Microsoft.TeamFoundation.Comments.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: CBA40CC5-9694-4582-97B5-1660FA9D4307
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Comments.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Microsoft.TeamFoundation.Comments.Server
{
  internal class CommentAttachmentService : 
    CommentServiceBase,
    ICommentAttachmentService,
    IVssFrameworkService
  {
    public CommentAttachmentService()
      : this(VssDateTimeProvider.DefaultProvider)
    {
    }

    internal CommentAttachmentService(IVssDateTimeProvider dateTimeProvider)
      : base(dateTimeProvider)
    {
    }

    internal CommentAttachmentService(
      IVssDateTimeProvider dateTimeProvider,
      IDisposableReadOnlyList<ICommentProvider> providers)
      : base(dateTimeProvider, providers)
    {
    }

    public CommentAttachment UploadAttachment(
      IVssRequestContext requestContext,
      Guid projectId,
      Guid artifactKind,
      string artifactId,
      Stream attachentStream)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForEmptyGuid(artifactKind, nameof (artifactKind));
      ArgumentUtility.CheckStringForNullOrWhiteSpace(artifactId, nameof (artifactId));
      ArgumentUtility.CheckForNull<Stream>(attachentStream, nameof (attachentStream));
      using (requestContext.TraceBlock(140121, 140129, "CommentService", "Service", nameof (UploadAttachment)))
      {
        ICommentProvider provider = this.GetProvider(requestContext, artifactKind);
        IVssRequestContext requestContext1 = requestContext;
        Guid projectId1 = projectId;
        HashSet<string> artifactIds = new HashSet<string>();
        artifactIds.Add(artifactId);
        IDictionary<string, ISecuredObject> dictionary;
        ref IDictionary<string, ISecuredObject> local = ref dictionary;
        provider.CheckAddPermission(requestContext1, projectId1, (ISet<string>) artifactIds, out local);
        int fileId = requestContext.GetService<ITeamFoundationFileService>().UploadFile(requestContext, attachentStream, OwnerId.Generic, projectId);
        CommentAttachment commentAttachment1 = new CommentAttachment(Guid.NewGuid(), fileId)
        {
          CreatedBy = requestContext.GetUserId(),
          CreatedDate = this.dateTimeProvider.UtcNow
        };
        using (CommentComponent component = requestContext.CreateComponent<CommentComponent>())
        {
          CommentAttachment commentAttachment2 = component.AddAttacmhents((IEnumerable<CommentAttachment>) new CommentAttachment[1]
          {
            commentAttachment1
          }).Single<CommentAttachment>();
          commentAttachment2.SetSecuredObject(dictionary[artifactId]);
          return commentAttachment2;
        }
      }
    }

    public (CommentAttachment, Stream) DownloadAttachment(
      IVssRequestContext requestContext,
      Guid projectId,
      Guid artifactKind,
      string artifactId,
      Guid attachmentId)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForEmptyGuid(artifactKind, nameof (artifactKind));
      ArgumentUtility.CheckStringForNullOrWhiteSpace(artifactId, nameof (artifactId));
      ArgumentUtility.CheckForEmptyGuid(attachmentId, nameof (attachmentId));
      using (requestContext.TraceBlock(140111, 140119, "CommentService", "Service", nameof (DownloadAttachment)))
      {
        ICommentProvider provider = this.GetProvider(requestContext, artifactKind);
        IVssRequestContext requestContext1 = requestContext;
        Guid projectId1 = projectId;
        HashSet<string> artifactIds = new HashSet<string>();
        artifactIds.Add(artifactId);
        IDictionary<string, ISecuredObject> dictionary;
        ref IDictionary<string, ISecuredObject> local = ref dictionary;
        provider.CheckReadPermission(requestContext1, projectId1, (ISet<string>) artifactIds, out local);
        CommentToAttachment commentToAttachment = new CommentToAttachment()
        {
          AttachmentId = attachmentId,
          ArtifactId = artifactId,
          ArtifactKind = artifactKind
        };
        CommentAttachment commentAttachment = CommentAttachmentService.GetCommentAttachment(requestContext, commentToAttachment);
        if (commentAttachment == null)
          throw new CommentAttachmentNotFoundException(commentToAttachment.AttachmentId).Expected(requestContext.ServiceName);
        commentAttachment.SetSecuredObject(dictionary[artifactId]);
        Stream stream = requestContext.GetService<ITeamFoundationFileService>().RetrieveFile(requestContext, (long) commentAttachment.FileId, false, out byte[] _, out long _, out CompressionType _);
        return (commentAttachment, stream);
      }
    }

    private static CommentAttachment GetCommentAttachment(
      IVssRequestContext requestContext,
      CommentToAttachment commentToAttachment)
    {
      using (requestContext.TraceBlock(140112, 140113, "CommentService", "Service", nameof (GetCommentAttachment)))
      {
        using (CommentComponent component = requestContext.CreateComponent<CommentComponent>())
          return component.GetAttacmhents((IEnumerable<CommentToAttachment>) new CommentToAttachment[1]
          {
            commentToAttachment
          }).FirstOrDefault<CommentAttachment>();
      }
    }
  }
}
