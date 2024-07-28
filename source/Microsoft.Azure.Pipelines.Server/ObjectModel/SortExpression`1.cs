// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Pipelines.Server.ObjectModel.SortExpression`1
// Assembly: Microsoft.Azure.Pipelines.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: DC20940E-746B-4985-9936-F8ACD7ADA1DB
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Pipelines.Server.dll

using Microsoft.Azure.Pipelines.WebApi;
using Microsoft.VisualStudio.Services.Common;
using System;

namespace Microsoft.Azure.Pipelines.Server.ObjectModel
{
  public sealed class SortExpression<T> where T : struct, Enum
  {
    private static readonly char[] Separator = new char[1]
    {
      ' '
    };

    public SortExpression(T field, SortDirection direction)
    {
      this.Field = field;
      this.Direction = direction;
    }

    public SortExpression(string value)
    {
      value = value?.Trim();
      ArgumentUtility.CheckStringForNullOrEmpty(value, nameof (value));
      string[] strArray = value.Split(SortExpression<T>.Separator, StringSplitOptions.RemoveEmptyEntries);
      T result1;
      if (!Enum.TryParse<T>(strArray[0], true, out result1))
        throw new UnsupportedSortExpressionException(PipelinesServerResources.UnsupportedSortExpressionField((object) value));
      SortDirection result2 = SortDirection.Ascending;
      if (strArray.Length > 1 && !Enum.TryParse<SortDirection>(strArray[1], true, out result2))
      {
        if (string.Equals(strArray[1], "asc", StringComparison.OrdinalIgnoreCase))
        {
          result2 = SortDirection.Ascending;
        }
        else
        {
          if (!string.Equals(strArray[1], "desc", StringComparison.OrdinalIgnoreCase))
            throw new UnsupportedSortExpressionException(PipelinesServerResources.UnsupportedSortExpressionDirection((object) value));
          result2 = SortDirection.Descending;
        }
      }
      this.Field = result1;
      this.Direction = result2;
    }

    public static implicit operator SortExpression<T>(string value)
    {
      value = value?.Trim();
      return string.IsNullOrEmpty(value) ? (SortExpression<T>) null : new SortExpression<T>(value);
    }

    public SortDirection Direction { get; }

    public T Field { get; }
  }
}
