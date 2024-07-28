// Decompiled with JetBrains decompiler
// Type: Nest.IFieldLookup
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System.Runtime.Serialization;

namespace Nest
{
  [ReadAs(typeof (FieldLookup))]
  public interface IFieldLookup
  {
    [DataMember(Name = "id")]
    Id Id { get; set; }

    [DataMember(Name = "index")]
    IndexName Index { get; set; }

    [DataMember(Name = "path")]
    Field Path { get; set; }

    [DataMember(Name = "routing")]
    Routing Routing { get; set; }
  }
}
