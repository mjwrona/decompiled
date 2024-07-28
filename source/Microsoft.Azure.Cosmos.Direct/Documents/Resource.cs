// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Documents.Resource
// Assembly: Microsoft.Azure.Cosmos.Direct, Version=3.29.4.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: FFE3C00D-4333-4294-8947-B1C93A852E2F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Direct.dll

using Newtonsoft.Json;
using System;
using System.IO;

namespace Microsoft.Azure.Documents
{
  internal abstract class Resource : JsonSerializable
  {
    internal static DateTime UnixStartTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);

    protected Resource()
    {
    }

    protected Resource(Resource resource)
    {
      this.Id = resource.Id;
      this.ResourceId = resource.ResourceId;
      this.SelfLink = resource.SelfLink;
      this.AltLink = resource.AltLink;
      this.Timestamp = resource.Timestamp;
      this.ETag = resource.ETag;
    }

    [JsonProperty(PropertyName = "id")]
    public virtual string Id
    {
      get => this.GetValue<string>("id");
      set => this.SetValue("id", (object) value);
    }

    [JsonProperty(PropertyName = "_rid")]
    public virtual string ResourceId
    {
      get => this.GetValue<string>("_rid");
      set => this.SetValue("_rid", (object) value);
    }

    [JsonProperty(PropertyName = "_self")]
    public string SelfLink
    {
      get => this.GetValue<string>("_self");
      internal set => this.SetValue("_self", (object) value);
    }

    [JsonIgnore]
    public string AltLink { get; set; }

    [JsonProperty(PropertyName = "_ts")]
    [JsonConverter(typeof (UnixDateTimeConverter))]
    public virtual DateTime Timestamp
    {
      get => Resource.UnixStartTime.AddSeconds(this.GetValue<double>("_ts"));
      internal set => this.SetValue("_ts", (object) (ulong) (value - Resource.UnixStartTime).TotalSeconds);
    }

    [JsonProperty(PropertyName = "_etag")]
    public string ETag
    {
      get => this.GetValue<string>("_etag");
      internal set => this.SetValue("_etag", (object) value);
    }

    public void SetPropertyValue(string propertyName, object propertyValue) => this.SetValue(propertyName, propertyValue);

    public T GetPropertyValue<T>(string propertyName) => this.GetValue<T>(propertyName);

    internal override void Validate()
    {
      base.Validate();
      this.GetValue<string>("id");
      this.GetValue<string>("_rid");
      this.GetValue<string>("_self");
      this.GetValue<double>("_ts");
      this.GetValue<string>("_etag");
    }

    public byte[] ToByteArray()
    {
      using (MemoryStream memoryStream = new MemoryStream())
      {
        this.SaveTo((Stream) memoryStream);
        return memoryStream.ToArray();
      }
    }
  }
}
