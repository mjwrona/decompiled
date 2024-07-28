// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype.DeletedPackageJobConstants
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;

namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype
{
  public static class DeletedPackageJobConstants
  {
    public static readonly JobType DeletedPackageJobType = new JobType(Guid.Parse("E5E75E39-98F9-4A5D-A073-F551A4FAC9B7"), "DeletePackageJob");
    public static readonly RegistryQuery MaxPermDeleteBatchSizeRegistryPath = (RegistryQuery) "/Configuration/Packaging/MaxPermanentDeletePackagesBatchSize";
    public const int MaxPermDeleteBatchSizeDefault = 100;
    public static readonly RegistryQuery MaxRunTimeRegistryPath = (RegistryQuery) "/Configuration/Packaging/DPJMaxRunTimeSeconds";
    public static TimeSpan MaxRunTimeDefault = TimeSpan.FromHours(1.0);
    public static readonly RegistryQuery FlushIntervalRegistryPath = (RegistryQuery) "/Configuration/Packaging/DPJFlushIntervalSeconds";
    public static TimeSpan FlushIntervalDefault = TimeSpan.FromMinutes(5.0);
    public static readonly RegistryQuery MaxPendingPermDeleteOpsRegistryPath = (RegistryQuery) "/Configuration/Packaging/DPJMaxPendingCommits";
    public const int MaxPendingPermDeleteOpsDefault = 98;
  }
}
