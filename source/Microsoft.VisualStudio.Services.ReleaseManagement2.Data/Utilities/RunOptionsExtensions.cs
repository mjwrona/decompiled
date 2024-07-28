// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Data.Utilities.RunOptionsExtensions
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Data, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A69B0FB7-3028-4162-BA38-794D80D7A49A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Data.dll

using Microsoft.VisualStudio.Services.ReleaseManagement.WebApi;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Data.Utilities
{
  public static class RunOptionsExtensions
  {
    private const int DefaultTimeoutInMinutes = 0;

    public static EnvironmentOptions ToEnvironmentOptions(
      this IDictionary<string, string> webApiRunOptions)
    {
      if (webApiRunOptions == null)
        return (EnvironmentOptions) null;
      EnvironmentOptions environmentOptions = new EnvironmentOptions();
      if (webApiRunOptions.ContainsKey("EnvironmentOwnerEmailNotificationType"))
        environmentOptions.EmailNotificationType = webApiRunOptions["EnvironmentOwnerEmailNotificationType"];
      if (webApiRunOptions.ContainsKey("skipArtifactsDownload"))
      {
        bool result;
        environmentOptions.SkipArtifactsDownload = bool.TryParse(webApiRunOptions["skipArtifactsDownload"], out result) & result;
      }
      if (webApiRunOptions.ContainsKey("TimeoutInMinutes"))
      {
        int result;
        environmentOptions.TimeoutInMinutes = int.TryParse(webApiRunOptions["TimeoutInMinutes"], out result) ? result : 0;
      }
      return environmentOptions;
    }
  }
}
