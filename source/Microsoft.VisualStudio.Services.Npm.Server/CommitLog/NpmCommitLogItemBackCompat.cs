// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Npm.Server.CommitLog.NpmCommitLogItemBackCompat
// Assembly: Microsoft.VisualStudio.Services.Npm.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 2F4F0262-1C1B-42F0-BCA7-1385424A0D51
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Npm.Server.dll

using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.ItemStore.Common;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.CommitLog;

namespace Microsoft.VisualStudio.Services.Npm.Server.CommitLog
{
  internal class NpmCommitLogItemBackCompat : CommitLogItemBackCompat
  {
    public const string NpmCommitLogItemType = "NpmCommitLog";
    private const string PackageJsonBytesKey = "packageJsonBytes";
    private const string PackageSha1SumKey = "packageSha1Sum";
    private const string MetadataFormatVersionKey = "metadataFormatVersion";

    public NpmCommitLogItemBackCompat()
      : base("NpmCommitLog")
    {
      this.MetadataFormatVersion = 1;
    }

    public NpmCommitLogItemBackCompat(IItemData data)
      : base(data, "NpmCommitLog")
    {
      this.MetadataFormatVersion = 1;
    }

    public byte[] PackageJsonBytes
    {
      get => System.Convert.FromBase64String(this.Data["packageJsonBytes"]);
      set
      {
        ArgumentUtility.CheckForNull<byte[]>(value, nameof (PackageJsonBytes));
        this.Data["packageJsonBytes"] = System.Convert.ToBase64String(value);
      }
    }

    public string PackageSha1Sum
    {
      get => this.Data["packageSha1Sum"];
      set
      {
        ArgumentUtility.CheckForNull<string>(value, nameof (PackageSha1Sum));
        this.Data["packageSha1Sum"] = value;
      }
    }

    private int MetadataFormatVersion
    {
      get => int.Parse(this.Data["metadataFormatVersion"]);
      set => this.Data["metadataFormatVersion"] = value.ToString();
    }
  }
}
