// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.Edm.EdmCoreModel
// Assembly: Microsoft.TeamFoundation.OData.Edm, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 75C95BF5-2544-413A-959F-F9F18C11D35F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Edm.dll

using Microsoft.OData.Edm.Vocabularies;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.OData.Edm
{
  public class EdmCoreModel : EdmElement, IEdmModel, IEdmElement, IEdmCoreModelElement
  {
    public static readonly EdmCoreModel Instance = new EdmCoreModel();
    private readonly IDictionary<string, EdmPrimitiveTypeKind> primitiveTypeKinds = (IDictionary<string, EdmPrimitiveTypeKind>) new Dictionary<string, EdmPrimitiveTypeKind>();
    private readonly IDictionary<EdmPrimitiveTypeKind, EdmCoreModelPrimitiveType> primitiveTypesByKind = (IDictionary<EdmPrimitiveTypeKind, EdmCoreModelPrimitiveType>) new Dictionary<EdmPrimitiveTypeKind, EdmCoreModelPrimitiveType>();
    private readonly IDictionary<string, EdmPathTypeKind> pathTypeKinds = (IDictionary<string, EdmPathTypeKind>) new Dictionary<string, EdmPathTypeKind>();
    private readonly IDictionary<EdmPathTypeKind, EdmCoreModelPathType> pathTypesByKind = (IDictionary<EdmPathTypeKind, EdmCoreModelPathType>) new Dictionary<EdmPathTypeKind, EdmCoreModelPathType>();
    private readonly IEdmDirectValueAnnotationsManager annotationsManager = (IEdmDirectValueAnnotationsManager) new EdmDirectValueAnnotationsManager();
    private readonly IList<IEdmSchemaElement> coreSchemaElements = (IList<IEdmSchemaElement>) new List<IEdmSchemaElement>();
    private readonly IDictionary<string, IEdmSchemaType> coreSchemaTypes = (IDictionary<string, IEdmSchemaType>) new Dictionary<string, IEdmSchemaType>();
    private readonly EdmCoreModelComplexType complexType = EdmCoreModelComplexType.Instance;
    private readonly EdmCoreModelEntityType entityType = EdmCoreModelEntityType.Instance;
    private readonly EdmCoreModelUntypedType untypedType = EdmCoreModelUntypedType.Instance;
    private readonly EdmCoreModelPrimitiveType primitiveType = new EdmCoreModelPrimitiveType(EdmPrimitiveTypeKind.PrimitiveType);

    private EdmCoreModel()
    {
      foreach (EdmCoreModelPrimitiveType modelPrimitiveType in (IEnumerable<EdmCoreModelPrimitiveType>) new List<EdmCoreModelPrimitiveType>()
      {
        new EdmCoreModelPrimitiveType(EdmPrimitiveTypeKind.Double),
        new EdmCoreModelPrimitiveType(EdmPrimitiveTypeKind.Single),
        new EdmCoreModelPrimitiveType(EdmPrimitiveTypeKind.Int64),
        new EdmCoreModelPrimitiveType(EdmPrimitiveTypeKind.Int32),
        new EdmCoreModelPrimitiveType(EdmPrimitiveTypeKind.Int16),
        new EdmCoreModelPrimitiveType(EdmPrimitiveTypeKind.SByte),
        new EdmCoreModelPrimitiveType(EdmPrimitiveTypeKind.Byte),
        new EdmCoreModelPrimitiveType(EdmPrimitiveTypeKind.Boolean),
        new EdmCoreModelPrimitiveType(EdmPrimitiveTypeKind.Guid),
        new EdmCoreModelPrimitiveType(EdmPrimitiveTypeKind.Duration),
        new EdmCoreModelPrimitiveType(EdmPrimitiveTypeKind.TimeOfDay),
        new EdmCoreModelPrimitiveType(EdmPrimitiveTypeKind.DateTimeOffset),
        new EdmCoreModelPrimitiveType(EdmPrimitiveTypeKind.Date),
        new EdmCoreModelPrimitiveType(EdmPrimitiveTypeKind.Decimal),
        new EdmCoreModelPrimitiveType(EdmPrimitiveTypeKind.Binary),
        new EdmCoreModelPrimitiveType(EdmPrimitiveTypeKind.String),
        new EdmCoreModelPrimitiveType(EdmPrimitiveTypeKind.Stream),
        new EdmCoreModelPrimitiveType(EdmPrimitiveTypeKind.Geography),
        new EdmCoreModelPrimitiveType(EdmPrimitiveTypeKind.GeographyPoint),
        new EdmCoreModelPrimitiveType(EdmPrimitiveTypeKind.GeographyLineString),
        new EdmCoreModelPrimitiveType(EdmPrimitiveTypeKind.GeographyPolygon),
        new EdmCoreModelPrimitiveType(EdmPrimitiveTypeKind.GeographyCollection),
        new EdmCoreModelPrimitiveType(EdmPrimitiveTypeKind.GeographyMultiPolygon),
        new EdmCoreModelPrimitiveType(EdmPrimitiveTypeKind.GeographyMultiLineString),
        new EdmCoreModelPrimitiveType(EdmPrimitiveTypeKind.GeographyMultiPoint),
        new EdmCoreModelPrimitiveType(EdmPrimitiveTypeKind.Geometry),
        new EdmCoreModelPrimitiveType(EdmPrimitiveTypeKind.GeometryPoint),
        new EdmCoreModelPrimitiveType(EdmPrimitiveTypeKind.GeometryLineString),
        new EdmCoreModelPrimitiveType(EdmPrimitiveTypeKind.GeometryPolygon),
        new EdmCoreModelPrimitiveType(EdmPrimitiveTypeKind.GeometryCollection),
        new EdmCoreModelPrimitiveType(EdmPrimitiveTypeKind.GeometryMultiPolygon),
        new EdmCoreModelPrimitiveType(EdmPrimitiveTypeKind.GeometryMultiLineString),
        new EdmCoreModelPrimitiveType(EdmPrimitiveTypeKind.GeometryMultiPoint),
        this.primitiveType
      })
      {
        this.primitiveTypeKinds[modelPrimitiveType.Name] = modelPrimitiveType.PrimitiveKind;
        this.primitiveTypeKinds[modelPrimitiveType.Namespace + "." + modelPrimitiveType.Name] = modelPrimitiveType.PrimitiveKind;
        this.primitiveTypesByKind[modelPrimitiveType.PrimitiveKind] = modelPrimitiveType;
        this.coreSchemaTypes[modelPrimitiveType.Namespace + "." + modelPrimitiveType.Name] = (IEdmSchemaType) modelPrimitiveType;
        this.coreSchemaTypes[modelPrimitiveType.Name] = (IEdmSchemaType) modelPrimitiveType;
        this.coreSchemaElements.Add((IEdmSchemaElement) modelPrimitiveType);
      }
      this.coreSchemaElements.Add((IEdmSchemaElement) this.complexType);
      this.coreSchemaTypes[this.complexType.Namespace + "." + this.complexType.Name] = (IEdmSchemaType) this.complexType;
      this.coreSchemaTypes[this.complexType.Name] = (IEdmSchemaType) this.complexType;
      this.coreSchemaElements.Add((IEdmSchemaElement) this.entityType);
      this.coreSchemaTypes[this.entityType.Namespace + "." + this.entityType.Name] = (IEdmSchemaType) this.entityType;
      this.coreSchemaTypes[this.entityType.Name] = (IEdmSchemaType) this.entityType;
      this.coreSchemaElements.Add((IEdmSchemaElement) this.untypedType);
      this.coreSchemaTypes[this.untypedType.Namespace + "." + this.untypedType.Name] = (IEdmSchemaType) this.untypedType;
      this.coreSchemaTypes[this.untypedType.Name] = (IEdmSchemaType) this.untypedType;
      EdmCoreModelPathType[] coreModelPathTypeArray = new EdmCoreModelPathType[3]
      {
        new EdmCoreModelPathType(EdmPathTypeKind.AnnotationPath),
        new EdmCoreModelPathType(EdmPathTypeKind.PropertyPath),
        new EdmCoreModelPathType(EdmPathTypeKind.NavigationPropertyPath)
      };
      foreach (EdmCoreModelPathType coreModelPathType in coreModelPathTypeArray)
      {
        this.pathTypeKinds[coreModelPathType.Name] = coreModelPathType.PathKind;
        this.pathTypeKinds[coreModelPathType.Namespace + "." + coreModelPathType.Name] = coreModelPathType.PathKind;
        this.pathTypesByKind[coreModelPathType.PathKind] = coreModelPathType;
        this.coreSchemaTypes[coreModelPathType.Namespace + "." + coreModelPathType.Name] = (IEdmSchemaType) coreModelPathType;
        this.coreSchemaTypes[coreModelPathType.Name] = (IEdmSchemaType) coreModelPathType;
        this.coreSchemaElements.Add((IEdmSchemaElement) coreModelPathType);
      }
    }

    public static string Namespace => "Edm";

    public IEnumerable<IEdmSchemaElement> SchemaElements => (IEnumerable<IEdmSchemaElement>) this.coreSchemaElements;

    public IEnumerable<string> DeclaredNamespaces => Enumerable.Empty<string>();

    public IEnumerable<IEdmVocabularyAnnotation> VocabularyAnnotations => Enumerable.Empty<IEdmVocabularyAnnotation>();

    public IEnumerable<IEdmModel> ReferencedModels => Enumerable.Empty<IEdmModel>();

    public IEdmDirectValueAnnotationsManager DirectValueAnnotationsManager => this.annotationsManager;

    public IEdmEntityContainer EntityContainer => (IEdmEntityContainer) null;

    public static IEdmCollectionTypeReference GetCollection(IEdmTypeReference elementType) => (IEdmCollectionTypeReference) new EdmCollectionTypeReference((IEdmCollectionType) new EdmCollectionType(elementType));

    public IEdmSchemaType FindDeclaredType(string qualifiedName)
    {
      IEdmSchemaType edmSchemaType;
      return this.coreSchemaTypes.TryGetValue(qualifiedName, out edmSchemaType) ? edmSchemaType : (IEdmSchemaType) null;
    }

    public IEnumerable<IEdmOperation> FindDeclaredBoundOperations(IEdmType bindingType) => Enumerable.Empty<IEdmOperation>();

    public IEnumerable<IEdmOperation> FindDeclaredBoundOperations(
      string qualifiedName,
      IEdmType bindingType)
    {
      return Enumerable.Empty<IEdmOperation>();
    }

    public IEdmTerm FindDeclaredTerm(string qualifiedName) => (IEdmTerm) null;

    public IEnumerable<IEdmOperation> FindDeclaredOperations(string qualifiedName) => Enumerable.Empty<IEdmOperation>();

    public IEnumerable<IEdmOperationImport> FindOperationImportsByNameNonBindingParameterType(
      string operationImportName,
      IEnumerable<string> parameterNames)
    {
      return Enumerable.Empty<IEdmOperationImport>();
    }

    public IEdmPrimitiveType GetPrimitiveType(EdmPrimitiveTypeKind kind) => (IEdmPrimitiveType) this.GetCoreModelPrimitiveType(kind);

    public IEdmPrimitiveType GetPrimitiveType() => (IEdmPrimitiveType) this.primitiveType;

    public IEdmComplexType GetComplexType() => (IEdmComplexType) this.complexType;

    public IEdmEntityType GetEntityType() => (IEdmEntityType) this.entityType;

    public IEdmUntypedType GetUntypedType() => (IEdmUntypedType) this.untypedType;

    public IEdmPathType GetPathType(EdmPathTypeKind kind)
    {
      EdmCoreModelPathType coreModelPathType;
      return !this.pathTypesByKind.TryGetValue(kind, out coreModelPathType) ? (IEdmPathType) null : (IEdmPathType) coreModelPathType;
    }

    public EdmPrimitiveTypeKind GetPrimitiveTypeKind(string typeName)
    {
      EdmPrimitiveTypeKind primitiveTypeKind;
      return !this.primitiveTypeKinds.TryGetValue(typeName, out primitiveTypeKind) ? EdmPrimitiveTypeKind.None : primitiveTypeKind;
    }

    public IEdmPrimitiveTypeReference GetPrimitive(EdmPrimitiveTypeKind kind, bool isNullable) => ((IEdmPrimitiveType) this.GetCoreModelPrimitiveType(kind) ?? throw new InvalidOperationException(Strings.EdmPrimitive_UnexpectedKind)).GetPrimitiveTypeReference(isNullable);

    public EdmPathTypeKind GetPathTypeKind(string typeName)
    {
      EdmPathTypeKind edmPathTypeKind;
      return !this.pathTypeKinds.TryGetValue(typeName, out edmPathTypeKind) ? EdmPathTypeKind.None : edmPathTypeKind;
    }

    public IEdmPathTypeReference GetPathType(EdmPathTypeKind kind, bool isNullable) => (IEdmPathTypeReference) new EdmPathTypeReference(this.GetPathType(kind) ?? throw new InvalidOperationException(Strings.EdmPath_UnexpectedKind), isNullable);

    public IEdmPathTypeReference GetAnnotationPath(bool isNullable) => (IEdmPathTypeReference) new EdmPathTypeReference(this.GetPathType(EdmPathTypeKind.AnnotationPath), isNullable);

    public IEdmPathTypeReference GetPropertyPath(bool isNullable) => (IEdmPathTypeReference) new EdmPathTypeReference(this.GetPathType(EdmPathTypeKind.PropertyPath), isNullable);

    public IEdmPathTypeReference GetNavigationPropertyPath(bool isNullable) => (IEdmPathTypeReference) new EdmPathTypeReference(this.GetPathType(EdmPathTypeKind.NavigationPropertyPath), isNullable);

    public IEdmEntityTypeReference GetEntityType(bool isNullable) => (IEdmEntityTypeReference) new EdmEntityTypeReference((IEdmEntityType) this.entityType, isNullable);

    public IEdmComplexTypeReference GetComplexType(bool isNullable) => (IEdmComplexTypeReference) new EdmComplexTypeReference((IEdmComplexType) this.complexType, isNullable);

    public IEdmPrimitiveTypeReference GetPrimitiveType(bool isNullable) => (IEdmPrimitiveTypeReference) new EdmPrimitiveTypeReference((IEdmPrimitiveType) this.primitiveType, isNullable);

    public IEdmPrimitiveTypeReference GetInt16(bool isNullable) => (IEdmPrimitiveTypeReference) new EdmPrimitiveTypeReference((IEdmPrimitiveType) this.GetCoreModelPrimitiveType(EdmPrimitiveTypeKind.Int16), isNullable);

    public IEdmPrimitiveTypeReference GetInt32(bool isNullable) => (IEdmPrimitiveTypeReference) new EdmPrimitiveTypeReference((IEdmPrimitiveType) this.GetCoreModelPrimitiveType(EdmPrimitiveTypeKind.Int32), isNullable);

    public IEdmPrimitiveTypeReference GetInt64(bool isNullable) => (IEdmPrimitiveTypeReference) new EdmPrimitiveTypeReference((IEdmPrimitiveType) this.GetCoreModelPrimitiveType(EdmPrimitiveTypeKind.Int64), isNullable);

    public IEdmPrimitiveTypeReference GetBoolean(bool isNullable) => (IEdmPrimitiveTypeReference) new EdmPrimitiveTypeReference((IEdmPrimitiveType) this.GetCoreModelPrimitiveType(EdmPrimitiveTypeKind.Boolean), isNullable);

    public IEdmPrimitiveTypeReference GetByte(bool isNullable) => (IEdmPrimitiveTypeReference) new EdmPrimitiveTypeReference((IEdmPrimitiveType) this.GetCoreModelPrimitiveType(EdmPrimitiveTypeKind.Byte), isNullable);

    public IEdmPrimitiveTypeReference GetSByte(bool isNullable) => (IEdmPrimitiveTypeReference) new EdmPrimitiveTypeReference((IEdmPrimitiveType) this.GetCoreModelPrimitiveType(EdmPrimitiveTypeKind.SByte), isNullable);

    public IEdmPrimitiveTypeReference GetGuid(bool isNullable) => (IEdmPrimitiveTypeReference) new EdmPrimitiveTypeReference((IEdmPrimitiveType) this.GetCoreModelPrimitiveType(EdmPrimitiveTypeKind.Guid), isNullable);

    public IEdmPrimitiveTypeReference GetDate(bool isNullable) => (IEdmPrimitiveTypeReference) new EdmPrimitiveTypeReference((IEdmPrimitiveType) this.GetCoreModelPrimitiveType(EdmPrimitiveTypeKind.Date), isNullable);

    public IEdmTemporalTypeReference GetDateTimeOffset(bool isNullable) => (IEdmTemporalTypeReference) new EdmTemporalTypeReference((IEdmPrimitiveType) this.GetCoreModelPrimitiveType(EdmPrimitiveTypeKind.DateTimeOffset), isNullable);

    public IEdmTemporalTypeReference GetDuration(bool isNullable) => (IEdmTemporalTypeReference) new EdmTemporalTypeReference((IEdmPrimitiveType) this.GetCoreModelPrimitiveType(EdmPrimitiveTypeKind.Duration), isNullable);

    public IEdmTemporalTypeReference GetTimeOfDay(bool isNullable) => (IEdmTemporalTypeReference) new EdmTemporalTypeReference((IEdmPrimitiveType) this.GetCoreModelPrimitiveType(EdmPrimitiveTypeKind.TimeOfDay), isNullable);

    public IEdmDecimalTypeReference GetDecimal(int? precision, int? scale, bool isNullable) => precision.HasValue || scale.HasValue ? (IEdmDecimalTypeReference) new EdmDecimalTypeReference((IEdmPrimitiveType) this.GetCoreModelPrimitiveType(EdmPrimitiveTypeKind.Decimal), isNullable, precision, scale) : (IEdmDecimalTypeReference) new EdmDecimalTypeReference((IEdmPrimitiveType) this.GetCoreModelPrimitiveType(EdmPrimitiveTypeKind.Decimal), isNullable);

    public IEdmDecimalTypeReference GetDecimal(bool isNullable) => (IEdmDecimalTypeReference) new EdmDecimalTypeReference((IEdmPrimitiveType) this.GetCoreModelPrimitiveType(EdmPrimitiveTypeKind.Decimal), isNullable);

    public IEdmPrimitiveTypeReference GetSingle(bool isNullable) => (IEdmPrimitiveTypeReference) new EdmPrimitiveTypeReference((IEdmPrimitiveType) this.GetCoreModelPrimitiveType(EdmPrimitiveTypeKind.Single), isNullable);

    public IEdmPrimitiveTypeReference GetDouble(bool isNullable) => (IEdmPrimitiveTypeReference) new EdmPrimitiveTypeReference((IEdmPrimitiveType) this.GetCoreModelPrimitiveType(EdmPrimitiveTypeKind.Double), isNullable);

    public IEdmPrimitiveTypeReference GetStream(bool isNullable) => (IEdmPrimitiveTypeReference) new EdmPrimitiveTypeReference((IEdmPrimitiveType) this.GetCoreModelPrimitiveType(EdmPrimitiveTypeKind.Stream), isNullable);

    public IEdmTemporalTypeReference GetTemporal(
      EdmPrimitiveTypeKind kind,
      int? precision,
      bool isNullable)
    {
      if (kind == EdmPrimitiveTypeKind.DateTimeOffset || kind == EdmPrimitiveTypeKind.Duration || kind == EdmPrimitiveTypeKind.TimeOfDay)
        return (IEdmTemporalTypeReference) new EdmTemporalTypeReference((IEdmPrimitiveType) this.GetCoreModelPrimitiveType(kind), isNullable, precision);
      throw new InvalidOperationException(Strings.EdmPrimitive_UnexpectedKind);
    }

    public IEdmTemporalTypeReference GetTemporal(EdmPrimitiveTypeKind kind, bool isNullable) => kind == EdmPrimitiveTypeKind.DateTimeOffset || kind == EdmPrimitiveTypeKind.Duration || kind == EdmPrimitiveTypeKind.TimeOfDay ? (IEdmTemporalTypeReference) new EdmTemporalTypeReference((IEdmPrimitiveType) this.GetCoreModelPrimitiveType(kind), isNullable) : throw new InvalidOperationException(Strings.EdmPrimitive_UnexpectedKind);

    public IEdmBinaryTypeReference GetBinary(bool isUnbounded, int? maxLength, bool isNullable) => (IEdmBinaryTypeReference) new EdmBinaryTypeReference((IEdmPrimitiveType) this.GetCoreModelPrimitiveType(EdmPrimitiveTypeKind.Binary), isNullable, isUnbounded, maxLength);

    public IEdmBinaryTypeReference GetBinary(bool isNullable) => (IEdmBinaryTypeReference) new EdmBinaryTypeReference((IEdmPrimitiveType) this.GetCoreModelPrimitiveType(EdmPrimitiveTypeKind.Binary), isNullable);

    public IEdmSpatialTypeReference GetSpatial(
      EdmPrimitiveTypeKind kind,
      int? spatialReferenceIdentifier,
      bool isNullable)
    {
      if ((uint) (kind - 16) <= 15U)
        return (IEdmSpatialTypeReference) new EdmSpatialTypeReference((IEdmPrimitiveType) this.GetCoreModelPrimitiveType(kind), isNullable, spatialReferenceIdentifier);
      throw new InvalidOperationException(Strings.EdmPrimitive_UnexpectedKind);
    }

    public IEdmSpatialTypeReference GetSpatial(EdmPrimitiveTypeKind kind, bool isNullable) => (uint) (kind - 16) <= 15U ? (IEdmSpatialTypeReference) new EdmSpatialTypeReference((IEdmPrimitiveType) this.GetCoreModelPrimitiveType(kind), isNullable) : throw new InvalidOperationException(Strings.EdmPrimitive_UnexpectedKind);

    public IEdmStringTypeReference GetString(
      bool isUnbounded,
      int? maxLength,
      bool? isUnicode,
      bool isNullable)
    {
      return (IEdmStringTypeReference) new EdmStringTypeReference((IEdmPrimitiveType) this.GetCoreModelPrimitiveType(EdmPrimitiveTypeKind.String), isNullable, isUnbounded, maxLength, isUnicode);
    }

    public IEdmStringTypeReference GetString(bool isNullable) => (IEdmStringTypeReference) new EdmStringTypeReference((IEdmPrimitiveType) this.GetCoreModelPrimitiveType(EdmPrimitiveTypeKind.String), isNullable);

    public IEdmUntypedTypeReference GetUntyped() => (IEdmUntypedTypeReference) new EdmUntypedTypeReference(this.GetUntypedType());

    public IEnumerable<IEdmVocabularyAnnotation> FindDeclaredVocabularyAnnotations(
      IEdmVocabularyAnnotatable element)
    {
      return Enumerable.Empty<IEdmVocabularyAnnotation>();
    }

    public IEnumerable<IEdmStructuredType> FindDirectlyDerivedTypes(IEdmStructuredType baseType) => Enumerable.Empty<IEdmStructuredType>();

    private EdmCoreModelPrimitiveType GetCoreModelPrimitiveType(EdmPrimitiveTypeKind kind)
    {
      EdmCoreModelPrimitiveType modelPrimitiveType;
      return !this.primitiveTypesByKind.TryGetValue(kind, out modelPrimitiveType) ? (EdmCoreModelPrimitiveType) null : modelPrimitiveType;
    }
  }
}
