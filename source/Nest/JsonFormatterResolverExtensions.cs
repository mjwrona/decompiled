// Decompiled with JetBrains decompiler
// Type: Nest.JsonFormatterResolverExtensions
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net.Utf8Json;
using System.Runtime.CompilerServices;

namespace Nest
{
  internal static class JsonFormatterResolverExtensions
  {
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IConnectionSettingsValues GetConnectionSettings(
      this IJsonFormatterResolver formatterResolver)
    {
      return ((IJsonFormatterResolverWithSettings) formatterResolver).Settings;
    }
  }
}
