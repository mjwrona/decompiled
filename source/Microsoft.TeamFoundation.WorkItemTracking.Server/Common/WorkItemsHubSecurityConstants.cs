// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.Common.WorkItemsHubSecurityConstants
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

using System;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server.Common
{
  public static class WorkItemsHubSecurityConstants
  {
    public static readonly Guid NamespaceId = new Guid("C0E7A722-1CAD-4AE6-B340-A8467501E7CE");
    public const string RootToken = "WorkItemsHub";
    public const string PersonalViewToken = "PersonalView";
    public const string QueryViewToken = "Query";
    public const string SendEmailViewToken = "SendEmail";
    public const string UserSettingsViewToken = "UserSettings";
    public const string NewWorkItemViewToken = "NewWorkItem";
    public const char PathSeparator = '/';
    public const int ViewPermission = 1;
  }
}
