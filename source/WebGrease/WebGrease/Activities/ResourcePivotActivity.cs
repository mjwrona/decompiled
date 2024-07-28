// Decompiled with JetBrains decompiler
// Type: WebGrease.Activities.ResourcePivotActivity
// Assembly: WebGrease, Version=1.6.5135.21930, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 86487675-C393-48D4-AFEC-7657DB09B21F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\WebGrease.dll

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using WebGrease.Configuration;
using WebGrease.Extensions;

namespace WebGrease.Activities
{
  internal static class ResourcePivotActivity
  {
    internal static IEnumerable<ContentItem> ApplyResourceKeys(
      ContentItem inputItem,
      Dictionary<string, IDictionary<string, IDictionary<string, string>>> mergedResoures)
    {
      if (mergedResoures == null || !mergedResoures.Any<KeyValuePair<string, IDictionary<string, IDictionary<string, string>>>>())
        return (IEnumerable<ContentItem>) new ContentItem[1]
        {
          inputItem
        };
      List<ContentItem> contentItemList = new List<ContentItem>();
      try
      {
        string content = inputItem.Content;
        foreach (KeyValuePair<ResourcePivotKey[], IDictionary<string, IDictionary<string, string>>> usedGroupedResource in ResourcePivotActivity.GetUsedGroupedResources(content, mergedResoures))
        {
          string str = content;
          foreach (KeyValuePair<string, IDictionary<string, string>> keyValuePair in (IEnumerable<KeyValuePair<string, IDictionary<string, string>>>) usedGroupedResource.Value)
            str = ResourcesResolver.ExpandResourceKeys(str, keyValuePair.Value);
          contentItemList.Add(ContentItem.FromContent(str, inputItem, usedGroupedResource.Key));
        }
      }
      catch (ResourceOverrideException ex)
      {
        throw new WorkflowException(string.Format((IFormatProvider) CultureInfo.CurrentUICulture, ResourceStrings.ResourcePivotActivityDuplicateKeysError, new object[1]
        {
          (object) ex.TokenKey
        }), (Exception) ex);
      }
      catch (Exception ex)
      {
        throw new WorkflowException(ResourceStrings.ResourcePivotActivityError, ex);
      }
      return (IEnumerable<ContentItem>) contentItemList;
    }

    internal static Dictionary<ResourcePivotKey[], IDictionary<string, IDictionary<string, string>>> GetUsedGroupedResources(
      string content,
      Dictionary<string, IDictionary<string, IDictionary<string, string>>> mergedResoures)
    {
      Dictionary<ResourcePivotKey[], IDictionary<string, IDictionary<string, string>>> groupedAndUsedResources = new Dictionary<ResourcePivotKey[], IDictionary<string, IDictionary<string, string>>>()
      {
        {
          new ResourcePivotKey[0],
          (IDictionary<string, IDictionary<string, string>>) new Dictionary<string, IDictionary<string, string>>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase)
        }
      };
      if (mergedResoures == null || !mergedResoures.Any<KeyValuePair<string, IDictionary<string, IDictionary<string, string>>>>())
        return groupedAndUsedResources;
      foreach (KeyValuePair<string, IDictionary<string, IDictionary<string, string>>> mergedResoure in mergedResoures)
        groupedAndUsedResources = ResourcePivotActivity.GetUsedGroupedResources(groupedAndUsedResources, content, mergedResoure.Key, mergedResoure.Value);
      return groupedAndUsedResources;
    }

    private static Dictionary<ResourcePivotKey[], IDictionary<string, IDictionary<string, string>>> GetUsedGroupedResources(
      Dictionary<ResourcePivotKey[], IDictionary<string, IDictionary<string, string>>> groupedAndUsedResources,
      string content,
      string resourcePivotGroupKey,
      IDictionary<string, IDictionary<string, string>> resourcePivotKeyValues)
    {
      if (resourcePivotKeyValues == null || !resourcePivotKeyValues.Any<KeyValuePair<string, IDictionary<string, string>>>())
        return groupedAndUsedResources;
      Dictionary<ResourcePivotKey[], IDictionary<string, IDictionary<string, string>>> groupedResources = new Dictionary<ResourcePivotKey[], IDictionary<string, IDictionary<string, string>>>();
      foreach (Tuple<List<string>, Dictionary<string, string>> groupedUsedResourceKey in ResourcesResolver.GetGroupedUsedResourceKeys(content, resourcePivotKeyValues))
      {
        foreach (KeyValuePair<ResourcePivotKey[], IDictionary<string, IDictionary<string, string>>> groupedAndUsedResource in groupedAndUsedResources)
        {
          IEnumerable<ResourcePivotKey> second = groupedUsedResourceKey.Item1.Select<string, ResourcePivotKey>((Func<string, ResourcePivotKey>) (key => new ResourcePivotKey(resourcePivotGroupKey, key)));
          ResourcePivotKey[] array = ((IEnumerable<ResourcePivotKey>) groupedAndUsedResource.Key).Concat<ResourcePivotKey>(second).ToArray<ResourcePivotKey>();
          Dictionary<string, IDictionary<string, string>> dictionary = new Dictionary<string, IDictionary<string, string>>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
          dictionary.AddRange<string, IDictionary<string, string>>((IEnumerable<KeyValuePair<string, IDictionary<string, string>>>) groupedAndUsedResource.Value);
          dictionary.Add(resourcePivotGroupKey, (IDictionary<string, string>) groupedUsedResourceKey.Item2);
          groupedResources.Add(array, (IDictionary<string, IDictionary<string, string>>) dictionary);
        }
      }
      return groupedResources;
    }
  }
}
