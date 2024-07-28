// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.CommitLog.CommitLogPointerItem
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using Microsoft.VisualStudio.Services.ItemStore.Common;


#nullable enable
namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared.CommitLog
{
  public class CommitLogPointerItem : StoredItem
  {
    private const string TargetKey = "Target";
    private const string SequenceNumberKey = "SequenceNumber";
    private const string VersionKey = "Version";
    private const string CommitLogItemType = "CommitLogPointer";

    public CommitLogPointerItem()
      : base("CommitLogPointer")
    {
      this.Version = 1;
    }

    public CommitLogPointerItem(IItemData data)
      : base(data, "CommitLogPointer")
    {
      this.Version = 1;
    }

    public PackagingCommitId Target
    {
      get => PackagingCommitId.Parse(this.Data[nameof (Target)]);
      set => this.Data[nameof (Target)] = value.ToString();
    }

    public long? SequenceNumber
    {
      get
      {
        string s = this.Data[nameof (SequenceNumber)];
        if (string.IsNullOrWhiteSpace(s))
          return new long?();
        long num = long.Parse(s);
        return num == 0L ? new long?() : new long?(num);
      }
      set => this.Data[nameof (SequenceNumber)] = value.ToString();
    }

    private int Version
    {
      get => int.Parse(this.Data[nameof (Version)]);
      set => this.Data[nameof (Version)] = value.ToString();
    }
  }
}
