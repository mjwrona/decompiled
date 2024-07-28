// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Content.Common.JsonWrites
// Assembly: Microsoft.VisualStudio.Services.Content.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: DC45E7D4-4445-41B3-8FA2-C13CD848D0F1
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Content.Common.dll

using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace Microsoft.VisualStudio.Services.Content.Common
{
  public struct JsonWrites
  {
    public readonly IConcurrentIterator<JsonWrite> Writes;

    internal JsonWrites(IConcurrentIterator<JsonWrite> writes) => this.Writes = writes;

    private JsonWrites(params IConcurrentIterator<JsonWrite>[] writes) => this.Writes = ((IEnumerable<IConcurrentIterator<JsonWrite>>) writes).CollectOrdered<JsonWrite>(CancellationToken.None);

    public static JsonWrites Object(params JsonWrites[] properties) => new JsonWrites(new IConcurrentIterator<JsonWrite>[3]
    {
      (IConcurrentIterator<JsonWrite>) new ConcurrentIterator<JsonWrite>((IEnumerable<JsonWrite>) new JsonWrite[1]
      {
        (JsonWrite) (writer => writer.WriteStartObject())
      }),
      ((IEnumerable<JsonWrites>) properties).Select<JsonWrites, IConcurrentIterator<JsonWrite>>((Func<JsonWrites, IConcurrentIterator<JsonWrite>>) (property => property.Writes)).CollectOrdered<JsonWrite>(CancellationToken.None),
      (IConcurrentIterator<JsonWrite>) new ConcurrentIterator<JsonWrite>((IEnumerable<JsonWrite>) new JsonWrite[1]
      {
        (JsonWrite) (writer => writer.WriteEndObject())
      })
    });

    public static JsonWrites Properties(
      IConcurrentIterator<IEnumerable<KeyValuePair<string, object>>> properties)
    {
      return new JsonWrites(properties.Select<IEnumerable<KeyValuePair<string, object>>, JsonWrite>((Func<IEnumerable<KeyValuePair<string, object>>, JsonWrite>) (propertyKvps => JsonWrites.WriteProperties(propertyKvps))));
    }

    public static JsonWrites Property(string name, object value) => new JsonWrites((IConcurrentIterator<JsonWrite>) new ConcurrentIterator<JsonWrite>((IEnumerable<JsonWrite>) new JsonWrite[1]
    {
      JsonWrites.WriteProperty(name, value)
    }));

    private static JsonWrite WriteProperty(string name, object value) => (JsonWrite) (writer =>
    {
      writer.WritePropertyName(name);
      writer.WriteValue(value);
    });

    private static JsonWrite WriteProperties(IEnumerable<KeyValuePair<string, object>> kvps) => (JsonWrite) (writer =>
    {
      foreach (KeyValuePair<string, object> kvp in kvps)
      {
        writer.WritePropertyName(kvp.Key);
        writer.WriteValue(kvp.Value);
      }
    });

    public static JsonWrites Property(string name, JsonWrites value) => new JsonWrites(new IConcurrentIterator<JsonWrite>[2]
    {
      (IConcurrentIterator<JsonWrite>) new ConcurrentIterator<JsonWrite>((IEnumerable<JsonWrite>) new JsonWrite[1]
      {
        (JsonWrite) (writer => writer.WritePropertyName(name))
      }),
      value.Writes
    });

    public static JsonWrites NoopAction(Action action) => new JsonWrites((IConcurrentIterator<JsonWrite>) new ConcurrentIterator<JsonWrite>((IEnumerable<JsonWrite>) new JsonWrite[1]
    {
      (JsonWrite) (_ => action())
    }));

    public static JsonWrites Array(
      IConcurrentIterator<JObject> objectConcurrentIterator)
    {
      return new JsonWrites(new IConcurrentIterator<JsonWrite>[3]
      {
        (IConcurrentIterator<JsonWrite>) new ConcurrentIterator<JsonWrite>((IEnumerable<JsonWrite>) new JsonWrite[1]
        {
          (JsonWrite) (writer => writer.WriteStartArray())
        }),
        objectConcurrentIterator.Select<JObject, JsonWrite>((Func<JObject, JsonWrite>) (obj => (JsonWrite) (writer => obj.WriteTo(writer)))),
        (IConcurrentIterator<JsonWrite>) new ConcurrentIterator<JsonWrite>((IEnumerable<JsonWrite>) new JsonWrite[1]
        {
          (JsonWrite) (writer => writer.WriteEndArray())
        })
      });
    }

    public static JsonWrites Array(
      IConcurrentIterator<IEnumerable<JObject>> objectPageConcurrentIterator)
    {
      return new JsonWrites(new IConcurrentIterator<JsonWrite>[3]
      {
        (IConcurrentIterator<JsonWrite>) new ConcurrentIterator<JsonWrite>((IEnumerable<JsonWrite>) new JsonWrite[1]
        {
          (JsonWrite) (writer => writer.WriteStartArray())
        }),
        objectPageConcurrentIterator.Select<IEnumerable<JObject>, JsonWrite>((Func<IEnumerable<JObject>, JsonWrite>) (objPage => (JsonWrite) (writer =>
        {
          foreach (JToken jtoken in objPage)
            jtoken.WriteTo(writer);
        }))),
        (IConcurrentIterator<JsonWrite>) new ConcurrentIterator<JsonWrite>((IEnumerable<JsonWrite>) new JsonWrite[1]
        {
          (JsonWrite) (writer => writer.WriteEndArray())
        })
      });
    }
  }
}
