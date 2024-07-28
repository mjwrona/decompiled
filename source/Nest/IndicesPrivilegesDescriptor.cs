// Decompiled with JetBrains decompiler
// Type: Nest.IndicesPrivilegesDescriptor
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;
using System.Collections.Generic;

namespace Nest
{
  public class IndicesPrivilegesDescriptor : 
    DescriptorPromiseBase<IndicesPrivilegesDescriptor, IList<IIndicesPrivileges>>
  {
    public IndicesPrivilegesDescriptor()
      : base((IList<IIndicesPrivileges>) new List<IIndicesPrivileges>())
    {
    }

    public IndicesPrivilegesDescriptor Add<T>(
      Func<IndicesPrivilegesDescriptor<T>, IIndicesPrivileges> selector)
      where T : class
    {
      return this.Assign<Func<IndicesPrivilegesDescriptor<T>, IIndicesPrivileges>>(selector, (Action<IList<IIndicesPrivileges>, Func<IndicesPrivilegesDescriptor<T>, IIndicesPrivileges>>) ((a, v) => a.AddIfNotNull<IIndicesPrivileges>(v != null ? v(new IndicesPrivilegesDescriptor<T>()) : (IIndicesPrivileges) null)));
    }
  }
}
