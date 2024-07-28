// Decompiled with JetBrains decompiler
// Type: Nest.SuggestContextsDescriptor`1
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;
using System.Collections.Generic;

namespace Nest
{
  public class SuggestContextsDescriptor<T> : 
    DescriptorPromiseBase<SuggestContextsDescriptor<T>, IList<ISuggestContext>>
    where T : class
  {
    public SuggestContextsDescriptor()
      : base((IList<ISuggestContext>) new List<ISuggestContext>())
    {
    }

    public SuggestContextsDescriptor<T> Category(
      Func<CategorySuggestContextDescriptor<T>, ICategorySuggestContext> categoryDescriptor)
    {
      return this.AddContext(categoryDescriptor != null ? (ISuggestContext) categoryDescriptor(new CategorySuggestContextDescriptor<T>()) : (ISuggestContext) null);
    }

    public SuggestContextsDescriptor<T> GeoLocation(
      Func<GeoSuggestContextDescriptor<T>, IGeoSuggestContext> geoLocationDescriptor)
    {
      return this.AddContext(geoLocationDescriptor != null ? (ISuggestContext) geoLocationDescriptor(new GeoSuggestContextDescriptor<T>()) : (ISuggestContext) null);
    }

    private SuggestContextsDescriptor<T> AddContext(ISuggestContext context) => context != null ? this.Assign<ISuggestContext>(context, (Action<IList<ISuggestContext>, ISuggestContext>) ((a, v) => a.Add(v))) : this;
  }
}
