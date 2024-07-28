// Decompiled with JetBrains decompiler
// Type: Nest.CompletionField
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Nest
{
  public class CompletionField
  {
    [DataMember(Name = "contexts")]
    public IDictionary<string, IEnumerable<string>> Contexts { get; set; }

    [DataMember(Name = "input")]
    public IEnumerable<string> Input { get; set; }

    [DataMember(Name = "weight")]
    public int? Weight { get; set; }
  }
}
