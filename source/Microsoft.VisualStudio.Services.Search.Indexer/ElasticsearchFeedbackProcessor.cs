// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Indexer.ElasticsearchFeedbackProcessor
// Assembly: Microsoft.VisualStudio.Services.Search.Indexer, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 167B1EA6-4D18-408E-89C6-597B8290976F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.Indexer.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Search.Common;
using Microsoft.VisualStudio.Services.Search.Common.Entities;
using Nest;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Microsoft.VisualStudio.Services.Search.Indexer
{
  public class ElasticsearchFeedbackProcessor
  {
    public bool IsIndexOversized(
      IVssRequestContext requestContext,
      long indexSize,
      IEntityType entityType,
      int numPrimaryShards)
    {
      long maxShardSize = this.GetMaxShardSize(requestContext, entityType);
      this.ValidateMaximumShardSize(entityType, maxShardSize);
      long num = maxShardSize * (long) numPrimaryShards;
      return indexSize > num;
    }

    public bool IsShardOverSized(
      IVssRequestContext requestContext,
      long shardSize,
      IEntityType entityType)
    {
      long maxShardSize = this.GetMaxShardSize(requestContext, entityType);
      this.ValidateMaximumShardSize(entityType, maxShardSize);
      return shardSize > maxShardSize;
    }

    public IEnumerable<string> GetExtremelyLargeShards(
      IVssRequestContext requestContext,
      IEntityType entityType,
      IEnumerable<KeyValuePair<string, double>> shardSizes)
    {
      long thresholdShardSize = this.GetExtremelyLargeShardSize(requestContext, entityType);
      this.ValidateMaximumShardSize(entityType, thresholdShardSize);
      return shardSizes.Where<KeyValuePair<string, double>>((Func<KeyValuePair<string, double>, bool>) (it => it.Value > (double) thresholdShardSize)).Select<KeyValuePair<string, double>, string>((Func<KeyValuePair<string, double>, string>) (it => it.Key));
    }

    public bool IsHavingHighDeletedDocCountPercentage(
      IVssRequestContext requestContext,
      CatIndicesRecord catIndicesRecord)
    {
      int thresholdPercentage = this.GetDeletedDocCountThresholdPercentage(requestContext, catIndicesRecord.Index);
      bool flag = false;
      try
      {
        long num1 = long.Parse(catIndicesRecord.DocsCount, (IFormatProvider) CultureInfo.InvariantCulture);
        long num2 = long.Parse(catIndicesRecord.DocsDeleted, (IFormatProvider) CultureInfo.InvariantCulture);
        if (num1 > 0L)
          flag = (int) (num2 * 100L / num1) > thresholdPercentage;
      }
      catch (Exception ex)
      {
        flag = false;
      }
      return flag;
    }

    public bool IsEsIndexWithHighDeletedDocsPercentage(
      IVssRequestContext requestContext,
      ElasticsearchIndexDetail elasticsearchIndexDetail)
    {
      int thresholdPercentage = this.GetDeletedDocCountThresholdPercentage(requestContext, elasticsearchIndexDetail.IndexName);
      int configValue = requestContext.GetConfigValue<int>("/Service/ALMSearch/Settings/EsIndexMinDocCountThreshold", TeamFoundationHostType.Deployment, 10000);
      int num = configValue > 0 ? configValue : 0;
      bool flag = false;
      if (elasticsearchIndexDetail.DocumentCount > (long) num)
        flag = (int) (elasticsearchIndexDetail.DeletedDocumentCount * 100L / elasticsearchIndexDetail.DocumentCount) > thresholdPercentage;
      return flag;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public long GetExtremelyLargeShardSize(
      IVssRequestContext requestContext,
      IEntityType entityType)
    {
      return requestContext.GetConfigValue<long>(entityType.GetEntityExtremlyLargeShardSizeKey(), TeamFoundationHostType.Deployment, 161061273600L);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private long GetMaxShardSize(IVssRequestContext requestContext, IEntityType entityType) => requestContext.GetConfigValue<long>(entityType.GetEntityShardSizeKey(), TeamFoundationHostType.Deployment);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private int GetDeletedDocCountThresholdPercentage(
      IVssRequestContext requestContext,
      string indexName)
    {
      return !indexName.StartsWith("workitem", StringComparison.OrdinalIgnoreCase) ? requestContext.GetConfigValue<int>("/Service/ALMSearch/Settings/DeletedDocCountThresholdPercentage/", TeamFoundationHostType.Deployment, 10) : requestContext.GetConfigValue<int>("/Service/ALMSearch/Settings/WorkItemDeletedDocCountThresholdPercentage/", TeamFoundationHostType.Deployment, 100);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void ValidateMaximumShardSize(IEntityType entityType, long maxShardSize)
    {
      if (maxShardSize <= 0L)
        throw new ArgumentOutOfRangeException(FormattableString.Invariant(FormattableStringFactory.Create("{0} for entity {1} is wrongly set in registry", (object) nameof (maxShardSize), (object) entityType.Name)));
    }
  }
}
