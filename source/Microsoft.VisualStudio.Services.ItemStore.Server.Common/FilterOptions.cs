// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ItemStore.Server.Common.FilterOptions
// Assembly: Microsoft.VisualStudio.Services.ItemStore.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8190F04D-5888-4DB5-A838-8C98A67C6E45
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ItemStore.Server.Common.dll

using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.ItemStore.Server.Common
{
  public class FilterOptions
  {
    public IEnumerable<Filter> Filters { get; private set; }

    public FilterOptions(IEnumerable<Filter> filters) => this.Filters = filters;

    public FilterOptions(Filter filter)
      : this((IEnumerable<Filter>) new Filter[1]{ filter })
    {
    }

    public FilterOptions(string name, string value)
      : this(new Filter(name, value))
    {
    }
  }
}
