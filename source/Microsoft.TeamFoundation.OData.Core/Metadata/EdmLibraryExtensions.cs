// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.Metadata.EdmLibraryExtensions
// Assembly: Microsoft.TeamFoundation.OData.Core, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6619C7F6-E44A-4143-AE77-6D570F968D9A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Core.dll

using Microsoft.OData.Edm;
using Microsoft.Spatial;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Linq;

namespace Microsoft.OData.Metadata
{
  [SuppressMessage("Microsoft.Maintainability", "CA1506:AvoidExcessiveClassCoupling", Justification = "The class coupling is due to mapping primitive types, lot of different types there.")]
  [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Edm", Justification = "Following EdmLib standards.")]
  internal static class EdmLibraryExtensions
  {
    private static readonly Dictionary<Type, IEdmPrimitiveTypeReference> PrimitiveTypeReferenceMap = new Dictionary<Type, IEdmPrimitiveTypeReference>((IEqualityComparer<Type>) EqualityComparer<Type>.Default);
    private static readonly EdmPrimitiveTypeReference BooleanTypeReference = EdmLibraryExtensions.ToTypeReference(EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.Boolean), false);
    private static readonly EdmPrimitiveTypeReference ByteTypeReference = EdmLibraryExtensions.ToTypeReference(EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.Byte), false);
    private static readonly EdmPrimitiveTypeReference DecimalTypeReference = EdmLibraryExtensions.ToTypeReference(EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.Decimal), false);
    private static readonly EdmPrimitiveTypeReference DoubleTypeReference = EdmLibraryExtensions.ToTypeReference(EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.Double), false);
    private static readonly EdmPrimitiveTypeReference Int16TypeReference = EdmLibraryExtensions.ToTypeReference(EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.Int16), false);
    private static readonly EdmPrimitiveTypeReference Int32TypeReference = EdmLibraryExtensions.ToTypeReference(EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.Int32), false);
    private static readonly EdmPrimitiveTypeReference Int64TypeReference = EdmLibraryExtensions.ToTypeReference(EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.Int64), false);
    private static readonly EdmPrimitiveTypeReference SByteTypeReference = EdmLibraryExtensions.ToTypeReference(EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.SByte), false);
    private static readonly EdmPrimitiveTypeReference StringTypeReference = EdmLibraryExtensions.ToTypeReference(EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.String), true);
    private static readonly EdmPrimitiveTypeReference SingleTypeReference = EdmLibraryExtensions.ToTypeReference(EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.Single), false);
    private const string CollectionTypeQualifier = "Collection";
    private const string CollectionTypeFormat = "Collection({0})";

    [SuppressMessage("Microsoft.Performance", "CA1810:InitializeReferenceTypeStaticFieldsInline", Justification = "Need to use the static constructor for the phone platform.")]
    static EdmLibraryExtensions()
    {
      EdmLibraryExtensions.PrimitiveTypeReferenceMap.Add(typeof (bool), (IEdmPrimitiveTypeReference) EdmLibraryExtensions.BooleanTypeReference);
      EdmLibraryExtensions.PrimitiveTypeReferenceMap.Add(typeof (byte), (IEdmPrimitiveTypeReference) EdmLibraryExtensions.ByteTypeReference);
      EdmLibraryExtensions.PrimitiveTypeReferenceMap.Add(typeof (Decimal), (IEdmPrimitiveTypeReference) EdmLibraryExtensions.DecimalTypeReference);
      EdmLibraryExtensions.PrimitiveTypeReferenceMap.Add(typeof (double), (IEdmPrimitiveTypeReference) EdmLibraryExtensions.DoubleTypeReference);
      EdmLibraryExtensions.PrimitiveTypeReferenceMap.Add(typeof (short), (IEdmPrimitiveTypeReference) EdmLibraryExtensions.Int16TypeReference);
      EdmLibraryExtensions.PrimitiveTypeReferenceMap.Add(typeof (int), (IEdmPrimitiveTypeReference) EdmLibraryExtensions.Int32TypeReference);
      EdmLibraryExtensions.PrimitiveTypeReferenceMap.Add(typeof (long), (IEdmPrimitiveTypeReference) EdmLibraryExtensions.Int64TypeReference);
      EdmLibraryExtensions.PrimitiveTypeReferenceMap.Add(typeof (sbyte), (IEdmPrimitiveTypeReference) EdmLibraryExtensions.SByteTypeReference);
      EdmLibraryExtensions.PrimitiveTypeReferenceMap.Add(typeof (string), (IEdmPrimitiveTypeReference) EdmLibraryExtensions.StringTypeReference);
      EdmLibraryExtensions.PrimitiveTypeReferenceMap.Add(typeof (float), (IEdmPrimitiveTypeReference) EdmLibraryExtensions.SingleTypeReference);
      EdmLibraryExtensions.PrimitiveTypeReferenceMap.Add(typeof (DateTimeOffset), (IEdmPrimitiveTypeReference) EdmLibraryExtensions.ToTypeReference(EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.DateTimeOffset), false));
      EdmLibraryExtensions.PrimitiveTypeReferenceMap.Add(typeof (Guid), (IEdmPrimitiveTypeReference) EdmLibraryExtensions.ToTypeReference(EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.Guid), false));
      EdmLibraryExtensions.PrimitiveTypeReferenceMap.Add(typeof (TimeSpan), (IEdmPrimitiveTypeReference) EdmLibraryExtensions.ToTypeReference(EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.Duration), false));
      EdmLibraryExtensions.PrimitiveTypeReferenceMap.Add(typeof (byte[]), (IEdmPrimitiveTypeReference) EdmLibraryExtensions.ToTypeReference(EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.Binary), true));
      EdmLibraryExtensions.PrimitiveTypeReferenceMap.Add(typeof (Stream), (IEdmPrimitiveTypeReference) EdmLibraryExtensions.ToTypeReference(EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.Stream), false));
      EdmLibraryExtensions.PrimitiveTypeReferenceMap.Add(typeof (bool?), (IEdmPrimitiveTypeReference) EdmLibraryExtensions.ToTypeReference(EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.Boolean), true));
      EdmLibraryExtensions.PrimitiveTypeReferenceMap.Add(typeof (byte?), (IEdmPrimitiveTypeReference) EdmLibraryExtensions.ToTypeReference(EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.Byte), true));
      EdmLibraryExtensions.PrimitiveTypeReferenceMap.Add(typeof (DateTimeOffset?), (IEdmPrimitiveTypeReference) EdmLibraryExtensions.ToTypeReference(EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.DateTimeOffset), true));
      EdmLibraryExtensions.PrimitiveTypeReferenceMap.Add(typeof (Decimal?), (IEdmPrimitiveTypeReference) EdmLibraryExtensions.ToTypeReference(EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.Decimal), true));
      EdmLibraryExtensions.PrimitiveTypeReferenceMap.Add(typeof (double?), (IEdmPrimitiveTypeReference) EdmLibraryExtensions.ToTypeReference(EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.Double), true));
      EdmLibraryExtensions.PrimitiveTypeReferenceMap.Add(typeof (short?), (IEdmPrimitiveTypeReference) EdmLibraryExtensions.ToTypeReference(EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.Int16), true));
      EdmLibraryExtensions.PrimitiveTypeReferenceMap.Add(typeof (int?), (IEdmPrimitiveTypeReference) EdmLibraryExtensions.ToTypeReference(EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.Int32), true));
      EdmLibraryExtensions.PrimitiveTypeReferenceMap.Add(typeof (long?), (IEdmPrimitiveTypeReference) EdmLibraryExtensions.ToTypeReference(EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.Int64), true));
      EdmLibraryExtensions.PrimitiveTypeReferenceMap.Add(typeof (sbyte?), (IEdmPrimitiveTypeReference) EdmLibraryExtensions.ToTypeReference(EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.SByte), true));
      EdmLibraryExtensions.PrimitiveTypeReferenceMap.Add(typeof (float?), (IEdmPrimitiveTypeReference) EdmLibraryExtensions.ToTypeReference(EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.Single), true));
      EdmLibraryExtensions.PrimitiveTypeReferenceMap.Add(typeof (Guid?), (IEdmPrimitiveTypeReference) EdmLibraryExtensions.ToTypeReference(EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.Guid), true));
      EdmLibraryExtensions.PrimitiveTypeReferenceMap.Add(typeof (TimeSpan?), (IEdmPrimitiveTypeReference) EdmLibraryExtensions.ToTypeReference(EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.Duration), true));
      EdmLibraryExtensions.PrimitiveTypeReferenceMap.Add(typeof (Date), (IEdmPrimitiveTypeReference) EdmLibraryExtensions.ToTypeReference(EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.Date), false));
      EdmLibraryExtensions.PrimitiveTypeReferenceMap.Add(typeof (Date?), (IEdmPrimitiveTypeReference) EdmLibraryExtensions.ToTypeReference(EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.Date), true));
      EdmLibraryExtensions.PrimitiveTypeReferenceMap.Add(typeof (TimeOfDay), (IEdmPrimitiveTypeReference) EdmLibraryExtensions.ToTypeReference(EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.TimeOfDay), false));
      EdmLibraryExtensions.PrimitiveTypeReferenceMap.Add(typeof (TimeOfDay?), (IEdmPrimitiveTypeReference) EdmLibraryExtensions.ToTypeReference(EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.TimeOfDay), true));
    }

    internal static IEnumerable<IEdmOperationImport> FilterOperationsByParameterNames(
      this IEnumerable<IEdmOperationImport> operationImports,
      IEnumerable<string> parameterNames,
      bool caseInsensitive)
    {
      IList<string> parameterNameList = (IList<string>) parameterNames.ToList<string>();
      foreach (IEdmOperationImport operationImport in operationImports)
      {
        if (EdmLibraryExtensions.ParametersSatisfyFunction(operationImport.Operation, parameterNameList, caseInsensitive))
          yield return operationImport;
      }
    }

    internal static IEnumerable<IEdmOperationImport> FindBestOverloadBasedOnParameters(
      this IEnumerable<IEdmOperationImport> functions,
      IEnumerable<string> parameters,
      bool caseInsensitive = false)
    {
      IEnumerable<IEdmOperationImport> source = functions.Where<IEdmOperationImport>((Func<IEdmOperationImport, bool>) (f => f.Operation.Parameters.Count<IEdmOperationParameter>() == parameters.Count<string>()));
      return source.Count<IEdmOperationImport>() <= 0 ? functions : source;
    }

    internal static IEnumerable<IEdmOperation> FilterBoundOperationsWithSameTypeHierarchyToTypeClosestToBindingType(
      this IEnumerable<IEdmOperation> operations,
      IEdmType bindingType)
    {
      IEdmStructuredType edmStructuredType1 = bindingType as IEdmStructuredType;
      if (bindingType.TypeKind == EdmTypeKind.Collection)
        edmStructuredType1 = ((IEdmCollectionType) bindingType).ElementType.Definition as IEdmStructuredType;
      if (edmStructuredType1 == null)
        return operations;
      Dictionary<IEdmType, List<IEdmOperation>> dictionary = new Dictionary<IEdmType, List<IEdmOperation>>((IEqualityComparer<IEdmType>) new EdmLibraryExtensions.EdmTypeEqualityComparer());
      IEdmType key = (IEdmType) null;
      int num1 = int.MaxValue;
      foreach (IEdmOperation operation in operations)
      {
        if (operation.IsBound)
        {
          IEdmOperationParameter operationParameter = operation.Parameters.FirstOrDefault<IEdmOperationParameter>();
          if (operationParameter != null)
          {
            IEdmType definition = operationParameter.Type.Definition;
            IEdmStructuredType edmStructuredType2 = definition as IEdmStructuredType;
            if (definition.TypeKind == EdmTypeKind.Collection)
              edmStructuredType2 = (definition as IEdmCollectionType).ElementType.Definition as IEdmStructuredType;
            if (edmStructuredType2 != null && edmStructuredType1.IsOrInheritsFrom((IEdmType) edmStructuredType2))
            {
              int num2 = edmStructuredType1.InheritanceLevelFromSpecifiedInheritedType(edmStructuredType2);
              if (num1 > num2)
              {
                num1 = num2;
                key = definition;
              }
              if (!dictionary.ContainsKey(definition))
                dictionary[definition] = new List<IEdmOperation>();
              dictionary[definition].Add(operation);
            }
          }
        }
      }
      return key != null ? (IEnumerable<IEdmOperation>) dictionary[key] : Enumerable.Empty<IEdmOperation>();
    }

    internal static IEnumerable<IEdmOperation> FindBestOverloadBasedOnParameters(
      this IEnumerable<IEdmOperation> functions,
      IEnumerable<string> parameters,
      bool caseInsensitive = false)
    {
      IEnumerable<IEdmOperation> source = functions.Where<IEdmOperation>((Func<IEdmOperation, bool>) (f => f.Parameters.Count<IEdmOperationParameter>() == parameters.Count<string>() + (f.IsBound ? 1 : 0)));
      return source.Count<IEdmOperation>() <= 0 ? functions : source;
    }

    internal static IEnumerable<IEdmOperation> FilterOperationsByParameterNames(
      this IEnumerable<IEdmOperation> operations,
      IEnumerable<string> parameters,
      bool caseInsensitive)
    {
      IList<string> parameterNameList = (IList<string>) parameters.ToList<string>();
      foreach (IEdmOperation operation in operations)
      {
        if (EdmLibraryExtensions.ParametersSatisfyFunction(operation, parameterNameList, caseInsensitive))
          yield return operation;
      }
    }

    internal static void EnsureOperationsBoundWithBindingParameter(
      this IEnumerable<IEdmOperation> operations)
    {
      foreach (IEdmOperation operation in operations)
      {
        if (!operation.IsBound)
          throw new ODataException(Microsoft.OData.Strings.EdmLibraryExtensions_UnBoundOperationsFoundFromIEdmModelFindMethodIsInvalid((object) operation.Name));
        if (operation.Parameters.FirstOrDefault<IEdmOperationParameter>() == null)
          throw new ODataException(Microsoft.OData.Strings.EdmLibraryExtensions_NoParameterBoundOperationsFoundFromIEdmModelFindMethodIsInvalid((object) operation.Name));
      }
    }

    internal static IEnumerable<IEdmOperation> ResolveOperations(
      this IEdmModel model,
      string namespaceQualifiedOperationName)
    {
      return model.ResolveOperations(namespaceQualifiedOperationName, true);
    }

    internal static IEnumerable<IEdmOperation> ResolveOperations(
      this IEdmModel model,
      string operationName,
      bool allowParameterTypeNames)
    {
      if (string.IsNullOrEmpty(operationName))
        return Enumerable.Empty<IEdmOperation>();
      int length = operationName.IndexOf('(');
      string str;
      if (length > 0)
      {
        if (!allowParameterTypeNames)
          return Enumerable.Empty<IEdmOperation>();
        str = operationName.Substring(0, length);
      }
      else
        str = operationName;
      IEnumerable<IEdmOperation> declaredOperations = model.FindDeclaredOperations(str);
      if (declaredOperations == null)
        return Enumerable.Empty<IEdmOperation>();
      return length > 0 ? declaredOperations.Where<IEdmOperation>((Func<IEdmOperation, bool>) (f => f.IsFunction() && f.FullNameWithNonBindingParameters().Equals(operationName, StringComparison.Ordinal) || f.IsAction())) : EdmLibraryExtensions.ValidateOperationGroupReturnsOnlyOnKind(declaredOperations, str);
    }

    internal static string NameWithParameters(this IEdmOperation operation) => operation.Name + operation.ParameterTypesToString();

    internal static string FullNameWithParameters(this IEdmOperation operation) => operation.FullName() + operation.ParameterTypesToString();

    internal static string FullNameWithNonBindingParameters(this IEdmOperation operation) => operation.FullName() + operation.NonBindingParameterNamesToString();

    internal static string NameWithParameters(this IEdmOperationImport operationImport) => operationImport.Name + operationImport.ParameterTypesToString();

    internal static string FullNameWithParameters(this IEdmOperationImport operationImport) => operationImport.FullName() + operationImport.ParameterTypesToString();

    internal static IEnumerable<IEdmOperation> RemoveActions(
      this IEnumerable<IEdmOperation> source,
      out IList<IEdmOperation> actionItems)
    {
      List<IEdmOperation> edmOperationList = new List<IEdmOperation>();
      actionItems = (IList<IEdmOperation>) new List<IEdmOperation>();
      foreach (IEdmOperation operation in source)
      {
        if (operation.IsAction())
          actionItems.Add(operation);
        else
          edmOperationList.Add(operation);
      }
      return (IEnumerable<IEdmOperation>) edmOperationList;
    }

    internal static IEnumerable<IEdmOperationImport> RemoveActionImports(
      this IEnumerable<IEdmOperationImport> source,
      out IList<IEdmOperationImport> actionImportItems)
    {
      List<IEdmOperationImport> edmOperationImportList = new List<IEdmOperationImport>();
      actionImportItems = (IList<IEdmOperationImport>) new List<IEdmOperationImport>();
      foreach (IEdmOperationImport operationImport in source)
      {
        if (operationImport.IsActionImport())
          actionImportItems.Add(operationImport);
        else
          edmOperationImportList.Add(operationImport);
      }
      return (IEnumerable<IEdmOperationImport>) edmOperationImportList;
    }

    internal static bool IsUserModel(this IEdmModel model) => !(model is EdmCoreModel);

    internal static bool IsPrimitiveType(Type clrType) => clrType == typeof (ushort) || clrType == typeof (uint) || clrType == typeof (ulong) || EdmLibraryExtensions.PrimitiveTypeReferenceMap.ContainsKey(clrType) || typeof (ISpatial).IsAssignableFrom(clrType);

    [SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters", Justification = "Extension method for primitive type references only.")]
    internal static IEdmCollectionTypeReference ToCollectionTypeReference(
      this IEdmPrimitiveTypeReference itemTypeReference)
    {
      return (IEdmCollectionTypeReference) new EdmCollectionType((IEdmTypeReference) itemTypeReference).ToTypeReference();
    }

    [SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters", Justification = "Extension method for complex type references only.")]
    internal static IEdmCollectionTypeReference ToCollectionTypeReference(
      this IEdmComplexTypeReference itemTypeReference)
    {
      return (IEdmCollectionTypeReference) new EdmCollectionType((IEdmTypeReference) itemTypeReference).ToTypeReference();
    }

    internal static bool IsAssignableFrom(
      this IEdmTypeReference baseType,
      IEdmTypeReference subtype)
    {
      return baseType.Definition.IsAssignableFrom(subtype.Definition);
    }

    internal static bool IsAssignableFrom(this IEdmType baseType, IEdmType subtype)
    {
      baseType = baseType.AsActualType();
      subtype = subtype.AsActualType();
      EdmTypeKind typeKind1 = baseType.TypeKind;
      EdmTypeKind typeKind2 = subtype.TypeKind;
      if (typeKind1 != typeKind2)
        return false;
      switch (typeKind1)
      {
        case EdmTypeKind.Primitive:
          return EdmLibraryExtensions.IsAssignableFrom((IEdmPrimitiveType) baseType, (IEdmPrimitiveType) subtype);
        case EdmTypeKind.Entity:
        case EdmTypeKind.Complex:
        case EdmTypeKind.Untyped:
          return EdmLibraryExtensions.IsAssignableFrom((IEdmStructuredType) baseType, (IEdmStructuredType) subtype);
        case EdmTypeKind.Collection:
          return ((IEdmCollectionType) baseType).ElementType.Definition.IsAssignableFrom(((IEdmCollectionType) subtype).ElementType.Definition);
        case EdmTypeKind.Enum:
          return baseType.IsEquivalentTo(subtype);
        default:
          throw new ODataException(Microsoft.OData.Strings.General_InternalError((object) InternalErrorCodesCommon.EdmLibraryExtensions_IsAssignableFrom_Type));
      }
    }

    internal static IEdmStructuredType GetCommonBaseType(
      this IEdmStructuredType firstType,
      IEdmStructuredType secondType)
    {
      if (firstType.IsEquivalentTo((IEdmType) secondType))
        return firstType;
      for (IEdmStructuredType baseType = firstType; baseType != null; baseType = baseType.BaseType)
      {
        if (EdmLibraryExtensions.IsAssignableFrom(baseType, secondType))
          return baseType;
      }
      for (IEdmStructuredType baseType = secondType; baseType != null; baseType = baseType.BaseType)
      {
        if (EdmLibraryExtensions.IsAssignableFrom(baseType, firstType))
          return baseType;
      }
      return (IEdmStructuredType) null;
    }

    internal static IEdmPrimitiveType GetCommonBaseType(
      this IEdmPrimitiveType firstType,
      IEdmPrimitiveType secondType)
    {
      if (firstType.IsEquivalentTo((IEdmType) secondType))
        return firstType;
      for (IEdmPrimitiveType commonBaseType = firstType; commonBaseType != null; commonBaseType = commonBaseType.BaseType())
      {
        if (EdmLibraryExtensions.IsAssignableFrom(commonBaseType, secondType))
          return commonBaseType;
      }
      for (IEdmPrimitiveType commonBaseType = secondType; commonBaseType != null; commonBaseType = commonBaseType.BaseType())
      {
        if (EdmLibraryExtensions.IsAssignableFrom(commonBaseType, firstType))
          return commonBaseType;
      }
      return (IEdmPrimitiveType) null;
    }

    internal static IEdmPrimitiveType BaseType(this IEdmPrimitiveType type)
    {
      switch (type.PrimitiveKind)
      {
        case EdmPrimitiveTypeKind.None:
        case EdmPrimitiveTypeKind.Binary:
        case EdmPrimitiveTypeKind.Boolean:
        case EdmPrimitiveTypeKind.Byte:
        case EdmPrimitiveTypeKind.DateTimeOffset:
        case EdmPrimitiveTypeKind.Decimal:
        case EdmPrimitiveTypeKind.Double:
        case EdmPrimitiveTypeKind.Guid:
        case EdmPrimitiveTypeKind.Int16:
        case EdmPrimitiveTypeKind.Int32:
        case EdmPrimitiveTypeKind.Int64:
        case EdmPrimitiveTypeKind.SByte:
        case EdmPrimitiveTypeKind.Single:
        case EdmPrimitiveTypeKind.String:
        case EdmPrimitiveTypeKind.Stream:
        case EdmPrimitiveTypeKind.Duration:
        case EdmPrimitiveTypeKind.Geography:
        case EdmPrimitiveTypeKind.Geometry:
        case EdmPrimitiveTypeKind.Date:
        case EdmPrimitiveTypeKind.TimeOfDay:
          return (IEdmPrimitiveType) null;
        case EdmPrimitiveTypeKind.GeographyPoint:
          return EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.Geography);
        case EdmPrimitiveTypeKind.GeographyLineString:
          return EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.Geography);
        case EdmPrimitiveTypeKind.GeographyPolygon:
          return EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.Geography);
        case EdmPrimitiveTypeKind.GeographyCollection:
          return EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.Geography);
        case EdmPrimitiveTypeKind.GeographyMultiPolygon:
          return EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.GeographyCollection);
        case EdmPrimitiveTypeKind.GeographyMultiLineString:
          return EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.GeographyCollection);
        case EdmPrimitiveTypeKind.GeographyMultiPoint:
          return EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.GeographyCollection);
        case EdmPrimitiveTypeKind.GeometryPoint:
          return EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.Geometry);
        case EdmPrimitiveTypeKind.GeometryLineString:
          return EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.Geometry);
        case EdmPrimitiveTypeKind.GeometryPolygon:
          return EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.Geometry);
        case EdmPrimitiveTypeKind.GeometryCollection:
          return EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.Geometry);
        case EdmPrimitiveTypeKind.GeometryMultiPolygon:
          return EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.GeometryCollection);
        case EdmPrimitiveTypeKind.GeometryMultiLineString:
          return EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.GeometryCollection);
        case EdmPrimitiveTypeKind.GeometryMultiPoint:
          return EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.GeometryCollection);
        default:
          throw new ODataException(Microsoft.OData.Strings.General_InternalError((object) InternalErrorCodesCommon.EdmLibraryExtensions_BaseType));
      }
    }

    internal static IEdmCollectionTypeReference AsCollectionOrNull(
      this IEdmTypeReference typeReference)
    {
      if (typeReference == null)
        return (IEdmCollectionTypeReference) null;
      if (typeReference.TypeKind() != EdmTypeKind.Collection)
        return (IEdmCollectionTypeReference) null;
      IEdmCollectionTypeReference typeReference1 = typeReference.AsCollection();
      return !typeReference1.IsNonEntityCollectionType() ? (IEdmCollectionTypeReference) null : typeReference1;
    }

    internal static bool IsElementTypeEquivalentTo(this IEdmType type, IEdmType other) => type.TypeKind == EdmTypeKind.Collection && other.TypeKind == EdmTypeKind.Collection && ((IEdmCollectionType) type).ElementType.Definition.IsEquivalentTo(((IEdmCollectionType) other).ElementType.Definition);

    internal static object ConvertToUnderlyingTypeIfUIntValue(
      this IEdmModel model,
      object value,
      IEdmTypeReference expectedTypeReference = null)
    {
      if (model == null)
        return value;
      try
      {
        if (expectedTypeReference == null)
          expectedTypeReference = (IEdmTypeReference) model.ResolveUIntTypeDefinition(value);
        return expectedTypeReference != null ? model.GetPrimitiveValueConverter(expectedTypeReference).ConvertToUnderlyingType(value) : value;
      }
      catch (OverflowException ex)
      {
        throw new ODataException(Microsoft.OData.Strings.EdmLibraryExtensions_ValueOverflowForUnderlyingType(value, (object) expectedTypeReference.FullName()));
      }
    }

    internal static IEdmTypeDefinitionReference ResolveUIntTypeDefinition(
      this IEdmModel model,
      object value)
    {
      if (model == null)
        return (IEdmTypeDefinitionReference) null;
      if (value == null)
        return (IEdmTypeDefinitionReference) null;
      if (!(value is ushort) && !(value is uint) && !(value is ulong))
        return (IEdmTypeDefinitionReference) null;
      return model.SchemaElements.SingleOrDefault<IEdmSchemaElement>((Func<IEdmSchemaElement, bool>) (e => string.CompareOrdinal(e.Name, value.GetType().Name) == 0)) is IEdmTypeDefinition typeDefinition ? (IEdmTypeDefinitionReference) new EdmTypeDefinitionReference(typeDefinition, true) : (IEdmTypeDefinitionReference) null;
    }

    internal static IEdmSchemaType ResolvePrimitiveTypeName(string typeName) => EdmCoreModel.Instance.FindDeclaredType(typeName);

    internal static IEdmTypeReference GetCollectionItemType(this IEdmTypeReference typeReference)
    {
      IEdmCollectionTypeReference type = typeReference.AsCollectionOrNull();
      return type != null ? type.ElementType() : (IEdmTypeReference) null;
    }

    internal static IEdmCollectionType GetCollectionType(IEdmType itemType) => EdmLibraryExtensions.GetCollectionType(itemType.ToTypeReference(true));

    internal static IEdmCollectionType GetCollectionType(IEdmTypeReference itemTypeReference) => (IEdmCollectionType) new EdmCollectionType(itemTypeReference);

    internal static string GetCollectionItemTypeName(string typeName) => EdmLibraryExtensions.GetCollectionItemTypeName(typeName, false);

    internal static string GetCollectionTypeFullName(string typeName)
    {
      if (typeName != null)
      {
        string collectionItemTypeName = EdmLibraryExtensions.GetCollectionItemTypeName(typeName);
        if (collectionItemTypeName != null)
        {
          IEdmSchemaType declaredType = EdmCoreModel.Instance.FindDeclaredType(collectionItemTypeName);
          if (declaredType != null)
            return EdmLibraryExtensions.GetCollectionTypeName(declaredType.FullName());
        }
      }
      return typeName;
    }

    internal static bool OperationsBoundToStructuredTypeMustBeContainerQualified(
      this IEdmStructuredType structuredType)
    {
      return structuredType.IsOpen;
    }

    internal static string ODataShortQualifiedName(this IEdmTypeReference typeReference) => typeReference.Definition.ODataShortQualifiedName();

    internal static string ODataShortQualifiedName(this IEdmType type)
    {
      switch (type)
      {
        case IEdmCollectionType edmCollectionType:
          string itemTypeName = edmCollectionType.ElementType.ODataShortQualifiedName();
          return itemTypeName == null ? (string) null : EdmLibraryExtensions.GetCollectionTypeName(itemTypeName);
        case IEdmSchemaElement element:
          return element.ShortQualifiedName();
        default:
          return (string) null;
      }
    }

    [SuppressMessage("Microsoft.Maintainability", "CA1506:AvoidExcessiveClassCoupling", Justification = "The clone logic should stay in one place.")]
    internal static IEdmTypeReference Clone(this IEdmTypeReference typeReference, bool nullable)
    {
      if (typeReference == null)
        return (IEdmTypeReference) null;
      switch (typeReference.TypeKind())
      {
        case EdmTypeKind.Primitive:
          EdmPrimitiveTypeKind primitiveTypeKind = typeReference.PrimitiveKind();
          IEdmPrimitiveType definition = (IEdmPrimitiveType) typeReference.Definition;
          switch (primitiveTypeKind)
          {
            case EdmPrimitiveTypeKind.Binary:
              IEdmBinaryTypeReference binaryTypeReference = (IEdmBinaryTypeReference) typeReference;
              return (IEdmTypeReference) new EdmBinaryTypeReference(definition, nullable, binaryTypeReference.IsUnbounded, binaryTypeReference.MaxLength);
            case EdmPrimitiveTypeKind.Boolean:
            case EdmPrimitiveTypeKind.Byte:
            case EdmPrimitiveTypeKind.Double:
            case EdmPrimitiveTypeKind.Guid:
            case EdmPrimitiveTypeKind.Int16:
            case EdmPrimitiveTypeKind.Int32:
            case EdmPrimitiveTypeKind.Int64:
            case EdmPrimitiveTypeKind.SByte:
            case EdmPrimitiveTypeKind.Single:
            case EdmPrimitiveTypeKind.Stream:
            case EdmPrimitiveTypeKind.Date:
            case EdmPrimitiveTypeKind.PrimitiveType:
              return (IEdmTypeReference) new EdmPrimitiveTypeReference(definition, nullable);
            case EdmPrimitiveTypeKind.DateTimeOffset:
            case EdmPrimitiveTypeKind.Duration:
            case EdmPrimitiveTypeKind.TimeOfDay:
              IEdmTemporalTypeReference temporalTypeReference = (IEdmTemporalTypeReference) typeReference;
              return (IEdmTypeReference) new EdmTemporalTypeReference(definition, nullable, temporalTypeReference.Precision);
            case EdmPrimitiveTypeKind.Decimal:
              IEdmDecimalTypeReference decimalTypeReference = (IEdmDecimalTypeReference) typeReference;
              return (IEdmTypeReference) new EdmDecimalTypeReference(definition, nullable, decimalTypeReference.Precision, decimalTypeReference.Scale);
            case EdmPrimitiveTypeKind.String:
              IEdmStringTypeReference stringTypeReference = (IEdmStringTypeReference) typeReference;
              return (IEdmTypeReference) new EdmStringTypeReference(definition, nullable, stringTypeReference.IsUnbounded, stringTypeReference.MaxLength, stringTypeReference.IsUnicode);
            case EdmPrimitiveTypeKind.Geography:
            case EdmPrimitiveTypeKind.GeographyPoint:
            case EdmPrimitiveTypeKind.GeographyLineString:
            case EdmPrimitiveTypeKind.GeographyPolygon:
            case EdmPrimitiveTypeKind.GeographyCollection:
            case EdmPrimitiveTypeKind.GeographyMultiPolygon:
            case EdmPrimitiveTypeKind.GeographyMultiLineString:
            case EdmPrimitiveTypeKind.GeographyMultiPoint:
            case EdmPrimitiveTypeKind.Geometry:
            case EdmPrimitiveTypeKind.GeometryPoint:
            case EdmPrimitiveTypeKind.GeometryLineString:
            case EdmPrimitiveTypeKind.GeometryPolygon:
            case EdmPrimitiveTypeKind.GeometryCollection:
            case EdmPrimitiveTypeKind.GeometryMultiPolygon:
            case EdmPrimitiveTypeKind.GeometryMultiLineString:
            case EdmPrimitiveTypeKind.GeometryMultiPoint:
              IEdmSpatialTypeReference spatialTypeReference = (IEdmSpatialTypeReference) typeReference;
              return (IEdmTypeReference) new EdmSpatialTypeReference(definition, nullable, spatialTypeReference.SpatialReferenceIdentifier);
            default:
              throw new ODataException(Microsoft.OData.Strings.General_InternalError((object) InternalErrorCodesCommon.EdmLibraryExtensions_Clone_PrimitiveTypeKind));
          }
        case EdmTypeKind.Entity:
          return (IEdmTypeReference) new EdmEntityTypeReference((IEdmEntityType) typeReference.Definition, nullable);
        case EdmTypeKind.Complex:
          return (IEdmTypeReference) new EdmComplexTypeReference((IEdmComplexType) typeReference.Definition, nullable);
        case EdmTypeKind.Collection:
          return (IEdmTypeReference) new EdmCollectionTypeReference((IEdmCollectionType) typeReference.Definition);
        case EdmTypeKind.EntityReference:
          return (IEdmTypeReference) new EdmEntityReferenceTypeReference((IEdmEntityReferenceType) typeReference.Definition, nullable);
        case EdmTypeKind.Enum:
          return (IEdmTypeReference) new EdmEnumTypeReference((IEdmEnumType) typeReference.Definition, nullable);
        default:
          throw new ODataException(Microsoft.OData.Strings.General_InternalError((object) InternalErrorCodesCommon.EdmLibraryExtensions_Clone_TypeKind));
      }
    }

    internal static string OperationGroupFullName(this IEnumerable<IEdmOperation> operationGroup) => operationGroup.First<IEdmOperation>().FullName();

    internal static string OperationImportGroupFullName(
      this IEnumerable<IEdmOperationImport> operationImportGroup)
    {
      return operationImportGroup.First<IEdmOperationImport>().FullName();
    }

    [SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters", Justification = "Extension method for structured types only.")]
    internal static bool IsAssignableFrom(
      this IEdmStructuredType baseType,
      IEdmStructuredType subtype)
    {
      if (baseType.TypeKind == EdmTypeKind.Untyped)
        return true;
      if (baseType.TypeKind != subtype.TypeKind)
        return false;
      if (subtype.IsEquivalentTo((IEdmType) baseType) || baseType == EdmCoreModel.Instance.GetComplexType() || baseType == EdmCoreModel.Instance.GetEntityType())
        return true;
      if (!baseType.IsODataEntityTypeKind() && !baseType.IsODataComplexTypeKind())
        return false;
      for (IEdmStructuredType thisType = subtype; thisType != null; thisType = thisType.BaseType)
      {
        if (thisType.IsEquivalentTo((IEdmType) baseType))
          return true;
      }
      return false;
    }

    [SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity", Justification = "Need to keep code together.")]
    internal static bool IsAssignableFrom(
      this IEdmPrimitiveType baseType,
      IEdmPrimitiveType subtype)
    {
      if (baseType.IsEquivalentTo((IEdmType) subtype) || baseType.PrimitiveKind == EdmPrimitiveTypeKind.PrimitiveType)
        return true;
      if (!baseType.IsSpatial() || !subtype.IsSpatial())
        return false;
      EdmPrimitiveTypeKind primitiveKind1 = baseType.PrimitiveKind;
      EdmPrimitiveTypeKind primitiveKind2 = subtype.PrimitiveKind;
      switch (primitiveKind1)
      {
        case EdmPrimitiveTypeKind.Geography:
          return primitiveKind2 == EdmPrimitiveTypeKind.Geography || primitiveKind2 == EdmPrimitiveTypeKind.GeographyCollection || primitiveKind2 == EdmPrimitiveTypeKind.GeographyLineString || primitiveKind2 == EdmPrimitiveTypeKind.GeographyMultiLineString || primitiveKind2 == EdmPrimitiveTypeKind.GeographyMultiPoint || primitiveKind2 == EdmPrimitiveTypeKind.GeographyMultiPolygon || primitiveKind2 == EdmPrimitiveTypeKind.GeographyPoint || primitiveKind2 == EdmPrimitiveTypeKind.GeographyPolygon;
        case EdmPrimitiveTypeKind.GeographyPoint:
          return primitiveKind2 == EdmPrimitiveTypeKind.GeographyPoint;
        case EdmPrimitiveTypeKind.GeographyLineString:
          return primitiveKind2 == EdmPrimitiveTypeKind.GeographyLineString;
        case EdmPrimitiveTypeKind.GeographyPolygon:
          return primitiveKind2 == EdmPrimitiveTypeKind.GeographyPolygon;
        case EdmPrimitiveTypeKind.GeographyCollection:
          return primitiveKind2 == EdmPrimitiveTypeKind.GeographyCollection || primitiveKind2 == EdmPrimitiveTypeKind.GeographyMultiLineString || primitiveKind2 == EdmPrimitiveTypeKind.GeographyMultiPoint || primitiveKind2 == EdmPrimitiveTypeKind.GeographyMultiPolygon;
        case EdmPrimitiveTypeKind.GeographyMultiPolygon:
          return primitiveKind2 == EdmPrimitiveTypeKind.GeographyMultiPolygon;
        case EdmPrimitiveTypeKind.GeographyMultiLineString:
          return primitiveKind2 == EdmPrimitiveTypeKind.GeographyMultiLineString;
        case EdmPrimitiveTypeKind.GeographyMultiPoint:
          return primitiveKind2 == EdmPrimitiveTypeKind.GeographyMultiPoint;
        case EdmPrimitiveTypeKind.Geometry:
          return primitiveKind2 == EdmPrimitiveTypeKind.Geometry || primitiveKind2 == EdmPrimitiveTypeKind.GeometryCollection || primitiveKind2 == EdmPrimitiveTypeKind.GeometryLineString || primitiveKind2 == EdmPrimitiveTypeKind.GeometryMultiLineString || primitiveKind2 == EdmPrimitiveTypeKind.GeometryMultiPoint || primitiveKind2 == EdmPrimitiveTypeKind.GeometryMultiPolygon || primitiveKind2 == EdmPrimitiveTypeKind.GeometryPoint || primitiveKind2 == EdmPrimitiveTypeKind.GeometryPolygon;
        case EdmPrimitiveTypeKind.GeometryPoint:
          return primitiveKind2 == EdmPrimitiveTypeKind.GeometryPoint;
        case EdmPrimitiveTypeKind.GeometryLineString:
          return primitiveKind2 == EdmPrimitiveTypeKind.GeometryLineString;
        case EdmPrimitiveTypeKind.GeometryPolygon:
          return primitiveKind2 == EdmPrimitiveTypeKind.GeometryPolygon;
        case EdmPrimitiveTypeKind.GeometryCollection:
          return primitiveKind2 == EdmPrimitiveTypeKind.GeometryCollection || primitiveKind2 == EdmPrimitiveTypeKind.GeometryMultiLineString || primitiveKind2 == EdmPrimitiveTypeKind.GeometryMultiPoint || primitiveKind2 == EdmPrimitiveTypeKind.GeometryMultiPolygon;
        case EdmPrimitiveTypeKind.GeometryMultiPolygon:
          return primitiveKind2 == EdmPrimitiveTypeKind.GeometryMultiPolygon;
        case EdmPrimitiveTypeKind.GeometryMultiLineString:
          return primitiveKind2 == EdmPrimitiveTypeKind.GeometryMultiLineString;
        case EdmPrimitiveTypeKind.GeometryMultiPoint:
          return primitiveKind2 == EdmPrimitiveTypeKind.GeometryMultiPoint;
        default:
          throw new ODataException(Microsoft.OData.Strings.General_InternalError((object) InternalErrorCodesCommon.EdmLibraryExtensions_IsAssignableFrom_Primitive));
      }
    }

    internal static Type GetPrimitiveClrType(IEdmPrimitiveTypeReference primitiveTypeReference) => EdmLibraryExtensions.GetPrimitiveClrType(primitiveTypeReference.PrimitiveDefinition(), primitiveTypeReference.IsNullable);

    internal static IEdmTypeReference ToTypeReference(this IEdmType type) => type.ToTypeReference(false);

    internal static string FullName(this IEdmEntityContainerElement containerElement) => containerElement.Container.Name + "." + containerElement.Name;

    [SuppressMessage("Microsoft.Maintainability", "CA1502", Justification = "cyclomatic complexity")]
    internal static IEdmPrimitiveTypeReference GetPrimitiveTypeReference(Type clrType)
    {
      IEdmPrimitiveTypeReference primitiveTypeReference;
      if (EdmLibraryExtensions.PrimitiveTypeReferenceMap.TryGetValue(clrType, out primitiveTypeReference))
        return primitiveTypeReference;
      IEdmPrimitiveType primitiveType = (IEdmPrimitiveType) null;
      if (typeof (GeographyPoint).IsAssignableFrom(clrType))
        primitiveType = EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.GeographyPoint);
      else if (typeof (GeographyLineString).IsAssignableFrom(clrType))
        primitiveType = EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.GeographyLineString);
      else if (typeof (GeographyPolygon).IsAssignableFrom(clrType))
        primitiveType = EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.GeographyPolygon);
      else if (typeof (GeographyMultiPoint).IsAssignableFrom(clrType))
        primitiveType = EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.GeographyMultiPoint);
      else if (typeof (GeographyMultiLineString).IsAssignableFrom(clrType))
        primitiveType = EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.GeographyMultiLineString);
      else if (typeof (GeographyMultiPolygon).IsAssignableFrom(clrType))
        primitiveType = EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.GeographyMultiPolygon);
      else if (typeof (GeographyCollection).IsAssignableFrom(clrType))
        primitiveType = EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.GeographyCollection);
      else if (typeof (Geography).IsAssignableFrom(clrType))
        primitiveType = EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.Geography);
      else if (typeof (GeometryPoint).IsAssignableFrom(clrType))
        primitiveType = EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.GeometryPoint);
      else if (typeof (GeometryLineString).IsAssignableFrom(clrType))
        primitiveType = EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.GeometryLineString);
      else if (typeof (GeometryPolygon).IsAssignableFrom(clrType))
        primitiveType = EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.GeometryPolygon);
      else if (typeof (GeometryMultiPoint).IsAssignableFrom(clrType))
        primitiveType = EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.GeometryMultiPoint);
      else if (typeof (GeometryMultiLineString).IsAssignableFrom(clrType))
        primitiveType = EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.GeometryMultiLineString);
      else if (typeof (GeometryMultiPolygon).IsAssignableFrom(clrType))
        primitiveType = EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.GeometryMultiPolygon);
      else if (typeof (GeometryCollection).IsAssignableFrom(clrType))
        primitiveType = EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.GeometryCollection);
      else if (typeof (Geometry).IsAssignableFrom(clrType))
        primitiveType = EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.Geometry);
      return primitiveType == null ? (IEdmPrimitiveTypeReference) null : (IEdmPrimitiveTypeReference) EdmLibraryExtensions.ToTypeReference(primitiveType, true);
    }

    internal static IEdmTypeReference ToTypeReference(this IEdmType type, bool nullable)
    {
      if (type == null)
        return (IEdmTypeReference) null;
      switch (type.TypeKind)
      {
        case EdmTypeKind.Primitive:
          return (IEdmTypeReference) EdmLibraryExtensions.ToTypeReference((IEdmPrimitiveType) type, nullable);
        case EdmTypeKind.Entity:
          return (IEdmTypeReference) new EdmEntityTypeReference((IEdmEntityType) type, nullable);
        case EdmTypeKind.Complex:
          return (IEdmTypeReference) new EdmComplexTypeReference((IEdmComplexType) type, nullable);
        case EdmTypeKind.Collection:
          return (IEdmTypeReference) new EdmCollectionTypeReference((IEdmCollectionType) type);
        case EdmTypeKind.EntityReference:
          return (IEdmTypeReference) new EdmEntityReferenceTypeReference((IEdmEntityReferenceType) type, nullable);
        case EdmTypeKind.Enum:
          return (IEdmTypeReference) new EdmEnumTypeReference((IEdmEnumType) type, nullable);
        case EdmTypeKind.TypeDefinition:
          return (IEdmTypeReference) new EdmTypeDefinitionReference((IEdmTypeDefinition) type, nullable);
        case EdmTypeKind.Untyped:
          return type is IEdmStructuredType definition ? (IEdmTypeReference) new EdmUntypedStructuredTypeReference(definition) : (IEdmTypeReference) new EdmUntypedTypeReference((IEdmUntypedType) type);
        default:
          throw new ODataException(Microsoft.OData.Strings.General_InternalError((object) InternalErrorCodesCommon.EdmLibraryExtensions_ToTypeReference));
      }
    }

    internal static string GetCollectionTypeName(string itemTypeName) => string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Collection({0})", new object[1]
    {
      (object) itemTypeName
    });

    internal static IEnumerable<IEdmOperationImport> ResolveOperationImports(
      this IEdmEntityContainer container,
      string operationImportName)
    {
      return container.ResolveOperationImports(operationImportName, true);
    }

    [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "allowParameterTypeNames", Justification = "Used in the ODL version of the method.")]
    internal static IEnumerable<IEdmOperationImport> ResolveOperationImports(
      this IEdmEntityContainer container,
      string operationImportName,
      bool allowParameterTypeNames)
    {
      if (string.IsNullOrEmpty(operationImportName))
        return Enumerable.Empty<IEdmOperationImport>();
      int length = operationImportName.IndexOf('(');
      string operationNameWithoutParameterTypes = operationImportName;
      if (length > 0)
      {
        if (!allowParameterTypeNames)
          return Enumerable.Empty<IEdmOperationImport>();
        operationNameWithoutParameterTypes = operationImportName.Substring(0, length);
      }
      string str = (string) null;
      string operationName = operationNameWithoutParameterTypes;
      int num = operationNameWithoutParameterTypes.LastIndexOf('.');
      if (num > -1)
      {
        operationName = operationNameWithoutParameterTypes.Substring(num, operationNameWithoutParameterTypes.Length - num).TrimStart('.');
        str = operationNameWithoutParameterTypes.Substring(0, num);
      }
      if (str != null && !container.Name.Equals(str) && !container.FullName().Equals(str))
        return Enumerable.Empty<IEdmOperationImport>();
      IEnumerable<IEdmOperationImport> operationImports = container.FindOperationImports(operationName);
      return length > 0 ? operationImports.FilterByOperationParameterTypes(operationNameWithoutParameterTypes, operationImportName) : operationImports;
    }

    [SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity", Justification = "Not too complex for what this method does.")]
    [SuppressMessage("Microsoft.Maintainability", "CA1506:AvoidExcessiveClassCoupling", Justification = "Class coupling is with all the primitive Clr types only.")]
    internal static Type GetPrimitiveClrType(IEdmPrimitiveType primitiveType, bool isNullable)
    {
      switch (primitiveType.PrimitiveKind)
      {
        case EdmPrimitiveTypeKind.Binary:
          return typeof (byte[]);
        case EdmPrimitiveTypeKind.Boolean:
          return !isNullable ? typeof (bool) : typeof (bool?);
        case EdmPrimitiveTypeKind.Byte:
          return !isNullable ? typeof (byte) : typeof (byte?);
        case EdmPrimitiveTypeKind.DateTimeOffset:
          return !isNullable ? typeof (DateTimeOffset) : typeof (DateTimeOffset?);
        case EdmPrimitiveTypeKind.Decimal:
          return !isNullable ? typeof (Decimal) : typeof (Decimal?);
        case EdmPrimitiveTypeKind.Double:
          return !isNullable ? typeof (double) : typeof (double?);
        case EdmPrimitiveTypeKind.Guid:
          return !isNullable ? typeof (Guid) : typeof (Guid?);
        case EdmPrimitiveTypeKind.Int16:
          return !isNullable ? typeof (short) : typeof (short?);
        case EdmPrimitiveTypeKind.Int32:
          return !isNullable ? typeof (int) : typeof (int?);
        case EdmPrimitiveTypeKind.Int64:
          return !isNullable ? typeof (long) : typeof (long?);
        case EdmPrimitiveTypeKind.SByte:
          return !isNullable ? typeof (sbyte) : typeof (sbyte?);
        case EdmPrimitiveTypeKind.Single:
          return !isNullable ? typeof (float) : typeof (float?);
        case EdmPrimitiveTypeKind.String:
          return typeof (string);
        case EdmPrimitiveTypeKind.Stream:
          return typeof (Stream);
        case EdmPrimitiveTypeKind.Duration:
          return !isNullable ? typeof (TimeSpan) : typeof (TimeSpan?);
        case EdmPrimitiveTypeKind.Geography:
          return typeof (Geography);
        case EdmPrimitiveTypeKind.GeographyPoint:
          return typeof (GeographyPoint);
        case EdmPrimitiveTypeKind.GeographyLineString:
          return typeof (GeographyLineString);
        case EdmPrimitiveTypeKind.GeographyPolygon:
          return typeof (GeographyPolygon);
        case EdmPrimitiveTypeKind.GeographyCollection:
          return typeof (GeographyCollection);
        case EdmPrimitiveTypeKind.GeographyMultiPolygon:
          return typeof (GeographyMultiPolygon);
        case EdmPrimitiveTypeKind.GeographyMultiLineString:
          return typeof (GeographyMultiLineString);
        case EdmPrimitiveTypeKind.GeographyMultiPoint:
          return typeof (GeographyMultiPoint);
        case EdmPrimitiveTypeKind.Geometry:
          return typeof (Geometry);
        case EdmPrimitiveTypeKind.GeometryPoint:
          return typeof (GeometryPoint);
        case EdmPrimitiveTypeKind.GeometryLineString:
          return typeof (GeometryLineString);
        case EdmPrimitiveTypeKind.GeometryPolygon:
          return typeof (GeometryPolygon);
        case EdmPrimitiveTypeKind.GeometryCollection:
          return typeof (GeometryCollection);
        case EdmPrimitiveTypeKind.GeometryMultiPolygon:
          return typeof (GeometryMultiPolygon);
        case EdmPrimitiveTypeKind.GeometryMultiLineString:
          return typeof (GeometryMultiLineString);
        case EdmPrimitiveTypeKind.GeometryMultiPoint:
          return typeof (GeometryMultiPoint);
        case EdmPrimitiveTypeKind.Date:
          return !isNullable ? typeof (Date) : typeof (Date?);
        case EdmPrimitiveTypeKind.TimeOfDay:
          return !isNullable ? typeof (TimeOfDay) : typeof (TimeOfDay?);
        default:
          return (Type) null;
      }
    }

    private static IEnumerable<IEdmOperation> ValidateOperationGroupReturnsOnlyOnKind(
      IEnumerable<IEdmOperation> operations,
      string operationNameWithoutParameterTypes)
    {
      EdmSchemaElementKind? operationKind = new EdmSchemaElementKind?();
      foreach (IEdmOperation operation in operations)
      {
        if (!operationKind.HasValue)
        {
          operationKind = new EdmSchemaElementKind?(operation.SchemaElementKind);
        }
        else
        {
          int schemaElementKind = (int) operation.SchemaElementKind;
          EdmSchemaElementKind? nullable = operationKind;
          int valueOrDefault = (int) nullable.GetValueOrDefault();
          if (!(schemaElementKind == valueOrDefault & nullable.HasValue))
            throw new ODataException(Microsoft.OData.Strings.EdmLibraryExtensions_OperationGroupReturningActionsAndFunctionsModelInvalid((object) operationNameWithoutParameterTypes));
        }
        yield return operation;
      }
    }

    private static string ParameterTypesToString(this IEdmOperation operation) => "(" + string.Join(",", operation.Parameters.Select<IEdmOperationParameter, string>((Func<IEdmOperationParameter, string>) (p => p.Type.FullName())).ToArray<string>()) + ")";

    private static string NonBindingParameterNamesToString(this IEdmOperation operation) => "(" + string.Join(",", (operation.IsBound ? operation.Parameters.Skip<IEdmOperationParameter>(1) : operation.Parameters).Select<IEdmOperationParameter, string>((Func<IEdmOperationParameter, string>) (p => p.Name)).ToArray<string>()) + ")";

    private static string GetCollectionItemTypeName(string typeName, bool isNested)
    {
      int length = "Collection".Length;
      if (typeName == null || !typeName.StartsWith("Collection(", StringComparison.Ordinal) || typeName[typeName.Length - 1] != ')' || typeName.Length == length + 2)
        return (string) null;
      if (isNested)
        throw new ODataException(Microsoft.OData.Strings.ValidationUtils_NestedCollectionsAreNotSupported);
      string typeName1 = typeName.Substring(length + 1, typeName.Length - (length + 2));
      EdmLibraryExtensions.GetCollectionItemTypeName(typeName1, true);
      return typeName1;
    }

    private static string ParameterTypesToString(this IEdmOperationImport operationImport) => "(" + string.Join(",", operationImport.Operation.Parameters.Select<IEdmOperationParameter, string>((Func<IEdmOperationParameter, string>) (p => p.Type.FullName())).ToArray<string>()) + ")";

    private static IEnumerable<IEdmOperationImport> FilterByOperationParameterTypes(
      this IEnumerable<IEdmOperationImport> operationImports,
      string operationNameWithoutParameterTypes,
      string originalFullOperationImportName)
    {
      foreach (IEdmOperationImport operationImport in operationImports)
      {
        if (operationNameWithoutParameterTypes.IndexOf(".", StringComparison.Ordinal) > -1)
        {
          if (operationImport.FullNameWithParameters().Equals(originalFullOperationImportName, StringComparison.Ordinal) || (operationImport.Container.Name + "." + operationImport.NameWithParameters()).Equals(originalFullOperationImportName, StringComparison.Ordinal))
            yield return operationImport;
        }
        else if (operationImport.NameWithParameters().Equals(originalFullOperationImportName, StringComparison.Ordinal))
          yield return operationImport;
      }
    }

    private static bool ParametersSatisfyFunction(
      IEdmOperation operation,
      IList<string> parameterNameList,
      bool caseInsensitive)
    {
      IEnumerable<IEdmOperationParameter> source = operation.Parameters;
      if (operation.IsBound)
        source = source.Skip<IEdmOperationParameter>(1);
      List<IEdmOperationParameter> functionParameters = source.ToList<IEdmOperationParameter>();
      return !functionParameters.Where<IEdmOperationParameter>((Func<IEdmOperationParameter, bool>) (p => !(p is IEdmOptionalParameter))).Any<IEdmOperationParameter>((Func<IEdmOperationParameter, bool>) (p => parameterNameList.All<string>((Func<string, bool>) (k => !string.Equals(k, p.Name, caseInsensitive ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal))))) && !parameterNameList.Any<string>((Func<string, bool>) (k => functionParameters.All<IEdmOperationParameter>((Func<IEdmOperationParameter, bool>) (p => !string.Equals(k, p.Name, caseInsensitive ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal)))));
    }

    private static int InheritanceLevelFromSpecifiedInheritedType(
      this IEdmStructuredType structuredType,
      IEdmStructuredType rootType)
    {
      IEdmStructuredType type = structuredType;
      int num = 0;
      while (type.InheritsFrom(rootType))
      {
        type = type.BaseType;
        ++num;
      }
      return num;
    }

    private static EdmPrimitiveTypeReference ToTypeReference(
      IEdmPrimitiveType primitiveType,
      bool nullable)
    {
      switch (primitiveType.PrimitiveKind)
      {
        case EdmPrimitiveTypeKind.Binary:
          return (EdmPrimitiveTypeReference) new EdmBinaryTypeReference(primitiveType, nullable);
        case EdmPrimitiveTypeKind.Boolean:
        case EdmPrimitiveTypeKind.Byte:
        case EdmPrimitiveTypeKind.Double:
        case EdmPrimitiveTypeKind.Guid:
        case EdmPrimitiveTypeKind.Int16:
        case EdmPrimitiveTypeKind.Int32:
        case EdmPrimitiveTypeKind.Int64:
        case EdmPrimitiveTypeKind.SByte:
        case EdmPrimitiveTypeKind.Single:
        case EdmPrimitiveTypeKind.Stream:
        case EdmPrimitiveTypeKind.Date:
        case EdmPrimitiveTypeKind.PrimitiveType:
          return new EdmPrimitiveTypeReference(primitiveType, nullable);
        case EdmPrimitiveTypeKind.DateTimeOffset:
        case EdmPrimitiveTypeKind.Duration:
        case EdmPrimitiveTypeKind.TimeOfDay:
          return (EdmPrimitiveTypeReference) new EdmTemporalTypeReference(primitiveType, nullable);
        case EdmPrimitiveTypeKind.Decimal:
          return (EdmPrimitiveTypeReference) new EdmDecimalTypeReference(primitiveType, nullable);
        case EdmPrimitiveTypeKind.String:
          return (EdmPrimitiveTypeReference) new EdmStringTypeReference(primitiveType, nullable);
        case EdmPrimitiveTypeKind.Geography:
        case EdmPrimitiveTypeKind.GeographyPoint:
        case EdmPrimitiveTypeKind.GeographyLineString:
        case EdmPrimitiveTypeKind.GeographyPolygon:
        case EdmPrimitiveTypeKind.GeographyCollection:
        case EdmPrimitiveTypeKind.GeographyMultiPolygon:
        case EdmPrimitiveTypeKind.GeographyMultiLineString:
        case EdmPrimitiveTypeKind.GeographyMultiPoint:
        case EdmPrimitiveTypeKind.Geometry:
        case EdmPrimitiveTypeKind.GeometryPoint:
        case EdmPrimitiveTypeKind.GeometryLineString:
        case EdmPrimitiveTypeKind.GeometryPolygon:
        case EdmPrimitiveTypeKind.GeometryCollection:
        case EdmPrimitiveTypeKind.GeometryMultiPolygon:
        case EdmPrimitiveTypeKind.GeometryMultiLineString:
        case EdmPrimitiveTypeKind.GeometryMultiPoint:
          return (EdmPrimitiveTypeReference) new EdmSpatialTypeReference(primitiveType, nullable);
        default:
          throw new ODataException(Microsoft.OData.Strings.General_InternalError((object) InternalErrorCodesCommon.EdmLibraryExtensions_PrimitiveTypeReference));
      }
    }

    private sealed class EdmTypeEqualityComparer : IEqualityComparer<IEdmType>
    {
      public bool Equals(IEdmType x, IEdmType y) => x.IsEquivalentTo(y);

      public int GetHashCode(IEdmType obj) => obj.GetHashCode();
    }
  }
}
