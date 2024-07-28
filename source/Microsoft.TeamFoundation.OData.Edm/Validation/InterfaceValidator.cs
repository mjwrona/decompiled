// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.Edm.Validation.InterfaceValidator
// Assembly: Microsoft.TeamFoundation.OData.Edm, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 75C95BF5-2544-413A-959F-F9F18C11D35F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Edm.dll

using Microsoft.OData.Edm.Vocabularies;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.OData.Edm.Validation
{
  internal class InterfaceValidator
  {
    private static readonly Dictionary<Type, InterfaceValidator.VisitorBase> InterfaceVisitors = InterfaceValidator.CreateInterfaceVisitorsMap();
    private static readonly Memoizer<Type, IEnumerable<InterfaceValidator.VisitorBase>> ConcreteTypeInterfaceVisitors = new Memoizer<Type, IEnumerable<InterfaceValidator.VisitorBase>>(new Func<Type, IEnumerable<InterfaceValidator.VisitorBase>>(InterfaceValidator.ComputeInterfaceVisitorsForObject), (IEqualityComparer<Type>) null);
    private readonly HashSetInternal<object> visited = new HashSetInternal<object>();
    private readonly HashSetInternal<object> visitedBad = new HashSetInternal<object>();
    private readonly HashSetInternal<object> danglingReferences = new HashSetInternal<object>();
    private readonly HashSetInternal<object> skipVisitation;
    private readonly bool validateDirectValueAnnotations;
    private readonly IEdmModel model;

    private InterfaceValidator(
      HashSetInternal<object> skipVisitation,
      IEdmModel model,
      bool validateDirectValueAnnotations)
    {
      this.skipVisitation = skipVisitation;
      this.model = model;
      this.validateDirectValueAnnotations = validateDirectValueAnnotations;
    }

    public static IEnumerable<EdmError> ValidateModelStructureAndSemantics(
      IEdmModel model,
      ValidationRuleSet semanticRuleSet)
    {
      InterfaceValidator modelValidator = new InterfaceValidator((HashSetInternal<object>) null, model, true);
      List<EdmError> source1 = new List<EdmError>(modelValidator.ValidateStructure((object) model));
      InterfaceValidator referencesValidator = new InterfaceValidator(modelValidator.visited, model, false);
      for (IEnumerable<object> source2 = (IEnumerable<object>) modelValidator.danglingReferences; source2.FirstOrDefault<object>() != null; source2 = (IEnumerable<object>) referencesValidator.danglingReferences.ToArray<object>())
      {
        foreach (object obj in source2)
          source1.AddRange(referencesValidator.ValidateStructure(obj));
      }
      if (source1.Any<EdmError>(new Func<EdmError, bool>(ValidationHelper.IsInterfaceCritical)))
        return (IEnumerable<EdmError>) source1;
      ValidationContext context = new ValidationContext(model, (Func<object, bool>) (item => modelValidator.visitedBad.Contains(item) || referencesValidator.visitedBad.Contains(item)));
      Dictionary<Type, List<ValidationRule>> concreteTypeSemanticInterfaceVisitors = new Dictionary<Type, List<ValidationRule>>();
      foreach (object obj in modelValidator.visited)
      {
        if (!modelValidator.visitedBad.Contains(obj))
        {
          foreach (ValidationRule validationRule in InterfaceValidator.GetSemanticInterfaceVisitorsForObject(obj.GetType(), semanticRuleSet, concreteTypeSemanticInterfaceVisitors))
            validationRule.Evaluate(context, obj);
        }
      }
      source1.AddRange(context.Errors);
      return (IEnumerable<EdmError>) source1;
    }

    public static IEnumerable<EdmError> GetStructuralErrors(IEdmElement item)
    {
      IEdmModel model = item as IEdmModel;
      return new InterfaceValidator((HashSetInternal<object>) null, model, model != null).ValidateStructure((object) item);
    }

    private static Dictionary<Type, InterfaceValidator.VisitorBase> CreateInterfaceVisitorsMap()
    {
      Dictionary<Type, InterfaceValidator.VisitorBase> interfaceVisitorsMap = new Dictionary<Type, InterfaceValidator.VisitorBase>();
      foreach (Type publicNestedType in typeof (InterfaceValidator).GetNonPublicNestedTypes())
      {
        if (publicNestedType.IsClass())
        {
          Type baseType = publicNestedType.GetBaseType();
          if (baseType.IsGenericType() && baseType.GetBaseType() == typeof (InterfaceValidator.VisitorBase))
            interfaceVisitorsMap.Add(baseType.GetGenericArguments()[0], (InterfaceValidator.VisitorBase) Activator.CreateInstance(publicNestedType));
        }
      }
      return interfaceVisitorsMap;
    }

    private static IEnumerable<InterfaceValidator.VisitorBase> ComputeInterfaceVisitorsForObject(
      Type objectType)
    {
      List<InterfaceValidator.VisitorBase> visitorsForObject = new List<InterfaceValidator.VisitorBase>();
      foreach (Type key in objectType.GetInterfaces())
      {
        InterfaceValidator.VisitorBase visitorBase;
        if (InterfaceValidator.InterfaceVisitors.TryGetValue(key, out visitorBase))
          visitorsForObject.Add(visitorBase);
      }
      return (IEnumerable<InterfaceValidator.VisitorBase>) visitorsForObject;
    }

    private static EdmError CreatePropertyMustNotBeNullError<T>(T item, string propertyName) => new EdmError(InterfaceValidator.GetLocation((object) item), EdmErrorCode.InterfaceCriticalPropertyValueMustNotBeNull, Strings.EdmModel_Validator_Syntactic_PropertyMustNotBeNull((object) typeof (T).Name, (object) propertyName));

    private static EdmError CreateEnumPropertyOutOfRangeError<T, E>(
      T item,
      E enumValue,
      string propertyName)
    {
      return new EdmError(InterfaceValidator.GetLocation((object) item), EdmErrorCode.InterfaceCriticalEnumPropertyValueOutOfRange, Strings.EdmModel_Validator_Syntactic_EnumPropertyValueOutOfRange((object) typeof (T).Name, (object) propertyName, (object) typeof (E).Name, (object) enumValue));
    }

    private static EdmError CheckForInterfaceKindValueMismatchError<T, K, I>(
      T item,
      K kind,
      string propertyName)
    {
      return (object) item is I ? (EdmError) null : new EdmError(InterfaceValidator.GetLocation((object) item), EdmErrorCode.InterfaceCriticalKindValueMismatch, Strings.EdmModel_Validator_Syntactic_InterfaceKindValueMismatch((object) kind, (object) typeof (T).Name, (object) propertyName, (object) typeof (I).Name));
    }

    private static EdmError CreateInterfaceKindValueUnexpectedError<T, K>(
      T item,
      K kind,
      string propertyName)
    {
      return new EdmError(InterfaceValidator.GetLocation((object) item), EdmErrorCode.InterfaceCriticalKindValueUnexpected, Strings.EdmModel_Validator_Syntactic_InterfaceKindValueUnexpected((object) kind, (object) typeof (T).Name, (object) propertyName));
    }

    private static EdmError CreateTypeRefInterfaceTypeKindValueMismatchError<T>(T item) where T : IEdmTypeReference => new EdmError(InterfaceValidator.GetLocation((object) item), EdmErrorCode.InterfaceCriticalKindValueMismatch, Strings.EdmModel_Validator_Syntactic_TypeRefInterfaceTypeKindValueMismatch((object) typeof (T).Name, (object) item.Definition.TypeKind));

    private static EdmError CreatePrimitiveTypeRefInterfaceTypeKindValueMismatchError<T>(T item) where T : IEdmPrimitiveTypeReference => new EdmError(InterfaceValidator.GetLocation((object) item), EdmErrorCode.InterfaceCriticalKindValueMismatch, Strings.EdmModel_Validator_Syntactic_TypeRefInterfaceTypeKindValueMismatch((object) typeof (T).Name, (object) ((IEdmPrimitiveType) item.Definition).PrimitiveKind));

    private static void ProcessEnumerable<T, E>(
      T item,
      IEnumerable<E> enumerable,
      string propertyName,
      IList targetList,
      ref List<EdmError> errors)
    {
      if (enumerable == null)
      {
        InterfaceValidator.CollectErrors(InterfaceValidator.CreatePropertyMustNotBeNullError<T>(item, propertyName), ref errors);
      }
      else
      {
        foreach (E e in enumerable)
        {
          if ((object) e != null)
          {
            targetList.Add((object) e);
          }
          else
          {
            InterfaceValidator.CollectErrors(new EdmError(InterfaceValidator.GetLocation((object) item), EdmErrorCode.InterfaceCriticalEnumerableMustNotHaveNullElements, Strings.EdmModel_Validator_Syntactic_EnumerableMustNotHaveNullElements((object) typeof (T).Name, (object) propertyName)), ref errors);
            break;
          }
        }
      }
    }

    private static void CollectErrors(EdmError newError, ref List<EdmError> errors)
    {
      if (newError == null)
        return;
      if (errors == null)
        errors = new List<EdmError>();
      errors.Add(newError);
    }

    private static bool IsCheckableBad(object element) => element is IEdmCheckable edmCheckable && edmCheckable.Errors != null && edmCheckable.Errors.Count<EdmError>() > 0;

    private static EdmLocation GetLocation(object item) => !(item is IEdmLocatable edmLocatable) || edmLocatable.Location == null ? (EdmLocation) new ObjectLocation(item) : edmLocatable.Location;

    private static IEnumerable<ValidationRule> GetSemanticInterfaceVisitorsForObject(
      Type objectType,
      ValidationRuleSet ruleSet,
      Dictionary<Type, List<ValidationRule>> concreteTypeSemanticInterfaceVisitors)
    {
      List<ValidationRule> visitorsForObject;
      if (!concreteTypeSemanticInterfaceVisitors.TryGetValue(objectType, out visitorsForObject))
      {
        visitorsForObject = new List<ValidationRule>();
        foreach (Type t in objectType.GetInterfaces())
          visitorsForObject.AddRange(ruleSet.GetRules(t));
        concreteTypeSemanticInterfaceVisitors.Add(objectType, visitorsForObject);
      }
      return (IEnumerable<ValidationRule>) visitorsForObject;
    }

    private IEnumerable<EdmError> ValidateStructure(object item)
    {
      if (item is IEdmCoreModelElement || this.visited.Contains(item) || this.skipVisitation != null && this.skipVisitation.Contains(item))
        return Enumerable.Empty<EdmError>();
      this.visited.Add(item);
      if (this.danglingReferences.Contains(item))
        this.danglingReferences.Remove(item);
      List<EdmError> edmErrorList1 = (List<EdmError>) null;
      List<object> followup = new List<object>();
      List<object> references = new List<object>();
      foreach (InterfaceValidator.VisitorBase visitorBase in InterfaceValidator.ConcreteTypeInterfaceVisitors.Evaluate(item.GetType()))
      {
        IEnumerable<EdmError> edmErrors = visitorBase.Visit(item, followup, references);
        if (edmErrors != null)
        {
          foreach (EdmError edmError in edmErrors)
          {
            if (edmErrorList1 == null)
              edmErrorList1 = new List<EdmError>();
            edmErrorList1.Add(edmError);
          }
        }
      }
      if (edmErrorList1 != null)
      {
        this.visitedBad.Add(item);
        return (IEnumerable<EdmError>) edmErrorList1;
      }
      List<EdmError> edmErrorList2 = new List<EdmError>();
      if (this.validateDirectValueAnnotations && item is IEdmElement element)
      {
        foreach (IEdmDirectValueAnnotation directValueAnnotation in this.model.DirectValueAnnotations(element))
          edmErrorList2.AddRange(this.ValidateStructure((object) directValueAnnotation));
      }
      IEdmVocabularyAnnotatable vocabularyAnnotatable = item as IEdmVocabularyAnnotatable;
      if (this.model != null && vocabularyAnnotatable != null)
      {
        foreach (IEdmVocabularyAnnotation vocabularyAnnotation in vocabularyAnnotatable.VocabularyAnnotations(this.model))
        {
          if (vocabularyAnnotation.Target == null)
            edmErrorList2.AddRange(this.ValidateStructure((object) new EdmVocabularyAnnotation(vocabularyAnnotatable, vocabularyAnnotation.Term, vocabularyAnnotation.Qualifier, vocabularyAnnotation.Value)));
        }
      }
      foreach (object obj in followup)
        edmErrorList2.AddRange(this.ValidateStructure(obj));
      foreach (object reference in references)
        this.CollectReference(reference);
      return (IEnumerable<EdmError>) edmErrorList2;
    }

    private void CollectReference(object reference)
    {
      if (reference is IEdmCoreModelElement || this.visited.Contains(reference) || this.skipVisitation != null && this.skipVisitation.Contains(reference))
        return;
      this.danglingReferences.Add(reference);
    }

    private abstract class VisitorBase
    {
      public abstract IEnumerable<EdmError> Visit(
        object item,
        List<object> followup,
        List<object> references);
    }

    private abstract class VisitorOfT<T> : InterfaceValidator.VisitorBase
    {
      public override IEnumerable<EdmError> Visit(
        object item,
        List<object> followup,
        List<object> references)
      {
        return this.VisitT((T) item, followup, references);
      }

      protected abstract IEnumerable<EdmError> VisitT(
        T item,
        List<object> followup,
        List<object> references);
    }

    private sealed class VisitorOfIEdmCheckable : InterfaceValidator.VisitorOfT<IEdmCheckable>
    {
      protected override IEnumerable<EdmError> VisitT(
        IEdmCheckable checkable,
        List<object> followup,
        List<object> references)
      {
        List<EdmError> targetList = new List<EdmError>();
        List<EdmError> errors = (List<EdmError>) null;
        InterfaceValidator.ProcessEnumerable<IEdmCheckable, EdmError>(checkable, checkable.Errors, "Errors", (IList) targetList, ref errors);
        return (IEnumerable<EdmError>) errors ?? (IEnumerable<EdmError>) targetList;
      }
    }

    private sealed class VisitorOfIEdmElement : InterfaceValidator.VisitorOfT<IEdmElement>
    {
      protected override IEnumerable<EdmError> VisitT(
        IEdmElement element,
        List<object> followup,
        List<object> references)
      {
        return (IEnumerable<EdmError>) null;
      }
    }

    private sealed class VisitorOfIEdmFullNamedElement : 
      InterfaceValidator.VisitorOfT<IEdmFullNamedElement>
    {
      protected override IEnumerable<EdmError> VisitT(
        IEdmFullNamedElement element,
        List<object> followup,
        List<object> references)
      {
        if (element.Name != null)
          return (IEnumerable<EdmError>) null;
        return (IEnumerable<EdmError>) new EdmError[1]
        {
          InterfaceValidator.CreatePropertyMustNotBeNullError<IEdmFullNamedElement>(element, "Name")
        };
      }
    }

    private sealed class VisitorOfIEdmNamedElement : InterfaceValidator.VisitorOfT<IEdmNamedElement>
    {
      protected override IEnumerable<EdmError> VisitT(
        IEdmNamedElement element,
        List<object> followup,
        List<object> references)
      {
        if (element.Name != null)
          return (IEnumerable<EdmError>) null;
        return (IEnumerable<EdmError>) new EdmError[1]
        {
          InterfaceValidator.CreatePropertyMustNotBeNullError<IEdmNamedElement>(element, "Name")
        };
      }
    }

    private sealed class VisitorOfIEdmSchemaElement : 
      InterfaceValidator.VisitorOfT<IEdmSchemaElement>
    {
      protected override IEnumerable<EdmError> VisitT(
        IEdmSchemaElement element,
        List<object> followup,
        List<object> references)
      {
        List<EdmError> errors = new List<EdmError>();
        switch (element.SchemaElementKind)
        {
          case EdmSchemaElementKind.None:
            if (element.Namespace == null)
              InterfaceValidator.CollectErrors(InterfaceValidator.CreatePropertyMustNotBeNullError<IEdmSchemaElement>(element, "Namespace"), ref errors);
            return (IEnumerable<EdmError>) errors;
          case EdmSchemaElementKind.TypeDefinition:
            InterfaceValidator.CollectErrors(InterfaceValidator.CheckForInterfaceKindValueMismatchError<IEdmSchemaElement, EdmSchemaElementKind, IEdmSchemaType>(element, element.SchemaElementKind, "SchemaElementKind"), ref errors);
            goto case EdmSchemaElementKind.None;
          case EdmSchemaElementKind.Term:
            InterfaceValidator.CollectErrors(InterfaceValidator.CheckForInterfaceKindValueMismatchError<IEdmSchemaElement, EdmSchemaElementKind, IEdmTerm>(element, element.SchemaElementKind, "SchemaElementKind"), ref errors);
            goto case EdmSchemaElementKind.None;
          case EdmSchemaElementKind.Action:
            InterfaceValidator.CollectErrors(InterfaceValidator.CheckForInterfaceKindValueMismatchError<IEdmSchemaElement, EdmSchemaElementKind, IEdmOperation>(element, element.SchemaElementKind, "SchemaElementKind"), ref errors);
            InterfaceValidator.CollectErrors(InterfaceValidator.CheckForInterfaceKindValueMismatchError<IEdmSchemaElement, EdmSchemaElementKind, IEdmAction>(element, element.SchemaElementKind, "SchemaElementKind"), ref errors);
            goto case EdmSchemaElementKind.None;
          case EdmSchemaElementKind.EntityContainer:
            InterfaceValidator.CollectErrors(InterfaceValidator.CheckForInterfaceKindValueMismatchError<IEdmSchemaElement, EdmSchemaElementKind, IEdmEntityContainer>(element, element.SchemaElementKind, "SchemaElementKind"), ref errors);
            goto case EdmSchemaElementKind.None;
          case EdmSchemaElementKind.Function:
            InterfaceValidator.CollectErrors(InterfaceValidator.CheckForInterfaceKindValueMismatchError<IEdmSchemaElement, EdmSchemaElementKind, IEdmOperation>(element, element.SchemaElementKind, "SchemaElementKind"), ref errors);
            InterfaceValidator.CollectErrors(InterfaceValidator.CheckForInterfaceKindValueMismatchError<IEdmSchemaElement, EdmSchemaElementKind, IEdmFunction>(element, element.SchemaElementKind, "SchemaElementKind"), ref errors);
            goto case EdmSchemaElementKind.None;
          default:
            InterfaceValidator.CollectErrors(InterfaceValidator.CreateEnumPropertyOutOfRangeError<IEdmSchemaElement, EdmSchemaElementKind>(element, element.SchemaElementKind, "SchemaElementKind"), ref errors);
            goto case EdmSchemaElementKind.None;
        }
      }
    }

    private sealed class VisitorOfIEdmModel : InterfaceValidator.VisitorOfT<IEdmModel>
    {
      protected override IEnumerable<EdmError> VisitT(
        IEdmModel model,
        List<object> followup,
        List<object> references)
      {
        List<EdmError> errors = (List<EdmError>) null;
        InterfaceValidator.ProcessEnumerable<IEdmModel, IEdmSchemaElement>(model, model.SchemaElements, "SchemaElements", (IList) followup, ref errors);
        InterfaceValidator.ProcessEnumerable<IEdmModel, IEdmVocabularyAnnotation>(model, model.VocabularyAnnotations, "VocabularyAnnotations", (IList) followup, ref errors);
        return (IEnumerable<EdmError>) errors;
      }
    }

    private sealed class VisitorOfIEdmEntityContainer : 
      InterfaceValidator.VisitorOfT<IEdmEntityContainer>
    {
      protected override IEnumerable<EdmError> VisitT(
        IEdmEntityContainer container,
        List<object> followup,
        List<object> references)
      {
        List<EdmError> errors = (List<EdmError>) null;
        InterfaceValidator.ProcessEnumerable<IEdmEntityContainer, IEdmEntityContainerElement>(container, container.Elements, "Elements", (IList) followup, ref errors);
        return (IEnumerable<EdmError>) errors;
      }
    }

    private sealed class VisitorOfIEdmEntityContainerElement : 
      InterfaceValidator.VisitorOfT<IEdmEntityContainerElement>
    {
      protected override IEnumerable<EdmError> VisitT(
        IEdmEntityContainerElement element,
        List<object> followup,
        List<object> references)
      {
        EdmError edmError = (EdmError) null;
        switch (element.ContainerElementKind)
        {
          case EdmContainerElementKind.None:
            if (edmError == null)
              return (IEnumerable<EdmError>) null;
            return (IEnumerable<EdmError>) new EdmError[1]
            {
              edmError
            };
          case EdmContainerElementKind.EntitySet:
            edmError = InterfaceValidator.CheckForInterfaceKindValueMismatchError<IEdmEntityContainerElement, EdmContainerElementKind, IEdmEntitySet>(element, element.ContainerElementKind, "ContainerElementKind");
            goto case EdmContainerElementKind.None;
          case EdmContainerElementKind.ActionImport:
          case EdmContainerElementKind.FunctionImport:
            edmError = InterfaceValidator.CheckForInterfaceKindValueMismatchError<IEdmEntityContainerElement, EdmContainerElementKind, IEdmOperationImport>(element, element.ContainerElementKind, "ContainerElementKind");
            goto case EdmContainerElementKind.None;
          case EdmContainerElementKind.Singleton:
            edmError = InterfaceValidator.CheckForInterfaceKindValueMismatchError<IEdmEntityContainerElement, EdmContainerElementKind, IEdmSingleton>(element, element.ContainerElementKind, "ContainerElementKind");
            goto case EdmContainerElementKind.None;
          default:
            edmError = InterfaceValidator.CreateEnumPropertyOutOfRangeError<IEdmEntityContainerElement, EdmContainerElementKind>(element, element.ContainerElementKind, "ContainerElementKind");
            goto case EdmContainerElementKind.None;
        }
      }
    }

    private sealed class VisitorOfIEdmContainedEntitySet : 
      InterfaceValidator.VisitorOfT<IEdmContainedEntitySet>
    {
      protected override IEnumerable<EdmError> VisitT(
        IEdmContainedEntitySet item,
        List<object> followup,
        List<object> references)
      {
        List<EdmError> errors = (List<EdmError>) null;
        if (item.ParentNavigationSource == null)
          InterfaceValidator.CollectErrors(InterfaceValidator.CreatePropertyMustNotBeNullError<IEdmContainedEntitySet>(item, "ParentNavigationSource"), ref errors);
        return (IEnumerable<EdmError>) errors;
      }
    }

    private sealed class VisitorOfIEdmNavigationSource : 
      InterfaceValidator.VisitorOfT<IEdmNavigationSource>
    {
      protected override IEnumerable<EdmError> VisitT(
        IEdmNavigationSource set,
        List<object> followup,
        List<object> references)
      {
        List<EdmError> errors = (List<EdmError>) null;
        List<IEdmNavigationPropertyBinding> targetList = new List<IEdmNavigationPropertyBinding>();
        InterfaceValidator.ProcessEnumerable<IEdmNavigationSource, IEdmNavigationPropertyBinding>(set, set.NavigationPropertyBindings, "NavigationPropertyBindings", (IList) targetList, ref errors);
        foreach (IEdmNavigationPropertyBinding navigationPropertyBinding in targetList)
        {
          if (navigationPropertyBinding.NavigationProperty != null)
            references.Add((object) navigationPropertyBinding.NavigationProperty);
          else
            InterfaceValidator.CollectErrors(InterfaceValidator.CreatePropertyMustNotBeNullError<IEdmNavigationPropertyBinding>(navigationPropertyBinding, "NavigationProperty"), ref errors);
          if (navigationPropertyBinding.Target != null)
            references.Add((object) navigationPropertyBinding.Target);
          else
            InterfaceValidator.CollectErrors(InterfaceValidator.CreatePropertyMustNotBeNullError<IEdmNavigationPropertyBinding>(navigationPropertyBinding, "Target"), ref errors);
        }
        return (IEnumerable<EdmError>) errors;
      }
    }

    private sealed class VisitorOfIEdmEntitySetBase : 
      InterfaceValidator.VisitorOfT<IEdmEntitySetBase>
    {
      protected override IEnumerable<EdmError> VisitT(
        IEdmEntitySetBase set,
        List<object> followup,
        List<object> references)
      {
        List<EdmError> errors = (List<EdmError>) null;
        if (set.Type != null)
          references.Add((object) set.Type);
        else
          InterfaceValidator.CollectErrors(InterfaceValidator.CreatePropertyMustNotBeNullError<IEdmEntitySetBase>(set, "Type"), ref errors);
        return (IEnumerable<EdmError>) errors;
      }
    }

    private sealed class VisitorOfIEdmSingleton : InterfaceValidator.VisitorOfT<IEdmSingleton>
    {
      protected override IEnumerable<EdmError> VisitT(
        IEdmSingleton singleton,
        List<object> followup,
        List<object> references)
      {
        List<EdmError> errors = (List<EdmError>) null;
        if (singleton.Type != null)
          references.Add((object) singleton.Type);
        else
          InterfaceValidator.CollectErrors(InterfaceValidator.CreatePropertyMustNotBeNullError<IEdmSingleton>(singleton, "Type"), ref errors);
        return (IEnumerable<EdmError>) errors;
      }
    }

    private sealed class VisitorOfIEdmTypeReference : 
      InterfaceValidator.VisitorOfT<IEdmTypeReference>
    {
      protected override IEnumerable<EdmError> VisitT(
        IEdmTypeReference type,
        List<object> followup,
        List<object> references)
      {
        if (type.Definition != null)
        {
          if (type.Definition is IEdmSchemaType)
            references.Add((object) type.Definition);
          else
            followup.Add((object) type.Definition);
          return (IEnumerable<EdmError>) null;
        }
        return (IEnumerable<EdmError>) new EdmError[1]
        {
          InterfaceValidator.CreatePropertyMustNotBeNullError<IEdmTypeReference>(type, "Definition")
        };
      }
    }

    private sealed class VisitorOfIEdmType : InterfaceValidator.VisitorOfT<IEdmType>
    {
      protected override IEnumerable<EdmError> VisitT(
        IEdmType type,
        List<object> followup,
        List<object> references)
      {
        EdmError edmError = (EdmError) null;
        switch (type.TypeKind)
        {
          case EdmTypeKind.None:
            if (edmError == null)
              return (IEnumerable<EdmError>) null;
            return (IEnumerable<EdmError>) new EdmError[1]
            {
              edmError
            };
          case EdmTypeKind.Primitive:
            edmError = InterfaceValidator.CheckForInterfaceKindValueMismatchError<IEdmType, EdmTypeKind, IEdmPrimitiveType>(type, type.TypeKind, "TypeKind");
            goto case EdmTypeKind.None;
          case EdmTypeKind.Entity:
            edmError = InterfaceValidator.CheckForInterfaceKindValueMismatchError<IEdmType, EdmTypeKind, IEdmEntityType>(type, type.TypeKind, "TypeKind");
            goto case EdmTypeKind.None;
          case EdmTypeKind.Complex:
            edmError = InterfaceValidator.CheckForInterfaceKindValueMismatchError<IEdmType, EdmTypeKind, IEdmComplexType>(type, type.TypeKind, "TypeKind");
            goto case EdmTypeKind.None;
          case EdmTypeKind.Collection:
            edmError = InterfaceValidator.CheckForInterfaceKindValueMismatchError<IEdmType, EdmTypeKind, IEdmCollectionType>(type, type.TypeKind, "TypeKind");
            goto case EdmTypeKind.None;
          case EdmTypeKind.EntityReference:
            edmError = InterfaceValidator.CheckForInterfaceKindValueMismatchError<IEdmType, EdmTypeKind, IEdmEntityReferenceType>(type, type.TypeKind, "TypeKind");
            goto case EdmTypeKind.None;
          case EdmTypeKind.Enum:
            edmError = InterfaceValidator.CheckForInterfaceKindValueMismatchError<IEdmType, EdmTypeKind, IEdmEnumType>(type, type.TypeKind, "TypeKind");
            goto case EdmTypeKind.None;
          case EdmTypeKind.TypeDefinition:
            edmError = InterfaceValidator.CheckForInterfaceKindValueMismatchError<IEdmType, EdmTypeKind, IEdmTypeDefinition>(type, type.TypeKind, "TypeKind");
            goto case EdmTypeKind.None;
          default:
            edmError = InterfaceValidator.CreateInterfaceKindValueUnexpectedError<IEdmType, EdmTypeKind>(type, type.TypeKind, "TypeKind");
            goto case EdmTypeKind.None;
        }
      }
    }

    private sealed class VisitorOfIEdmPrimitiveType : 
      InterfaceValidator.VisitorOfT<IEdmPrimitiveType>
    {
      protected override IEnumerable<EdmError> VisitT(
        IEdmPrimitiveType type,
        List<object> followup,
        List<object> references)
      {
        if (InterfaceValidator.IsCheckableBad((object) type) || type.PrimitiveKind >= EdmPrimitiveTypeKind.None && type.PrimitiveKind <= EdmPrimitiveTypeKind.GeometryMultiPoint)
          return (IEnumerable<EdmError>) null;
        return (IEnumerable<EdmError>) new EdmError[1]
        {
          InterfaceValidator.CreateInterfaceKindValueUnexpectedError<IEdmPrimitiveType, EdmPrimitiveTypeKind>(type, type.PrimitiveKind, "PrimitiveKind")
        };
      }
    }

    private sealed class VisitorOfIEdmStructuredType : 
      InterfaceValidator.VisitorOfT<IEdmStructuredType>
    {
      protected override IEnumerable<EdmError> VisitT(
        IEdmStructuredType type,
        List<object> followup,
        List<object> references)
      {
        List<EdmError> errors = (List<EdmError>) null;
        InterfaceValidator.ProcessEnumerable<IEdmStructuredType, IEdmProperty>(type, type.DeclaredProperties, "DeclaredProperties", (IList) followup, ref errors);
        if (type.BaseType != null)
        {
          HashSetInternal<IEdmStructuredType> hashSetInternal = new HashSetInternal<IEdmStructuredType>();
          hashSetInternal.Add(type);
          IEdmStructuredType baseType;
          for (IEdmStructuredType edmStructuredType = baseType = type.BaseType; edmStructuredType != null; edmStructuredType = edmStructuredType.BaseType)
          {
            if (hashSetInternal.Contains(edmStructuredType))
            {
              string p0 = type is IEdmSchemaType element ? element.FullName() : typeof (Type).Name;
              InterfaceValidator.CollectErrors(new EdmError(InterfaceValidator.GetLocation((object) type), EdmErrorCode.InterfaceCriticalCycleInTypeHierarchy, Strings.EdmModel_Validator_Syntactic_InterfaceCriticalCycleInTypeHierarchy((object) p0)), ref errors);
              break;
            }
          }
          references.Add((object) type.BaseType);
        }
        return (IEnumerable<EdmError>) errors;
      }
    }

    private sealed class VisitorOfIEdmEntityType : InterfaceValidator.VisitorOfT<IEdmEntityType>
    {
      protected override IEnumerable<EdmError> VisitT(
        IEdmEntityType type,
        List<object> followup,
        List<object> references)
      {
        List<EdmError> errors = (List<EdmError>) null;
        if (type.DeclaredKey != null)
          InterfaceValidator.ProcessEnumerable<IEdmEntityType, IEdmStructuralProperty>(type, type.DeclaredKey, "DeclaredKey", (IList) references, ref errors);
        return (IEnumerable<EdmError>) errors;
      }
    }

    private sealed class VisitorOfIEdmEntityReferenceType : 
      InterfaceValidator.VisitorOfT<IEdmEntityReferenceType>
    {
      protected override IEnumerable<EdmError> VisitT(
        IEdmEntityReferenceType type,
        List<object> followup,
        List<object> references)
      {
        if (type.EntityType != null)
        {
          references.Add((object) type.EntityType);
          return (IEnumerable<EdmError>) null;
        }
        return (IEnumerable<EdmError>) new EdmError[1]
        {
          InterfaceValidator.CreatePropertyMustNotBeNullError<IEdmEntityReferenceType>(type, "EntityType")
        };
      }
    }

    private sealed class VisitorOfIEdmUntypedType : InterfaceValidator.VisitorOfT<IEdmUntypedType>
    {
      protected override IEnumerable<EdmError> VisitT(
        IEdmUntypedType type,
        List<object> followup,
        List<object> references)
      {
        return (IEnumerable<EdmError>) null;
      }
    }

    private sealed class VisitorOfIEdmPathType : InterfaceValidator.VisitorOfT<IEdmPathType>
    {
      protected override IEnumerable<EdmError> VisitT(
        IEdmPathType type,
        List<object> followup,
        List<object> references)
      {
        return (IEnumerable<EdmError>) null;
      }
    }

    private sealed class VisitorOfIEdmEnumType : InterfaceValidator.VisitorOfT<IEdmEnumType>
    {
      protected override IEnumerable<EdmError> VisitT(
        IEdmEnumType type,
        List<object> followup,
        List<object> references)
      {
        List<EdmError> errors = (List<EdmError>) null;
        InterfaceValidator.ProcessEnumerable<IEdmEnumType, IEdmEnumMember>(type, type.Members, "Members", (IList) followup, ref errors);
        if (type.UnderlyingType != null)
          references.Add((object) type.UnderlyingType);
        else
          InterfaceValidator.CollectErrors(InterfaceValidator.CreatePropertyMustNotBeNullError<IEdmEnumType>(type, "UnderlyingType"), ref errors);
        return (IEnumerable<EdmError>) errors;
      }
    }

    private sealed class VisitorOfIEdmTypeDefinition : 
      InterfaceValidator.VisitorOfT<IEdmTypeDefinition>
    {
      protected override IEnumerable<EdmError> VisitT(
        IEdmTypeDefinition type,
        List<object> followup,
        List<object> references)
      {
        List<EdmError> errors = (List<EdmError>) null;
        if (type.UnderlyingType != null)
          references.Add((object) type.UnderlyingType);
        else
          InterfaceValidator.CollectErrors(InterfaceValidator.CreatePropertyMustNotBeNullError<IEdmTypeDefinition>(type, "UnderlyingType"), ref errors);
        return (IEnumerable<EdmError>) errors;
      }
    }

    private sealed class VisitorOfIEdmTerm : InterfaceValidator.VisitorOfT<IEdmTerm>
    {
      protected override IEnumerable<EdmError> VisitT(
        IEdmTerm term,
        List<object> followup,
        List<object> references)
      {
        if (term.Type != null)
        {
          followup.Add((object) term.Type);
          return (IEnumerable<EdmError>) null;
        }
        return (IEnumerable<EdmError>) new EdmError[1]
        {
          InterfaceValidator.CreatePropertyMustNotBeNullError<IEdmTerm>(term, "Type")
        };
      }
    }

    private sealed class VisitorOfIEdmCollectionType : 
      InterfaceValidator.VisitorOfT<IEdmCollectionType>
    {
      protected override IEnumerable<EdmError> VisitT(
        IEdmCollectionType type,
        List<object> followup,
        List<object> references)
      {
        if (type.ElementType != null)
        {
          followup.Add((object) type.ElementType);
          return (IEnumerable<EdmError>) null;
        }
        return (IEnumerable<EdmError>) new EdmError[1]
        {
          InterfaceValidator.CreatePropertyMustNotBeNullError<IEdmCollectionType>(type, "ElementType")
        };
      }
    }

    private sealed class VisitorOfIEdmProperty : InterfaceValidator.VisitorOfT<IEdmProperty>
    {
      protected override IEnumerable<EdmError> VisitT(
        IEdmProperty property,
        List<object> followup,
        List<object> references)
      {
        List<EdmError> errors = (List<EdmError>) null;
        switch (property.PropertyKind)
        {
          case EdmPropertyKind.None:
            if (property.Type != null)
              followup.Add((object) property.Type);
            else
              InterfaceValidator.CollectErrors(InterfaceValidator.CreatePropertyMustNotBeNullError<IEdmProperty>(property, "Type"), ref errors);
            if (property.DeclaringType != null)
              references.Add((object) property.DeclaringType);
            else
              InterfaceValidator.CollectErrors(InterfaceValidator.CreatePropertyMustNotBeNullError<IEdmProperty>(property, "DeclaringType"), ref errors);
            return (IEnumerable<EdmError>) errors;
          case EdmPropertyKind.Structural:
            InterfaceValidator.CollectErrors(InterfaceValidator.CheckForInterfaceKindValueMismatchError<IEdmProperty, EdmPropertyKind, IEdmStructuralProperty>(property, property.PropertyKind, "PropertyKind"), ref errors);
            goto case EdmPropertyKind.None;
          case EdmPropertyKind.Navigation:
            InterfaceValidator.CollectErrors(InterfaceValidator.CheckForInterfaceKindValueMismatchError<IEdmProperty, EdmPropertyKind, IEdmNavigationProperty>(property, property.PropertyKind, "PropertyKind"), ref errors);
            goto case EdmPropertyKind.None;
          default:
            InterfaceValidator.CollectErrors(InterfaceValidator.CreateInterfaceKindValueUnexpectedError<IEdmProperty, EdmPropertyKind>(property, property.PropertyKind, "PropertyKind"), ref errors);
            goto case EdmPropertyKind.None;
        }
      }
    }

    private sealed class VisitorOfIEdmStructuralProperty : 
      InterfaceValidator.VisitorOfT<IEdmStructuralProperty>
    {
      protected override IEnumerable<EdmError> VisitT(
        IEdmStructuralProperty property,
        List<object> followup,
        List<object> references)
      {
        return (IEnumerable<EdmError>) null;
      }
    }

    private sealed class VisitorOfIEdmNavigationProperty : 
      InterfaceValidator.VisitorOfT<IEdmNavigationProperty>
    {
      protected override IEnumerable<EdmError> VisitT(
        IEdmNavigationProperty property,
        List<object> followup,
        List<object> references)
      {
        List<EdmError> errors = (List<EdmError>) null;
        followup.Add((object) property.Type);
        if (property.Partner != null)
        {
          followup.Add((object) property.Partner);
          if (!(property.Partner is BadNavigationProperty) && (property.Partner.Partner != null && property.Partner.Partner != property || property.Partner == property && ValidationHelper.ComputeNavigationPropertyTarget(property) != property.DeclaringType))
            InterfaceValidator.CollectErrors(new EdmError(InterfaceValidator.GetLocation((object) property), EdmErrorCode.InterfaceCriticalNavigationPartnerInvalid, Strings.EdmModel_Validator_Syntactic_NavigationPartnerInvalid((object) property.Name)), ref errors);
        }
        if (property.ReferentialConstraint != null)
          followup.Add((object) property.ReferentialConstraint);
        if (property.OnDelete < EdmOnDeleteAction.None || property.OnDelete > EdmOnDeleteAction.Cascade)
          InterfaceValidator.CollectErrors(InterfaceValidator.CreateEnumPropertyOutOfRangeError<IEdmNavigationProperty, EdmOnDeleteAction>(property, property.OnDelete, "OnDelete"), ref errors);
        return (IEnumerable<EdmError>) errors;
      }
    }

    private sealed class VisitorOfIEdmReferentialConstraint : 
      InterfaceValidator.VisitorOfT<IEdmReferentialConstraint>
    {
      protected override IEnumerable<EdmError> VisitT(
        IEdmReferentialConstraint member,
        List<object> followup,
        List<object> references)
      {
        List<EdmError> errors = (List<EdmError>) null;
        if (member.PropertyPairs == null)
        {
          InterfaceValidator.CollectErrors(InterfaceValidator.CreatePropertyMustNotBeNullError<IEdmReferentialConstraint>(member, "PropertyPairs"), ref errors);
        }
        else
        {
          foreach (EdmReferentialConstraintPropertyPair propertyPair in member.PropertyPairs)
          {
            if (propertyPair == null)
            {
              InterfaceValidator.CollectErrors(new EdmError(InterfaceValidator.GetLocation((object) member), EdmErrorCode.InterfaceCriticalEnumerableMustNotHaveNullElements, Strings.EdmModel_Validator_Syntactic_EnumerableMustNotHaveNullElements((object) typeof (IEdmReferentialConstraint).Name, (object) "PropertyPairs")), ref errors);
              break;
            }
            followup.Add((object) propertyPair.PrincipalProperty);
            followup.Add((object) propertyPair.DependentProperty);
          }
        }
        return (IEnumerable<EdmError>) errors;
      }
    }

    private sealed class VisitorOfIEdmEnumMember : InterfaceValidator.VisitorOfT<IEdmEnumMember>
    {
      protected override IEnumerable<EdmError> VisitT(
        IEdmEnumMember member,
        List<object> followup,
        List<object> references)
      {
        List<EdmError> errors = (List<EdmError>) null;
        if (member.DeclaringType != null)
          references.Add((object) member.DeclaringType);
        else
          InterfaceValidator.CollectErrors(InterfaceValidator.CreatePropertyMustNotBeNullError<IEdmEnumMember>(member, "DeclaringType"), ref errors);
        if (member.Value != null)
          followup.Add((object) member.Value);
        else
          InterfaceValidator.CollectErrors(InterfaceValidator.CreatePropertyMustNotBeNullError<IEdmEnumMember>(member, "Value"), ref errors);
        return (IEnumerable<EdmError>) errors;
      }
    }

    private sealed class VisitorOfIEdmOperation : InterfaceValidator.VisitorOfT<IEdmOperation>
    {
      protected override IEnumerable<EdmError> VisitT(
        IEdmOperation operation,
        List<object> followup,
        List<object> references)
      {
        List<EdmError> errors = (List<EdmError>) null;
        InterfaceValidator.ProcessEnumerable<IEdmOperation, IEdmOperationParameter>(operation, operation.Parameters, "Parameters", (IList) followup, ref errors);
        if (operation.ReturnType != null)
          followup.Add((object) operation.ReturnType);
        return (IEnumerable<EdmError>) errors;
      }
    }

    private sealed class VisitorOfIEdmAction : InterfaceValidator.VisitorOfT<IEdmAction>
    {
      protected override IEnumerable<EdmError> VisitT(
        IEdmAction operation,
        List<object> followup,
        List<object> references)
      {
        return (IEnumerable<EdmError>) null;
      }
    }

    private sealed class VisitorOfIEdmFunction : InterfaceValidator.VisitorOfT<IEdmFunction>
    {
      protected override IEnumerable<EdmError> VisitT(
        IEdmFunction operation,
        List<object> followup,
        List<object> references)
      {
        return (IEnumerable<EdmError>) null;
      }
    }

    private sealed class VisitorOfIEdmOperationImport : 
      InterfaceValidator.VisitorOfT<IEdmOperationImport>
    {
      protected override IEnumerable<EdmError> VisitT(
        IEdmOperationImport functionImport,
        List<object> followup,
        List<object> references)
      {
        if (functionImport.EntitySet != null)
          followup.Add((object) functionImport.EntitySet);
        followup.Add((object) functionImport.Operation);
        return (IEnumerable<EdmError>) null;
      }
    }

    private sealed class VisitorOfIEdmActionImport : InterfaceValidator.VisitorOfT<IEdmActionImport>
    {
      protected override IEnumerable<EdmError> VisitT(
        IEdmActionImport actionImport,
        List<object> followup,
        List<object> references)
      {
        return (IEnumerable<EdmError>) null;
      }
    }

    private sealed class VisitorOfIEdmFunctionImport : 
      InterfaceValidator.VisitorOfT<IEdmFunctionImport>
    {
      protected override IEnumerable<EdmError> VisitT(
        IEdmFunctionImport functionImport,
        List<object> followup,
        List<object> references)
      {
        return (IEnumerable<EdmError>) null;
      }
    }

    private sealed class VisitorOfIEdmOperationParameter : 
      InterfaceValidator.VisitorOfT<IEdmOperationParameter>
    {
      protected override IEnumerable<EdmError> VisitT(
        IEdmOperationParameter parameter,
        List<object> followup,
        List<object> references)
      {
        List<EdmError> errors = (List<EdmError>) null;
        if (parameter.Type != null)
          followup.Add((object) parameter.Type);
        else
          InterfaceValidator.CollectErrors(InterfaceValidator.CreatePropertyMustNotBeNullError<IEdmOperationParameter>(parameter, "Type"), ref errors);
        if (parameter.DeclaringOperation != null)
          references.Add((object) parameter.DeclaringOperation);
        else
          InterfaceValidator.CollectErrors(InterfaceValidator.CreatePropertyMustNotBeNullError<IEdmOperationParameter>(parameter, "DeclaringFunction"), ref errors);
        return (IEnumerable<EdmError>) errors;
      }
    }

    private sealed class VisitorOfIEdmOptionalParameter : 
      InterfaceValidator.VisitorOfT<IEdmOptionalParameter>
    {
      protected override IEnumerable<EdmError> VisitT(
        IEdmOptionalParameter parameter,
        List<object> followup,
        List<object> references)
      {
        return (IEnumerable<EdmError>) null;
      }
    }

    private sealed class VisitorOfIEdmOperationReturn : 
      InterfaceValidator.VisitorOfT<IEdmOperationReturn>
    {
      protected override IEnumerable<EdmError> VisitT(
        IEdmOperationReturn operationReturn,
        List<object> followup,
        List<object> references)
      {
        List<EdmError> errors = (List<EdmError>) null;
        if (operationReturn.Type != null)
          followup.Add((object) operationReturn.Type);
        else
          InterfaceValidator.CollectErrors(InterfaceValidator.CreatePropertyMustNotBeNullError<IEdmOperationReturn>(operationReturn, "Type"), ref errors);
        if (operationReturn.DeclaringOperation != null)
          references.Add((object) operationReturn.DeclaringOperation);
        else
          InterfaceValidator.CollectErrors(InterfaceValidator.CreatePropertyMustNotBeNullError<IEdmOperationReturn>(operationReturn, "DeclaringOperation"), ref errors);
        return (IEnumerable<EdmError>) errors;
      }
    }

    private sealed class VisitorOfIEdmCollectionTypeReference : 
      InterfaceValidator.VisitorOfT<IEdmCollectionTypeReference>
    {
      protected override IEnumerable<EdmError> VisitT(
        IEdmCollectionTypeReference typeRef,
        List<object> followup,
        List<object> references)
      {
        if (typeRef.Definition == null || typeRef.Definition.TypeKind == EdmTypeKind.Collection)
          return (IEnumerable<EdmError>) null;
        return (IEnumerable<EdmError>) new EdmError[1]
        {
          InterfaceValidator.CreateTypeRefInterfaceTypeKindValueMismatchError<IEdmCollectionTypeReference>(typeRef)
        };
      }
    }

    private sealed class VisitorOfIEdmEntityReferenceTypeReference : 
      InterfaceValidator.VisitorOfT<IEdmEntityReferenceTypeReference>
    {
      protected override IEnumerable<EdmError> VisitT(
        IEdmEntityReferenceTypeReference typeRef,
        List<object> followup,
        List<object> references)
      {
        if (typeRef.Definition == null || typeRef.Definition.TypeKind == EdmTypeKind.EntityReference)
          return (IEnumerable<EdmError>) null;
        return (IEnumerable<EdmError>) new EdmError[1]
        {
          InterfaceValidator.CreateTypeRefInterfaceTypeKindValueMismatchError<IEdmEntityReferenceTypeReference>(typeRef)
        };
      }
    }

    private sealed class VisitorOfIEdmStructuredTypeReference : 
      InterfaceValidator.VisitorOfT<IEdmStructuredTypeReference>
    {
      protected override IEnumerable<EdmError> VisitT(
        IEdmStructuredTypeReference typeRef,
        List<object> followup,
        List<object> references)
      {
        if (typeRef.Definition == null || typeRef.Definition.TypeKind.IsStructured())
          return (IEnumerable<EdmError>) null;
        return (IEnumerable<EdmError>) new EdmError[1]
        {
          InterfaceValidator.CreateTypeRefInterfaceTypeKindValueMismatchError<IEdmStructuredTypeReference>(typeRef)
        };
      }
    }

    private sealed class VisitorOfIEdmEntityTypeReference : 
      InterfaceValidator.VisitorOfT<IEdmEntityTypeReference>
    {
      protected override IEnumerable<EdmError> VisitT(
        IEdmEntityTypeReference typeRef,
        List<object> followup,
        List<object> references)
      {
        if (typeRef.Definition == null || typeRef.Definition.TypeKind == EdmTypeKind.Entity)
          return (IEnumerable<EdmError>) null;
        return (IEnumerable<EdmError>) new EdmError[1]
        {
          InterfaceValidator.CreateTypeRefInterfaceTypeKindValueMismatchError<IEdmEntityTypeReference>(typeRef)
        };
      }
    }

    private sealed class VisitorOfIEdmComplexTypeReference : 
      InterfaceValidator.VisitorOfT<IEdmComplexTypeReference>
    {
      protected override IEnumerable<EdmError> VisitT(
        IEdmComplexTypeReference typeRef,
        List<object> followup,
        List<object> references)
      {
        if (typeRef.Definition == null || typeRef.Definition.TypeKind == EdmTypeKind.Complex)
          return (IEnumerable<EdmError>) null;
        return (IEnumerable<EdmError>) new EdmError[1]
        {
          InterfaceValidator.CreateTypeRefInterfaceTypeKindValueMismatchError<IEdmComplexTypeReference>(typeRef)
        };
      }
    }

    private sealed class VisitorOfIEdmUntypedTypeReference : 
      InterfaceValidator.VisitorOfT<IEdmUntypedTypeReference>
    {
      protected override IEnumerable<EdmError> VisitT(
        IEdmUntypedTypeReference typeRef,
        List<object> followup,
        List<object> references)
      {
        if (typeRef.Definition == null || typeRef.Definition.TypeKind == EdmTypeKind.Untyped)
          return (IEnumerable<EdmError>) null;
        return (IEnumerable<EdmError>) new EdmError[1]
        {
          InterfaceValidator.CreateTypeRefInterfaceTypeKindValueMismatchError<IEdmUntypedTypeReference>(typeRef)
        };
      }
    }

    private sealed class VisitorOfIEdmPathTypeReference : 
      InterfaceValidator.VisitorOfT<IEdmPathTypeReference>
    {
      protected override IEnumerable<EdmError> VisitT(
        IEdmPathTypeReference typeRef,
        List<object> followup,
        List<object> references)
      {
        if (typeRef.Definition == null || typeRef.Definition.TypeKind == EdmTypeKind.Path)
          return (IEnumerable<EdmError>) null;
        return (IEnumerable<EdmError>) new EdmError[1]
        {
          InterfaceValidator.CreateTypeRefInterfaceTypeKindValueMismatchError<IEdmPathTypeReference>(typeRef)
        };
      }
    }

    private sealed class VisitorOfIEdmEnumTypeReference : 
      InterfaceValidator.VisitorOfT<IEdmEnumTypeReference>
    {
      protected override IEnumerable<EdmError> VisitT(
        IEdmEnumTypeReference typeRef,
        List<object> followup,
        List<object> references)
      {
        if (typeRef.Definition == null || typeRef.Definition.TypeKind == EdmTypeKind.Enum)
          return (IEnumerable<EdmError>) null;
        return (IEnumerable<EdmError>) new EdmError[1]
        {
          InterfaceValidator.CreateTypeRefInterfaceTypeKindValueMismatchError<IEdmEnumTypeReference>(typeRef)
        };
      }
    }

    private sealed class VisitorOfIEdmTypeDefinitionReference : 
      InterfaceValidator.VisitorOfT<IEdmTypeDefinitionReference>
    {
      protected override IEnumerable<EdmError> VisitT(
        IEdmTypeDefinitionReference typeRef,
        List<object> followup,
        List<object> references)
      {
        if (typeRef.Definition == null || typeRef.Definition.TypeKind == EdmTypeKind.TypeDefinition)
          return (IEnumerable<EdmError>) null;
        return (IEnumerable<EdmError>) new EdmError[1]
        {
          InterfaceValidator.CreateTypeRefInterfaceTypeKindValueMismatchError<IEdmTypeDefinitionReference>(typeRef)
        };
      }
    }

    private sealed class VisitorOfIEdmPrimitiveTypeReference : 
      InterfaceValidator.VisitorOfT<IEdmPrimitiveTypeReference>
    {
      protected override IEnumerable<EdmError> VisitT(
        IEdmPrimitiveTypeReference typeRef,
        List<object> followup,
        List<object> references)
      {
        if (typeRef.Definition == null || typeRef.Definition.TypeKind == EdmTypeKind.Primitive)
          return (IEnumerable<EdmError>) null;
        return (IEnumerable<EdmError>) new EdmError[1]
        {
          InterfaceValidator.CreateTypeRefInterfaceTypeKindValueMismatchError<IEdmPrimitiveTypeReference>(typeRef)
        };
      }
    }

    private sealed class VisitorOfIEdmBinaryTypeReference : 
      InterfaceValidator.VisitorOfT<IEdmBinaryTypeReference>
    {
      protected override IEnumerable<EdmError> VisitT(
        IEdmBinaryTypeReference typeRef,
        List<object> followup,
        List<object> references)
      {
        if (!(typeRef.Definition is IEdmPrimitiveType definition) || definition.PrimitiveKind == EdmPrimitiveTypeKind.Binary)
          return (IEnumerable<EdmError>) null;
        return (IEnumerable<EdmError>) new EdmError[1]
        {
          InterfaceValidator.CreatePrimitiveTypeRefInterfaceTypeKindValueMismatchError<IEdmBinaryTypeReference>(typeRef)
        };
      }
    }

    private sealed class VisitorOfIEdmDecimalTypeReference : 
      InterfaceValidator.VisitorOfT<IEdmDecimalTypeReference>
    {
      protected override IEnumerable<EdmError> VisitT(
        IEdmDecimalTypeReference typeRef,
        List<object> followup,
        List<object> references)
      {
        if (!(typeRef.Definition is IEdmPrimitiveType definition) || definition.PrimitiveKind == EdmPrimitiveTypeKind.Decimal)
          return (IEnumerable<EdmError>) null;
        return (IEnumerable<EdmError>) new EdmError[1]
        {
          InterfaceValidator.CreatePrimitiveTypeRefInterfaceTypeKindValueMismatchError<IEdmDecimalTypeReference>(typeRef)
        };
      }
    }

    private sealed class VisitorOfIEdmStringTypeReference : 
      InterfaceValidator.VisitorOfT<IEdmStringTypeReference>
    {
      protected override IEnumerable<EdmError> VisitT(
        IEdmStringTypeReference typeRef,
        List<object> followup,
        List<object> references)
      {
        if (!(typeRef.Definition is IEdmPrimitiveType definition) || definition.PrimitiveKind == EdmPrimitiveTypeKind.String)
          return (IEnumerable<EdmError>) null;
        return (IEnumerable<EdmError>) new EdmError[1]
        {
          InterfaceValidator.CreatePrimitiveTypeRefInterfaceTypeKindValueMismatchError<IEdmStringTypeReference>(typeRef)
        };
      }
    }

    private sealed class VisitorOfIEdmTemporalTypeReference : 
      InterfaceValidator.VisitorOfT<IEdmTemporalTypeReference>
    {
      protected override IEnumerable<EdmError> VisitT(
        IEdmTemporalTypeReference typeRef,
        List<object> followup,
        List<object> references)
      {
        if (!(typeRef.Definition is IEdmPrimitiveType definition) || definition.PrimitiveKind.IsTemporal())
          return (IEnumerable<EdmError>) null;
        return (IEnumerable<EdmError>) new EdmError[1]
        {
          InterfaceValidator.CreatePrimitiveTypeRefInterfaceTypeKindValueMismatchError<IEdmTemporalTypeReference>(typeRef)
        };
      }
    }

    private sealed class VisitorOfIEdmSpatialTypeReference : 
      InterfaceValidator.VisitorOfT<IEdmSpatialTypeReference>
    {
      protected override IEnumerable<EdmError> VisitT(
        IEdmSpatialTypeReference typeRef,
        List<object> followup,
        List<object> references)
      {
        if (!(typeRef.Definition is IEdmPrimitiveType definition) || definition.PrimitiveKind.IsSpatial())
          return (IEnumerable<EdmError>) null;
        return (IEnumerable<EdmError>) new EdmError[1]
        {
          InterfaceValidator.CreatePrimitiveTypeRefInterfaceTypeKindValueMismatchError<IEdmSpatialTypeReference>(typeRef)
        };
      }
    }

    private sealed class VisitorOfIEdmReference : InterfaceValidator.VisitorOfT<IEdmReference>
    {
      protected override IEnumerable<EdmError> VisitT(
        IEdmReference edmReference,
        List<object> followup,
        List<object> references)
      {
        if (edmReference.Includes.Any<IEdmInclude>() || !edmReference.IncludeAnnotations.Any<IEdmIncludeAnnotations>())
          return (IEnumerable<EdmError>) null;
        return (IEnumerable<EdmError>) new EdmError[1]
        {
          InterfaceValidator.CreatePropertyMustNotBeNullError<IEdmReference>(edmReference, "Includes/IncludeAnnotations")
        };
      }
    }

    private sealed class VisitorOfIEdmInclude : InterfaceValidator.VisitorOfT<IEdmInclude>
    {
      protected override IEnumerable<EdmError> VisitT(
        IEdmInclude edmInclude,
        List<object> followup,
        List<object> references)
      {
        if (!string.IsNullOrEmpty(edmInclude.Namespace))
          return (IEnumerable<EdmError>) null;
        return (IEnumerable<EdmError>) new EdmError[1]
        {
          InterfaceValidator.CreatePropertyMustNotBeNullError<IEdmInclude>(edmInclude, "Namespace")
        };
      }
    }

    private sealed class VisitorOfIEdmIncludeAnnotations : 
      InterfaceValidator.VisitorOfT<IEdmIncludeAnnotations>
    {
      protected override IEnumerable<EdmError> VisitT(
        IEdmIncludeAnnotations edmIncludeAnnotations,
        List<object> followup,
        List<object> references)
      {
        if (!string.IsNullOrEmpty(edmIncludeAnnotations.TermNamespace))
          return (IEnumerable<EdmError>) null;
        return (IEnumerable<EdmError>) new EdmError[1]
        {
          InterfaceValidator.CreatePropertyMustNotBeNullError<IEdmIncludeAnnotations>(edmIncludeAnnotations, "TermNamespace")
        };
      }
    }

    private sealed class VisitorOfIEdmExpression : InterfaceValidator.VisitorOfT<IEdmExpression>
    {
      protected override IEnumerable<EdmError> VisitT(
        IEdmExpression expression,
        List<object> followup,
        List<object> references)
      {
        EdmError edmError = (EdmError) null;
        if (!InterfaceValidator.IsCheckableBad((object) expression))
        {
          switch (expression.ExpressionKind)
          {
            case EdmExpressionKind.BinaryConstant:
              edmError = InterfaceValidator.CheckForInterfaceKindValueMismatchError<IEdmExpression, EdmExpressionKind, IEdmBinaryConstantExpression>(expression, expression.ExpressionKind, "ExpressionKind");
              break;
            case EdmExpressionKind.BooleanConstant:
              edmError = InterfaceValidator.CheckForInterfaceKindValueMismatchError<IEdmExpression, EdmExpressionKind, IEdmBooleanConstantExpression>(expression, expression.ExpressionKind, "ExpressionKind");
              break;
            case EdmExpressionKind.DateTimeOffsetConstant:
              edmError = InterfaceValidator.CheckForInterfaceKindValueMismatchError<IEdmExpression, EdmExpressionKind, IEdmDateTimeOffsetConstantExpression>(expression, expression.ExpressionKind, "ExpressionKind");
              break;
            case EdmExpressionKind.DecimalConstant:
              edmError = InterfaceValidator.CheckForInterfaceKindValueMismatchError<IEdmExpression, EdmExpressionKind, IEdmDecimalConstantExpression>(expression, expression.ExpressionKind, "ExpressionKind");
              break;
            case EdmExpressionKind.FloatingConstant:
              edmError = InterfaceValidator.CheckForInterfaceKindValueMismatchError<IEdmExpression, EdmExpressionKind, IEdmFloatingConstantExpression>(expression, expression.ExpressionKind, "ExpressionKind");
              break;
            case EdmExpressionKind.GuidConstant:
              edmError = InterfaceValidator.CheckForInterfaceKindValueMismatchError<IEdmExpression, EdmExpressionKind, IEdmGuidConstantExpression>(expression, expression.ExpressionKind, "ExpressionKind");
              break;
            case EdmExpressionKind.IntegerConstant:
              edmError = InterfaceValidator.CheckForInterfaceKindValueMismatchError<IEdmExpression, EdmExpressionKind, IEdmIntegerConstantExpression>(expression, expression.ExpressionKind, "ExpressionKind");
              break;
            case EdmExpressionKind.StringConstant:
              edmError = InterfaceValidator.CheckForInterfaceKindValueMismatchError<IEdmExpression, EdmExpressionKind, IEdmStringConstantExpression>(expression, expression.ExpressionKind, "ExpressionKind");
              break;
            case EdmExpressionKind.DurationConstant:
              edmError = InterfaceValidator.CheckForInterfaceKindValueMismatchError<IEdmExpression, EdmExpressionKind, IEdmDurationConstantExpression>(expression, expression.ExpressionKind, "ExpressionKind");
              break;
            case EdmExpressionKind.Null:
              edmError = InterfaceValidator.CheckForInterfaceKindValueMismatchError<IEdmExpression, EdmExpressionKind, IEdmNullExpression>(expression, expression.ExpressionKind, "ExpressionKind");
              break;
            case EdmExpressionKind.Record:
              edmError = InterfaceValidator.CheckForInterfaceKindValueMismatchError<IEdmExpression, EdmExpressionKind, IEdmRecordExpression>(expression, expression.ExpressionKind, "ExpressionKind");
              break;
            case EdmExpressionKind.Collection:
              edmError = InterfaceValidator.CheckForInterfaceKindValueMismatchError<IEdmExpression, EdmExpressionKind, IEdmCollectionExpression>(expression, expression.ExpressionKind, "ExpressionKind");
              break;
            case EdmExpressionKind.Path:
            case EdmExpressionKind.PropertyPath:
            case EdmExpressionKind.NavigationPropertyPath:
              edmError = InterfaceValidator.CheckForInterfaceKindValueMismatchError<IEdmExpression, EdmExpressionKind, IEdmPathExpression>(expression, expression.ExpressionKind, "ExpressionKind");
              break;
            case EdmExpressionKind.If:
              edmError = InterfaceValidator.CheckForInterfaceKindValueMismatchError<IEdmExpression, EdmExpressionKind, IEdmIfExpression>(expression, expression.ExpressionKind, "ExpressionKind");
              break;
            case EdmExpressionKind.Cast:
              edmError = InterfaceValidator.CheckForInterfaceKindValueMismatchError<IEdmExpression, EdmExpressionKind, IEdmCastExpression>(expression, expression.ExpressionKind, "ExpressionKind");
              break;
            case EdmExpressionKind.IsType:
              edmError = InterfaceValidator.CheckForInterfaceKindValueMismatchError<IEdmExpression, EdmExpressionKind, IEdmIsTypeExpression>(expression, expression.ExpressionKind, "ExpressionKind");
              break;
            case EdmExpressionKind.FunctionApplication:
              edmError = InterfaceValidator.CheckForInterfaceKindValueMismatchError<IEdmExpression, EdmExpressionKind, IEdmApplyExpression>(expression, expression.ExpressionKind, "ExpressionKind");
              break;
            case EdmExpressionKind.LabeledExpressionReference:
              edmError = InterfaceValidator.CheckForInterfaceKindValueMismatchError<IEdmExpression, EdmExpressionKind, IEdmLabeledExpressionReferenceExpression>(expression, expression.ExpressionKind, "ExpressionKind");
              break;
            case EdmExpressionKind.Labeled:
              edmError = InterfaceValidator.CheckForInterfaceKindValueMismatchError<IEdmExpression, EdmExpressionKind, IEdmLabeledExpression>(expression, expression.ExpressionKind, "ExpressionKind");
              break;
            case EdmExpressionKind.DateConstant:
              edmError = InterfaceValidator.CheckForInterfaceKindValueMismatchError<IEdmExpression, EdmExpressionKind, IEdmDateConstantExpression>(expression, expression.ExpressionKind, "ExpressionKind");
              break;
            case EdmExpressionKind.TimeOfDayConstant:
              edmError = InterfaceValidator.CheckForInterfaceKindValueMismatchError<IEdmExpression, EdmExpressionKind, IEdmTimeOfDayConstantExpression>(expression, expression.ExpressionKind, "ExpressionKind");
              break;
            case EdmExpressionKind.EnumMember:
              edmError = InterfaceValidator.CheckForInterfaceKindValueMismatchError<IEdmExpression, EdmExpressionKind, IEdmEnumMemberExpression>(expression, expression.ExpressionKind, "ExpressionKind");
              break;
            default:
              edmError = InterfaceValidator.CreateInterfaceKindValueUnexpectedError<IEdmExpression, EdmExpressionKind>(expression, expression.ExpressionKind, "ExpressionKind");
              break;
          }
        }
        if (edmError == null)
          return (IEnumerable<EdmError>) null;
        return (IEnumerable<EdmError>) new EdmError[1]
        {
          edmError
        };
      }
    }

    private sealed class VisitorOfIEdmRecordExpression : 
      InterfaceValidator.VisitorOfT<IEdmRecordExpression>
    {
      protected override IEnumerable<EdmError> VisitT(
        IEdmRecordExpression expression,
        List<object> followup,
        List<object> references)
      {
        List<EdmError> errors = (List<EdmError>) null;
        InterfaceValidator.ProcessEnumerable<IEdmRecordExpression, IEdmPropertyConstructor>(expression, expression.Properties, "Properties", (IList) followup, ref errors);
        if (expression.DeclaredType != null)
          followup.Add((object) expression.DeclaredType);
        return (IEnumerable<EdmError>) errors;
      }
    }

    private sealed class VisitorOfIEdmPropertyConstructor : 
      InterfaceValidator.VisitorOfT<IEdmPropertyConstructor>
    {
      protected override IEnumerable<EdmError> VisitT(
        IEdmPropertyConstructor expression,
        List<object> followup,
        List<object> references)
      {
        List<EdmError> errors = (List<EdmError>) null;
        if (expression.Name == null)
          InterfaceValidator.CollectErrors(InterfaceValidator.CreatePropertyMustNotBeNullError<IEdmPropertyConstructor>(expression, "Name"), ref errors);
        if (expression.Value != null)
          followup.Add((object) expression.Value);
        else
          InterfaceValidator.CollectErrors(InterfaceValidator.CreatePropertyMustNotBeNullError<IEdmPropertyConstructor>(expression, "Value"), ref errors);
        return (IEnumerable<EdmError>) errors;
      }
    }

    private sealed class VisitorOfIEdmCollectionExpression : 
      InterfaceValidator.VisitorOfT<IEdmCollectionExpression>
    {
      protected override IEnumerable<EdmError> VisitT(
        IEdmCollectionExpression expression,
        List<object> followup,
        List<object> references)
      {
        List<EdmError> errors = (List<EdmError>) null;
        InterfaceValidator.ProcessEnumerable<IEdmCollectionExpression, IEdmExpression>(expression, expression.Elements, "Elements", (IList) followup, ref errors);
        if (expression.DeclaredType != null)
          followup.Add((object) expression.DeclaredType);
        return (IEnumerable<EdmError>) errors;
      }
    }

    private sealed class VisitorOfIEdmLabeledElement : 
      InterfaceValidator.VisitorOfT<IEdmLabeledExpression>
    {
      protected override IEnumerable<EdmError> VisitT(
        IEdmLabeledExpression expression,
        List<object> followup,
        List<object> references)
      {
        if (expression.Expression != null)
        {
          followup.Add((object) expression.Expression);
          return (IEnumerable<EdmError>) null;
        }
        return (IEnumerable<EdmError>) new EdmError[1]
        {
          InterfaceValidator.CreatePropertyMustNotBeNullError<IEdmLabeledExpression>(expression, "Expression")
        };
      }
    }

    private sealed class VisitorOfIEdmPathExpression : 
      InterfaceValidator.VisitorOfT<IEdmPathExpression>
    {
      protected override IEnumerable<EdmError> VisitT(
        IEdmPathExpression expression,
        List<object> followup,
        List<object> references)
      {
        List<EdmError> errors = (List<EdmError>) null;
        List<string> targetList = new List<string>();
        InterfaceValidator.ProcessEnumerable<IEdmPathExpression, string>(expression, expression.PathSegments, "Path", (IList) targetList, ref errors);
        return (IEnumerable<EdmError>) errors;
      }
    }

    private sealed class VistorOfIEdmEnumMemberExpression : 
      InterfaceValidator.VisitorOfT<IEdmEnumMemberExpression>
    {
      protected override IEnumerable<EdmError> VisitT(
        IEdmEnumMemberExpression expression,
        List<object> followup,
        List<object> references)
      {
        List<EdmError> errors = (List<EdmError>) null;
        InterfaceValidator.ProcessEnumerable<IEdmEnumMemberExpression, IEdmEnumMember>(expression, expression.EnumMembers, "EnumMembers", (IList) followup, ref errors);
        return (IEnumerable<EdmError>) errors;
      }
    }

    private sealed class VistorOfIEdmIfExpression : InterfaceValidator.VisitorOfT<IEdmIfExpression>
    {
      protected override IEnumerable<EdmError> VisitT(
        IEdmIfExpression expression,
        List<object> followup,
        List<object> references)
      {
        List<EdmError> errors = (List<EdmError>) null;
        if (expression.TestExpression != null)
          followup.Add((object) expression.TestExpression);
        else
          InterfaceValidator.CollectErrors(InterfaceValidator.CreatePropertyMustNotBeNullError<IEdmIfExpression>(expression, "TestExpression"), ref errors);
        if (expression.TrueExpression != null)
          followup.Add((object) expression.TrueExpression);
        else
          InterfaceValidator.CollectErrors(InterfaceValidator.CreatePropertyMustNotBeNullError<IEdmIfExpression>(expression, "TrueExpression"), ref errors);
        if (expression.FalseExpression != null)
          followup.Add((object) expression.FalseExpression);
        else
          InterfaceValidator.CollectErrors(InterfaceValidator.CreatePropertyMustNotBeNullError<IEdmIfExpression>(expression, "FalseExpression"), ref errors);
        return (IEnumerable<EdmError>) errors;
      }
    }

    private sealed class VistorOfIEdmCastExpression : 
      InterfaceValidator.VisitorOfT<IEdmCastExpression>
    {
      protected override IEnumerable<EdmError> VisitT(
        IEdmCastExpression expression,
        List<object> followup,
        List<object> references)
      {
        List<EdmError> errors = (List<EdmError>) null;
        if (expression.Operand != null)
          followup.Add((object) expression.Operand);
        else
          InterfaceValidator.CollectErrors(InterfaceValidator.CreatePropertyMustNotBeNullError<IEdmCastExpression>(expression, "Operand"), ref errors);
        if (expression.Type != null)
          followup.Add((object) expression.Type);
        else
          InterfaceValidator.CollectErrors(InterfaceValidator.CreatePropertyMustNotBeNullError<IEdmCastExpression>(expression, "Type"), ref errors);
        return (IEnumerable<EdmError>) errors;
      }
    }

    private sealed class VistorOfIEdmIsTypeExpression : 
      InterfaceValidator.VisitorOfT<IEdmIsTypeExpression>
    {
      protected override IEnumerable<EdmError> VisitT(
        IEdmIsTypeExpression expression,
        List<object> followup,
        List<object> references)
      {
        List<EdmError> errors = (List<EdmError>) null;
        if (expression.Operand != null)
          followup.Add((object) expression.Operand);
        else
          InterfaceValidator.CollectErrors(InterfaceValidator.CreatePropertyMustNotBeNullError<IEdmIsTypeExpression>(expression, "Operand"), ref errors);
        if (expression.Type != null)
          followup.Add((object) expression.Type);
        else
          InterfaceValidator.CollectErrors(InterfaceValidator.CreatePropertyMustNotBeNullError<IEdmIsTypeExpression>(expression, "Type"), ref errors);
        return (IEnumerable<EdmError>) errors;
      }
    }

    private sealed class VistorOfIEdmFunctionApplicationExpression : 
      InterfaceValidator.VisitorOfT<IEdmApplyExpression>
    {
      protected override IEnumerable<EdmError> VisitT(
        IEdmApplyExpression expression,
        List<object> followup,
        List<object> references)
      {
        List<EdmError> errors = (List<EdmError>) null;
        if (expression.AppliedFunction != null)
          followup.Add((object) expression.AppliedFunction);
        else
          InterfaceValidator.CollectErrors(InterfaceValidator.CreatePropertyMustNotBeNullError<IEdmApplyExpression>(expression, "AppliedFunction"), ref errors);
        InterfaceValidator.ProcessEnumerable<IEdmApplyExpression, IEdmExpression>(expression, expression.Arguments, "Arguments", (IList) followup, ref errors);
        return (IEnumerable<EdmError>) errors;
      }
    }

    private sealed class VistorOfIEdmLabeledElementReferenceExpression : 
      InterfaceValidator.VisitorOfT<IEdmLabeledExpressionReferenceExpression>
    {
      protected override IEnumerable<EdmError> VisitT(
        IEdmLabeledExpressionReferenceExpression expression,
        List<object> followup,
        List<object> references)
      {
        if (expression.ReferencedLabeledExpression != null)
        {
          references.Add((object) expression.ReferencedLabeledExpression);
          return (IEnumerable<EdmError>) null;
        }
        return (IEnumerable<EdmError>) new EdmError[1]
        {
          InterfaceValidator.CreatePropertyMustNotBeNullError<IEdmLabeledExpressionReferenceExpression>(expression, "ReferencedLabeledExpression")
        };
      }
    }

    private sealed class VisitorOfIEdmValue : InterfaceValidator.VisitorOfT<IEdmValue>
    {
      protected override IEnumerable<EdmError> VisitT(
        IEdmValue value,
        List<object> followup,
        List<object> references)
      {
        List<EdmError> errors = (List<EdmError>) null;
        if (value.Type != null)
          followup.Add((object) value.Type);
        switch (value.ValueKind)
        {
          case EdmValueKind.None:
            return (IEnumerable<EdmError>) errors;
          case EdmValueKind.Binary:
            InterfaceValidator.CollectErrors(InterfaceValidator.CheckForInterfaceKindValueMismatchError<IEdmValue, EdmValueKind, IEdmBinaryValue>(value, value.ValueKind, "ValueKind"), ref errors);
            goto case EdmValueKind.None;
          case EdmValueKind.Boolean:
            InterfaceValidator.CollectErrors(InterfaceValidator.CheckForInterfaceKindValueMismatchError<IEdmValue, EdmValueKind, IEdmBooleanValue>(value, value.ValueKind, "ValueKind"), ref errors);
            goto case EdmValueKind.None;
          case EdmValueKind.Collection:
            InterfaceValidator.CollectErrors(InterfaceValidator.CheckForInterfaceKindValueMismatchError<IEdmValue, EdmValueKind, IEdmCollectionValue>(value, value.ValueKind, "ValueKind"), ref errors);
            goto case EdmValueKind.None;
          case EdmValueKind.DateTimeOffset:
            InterfaceValidator.CollectErrors(InterfaceValidator.CheckForInterfaceKindValueMismatchError<IEdmValue, EdmValueKind, IEdmDateTimeOffsetValue>(value, value.ValueKind, "ValueKind"), ref errors);
            goto case EdmValueKind.None;
          case EdmValueKind.Decimal:
            InterfaceValidator.CollectErrors(InterfaceValidator.CheckForInterfaceKindValueMismatchError<IEdmValue, EdmValueKind, IEdmDecimalValue>(value, value.ValueKind, "ValueKind"), ref errors);
            goto case EdmValueKind.None;
          case EdmValueKind.Enum:
            InterfaceValidator.CollectErrors(InterfaceValidator.CheckForInterfaceKindValueMismatchError<IEdmValue, EdmValueKind, IEdmEnumValue>(value, value.ValueKind, "ValueKind"), ref errors);
            goto case EdmValueKind.None;
          case EdmValueKind.Floating:
            InterfaceValidator.CollectErrors(InterfaceValidator.CheckForInterfaceKindValueMismatchError<IEdmValue, EdmValueKind, IEdmFloatingValue>(value, value.ValueKind, "ValueKind"), ref errors);
            goto case EdmValueKind.None;
          case EdmValueKind.Guid:
            InterfaceValidator.CollectErrors(InterfaceValidator.CheckForInterfaceKindValueMismatchError<IEdmValue, EdmValueKind, IEdmGuidValue>(value, value.ValueKind, "ValueKind"), ref errors);
            goto case EdmValueKind.None;
          case EdmValueKind.Integer:
            InterfaceValidator.CollectErrors(InterfaceValidator.CheckForInterfaceKindValueMismatchError<IEdmValue, EdmValueKind, IEdmIntegerValue>(value, value.ValueKind, "ValueKind"), ref errors);
            goto case EdmValueKind.None;
          case EdmValueKind.Null:
            InterfaceValidator.CollectErrors(InterfaceValidator.CheckForInterfaceKindValueMismatchError<IEdmValue, EdmValueKind, IEdmNullValue>(value, value.ValueKind, "ValueKind"), ref errors);
            goto case EdmValueKind.None;
          case EdmValueKind.String:
            InterfaceValidator.CollectErrors(InterfaceValidator.CheckForInterfaceKindValueMismatchError<IEdmValue, EdmValueKind, IEdmStringValue>(value, value.ValueKind, "ValueKind"), ref errors);
            goto case EdmValueKind.None;
          case EdmValueKind.Structured:
            InterfaceValidator.CollectErrors(InterfaceValidator.CheckForInterfaceKindValueMismatchError<IEdmValue, EdmValueKind, IEdmStructuredValue>(value, value.ValueKind, "ValueKind"), ref errors);
            goto case EdmValueKind.None;
          case EdmValueKind.Duration:
            InterfaceValidator.CollectErrors(InterfaceValidator.CheckForInterfaceKindValueMismatchError<IEdmValue, EdmValueKind, IEdmDurationValue>(value, value.ValueKind, "ValueKind"), ref errors);
            goto case EdmValueKind.None;
          case EdmValueKind.Date:
            InterfaceValidator.CollectErrors(InterfaceValidator.CheckForInterfaceKindValueMismatchError<IEdmValue, EdmValueKind, IEdmDateValue>(value, value.ValueKind, "ValueKind"), ref errors);
            goto case EdmValueKind.None;
          case EdmValueKind.TimeOfDay:
            InterfaceValidator.CollectErrors(InterfaceValidator.CheckForInterfaceKindValueMismatchError<IEdmValue, EdmValueKind, IEdmTimeOfDayValue>(value, value.ValueKind, "ValueKind"), ref errors);
            goto case EdmValueKind.None;
          default:
            InterfaceValidator.CollectErrors(InterfaceValidator.CreateInterfaceKindValueUnexpectedError<IEdmValue, EdmValueKind>(value, value.ValueKind, "ValueKind"), ref errors);
            goto case EdmValueKind.None;
        }
      }
    }

    private sealed class VisitorOfIEdmDelayedValue : InterfaceValidator.VisitorOfT<IEdmDelayedValue>
    {
      protected override IEnumerable<EdmError> VisitT(
        IEdmDelayedValue value,
        List<object> followup,
        List<object> references)
      {
        if (value.Value != null)
        {
          followup.Add((object) value.Value);
          return (IEnumerable<EdmError>) null;
        }
        return (IEnumerable<EdmError>) new EdmError[1]
        {
          InterfaceValidator.CreatePropertyMustNotBeNullError<IEdmDelayedValue>(value, "Value")
        };
      }
    }

    private sealed class VisitorOfIEdmPropertyValue : 
      InterfaceValidator.VisitorOfT<IEdmPropertyValue>
    {
      protected override IEnumerable<EdmError> VisitT(
        IEdmPropertyValue value,
        List<object> followup,
        List<object> references)
      {
        if (value.Name != null)
          return (IEnumerable<EdmError>) null;
        return (IEnumerable<EdmError>) new EdmError[1]
        {
          InterfaceValidator.CreatePropertyMustNotBeNullError<IEdmPropertyValue>(value, "Name")
        };
      }
    }

    private sealed class VisitorOfIEdmEnumValue : InterfaceValidator.VisitorOfT<IEdmEnumValue>
    {
      protected override IEnumerable<EdmError> VisitT(
        IEdmEnumValue value,
        List<object> followup,
        List<object> references)
      {
        if (value.Value != null)
        {
          followup.Add((object) value.Value);
          return (IEnumerable<EdmError>) null;
        }
        return (IEnumerable<EdmError>) new EdmError[1]
        {
          InterfaceValidator.CreatePropertyMustNotBeNullError<IEdmEnumValue>(value, "Value")
        };
      }
    }

    private sealed class VisitorOfIEdmCollectionValue : 
      InterfaceValidator.VisitorOfT<IEdmCollectionValue>
    {
      protected override IEnumerable<EdmError> VisitT(
        IEdmCollectionValue value,
        List<object> followup,
        List<object> references)
      {
        List<EdmError> errors = (List<EdmError>) null;
        InterfaceValidator.ProcessEnumerable<IEdmCollectionValue, IEdmDelayedValue>(value, value.Elements, "Elements", (IList) followup, ref errors);
        return (IEnumerable<EdmError>) errors;
      }
    }

    private sealed class VisitorOfIEdmStructuredValue : 
      InterfaceValidator.VisitorOfT<IEdmStructuredValue>
    {
      protected override IEnumerable<EdmError> VisitT(
        IEdmStructuredValue value,
        List<object> followup,
        List<object> references)
      {
        List<EdmError> errors = (List<EdmError>) null;
        InterfaceValidator.ProcessEnumerable<IEdmStructuredValue, IEdmPropertyValue>(value, value.PropertyValues, "PropertyValues", (IList) followup, ref errors);
        return (IEnumerable<EdmError>) errors;
      }
    }

    private sealed class VisitorOfIEdmBinaryValue : InterfaceValidator.VisitorOfT<IEdmBinaryValue>
    {
      protected override IEnumerable<EdmError> VisitT(
        IEdmBinaryValue value,
        List<object> followup,
        List<object> references)
      {
        if (value.Value != null)
          return (IEnumerable<EdmError>) null;
        return (IEnumerable<EdmError>) new EdmError[1]
        {
          InterfaceValidator.CreatePropertyMustNotBeNullError<IEdmBinaryValue>(value, "Value")
        };
      }
    }

    private sealed class VisitorOfIEdmStringValue : InterfaceValidator.VisitorOfT<IEdmStringValue>
    {
      protected override IEnumerable<EdmError> VisitT(
        IEdmStringValue value,
        List<object> followup,
        List<object> references)
      {
        if (value.Value != null)
          return (IEnumerable<EdmError>) null;
        return (IEnumerable<EdmError>) new EdmError[1]
        {
          InterfaceValidator.CreatePropertyMustNotBeNullError<IEdmStringValue>(value, "Value")
        };
      }
    }

    private sealed class VisitorOfIEdmVocabularyAnnotation : 
      InterfaceValidator.VisitorOfT<IEdmVocabularyAnnotation>
    {
      protected override IEnumerable<EdmError> VisitT(
        IEdmVocabularyAnnotation annotation,
        List<object> followup,
        List<object> references)
      {
        List<EdmError> errors = (List<EdmError>) null;
        if (annotation.Term != null)
          references.Add((object) annotation.Term);
        else
          InterfaceValidator.CollectErrors(InterfaceValidator.CreatePropertyMustNotBeNullError<IEdmVocabularyAnnotation>(annotation, "Term"), ref errors);
        if (annotation.Target != null)
          references.Add((object) annotation.Target);
        else
          InterfaceValidator.CollectErrors(InterfaceValidator.CreatePropertyMustNotBeNullError<IEdmVocabularyAnnotation>(annotation, "Target"), ref errors);
        if (annotation.Value != null)
          followup.Add((object) annotation.Value);
        else
          InterfaceValidator.CollectErrors(InterfaceValidator.CreatePropertyMustNotBeNullError<IEdmVocabularyAnnotation>(annotation, "Value"), ref errors);
        return (IEnumerable<EdmError>) errors;
      }
    }

    private sealed class VisitorOfIEdmPropertyValueBinding : 
      InterfaceValidator.VisitorOfT<IEdmPropertyValueBinding>
    {
      protected override IEnumerable<EdmError> VisitT(
        IEdmPropertyValueBinding binding,
        List<object> followup,
        List<object> references)
      {
        List<EdmError> errors = (List<EdmError>) null;
        if (binding.Value != null)
          followup.Add((object) binding.Value);
        else
          InterfaceValidator.CollectErrors(InterfaceValidator.CreatePropertyMustNotBeNullError<IEdmPropertyValueBinding>(binding, "Value"), ref errors);
        if (binding.BoundProperty != null)
          references.Add((object) binding.BoundProperty);
        else
          InterfaceValidator.CollectErrors(InterfaceValidator.CreatePropertyMustNotBeNullError<IEdmPropertyValueBinding>(binding, "BoundProperty"), ref errors);
        return (IEnumerable<EdmError>) errors;
      }
    }

    private sealed class VisitorOfIEdmDirectValueAnnotation : 
      InterfaceValidator.VisitorOfT<IEdmDirectValueAnnotation>
    {
      protected override IEnumerable<EdmError> VisitT(
        IEdmDirectValueAnnotation annotation,
        List<object> followup,
        List<object> references)
      {
        List<EdmError> errors = (List<EdmError>) null;
        if (annotation.NamespaceUri == null)
          InterfaceValidator.CollectErrors(InterfaceValidator.CreatePropertyMustNotBeNullError<IEdmDirectValueAnnotation>(annotation, "NamespaceUri"), ref errors);
        if (annotation.Value == null)
          InterfaceValidator.CollectErrors(InterfaceValidator.CreatePropertyMustNotBeNullError<IEdmDirectValueAnnotation>(annotation, "Value"), ref errors);
        return (IEnumerable<EdmError>) errors;
      }
    }
  }
}
