// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.Edm.Csdl.Serialization.EdmModelCsdlSerializationVisitor
// Assembly: Microsoft.TeamFoundation.OData.Edm, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 75C95BF5-2544-413A-959F-F9F18C11D35F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Edm.dll

using Microsoft.OData.Edm.Vocabularies;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;

namespace Microsoft.OData.Edm.Csdl.Serialization
{
  internal sealed class EdmModelCsdlSerializationVisitor : EdmModelVisitor
  {
    private readonly Version edmVersion;
    private readonly EdmModelCsdlSchemaWriter schemaWriter;
    private readonly List<IEdmNavigationProperty> navigationProperties = new List<IEdmNavigationProperty>();
    private readonly VersioningDictionary<string, string> namespaceAliasMappings;

    internal EdmModelCsdlSerializationVisitor(
      IEdmModel model,
      XmlWriter xmlWriter,
      Version edmVersion)
      : base(model)
    {
      this.edmVersion = edmVersion;
      this.namespaceAliasMappings = model.GetNamespaceAliases();
      this.schemaWriter = new EdmModelCsdlSchemaWriter(model, this.namespaceAliasMappings, xmlWriter, this.edmVersion);
    }

    public override void VisitEntityContainerElements(
      IEnumerable<IEdmEntityContainerElement> elements)
    {
      HashSet<string> stringSet1 = new HashSet<string>();
      HashSet<string> stringSet2 = new HashSet<string>();
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
            IEdmActionImport edmActionImport = (IEdmActionImport) element;
            string str1 = edmActionImport.Name + "_" + edmActionImport.Action.FullName() + EdmModelCsdlSerializationVisitor.GetEntitySetString((IEdmOperationImport) edmActionImport);
            if (!stringSet2.Contains(str1))
            {
              stringSet2.Add(str1);
              this.ProcessActionImport(edmActionImport);
              continue;
            }
            continue;
          case EdmContainerElementKind.FunctionImport:
            IEdmFunctionImport edmFunctionImport = (IEdmFunctionImport) element;
            string str2 = edmFunctionImport.Name + "_" + edmFunctionImport.Function.FullName() + EdmModelCsdlSerializationVisitor.GetEntitySetString((IEdmOperationImport) edmFunctionImport);
            if (!stringSet1.Contains(str2))
            {
              stringSet1.Add(str2);
              this.ProcessFunctionImport(edmFunctionImport);
              continue;
            }
            continue;
          case EdmContainerElementKind.Singleton:
            this.ProcessSingleton((IEdmSingleton) element);
            continue;
          default:
            throw new InvalidOperationException(Strings.UnknownEnumVal_ContainerElementKind((object) element.ContainerElementKind.ToString()));
        }
      }
    }

    internal void VisitEdmSchema(
      EdmSchema element,
      IEnumerable<KeyValuePair<string, string>> mappings)
    {
      string alias = (string) null;
      if (this.namespaceAliasMappings != null)
        this.namespaceAliasMappings.TryGetValue(element.Namespace, out alias);
      this.schemaWriter.WriteSchemaElementHeader(element, alias, mappings);
      this.VisitSchemaElements((IEnumerable<IEdmSchemaElement>) element.SchemaElements);
      EdmModelVisitor.VisitCollection<IEdmEntityContainer>((IEnumerable<IEdmEntityContainer>) element.EntityContainers, new Action<IEdmEntityContainer>(((EdmModelVisitor) this).ProcessEntityContainer));
      foreach (KeyValuePair<string, List<IEdmVocabularyAnnotation>> ofLineAnnotation in element.OutOfLineAnnotations)
      {
        this.schemaWriter.WriteAnnotationsElementHeader(ofLineAnnotation.Key);
        this.VisitVocabularyAnnotations((IEnumerable<IEdmVocabularyAnnotation>) ofLineAnnotation.Value);
        this.schemaWriter.WriteEndElement();
      }
      this.schemaWriter.WriteEndElement();
    }

    protected override void ProcessEntityContainer(IEdmEntityContainer element)
    {
      this.BeginElement<IEdmEntityContainer>(element, new Action<IEdmEntityContainer>(this.schemaWriter.WriteEntityContainerElementHeader));
      base.ProcessEntityContainer(element);
      this.EndElement((IEdmElement) element);
    }

    protected override void ProcessEntitySet(IEdmEntitySet element)
    {
      this.BeginElement<IEdmEntitySet>(element, new Action<IEdmEntitySet>(this.schemaWriter.WriteEntitySetElementHeader));
      base.ProcessEntitySet(element);
      foreach (IEdmNavigationPropertyBinding navigationPropertyBinding in element.NavigationPropertyBindings)
        this.schemaWriter.WriteNavigationPropertyBinding((IEdmNavigationSource) element, navigationPropertyBinding);
      this.EndElement((IEdmElement) element);
    }

    protected override void ProcessSingleton(IEdmSingleton element)
    {
      this.BeginElement<IEdmSingleton>(element, new Action<IEdmSingleton>(this.schemaWriter.WriteSingletonElementHeader));
      base.ProcessSingleton(element);
      foreach (IEdmNavigationPropertyBinding navigationPropertyBinding in element.NavigationPropertyBindings)
        this.schemaWriter.WriteNavigationPropertyBinding((IEdmNavigationSource) element, navigationPropertyBinding);
      this.EndElement((IEdmElement) element);
    }

    protected override void ProcessEntityType(IEdmEntityType element)
    {
      this.BeginElement<IEdmEntityType>(element, new Action<IEdmEntityType>(this.schemaWriter.WriteEntityTypeElementHeader));
      if (element.DeclaredKey != null && element.DeclaredKey.Any<IEdmStructuralProperty>())
        this.VisitEntityTypeDeclaredKey(element.DeclaredKey);
      this.VisitProperties(element.DeclaredStructuralProperties().Cast<IEdmProperty>());
      this.VisitProperties(element.DeclaredNavigationProperties().Cast<IEdmProperty>());
      this.EndElement((IEdmElement) element);
    }

    protected override void ProcessStructuralProperty(IEdmStructuralProperty element)
    {
      bool inlineType = EdmModelCsdlSerializationVisitor.IsInlineType(element.Type);
      this.BeginElement<IEdmStructuralProperty>(element, (Action<IEdmStructuralProperty>) (t => this.schemaWriter.WriteStructuralPropertyElementHeader(t, inlineType)), (Action<IEdmStructuralProperty>) (e => this.ProcessFacets(e.Type, inlineType)));
      if (!inlineType)
        this.VisitTypeReference(element.Type);
      this.EndElement((IEdmElement) element);
    }

    protected override void ProcessTypeDefinitionReference(IEdmTypeDefinitionReference element) => this.schemaWriter.WriteTypeDefinitionAttributes(element);

    protected override void ProcessBinaryTypeReference(IEdmBinaryTypeReference element) => this.schemaWriter.WriteBinaryTypeAttributes(element);

    protected override void ProcessDecimalTypeReference(IEdmDecimalTypeReference element) => this.schemaWriter.WriteDecimalTypeAttributes(element);

    protected override void ProcessSpatialTypeReference(IEdmSpatialTypeReference element) => this.schemaWriter.WriteSpatialTypeAttributes(element);

    protected override void ProcessStringTypeReference(IEdmStringTypeReference element) => this.schemaWriter.WriteStringTypeAttributes(element);

    protected override void ProcessTemporalTypeReference(IEdmTemporalTypeReference element) => this.schemaWriter.WriteTemporalTypeAttributes(element);

    protected override void ProcessNavigationProperty(IEdmNavigationProperty element)
    {
      this.BeginElement<IEdmNavigationProperty>(element, new Action<IEdmNavigationProperty>(this.schemaWriter.WriteNavigationPropertyElementHeader));
      if (element.OnDelete != EdmOnDeleteAction.None)
        this.schemaWriter.WriteOperationActionElement("OnDelete", element.OnDelete);
      this.ProcessReferentialConstraint(element.ReferentialConstraint);
      this.EndElement((IEdmElement) element);
      this.navigationProperties.Add(element);
    }

    protected override void ProcessComplexType(IEdmComplexType element)
    {
      this.BeginElement<IEdmComplexType>(element, new Action<IEdmComplexType>(this.schemaWriter.WriteComplexTypeElementHeader));
      base.ProcessComplexType(element);
      this.EndElement((IEdmElement) element);
    }

    protected override void ProcessEnumType(IEdmEnumType element)
    {
      this.BeginElement<IEdmEnumType>(element, new Action<IEdmEnumType>(this.schemaWriter.WriteEnumTypeElementHeader));
      base.ProcessEnumType(element);
      this.EndElement((IEdmElement) element);
    }

    protected override void ProcessEnumMember(IEdmEnumMember element)
    {
      this.BeginElement<IEdmEnumMember>(element, new Action<IEdmEnumMember>(this.schemaWriter.WriteEnumMemberElementHeader));
      this.EndElement((IEdmElement) element);
    }

    protected override void ProcessTypeDefinition(IEdmTypeDefinition element)
    {
      this.BeginElement<IEdmTypeDefinition>(element, new Action<IEdmTypeDefinition>(this.schemaWriter.WriteTypeDefinitionElementHeader));
      base.ProcessTypeDefinition(element);
      this.EndElement((IEdmElement) element);
    }

    protected override void ProcessTerm(IEdmTerm term)
    {
      bool inlineType = term.Type != null && EdmModelCsdlSerializationVisitor.IsInlineType(term.Type);
      this.BeginElement<IEdmTerm>(term, (Action<IEdmTerm>) (t => this.schemaWriter.WriteTermElementHeader(t, inlineType)), (Action<IEdmTerm>) (e => this.ProcessFacets(e.Type, inlineType)));
      if (!inlineType && term.Type != null)
        this.VisitTypeReference(term.Type);
      this.EndElement((IEdmElement) term);
    }

    protected override void ProcessAction(IEdmAction action) => this.ProcessOperation<IEdmAction>(action, new Action<IEdmAction>(this.schemaWriter.WriteActionElementHeader));

    protected override void ProcessFunction(IEdmFunction function) => this.ProcessOperation<IEdmFunction>(function, new Action<IEdmFunction>(this.schemaWriter.WriteFunctionElementHeader));

    protected override void ProcessOperationParameter(IEdmOperationParameter element)
    {
      bool inlineType = EdmModelCsdlSerializationVisitor.IsInlineType(element.Type);
      this.BeginElement<IEdmOperationParameter>(element, (Action<IEdmOperationParameter>) (t => this.schemaWriter.WriteOperationParameterElementHeader(t, inlineType)), (Action<IEdmOperationParameter>) (e => this.ProcessFacets(e.Type, inlineType)));
      if (!inlineType)
        this.VisitTypeReference(element.Type);
      this.VisitPrimitiveElementAnnotations(this.Model.DirectValueAnnotations((IEdmElement) element));
      IEdmVocabularyAnnotatable element1 = (IEdmVocabularyAnnotatable) element;
      if (element1 != null)
        this.VisitElementVocabularyAnnotations(this.Model.FindDeclaredVocabularyAnnotations(element1).Where<IEdmVocabularyAnnotation>((Func<IEdmVocabularyAnnotation, bool>) (a => a.IsInline(this.Model))));
      this.schemaWriter.WriteOperationParameterEndElement(element);
    }

    protected override void ProcessOperationReturn(IEdmOperationReturn operationReturn)
    {
      if (operationReturn == null)
        return;
      bool inlineType = EdmModelCsdlSerializationVisitor.IsInlineType(operationReturn.Type);
      this.BeginElement<IEdmTypeReference>(operationReturn.Type, (Action<IEdmTypeReference>) (type => this.schemaWriter.WriteReturnTypeElementHeader()), (Action<IEdmTypeReference>) (type =>
      {
        if (inlineType)
        {
          this.schemaWriter.WriteTypeAttribute(type);
          this.ProcessFacets(type, true);
        }
        else
          this.VisitTypeReference(type);
      }));
      this.EndElement((IEdmElement) operationReturn);
    }

    protected override void ProcessCollectionType(IEdmCollectionType element)
    {
      bool inlineType = EdmModelCsdlSerializationVisitor.IsInlineType(element.ElementType);
      this.BeginElement<IEdmCollectionType>(element, (Action<IEdmCollectionType>) (t => this.schemaWriter.WriteCollectionTypeElementHeader(t, inlineType)), (Action<IEdmCollectionType>) (e => this.ProcessFacets(e.ElementType, inlineType)));
      if (!inlineType)
        this.VisitTypeReference(element.ElementType);
      this.EndElement((IEdmElement) element);
    }

    protected override void ProcessActionImport(IEdmActionImport actionImport)
    {
      this.BeginElement<IEdmActionImport>(actionImport, new Action<IEdmActionImport>(this.schemaWriter.WriteActionImportElementHeader));
      this.EndElement((IEdmElement) actionImport);
    }

    protected override void ProcessFunctionImport(IEdmFunctionImport functionImport)
    {
      this.BeginElement<IEdmFunctionImport>(functionImport, new Action<IEdmFunctionImport>(this.schemaWriter.WriteFunctionImportElementHeader));
      this.EndElement((IEdmElement) functionImport);
    }

    protected override void ProcessAnnotation(IEdmVocabularyAnnotation annotation)
    {
      bool isInline = EdmModelCsdlSerializationVisitor.IsInlineExpression(annotation.Value);
      this.BeginElement<IEdmVocabularyAnnotation>(annotation, (Action<IEdmVocabularyAnnotation>) (t => this.schemaWriter.WriteVocabularyAnnotationElementHeader(t, isInline)));
      if (!isInline)
        base.ProcessAnnotation(annotation);
      this.EndElement((IEdmElement) annotation);
    }

    protected override void ProcessStringConstantExpression(IEdmStringConstantExpression expression) => this.schemaWriter.WriteStringConstantExpressionElement(expression);

    protected override void ProcessBinaryConstantExpression(IEdmBinaryConstantExpression expression) => this.schemaWriter.WriteBinaryConstantExpressionElement(expression);

    protected override void ProcessRecordExpression(IEdmRecordExpression expression)
    {
      this.BeginElement<IEdmRecordExpression>(expression, new Action<IEdmRecordExpression>(this.schemaWriter.WriteRecordExpressionElementHeader));
      this.VisitPropertyConstructors(expression.Properties);
      this.EndElement((IEdmElement) expression);
    }

    protected override void ProcessLabeledExpression(IEdmLabeledExpression element)
    {
      if (element.Name == null)
      {
        base.ProcessLabeledExpression(element);
      }
      else
      {
        this.BeginElement<IEdmLabeledExpression>(element, new Action<IEdmLabeledExpression>(this.schemaWriter.WriteLabeledElementHeader));
        base.ProcessLabeledExpression(element);
        this.EndElement((IEdmElement) element);
      }
    }

    protected override void ProcessPropertyConstructor(IEdmPropertyConstructor constructor)
    {
      bool isInline = EdmModelCsdlSerializationVisitor.IsInlineExpression(constructor.Value);
      this.BeginElement<IEdmPropertyConstructor>(constructor, (Action<IEdmPropertyConstructor>) (t => this.schemaWriter.WritePropertyConstructorElementHeader(t, isInline)));
      if (!isInline)
        base.ProcessPropertyConstructor(constructor);
      this.EndElement((IEdmElement) constructor);
    }

    protected override void ProcessPathExpression(IEdmPathExpression expression) => this.schemaWriter.WritePathExpressionElement(expression);

    protected override void ProcessPropertyPathExpression(IEdmPathExpression expression) => this.schemaWriter.WritePropertyPathExpressionElement(expression);

    protected override void ProcessNavigationPropertyPathExpression(IEdmPathExpression expression) => this.schemaWriter.WriteNavigationPropertyPathExpressionElement(expression);

    protected override void ProcessCollectionExpression(IEdmCollectionExpression expression)
    {
      this.BeginElement<IEdmCollectionExpression>(expression, new Action<IEdmCollectionExpression>(this.schemaWriter.WriteCollectionExpressionElementHeader));
      this.VisitExpressions(expression.Elements);
      this.EndElement((IEdmElement) expression);
    }

    protected override void ProcessIsTypeExpression(IEdmIsTypeExpression expression)
    {
      bool inlineType = EdmModelCsdlSerializationVisitor.IsInlineType(expression.Type);
      this.BeginElement<IEdmIsTypeExpression>(expression, (Action<IEdmIsTypeExpression>) (t => this.schemaWriter.WriteIsTypeExpressionElementHeader(t, inlineType)), (Action<IEdmIsTypeExpression>) (e => this.ProcessFacets(e.Type, inlineType)));
      if (!inlineType)
        this.VisitTypeReference(expression.Type);
      this.VisitExpression(expression.Operand);
      this.EndElement((IEdmElement) expression);
    }

    protected override void ProcessIntegerConstantExpression(
      IEdmIntegerConstantExpression expression)
    {
      this.schemaWriter.WriteIntegerConstantExpressionElement(expression);
    }

    protected override void ProcessIfExpression(IEdmIfExpression expression)
    {
      this.BeginElement<IEdmIfExpression>(expression, new Action<IEdmIfExpression>(this.schemaWriter.WriteIfExpressionElementHeader));
      base.ProcessIfExpression(expression);
      this.EndElement((IEdmElement) expression);
    }

    protected override void ProcessFunctionApplicationExpression(IEdmApplyExpression expression)
    {
      this.BeginElement<IEdmApplyExpression>(expression, (Action<IEdmApplyExpression>) (e => this.schemaWriter.WriteFunctionApplicationElementHeader(e)));
      this.VisitExpressions(expression.Arguments);
      this.EndElement((IEdmElement) expression);
    }

    protected override void ProcessFloatingConstantExpression(
      IEdmFloatingConstantExpression expression)
    {
      this.schemaWriter.WriteFloatingConstantExpressionElement(expression);
    }

    protected override void ProcessGuidConstantExpression(IEdmGuidConstantExpression expression) => this.schemaWriter.WriteGuidConstantExpressionElement(expression);

    protected override void ProcessEnumMemberExpression(IEdmEnumMemberExpression expression) => this.schemaWriter.WriteEnumMemberExpressionElement(expression);

    protected override void ProcessDecimalConstantExpression(
      IEdmDecimalConstantExpression expression)
    {
      this.schemaWriter.WriteDecimalConstantExpressionElement(expression);
    }

    protected override void ProcessDateConstantExpression(IEdmDateConstantExpression expression) => this.schemaWriter.WriteDateConstantExpressionElement(expression);

    protected override void ProcessDateTimeOffsetConstantExpression(
      IEdmDateTimeOffsetConstantExpression expression)
    {
      this.schemaWriter.WriteDateTimeOffsetConstantExpressionElement(expression);
    }

    protected override void ProcessDurationConstantExpression(
      IEdmDurationConstantExpression expression)
    {
      this.schemaWriter.WriteDurationConstantExpressionElement(expression);
    }

    protected override void ProcessTimeOfDayConstantExpression(
      IEdmTimeOfDayConstantExpression expression)
    {
      this.schemaWriter.WriteTimeOfDayConstantExpressionElement(expression);
    }

    protected override void ProcessBooleanConstantExpression(
      IEdmBooleanConstantExpression expression)
    {
      this.schemaWriter.WriteBooleanConstantExpressionElement(expression);
    }

    protected override void ProcessNullConstantExpression(IEdmNullExpression expression) => this.schemaWriter.WriteNullConstantExpressionElement(expression);

    protected override void ProcessCastExpression(IEdmCastExpression expression)
    {
      bool inlineType = EdmModelCsdlSerializationVisitor.IsInlineType(expression.Type);
      this.BeginElement<IEdmCastExpression>(expression, (Action<IEdmCastExpression>) (t => this.schemaWriter.WriteCastExpressionElementHeader(t, inlineType)), (Action<IEdmCastExpression>) (e => this.ProcessFacets(e.Type, inlineType)));
      if (!inlineType)
        this.VisitTypeReference(expression.Type);
      this.VisitExpression(expression.Operand);
      this.EndElement((IEdmElement) expression);
    }

    private static bool IsInlineType(IEdmTypeReference reference)
    {
      if (reference.Definition is IEdmSchemaElement || reference.IsEntityReference())
        return true;
      return reference.IsCollection() && reference.AsCollection().CollectionDefinition().ElementType.Definition is IEdmSchemaElement;
    }

    private static bool IsInlineExpression(IEdmExpression expression)
    {
      switch (expression.ExpressionKind)
      {
        case EdmExpressionKind.BinaryConstant:
        case EdmExpressionKind.BooleanConstant:
        case EdmExpressionKind.DateTimeOffsetConstant:
        case EdmExpressionKind.DecimalConstant:
        case EdmExpressionKind.FloatingConstant:
        case EdmExpressionKind.GuidConstant:
        case EdmExpressionKind.IntegerConstant:
        case EdmExpressionKind.StringConstant:
        case EdmExpressionKind.DurationConstant:
        case EdmExpressionKind.Path:
        case EdmExpressionKind.PropertyPath:
        case EdmExpressionKind.NavigationPropertyPath:
        case EdmExpressionKind.DateConstant:
        case EdmExpressionKind.TimeOfDayConstant:
          return true;
        default:
          return false;
      }
    }

    private static string GetEntitySetString(IEdmOperationImport operationImport) => operationImport.EntitySet != null && operationImport.EntitySet is IEdmPathExpression entitySet ? EdmModelCsdlSchemaWriter.PathAsXml(entitySet.PathSegments) : (string) null;

    private void ProcessOperation<TOperation>(
      TOperation operation,
      Action<TOperation> writeElementAction)
      where TOperation : IEdmOperation
    {
      this.BeginElement<TOperation>(operation, writeElementAction);
      this.VisitOperationParameters(operation.Parameters);
      this.ProcessOperationReturn(operation.GetReturn());
      this.EndElement((IEdmElement) operation);
    }

    private void ProcessReferentialConstraint(IEdmReferentialConstraint element)
    {
      if (element == null)
        return;
      foreach (EdmReferentialConstraintPropertyPair propertyPair in element.PropertyPairs)
        this.schemaWriter.WriteReferentialConstraintPair(propertyPair);
    }

    private void ProcessFacets(IEdmTypeReference element, bool inlineType)
    {
      if (element == null || element.IsEntityReference() || !inlineType)
        return;
      if (element.TypeKind() == EdmTypeKind.Collection)
      {
        IEdmCollectionTypeReference type = element.AsCollection();
        this.schemaWriter.WriteNullableAttribute(type.CollectionDefinition().ElementType);
        this.VisitTypeReference(type.CollectionDefinition().ElementType);
      }
      else
      {
        this.schemaWriter.WriteNullableAttribute(element);
        this.VisitTypeReference(element);
      }
    }

    private void VisitEntityTypeDeclaredKey(IEnumerable<IEdmStructuralProperty> keyProperties)
    {
      this.schemaWriter.WriteDeclaredKeyPropertiesElementHeader();
      this.VisitPropertyRefs(keyProperties);
      this.schemaWriter.WriteEndElement();
    }

    private void VisitPropertyRefs(IEnumerable<IEdmStructuralProperty> properties)
    {
      foreach (IEdmStructuralProperty property in properties)
        this.schemaWriter.WritePropertyRefElement(property);
    }

    private void VisitAttributeAnnotations(IEnumerable<IEdmDirectValueAnnotation> annotations)
    {
      foreach (IEdmDirectValueAnnotation annotation in annotations)
      {
        if (annotation.NamespaceUri != "http://schemas.microsoft.com/ado/2011/04/edm/internal" && annotation.Value is IEdmValue edmValue && !edmValue.IsSerializedAsElement(this.Model) && edmValue.Type.TypeKind() == EdmTypeKind.Primitive)
          this.ProcessAttributeAnnotation(annotation);
      }
    }

    private void VisitPrimitiveElementAnnotations(IEnumerable<IEdmDirectValueAnnotation> annotations)
    {
      foreach (IEdmDirectValueAnnotation annotation in annotations)
      {
        if (annotation.NamespaceUri != "http://schemas.microsoft.com/ado/2011/04/edm/internal" && annotation.Value is IEdmValue edmValue && edmValue.IsSerializedAsElement(this.Model) && edmValue.Type.TypeKind() == EdmTypeKind.Primitive)
          this.ProcessElementAnnotation(annotation);
      }
    }

    private void ProcessAttributeAnnotation(IEdmDirectValueAnnotation annotation) => this.schemaWriter.WriteAnnotationStringAttribute(annotation);

    private void ProcessElementAnnotation(IEdmDirectValueAnnotation annotation) => this.schemaWriter.WriteAnnotationStringElement(annotation);

    private void VisitElementVocabularyAnnotations(IEnumerable<IEdmVocabularyAnnotation> annotations)
    {
      foreach (IEdmVocabularyAnnotation annotation in annotations)
        this.ProcessAnnotation(annotation);
    }

    private void BeginElement<TElement>(
      TElement element,
      Action<TElement> elementHeaderWriter,
      params Action<TElement>[] additionalAttributeWriters)
      where TElement : IEdmElement
    {
      elementHeaderWriter(element);
      if (additionalAttributeWriters != null)
      {
        foreach (Action<TElement> additionalAttributeWriter in additionalAttributeWriters)
          additionalAttributeWriter(element);
      }
      this.VisitAttributeAnnotations(this.Model.DirectValueAnnotations((IEdmElement) element));
    }

    private void EndElement(IEdmElement element)
    {
      this.VisitPrimitiveElementAnnotations(this.Model.DirectValueAnnotations(element));
      if (element is IEdmVocabularyAnnotatable element1)
        this.VisitElementVocabularyAnnotations(this.Model.FindDeclaredVocabularyAnnotations(element1).Where<IEdmVocabularyAnnotation>((Func<IEdmVocabularyAnnotation, bool>) (a => a.IsInline(this.Model))));
      this.schemaWriter.WriteEndElement();
    }
  }
}
