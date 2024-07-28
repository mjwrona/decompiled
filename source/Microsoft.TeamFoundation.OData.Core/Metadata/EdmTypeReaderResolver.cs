// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.Metadata.EdmTypeReaderResolver
// Assembly: Microsoft.TeamFoundation.OData.Core, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6619C7F6-E44A-4143-AE77-6D570F968D9A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Core.dll

using Microsoft.OData.Edm;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.OData.Metadata
{
  internal sealed class EdmTypeReaderResolver : EdmTypeResolver
  {
    private readonly IEdmModel model;
    private readonly Func<IEdmType, string, IEdmType> clientCustomTypeResolver;

    public EdmTypeReaderResolver(
      IEdmModel model,
      Func<IEdmType, string, IEdmType> clientCustomTypeResolver)
    {
      this.model = model;
      this.clientCustomTypeResolver = clientCustomTypeResolver;
    }

    internal override IEdmEntityType GetElementType(IEdmNavigationSource navigationSource)
    {
      IEdmEntityType typeToResolve = navigationSource.EntityType();
      return typeToResolve == null ? (IEdmEntityType) null : (IEdmEntityType) this.ResolveType((IEdmType) typeToResolve);
    }

    internal override IEdmTypeReference GetReturnType(IEdmOperationImport operationImport) => operationImport != null && operationImport.Operation.ReturnType != null ? this.ResolveTypeReference(operationImport.Operation.ReturnType) : (IEdmTypeReference) null;

    internal override IEdmTypeReference GetReturnType(
      IEnumerable<IEdmOperationImport> functionImportGroup)
    {
      return this.GetReturnType(functionImportGroup.FirstOrDefault<IEdmOperationImport>());
    }

    internal override IEdmTypeReference GetParameterType(IEdmOperationParameter operationParameter) => operationParameter != null ? this.ResolveTypeReference(operationParameter.Type) : (IEdmTypeReference) null;

    private IEdmTypeReference ResolveTypeReference(IEdmTypeReference typeReferenceToResolve) => this.clientCustomTypeResolver == null ? typeReferenceToResolve : this.ResolveType(typeReferenceToResolve.Definition).ToTypeReference(typeReferenceToResolve.IsNullable);

    private IEdmType ResolveType(IEdmType typeToResolve)
    {
      if (this.clientCustomTypeResolver == null)
        return typeToResolve;
      EdmTypeKind typeKind;
      if (!(typeToResolve is IEdmCollectionType edmCollectionType) || !edmCollectionType.ElementType.IsEntity())
        return MetadataUtils.ResolveTypeName(this.model, (IEdmType) null, typeToResolve.FullTypeName(), this.clientCustomTypeResolver, out typeKind);
      IEdmTypeReference elementType = edmCollectionType.ElementType;
      return (IEdmType) new EdmCollectionType(MetadataUtils.ResolveTypeName(this.model, (IEdmType) null, elementType.FullName(), this.clientCustomTypeResolver, out typeKind).ToTypeReference(elementType.IsNullable));
    }
  }
}
