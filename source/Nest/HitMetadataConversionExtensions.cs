// Decompiled with JetBrains decompiler
// Type: Nest.HitMetadataConversionExtensions
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;

namespace Nest
{
  internal static class HitMetadataConversionExtensions
  {
    public static IHitMetadata<TTarget> Copy<TDocument, TTarget>(
      this IHitMetadata<TDocument> source,
      Func<TDocument, TTarget> mapper)
      where TDocument : class
      where TTarget : class
    {
      return (IHitMetadata<TTarget>) new Hit<TTarget>()
      {
        Type = source.Type,
        Index = source.Index,
        Id = source.Id,
        Routing = source.Routing,
        Source = mapper(source.Source)
      };
    }
  }
}
