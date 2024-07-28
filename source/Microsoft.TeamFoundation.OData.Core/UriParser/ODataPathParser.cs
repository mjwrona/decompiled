// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.UriParser.ODataPathParser
// Assembly: Microsoft.TeamFoundation.OData.Core, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6619C7F6-E44A-4143-AE77-6D570F968D9A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Core.dll

using Microsoft.OData.Edm;
using Microsoft.OData.Edm.Vocabularies;
using Microsoft.OData.Edm.Vocabularies.Community.V1;
using Microsoft.OData.Metadata;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text.RegularExpressions;

namespace Microsoft.OData.UriParser
{
  internal sealed class ODataPathParser
  {
    internal static readonly Regex ContentIdRegex = Microsoft.OData.PlatformHelper.CreateCompiled("^\\$[0-9]+$", RegexOptions.Singleline);
    private static readonly IList<string> EmptyList = (IList<string>) new List<string>();
    private readonly Queue<string> segmentQueue = new Queue<string>();
    private readonly List<ODataPathSegment> parsedSegments = new List<ODataPathSegment>();
    private readonly ODataUriParserConfiguration configuration;
    private bool nextSegmentMustReferToMetadata;
    private IEdmNavigationSource lastNavigationSource;

    internal ODataPathParser(ODataUriParserConfiguration configuration) => this.configuration = configuration;

    internal static void ExtractSegmentIdentifierAndParenthesisExpression(
      string segmentText,
      out string identifier,
      out string parenthesisExpression)
    {
      int length = segmentText.IndexOf('(');
      if (length < 0)
      {
        identifier = segmentText;
        parenthesisExpression = (string) null;
      }
      else
      {
        identifier = segmentText[segmentText.Length - 1] == ')' ? segmentText.Substring(0, length) : throw ExceptionUtil.CreateSyntaxError();
        parenthesisExpression = segmentText.Substring(length + 1, segmentText.Length - identifier.Length - 2);
      }
      if (identifier.Length == 0)
        throw ExceptionUtil.ResourceNotFoundError(Microsoft.OData.Strings.RequestUriProcessor_EmptySegmentInRequestUrl);
    }

    internal IList<ODataPathSegment> ParsePath(ICollection<string> segments)
    {
      foreach (string segment in (IEnumerable<string>) segments)
        this.segmentQueue.Enqueue(segment);
      string segmentText = (string) null;
      try
      {
        while (this.TryGetNextSegmentText(out segmentText))
        {
          if (this.parsedSegments.Count == 0)
            this.CreateFirstSegment(segmentText);
          else
            this.CreateNextSegment(segmentText);
          IEdmNavigationSource navigationSource = this.parsedSegments.Last<ODataPathSegment>().TranslateWith<IEdmNavigationSource>((PathSegmentTranslator<IEdmNavigationSource>) new DetermineNavigationSourceTranslator());
          if (navigationSource != null)
            this.lastNavigationSource = navigationSource;
        }
      }
      catch (ODataUnrecognizedPathException ex)
      {
        ex.ParsedSegments = (IEnumerable<ODataPathSegment>) this.parsedSegments;
        ex.CurrentSegment = segmentText;
        ex.UnparsedSegments = (IEnumerable<string>) this.segmentQueue.ToList<string>();
        throw;
      }
      List<ODataPathSegment> validatedPathSegments = this.CreateValidatedPathSegments();
      this.parsedSegments.Clear();
      return (IList<ODataPathSegment>) validatedPathSegments;
    }

    private static bool TryBindingParametersAndMatchingOperationImport(
      string identifier,
      string parenthesisExpression,
      ODataUriParserConfiguration configuration,
      out ICollection<OperationSegmentParameter> boundParameters,
      out IEdmOperationImport matchingFunctionImport)
    {
      matchingFunctionImport = (IEdmOperationImport) null;
      ICollection<FunctionParameterToken> splitParameters = (ICollection<FunctionParameterToken>) null;
      if (!string.IsNullOrEmpty(parenthesisExpression))
      {
        if (!FunctionParameterParser.TrySplitOperationParameters(parenthesisExpression, configuration, out splitParameters))
        {
          IEdmOperationImport matchingOperationImport = (IEdmOperationImport) null;
          if (FunctionOverloadResolver.ResolveOperationImportFromList(identifier, ODataPathParser.EmptyList, configuration.Model, out matchingOperationImport, configuration.Resolver))
          {
            if (!(matchingOperationImport.Operation.ReturnType is IEdmCollectionTypeReference returnType) || !returnType.ElementType().IsEntity())
              throw ExceptionUtil.CreateBadRequestError(Microsoft.OData.Strings.RequestUriProcessor_SegmentDoesNotSupportKeyPredicates((object) identifier));
            matchingFunctionImport = matchingOperationImport;
            boundParameters = (ICollection<OperationSegmentParameter>) null;
            return true;
          }
          boundParameters = (ICollection<OperationSegmentParameter>) null;
          return false;
        }
      }
      else
        splitParameters = (ICollection<FunctionParameterToken>) new Collection<FunctionParameterToken>();
      if (FunctionOverloadResolver.ResolveOperationImportFromList(identifier, (IList<string>) splitParameters.Select<FunctionParameterToken, string>((Func<FunctionParameterToken, string>) (k => k.ParameterName)).ToList<string>(), configuration.Model, out matchingFunctionImport, configuration.Resolver))
      {
        IEdmOperation operation = matchingFunctionImport.Operation;
        boundParameters = (ICollection<OperationSegmentParameter>) FunctionCallBinder.BindSegmentParameters(configuration, operation, splitParameters);
        return true;
      }
      boundParameters = (ICollection<OperationSegmentParameter>) null;
      return false;
    }

    private static bool TryBindingParametersAndMatchingOperation(
      string identifier,
      string parenthesisExpression,
      IEdmType bindingType,
      ODataUriParserConfiguration configuration,
      out ICollection<OperationSegmentParameter> boundParameters,
      out IEdmOperation matchingOperation)
    {
      if (identifier != null && identifier.IndexOf(".", StringComparison.Ordinal) == -1 && configuration.Resolver.GetType() == typeof (ODataUriResolver))
      {
        boundParameters = (ICollection<OperationSegmentParameter>) null;
        matchingOperation = (IEdmOperation) null;
        return false;
      }
      matchingOperation = (IEdmOperation) null;
      ICollection<FunctionParameterToken> splitParameters;
      if (!string.IsNullOrEmpty(parenthesisExpression))
      {
        if (!FunctionParameterParser.TrySplitOperationParameters(parenthesisExpression, configuration, out splitParameters))
        {
          IEdmOperation matchingOperation1 = (IEdmOperation) null;
          if (FunctionOverloadResolver.ResolveOperationFromList(identifier, (IEnumerable<string>) new List<string>(), bindingType, configuration.Model, out matchingOperation1, configuration.Resolver))
          {
            if (!(matchingOperation1.ReturnType is IEdmCollectionTypeReference returnType) || !returnType.ElementType().IsEntity())
              throw ExceptionUtil.CreateBadRequestError(Microsoft.OData.Strings.RequestUriProcessor_SegmentDoesNotSupportKeyPredicates((object) identifier));
            matchingOperation = matchingOperation1;
            boundParameters = (ICollection<OperationSegmentParameter>) null;
            return true;
          }
          boundParameters = (ICollection<OperationSegmentParameter>) null;
          return false;
        }
      }
      else
        splitParameters = (ICollection<FunctionParameterToken>) new Collection<FunctionParameterToken>();
      if (FunctionOverloadResolver.ResolveOperationFromList(identifier, (IEnumerable<string>) splitParameters.Select<FunctionParameterToken, string>((Func<FunctionParameterToken, string>) (k => k.ParameterName)).ToList<string>(), bindingType, configuration.Model, out matchingOperation, configuration.Resolver))
      {
        boundParameters = (ICollection<OperationSegmentParameter>) FunctionCallBinder.BindSegmentParameters(configuration, matchingOperation, splitParameters);
        return true;
      }
      boundParameters = (ICollection<OperationSegmentParameter>) null;
      return false;
    }

    private static void CheckSingleResult(bool isSingleResult, string identifier)
    {
      if (!isSingleResult)
        throw ExceptionUtil.CreateBadRequestError(Microsoft.OData.Strings.RequestUriProcessor_CannotQueryCollections((object) identifier));
    }

    private bool TryGetNextSegmentText(out string segmentText) => this.TryGetNextSegmentText(false, out segmentText);

    private bool TryGetNextSegmentText(bool previousSegmentWasEscapeMarker, out string segmentText)
    {
      if (this.segmentQueue.Count == 0)
      {
        segmentText = (string) null;
        return false;
      }
      segmentText = this.segmentQueue.Dequeue();
      if (segmentText == "$")
      {
        this.nextSegmentMustReferToMetadata = true;
        return this.TryGetNextSegmentText(true, out segmentText);
      }
      if (!previousSegmentWasEscapeMarker)
        this.nextSegmentMustReferToMetadata = false;
      if (this.parsedSegments.Count > 0)
        ODataPathParser.ThrowIfMustBeLeafSegment(this.parsedSegments[this.parsedSegments.Count - 1]);
      return true;
    }

    private bool TryHandleAsKeySegment(string segmentText)
    {
      ODataPathSegment parsedSegment = this.parsedSegments[this.parsedSegments.Count - 1];
      KeySegment previousKeySegment = this.FindPreviousKeySegment();
      KeySegment keySegment;
      if (this.nextSegmentMustReferToMetadata || !SegmentKeyHandler.TryHandleSegmentAsKey(segmentText, parsedSegment, previousKeySegment, this.configuration.UrlKeyDelimiter, this.configuration.Resolver, out keySegment, this.configuration.EnableUriTemplateParsing))
        return false;
      this.parsedSegments.Add((ODataPathSegment) keySegment);
      return true;
    }

    private KeySegment FindPreviousKeySegment() => (KeySegment) this.parsedSegments.LastOrDefault<ODataPathSegment>((Func<ODataPathSegment, bool>) (s => s is KeySegment));

    private static void ThrowIfMustBeLeafSegment(ODataPathSegment previous)
    {
      if (previous is OperationImportSegment operationImportSegment)
      {
        foreach (IEdmOperationImport operationImport in operationImportSegment.OperationImports)
        {
          if (operationImport.IsActionImport() || operationImport.IsFunctionImport() && !((IEdmFunctionImport) operationImport).Function.IsComposable)
            throw ExceptionUtil.CreateBadRequestError(Microsoft.OData.Strings.RequestUriProcessor_MustBeLeafSegment((object) previous.Identifier));
        }
      }
      if (previous is OperationSegment operationSegment)
      {
        foreach (IEdmOperation operation in operationSegment.Operations)
        {
          if (operation.IsAction() || operation.IsFunction() && !((IEdmFunction) operation).IsComposable)
            throw ExceptionUtil.CreateBadRequestError(Microsoft.OData.Strings.RequestUriProcessor_MustBeLeafSegment((object) previous.Identifier));
        }
      }
      if (previous.TargetKind == RequestTargetKind.Batch || previous.TargetKind == RequestTargetKind.Metadata || previous.TargetKind == RequestTargetKind.PrimitiveValue || previous.TargetKind == RequestTargetKind.DynamicValue || previous.TargetKind == RequestTargetKind.EnumValue || previous.TargetKind == RequestTargetKind.MediaResource || previous.TargetKind == RequestTargetKind.VoidOperation || previous.TargetKind == RequestTargetKind.Nothing)
        throw ExceptionUtil.ResourceNotFoundError(Microsoft.OData.Strings.RequestUriProcessor_MustBeLeafSegment((object) previous.Identifier));
    }

    private bool TryCreateCountSegment(string identifier, string parenthesisExpression)
    {
      if (!this.IdentifierIs("$count", identifier))
        return false;
      if (parenthesisExpression != null)
        throw ExceptionUtil.CreateSyntaxError();
      ODataPathSegment parsedSegment = this.parsedSegments[this.parsedSegments.Count - 1];
      if ((parsedSegment.TargetKind != RequestTargetKind.Resource || parsedSegment.SingleResult) && parsedSegment.TargetKind != RequestTargetKind.Collection)
        throw ExceptionUtil.ResourceNotFoundError(Microsoft.OData.Strings.RequestUriProcessor_CountNotSupported((object) parsedSegment.Identifier));
      this.parsedSegments.Add((ODataPathSegment) CountSegment.Instance);
      return true;
    }

    private FilterClause GenerateFilterClause(
      IEdmNavigationSource navigationSource,
      IEdmType targetEdmType,
      string filter)
    {
      ODataPathInfo odataPathInfo = new ODataPathInfo(targetEdmType, navigationSource);
      QueryToken filter1 = new UriQueryExpressionParser(this.configuration.Settings.FilterLimit, this.configuration.EnableCaseInsensitiveUriFunctionIdentifier).ParseFilter(filter);
      BindingState bindingState = new BindingState(this.configuration, odataPathInfo.Segments.ToList<ODataPathSegment>())
      {
        ImplicitRangeVariable = NodeFactory.CreateImplicitRangeVariable(odataPathInfo.TargetEdmType.ToTypeReference(), odataPathInfo.TargetNavigationSource)
      };
      bindingState.RangeVariables.Push(bindingState.ImplicitRangeVariable);
      return new FilterBinder(new MetadataBinder.QueryTokenVisitor(new MetadataBinder(bindingState).Bind), bindingState).BindFilter(filter1);
    }

    private bool TryCreateFilterSegment(string segmentText)
    {
      if (!segmentText.StartsWith("$filter", this.configuration.EnableCaseInsensitiveUriFunctionIdentifier ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal))
        return false;
      int length = "$filter".Length;
      if (segmentText.Length <= length + 2 || segmentText[length] != '(' || segmentText[segmentText.Length - 1] != ')')
        throw new ODataException(Microsoft.OData.Strings.RequestUriProcessor_FilterPathSegmentSyntaxError);
      if (this.lastNavigationSource == null)
        throw new ODataException(Microsoft.OData.Strings.RequestUriProcessor_NoNavigationSourceFound((object) "$filter"));
      if (this.lastNavigationSource is IEdmSingleton || this.parsedSegments.Last<ODataPathSegment>() is KeySegment)
        throw new ODataException(Microsoft.OData.Strings.RequestUriProcessor_CannotApplyFilterOnSingleEntities((object) this.lastNavigationSource.Name));
      string filter = segmentText.Substring(length + 1, segmentText.Length - "$filter".Length - 2);
      TypeSegment typeSegment = this.parsedSegments.Last<ODataPathSegment>() as TypeSegment;
      FilterClause filterClause = this.GenerateFilterClause(this.lastNavigationSource, typeSegment == null ? (IEdmType) this.lastNavigationSource.EntityType() : typeSegment.TargetEdmType, filter);
      this.parsedSegments.Add((ODataPathSegment) new FilterSegment(filterClause.Expression, filterClause.RangeVariable, this.lastNavigationSource));
      return true;
    }

    private bool TryCreateEachSegment(string identifier, string parenthesisExpression)
    {
      if (!this.IdentifierIs("$each", identifier))
        return false;
      if (parenthesisExpression != null)
        throw ExceptionUtil.CreateSyntaxError();
      ODataPathSegment odataPathSegment = this.parsedSegments.Last<ODataPathSegment>();
      if (this.lastNavigationSource == null)
        throw new ODataException(Microsoft.OData.Strings.RequestUriProcessor_NoNavigationSourceFound((object) "$each"));
      if (this.lastNavigationSource is IEdmSingleton || odataPathSegment is KeySegment)
        throw new ODataException(Microsoft.OData.Strings.RequestUriProcessor_CannotApplyEachOnSingleEntities((object) this.lastNavigationSource.Name));
      this.parsedSegments.Add((ODataPathSegment) new EachSegment(this.lastNavigationSource, odataPathSegment.TargetEdmType.AsElementType()));
      return true;
    }

    private bool TryCreateEntityReferenceSegment(string identifier, string parenthesisExpression)
    {
      if (!this.IdentifierIs("$ref", identifier))
        return false;
      if (parenthesisExpression != null)
        throw ExceptionUtil.CreateSyntaxError();
      int index = this.parsedSegments.Count - 1;
      while (index > 0 && this.parsedSegments[index] is KeySegment)
        --index;
      if (this.parsedSegments[index] is NavigationPropertySegment parsedSegment)
      {
        if (parsedSegment.TargetKind != RequestTargetKind.Resource)
          throw ExceptionUtil.CreateBadRequestError(Microsoft.OData.Strings.PathParser_EntityReferenceNotSupported((object) parsedSegment.Identifier));
        NavigationPropertyLinkSegment propertyLinkSegment = new NavigationPropertyLinkSegment(parsedSegment.NavigationProperty, this.parsedSegments[index - 1].TargetEdmNavigationSource.FindNavigationTarget(parsedSegment.NavigationProperty, new Func<IEdmPathExpression, List<ODataPathSegment>, bool>(BindingPathHelper.MatchBindingPath), this.parsedSegments, out IEdmPathExpression _) ?? throw ExceptionUtil.CreateResourceNotFoundError(parsedSegment.NavigationProperty.Name));
        this.parsedSegments[index] = (ODataPathSegment) propertyLinkSegment;
      }
      else
      {
        ODataPathSegment odataPathSegment = this.parsedSegments.Last<ODataPathSegment>();
        if (odataPathSegment.TargetKind != RequestTargetKind.Resource)
          throw ExceptionUtil.CreateBadRequestError(Microsoft.OData.Strings.PathParser_EntityReferenceNotSupported((object) odataPathSegment.Identifier));
        ReferenceSegment referenceSegment = new ReferenceSegment(this.lastNavigationSource);
        referenceSegment.SingleResult = odataPathSegment.SingleResult;
        this.parsedSegments.Add((ODataPathSegment) referenceSegment);
      }
      if (this.TryGetNextSegmentText(out string _))
        throw ExceptionUtil.ResourceNotFoundError(Microsoft.OData.Strings.RequestUriProcessor_MustBeLeafSegment((object) "$ref"));
      return true;
    }

    private bool TryBindKeyFromParentheses(string parenthesesSection)
    {
      ODataPathSegment keySegment;
      if (parenthesesSection == null || !SegmentKeyHandler.TryCreateKeySegmentFromParentheses(this.parsedSegments[this.parsedSegments.Count - 1], this.FindPreviousKeySegment(), parenthesesSection, this.configuration.Resolver, out keySegment, this.configuration.EnableUriTemplateParsing))
        return false;
      this.parsedSegments.Add(keySegment);
      return true;
    }

    private bool TryCreateValueSegment(string identifier, string parenthesisExpression)
    {
      if (!this.IdentifierIs("$value", identifier))
        return false;
      if (parenthesisExpression != null)
        throw ExceptionUtil.CreateSyntaxError();
      ODataPathSegment parsedSegment = this.parsedSegments[this.parsedSegments.Count - 1];
      ODataPathSegment odataPathSegment = (ODataPathSegment) new ValueSegment(parsedSegment.EdmType);
      if (parsedSegment.TargetKind == RequestTargetKind.Primitive || parsedSegment.TargetKind == RequestTargetKind.Enum)
        odataPathSegment.CopyValuesFrom(parsedSegment);
      else
        odataPathSegment.TargetEdmType = parsedSegment.TargetEdmType;
      odataPathSegment.Identifier = "$value";
      odataPathSegment.SingleResult = true;
      ODataPathParser.CheckSingleResult(parsedSegment.SingleResult, parsedSegment.Identifier);
      odataPathSegment.TargetKind = parsedSegment.TargetKind != RequestTargetKind.Primitive ? (parsedSegment.TargetKind != RequestTargetKind.Enum ? (parsedSegment.TargetKind != RequestTargetKind.Dynamic ? RequestTargetKind.MediaResource : RequestTargetKind.DynamicValue) : RequestTargetKind.EnumValue) : RequestTargetKind.PrimitiveValue;
      this.parsedSegments.Add(odataPathSegment);
      return true;
    }

    private void CreateDynamicPathSegment(
      ODataPathSegment previous,
      string identifier,
      string parenthesisExpression)
    {
      if (this.configuration.ParseDynamicPathSegmentFunc != null)
      {
        this.parsedSegments.AddRange((IEnumerable<ODataPathSegment>) this.configuration.ParseDynamicPathSegmentFunc(previous, identifier, parenthesisExpression));
      }
      else
      {
        if (previous == null)
          throw ExceptionUtil.CreateResourceNotFoundError(identifier);
        ODataPathParser.CheckSingleResult(previous.SingleResult, previous.Identifier);
        if (previous.TargetEdmType != null && !previous.TargetEdmType.IsOpen())
          throw ExceptionUtil.CreateResourceNotFoundError(identifier);
        if (parenthesisExpression != null)
          throw ExceptionUtil.CreateBadRequestError(Microsoft.OData.Strings.OpenNavigationPropertiesNotSupportedOnOpenTypes((object) identifier));
        this.parsedSegments.Add((ODataPathSegment) new DynamicPathSegment(identifier));
      }
    }

    private void CreateNamedStreamSegment(ODataPathSegment previous, IEdmProperty streamProperty)
    {
      ODataPathSegment odataPathSegment = (ODataPathSegment) new PropertySegment((IEdmStructuralProperty) streamProperty);
      odataPathSegment.TargetKind = RequestTargetKind.MediaResource;
      odataPathSegment.SingleResult = true;
      odataPathSegment.TargetEdmType = previous.TargetEdmType;
      this.parsedSegments.Add(odataPathSegment);
    }

    private void CreateFirstSegment(string segmentText)
    {
      string identifier;
      string parenthesisExpression;
      ODataPathParser.ExtractSegmentIdentifierAndParenthesisExpression(segmentText, out identifier, out parenthesisExpression);
      if (this.IdentifierIs("$metadata", identifier))
      {
        if (parenthesisExpression != null)
          throw ExceptionUtil.CreateSyntaxError();
        this.parsedSegments.Add((ODataPathSegment) MetadataSegment.Instance);
      }
      else if (this.IdentifierIs("$batch", identifier))
      {
        if (parenthesisExpression != null)
          throw ExceptionUtil.CreateSyntaxError();
        this.parsedSegments.Add((ODataPathSegment) BatchSegment.Instance);
      }
      else
      {
        if (this.IdentifierIs("$count", identifier))
          throw ExceptionUtil.ResourceNotFoundError(Microsoft.OData.Strings.RequestUriProcessor_CountOnRoot);
        if (this.IdentifierIs("$filter", identifier))
          throw ExceptionUtil.ResourceNotFoundError(Microsoft.OData.Strings.RequestUriProcessor_FilterOnRoot);
        if (this.IdentifierIs("$each", identifier))
          throw ExceptionUtil.ResourceNotFoundError(Microsoft.OData.Strings.RequestUriProcessor_EachOnRoot);
        if (this.IdentifierIs("$ref", identifier))
          throw ExceptionUtil.ResourceNotFoundError(Microsoft.OData.Strings.RequestUriProcessor_RefOnRoot);
        if (this.configuration.BatchReferenceCallback != null && ODataPathParser.ContentIdRegex.IsMatch(identifier))
        {
          if (parenthesisExpression != null)
            throw ExceptionUtil.CreateSyntaxError();
          BatchReferenceSegment referenceSegment = this.configuration.BatchReferenceCallback(identifier);
          if (referenceSegment != null)
          {
            this.parsedSegments.Add((ODataPathSegment) referenceSegment);
            return;
          }
        }
        if (this.TryCreateSegmentForNavigationSource(identifier, parenthesisExpression) || this.TryCreateSegmentForOperationImport(identifier, parenthesisExpression))
          return;
        this.CreateDynamicPathSegment((ODataPathSegment) null, identifier, parenthesisExpression);
      }
    }

    private bool TryCreateSegmentForNavigationSource(
      string identifier,
      string parenthesisExpression)
    {
      ODataPathSegment odataPathSegment = (ODataPathSegment) null;
      IEdmNavigationSource navigationSource = this.configuration.Resolver.ResolveNavigationSource(this.configuration.Model, identifier);
      if (navigationSource is IEdmEntitySet entitySet)
      {
        EntitySetSegment entitySetSegment = new EntitySetSegment(entitySet);
        entitySetSegment.Identifier = identifier;
        odataPathSegment = (ODataPathSegment) entitySetSegment;
      }
      else if (navigationSource is IEdmSingleton singleton)
      {
        SingletonSegment singletonSegment = new SingletonSegment(singleton);
        singletonSegment.Identifier = identifier;
        odataPathSegment = (ODataPathSegment) singletonSegment;
      }
      if (odataPathSegment == null)
        return false;
      this.parsedSegments.Add(odataPathSegment);
      this.TryBindKeyFromParentheses(parenthesisExpression);
      return true;
    }

    [SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "IEdmModel", Justification = "The spelling is correct.")]
    private bool TryCreateSegmentForOperationImport(string identifier, string parenthesisExpression)
    {
      ICollection<OperationSegmentParameter> boundParameters;
      IEdmOperationImport matchingFunctionImport;
      if (!ODataPathParser.TryBindingParametersAndMatchingOperationImport(identifier, parenthesisExpression, this.configuration, out boundParameters, out matchingFunctionImport))
        return false;
      IEdmTypeReference returnType = matchingFunctionImport.Operation.ReturnType;
      IEdmEntitySetBase entitySet = (IEdmEntitySetBase) null;
      if (returnType != null)
        entitySet = matchingFunctionImport.GetTargetEntitySet((IEdmEntitySetBase) null, this.configuration.Model);
      ODataPathSegment segment = (ODataPathSegment) new OperationImportSegment(matchingFunctionImport, entitySet, (IEnumerable<OperationSegmentParameter>) boundParameters);
      this.parsedSegments.Add(segment);
      this.TryBindKeySegmentIfNoResolvedParametersAndParenthesisValueExists(parenthesisExpression, returnType, boundParameters, segment);
      return true;
    }

    private void TryBindKeySegmentIfNoResolvedParametersAndParenthesisValueExists(
      string parenthesisExpression,
      IEdmTypeReference returnType,
      ICollection<OperationSegmentParameter> resolvedParameters,
      ODataPathSegment segment)
    {
      if (!(returnType is IEdmCollectionTypeReference type) || !type.ElementType().IsEntity() || resolvedParameters != null || parenthesisExpression == null || !this.TryBindKeyFromParentheses(parenthesisExpression))
        return;
      ODataPathParser.ThrowIfMustBeLeafSegment(segment);
    }

    [SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "IEdmModel", Justification = "The spelling is correct.")]
    private bool TryCreateSegmentForOperation(
      ODataPathSegment previousSegment,
      string identifier,
      string parenthesisExpression)
    {
      IEdmType bindingType = (IEdmType) null;
      if (previousSegment != null)
        bindingType = previousSegment is EachSegment ? previousSegment.TargetEdmType : previousSegment.EdmType;
      if (!string.IsNullOrEmpty(identifier) && identifier[0] == ':' && bindingType != null)
        identifier = ODataPathParser.ResolveEscapeFunction(identifier, bindingType, this.configuration.Model, out parenthesisExpression);
      ICollection<OperationSegmentParameter> boundParameters;
      IEdmOperation matchingOperation;
      if (!ODataPathParser.TryBindingParametersAndMatchingOperation(identifier, parenthesisExpression, bindingType, this.configuration, out boundParameters, out matchingOperation))
        return false;
      if (!UriEdmHelpers.IsBindingTypeValid(bindingType))
        throw ExceptionUtil.CreateBadRequestError(Microsoft.OData.Strings.RequestUriProcessor_OperationSegmentBoundToANonEntityType);
      if (previousSegment != null && bindingType == null)
        throw new ODataException(Microsoft.OData.Strings.FunctionCallBinder_CallingFunctionOnOpenProperty((object) identifier));
      IEdmTypeReference returnType = matchingOperation.ReturnType;
      IEdmEntitySetBase entitySet = (IEdmEntitySetBase) null;
      if (returnType != null)
      {
        IEdmNavigationSource navigationSource = previousSegment == null ? (IEdmNavigationSource) null : previousSegment.TargetEdmNavigationSource;
        entitySet = matchingOperation.GetTargetEntitySet(navigationSource, this.configuration.Model);
      }
      if (previousSegment is BatchReferenceSegment)
        throw ExceptionUtil.CreateBadRequestError(Microsoft.OData.Strings.RequestUriProcessor_BatchedActionOnEntityCreatedInSameChangeset((object) identifier));
      this.CheckOperationTypeCastSegmentRestriction(matchingOperation);
      OperationSegment operationSegment = new OperationSegment(matchingOperation, (IEnumerable<OperationSegmentParameter>) boundParameters, entitySet);
      operationSegment.Identifier = identifier;
      ODataPathSegment segment = (ODataPathSegment) operationSegment;
      this.parsedSegments.Add(segment);
      this.TryBindKeySegmentIfNoResolvedParametersAndParenthesisValueExists(parenthesisExpression, returnType, boundParameters, segment);
      return true;
    }

    private void CreateNextSegment(string text)
    {
      string identifier;
      string parenthesisExpression;
      ODataPathParser.ExtractSegmentIdentifierAndParenthesisExpression(text, out identifier, out parenthesisExpression);
      if (this.TryCreateValueSegment(identifier, parenthesisExpression))
        return;
      ODataPathSegment parsedSegment = this.parsedSegments[this.parsedSegments.Count - 1];
      if (parsedSegment.TargetKind == RequestTargetKind.Primitive)
        throw ExceptionUtil.ResourceNotFoundError(Microsoft.OData.Strings.RequestUriProcessor_ValueSegmentAfterScalarPropertySegment((object) parsedSegment.Identifier, (object) text));
      if (this.TryCreateEntityReferenceSegment(identifier, parenthesisExpression) || this.TryCreateCountSegment(identifier, parenthesisExpression) || this.TryCreateFilterSegment(text) || this.TryCreateEachSegment(identifier, parenthesisExpression))
        return;
      IEdmProperty projectedProperty;
      if (parsedSegment.SingleResult && parsedSegment.TargetEdmType != null && this.TryBindProperty(identifier, out projectedProperty))
      {
        ODataPathParser.CheckSingleResult(parsedSegment.SingleResult, parsedSegment.Identifier);
        this.CreatePropertySegment(parsedSegment, projectedProperty, parenthesisExpression);
      }
      else
      {
        if (text.IndexOf('.') >= 0 && this.TryCreateTypeNameSegment(parsedSegment, identifier, parenthesisExpression) || this.TryCreateSegmentForOperation(parsedSegment, identifier, parenthesisExpression) || this.configuration.UrlKeyDelimiter.EnableKeyAsSegment && this.TryHandleAsKeySegment(text))
          return;
        if (this.configuration.EnableUriTemplateParsing && UriTemplateParser.IsValidTemplateLiteral(text))
          this.parsedSegments.Add((ODataPathSegment) new PathTemplateSegment(text));
        else
          this.CreateDynamicPathSegment(parsedSegment, identifier, parenthesisExpression);
      }
    }

    private bool TryBindProperty(string identifier, out IEdmProperty projectedProperty)
    {
      ODataPathSegment parsedSegment = this.parsedSegments[this.parsedSegments.Count - 1];
      projectedProperty = (IEdmProperty) null;
      if (!(parsedSegment.TargetEdmType is IEdmStructuredType type) && parsedSegment.TargetEdmType is IEdmCollectionType targetEdmType)
        type = targetEdmType.ElementType.Definition as IEdmStructuredType;
      if (type == null)
        return false;
      projectedProperty = this.configuration.Resolver.ResolveProperty(type, identifier);
      return projectedProperty != null;
    }

    private bool TryCreateTypeNameSegment(
      ODataPathSegment previous,
      string identifier,
      string parenthesisExpression)
    {
      IEdmType typeFromModel;
      if (previous.TargetEdmType == null || (IEdmSchemaType) (typeFromModel = (IEdmType) UriEdmHelpers.FindTypeFromModel(this.configuration.Model, identifier, this.configuration.Resolver)) == null)
        return false;
      IEdmType edmType = previous.TargetEdmType;
      if (edmType.TypeKind == EdmTypeKind.Collection)
        edmType = ((IEdmCollectionType) edmType).ElementType.Definition;
      if (!typeFromModel.IsOrInheritsFrom(edmType) && !edmType.IsOrInheritsFrom(typeFromModel))
        throw ExceptionUtil.CreateBadRequestError(Microsoft.OData.Strings.RequestUriProcessor_InvalidTypeIdentifier_UnrelatedType((object) typeFromModel.FullTypeName(), (object) edmType.FullTypeName()));
      this.CheckTypeCastSegmentRestriction(previous, typeFromModel);
      IEdmType actualType = typeFromModel;
      if (previous.EdmType.TypeKind == EdmTypeKind.Collection)
      {
        switch (actualType)
        {
          case IEdmEntityType entityType:
            actualType = (IEdmType) new EdmCollectionType((IEdmTypeReference) new EdmEntityTypeReference(entityType, false));
            break;
          case IEdmComplexType complexType:
            actualType = (IEdmType) new EdmCollectionType((IEdmTypeReference) new EdmComplexTypeReference(complexType, false));
            break;
          default:
            throw new ODataException(Microsoft.OData.Strings.PathParser_TypeCastOnlyAllowedAfterStructuralCollection((object) identifier));
        }
      }
      TypeSegment typeSegment = new TypeSegment(actualType, previous.EdmType, previous.TargetEdmNavigationSource);
      typeSegment.Identifier = identifier;
      typeSegment.TargetKind = previous.TargetKind;
      typeSegment.SingleResult = previous.SingleResult;
      typeSegment.TargetEdmType = typeFromModel;
      this.parsedSegments.Add((ODataPathSegment) typeSegment);
      this.TryBindKeyFromParentheses(parenthesisExpression);
      return true;
    }

    [SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "IEdmModel", Justification = "The spelling is correct.")]
    private void CreatePropertySegment(
      ODataPathSegment previous,
      IEdmProperty property,
      string queryPortion)
    {
      if (property.Type.IsStream())
      {
        if (queryPortion != null)
          throw ExceptionUtil.CreateSyntaxError();
        this.CreateNamedStreamSegment(previous, property);
      }
      else
      {
        ODataPathSegment odataPathSegment;
        if (property.PropertyKind == EdmPropertyKind.Navigation)
        {
          IEdmNavigationProperty navigationProperty = (IEdmNavigationProperty) property;
          IEdmNavigationSource navigationSource = (IEdmNavigationSource) null;
          if (previous.TargetEdmNavigationSource != null)
            navigationSource = previous.TargetEdmNavigationSource.FindNavigationTarget(navigationProperty, new Func<IEdmPathExpression, List<ODataPathSegment>, bool>(BindingPathHelper.MatchBindingPath), this.parsedSegments, out IEdmPathExpression _);
          odataPathSegment = navigationProperty.TargetMultiplicity() != EdmMultiplicity.One || !(navigationSource is IEdmUnknownEntitySet) ? (ODataPathSegment) new NavigationPropertySegment(navigationProperty, navigationSource) : throw new ODataException(Microsoft.OData.Strings.RequestUriProcessor_TargetEntitySetNotFound((object) property.Name));
        }
        else
        {
          odataPathSegment = (ODataPathSegment) new PropertySegment((IEdmStructuralProperty) property);
          switch (property.Type.TypeKind())
          {
            case EdmTypeKind.Complex:
              odataPathSegment.TargetKind = RequestTargetKind.Resource;
              odataPathSegment.TargetEdmNavigationSource = previous.TargetEdmNavigationSource;
              break;
            case EdmTypeKind.Collection:
              if (property.Type.IsStructuredCollectionType())
              {
                odataPathSegment.TargetKind = RequestTargetKind.Resource;
                odataPathSegment.TargetEdmNavigationSource = previous.TargetEdmNavigationSource;
              }
              odataPathSegment.TargetKind = RequestTargetKind.Collection;
              break;
            case EdmTypeKind.Enum:
              odataPathSegment.TargetKind = RequestTargetKind.Enum;
              break;
            default:
              odataPathSegment.TargetKind = RequestTargetKind.Primitive;
              break;
          }
        }
        this.parsedSegments.Add(odataPathSegment);
        if (queryPortion != null && (!property.Type.IsCollection() || !property.Type.AsCollection().ElementType().IsEntity()))
          throw ExceptionUtil.CreateSyntaxError();
        this.TryBindKeyFromParentheses(queryPortion);
      }
    }

    private bool IdentifierIs(string expected, string identifier) => string.Equals(expected, identifier, this.configuration.EnableCaseInsensitiveUriFunctionIdentifier ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal);

    private List<ODataPathSegment> CreateValidatedPathSegments()
    {
      List<ODataPathSegment> validatedPathSegments = new List<ODataPathSegment>(this.parsedSegments.Count);
      int index = 0;
      for (int count = this.parsedSegments.Count; index < count; ++index)
      {
        this.CheckDollarEachSegmentRestrictions(index);
        validatedPathSegments.Add(this.parsedSegments[index]);
      }
      return validatedPathSegments;
    }

    private void CheckDollarEachSegmentRestrictions(int index)
    {
      int num = this.parsedSegments.Count - index - 1;
      if (!(this.parsedSegments[index] is EachSegment) || num <= 0)
        return;
      if (num > 1)
        throw new ODataException(Microsoft.OData.Strings.RequestUriProcessor_OnlySingleOperationCanFollowEachPathSegment);
      if (!(this.parsedSegments[index + 1] is OperationSegment))
        throw new ODataException(Microsoft.OData.Strings.RequestUriProcessor_OnlySingleOperationCanFollowEachPathSegment);
    }

    private void CheckTypeCastSegmentRestriction(ODataPathSegment previous, IEdmType targetEdmType)
    {
      if (previous.TargetEdmType.AsElementType() == targetEdmType)
        return;
      string fullTypeName = targetEdmType.FullTypeName();
      switch (previous)
      {
        case SingletonSegment singletonSegment:
          ODataPathParser.VerifyDerivedTypeConstraints(this.configuration.Model, (IEdmVocabularyAnnotatable) singletonSegment.Singleton, fullTypeName, "singleton", singletonSegment.Singleton.Name);
          break;
        case EntitySetSegment entitySetSegment2:
          ODataPathParser.VerifyDerivedTypeConstraints(this.configuration.Model, (IEdmVocabularyAnnotatable) entitySetSegment2.EntitySet, fullTypeName, "entity set", entitySetSegment2.EntitySet.Name);
          break;
        case KeySegment _:
          ODataPathSegment parsedSegment = this.parsedSegments[this.parsedSegments.Count - 2];
          EntitySetSegment entitySetSegment1 = parsedSegment as EntitySetSegment;
          NavigationPropertySegment navigationPropertySegment1 = parsedSegment as NavigationPropertySegment;
          if (entitySetSegment1 == null && navigationPropertySegment1 == null)
            break;
          IEdmVocabularyAnnotatable target;
          string kind;
          string name;
          if (entitySetSegment1 != null)
          {
            target = (IEdmVocabularyAnnotatable) entitySetSegment1.EntitySet;
            kind = "entity set";
            name = entitySetSegment1.EntitySet.Name;
          }
          else
          {
            target = (IEdmVocabularyAnnotatable) navigationPropertySegment1.NavigationProperty;
            kind = "navigation property";
            name = navigationPropertySegment1.NavigationProperty.Name;
          }
          ODataPathParser.VerifyDerivedTypeConstraints(this.configuration.Model, target, fullTypeName, kind, name);
          break;
        case NavigationPropertySegment navigationPropertySegment2:
          ODataPathParser.VerifyDerivedTypeConstraints(this.configuration.Model, (IEdmVocabularyAnnotatable) navigationPropertySegment2.NavigationProperty, fullTypeName, "navigation property", navigationPropertySegment2.NavigationProperty.Name);
          break;
        case PropertySegment propertySegment:
          IEdmProperty property = (IEdmProperty) propertySegment.Property;
          ODataPathParser.VerifyDerivedTypeConstraints(this.configuration.Model, (IEdmVocabularyAnnotatable) property, fullTypeName, "property", property.Name);
          break;
      }
    }

    private void CheckOperationTypeCastSegmentRestriction(IEdmOperation operation)
    {
      if (this.parsedSegments == null || !(this.parsedSegments.LastOrDefault<ODataPathSegment>((Func<ODataPathSegment, bool>) (s => s is TypeSegment)) is TypeSegment typeSegment))
        return;
      ODataPathSegment parsedSegment1 = this.parsedSegments[this.parsedSegments.Count - 1];
      ODataPathSegment parsedSegment2 = this.parsedSegments.Count >= 2 ? this.parsedSegments[this.parsedSegments.Count - 2] : (ODataPathSegment) null;
      if (typeSegment != parsedSegment1 && (typeSegment != parsedSegment2 || !(parsedSegment1 is KeySegment)) || !operation.IsBound)
        return;
      string fullTypeName = typeSegment.TargetEdmType.FullTypeName();
      IEdmOperationParameter target = operation.Parameters.First<IEdmOperationParameter>();
      IEdmType type = target.Type.Definition.AsElementType();
      if (fullTypeName == type.FullTypeName())
        return;
      ODataPathParser.VerifyDerivedTypeConstraints(this.configuration.Model, (IEdmVocabularyAnnotatable) target, fullTypeName, nameof (operation), operation.Name);
    }

    private static void VerifyDerivedTypeConstraints(
      IEdmModel model,
      IEdmVocabularyAnnotatable target,
      string fullTypeName,
      string kind,
      string name)
    {
      IEnumerable<string> derivedTypeConstraints = model.GetDerivedTypeConstraints(target);
      if (derivedTypeConstraints != null && !derivedTypeConstraints.Any<string>((Func<string, bool>) (d => d == fullTypeName)))
        throw new ODataException(Microsoft.OData.Strings.PathParser_TypeCastOnlyAllowedInDerivedTypeConstraint((object) fullTypeName, (object) kind, (object) name));
    }

    private static string ResolveEscapeFunction(
      string identifier,
      IEdmType bindingType,
      IEdmModel model,
      out string parenthesisExpression)
    {
      bool isComposableRequired = identifier.Length >= 2 && identifier[identifier.Length - 1] == ':';
      IEdmFunction element = model.FindBoundOperations(bindingType).OfType<IEdmFunction>().FirstOrDefault<IEdmFunction>((Func<IEdmFunction, bool>) (f => f.IsComposable == isComposableRequired && ODataPathParser.IsUrlEscapeFunction(model, f)));
      if (element == null)
        throw ExceptionUtil.CreateBadRequestError(Microsoft.OData.Strings.RequestUriProcessor_NoBoundEscapeFunctionSupported((object) bindingType.FullTypeName()));
      if (element.Parameters == null || element.Parameters.Count<IEdmOperationParameter>() != 2 || !element.Parameters.ElementAt<IEdmOperationParameter>(1).Type.IsString())
        throw ExceptionUtil.CreateBadRequestError(Microsoft.OData.Strings.RequestUriProcessor_EscapeFunctionMustHaveOneStringParameter((object) element.FullName()));
      parenthesisExpression = element.Parameters.ElementAt<IEdmOperationParameter>(1).Name + "='" + (isComposableRequired ? identifier.Substring(1, identifier.Length - 2) : identifier.Substring(1)) + "'";
      return element.FullName();
    }

    internal static bool IsUrlEscapeFunction(IEdmModel model, IEdmFunction function)
    {
      IEdmVocabularyAnnotation vocabularyAnnotation = model.FindVocabularyAnnotations<IEdmVocabularyAnnotation>((IEdmVocabularyAnnotatable) function, CommunityVocabularyModel.UrlEscapeFunctionTerm).FirstOrDefault<IEdmVocabularyAnnotation>();
      if (vocabularyAnnotation != null)
      {
        if (vocabularyAnnotation.Value == null)
          return true;
        if (vocabularyAnnotation.Value is IEdmBooleanConstantExpression constantExpression)
          return constantExpression.Value;
      }
      return false;
    }
  }
}
