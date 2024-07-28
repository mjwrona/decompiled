// Decompiled with JetBrains decompiler
// Type: Nest.InnerHitsResult
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace Nest
{
  public class InnerHitsResult
  {
    [DataMember(Name = "hits")]
    public InnerHitsMetadata Hits { get; internal set; }

    public IEnumerable<T> Documents<T>() where T : class => this.Hits != null ? this.Hits.Documents<T>() : Enumerable.Empty<T>();
  }
}
