// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Gallery.Server.ReservedPublisher
// Assembly: Microsoft.VisualStudio.Services.Gallery.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B9EBBED5-135E-45CD-B0B4-F747360599CD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Gallery.Server.dll

using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Gallery.Server
{
  internal class ReservedPublisher
  {
    public static void IsPublisherNameReserved(
      string disallowedDisplayName,
      string displayName,
      int similarityPercentageBoundary)
    {
      string[] strArray = disallowedDisplayName.Split(',');
      string upperInvariant = displayName.Trim().ToUpperInvariant();
      foreach (string str in strArray)
      {
        if (ReservedPublisher.IsSimilar(str.Trim().ToUpperInvariant(), upperInvariant, similarityPercentageBoundary) || upperInvariant.Contains("Microsoft".ToUpperInvariant()))
          throw new ArgumentException(GalleryResources.DisallowedStringInPublisherName());
      }
    }

    private static bool IsSimilar(
      string original,
      string modified,
      int similarityPercentageBoundary)
    {
      return (int) Math.Ceiling((1.0 - (double) ReservedPublisher.EditDistance(original, modified) / (double) Math.Max(original.Length, modified.Length)) * 100.0) > similarityPercentageBoundary;
    }

    private static int EditDistance(string original, string modified)
    {
      if (string.Equals(original, modified))
        return 0;
      int length1 = original.Length;
      int length2 = modified.Length;
      if (string.IsNullOrEmpty(original) || string.IsNullOrEmpty(modified))
        return length1 + length2;
      int[,] numArray = new int[length1 + 1, length2 + 1];
      for (int index = 0; index <= length1; ++index)
        numArray[index, 0] = index;
      for (int index = 0; index <= length2; ++index)
        numArray[0, index] = index;
      for (int index1 = 1; index1 <= length1; ++index1)
      {
        for (int index2 = 1; index2 <= length2; ++index2)
        {
          int num = (int) original[index1 - 1] != (int) modified[index2 - 1] ? 1 : 0;
          int[] source = new int[3]
          {
            numArray[index1 - 1, index2] + 1,
            numArray[index1, index2 - 1] + 1,
            numArray[index1 - 1, index2 - 1] + num
          };
          numArray[index1, index2] = ((IEnumerable<int>) source).Min();
          if (index1 > 1 && index2 > 1 && (int) original[index1 - 1] == (int) modified[index2 - 2] && (int) original[index1 - 2] == (int) modified[index2 - 1])
            numArray[index1, index2] = Math.Min(numArray[index1, index2], numArray[index1 - 2, index2 - 2] + num);
        }
      }
      return numArray[length1, length2];
    }
  }
}
