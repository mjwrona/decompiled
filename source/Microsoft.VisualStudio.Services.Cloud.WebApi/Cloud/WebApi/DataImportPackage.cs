// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Cloud.WebApi.DataImportPackage
// Assembly: Microsoft.VisualStudio.Services.Cloud.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 52A8E326-8E84-4175-AE92-8ED7AF376B63
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cloud.WebApi.dll

using Microsoft.VisualStudio.Services.WebApi;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.Cloud.WebApi
{
  [DataContract]
  public class DataImportPackage
  {
    public DataImportPackage()
    {
      this.Source = new DataImportPackage.DataImportSource();
      this.Target = new DataImportPackage.DataImportTarget();
      this.Properties = new PropertiesCollection();
      this.ValidationData = new PropertiesCollection();
      this.Identities = new List<string>();
    }

    [DataMember]
    [JsonProperty(Order = 1)]
    public DataImportPackage.DataImportSource Source { get; set; }

    [DataMember]
    [JsonProperty(Order = 2)]
    public DataImportPackage.DataImportTarget Target { get; set; }

    [DataMember]
    [JsonProperty(Order = 3)]
    public PropertiesCollection Properties { get; set; }

    [DataMember]
    [JsonProperty(Order = 4)]
    public PropertiesCollection ValidationData { get; set; }

    [DataMember]
    [JsonProperty(Order = 5)]
    public List<string> Identities { get; set; }

    public bool ShouldSerializeProperties() => this.Properties != null && this.Properties.Count > 0;

    public bool ShouldSerializeIdentities() => this.Identities != null && this.Identities.Any<string>();

    public DataImportPackage Clone()
    {
      DataImportPackage dataImportPackage = (DataImportPackage) this.MemberwiseClone();
      dataImportPackage.Source = this.Source.Clone();
      dataImportPackage.Target = this.Target.Clone();
      dataImportPackage.Properties = new PropertiesCollection((IDictionary<string, object>) this.Properties);
      dataImportPackage.ValidationData = new PropertiesCollection((IDictionary<string, object>) this.ValidationData);
      dataImportPackage.Identities = new List<string>((IEnumerable<string>) this.Identities);
      return dataImportPackage;
    }

    public static DataImportPackage Load(string path) => JsonConvert.DeserializeObject<DataImportPackage>(File.ReadAllText(path));

    public override string ToString() => JsonConvert.SerializeObject((object) this, Formatting.Indented, (JsonConverter[]) new DataImportPackage.PropertiesCollectionConverter[1]
    {
      new DataImportPackage.PropertiesCollectionConverter()
    });

    public void ToFile(string path) => File.WriteAllText(path, this.ToString());

    public class PropertiesCollectionConverter : JsonConverter
    {
      public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
      {
        Dictionary<string, object> dictionary = new Dictionary<string, object>((IDictionary<string, object>) (value as PropertiesCollection));
        serializer.Serialize(writer, (object) dictionary);
      }

      public override bool CanConvert(Type objectType) => objectType == typeof (PropertiesCollection);

      public override object ReadJson(
        JsonReader reader,
        Type objectType,
        object existingValue,
        JsonSerializer serializer)
      {
        return (object) serializer.Deserialize<PropertiesCollection>(reader);
      }
    }

    public class DataImportSource
    {
      public DataImportSource()
      {
        this.Files = new PropertiesCollection();
        this.Properties = new PropertiesCollection();
      }

      [DataMember]
      [JsonProperty(Order = 1)]
      public string Location { get; set; }

      [DataMember]
      [JsonProperty(Order = 2)]
      public PropertiesCollection Files { get; set; }

      [DataMember]
      [JsonProperty(Order = 3)]
      public PropertiesCollection Properties { get; set; }

      public bool ShouldSerializeProperties() => this.Properties != null && this.Properties.Count > 0;

      public DataImportPackage.DataImportSource Clone()
      {
        DataImportPackage.DataImportSource dataImportSource = (DataImportPackage.DataImportSource) this.MemberwiseClone();
        dataImportSource.Files = new PropertiesCollection((IDictionary<string, object>) this.Files);
        dataImportSource.Properties = new PropertiesCollection((IDictionary<string, object>) this.Properties);
        return dataImportSource;
      }
    }

    public class DataImportTarget
    {
      public DataImportTarget() => this.Properties = new PropertiesCollection();

      [DataMember]
      [JsonProperty(Order = 1)]
      public string Name { get; set; }

      [DataMember]
      [JsonProperty(Order = 2)]
      public PropertiesCollection Properties { get; set; }

      public bool ShouldSerializeProperties() => this.Properties != null && this.Properties.Count > 0;

      public DataImportPackage.DataImportTarget Clone()
      {
        DataImportPackage.DataImportTarget dataImportTarget = (DataImportPackage.DataImportTarget) this.MemberwiseClone();
        dataImportTarget.Properties = new PropertiesCollection((IDictionary<string, object>) this.Properties);
        return dataImportTarget;
      }
    }
  }
}
