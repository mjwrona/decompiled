// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.ErrorModel
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.Platform, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A6A2C403-5081-466C-A570-9B50BFA8E213
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.Platform.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Globalization;
using System.Net;
using System.Web.Mvc;

namespace Microsoft.TeamFoundation.Server.WebAccess
{
  public class ErrorModel
  {
    private readonly UrlHelper urlHelper;

    private HttpStatusCode StatusCode { get; set; }

    public string UserName { get; private set; }

    public string Code { get; private set; }

    public string Name { get; private set; }

    public string Message { get; set; }

    public string HtmlMessage { get; set; }

    public bool EncodeTips { get; set; }

    public string[] Tips { get; set; }

    public string Title { get; set; }

    public string InnerMessage { get; set; }

    public Guid ActivityId { get; set; }

    public DateTime ErrorDateTime { get; set; }

    public bool ShowInnerMessage { get; set; }

    public bool EncodeStackTrace { get; set; }

    public string StackTrace { get; set; }

    public string ActionText { get; private set; }

    public string ActionLink { get; private set; }

    public string SecondActionText { get; private set; }

    public string SecondActionLink { get; private set; }

    public string ServiceCommunicationLink { get; set; }

    public string ServiceCommunicationLinkText { get; set; }

    public string HeaderLogoBase64 { get; set; }

    public bool HasAction => !string.IsNullOrEmpty(this.ActionLink) && !string.IsNullOrEmpty(this.ActionText);

    public bool HasSecondAction => !string.IsNullOrEmpty(this.SecondActionLink) && !string.IsNullOrEmpty(this.SecondActionText);

    public bool HasHtmlMessage => !string.IsNullOrEmpty(this.HtmlMessage);

    public bool HasInnerMessage => this.ShowInnerMessage && !string.IsNullOrEmpty(this.InnerMessage);

    public bool HasStackTrace => !string.IsNullOrEmpty(this.StackTrace);

    public bool HasTips => this.Tips != null && this.Tips.Length != 0;

    public ErrorModel(int statusCode, string userName, UrlHelper urlHelper)
    {
      this.urlHelper = urlHelper;
      this.StatusCode = (HttpStatusCode) statusCode;
      this.UserName = userName;
      this.EncodeTips = true;
      this.Initialize();
    }

    private string[] ParseTips(string tips)
    {
      if (string.IsNullOrEmpty(tips))
        return (string[]) null;
      return tips.Split(new string[1]{ Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
    }

    private void Initialize()
    {
      this.Code = ((int) this.StatusCode).ToString((IFormatProvider) CultureInfo.CurrentCulture);
      switch (this.StatusCode)
      {
        case HttpStatusCode.BadRequest:
          this.Name = PlatformResources.ErrorName_BadRequest;
          this.Message = PlatformResources.ErrorMessage_BadRequest;
          break;
        case HttpStatusCode.Unauthorized:
          this.Name = PlatformResources.ErrorName_Unauthorized;
          if (!string.IsNullOrEmpty(this.UserName))
          {
            this.Message = string.Format((IFormatProvider) CultureInfo.CurrentUICulture, PlatformResources.ErrorMessage_Unauthorized, (object) SafeHtmlWrapper.MakeSafeWithHtmlEncode(this.UserName));
            this.Tips = this.ParseTips(string.Format((IFormatProvider) CultureInfo.CurrentUICulture, PlatformResources.Tips_Unauthorized, (object) SafeHtmlWrapper.MakeSafeWithHtmlEncode(this.UserName)));
            this.EncodeTips = false;
            break;
          }
          this.Message = PlatformResources.ErrorMessage_UnauthorizedNoUser;
          break;
        case HttpStatusCode.Forbidden:
          this.Name = PlatformResources.ErrorName_Forbidden;
          this.Message = !string.IsNullOrEmpty(this.UserName) ? string.Format((IFormatProvider) CultureInfo.CurrentUICulture, PlatformResources.ErrorMessage_Forbidden, (object) SafeHtmlWrapper.MakeSafeWithHtmlEncode(this.UserName)) : PlatformResources.ErrorMessage_ForbiddenNoUser;
          break;
        case HttpStatusCode.NotFound:
          this.Name = PlatformResources.ErrorName_NotFound;
          this.Message = PlatformResources.ErrorMessage_NotFound;
          this.Tips = this.ParseTips(PlatformResources.Tips_NotFound);
          break;
        case HttpStatusCode.Gone:
          this.Name = PlatformResources.ErrorName_Gone;
          this.Message = PlatformResources.ErrorMessage_Gone;
          break;
        case (HttpStatusCode) 429:
          this.Name = PlatformResources.ErrorName_TooManyRequests;
          this.Message = PlatformResources.ErrorMessage_TooManyRequests;
          break;
        case HttpStatusCode.InternalServerError:
          this.Name = PlatformResources.ErrorName_InternalServerError;
          this.Message = PlatformResources.ErrorMessage_InternalServerError;
          this.ShowInnerMessage = true;
          break;
        case HttpStatusCode.ServiceUnavailable:
          this.Name = PlatformResources.ErrorName_ServiceUnavailable;
          this.Message = PlatformResources.ErrorMessage_ServiceUnavailable;
          break;
        default:
          this.Code = "ERR";
          this.Name = PlatformResources.ErrorName_GeneralException;
          this.Message = PlatformResources.ErrorMessage_GeneralException;
          this.ShowInnerMessage = true;
          break;
      }
    }

    public void SetActionLink(string signoutURL, string rootURL, string signinURL)
    {
      switch (this.StatusCode)
      {
        case HttpStatusCode.Unauthorized:
          if (!string.IsNullOrEmpty(this.UserName))
          {
            this.ActionText = PlatformResources.ActionLink_SignOut;
            this.ActionLink = signoutURL;
            break;
          }
          this.ActionText = PlatformResources.ActionLink_SignIn;
          this.ActionLink = signinURL;
          break;
        case HttpStatusCode.Forbidden:
          this.ActionText = PlatformResources.ActionLink_SignOut;
          this.ActionLink = signoutURL;
          break;
        case HttpStatusCode.NotFound:
          this.ActionText = PlatformResources.ActionLink_GoBackHome;
          this.ActionLink = rootURL;
          break;
        case (HttpStatusCode) 429:
          if (string.Equals(this.UserName, "Anonymous", StringComparison.InvariantCultureIgnoreCase))
          {
            this.ActionText = PlatformResources.ActionLink_SignIn;
            this.ActionLink = signinURL;
            break;
          }
          this.ActionText = PlatformResources.ActionLink_TellUsAboutThis;
          this.ActionLink = this.urlHelper.ForwardLink(328778);
          break;
        case HttpStatusCode.InternalServerError:
          this.ActionText = PlatformResources.ActionLink_TellUsAboutThis;
          this.ActionLink = this.urlHelper.ForwardLink(328778);
          break;
        case HttpStatusCode.ServiceUnavailable:
          this.ActionText = PlatformResources.ActionLink_ViewServiceStatus;
          this.ActionLink = this.urlHelper.ForwardLink(242573);
          break;
        default:
          this.ActionText = PlatformResources.ActionLink_TellUsAboutThis;
          this.ActionLink = this.urlHelper.ForwardLink(328778);
          break;
      }
    }

    public void SetSwitchUserActionLinks(string actionText, string actionLink, string signoutURL)
    {
      this.ActionText = actionText;
      this.ActionLink = actionLink;
      this.SecondActionText = PlatformResources.ActionLink_SignOut;
      this.SecondActionLink = signoutURL;
    }
  }
}
