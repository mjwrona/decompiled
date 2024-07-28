// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.Metadata.MetadataUtils
// Assembly: Microsoft.TeamFoundation.OData.Core, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6619C7F6-E44A-4143-AE77-6D570F968D9A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Core.dll

using Microsoft.OData.Edm;
using Microsoft.OData.Edm.Vocabularies;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.OData.Metadata
{
  internal static class MetadataUtils
  {
    internal static IEnumerable<IEdmDirectValueAnnotation> GetODataAnnotations(
      this IEdmModel model,
      IEdmElement annotatable)
    {
      IEnumerable<IEdmDirectValueAnnotation> source = model.DirectValueAnnotations(annotatable);
      return source == null ? (IEnumerable<IEdmDirectValueAnnotation>) null : source.Where<IEdmDirectValueAnnotation>((Func<IEdmDirectValueAnnotation, bool>) (a => a.NamespaceUri == "http://docs.oasis-open.org/odata/ns/metadata"));
    }

    internal static IEdmType ResolveTypeNameForWrite(IEdmModel model, string typeName) => MetadataUtils.ResolveTypeName(model, (IEdmType) null, typeName, (Func<IEdmType, string, IEdmType>) null, out EdmTypeKind _);

    internal static IEdmType ResolveTypeNameForRead(
      IEdmModel model,
      IEdmType expectedType,
      string typeName,
      Func<IEdmType, string, IEdmType> clientCustomTypeResolver,
      out EdmTypeKind typeKind)
    {
      return MetadataUtils.ResolveTypeName(model, expectedType, typeName, clientCustomTypeResolver, out typeKind);
    }

    internal static IEdmType ResolveTypeName(
      IEdmModel model,
      IEdmType expectedType,
      string typeName,
      Func<IEdmType, string, IEdmType> customTypeResolver,
      out EdmTypeKind typeKind)
    {
      IEdmType edmType = (IEdmType) null;
      string collectionItemTypeName = EdmLibraryExtensions.GetCollectionItemTypeName(typeName);
      if (collectionItemTypeName == null)
      {
        if (customTypeResolver != null && model.IsUserModel())
        {
          edmType = customTypeResolver(expectedType, typeName);
          if (edmType == null)
            throw new ODataException(Microsoft.OData.Strings.MetadataUtils_ResolveTypeName((object) typeName));
        }
        else
          edmType = (IEdmType) model.FindType(typeName);
        typeKind = edmType == null ? EdmTypeKind.None : edmType.TypeKind;
      }
      else
      {
        typeKind = EdmTypeKind.Collection;
        IEdmType expectedType1 = (IEdmType) null;
        if (customTypeResolver != null && expectedType != null && expectedType.TypeKind == EdmTypeKind.Collection)
          expectedType1 = ((IEdmCollectionType) expectedType).ElementType.Definition;
        IEdmType itemType = MetadataUtils.ResolveTypeName(model, expectedType1, collectionItemTypeName, customTypeResolver, out EdmTypeKind _);
        if (itemType != null)
          edmType = (IEdmType) EdmLibraryExtensions.GetCollectionType(itemType);
      }
      return edmType;
    }

    internal static IList<IEdmOperation> CalculateBindableOperationsForType(
      IEdmType bindingType,
      IEdmModel model,
      EdmTypeResolver edmTypeResolver)
    {
      IEnumerable<IEdmOperation> boundOperations;
      try
      {
        boundOperations = model.FindBoundOperations(bindingType);
      }
      catch (Exception ex)
      {
        if (ExceptionUtils.IsCatchableExceptionType(ex))
          throw new ODataException(Microsoft.OData.Strings.MetadataUtils_CalculateBindableOperationsForType((object) bindingType.FullTypeName()), ex);
        throw;
      }
      List<IEdmOperation> operationsForType = new List<IEdmOperation>();
      foreach (IEdmOperation edmOperation in boundOperations)
      {
        if (!edmOperation.IsBound)
          throw new ODataException(Microsoft.OData.Strings.EdmLibraryExtensions_UnBoundOperationsFoundFromIEdmModelFindMethodIsInvalid((object) edmOperation.Name));
        IEdmOperationParameter operationParameter = edmOperation.Parameters.FirstOrDefault<IEdmOperationParameter>() != null ? edmOperation.Parameters.FirstOrDefault<IEdmOperationParameter>() : throw new ODataException(Microsoft.OData.Strings.EdmLibraryExtensions_NoParameterBoundOperationsFoundFromIEdmModelFindMethodIsInvalid((object) edmOperation.Name));
        if (edmTypeResolver.GetParameterType(operationParameter).Definition.IsAssignableFrom(bindingType))
          operationsForType.Add(edmOperation);
      }
      return (IList<IEdmOperation>) operationsForType;
    }

    internal static IEdmTypeReference LookupTypeOfTerm(string qualifiedTermName, IEdmModel model)
    {
      IEdmTypeReference edmTypeReference = (IEdmTypeReference) null;
      IEdmTerm term = model.FindTerm(qualifiedTermName);
      if (term != null)
        edmTypeReference = term.Type;
      return edmTypeReference;
    }

    internal static IEdmTypeReference GetNullablePayloadTypeReference(IEdmType payloadType) => payloadType != null ? payloadType.ToTypeReference(true) : (IEdmTypeReference) null;
  }
}
