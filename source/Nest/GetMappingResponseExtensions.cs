// Decompiled with JetBrains decompiler
// Type: Nest.GetMappingResponseExtensions
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

namespace Nest
{
  public static class GetMappingResponseExtensions
  {
    public static ITypeMapping GetMappingFor<T>(this GetMappingResponse response) => response.GetMappingFor((IndexName) typeof (T));

    public static ITypeMapping GetMappingFor(this GetMappingResponse response, IndexName index)
    {
      if (index.IsNullOrEmpty())
        return (ITypeMapping) null;
      IndexMappings indexMappings;
      return !response.Indices.TryGetValue(index, out indexMappings) ? (ITypeMapping) null : (ITypeMapping) indexMappings.Mappings;
    }
  }
}
