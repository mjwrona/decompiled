// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.CodeSense.Server.Data.EnumerableExtension
// Assembly: Microsoft.TeamFoundation.CodeSense.Server.Shared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 548902A5-AE61-4BC7-8D52-315B40AB5900
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.CodeSense.Server.Shared.dll

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Microsoft.TeamFoundation.CodeSense.Server.Data
{
  internal static class EnumerableExtension
  {
    internal static IReadOnlyList<T> ToReadOnlyList<T>(this IList<T> source) => source != null ? (IReadOnlyList<T>) new ReadOnlyCollection<T>(source) : throw new ArgumentNullException(nameof (source));

    internal static IReadOnlyList<T> ToReadOnlyList<T>(this IEnumerable<T> source) => source != null ? (IReadOnlyList<T>) new ReadOnlyCollection<T>((IList<T>) source.ToList<T>()) : throw new ArgumentNullException(nameof (source));
  }
}
