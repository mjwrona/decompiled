// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Licensing.UserInvitedToAccountNotificationEmailData
// Assembly: Microsoft.VisualStudio.Services.Licensing, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1B3CC7E6-129F-4267-B8E5-2210320176C7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Licensing.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.EmailNotification;
using System;
using System.Globalization;

namespace Microsoft.VisualStudio.Services.Licensing
{
  public class UserInvitedToAccountNotificationEmailData : BaseNotificationEmailData
  {
    protected const string InvitingUserNameToken = "InvitingUserName";
    protected const string AccountUrlToken = "AccountUrl";
    private const string InvitingUserEmailToken = "InvitingUserEmail";

    public UserInvitedToAccountNotificationEmailData(
      IVssRequestContext requestContext,
      License assignedLicense,
      bool featureFlagForLicenseInfoEnabled,
      Uri accountUri,
      CultureInfo culture = null)
    {
      this.AccountUrl = accountUri.ToString();
      this.HeaderType = EmailTemplateHeaderType.ServiceNotification;
      this.FooterType = EmailTemplateFooterType.ServiceNotification;
      this.Body = this.GetBody();
      this.IsHtml = true;
      string str = string.Empty;
      if (featureFlagForLicenseInfoEnabled && assignedLicense is AccountLicense && assignedLicense.GetLicenseAsInt32() == 4)
        str = LicensingResources.AdvancedLicenseMessage((object) UserInvitedToAccountNotificationEmailData.Links.TestHub, (object) UserInvitedToAccountNotificationEmailData.Links.DownloadMTM);
      this.Attributes["AdvancedLicenseMessage"] = str;
      this.AddLocalizedContent(LicensingResources.Manager, culture);
      this.TokenFormatter.Format("EmailContentJoinPersonAtAccount", this.Tokenize(nameof (InvitingUserName)), this.Tokenize(nameof (AccountUrl)));
      this.TokenFormatter.FormatAnchor("EmailContentServicesYoullLoveLike", UserInvitedToAccountNotificationEmailData.Links.Services);
      this.TokenFormatter.FormatAnchor("EmailContentUseVisualStudioEclipse", new string[2]
      {
        UserInvitedToAccountNotificationEmailData.Links.VisualStudio,
        UserInvitedToAccountNotificationEmailData.Links.Eclipse
      });
      this.TokenFormatter.FormatAnchor("EmailContentNeedHelp", UserInvitedToAccountNotificationEmailData.Links.Support);
      this.TokenFormatter.Format("EmailContentNotSureWhatToDo", this.Tokenize(nameof (InvitingUserName)), this.Tokenize(nameof (InvitingUserEmail)));
    }

    public string AccountName
    {
      set => this.Attributes[nameof (AccountName)] = value;
    }

    public string UserEmail
    {
      set => this.Attributes[nameof (UserEmail)] = value;
    }

    public string InvitingUserEmail
    {
      set => this.Attributes[nameof (InvitingUserEmail)] = value;
    }

    public string InvitingUserName
    {
      set => this.Attributes[nameof (InvitingUserName)] = value;
      get => this.Attributes[nameof (InvitingUserName)];
    }

    public string AccountUrl
    {
      set => this.Attributes[nameof (AccountUrl)] = value;
    }

    public string AccountHost
    {
      set => this.Attributes[nameof (AccountHost)] = value;
    }

    public string SignInAddress
    {
      set => this.Attributes[nameof (SignInAddress)] = value;
    }

    public string OpenInVsUri
    {
      set => this.Attributes[nameof (OpenInVsUri)] = value;
    }

    public override string EmailType() => "User Invited To Account";

    protected virtual string GetBody() => LicensingResources.UserInvitedToAccountBodyTemplate();

    protected internal static class Links
    {
      public static readonly string TestHub = Microsoft.VisualStudio.Services.EmailNotification.CommonUtil.GetForwardLink(330873);
      public static readonly string DownloadMTM = Microsoft.VisualStudio.Services.EmailNotification.CommonUtil.GetForwardLink(616953);
      public static readonly string Services = Microsoft.VisualStudio.Services.EmailNotification.CommonUtil.GetForwardLink(307109, "o~msft~profile~lifecyclemail_service_attach");
      public static readonly string VisualStudio = Microsoft.VisualStudio.Services.EmailNotification.CommonUtil.GetForwardLink(304133);
      public static readonly string Eclipse = Microsoft.VisualStudio.Services.EmailNotification.CommonUtil.GetForwardLink(623903);
      public static readonly string Support = Microsoft.VisualStudio.Services.EmailNotification.CommonUtil.GetForwardLink(253553);
      public static readonly string ConnectToVSO = "https://www.visualstudio.com/en-us/get-started/setup/connect-to-visual-studio-online";
    }
  }
}
