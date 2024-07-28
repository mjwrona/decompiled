// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.UriParser.FunctionCallBinder
// Assembly: Microsoft.TeamFoundation.OData.Core, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6619C7F6-E44A-4143-AE77-6D570F968D9A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Core.dll

using Microsoft.OData.Edm;
using Microsoft.OData.Metadata;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;

namespace Microsoft.OData.UriParser
{
  internal sealed class FunctionCallBinder : BinderBase
  {
    private static readonly string[] UnboundFunctionNames = new string[2]
    {
      "cast",
      "isof"
    };

    internal FunctionCallBinder(MetadataBinder.QueryTokenVisitor bindMethod, BindingState state)
      : base(bindMethod, state)
    {
    }

    internal static void TypePromoteArguments(
      FunctionSignatureWithReturnType signature,
      List<QueryNode> argumentNodes)
    {
      for (int index = 0; index < argumentNodes.Count; ++index)
      {
        SingleValueNode argumentNode = (SingleValueNode) argumentNodes[index];
        IEdmTypeReference argumentType = signature.ArgumentTypes[index];
        argumentNodes[index] = (QueryNode) MetadataBindingUtils.ConvertToTypeIfNeeded(argumentNode, argumentType);
      }
    }

    internal static SingleValueNode[] ValidateArgumentsAreSingleValue(
      string functionName,
      List<QueryNode> argumentNodes)
    {
      ExceptionUtils.CheckArgumentNotNull<string>(functionName, "functionCallToken");
      ExceptionUtils.CheckArgumentNotNull<List<QueryNode>>(argumentNodes, nameof (argumentNodes));
      SingleValueNode[] singleValueNodeArray = new SingleValueNode[argumentNodes.Count];
      for (int index = 0; index < argumentNodes.Count; ++index)
      {
        if (!(argumentNodes[index] is SingleValueNode argumentNode))
          throw new ODataException(Microsoft.OData.Strings.MetadataBinder_FunctionArgumentNotSingleValue((object) functionName));
        singleValueNodeArray[index] = argumentNode;
      }
      return singleValueNodeArray;
    }

    internal static KeyValuePair<string, FunctionSignatureWithReturnType> MatchSignatureToUriFunction(
      string functionCallToken,
      SingleValueNode[] argumentNodes,
      IList<KeyValuePair<string, FunctionSignatureWithReturnType>> nameSignatures)
    {
      IEdmTypeReference[] array = ((IEnumerable<SingleValueNode>) argumentNodes).Select<SingleValueNode, IEdmTypeReference>((Func<SingleValueNode, IEdmTypeReference>) (s => s.TypeReference)).ToArray<IEdmTypeReference>();
      int argumentCount = array.Length;
      KeyValuePair<string, FunctionSignatureWithReturnType> uriFunction;
      if (((IEnumerable<IEdmTypeReference>) array).All<IEdmTypeReference>((Func<IEdmTypeReference, bool>) (a => a == null)) && argumentCount > 0)
      {
        KeyValuePair<string, FunctionSignatureWithReturnType> keyValuePair = nameSignatures.FirstOrDefault<KeyValuePair<string, FunctionSignatureWithReturnType>>((Func<KeyValuePair<string, FunctionSignatureWithReturnType>, bool>) (pair => ((IEnumerable<IEdmTypeReference>) pair.Value.ArgumentTypes).Count<IEdmTypeReference>() == argumentCount));
        if (keyValuePair.Equals((object) TypePromotionUtils.NotFoundKeyValuePair))
          throw new ODataException(Microsoft.OData.Strings.FunctionCallBinder_CannotFindASuitableOverload((object) functionCallToken, (object) ((IEnumerable<IEdmTypeReference>) array).Count<IEdmTypeReference>()));
        uriFunction = new KeyValuePair<string, FunctionSignatureWithReturnType>(keyValuePair.Key, new FunctionSignatureWithReturnType((IEdmTypeReference) null, keyValuePair.Value.ArgumentTypes));
      }
      else
      {
        uriFunction = TypePromotionUtils.FindBestFunctionSignature(nameSignatures, argumentNodes, functionCallToken);
        if (uriFunction.Equals((object) TypePromotionUtils.NotFoundKeyValuePair))
          throw new ODataException(Microsoft.OData.Strings.MetadataBinder_NoApplicableFunctionFound((object) functionCallToken, (object) UriFunctionsHelper.BuildFunctionSignatureListDescription(functionCallToken, nameSignatures.Select<KeyValuePair<string, FunctionSignatureWithReturnType>, FunctionSignatureWithReturnType>((Func<KeyValuePair<string, FunctionSignatureWithReturnType>, FunctionSignatureWithReturnType>) (sig => sig.Value)))));
      }
      return uriFunction;
    }

    [SuppressMessage("Microsoft.Globalization", "CA1308:NormalizeStringsToUppercase", Justification = "need to use lower characters for built-in functions.")]
    internal static IList<KeyValuePair<string, FunctionSignatureWithReturnType>> GetUriFunctionSignatures(
      string functionCallToken,
      bool enableCaseInsensitive = false)
    {
      IList<KeyValuePair<string, FunctionSignatureWithReturnType>> nameSignatures = (IList<KeyValuePair<string, FunctionSignatureWithReturnType>>) null;
      FunctionSignatureWithReturnType[] signatures = (FunctionSignatureWithReturnType[]) null;
      IList<KeyValuePair<string, FunctionSignatureWithReturnType>> first = (IList<KeyValuePair<string, FunctionSignatureWithReturnType>>) null;
      bool customFunction = CustomUriFunctions.TryGetCustomFunction(functionCallToken, out nameSignatures, enableCaseInsensitive);
      string nameKey = enableCaseInsensitive ? functionCallToken.ToLowerInvariant() : functionCallToken;
      bool builtInFunction = BuiltInUriFunctions.TryGetBuiltInFunction(nameKey, out signatures);
      if (builtInFunction)
        first = (IList<KeyValuePair<string, FunctionSignatureWithReturnType>>) ((IEnumerable<FunctionSignatureWithReturnType>) signatures).Select<FunctionSignatureWithReturnType, KeyValuePair<string, FunctionSignatureWithReturnType>>((Func<FunctionSignatureWithReturnType, KeyValuePair<string, FunctionSignatureWithReturnType>>) (sig => new KeyValuePair<string, FunctionSignatureWithReturnType>(nameKey, sig))).ToList<KeyValuePair<string, FunctionSignatureWithReturnType>>();
      if (!customFunction && !builtInFunction)
        throw new ODataException(Microsoft.OData.Strings.MetadataBinder_UnknownFunction((object) functionCallToken));
      if (!customFunction)
        return first;
      return !builtInFunction ? nameSignatures : (IList<KeyValuePair<string, FunctionSignatureWithReturnType>>) first.Concat<KeyValuePair<string, FunctionSignatureWithReturnType>>((IEnumerable<KeyValuePair<string, FunctionSignatureWithReturnType>>) nameSignatures).ToArray<KeyValuePair<string, FunctionSignatureWithReturnType>>();
    }

    internal static FunctionSignatureWithReturnType[] ExtractSignatures(
      IList<KeyValuePair<string, FunctionSignatureWithReturnType>> nameSignatures)
    {
      return nameSignatures.Select<KeyValuePair<string, FunctionSignatureWithReturnType>, FunctionSignatureWithReturnType>((Func<KeyValuePair<string, FunctionSignatureWithReturnType>, FunctionSignatureWithReturnType>) (nameSig => nameSig.Value)).ToArray<FunctionSignatureWithReturnType>();
    }

    internal QueryNode BindFunctionCall(FunctionCallToken functionCallToken)
    {
      ExceptionUtils.CheckArgumentNotNull<FunctionCallToken>(functionCallToken, nameof (functionCallToken));
      ExceptionUtils.CheckArgumentNotNull<string>(functionCallToken.Name, "functionCallToken.Name");
      QueryNode parent = (QueryNode) null;
      if (this.state.ImplicitRangeVariable != null)
        parent = functionCallToken.Source == null ? (QueryNode) NodeFactory.CreateRangeVariableReferenceNode(this.state.ImplicitRangeVariable) : this.bindMethod(functionCallToken.Source);
      QueryNode boundFunction;
      if (this.TryBindIdentifier(functionCallToken.Name, functionCallToken.Arguments, parent, this.state, out boundFunction) || this.TryBindIdentifier(functionCallToken.Name, functionCallToken.Arguments, (QueryNode) null, this.state, out boundFunction))
        return boundFunction;
      List<QueryNode> argumentNodes = new List<QueryNode>(functionCallToken.Arguments.Select<FunctionParameterToken, QueryNode>((Func<FunctionParameterToken, QueryNode>) (ar => this.bindMethod((QueryToken) ar))));
      return this.BindAsUriFunction(functionCallToken, argumentNodes);
    }

    internal bool TryBindEndPathAsFunctionCall(
      EndPathToken endPathToken,
      QueryNode parent,
      BindingState state,
      out QueryNode boundFunction)
    {
      return this.TryBindIdentifier(endPathToken.Identifier, (IEnumerable<FunctionParameterToken>) null, parent, state, out boundFunction);
    }

    internal bool TryBindInnerPathAsFunctionCall(
      InnerPathToken innerPathToken,
      QueryNode parent,
      out QueryNode boundFunction)
    {
      return this.TryBindIdentifier(innerPathToken.Identifier, (IEnumerable<FunctionParameterToken>) null, parent, this.state, out boundFunction);
    }

    internal bool TryBindDottedIdentifierAsFunctionCall(
      DottedIdentifierToken dottedIdentifierToken,
      SingleValueNode parent,
      out QueryNode boundFunction)
    {
      return this.TryBindIdentifier(dottedIdentifierToken.Identifier, (IEnumerable<FunctionParameterToken>) null, (QueryNode) parent, this.state, out boundFunction);
    }

    private QueryNode BindAsUriFunction(
      FunctionCallToken functionCallToken,
      List<QueryNode> argumentNodes)
    {
      string functionCallTokenName = functionCallToken.Source == null ? this.IsUnboundFunction(functionCallToken.Name) : throw new ODataException(Microsoft.OData.Strings.FunctionCallBinder_UriFunctionMustHaveHaveNullParent((object) functionCallToken.Name));
      if (functionCallTokenName != null)
        return (QueryNode) this.CreateUnboundFunctionNode(functionCallTokenName, argumentNodes);
      IList<KeyValuePair<string, FunctionSignatureWithReturnType>> functionSignatures = FunctionCallBinder.GetUriFunctionSignatures(functionCallToken.Name, this.state.Configuration.EnableCaseInsensitiveUriFunctionIdentifier);
      SingleValueNode[] argumentNodes1 = FunctionCallBinder.ValidateArgumentsAreSingleValue(functionCallToken.Name, argumentNodes);
      KeyValuePair<string, FunctionSignatureWithReturnType> uriFunction = FunctionCallBinder.MatchSignatureToUriFunction(functionCallToken.Name, argumentNodes1, functionSignatures);
      string key = uriFunction.Key;
      FunctionSignatureWithReturnType signature = uriFunction.Value;
      if (signature.ReturnType != null)
        FunctionCallBinder.TypePromoteArguments(signature, argumentNodes);
      return signature.ReturnType != null && signature.ReturnType.IsStructured() ? (QueryNode) new SingleResourceFunctionCallNode(key, (IEnumerable<QueryNode>) new ReadOnlyCollection<QueryNode>((IList<QueryNode>) argumentNodes), signature.ReturnType.AsStructured(), (IEdmNavigationSource) null) : (QueryNode) new SingleValueFunctionCallNode(key, (IEnumerable<QueryNode>) new ReadOnlyCollection<QueryNode>((IList<QueryNode>) argumentNodes), signature.ReturnType);
    }

    private bool TryBindIdentifier(
      string identifier,
      IEnumerable<FunctionParameterToken> arguments,
      QueryNode parent,
      BindingState state,
      out QueryNode boundFunction)
    {
      boundFunction = (QueryNode) null;
      IEdmType bindingType = (IEdmType) null;
      if (parent is SingleValueNode singleValueNode)
      {
        if (singleValueNode.TypeReference != null)
          bindingType = singleValueNode.TypeReference.Definition;
      }
      else if (parent is CollectionNode collectionNode)
        bindingType = collectionNode.CollectionType.Definition;
      if (!UriEdmHelpers.IsBindingTypeValid(bindingType) || identifier.IndexOf(".", StringComparison.Ordinal) == -1 && this.Resolver.GetType() == typeof (ODataUriResolver))
        return false;
      List<FunctionParameterToken> functionParameterTokenList = arguments == null ? new List<FunctionParameterToken>() : arguments.ToList<FunctionParameterToken>();
      IEdmOperation matchingOperation;
      if (!FunctionOverloadResolver.ResolveOperationFromList(identifier, (IEnumerable<string>) functionParameterTokenList.Select<FunctionParameterToken, string>((Func<FunctionParameterToken, string>) (ar => ar.ParameterName)).ToList<string>(), bindingType, state.Model, out matchingOperation, this.Resolver))
        return false;
      if (singleValueNode != null && singleValueNode.TypeReference == null)
        throw new ODataException(Microsoft.OData.Strings.FunctionCallBinder_CallingFunctionOnOpenProperty((object) identifier));
      if (matchingOperation.IsAction())
        return false;
      IEdmFunction edmFunction = (IEdmFunction) matchingOperation;
      IEnumerable<QueryNode> source1 = FunctionCallBinder.HandleComplexOrCollectionParameterValueIfExists(state.Configuration.Model, (IEdmOperation) edmFunction, (ICollection<FunctionParameterToken>) functionParameterTokenList, state.Configuration.Resolver.EnableCaseInsensitive).Select<FunctionParameterToken, QueryNode>((Func<FunctionParameterToken, QueryNode>) (p => this.bindMethod((QueryToken) p)));
      List<QueryNode> list = source1.ToList<QueryNode>();
      for (int index = 0; index < source1.Count<QueryNode>(); ++index)
      {
        if (list[index] is NamedFunctionParameterNode functionParameterNode && functionParameterNode.Value is ConstantNode source2)
        {
          IEdmTypeReference typeReference = source2.TypeReference;
          if ((typeReference != null ? (typeReference.IsString() ? 1 : 0) : 0) != 0)
          {
            IEdmTypeReference type = edmFunction.FindParameter(functionParameterNode.Name)?.Type;
            if (type != null && type.IsEnum())
              list[index] = (QueryNode) new NamedFunctionParameterNode(functionParameterNode.Name, (QueryNode) MetadataBindingUtils.ConvertToTypeIfNeeded((SingleValueNode) source2, type));
          }
        }
      }
      IEnumerable<QueryNode> parameters = (IEnumerable<QueryNode>) list;
      IEdmTypeReference returnType = edmFunction.ReturnType;
      IEdmEntitySetBase navigationSource = (IEdmEntitySetBase) null;
      if (parent is SingleResourceNode singleResourceNode)
        navigationSource = edmFunction.GetTargetEntitySet(singleResourceNode.NavigationSource, state.Model);
      string name = edmFunction.FullName();
      if (returnType.IsEntity())
        boundFunction = (QueryNode) new SingleResourceFunctionCallNode(name, (IEnumerable<IEdmFunction>) new IEdmFunction[1]
        {
          edmFunction
        }, parameters, (IEdmStructuredTypeReference) returnType.Definition.ToTypeReference(), (IEdmNavigationSource) navigationSource, parent);
      else if (returnType.IsEntityCollectionType())
      {
        IEdmCollectionTypeReference returnedCollectionTypeReference = (IEdmCollectionTypeReference) returnType;
        boundFunction = (QueryNode) new CollectionResourceFunctionCallNode(name, (IEnumerable<IEdmFunction>) new IEdmFunction[1]
        {
          edmFunction
        }, parameters, returnedCollectionTypeReference, navigationSource, parent);
      }
      else if (returnType.IsCollection())
      {
        IEdmCollectionTypeReference returnedCollectionType = (IEdmCollectionTypeReference) returnType;
        boundFunction = (QueryNode) new CollectionFunctionCallNode(name, (IEnumerable<IEdmFunction>) new IEdmFunction[1]
        {
          edmFunction
        }, parameters, returnedCollectionType, parent);
      }
      else
        boundFunction = (QueryNode) new SingleValueFunctionCallNode(name, (IEnumerable<IEdmFunction>) new IEdmFunction[1]
        {
          edmFunction
        }, parameters, returnType, parent);
      return true;
    }

    internal static List<OperationSegmentParameter> BindSegmentParameters(
      ODataUriParserConfiguration configuration,
      IEdmOperation functionOrOpertion,
      ICollection<FunctionParameterToken> segmentParameterTokens)
    {
      ICollection<FunctionParameterToken> functionParameterTokens = FunctionCallBinder.HandleComplexOrCollectionParameterValueIfExists(configuration.Model, functionOrOpertion, segmentParameterTokens, configuration.Resolver.EnableCaseInsensitive, configuration.EnableUriTemplateParsing);
      BindingState initialState = new BindingState(configuration);
      initialState.ImplicitRangeVariable = (RangeVariable) null;
      initialState.RangeVariables.Clear();
      MetadataBinder metadataBinder = new MetadataBinder(initialState);
      List<OperationSegmentParameter> segmentParameterList = new List<OperationSegmentParameter>();
      IDictionary<string, SingleValueNode> input = (IDictionary<string, SingleValueNode>) new Dictionary<string, SingleValueNode>((IEqualityComparer<string>) StringComparer.Ordinal);
      foreach (FunctionParameterToken functionParameterToken in (IEnumerable<FunctionParameterToken>) functionParameterTokens)
      {
        if (functionParameterToken.ValueToken is EndPathToken)
          throw new ODataException(Microsoft.OData.Strings.MetadataBinder_ParameterNotInScope((object) string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}={1}", new object[2]
          {
            (object) functionParameterToken.ParameterName,
            (object) (functionParameterToken.ValueToken as EndPathToken).Identifier
          })));
        SingleValueNode singleValueNode = (SingleValueNode) metadataBinder.Bind(functionParameterToken.ValueToken);
        if (!input.ContainsKey(functionParameterToken.ParameterName))
          input.Add(functionParameterToken.ParameterName, singleValueNode);
      }
      foreach (KeyValuePair<IEdmOperationParameter, SingleValueNode> operationParameter in (IEnumerable<KeyValuePair<IEdmOperationParameter, SingleValueNode>>) configuration.Resolver.ResolveOperationParameters(functionOrOpertion, input))
      {
        SingleValueNode typeIfNeeded = operationParameter.Value;
        if (typeIfNeeded.GetEdmTypeReference() != null && !FunctionCallBinder.TryRewriteIntegralConstantNode(ref typeIfNeeded, operationParameter.Key.Type))
          typeIfNeeded = MetadataBindingUtils.ConvertToTypeIfNeeded(typeIfNeeded, operationParameter.Key.Type);
        OperationSegmentParameter segmentParameter = new OperationSegmentParameter(operationParameter.Key.Name, (object) typeIfNeeded);
        segmentParameterList.Add(segmentParameter);
      }
      return segmentParameterList;
    }

    private static bool TryRewriteIntegralConstantNode(
      ref SingleValueNode boundNode,
      IEdmTypeReference targetType)
    {
      if (targetType == null || !targetType.IsByte() && !targetType.IsSByte() && !targetType.IsInt16() || !(boundNode is ConstantNode constantNode))
        return false;
      IEdmTypeReference typeReference = constantNode.TypeReference;
      if (typeReference == null || !typeReference.IsInt32())
        return false;
      int num = (int) constantNode.Value;
      object constantValue = (object) null;
      switch (targetType.PrimitiveKind())
      {
        case EdmPrimitiveTypeKind.Byte:
          if (num >= 0 && num <= (int) byte.MaxValue)
          {
            constantValue = (object) (byte) num;
            break;
          }
          break;
        case EdmPrimitiveTypeKind.Int16:
          if (num >= (int) short.MinValue && num <= (int) short.MaxValue)
          {
            constantValue = (object) (short) num;
            break;
          }
          break;
        case EdmPrimitiveTypeKind.SByte:
          if (num >= (int) sbyte.MinValue && num <= (int) sbyte.MaxValue)
          {
            constantValue = (object) (sbyte) num;
            break;
          }
          break;
      }
      if (constantValue == null)
        return false;
      boundNode = (SingleValueNode) new ConstantNode(constantValue, constantNode.LiteralText, targetType);
      return true;
    }

    private static ICollection<FunctionParameterToken> HandleComplexOrCollectionParameterValueIfExists(
      IEdmModel model,
      IEdmOperation operation,
      ICollection<FunctionParameterToken> parameterTokens,
      bool enableCaseInsensitive,
      bool enableUriTemplateParsing = false)
    {
      ICollection<FunctionParameterToken> functionParameterTokens = (ICollection<FunctionParameterToken>) new Collection<FunctionParameterToken>();
      foreach (FunctionParameterToken parameterToken in (IEnumerable<FunctionParameterToken>) parameterTokens)
      {
        IEdmOperationParameter operationParameter = operation.FindParameter(parameterToken.ParameterName);
        FunctionParameterToken functionParameterToken1;
        if (enableCaseInsensitive && operationParameter == null)
        {
          operationParameter = ODataUriResolver.ResolveOperationParameterNameCaseInsensitive(operation, parameterToken.ParameterName);
          functionParameterToken1 = new FunctionParameterToken(operationParameter.Name, parameterToken.ValueToken);
        }
        else
          functionParameterToken1 = parameterToken;
        if (functionParameterToken1.ValueToken is FunctionParameterAliasToken valueToken1)
          valueToken1.ExpectedParameterType = operationParameter.Type;
        LiteralToken valueToken2 = functionParameterToken1.ValueToken as LiteralToken;
        if (valueToken2 != null && valueToken2.Value is string str && !string.IsNullOrEmpty(valueToken2.OriginalText))
        {
          ExpressionLexer expressionLexer = new ExpressionLexer(valueToken2.OriginalText, true, false, true);
          if (expressionLexer.CurrentToken.Kind == ExpressionTokenKind.BracketedExpression || expressionLexer.CurrentToken.Kind == ExpressionTokenKind.BracedExpression)
          {
            UriTemplateExpression expression;
            object obj;
            if (enableUriTemplateParsing && UriTemplateParser.TryParseLiteral(expressionLexer.CurrentToken.Text, operationParameter.Type, out expression))
              obj = (object) expression;
            else if (!operationParameter.Type.IsStructured() && !operationParameter.Type.IsStructuredCollectionType())
            {
              obj = ODataUriUtils.ConvertFromUriLiteral(str, ODataVersion.V4, model, operationParameter.Type);
            }
            else
            {
              functionParameterTokens.Add(functionParameterToken1);
              continue;
            }
            LiteralToken valueToken3 = new LiteralToken(obj, valueToken2.OriginalText);
            FunctionParameterToken functionParameterToken2 = new FunctionParameterToken(functionParameterToken1.ParameterName, (QueryToken) valueToken3);
            functionParameterTokens.Add(functionParameterToken2);
            continue;
          }
        }
        functionParameterTokens.Add(functionParameterToken1);
      }
      return functionParameterTokens;
    }

    [SuppressMessage("Microsoft.Globalization", "CA1308:NormalizeStringsToUppercase", Justification = "need to use lower characters for unbound functions.")]
    private string IsUnboundFunction(string functionName)
    {
      functionName = this.state.Configuration.EnableCaseInsensitiveUriFunctionIdentifier ? functionName.ToLowerInvariant() : functionName;
      return ((IEnumerable<string>) FunctionCallBinder.UnboundFunctionNames).FirstOrDefault<string>((Func<string, bool>) (name => name.Equals(functionName, StringComparison.Ordinal)));
    }

    private SingleValueNode CreateUnboundFunctionNode(
      string functionCallTokenName,
      List<QueryNode> args)
    {
      IEdmTypeReference edmTypeReference = (IEdmTypeReference) null;
      switch (functionCallTokenName)
      {
        case "isof":
          edmTypeReference = FunctionCallBinder.ValidateAndBuildIsOfArgs(this.state, ref args);
          break;
        case "cast":
          edmTypeReference = FunctionCallBinder.ValidateAndBuildCastArgs(this.state, ref args);
          if (edmTypeReference.IsStructured())
          {
            SingleResourceNode singleResourceNode = args.ElementAt<QueryNode>(0) as SingleResourceNode;
            return (SingleValueNode) new SingleResourceFunctionCallNode(functionCallTokenName, (IEnumerable<QueryNode>) args, edmTypeReference.AsStructured(), singleResourceNode?.NavigationSource);
          }
          break;
      }
      return (SingleValueNode) new SingleValueFunctionCallNode(functionCallTokenName, (IEnumerable<QueryNode>) args, edmTypeReference);
    }

    private static IEdmTypeReference ValidateAndBuildCastArgs(
      BindingState state,
      ref List<QueryNode> args)
    {
      return FunctionCallBinder.ValidateIsOfOrCast(state, true, ref args);
    }

    private static IEdmTypeReference ValidateAndBuildIsOfArgs(
      BindingState state,
      ref List<QueryNode> args)
    {
      return FunctionCallBinder.ValidateIsOfOrCast(state, false, ref args);
    }

    private static IEdmTypeReference ValidateIsOfOrCast(
      BindingState state,
      bool isCast,
      ref List<QueryNode> args)
    {
      ConstantNode constantNode = args.Count == 1 || args.Count == 2 ? args.Last<QueryNode>() as ConstantNode : throw new ODataErrorException(Microsoft.OData.Strings.MetadataBinder_CastOrIsOfExpressionWithWrongNumberOfOperands((object) args.Count));
      IEdmTypeReference type = (IEdmTypeReference) null;
      if (constantNode != null)
        type = FunctionCallBinder.TryGetTypeReference(state.Model, constantNode.Value as string, state.Configuration.Resolver);
      if (type == null)
        throw new ODataException(Microsoft.OData.Strings.MetadataBinder_CastOrIsOfFunctionWithoutATypeArgument);
      if (type.IsCollection())
        throw new ODataException(Microsoft.OData.Strings.MetadataBinder_CastOrIsOfCollectionsNotSupported);
      if (args.Count == 1)
        args = new List<QueryNode>()
        {
          (QueryNode) new ResourceRangeVariableReferenceNode(state.ImplicitRangeVariable.Name, state.ImplicitRangeVariable as ResourceRangeVariable),
          args[0]
        };
      else if (!(args[0] is SingleValueNode))
        throw new ODataException(Microsoft.OData.Strings.MetadataBinder_CastOrIsOfCollectionsNotSupported);
      if (isCast && args.Count == 2)
      {
        if (args[0].GetEdmTypeReference() is IEdmEnumTypeReference && !string.Equals(constantNode.Value as string, "Edm.String", StringComparison.Ordinal))
          throw new ODataException(Microsoft.OData.Strings.CastBinder_EnumOnlyCastToOrFromString);
        if (type is IEdmEnumTypeReference)
        {
          IEdmTypeReference edmTypeReference = args[0].GetEdmTypeReference();
          if (edmTypeReference != null && (!(edmTypeReference is IEdmPrimitiveTypeReference primitiveTypeReference) || !(primitiveTypeReference.Definition is IEdmPrimitiveType definition) || definition.PrimitiveKind != EdmPrimitiveTypeKind.String))
            throw new ODataException(Microsoft.OData.Strings.CastBinder_EnumOnlyCastToOrFromString);
        }
      }
      return isCast ? type : (IEdmTypeReference) EdmCoreModel.Instance.GetBoolean(true);
    }

    private static IEdmTypeReference TryGetTypeReference(
      IEdmModel model,
      string fullTypeName,
      ODataUriResolver resolver)
    {
      IEdmTypeReference typeReference = UriEdmHelpers.FindTypeFromModel(model, fullTypeName, resolver).ToTypeReference();
      if (typeReference != null)
        return typeReference;
      if (!fullTypeName.StartsWith("Collection", StringComparison.Ordinal))
        return (IEdmTypeReference) null;
      string qualifiedName = fullTypeName.Split('(')[1].Split(')')[0];
      return (IEdmTypeReference) EdmCoreModel.GetCollection(UriEdmHelpers.FindTypeFromModel(model, qualifiedName, resolver).ToTypeReference());
    }
  }
}
