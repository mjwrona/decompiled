// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Common.Enums.MaintenanceJobStatusExtensions
// Assembly: Microsoft.VisualStudio.Services.Search.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8E09DCBA-148E-4EB7-BB73-B53B030BE93E
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.Common.dll

namespace Microsoft.VisualStudio.Services.Search.Common.Enums
{
  public static class MaintenanceJobStatusExtensions
  {
    public static bool IsCompletedOrPending(this MaintenanceJobStatus state) => state == MaintenanceJobStatus.Failed || state == MaintenanceJobStatus.Succeeded || state == MaintenanceJobStatus.Pending;

    public static bool IsInProgressOrPending(this MaintenanceJobStatus state) => state == MaintenanceJobStatus.InProgress || state == MaintenanceJobStatus.Pending;
  }
}
