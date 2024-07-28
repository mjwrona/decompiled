// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Pipelines.Server.ObjectModel.PipelineOrderByExpression
// Assembly: Microsoft.Azure.Pipelines.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: DC20940E-746B-4985-9936-F8ACD7ADA1DB
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Pipelines.Server.dll

using Microsoft.Azure.Pipelines.WebApi;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace Microsoft.Azure.Pipelines.Server.ObjectModel
{
  [TypeConverter(typeof (PipelineOrderByExpressionConverter))]
  public sealed class PipelineOrderByExpression : OrderByExpression<PipelineSortField>
  {
    public static readonly PipelineOrderByExpression Default;
    private static readonly char[] Separator = new char[1]
    {
      ','
    };
    private static readonly IReadOnlyList<SortExpression<PipelineSortField>> DefaultList = (IReadOnlyList<SortExpression<PipelineSortField>>) new SortExpression<PipelineSortField>[1]
    {
      new SortExpression<PipelineSortField>(PipelineSortField.Name, SortDirection.Ascending)
    };

    private PipelineOrderByExpression()
      : base(PipelineOrderByExpression.DefaultList)
    {
    }

    public PipelineOrderByExpression(string orderBy)
      : base(PipelineOrderByExpression.Parse(orderBy))
    {
    }

    static PipelineOrderByExpression() => PipelineOrderByExpression.Default = new PipelineOrderByExpression();

    public string GenerateContinuationToken(Pipeline nextPipeline)
    {
      ArgumentUtility.CheckForNull<Pipeline>(nextPipeline, nameof (nextPipeline));
      List<string> values = new List<string>();
      foreach (SortExpression<PipelineSortField> sortExpression in (OrderByExpression<PipelineSortField>) this)
      {
        if (sortExpression.Field == PipelineSortField.Name)
          values.Add(nextPipeline.Name);
      }
      return string.Join(",", (IEnumerable<string>) values);
    }

    public QueryPipelinesParameters GenerateQueryParameters(string continuationToken)
    {
      continuationToken = continuationToken?.Trim();
      QueryPipelinesParameters queryParameters = new QueryPipelinesParameters();
      if (!string.IsNullOrEmpty(continuationToken))
      {
        string[] strArray = continuationToken.Split(PipelineOrderByExpression.Separator);
        if (strArray.Length != this.Count)
          throw new InvalidContinuationTokenException(PipelinesServerResources.InvalidContinuationToken());
        for (int index = 0; index < strArray.Length; ++index)
        {
          if (this[index].Field == PipelineSortField.Name)
            queryParameters.NextName = strArray[index].Trim();
        }
      }
      return queryParameters;
    }

    public static implicit operator PipelineOrderByExpression(string orderBy) => new PipelineOrderByExpression(orderBy);

    private static IReadOnlyList<SortExpression<PipelineSortField>> Parse(string orderBy)
    {
      orderBy = orderBy?.Trim();
      IReadOnlyList<SortExpression<PipelineSortField>> sortExpressionList = PipelineOrderByExpression.DefaultList;
      if (!string.IsNullOrEmpty(orderBy))
      {
        sortExpressionList = (IReadOnlyList<SortExpression<PipelineSortField>>) ((IEnumerable<string>) orderBy.Split(PipelineOrderByExpression.Separator, StringSplitOptions.RemoveEmptyEntries)).Select<string, SortExpression<PipelineSortField>>((Func<string, SortExpression<PipelineSortField>>) (x => new SortExpression<PipelineSortField>(x))).ToList<SortExpression<PipelineSortField>>();
        if (sortExpressionList.Count == 0)
          sortExpressionList = PipelineOrderByExpression.DefaultList;
      }
      return sortExpressionList;
    }
  }
}
