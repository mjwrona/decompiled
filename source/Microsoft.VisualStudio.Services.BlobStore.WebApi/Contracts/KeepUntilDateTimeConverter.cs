// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.BlobStore.WebApi.Contracts.KeepUntilDateTimeConverter
// Assembly: Microsoft.VisualStudio.Services.BlobStore.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4500AC57-FBCC-4F18-B11F-F661A75E4A46
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.BlobStore.WebApi.dll

using Microsoft.VisualStudio.Services.BlobStore.Common;
using Microsoft.VisualStudio.Services.WebApi;
using Newtonsoft.Json;
using System;
using System.Globalization;

namespace Microsoft.VisualStudio.Services.BlobStore.WebApi.Contracts
{
  internal class KeepUntilDateTimeConverter : VssSecureDateTimeConverterBase
  {
    public override object ReadJson(
      JsonReader reader,
      Type objectType,
      object existingValue,
      JsonSerializer serializer)
    {
      return (object) KeepUntilBlobReference.ParseDate((string) reader.Value);
    }

    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
    {
      base.WriteJson(writer, value, serializer);
      writer.WriteValue(((DateTime) value).ToString(KeepUntilBlobReference.KeepUntilFormat, (IFormatProvider) CultureInfo.InvariantCulture));
    }
  }
}
