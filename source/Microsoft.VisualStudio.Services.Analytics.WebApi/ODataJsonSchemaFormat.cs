// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Analytics.WebApi.ODataJsonSchemaFormat
// Assembly: Microsoft.VisualStudio.Services.Analytics.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F858B048-FFE8-4E5F-8EBC-9B25DAC23DF8
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Analytics.WebApi.dll

using Newtonsoft.Json;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.Analytics.WebApi
{
  [DataContract]
  [JsonConverter(typeof (CamelCaseStringEnumConverter))]
  public enum ODataJsonSchemaFormat
  {
    Undefined,
    Date,
    [EnumMember(Value = "date-time")] DateTime,
    Double,
    Int8,
    Int16,
    Int32,
    Int64,
    Single,
    [EnumMember(Value = "uint8")] UInt8,
    Uuid,
    [EnumMember(Value = "base64url")] Base64Url,
    Decimal,
    Duration,
    Time,
  }
}
