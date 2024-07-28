// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Licensing.UserInvitedToAccountEmailExperiment
// Assembly: Microsoft.VisualStudio.Services.Licensing, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1B3CC7E6-129F-4267-B8E5-2210320176C7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Licensing.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.EmailNotification;
using System;
using System.Globalization;

namespace Microsoft.VisualStudio.Services.Licensing
{
  public class UserInvitedToAccountEmailExperiment : 
    EmailAbExperiment<UserInvitedToAccountNotificationEmailData>
  {
    protected License AssignedLicense { get; set; }

    protected bool FeatureFlagForLicenseInfoEnabled { get; set; }

    protected Uri AccountUri { get; set; }

    protected CultureInfo Culture { get; set; }

    public UserInvitedToAccountEmailExperiment(
      IVssRequestContext requestContext,
      License assignedLicense,
      bool featureFlagForLicenseInfoEnabled,
      Uri accountUri,
      CultureInfo culture = null,
      float throttle = 0.8f)
      : base(requestContext, throttle)
    {
      this.AssignedLicense = assignedLicense;
      this.FeatureFlagForLicenseInfoEnabled = featureFlagForLicenseInfoEnabled;
      this.AccountUri = accountUri;
      this.Culture = culture;
    }

    protected override UserInvitedToAccountNotificationEmailData CreateEmailForA() => (UserInvitedToAccountNotificationEmailData) new UserInvitedToAccountV1Email(this.RequestContext, this.AssignedLicense, this.FeatureFlagForLicenseInfoEnabled, this.AccountUri, this.Culture);

    protected override UserInvitedToAccountNotificationEmailData CreateEmailForB() => (UserInvitedToAccountNotificationEmailData) new UserInvitedToAccountV2Email(this.RequestContext, this.AssignedLicense, this.FeatureFlagForLicenseInfoEnabled, this.AccountUri, this.Culture);

    protected override UserInvitedToAccountNotificationEmailData CreateOriginalEmail() => new UserInvitedToAccountNotificationEmailData(this.RequestContext, this.AssignedLicense, this.FeatureFlagForLicenseInfoEnabled, this.AccountUri, this.Culture);
  }
}
