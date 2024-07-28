// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Client.ConnectFailureReason
// Assembly: Microsoft.TeamFoundation.Client, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 03892C75-AE2B-482B-8E0D-B14588A2C857
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Client.dll

using Microsoft.TeamFoundation.Client.Internal;
using Microsoft.TeamFoundation.Framework.Client;
using System;
using System.Net;
using System.Net.Sockets;
using System.Security;

namespace Microsoft.TeamFoundation.Client
{
  internal class ConnectFailureReason
  {
    private ConnectFailureStatus _statusCode;
    private ConnectFailureCategory _category;
    protected Exception _exception;

    protected ConnectFailureReason(ConnectFailureStatus code, Exception ex)
    {
      this._statusCode = code;
      this._exception = ex;
      switch (this._statusCode)
      {
        case ConnectFailureStatus.NotPermitted:
        case ConnectFailureStatus.NotAuthorized:
        case ConnectFailureStatus.AccessCheck:
          this._category = ConnectFailureCategory.NotPermitted;
          break;
        case ConnectFailureStatus.TimedOut:
        case ConnectFailureStatus.ConnectFailure:
          this._category = ConnectFailureCategory.ConnectFailure;
          break;
        case ConnectFailureStatus.TrustFailure:
          this._category = ConnectFailureCategory.TrustFailure;
          break;
        case ConnectFailureStatus.UnExpected:
          this._category = ConnectFailureCategory.UnExpected;
          break;
        case ConnectFailureStatus.NotWorkgroupUser:
        case ConnectFailureStatus.TrialExpired:
          this._category = ConnectFailureCategory.ServerFailure;
          break;
        default:
          this._category = ConnectFailureCategory.Unknown;
          break;
      }
    }

    public ConnectFailureStatus StatusCode => this._statusCode;

    public ConnectFailureCategory Category => this._category;

    public string HelpTopic
    {
      get
      {
        string helpTopic = ConnectToTfsHelpKeyWords.UnableToConnect;
        switch (this.Category)
        {
          case ConnectFailureCategory.NotPermitted:
            helpTopic = ConnectToTfsHelpKeyWords.NoPermission;
            break;
          case ConnectFailureCategory.TrustFailure:
            helpTopic = ConnectToTfsHelpKeyWords.InvalidCertificate;
            break;
          case ConnectFailureCategory.UnExpected:
            helpTopic = ConnectToTfsHelpKeyWords.UnExpected;
            break;
        }
        return helpTopic;
      }
    }

    public virtual string GetErrorMessage(string serverName)
    {
      string empty = string.Empty;
      string errorMessage;
      switch (this.Category)
      {
        case ConnectFailureCategory.NotPermitted:
          TeamFoundationInvalidAuthenticationException exception = this._exception as TeamFoundationInvalidAuthenticationException;
          errorMessage = this.StatusCode != ConnectFailureStatus.AccessCheck ? (exception == null || exception.AuthenticationError != TeamFoundationAuthenticationError.UserMismatched ? ClientResources.ConnectToTfs_NoPermission((object) serverName, (object) this._exception.Message) : exception.Message) : ClientResources.ConnectToTfs_AccessCheck((object) serverName, (object) this._exception.Message);
          break;
        case ConnectFailureCategory.ConnectFailure:
          errorMessage = !(this._exception is TeamFoundationServiceUnavailableException) ? (!(this._exception is WebException) ? ClientResources.ConnectToTfs_UnableToConnect((object) serverName) : ClientResources.ConnectToTfs_Unknown((object) serverName, (object) this._exception.Message)) : this._exception.Message;
          break;
        case ConnectFailureCategory.TrustFailure:
          errorMessage = ClientResources.ConnectToTfs_TrustFailure((object) serverName);
          break;
        case ConnectFailureCategory.UnExpected:
          errorMessage = ClientResources.ConnectToTfs_UnExpected((object) serverName);
          break;
        default:
          errorMessage = ClientResources.ConnectToTfs_Unknown((object) serverName, (object) this._exception.Message);
          break;
      }
      return errorMessage;
    }

    internal static ConnectFailureStatus GetStatus(Exception e)
    {
      ConnectFailureStatus status = ConnectFailureStatus.Unknown;
      switch (e)
      {
        case SecurityException _:
          status = ConnectFailureStatus.NotPermitted;
          break;
        case TeamFoundationServerUnauthorizedException _:
          status = ConnectFailureStatus.NotAuthorized;
          break;
        case AccessCheckException _:
          status = ConnectFailureStatus.AccessCheck;
          break;
        case WebException _:
          WebException webException = (WebException) e;
          if (webException.Status == WebExceptionStatus.TrustFailure)
          {
            status = ConnectFailureStatus.TrustFailure;
            break;
          }
          if (webException.Status == WebExceptionStatus.ProtocolError)
          {
            if (webException.Response is HttpWebResponse response)
            {
              string str = response.Headers.Get("X-TFS-Exception");
              if (str != null)
              {
                if (str.Equals("UnauthorizedWorkgroupUser", StringComparison.OrdinalIgnoreCase))
                {
                  status = ConnectFailureStatus.NotWorkgroupUser;
                  break;
                }
                if (str.Equals("InvalidLicenseException", StringComparison.OrdinalIgnoreCase))
                {
                  status = ConnectFailureStatus.TrialExpired;
                  break;
                }
                break;
              }
              break;
            }
            break;
          }
          status = !(e.InnerException is SocketException innerException) || innerException.ErrorCode != 10060 ? ConnectFailureStatus.ConnectFailure : ConnectFailureStatus.TimedOut;
          break;
        case InvalidOperationException _:
          status = ConnectFailureStatus.UnExpected;
          break;
        case TeamFoundationServiceUnavailableException _:
          status = ConnectFailureStatus.ConnectFailure;
          break;
      }
      return status;
    }

    public static ConnectFailureReason GetReason(Exception e) => new ConnectFailureReason(ConnectFailureReason.GetStatus(e), e);
  }
}
