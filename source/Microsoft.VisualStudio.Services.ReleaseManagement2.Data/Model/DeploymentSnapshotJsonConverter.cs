// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.DeploymentSnapshotJsonConverter
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Data, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A69B0FB7-3028-4162-BA38-794D80D7A49A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Data.dll

using Microsoft.VisualStudio.Services.WebApi;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Reflection;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model
{
  public class DeploymentSnapshotJsonConverter : VssSecureJsonConverter
  {
    public override bool CanWrite => false;

    public override bool CanConvert(Type objectType) => typeof (IDeploymentSnapshot).GetTypeInfo().IsAssignableFrom(objectType.GetTypeInfo());

    public override object ReadJson(
      JsonReader reader,
      Type objectType,
      object existingValue,
      JsonSerializer serializer)
    {
      if (reader == null)
        throw new ArgumentNullException(nameof (reader));
      if (serializer == null)
        throw new ArgumentNullException(nameof (serializer));
      IDeploymentSnapshot target1 = (IDeploymentSnapshot) null;
      if (reader.TokenType == JsonToken.StartObject)
      {
        JObject jobject = JObject.Load(reader);
        JToken jtoken;
        if (jobject.TryGetValue("SnapshotType", StringComparison.OrdinalIgnoreCase, out jtoken))
        {
          if (jtoken.Type != JTokenType.String)
            return (object) null;
          switch ((string) jtoken)
          {
            case "Designer":
              target1 = (IDeploymentSnapshot) new DesignerDeploymentSnapshot();
              break;
            case "Yaml":
              target1 = (IDeploymentSnapshot) new YamlDeploymentSnapshot();
              break;
          }
          if (target1 != null)
          {
            using (JsonReader reader1 = jobject.CreateReader())
              serializer.Populate(reader1, (object) target1);
          }
        }
      }
      else if (reader.TokenType == JsonToken.StartArray)
      {
        JArray jarray = JArray.Load(reader);
        DesignerDeploymentSnapshot deploymentSnapshot = new DesignerDeploymentSnapshot();
        foreach (JToken jtoken in jarray)
        {
          DeployPhaseSnapshot target2 = new DeployPhaseSnapshot();
          using (JsonReader reader2 = jtoken.CreateReader())
            serializer.Populate(reader2, (object) target2);
          deploymentSnapshot.DeployPhaseSnapshots.Add(target2);
        }
        target1 = (IDeploymentSnapshot) deploymentSnapshot;
      }
      return (object) target1;
    }

    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer) => throw new NotImplementedException();
  }
}
