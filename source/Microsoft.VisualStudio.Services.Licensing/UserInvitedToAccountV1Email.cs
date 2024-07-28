// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Licensing.UserInvitedToAccountV1Email
// Assembly: Microsoft.VisualStudio.Services.Licensing, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1B3CC7E6-129F-4267-B8E5-2210320176C7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Licensing.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Globalization;

namespace Microsoft.VisualStudio.Services.Licensing
{
  public class UserInvitedToAccountV1Email : UserInvitedToAccountNotificationEmailData
  {
    private const string AccountUrlTrackedToken = "AccountUrlTracked";

    public UserInvitedToAccountV1Email(
      IVssRequestContext requestContext,
      License assignedLicense,
      bool featureFlagForLicenseInfoEnabled,
      Uri accountUri,
      CultureInfo culture = null)
      : base(requestContext, assignedLicense, featureFlagForLicenseInfoEnabled, accountUri, culture)
    {
      this.AccountUrlTracked = this.WrapTrackingUrl(requestContext, accountUri.ToString(), "VstsJoinLink");
      this.OpenInVsUri = this.WrapTrackingUrl(requestContext, UserInvitedToAccountNotificationEmailData.Links.ConnectToVSO, "OpenInVsLink");
      this.TokenFormatter.Format("EmailContentJoinPersonAtAccountTracked", this.Tokenize("InvitingUserName"), this.TokenFormatter.WrapWithAnchor(this.Tokenize("AccountUrl"), this.AccountUrlTracked));
      this.TokenFormatter.FormatAnchor("EmailContentServicesYoullLoveLikeTracked", this.WrapTrackingUrl(requestContext, UserInvitedToAccountNotificationEmailData.Links.Services, "VstsLearnMoreLink"));
      this.TokenFormatter.FormatAnchor("EmailContentUseVisualStudioEclipseTracked", new string[2]
      {
        this.WrapTrackingUrl(requestContext, UserInvitedToAccountNotificationEmailData.Links.VisualStudio, "VsActionLink"),
        this.WrapTrackingUrl(requestContext, UserInvitedToAccountNotificationEmailData.Links.Eclipse, "EclipseActionLink")
      });
      this.TokenFormatter.FormatAnchor("EmailContentNeedHelpTracked", this.WrapTrackingUrl(requestContext, UserInvitedToAccountNotificationEmailData.Links.Support, "SupportLink"));
      this.EmailOpenedEventTracking = this.GetEmailOpenedEventTrackingHtml(requestContext);
    }

    public override string EmailVariation => "V1";

    public virtual string EmailOpenedEventTracking
    {
      set => this.Attributes[nameof (EmailOpenedEventTracking)] = value;
    }

    public string AccountUrlTracked
    {
      set => this.Attributes[nameof (AccountUrlTracked)] = value;
      get => this.Attributes[nameof (AccountUrlTracked)];
    }

    protected override string GetBody() => LicensingResources.UserInvitedToAccountV1_Template();

    protected internal static class InteractionIdsV1
    {
      public const string AccountUrl = "VstsJoinLink";
      public const string OpenInVsUri = "OpenInVsLink";
      public const string SupportLink = "SupportLink";
      public const string DeveloperServices = "VstsLearnMoreLink";
      public const string VisualStudio = "VsActionLink";
      public const string Eclipse = "EclipseActionLink";
      public const string EmailOpenedEventTracking = "EmailOpenedEventTracking";
    }
  }
}
