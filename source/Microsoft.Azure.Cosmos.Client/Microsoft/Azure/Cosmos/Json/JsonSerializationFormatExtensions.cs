// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.Json.JsonSerializationFormatExtensions
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using Microsoft.Azure.Documents;
using System;

namespace Microsoft.Azure.Cosmos.Json
{
  internal static class JsonSerializationFormatExtensions
  {
    private static readonly string Text = ContentSerializationFormat.JsonText.ToString();
    private static readonly string Binary = ContentSerializationFormat.CosmosBinary.ToString();
    private static readonly string HybridRow = ContentSerializationFormat.HybridRow.ToString();

    public static string ToContentSerializationFormatString(
      this JsonSerializationFormat jsonSerializationFormat)
    {
      if (jsonSerializationFormat == JsonSerializationFormat.Text)
        return JsonSerializationFormatExtensions.Text;
      if (jsonSerializationFormat == JsonSerializationFormat.Binary)
        return JsonSerializationFormatExtensions.Binary;
      if (jsonSerializationFormat == JsonSerializationFormat.HybridRow)
        return JsonSerializationFormatExtensions.HybridRow;
      throw new ArgumentOutOfRangeException(string.Format("Unknown {0}: {1}", (object) "JsonSerializationFormat", (object) jsonSerializationFormat));
    }
  }
}
