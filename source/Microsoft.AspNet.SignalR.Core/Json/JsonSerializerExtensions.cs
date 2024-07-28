// Decompiled with JetBrains decompiler
// Type: Microsoft.AspNet.SignalR.Json.JsonSerializerExtensions
// Assembly: Microsoft.AspNet.SignalR.Core, Version=2.4.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 095D5FBC-6474-494D-BE26-4EBE2B9AD3D0
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.AspNet.SignalR.Core.dll

using Microsoft.AspNet.SignalR.Infrastructure;
using Newtonsoft.Json;
using System;
using System.Globalization;
using System.IO;
using System.Text;

namespace Microsoft.AspNet.SignalR.Json
{
  public static class JsonSerializerExtensions
  {
    public static T Parse<T>(this JsonSerializer serializer, string json)
    {
      if (serializer == null)
        throw new ArgumentNullException(nameof (serializer));
      using (StringReader reader = new StringReader(json))
        return (T) serializer.Deserialize((TextReader) reader, typeof (T));
    }

    public static T Parse<T>(
      this JsonSerializer serializer,
      ArraySegment<byte> jsonBuffer,
      Encoding encoding)
    {
      if (serializer == null)
        throw new ArgumentNullException(nameof (serializer));
      using (ArraySegmentTextReader reader = new ArraySegmentTextReader(jsonBuffer, encoding))
        return (T) serializer.Deserialize((TextReader) reader, typeof (T));
    }

    public static void Serialize(this JsonSerializer serializer, object value, TextWriter writer)
    {
      if (serializer == null)
        throw new ArgumentNullException(nameof (serializer));
      if (value is IJsonWritable jsonWritable)
        jsonWritable.WriteJson(writer);
      else
        serializer.Serialize(writer, value);
    }

    public static string Stringify(this JsonSerializer serializer, object value)
    {
      if (serializer == null)
        throw new ArgumentNullException(nameof (serializer));
      using (StringWriter writer = new StringWriter((IFormatProvider) CultureInfo.InvariantCulture))
      {
        serializer.Serialize(value, (TextWriter) writer);
        return writer.ToString();
      }
    }
  }
}
