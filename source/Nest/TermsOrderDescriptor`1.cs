// Decompiled with JetBrains decompiler
// Type: Nest.TermsOrderDescriptor`1
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;
using System.Collections.Generic;

namespace Nest
{
  public class TermsOrderDescriptor<T> : 
    DescriptorPromiseBase<TermsOrderDescriptor<T>, IList<TermsOrder>>
    where T : class
  {
    public TermsOrderDescriptor()
      : base((IList<TermsOrder>) new List<TermsOrder>())
    {
    }

    public TermsOrderDescriptor<T> CountAscending() => this.Assign((Action<IList<TermsOrder>>) (a => a.Add(TermsOrder.CountAscending)));

    public TermsOrderDescriptor<T> CountDescending() => this.Assign((Action<IList<TermsOrder>>) (a => a.Add(TermsOrder.CountDescending)));

    public TermsOrderDescriptor<T> KeyAscending() => this.Assign((Action<IList<TermsOrder>>) (a => a.Add(TermsOrder.KeyAscending)));

    public TermsOrderDescriptor<T> KeyDescending() => this.Assign((Action<IList<TermsOrder>>) (a => a.Add(TermsOrder.KeyDescending)));

    public TermsOrderDescriptor<T> Ascending(string key) => !string.IsNullOrWhiteSpace(key) ? this.Assign<string>(key, (Action<IList<TermsOrder>, string>) ((a, v) => a.Add(new TermsOrder()
    {
      Key = v,
      Order = SortOrder.Ascending
    }))) : this;

    public TermsOrderDescriptor<T> Descending(string key) => !string.IsNullOrWhiteSpace(key) ? this.Assign<string>(key, (Action<IList<TermsOrder>, string>) ((a, v) => a.Add(new TermsOrder()
    {
      Key = v,
      Order = SortOrder.Descending
    }))) : this;
  }
}
