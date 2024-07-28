// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Data.Converters.EnvironmentOptionsConverter
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Data, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A69B0FB7-3028-4162-BA38-794D80D7A49A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Data.dll

using System;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Data.Converters
{
  public static class EnvironmentOptionsConverter
  {
    public static Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.EnvironmentOptions ToServerEnvironmentOptions(
      Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.EnvironmentOptions environmentOptions)
    {
      if (environmentOptions == null)
        throw new ArgumentNullException(nameof (environmentOptions));
      return new Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.EnvironmentOptions()
      {
        EmailNotificationType = environmentOptions.EmailNotificationType,
        EmailRecipients = environmentOptions.EmailRecipients,
        PublishDeploymentStatus = environmentOptions.PublishDeploymentStatus,
        BadgeEnabled = environmentOptions.BadgeEnabled,
        AutoLinkWorkItems = environmentOptions.AutoLinkWorkItems,
        PullRequestDeploymentEnabled = environmentOptions.PullRequestDeploymentEnabled
      };
    }

    public static Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.EnvironmentOptions ToWebApiEnvironmentOptions(
      Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.EnvironmentOptions environmentOptions)
    {
      if (environmentOptions == null)
        throw new ArgumentNullException(nameof (environmentOptions));
      return new Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.EnvironmentOptions()
      {
        EmailNotificationType = environmentOptions.EmailNotificationType,
        EmailRecipients = environmentOptions.EmailRecipients,
        PublishDeploymentStatus = environmentOptions.PublishDeploymentStatus,
        BadgeEnabled = environmentOptions.BadgeEnabled,
        AutoLinkWorkItems = environmentOptions.AutoLinkWorkItems,
        PullRequestDeploymentEnabled = environmentOptions.PullRequestDeploymentEnabled
      };
    }
  }
}
