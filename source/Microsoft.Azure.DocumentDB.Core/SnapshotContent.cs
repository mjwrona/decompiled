// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Documents.SnapshotContent
// Assembly: Microsoft.Azure.DocumentDB.Core, Version=2.10.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 20BB0EB7-1465-494C-8F4E-F898448B85D9
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.DocumentDB.Core.dll

using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Microsoft.Azure.Documents
{
  internal sealed class SnapshotContent : JsonSerializable
  {
    private IList<string> partitionKeyRangeResourceIds;
    private IList<string> dataDirectories;
    private IList<string> partitionKeyRanges;
    private IList<PartitionKeyRange> partitionKeyRangeList;
    private DocumentCollection collectionResource;

    public IList<PartitionKeyRange> PartitionKeyRangeList
    {
      get
      {
        if (this.partitionKeyRangeList == null && this.SerializedPartitionKeyRanges != null)
        {
          this.partitionKeyRangeList = (IList<PartitionKeyRange>) new List<PartitionKeyRange>();
          foreach (string partitionKeyRange in (IEnumerable<string>) this.SerializedPartitionKeyRanges)
            this.partitionKeyRangeList.Add(this.GetResourceIfDeserialized<PartitionKeyRange>(partitionKeyRange));
        }
        return this.partitionKeyRangeList;
      }
    }

    public DocumentCollection DocumentCollection
    {
      get
      {
        if (this.collectionResource == null && this.SerializedCollection != null)
          this.collectionResource = this.GetResourceIfDeserialized<DocumentCollection>(this.SerializedCollection);
        return this.collectionResource;
      }
    }

    [JsonProperty(PropertyName = "partitionKeyRangeResourceIds")]
    public IList<string> PartitionKeyRangeResourceIds
    {
      get
      {
        if (this.partitionKeyRangeResourceIds == null)
          this.partitionKeyRangeResourceIds = this.GetValue<IList<string>>("partitionKeyRangeResourceIds");
        return this.partitionKeyRangeResourceIds;
      }
      internal set => this.partitionKeyRangeResourceIds = value;
    }

    [JsonProperty(PropertyName = "dataDirectories")]
    public IList<string> DataDirectories
    {
      get
      {
        if (this.dataDirectories == null)
          this.dataDirectories = this.GetValue<IList<string>>("dataDirectories");
        return this.dataDirectories;
      }
      internal set => this.dataDirectories = value;
    }

    [JsonProperty(PropertyName = "metadataDirectory")]
    public string MetadataDirectory
    {
      get => this.GetValue<string>("metadataDirectory");
      internal set => this.SetValue("metadataDirectory", (object) value);
    }

    internal override void OnSave()
    {
      base.OnSave();
      if (this.partitionKeyRangeResourceIds != null)
        this.SetValue("partitionKeyRangeResourceIds", (object) this.partitionKeyRangeResourceIds);
      if (this.dataDirectories == null)
        return;
      this.SetValue("dataDirectories", (object) this.dataDirectories);
    }

    [JsonProperty(PropertyName = "partitionKeyRanges")]
    private IList<string> SerializedPartitionKeyRanges
    {
      get
      {
        if (this.partitionKeyRanges == null)
          this.partitionKeyRanges = this.GetValue<IList<string>>("partitionKeyRanges");
        return this.partitionKeyRanges;
      }
      set => this.partitionKeyRanges = value;
    }

    [JsonProperty(PropertyName = "collectionContent")]
    private string SerializedCollection
    {
      get => this.GetValue<string>("collectionContent");
      set => this.SetValue("collectionContent", (object) value);
    }

    private T GetResourceIfDeserialized<T>(string body) where T : Resource, new()
    {
      try
      {
        using (MemoryStream memoryStream = new MemoryStream(Encoding.UTF8.GetBytes(body)))
        {
          ITypeResolver<T> typeResolver = JsonSerializable.GetTypeResolver<T>();
          return JsonSerializable.LoadFrom<T>((Stream) memoryStream, typeResolver);
        }
      }
      catch (JsonException ex)
      {
        return default (T);
      }
    }

    internal override void Validate()
    {
      base.Validate();
      this.GetValue<IList<string>>("partitionKeyRangeResourceIds");
      this.GetValue<IList<string>>("dataDirectories");
    }
  }
}
