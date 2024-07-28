// Decompiled with JetBrains decompiler
// Type: Nest.DeprecationInfo
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System.Runtime.Serialization;

namespace Nest
{
  public class DeprecationInfo
  {
    [DataMember(Name = "details")]
    public string Details { get; internal set; }

    [DataMember(Name = "level")]
    public DeprecationWarningLevel Level { get; internal set; }

    [DataMember(Name = "message")]
    public string Message { get; internal set; }

    [DataMember(Name = "url")]
    public string Url { get; internal set; }
  }
}
