// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Licensing.UserInvitedToAccountV2Email
// Assembly: Microsoft.VisualStudio.Services.Licensing, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1B3CC7E6-129F-4267-B8E5-2210320176C7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Licensing.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Globalization;

namespace Microsoft.VisualStudio.Services.Licensing
{
  public class UserInvitedToAccountV2Email : UserInvitedToAccountV1Email
  {
    private const int StartProject = 627447;

    public UserInvitedToAccountV2Email(
      IVssRequestContext requestContext,
      License assignedLicense,
      bool featureFlagForLicenseInfoEnabled,
      Uri accountUri,
      CultureInfo culture = null)
      : base(requestContext, assignedLicense, featureFlagForLicenseInfoEnabled, accountUri, culture)
    {
      this.OpenInVsUri = this.WrapTrackingUrl(requestContext, Microsoft.VisualStudio.Services.EmailNotification.CommonUtil.GetOpenInVisualStudioUri(accountUri), "OpenInVsLink");
      this.StartOwnProject = this.WrapTrackingUrl(requestContext, this.AccountUrlTracked, "VstsActionLink");
      this.TokenFormatter.FormatBold("EmailContentJoinAtTitle", this.Tokenize("InvitingUserName"));
    }

    public override string EmailVariation => "V2";

    public string StartOwnProject
    {
      set => this.Attributes[nameof (StartOwnProject)] = value;
    }

    protected override string GetBody() => LicensingResources.UserInvitedToAccountV2_Template();

    protected internal static class InteractionIdsV2
    {
      public const string StartYourOwnProject = "VstsActionLink";
    }
  }
}
