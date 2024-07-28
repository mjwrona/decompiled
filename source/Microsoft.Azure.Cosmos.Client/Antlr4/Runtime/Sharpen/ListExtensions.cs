// Decompiled with JetBrains decompiler
// Type: Antlr4.Runtime.Sharpen.ListExtensions
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using System.Collections.Generic;

namespace Antlr4.Runtime.Sharpen
{
  internal static class ListExtensions
  {
    public static T Set<T>(this IList<T> list, int index, T value) where T : class
    {
      T obj = list[index];
      list[index] = value;
      return obj;
    }
  }
}
