// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Events.EventConstants
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: AE7F604E-30D7-44A7-BE7B-AB7FB5A67B31
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.dll

namespace Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Events
{
  public static class EventConstants
  {
    public const string WorkItemsDataKey = "workItems";
    public const string WorkItemsMessageKey = "moreWorkItemsMessage";
    public const string TestResultsKey = "testResults";
    public const string CommitsDataKey = "commits";
    public const string ReleasePropertiesDataKey = "releaseProperties";
    public const string EnvironmentStatusesDataKey = "environmentStatuses";
    public const string ReleaseNotCreatedEventType = "ms.vss-release.release-not-created-event";
  }
}
