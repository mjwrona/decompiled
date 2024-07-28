// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.AttachFileHandler
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server.DataServices, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 929F0284-16B2-4277-9F4A-B615689A77D1
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.WorkItemTracking.Server.DataServices.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Runtime.InteropServices;
using System.Web;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server
{
  public class AttachFileHandler : TeamFoundationHttpHandler
  {
    private const uint ERROR_CODE_REMOTE_HOST_CLOSED_THE_CONNECTION = 2147943395;

    public AttachFileHandler()
      : this((HttpContextBase) new HttpContextWrapper(HttpContext.Current))
    {
    }

    internal AttachFileHandler(HttpContextBase httpContext)
      : base(httpContext)
    {
      this.RequestContext.ServiceName = "WorkItem Tracking";
      this.RequestContext.GetService<WorkItemTrackingConfigurationSettingService>();
    }

    protected override bool AllowSimplePostRequests => true;

    protected override void ProcessRequestImpl(HttpContext context) => this.ProcessRequest();

    public void ProcessRequest()
    {
      string httpMethod = this.HandlerHttpContext.Request.HttpMethod;
      if (VssStringComparer.HttpRequestMethod.Equals(httpMethod, "HEAD") || VssStringComparer.HttpRequestMethod.Equals(httpMethod, "GET"))
        this.ProcessDownload();
      else if (VssStringComparer.HttpRequestMethod.Equals(httpMethod, "POST"))
      {
        ApiStackRouter.ValidateSimplePostRequestOrigin((TeamFoundationHttpHandler) this, this.RequestContext, this.HandlerHttpContext.Request);
        this.ProcessUpload();
      }
      else
      {
        this.HandlerHttpContext.ApplicationInstance.CompleteRequest();
        this.HandlerHttpContext.Response.StatusCode = 405;
        this.HandlerHttpContext.Response.ContentType = "text/plain";
        this.HandlerHttpContext.Response.Write("AttachFileHandler.ashx: Only HEAD, GET and POST request methods are supported.");
      }
    }

    private void ProcessDownload()
    {
      MethodInformation methodInformation = new MethodInformation("AttachmentDownload", MethodType.Normal, EstimatedMethodCost.Low, TimeSpan.FromMinutes(60.0));
      this.EnterMethod(methodInformation);
      this.RequestContext.TraceEnter(900328, "WebServices", nameof (AttachFileHandler), nameof (ProcessDownload));
      HttpRequestBase request = this.HandlerHttpContext.Request;
      HttpResponseBase response = this.HandlerHttpContext.Response;
      bool closeResponse = false;
      try
      {
        if (!request.IsAuthenticated)
        {
          this.RequestContext.Trace(900329, TraceLevel.Verbose, "WebServices", nameof (AttachFileHandler), "User is not authenticated!");
          this.HandleException((Exception) new LegacySecurityException(ResourceStrings.Get("FileDownloadUserNotAuthenticated"), 600034), "X-WorkItemTracking-Exception", 401, false);
        }
        else
        {
          Guid result = Guid.Empty;
          int attachmentExtId = 0;
          if (request.Params["FileID"] != null)
          {
            methodInformation.AddParameter("fileId", (object) request.Params["FileID"]);
            try
            {
              this.RequestContext.Trace(900590, TraceLevel.Info, "WebServices", nameof (AttachFileHandler), request.Params["FileID"]);
              attachmentExtId = int.Parse(request.Params["FileID"], (IFormatProvider) CultureInfo.InvariantCulture);
            }
            catch (FormatException ex)
            {
              this.RequestContext.Trace(900330, TraceLevel.Error, "WebServices", nameof (AttachFileHandler), "FormatException");
              this.HandleException(new ArgumentException(ResourceStrings.Get("FileDownloadInvalidFileId")).Expected(this.RequestContext.ServiceName), "X-WorkItemTracking-Exception", 400, false);
              return;
            }
            catch (OverflowException ex)
            {
              this.RequestContext.Trace(900331, TraceLevel.Error, "WebServices", nameof (AttachFileHandler), "OverflowException");
              this.HandleException(new ArgumentException(ResourceStrings.Get("FileDownloadInvalidFileId")).Expected(this.RequestContext.ServiceName), "X-WorkItemTracking-Exception", 400, false);
              return;
            }
            catch (ArgumentOutOfRangeException ex)
            {
              this.RequestContext.Trace(900332, TraceLevel.Error, "WebServices", nameof (AttachFileHandler), "ArgumentOutOfRangeException");
              this.HandleException(new ArgumentException(ResourceStrings.Get("FileDownloadInvalidFileId")).Expected(this.RequestContext.ServiceName), "X-WorkItemTracking-Exception", 400, false);
              return;
            }
          }
          else
          {
            string str = request.Params["FileNameGUID"];
            if (string.IsNullOrEmpty(str))
            {
              this.RequestContext.Trace(900333, TraceLevel.Error, "WebServices", nameof (AttachFileHandler), "File ID Not Provided. Returning 400");
              this.HandleException(new ArgumentException(ResourceStrings.Get("FileDownloadInvalidFileId")).Expected(this.RequestContext.ServiceName), "X-WorkItemTracking-Exception", 400, false);
              return;
            }
            methodInformation.AddParameter("fileGuid", (object) str);
            if (!Guid.TryParse(str, out result) || result == Guid.Empty)
            {
              this.RequestContext.Trace(900334, TraceLevel.Error, "WebServices", nameof (AttachFileHandler), "FormatException");
              this.HandleException(new ArgumentException(ResourceStrings.Get("FileDownloadInvalidFileId")).Expected(this.RequestContext.ServiceName), "X-WorkItemTracking-Exception", 400, false);
              return;
            }
          }
          ITeamFoundationWorkItemAttachmentService service = this.RequestContext.GetService<ITeamFoundationWorkItemAttachmentService>();
          Stream stream = (Stream) null;
          long contentLength = 0;
          try
          {
            using (this.RequestContext.CreateTimeToFirstPageExclusionBlock())
              stream = attachmentExtId == 0 ? service.RetrieveAttachment(this.RequestContext, new Guid?(), result, out contentLength, out ISecuredObject _, out CompressionType _) : service.RetrieveAttachment(this.RequestContext, new Guid?(), attachmentExtId, out contentLength, out ISecuredObject _, out CompressionType _, out Guid _);
          }
          catch (AttachmentNotFoundException ex)
          {
            this.RequestContext.Trace(900335, TraceLevel.Error, "WebServices", nameof (AttachFileHandler), "AttachmentNotFoundException");
            this.HandleException(new ArgumentException(ResourceStrings.Get("FileDownloadInvalidFileId")).Expected(this.RequestContext.ServiceName), "X-WorkItemTracking-Exception", 400, false);
            return;
          }
          catch (WorkItemUnauthorizedAttachmentException ex)
          {
            this.RequestContext.Trace(900339, TraceLevel.Error, "WebServices", nameof (AttachFileHandler), "WorkItemUnauthorizedException");
            this.HandleException(new ArgumentException(ResourceStrings.Get("FileDownloadUserNotAuthenticated")).Expected(this.RequestContext.ServiceName), "X-WorkItemTracking-Exception", 400, false);
            return;
          }
          using (stream)
          {
            response.BufferOutput = false;
            string str1 = "attachment";
            string str2 = "application/octet-stream";
            string str3 = request.Params["FileName"];
            methodInformation.AddParameter("fileName", (object) request.Params[str3]);
            if (!string.IsNullOrEmpty(str3))
            {
              string mimeMapping = Microsoft.TeamFoundation.Framework.Server.MimeMapping.GetMimeMapping(str3);
              if (mimeMapping.StartsWith("image/", StringComparison.OrdinalIgnoreCase))
              {
                str2 = mimeMapping;
                if (!mimeMapping.StartsWith("image/svg", StringComparison.OrdinalIgnoreCase))
                  str1 = string.Empty;
              }
              else
                str1 = str1 + ";filename=" + HttpUtility.UrlEncode(str3).Replace("+", "%20");
            }
            if (!string.IsNullOrWhiteSpace(str1))
              response.AppendHeader("content-disposition", str1);
            response.AppendHeader("Content-Length", contentLength.ToString((IFormatProvider) CultureInfo.InvariantCulture));
            response.ContentType = str2;
            closeResponse = true;
            this.RequestContext.RequestTimer.SetTimeToFirstPageEnd();
            stream.CopyTo(response.OutputStream);
          }
        }
      }
      catch (Exception ex)
      {
        if (this.RequestContext is ITrackClientConnection && ((ITrackClientConnection) this.RequestContext).IsClientConnected)
        {
          if (ex is HttpException && ((ExternalException) ex).ErrorCode == -2147023901)
            return;
          string message = ResourceStrings.Format("UnknownServiceError", (object) ex.Message);
          TeamFoundationEventLog.Default.LogException(this.RequestContext, message, (Exception) new LegacyConfigurationException(message, ex), TeamFoundationEventId.WitBaseEventId, EventLogEntryType.Error);
          this.HandleException((Exception) new LegacyServerException(message), "X-WorkItemTracking-Exception", 500, closeResponse);
        }
        else
          this.RequestContext.TraceException(900361, "WebServices", nameof (AttachFileHandler), ex);
      }
      finally
      {
        this.LeaveMethod();
        this.RequestContext.TraceLeave(900336, "WebServices", nameof (AttachFileHandler), nameof (ProcessDownload));
      }
    }

    private void ProcessUpload()
    {
      MethodInformation methodInformation = new MethodInformation("AttachmentUpload", MethodType.ReadWrite, EstimatedMethodCost.Low, true, true);
      this.EnterMethod(methodInformation);
      this.RequestContext.TraceEnter(900337, "WebServices", nameof (AttachFileHandler), nameof (ProcessUpload));
      HttpRequestBase request = this.HandlerHttpContext.Request;
      this.HandlerHttpContext.Response.ContentType = "text/plain";
      try
      {
        if (!request.IsAuthenticated)
        {
          this.RequestContext.Trace(900338, TraceLevel.Verbose, "WebServices", nameof (AttachFileHandler), "User is not authenticated!");
          this.HandleException((Exception) new LegacySecurityException(ResourceStrings.Get("FileUploadUserNotAuthenticated"), 600034), "X-WorkItemTracking-Exception", 401, false);
        }
        else
        {
          string str1 = request.Params["AreaNodeUri"];
          this.RequestContext.Trace(900344, TraceLevel.Verbose, "WebServices", nameof (AttachFileHandler), string.IsNullOrEmpty(str1) ? "New File Received without AreaNodeUri" : string.Format((IFormatProvider) CultureInfo.InvariantCulture, "New File Received with AreaNodeUri:{0}", (object) str1));
          methodInformation.AddParameter("areaNodeUriString", (object) str1);
          string str2 = request.Params["FileNameGUID"];
          if (string.IsNullOrEmpty(str2))
          {
            this.RequestContext.Trace(900342, TraceLevel.Error, "WebServices", nameof (AttachFileHandler), "Invalid GUID");
            this.HandleException(new ArgumentException(ResourceStrings.Get("FileUploadInvalidGuid")).Expected(this.RequestContext.ServiceName), "X-WorkItemTracking-Exception", 400, false);
          }
          else
          {
            methodInformation.AddParameter("fileGuid", (object) str2);
            this.RequestContext.Trace(900346, TraceLevel.Verbose, "WebServices", nameof (AttachFileHandler), "New File Received with GUID:{0}", (object) str2);
            Guid result;
            if (!Guid.TryParse(str2, out result) || result == Guid.Empty)
            {
              this.RequestContext.Trace(900343, TraceLevel.Error, "WebServices", nameof (AttachFileHandler), "Invalid GUID");
              this.HandleException(new ArgumentException(ResourceStrings.Get("FileUploadInvalidGuid")).Expected(this.RequestContext.ServiceName), "X-WorkItemTracking-Exception", 400, false);
            }
            else
            {
              HttpPostedFileBase httpPostedFileBase = (HttpPostedFileBase) null;
              using (this.RequestContext.CreateTimeToFirstPageExclusionBlock())
              {
                httpPostedFileBase = request.Files["Content"];
                if (httpPostedFileBase == null)
                {
                  this.RequestContext.Trace(900754, TraceLevel.Error, "WebServices", nameof (AttachFileHandler), "File content is missing");
                  this.HandleException((Exception) new IncompleteUploadException(str2), "X-WorkItemTracking-Exception", 400, false);
                  return;
                }
              }
              try
              {
                ITeamFoundationWorkItemAttachmentService service = this.RequestContext.GetService<ITeamFoundationWorkItemAttachmentService>();
                using (this.RequestContext.CreateTimeToFirstPageExclusionBlock())
                  service.UploadAttachment(this.RequestContext, new Guid?(), result, httpPostedFileBase.InputStream, out ISecuredObject _, str1);
              }
              catch (UnauthorizedAccessException ex)
              {
                this.RequestContext.Trace(900592, TraceLevel.Verbose, "WebServices", nameof (AttachFileHandler), "User does not have permissions to upload attachment to the area! AreaID: {0} Action: {1} User: {2}", (object) str1, (object) "WORK_ITEM_WRITE", (object) this.RequestContext.DomainUserName);
                this.HandleException((Exception) ex, "X-WorkItemTracking-Exception", 403, false);
              }
            }
          }
        }
      }
      catch (WorkItemAttachmentException ex)
      {
        this.RequestContext.TraceException(907501, "Services", "AttachmentService", (Exception) ex);
        this.HandleException((Exception) ex, "X-WorkItemTracking-Exception", 400, false);
      }
      catch (WorkItemTrackingServiceException ex)
      {
        this.RequestContext.TraceException(907500, "Services", "AttachmentService", (Exception) ex);
        this.HandleException((Exception) ex, "X-WorkItemTracking-Exception", 500, false);
      }
      catch (Exception ex)
      {
        string message = ResourceStrings.Format("UnknownServiceError", (object) ex.Message);
        TeamFoundationEventLog.Default.LogException(this.RequestContext, message, (Exception) new LegacyConfigurationException(message, ex), TeamFoundationEventId.WitBaseEventId, EventLogEntryType.Error);
        this.HandleException((Exception) new LegacyServerException(message), "X-WorkItemTracking-Exception", 500, false);
      }
      finally
      {
        this.LeaveMethod();
        this.RequestContext.TraceLeave(900593, "WebServices", nameof (AttachFileHandler), nameof (ProcessUpload));
      }
    }
  }
}
