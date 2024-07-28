// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Documents.Schema
// Assembly: Microsoft.Azure.Cosmos.Direct, Version=3.29.4.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: FFE3C00D-4333-4294-8947-B1C93A852E2F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Direct.dll

using Newtonsoft.Json.Linq;
using System;
using System.Globalization;
using System.Net;

namespace Microsoft.Azure.Documents
{
  internal sealed class Schema : Resource
  {
    public string ResourceLink => this.GetValue<string>("resource");

    internal static Schema FromObject(object schema)
    {
      if (schema == null)
        return (Schema) null;
      if (typeof (Schema).IsAssignableFrom(schema.GetType()))
        return (Schema) schema;
      JObject jobject = JObject.FromObject(schema);
      Schema schema1 = new Schema();
      schema1.propertyBag = jobject;
      return schema1;
    }

    private object GetProperty(string propertyName, Type returnType)
    {
      JToken jtoken = this.propertyBag != null ? this.propertyBag[propertyName] : throw new DocumentClientException(string.Format((IFormatProvider) CultureInfo.CurrentUICulture, RMResources.PropertyNotFound, (object) propertyName), (Exception) null, new HttpStatusCode?());
      if (jtoken != null)
        return jtoken.ToObject(returnType);
    }

    private object SetProperty(string propertyName, object value)
    {
      if (value != null)
      {
        if (this.propertyBag == null)
          this.propertyBag = new JObject();
        this.propertyBag[propertyName] = JToken.FromObject(value);
      }
      else if (this.propertyBag != null)
        this.propertyBag.Remove(propertyName);
      return value;
    }

    private T AsType<T>()
    {
      if (typeof (T) == typeof (Schema) || typeof (T) == typeof (object))
        return (T) this;
      return this.propertyBag == null ? default (T) : this.propertyBag.ToObject<T>();
    }
  }
}
