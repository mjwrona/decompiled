// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.Common.WorkItemTrackingNamespaceSecurityConstants
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

using System;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server.Common
{
  public static class WorkItemTrackingNamespaceSecurityConstants
  {
    public static readonly Guid NamespaceId = new Guid("73E71C45-D483-40D5-BDBA-62FD076F7F87");
    public const string RootToken = "WorkItemTracking";
    public const string WITFormUserLayoutSettings = "WorkItemFormUserLayoutSettings";
    public const char PathSeparator = '/';
    public static readonly string WorkItemTrackingToken = "/WorkItemTracking";
    public const int ReadPermission = 1;
    public const int CrossProjectReadPermission = 2;
    public const int TrackWorkItemActivity = 4;
    public const int ReadRules = 8;
    public const int ReadHistoricalWorkItemResources = 16;
  }
}
