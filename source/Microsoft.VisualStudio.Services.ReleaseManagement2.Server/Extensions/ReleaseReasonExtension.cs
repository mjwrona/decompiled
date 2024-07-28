// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Server.Extensions.ReleaseReasonExtension
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 134B1041-BFA6-49C6-8C6D-CA5ADF31AF54
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Server.dll

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Server.Extensions
{
  public static class ReleaseReasonExtension
  {
    public static bool IsAutoTriggeredRelease(this Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ReleaseReason releaseReason) => releaseReason == Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ReleaseReason.ContinuousIntegration || releaseReason == Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ReleaseReason.PullRequest;

    public static bool IsAutoTriggeredRelease(this Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Enumerations.ReleaseReason releaseReason) => releaseReason == Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Enumerations.ReleaseReason.ContinuousIntegration || releaseReason == Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Enumerations.ReleaseReason.PullRequest;
  }
}
