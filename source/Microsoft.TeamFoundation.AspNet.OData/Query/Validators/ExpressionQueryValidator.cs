// Decompiled with JetBrains decompiler
// Type: Microsoft.AspNet.OData.Query.Validators.ExpressionQueryValidator
// Assembly: Microsoft.TeamFoundation.AspNet.OData, Version=7.3.1.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 08758328-0988-4E6C-88EC-9BA90EA42587
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.AspNet.OData.dll

using Microsoft.AspNet.OData.Common;
using Microsoft.AspNet.OData.Formatter;
using Microsoft.OData;
using Microsoft.OData.Edm;
using Microsoft.OData.UriParser;
using System.Runtime.CompilerServices;

namespace Microsoft.AspNet.OData.Query.Validators
{
  public abstract class ExpressionQueryValidator
  {
    private readonly DefaultQuerySettings _defaultQuerySettings;

    protected int CurrentAnyAllExpressionDepth { get; set; }

    protected int CurrentNodeCount { get; set; }

    protected IEdmProperty Property { get; set; }

    protected IEdmStructuredType StructuredType { get; set; }

    protected IEdmModel Model { get; set; }

    protected ExpressionQueryValidator(DefaultQuerySettings defaultQuerySettings) => this._defaultQuerySettings = defaultQuerySettings;

    public virtual void ValidateAllNode(AllNode allNode, ODataValidationSettings settings)
    {
      if (allNode == null)
        throw Microsoft.AspNet.OData.Common.Error.ArgumentNull(nameof (allNode));
      if (settings == null)
        throw Microsoft.AspNet.OData.Common.Error.ArgumentNull(nameof (settings));
      ExpressionQueryValidator.ValidateFunction("all", settings);
      this.EnterLambda(settings);
      try
      {
        this.ValidateQueryNode((QueryNode) allNode.Source, settings);
        this.ValidateQueryNode((QueryNode) allNode.Body, settings);
      }
      finally
      {
        this.ExitLambda();
      }
    }

    public virtual void ValidateAnyNode(AnyNode anyNode, ODataValidationSettings settings)
    {
      if (anyNode == null)
        throw Microsoft.AspNet.OData.Common.Error.ArgumentNull(nameof (anyNode));
      if (settings == null)
        throw Microsoft.AspNet.OData.Common.Error.ArgumentNull(nameof (settings));
      ExpressionQueryValidator.ValidateFunction("any", settings);
      this.EnterLambda(settings);
      try
      {
        this.ValidateQueryNode((QueryNode) anyNode.Source, settings);
        if (anyNode.Body == null || anyNode.Body.Kind == QueryNodeKind.Constant)
          return;
        this.ValidateQueryNode((QueryNode) anyNode.Body, settings);
      }
      finally
      {
        this.ExitLambda();
      }
    }

    public virtual void ValidateBinaryOperatorNode(
      BinaryOperatorNode binaryOperatorNode,
      ODataValidationSettings settings)
    {
      if (binaryOperatorNode == null)
        throw Microsoft.AspNet.OData.Common.Error.ArgumentNull(nameof (binaryOperatorNode));
      if (settings == null)
        throw Microsoft.AspNet.OData.Common.Error.ArgumentNull(nameof (settings));
      switch (binaryOperatorNode.OperatorKind)
      {
        case BinaryOperatorKind.Or:
        case BinaryOperatorKind.And:
        case BinaryOperatorKind.Equal:
        case BinaryOperatorKind.NotEqual:
        case BinaryOperatorKind.GreaterThan:
        case BinaryOperatorKind.GreaterThanOrEqual:
        case BinaryOperatorKind.LessThan:
        case BinaryOperatorKind.LessThanOrEqual:
        case BinaryOperatorKind.Has:
          this.ValidateLogicalOperator(binaryOperatorNode, settings);
          break;
        default:
          this.ValidateArithmeticOperator(binaryOperatorNode, settings);
          break;
      }
    }

    public virtual void ValidateLogicalOperator(
      BinaryOperatorNode binaryNode,
      ODataValidationSettings settings)
    {
      if (binaryNode == null)
        throw Microsoft.AspNet.OData.Common.Error.ArgumentNull(nameof (binaryNode));
      if (settings == null)
        throw Microsoft.AspNet.OData.Common.Error.ArgumentNull(nameof (settings));
      AllowedLogicalOperators logicalOperator = ExpressionQueryValidator.ToLogicalOperator(binaryNode);
      if ((settings.AllowedLogicalOperators & logicalOperator) != logicalOperator)
        throw new ODataException(Microsoft.AspNet.OData.Common.Error.Format(SRResources.NotAllowedLogicalOperator, (object) logicalOperator, (object) "AllowedLogicalOperators"));
      this.ValidateQueryNode((QueryNode) binaryNode.Left, settings);
      this.ValidateQueryNode((QueryNode) binaryNode.Right, settings);
    }

    public virtual void ValidateArithmeticOperator(
      BinaryOperatorNode binaryNode,
      ODataValidationSettings settings)
    {
      if (binaryNode == null)
        throw Microsoft.AspNet.OData.Common.Error.ArgumentNull(nameof (binaryNode));
      if (settings == null)
        throw Microsoft.AspNet.OData.Common.Error.ArgumentNull(nameof (settings));
      AllowedArithmeticOperators arithmeticOperator = ExpressionQueryValidator.ToArithmeticOperator(binaryNode);
      if ((settings.AllowedArithmeticOperators & arithmeticOperator) != arithmeticOperator)
        throw new ODataException(Microsoft.AspNet.OData.Common.Error.Format(SRResources.NotAllowedArithmeticOperator, (object) arithmeticOperator, (object) "AllowedArithmeticOperators"));
      this.ValidateQueryNode((QueryNode) binaryNode.Left, settings);
      this.ValidateQueryNode((QueryNode) binaryNode.Right, settings);
    }

    public virtual void ValidateConstantNode(
      ConstantNode constantNode,
      ODataValidationSettings settings)
    {
      if (constantNode == null)
        throw Microsoft.AspNet.OData.Common.Error.ArgumentNull(nameof (constantNode));
      if (settings == null)
        throw Microsoft.AspNet.OData.Common.Error.ArgumentNull(nameof (settings));
    }

    public virtual void ValidateConvertNode(
      ConvertNode convertNode,
      ODataValidationSettings settings)
    {
      if (convertNode == null)
        throw Microsoft.AspNet.OData.Common.Error.ArgumentNull(nameof (convertNode));
      if (settings == null)
        throw Microsoft.AspNet.OData.Common.Error.ArgumentNull(nameof (settings));
      this.ValidateQueryNode((QueryNode) convertNode.Source, settings);
    }

    public virtual void ValidateNavigationPropertyNode(
      QueryNode sourceNode,
      IEdmNavigationProperty navigationProperty,
      ODataValidationSettings settings)
    {
      if (settings == null)
        throw Microsoft.AspNet.OData.Common.Error.ArgumentNull(nameof (settings));
      if (EdmLibHelpers.IsNotFilterable((IEdmProperty) navigationProperty, this.Property, this.StructuredType, this.Model, this._defaultQuerySettings.EnableFilter))
        throw new ODataException(Microsoft.AspNet.OData.Common.Error.Format(SRResources.NotFilterablePropertyUsedInFilter, (object) navigationProperty.Name));
      if (sourceNode == null)
        return;
      this.ValidateQueryNode(sourceNode, settings);
    }

    public virtual void ValidateRangeVariable(
      RangeVariable rangeVariable,
      ODataValidationSettings settings)
    {
      if (rangeVariable == null)
        throw Microsoft.AspNet.OData.Common.Error.ArgumentNull(nameof (rangeVariable));
      if (settings == null)
        throw Microsoft.AspNet.OData.Common.Error.ArgumentNull(nameof (settings));
    }

    public virtual void ValidateSingleValuePropertyAccessNode(
      SingleValuePropertyAccessNode propertyAccessNode,
      ODataValidationSettings settings)
    {
      if (propertyAccessNode == null)
        throw Microsoft.AspNet.OData.Common.Error.ArgumentNull(nameof (propertyAccessNode));
      if (settings == null)
        throw Microsoft.AspNet.OData.Common.Error.ArgumentNull(nameof (settings));
      IEdmProperty property = propertyAccessNode.Property;
      bool flag = false;
      if (propertyAccessNode.Source != null)
      {
        if (propertyAccessNode.Source.Kind == QueryNodeKind.SingleNavigationNode)
        {
          SingleNavigationNode source = propertyAccessNode.Source as SingleNavigationNode;
          flag = EdmLibHelpers.IsNotFilterable(property, (IEdmProperty) source.NavigationProperty, (IEdmStructuredType) source.NavigationProperty.ToEntityType(), this.Model, this._defaultQuerySettings.EnableFilter);
        }
        else if (propertyAccessNode.Source.Kind == QueryNodeKind.SingleComplexNode)
        {
          SingleComplexNode source = propertyAccessNode.Source as SingleComplexNode;
          flag = EdmLibHelpers.IsNotFilterable(property, source.Property, property.DeclaringType, this.Model, this._defaultQuerySettings.EnableFilter);
        }
        else
          flag = EdmLibHelpers.IsNotFilterable(property, this.Property, this.StructuredType, this.Model, this._defaultQuerySettings.EnableFilter);
      }
      if (flag)
        throw new ODataException(Microsoft.AspNet.OData.Common.Error.Format(SRResources.NotFilterablePropertyUsedInFilter, (object) property.Name));
      this.ValidateQueryNode((QueryNode) propertyAccessNode.Source, settings);
    }

    public virtual void ValidateSingleComplexNode(
      SingleComplexNode singleComplexNode,
      ODataValidationSettings settings)
    {
      if (singleComplexNode == null)
        throw Microsoft.AspNet.OData.Common.Error.ArgumentNull(nameof (singleComplexNode));
      if (settings == null)
        throw Microsoft.AspNet.OData.Common.Error.ArgumentNull(nameof (settings));
      IEdmProperty property = singleComplexNode.Property;
      if (EdmLibHelpers.IsNotFilterable(property, this.Property, this.StructuredType, this.Model, this._defaultQuerySettings.EnableFilter))
        throw new ODataException(Microsoft.AspNet.OData.Common.Error.Format(SRResources.NotFilterablePropertyUsedInFilter, (object) property.Name));
      this.ValidateQueryNode((QueryNode) singleComplexNode.Source, settings);
    }

    public virtual void ValidateCollectionPropertyAccessNode(
      CollectionPropertyAccessNode propertyAccessNode,
      ODataValidationSettings settings)
    {
      if (propertyAccessNode == null)
        throw Microsoft.AspNet.OData.Common.Error.ArgumentNull(nameof (propertyAccessNode));
      if (settings == null)
        throw Microsoft.AspNet.OData.Common.Error.ArgumentNull(nameof (settings));
      IEdmProperty property = propertyAccessNode.Property;
      if (EdmLibHelpers.IsNotFilterable(property, this.Property, this.StructuredType, this.Model, this._defaultQuerySettings.EnableFilter))
        throw new ODataException(Microsoft.AspNet.OData.Common.Error.Format(SRResources.NotFilterablePropertyUsedInFilter, (object) property.Name));
      this.ValidateQueryNode((QueryNode) propertyAccessNode.Source, settings);
    }

    public virtual void ValidateCollectionComplexNode(
      CollectionComplexNode collectionComplexNode,
      ODataValidationSettings settings)
    {
      if (collectionComplexNode == null)
        throw Microsoft.AspNet.OData.Common.Error.ArgumentNull(nameof (collectionComplexNode));
      if (settings == null)
        throw Microsoft.AspNet.OData.Common.Error.ArgumentNull(nameof (settings));
      IEdmProperty property = collectionComplexNode.Property;
      if (EdmLibHelpers.IsNotFilterable(property, this.Property, this.StructuredType, this.Model, this._defaultQuerySettings.EnableFilter))
        throw new ODataException(Microsoft.AspNet.OData.Common.Error.Format(SRResources.NotFilterablePropertyUsedInFilter, (object) property.Name));
      this.ValidateQueryNode((QueryNode) collectionComplexNode.Source, settings);
    }

    public virtual void ValidateSingleValueFunctionCallNode(
      SingleValueFunctionCallNode node,
      ODataValidationSettings settings)
    {
      if (node == null)
        throw Microsoft.AspNet.OData.Common.Error.ArgumentNull(nameof (node));
      if (settings == null)
        throw Microsoft.AspNet.OData.Common.Error.ArgumentNull(nameof (settings));
      ExpressionQueryValidator.ValidateFunction(node.Name, settings);
      foreach (QueryNode parameter in node.Parameters)
        this.ValidateQueryNode(parameter, settings);
    }

    public virtual void ValidateSingleResourceFunctionCallNode(
      SingleResourceFunctionCallNode node,
      ODataValidationSettings settings)
    {
      if (node == null)
        throw Microsoft.AspNet.OData.Common.Error.ArgumentNull(nameof (node));
      if (settings == null)
        throw Microsoft.AspNet.OData.Common.Error.ArgumentNull(nameof (settings));
      ExpressionQueryValidator.ValidateFunction(node.Name, settings);
      foreach (QueryNode parameter in node.Parameters)
        this.ValidateQueryNode(parameter, settings);
    }

    public virtual void ValidateUnaryOperatorNode(
      UnaryOperatorNode unaryOperatorNode,
      ODataValidationSettings settings)
    {
      this.ValidateQueryNode((QueryNode) unaryOperatorNode.Operand, settings);
      switch (unaryOperatorNode.OperatorKind)
      {
        case UnaryOperatorKind.Negate:
        case UnaryOperatorKind.Not:
          if ((settings.AllowedLogicalOperators & AllowedLogicalOperators.Not) == AllowedLogicalOperators.Not)
            break;
          throw new ODataException(Microsoft.AspNet.OData.Common.Error.Format(SRResources.NotAllowedLogicalOperator, (object) unaryOperatorNode.OperatorKind, (object) "AllowedLogicalOperators"));
        default:
          throw Microsoft.AspNet.OData.Common.Error.NotSupported(SRResources.UnaryNodeValidationNotSupported, (object) unaryOperatorNode.OperatorKind, (object) typeof (FilterQueryValidator).Name);
      }
    }

    public virtual void ValidateCollectionResourceCastNode(
      CollectionResourceCastNode collectionResourceCastNode,
      ODataValidationSettings settings)
    {
      if (collectionResourceCastNode == null)
        throw Microsoft.AspNet.OData.Common.Error.ArgumentNull(nameof (collectionResourceCastNode));
      this.ValidateQueryNode((QueryNode) collectionResourceCastNode.Source, settings);
    }

    public virtual void ValidateSingleResourceCastNode(
      SingleResourceCastNode singleResourceCastNode,
      ODataValidationSettings settings)
    {
      if (singleResourceCastNode == null)
        throw Microsoft.AspNet.OData.Common.Error.ArgumentNull(nameof (singleResourceCastNode));
      this.ValidateQueryNode((QueryNode) singleResourceCastNode.Source, settings);
    }

    public virtual void ValidateQueryNode(QueryNode node, ODataValidationSettings settings)
    {
      RuntimeHelpers.EnsureSufficientExecutionStack();
      SingleValueNode node1 = node as SingleValueNode;
      CollectionNode node2 = node as CollectionNode;
      this.IncrementNodeCount(settings);
      if (node1 != null)
      {
        this.ValidateSingleValueNode(node1, settings);
      }
      else
      {
        if (node2 == null)
          return;
        this.ValidateCollectionNode(node2, settings);
      }
    }

    private void IncrementNodeCount(ODataValidationSettings validationSettings)
    {
      if (this.CurrentNodeCount >= validationSettings.MaxNodeCount)
        throw new ODataException(Microsoft.AspNet.OData.Common.Error.Format(SRResources.MaxNodeLimitExceeded, (object) validationSettings.MaxNodeCount, (object) "MaxNodeCount"));
      ++this.CurrentNodeCount;
    }

    private void EnterLambda(ODataValidationSettings validationSettings)
    {
      if (this.CurrentAnyAllExpressionDepth >= validationSettings.MaxAnyAllExpressionDepth)
        throw new ODataException(Microsoft.AspNet.OData.Common.Error.Format(SRResources.MaxAnyAllExpressionLimitExceeded, (object) validationSettings.MaxAnyAllExpressionDepth, (object) "MaxAnyAllExpressionDepth"));
      ++this.CurrentAnyAllExpressionDepth;
    }

    private void ExitLambda() => --this.CurrentAnyAllExpressionDepth;

    private void ValidateCollectionNode(CollectionNode node, ODataValidationSettings settings)
    {
      switch (node.Kind)
      {
        case QueryNodeKind.CollectionPropertyAccess:
          this.ValidateCollectionPropertyAccessNode(node as CollectionPropertyAccessNode, settings);
          break;
        case QueryNodeKind.CollectionNavigationNode:
          CollectionNavigationNode collectionNavigationNode = node as CollectionNavigationNode;
          this.ValidateNavigationPropertyNode((QueryNode) collectionNavigationNode.Source, collectionNavigationNode.NavigationProperty, settings);
          break;
        case QueryNodeKind.CollectionResourceCast:
          this.ValidateCollectionResourceCastNode(node as CollectionResourceCastNode, settings);
          break;
        case QueryNodeKind.CollectionResourceFunctionCall:
          break;
        case QueryNodeKind.CollectionComplexNode:
          this.ValidateCollectionComplexNode(node as CollectionComplexNode, settings);
          break;
        default:
          throw Microsoft.AspNet.OData.Common.Error.NotSupported(SRResources.QueryNodeValidationNotSupported, (object) node.Kind, (object) typeof (FilterQueryValidator).Name);
      }
    }

    private void ValidateSingleValueNode(SingleValueNode node, ODataValidationSettings settings)
    {
      switch (node.Kind)
      {
        case QueryNodeKind.Constant:
          this.ValidateConstantNode(node as ConstantNode, settings);
          break;
        case QueryNodeKind.Convert:
          this.ValidateConvertNode(node as ConvertNode, settings);
          break;
        case QueryNodeKind.NonResourceRangeVariableReference:
          this.ValidateRangeVariable((RangeVariable) (node as NonResourceRangeVariableReferenceNode).RangeVariable, settings);
          break;
        case QueryNodeKind.BinaryOperator:
          this.ValidateBinaryOperatorNode(node as BinaryOperatorNode, settings);
          break;
        case QueryNodeKind.UnaryOperator:
          this.ValidateUnaryOperatorNode(node as UnaryOperatorNode, settings);
          break;
        case QueryNodeKind.SingleValuePropertyAccess:
          this.ValidateSingleValuePropertyAccessNode(node as SingleValuePropertyAccessNode, settings);
          break;
        case QueryNodeKind.SingleValueFunctionCall:
          this.ValidateSingleValueFunctionCallNode(node as SingleValueFunctionCallNode, settings);
          break;
        case QueryNodeKind.Any:
          this.ValidateAnyNode(node as AnyNode, settings);
          break;
        case QueryNodeKind.SingleNavigationNode:
          SingleNavigationNode singleNavigationNode = node as SingleNavigationNode;
          this.ValidateNavigationPropertyNode((QueryNode) singleNavigationNode.Source, singleNavigationNode.NavigationProperty, settings);
          break;
        case QueryNodeKind.SingleValueOpenPropertyAccess:
          break;
        case QueryNodeKind.SingleResourceCast:
          this.ValidateSingleResourceCastNode(node as SingleResourceCastNode, settings);
          break;
        case QueryNodeKind.All:
          this.ValidateAllNode(node as AllNode, settings);
          break;
        case QueryNodeKind.ResourceRangeVariableReference:
          this.ValidateRangeVariable((RangeVariable) (node as ResourceRangeVariableReferenceNode).RangeVariable, settings);
          break;
        case QueryNodeKind.SingleResourceFunctionCall:
          this.ValidateSingleResourceFunctionCallNode((SingleResourceFunctionCallNode) node, settings);
          break;
        case QueryNodeKind.SingleComplexNode:
          this.ValidateSingleComplexNode(node as SingleComplexNode, settings);
          break;
        case QueryNodeKind.In:
          break;
        default:
          throw Microsoft.AspNet.OData.Common.Error.NotSupported(SRResources.QueryNodeValidationNotSupported, (object) node.Kind, (object) typeof (FilterQueryValidator).Name);
      }
    }

    private static void ValidateFunction(string functionName, ODataValidationSettings settings)
    {
      AllowedFunctions odataFunction = ExpressionQueryValidator.ToODataFunction(functionName);
      if ((settings.AllowedFunctions & odataFunction) != odataFunction)
        throw new ODataException(Microsoft.AspNet.OData.Common.Error.Format(SRResources.NotAllowedFunction, (object) functionName, (object) "AllowedFunctions"));
    }

    private static AllowedFunctions ToODataFunction(string functionName)
    {
      AllowedFunctions odataFunction = AllowedFunctions.None;
      switch (functionName)
      {
        case "all":
          odataFunction = AllowedFunctions.All;
          break;
        case "any":
          odataFunction = AllowedFunctions.Any;
          break;
        case "cast":
          odataFunction = AllowedFunctions.Cast;
          break;
        case "ceiling":
          odataFunction = AllowedFunctions.Ceiling;
          break;
        case "concat":
          odataFunction = AllowedFunctions.Concat;
          break;
        case "contains":
          odataFunction = AllowedFunctions.Contains;
          break;
        case "date":
          odataFunction = AllowedFunctions.Date;
          break;
        case "day":
          odataFunction = AllowedFunctions.Day;
          break;
        case "endswith":
          odataFunction = AllowedFunctions.EndsWith;
          break;
        case "floor":
          odataFunction = AllowedFunctions.Floor;
          break;
        case "fractionalseconds":
          odataFunction = AllowedFunctions.FractionalSeconds;
          break;
        case "hour":
          odataFunction = AllowedFunctions.Hour;
          break;
        case "indexof":
          odataFunction = AllowedFunctions.IndexOf;
          break;
        case "isof":
          odataFunction = AllowedFunctions.IsOf;
          break;
        case "length":
          odataFunction = AllowedFunctions.Length;
          break;
        case "minute":
          odataFunction = AllowedFunctions.Minute;
          break;
        case "month":
          odataFunction = AllowedFunctions.Month;
          break;
        case "round":
          odataFunction = AllowedFunctions.Round;
          break;
        case "second":
          odataFunction = AllowedFunctions.Second;
          break;
        case "startswith":
          odataFunction = AllowedFunctions.StartsWith;
          break;
        case "substring":
          odataFunction = AllowedFunctions.Substring;
          break;
        case "time":
          odataFunction = AllowedFunctions.Time;
          break;
        case "tolower":
          odataFunction = AllowedFunctions.ToLower;
          break;
        case "toupper":
          odataFunction = AllowedFunctions.ToUpper;
          break;
        case "trim":
          odataFunction = AllowedFunctions.Trim;
          break;
        case "year":
          odataFunction = AllowedFunctions.Year;
          break;
      }
      return odataFunction;
    }

    private static AllowedLogicalOperators ToLogicalOperator(BinaryOperatorNode binaryNode)
    {
      AllowedLogicalOperators logicalOperator = AllowedLogicalOperators.None;
      switch (binaryNode.OperatorKind)
      {
        case BinaryOperatorKind.Or:
          logicalOperator = AllowedLogicalOperators.Or;
          break;
        case BinaryOperatorKind.And:
          logicalOperator = AllowedLogicalOperators.And;
          break;
        case BinaryOperatorKind.Equal:
          logicalOperator = AllowedLogicalOperators.Equal;
          break;
        case BinaryOperatorKind.NotEqual:
          logicalOperator = AllowedLogicalOperators.NotEqual;
          break;
        case BinaryOperatorKind.GreaterThan:
          logicalOperator = AllowedLogicalOperators.GreaterThan;
          break;
        case BinaryOperatorKind.GreaterThanOrEqual:
          logicalOperator = AllowedLogicalOperators.GreaterThanOrEqual;
          break;
        case BinaryOperatorKind.LessThan:
          logicalOperator = AllowedLogicalOperators.LessThan;
          break;
        case BinaryOperatorKind.LessThanOrEqual:
          logicalOperator = AllowedLogicalOperators.LessThanOrEqual;
          break;
        case BinaryOperatorKind.Has:
          logicalOperator = AllowedLogicalOperators.Has;
          break;
      }
      return logicalOperator;
    }

    private static AllowedArithmeticOperators ToArithmeticOperator(BinaryOperatorNode binaryNode)
    {
      AllowedArithmeticOperators arithmeticOperator = AllowedArithmeticOperators.None;
      switch (binaryNode.OperatorKind)
      {
        case BinaryOperatorKind.Add:
          arithmeticOperator = AllowedArithmeticOperators.Add;
          break;
        case BinaryOperatorKind.Subtract:
          arithmeticOperator = AllowedArithmeticOperators.Subtract;
          break;
        case BinaryOperatorKind.Multiply:
          arithmeticOperator = AllowedArithmeticOperators.Multiply;
          break;
        case BinaryOperatorKind.Divide:
          arithmeticOperator = AllowedArithmeticOperators.Divide;
          break;
        case BinaryOperatorKind.Modulo:
          arithmeticOperator = AllowedArithmeticOperators.Modulo;
          break;
      }
      return arithmeticOperator;
    }
  }
}
