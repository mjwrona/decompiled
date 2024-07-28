// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.CommitLog.CommitLogRepairItem
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared.CommitLog
{
  public class CommitLogRepairItem
  {
    public CommitLogRepairItem(
      PackagingCommitId commitId,
      PackagingCommitId newPointer,
      RepairType repairType)
    {
      this.CommitId = commitId;
      this.NewPointer = newPointer;
      this.RepairType = repairType;
    }

    public PackagingCommitId CommitId { get; set; }

    public PackagingCommitId NewPointer { get; set; }

    public RepairType RepairType { get; set; }

    public override string ToString()
    {
      switch (this.RepairType)
      {
        case RepairType.ForwardPointer:
          return string.Format("Commit Log with ID {0} updated forward pointer with {1}.", (object) this.CommitId, (object) this.NewPointer);
        case RepairType.PreviousPointer:
          return string.Format("Commit Log with ID {0} updated previous pointer with {1}.", (object) this.CommitId, (object) this.NewPointer);
        case RepairType.Head:
          return string.Format("HEAD was updated to target {0}.", (object) this.NewPointer);
        case RepairType.Tail:
          return string.Format("TAIL was updated to target {0}.", (object) this.NewPointer);
        case RepairType.HeadDeletion:
          return "Head was deleted.";
        case RepairType.TailDeletion:
          return "Tail was deleted.";
        default:
          return "";
      }
    }
  }
}
