// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.ITeamFoundationWorkItemAttachmentService
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.IO;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server
{
  [DefaultServiceImplementation(typeof (TeamFoundationWorkItemAttachmentService))]
  public interface ITeamFoundationWorkItemAttachmentService : IVssFrameworkService
  {
    void UploadAttachment(
      IVssRequestContext requestContext,
      Guid? projectId,
      Guid attachmentId,
      Stream inputStream,
      out ISecuredObject securedObject,
      string areaPath = null);

    bool UploadAttachmentChunk(
      IVssRequestContext requestContext,
      Guid? projectId,
      Guid attachmentId,
      Stream inputStream,
      long fileSize,
      long offsetFrom,
      out ISecuredObject securedObject);

    void RegisterAttachmentUpload(
      IVssRequestContext requestContext,
      Guid? projectId,
      Guid attachmentId,
      out ISecuredObject securedObject,
      string areaPath = null);

    int GetAttachmentTfsFileId(
      IVssRequestContext requestContext,
      Guid? projectId,
      Guid attachmentId,
      out ISecuredObject securedObject,
      bool includePartialUploads = false);

    int GetAttachmentTfsFileId(
      IVssRequestContext requestContext,
      Guid? projectId,
      int attachmentId,
      out ISecuredObject securedObject,
      out Guid attachmentGuid);

    Stream RetrieveAttachment(
      IVssRequestContext requestContext,
      Guid? projectId,
      Guid attachmentId,
      out long contentLength,
      out ISecuredObject securedObject,
      out CompressionType compressionType);

    Stream RetrieveAttachment(
      IVssRequestContext requestContext,
      Guid? projectId,
      int attachmentExtId,
      out long contentLength,
      out ISecuredObject securedObject,
      out CompressionType compressionType,
      out Guid attachmentGuid);

    void DeleteOrphanAttachments(IVssRequestContext requestContext);

    void DeleteAttachmentsWithNoCurrentReference(IVssRequestContext requestContext);
  }
}
