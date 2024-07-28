// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Utilities.Internal.ObjectExtensions
// Assembly: Microsoft.VisualStudio.Utilities.Internal, Version=14.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E02F1555-E063-4795-BC05-853CA7424F51
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Utilities.Internal.dll

using System.Collections.Generic;
using System.Linq;

namespace Microsoft.VisualStudio.Utilities.Internal
{
  public static class ObjectExtensions
  {
    public static IEnumerable<T> Enumerate<T>(this T value)
    {
      yield return value;
    }

    public static IEnumerable<T> EmptyIfNull<T>(this IEnumerable<T> enumeration) => enumeration ?? Enumerable.Empty<T>();
  }
}
