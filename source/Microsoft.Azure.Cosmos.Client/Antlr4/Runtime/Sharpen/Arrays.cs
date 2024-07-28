// Decompiled with JetBrains decompiler
// Type: Antlr4.Runtime.Sharpen.Arrays
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using System;
using System.Collections.Generic;
using System.Text;

namespace Antlr4.Runtime.Sharpen
{
  internal static class Arrays
  {
    public static T[] CopyOf<T>(T[] array, int newSize)
    {
      if (array.Length == newSize)
        return (T[]) array.Clone();
      Array.Resize<T>(ref array, newSize);
      return array;
    }

    public static IList<T> AsList<T>(params T[] array) => (IList<T>) array;

    public static void Fill<T>(T[] array, T value)
    {
      for (int index = 0; index < array.Length; ++index)
        array[index] = value;
    }

    public static int HashCode<T>(T[] array)
    {
      if (array == null)
        return 0;
      int num = 1;
      foreach (T obj1 in array)
      {
        object obj2 = (object) obj1;
        num = 31 * num + (obj2 == null ? 0 : obj2.GetHashCode());
      }
      return num;
    }

    public static bool Equals<T>(T[] left, T[] right)
    {
      if (left == right)
        return true;
      if (left == null || right == null || left.Length != right.Length)
        return false;
      for (int index = 0; index < left.Length; ++index)
      {
        if (!object.Equals((object) left[index], (object) right[index]))
          return false;
      }
      return true;
    }

    public static string ToString<T>(T[] array)
    {
      if (array == null)
        return "null";
      StringBuilder stringBuilder = new StringBuilder();
      stringBuilder.Append('[');
      for (int index = 0; index < array.Length; ++index)
      {
        if (index > 0)
          stringBuilder.Append(", ");
        T obj = array[index];
        if ((object) obj == null)
          stringBuilder.Append("null");
        else
          stringBuilder.Append((object) obj);
      }
      stringBuilder.Append(']');
      return stringBuilder.ToString();
    }
  }
}
