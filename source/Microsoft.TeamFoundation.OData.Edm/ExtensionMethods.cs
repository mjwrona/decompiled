// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.Edm.ExtensionMethods
// Assembly: Microsoft.TeamFoundation.OData.Edm, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 75C95BF5-2544-413A-959F-F9F18C11D35F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Edm.dll

using Microsoft.OData.Edm.Csdl;
using Microsoft.OData.Edm.Csdl.CsdlSemantics;
using Microsoft.OData.Edm.Csdl.Parsing.Ast;
using Microsoft.OData.Edm.Csdl.Serialization;
using Microsoft.OData.Edm.Validation;
using Microsoft.OData.Edm.Vocabularies;
using Microsoft.OData.Edm.Vocabularies.Community.V1;
using Microsoft.OData.Edm.Vocabularies.V1;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;

namespace Microsoft.OData.Edm
{
  public static class ExtensionMethods
  {
    private const int ContainerExtendsMaxDepth = 100;
    private const string CollectionTypeFormat = "Collection({0})";
    private static readonly IEnumerable<IEdmStructuralProperty> EmptyStructuralProperties = (IEnumerable<IEdmStructuralProperty>) new Collection<IEdmStructuralProperty>();
    private static readonly IEnumerable<IEdmNavigationProperty> EmptyNavigationProperties = (IEnumerable<IEdmNavigationProperty>) new Collection<IEdmNavigationProperty>();
    private static readonly Func<IEdmModel, string, IEdmSchemaType> findType = (Func<IEdmModel, string, IEdmSchemaType>) ((model, qualifiedName) => model.FindDeclaredType(qualifiedName));
    private static readonly Func<IEdmModel, IEdmType, IEnumerable<IEdmOperation>> findBoundOperations = (Func<IEdmModel, IEdmType, IEnumerable<IEdmOperation>>) ((model, bindingType) => model.FindDeclaredBoundOperations(bindingType));
    private static readonly Func<IEdmModel, string, IEdmTerm> findTerm = (Func<IEdmModel, string, IEdmTerm>) ((model, qualifiedName) => model.FindDeclaredTerm(qualifiedName));
    private static readonly Func<IEdmModel, string, IEnumerable<IEdmOperation>> findOperations = (Func<IEdmModel, string, IEnumerable<IEdmOperation>>) ((model, qualifiedName) => model.FindDeclaredOperations(qualifiedName));
    private static readonly Func<IEdmModel, string, IEdmEntityContainer> findEntityContainer = (Func<IEdmModel, string, IEdmEntityContainer>) ((model, qualifiedName) => !model.ExistsContainer(qualifiedName) ? (IEdmEntityContainer) null : model.EntityContainer);
    private static readonly Func<IEnumerable<IEdmOperation>, IEnumerable<IEdmOperation>, IEnumerable<IEdmOperation>> mergeFunctions = (Func<IEnumerable<IEdmOperation>, IEnumerable<IEdmOperation>, IEnumerable<IEdmOperation>>) ((f1, f2) => f1.Concat<IEdmOperation>(f2));

    public static Version GetEdmVersion(this IEdmModel model)
    {
      EdmUtil.CheckArgumentNull<IEdmModel>(model, nameof (model));
      return model.GetAnnotationValue<Version>((IEdmElement) model, "http://schemas.microsoft.com/ado/2011/04/edm/internal", "EdmVersion");
    }

    public static void SetEdmVersion(this IEdmModel model, Version version)
    {
      EdmUtil.CheckArgumentNull<IEdmModel>(model, nameof (model));
      model.SetAnnotationValue((IEdmElement) model, "http://schemas.microsoft.com/ado/2011/04/edm/internal", "EdmVersion", (object) version);
    }

    public static IEdmSchemaType FindType(this IEdmModel model, string qualifiedName)
    {
      EdmUtil.CheckArgumentNull<IEdmModel>(model, nameof (model));
      EdmUtil.CheckArgumentNull<string>(qualifiedName, nameof (qualifiedName));
      string qualifiedName1 = model.ReplaceAlias(qualifiedName);
      return model.FindAcrossModels<IEdmSchemaType, string>(qualifiedName1, ExtensionMethods.findType, new Func<IEdmSchemaType, IEdmSchemaType, IEdmSchemaType>(RegistrationHelper.CreateAmbiguousTypeBinding));
    }

    public static IEnumerable<IEdmOperation> FindBoundOperations(
      this IEdmModel model,
      IEdmType bindingType)
    {
      EdmUtil.CheckArgumentNull<IEdmModel>(model, nameof (model));
      EdmUtil.CheckArgumentNull<IEdmType>(bindingType, nameof (bindingType));
      return model.FindAcrossModels<IEnumerable<IEdmOperation>, IEdmType>(bindingType, ExtensionMethods.findBoundOperations, ExtensionMethods.mergeFunctions);
    }

    public static IEnumerable<IEdmOperation> FindBoundOperations(
      this IEdmModel model,
      string qualifiedName,
      IEdmType bindingType)
    {
      EdmUtil.CheckArgumentNull<IEdmModel>(model, nameof (model));
      EdmUtil.CheckArgumentNull<string>(qualifiedName, nameof (qualifiedName));
      EdmUtil.CheckArgumentNull<IEdmType>(bindingType, nameof (bindingType));
      string qualifiedName1 = model.ReplaceAlias(qualifiedName);
      IEnumerable<IEdmOperation> boundOperations = model.FindDeclaredBoundOperations(qualifiedName1, bindingType);
      foreach (IEdmModel referencedModel in model.ReferencedModels)
      {
        IEnumerable<IEdmOperation> declaredBoundOperations = referencedModel.FindDeclaredBoundOperations(qualifiedName1, bindingType);
        if (declaredBoundOperations != null)
          boundOperations = boundOperations == null ? declaredBoundOperations : ExtensionMethods.mergeFunctions(boundOperations, declaredBoundOperations);
      }
      return boundOperations;
    }

    public static IEdmTerm FindTerm(this IEdmModel model, string qualifiedName)
    {
      EdmUtil.CheckArgumentNull<IEdmModel>(model, nameof (model));
      EdmUtil.CheckArgumentNull<string>(qualifiedName, nameof (qualifiedName));
      return model.FindAcrossModels<IEdmTerm, string>(qualifiedName, ExtensionMethods.findTerm, new Func<IEdmTerm, IEdmTerm, IEdmTerm>(RegistrationHelper.CreateAmbiguousTermBinding));
    }

    public static IEnumerable<IEdmOperation> FindOperations(
      this IEdmModel model,
      string qualifiedName)
    {
      EdmUtil.CheckArgumentNull<IEdmModel>(model, nameof (model));
      EdmUtil.CheckArgumentNull<string>(qualifiedName, nameof (qualifiedName));
      return model.FindAcrossModels<IEnumerable<IEdmOperation>, string>(qualifiedName, ExtensionMethods.findOperations, ExtensionMethods.mergeFunctions);
    }

    public static bool ExistsContainer(this IEdmModel model, string containerName)
    {
      if (model.EntityContainer == null)
        return false;
      string b = (model.EntityContainer.Namespace ?? string.Empty) + "." + (containerName ?? string.Empty);
      return string.Equals(model.EntityContainer.FullName(), b, StringComparison.Ordinal) || string.Equals(model.EntityContainer.FullName(), containerName, StringComparison.Ordinal);
    }

    public static IEdmEntityContainer FindEntityContainer(
      this IEdmModel model,
      string qualifiedName)
    {
      EdmUtil.CheckArgumentNull<IEdmModel>(model, nameof (model));
      EdmUtil.CheckArgumentNull<string>(qualifiedName, nameof (qualifiedName));
      return model.FindAcrossModels<IEdmEntityContainer, string>(qualifiedName, ExtensionMethods.findEntityContainer, new Func<IEdmEntityContainer, IEdmEntityContainer, IEdmEntityContainer>(RegistrationHelper.CreateAmbiguousEntityContainerBinding));
    }

    public static IEnumerable<IEdmVocabularyAnnotation> FindVocabularyAnnotationsIncludingInheritedAnnotations(
      this IEdmModel model,
      IEdmVocabularyAnnotatable element)
    {
      EdmUtil.CheckArgumentNull<IEdmModel>(model, nameof (model));
      EdmUtil.CheckArgumentNull<IEdmVocabularyAnnotatable>(element, nameof (element));
      IEnumerable<IEdmVocabularyAnnotation> first = model.FindDeclaredVocabularyAnnotations(element);
      if (element is IEdmStructuredType edmStructuredType)
      {
        for (IEdmStructuredType baseType = edmStructuredType.BaseType; baseType != null; baseType = baseType.BaseType)
        {
          if (baseType is IEdmVocabularyAnnotatable element1)
            first = first.Concat<IEdmVocabularyAnnotation>(model.FindDeclaredVocabularyAnnotations(element1));
        }
      }
      return first;
    }

    public static IEnumerable<IEdmVocabularyAnnotation> FindVocabularyAnnotations(
      this IEdmModel model,
      IEdmVocabularyAnnotatable element)
    {
      EdmUtil.CheckArgumentNull<IEdmModel>(model, nameof (model));
      EdmUtil.CheckArgumentNull<IEdmVocabularyAnnotatable>(element, nameof (element));
      IEnumerable<IEdmVocabularyAnnotation> first = model.FindVocabularyAnnotationsIncludingInheritedAnnotations(element);
      foreach (IEdmModel referencedModel in model.ReferencedModels)
        first = first.Concat<IEdmVocabularyAnnotation>(referencedModel.FindVocabularyAnnotationsIncludingInheritedAnnotations(element));
      return first;
    }

    public static IEnumerable<T> FindVocabularyAnnotations<T>(
      this IEdmModel model,
      IEdmVocabularyAnnotatable element,
      IEdmTerm term)
      where T : IEdmVocabularyAnnotation
    {
      EdmUtil.CheckArgumentNull<IEdmModel>(model, nameof (model));
      EdmUtil.CheckArgumentNull<IEdmVocabularyAnnotatable>(element, nameof (element));
      EdmUtil.CheckArgumentNull<IEdmTerm>(term, nameof (term));
      return model.FindVocabularyAnnotations<T>(element, term, (string) null);
    }

    public static IEnumerable<T> FindVocabularyAnnotations<T>(
      this IEdmModel model,
      IEdmVocabularyAnnotatable element,
      IEdmTerm term,
      string qualifier)
      where T : IEdmVocabularyAnnotation
    {
      EdmUtil.CheckArgumentNull<IEdmModel>(model, nameof (model));
      EdmUtil.CheckArgumentNull<IEdmVocabularyAnnotatable>(element, nameof (element));
      EdmUtil.CheckArgumentNull<IEdmTerm>(term, nameof (term));
      List<T> objList = (List<T>) null;
      foreach (T obj in model.FindVocabularyAnnotations(element).OfType<T>())
      {
        if (obj.Term == term && (qualifier == null || qualifier == obj.Qualifier))
        {
          if (objList == null)
            objList = new List<T>();
          objList.Add(obj);
        }
      }
      return (IEnumerable<T>) objList ?? Enumerable.Empty<T>();
    }

    public static IEnumerable<T> FindVocabularyAnnotations<T>(
      this IEdmModel model,
      IEdmVocabularyAnnotatable element,
      string termName)
      where T : IEdmVocabularyAnnotation
    {
      EdmUtil.CheckArgumentNull<IEdmModel>(model, nameof (model));
      EdmUtil.CheckArgumentNull<IEdmVocabularyAnnotatable>(element, nameof (element));
      EdmUtil.CheckArgumentNull<string>(termName, nameof (termName));
      return model.FindVocabularyAnnotations<T>(element, termName, (string) null);
    }

    public static IEnumerable<T> FindVocabularyAnnotations<T>(
      this IEdmModel model,
      IEdmVocabularyAnnotatable element,
      string termName,
      string qualifier)
      where T : IEdmVocabularyAnnotation
    {
      EdmUtil.CheckArgumentNull<IEdmModel>(model, nameof (model));
      EdmUtil.CheckArgumentNull<IEdmVocabularyAnnotatable>(element, nameof (element));
      EdmUtil.CheckArgumentNull<string>(termName, nameof (termName));
      string name;
      string namespaceName;
      if (EdmUtil.TryGetNamespaceNameFromQualifiedName(termName, out namespaceName, out name))
      {
        foreach (T vocabularyAnnotation in model.FindVocabularyAnnotations(element).OfType<T>())
        {
          IEdmTerm term = vocabularyAnnotation.Term;
          if (term.Namespace == namespaceName && term.Name == name && (qualifier == null || qualifier == vocabularyAnnotation.Qualifier))
            yield return vocabularyAnnotation;
        }
      }
    }

    public static IEdmValue GetTermValue(
      this IEdmModel model,
      IEdmStructuredValue context,
      string termName,
      EdmExpressionEvaluator expressionEvaluator)
    {
      EdmUtil.CheckArgumentNull<IEdmModel>(model, nameof (model));
      EdmUtil.CheckArgumentNull<IEdmStructuredValue>(context, nameof (context));
      EdmUtil.CheckArgumentNull<string>(termName, nameof (termName));
      EdmUtil.CheckArgumentNull<EdmExpressionEvaluator>(expressionEvaluator, nameof (expressionEvaluator));
      return model.GetTermValue<IEdmValue>(context, context.Type.AsEntity().EntityDefinition(), termName, (string) null, new Func<IEdmExpression, IEdmStructuredValue, IEdmTypeReference, IEdmValue>(expressionEvaluator.Evaluate));
    }

    public static IEdmValue GetTermValue(
      this IEdmModel model,
      IEdmStructuredValue context,
      string termName,
      string qualifier,
      EdmExpressionEvaluator expressionEvaluator)
    {
      EdmUtil.CheckArgumentNull<IEdmModel>(model, nameof (model));
      EdmUtil.CheckArgumentNull<IEdmStructuredValue>(context, nameof (context));
      EdmUtil.CheckArgumentNull<string>(termName, nameof (termName));
      EdmUtil.CheckArgumentNull<EdmExpressionEvaluator>(expressionEvaluator, nameof (expressionEvaluator));
      return model.GetTermValue<IEdmValue>(context, context.Type.AsEntity().EntityDefinition(), termName, qualifier, new Func<IEdmExpression, IEdmStructuredValue, IEdmTypeReference, IEdmValue>(expressionEvaluator.Evaluate));
    }

    public static IEdmValue GetTermValue(
      this IEdmModel model,
      IEdmStructuredValue context,
      IEdmTerm term,
      EdmExpressionEvaluator expressionEvaluator)
    {
      EdmUtil.CheckArgumentNull<IEdmModel>(model, nameof (model));
      EdmUtil.CheckArgumentNull<IEdmStructuredValue>(context, nameof (context));
      EdmUtil.CheckArgumentNull<IEdmTerm>(term, nameof (term));
      EdmUtil.CheckArgumentNull<EdmExpressionEvaluator>(expressionEvaluator, nameof (expressionEvaluator));
      return model.GetTermValue<IEdmValue>(context, context.Type.AsEntity().EntityDefinition(), term, (string) null, new Func<IEdmExpression, IEdmStructuredValue, IEdmTypeReference, IEdmValue>(expressionEvaluator.Evaluate));
    }

    public static IEdmValue GetTermValue(
      this IEdmModel model,
      IEdmStructuredValue context,
      IEdmTerm term,
      string qualifier,
      EdmExpressionEvaluator expressionEvaluator)
    {
      EdmUtil.CheckArgumentNull<IEdmModel>(model, nameof (model));
      EdmUtil.CheckArgumentNull<IEdmStructuredValue>(context, nameof (context));
      EdmUtil.CheckArgumentNull<IEdmTerm>(term, nameof (term));
      EdmUtil.CheckArgumentNull<EdmExpressionEvaluator>(expressionEvaluator, nameof (expressionEvaluator));
      return model.GetTermValue<IEdmValue>(context, context.Type.AsEntity().EntityDefinition(), term, qualifier, new Func<IEdmExpression, IEdmStructuredValue, IEdmTypeReference, IEdmValue>(expressionEvaluator.Evaluate));
    }

    public static T GetTermValue<T>(
      this IEdmModel model,
      IEdmStructuredValue context,
      string termName,
      EdmToClrEvaluator evaluator)
    {
      EdmUtil.CheckArgumentNull<IEdmModel>(model, nameof (model));
      EdmUtil.CheckArgumentNull<IEdmStructuredValue>(context, nameof (context));
      EdmUtil.CheckArgumentNull<string>(termName, nameof (termName));
      EdmUtil.CheckArgumentNull<EdmToClrEvaluator>(evaluator, nameof (evaluator));
      return model.GetTermValue<T>(context, context.Type.AsEntity().EntityDefinition(), termName, (string) null, new Func<IEdmExpression, IEdmStructuredValue, IEdmTypeReference, T>(evaluator.EvaluateToClrValue<T>));
    }

    public static T GetTermValue<T>(
      this IEdmModel model,
      IEdmStructuredValue context,
      string termName,
      string qualifier,
      EdmToClrEvaluator evaluator)
    {
      EdmUtil.CheckArgumentNull<IEdmModel>(model, nameof (model));
      EdmUtil.CheckArgumentNull<IEdmStructuredValue>(context, nameof (context));
      EdmUtil.CheckArgumentNull<string>(termName, nameof (termName));
      EdmUtil.CheckArgumentNull<EdmToClrEvaluator>(evaluator, nameof (evaluator));
      return model.GetTermValue<T>(context, context.Type.AsEntity().EntityDefinition(), termName, qualifier, new Func<IEdmExpression, IEdmStructuredValue, IEdmTypeReference, T>(evaluator.EvaluateToClrValue<T>));
    }

    public static T GetTermValue<T>(
      this IEdmModel model,
      IEdmStructuredValue context,
      IEdmTerm term,
      EdmToClrEvaluator evaluator)
    {
      EdmUtil.CheckArgumentNull<IEdmModel>(model, nameof (model));
      EdmUtil.CheckArgumentNull<IEdmStructuredValue>(context, nameof (context));
      EdmUtil.CheckArgumentNull<IEdmTerm>(term, nameof (term));
      EdmUtil.CheckArgumentNull<EdmToClrEvaluator>(evaluator, nameof (evaluator));
      return model.GetTermValue<T>(context, context.Type.AsEntity().EntityDefinition(), term, (string) null, new Func<IEdmExpression, IEdmStructuredValue, IEdmTypeReference, T>(evaluator.EvaluateToClrValue<T>));
    }

    public static T GetTermValue<T>(
      this IEdmModel model,
      IEdmStructuredValue context,
      IEdmTerm term,
      string qualifier,
      EdmToClrEvaluator evaluator)
    {
      EdmUtil.CheckArgumentNull<IEdmModel>(model, nameof (model));
      EdmUtil.CheckArgumentNull<IEdmStructuredValue>(context, nameof (context));
      EdmUtil.CheckArgumentNull<IEdmTerm>(term, nameof (term));
      EdmUtil.CheckArgumentNull<EdmToClrEvaluator>(evaluator, nameof (evaluator));
      return model.GetTermValue<T>(context, context.Type.AsEntity().EntityDefinition(), term, qualifier, new Func<IEdmExpression, IEdmStructuredValue, IEdmTypeReference, T>(evaluator.EvaluateToClrValue<T>));
    }

    public static IEdmValue GetTermValue(
      this IEdmModel model,
      IEdmVocabularyAnnotatable element,
      string termName,
      EdmExpressionEvaluator expressionEvaluator)
    {
      EdmUtil.CheckArgumentNull<IEdmModel>(model, nameof (model));
      EdmUtil.CheckArgumentNull<IEdmVocabularyAnnotatable>(element, nameof (element));
      EdmUtil.CheckArgumentNull<string>(termName, nameof (termName));
      EdmUtil.CheckArgumentNull<EdmExpressionEvaluator>(expressionEvaluator, "evaluator");
      return model.GetTermValue<IEdmValue>(element, termName, (string) null, new Func<IEdmExpression, IEdmStructuredValue, IEdmTypeReference, IEdmValue>(expressionEvaluator.Evaluate));
    }

    public static IEdmValue GetTermValue(
      this IEdmModel model,
      IEdmVocabularyAnnotatable element,
      string termName,
      string qualifier,
      EdmExpressionEvaluator expressionEvaluator)
    {
      EdmUtil.CheckArgumentNull<IEdmModel>(model, nameof (model));
      EdmUtil.CheckArgumentNull<IEdmVocabularyAnnotatable>(element, nameof (element));
      EdmUtil.CheckArgumentNull<string>(termName, nameof (termName));
      EdmUtil.CheckArgumentNull<EdmExpressionEvaluator>(expressionEvaluator, "evaluator");
      return model.GetTermValue<IEdmValue>(element, termName, qualifier, new Func<IEdmExpression, IEdmStructuredValue, IEdmTypeReference, IEdmValue>(expressionEvaluator.Evaluate));
    }

    public static IEdmValue GetTermValue(
      this IEdmModel model,
      IEdmVocabularyAnnotatable element,
      IEdmTerm term,
      EdmExpressionEvaluator expressionEvaluator)
    {
      EdmUtil.CheckArgumentNull<IEdmModel>(model, nameof (model));
      EdmUtil.CheckArgumentNull<IEdmVocabularyAnnotatable>(element, nameof (element));
      EdmUtil.CheckArgumentNull<IEdmTerm>(term, nameof (term));
      EdmUtil.CheckArgumentNull<EdmExpressionEvaluator>(expressionEvaluator, "evaluator");
      return model.GetTermValue<IEdmValue>(element, term, (string) null, new Func<IEdmExpression, IEdmStructuredValue, IEdmTypeReference, IEdmValue>(expressionEvaluator.Evaluate));
    }

    public static IEdmValue GetTermValue(
      this IEdmModel model,
      IEdmVocabularyAnnotatable element,
      IEdmTerm term,
      string qualifier,
      EdmExpressionEvaluator expressionEvaluator)
    {
      EdmUtil.CheckArgumentNull<IEdmModel>(model, nameof (model));
      EdmUtil.CheckArgumentNull<IEdmVocabularyAnnotatable>(element, nameof (element));
      EdmUtil.CheckArgumentNull<IEdmTerm>(term, nameof (term));
      EdmUtil.CheckArgumentNull<EdmExpressionEvaluator>(expressionEvaluator, "evaluator");
      return model.GetTermValue<IEdmValue>(element, term, qualifier, new Func<IEdmExpression, IEdmStructuredValue, IEdmTypeReference, IEdmValue>(expressionEvaluator.Evaluate));
    }

    public static T GetTermValue<T>(
      this IEdmModel model,
      IEdmVocabularyAnnotatable element,
      string termName,
      EdmToClrEvaluator evaluator)
    {
      EdmUtil.CheckArgumentNull<IEdmModel>(model, nameof (model));
      EdmUtil.CheckArgumentNull<IEdmVocabularyAnnotatable>(element, nameof (element));
      EdmUtil.CheckArgumentNull<string>(termName, nameof (termName));
      EdmUtil.CheckArgumentNull<EdmToClrEvaluator>(evaluator, nameof (evaluator));
      return model.GetTermValue<T>(element, termName, (string) null, new Func<IEdmExpression, IEdmStructuredValue, IEdmTypeReference, T>(evaluator.EvaluateToClrValue<T>));
    }

    public static T GetTermValue<T>(
      this IEdmModel model,
      IEdmVocabularyAnnotatable element,
      string termName,
      string qualifier,
      EdmToClrEvaluator evaluator)
    {
      EdmUtil.CheckArgumentNull<IEdmModel>(model, nameof (model));
      EdmUtil.CheckArgumentNull<IEdmVocabularyAnnotatable>(element, nameof (element));
      EdmUtil.CheckArgumentNull<string>(termName, nameof (termName));
      EdmUtil.CheckArgumentNull<EdmToClrEvaluator>(evaluator, nameof (evaluator));
      return model.GetTermValue<T>(element, termName, qualifier, new Func<IEdmExpression, IEdmStructuredValue, IEdmTypeReference, T>(evaluator.EvaluateToClrValue<T>));
    }

    public static T GetTermValue<T>(
      this IEdmModel model,
      IEdmVocabularyAnnotatable element,
      IEdmTerm term,
      EdmToClrEvaluator evaluator)
    {
      EdmUtil.CheckArgumentNull<IEdmModel>(model, nameof (model));
      EdmUtil.CheckArgumentNull<IEdmVocabularyAnnotatable>(element, nameof (element));
      EdmUtil.CheckArgumentNull<IEdmTerm>(term, nameof (term));
      EdmUtil.CheckArgumentNull<EdmToClrEvaluator>(evaluator, nameof (evaluator));
      return model.GetTermValue<T>(element, term, (string) null, new Func<IEdmExpression, IEdmStructuredValue, IEdmTypeReference, T>(evaluator.EvaluateToClrValue<T>));
    }

    public static T GetTermValue<T>(
      this IEdmModel model,
      IEdmVocabularyAnnotatable element,
      IEdmTerm term,
      string qualifier,
      EdmToClrEvaluator evaluator)
    {
      EdmUtil.CheckArgumentNull<IEdmModel>(model, nameof (model));
      EdmUtil.CheckArgumentNull<IEdmVocabularyAnnotatable>(element, nameof (element));
      EdmUtil.CheckArgumentNull<IEdmTerm>(term, nameof (term));
      EdmUtil.CheckArgumentNull<EdmToClrEvaluator>(evaluator, nameof (evaluator));
      return model.GetTermValue<T>(element, term, qualifier, new Func<IEdmExpression, IEdmStructuredValue, IEdmTypeReference, T>(evaluator.EvaluateToClrValue<T>));
    }

    public static object GetAnnotationValue(
      this IEdmModel model,
      IEdmElement element,
      string namespaceName,
      string localName)
    {
      EdmUtil.CheckArgumentNull<IEdmModel>(model, nameof (model));
      EdmUtil.CheckArgumentNull<IEdmElement>(element, nameof (element));
      return model.DirectValueAnnotationsManager.GetAnnotationValue(element, namespaceName, localName);
    }

    public static T GetAnnotationValue<T>(
      this IEdmModel model,
      IEdmElement element,
      string namespaceName,
      string localName)
      where T : class
    {
      EdmUtil.CheckArgumentNull<IEdmModel>(model, nameof (model));
      EdmUtil.CheckArgumentNull<IEdmElement>(element, nameof (element));
      return ExtensionMethods.AnnotationValue<T>(model.GetAnnotationValue(element, namespaceName, localName));
    }

    public static T GetAnnotationValue<T>(this IEdmModel model, IEdmElement element) where T : class
    {
      EdmUtil.CheckArgumentNull<IEdmModel>(model, nameof (model));
      EdmUtil.CheckArgumentNull<IEdmElement>(element, nameof (element));
      return model.GetAnnotationValue<T>(element, "http://schemas.microsoft.com/ado/2011/04/edm/internal", ExtensionMethods.TypeName<T>.LocalName);
    }

    public static void SetAnnotationValue(
      this IEdmModel model,
      IEdmElement element,
      string namespaceName,
      string localName,
      object value)
    {
      EdmUtil.CheckArgumentNull<IEdmModel>(model, nameof (model));
      EdmUtil.CheckArgumentNull<IEdmElement>(element, nameof (element));
      model.DirectValueAnnotationsManager.SetAnnotationValue(element, namespaceName, localName, value);
    }

    public static string GetDescriptionAnnotation(
      this IEdmModel model,
      IEdmVocabularyAnnotatable target)
    {
      EdmUtil.CheckArgumentNull<IEdmModel>(model, nameof (model));
      EdmUtil.CheckArgumentNull<IEdmVocabularyAnnotatable>(target, nameof (target));
      IEdmVocabularyAnnotation vocabularyAnnotation = model.FindVocabularyAnnotations<IEdmVocabularyAnnotation>(target, CoreVocabularyModel.DescriptionTerm).FirstOrDefault<IEdmVocabularyAnnotation>();
      return vocabularyAnnotation != null && vocabularyAnnotation.Value is IEdmStringConstantExpression constantExpression ? constantExpression.Value : (string) null;
    }

    public static string GetLongDescriptionAnnotation(
      this IEdmModel model,
      IEdmVocabularyAnnotatable target)
    {
      EdmUtil.CheckArgumentNull<IEdmModel>(model, nameof (model));
      EdmUtil.CheckArgumentNull<IEdmVocabularyAnnotatable>(target, nameof (target));
      IEdmVocabularyAnnotation vocabularyAnnotation = model.FindVocabularyAnnotations<IEdmVocabularyAnnotation>(target, CoreVocabularyModel.LongDescriptionTerm).FirstOrDefault<IEdmVocabularyAnnotation>();
      return vocabularyAnnotation != null && vocabularyAnnotation.Value is IEdmStringConstantExpression constantExpression ? constantExpression.Value : (string) null;
    }

    public static IEnumerable<string> GetDerivedTypeConstraints(
      this IEdmModel model,
      IEdmNavigationSource navigationSource)
    {
      if (model == null || navigationSource == null)
        return (IEnumerable<string>) null;
      IEnumerable<string> derivedTypeConstraints = (IEnumerable<string>) null;
      switch (navigationSource.NavigationSourceKind())
      {
        case EdmNavigationSourceKind.EntitySet:
          derivedTypeConstraints = model.GetDerivedTypeConstraints((IEdmVocabularyAnnotatable) navigationSource);
          break;
        case EdmNavigationSourceKind.Singleton:
          derivedTypeConstraints = model.GetDerivedTypeConstraints((IEdmVocabularyAnnotatable) navigationSource);
          break;
      }
      return derivedTypeConstraints;
    }

    public static IEnumerable<string> GetDerivedTypeConstraints(
      this IEdmModel model,
      IEdmVocabularyAnnotatable target)
    {
      if (model == null || target == null)
        return (IEnumerable<string>) null;
      IEdmVocabularyAnnotation vocabularyAnnotation = model.FindVocabularyAnnotations<IEdmVocabularyAnnotation>(target, ValidationVocabularyModel.DerivedTypeConstraintTerm).FirstOrDefault<IEdmVocabularyAnnotation>();
      return vocabularyAnnotation != null && vocabularyAnnotation.Value is IEdmCollectionExpression collectionExpression && collectionExpression.Elements != null ? collectionExpression.Elements.OfType<IEdmStringConstantExpression>().Select<IEdmStringConstantExpression, string>((Func<IEdmStringConstantExpression, string>) (e => e.Value)) : (IEnumerable<string>) null;
    }

    public static IEnumerable<IEdmSchemaElement> SchemaElementsAcrossModels(this IEdmModel model)
    {
      EdmUtil.CheckArgumentNull<IEdmModel>(model, nameof (model));
      IEnumerable<IEdmSchemaElement> first = (IEnumerable<IEdmSchemaElement>) new IEdmSchemaElement[0];
      foreach (IEdmModel referencedModel in model.ReferencedModels)
        first = first.Concat<IEdmSchemaElement>(referencedModel.SchemaElements);
      return first.Concat<IEdmSchemaElement>(model.SchemaElements);
    }

    public static IEnumerable<IEdmStructuredType> FindAllDerivedTypes(
      this IEdmModel model,
      IEdmStructuredType baseType)
    {
      List<IEdmStructuredType> derivedTypes = new List<IEdmStructuredType>();
      if (baseType is IEdmSchemaElement)
        model.DerivedFrom(baseType, new HashSetInternal<IEdmStructuredType>(), derivedTypes);
      return (IEnumerable<IEdmStructuredType>) derivedTypes;
    }

    public static void SetAnnotationValue<T>(this IEdmModel model, IEdmElement element, T value) where T : class
    {
      EdmUtil.CheckArgumentNull<IEdmModel>(model, nameof (model));
      EdmUtil.CheckArgumentNull<IEdmElement>(element, nameof (element));
      model.SetAnnotationValue(element, "http://schemas.microsoft.com/ado/2011/04/edm/internal", ExtensionMethods.TypeName<T>.LocalName, (object) value);
    }

    public static object[] GetAnnotationValues(
      this IEdmModel model,
      IEnumerable<IEdmDirectValueAnnotationBinding> annotations)
    {
      EdmUtil.CheckArgumentNull<IEdmModel>(model, nameof (model));
      EdmUtil.CheckArgumentNull<IEnumerable<IEdmDirectValueAnnotationBinding>>(annotations, nameof (annotations));
      return model.DirectValueAnnotationsManager.GetAnnotationValues(annotations);
    }

    public static void SetAnnotationValues(
      this IEdmModel model,
      IEnumerable<IEdmDirectValueAnnotationBinding> annotations)
    {
      EdmUtil.CheckArgumentNull<IEdmModel>(model, nameof (model));
      EdmUtil.CheckArgumentNull<IEnumerable<IEdmDirectValueAnnotationBinding>>(annotations, nameof (annotations));
      model.DirectValueAnnotationsManager.SetAnnotationValues(annotations);
    }

    public static IEnumerable<IEdmDirectValueAnnotation> DirectValueAnnotations(
      this IEdmModel model,
      IEdmElement element)
    {
      EdmUtil.CheckArgumentNull<IEdmModel>(model, nameof (model));
      EdmUtil.CheckArgumentNull<IEdmElement>(element, nameof (element));
      return model.DirectValueAnnotationsManager.GetDirectValueAnnotations(element);
    }

    public static bool TryFindContainerQualifiedEntitySet(
      this IEdmModel model,
      string containerQualifiedEntitySetName,
      out IEdmEntitySet entitySet)
    {
      entitySet = (IEdmEntitySet) null;
      string containerName = (string) null;
      string containerElementName = (string) null;
      if (containerQualifiedEntitySetName != null && containerQualifiedEntitySetName.IndexOf(".", StringComparison.Ordinal) > -1 && EdmUtil.TryParseContainerQualifiedElementName(containerQualifiedEntitySetName, out containerName, out containerElementName) && model.ExistsContainer(containerName))
      {
        IEdmEntityContainer entityContainer = model.EntityContainer;
        if (entityContainer != null)
          entitySet = entityContainer.FindEntitySetExtended(containerElementName);
      }
      return entitySet != null;
    }

    public static bool TryFindContainerQualifiedSingleton(
      this IEdmModel model,
      string containerQualifiedSingletonName,
      out IEdmSingleton singleton)
    {
      singleton = (IEdmSingleton) null;
      string containerName = (string) null;
      string containerElementName = (string) null;
      if (containerQualifiedSingletonName != null && containerQualifiedSingletonName.IndexOf(".", StringComparison.Ordinal) > -1 && EdmUtil.TryParseContainerQualifiedElementName(containerQualifiedSingletonName, out containerName, out containerElementName) && model.ExistsContainer(containerName))
      {
        singleton = model.EntityContainer.FindSingletonExtended(containerElementName);
        if (singleton != null)
          return true;
      }
      return false;
    }

    public static bool TryFindContainerQualifiedOperationImports(
      this IEdmModel model,
      string containerQualifiedOperationImportName,
      out IEnumerable<IEdmOperationImport> operationImports)
    {
      operationImports = (IEnumerable<IEdmOperationImport>) null;
      string containerName = (string) null;
      string containerElementName = (string) null;
      if (containerQualifiedOperationImportName.IndexOf(".", StringComparison.Ordinal) > -1 && EdmUtil.TryParseContainerQualifiedElementName(containerQualifiedOperationImportName, out containerName, out containerElementName) && model.ExistsContainer(containerName))
      {
        operationImports = model.EntityContainer.FindOperationImportsExtended(containerElementName);
        if (operationImports != null && operationImports.Count<IEdmOperationImport>() > 0)
          return true;
      }
      return false;
    }

    public static IEdmEntitySet FindDeclaredEntitySet(this IEdmModel model, string qualifiedName)
    {
      IEdmEntitySet entitySet = (IEdmEntitySet) null;
      if (!model.TryFindContainerQualifiedEntitySet(qualifiedName, out entitySet))
      {
        try
        {
          IEdmEntityContainer entityContainer = model.EntityContainer;
          if (entityContainer != null)
            return entityContainer.FindEntitySetExtended(qualifiedName);
        }
        catch (NotImplementedException ex)
        {
          return (IEdmEntitySet) null;
        }
      }
      return entitySet;
    }

    public static IEdmSingleton FindDeclaredSingleton(this IEdmModel model, string qualifiedName)
    {
      IEdmSingleton singleton = (IEdmSingleton) null;
      if (!model.TryFindContainerQualifiedSingleton(qualifiedName, out singleton))
      {
        try
        {
          IEdmEntityContainer entityContainer = model.EntityContainer;
          if (entityContainer != null)
            return entityContainer.FindSingletonExtended(qualifiedName);
        }
        catch (NotImplementedException ex)
        {
          return (IEdmSingleton) null;
        }
      }
      return singleton;
    }

    public static IEdmNavigationSource FindDeclaredNavigationSource(
      this IEdmModel model,
      string qualifiedName)
    {
      return (IEdmNavigationSource) model.FindDeclaredEntitySet(qualifiedName) ?? (IEdmNavigationSource) model.FindDeclaredSingleton(qualifiedName);
    }

    public static IEnumerable<IEdmOperationImport> FindDeclaredOperationImports(
      this IEdmModel model,
      string qualifiedName)
    {
      IEnumerable<IEdmOperationImport> operationImports = (IEnumerable<IEdmOperationImport>) null;
      if (!model.TryFindContainerQualifiedOperationImports(qualifiedName, out operationImports))
      {
        IEdmEntityContainer entityContainer = model.EntityContainer;
        if (entityContainer != null)
          return entityContainer.FindOperationImportsExtended(qualifiedName);
      }
      return operationImports;
    }

    public static IPrimitiveValueConverter GetPrimitiveValueConverter(
      this IEdmModel model,
      IEdmTypeReference type)
    {
      EdmUtil.CheckArgumentNull<IEdmModel>(model, "mode");
      return type == null || !type.IsTypeDefinition() ? PassThroughPrimitiveValueConverter.Instance : model.GetPrimitiveValueConverter(type.Definition);
    }

    public static void SetPrimitiveValueConverter(
      this IEdmModel model,
      IEdmTypeDefinitionReference typeDefinition,
      IPrimitiveValueConverter converter)
    {
      EdmUtil.CheckArgumentNull<IEdmModel>(model, nameof (model));
      EdmUtil.CheckArgumentNull<IEdmTypeDefinitionReference>(typeDefinition, nameof (typeDefinition));
      EdmUtil.CheckArgumentNull<IPrimitiveValueConverter>(converter, nameof (converter));
      model.SetPrimitiveValueConverter(typeDefinition.Definition, converter);
    }

    public static EdmComplexType AddComplexType(
      this EdmModel model,
      string namespaceName,
      string name)
    {
      return model.AddComplexType(namespaceName, name, (IEdmComplexType) null, false);
    }

    public static EdmComplexType AddComplexType(
      this EdmModel model,
      string namespaceName,
      string name,
      IEdmComplexType baseType)
    {
      return model.AddComplexType(namespaceName, name, baseType, false, false);
    }

    public static EdmComplexType AddComplexType(
      this EdmModel model,
      string namespaceName,
      string name,
      IEdmComplexType baseType,
      bool isAbstract)
    {
      return model.AddComplexType(namespaceName, name, baseType, isAbstract, false);
    }

    public static EdmComplexType AddComplexType(
      this EdmModel model,
      string namespaceName,
      string name,
      IEdmComplexType baseType,
      bool isAbstract,
      bool isOpen)
    {
      EdmComplexType element = new EdmComplexType(namespaceName, name, baseType, isAbstract, isOpen);
      model.AddElement((IEdmSchemaElement) element);
      return element;
    }

    public static EdmEntityType AddEntityType(
      this EdmModel model,
      string namespaceName,
      string name)
    {
      return model.AddEntityType(namespaceName, name, (IEdmEntityType) null, false, false);
    }

    public static EdmEntityType AddEntityType(
      this EdmModel model,
      string namespaceName,
      string name,
      IEdmEntityType baseType)
    {
      return model.AddEntityType(namespaceName, name, baseType, false, false);
    }

    public static EdmEntityType AddEntityType(
      this EdmModel model,
      string namespaceName,
      string name,
      IEdmEntityType baseType,
      bool isAbstract,
      bool isOpen)
    {
      return model.AddEntityType(namespaceName, name, baseType, isAbstract, isOpen, false);
    }

    public static EdmEntityType AddEntityType(
      this EdmModel model,
      string namespaceName,
      string name,
      IEdmEntityType baseType,
      bool isAbstract,
      bool isOpen,
      bool hasStream)
    {
      EdmEntityType element = new EdmEntityType(namespaceName, name, baseType, isAbstract, isOpen, hasStream);
      model.AddElement((IEdmSchemaElement) element);
      return element;
    }

    public static EdmEntityContainer AddEntityContainer(
      this EdmModel model,
      string namespaceName,
      string name)
    {
      EdmEntityContainer element = new EdmEntityContainer(namespaceName, name);
      model.AddElement((IEdmSchemaElement) element);
      return element;
    }

    public static EdmTerm AddTerm(
      this EdmModel model,
      string namespaceName,
      string name,
      EdmPrimitiveTypeKind kind)
    {
      EdmTerm element = new EdmTerm(namespaceName, name, kind);
      model.AddElement((IEdmSchemaElement) element);
      return element;
    }

    public static EdmTerm AddTerm(
      this EdmModel model,
      string namespaceName,
      string name,
      IEdmTypeReference type)
    {
      return model.AddTerm(namespaceName, name, type, (string) null, (string) null);
    }

    public static EdmTerm AddTerm(
      this EdmModel model,
      string namespaceName,
      string name,
      IEdmTypeReference type,
      string appliesTo,
      string defaultValue)
    {
      EdmTerm element = new EdmTerm(namespaceName, name, type, appliesTo, defaultValue);
      model.AddElement((IEdmSchemaElement) element);
      return element;
    }

    public static void SetOptimisticConcurrencyAnnotation(
      this EdmModel model,
      IEdmEntitySet target,
      IEnumerable<IEdmStructuralProperty> properties)
    {
      EdmUtil.CheckArgumentNull<EdmModel>(model, nameof (model));
      EdmUtil.CheckArgumentNull<IEdmEntitySet>(target, nameof (target));
      EdmUtil.CheckArgumentNull<IEnumerable<IEdmStructuralProperty>>(properties, nameof (properties));
      IEdmCollectionExpression collectionExpression = (IEdmCollectionExpression) new EdmCollectionExpression((IEdmExpression[]) properties.Select<IEdmStructuralProperty, EdmPropertyPathExpression>((Func<IEdmStructuralProperty, EdmPropertyPathExpression>) (p => new EdmPropertyPathExpression(p.Name))).ToArray<EdmPropertyPathExpression>());
      IEdmTerm concurrencyTerm = CoreVocabularyModel.ConcurrencyTerm;
      EdmVocabularyAnnotation annotation = new EdmVocabularyAnnotation((IEdmVocabularyAnnotatable) target, concurrencyTerm, (IEdmExpression) collectionExpression);
      annotation.SetSerializationLocation((IEdmModel) model, new EdmVocabularyAnnotationSerializationLocation?(EdmVocabularyAnnotationSerializationLocation.Inline));
      model.SetVocabularyAnnotation((IEdmVocabularyAnnotation) annotation);
    }

    public static void SetDescriptionAnnotation(
      this EdmModel model,
      IEdmVocabularyAnnotatable target,
      string description)
    {
      EdmUtil.CheckArgumentNull<EdmModel>(model, nameof (model));
      EdmUtil.CheckArgumentNull<IEdmVocabularyAnnotatable>(target, nameof (target));
      EdmUtil.CheckArgumentNull<string>(description, nameof (description));
      EdmVocabularyAnnotation annotation = new EdmVocabularyAnnotation(target, CoreVocabularyModel.DescriptionTerm, (IEdmExpression) new EdmStringConstant(description));
      annotation.SetSerializationLocation((IEdmModel) model, new EdmVocabularyAnnotationSerializationLocation?(EdmVocabularyAnnotationSerializationLocation.Inline));
      model.SetVocabularyAnnotation((IEdmVocabularyAnnotation) annotation);
    }

    public static void SetLongDescriptionAnnotation(
      this EdmModel model,
      IEdmVocabularyAnnotatable target,
      string description)
    {
      EdmUtil.CheckArgumentNull<EdmModel>(model, nameof (model));
      EdmUtil.CheckArgumentNull<IEdmVocabularyAnnotatable>(target, nameof (target));
      EdmUtil.CheckArgumentNull<string>(description, nameof (description));
      EdmVocabularyAnnotation annotation = new EdmVocabularyAnnotation(target, CoreVocabularyModel.LongDescriptionTerm, (IEdmExpression) new EdmStringConstant(description));
      annotation.SetSerializationLocation((IEdmModel) model, new EdmVocabularyAnnotationSerializationLocation?(EdmVocabularyAnnotationSerializationLocation.Inline));
      model.SetVocabularyAnnotation((IEdmVocabularyAnnotation) annotation);
    }

    public static void SetChangeTrackingAnnotation(
      this EdmModel model,
      IEdmEntityContainer target,
      bool isSupported)
    {
      model.SetChangeTrackingAnnotationImplementation((IEdmVocabularyAnnotatable) target, isSupported, (IEnumerable<IEdmStructuralProperty>) null, (IEnumerable<IEdmNavigationProperty>) null);
    }

    public static void SetChangeTrackingAnnotation(
      this EdmModel model,
      IEdmEntitySet target,
      bool isSupported,
      IEnumerable<IEdmStructuralProperty> filterableProperties,
      IEnumerable<IEdmNavigationProperty> expandableProperties)
    {
      model.SetChangeTrackingAnnotationImplementation((IEdmVocabularyAnnotatable) target, isSupported, filterableProperties, expandableProperties);
    }

    public static IEdmTypeDefinitionReference GetUInt16(
      this EdmModel model,
      string namespaceName,
      bool isNullable)
    {
      return model.GetUIntImplementation(namespaceName, "UInt16", "Edm.Int32", isNullable);
    }

    public static IEdmTypeDefinitionReference GetUInt32(
      this EdmModel model,
      string namespaceName,
      bool isNullable)
    {
      return model.GetUIntImplementation(namespaceName, "UInt32", "Edm.Int64", isNullable);
    }

    public static IEdmTypeDefinitionReference GetUInt64(
      this EdmModel model,
      string namespaceName,
      bool isNullable)
    {
      return model.GetUIntImplementation(namespaceName, "UInt64", "Edm.Decimal", isNullable);
    }

    public static EdmLocation Location(this IEdmElement item)
    {
      EdmUtil.CheckArgumentNull<IEdmElement>(item, nameof (item));
      return !(item is IEdmLocatable edmLocatable) || edmLocatable.Location == null ? (EdmLocation) new ObjectLocation((object) item) : edmLocatable.Location;
    }

    public static IEnumerable<IEdmVocabularyAnnotation> VocabularyAnnotations(
      this IEdmVocabularyAnnotatable element,
      IEdmModel model)
    {
      EdmUtil.CheckArgumentNull<IEdmVocabularyAnnotatable>(element, nameof (element));
      EdmUtil.CheckArgumentNull<IEdmModel>(model, nameof (model));
      return model.FindVocabularyAnnotations(element);
    }

    public static string FullName(this IEdmSchemaElement element)
    {
      EdmUtil.CheckArgumentNull<IEdmSchemaElement>(element, nameof (element));
      if (element.Name == null)
        return string.Empty;
      if (element.Namespace == null)
        return element.Name;
      return element is IEdmFullNamedElement fullNamedElement ? fullNamedElement.FullName : element.Namespace + "." + element.Name;
    }

    public static string ShortQualifiedName(this IEdmSchemaElement element)
    {
      EdmUtil.CheckArgumentNull<IEdmSchemaElement>(element, nameof (element));
      return element.Namespace != null && element.Namespace.Equals("Edm") ? element.Name ?? string.Empty : element.FullName();
    }

    public static IEnumerable<IEdmEntitySet> EntitySets(this IEdmEntityContainer container)
    {
      EdmUtil.CheckArgumentNull<IEdmEntityContainer>(container, nameof (container));
      return container.AllElements().OfType<IEdmEntitySet>();
    }

    public static IEnumerable<IEdmSingleton> Singletons(this IEdmEntityContainer container)
    {
      EdmUtil.CheckArgumentNull<IEdmEntityContainer>(container, nameof (container));
      return container.AllElements().OfType<IEdmSingleton>();
    }

    public static IEnumerable<IEdmOperationImport> OperationImports(
      this IEdmEntityContainer container)
    {
      EdmUtil.CheckArgumentNull<IEdmEntityContainer>(container, nameof (container));
      return container.AllElements().OfType<IEdmOperationImport>();
    }

    public static EdmTypeKind TypeKind(this IEdmTypeReference type)
    {
      EdmUtil.CheckArgumentNull<IEdmTypeReference>(type, nameof (type));
      IEdmType definition = type.Definition;
      return definition == null ? EdmTypeKind.None : definition.TypeKind;
    }

    public static string FullName(this IEdmTypeReference type)
    {
      EdmUtil.CheckArgumentNull<IEdmTypeReference>(type, nameof (type));
      return type.Definition.FullTypeName();
    }

    public static string ShortQualifiedName(this IEdmTypeReference type)
    {
      EdmUtil.CheckArgumentNull<IEdmTypeReference>(type, nameof (type));
      return !(type.Definition is IEdmSchemaElement definition) ? (string) null : definition.ShortQualifiedName();
    }

    public static string FullTypeName(this IEdmType type)
    {
      EdmUtil.CheckArgumentNull<IEdmType>(type, nameof (type));
      if (type is EdmCoreModelPrimitiveType modelPrimitiveType)
        return modelPrimitiveType.FullName;
      IEdmSchemaElement element = type as IEdmSchemaElement;
      if (!(type is IEdmCollectionType edmCollectionType))
        return element == null ? (string) null : element.FullName();
      if (!(edmCollectionType.ElementType.Definition is IEdmSchemaElement definition))
        return (string) null;
      return string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Collection({0})", new object[1]
      {
        (object) definition.FullName()
      });
    }

    public static IEdmType AsElementType(this IEdmType type) => !(type is IEdmCollectionType edmCollectionType) ? type : edmCollectionType.ElementType.Definition;

    public static IEdmPrimitiveType PrimitiveDefinition(this IEdmPrimitiveTypeReference type)
    {
      EdmUtil.CheckArgumentNull<IEdmPrimitiveTypeReference>(type, nameof (type));
      return (IEdmPrimitiveType) type.Definition;
    }

    public static EdmPrimitiveTypeKind PrimitiveKind(this IEdmPrimitiveTypeReference type)
    {
      EdmUtil.CheckArgumentNull<IEdmPrimitiveTypeReference>(type, nameof (type));
      IEdmPrimitiveType edmPrimitiveType = type.PrimitiveDefinition();
      return edmPrimitiveType == null ? EdmPrimitiveTypeKind.None : edmPrimitiveType.PrimitiveKind;
    }

    public static IEnumerable<IEdmProperty> Properties(this IEdmStructuredType type)
    {
      EdmUtil.CheckArgumentNull<IEdmStructuredType>(type, nameof (type));
      if (type.BaseType != null)
      {
        foreach (IEdmProperty property in type.BaseType.Properties())
          yield return property;
      }
      if (type.DeclaredProperties != null)
      {
        foreach (IEdmProperty declaredProperty in type.DeclaredProperties)
          yield return declaredProperty;
      }
    }

    public static IEnumerable<IEdmStructuralProperty> DeclaredStructuralProperties(
      this IEdmStructuredType type)
    {
      EdmUtil.CheckArgumentNull<IEdmStructuredType>(type, nameof (type));
      return type.DeclaredProperties.OfType<IEdmStructuralProperty>();
    }

    public static IEnumerable<IEdmStructuralProperty> StructuralProperties(
      this IEdmStructuredType type)
    {
      EdmUtil.CheckArgumentNull<IEdmStructuredType>(type, nameof (type));
      return type.Properties().OfType<IEdmStructuralProperty>();
    }

    public static IEdmStructuredType StructuredDefinition(this IEdmStructuredTypeReference type)
    {
      EdmUtil.CheckArgumentNull<IEdmStructuredTypeReference>(type, nameof (type));
      return (IEdmStructuredType) type.Definition;
    }

    public static bool IsAbstract(this IEdmStructuredTypeReference type)
    {
      EdmUtil.CheckArgumentNull<IEdmStructuredTypeReference>(type, nameof (type));
      return type.StructuredDefinition().IsAbstract;
    }

    public static bool IsOpen(this IEdmStructuredTypeReference type)
    {
      EdmUtil.CheckArgumentNull<IEdmStructuredTypeReference>(type, nameof (type));
      return type.StructuredDefinition().IsOpen;
    }

    public static bool IsOpen(this IEdmType type)
    {
      EdmUtil.CheckArgumentNull<IEdmType>(type, nameof (type));
      switch (type)
      {
        case IEdmStructuredType edmStructuredType:
          return edmStructuredType.IsOpen;
        case IEdmCollectionType edmCollectionType:
          return edmCollectionType.ElementType.Definition.IsOpen();
        default:
          return false;
      }
    }

    public static IEdmStructuredType BaseType(this IEdmStructuredTypeReference type)
    {
      EdmUtil.CheckArgumentNull<IEdmStructuredTypeReference>(type, nameof (type));
      return type.StructuredDefinition().BaseType;
    }

    public static IEnumerable<IEdmStructuralProperty> DeclaredStructuralProperties(
      this IEdmStructuredTypeReference type)
    {
      EdmUtil.CheckArgumentNull<IEdmStructuredTypeReference>(type, nameof (type));
      return type.StructuredDefinition().DeclaredStructuralProperties();
    }

    public static IEnumerable<IEdmStructuralProperty> StructuralProperties(
      this IEdmStructuredTypeReference type)
    {
      EdmUtil.CheckArgumentNull<IEdmStructuredTypeReference>(type, nameof (type));
      return type.StructuredDefinition().StructuralProperties();
    }

    public static IEdmProperty FindProperty(this IEdmStructuredTypeReference type, string name)
    {
      EdmUtil.CheckArgumentNull<IEdmStructuredTypeReference>(type, nameof (type));
      EdmUtil.CheckArgumentNull<string>(name, nameof (name));
      return type.StructuredDefinition().FindProperty(name);
    }

    public static IEnumerable<IEdmNavigationProperty> NavigationProperties(
      this IEdmStructuredTypeReference type)
    {
      EdmUtil.CheckArgumentNull<IEdmStructuredTypeReference>(type, nameof (type));
      return type.StructuredDefinition().NavigationProperties();
    }

    public static IEnumerable<IEdmNavigationProperty> DeclaredNavigationProperties(
      this IEdmStructuredTypeReference type)
    {
      EdmUtil.CheckArgumentNull<IEdmStructuredTypeReference>(type, nameof (type));
      return type.StructuredDefinition().DeclaredNavigationProperties();
    }

    public static IEdmNavigationProperty FindNavigationProperty(
      this IEdmStructuredTypeReference type,
      string name)
    {
      EdmUtil.CheckArgumentNull<IEdmStructuredTypeReference>(type, nameof (type));
      EdmUtil.CheckArgumentNull<string>(name, nameof (name));
      return type.StructuredDefinition().FindProperty(name) as IEdmNavigationProperty;
    }

    public static IEdmEntityType BaseEntityType(this IEdmEntityType type)
    {
      EdmUtil.CheckArgumentNull<IEdmEntityType>(type, nameof (type));
      return type.BaseType as IEdmEntityType;
    }

    public static IEdmStructuredType BaseType(this IEdmStructuredType type)
    {
      EdmUtil.CheckArgumentNull<IEdmStructuredType>(type, nameof (type));
      return type.BaseType;
    }

    public static IEnumerable<IEdmNavigationProperty> DeclaredNavigationProperties(
      this IEdmStructuredType type)
    {
      EdmUtil.CheckArgumentNull<IEdmStructuredType>(type, nameof (type));
      return type.DeclaredProperties.OfType<IEdmNavigationProperty>();
    }

    public static IEnumerable<IEdmNavigationProperty> NavigationProperties(
      this IEdmStructuredType type)
    {
      EdmUtil.CheckArgumentNull<IEdmStructuredType>(type, nameof (type));
      return type.Properties().OfType<IEdmNavigationProperty>();
    }

    public static IEnumerable<IEdmStructuralProperty> Key(this IEdmEntityType type)
    {
      EdmUtil.CheckArgumentNull<IEdmEntityType>(type, nameof (type));
      for (IEdmEntityType type1 = type; type1 != null; type1 = type1.BaseEntityType())
      {
        if (type1.DeclaredKey != null)
          return type1.DeclaredKey;
      }
      return Enumerable.Empty<IEdmStructuralProperty>();
    }

    public static bool IsKey(this IEdmProperty property)
    {
      EdmUtil.CheckArgumentNull<IEdmProperty>(property, nameof (property));
      if (property.DeclaringType is IEdmEntityType declaringType)
      {
        foreach (IEdmProperty edmProperty in declaringType.Key())
        {
          if (edmProperty == property)
            return true;
        }
      }
      return false;
    }

    public static IEnumerable<IDictionary<string, IEdmProperty>> GetAlternateKeysAnnotation(
      this IEdmModel model,
      IEdmEntityType type)
    {
      EdmUtil.CheckArgumentNull<IEdmModel>(model, nameof (model));
      EdmUtil.CheckArgumentNull<IEdmEntityType>(type, nameof (type));
      for (IEdmEntityType type1 = type; type1 != null; type1 = type1.BaseEntityType())
      {
        IEnumerable<IDictionary<string, IEdmProperty>> alternateKeysForType = ExtensionMethods.GetDeclaredAlternateKeysForType(type1, model);
        if (alternateKeysForType != null)
          return alternateKeysForType;
      }
      return Enumerable.Empty<IDictionary<string, IEdmProperty>>();
    }

    public static void AddAlternateKeyAnnotation(
      this EdmModel model,
      IEdmEntityType type,
      IDictionary<string, IEdmProperty> alternateKey)
    {
      EdmUtil.CheckArgumentNull<EdmModel>(model, nameof (model));
      EdmUtil.CheckArgumentNull<IEdmEntityType>(type, nameof (type));
      EdmUtil.CheckArgumentNull<IDictionary<string, IEdmProperty>>(alternateKey, nameof (alternateKey));
      EdmCollectionExpression collectionExpression = (EdmCollectionExpression) null;
      IEdmVocabularyAnnotation vocabularyAnnotation = model.FindVocabularyAnnotations<IEdmVocabularyAnnotation>((IEdmVocabularyAnnotatable) type, AlternateKeysVocabularyModel.AlternateKeysTerm).FirstOrDefault<IEdmVocabularyAnnotation>();
      if (vocabularyAnnotation != null)
        collectionExpression = vocabularyAnnotation.Value as EdmCollectionExpression;
      List<IEdmExpression> elements1 = collectionExpression != null ? new List<IEdmExpression>(collectionExpression.Elements) : new List<IEdmExpression>();
      List<IEdmExpression> elements2 = new List<IEdmExpression>();
      foreach (KeyValuePair<string, IEdmProperty> keyValuePair in (IEnumerable<KeyValuePair<string, IEdmProperty>>) alternateKey)
      {
        IEdmRecordExpression recordExpression = (IEdmRecordExpression) new EdmRecordExpression((IEdmStructuredTypeReference) new EdmComplexTypeReference(AlternateKeysVocabularyModel.PropertyRefType, false), new IEdmPropertyConstructor[2]
        {
          (IEdmPropertyConstructor) new EdmPropertyConstructor("Alias", (IEdmExpression) new EdmStringConstant(keyValuePair.Key)),
          (IEdmPropertyConstructor) new EdmPropertyConstructor("Name", (IEdmExpression) new EdmPropertyPathExpression(keyValuePair.Value.Name))
        });
        elements2.Add((IEdmExpression) recordExpression);
      }
      EdmRecordExpression recordExpression1 = new EdmRecordExpression((IEdmStructuredTypeReference) new EdmComplexTypeReference(AlternateKeysVocabularyModel.AlternateKeyType, false), new IEdmPropertyConstructor[1]
      {
        (IEdmPropertyConstructor) new EdmPropertyConstructor("Key", (IEdmExpression) new EdmCollectionExpression((IEnumerable<IEdmExpression>) elements2))
      });
      elements1.Add((IEdmExpression) recordExpression1);
      EdmVocabularyAnnotation annotation = new EdmVocabularyAnnotation((IEdmVocabularyAnnotatable) type, AlternateKeysVocabularyModel.AlternateKeysTerm, (IEdmExpression) new EdmCollectionExpression((IEnumerable<IEdmExpression>) elements1));
      annotation.SetSerializationLocation((IEdmModel) model, new EdmVocabularyAnnotationSerializationLocation?(EdmVocabularyAnnotationSerializationLocation.Inline));
      model.SetVocabularyAnnotation((IEdmVocabularyAnnotation) annotation);
    }

    public static bool HasDeclaredKeyProperty(this IEdmEntityType entityType, IEdmProperty property)
    {
      EdmUtil.CheckArgumentNull<IEdmEntityType>(entityType, nameof (entityType));
      EdmUtil.CheckArgumentNull<IEdmProperty>(property, nameof (property));
      for (; entityType != null; entityType = entityType.BaseEntityType())
      {
        if (entityType.DeclaredKey != null && entityType.DeclaredKey.Any<IEdmStructuralProperty>((Func<IEdmStructuralProperty, bool>) (k => k == property)))
          return true;
      }
      return false;
    }

    public static IEdmEntityType EntityDefinition(this IEdmEntityTypeReference type)
    {
      EdmUtil.CheckArgumentNull<IEdmEntityTypeReference>(type, nameof (type));
      return (IEdmEntityType) type.Definition;
    }

    public static IEdmEntityType BaseEntityType(this IEdmEntityTypeReference type)
    {
      EdmUtil.CheckArgumentNull<IEdmEntityTypeReference>(type, nameof (type));
      return type.EntityDefinition().BaseEntityType();
    }

    public static IEnumerable<IEdmStructuralProperty> Key(this IEdmEntityTypeReference type)
    {
      EdmUtil.CheckArgumentNull<IEdmEntityTypeReference>(type, nameof (type));
      return type.EntityDefinition().Key();
    }

    public static IEdmComplexType BaseComplexType(this IEdmComplexType type)
    {
      EdmUtil.CheckArgumentNull<IEdmComplexType>(type, nameof (type));
      return type.BaseType as IEdmComplexType;
    }

    public static IEdmComplexType ComplexDefinition(this IEdmComplexTypeReference type)
    {
      EdmUtil.CheckArgumentNull<IEdmComplexTypeReference>(type, nameof (type));
      return (IEdmComplexType) type.Definition;
    }

    public static IEdmComplexType BaseComplexType(this IEdmComplexTypeReference type)
    {
      EdmUtil.CheckArgumentNull<IEdmComplexTypeReference>(type, nameof (type));
      return type.ComplexDefinition().BaseComplexType();
    }

    public static IEdmEntityReferenceType EntityReferenceDefinition(
      this IEdmEntityReferenceTypeReference type)
    {
      EdmUtil.CheckArgumentNull<IEdmEntityReferenceTypeReference>(type, nameof (type));
      return (IEdmEntityReferenceType) type.Definition;
    }

    public static IEdmEntityType EntityType(this IEdmEntityReferenceTypeReference type)
    {
      EdmUtil.CheckArgumentNull<IEdmEntityReferenceTypeReference>(type, nameof (type));
      return type.EntityReferenceDefinition().EntityType;
    }

    public static IEdmCollectionType CollectionDefinition(this IEdmCollectionTypeReference type)
    {
      EdmUtil.CheckArgumentNull<IEdmCollectionTypeReference>(type, nameof (type));
      return (IEdmCollectionType) type.Definition;
    }

    public static IEdmTypeReference ElementType(this IEdmCollectionTypeReference type)
    {
      EdmUtil.CheckArgumentNull<IEdmCollectionTypeReference>(type, nameof (type));
      return type.CollectionDefinition().ElementType;
    }

    public static IEdmEnumType EnumDefinition(this IEdmEnumTypeReference type)
    {
      EdmUtil.CheckArgumentNull<IEdmEnumTypeReference>(type, nameof (type));
      return (IEdmEnumType) type.Definition;
    }

    public static IEdmTypeDefinition TypeDefinition(this IEdmTypeDefinitionReference type)
    {
      EdmUtil.CheckArgumentNull<IEdmTypeDefinitionReference>(type, nameof (type));
      return (IEdmTypeDefinition) type.Definition;
    }

    public static EdmMultiplicity TargetMultiplicity(this IEdmNavigationProperty property)
    {
      EdmUtil.CheckArgumentNull<IEdmNavigationProperty>(property, nameof (property));
      IEdmTypeReference type = property.Type;
      if (type.IsCollection())
        return EdmMultiplicity.Many;
      return !type.IsNullable ? EdmMultiplicity.One : EdmMultiplicity.ZeroOrOne;
    }

    public static IEdmEntityType ToEntityType(this IEdmNavigationProperty property) => property.Type.Definition.AsElementType() as IEdmEntityType;

    public static IEdmStructuredType ToStructuredType(this IEdmTypeReference propertyTypeReference)
    {
      IEdmType definition = propertyTypeReference.Definition;
      if (definition.TypeKind == EdmTypeKind.Collection)
        definition = ((IEdmCollectionType) definition).ElementType.Definition;
      return definition as IEdmStructuredType;
    }

    public static IEdmEntityType DeclaringEntityType(this IEdmNavigationProperty property) => (IEdmEntityType) property.DeclaringType;

    public static bool IsPrincipal(this IEdmNavigationProperty navigationProperty) => navigationProperty.ReferentialConstraint == null && navigationProperty.Partner != null && navigationProperty.Partner.ReferentialConstraint != null;

    public static IEnumerable<IEdmStructuralProperty> DependentProperties(
      this IEdmNavigationProperty navigationProperty)
    {
      return navigationProperty.ReferentialConstraint != null ? navigationProperty.ReferentialConstraint.PropertyPairs.Select<EdmReferentialConstraintPropertyPair, IEdmStructuralProperty>((Func<EdmReferentialConstraintPropertyPair, IEdmStructuralProperty>) (p => p.DependentProperty)) : (IEnumerable<IEdmStructuralProperty>) null;
    }

    public static IEnumerable<IEdmStructuralProperty> PrincipalProperties(
      this IEdmNavigationProperty navigationProperty)
    {
      return navigationProperty.ReferentialConstraint != null ? navigationProperty.ReferentialConstraint.PropertyPairs.Select<EdmReferentialConstraintPropertyPair, IEdmStructuralProperty>((Func<EdmReferentialConstraintPropertyPair, IEdmStructuralProperty>) (p => p.PrincipalProperty)) : (IEnumerable<IEdmStructuralProperty>) null;
    }

    public static IEdmTerm Term(this IEdmVocabularyAnnotation annotation)
    {
      EdmUtil.CheckArgumentNull<IEdmVocabularyAnnotation>(annotation, nameof (annotation));
      return annotation.Term;
    }

    public static bool TryGetRelativeEntitySetPath(
      this IEdmOperation operation,
      IEdmModel model,
      out IEdmOperationParameter parameter,
      out Dictionary<IEdmNavigationProperty, IEdmPathExpression> relativeNavigations,
      out IEdmEntityType lastEntityType,
      out IEnumerable<EdmError> errors)
    {
      errors = Enumerable.Empty<EdmError>();
      parameter = (IEdmOperationParameter) null;
      relativeNavigations = (Dictionary<IEdmNavigationProperty, IEdmPathExpression>) null;
      lastEntityType = (IEdmEntityType) null;
      if (operation.EntitySetPath == null)
        return false;
      Collection<EdmError> foundErrors = new Collection<EdmError>();
      errors = (IEnumerable<EdmError>) foundErrors;
      if (!operation.IsBound)
        foundErrors.Add(new EdmError(operation.Location(), EdmErrorCode.OperationCannotHaveEntitySetPathWithUnBoundOperation, Strings.EdmModel_Validator_Semantic_OperationCannotHaveEntitySetPathWithUnBoundOperation((object) operation.Name)));
      return ExtensionMethods.TryGetRelativeEntitySetPath((IEdmElement) operation, foundErrors, operation.EntitySetPath, model, operation.Parameters, out parameter, out relativeNavigations, out lastEntityType);
    }

    public static bool IsActionImport(this IEdmOperationImport operationImport) => operationImport.ContainerElementKind == EdmContainerElementKind.ActionImport;

    public static bool IsFunctionImport(this IEdmOperationImport operationImport) => operationImport.ContainerElementKind == EdmContainerElementKind.FunctionImport;

    public static bool TryGetStaticEntitySet(
      this IEdmOperationImport operationImport,
      IEdmModel model,
      out IEdmEntitySetBase entitySet)
    {
      if (operationImport.EntitySet is IEdmPathExpression entitySet1)
        return entitySet1.TryGetStaticEntitySet(model, out entitySet);
      entitySet = (IEdmEntitySetBase) null;
      return false;
    }

    public static bool TryGetRelativeEntitySetPath(
      this IEdmOperationImport operationImport,
      IEdmModel model,
      out IEdmOperationParameter parameter,
      out Dictionary<IEdmNavigationProperty, IEdmPathExpression> relativeNavigations,
      out IEnumerable<EdmError> edmErrors)
    {
      EdmUtil.CheckArgumentNull<IEdmOperationImport>(operationImport, nameof (operationImport));
      EdmUtil.CheckArgumentNull<IEdmModel>(model, nameof (model));
      parameter = (IEdmOperationParameter) null;
      relativeNavigations = (Dictionary<IEdmNavigationProperty, IEdmPathExpression>) null;
      edmErrors = (IEnumerable<EdmError>) new ReadOnlyCollection<EdmError>((IList<EdmError>) new List<EdmError>());
      if (!(operationImport.EntitySet is IEdmPathExpression entitySet))
        return false;
      IEdmEntityType lastEntityType = (IEdmEntityType) null;
      Collection<EdmError> collection = new Collection<EdmError>();
      bool relativeEntitySetPath = ExtensionMethods.TryGetRelativeEntitySetPath((IEdmElement) operationImport, collection, entitySet, model, operationImport.Operation.Parameters, out parameter, out relativeNavigations, out lastEntityType);
      edmErrors = (IEnumerable<EdmError>) new ReadOnlyCollection<EdmError>((IList<EdmError>) collection);
      return relativeEntitySetPath;
    }

    public static bool IsAction(this IEdmOperation operation) => operation.SchemaElementKind == EdmSchemaElementKind.Action;

    public static bool IsFunction(this IEdmOperation operation) => operation.SchemaElementKind == EdmSchemaElementKind.Function;

    public static IEdmOperationReturn GetReturn(this IEdmOperation operation)
    {
      if (operation is EdmOperation edmOperation)
        return edmOperation.Return;
      if (operation is CsdlSemanticsOperation semanticsOperation)
        return semanticsOperation.Return;
      return operation == null || operation.ReturnType == null ? (IEdmOperationReturn) null : (IEdmOperationReturn) new EdmOperationReturn(operation, operation.ReturnType);
    }

    public static IEnumerable<IEdmOperation> FilterByName(
      this IEnumerable<IEdmOperation> operations,
      bool forceFullyQualifiedNameFilter,
      string operationName)
    {
      EdmUtil.CheckArgumentNull<IEnumerable<IEdmOperation>>(operations, nameof (operations));
      EdmUtil.CheckArgumentNull<string>(operationName, nameof (operationName));
      return forceFullyQualifiedNameFilter || operationName.IndexOf(".", StringComparison.Ordinal) > -1 ? operations.Where<IEdmOperation>((Func<IEdmOperation, bool>) (o => o.FullName() == operationName)) : operations.Where<IEdmOperation>((Func<IEdmOperation, bool>) (o => o.Name == operationName));
    }

    public static bool HasEquivalentBindingType(this IEdmOperation operation, IEdmType bindingType)
    {
      EdmUtil.CheckArgumentNull<IEdmOperation>(operation, nameof (operation));
      EdmUtil.CheckArgumentNull<IEdmType>(bindingType, nameof (bindingType));
      if (!operation.IsBound)
        return false;
      IEdmOperationParameter operationParameter = operation.Parameters.FirstOrDefault<IEdmOperationParameter>();
      if (operationParameter == null)
        return false;
      IEdmType definition = operationParameter.Type.Definition;
      if (definition.TypeKind != bindingType.TypeKind)
        return false;
      if (definition.TypeKind != EdmTypeKind.Collection)
        return bindingType.IsOrInheritsFrom(definition);
      IEdmCollectionType edmCollectionType = (IEdmCollectionType) definition;
      return ((IEdmCollectionType) bindingType).ElementType.Definition.IsOrInheritsFrom(edmCollectionType.ElementType.Definition);
    }

    public static IEdmPropertyConstructor FindProperty(
      this IEdmRecordExpression expression,
      string name)
    {
      foreach (IEdmPropertyConstructor property in expression.Properties)
      {
        if (property.Name == name)
          return property;
      }
      return (IEdmPropertyConstructor) null;
    }

    public static EdmNavigationSourceKind NavigationSourceKind(
      this IEdmNavigationSource navigationSource)
    {
      switch (navigationSource)
      {
        case IEdmEntitySet _:
          return EdmNavigationSourceKind.EntitySet;
        case IEdmSingleton _:
          return EdmNavigationSourceKind.Singleton;
        case IEdmContainedEntitySet _:
          return EdmNavigationSourceKind.ContainedEntitySet;
        case IEdmUnknownEntitySet _:
          return EdmNavigationSourceKind.UnknownEntitySet;
        default:
          return EdmNavigationSourceKind.None;
      }
    }

    public static string FullNavigationSourceName(this IEdmNavigationSource navigationSource)
    {
      EdmUtil.CheckArgumentNull<IEdmNavigationSource>(navigationSource, nameof (navigationSource));
      return string.Join(".", navigationSource.Path.PathSegments.ToArray<string>());
    }

    public static IEdmEntityType EntityType(this IEdmNavigationSource navigationSource)
    {
      switch (navigationSource)
      {
        case IEdmEntitySetBase edmEntitySetBase:
          if (edmEntitySetBase.Type is IEdmCollectionType type)
            return type.ElementType.Definition as IEdmEntityType;
          return edmEntitySetBase is IEdmUnknownEntitySet unknownEntitySet ? unknownEntitySet.Type as IEdmEntityType : (IEdmEntityType) null;
        case IEdmSingleton edmSingleton:
          return edmSingleton.Type as IEdmEntityType;
        default:
          return (IEdmEntityType) null;
      }
    }

    public static void SetEdmReferences(
      this IEdmModel model,
      IEnumerable<IEdmReference> edmReferences)
    {
      EdmUtil.CheckArgumentNull<IEdmModel>(model, nameof (model));
      model.SetAnnotationValue((IEdmElement) model, "http://schemas.microsoft.com/ado/2011/04/edm/internal", "References", (object) edmReferences);
    }

    public static IEnumerable<IEdmReference> GetEdmReferences(this IEdmModel model)
    {
      EdmUtil.CheckArgumentNull<IEdmModel>(model, nameof (model));
      return (IEnumerable<IEdmReference>) model.GetAnnotationValue((IEdmElement) model, "http://schemas.microsoft.com/ado/2011/04/edm/internal", "References");
    }

    public static IEdmPathExpression GetPartnerPath(this IEdmNavigationProperty navigationProperty)
    {
      if (navigationProperty is EdmNavigationProperty navigationProperty1)
        return navigationProperty1.PartnerPath;
      if (navigationProperty is CsdlSemanticsNavigationProperty navigationProperty2)
        return ((CsdlNavigationProperty) navigationProperty2.Element).PartnerPath;
      return navigationProperty.Partner != null ? (IEdmPathExpression) new EdmPathExpression(navigationProperty.Partner.Name) : (IEdmPathExpression) null;
    }

    internal static string ReplaceAlias(this IEdmModel model, string name)
    {
      VersioningDictionary<string, string> mappings = model.GetNamespaceAliases();
      VersioningList<string> namespacesHavingAlias = model.GetUsedNamespacesHavingAlias();
      int num = name.IndexOf('.');
      if (namespacesHavingAlias == null || mappings == null || num <= 0)
        return name;
      string typeAlias = name.Substring(0, num);
      string str1;
      string str2 = namespacesHavingAlias.FirstOrDefault<string>((Func<string, bool>) (n => mappings.TryGetValue(n, out str1) && str1 == typeAlias));
      if (str2 == null)
        return name;
      return string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}{1}", new object[2]
      {
        (object) str2,
        (object) name.Substring(num)
      });
    }

    internal static IEnumerable<IEdmOperation> FindOperationsInModelTree(
      this CsdlSemanticsModel model,
      string name)
    {
      return model.FindInModelTree<IEnumerable<IEdmOperation>>(ExtensionMethods.findOperations, name, ExtensionMethods.mergeFunctions);
    }

    internal static IEdmSchemaType FindTypeInModelTree(this CsdlSemanticsModel model, string name) => model.FindInModelTree<IEdmSchemaType>(ExtensionMethods.findType, name, new Func<IEdmSchemaType, IEdmSchemaType, IEdmSchemaType>(RegistrationHelper.CreateAmbiguousTypeBinding));

    internal static T FindInModelTree<T>(
      this CsdlSemanticsModel model,
      Func<IEdmModel, string, T> finderFunc,
      string qualifiedName,
      Func<T, T, T> ambiguousCreator)
    {
      EdmUtil.CheckArgumentNull<CsdlSemanticsModel>(model, nameof (model));
      EdmUtil.CheckArgumentNull<Func<IEdmModel, string, T>>(finderFunc, nameof (finderFunc));
      EdmUtil.CheckArgumentNull<string>(qualifiedName, nameof (qualifiedName));
      EdmUtil.CheckArgumentNull<Func<T, T, T>>(ambiguousCreator, nameof (ambiguousCreator));
      T inModelTree = finderFunc((IEdmModel) model, qualifiedName);
      if (model.MainModel != null)
      {
        T obj1;
        if ((object) (obj1 = finderFunc((IEdmModel) model.MainModel, qualifiedName)) != null)
          inModelTree = (object) inModelTree == null ? obj1 : ambiguousCreator(inModelTree, obj1);
        foreach (IEdmModel referencedModel in model.MainModel.ReferencedModels)
        {
          T obj2;
          if (referencedModel != EdmCoreModel.Instance && referencedModel != CoreVocabularyModel.Instance && referencedModel != model && (object) (obj2 = finderFunc(referencedModel, qualifiedName)) != null)
            inModelTree = (object) inModelTree == null ? obj2 : ambiguousCreator(inModelTree, obj2);
        }
      }
      foreach (IEdmModel referencedModel in model.ReferencedModels)
      {
        T obj = finderFunc(referencedModel, qualifiedName);
        if ((object) obj != null)
          inModelTree = (object) inModelTree == null ? obj : ambiguousCreator(inModelTree, obj);
      }
      return inModelTree;
    }

    internal static bool IsUrlEscapeFunction(this IEdmModel model, IEdmFunction function)
    {
      EdmUtil.CheckArgumentNull<IEdmModel>(model, nameof (model));
      EdmUtil.CheckArgumentNull<IEdmFunction>(function, nameof (function));
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

    internal static void SetUrlEscapeFunction(this EdmModel model, IEdmFunction function)
    {
      EdmUtil.CheckArgumentNull<EdmModel>(model, nameof (model));
      EdmUtil.CheckArgumentNull<IEdmFunction>(function, nameof (function));
      IEdmBooleanConstantExpression constantExpression = (IEdmBooleanConstantExpression) new EdmBooleanConstant(true);
      IEdmTerm escapeFunctionTerm = CommunityVocabularyModel.UrlEscapeFunctionTerm;
      EdmVocabularyAnnotation annotation = new EdmVocabularyAnnotation((IEdmVocabularyAnnotatable) function, escapeFunctionTerm, (IEdmExpression) constantExpression);
      annotation.SetSerializationLocation((IEdmModel) model, new EdmVocabularyAnnotationSerializationLocation?(EdmVocabularyAnnotationSerializationLocation.Inline));
      model.SetVocabularyAnnotation((IEdmVocabularyAnnotation) annotation);
    }

    internal static bool TryGetRelativeEntitySetPath(
      IEdmElement element,
      Collection<EdmError> foundErrors,
      IEdmPathExpression pathExpression,
      IEdmModel model,
      IEnumerable<IEdmOperationParameter> parameters,
      out IEdmOperationParameter parameter,
      out Dictionary<IEdmNavigationProperty, IEdmPathExpression> relativeNavigations,
      out IEdmEntityType lastEntityType)
    {
      parameter = (IEdmOperationParameter) null;
      relativeNavigations = (Dictionary<IEdmNavigationProperty, IEdmPathExpression>) null;
      lastEntityType = (IEdmEntityType) null;
      List<string> list = pathExpression.PathSegments.ToList<string>();
      if (list.Count < 1)
      {
        foundErrors.Add(new EdmError(element.Location(), EdmErrorCode.OperationWithInvalidEntitySetPathMissingCompletePath, Strings.EdmModel_Validator_Semantic_InvalidEntitySetPathMissingBindingParameterName((object) "EntitySetPath")));
        return false;
      }
      parameter = parameters.FirstOrDefault<IEdmOperationParameter>();
      if (parameter == null)
        return false;
      bool relativeEntitySetPath = true;
      string p2 = list.First<string>();
      if (parameter.Name != p2)
      {
        foundErrors.Add(new EdmError(element.Location(), EdmErrorCode.InvalidPathFirstPathParameterNotMatchingFirstParameterName, Strings.EdmModel_Validator_Semantic_InvalidEntitySetPathWithFirstPathParameterNotMatchingFirstParameterName((object) "EntitySetPath", (object) EdmModelCsdlSchemaWriter.PathAsXml(pathExpression.PathSegments), (object) p2, (object) parameter.Name)));
        relativeEntitySetPath = false;
      }
      lastEntityType = parameter.Type.Definition as IEdmEntityType;
      if (lastEntityType == null)
      {
        if (parameter.Type is IEdmCollectionTypeReference type && type.ElementType().IsEntity())
        {
          lastEntityType = type.ElementType().Definition as IEdmEntityType;
        }
        else
        {
          foundErrors.Add(new EdmError(element.Location(), EdmErrorCode.InvalidPathWithNonEntityBindingParameter, Strings.EdmModel_Validator_Semantic_InvalidEntitySetPathWithNonEntityBindingParameter((object) "EntitySetPath", (object) EdmModelCsdlSchemaWriter.PathAsXml(pathExpression.PathSegments), (object) p2)));
          return false;
        }
      }
      Dictionary<IEdmNavigationProperty, IEdmPathExpression> dictionary = new Dictionary<IEdmNavigationProperty, IEdmPathExpression>();
      List<string> pathSegments = new List<string>();
      foreach (string str in list.Skip<string>(1))
      {
        pathSegments.Add(str);
        if (EdmUtil.IsQualifiedName(str))
        {
          IEdmSchemaType declaredType = model.FindDeclaredType(str);
          if (declaredType == null)
          {
            foundErrors.Add(new EdmError(element.Location(), EdmErrorCode.InvalidPathUnknownTypeCastSegment, Strings.EdmModel_Validator_Semantic_InvalidEntitySetPathUnknownTypeCastSegment((object) "EntitySetPath", (object) EdmModelCsdlSchemaWriter.PathAsXml(pathExpression.PathSegments), (object) str)));
            relativeEntitySetPath = false;
            break;
          }
          if (!(declaredType is IEdmEntityType edmEntityType))
          {
            foundErrors.Add(new EdmError(element.Location(), EdmErrorCode.InvalidPathTypeCastSegmentMustBeEntityType, Strings.EdmModel_Validator_Semantic_InvalidEntitySetPathTypeCastSegmentMustBeEntityType((object) "EntitySetPath", (object) EdmModelCsdlSchemaWriter.PathAsXml(pathExpression.PathSegments), (object) declaredType.FullName())));
            relativeEntitySetPath = false;
            break;
          }
          if (!edmEntityType.IsOrInheritsFrom((IEdmType) lastEntityType))
          {
            foundErrors.Add(new EdmError(element.Location(), EdmErrorCode.InvalidPathInvalidTypeCastSegment, Strings.EdmModel_Validator_Semantic_InvalidEntitySetPathInvalidTypeCastSegment((object) "EntitySetPath", (object) EdmModelCsdlSchemaWriter.PathAsXml(pathExpression.PathSegments), (object) lastEntityType.FullName(), (object) edmEntityType.FullName())));
            relativeEntitySetPath = false;
            break;
          }
          lastEntityType = edmEntityType;
        }
        else
        {
          if (!(lastEntityType.FindProperty(str) is IEdmNavigationProperty property))
          {
            foundErrors.Add(new EdmError(element.Location(), EdmErrorCode.InvalidPathUnknownNavigationProperty, Strings.EdmModel_Validator_Semantic_InvalidEntitySetPathUnknownNavigationProperty((object) "EntitySetPath", (object) EdmModelCsdlSchemaWriter.PathAsXml(pathExpression.PathSegments), (object) str)));
            relativeEntitySetPath = false;
            break;
          }
          dictionary[property] = (IEdmPathExpression) new EdmPathExpression((IEnumerable<string>) pathSegments);
          if (!property.ContainsTarget)
            pathSegments.Clear();
          lastEntityType = property.ToEntityType();
        }
      }
      relativeNavigations = dictionary;
      return relativeEntitySetPath;
    }

    internal static IEdmEntityType GetPathSegmentEntityType(IEdmTypeReference segmentType) => (segmentType.IsCollection() ? segmentType.AsCollection().ElementType() : segmentType).AsEntity().EntityDefinition();

    internal static IEnumerable<IEdmEntityContainerElement> AllElements(
      this IEdmEntityContainer container,
      int depth = 100)
    {
      if (depth <= 0)
        throw new InvalidOperationException(Strings.Bad_CyclicEntityContainer((object) container.FullName()));
      return !(container is CsdlSemanticsEntityContainer semanticsEntityContainer) || semanticsEntityContainer.Extends == null ? container.Elements : container.Elements.Concat<IEdmEntityContainerElement>(semanticsEntityContainer.Extends.AllElements(depth - 1));
    }

    internal static IEdmEntitySet FindEntitySetExtended(
      this IEdmEntityContainer container,
      string qualifiedName)
    {
      return ExtensionMethods.FindInContainerAndExtendsRecursively<IEdmEntitySet>(container, qualifiedName, (Func<IEdmEntityContainer, string, IEdmEntitySet>) ((c, n) => c.FindEntitySet(n)), 100);
    }

    internal static IEdmNavigationSource FindNavigationSourceExtended(
      this IEdmEntityContainer container,
      string path)
    {
      return ExtensionMethods.FindInContainerAndExtendsRecursively<IEdmNavigationSource>(container, path, (Func<IEdmEntityContainer, string, IEdmNavigationSource>) ((c, n) => c.FindNavigationSource(n)), 100);
    }

    internal static IEdmNavigationSource FindNavigationSource(
      this IEdmEntityContainer container,
      string path)
    {
      string[] strArray = ((IEnumerable<string>) path.Split('.')).Last<string>().Split('/');
      IEdmNavigationSource navigationSource = (IEdmNavigationSource) container.FindEntitySet(strArray[0]) ?? (IEdmNavigationSource) container.FindSingleton(strArray[0]);
      List<string> pathSegments = new List<string>();
      for (int index = 1; index < strArray.Length && navigationSource != null; ++index)
      {
        pathSegments.Add(strArray[index]);
        if (navigationSource.EntityType().FindProperty(strArray[index]) is IEdmNavigationProperty property)
        {
          navigationSource = navigationSource.FindNavigationTarget(property, (IEdmPathExpression) new EdmPathExpression((IEnumerable<string>) pathSegments));
          pathSegments.Clear();
        }
      }
      return navigationSource;
    }

    internal static IEdmSingleton FindSingletonExtended(
      this IEdmEntityContainer container,
      string qualifiedName)
    {
      return ExtensionMethods.FindInContainerAndExtendsRecursively<IEdmSingleton>(container, qualifiedName, (Func<IEdmEntityContainer, string, IEdmSingleton>) ((c, n) => c.FindSingleton(n)), 100);
    }

    internal static IEnumerable<IEdmOperationImport> FindOperationImportsExtended(
      this IEdmEntityContainer container,
      string qualifiedName)
    {
      return ExtensionMethods.FindInContainerAndExtendsRecursively<IEnumerable<IEdmOperationImport>>(container, qualifiedName, (Func<IEdmEntityContainer, string, IEnumerable<IEdmOperationImport>>) ((c, n) => c.FindOperationImports(n)), 100);
    }

    internal static IPrimitiveValueConverter GetPrimitiveValueConverter(
      this IEdmModel model,
      IEdmType typeDefinition)
    {
      return model.GetAnnotationValue<IPrimitiveValueConverter>((IEdmElement) typeDefinition, "http://schemas.microsoft.com/ado/2011/04/edm/internal", "PrimitiveValueConverterMap") ?? PassThroughPrimitiveValueConverter.Instance;
    }

    internal static void SetPrimitiveValueConverter(
      this IEdmModel model,
      IEdmType typeDefinition,
      IPrimitiveValueConverter converter)
    {
      model.SetAnnotationValue((IEdmElement) typeDefinition, "http://schemas.microsoft.com/ado/2011/04/edm/internal", "PrimitiveValueConverterMap", (object) converter);
    }

    internal static bool TryGetStaticEntitySet(
      this IEdmPathExpression pathExpression,
      IEdmModel model,
      out IEdmEntitySetBase entitySet)
    {
      IEnumerator<string> enumerator = pathExpression.PathSegments.GetEnumerator();
      if (!enumerator.MoveNext())
      {
        entitySet = (IEdmEntitySetBase) null;
        return false;
      }
      string current = enumerator.Current;
      IEdmEntityContainer entityContainer;
      if (current.Contains("."))
      {
        entityContainer = model.FindEntityContainer(current);
        if (enumerator.MoveNext())
        {
          current = enumerator.Current;
        }
        else
        {
          entitySet = (IEdmEntitySetBase) null;
          return false;
        }
      }
      else
        entityContainer = model.EntityContainer;
      if (entityContainer == null)
      {
        entitySet = (IEdmEntitySetBase) null;
        return false;
      }
      IEdmEntitySet entitySet1 = entityContainer.FindEntitySet(current);
      entitySet = enumerator.MoveNext() ? (IEdmEntitySetBase) null : (IEdmEntitySetBase) entitySet1;
      return entitySet != null;
    }

    internal static bool HasAny<T>(this IEnumerable<T> enumerable) where T : class
    {
      switch (enumerable)
      {
        case IList<T> objList:
          return objList.Count > 0;
        case T[] objArray:
          return objArray.Length != 0;
        default:
          return (object) enumerable.FirstOrDefault<T>() != null;
      }
    }

    private static IEnumerable<IDictionary<string, IEdmProperty>> GetDeclaredAlternateKeysForType(
      IEdmEntityType type,
      IEdmModel model)
    {
      IEdmVocabularyAnnotation vocabularyAnnotation = model.FindVocabularyAnnotations<IEdmVocabularyAnnotation>((IEdmVocabularyAnnotatable) type, AlternateKeysVocabularyModel.AlternateKeysTerm).FirstOrDefault<IEdmVocabularyAnnotation>();
      if (vocabularyAnnotation == null)
        return (IEnumerable<IDictionary<string, IEdmProperty>>) null;
      List<IDictionary<string, IEdmProperty>> alternateKeysForType = new List<IDictionary<string, IEdmProperty>>();
      foreach (IEdmRecordExpression recordExpression1 in (vocabularyAnnotation.Value as IEdmCollectionExpression).Elements.OfType<IEdmRecordExpression>())
      {
        IEdmPropertyConstructor propertyConstructor = recordExpression1.Properties.FirstOrDefault<IEdmPropertyConstructor>((Func<IEdmPropertyConstructor, bool>) (e => e.Name == "Key"));
        if (propertyConstructor != null)
        {
          IEdmCollectionExpression collectionExpression = propertyConstructor.Value as IEdmCollectionExpression;
          IDictionary<string, IEdmProperty> source = (IDictionary<string, IEdmProperty>) new Dictionary<string, IEdmProperty>();
          foreach (IEdmRecordExpression recordExpression2 in collectionExpression.Elements.OfType<IEdmRecordExpression>())
          {
            string key = ((IEdmStringValue) recordExpression2.Properties.FirstOrDefault<IEdmPropertyConstructor>((Func<IEdmPropertyConstructor, bool>) (e => e.Name == "Alias")).Value).Value;
            string name = ((IEdmPathExpression) recordExpression2.Properties.FirstOrDefault<IEdmPropertyConstructor>((Func<IEdmPropertyConstructor, bool>) (e => e.Name == "Name")).Value).PathSegments.FirstOrDefault<string>();
            source[key] = type.FindProperty(name);
          }
          if (source.Any<KeyValuePair<string, IEdmProperty>>())
            alternateKeysForType.Add(source);
        }
      }
      return (IEnumerable<IDictionary<string, IEdmProperty>>) alternateKeysForType;
    }

    private static T FindAcrossModels<T, TInput>(
      this IEdmModel model,
      TInput qualifiedName,
      Func<IEdmModel, TInput, T> finder,
      Func<T, T, T> ambiguousCreator)
    {
      T acrossModels = finder(model, qualifiedName);
      foreach (IEdmModel referencedModel in model.ReferencedModels)
      {
        T obj = finder(referencedModel, qualifiedName);
        if ((object) obj != null)
          acrossModels = (object) acrossModels == null ? obj : ambiguousCreator(acrossModels, obj);
      }
      return acrossModels;
    }

    private static T GetTermValue<T>(
      this IEdmModel model,
      IEdmStructuredValue context,
      IEdmEntityType contextType,
      IEdmTerm term,
      string qualifier,
      Func<IEdmExpression, IEdmStructuredValue, IEdmTypeReference, T> evaluator)
    {
      IEnumerable<IEdmVocabularyAnnotation> vocabularyAnnotations = model.FindVocabularyAnnotations<IEdmVocabularyAnnotation>((IEdmVocabularyAnnotatable) contextType, term, qualifier);
      if (vocabularyAnnotations.Count<IEdmVocabularyAnnotation>() != 1)
        throw new InvalidOperationException(Strings.Edm_Evaluator_NoValueAnnotationOnType((object) ToTraceStringExtensionMethods.ToTraceString(contextType), (object) term.ToTraceString()));
      return evaluator(vocabularyAnnotations.Single<IEdmVocabularyAnnotation>().Value, context, term.Type);
    }

    private static T GetTermValue<T>(
      this IEdmModel model,
      IEdmStructuredValue context,
      IEdmEntityType contextType,
      string termName,
      string qualifier,
      Func<IEdmExpression, IEdmStructuredValue, IEdmTypeReference, T> evaluator)
    {
      IEnumerable<IEdmVocabularyAnnotation> vocabularyAnnotations = model.FindVocabularyAnnotations<IEdmVocabularyAnnotation>((IEdmVocabularyAnnotatable) contextType, termName, qualifier);
      IEdmVocabularyAnnotation annotation = vocabularyAnnotations.Count<IEdmVocabularyAnnotation>() == 1 ? vocabularyAnnotations.Single<IEdmVocabularyAnnotation>() : throw new InvalidOperationException(Strings.Edm_Evaluator_NoValueAnnotationOnType((object) ToTraceStringExtensionMethods.ToTraceString(contextType), (object) termName));
      return evaluator(annotation.Value, context, annotation.Term().Type);
    }

    private static T GetTermValue<T>(
      this IEdmModel model,
      IEdmVocabularyAnnotatable element,
      IEdmTerm term,
      string qualifier,
      Func<IEdmExpression, IEdmStructuredValue, IEdmTypeReference, T> evaluator)
    {
      IEnumerable<IEdmVocabularyAnnotation> vocabularyAnnotations = model.FindVocabularyAnnotations<IEdmVocabularyAnnotation>(element, term, qualifier);
      if (vocabularyAnnotations.Count<IEdmVocabularyAnnotation>() != 1)
        throw new InvalidOperationException(Strings.Edm_Evaluator_NoValueAnnotationOnElement((object) term.ToTraceString()));
      return evaluator(vocabularyAnnotations.Single<IEdmVocabularyAnnotation>().Value, (IEdmStructuredValue) null, term.Type);
    }

    private static T GetTermValue<T>(
      this IEdmModel model,
      IEdmVocabularyAnnotatable element,
      string termName,
      string qualifier,
      Func<IEdmExpression, IEdmStructuredValue, IEdmTypeReference, T> evaluator)
    {
      IEnumerable<IEdmVocabularyAnnotation> vocabularyAnnotations = model.FindVocabularyAnnotations<IEdmVocabularyAnnotation>(element, termName, qualifier);
      IEdmVocabularyAnnotation annotation = vocabularyAnnotations.Count<IEdmVocabularyAnnotation>() == 1 ? vocabularyAnnotations.Single<IEdmVocabularyAnnotation>() : throw new InvalidOperationException(Strings.Edm_Evaluator_NoValueAnnotationOnElement((object) termName));
      return evaluator(annotation.Value, (IEdmStructuredValue) null, annotation.Term().Type);
    }

    private static T FindInContainerAndExtendsRecursively<T>(
      IEdmEntityContainer container,
      string simpleName,
      Func<IEdmEntityContainer, string, T> finderFunc,
      int depth)
    {
      EdmUtil.CheckArgumentNull<IEdmEntityContainer>(container, nameof (container));
      if (depth <= 0)
        throw new InvalidOperationException(Strings.Bad_CyclicEntityContainer((object) container.FullName()));
      T obj = finderFunc(container, simpleName);
      IEnumerable<IEdmOperationImport> enumerable = (object) obj as IEnumerable<IEdmOperationImport>;
      return ((object) obj == null || enumerable != null && !enumerable.HasAny<IEdmOperationImport>()) && container is CsdlSemanticsEntityContainer semanticsEntityContainer && semanticsEntityContainer.Extends != null ? ExtensionMethods.FindInContainerAndExtendsRecursively<T>(semanticsEntityContainer.Extends, simpleName, finderFunc, --depth) : obj;
    }

    private static T AnnotationValue<T>(object annotation) where T : class
    {
      if (annotation == null)
        return default (T);
      if (annotation is T obj)
        return obj;
      IEdmValue edmValue = annotation as IEdmValue;
      throw new InvalidOperationException(Strings.Annotations_TypeMismatch((object) annotation.GetType().Name, (object) typeof (T).Name));
    }

    private static void DerivedFrom(
      this IEdmModel model,
      IEdmStructuredType baseType,
      HashSetInternal<IEdmStructuredType> visited,
      List<IEdmStructuredType> derivedTypes)
    {
      if (!visited.Add(baseType))
        return;
      IEnumerable<IEdmStructuredType> directlyDerivedTypes1 = model.FindDirectlyDerivedTypes(baseType);
      if (directlyDerivedTypes1 != null && directlyDerivedTypes1.HasAny<IEdmStructuredType>())
      {
        foreach (IEdmStructuredType baseType1 in directlyDerivedTypes1)
        {
          derivedTypes.Add(baseType1);
          model.DerivedFrom(baseType1, visited, derivedTypes);
        }
      }
      foreach (IEdmModel referencedModel in model.ReferencedModels)
      {
        IEnumerable<IEdmStructuredType> directlyDerivedTypes2 = referencedModel.FindDirectlyDerivedTypes(baseType);
        if (directlyDerivedTypes2 != null && directlyDerivedTypes2.HasAny<IEdmStructuredType>())
        {
          foreach (IEdmStructuredType baseType2 in directlyDerivedTypes2)
          {
            derivedTypes.Add(baseType2);
            model.DerivedFrom(baseType2, visited, derivedTypes);
          }
        }
      }
    }

    private static void SetChangeTrackingAnnotationImplementation(
      this EdmModel model,
      IEdmVocabularyAnnotatable target,
      bool isSupported,
      IEnumerable<IEdmStructuralProperty> filterableProperties,
      IEnumerable<IEdmNavigationProperty> expandableProperties)
    {
      EdmUtil.CheckArgumentNull<EdmModel>(model, nameof (model));
      EdmUtil.CheckArgumentNull<IEdmVocabularyAnnotatable>(target, nameof (target));
      if (filterableProperties == null)
        filterableProperties = ExtensionMethods.EmptyStructuralProperties;
      if (expandableProperties == null)
        expandableProperties = ExtensionMethods.EmptyNavigationProperties;
      IEdmRecordExpression recordExpression = (IEdmRecordExpression) new EdmRecordExpression((IEnumerable<IEdmPropertyConstructor>) new List<IEdmPropertyConstructor>()
      {
        (IEdmPropertyConstructor) new EdmPropertyConstructor("Supported", (IEdmExpression) new EdmBooleanConstant(isSupported)),
        (IEdmPropertyConstructor) new EdmPropertyConstructor("FilterableProperties", (IEdmExpression) new EdmCollectionExpression((IEdmExpression[]) filterableProperties.Select<IEdmStructuralProperty, EdmPropertyPathExpression>((Func<IEdmStructuralProperty, EdmPropertyPathExpression>) (p => new EdmPropertyPathExpression(p.Name))).ToArray<EdmPropertyPathExpression>())),
        (IEdmPropertyConstructor) new EdmPropertyConstructor("ExpandableProperties", (IEdmExpression) new EdmCollectionExpression((IEdmExpression[]) expandableProperties.Select<IEdmNavigationProperty, EdmNavigationPropertyPathExpression>((Func<IEdmNavigationProperty, EdmNavigationPropertyPathExpression>) (p => new EdmNavigationPropertyPathExpression(p.Name))).ToArray<EdmNavigationPropertyPathExpression>()))
      });
      IEdmTerm changeTrackingTerm = CapabilitiesVocabularyModel.ChangeTrackingTerm;
      EdmVocabularyAnnotation annotation = new EdmVocabularyAnnotation(target, changeTrackingTerm, (IEdmExpression) recordExpression);
      annotation.SetSerializationLocation((IEdmModel) model, new EdmVocabularyAnnotationSerializationLocation?(EdmVocabularyAnnotationSerializationLocation.Inline));
      model.SetVocabularyAnnotation((IEdmVocabularyAnnotation) annotation);
    }

    private static IEdmTypeDefinitionReference GetUIntImplementation(
      this EdmModel model,
      string namespaceName,
      string name,
      string underlyingType,
      bool isNullable)
    {
      EdmUtil.CheckArgumentNull<EdmModel>(model, nameof (model));
      EdmUtil.CheckArgumentNull<string>(namespaceName, nameof (namespaceName));
      string qualifiedName = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}.{1}", new object[2]
      {
        (object) namespaceName,
        (object) name
      });
      if (!(model.FindDeclaredType(qualifiedName) is IEdmTypeDefinition edmTypeDefinition))
      {
        edmTypeDefinition = (IEdmTypeDefinition) new EdmTypeDefinition(namespaceName, name, EdmCoreModel.Instance.GetPrimitiveTypeKind(underlyingType));
        model.AddElement((IEdmSchemaElement) edmTypeDefinition);
        model.SetPrimitiveValueConverter((IEdmType) edmTypeDefinition, DefaultPrimitiveValueConverter.Instance);
      }
      return (IEdmTypeDefinitionReference) new EdmTypeDefinitionReference(edmTypeDefinition, isNullable);
    }

    internal static class TypeName<T>
    {
      public static readonly string LocalName = typeof (T).ToString().Replace("_", "_____").Replace('.', '_').Replace("[", "").Replace("]", "").Replace(",", "__").Replace("`", "___").Replace("+", "____");
    }
  }
}
