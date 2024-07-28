// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Common.ReindexingStatusEntryExtension
// Assembly: Microsoft.VisualStudio.Services.Search.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8E09DCBA-148E-4EB7-BB73-B53B030BE93E
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.Common.dll

using Microsoft.VisualStudio.Services.Search.Common.Enums;
using System;

namespace Microsoft.VisualStudio.Services.Search.Common
{
  public static class ReindexingStatusEntryExtension
  {
    public static bool IsReindexingFailedOrInProgress(
      this ReindexingStatusEntry reindexingStatusEntry)
    {
      if (reindexingStatusEntry == null)
        throw new ArgumentNullException(nameof (reindexingStatusEntry));
      return reindexingStatusEntry.Status == ReindexingStatus.InProgress || reindexingStatusEntry.Status == ReindexingStatus.Failed || reindexingStatusEntry.Status == ReindexingStatus.Queued;
    }

    public static bool IsReindexingInProgress(this ReindexingStatusEntry reindexingStatusEntry)
    {
      if (reindexingStatusEntry == null)
        throw new ArgumentNullException(nameof (reindexingStatusEntry));
      return reindexingStatusEntry.Status == ReindexingStatus.InProgress || reindexingStatusEntry.Status == ReindexingStatus.Queued;
    }
  }
}
