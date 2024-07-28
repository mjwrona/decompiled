// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Npm.WebApi.Converters.LegacyKeywordJsonConverter
// Assembly: Microsoft.VisualStudio.Services.Npm.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 639B57A1-1338-429F-9659-38C0A0394E05
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Npm.WebApi.dll

using Microsoft.VisualStudio.Services.Npm.WebApi.Exceptions;
using Microsoft.VisualStudio.Services.WebApi;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Npm.WebApi.Converters
{
  public class LegacyKeywordJsonConverter : VssSecureJsonConverter
  {
    public override bool CanWrite => false;

    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer) => throw new NotSupportedException();

    public override object ReadJson(
      JsonReader reader,
      Type objectType,
      object existingValue,
      JsonSerializer serializer)
    {
      if (reader.TokenType == JsonToken.String)
        return (object) this.ParseKeywords(reader.Value as string);
      if (reader.TokenType != JsonToken.StartArray)
        throw new InvalidPackageJsonException("Unexpected Keyword Format.");
      List<string> stringList = new List<string>();
      while (reader.Read() && JsonToken.EndArray != reader.TokenType)
      {
        string str = reader.Value as string;
        if (!string.IsNullOrEmpty(str))
          stringList.Add(str.Trim());
      }
      return (object) stringList.ToArray();
    }

    public override bool CanConvert(Type objectType) => objectType == typeof (string[]);

    private string[] ParseKeywords(string keywordString) => ((IEnumerable<string>) keywordString.Split(',')).Select<string, string>((Func<string, string>) (x => x.Trim())).ToArray<string>();
  }
}
