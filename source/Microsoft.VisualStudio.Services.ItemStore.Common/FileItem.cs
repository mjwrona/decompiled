// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ItemStore.Common.FileItem
// Assembly: Microsoft.VisualStudio.Services.ItemStore.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 44753C0C-D541-4975-AF3F-2B606DE6FF70
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ItemStore.Common.dll

using Newtonsoft.Json;
using System;

namespace Microsoft.VisualStudio.Services.ItemStore.Common
{
  public class FileItem : BlobItem, IByteSizeItem
  {
    public const string LengthKey = "cr:length";
    public const string SizeKey = "cr:size";
    private const string DefaultItemType = "file";
    private const string SymbolicLinkKey = "symbolicLink";
    private const string PermissionValueKey = "permissionValue";

    public FileItem()
      : this("file")
    {
    }

    public FileItem(IItemData data)
      : this(data, "file")
    {
    }

    public FileItem(string itemType)
      : base(itemType)
    {
    }

    protected FileItem(IItemData data, string itemType)
      : base(data, itemType)
    {
    }

    public long Length
    {
      get => long.Parse(this.Data["cr:length"]);
      set => this.Data["cr:length"] = value.ToString();
    }

    public string SymbolicLink
    {
      get => this.Data["symbolicLink"];
      set => this.Data["symbolicLink"] = value.ToString();
    }

    [CLSCompliant(false)]
    public uint PermissionValue
    {
      get => uint.Parse(this.Data["permissionValue"]);
      set => this.Data["permissionValue"] = value.ToString();
    }

    [JsonIgnore]
    public long Size
    {
      get => long.Parse(this.Data["cr:length"]);
      set => this.Data["cr:length"] = value.ToString();
    }

    public bool HasLength => this.Data["cr:length"] != null;

    [JsonIgnore]
    public bool HasSize => this.Data["cr:size"] != null;

    public bool IsSymbolicLink => this.Data["symbolicLink"] != null;

    public bool HasPermissionValue => this.Data["permissionValue"] != null;
  }
}
