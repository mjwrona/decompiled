// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.Edm.Csdl.Serialization.EdmModelCsdlSchemaWriter
// Assembly: Microsoft.TeamFoundation.OData.Edm, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 75C95BF5-2544-413A-959F-F9F18C11D35F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Edm.dll

using Microsoft.OData.Edm.Csdl.CsdlSemantics;
using Microsoft.OData.Edm.Csdl.Parsing.Ast;
using Microsoft.OData.Edm.Vocabularies;
using Microsoft.OData.Edm.Vocabularies.V1;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Xml;

namespace Microsoft.OData.Edm.Csdl.Serialization
{
  internal class EdmModelCsdlSchemaWriter
  {
    protected XmlWriter xmlWriter;
    protected Version version;
    private readonly string edmxNamespace;
    private readonly VersioningDictionary<string, string> namespaceAliasMappings;
    private readonly IEdmModel model;

    internal EdmModelCsdlSchemaWriter(
      IEdmModel model,
      VersioningDictionary<string, string> namespaceAliasMappings,
      XmlWriter xmlWriter,
      Version edmVersion)
    {
      this.xmlWriter = xmlWriter;
      this.version = edmVersion;
      this.edmxNamespace = CsdlConstants.SupportedEdmxVersions[edmVersion];
      this.model = model;
      this.namespaceAliasMappings = namespaceAliasMappings;
    }

    internal static string PathAsXml(IEnumerable<string> path) => EdmUtil.JoinInternal<string>("/", path);

    internal void WriteReferenceElementHeader(IEdmReference reference)
    {
      this.xmlWriter.WriteStartElement("edmx", "Reference", this.edmxNamespace);
      this.WriteRequiredAttribute<Uri>("Uri", reference.Uri, new Func<Uri, string>(EdmValueWriter.UriAsXml));
    }

    internal void WriteIncludeElement(IEdmInclude include)
    {
      this.xmlWriter.WriteStartElement("edmx", "Include", this.edmxNamespace);
      this.WriteRequiredAttribute<string>("Namespace", include.Namespace, new Func<string, string>(EdmValueWriter.StringAsXml));
      this.WriteRequiredAttribute<string>("Alias", include.Alias, new Func<string, string>(EdmValueWriter.StringAsXml));
      this.xmlWriter.WriteEndElement();
    }

    internal void WriteIncludeAnnotationsElement(IEdmIncludeAnnotations includeAnnotations)
    {
      this.xmlWriter.WriteStartElement("edmx", "IncludeAnnotations", this.edmxNamespace);
      this.WriteRequiredAttribute<string>("TermNamespace", includeAnnotations.TermNamespace, new Func<string, string>(EdmValueWriter.StringAsXml));
      this.WriteOptionalAttribute<string>("Qualifier", includeAnnotations.Qualifier, new Func<string, string>(EdmValueWriter.StringAsXml));
      this.WriteOptionalAttribute<string>("TargetNamespace", includeAnnotations.TargetNamespace, new Func<string, string>(EdmValueWriter.StringAsXml));
      this.xmlWriter.WriteEndElement();
    }

    internal void WriteTermElementHeader(IEdmTerm term, bool inlineType)
    {
      this.xmlWriter.WriteStartElement("Term");
      this.WriteRequiredAttribute<string>("Name", term.Name, new Func<string, string>(EdmValueWriter.StringAsXml));
      if (inlineType && term.Type != null)
        this.WriteRequiredAttribute<IEdmTypeReference>("Type", term.Type, new Func<IEdmTypeReference, string>(this.TypeReferenceAsXml));
      this.WriteOptionalAttribute<string>("DefaultValue", term.DefaultValue, new Func<string, string>(EdmValueWriter.StringAsXml));
      this.WriteOptionalAttribute<string>("AppliesTo", term.AppliesTo, new Func<string, string>(EdmValueWriter.StringAsXml));
    }

    internal void WriteComplexTypeElementHeader(IEdmComplexType complexType)
    {
      this.xmlWriter.WriteStartElement("ComplexType");
      this.WriteRequiredAttribute<string>("Name", complexType.Name, new Func<string, string>(EdmValueWriter.StringAsXml));
      this.WriteOptionalAttribute<IEdmComplexType>("BaseType", complexType.BaseComplexType(), new Func<IEdmComplexType, string>(this.TypeDefinitionAsXml));
      this.WriteOptionalAttribute<bool>("Abstract", complexType.IsAbstract, false, new Func<bool, string>(EdmValueWriter.BooleanAsXml));
      this.WriteOptionalAttribute<bool>("OpenType", complexType.IsOpen, false, new Func<bool, string>(EdmValueWriter.BooleanAsXml));
    }

    internal void WriteEnumTypeElementHeader(IEdmEnumType enumType)
    {
      this.xmlWriter.WriteStartElement("EnumType");
      this.WriteRequiredAttribute<string>("Name", enumType.Name, new Func<string, string>(EdmValueWriter.StringAsXml));
      if (enumType.UnderlyingType.PrimitiveKind != EdmPrimitiveTypeKind.Int32)
        this.WriteRequiredAttribute<IEdmPrimitiveType>("UnderlyingType", enumType.UnderlyingType, new Func<IEdmPrimitiveType, string>(this.TypeDefinitionAsXml));
      this.WriteOptionalAttribute<bool>("IsFlags", enumType.IsFlags, false, new Func<bool, string>(EdmValueWriter.BooleanAsXml));
    }

    internal void WriteEntityContainerElementHeader(IEdmEntityContainer container)
    {
      this.xmlWriter.WriteStartElement("EntityContainer");
      this.WriteRequiredAttribute<string>("Name", container.Name, new Func<string, string>(EdmValueWriter.StringAsXml));
      CsdlSemanticsEntityContainer semanticsEntityContainer = container as CsdlSemanticsEntityContainer;
      if (semanticsEntityContainer == null || !(semanticsEntityContainer.Element is CsdlEntityContainer element))
        return;
      this.WriteOptionalAttribute<string>("Extends", element.Extends, new Func<string, string>(EdmValueWriter.StringAsXml));
    }

    internal void WriteEntitySetElementHeader(IEdmEntitySet entitySet)
    {
      this.xmlWriter.WriteStartElement("EntitySet");
      this.WriteRequiredAttribute<string>("Name", entitySet.Name, new Func<string, string>(EdmValueWriter.StringAsXml));
      this.WriteRequiredAttribute<string>("EntityType", entitySet.EntityType().FullName(), new Func<string, string>(EdmValueWriter.StringAsXml));
    }

    internal void WriteSingletonElementHeader(IEdmSingleton singleton)
    {
      this.xmlWriter.WriteStartElement("Singleton");
      this.WriteRequiredAttribute<string>("Name", singleton.Name, new Func<string, string>(EdmValueWriter.StringAsXml));
      this.WriteRequiredAttribute<string>("Type", singleton.EntityType().FullName(), new Func<string, string>(EdmValueWriter.StringAsXml));
    }

    internal void WriteNavigationPropertyBinding(
      IEdmNavigationSource navigationSource,
      IEdmNavigationPropertyBinding binding)
    {
      this.WriteNavigationPropertyBinding(binding);
    }

    internal void WriteEntityTypeElementHeader(IEdmEntityType entityType)
    {
      this.xmlWriter.WriteStartElement("EntityType");
      this.WriteRequiredAttribute<string>("Name", entityType.Name, new Func<string, string>(EdmValueWriter.StringAsXml));
      this.WriteOptionalAttribute<IEdmEntityType>("BaseType", entityType.BaseEntityType(), new Func<IEdmEntityType, string>(this.TypeDefinitionAsXml));
      this.WriteOptionalAttribute<bool>("Abstract", entityType.IsAbstract, false, new Func<bool, string>(EdmValueWriter.BooleanAsXml));
      this.WriteOptionalAttribute<bool>("OpenType", entityType.IsOpen, false, new Func<bool, string>(EdmValueWriter.BooleanAsXml));
      this.WriteOptionalAttribute<bool>("HasStream", entityType.HasStream && (entityType.BaseEntityType() == null || entityType.BaseEntityType() != null && !entityType.BaseEntityType().HasStream), false, new Func<bool, string>(EdmValueWriter.BooleanAsXml));
    }

    internal void WriteDeclaredKeyPropertiesElementHeader() => this.xmlWriter.WriteStartElement("Key");

    internal void WritePropertyRefElement(IEdmStructuralProperty property)
    {
      this.xmlWriter.WriteStartElement("PropertyRef");
      this.WriteRequiredAttribute<string>("Name", property.Name, new Func<string, string>(EdmValueWriter.StringAsXml));
      this.WriteEndElement();
    }

    internal void WriteNavigationPropertyElementHeader(IEdmNavigationProperty member)
    {
      this.xmlWriter.WriteStartElement("NavigationProperty");
      this.WriteRequiredAttribute<string>("Name", member.Name, new Func<string, string>(EdmValueWriter.StringAsXml));
      this.WriteRequiredAttribute<IEdmTypeReference>("Type", member.Type, new Func<IEdmTypeReference, string>(this.TypeReferenceAsXml));
      if (!member.Type.IsCollection() && !member.Type.IsNullable)
        this.WriteRequiredAttribute<bool>("Nullable", member.Type.IsNullable, new Func<bool, string>(EdmValueWriter.BooleanAsXml));
      if (member.Partner != null)
        this.WriteRequiredAttribute<string>("Partner", member.GetPartnerPath().Path, new Func<string, string>(EdmValueWriter.StringAsXml));
      this.WriteOptionalAttribute<bool>("ContainsTarget", member.ContainsTarget, false, new Func<bool, string>(EdmValueWriter.BooleanAsXml));
    }

    internal void WriteOperationActionElement(string elementName, EdmOnDeleteAction operationAction)
    {
      this.xmlWriter.WriteStartElement(elementName);
      this.WriteRequiredAttribute<string>("Action", operationAction.ToString(), new Func<string, string>(EdmValueWriter.StringAsXml));
      this.WriteEndElement();
    }

    internal void WriteSchemaElementHeader(
      EdmSchema schema,
      string alias,
      IEnumerable<KeyValuePair<string, string>> mappings)
    {
      this.xmlWriter.WriteStartElement("Schema", EdmModelCsdlSchemaWriter.GetCsdlNamespace(this.version));
      this.WriteOptionalAttribute<string>("Namespace", schema.Namespace, string.Empty, new Func<string, string>(EdmValueWriter.StringAsXml));
      this.WriteOptionalAttribute<string>("Alias", alias, new Func<string, string>(EdmValueWriter.StringAsXml));
      if (mappings == null)
        return;
      foreach (KeyValuePair<string, string> mapping in mappings)
        this.xmlWriter.WriteAttributeString("xmlns", mapping.Key, (string) null, mapping.Value);
    }

    internal void WriteAnnotationsElementHeader(string annotationsTarget)
    {
      this.xmlWriter.WriteStartElement("Annotations");
      this.WriteRequiredAttribute<string>("Target", annotationsTarget, new Func<string, string>(EdmValueWriter.StringAsXml));
    }

    internal void WriteStructuralPropertyElementHeader(
      IEdmStructuralProperty property,
      bool inlineType)
    {
      this.xmlWriter.WriteStartElement("Property");
      this.WriteRequiredAttribute<string>("Name", property.Name, new Func<string, string>(EdmValueWriter.StringAsXml));
      if (inlineType)
        this.WriteRequiredAttribute<IEdmTypeReference>("Type", property.Type, new Func<IEdmTypeReference, string>(this.TypeReferenceAsXml));
      this.WriteOptionalAttribute<string>("DefaultValue", property.DefaultValueString, new Func<string, string>(EdmValueWriter.StringAsXml));
    }

    internal void WriteEnumMemberElementHeader(IEdmEnumMember member)
    {
      this.xmlWriter.WriteStartElement("Member");
      this.WriteRequiredAttribute<string>("Name", member.Name, new Func<string, string>(EdmValueWriter.StringAsXml));
      bool? nullable = member.IsValueExplicit(this.model);
      if (nullable.HasValue && !nullable.Value)
        return;
      this.xmlWriter.WriteAttributeString("Value", EdmValueWriter.LongAsXml(member.Value.Value));
    }

    internal void WriteNullableAttribute(IEdmTypeReference reference) => this.WriteOptionalAttribute<bool>("Nullable", reference.IsNullable, true, new Func<bool, string>(EdmValueWriter.BooleanAsXml));

    internal void WriteTypeDefinitionAttributes(IEdmTypeDefinitionReference reference)
    {
      IEdmTypeReference type = reference.AsActualTypeReference();
      if (type.IsBinary())
        this.WriteBinaryTypeAttributes(type.AsBinary());
      else if (type.IsString())
        this.WriteStringTypeAttributes(type.AsString());
      else if (type.IsTemporal())
        this.WriteTemporalTypeAttributes(type.AsTemporal());
      else if (type.IsDecimal())
      {
        this.WriteDecimalTypeAttributes(type.AsDecimal());
      }
      else
      {
        if (!type.IsSpatial())
          return;
        this.WriteSpatialTypeAttributes(type.AsSpatial());
      }
    }

    internal void WriteBinaryTypeAttributes(IEdmBinaryTypeReference reference)
    {
      if (reference.IsUnbounded)
        this.WriteRequiredAttribute<string>("MaxLength", "max", new Func<string, string>(EdmValueWriter.StringAsXml));
      else
        this.WriteOptionalAttribute<int?>("MaxLength", reference.MaxLength, new Func<int?, string>(EdmValueWriter.IntAsXml));
    }

    internal void WriteDecimalTypeAttributes(IEdmDecimalTypeReference reference)
    {
      this.WriteOptionalAttribute<int?>("Precision", reference.Precision, new Func<int?, string>(EdmValueWriter.IntAsXml));
      this.WriteOptionalAttribute<int?>("Scale", reference.Scale, new int?(0), new Func<int?, string>(EdmModelCsdlSchemaWriter.ScaleAsXml));
    }

    internal void WriteSpatialTypeAttributes(IEdmSpatialTypeReference reference)
    {
      if (reference.IsGeography())
      {
        this.WriteOptionalAttribute<int?>("SRID", reference.SpatialReferenceIdentifier, new int?(4326), new Func<int?, string>(EdmModelCsdlSchemaWriter.SridAsXml));
      }
      else
      {
        if (!reference.IsGeometry())
          return;
        this.WriteOptionalAttribute<int?>("SRID", reference.SpatialReferenceIdentifier, new int?(0), new Func<int?, string>(EdmModelCsdlSchemaWriter.SridAsXml));
      }
    }

    internal void WriteStringTypeAttributes(IEdmStringTypeReference reference)
    {
      if (reference.IsUnbounded)
        this.WriteRequiredAttribute<string>("MaxLength", "max", new Func<string, string>(EdmValueWriter.StringAsXml));
      else
        this.WriteOptionalAttribute<int?>("MaxLength", reference.MaxLength, new Func<int?, string>(EdmValueWriter.IntAsXml));
      if (!reference.IsUnicode.HasValue)
        return;
      this.WriteOptionalAttribute<bool?>("Unicode", reference.IsUnicode, new bool?(true), new Func<bool?, string>(EdmValueWriter.BooleanAsXml));
    }

    internal void WriteTemporalTypeAttributes(IEdmTemporalTypeReference reference)
    {
      if (!reference.Precision.HasValue)
        return;
      this.WriteOptionalAttribute<int?>("Precision", reference.Precision, new int?(0), new Func<int?, string>(EdmValueWriter.IntAsXml));
    }

    internal void WriteReferentialConstraintElementHeader(IEdmNavigationProperty constraint) => this.xmlWriter.WriteStartElement("ReferentialConstraint");

    internal void WriteReferentialConstraintPair(EdmReferentialConstraintPropertyPair pair)
    {
      this.xmlWriter.WriteStartElement("ReferentialConstraint");
      this.WriteRequiredAttribute<string>("Property", pair.DependentProperty.Name, new Func<string, string>(EdmValueWriter.StringAsXml));
      this.WriteRequiredAttribute<string>("ReferencedProperty", pair.PrincipalProperty.Name, new Func<string, string>(EdmValueWriter.StringAsXml));
      this.WriteEndElement();
    }

    internal void WriteAnnotationStringAttribute(IEdmDirectValueAnnotation annotation)
    {
      IEdmPrimitiveValue v = (IEdmPrimitiveValue) annotation.Value;
      if (v == null)
        return;
      this.xmlWriter.WriteAttributeString(annotation.Name, annotation.NamespaceUri, EdmValueWriter.PrimitiveValueAsXml(v));
    }

    internal void WriteAnnotationStringElement(IEdmDirectValueAnnotation annotation)
    {
      IEdmPrimitiveValue edmPrimitiveValue = (IEdmPrimitiveValue) annotation.Value;
      if (edmPrimitiveValue == null)
        return;
      this.xmlWriter.WriteRaw(((IEdmStringValue) edmPrimitiveValue).Value);
    }

    internal void WriteActionElementHeader(IEdmAction action)
    {
      this.xmlWriter.WriteStartElement("Action");
      this.WriteOperationElementAttributes((IEdmOperation) action);
    }

    internal void WriteFunctionElementHeader(IEdmFunction function)
    {
      this.xmlWriter.WriteStartElement("Function");
      this.WriteOperationElementAttributes((IEdmOperation) function);
      if (!function.IsComposable)
        return;
      this.WriteOptionalAttribute<bool>("IsComposable", function.IsComposable, new Func<bool, string>(EdmValueWriter.BooleanAsXml));
    }

    internal void WriteReturnTypeElementHeader() => this.xmlWriter.WriteStartElement("ReturnType");

    internal void WriteTypeAttribute(IEdmTypeReference typeReference) => this.WriteRequiredAttribute<IEdmTypeReference>("Type", typeReference, new Func<IEdmTypeReference, string>(this.TypeReferenceAsXml));

    internal void WriteActionImportElementHeader(IEdmActionImport actionImport)
    {
      this.xmlWriter.WriteStartElement("ActionImport");
      this.WriteOperationImportAttributes((IEdmOperationImport) actionImport, "Action");
    }

    internal void WriteFunctionImportElementHeader(IEdmFunctionImport functionImport)
    {
      this.xmlWriter.WriteStartElement("FunctionImport");
      this.WriteOperationImportAttributes((IEdmOperationImport) functionImport, "Function");
      this.WriteOptionalAttribute<bool>("IncludeInServiceDocument", functionImport.IncludeInServiceDocument, false, new Func<bool, string>(EdmValueWriter.BooleanAsXml));
    }

    internal void WriteOperationParameterElementHeader(
      IEdmOperationParameter parameter,
      bool inlineType)
    {
      this.xmlWriter.WriteStartElement("Parameter");
      this.WriteRequiredAttribute<string>("Name", parameter.Name, new Func<string, string>(EdmValueWriter.StringAsXml));
      if (!inlineType)
        return;
      this.WriteRequiredAttribute<IEdmTypeReference>("Type", parameter.Type, new Func<IEdmTypeReference, string>(this.TypeReferenceAsXml));
    }

    internal void WriteOperationParameterEndElement(IEdmOperationParameter parameter)
    {
      if (parameter is IEdmOptionalParameter element && !element.VocabularyAnnotations(this.model).Any<IEdmVocabularyAnnotation>((Func<IEdmVocabularyAnnotation, bool>) (a => a.Term == CoreVocabularyModel.OptionalParameterTerm)))
      {
        string defaultValueString = element.DefaultValueString;
        EdmRecordExpression expression = new EdmRecordExpression(new IEdmPropertyConstructor[0]);
        this.WriteVocabularyAnnotationElementHeader((IEdmVocabularyAnnotation) new EdmVocabularyAnnotation((IEdmVocabularyAnnotatable) parameter, CoreVocabularyModel.OptionalParameterTerm, (IEdmExpression) expression), false);
        if (!string.IsNullOrEmpty(defaultValueString))
        {
          EdmPropertyConstructor propertyConstructor = new EdmPropertyConstructor("DefaultValue", (IEdmExpression) new EdmStringConstant(defaultValueString));
          this.WriteRecordExpressionElementHeader((IEdmRecordExpression) expression);
          this.WritePropertyValueElementHeader((IEdmPropertyConstructor) propertyConstructor, true);
          this.WriteEndElement();
          this.WriteEndElement();
        }
        this.WriteEndElement();
      }
      this.WriteEndElement();
    }

    internal void WriteCollectionTypeElementHeader(
      IEdmCollectionType collectionType,
      bool inlineType)
    {
      this.xmlWriter.WriteStartElement("CollectionType");
      if (!inlineType)
        return;
      this.WriteRequiredAttribute<IEdmTypeReference>("ElementType", collectionType.ElementType, new Func<IEdmTypeReference, string>(this.TypeReferenceAsXml));
    }

    internal void WriteInlineExpression(IEdmExpression expression)
    {
      switch (expression.ExpressionKind)
      {
        case EdmExpressionKind.BinaryConstant:
          this.WriteRequiredAttribute<byte[]>("Binary", ((IEdmBinaryValue) expression).Value, new Func<byte[], string>(EdmValueWriter.BinaryAsXml));
          break;
        case EdmExpressionKind.BooleanConstant:
          this.WriteRequiredAttribute<bool>("Bool", ((IEdmBooleanValue) expression).Value, new Func<bool, string>(EdmValueWriter.BooleanAsXml));
          break;
        case EdmExpressionKind.DateTimeOffsetConstant:
          this.WriteRequiredAttribute<DateTimeOffset>("DateTimeOffset", ((IEdmDateTimeOffsetValue) expression).Value, new Func<DateTimeOffset, string>(EdmValueWriter.DateTimeOffsetAsXml));
          break;
        case EdmExpressionKind.DecimalConstant:
          this.WriteRequiredAttribute<Decimal>("Decimal", ((IEdmDecimalValue) expression).Value, new Func<Decimal, string>(EdmValueWriter.DecimalAsXml));
          break;
        case EdmExpressionKind.FloatingConstant:
          this.WriteRequiredAttribute<double>("Float", ((IEdmFloatingValue) expression).Value, new Func<double, string>(EdmValueWriter.FloatAsXml));
          break;
        case EdmExpressionKind.GuidConstant:
          this.WriteRequiredAttribute<Guid>("Guid", ((IEdmGuidValue) expression).Value, new Func<Guid, string>(EdmValueWriter.GuidAsXml));
          break;
        case EdmExpressionKind.IntegerConstant:
          this.WriteRequiredAttribute<long>("Int", ((IEdmIntegerValue) expression).Value, new Func<long, string>(EdmValueWriter.LongAsXml));
          break;
        case EdmExpressionKind.StringConstant:
          this.WriteRequiredAttribute<string>("String", ((IEdmStringValue) expression).Value, new Func<string, string>(EdmValueWriter.StringAsXml));
          break;
        case EdmExpressionKind.DurationConstant:
          this.WriteRequiredAttribute<TimeSpan>("Duration", ((IEdmDurationValue) expression).Value, new Func<TimeSpan, string>(EdmValueWriter.DurationAsXml));
          break;
        case EdmExpressionKind.Path:
          this.WriteRequiredAttribute<IEnumerable<string>>("Path", ((IEdmPathExpression) expression).PathSegments, new Func<IEnumerable<string>, string>(EdmModelCsdlSchemaWriter.PathAsXml));
          break;
        case EdmExpressionKind.PropertyPath:
          this.WriteRequiredAttribute<IEnumerable<string>>("PropertyPath", ((IEdmPathExpression) expression).PathSegments, new Func<IEnumerable<string>, string>(EdmModelCsdlSchemaWriter.PathAsXml));
          break;
        case EdmExpressionKind.NavigationPropertyPath:
          this.WriteRequiredAttribute<IEnumerable<string>>("NavigationPropertyPath", ((IEdmPathExpression) expression).PathSegments, new Func<IEnumerable<string>, string>(EdmModelCsdlSchemaWriter.PathAsXml));
          break;
        case EdmExpressionKind.DateConstant:
          this.WriteRequiredAttribute<Date>("Date", ((IEdmDateValue) expression).Value, new Func<Date, string>(EdmValueWriter.DateAsXml));
          break;
        case EdmExpressionKind.TimeOfDayConstant:
          this.WriteRequiredAttribute<TimeOfDay>("TimeOfDay", ((IEdmTimeOfDayValue) expression).Value, new Func<TimeOfDay, string>(EdmValueWriter.TimeOfDayAsXml));
          break;
      }
    }

    internal void WriteVocabularyAnnotationElementHeader(
      IEdmVocabularyAnnotation annotation,
      bool isInline)
    {
      this.xmlWriter.WriteStartElement("Annotation");
      this.WriteRequiredAttribute<IEdmTerm>("Term", annotation.Term, new Func<IEdmTerm, string>(this.TermAsXml));
      this.WriteOptionalAttribute<string>("Qualifier", annotation.Qualifier, new Func<string, string>(EdmValueWriter.StringAsXml));
      if (!isInline)
        return;
      this.WriteInlineExpression(annotation.Value);
    }

    internal void WritePropertyValueElementHeader(IEdmPropertyConstructor value, bool isInline)
    {
      this.xmlWriter.WriteStartElement("PropertyValue");
      this.WriteRequiredAttribute<string>("Property", value.Name, new Func<string, string>(EdmValueWriter.StringAsXml));
      if (!isInline)
        return;
      this.WriteInlineExpression(value.Value);
    }

    internal void WriteRecordExpressionElementHeader(IEdmRecordExpression expression)
    {
      this.xmlWriter.WriteStartElement("Record");
      this.WriteOptionalAttribute<IEdmStructuredTypeReference>("Type", expression.DeclaredType, new Func<IEdmStructuredTypeReference, string>(this.TypeReferenceAsXml));
    }

    internal void WritePropertyConstructorElementHeader(
      IEdmPropertyConstructor constructor,
      bool isInline)
    {
      this.xmlWriter.WriteStartElement("PropertyValue");
      this.WriteRequiredAttribute<string>("Property", constructor.Name, new Func<string, string>(EdmValueWriter.StringAsXml));
      if (!isInline)
        return;
      this.WriteInlineExpression(constructor.Value);
    }

    internal void WriteStringConstantExpressionElement(IEdmStringConstantExpression expression)
    {
      this.xmlWriter.WriteStartElement("String");
      this.xmlWriter.WriteString(EdmValueWriter.StringAsXml(expression.Value));
      this.WriteEndElement();
    }

    internal void WriteBinaryConstantExpressionElement(IEdmBinaryConstantExpression expression)
    {
      this.xmlWriter.WriteStartElement("Binary");
      this.xmlWriter.WriteString(EdmValueWriter.BinaryAsXml(expression.Value));
      this.WriteEndElement();
    }

    internal void WriteBooleanConstantExpressionElement(IEdmBooleanConstantExpression expression)
    {
      this.xmlWriter.WriteStartElement("Bool");
      this.xmlWriter.WriteString(EdmValueWriter.BooleanAsXml(expression.Value));
      this.WriteEndElement();
    }

    internal void WriteNullConstantExpressionElement(IEdmNullExpression expression)
    {
      this.xmlWriter.WriteStartElement("Null");
      this.WriteEndElement();
    }

    internal void WriteDateConstantExpressionElement(IEdmDateConstantExpression expression)
    {
      this.xmlWriter.WriteStartElement("Date");
      this.xmlWriter.WriteString(EdmValueWriter.DateAsXml(expression.Value));
      this.WriteEndElement();
    }

    internal void WriteDateTimeOffsetConstantExpressionElement(
      IEdmDateTimeOffsetConstantExpression expression)
    {
      this.xmlWriter.WriteStartElement("DateTimeOffset");
      this.xmlWriter.WriteString(EdmValueWriter.DateTimeOffsetAsXml(expression.Value));
      this.WriteEndElement();
    }

    internal void WriteDurationConstantExpressionElement(IEdmDurationConstantExpression expression)
    {
      this.xmlWriter.WriteStartElement("Duration");
      this.xmlWriter.WriteString(EdmValueWriter.DurationAsXml(expression.Value));
      this.WriteEndElement();
    }

    internal void WriteDecimalConstantExpressionElement(IEdmDecimalConstantExpression expression)
    {
      this.xmlWriter.WriteStartElement("Decimal");
      this.xmlWriter.WriteString(EdmValueWriter.DecimalAsXml(expression.Value));
      this.WriteEndElement();
    }

    internal void WriteFloatingConstantExpressionElement(IEdmFloatingConstantExpression expression)
    {
      this.xmlWriter.WriteStartElement("Float");
      this.xmlWriter.WriteString(EdmValueWriter.FloatAsXml(expression.Value));
      this.WriteEndElement();
    }

    internal void WriteFunctionApplicationElementHeader(IEdmApplyExpression expression)
    {
      this.xmlWriter.WriteStartElement("Apply");
      this.WriteRequiredAttribute<IEdmFunction>("Function", expression.AppliedFunction, new Func<IEdmFunction, string>(this.FunctionAsXml));
    }

    internal void WriteGuidConstantExpressionElement(IEdmGuidConstantExpression expression)
    {
      this.xmlWriter.WriteStartElement("Guid");
      this.xmlWriter.WriteString(EdmValueWriter.GuidAsXml(expression.Value));
      this.WriteEndElement();
    }

    internal void WriteIntegerConstantExpressionElement(IEdmIntegerConstantExpression expression)
    {
      this.xmlWriter.WriteStartElement("Int");
      this.xmlWriter.WriteString(EdmValueWriter.LongAsXml(expression.Value));
      this.WriteEndElement();
    }

    internal void WritePathExpressionElement(IEdmPathExpression expression)
    {
      this.xmlWriter.WriteStartElement("Path");
      this.xmlWriter.WriteString(EdmModelCsdlSchemaWriter.PathAsXml(expression.PathSegments));
      this.WriteEndElement();
    }

    internal void WritePropertyPathExpressionElement(IEdmPathExpression expression)
    {
      this.xmlWriter.WriteStartElement("PropertyPath");
      this.xmlWriter.WriteString(EdmModelCsdlSchemaWriter.PathAsXml(expression.PathSegments));
      this.WriteEndElement();
    }

    internal void WriteNavigationPropertyPathExpressionElement(IEdmPathExpression expression)
    {
      this.xmlWriter.WriteStartElement("NavigationPropertyPath");
      this.xmlWriter.WriteString(EdmModelCsdlSchemaWriter.PathAsXml(expression.PathSegments));
      this.WriteEndElement();
    }

    internal void WriteIfExpressionElementHeader(IEdmIfExpression expression) => this.xmlWriter.WriteStartElement("If");

    internal void WriteCollectionExpressionElementHeader(IEdmCollectionExpression expression) => this.xmlWriter.WriteStartElement("Collection");

    internal void WriteLabeledElementHeader(IEdmLabeledExpression labeledElement)
    {
      this.xmlWriter.WriteStartElement("LabeledElement");
      this.WriteRequiredAttribute<string>("Name", labeledElement.Name, new Func<string, string>(EdmValueWriter.StringAsXml));
    }

    internal void WriteTimeOfDayConstantExpressionElement(IEdmTimeOfDayConstantExpression expression)
    {
      this.xmlWriter.WriteStartElement("TimeOfDay");
      this.xmlWriter.WriteString(EdmValueWriter.TimeOfDayAsXml(expression.Value));
      this.WriteEndElement();
    }

    internal void WriteIsTypeExpressionElementHeader(
      IEdmIsTypeExpression expression,
      bool inlineType)
    {
      this.xmlWriter.WriteStartElement("IsType");
      if (!inlineType)
        return;
      this.WriteRequiredAttribute<IEdmTypeReference>("Type", expression.Type, new Func<IEdmTypeReference, string>(this.TypeReferenceAsXml));
    }

    internal void WriteCastExpressionElementHeader(IEdmCastExpression expression, bool inlineType)
    {
      this.xmlWriter.WriteStartElement("Cast");
      if (!inlineType)
        return;
      this.WriteRequiredAttribute<IEdmTypeReference>("Type", expression.Type, new Func<IEdmTypeReference, string>(this.TypeReferenceAsXml));
    }

    internal void WriteEnumMemberExpressionElement(IEdmEnumMemberExpression expression)
    {
      this.xmlWriter.WriteStartElement("EnumMember");
      this.xmlWriter.WriteString(EdmModelCsdlSchemaWriter.EnumMemberAsXml(expression.EnumMembers));
      this.WriteEndElement();
    }

    internal void WriteTypeDefinitionElementHeader(IEdmTypeDefinition typeDefinition)
    {
      this.xmlWriter.WriteStartElement("TypeDefinition");
      this.WriteRequiredAttribute<string>("Name", typeDefinition.Name, new Func<string, string>(EdmValueWriter.StringAsXml));
      this.WriteRequiredAttribute<IEdmPrimitiveType>("UnderlyingType", typeDefinition.UnderlyingType, new Func<IEdmPrimitiveType, string>(this.TypeDefinitionAsXml));
    }

    internal void WriteEndElement() => this.xmlWriter.WriteEndElement();

    internal void WriteOptionalAttribute<T>(
      string attribute,
      T value,
      T defaultValue,
      Func<T, string> toXml)
    {
      if (value.Equals((object) defaultValue))
        return;
      this.xmlWriter.WriteAttributeString(attribute, toXml(value));
    }

    internal void WriteOptionalAttribute<T>(string attribute, T value, Func<T, string> toXml)
    {
      if ((object) value == null)
        return;
      this.xmlWriter.WriteAttributeString(attribute, toXml(value));
    }

    internal void WriteRequiredAttribute<T>(string attribute, T value, Func<T, string> toXml) => this.xmlWriter.WriteAttributeString(attribute, toXml(value));

    private void WriteOperationElementAttributes(IEdmOperation operation)
    {
      this.WriteRequiredAttribute<string>("Name", operation.Name, new Func<string, string>(EdmValueWriter.StringAsXml));
      if (operation.IsBound)
        this.WriteOptionalAttribute<bool>("IsBound", operation.IsBound, new Func<bool, string>(EdmValueWriter.BooleanAsXml));
      if (operation.EntitySetPath == null)
        return;
      this.WriteOptionalAttribute<IEnumerable<string>>("EntitySetPath", operation.EntitySetPath.PathSegments, new Func<IEnumerable<string>, string>(EdmModelCsdlSchemaWriter.PathAsXml));
    }

    private void WriteNavigationPropertyBinding(IEdmNavigationPropertyBinding binding)
    {
      this.xmlWriter.WriteStartElement("NavigationPropertyBinding");
      this.WriteRequiredAttribute<string>("Path", binding.Path.Path, new Func<string, string>(EdmValueWriter.StringAsXml));
      if (binding.Target is IEdmContainedEntitySet target)
        this.WriteRequiredAttribute<string>("Target", target.Path.Path, new Func<string, string>(EdmValueWriter.StringAsXml));
      else
        this.WriteRequiredAttribute<string>("Target", binding.Target.Name, new Func<string, string>(EdmValueWriter.StringAsXml));
      this.xmlWriter.WriteEndElement();
    }

    private static string EnumMemberAsXml(IEnumerable<IEdmEnumMember> members)
    {
      string str = members.First<IEdmEnumMember>().DeclaringType.FullName();
      List<string> stringList = new List<string>();
      foreach (IEdmEnumMember member in members)
        stringList.Add(str + "/" + member.Name);
      return string.Join(" ", stringList.ToArray());
    }

    private static string SridAsXml(int? i) => !i.HasValue ? "Variable" : Convert.ToString(i.Value, (IFormatProvider) CultureInfo.InvariantCulture);

    private static string ScaleAsXml(int? i) => !i.HasValue ? "Variable" : Convert.ToString(i.Value, (IFormatProvider) CultureInfo.InvariantCulture);

    private static string GetCsdlNamespace(Version edmVersion)
    {
      string[] strArray;
      if (CsdlConstants.SupportedVersions.TryGetValue(edmVersion, out strArray))
        return strArray[0];
      throw new InvalidOperationException(Strings.Serializer_UnknownEdmVersion);
    }

    private void WriteOperationImportAttributes(
      IEdmOperationImport operationImport,
      string operationAttributeName)
    {
      this.WriteRequiredAttribute<string>("Name", operationImport.Name, new Func<string, string>(EdmValueWriter.StringAsXml));
      this.WriteRequiredAttribute<string>(operationAttributeName, operationImport.Operation.FullName(), new Func<string, string>(EdmValueWriter.StringAsXml));
      if (operationImport.EntitySet == null)
        return;
      if (!(operationImport.EntitySet is IEdmPathExpression entitySet))
        throw new InvalidOperationException(Strings.EdmModel_Validator_Semantic_OperationImportEntitySetExpressionIsInvalid((object) operationImport.Name));
      this.WriteOptionalAttribute<IEnumerable<string>>("EntitySet", entitySet.PathSegments, new Func<IEnumerable<string>, string>(EdmModelCsdlSchemaWriter.PathAsXml));
    }

    private string SerializationName(IEdmSchemaElement element)
    {
      string str;
      return this.namespaceAliasMappings != null && this.namespaceAliasMappings.TryGetValue(element.Namespace, out str) ? str + "." + element.Name : element.FullName();
    }

    private string TypeReferenceAsXml(IEdmTypeReference type)
    {
      if (type.IsCollection())
        return "Collection(" + this.SerializationName((IEdmSchemaElement) type.AsCollection().ElementType().Definition) + ")";
      return type.IsEntityReference() ? "Ref(" + this.SerializationName((IEdmSchemaElement) type.AsEntityReference().EntityReferenceDefinition().EntityType) + ")" : this.SerializationName((IEdmSchemaElement) type.Definition);
    }

    private string TypeDefinitionAsXml(IEdmSchemaType type) => this.SerializationName((IEdmSchemaElement) type);

    private string FunctionAsXml(IEdmOperation operation) => this.SerializationName((IEdmSchemaElement) operation);

    private string TermAsXml(IEdmTerm term) => term == null ? string.Empty : this.SerializationName((IEdmSchemaElement) term);
  }
}
