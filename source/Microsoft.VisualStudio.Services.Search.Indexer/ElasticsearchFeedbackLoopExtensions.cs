// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Indexer.ElasticsearchFeedbackLoopExtensions
// Assembly: Microsoft.VisualStudio.Services.Search.Indexer, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 167B1EA6-4D18-408E-89C6-597B8290976F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.Indexer.dll

using Microsoft.VisualStudio.Services.Search.Common.Entities;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Search.Indexer
{
  internal static class ElasticsearchFeedbackLoopExtensions
  {
    private static readonly IDictionary<string, string> s_entityShardSizeKeyMap = (IDictionary<string, string>) new Dictionary<string, string>();
    private static readonly IDictionary<string, string> s_entityExtremlyLargeShardSizeKeyMap = (IDictionary<string, string>) new Dictionary<string, string>();
    private static readonly IDictionary<string, string> s_esFeedbackloopRegKeyEntityMap = (IDictionary<string, string>) new Dictionary<string, string>();
    private static readonly object s_lock = new object();

    internal static string GetEntityShardSizeKey(this IEntityType entityType) => ElasticsearchFeedbackLoopExtensions.GetOrUpdateFromMap<string>(ElasticsearchFeedbackLoopExtensions.s_entityShardSizeKeyMap, entityType.Name, "/Service/ALMSearch/Settings/MaxShardSize/");

    internal static string GetEntityExtremlyLargeShardSizeKey(this IEntityType entityType) => ElasticsearchFeedbackLoopExtensions.GetOrUpdateFromMap<string>(ElasticsearchFeedbackLoopExtensions.s_entityExtremlyLargeShardSizeKeyMap, entityType.Name, "/Service/ALMSearch/Settings/LargeShardSizeThresholdBytes/");

    internal static string GetESFeedbackLoopRegKey(this IEntityType entityType) => ElasticsearchFeedbackLoopExtensions.GetOrUpdateFromMap<string>(ElasticsearchFeedbackLoopExtensions.s_esFeedbackloopRegKeyEntityMap, entityType.Name, "/Service/ALMSearch/Settings/EnableESFeedbackLoop/");

    private static string GetOrUpdateFromMap<TK>(
      IDictionary<TK, string> lookupMap,
      TK key,
      string basePath)
    {
      if (!lookupMap.ContainsKey(key))
      {
        lock (ElasticsearchFeedbackLoopExtensions.s_lock)
        {
          if (!lookupMap.ContainsKey(key))
            lookupMap.Add(key, basePath + key?.ToString());
        }
      }
      return lookupMap[key];
    }
  }
}
