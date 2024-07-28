// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.WebPlatform.Sdk.Server.ErrorContext
// Assembly: Microsoft.VisualStudio.Services.WebPlatform.Sdk.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A7EB5677-18AD-4D09-80BD-B83CBD009DB7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.WebPlatform.Sdk.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Net;
using System.Web;

namespace Microsoft.VisualStudio.Services.WebPlatform.Sdk.Server
{
  public class ErrorContext
  {
    public ErrorContext(
      bool isHosted,
      Exception exception,
      int statusCode,
      string message,
      Uri requestUri,
      Guid activityId)
    {
      this.ActivityId = activityId;
      this.EncodeErrorDescription = true;
      this.ErrorDateTime = DateTime.UtcNow;
      this.ErrorTitle = WebFrameworkResources.GenericError_Title();
      this.Exception = exception;
      this.Message = message;
      this.PageTitle = message;
      this.RequestUri = requestUri;
      this.StatusCode = statusCode;
      this.IsHosted = isHosted;
      this.SecondaryActions = new List<ErrorAction>();
      this.Initialize();
    }

    public Guid ActivityId { get; private set; }

    public bool DebugMode { get; private set; }

    public bool EncodeErrorDescription { get; set; }

    public DateTime ErrorDateTime { get; private set; }

    public string ErrorDescription { get; set; }

    public string ErrorTitle { get; set; }

    public Exception Exception { get; private set; }

    public string ImageContentId { get; set; }

    public bool IsHosted { get; private set; }

    public string Message { get; set; }

    public string PageTitle { get; set; }

    public ErrorAction PrimaryAction { get; set; }

    public Uri RequestUri { get; private set; }

    public List<ErrorAction> SecondaryActions { get; private set; }

    public string StackTrace { get; private set; }

    public int StatusCode { get; set; }

    private void Initialize()
    {
      if (this.Exception == null)
        return;
      if (this.Exception is HttpException exception)
        this.StatusCode = !(this.Exception is HttpRequestValidationException) ? exception.GetHttpCode() : 400;
      if ((this.Exception is HttpUnhandledException || this.Exception is RequestFilterException) && this.Exception.InnerException != null)
        this.Exception = this.Exception.InnerException;
      UserFriendlyError userFriendlyError = new UserFriendlyError(this.Exception, this.IsHosted, (HttpStatusCode) this.StatusCode, this.ActivityId);
      this.ErrorTitle = userFriendlyError.Title;
      this.ErrorDescription = userFriendlyError.Message;
      if (!this.DebugMode)
        return;
      this.StackTrace = this.Exception.ToString();
    }
  }
}
