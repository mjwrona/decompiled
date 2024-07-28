// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Npm.WebApi.Converters.ReadMeJsonConverter
// Assembly: Microsoft.VisualStudio.Services.Npm.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 639B57A1-1338-429F-9659-38C0A0394E05
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Npm.WebApi.dll

using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Npm.WebApi.Converters
{
  public class ReadMeJsonConverter : BaseMultiFormatJsonConverter<string>
  {
    protected override object ParseToken(
      JsonReader reader,
      Type objectType,
      object existingValue,
      JsonSerializer serializer)
    {
      return (object) (string) this.ReadJson(reader, (Type) null, existingValue, serializer);
    }

    protected override string ParseStringValue(string value) => value;

    protected override string ParseProperties(Dictionary<string, string> properties) => string.Empty;

    protected override string ParseArray(List<object> tokens) => string.Empty;
  }
}
