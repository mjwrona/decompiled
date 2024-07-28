// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Documents.Snapshot
// Assembly: Microsoft.Azure.DocumentDB.Core, Version=2.10.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 20BB0EB7-1465-494C-8F4E-F898448B85D9
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.DocumentDB.Core.dll

using Newtonsoft.Json;
using System;

namespace Microsoft.Azure.Documents
{
  internal class Snapshot : Resource
  {
    private static DateTime UnixStartTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
    private SnapshotContent snapshotContent;

    [JsonProperty(PropertyName = "resource")]
    public string ResourceLink
    {
      get => this.GetValue<string>("resource");
      set => this.SetValue("resource", (object) value);
    }

    [JsonProperty(PropertyName = "state")]
    public SnapshotState State
    {
      get
      {
        string str = this.GetValue<string>("state");
        return string.IsNullOrEmpty(str) ? SnapshotState.Invalid : (SnapshotState) Enum.Parse(typeof (SnapshotState), str, true);
      }
      internal set => this.SetValue("state", (object) value.ToString());
    }

    [JsonProperty(PropertyName = "kind")]
    public SnapshotKind Kind
    {
      get
      {
        string str = this.GetValue<string>("kind");
        return string.IsNullOrEmpty(str) ? SnapshotKind.Invalid : (SnapshotKind) Enum.Parse(typeof (SnapshotKind), str, true);
      }
      internal set => this.SetValue("kind", (object) value.ToString());
    }

    [JsonProperty(PropertyName = "snapshotTimestamp")]
    [JsonConverter(typeof (UnixDateTimeConverter))]
    public DateTime SnapshotTimestamp
    {
      get => Snapshot.UnixStartTime.AddSeconds(this.GetValue<double>("snapshotTimestamp"));
      set => this.SetValue("snapshotTimestamp", (object) value.Subtract(Snapshot.UnixStartTime).TotalSeconds);
    }

    [JsonProperty(PropertyName = "ownerResourceId")]
    internal string OwnerResourceId
    {
      get => this.GetValue<string>("ownerResourceId");
      set => this.SetValue("ownerResourceId", (object) value);
    }

    [JsonProperty(PropertyName = "sizeInKB")]
    public ulong SizeInKB
    {
      get => this.GetValue<ulong>("sizeInKB");
      internal set => this.SetValue("sizeInKB", (object) value);
    }

    [JsonProperty(PropertyName = "compressedSizeInKB")]
    public ulong CompressedSizeInKB
    {
      get => this.GetValue<ulong>("compressedSizeInKB");
      internal set => this.SetValue("compressedSizeInKB", (object) value);
    }

    [JsonProperty(PropertyName = "content")]
    internal SnapshotContent Content
    {
      get
      {
        if (this.snapshotContent == null)
          this.snapshotContent = this.GetValue<SnapshotContent>("content");
        return this.snapshotContent;
      }
      set
      {
        this.snapshotContent = value;
        this.SetValue("content", (object) value);
      }
    }

    internal override void OnSave()
    {
      base.OnSave();
      if (this.snapshotContent == null)
        return;
      this.SetValue("content", (object) this.snapshotContent);
    }

    internal override void Validate()
    {
      base.Validate();
      this.GetValue<string>("resource");
      this.GetValue<string>("state");
      this.GetValue<string>("kind");
      this.GetValue<double>("snapshotTimestamp");
      this.GetValue<string>("ownerResourceId");
      long num1 = (long) this.GetValue<ulong>("sizeInKB");
      long num2 = (long) this.GetValue<ulong>("compressedSizeInKB");
      if (this.Content == null)
        return;
      this.Content.Validate();
    }
  }
}
