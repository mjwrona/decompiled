// Decompiled with JetBrains decompiler
// Type: Nest.Mappings
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;
using System.Collections;
using System.Collections.Generic;

namespace Nest
{
  [Obsolete("Mappings are no longer type dependent, please use TypeMapping directly")]
  public class Mappings : ObsoleteMappingsBase, ITypeMapping, IEnumerable<ITypeMapping>, IEnumerable
  {
    private IEnumerable<ITypeMapping> AsEnumerable => (IEnumerable<ITypeMapping>) new TypeMapping[1]
    {
      new TypeMapping()
    };

    public void Add(object _, ITypeMapping mapping) => this.Wrapped = mapping ?? this.Wrapped;

    public ITypeMapping this[object key] => this.Wrapped;

    public IEnumerator<ITypeMapping> GetEnumerator() => this.AsEnumerable.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => (IEnumerator) this.GetEnumerator();
  }
}
