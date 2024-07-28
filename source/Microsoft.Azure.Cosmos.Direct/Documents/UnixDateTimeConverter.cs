// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Documents.UnixDateTimeConverter
// Assembly: Microsoft.Azure.Cosmos.Direct, Version=3.29.4.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: FFE3C00D-4333-4294-8947-B1C93A852E2F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Direct.dll

using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Globalization;

namespace Microsoft.Azure.Documents
{
  internal sealed class UnixDateTimeConverter : DateTimeConverterBase
  {
    private static DateTime UnixStartTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);

    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
    {
      if (!(value is DateTime dateTime))
        throw new ArgumentException(RMResources.DateTimeConverterInvalidDateTime, nameof (value));
      long totalSeconds = (long) (dateTime - UnixDateTimeConverter.UnixStartTime).TotalSeconds;
      writer.WriteValue(totalSeconds);
    }

    public override object ReadJson(
      JsonReader reader,
      Type objectType,
      object existingValue,
      JsonSerializer serializer)
    {
      if (reader.TokenType != JsonToken.Integer)
        throw new Exception(RMResources.DateTimeConverterInvalidReaderValue);
      double num;
      try
      {
        num = Convert.ToDouble(reader.Value, (IFormatProvider) CultureInfo.InvariantCulture);
      }
      catch
      {
        throw new Exception(RMResources.DateTimeConveterInvalidReaderDoubleValue);
      }
      return (object) UnixDateTimeConverter.UnixStartTime.AddSeconds(num);
    }
  }
}
