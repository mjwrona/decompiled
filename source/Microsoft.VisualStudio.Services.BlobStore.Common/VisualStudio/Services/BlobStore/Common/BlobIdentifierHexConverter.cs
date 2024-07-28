// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.BlobStore.Common.BlobIdentifierHexConverter
// Assembly: Microsoft.VisualStudio.Services.BlobStore.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: FAFB0281-5CF2-4D3F-992C-49FBB9BEC906
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.BlobStore.Common.dll

using BuildXL.Cache.ContentStore.Hashing;
using Microsoft.VisualStudio.Services.WebApi;
using Newtonsoft.Json;
using System;

namespace Microsoft.VisualStudio.Services.BlobStore.Common
{
  public class BlobIdentifierHexConverter : VssSecureJsonConverter
  {
    private static readonly Lazy<BlobIdentifierHexConverter> _instance = new Lazy<BlobIdentifierHexConverter>((Func<BlobIdentifierHexConverter>) (() => new BlobIdentifierHexConverter()));

    public static BlobIdentifierHexConverter Instance => BlobIdentifierHexConverter._instance.Value;

    public override bool CanConvert(Type objectType) => objectType == typeof (BlobIdentifier);

    public override object ReadJson(
      JsonReader reader,
      Type objectType,
      object existingValue,
      JsonSerializer serializer)
    {
      if (!(reader.ValueType == typeof (string)))
        throw new JsonReaderException(string.Format("unexpected JSON token type {0} while reading BlobIdentifier", (object) reader.TokenType));
      try
      {
        return (object) BlobIdentifier.Deserialize((string) reader.Value);
      }
      catch (Exception ex)
      {
        throw new JsonReaderException("failed to parse BlobIdentifier", ex);
      }
    }

    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
    {
      base.WriteJson(writer, value, serializer);
      writer.WriteValue(((BlobIdentifier) value).ValueString);
    }
  }
}
