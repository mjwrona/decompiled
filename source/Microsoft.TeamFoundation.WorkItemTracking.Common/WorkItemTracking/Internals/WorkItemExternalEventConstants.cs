// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Internals.WorkItemExternalEventConstants
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E674B254-26A0-4D88-8FF3-86800090789C
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.WorkItemTracking.Common.dll

using System;
using System.ComponentModel;

namespace Microsoft.TeamFoundation.WorkItemTracking.Internals
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  public static class WorkItemExternalEventConstants
  {
    public const string WorkRestAreaName = "work";
    public const string WorkRestResourceEvents = "events";
    public const string WorkRestResourceEventsGuidString = "82d2847f-626e-4f73-a213-3d0ede1823bb";
    public static readonly Guid WorkRestResourceEventsGuid = new Guid("82d2847f-626e-4f73-a213-3d0ede1823bb");
  }
}
