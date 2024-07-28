// Decompiled with JetBrains decompiler
// Type: Nest.IndexPrivilegesChecksDescriptor
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;
using System.Collections.Generic;

namespace Nest
{
  public class IndexPrivilegesChecksDescriptor : 
    DescriptorPromiseBase<IndexPrivilegesChecksDescriptor, List<IIndexPrivilegesCheck>>
  {
    public IndexPrivilegesChecksDescriptor()
      : base(new List<IIndexPrivilegesCheck>())
    {
    }

    public IndexPrivilegesChecksDescriptor Index(
      Func<IndexPrivilegesChecksDescriptor.IndexPrivilegesCheckDesciptor, IIndexPrivilegesCheck> selector)
    {
      return this.Assign<Func<IndexPrivilegesChecksDescriptor.IndexPrivilegesCheckDesciptor, IIndexPrivilegesCheck>>(selector, (Action<List<IIndexPrivilegesCheck>, Func<IndexPrivilegesChecksDescriptor.IndexPrivilegesCheckDesciptor, IIndexPrivilegesCheck>>) ((a, v) => a.Add(v.InvokeOrDefault<IndexPrivilegesChecksDescriptor.IndexPrivilegesCheckDesciptor, IIndexPrivilegesCheck>(new IndexPrivilegesChecksDescriptor.IndexPrivilegesCheckDesciptor()))));
    }

    public class IndexPrivilegesCheckDesciptor : 
      DescriptorBase<IndexPrivilegesChecksDescriptor.IndexPrivilegesCheckDesciptor, IIndexPrivilegesCheck>,
      IIndexPrivilegesCheck
    {
      IEnumerable<string> IIndexPrivilegesCheck.Names { get; set; }

      IEnumerable<string> IIndexPrivilegesCheck.Privileges { get; set; }

      public IndexPrivilegesChecksDescriptor.IndexPrivilegesCheckDesciptor Privileges(
        params string[] privileges)
      {
        return this.Assign<string[]>(privileges, (Action<IIndexPrivilegesCheck, string[]>) ((a, v) => a.Privileges = (IEnumerable<string>) v));
      }

      public IndexPrivilegesChecksDescriptor.IndexPrivilegesCheckDesciptor Privileges(
        IEnumerable<string> privileges)
      {
        return this.Assign<IEnumerable<string>>(privileges, (Action<IIndexPrivilegesCheck, IEnumerable<string>>) ((a, v) => a.Privileges = v));
      }

      public IndexPrivilegesChecksDescriptor.IndexPrivilegesCheckDesciptor Names(
        params string[] names)
      {
        return this.Assign<string[]>(names, (Action<IIndexPrivilegesCheck, string[]>) ((a, v) => a.Names = (IEnumerable<string>) v));
      }

      public IndexPrivilegesChecksDescriptor.IndexPrivilegesCheckDesciptor Names(
        IEnumerable<string> names)
      {
        return this.Assign<IEnumerable<string>>(names, (Action<IIndexPrivilegesCheck, IEnumerable<string>>) ((a, v) => a.Names = v));
      }
    }
  }
}
