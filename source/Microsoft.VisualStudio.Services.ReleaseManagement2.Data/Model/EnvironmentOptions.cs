// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.EnvironmentOptions
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Data, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A69B0FB7-3028-4162-BA38-794D80D7A49A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Data.dll

using Microsoft.VisualStudio.Services.ReleaseManagement.WebApi;
using Newtonsoft.Json;
using System.ComponentModel;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model
{
  public class EnvironmentOptions
  {
    public string EmailNotificationType { get; set; }

    public string EmailRecipients { get; set; }

    [JsonProperty(NullValueHandling = NullValueHandling.Ignore, DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
    public DeploymentAuthorizationOwner DeploymentAuthorizationOwner { get; set; }

    [JsonProperty(NullValueHandling = NullValueHandling.Ignore, DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
    [DefaultValue(false)]
    public bool PublishDeploymentStatus { get; set; }

    [JsonProperty(NullValueHandling = NullValueHandling.Ignore, DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
    [DefaultValue(false)]
    public bool BadgeEnabled { get; set; }

    [JsonProperty(NullValueHandling = NullValueHandling.Ignore, DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
    [DefaultValue(false)]
    public bool AutoLinkWorkItems { get; set; }

    [JsonProperty(NullValueHandling = NullValueHandling.Ignore, DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
    [DefaultValue(false)]
    public bool PullRequestDeploymentEnabled { get; set; }

    public EnvironmentOptions()
    {
      this.EmailNotificationType = "OnlyOnFailure";
      this.EmailRecipients = Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.RunOptionsConstants.DefaultEmailRecipients;
      this.PublishDeploymentStatus = false;
      this.BadgeEnabled = false;
      this.AutoLinkWorkItems = false;
      this.PullRequestDeploymentEnabled = false;
    }

    public EnvironmentOptions DeepClone()
    {
      EnvironmentOptions environmentOptions = (EnvironmentOptions) this.MemberwiseClone();
      if (this.EmailNotificationType != null)
        environmentOptions.EmailNotificationType = string.Copy(this.EmailNotificationType);
      if (this.EmailRecipients != null)
        environmentOptions.EmailRecipients = string.Copy(this.EmailRecipients);
      return environmentOptions;
    }
  }
}
