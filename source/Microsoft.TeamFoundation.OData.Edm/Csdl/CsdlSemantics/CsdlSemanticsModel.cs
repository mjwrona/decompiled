// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.Edm.Csdl.CsdlSemantics.CsdlSemanticsModel
// Assembly: Microsoft.TeamFoundation.OData.Edm, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 75C95BF5-2544-413A-959F-F9F18C11D35F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Edm.dll

using Microsoft.OData.Edm.Csdl.Parsing.Ast;
using Microsoft.OData.Edm.Validation;
using Microsoft.OData.Edm.Vocabularies;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Microsoft.OData.Edm.Csdl.CsdlSemantics
{
  [DebuggerDisplay("CsdlSemanticsModel({string.Join(\",\", DeclaredNamespaces)})")]
  internal class CsdlSemanticsModel : EdmModelBase, IEdmCheckable
  {
    private readonly CsdlSemanticsModel mainEdmModel;
    private readonly CsdlModel astModel;
    private readonly List<CsdlSemanticsSchema> schemata = new List<CsdlSemanticsSchema>();
    private readonly Dictionary<string, List<CsdlSemanticsAnnotations>> outOfLineAnnotations = new Dictionary<string, List<CsdlSemanticsAnnotations>>();
    private readonly Dictionary<CsdlAnnotation, CsdlSemanticsVocabularyAnnotation> wrappedAnnotations = new Dictionary<CsdlAnnotation, CsdlSemanticsVocabularyAnnotation>();
    private readonly Dictionary<string, List<IEdmStructuredType>> derivedTypeMappings = new Dictionary<string, List<IEdmStructuredType>>();

    public CsdlSemanticsModel(
      CsdlModel astModel,
      IEdmDirectValueAnnotationsManager annotationsManager,
      IEnumerable<IEdmModel> referencedModels,
      bool includeDefaultVocabularies = true)
      : base(referencedModels, annotationsManager, includeDefaultVocabularies)
    {
      this.astModel = astModel;
      this.SetEdmReferences(astModel.CurrentModelReferences);
      foreach (CsdlSchema schema in this.astModel.Schemata)
        this.AddSchema(schema);
    }

    public CsdlSemanticsModel(
      CsdlModel mainCsdlModel,
      IEdmDirectValueAnnotationsManager annotationsManager,
      IEnumerable<CsdlModel> referencedCsdlModels,
      bool includeDefaultVocabularies)
      : base(Enumerable.Empty<IEdmModel>(), annotationsManager, includeDefaultVocabularies)
    {
      this.astModel = mainCsdlModel;
      this.SetEdmReferences(this.astModel.CurrentModelReferences);
      foreach (CsdlModel referencedCsdlModel in referencedCsdlModels)
        this.AddReferencedModel((IEdmModel) new CsdlSemanticsModel(referencedCsdlModel, this.DirectValueAnnotationsManager, this, includeDefaultVocabularies));
      foreach (IEdmInclude edmInclude in mainCsdlModel.CurrentModelReferences.SelectMany<IEdmReference, IEdmInclude>((Func<IEdmReference, IEnumerable<IEdmInclude>>) (s => s.Includes)))
        this.SetNamespaceAlias(edmInclude.Namespace, edmInclude.Alias);
      foreach (CsdlSchema schema in this.astModel.Schemata)
        this.AddSchema(schema);
    }

    private CsdlSemanticsModel(
      CsdlModel referencedCsdlModel,
      IEdmDirectValueAnnotationsManager annotationsManager,
      CsdlSemanticsModel mainCsdlSemanticsModel,
      bool includeDefaultVocabularies)
      : base(Enumerable.Empty<IEdmModel>(), annotationsManager, includeDefaultVocabularies)
    {
      this.mainEdmModel = mainCsdlSemanticsModel;
      this.astModel = referencedCsdlModel;
      this.SetEdmReferences(referencedCsdlModel.CurrentModelReferences);
      foreach (IEdmInclude edmInclude in referencedCsdlModel.ParentModelReferences.SelectMany<IEdmReference, IEdmInclude>((Func<IEdmReference, IEnumerable<IEdmInclude>>) (s => s.Includes)))
      {
        string includeNs = edmInclude.Namespace;
        referencedCsdlModel.Schemata.Any<CsdlSchema>((Func<CsdlSchema, bool>) (s => s.Namespace == includeNs));
      }
      foreach (IEdmInclude edmInclude in referencedCsdlModel.CurrentModelReferences.SelectMany<IEdmReference, IEdmInclude>((Func<IEdmReference, IEnumerable<IEdmInclude>>) (s => s.Includes)))
        this.SetNamespaceAlias(edmInclude.Namespace, edmInclude.Alias);
      foreach (CsdlSchema schema in referencedCsdlModel.Schemata)
      {
        string schemaNamespace = schema.Namespace;
        if (referencedCsdlModel.ParentModelReferences.SelectMany<IEdmReference, IEdmInclude>((Func<IEdmReference, IEnumerable<IEdmInclude>>) (s => s.Includes)).FirstOrDefault<IEdmInclude>((Func<IEdmInclude, bool>) (s => s.Namespace == schemaNamespace)) != null)
          this.AddSchema(schema, false);
      }
    }

    public override IEnumerable<IEdmSchemaElement> SchemaElements
    {
      get
      {
        foreach (CsdlSemanticsSchema schema in this.schemata)
        {
          foreach (IEdmSchemaElement type in schema.Types)
            yield return type;
          foreach (IEdmSchemaElement operation in schema.Operations)
            yield return operation;
          foreach (IEdmSchemaElement term in schema.Terms)
            yield return term;
          foreach (IEdmSchemaElement entityContainer in schema.EntityContainers)
            yield return entityContainer;
        }
      }
    }

    public override IEnumerable<string> DeclaredNamespaces => this.schemata.Select<CsdlSemanticsSchema, string>((Func<CsdlSemanticsSchema, string>) (s => s.Namespace));

    public IDictionary<string, List<CsdlSemanticsAnnotations>> OutOfLineAnnotations => (IDictionary<string, List<CsdlSemanticsAnnotations>>) this.outOfLineAnnotations;

    public override IEnumerable<IEdmVocabularyAnnotation> VocabularyAnnotations
    {
      get
      {
        List<IEdmVocabularyAnnotation> vocabularyAnnotations = new List<IEdmVocabularyAnnotation>();
        foreach (CsdlSemanticsSchema schema in this.schemata)
        {
          foreach (CsdlAnnotations ofLineAnnotation in ((CsdlSchema) schema.Element).OutOfLineAnnotations)
          {
            CsdlSemanticsAnnotations annotationsContext = new CsdlSemanticsAnnotations(schema, ofLineAnnotation);
            foreach (CsdlAnnotation annotation1 in ofLineAnnotation.Annotations)
            {
              IEdmVocabularyAnnotation annotation2 = this.WrapVocabularyAnnotation(annotation1, schema, (IEdmVocabularyAnnotatable) null, annotationsContext, ofLineAnnotation.Qualifier);
              annotation2.SetSerializationLocation((IEdmModel) this, new EdmVocabularyAnnotationSerializationLocation?(EdmVocabularyAnnotationSerializationLocation.OutOfLine));
              annotation2.SetSchemaNamespace((IEdmModel) this, schema.Namespace);
              vocabularyAnnotations.Add(annotation2);
            }
          }
        }
        foreach (IEdmSchemaElement schemaElement in this.SchemaElements)
        {
          vocabularyAnnotations.AddRange(((CsdlSemanticsElement) schemaElement).InlineVocabularyAnnotations);
          if (schemaElement is CsdlSemanticsStructuredTypeDefinition structuredTypeDefinition)
          {
            foreach (IEdmProperty declaredProperty in structuredTypeDefinition.DeclaredProperties)
              vocabularyAnnotations.AddRange(((CsdlSemanticsElement) declaredProperty).InlineVocabularyAnnotations);
          }
          if (schemaElement is CsdlSemanticsOperation semanticsOperation)
          {
            foreach (IEdmOperationParameter parameter in semanticsOperation.Parameters)
              vocabularyAnnotations.AddRange(((CsdlSemanticsElement) parameter).InlineVocabularyAnnotations);
          }
          if (schemaElement is CsdlSemanticsEntityContainer semanticsEntityContainer)
          {
            foreach (IEdmEntityContainerElement element in semanticsEntityContainer.Elements)
              vocabularyAnnotations.AddRange(((CsdlSemanticsElement) element).InlineVocabularyAnnotations);
          }
          if (schemaElement is CsdlSemanticsEnumTypeDefinition enumTypeDefinition)
          {
            foreach (IEdmEnumMember member in enumTypeDefinition.Members)
              vocabularyAnnotations.AddRange(((CsdlSemanticsElement) member).InlineVocabularyAnnotations);
          }
        }
        return (IEnumerable<IEdmVocabularyAnnotation>) vocabularyAnnotations;
      }
    }

    public IEnumerable<EdmError> Errors
    {
      get
      {
        List<EdmError> errors = new List<EdmError>();
        HashSetInternal<string> hashSetInternal = new HashSetInternal<string>();
        VersioningList<string> namespacesHavingAlias = this.GetUsedNamespacesHavingAlias();
        VersioningDictionary<string, string> namespaceAliases = this.GetNamespaceAliases();
        if (namespacesHavingAlias != null && namespaceAliases != null)
        {
          foreach (string str1 in namespacesHavingAlias)
          {
            string str2;
            if (namespaceAliases.TryGetValue(str1, out str2) && !hashSetInternal.Add(str2))
              errors.Add(new EdmError(this.Location(), EdmErrorCode.DuplicateAlias, Strings.CsdlSemantics_DuplicateAlias((object) str1, (object) str2)));
          }
        }
        foreach (CsdlSemanticsSchema schema in this.schemata)
          errors.AddRange(schema.Errors());
        return (IEnumerable<EdmError>) errors;
      }
    }

    internal CsdlSemanticsModel MainModel => this.mainEdmModel;

    public override IEnumerable<IEdmVocabularyAnnotation> FindDeclaredVocabularyAnnotations(
      IEdmVocabularyAnnotatable element)
    {
      IEnumerable<IEdmVocabularyAnnotation> first = !(element is CsdlSemanticsElement semanticsElement) || semanticsElement.Model != this ? Enumerable.Empty<IEdmVocabularyAnnotation>() : semanticsElement.InlineVocabularyAnnotations;
      string key = EdmUtil.FullyQualifiedName(element);
      List<CsdlSemanticsAnnotations> semanticsAnnotationsList;
      if (key == null || !this.outOfLineAnnotations.TryGetValue(key, out semanticsAnnotationsList))
        return first;
      List<IEdmVocabularyAnnotation> second = new List<IEdmVocabularyAnnotation>();
      foreach (CsdlSemanticsAnnotations annotationsContext in semanticsAnnotationsList)
      {
        foreach (CsdlAnnotation annotation1 in annotationsContext.Annotations.Annotations)
        {
          IEdmVocabularyAnnotation annotation2 = this.WrapVocabularyAnnotation(annotation1, annotationsContext.Context, (IEdmVocabularyAnnotatable) null, annotationsContext, annotationsContext.Annotations.Qualifier);
          annotation2.SetSerializationLocation((IEdmModel) this, new EdmVocabularyAnnotationSerializationLocation?(EdmVocabularyAnnotationSerializationLocation.OutOfLine));
          second.Add(annotation2);
        }
      }
      return first.Concat<IEdmVocabularyAnnotation>((IEnumerable<IEdmVocabularyAnnotation>) second);
    }

    public override IEnumerable<IEdmStructuredType> FindDirectlyDerivedTypes(
      IEdmStructuredType baseType)
    {
      List<IEdmStructuredType> directlyDerivedTypes = new List<IEdmStructuredType>();
      List<IEdmStructuredType> source;
      if (this.derivedTypeMappings.TryGetValue(((IEdmNamedElement) baseType).Name, out source))
        directlyDerivedTypes.AddRange(source.Where<IEdmStructuredType>((Func<IEdmStructuredType, bool>) (t => t.BaseType == baseType)));
      foreach (IEdmModel referencedModel in this.ReferencedModels)
        directlyDerivedTypes.AddRange(referencedModel.FindDirectlyDerivedTypes(baseType));
      return (IEnumerable<IEdmStructuredType>) directlyDerivedTypes;
    }

    internal void AddToReferencedModels(IEnumerable<IEdmModel> models)
    {
      foreach (IEdmModel model in models)
        this.AddReferencedModel(model);
    }

    internal static IEdmExpression WrapExpression(
      CsdlExpressionBase expression,
      IEdmEntityType bindingContext,
      CsdlSemanticsSchema schema)
    {
      if (expression != null)
      {
        switch (expression.ExpressionKind)
        {
          case EdmExpressionKind.BinaryConstant:
            return (IEdmExpression) new CsdlSemanticsBinaryConstantExpression((CsdlConstantExpression) expression, schema);
          case EdmExpressionKind.BooleanConstant:
            return (IEdmExpression) new CsdlSemanticsBooleanConstantExpression((CsdlConstantExpression) expression, schema);
          case EdmExpressionKind.DateTimeOffsetConstant:
            return (IEdmExpression) new CsdlSemanticsDateTimeOffsetConstantExpression((CsdlConstantExpression) expression, schema);
          case EdmExpressionKind.DecimalConstant:
            return (IEdmExpression) new CsdlSemanticsDecimalConstantExpression((CsdlConstantExpression) expression, schema);
          case EdmExpressionKind.FloatingConstant:
            return (IEdmExpression) new CsdlSemanticsFloatingConstantExpression((CsdlConstantExpression) expression, schema);
          case EdmExpressionKind.GuidConstant:
            return (IEdmExpression) new CsdlSemanticsGuidConstantExpression((CsdlConstantExpression) expression, schema);
          case EdmExpressionKind.IntegerConstant:
            return (IEdmExpression) new CsdlSemanticsIntConstantExpression((CsdlConstantExpression) expression, schema);
          case EdmExpressionKind.StringConstant:
            return (IEdmExpression) new CsdlSemanticsStringConstantExpression((CsdlConstantExpression) expression, schema);
          case EdmExpressionKind.DurationConstant:
            return (IEdmExpression) new CsdlSemanticsDurationConstantExpression((CsdlConstantExpression) expression, schema);
          case EdmExpressionKind.Null:
            return (IEdmExpression) new CsdlSemanticsNullExpression((CsdlConstantExpression) expression, schema);
          case EdmExpressionKind.Record:
            return (IEdmExpression) new CsdlSemanticsRecordExpression((CsdlRecordExpression) expression, bindingContext, schema);
          case EdmExpressionKind.Collection:
            return (IEdmExpression) new CsdlSemanticsCollectionExpression((CsdlCollectionExpression) expression, bindingContext, schema);
          case EdmExpressionKind.Path:
            return (IEdmExpression) new CsdlSemanticsPathExpression((CsdlPathExpression) expression, bindingContext, schema);
          case EdmExpressionKind.If:
            return (IEdmExpression) new CsdlSemanticsIfExpression((CsdlIfExpression) expression, bindingContext, schema);
          case EdmExpressionKind.Cast:
            return (IEdmExpression) new CsdlSemanticsCastExpression((CsdlCastExpression) expression, bindingContext, schema);
          case EdmExpressionKind.IsType:
            return (IEdmExpression) new CsdlSemanticsIsTypeExpression((CsdlIsTypeExpression) expression, bindingContext, schema);
          case EdmExpressionKind.FunctionApplication:
            return (IEdmExpression) new CsdlSemanticsApplyExpression((CsdlApplyExpression) expression, bindingContext, schema);
          case EdmExpressionKind.LabeledExpressionReference:
            return (IEdmExpression) new CsdlSemanticsLabeledExpressionReferenceExpression((CsdlLabeledExpressionReferenceExpression) expression, bindingContext, schema);
          case EdmExpressionKind.Labeled:
            return (IEdmExpression) schema.WrapLabeledElement((CsdlLabeledExpression) expression, bindingContext);
          case EdmExpressionKind.PropertyPath:
            return (IEdmExpression) new CsdlSemanticsPropertyPathExpression((CsdlPathExpression) expression, bindingContext, schema);
          case EdmExpressionKind.NavigationPropertyPath:
            return (IEdmExpression) new CsdlSemanticsNavigationPropertyPathExpression((CsdlPathExpression) expression, bindingContext, schema);
          case EdmExpressionKind.DateConstant:
            return (IEdmExpression) new CsdlSemanticsDateConstantExpression((CsdlConstantExpression) expression, schema);
          case EdmExpressionKind.TimeOfDayConstant:
            return (IEdmExpression) new CsdlSemanticsTimeOfDayConstantExpression((CsdlConstantExpression) expression, schema);
          case EdmExpressionKind.EnumMember:
            return (IEdmExpression) new CsdlSemanticsEnumMemberExpression((CsdlEnumMemberExpression) expression, bindingContext, schema);
          case EdmExpressionKind.AnnotationPath:
            return (IEdmExpression) new CsdlSemanticsAnnotationPathExpression((CsdlPathExpression) expression, bindingContext, schema);
        }
      }
      return (IEdmExpression) null;
    }

    internal static IEdmTypeReference WrapTypeReference(
      CsdlSemanticsSchema schema,
      CsdlTypeReference type)
    {
      switch (type)
      {
        case CsdlNamedTypeReference reference2:
          if (reference2 is CsdlPrimitiveTypeReference reference1)
          {
            switch (reference1.Kind)
            {
              case EdmPrimitiveTypeKind.Binary:
                return (IEdmTypeReference) new CsdlSemanticsBinaryTypeReference(schema, (CsdlBinaryTypeReference) reference1);
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
                return (IEdmTypeReference) new CsdlSemanticsPrimitiveTypeReference(schema, reference1);
              case EdmPrimitiveTypeKind.DateTimeOffset:
              case EdmPrimitiveTypeKind.Duration:
              case EdmPrimitiveTypeKind.TimeOfDay:
                return (IEdmTypeReference) new CsdlSemanticsTemporalTypeReference(schema, (CsdlTemporalTypeReference) reference1);
              case EdmPrimitiveTypeKind.Decimal:
                return (IEdmTypeReference) new CsdlSemanticsDecimalTypeReference(schema, (CsdlDecimalTypeReference) reference1);
              case EdmPrimitiveTypeKind.String:
                return (IEdmTypeReference) new CsdlSemanticsStringTypeReference(schema, (CsdlStringTypeReference) reference1);
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
                return (IEdmTypeReference) new CsdlSemanticsSpatialTypeReference(schema, (CsdlSpatialTypeReference) reference1);
            }
          }
          else
          {
            if (reference2 is CsdlUntypedTypeReference reference)
              return (IEdmTypeReference) new CsdlSemanticsUntypedTypeReference(schema, reference);
            if (schema.FindType(reference2.FullName) is IEdmTypeDefinition)
              return (IEdmTypeReference) new CsdlSemanticsTypeDefinitionReference(schema, reference2);
          }
          return (IEdmTypeReference) new CsdlSemanticsNamedTypeReference(schema, reference2);
        case CsdlExpressionTypeReference expressionUsage:
          if (expressionUsage.TypeExpression is CsdlCollectionType typeExpression1)
            return (IEdmTypeReference) new CsdlSemanticsCollectionTypeExpression(expressionUsage, (CsdlSemanticsTypeDefinition) new CsdlSemanticsCollectionTypeDefinition(schema, typeExpression1));
          if (expressionUsage.TypeExpression is CsdlEntityReferenceType typeExpression2)
            return (IEdmTypeReference) new CsdlSemanticsEntityReferenceTypeExpression(expressionUsage, (CsdlSemanticsTypeDefinition) new CsdlSemanticsEntityReferenceTypeDefinition(schema, typeExpression2));
          break;
      }
      return (IEdmTypeReference) null;
    }

    internal IEnumerable<IEdmVocabularyAnnotation> WrapInlineVocabularyAnnotations(
      CsdlSemanticsElement element,
      CsdlSemanticsSchema schema)
    {
      if (element is IEdmVocabularyAnnotatable targetContext)
      {
        IEnumerable<CsdlAnnotation> vocabularyAnnotations = element.Element.VocabularyAnnotations;
        if (vocabularyAnnotations.FirstOrDefault<CsdlAnnotation>() != null)
        {
          List<IEdmVocabularyAnnotation> vocabularyAnnotationList = new List<IEdmVocabularyAnnotation>();
          foreach (CsdlAnnotation annotation1 in vocabularyAnnotations)
          {
            IEdmVocabularyAnnotation annotation2 = this.WrapVocabularyAnnotation(annotation1, schema, targetContext, (CsdlSemanticsAnnotations) null, annotation1.Qualifier);
            annotation2.SetSerializationLocation((IEdmModel) this, new EdmVocabularyAnnotationSerializationLocation?(EdmVocabularyAnnotationSerializationLocation.Inline));
            vocabularyAnnotationList.Add(annotation2);
          }
          return (IEnumerable<IEdmVocabularyAnnotation>) vocabularyAnnotationList;
        }
      }
      return Enumerable.Empty<IEdmVocabularyAnnotation>();
    }

    private IEdmVocabularyAnnotation WrapVocabularyAnnotation(
      CsdlAnnotation annotation,
      CsdlSemanticsSchema schema,
      IEdmVocabularyAnnotatable targetContext,
      CsdlSemanticsAnnotations annotationsContext,
      string qualifier)
    {
      return (IEdmVocabularyAnnotation) EdmUtil.DictionaryGetOrUpdate<CsdlAnnotation, CsdlSemanticsVocabularyAnnotation>((IDictionary<CsdlAnnotation, CsdlSemanticsVocabularyAnnotation>) this.wrappedAnnotations, annotation, (Func<CsdlAnnotation, CsdlSemanticsVocabularyAnnotation>) (ann => new CsdlSemanticsVocabularyAnnotation(schema, targetContext, annotationsContext, ann, qualifier)));
    }

    private void AddSchema(CsdlSchema schema) => this.AddSchema(schema, true);

    private void AddSchema(CsdlSchema schema, bool addAnnotations)
    {
      CsdlSemanticsSchema context = new CsdlSemanticsSchema(this, schema);
      this.schemata.Add(context);
      foreach (IEdmSchemaType type in context.Types)
      {
        if (type is CsdlSemanticsStructuredTypeDefinition structuredTypeDefinition)
        {
          string baseTypeName = ((CsdlNamedStructuredType) structuredTypeDefinition.Element).BaseTypeName;
          if (baseTypeName != null)
          {
            string name;
            EdmUtil.TryGetNamespaceNameFromQualifiedName(baseTypeName, out string _, out name);
            if (name != null)
            {
              List<IEdmStructuredType> edmStructuredTypeList;
              if (!this.derivedTypeMappings.TryGetValue(name, out edmStructuredTypeList))
              {
                edmStructuredTypeList = new List<IEdmStructuredType>();
                this.derivedTypeMappings[name] = edmStructuredTypeList;
              }
              edmStructuredTypeList.Add((IEdmStructuredType) structuredTypeDefinition);
            }
          }
        }
        this.RegisterElement((IEdmSchemaElement) type);
      }
      foreach (IEdmSchemaElement operation in context.Operations)
        this.RegisterElement(operation);
      foreach (IEdmSchemaElement term in context.Terms)
        this.RegisterElement(term);
      foreach (IEdmSchemaElement entityContainer in context.EntityContainers)
        this.RegisterElement(entityContainer);
      if (!string.IsNullOrEmpty(schema.Alias))
        this.SetNamespaceAlias(schema.Namespace, schema.Alias);
      if (addAnnotations)
      {
        foreach (CsdlAnnotations ofLineAnnotation in schema.OutOfLineAnnotations)
        {
          string key = this.ReplaceAlias(ofLineAnnotation.Target);
          List<CsdlSemanticsAnnotations> semanticsAnnotationsList;
          if (!this.outOfLineAnnotations.TryGetValue(key, out semanticsAnnotationsList))
          {
            semanticsAnnotationsList = new List<CsdlSemanticsAnnotations>();
            this.outOfLineAnnotations[key] = semanticsAnnotationsList;
          }
          semanticsAnnotationsList.Add(new CsdlSemanticsAnnotations(context, ofLineAnnotation));
        }
      }
      Version edmVersion = this.GetEdmVersion();
      if (!(edmVersion == (Version) null) && !(edmVersion < schema.Version))
        return;
      this.SetEdmVersion(schema.Version);
    }
  }
}
