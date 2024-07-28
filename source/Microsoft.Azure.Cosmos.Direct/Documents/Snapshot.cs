// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Documents.Snapshot
// Assembly: Microsoft.Azure.Cosmos.Direct, Version=3.29.4.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: FFE3C00D-4333-4294-8947-B1C93A852E2F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Direct.dll

using Newtonsoft.Json;
using System;
using System.Globalization;

namespace Microsoft.Azure.Documents
{
  internal class Snapshot : Resource
  {
    private new static DateTime UnixStartTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
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
      set => this.SetValue("snapshotTimestamp", (object) (ulong) (value - Snapshot.UnixStartTime).TotalSeconds);
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

    [JsonProperty(PropertyName = "lsn")]
    internal long LSN
    {
      get => this.GetValue<long>("lsn");
      set => this.SetValue("lsn", (object) value);
    }

    [JsonProperty(PropertyName = "content")]
    internal SnapshotContent Content
    {
      get
      {
        if (this.snapshotContent == null)
          this.snapshotContent = this.GetObject<SnapshotContent>("content");
        return this.snapshotContent;
      }
      set
      {
        this.snapshotContent = value;
        this.SetObject<SnapshotContent>("content", value);
      }
    }

    [JsonProperty(PropertyName = "parentResourceId")]
    internal string ParentResourceId
    {
      get => this.GetValue<string>("parentResourceId");
      set => this.SetValue("parentResourceId", (object) value);
    }

    internal override void OnSave()
    {
      base.OnSave();
      if (this.snapshotContent == null)
        return;
      this.snapshotContent.OnSave();
      this.SetObject<SnapshotContent>("content", this.snapshotContent);
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
      this.GetValue<long>("lsn");
      long num2 = (long) this.GetValue<ulong>("compressedSizeInKB");
      this.GetValue<string>("parentResourceId");
      if (this.Content == null)
        return;
      this.Content.Validate();
    }

    internal static Snapshot CloneSystemSnapshot(
      Snapshot existingSnapshot,
      OperationType operationType,
      bool inheritSnapshotTimestamp)
    {
      if (existingSnapshot.Kind != SnapshotKind.System)
        throw new ArgumentException(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Invalid snapshot kind {0}", (object) existingSnapshot.Kind));
      return new Snapshot()
      {
        Kind = existingSnapshot.Kind,
        OwnerResourceId = existingSnapshot.OwnerResourceId,
        ResourceLink = existingSnapshot.ResourceLink,
        Content = new SnapshotContent()
        {
          OperationType = operationType,
          SerializedDatabase = existingSnapshot.Content.SerializedDatabase,
          SerializedCollection = existingSnapshot.Content.SerializedCollection,
          SerializedOffer = existingSnapshot.Content.SerializedOffer,
          SerializedPartitionKeyRanges = existingSnapshot.Content.SerializedPartitionKeyRanges,
          GeoLinkIdToPKRangeRid = existingSnapshot.Content.GeoLinkIdToPKRangeRid
        },
        SnapshotTimestamp = !inheritSnapshotTimestamp ? Snapshot.UnixStartTime : existingSnapshot.SnapshotTimestamp,
        State = SnapshotState.Completed
      };
    }
  }
}
