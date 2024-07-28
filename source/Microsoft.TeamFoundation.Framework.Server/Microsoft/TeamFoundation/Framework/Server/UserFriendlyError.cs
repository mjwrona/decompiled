// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.UserFriendlyError
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.VisualStudio.Services.Common;
using System;
using System.Data.SqlClient;
using System.Net;
using System.Net.Mail;
using System.Security.Authentication;
using System.Web;
using System.Web.Configuration;
using System.Web.Mvc;

namespace Microsoft.TeamFoundation.Framework.Server
{
  public class UserFriendlyError
  {
    private static readonly bool s_isHosted;

    static UserFriendlyError()
    {
      try
      {
        string appSetting = WebConfigurationManager.AppSettings["IsHosted"];
        UserFriendlyError.s_isHosted = !string.IsNullOrEmpty(appSetting) && string.Equals(appSetting, "true", StringComparison.OrdinalIgnoreCase);
      }
      catch
      {
      }
    }

    public UserFriendlyError(
      Exception exception,
      bool isHosted = false,
      HttpStatusCode httpStatusCode = HttpStatusCode.InternalServerError,
      Guid activityId = default (Guid))
    {
      this.ParseException(exception, isHosted, httpStatusCode, activityId);
      if (!isHosted || this.ShowMessageToHostedUsers)
        return;
      this.Message = UserFriendlyError.FormatInternalError(isHosted, activityId, httpStatusCode);
      this.Title = FrameworkResources.ErrorCaption((object) this.Message);
    }

    public string Message { get; private set; }

    public string Title { get; private set; }

    public bool ReportException { get; private set; }

    public bool ShowMessageToHostedUsers { get; private set; }

    public static string GetMessageFromException(
      Exception e,
      HttpStatusCode httpStatusCode = HttpStatusCode.InternalServerError,
      Guid activityId = default (Guid))
    {
      return new UserFriendlyError(e, UserFriendlyError.s_isHosted, httpStatusCode, activityId).Message;
    }

    public static bool ShouldReportException(Exception e) => new UserFriendlyError(e).ReportException;

    private void ParseException(
      Exception exception,
      bool isHosted,
      HttpStatusCode httpStatusCode,
      Guid activityId)
    {
      this.ReportException = false;
      if (exception == null || string.IsNullOrEmpty(exception.Message))
      {
        if (exception is VssException && ((VssException) exception).EventId == TeamFoundationEventId.SecurityException || exception is TeamFoundationServerUnauthorizedException || exception is UnauthorizedAccessException)
        {
          this.Message = FrameworkResources.Error_UnAuthorizedAccess();
          this.Title = this.Message;
          this.ShowMessageToHostedUsers = true;
        }
        else
        {
          this.Message = UserFriendlyError.FormatInternalError(isHosted, activityId, httpStatusCode);
          this.Title = FrameworkResources.ErrorCaption((object) this.Message);
          if (exception is RequestCanceledException || exception is DatabaseOperationCanceledException)
            this.ReportException = false;
          else
            this.ReportException = true;
        }
      }
      else
      {
        if (exception is AggregateException)
        {
          AggregateException aggregateException = (exception as AggregateException).Flatten();
          if (aggregateException.InnerExceptions.Count == 1)
            exception = aggregateException.InnerExceptions[0];
        }
        if (exception is DatabaseFullException)
        {
          this.Message = SecretUtility.ScrubSecrets((exception as DatabaseFullException).CleanMessage);
          this.Title = this.Message;
          this.ShowMessageToHostedUsers = true;
        }
        else if (exception is TeamFoundationServerException || exception.GetType().FullName.StartsWith("Microsoft.TeamFoundation"))
        {
          this.Message = SecretUtility.ScrubSecrets(exception.Message);
          this.Title = this.Message;
          this.ShowMessageToHostedUsers = true;
        }
        else
        {
          switch (exception)
          {
            case VssServiceException _:
              this.Message = SecretUtility.ScrubSecrets(exception.Message);
              this.Title = this.Message;
              this.ShowMessageToHostedUsers = true;
              break;
            case HttpAntiForgeryException _:
              this.Message = FrameworkResources.CookiesSupportRequired((object) exception.Message);
              this.Title = this.Message;
              this.ShowMessageToHostedUsers = true;
              break;
            case HttpException _:
              this.Message = SecretUtility.ScrubSecrets(exception.Message);
              this.Title = this.Message;
              switch ((exception as HttpException).GetHttpCode())
              {
                case 400:
                case 401:
                case 403:
                case 404:
                  this.ShowMessageToHostedUsers = true;
                  return;
                default:
                  return;
              }
            default:
              if (exception.InnerException != null && !string.IsNullOrEmpty(exception.InnerException.Message))
              {
                this.ParseException(exception.InnerException, isHosted, httpStatusCode, activityId);
                this.ReportException = true;
                break;
              }
              switch (exception)
              {
                case ArgumentException _:
                case NotSupportedException _:
                case InvalidOperationException _:
                  this.Message = SecretUtility.ScrubSecrets(exception.Message);
                  this.Title = this.Message;
                  this.ShowMessageToHostedUsers = true;
                  return;
                case HttpRequestValidationException _:
                  this.Message = FrameworkResources.InvalidAndPotentiallyDangerousRequest();
                  this.Title = this.Message;
                  return;
                case UnauthorizedAccessException _:
                  this.Message = SecretUtility.ScrubSecrets(exception.Message);
                  this.Title = this.Message;
                  this.ShowMessageToHostedUsers = true;
                  return;
                case SmtpException _:
                  this.Message = SecretUtility.ScrubSecrets(exception.Message);
                  this.Title = this.Message;
                  this.ShowMessageToHostedUsers = false;
                  return;
                case AuthenticationException _:
                  this.Message = SecretUtility.ScrubSecrets(exception.Message);
                  this.Title = this.Message;
                  this.ShowMessageToHostedUsers = false;
                  return;
                case SqlException _:
                  this.Message = FrameworkResources.SqlException((object) (exception as SqlException).Number);
                  this.Title = this.Message;
                  this.ShowMessageToHostedUsers = false;
                  return;
                default:
                  this.Message = UserFriendlyError.FormatInternalError(isHosted, activityId, httpStatusCode);
                  this.Title = FrameworkResources.ErrorCaption((object) this.Message);
                  this.ReportException = true;
                  return;
              }
          }
        }
      }
    }

    private static string FormatInternalError(
      bool isHosted,
      Guid activityId,
      HttpStatusCode httpStatusCode)
    {
      if (httpStatusCode == HttpStatusCode.ServiceUnavailable)
      {
        if (!isHosted)
          return FrameworkResources.TeamFoundationUnavilable();
        return !(activityId != Guid.Empty) ? FrameworkResources.Error_TeamFoundationUnavailable() : FrameworkResources.Error_TeamFoundationUnavailableWithCorrelation((object) activityId);
      }
      return !(activityId != Guid.Empty) ? FrameworkResources.Error_InternalTFSFailure() : FrameworkResources.Error_InternalTFSFailureWithCorrelation((object) activityId);
    }
  }
}
