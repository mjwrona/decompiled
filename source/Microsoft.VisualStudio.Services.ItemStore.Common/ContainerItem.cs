// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ItemStore.Common.ContainerItem
// Assembly: Microsoft.VisualStudio.Services.ItemStore.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 44753C0C-D541-4975-AF3F-2B606DE6FF70
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ItemStore.Common.dll

using Microsoft.VisualStudio.Services.Content.Common;
using System;

namespace Microsoft.VisualStudio.Services.ItemStore.Common
{
  public class ContainerItem : StoredItem, IExpiringItem
  {
    private const string RootLocationKey = "cr:rootLocation";
    private const string NameKey = "cr:name";
    private const string SealedKey = "cr:sealed";
    private const string AppendOnlyKey = "cr:appendOnly";
    internal const string ExpirationTimeKey = "cr:expirationTime";
    private const string DeletePendingKey = "cr:deletePending";
    private const string CreationTimeKey = "cr:creationTime";
    private const string UseIdReferencesKey = "cr:useIdReferences";
    private static readonly string DefaultItemType = "container";

    public ContainerItem()
      : this(ContainerItem.DefaultItemType)
    {
    }

    public ContainerItem(IItemData data)
      : this(data, ContainerItem.DefaultItemType)
    {
    }

    protected ContainerItem(string itemType)
      : base(itemType)
    {
    }

    protected ContainerItem(IItemData data, string itemType)
      : base(data, itemType)
    {
    }

    public string RootLocation
    {
      get => this.Data["cr:rootLocation"];
      set => this.Data["cr:rootLocation"] = value;
    }

    public Locator Name
    {
      get => this.Data["cr:name"] != null ? Locator.Parse(this.Data["cr:name"]) : (Locator) null;
      set => this.Data["cr:name"] = value.ToString();
    }

    public bool Sealed
    {
      get
      {
        bool? nullable = this.Data.GetBool("cr:sealed");
        return nullable.HasValue && nullable.Value;
      }
      set => this.Data["cr:sealed"] = value.ToString();
    }

    public bool IsAppendOnly
    {
      get
      {
        bool? nullable = this.Data.GetBool("cr:appendOnly");
        return nullable.HasValue && nullable.Value;
      }
      set => this.Data["cr:appendOnly"] = value.ToString();
    }

    public bool DeletePending
    {
      get => this.Data.TryGetWithNullCheck<bool>("cr:deletePending", false, ContainerItem.\u003C\u003EO.\u003C0\u003E__Parse ?? (ContainerItem.\u003C\u003EO.\u003C0\u003E__Parse = new Func<string, bool>(bool.Parse)));
      set => this.Data["cr:deletePending"] = value.ToString();
    }

    public bool? UseIdReferences
    {
      get => this.Data.TryGetWithNullCheck<bool?>("cr:useIdReferences", new bool?(), (Func<string, bool?>) (v => new bool?(bool.Parse(v))));
      set => this.Data["cr:useIdReferences"] = value.ToString();
    }

    public DateTime? CreationTime => this.Data.TryGetWithNullCheck<DateTime?>("cr:creationTime", new DateTime?(), (Func<string, DateTime?>) (v => new DateTime?(DateTime.Parse(v))));

    public void SetCreationTimeToNow()
    {
      IItemData data = this.Data;
      DateTime dateTime = DateTime.UtcNow;
      dateTime = dateTime.ToUniversalTime();
      string str = dateTime.ToString("o");
      data["cr:creationTime"] = str;
    }

    public DateTime? ExpirationTime
    {
      set => this.Data["cr:expirationTime"] = ExpiringItemHelper.GetExpirationTimeStringToSetTo(value);
    }

    public bool TryGetExpirationTime(out DateTime? expirationTime) => ExpiringItemHelper.TryGetExpirationTime(this.Data["cr:expirationTime"], out expirationTime);

    public bool IsRetainedForever() => ExpiringItemHelper.IsRetainedForever(this.Data["cr:expirationTime"]);
  }
}
