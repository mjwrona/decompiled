// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Documents.Schema
// Assembly: Microsoft.Azure.DocumentDB.Core, Version=2.10.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 20BB0EB7-1465-494C-8F4E-F898448B85D9
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.DocumentDB.Core.dll

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
      if (CustomTypeExtensions.IsAssignableFrom(typeof (Schema), schema.GetType()))
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
      if ((object) typeof (T) == (object) typeof (Schema) || (object) typeof (T) == (object) typeof (object))
        return (T) this;
      return this.propertyBag == null ? default (T) : this.propertyBag.ToObject<T>();
    }
  }
}
