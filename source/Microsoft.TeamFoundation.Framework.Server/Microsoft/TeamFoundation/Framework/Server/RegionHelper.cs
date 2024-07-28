// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.RegionHelper
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Common;
using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.Framework.Server
{
  internal static class RegionHelper
  {
    internal static RegionCollection ParseRegionsAndWeights(
      string regions,
      string weights,
      bool throwOnError)
    {
      return RegionHelper.ParseRegionsAndWeights(RegionHelper.ParseRegions(regions, throwOnError), RegionHelper.ParseWeights(weights, throwOnError), throwOnError);
    }

    private static RegionCollection ParseRegionsAndWeights(
      string[] regions,
      int[] weights,
      bool throwOnError)
    {
      IDictionary<string, int> regionWeights = (IDictionary<string, int>) new Dictionary<string, int>();
      if (regions != null && weights != null)
      {
        if (regions.Length == weights.Length)
        {
          for (int index = 0; index < regions.Length; ++index)
          {
            if (!string.IsNullOrEmpty(regions[index]))
              regionWeights.Add(regions[index], weights[index]);
          }
        }
        else if (throwOnError)
          throw new InvalidRegionsWeightsException(string.Join(",", regions), string.Join<int>(",", (IEnumerable<int>) weights));
      }
      return new RegionCollection(regionWeights);
    }

    internal static void GetRegionsAndWeights(
      RegionCollection regionCollection,
      out string regions,
      out string weights)
    {
      KeyValuePair<string, int>[] regionWeights = regionCollection.GetRegionWeights();
      string[] strArray1 = new string[regionWeights.Length];
      string[] strArray2 = new string[regionWeights.Length];
      for (int index = 0; index < regionWeights.Length; ++index)
      {
        strArray1[index] = regionWeights[index].Key;
        strArray2[index] = regionWeights[index].Value.ToString();
      }
      regions = string.Join(",", strArray1);
      weights = string.Join(",", strArray2);
    }

    internal static string[] ParseRegions(string regions, bool throwOnError)
    {
      if (string.IsNullOrEmpty(regions))
        return Array.Empty<string>();
      string[] regions1 = regions.Split(new char[1]{ ',' }, StringSplitOptions.RemoveEmptyEntries);
      for (int index = 0; index < regions1.Length; ++index)
      {
        if (!TFCommonUtil.IsAlphaNumString(regions1[index]))
        {
          if (throwOnError)
            throw new InvalidRegionException(regions1[index]);
          regions1[index] = (string) null;
        }
      }
      return regions1;
    }

    internal static int[] ParseWeights(string weights, bool throwOnError)
    {
      if (string.IsNullOrEmpty(weights))
        return Array.Empty<int>();
      string[] strArray = weights.Split(new char[1]{ ',' }, StringSplitOptions.RemoveEmptyEntries);
      int[] weights1 = new int[strArray.Length];
      for (int index = 0; index < strArray.Length; ++index)
      {
        int result = 0;
        if (!int.TryParse(strArray[index], out result) && throwOnError)
          throw new InvalidWeightException();
        if (result < 0)
        {
          if (throwOnError)
            throw new InvalidWeightException(result);
          result = 0;
        }
        else if (result > 100)
        {
          if (throwOnError)
            throw new InvalidWeightException(result);
          result = 100;
        }
        weights1[index] = result;
      }
      return weights1;
    }
  }
}
