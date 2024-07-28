// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.Edm.Csdl.CsdlSemantics.CsdlSemanticsSchema
// Assembly: Microsoft.TeamFoundation.OData.Edm, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 75C95BF5-2544-413A-959F-F9F18C11D35F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Edm.dll

using Microsoft.OData.Edm.Csdl.Parsing.Ast;
using Microsoft.OData.Edm.Validation;
using Microsoft.OData.Edm.Vocabularies;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.OData.Edm.Csdl.CsdlSemantics
{
  internal class CsdlSemanticsSchema : CsdlSemanticsElement, IEdmCheckable
  {
    private readonly CsdlSemanticsModel model;
    private readonly CsdlSchema schema;
    private readonly Cache<CsdlSemanticsSchema, IEnumerable<IEdmSchemaType>> typesCache = new Cache<CsdlSemanticsSchema, IEnumerable<IEdmSchemaType>>();
    private static readonly Func<CsdlSemanticsSchema, IEnumerable<IEdmSchemaType>> ComputeTypesFunc = (Func<CsdlSemanticsSchema, IEnumerable<IEdmSchemaType>>) (me => me.ComputeTypes());
    private readonly Cache<CsdlSemanticsSchema, IEnumerable<IEdmOperation>> operationsCache = new Cache<CsdlSemanticsSchema, IEnumerable<IEdmOperation>>();
    private static readonly Func<CsdlSemanticsSchema, IEnumerable<IEdmOperation>> ComputeFunctionsFunc = (Func<CsdlSemanticsSchema, IEnumerable<IEdmOperation>>) (me => me.ComputeOperations());
    private readonly Cache<CsdlSemanticsSchema, IEnumerable<IEdmEntityContainer>> entityContainersCache = new Cache<CsdlSemanticsSchema, IEnumerable<IEdmEntityContainer>>();
    private static readonly Func<CsdlSemanticsSchema, IEnumerable<IEdmEntityContainer>> ComputeEntityContainersFunc = (Func<CsdlSemanticsSchema, IEnumerable<IEdmEntityContainer>>) (me => me.ComputeEntityContainers());
    private readonly Cache<CsdlSemanticsSchema, IEnumerable<IEdmTerm>> termsCache = new Cache<CsdlSemanticsSchema, IEnumerable<IEdmTerm>>();
    private static readonly Func<CsdlSemanticsSchema, IEnumerable<IEdmTerm>> ComputeTermsFunc = (Func<CsdlSemanticsSchema, IEnumerable<IEdmTerm>>) (me => me.ComputeTerms());
    private readonly Cache<CsdlSemanticsSchema, Dictionary<string, object>> labeledExpressionsCache = new Cache<CsdlSemanticsSchema, Dictionary<string, object>>();
    private static readonly Func<CsdlSemanticsSchema, Dictionary<string, object>> ComputeLabeledExpressionsFunc = (Func<CsdlSemanticsSchema, Dictionary<string, object>>) (me => me.ComputeLabeledExpressions());
    private readonly Dictionary<CsdlLabeledExpression, IEdmLabeledExpression> semanticsLabeledElements = new Dictionary<CsdlLabeledExpression, IEdmLabeledExpression>();
    private readonly Dictionary<List<CsdlLabeledExpression>, IEdmLabeledExpression> ambiguousLabeledExpressions = new Dictionary<List<CsdlLabeledExpression>, IEdmLabeledExpression>();

    public CsdlSemanticsSchema(CsdlSemanticsModel model, CsdlSchema schema)
      : base((CsdlElement) schema)
    {
      this.model = model;
      this.schema = schema;
    }

    public override CsdlSemanticsModel Model => this.model;

    public override CsdlElement Element => (CsdlElement) this.schema;

    public IEnumerable<IEdmSchemaType> Types => this.typesCache.GetValue(this, CsdlSemanticsSchema.ComputeTypesFunc, (Func<CsdlSemanticsSchema, IEnumerable<IEdmSchemaType>>) null);

    public IEnumerable<IEdmOperation> Operations => this.operationsCache.GetValue(this, CsdlSemanticsSchema.ComputeFunctionsFunc, (Func<CsdlSemanticsSchema, IEnumerable<IEdmOperation>>) null);

    public IEnumerable<IEdmTerm> Terms => this.termsCache.GetValue(this, CsdlSemanticsSchema.ComputeTermsFunc, (Func<CsdlSemanticsSchema, IEnumerable<IEdmTerm>>) null);

    public IEnumerable<IEdmEntityContainer> EntityContainers => this.entityContainersCache.GetValue(this, CsdlSemanticsSchema.ComputeEntityContainersFunc, (Func<CsdlSemanticsSchema, IEnumerable<IEdmEntityContainer>>) null);

    public string Namespace => this.schema.Namespace;

    public IEnumerable<EdmError> Errors => Enumerable.Empty<EdmError>();

    private Dictionary<string, object> LabeledExpressions => this.labeledExpressionsCache.GetValue(this, CsdlSemanticsSchema.ComputeLabeledExpressionsFunc, (Func<CsdlSemanticsSchema, Dictionary<string, object>>) null);

    public IEnumerable<IEdmOperation> FindOperations(string name) => this.FindSchemaElement<IEnumerable<IEdmOperation>>(name, new Func<CsdlSemanticsModel, string, IEnumerable<IEdmOperation>>(ExtensionMethods.FindOperationsInModelTree));

    public IEdmSchemaType FindType(string name) => this.FindSchemaElement<IEdmSchemaType>(name, new Func<CsdlSemanticsModel, string, IEdmSchemaType>(ExtensionMethods.FindTypeInModelTree));

    public IEdmTerm FindTerm(string name) => this.FindSchemaElement<IEdmTerm>(name, new Func<CsdlSemanticsModel, string, IEdmTerm>(CsdlSemanticsSchema.FindTerm));

    public IEdmEntityContainer FindEntityContainer(string name) => this.FindSchemaElement<IEdmEntityContainer>(name, new Func<CsdlSemanticsModel, string, IEdmEntityContainer>(CsdlSemanticsSchema.FindEntityContainer));

    public T FindSchemaElement<T>(string name, Func<CsdlSemanticsModel, string, T> modelFinder)
    {
      string str = this.ReplaceAlias(name);
      return modelFinder(this.model, str);
    }

    public string UnresolvedName(string qualifiedName) => qualifiedName == null ? (string) null : this.ReplaceAlias(qualifiedName);

    public IEdmLabeledExpression FindLabeledElement(string label, IEdmEntityType bindingContext)
    {
      object labeledExpressions;
      if (!this.LabeledExpressions.TryGetValue(label, out labeledExpressions))
        return (IEdmLabeledExpression) null;
      return labeledExpressions is CsdlLabeledExpression labeledElement ? this.WrapLabeledElement(labeledElement, bindingContext) : this.WrapLabeledElementList((List<CsdlLabeledExpression>) labeledExpressions, bindingContext);
    }

    public IEdmLabeledExpression WrapLabeledElement(
      CsdlLabeledExpression labeledElement,
      IEdmEntityType bindingContext)
    {
      IEdmLabeledExpression labeledExpression;
      if (!this.semanticsLabeledElements.TryGetValue(labeledElement, out labeledExpression))
      {
        labeledExpression = (IEdmLabeledExpression) new CsdlSemanticsLabeledExpression(labeledElement.Label, labeledElement.Element, bindingContext, this);
        this.semanticsLabeledElements[labeledElement] = labeledExpression;
      }
      return labeledExpression;
    }

    internal string ReplaceAlias(string name) => this.model.ReplaceAlias(name);

    private static IEdmTerm FindTerm(IEdmModel model, string name) => model.FindTerm(name);

    private static IEdmEntityContainer FindEntityContainer(IEdmModel model, string name) => model.FindEntityContainer(name);

    private static void AddLabeledExpressions(
      CsdlExpressionBase expression,
      Dictionary<string, object> result)
    {
      if (expression == null)
        return;
      switch (expression.ExpressionKind)
      {
        case EdmExpressionKind.Record:
          using (IEnumerator<CsdlPropertyValue> enumerator = ((CsdlRecordExpression) expression).PropertyValues.GetEnumerator())
          {
            while (enumerator.MoveNext())
              CsdlSemanticsSchema.AddLabeledExpressions(enumerator.Current.Expression, result);
            break;
          }
        case EdmExpressionKind.Collection:
          using (IEnumerator<CsdlExpressionBase> enumerator = ((CsdlCollectionExpression) expression).ElementValues.GetEnumerator())
          {
            while (enumerator.MoveNext())
              CsdlSemanticsSchema.AddLabeledExpressions(enumerator.Current, result);
            break;
          }
        case EdmExpressionKind.If:
          CsdlIfExpression csdlIfExpression = (CsdlIfExpression) expression;
          CsdlSemanticsSchema.AddLabeledExpressions(csdlIfExpression.Test, result);
          CsdlSemanticsSchema.AddLabeledExpressions(csdlIfExpression.IfTrue, result);
          CsdlSemanticsSchema.AddLabeledExpressions(csdlIfExpression.IfFalse, result);
          break;
        case EdmExpressionKind.Cast:
          CsdlSemanticsSchema.AddLabeledExpressions(((CsdlCastExpression) expression).Operand, result);
          break;
        case EdmExpressionKind.IsType:
          CsdlSemanticsSchema.AddLabeledExpressions(((CsdlIsTypeExpression) expression).Operand, result);
          break;
        case EdmExpressionKind.FunctionApplication:
          using (IEnumerator<CsdlExpressionBase> enumerator = ((CsdlApplyExpression) expression).Arguments.GetEnumerator())
          {
            while (enumerator.MoveNext())
              CsdlSemanticsSchema.AddLabeledExpressions(enumerator.Current, result);
            break;
          }
        case EdmExpressionKind.Labeled:
          CsdlLabeledExpression labeledExpression = (CsdlLabeledExpression) expression;
          string label = labeledExpression.Label;
          object obj;
          if (result.TryGetValue(label, out obj))
          {
            if (!(obj is List<CsdlLabeledExpression> labeledExpressionList))
            {
              labeledExpressionList = new List<CsdlLabeledExpression>();
              labeledExpressionList.Add((CsdlLabeledExpression) obj);
              result[label] = (object) labeledExpressionList;
            }
            labeledExpressionList.Add(labeledExpression);
          }
          else
            result[label] = (object) labeledExpression;
          CsdlSemanticsSchema.AddLabeledExpressions(labeledExpression.Element, result);
          break;
      }
    }

    private static void AddLabeledExpressions(
      IEnumerable<CsdlAnnotation> annotations,
      Dictionary<string, object> result)
    {
      foreach (CsdlAnnotation annotation in annotations)
      {
        if (annotation != null)
          CsdlSemanticsSchema.AddLabeledExpressions(annotation.Expression, result);
      }
    }

    private IEdmLabeledExpression WrapLabeledElementList(
      List<CsdlLabeledExpression> labeledExpressions,
      IEdmEntityType bindingContext)
    {
      IEdmLabeledExpression first;
      if (!this.ambiguousLabeledExpressions.TryGetValue(labeledExpressions, out first))
      {
        foreach (CsdlLabeledExpression labeledExpression in labeledExpressions)
        {
          IEdmLabeledExpression second = this.WrapLabeledElement(labeledExpression, bindingContext);
          first = first == null ? second : (IEdmLabeledExpression) new AmbiguousLabeledExpressionBinding(first, second);
        }
        this.ambiguousLabeledExpressions[labeledExpressions] = first;
      }
      return first;
    }

    private IEnumerable<IEdmTerm> ComputeTerms()
    {
      List<IEdmTerm> terms = new List<IEdmTerm>();
      foreach (CsdlTerm term in this.schema.Terms)
        terms.Add((IEdmTerm) new CsdlSemanticsTerm(this, term));
      return (IEnumerable<IEdmTerm>) terms;
    }

    private IEnumerable<IEdmEntityContainer> ComputeEntityContainers()
    {
      List<IEdmEntityContainer> entityContainers = new List<IEdmEntityContainer>();
      foreach (CsdlEntityContainer entityContainer in this.schema.EntityContainers)
        entityContainers.Add((IEdmEntityContainer) new CsdlSemanticsEntityContainer(this, entityContainer));
      return (IEnumerable<IEdmEntityContainer>) entityContainers;
    }

    private IEnumerable<IEdmOperation> ComputeOperations()
    {
      List<IEdmOperation> operations = new List<IEdmOperation>();
      foreach (CsdlOperation operation in this.schema.Operations)
      {
        if (operation is CsdlAction action)
        {
          operations.Add((IEdmOperation) new CsdlSemanticsAction(this, action));
        }
        else
        {
          CsdlFunction function = operation as CsdlFunction;
          operations.Add((IEdmOperation) new CsdlSemanticsFunction(this, function));
        }
      }
      return (IEnumerable<IEdmOperation>) operations;
    }

    private IEnumerable<IEdmSchemaType> ComputeTypes()
    {
      List<IEdmSchemaType> types = new List<IEdmSchemaType>();
      foreach (CsdlTypeDefinition typeDefinition in this.schema.TypeDefinitions)
      {
        CsdlSemanticsTypeDefinitionDefinition definitionDefinition = new CsdlSemanticsTypeDefinitionDefinition(this, typeDefinition);
        this.AttachDefaultPrimitiveValueConverter(typeDefinition, (IEdmTypeDefinition) definitionDefinition);
        types.Add((IEdmSchemaType) definitionDefinition);
      }
      foreach (CsdlStructuredType structuredType in this.schema.StructuredTypes)
      {
        if (structuredType is CsdlEntityType entity)
          types.Add((IEdmSchemaType) new CsdlSemanticsEntityTypeDefinition(this, entity));
        else if (structuredType is CsdlComplexType complex)
          types.Add((IEdmSchemaType) new CsdlSemanticsComplexTypeDefinition(this, complex));
      }
      foreach (CsdlEnumType enumType in this.schema.EnumTypes)
        types.Add((IEdmSchemaType) new CsdlSemanticsEnumTypeDefinition(this, enumType));
      return (IEnumerable<IEdmSchemaType>) types;
    }

    private void AttachDefaultPrimitiveValueConverter(
      CsdlTypeDefinition typeDefinition,
      IEdmTypeDefinition edmTypeDefinition)
    {
      string strA;
      switch (typeDefinition.Name)
      {
        case "UInt16":
          strA = "Edm.Int32";
          break;
        case "UInt32":
          strA = "Edm.Int64";
          break;
        case "UInt64":
          strA = "Edm.Decimal";
          break;
        default:
          return;
      }
      if (string.CompareOrdinal(strA, typeDefinition.UnderlyingTypeName) != 0)
        return;
      this.Model.SetPrimitiveValueConverter((IEdmType) edmTypeDefinition, DefaultPrimitiveValueConverter.Instance);
    }

    private Dictionary<string, object> ComputeLabeledExpressions()
    {
      Dictionary<string, object> result = new Dictionary<string, object>();
      foreach (CsdlAnnotations ofLineAnnotation in this.schema.OutOfLineAnnotations)
        CsdlSemanticsSchema.AddLabeledExpressions(ofLineAnnotation.Annotations, result);
      foreach (CsdlStructuredType structuredType in this.schema.StructuredTypes)
      {
        CsdlSemanticsSchema.AddLabeledExpressions(structuredType.VocabularyAnnotations, result);
        foreach (CsdlElement structuralProperty in structuredType.StructuralProperties)
          CsdlSemanticsSchema.AddLabeledExpressions(structuralProperty.VocabularyAnnotations, result);
      }
      foreach (CsdlOperation operation in this.schema.Operations)
      {
        CsdlSemanticsSchema.AddLabeledExpressions(operation.VocabularyAnnotations, result);
        foreach (CsdlElement parameter in operation.Parameters)
          CsdlSemanticsSchema.AddLabeledExpressions(parameter.VocabularyAnnotations, result);
      }
      foreach (CsdlElement term in this.schema.Terms)
        CsdlSemanticsSchema.AddLabeledExpressions(term.VocabularyAnnotations, result);
      foreach (CsdlEntityContainer entityContainer in this.schema.EntityContainers)
      {
        CsdlSemanticsSchema.AddLabeledExpressions(entityContainer.VocabularyAnnotations, result);
        foreach (CsdlElement entitySet in entityContainer.EntitySets)
          CsdlSemanticsSchema.AddLabeledExpressions(entitySet.VocabularyAnnotations, result);
        foreach (CsdlOperationImport operationImport in entityContainer.OperationImports)
        {
          CsdlSemanticsSchema.AddLabeledExpressions(operationImport.VocabularyAnnotations, result);
          foreach (CsdlElement parameter in operationImport.Parameters)
            CsdlSemanticsSchema.AddLabeledExpressions(parameter.VocabularyAnnotations, result);
        }
      }
      return result;
    }
  }
}
