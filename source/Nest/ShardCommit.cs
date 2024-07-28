// Decompiled with JetBrains decompiler
// Type: Nest.ShardCommit
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Nest
{
  [DataContract]
  public class ShardCommit
  {
    [DataMember(Name = "generation")]
    public int Generation { get; internal set; }

    [DataMember(Name = "id")]
    public string Id { get; internal set; }

    [DataMember(Name = "num_docs")]
    public long NumberOfDocuments { get; internal set; }

    [DataMember(Name = "user_data")]
    public IReadOnlyDictionary<string, string> UserData { get; internal set; }
  }
}
