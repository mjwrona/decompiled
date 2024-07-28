// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.VersionControl.Server.UploadHandler
// Assembly: Microsoft.TeamFoundation.VersionControl.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: C5D9EE74-8805-4E00-959F-39760D2358B5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.VersionControl.Server.dll

using Microsoft.TeamFoundation.Common;
using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Web;

namespace Microsoft.TeamFoundation.VersionControl.Server
{
  public class UploadHandler : VersionControlHttpHandler
  {
    protected override bool AllowSimplePostRequests => true;

    protected override string Layer => "Upload";

    protected override MethodType MethodType => MethodType.ReadWrite;

    protected override EstimatedMethodCost EstimatedMethodCost => EstimatedMethodCost.Moderate;

    protected override void Execute()
    {
      HttpRequest request = HttpContext.Current.Request;
      HttpResponse response = HttpContext.Current.Response;
      try
      {
        VssPerformanceCounterManager.GetPerformanceCounter("Microsoft.TeamFoundation.VersionControl.Server.PerformanceCounters.CurrentFileUploads").Increment();
        VssPerformanceCounterManager.GetPerformanceCounter("Microsoft.TeamFoundation.VersionControl.Server.PerformanceCounters.CurrentFileUploadsPerSec").Increment();
        MethodInformation methodInformation = this.GetMethodInformation(TimeSpan.FromMinutes((double) this.RequestContext.GetService<IVssRegistryService>().GetValue<int>(this.RequestContext, (RegistryQuery) "/Configuration/VersionControl/UploadHandler/TimeoutInMinutes", true, 60)));
        string workspaceName;
        string workspaceOwner;
        string serverItem;
        byte[] hash;
        HttpPostedFile file;
        long fileLength;
        long compressedLength;
        long offsetFrom;
        CompressionType compressionType;
        try
        {
          this.ParseRequestParameters(this.VersionControlRequestContext, out workspaceName, out workspaceOwner, out serverItem, out hash, out file, out fileLength, out compressedLength, out offsetFrom, out compressionType);
          methodInformation.AddParameter("workspaceName", (object) workspaceName);
          methodInformation.AddParameter("workspaceOwner", (object) workspaceOwner);
          methodInformation.AddParameter("serverItem", (object) serverItem);
          methodInformation.AddParameter("fileLength", (object) fileLength);
          methodInformation.AddParameter("offsetFrom", (object) offsetFrom);
          methodInformation.AddParameter("compressedLength", (object) compressedLength);
          methodInformation.AddParameter("compressionType", (object) compressionType);
        }
        finally
        {
          this.EnterMethod(methodInformation);
        }
        response.BufferOutput = false;
        this.VersionControlRequestContext.VersionControlService.UploadFile(this.VersionControlRequestContext.RequestContext, workspaceName, workspaceOwner, serverItem, hash, file.InputStream, fileLength, compressedLength, offsetFrom, compressionType);
      }
      catch (ItemNotFoundException ex)
      {
        this.HandleException((Exception) ex, "X-VersionControl-Exception", 404, false);
      }
      catch (RequestCanceledException ex)
      {
        this.HandleException((Exception) ex, "X-VersionControl-Exception", 503, false);
      }
      catch (Exception ex)
      {
        this.HandleException(ex, "X-VersionControl-Exception", 500, false);
      }
      finally
      {
        VssPerformanceCounterManager.GetPerformanceCounter("Microsoft.TeamFoundation.VersionControl.Server.PerformanceCounters.CurrentFileUploads").Decrement();
      }
    }

    internal static void UploadFile(
      VersionControlRequestContext versionControlRequestContext,
      string workspaceName,
      string workspaceOwner,
      string serverItem,
      byte[] hash,
      Stream fileStream,
      long fileLength,
      long compressedLength,
      long offsetFrom,
      CompressionType compressionType)
    {
      versionControlRequestContext.RequestContext.TraceEnter(700194, TraceArea.Upload, TraceLayer.BusinessLogic, nameof (UploadFile));
      versionControlRequestContext.RequestContext.Trace(700195, TraceLevel.Info, TraceArea.Upload, TraceLayer.BusinessLogic, "FileLength {0}, compressed length {1}", (object) fileLength, (object) compressedLength);
      IVssRequestContext requestContext = versionControlRequestContext.RequestContext;
      if (compressionType != CompressionType.GZip && compressionType != CompressionType.None)
        throw new IncompatibleCompressionFormatException();
      Workspace workspace = Workspace.FindWorkspace(versionControlRequestContext, workspaceOwner, workspaceName, true);
      versionControlRequestContext.VersionControlService.SecurityWrapper.CheckWorkspacePermission(versionControlRequestContext, 2, workspace);
      PreUploadFileResult uploadFileResult;
      using (ContentComponent contentComponent = versionControlRequestContext.VersionControlService.GetContentComponent(versionControlRequestContext))
        uploadFileResult = contentComponent.PreUploadFile(workspace, serverItem);
      int fileId = uploadFileResult.TempFileId;
      if (uploadFileResult.TempFileId == 0 && uploadFileResult.ExistingHashValue != null && hash != null && ArrayUtil.Equals(hash, uploadFileResult.ExistingHashValue))
      {
        versionControlRequestContext.RequestContext.Trace(700196, TraceLevel.Info, TraceArea.Upload, TraceLayer.BusinessLogic, "Re-upload for file ID {0} - call to file service skipped", (object) uploadFileResult.ExistingFileId);
      }
      else
      {
        if (offsetFrom == 0L)
          fileId = 0;
        if (fileId != 0 && fileId < 1024)
          versionControlRequestContext.RequestContext.Trace(700197, TraceLevel.Error, TraceArea.Upload, TraceLayer.BusinessLogic, "FileId {0} from PreUploadFile is unexpected!", (object) fileId);
        TeamFoundationFileService service = versionControlRequestContext.RequestContext.GetService<TeamFoundationFileService>();
        bool isLastChunk;
        using (versionControlRequestContext.RequestContext.CreateTimeToFirstPageExclusionBlock())
          isLastChunk = service.UploadFile(requestContext, ref fileId, fileStream, hash, compressedLength, fileLength, offsetFrom, compressionType, OwnerId.VersionControl, uploadFileResult.TempFileDataspaceId, (string) null, false);
        if (fileId < 1024)
          versionControlRequestContext.RequestContext.Trace(700198, TraceLevel.Error, TraceArea.Upload, TraceLayer.BusinessLogic, "FileId {0} from UploadFile is unexpected!", (object) fileId);
        using (ContentComponent contentComponent = versionControlRequestContext.VersionControlService.GetContentComponent(versionControlRequestContext))
          contentComponent.PostUploadFile(workspace, serverItem, fileId, isLastChunk);
      }
      versionControlRequestContext.RequestContext.TraceLeave(700199, TraceArea.Upload, TraceLayer.BusinessLogic, nameof (UploadFile));
    }

    private void ParseRequestParameters(
      VersionControlRequestContext versionControlRequestContext,
      out string workspaceName,
      out string workspaceOwner,
      out string serverItem,
      out byte[] hash,
      out HttpPostedFile file,
      out long fileLength,
      out long compressedLength,
      out long offsetFrom,
      out CompressionType compressionType)
    {
      HttpRequest request = HttpContext.Current.Request;
      workspaceName = request.Params["wsname"];
      workspaceOwner = request.Params["wsowner"];
      serverItem = request.Params["item"];
      string s1 = request.Params[nameof (hash)];
      string s2 = request.Params["filelength"];
      string range = request.Params["range"];
      file = request.Files["content"];
      if (s1 == null)
      {
        if (versionControlRequestContext.RequestContext is ITrackClientConnection && ((ITrackClientConnection) versionControlRequestContext.RequestContext).IsClientConnected)
          versionControlRequestContext.RequestContext.Trace(1013161, TraceLevel.Error, TraceArea.Upload, TraceLayer.BusinessLogic, "Client is still connected!");
        else
          versionControlRequestContext.RequestContext.Trace(1013163, TraceLevel.Info, TraceArea.Upload, TraceLayer.BusinessLogic, "Client is not connected!");
        throw new IncompleteUploadException(serverItem);
      }
      if (s2 == null)
      {
        if (versionControlRequestContext.RequestContext is ITrackClientConnection && ((ITrackClientConnection) versionControlRequestContext.RequestContext).IsClientConnected)
          versionControlRequestContext.RequestContext.Trace(1013162, TraceLevel.Error, TraceArea.Upload, TraceLayer.BusinessLogic, "Client is still connected!");
        else
          versionControlRequestContext.RequestContext.Trace(1013164, TraceLevel.Info, TraceArea.Upload, TraceLayer.BusinessLogic, "Client is not connected!");
        throw new IncompleteUploadException(serverItem);
      }
      if (file == null)
      {
        if (versionControlRequestContext.RequestContext is ITrackClientConnection && ((ITrackClientConnection) versionControlRequestContext.RequestContext).IsClientConnected)
          versionControlRequestContext.RequestContext.Trace(1013179, TraceLevel.Error, TraceArea.Upload, TraceLayer.BusinessLogic, "Client is still connected!");
        throw new IncompleteUploadException(serverItem);
      }
      try
      {
        hash = Convert.FromBase64String(s1);
      }
      catch (FormatException ex)
      {
        versionControlRequestContext.RequestContext.TraceException(700200, TraceLevel.Info, TraceArea.Upload, TraceLayer.BusinessLogic, (Exception) ex);
        throw new ArgumentException(Resources.Format("ParameterFormatException", (object) nameof (hash)), (Exception) ex);
      }
      if (!long.TryParse(s2, out fileLength) || fileLength < 0L)
        throw new ArgumentException(Resources.Format("ParameterFormatException", (object) "filelength"));
      compressionType = TeamFoundationFileService.FromMimeType(file.ContentType);
      long end;
      if (range != null)
      {
        if (!UploadHandler.ParseRangeHeader(versionControlRequestContext.RequestContext, range, out compressedLength, out offsetFrom, out end))
          throw new ArgumentException(Resources.Format("ParameterFormatException", (object) "range"));
      }
      else
      {
        compressedLength = (long) file.ContentLength;
        offsetFrom = 0L;
        end = compressedLength;
      }
    }

    private static bool ParseRangeHeader(
      IVssRequestContext requestContext,
      string range,
      out long totalLength,
      out long start,
      out long end)
    {
      int num1 = range.IndexOf('-');
      int num2 = range.IndexOf('/');
      if (range.StartsWith("bytes=", StringComparison.OrdinalIgnoreCase) && num1 > -1)
      {
        if (num2 > num1)
        {
          try
          {
            start = long.Parse(range.Substring(6, num1 - 6), (IFormatProvider) CultureInfo.InvariantCulture);
            end = long.Parse(range.Substring(num1 + 1, num2 - num1 - 1), (IFormatProvider) CultureInfo.InvariantCulture);
            totalLength = long.Parse(range.Substring(num2 + 1), (IFormatProvider) CultureInfo.InvariantCulture);
            return true;
          }
          catch (ArgumentOutOfRangeException ex)
          {
            requestContext.TraceException(700201, TraceLevel.Info, TraceArea.Upload, TraceLayer.BusinessLogic, (Exception) ex);
          }
          catch (FormatException ex)
          {
            requestContext.TraceException(700202, TraceLevel.Info, TraceArea.Upload, TraceLayer.BusinessLogic, (Exception) ex);
          }
        }
      }
      totalLength = 0L;
      start = 0L;
      end = 0L;
      return false;
    }
  }
}
