// Decompiled with JetBrains decompiler
// Type: Nest.UserAgentProperty
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net;
using System.Runtime.Serialization;

namespace Nest
{
  [StringEnum]
  public enum UserAgentProperty
  {
    [EnumMember(Value = "NAME")] Name,
    [EnumMember(Value = "MAJOR")] Major,
    [EnumMember(Value = "MINOR")] Minor,
    [EnumMember(Value = "PATCH")] Patch,
    [EnumMember(Value = "OS")] Os,
    [EnumMember(Value = "OS_NAME")] OsName,
    [EnumMember(Value = "OS_MAJOR")] OsMajor,
    [EnumMember(Value = "OS_MINOR")] OsMinor,
    [EnumMember(Value = "DEVICE")] Device,
    [EnumMember(Value = "BUILD")] Build,
  }
}
