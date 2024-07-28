// Decompiled with JetBrains decompiler
// Type: HdrHistogram.Utilities.ArrayExtensions
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

namespace HdrHistogram.Utilities
{
  internal static class ArrayExtensions
  {
    public static bool IsSequenceEqual<T>(this T[] source, T[] other)
    {
      if (source.Length != other.Length)
        return false;
      for (int index = 0; index < other.Length; ++index)
      {
        if (!object.Equals((object) source[index], (object) other[index]))
          return false;
      }
      return true;
    }
  }
}
