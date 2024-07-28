// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.DeleteProcessingJob.PackageDeletionState
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using System;

namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared.DeleteProcessingJob
{
  public class PackageDeletionState
  {
    public PackageDeletionState(
      DateTime? deletedDate,
      DateTime? scheduledPermanentDeleteDate,
      DateTime? permanentDeletedDate)
    {
      this.DeletedDate = deletedDate;
      this.PermanentDeletedDate = permanentDeletedDate;
      this.ScheduledPermanentDeleteDate = scheduledPermanentDeleteDate;
    }

    public DateTime? DeletedDate { get; }

    public DateTime? ScheduledPermanentDeleteDate { get; }

    public DateTime? PermanentDeletedDate { get; }
  }
}
