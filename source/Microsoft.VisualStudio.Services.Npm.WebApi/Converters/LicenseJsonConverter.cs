// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Npm.WebApi.Converters.LicenseJsonConverter
// Assembly: Microsoft.VisualStudio.Services.Npm.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 639B57A1-1338-429F-9659-38C0A0394E05
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Npm.WebApi.dll

using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Npm.WebApi.Converters
{
  public class LicenseJsonConverter : BaseMultiFormatJsonConverter<string>
  {
    public const string LicenseTypePropertyName = "type";
    public const string UrlPropertyName = "url";

    protected override object ParseToken(
      JsonReader reader,
      Type objectType,
      object existingValue,
      JsonSerializer serializer)
    {
      return (object) (string) this.ReadJson(reader, (Type) null, existingValue, serializer);
    }

    protected override string ParseStringValue(string value) => value;

    protected override string ParseProperties(Dictionary<string, string> properties)
    {
      if (properties.ContainsKey("type"))
        return properties["type"] + (properties.ContainsKey("url") ? " (" + properties["url"] + ")" : (string) null);
      return !properties.ContainsKey("url") ? string.Empty : properties["url"];
    }

    protected override string ParseArray(List<object> tokens) => string.Join(" OR ", tokens.Where<object>((Func<object, bool>) (x => x != null)).Select<object, string>((Func<object, string>) (x => x.ToString())));
  }
}
