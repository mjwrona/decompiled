// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.UriParser.ODataUriResolver
// Assembly: Microsoft.TeamFoundation.OData.Core, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6619C7F6-E44A-4143-AE77-6D570F968D9A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Core.dll

using Microsoft.OData.Edm;
using Microsoft.OData.Edm.Vocabularies;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.OData.UriParser
{
  public class ODataUriResolver
  {
    private static readonly ODataUriResolver Default = new ODataUriResolver();
    private TypeFacetsPromotionRules typeFacetsPromotionRules = new TypeFacetsPromotionRules();

    public virtual bool EnableCaseInsensitive { get; set; }

    public virtual bool EnableNoDollarQueryOptions { get; set; }

    public TypeFacetsPromotionRules TypeFacetsPromotionRules
    {
      get => this.typeFacetsPromotionRules;
      set => this.typeFacetsPromotionRules = value;
    }

    public virtual void PromoteBinaryOperandTypes(
      BinaryOperatorKind binaryOperatorKind,
      ref SingleValueNode leftNode,
      ref SingleValueNode rightNode,
      out IEdmTypeReference typeReference)
    {
      typeReference = (IEdmTypeReference) null;
      BinaryOperatorBinder.PromoteOperandTypes(binaryOperatorKind, ref leftNode, ref rightNode, this.typeFacetsPromotionRules);
    }

    public virtual IEdmNavigationSource ResolveNavigationSource(IEdmModel model, string identifier)
    {
      IEdmNavigationSource navigationSource = model.FindDeclaredNavigationSource(identifier);
      if (navigationSource != null || !this.EnableCaseInsensitive)
        return navigationSource;
      IEdmEntityContainer entityContainer = model.EntityContainer;
      if (entityContainer == null)
        return (IEdmNavigationSource) null;
      List<IEdmNavigationSource> list = entityContainer.Elements.OfType<IEdmNavigationSource>().Where<IEdmNavigationSource>((Func<IEdmNavigationSource, bool>) (source => string.Equals(identifier, source.Name, StringComparison.OrdinalIgnoreCase))).ToList<IEdmNavigationSource>();
      return list.Count <= 1 ? list.SingleOrDefault<IEdmNavigationSource>() : throw new ODataException(Microsoft.OData.Strings.UriParserMetadata_MultipleMatchingNavigationSourcesFound((object) identifier));
    }

    public virtual IEdmProperty ResolveProperty(IEdmStructuredType type, string propertyName)
    {
      IEdmProperty property = type.FindProperty(propertyName);
      if (property != null || !this.EnableCaseInsensitive)
        return property;
      List<IEdmProperty> list = type.Properties().Where<IEdmProperty>((Func<IEdmProperty, bool>) (_ => string.Equals(propertyName, _.Name, StringComparison.OrdinalIgnoreCase))).ToList<IEdmProperty>();
      return list.Count <= 1 ? list.SingleOrDefault<IEdmProperty>() : throw new ODataException(Microsoft.OData.Strings.UriParserMetadata_MultipleMatchingPropertiesFound((object) propertyName, (object) type.FullTypeName()));
    }

    public virtual IEdmTerm ResolveTerm(IEdmModel model, string termName)
    {
      IEdmTerm term = model.FindTerm(termName);
      if (term != null || !this.EnableCaseInsensitive)
        return term;
      IList<IEdmTerm> acrossModels = (IList<IEdmTerm>) ODataUriResolver.FindAcrossModels<IEdmTerm>(model, termName, true);
      return acrossModels.Count <= 1 ? acrossModels.SingleOrDefault<IEdmTerm>() : throw new ODataException(Microsoft.OData.Strings.UriParserMetadata_MultipleMatchingTypesFound((object) termName));
    }

    public virtual IEdmSchemaType ResolveType(IEdmModel model, string typeName)
    {
      IEdmSchemaType type = model.FindType(typeName);
      if (type != null || !this.EnableCaseInsensitive)
        return type;
      IList<IEdmSchemaType> acrossModels = (IList<IEdmSchemaType>) ODataUriResolver.FindAcrossModels<IEdmSchemaType>(model, typeName, true);
      return acrossModels.Count <= 1 ? acrossModels.SingleOrDefault<IEdmSchemaType>() : throw new ODataException(Microsoft.OData.Strings.UriParserMetadata_MultipleMatchingTypesFound((object) typeName));
    }

    public virtual IEnumerable<IEdmOperation> ResolveBoundOperations(
      IEdmModel model,
      string identifier,
      IEdmType bindingType)
    {
      IEnumerable<IEdmOperation> boundOperations = model.FindBoundOperations(identifier, bindingType);
      if (boundOperations.Any<IEdmOperation>() || !this.EnableCaseInsensitive)
        return boundOperations;
      IList<IEdmOperation> acrossModels = (IList<IEdmOperation>) ODataUriResolver.FindAcrossModels<IEdmOperation>(model, identifier, true);
      if (acrossModels == null || acrossModels.Count<IEdmOperation>() <= 0)
        return Enumerable.Empty<IEdmOperation>();
      IList<IEdmOperation> edmOperationList = (IList<IEdmOperation>) new List<IEdmOperation>();
      for (int index = 0; index < acrossModels.Count<IEdmOperation>(); ++index)
      {
        if (acrossModels[index].HasEquivalentBindingType(bindingType))
          edmOperationList.Add(acrossModels[index]);
      }
      return (IEnumerable<IEdmOperation>) edmOperationList;
    }

    public virtual IEnumerable<IEdmOperation> ResolveUnboundOperations(
      IEdmModel model,
      string identifier)
    {
      IEnumerable<IEdmOperation> operations = model.FindOperations(identifier);
      if (operations.Any<IEdmOperation>() || !this.EnableCaseInsensitive)
        return operations;
      IList<IEdmOperation> acrossModels = (IList<IEdmOperation>) ODataUriResolver.FindAcrossModels<IEdmOperation>(model, identifier, true);
      if (acrossModels == null || acrossModels.Count<IEdmOperation>() <= 0)
        return Enumerable.Empty<IEdmOperation>();
      IList<IEdmOperation> edmOperationList = (IList<IEdmOperation>) new List<IEdmOperation>();
      for (int index = 0; index < acrossModels.Count<IEdmOperation>(); ++index)
      {
        if (!acrossModels[index].IsBound)
          edmOperationList.Add(acrossModels[index]);
      }
      return (IEnumerable<IEdmOperation>) edmOperationList;
    }

    public virtual IEnumerable<IEdmOperationImport> ResolveOperationImports(
      IEdmModel model,
      string identifier)
    {
      IEnumerable<IEdmOperationImport> operationImports = model.FindDeclaredOperationImports(identifier);
      if (operationImports.Any<IEdmOperationImport>() || !this.EnableCaseInsensitive)
        return operationImports;
      IEdmEntityContainer entityContainer = model.EntityContainer;
      return entityContainer == null ? (IEnumerable<IEdmOperationImport>) null : entityContainer.Elements.OfType<IEdmOperationImport>().Where<IEdmOperationImport>((Func<IEdmOperationImport, bool>) (source => string.Equals(identifier, source.Name, StringComparison.OrdinalIgnoreCase)));
    }

    public virtual IDictionary<IEdmOperationParameter, SingleValueNode> ResolveOperationParameters(
      IEdmOperation operation,
      IDictionary<string, SingleValueNode> input)
    {
      Dictionary<IEdmOperationParameter, SingleValueNode> dictionary = new Dictionary<IEdmOperationParameter, SingleValueNode>((IEqualityComparer<IEdmOperationParameter>) EqualityComparer<IEdmOperationParameter>.Default);
      foreach (KeyValuePair<string, SingleValueNode> keyValuePair in (IEnumerable<KeyValuePair<string, SingleValueNode>>) input)
        dictionary.Add((!this.EnableCaseInsensitive ? operation.FindParameter(keyValuePair.Key) : ODataUriResolver.ResolveOperationParameterNameCaseInsensitive(operation, keyValuePair.Key)) ?? throw new ODataException(Microsoft.OData.Strings.ODataParameterWriterCore_ParameterNameNotFoundInOperation((object) keyValuePair.Key, (object) operation.Name)), keyValuePair.Value);
      return (IDictionary<IEdmOperationParameter, SingleValueNode>) dictionary;
    }

    public virtual IEnumerable<KeyValuePair<string, object>> ResolveKeys(
      IEdmEntityType type,
      IList<string> positionalValues,
      Func<IEdmTypeReference, string, object> convertFunc)
    {
      List<IEdmStructuralProperty> list = type.Key().ToList<IEdmStructuralProperty>();
      if (list.Count != positionalValues.Count)
        throw ExceptionUtil.CreateBadRequestError(Microsoft.OData.Strings.BadRequest_KeyCountMismatch((object) type.FullName()));
      List<KeyValuePair<string, object>> keyValuePairList = new List<KeyValuePair<string, object>>(positionalValues.Count);
      for (int index = 0; index < list.Count; ++index)
      {
        string positionalValue = positionalValues[index];
        IEdmProperty edmProperty = (IEdmProperty) list[index];
        keyValuePairList.Add(new KeyValuePair<string, object>(edmProperty.Name, convertFunc(edmProperty.Type, positionalValue) ?? throw ExceptionUtil.CreateSyntaxError()));
      }
      return (IEnumerable<KeyValuePair<string, object>>) keyValuePairList;
    }

    public virtual IEnumerable<KeyValuePair<string, object>> ResolveKeys(
      IEdmEntityType type,
      IDictionary<string, string> namedValues,
      Func<IEdmTypeReference, string, object> convertFunc)
    {
      Dictionary<string, object> dictionary = new Dictionary<string, object>((IEqualityComparer<string>) StringComparer.Ordinal);
      List<IEdmStructuralProperty> list1 = type.Key().ToList<IEdmStructuralProperty>();
      if (list1.Count != namedValues.Count)
        throw ExceptionUtil.CreateBadRequestError(Microsoft.OData.Strings.BadRequest_KeyCountMismatch((object) type.FullName()));
      foreach (IEdmStructuralProperty structuralProperty in list1)
      {
        IEdmStructuralProperty property = structuralProperty;
        string str;
        if (!namedValues.TryGetValue(property.Name, out str))
        {
          if (!this.EnableCaseInsensitive)
            throw ExceptionUtil.CreateSyntaxError();
          List<string> list2 = namedValues.Keys.Where<string>((Func<string, bool>) (key => string.Equals(property.Name, key, StringComparison.OrdinalIgnoreCase))).ToList<string>();
          if (list2.Count > 1)
            throw new ODataException(Microsoft.OData.Strings.UriParserMetadata_MultipleMatchingKeysFound((object) property.Name));
          str = list2.Count != 0 ? namedValues[list2.Single<string>()] : throw ExceptionUtil.CreateSyntaxError();
        }
        dictionary[property.Name] = convertFunc(property.Type, str) ?? throw ExceptionUtil.CreateSyntaxError();
      }
      return (IEnumerable<KeyValuePair<string, object>>) dictionary;
    }

    internal static IEdmOperationParameter ResolveOperationParameterNameCaseInsensitive(
      IEdmOperation operation,
      string identifier)
    {
      List<IEdmOperationParameter> list = operation.Parameters.Where<IEdmOperationParameter>((Func<IEdmOperationParameter, bool>) (parameter => string.Equals(identifier, parameter.Name, StringComparison.Ordinal))).ToList<IEdmOperationParameter>();
      if (list.Count == 0)
        list = operation.Parameters.Where<IEdmOperationParameter>((Func<IEdmOperationParameter, bool>) (parameter => string.Equals(identifier, parameter.Name, StringComparison.OrdinalIgnoreCase))).ToList<IEdmOperationParameter>();
      if (list.Count > 1)
        throw new ODataException(Microsoft.OData.Strings.UriParserMetadata_MultipleMatchingParametersFound((object) identifier));
      return list.Count == 1 ? list.Single<IEdmOperationParameter>() : (IEdmOperationParameter) null;
    }

    internal static ODataUriResolver GetUriResolver(IServiceProvider container) => container == null ? ODataUriResolver.Default : container.GetRequiredService<ODataUriResolver>();

    private static List<T> FindAcrossModels<T>(
      IEdmModel model,
      string qualifiedName,
      bool caseInsensitive)
      where T : IEdmSchemaElement
    {
      List<T> list = ODataUriResolver.FindSchemaElements<T>(model, qualifiedName, caseInsensitive).ToList<T>();
      foreach (IEdmModel referencedModel in model.ReferencedModels)
        list.AddRange(ODataUriResolver.FindSchemaElements<T>(referencedModel, qualifiedName, caseInsensitive));
      return list;
    }

    private static IEnumerable<T> FindSchemaElements<T>(
      IEdmModel model,
      string qualifiedName,
      bool caseInsensitive)
      where T : IEdmSchemaElement
    {
      return model.SchemaElements.OfType<T>().Where<T>((Func<T, bool>) (e => string.Equals(qualifiedName, e.FullName(), caseInsensitive ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal)));
    }
  }
}
