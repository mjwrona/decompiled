// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Content.Server.Azure.IFilterExtensions
// Assembly: Microsoft.VisualStudio.Services.Content.Server.Azure, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7823E4AE-BEB6-4A7C-9914-276DEAE1FB1F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Content.Server.Azure.dll

using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Content.Server.Azure
{
  public static class IFilterExtensions
  {
    public static IEnumerable<TEntity> Evaluate<T, TEntity>(
      this IFilter<T> filter,
      IEnumerable<TEntity> unfiltered)
      where T : IColumn
      where TEntity : ITableEntityWithColumns
    {
      return unfiltered.Where<TEntity>((Func<TEntity, bool>) (e => filter.IsMatch((ITableEntityWithColumns) e)));
    }
  }
}
