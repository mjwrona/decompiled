// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.NuGet.Server.CommitLog.NuGetCommitLogItem
// Assembly: Microsoft.VisualStudio.Services.NuGet.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1B6DD8F0-A807-4AA3-9A6E-1E5CDBF27B34
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.NuGet.Server.dll

using Microsoft.VisualStudio.Services.ItemStore.Common;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.CommitLog;

namespace Microsoft.VisualStudio.Services.NuGet.Server.CommitLog
{
  internal class NuGetCommitLogItem : CommitLogItemBackCompat
  {
    public const string NuGetCommitLogItemType = "NuGetCommitLog";
    private const string NuspecBytesKey = "nuspecBytes";
    private const string MetadataFormatVersionKey = "metadataFormatVersion";
    private const string PackageDisplayNameKey = "packageDisplayName";
    private const string PackageDisplayVersionKey = "packageDisplayVersion";

    public NuGetCommitLogItem()
      : base("NuGetCommitLog")
    {
      this.MetadataFormatVersion = 1;
    }

    public NuGetCommitLogItem(IItemData data)
      : base(data, "NuGetCommitLog")
    {
      this.MetadataFormatVersion = 1;
    }

    public string PackageDisplayName
    {
      get => this.Data["packageDisplayName"];
      set => this.Data["packageDisplayName"] = value;
    }

    public string PackageDisplayVersion
    {
      get => this.Data["packageDisplayVersion"];
      set => this.Data["packageDisplayVersion"] = value;
    }

    public byte[] NuspecBytes
    {
      get => System.Convert.FromBase64String(this.Data["nuspecBytes"]);
      set => this.Data["nuspecBytes"] = System.Convert.ToBase64String(value);
    }

    private int MetadataFormatVersion
    {
      get => int.Parse(this.Data["metadataFormatVersion"]);
      set => this.Data["metadataFormatVersion"] = value.ToString();
    }
  }
}
