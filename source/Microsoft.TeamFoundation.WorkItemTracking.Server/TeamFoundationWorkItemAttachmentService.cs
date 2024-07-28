// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.TeamFoundationWorkItemAttachmentService
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

using Microsoft.Azure.Boards.CssNodes;
using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.WorkItemTracking.Internals;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Attachment;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Common;
using Microsoft.TeamFoundation.WorkItemTracking.Server.DataAccess;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server
{
  public class TeamFoundationWorkItemAttachmentService : 
    ITeamFoundationWorkItemAttachmentService,
    IVssFrameworkService
  {
    public Stream RetrieveAttachment(
      IVssRequestContext requestContext,
      Guid? projectId,
      Guid attachmentId,
      out long contentLength,
      out ISecuredObject securedObject,
      out CompressionType compressionType)
    {
      int attachmentTfsFileId = this.GetAttachmentTfsFileId(requestContext, projectId, attachmentId, out securedObject, false);
      return this.RetrieveAttachmentInternal(requestContext, attachmentTfsFileId, out contentLength, out compressionType);
    }

    public Stream RetrieveAttachment(
      IVssRequestContext requestContext,
      Guid? projectId,
      int attachmentExtId,
      out long contentLength,
      out ISecuredObject securedObject,
      out CompressionType compressionType,
      out Guid attachmentGuid)
    {
      int attachmentTfsFileId = this.GetAttachmentTfsFileId(requestContext, projectId, attachmentExtId, out securedObject, out attachmentGuid);
      return this.RetrieveAttachmentInternal(requestContext, attachmentTfsFileId, out contentLength, out compressionType);
    }

    public int GetAttachmentTfsFileId(
      IVssRequestContext requestContext,
      Guid? projectId,
      Guid attachmentId,
      out ISecuredObject securedObject,
      bool includePartialUploads = false)
    {
      ArgumentUtility.CheckForEmptyGuid(attachmentId, nameof (attachmentId));
      InternalAttachmentInfo attachmentInfo = (InternalAttachmentInfo) null;
      requestContext.TraceBlock(900076, 900558, "Services", "AttachmentService", "GetAttachmentFileId", (Action) (() =>
      {
        using (WorkItemAttachmentComponent component = requestContext.CreateComponent<WorkItemAttachmentComponent>())
          attachmentInfo = component.GetAttachment(attachmentId);
      }));
      return this.VerifyAndGetAttachmentFileId(requestContext, projectId, attachmentInfo, out securedObject, includePartialUploads);
    }

    public int GetAttachmentTfsFileId(
      IVssRequestContext requestContext,
      Guid? projectId,
      int attachmentId,
      out ISecuredObject securedObject,
      out Guid attachmentGuid)
    {
      InternalAttachmentInfo attachmentInfo = (InternalAttachmentInfo) null;
      requestContext.TraceBlock(900076, 900558, "Services", "AttachmentService", "GetAttachmentFileId", (Action) (() =>
      {
        using (WorkItemAttachmentComponent component = requestContext.CreateComponent<WorkItemAttachmentComponent>())
          attachmentInfo = component.GetAttachment(attachmentId);
      }));
      attachmentGuid = attachmentInfo != null ? attachmentInfo.Id : throw new AttachmentNotFoundException(attachmentId);
      return this.VerifyAndGetAttachmentFileId(requestContext, projectId, attachmentInfo, out securedObject);
    }

    internal Stream RetrieveAttachmentInternal(
      IVssRequestContext requestContext,
      int tfsFileId,
      out long contentLength,
      out CompressionType compressionType)
    {
      long contentLengthInternal = contentLength = 0L;
      CompressionType compressionTypeInternal = compressionType = CompressionType.None;
      Stream stream = (Stream) null;
      requestContext.TraceBlock(900079, 900080, "Services", "AttachmentService", nameof (RetrieveAttachmentInternal), (Action) (() =>
      {
        stream = requestContext.GetService<TeamFoundationFileService>().RetrieveFile(requestContext, (long) tfsFileId, false, out byte[] _, out contentLengthInternal, out compressionTypeInternal);
        requestContext.Trace(900018, TraceLevel.Verbose, "Services", "AttachmentService", "Content-Length: {0}", (object) contentLengthInternal.ToString((IFormatProvider) CultureInfo.InvariantCulture));
        requestContext.Trace(900500, TraceLevel.Verbose, "Services", "AttachmentService", "Compression Type: {0}", (object) compressionTypeInternal);
      }));
      contentLength = contentLengthInternal;
      compressionType = compressionTypeInternal;
      this.VerifyAttachmentSizeLimit(requestContext, contentLength);
      WorkItemTrackingTelemetry.TraceCustomerIntelligence(requestContext, AttachmentTelemetry.Feature, (object) "Download", (object) tfsFileId.ToString(), (object) contentLength);
      return stream;
    }

    private void VerifyAttachmentSizeLimit(IVssRequestContext requestContext, long contentLength)
    {
      WorkItemTrackingRequestContext trackingRequestContext = requestContext.WitContext();
      int num = CommonWITUtils.HasCrossProjectQueryPermission(requestContext) ? 1 : 0;
      long sizeForPublicUser = trackingRequestContext.ServerSettings.MaxAttachmentSizeForPublicUser;
      if (num == 0 && contentLength > sizeForPublicUser)
        throw new WorkItemAttachmentDownloadExceedsMaxSizeException(sizeForPublicUser);
    }

    private void CheckAttachmentExistsInProject(
      IVssRequestContext requestContext,
      Guid? projectGuid,
      InternalAttachmentInfo attachmentInfo)
    {
      if (projectGuid.HasValue && !(projectGuid.Value == Guid.Empty))
      {
        requestContext.Trace(900140, TraceLevel.Verbose, "Services", "AttachmentService", "TfsFileId: {0}, ProjectId: {1}", (object) attachmentInfo.TfsFileId, (object) projectGuid.Value);
        WorkItemTrackingTreeService service = requestContext.GetService<WorkItemTrackingTreeService>();
        foreach (int permittedAreaId in attachmentInfo.PermittedAreaIds)
        {
          if (service.GetTreeNode(requestContext, projectGuid.Value, permittedAreaId, false) != null)
            return;
        }
        throw new AttachmentNotFoundException(attachmentInfo.TfsFileId);
      }
    }

    private int VerifyAndGetAttachmentFileId(
      IVssRequestContext requestContext,
      Guid? projectGuid,
      InternalAttachmentInfo attachmentInfo,
      out ISecuredObject securedObject,
      bool includePartialUploads = false)
    {
      securedObject = (ISecuredObject) null;
      if (attachmentInfo == null || !(attachmentInfo.TfsFileId != 0 | includePartialUploads))
        throw new AttachmentNotFoundException(0);
      bool flag = false;
      if (VssStringComparer.SID.Equals(attachmentInfo.UploaderSid, requestContext.UserContext.Identifier) && !attachmentInfo.PermittedAreaIds.Any<int>())
      {
        if (projectGuid.HasValue)
        {
          Guid? nullable = projectGuid;
          Guid empty = Guid.Empty;
          if ((nullable.HasValue ? (nullable.HasValue ? (nullable.GetValueOrDefault() != empty ? 1 : 0) : 0) : 1) != 0)
          {
            IVssSecurityNamespace securityNamespace = requestContext.GetService<TeamFoundationSecurityService>().GetSecurityNamespace(requestContext, FrameworkSecurity.TeamProjectNamespaceId);
            string token = securityNamespace.NamespaceExtension.HandleIncomingToken(requestContext, securityNamespace, ProjectInfo.GetProjectUri(projectGuid.Value));
            SecuredObject securedObject1 = new SecuredObject(FrameworkSecurity.TeamProjectNamespaceId, TeamProjectSecurityConstants.GenericRead, token);
            securedObject = (ISecuredObject) securedObject1;
            goto label_6;
          }
        }
        securedObject = (ISecuredObject) new SecuredObject();
label_6:
        flag = true;
      }
      else
      {
        if (!CommonWITUtils.CanAccessCrossProjectWorkItems(requestContext))
          this.CheckAttachmentExistsInProject(requestContext, projectGuid, attachmentInfo);
        IPermissionCheckHelper permissionCheckHelper = this.GetPermissionCheckHelper(requestContext);
        foreach (int permittedAreaId in attachmentInfo.PermittedAreaIds)
        {
          if (permissionCheckHelper.HasWorkItemPermission(permittedAreaId, 16))
          {
            flag = true;
            string itemSecurityToken = PermissionCheckHelper.GetWorkItemSecurityToken(requestContext, permittedAreaId);
            WorkItemSecuredObject itemSecuredObject = new WorkItemSecuredObject();
            itemSecuredObject.SetSecuredToken(itemSecurityToken);
            itemSecuredObject.SetRequiredPermissions(16);
            securedObject = (ISecuredObject) itemSecuredObject;
            break;
          }
        }
      }
      if (!flag)
      {
        WorkItemTrackingTreeService treeService = requestContext.GetService<WorkItemTrackingTreeService>();
        throw new WorkItemUnauthorizedAttachmentException(attachmentInfo.PermittedAreaIds.Select<int, Uri>((Func<int, Uri>) (areaId => treeService.LegacyGetTreeNodeUriForPermissionCheck(requestContext, areaId))).Where<Uri>((Func<Uri, bool>) (x => x != (Uri) null)).Select<Uri, string>((Func<Uri, string>) (x => x.ToString())), AccessType.Read);
      }
      if (attachmentInfo.IsDeleted && !CommonWITUtils.HasReadHistoricalWorkItemResourcesPermission(requestContext))
        throw new AttachmentNotFoundException(attachmentInfo.TfsFileId);
      requestContext.Trace(900137, TraceLevel.Verbose, "Services", "AttachmentService", "TfsFileId: {0}", (object) attachmentInfo.TfsFileId);
      return attachmentInfo.TfsFileId;
    }

    public void RegisterAttachmentUpload(
      IVssRequestContext requestContext,
      Guid? projectId,
      Guid attachmentId,
      out ISecuredObject securedObject,
      string areaPath = null)
    {
      requestContext.Trace(900340, TraceLevel.Verbose, "Services", "AttachmentService", "Checking for user upload permission");
      this.CheckUploadPermission(requestContext, projectId, out securedObject, areaPath);
      using (WorkItemAttachmentComponent component = requestContext.CreateComponent<WorkItemAttachmentComponent>())
        component.CreateAttachment(requestContext.UserContext.Identifier, attachmentId, 0);
    }

    public bool UploadAttachmentChunk(
      IVssRequestContext requestContext,
      Guid? projectId,
      Guid attachmentId,
      Stream inputStream,
      long fileSize,
      long offsetFrom,
      out ISecuredObject securedObject)
    {
      ArgumentUtility.CheckForNull<Stream>(inputStream, nameof (inputStream));
      long maxAttachmentSize = requestContext.GetService<WorkItemTrackingConfigurationSettingService>().GetConfigurationInfo(requestContext).MaxAttachmentSize;
      if (fileSize > maxAttachmentSize)
        throw new WorkItemAttachmentExceedsMaxSizeException(maxAttachmentSize);
      bool isLastBlock = false;
      ISecuredObject generatedSecuredObject = (ISecuredObject) null;
      int tfsFileId;
      requestContext.TraceBlock(900077, 900078, "Services", "AttachmentService", "UploadAttachment", (Action) (() =>
      {
        tfsFileId = this.GetAttachmentTfsFileId(requestContext, projectId, attachmentId, out generatedSecuredObject, true);
        if (offsetFrom == 0L)
          tfsFileId = 0;
        else if (tfsFileId == 0)
          throw new WorkItemAttachmentIncorrectOffsetException(attachmentId.ToString(), offsetFrom);
        isLastBlock = requestContext.GetService<TeamFoundationFileService>().UploadFile(requestContext, ref tfsFileId, inputStream, (byte[]) null, fileSize, fileSize, offsetFrom, CompressionType.None, OwnerId.WorkItemTracking, projectId.HasValue ? projectId.Value : Guid.Empty, (string) null, false);
        if (offsetFrom != 0L)
          return;
        using (WorkItemAttachmentComponent component = requestContext.CreateComponent<WorkItemAttachmentComponent>())
          component.UpdateAttachment(attachmentId, tfsFileId);
      }));
      securedObject = generatedSecuredObject;
      return isLastBlock;
    }

    public void UploadAttachment(
      IVssRequestContext requestContext,
      Guid? projectId,
      Guid attachmentId,
      Stream inputStream,
      out ISecuredObject securedObject,
      string areaPath = null)
    {
      ArgumentUtility.CheckForNull<Stream>(inputStream, nameof (inputStream));
      ArgumentUtility.CheckForEmptyGuid(attachmentId, nameof (attachmentId));
      int tfsFileId = 0;
      ISecuredObject generatedSecuredObject = (ISecuredObject) null;
      requestContext.TraceBlock(900077, 900078, "Services", "AttachmentService", nameof (UploadAttachment), (Action) (() =>
      {
        requestContext.Trace(900340, TraceLevel.Verbose, "Services", "AttachmentService", "Checking for user upload permission");
        this.CheckUploadPermission(requestContext, projectId, out generatedSecuredObject, areaPath);
        requestContext.Trace(900341, TraceLevel.Verbose, "Services", "AttachmentService", "User has permissions to upload.");
        long maxAttachmentSize = requestContext.GetService<WorkItemTrackingConfigurationSettingService>().GetConfigurationInfo(requestContext).MaxAttachmentSize;
        if (inputStream.Length > maxAttachmentSize)
          throw new WorkItemAttachmentExceedsMaxSizeException(maxAttachmentSize);
        tfsFileId = requestContext.GetService<TeamFoundationFileService>().UploadFile(requestContext, inputStream, OwnerId.WorkItemTracking, projectId.HasValue ? projectId.Value : Guid.Empty);
        using (WorkItemAttachmentComponent component = requestContext.CreateComponent<WorkItemAttachmentComponent>())
          component.CreateAttachment(requestContext.UserContext.Identifier, attachmentId, tfsFileId);
      }));
      securedObject = generatedSecuredObject;
      WorkItemTrackingTelemetry.TraceCustomerIntelligence(requestContext, AttachmentTelemetry.Feature, (object) "Upload", (object) tfsFileId.ToString(), (object) inputStream.Length.ToString(), (object) attachmentId.ToString());
    }

    private void CheckUploadPermission(
      IVssRequestContext requestContext,
      Guid? projectGuid,
      out ISecuredObject securedObject,
      string areaPath = null)
    {
      ITreeDictionary snapshot = requestContext.GetService<WorkItemTrackingTreeService>().GetSnapshot(requestContext);
      string str = (string) null;
      TreeNode treeNode;
      if (!string.IsNullOrEmpty(areaPath) && snapshot.TryGetNodeFromPath(requestContext, areaPath, TreeStructureType.Area, out treeNode))
      {
        str = requestContext.GetService<WorkItemTrackingTreeService>().LegacyGetTreeNodeUriForPermissionCheck(requestContext, treeNode.Id).ToString();
        if (projectGuid.HasValue && projectGuid.Value != Guid.Empty && !snapshot.TryGetTreeNode(projectGuid.Value, treeNode.Id, out TreeNode _))
          throw new WorkItemUnauthorizedAttachmentException(projectGuid.Value, areaPath ?? str, AccessType.Write);
      }
      ITeamFoundationSecurityService service1 = requestContext.GetService<ITeamFoundationSecurityService>();
      IVssSecurityNamespace securityNamespace1 = service1.GetSecurityNamespace(requestContext, AuthorizationSecurityConstants.CommonStructureNodeSecurityGuid);
      CustomerIntelligenceService service2 = requestContext.GetService<CustomerIntelligenceService>();
      CustomerIntelligenceData intelligenceData = new CustomerIntelligenceData();
      intelligenceData.Add("areaNodeUri", str ?? string.Empty);
      intelligenceData.Add("userAgent", string.Format("[NonEmail:{0}]", (object) requestContext.UserAgent));
      if (!string.IsNullOrWhiteSpace(str))
      {
        string token = securityNamespace1.NamespaceExtension.HandleIncomingToken(requestContext, securityNamespace1, str);
        if (securityNamespace1.HasPermission(requestContext, token, 32))
        {
          WorkItemSecuredObject itemSecuredObject = new WorkItemSecuredObject();
          itemSecuredObject.SetSecuredToken(token);
          itemSecuredObject.SetRequiredPermissions(32);
          securedObject = (ISecuredObject) itemSecuredObject;
        }
        else
        {
          areaPath = TeamFoundationWorkItemAttachmentService.GetPathFromNodeUri(requestContext, str, snapshot);
          throw new WorkItemUnauthorizedAttachmentException(areaPath ?? str, AccessType.Write);
        }
      }
      else
      {
        if (projectGuid.HasValue)
        {
          Guid? nullable = projectGuid;
          Guid empty = Guid.Empty;
          if ((nullable.HasValue ? (nullable.HasValue ? (nullable.GetValueOrDefault() != empty ? 1 : 0) : 0) : 1) != 0)
          {
            IVssSecurityNamespace securityNamespace2 = service1.GetSecurityNamespace(requestContext, FrameworkSecurity.TeamProjectNamespaceId);
            string token = securityNamespace2.NamespaceExtension.HandleIncomingToken(requestContext, securityNamespace2, ProjectInfo.GetProjectUri(projectGuid.Value));
            SecuredObject securedObject1 = new SecuredObject(FrameworkSecurity.TeamProjectNamespaceId, TeamProjectSecurityConstants.GenericRead, token);
            securedObject = (ISecuredObject) securedObject1;
            goto label_11;
          }
        }
        securedObject = (ISecuredObject) new SecuredObject();
      }
label_11:
      IVssRequestContext requestContext1 = requestContext;
      CustomerIntelligenceData properties = intelligenceData;
      service2.Publish(requestContext1, "WIT", "AttachmentUploadPermission", properties);
    }

    private static string GetPathFromNodeUri(
      IVssRequestContext requestContext,
      string areaNodeUri,
      ITreeDictionary treeService)
    {
      try
      {
        string pathFromNodeUri = (string) null;
        Guid result;
        if (Guid.TryParse(LinkingUtilities.DecodeUri(areaNodeUri).ToolSpecificId, out result))
        {
          TreeNode treeNode = treeService.LegacyGetTreeNode(result);
          if (treeNode != null)
            pathFromNodeUri = treeNode.GetPath(requestContext);
        }
        return pathFromNodeUri;
      }
      catch (Exception ex)
      {
        requestContext.TraceException(900368, "Services", "AttachmentService", ex);
        return (string) null;
      }
    }

    public void DeleteOrphanAttachments(IVssRequestContext requestContext)
    {
      IEnumerable<WorkItemAttachmentInfo> orphanAttachments = (IEnumerable<WorkItemAttachmentInfo>) null;
      requestContext.TraceBlock(900650, 900651, "Services", "AttachmentService", "GetOrphanAttachments", (Action) (() =>
      {
        using (WorkItemAttachmentComponent component = requestContext.CreateComponent<WorkItemAttachmentComponent>())
          orphanAttachments = component.GetOrphanAttachments();
      }));
      if (orphanAttachments == null || orphanAttachments.Count<WorkItemAttachmentInfo>() <= 0)
        return;
      this.DeleteAttachments(requestContext, orphanAttachments);
    }

    public void DeleteAttachmentsWithNoCurrentReference(IVssRequestContext requestContext)
    {
      IEnumerable<WorkItemAttachmentInfo> noCurrentReferenceAttachments = (IEnumerable<WorkItemAttachmentInfo>) null;
      requestContext.TraceBlock(900650, 900651, "Services", "AttachmentService", "GetNoCurrentReferenceAttachments", (Action) (() =>
      {
        using (WorkItemAttachmentComponent component = requestContext.CreateComponent<WorkItemAttachmentComponent>())
          noCurrentReferenceAttachments = component.GetNoCurrentReferenceAttachments();
      }));
      if (noCurrentReferenceAttachments == null || noCurrentReferenceAttachments.Count<WorkItemAttachmentInfo>() <= 0)
        return;
      this.DeleteAttachments(requestContext, noCurrentReferenceAttachments);
    }

    internal void DeleteAttachments(
      IVssRequestContext requestContext,
      IEnumerable<WorkItemAttachmentInfo> attachments)
    {
      requestContext.GetService<TeamFoundationFileService>().DeleteFiles(requestContext, attachments.Select<WorkItemAttachmentInfo, int>((Func<WorkItemAttachmentInfo, int>) (attachment => attachment.TfsFileId)));
      requestContext.TraceBlock(900652, 900653, "Services", "AttachmentService", nameof (DeleteAttachments), (Action) (() =>
      {
        using (WorkItemAttachmentComponent component = requestContext.CreateComponent<WorkItemAttachmentComponent>())
          component.DeleteAttachments(attachments);
      }));
    }

    public void ServiceEnd(IVssRequestContext systemRequestContext)
    {
    }

    public void ServiceStart(IVssRequestContext systemRequestContext)
    {
    }

    protected virtual IPermissionCheckHelper GetPermissionCheckHelper(
      IVssRequestContext requestContext)
    {
      return (IPermissionCheckHelper) new PermissionCheckHelper(requestContext);
    }
  }
}
