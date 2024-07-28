// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.EnvironmentOptions
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: AE7F604E-30D7-44A7-BE7B-AB7FB5A67B31
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.dll

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.WebApi
{
  [DataContract]
  public class EnvironmentOptions : ReleaseManagementSecuredObject
  {
    [Obsolete("Use Notifications instead.")]
    [DataMember]
    public string EmailNotificationType { get; set; }

    [Obsolete("Use Notifications instead.")]
    [DataMember]
    public string EmailRecipients { get; set; }

    [Obsolete("Use DeploymentInput.SkipArtifactsDownload instead.")]
    [DataMember]
    public bool SkipArtifactsDownload { get; set; }

    [Obsolete("Use DeploymentInput.TimeoutInMinutes instead.")]
    [DataMember]
    public int TimeoutInMinutes { get; set; }

    [Obsolete("Use DeploymentInput.EnableAccessToken instead.")]
    [DataMember]
    public bool EnableAccessToken { get; set; }

    [DataMember]
    public bool PublishDeploymentStatus { get; set; }

    [DataMember]
    public bool BadgeEnabled { get; set; }

    [DataMember]
    public bool AutoLinkWorkItems { get; set; }

    [DataMember]
    public bool PullRequestDeploymentEnabled { get; set; }

    public EnvironmentOptions()
    {
      this.EmailNotificationType = "OnlyOnFailure";
      this.EmailRecipients = RunOptionsConstants.DefaultEmailRecipients;
      this.PublishDeploymentStatus = false;
      this.BadgeEnabled = false;
      this.AutoLinkWorkItems = false;
      this.PullRequestDeploymentEnabled = false;
    }

    public bool IsEqual(EnvironmentOptions environmentOptions) => environmentOptions != null && this.AreEmailNotificationPropertiesSame(environmentOptions) && environmentOptions.PublishDeploymentStatus == this.PublishDeploymentStatus && environmentOptions.BadgeEnabled == this.BadgeEnabled && environmentOptions.AutoLinkWorkItems == this.AutoLinkWorkItems && environmentOptions.PullRequestDeploymentEnabled == this.PullRequestDeploymentEnabled;

    public bool AreEmailNotificationPropertiesSame(EnvironmentOptions environmentOptions)
    {
      if (environmentOptions == null || !string.Equals(this.EmailNotificationType, environmentOptions.EmailNotificationType, StringComparison.OrdinalIgnoreCase))
        return false;
      return string.Equals(environmentOptions.EmailNotificationType, "Never", StringComparison.OrdinalIgnoreCase) || this.AreEmailRecipientsSame(environmentOptions.EmailRecipients);
    }

    private bool AreEmailRecipientsSame(string emailRecipients)
    {
      if (string.IsNullOrWhiteSpace(emailRecipients))
        return false;
      return ((IEnumerable<string>) emailRecipients.Split(';')).OrderBy<string, string>((Func<string, string>) (t => t)).SequenceEqual<string>((IEnumerable<string>) ((IEnumerable<string>) this.EmailRecipients.Split(';')).OrderBy<string, string>((Func<string, string>) (t => t)), (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
    }
  }
}
