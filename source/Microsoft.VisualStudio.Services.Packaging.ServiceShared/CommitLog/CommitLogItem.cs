// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.CommitLog.CommitLogItem
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using Microsoft.TeamFoundation.Common;
using Microsoft.VisualStudio.Services.ItemStore.Common;
using System;
using System.ComponentModel;

namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared.CommitLog
{
  public class CommitLogItem : StoredItem
  {
    private const string CommitTypeKey = "commitType";
    private const string CreatedDateKey = "createdDate";
    private const string ModifiedDateKey = "modifiedDate";
    private const string PreviousCommitIdKey = "previousCommitId";
    private const string NextCommitIdKey = "nextCommitId";
    private const string CommitIdKey = "commitId";
    private const string SequenceNumberKey = "sequenceNumber";
    private const string ProtocolOperationKey = "protocolOperation";
    private const string UserIdKey = "userId";
    private const string CorruptEntryKey = "corruptEntry";
    private const string CorruptReasonKey = "corruptReason";

    [Obsolete("Only for JSON deserialization, do not directly use")]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public CommitLogItem(IItemData data)
      : base(data)
    {
    }

    public CommitLogItem(IItemData data, string commitLogItemType)
      : base(data, commitLogItemType)
    {
    }

    public PackagingCommitId PreviousPackagingCommitId
    {
      get
      {
        string str = this.Data["previousCommitId"];
        return str != null ? PackagingCommitId.Parse(str) : PackagingCommitId.Empty;
      }
      set => this.Data["previousCommitId"] = value.ToString();
    }

    public PackagingCommitId PackagingCommitId
    {
      get => PackagingCommitId.Parse(this.Data["commitId"]);
      set => this.Data["commitId"] = value.ToString();
    }

    public PackagingCommitId NextPackagingCommitId
    {
      get
      {
        string str = this.Data["nextCommitId"];
        return str != null ? PackagingCommitId.Parse(str) : PackagingCommitId.Empty;
      }
      set => this.Data["nextCommitId"] = value.ToString();
    }

    public DateTime CreatedDate
    {
      get => DateTime.FromBinary(long.Parse(this.Data["createdDate"]));
      set
      {
        CommitLogUtils.CheckDateIsUtc(value, nameof (value));
        this.Data["createdDate"] = value.ToBinary().ToString();
      }
    }

    public DateTime ModifiedDate
    {
      get => DateTime.FromBinary(long.Parse(this.Data["modifiedDate"]));
      set
      {
        CommitLogUtils.CheckDateIsUtc(value, nameof (value));
        this.Data["modifiedDate"] = value.ToBinary().ToString();
      }
    }

    public long SequenceNumber
    {
      get
      {
        string s = this.Data["sequenceNumber"];
        return s != null ? long.Parse(s) : 0L;
      }
      set => this.Data["sequenceNumber"] = value.ToString();
    }

    public ProtocolOperation ProtocolOperation
    {
      get
      {
        string compressedForm = this.Data["protocolOperation"];
        return compressedForm != null ? ProtocolOperation.ParseFromCompressedForm(compressedForm) : (ProtocolOperation) null;
      }
      set => this.Data["protocolOperation"] = value.ToString();
    }

    public Guid UserId
    {
      get
      {
        string input = this.Data["userId"];
        return input != null ? Guid.Parse(input) : Guid.Empty;
      }
      set => this.Data["userId"] = value.ToString();
    }

    public bool CorruptEntry
    {
      get
      {
        string str = this.Data["corruptEntry"];
        return str != null && bool.Parse(str);
      }
      set => this.Data["corruptEntry"] = value.ToString();
    }

    public string CorruptReason
    {
      get => this.Data["corruptReason"];
      set
      {
        if (value.IsNullOrEmpty<char>())
          return;
        this.Data["corruptReason"] = value;
      }
    }
  }
}
