// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.BlobStore.Server.Common.DeleteResult
// Assembly: Microsoft.VisualStudio.Services.BlobStore.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: CB48D0BF-32A2-483C-A1D4-2F10DEBB3D56
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.BlobStore.Server.Common.dll

namespace Microsoft.VisualStudio.Services.BlobStore.Server.Common
{
  public struct DeleteResult
  {
    public readonly bool Completed;
    public readonly long TotalScanned;
    public readonly long TotalRequiringDelete;
    public readonly long TotalSuccessfullyDeleted;
    public readonly ulong TotalPhysicalBytesDeleted;

    public DeleteResult(
      bool completed,
      long totalScanned,
      long totalRequiringDelete,
      long totalSuccessfullyDeleted,
      ulong totalPhysicalBytesDeleted)
    {
      this.Completed = completed;
      this.TotalScanned = totalScanned;
      this.TotalRequiringDelete = totalRequiringDelete;
      this.TotalSuccessfullyDeleted = totalSuccessfullyDeleted;
      this.TotalPhysicalBytesDeleted = totalPhysicalBytesDeleted;
    }
  }
}
