// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.PatchOperationType
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Runtime.Serialization;

namespace Microsoft.Azure.Cosmos
{
  [JsonConverter(typeof (StringEnumConverter))]
  public enum PatchOperationType
  {
    [EnumMember(Value = "add")] Add,
    [EnumMember(Value = "remove")] Remove,
    [EnumMember(Value = "replace")] Replace,
    [EnumMember(Value = "set")] Set,
    [EnumMember(Value = "incr")] Increment,
  }
}
