// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Content.Server.Azure.BooleanFilter`1
// Assembly: Microsoft.VisualStudio.Services.Content.Server.Azure, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7823E4AE-BEB6-4A7C-9914-276DEAE1FB1F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Content.Server.Azure.dll

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Microsoft.VisualStudio.Services.Content.Server.Azure
{
  public class BooleanFilter<T> : IFilter<T> where T : IColumn
  {
    public readonly IFilter<T>[] Filters;
    public readonly BooleanOperator Operator;

    public BooleanFilter(BooleanOperator @operator, params IFilter<T>[] filters)
    {
      if (filters == null)
        throw new ArgumentNullException(nameof (filters));
      this.Filters = ((IEnumerable<IFilter<T>>) filters).Where<IFilter<T>>((Func<IFilter<T>, bool>) (filter => filter != null && !filter.IsNull)).ToArray<IFilter<T>>();
      this.Operator = @operator;
    }

    public bool IsNull => ((IEnumerable<IFilter<T>>) this.Filters).All<IFilter<T>>((Func<IFilter<T>, bool>) (filter => filter.IsNull));

    public StringBuilder CreateFilter(StringBuilder builder)
    {
      if (((IEnumerable<IFilter<T>>) this.Filters).Any<IFilter<T>>())
      {
        bool flag = true;
        builder.Append('(');
        foreach (IFilter<T> filter in this.Filters)
        {
          if (!flag)
            builder.Append(this.Operator.OperatorString);
          builder.Append('(');
          StringBuilder builder1 = builder;
          filter.CreateFilter(builder1);
          builder.Append(')');
          flag = false;
        }
        builder.Append(')');
      }
      return builder;
    }

    public bool IsMatch(ITableEntityWithColumns entity)
    {
      if (!((IEnumerable<IFilter<T>>) this.Filters).Any<IFilter<T>>())
        return true;
      if (this.Operator.Equals((object) BooleanOperator.And))
        return ((IEnumerable<IFilter<T>>) this.Filters).All<IFilter<T>>((Func<IFilter<T>, bool>) (f => f.IsMatch(entity)));
      if (this.Operator.Equals((object) BooleanOperator.Or))
        return ((IEnumerable<IFilter<T>>) this.Filters).Any<IFilter<T>>((Func<IFilter<T>, bool>) (f => f.IsMatch(entity)));
      throw new InvalidOperationException(string.Format("unable to intepret operator {0}", (object) this.Operator));
    }
  }
}
