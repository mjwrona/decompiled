// Decompiled with JetBrains decompiler
// Type: Nest.Month
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net;
using System.Runtime.Serialization;

namespace Nest
{
  [StringEnum]
  public enum Month
  {
    [EnumMember(Value = "JAN")] January,
    [EnumMember(Value = "FEB")] February,
    [EnumMember(Value = "MAR")] March,
    [EnumMember(Value = "APR")] April,
    [EnumMember(Value = "MAY")] May,
    [EnumMember(Value = "JUN")] June,
    [EnumMember(Value = "JUL")] July,
    [EnumMember(Value = "AUG")] August,
    [EnumMember(Value = "SEP")] September,
    [EnumMember(Value = "OCT")] October,
    [EnumMember(Value = "NOV")] November,
    [EnumMember(Value = "DEC")] December,
  }
}
