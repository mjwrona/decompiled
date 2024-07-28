// Decompiled with JetBrains decompiler
// Type: Nest.ResolvedIndex
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Nest
{
  public class ResolvedIndex
  {
    [DataMember(Name = "name")]
    public string Name { get; internal set; }

    [DataMember(Name = "aliases")]
    public IReadOnlyCollection<string> Aliases { get; internal set; }

    [DataMember(Name = "attributes")]
    public IReadOnlyCollection<string> Attributes { get; internal set; }

    [DataMember(Name = "data_stream")]
    public string DataStream { get; internal set; }
  }
}
