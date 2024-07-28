// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.Metadata.EdmTypeWriterResolver
// Assembly: Microsoft.TeamFoundation.OData.Core, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6619C7F6-E44A-4143-AE77-6D570F968D9A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Core.dll

using Microsoft.OData.Edm;
using System.Collections.Generic;

namespace Microsoft.OData.Metadata
{
  internal sealed class EdmTypeWriterResolver : EdmTypeResolver
  {
    internal static EdmTypeWriterResolver Instance = new EdmTypeWriterResolver();

    private EdmTypeWriterResolver()
    {
    }

    internal override IEdmEntityType GetElementType(IEdmNavigationSource navigationSource) => navigationSource.EntityType();

    internal override IEdmTypeReference GetReturnType(IEdmOperationImport operationImport) => operationImport?.Operation.ReturnType;

    internal override IEdmTypeReference GetReturnType(
      IEnumerable<IEdmOperationImport> functionImportGroup)
    {
      throw new ODataException(Microsoft.OData.Strings.General_InternalError((object) InternalErrorCodes.EdmTypeWriterResolver_GetReturnTypeForOperationImportGroup));
    }

    internal override IEdmTypeReference GetParameterType(IEdmOperationParameter operationParameter) => operationParameter?.Type;
  }
}
