// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.Edm.EdmModelVisitor
// Assembly: Microsoft.TeamFoundation.OData.Edm, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 75C95BF5-2544-413A-959F-F9F18C11D35F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Edm.dll

using Microsoft.OData.Edm.Vocabularies;
using System;
using System.Collections.Generic;

namespace Microsoft.OData.Edm
{
  internal abstract class EdmModelVisitor
  {
    protected readonly IEdmModel Model;

    protected EdmModelVisitor(IEdmModel model) => this.Model = model;

    public void VisitEdmModel() => this.ProcessModel(this.Model);

    public void VisitSchemaElements(IEnumerable<IEdmSchemaElement> elements) => EdmModelVisitor.VisitCollection<IEdmSchemaElement>(elements, new Action<IEdmSchemaElement>(this.VisitSchemaElement));

    public void VisitSchemaElement(IEdmSchemaElement element)
    {
      switch (element.SchemaElementKind)
      {
        case EdmSchemaElementKind.None:
          this.ProcessSchemaElement(element);
          break;
        case EdmSchemaElementKind.TypeDefinition:
          this.VisitSchemaType((IEdmType) element);
          break;
        case EdmSchemaElementKind.Term:
          this.ProcessTerm((IEdmTerm) element);
          break;
        case EdmSchemaElementKind.Action:
          this.ProcessAction((IEdmAction) element);
          break;
        case EdmSchemaElementKind.EntityContainer:
          this.ProcessEntityContainer((IEdmEntityContainer) element);
          break;
        case EdmSchemaElementKind.Function:
          this.ProcessFunction((IEdmFunction) element);
          break;
        default:
          throw new InvalidOperationException(Strings.UnknownEnumVal_SchemaElementKind((object) element.SchemaElementKind));
      }
    }

    public void VisitAnnotations(IEnumerable<IEdmDirectValueAnnotation> annotations) => EdmModelVisitor.VisitCollection<IEdmDirectValueAnnotation>(annotations, new Action<IEdmDirectValueAnnotation>(this.VisitAnnotation));

    public void VisitVocabularyAnnotations(IEnumerable<IEdmVocabularyAnnotation> annotations) => EdmModelVisitor.VisitCollection<IEdmVocabularyAnnotation>(annotations, new Action<IEdmVocabularyAnnotation>(this.VisitVocabularyAnnotation));

    public void VisitAnnotation(IEdmDirectValueAnnotation annotation) => this.ProcessImmediateValueAnnotation(annotation);

    public void VisitVocabularyAnnotation(IEdmVocabularyAnnotation annotation)
    {
      if (annotation.Term != null)
        this.ProcessAnnotation(annotation);
      else
        this.ProcessVocabularyAnnotation(annotation);
    }

    public void VisitPropertyValueBindings(IEnumerable<IEdmPropertyValueBinding> bindings) => EdmModelVisitor.VisitCollection<IEdmPropertyValueBinding>(bindings, new Action<IEdmPropertyValueBinding>(this.ProcessPropertyValueBinding));

    public void VisitExpressions(IEnumerable<IEdmExpression> expressions) => EdmModelVisitor.VisitCollection<IEdmExpression>(expressions, new Action<IEdmExpression>(this.VisitExpression));

    public void VisitExpression(IEdmExpression expression)
    {
      switch (expression.ExpressionKind)
      {
        case EdmExpressionKind.None:
          this.ProcessExpression(expression);
          break;
        case EdmExpressionKind.BinaryConstant:
          this.ProcessBinaryConstantExpression((IEdmBinaryConstantExpression) expression);
          break;
        case EdmExpressionKind.BooleanConstant:
          this.ProcessBooleanConstantExpression((IEdmBooleanConstantExpression) expression);
          break;
        case EdmExpressionKind.DateTimeOffsetConstant:
          this.ProcessDateTimeOffsetConstantExpression((IEdmDateTimeOffsetConstantExpression) expression);
          break;
        case EdmExpressionKind.DecimalConstant:
          this.ProcessDecimalConstantExpression((IEdmDecimalConstantExpression) expression);
          break;
        case EdmExpressionKind.FloatingConstant:
          this.ProcessFloatingConstantExpression((IEdmFloatingConstantExpression) expression);
          break;
        case EdmExpressionKind.GuidConstant:
          this.ProcessGuidConstantExpression((IEdmGuidConstantExpression) expression);
          break;
        case EdmExpressionKind.IntegerConstant:
          this.ProcessIntegerConstantExpression((IEdmIntegerConstantExpression) expression);
          break;
        case EdmExpressionKind.StringConstant:
          this.ProcessStringConstantExpression((IEdmStringConstantExpression) expression);
          break;
        case EdmExpressionKind.DurationConstant:
          this.ProcessDurationConstantExpression((IEdmDurationConstantExpression) expression);
          break;
        case EdmExpressionKind.Null:
          this.ProcessNullConstantExpression((IEdmNullExpression) expression);
          break;
        case EdmExpressionKind.Record:
          this.ProcessRecordExpression((IEdmRecordExpression) expression);
          break;
        case EdmExpressionKind.Collection:
          this.ProcessCollectionExpression((IEdmCollectionExpression) expression);
          break;
        case EdmExpressionKind.Path:
          this.ProcessPathExpression((IEdmPathExpression) expression);
          break;
        case EdmExpressionKind.If:
          this.ProcessIfExpression((IEdmIfExpression) expression);
          break;
        case EdmExpressionKind.Cast:
          this.ProcessCastExpression((IEdmCastExpression) expression);
          break;
        case EdmExpressionKind.IsType:
          this.ProcessIsTypeExpression((IEdmIsTypeExpression) expression);
          break;
        case EdmExpressionKind.FunctionApplication:
          this.ProcessFunctionApplicationExpression((IEdmApplyExpression) expression);
          break;
        case EdmExpressionKind.LabeledExpressionReference:
          this.ProcessLabeledExpressionReferenceExpression((IEdmLabeledExpressionReferenceExpression) expression);
          break;
        case EdmExpressionKind.Labeled:
          this.ProcessLabeledExpression((IEdmLabeledExpression) expression);
          break;
        case EdmExpressionKind.PropertyPath:
          this.ProcessPropertyPathExpression((IEdmPathExpression) expression);
          break;
        case EdmExpressionKind.NavigationPropertyPath:
          this.ProcessNavigationPropertyPathExpression((IEdmPathExpression) expression);
          break;
        case EdmExpressionKind.DateConstant:
          this.ProcessDateConstantExpression((IEdmDateConstantExpression) expression);
          break;
        case EdmExpressionKind.TimeOfDayConstant:
          this.ProcessTimeOfDayConstantExpression((IEdmTimeOfDayConstantExpression) expression);
          break;
        case EdmExpressionKind.EnumMember:
          this.ProcessEnumMemberExpression((IEdmEnumMemberExpression) expression);
          break;
        default:
          throw new InvalidOperationException(Strings.UnknownEnumVal_ExpressionKind((object) expression.ExpressionKind));
      }
    }

    public void VisitPropertyConstructors(IEnumerable<IEdmPropertyConstructor> constructor) => EdmModelVisitor.VisitCollection<IEdmPropertyConstructor>(constructor, new Action<IEdmPropertyConstructor>(this.ProcessPropertyConstructor));

    public virtual void VisitEntityContainerElements(
      IEnumerable<IEdmEntityContainerElement> elements)
    {
      foreach (IEdmEntityContainerElement element in elements)
      {
        switch (element.ContainerElementKind)
        {
          case EdmContainerElementKind.None:
            this.ProcessEntityContainerElement(element);
            continue;
          case EdmContainerElementKind.EntitySet:
            this.ProcessEntitySet((IEdmEntitySet) element);
            continue;
          case EdmContainerElementKind.ActionImport:
            this.ProcessActionImport((IEdmActionImport) element);
            continue;
          case EdmContainerElementKind.FunctionImport:
            this.ProcessFunctionImport((IEdmFunctionImport) element);
            continue;
          case EdmContainerElementKind.Singleton:
            this.ProcessSingleton((IEdmSingleton) element);
            continue;
          default:
            throw new InvalidOperationException(Strings.UnknownEnumVal_ContainerElementKind((object) element.ContainerElementKind.ToString()));
        }
      }
    }

    public void VisitTypeReference(IEdmTypeReference reference)
    {
      switch (reference.TypeKind())
      {
        case EdmTypeKind.None:
          this.ProcessTypeReference(reference);
          break;
        case EdmTypeKind.Primitive:
          this.VisitPrimitiveTypeReference(reference.AsPrimitive());
          break;
        case EdmTypeKind.Entity:
          this.ProcessEntityTypeReference(reference.AsEntity());
          break;
        case EdmTypeKind.Complex:
          this.ProcessComplexTypeReference(reference.AsComplex());
          break;
        case EdmTypeKind.Collection:
          this.ProcessCollectionTypeReference(reference.AsCollection());
          break;
        case EdmTypeKind.EntityReference:
          this.ProcessEntityReferenceTypeReference(reference.AsEntityReference());
          break;
        case EdmTypeKind.Enum:
          this.ProcessEnumTypeReference(reference.AsEnum());
          break;
        case EdmTypeKind.TypeDefinition:
          this.ProcessTypeDefinitionReference(reference.AsTypeDefinition());
          break;
        case EdmTypeKind.Path:
          this.ProcessPathTypeReference(reference.AsPath());
          break;
        default:
          throw new InvalidOperationException(Strings.UnknownEnumVal_TypeKind((object) reference.TypeKind().ToString()));
      }
    }

    public void VisitPrimitiveTypeReference(IEdmPrimitiveTypeReference reference)
    {
      switch (ExtensionMethods.PrimitiveKind(reference))
      {
        case EdmPrimitiveTypeKind.None:
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
          this.ProcessPrimitiveTypeReference(reference);
          break;
        case EdmPrimitiveTypeKind.Binary:
          this.ProcessBinaryTypeReference(reference.AsBinary());
          break;
        case EdmPrimitiveTypeKind.DateTimeOffset:
        case EdmPrimitiveTypeKind.Duration:
        case EdmPrimitiveTypeKind.TimeOfDay:
          this.ProcessTemporalTypeReference(reference.AsTemporal());
          break;
        case EdmPrimitiveTypeKind.Decimal:
          this.ProcessDecimalTypeReference(reference.AsDecimal());
          break;
        case EdmPrimitiveTypeKind.String:
          this.ProcessStringTypeReference(reference.AsString());
          break;
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
          this.ProcessSpatialTypeReference(reference.AsSpatial());
          break;
        default:
          throw new InvalidOperationException(Strings.UnknownEnumVal_PrimitiveKind((object) ExtensionMethods.PrimitiveKind(reference).ToString()));
      }
    }

    public void VisitSchemaType(IEdmType definition)
    {
      switch (definition.TypeKind)
      {
        case EdmTypeKind.None:
          this.VisitSchemaType(definition);
          break;
        case EdmTypeKind.Entity:
          this.ProcessEntityType((IEdmEntityType) definition);
          break;
        case EdmTypeKind.Complex:
          this.ProcessComplexType((IEdmComplexType) definition);
          break;
        case EdmTypeKind.Enum:
          this.ProcessEnumType((IEdmEnumType) definition);
          break;
        case EdmTypeKind.TypeDefinition:
          this.ProcessTypeDefinition((IEdmTypeDefinition) definition);
          break;
        default:
          throw new InvalidOperationException(Strings.UnknownEnumVal_TypeKind((object) definition.TypeKind));
      }
    }

    public void VisitProperties(IEnumerable<IEdmProperty> properties) => EdmModelVisitor.VisitCollection<IEdmProperty>(properties, new Action<IEdmProperty>(this.VisitProperty));

    public void VisitProperty(IEdmProperty property)
    {
      switch (property.PropertyKind)
      {
        case EdmPropertyKind.None:
          this.ProcessProperty(property);
          break;
        case EdmPropertyKind.Structural:
          this.ProcessStructuralProperty((IEdmStructuralProperty) property);
          break;
        case EdmPropertyKind.Navigation:
          this.ProcessNavigationProperty((IEdmNavigationProperty) property);
          break;
        default:
          throw new InvalidOperationException(Strings.UnknownEnumVal_PropertyKind((object) property.PropertyKind.ToString()));
      }
    }

    public void VisitEnumMembers(IEnumerable<IEdmEnumMember> enumMembers) => EdmModelVisitor.VisitCollection<IEdmEnumMember>(enumMembers, new Action<IEdmEnumMember>(this.VisitEnumMember));

    public void VisitEnumMember(IEdmEnumMember enumMember) => this.ProcessEnumMember(enumMember);

    public void VisitOperationParameters(IEnumerable<IEdmOperationParameter> parameters) => EdmModelVisitor.VisitCollection<IEdmOperationParameter>(parameters, new Action<IEdmOperationParameter>(this.ProcessOperationParameter));

    protected static void VisitCollection<T>(IEnumerable<T> collection, Action<T> visitMethod)
    {
      foreach (T obj in collection)
        visitMethod(obj);
    }

    protected virtual void ProcessModel(IEdmModel model)
    {
      this.ProcessElement((IEdmElement) model);
      this.VisitSchemaElements(model.SchemaElements);
      this.VisitVocabularyAnnotations(model.VocabularyAnnotations);
    }

    protected virtual void ProcessElement(IEdmElement element) => this.VisitAnnotations(this.Model.DirectValueAnnotations(element));

    protected virtual void ProcessNamedElement(IEdmNamedElement element) => this.ProcessElement((IEdmElement) element);

    protected virtual void ProcessSchemaElement(IEdmSchemaElement element)
    {
      this.ProcessVocabularyAnnotatable((IEdmVocabularyAnnotatable) element);
      this.ProcessNamedElement((IEdmNamedElement) element);
    }

    protected virtual void ProcessVocabularyAnnotatable(IEdmVocabularyAnnotatable annotatable)
    {
    }

    protected virtual void ProcessComplexTypeReference(IEdmComplexTypeReference reference) => this.ProcessStructuredTypeReference((IEdmStructuredTypeReference) reference);

    protected virtual void ProcessEntityTypeReference(IEdmEntityTypeReference reference) => this.ProcessStructuredTypeReference((IEdmStructuredTypeReference) reference);

    protected virtual void ProcessEntityReferenceTypeReference(
      IEdmEntityReferenceTypeReference reference)
    {
      this.ProcessTypeReference((IEdmTypeReference) reference);
      this.ProcessEntityReferenceType(reference.EntityReferenceDefinition());
    }

    protected virtual void ProcessCollectionTypeReference(IEdmCollectionTypeReference reference)
    {
      this.ProcessTypeReference((IEdmTypeReference) reference);
      this.ProcessCollectionType(reference.CollectionDefinition());
    }

    protected virtual void ProcessEnumTypeReference(IEdmEnumTypeReference reference) => this.ProcessTypeReference((IEdmTypeReference) reference);

    protected virtual void ProcessTypeDefinitionReference(IEdmTypeDefinitionReference reference) => this.ProcessTypeReference((IEdmTypeReference) reference);

    protected virtual void ProcessBinaryTypeReference(IEdmBinaryTypeReference reference) => this.ProcessPrimitiveTypeReference((IEdmPrimitiveTypeReference) reference);

    protected virtual void ProcessDecimalTypeReference(IEdmDecimalTypeReference reference) => this.ProcessPrimitiveTypeReference((IEdmPrimitiveTypeReference) reference);

    protected virtual void ProcessSpatialTypeReference(IEdmSpatialTypeReference reference) => this.ProcessPrimitiveTypeReference((IEdmPrimitiveTypeReference) reference);

    protected virtual void ProcessStringTypeReference(IEdmStringTypeReference reference) => this.ProcessPrimitiveTypeReference((IEdmPrimitiveTypeReference) reference);

    protected virtual void ProcessTemporalTypeReference(IEdmTemporalTypeReference reference) => this.ProcessPrimitiveTypeReference((IEdmPrimitiveTypeReference) reference);

    protected virtual void ProcessPrimitiveTypeReference(IEdmPrimitiveTypeReference reference) => this.ProcessTypeReference((IEdmTypeReference) reference);

    protected virtual void ProcessStructuredTypeReference(IEdmStructuredTypeReference reference) => this.ProcessTypeReference((IEdmTypeReference) reference);

    protected virtual void ProcessTypeReference(IEdmTypeReference element) => this.ProcessElement((IEdmElement) element);

    protected virtual void ProcessPathTypeReference(IEdmPathTypeReference reference) => this.ProcessTypeReference((IEdmTypeReference) reference);

    protected virtual void ProcessTerm(IEdmTerm term)
    {
      this.ProcessSchemaElement((IEdmSchemaElement) term);
      this.VisitTypeReference(term.Type);
    }

    protected virtual void ProcessComplexType(IEdmComplexType definition)
    {
      this.ProcessSchemaElement((IEdmSchemaElement) definition);
      this.ProcessStructuredType((IEdmStructuredType) definition);
      this.ProcessSchemaType((IEdmSchemaType) definition);
    }

    protected virtual void ProcessEntityType(IEdmEntityType definition)
    {
      this.ProcessSchemaElement((IEdmSchemaElement) definition);
      this.ProcessStructuredType((IEdmStructuredType) definition);
      this.ProcessSchemaType((IEdmSchemaType) definition);
    }

    protected virtual void ProcessCollectionType(IEdmCollectionType definition)
    {
      this.ProcessElement((IEdmElement) definition);
      this.ProcessType((IEdmType) definition);
      this.VisitTypeReference(definition.ElementType);
    }

    protected virtual void ProcessEnumType(IEdmEnumType definition)
    {
      this.ProcessSchemaElement((IEdmSchemaElement) definition);
      this.ProcessType((IEdmType) definition);
      this.ProcessSchemaType((IEdmSchemaType) definition);
      this.VisitEnumMembers(definition.Members);
    }

    protected virtual void ProcessTypeDefinition(IEdmTypeDefinition definition)
    {
      this.ProcessSchemaElement((IEdmSchemaElement) definition);
      this.ProcessType((IEdmType) definition);
      this.ProcessSchemaType((IEdmSchemaType) definition);
    }

    protected virtual void ProcessEntityReferenceType(IEdmEntityReferenceType definition)
    {
      this.ProcessElement((IEdmElement) definition);
      this.ProcessType((IEdmType) definition);
    }

    protected virtual void ProcessStructuredType(IEdmStructuredType definition)
    {
      this.ProcessType((IEdmType) definition);
      this.VisitProperties(definition.DeclaredProperties);
    }

    protected virtual void ProcessSchemaType(IEdmSchemaType type)
    {
    }

    protected virtual void ProcessType(IEdmType definition)
    {
    }

    protected virtual void ProcessNavigationProperty(IEdmNavigationProperty property) => this.ProcessProperty((IEdmProperty) property);

    protected virtual void ProcessStructuralProperty(IEdmStructuralProperty property) => this.ProcessProperty((IEdmProperty) property);

    protected virtual void ProcessProperty(IEdmProperty property)
    {
      this.ProcessVocabularyAnnotatable((IEdmVocabularyAnnotatable) property);
      this.ProcessNamedElement((IEdmNamedElement) property);
      this.VisitTypeReference(property.Type);
    }

    protected virtual void ProcessEnumMember(IEdmEnumMember enumMember) => this.ProcessNamedElement((IEdmNamedElement) enumMember);

    protected virtual void ProcessVocabularyAnnotation(IEdmVocabularyAnnotation annotation) => this.ProcessElement((IEdmElement) annotation);

    protected virtual void ProcessImmediateValueAnnotation(IEdmDirectValueAnnotation annotation) => this.ProcessNamedElement((IEdmNamedElement) annotation);

    protected virtual void ProcessAnnotation(IEdmVocabularyAnnotation annotation)
    {
      this.ProcessVocabularyAnnotation(annotation);
      this.VisitExpression(annotation.Value);
    }

    protected virtual void ProcessPropertyValueBinding(IEdmPropertyValueBinding binding) => this.VisitExpression(binding.Value);

    protected virtual void ProcessExpression(IEdmExpression expression)
    {
    }

    protected virtual void ProcessStringConstantExpression(IEdmStringConstantExpression expression) => this.ProcessExpression((IEdmExpression) expression);

    protected virtual void ProcessBinaryConstantExpression(IEdmBinaryConstantExpression expression) => this.ProcessExpression((IEdmExpression) expression);

    protected virtual void ProcessRecordExpression(IEdmRecordExpression expression)
    {
      this.ProcessExpression((IEdmExpression) expression);
      if (expression.DeclaredType != null)
        this.VisitTypeReference((IEdmTypeReference) expression.DeclaredType);
      this.VisitPropertyConstructors(expression.Properties);
    }

    protected virtual void ProcessPathExpression(IEdmPathExpression expression) => this.ProcessExpression((IEdmExpression) expression);

    protected virtual void ProcessPropertyPathExpression(IEdmPathExpression expression) => this.ProcessExpression((IEdmExpression) expression);

    protected virtual void ProcessNavigationPropertyPathExpression(IEdmPathExpression expression) => this.ProcessExpression((IEdmExpression) expression);

    protected virtual void ProcessCollectionExpression(IEdmCollectionExpression expression)
    {
      this.ProcessExpression((IEdmExpression) expression);
      this.VisitExpressions(expression.Elements);
    }

    protected virtual void ProcessLabeledExpressionReferenceExpression(
      IEdmLabeledExpressionReferenceExpression expression)
    {
      this.ProcessExpression((IEdmExpression) expression);
    }

    protected virtual void ProcessIsTypeExpression(IEdmIsTypeExpression expression)
    {
      this.ProcessExpression((IEdmExpression) expression);
      this.VisitTypeReference(expression.Type);
      this.VisitExpression(expression.Operand);
    }

    protected virtual void ProcessIntegerConstantExpression(IEdmIntegerConstantExpression expression) => this.ProcessExpression((IEdmExpression) expression);

    protected virtual void ProcessIfExpression(IEdmIfExpression expression)
    {
      this.ProcessExpression((IEdmExpression) expression);
      this.VisitExpression(expression.TestExpression);
      this.VisitExpression(expression.TrueExpression);
      this.VisitExpression(expression.FalseExpression);
    }

    protected virtual void ProcessFunctionApplicationExpression(IEdmApplyExpression expression)
    {
      this.ProcessExpression((IEdmExpression) expression);
      this.VisitExpressions(expression.Arguments);
    }

    protected virtual void ProcessFloatingConstantExpression(
      IEdmFloatingConstantExpression expression)
    {
      this.ProcessExpression((IEdmExpression) expression);
    }

    protected virtual void ProcessGuidConstantExpression(IEdmGuidConstantExpression expression) => this.ProcessExpression((IEdmExpression) expression);

    protected virtual void ProcessEnumMemberExpression(IEdmEnumMemberExpression expression) => this.ProcessExpression((IEdmExpression) expression);

    protected virtual void ProcessDecimalConstantExpression(IEdmDecimalConstantExpression expression) => this.ProcessExpression((IEdmExpression) expression);

    protected virtual void ProcessDateConstantExpression(IEdmDateConstantExpression expression) => this.ProcessExpression((IEdmExpression) expression);

    protected virtual void ProcessTimeOfDayConstantExpression(
      IEdmTimeOfDayConstantExpression expression)
    {
      this.ProcessExpression((IEdmExpression) expression);
    }

    protected virtual void ProcessDateTimeOffsetConstantExpression(
      IEdmDateTimeOffsetConstantExpression expression)
    {
      this.ProcessExpression((IEdmExpression) expression);
    }

    protected virtual void ProcessDurationConstantExpression(
      IEdmDurationConstantExpression expression)
    {
      this.ProcessExpression((IEdmExpression) expression);
    }

    protected virtual void ProcessBooleanConstantExpression(IEdmBooleanConstantExpression expression) => this.ProcessExpression((IEdmExpression) expression);

    protected virtual void ProcessCastExpression(IEdmCastExpression expression)
    {
      this.ProcessExpression((IEdmExpression) expression);
      this.VisitTypeReference(expression.Type);
      this.VisitExpression(expression.Operand);
    }

    protected virtual void ProcessLabeledExpression(IEdmLabeledExpression element) => this.VisitExpression(element.Expression);

    protected virtual void ProcessPropertyConstructor(IEdmPropertyConstructor constructor) => this.VisitExpression(constructor.Value);

    protected virtual void ProcessNullConstantExpression(IEdmNullExpression expression) => this.ProcessExpression((IEdmExpression) expression);

    protected virtual void ProcessEntityContainer(IEdmEntityContainer container)
    {
      this.ProcessVocabularyAnnotatable((IEdmVocabularyAnnotatable) container);
      this.ProcessNamedElement((IEdmNamedElement) container);
      this.VisitEntityContainerElements(container.Elements);
    }

    protected virtual void ProcessEntityContainerElement(IEdmEntityContainerElement element) => this.ProcessNamedElement((IEdmNamedElement) element);

    protected virtual void ProcessEntitySet(IEdmEntitySet set) => this.ProcessEntityContainerElement((IEdmEntityContainerElement) set);

    protected virtual void ProcessSingleton(IEdmSingleton singleton) => this.ProcessEntityContainerElement((IEdmEntityContainerElement) singleton);

    protected virtual void ProcessAction(IEdmAction action)
    {
      this.ProcessSchemaElement((IEdmSchemaElement) action);
      this.ProcessOperation((IEdmOperation) action);
    }

    protected virtual void ProcessFunction(IEdmFunction function)
    {
      this.ProcessSchemaElement((IEdmSchemaElement) function);
      this.ProcessOperation((IEdmOperation) function);
    }

    protected virtual void ProcessActionImport(IEdmActionImport actionImport) => this.ProcessEntityContainerElement((IEdmEntityContainerElement) actionImport);

    protected virtual void ProcessFunctionImport(IEdmFunctionImport functionImport) => this.ProcessEntityContainerElement((IEdmEntityContainerElement) functionImport);

    protected virtual void ProcessOperation(IEdmOperation operation)
    {
      this.VisitOperationParameters(operation.Parameters);
      this.ProcessOperationReturn(operation.GetReturn());
    }

    protected virtual void ProcessOperationParameter(IEdmOperationParameter parameter)
    {
      this.ProcessVocabularyAnnotatable((IEdmVocabularyAnnotatable) parameter);
      this.ProcessNamedElement((IEdmNamedElement) parameter);
      this.VisitTypeReference(parameter.Type);
    }

    protected virtual void ProcessOperationReturn(IEdmOperationReturn operationReturn)
    {
      if (operationReturn == null)
        return;
      this.ProcessVocabularyAnnotatable((IEdmVocabularyAnnotatable) operationReturn);
      this.VisitTypeReference(operationReturn.Type);
    }
  }
}
