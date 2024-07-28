// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.WriterUtils
// Assembly: Microsoft.TeamFoundation.OData.Core, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6619C7F6-E44A-4143-AE77-6D570F968D9A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Core.dll

using Microsoft.OData.Edm;
using Microsoft.OData.Metadata;

namespace Microsoft.OData
{
  internal static class WriterUtils
  {
    internal static string PrefixTypeNameForWriting(string typeName, ODataVersion version)
    {
      if (!string.IsNullOrEmpty(typeName))
      {
        string collectionItemTypeName = EdmLibraryExtensions.GetCollectionItemTypeName(typeName);
        if (collectionItemTypeName == null)
        {
          IEdmSchemaType element = EdmLibraryExtensions.ResolvePrimitiveTypeName(typeName);
          if (element != null)
          {
            typeName = element.ShortQualifiedName();
            return version >= ODataVersion.V401 ? typeName : WriterUtils.PrefixTypeName(typeName);
          }
        }
        else
        {
          IEdmSchemaType element = EdmLibraryExtensions.ResolvePrimitiveTypeName(collectionItemTypeName);
          if (element != null)
          {
            typeName = EdmLibraryExtensions.GetCollectionTypeName(element.ShortQualifiedName());
            return version >= ODataVersion.V401 ? typeName : WriterUtils.PrefixTypeName(typeName);
          }
        }
      }
      return WriterUtils.PrefixTypeName(typeName);
    }

    private static string PrefixTypeName(string typeName) => string.IsNullOrEmpty(typeName) ? typeName : "#" + typeName;
  }
}
