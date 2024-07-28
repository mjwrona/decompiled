// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.AttachmentUploadHandler
// Assembly: Microsoft.TeamFoundation.TestManagement.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F9B71993-88CC-4B0D-89B6-4ADDEEAB3DE1
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.TestManagement.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.TestManagement.WebApi.Legacy;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Web;

namespace Microsoft.TeamFoundation.TestManagement.Server
{
  public class AttachmentUploadHandler : TeamFoundationHttpHandler
  {
    private const string urlencodedContentType = "application/x-www-form-urlencoded";
    private const string textPlainContentType = "text/plain";
    private const int TestManagementStart = 1015000;

    public AttachmentUploadHandler()
    {
    }

    public AttachmentUploadHandler(string featureAreaName) => this.RequestContext.ServiceName = featureAreaName;

    protected override bool AllowSimplePostRequests => true;

    private void ValidateContentType(HttpRequest request)
    {
      if (string.IsNullOrWhiteSpace(request.ContentType) || string.Equals(request.ContentType, "application/x-www-form-urlencoded", StringComparison.Ordinal) || string.Equals(request.ContentType, "text/plain", StringComparison.Ordinal))
      {
        this.RequestContext.Trace(1015003, TraceLevel.Error, "TestManagement", "AttachmentHandler", "AttachmentUploadHandler.ProcessRequest - ContentType is null");
        throw new InvalidDataException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, ServerResources.InvalidContentType));
      }
    }

    protected override void ProcessRequestImpl(HttpContext context)
    {
      HttpRequest request = context.Request;
      this.ValidateContentType(request);
      TfsTestManagementRequestContext context1 = new TfsTestManagementRequestContext(this.RequestContext);
      this.EnterMethod(new MethodInformation(this.AppendXTSuffixStringIfApplicable("TCMAttachmentUploadHandler"), MethodType.Normal, EstimatedMethodCost.Low, true, true));
      try
      {
        this.RequestContext.UpdateTimeToFirstPage();
        HttpRequestWrapper httpRequestWrapper = new HttpRequestWrapper(request);
        Dictionary<string, string> dictionary = httpRequestWrapper.Params.ToDictionary();
        List<HttpPostedTcmAttachment> postedAttachments = httpRequestWrapper.Files.ToHttpPostedAttachments();
        int testRunId = AttachmentUploadHelper.ReadIntField((TestManagementRequestContext) context1, dictionary, "TestRunId");
        int sessionId = AttachmentUploadHelper.ReadIntField(dictionary, "SessionId", 0);
        if (context1.LegacyTcmServiceHelper.TryUploadAttachments(this.RequestContext, testRunId, sessionId, dictionary, postedAttachments))
          return;
        new AttachmentUploadHelper().ProcessUpload((TestManagementRequestContext) context1, dictionary, postedAttachments);
      }
      catch (Exception ex)
      {
        TeamFoundationTracingService.TraceExceptionRaw(1015000, "TestManagement", "AttachmentHandler", ex);
        this.HandleException(ex, "X-TestManagement-Exception", 500, false);
      }
      finally
      {
        this.LeaveMethod();
      }
    }

    private string AppendXTSuffixStringIfApplicable(string command)
    {
      string str = command;
      if (!string.IsNullOrEmpty(this.RequestContext.UserAgent) && this.RequestContext.UserAgent.StartsWith("Mozilla", true, CultureInfo.InvariantCulture))
        str = command + "_XTWeb";
      return str;
    }
  }
}
