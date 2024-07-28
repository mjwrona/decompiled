// Decompiled with JetBrains decompiler
// Type: Nest.IndicesPrivilegesDescriptor`1
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;
using System.Collections.Generic;
using System.Linq;

namespace Nest
{
  public class IndicesPrivilegesDescriptor<T> : 
    DescriptorBase<IndicesPrivilegesDescriptor<T>, IIndicesPrivileges>,
    IIndicesPrivileges
    where T : class
  {
    IFieldSecurity IIndicesPrivileges.FieldSecurity { get; set; }

    Indices IIndicesPrivileges.Names { get; set; }

    IEnumerable<string> IIndicesPrivileges.Privileges { get; set; }

    QueryContainer IIndicesPrivileges.Query { get; set; }

    public IndicesPrivilegesDescriptor<T> Names(Indices indices) => this.Assign<Indices>(indices, (Action<IIndicesPrivileges, Indices>) ((a, v) => a.Names = v));

    public IndicesPrivilegesDescriptor<T> Names(params IndexName[] indices) => this.Assign<IndexName[]>(indices, (Action<IIndicesPrivileges, IndexName[]>) ((a, v) => a.Names = (Indices) v));

    public IndicesPrivilegesDescriptor<T> Names(IEnumerable<IndexName> indices) => this.Assign<IndexName[]>(indices.ToArray<IndexName>(), (Action<IIndicesPrivileges, IndexName[]>) ((a, v) => a.Names = (Indices) v));

    public IndicesPrivilegesDescriptor<T> Privileges(params string[] privileges) => this.Assign<string[]>(privileges, (Action<IIndicesPrivileges, string[]>) ((a, v) => a.Privileges = (IEnumerable<string>) v));

    public IndicesPrivilegesDescriptor<T> Privileges(IEnumerable<string> privileges) => this.Assign<IEnumerable<string>>(privileges, (Action<IIndicesPrivileges, IEnumerable<string>>) ((a, v) => a.Privileges = v));

    public IndicesPrivilegesDescriptor<T> FieldSecurity(
      Func<FieldSecurityDescriptor<T>, IFieldSecurity> fields)
    {
      return this.Assign<Func<FieldSecurityDescriptor<T>, IFieldSecurity>>(fields, (Action<IIndicesPrivileges, Func<FieldSecurityDescriptor<T>, IFieldSecurity>>) ((a, v) => a.FieldSecurity = v != null ? v(new FieldSecurityDescriptor<T>()) : (IFieldSecurity) null));
    }

    public IndicesPrivilegesDescriptor<T> Query(
      Func<QueryContainerDescriptor<T>, QueryContainer> query)
    {
      return this.Assign<Func<QueryContainerDescriptor<T>, QueryContainer>>(query, (Action<IIndicesPrivileges, Func<QueryContainerDescriptor<T>, QueryContainer>>) ((a, v) => a.Query = v != null ? v(new QueryContainerDescriptor<T>()) : (QueryContainer) null));
    }
  }
}
