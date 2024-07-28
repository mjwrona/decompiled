// Decompiled with JetBrains decompiler
// Type: Nest.ShardPath
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System.Runtime.Serialization;

namespace Nest
{
  [DataContract]
  public class ShardPath
  {
    [DataMember(Name = "data_path")]
    public string DataPath { get; internal set; }

    [DataMember(Name = "is_custom_data_path")]
    public bool IsCustomDataPath { get; internal set; }

    [DataMember(Name = "state_path")]
    public string StatePath { get; internal set; }
  }
}
