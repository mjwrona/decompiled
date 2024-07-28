// Decompiled with JetBrains decompiler
// Type: Microsoft.AspNet.OData.Builder.CapabilitiesVocabularyExtensionMethods
// Assembly: Microsoft.TeamFoundation.AspNet.OData, Version=7.3.1.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 08758328-0988-4E6C-88EC-9BA90EA42587
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.AspNet.OData.dll

using Microsoft.OData.Edm;
using Microsoft.OData.Edm.Csdl;
using Microsoft.OData.Edm.Vocabularies;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.AspNet.OData.Builder
{
  internal static class CapabilitiesVocabularyExtensionMethods
  {
    private static readonly IEnumerable<IEdmStructuralProperty> EmptyStructuralProperties = Enumerable.Empty<IEdmStructuralProperty>();
    private static readonly IEnumerable<IEdmNavigationProperty> EmptyNavigationProperties = Enumerable.Empty<IEdmNavigationProperty>();
    private static IEdmEnumType _navigationType;

    public static void SetCountRestrictionsAnnotation(
      this EdmModel model,
      IEdmEntitySet target,
      bool isCountable,
      IEnumerable<IEdmProperty> nonCountableProperties,
      IEnumerable<IEdmNavigationProperty> nonCountableNavigationProperties)
    {
      if (model == null)
        throw Microsoft.AspNet.OData.Common.Error.ArgumentNull(nameof (model));
      if (target == null)
        throw Microsoft.AspNet.OData.Common.Error.ArgumentNull(nameof (target));
      nonCountableProperties = nonCountableProperties ?? (IEnumerable<IEdmProperty>) CapabilitiesVocabularyExtensionMethods.EmptyStructuralProperties;
      nonCountableNavigationProperties = nonCountableNavigationProperties ?? CapabilitiesVocabularyExtensionMethods.EmptyNavigationProperties;
      IList<IEdmPropertyConstructor> properties = (IList<IEdmPropertyConstructor>) new List<IEdmPropertyConstructor>()
      {
        (IEdmPropertyConstructor) new EdmPropertyConstructor("Countable", (IEdmExpression) new EdmBooleanConstant(isCountable)),
        (IEdmPropertyConstructor) new EdmPropertyConstructor("NonCountableProperties", (IEdmExpression) new EdmCollectionExpression((IEdmExpression[]) nonCountableProperties.Select<IEdmProperty, EdmPropertyPathExpression>((Func<IEdmProperty, EdmPropertyPathExpression>) (p => new EdmPropertyPathExpression(p.Name))).ToArray<EdmPropertyPathExpression>())),
        (IEdmPropertyConstructor) new EdmPropertyConstructor("NonCountableNavigationProperties", (IEdmExpression) new EdmCollectionExpression((IEdmExpression[]) nonCountableNavigationProperties.Select<IEdmNavigationProperty, EdmNavigationPropertyPathExpression>((Func<IEdmNavigationProperty, EdmNavigationPropertyPathExpression>) (p => new EdmNavigationPropertyPathExpression(p.Name))).ToArray<EdmNavigationPropertyPathExpression>()))
      };
      model.SetVocabularyAnnotation((IEdmVocabularyAnnotatable) target, properties, "Org.OData.Capabilities.V1.CountRestrictions");
    }

    public static void SetNavigationRestrictionsAnnotation(
      this EdmModel model,
      IEdmEntitySet target,
      CapabilitiesNavigationType navigability,
      IEnumerable<Tuple<IEdmNavigationProperty, CapabilitiesNavigationType>> restrictedProperties)
    {
      if (model == null)
        throw Microsoft.AspNet.OData.Common.Error.ArgumentNull(nameof (model));
      if (target == null)
        throw Microsoft.AspNet.OData.Common.Error.ArgumentNull(nameof (target));
      IEdmEnumType navigationType = model.GetCapabilitiesNavigationType();
      if (navigationType == null)
        return;
      restrictedProperties = (IEnumerable<Tuple<IEdmNavigationProperty, CapabilitiesNavigationType>>) ((object) restrictedProperties ?? (object) new Tuple<IEdmNavigationProperty, CapabilitiesNavigationType>[0]);
      string type = new EdmEnumTypeReference(navigationType, false).ToStringLiteral((long) navigability);
      IEnumerable<EdmRecordExpression> elements = restrictedProperties.Select<Tuple<IEdmNavigationProperty, CapabilitiesNavigationType>, EdmRecordExpression>((Func<Tuple<IEdmNavigationProperty, CapabilitiesNavigationType>, EdmRecordExpression>) (p =>
      {
        string name = new EdmEnumTypeReference(navigationType, false).ToStringLiteral((long) p.Item2);
        return new EdmRecordExpression(new IEdmPropertyConstructor[2]
        {
          (IEdmPropertyConstructor) new EdmPropertyConstructor("NavigationProperty", (IEdmExpression) new EdmNavigationPropertyPathExpression(p.Item1.Name)),
          (IEdmPropertyConstructor) new EdmPropertyConstructor("Navigability", (IEdmExpression) new EdmEnumMemberExpression(new IEdmEnumMember[1]
          {
            navigationType.Members.Single<IEdmEnumMember>((Func<IEdmEnumMember, bool>) (m => m.Name == name))
          }))
        });
      }));
      IList<IEdmPropertyConstructor> properties = (IList<IEdmPropertyConstructor>) new List<IEdmPropertyConstructor>()
      {
        (IEdmPropertyConstructor) new EdmPropertyConstructor("Navigability", (IEdmExpression) new EdmEnumMemberExpression(new IEdmEnumMember[1]
        {
          navigationType.Members.Single<IEdmEnumMember>((Func<IEdmEnumMember, bool>) (m => m.Name == type))
        })),
        (IEdmPropertyConstructor) new EdmPropertyConstructor("RestrictedProperties", (IEdmExpression) new EdmCollectionExpression((IEnumerable<IEdmExpression>) elements))
      };
      model.SetVocabularyAnnotation((IEdmVocabularyAnnotatable) target, properties, "Org.OData.Capabilities.V1.NavigationRestrictions");
    }

    public static void SetFilterRestrictionsAnnotation(
      this EdmModel model,
      IEdmEntitySet target,
      bool isFilterable,
      bool isRequiresFilter,
      IEnumerable<IEdmProperty> requiredProperties,
      IEnumerable<IEdmProperty> nonFilterableProperties)
    {
      if (model == null)
        throw Microsoft.AspNet.OData.Common.Error.ArgumentNull(nameof (model));
      if (target == null)
        throw Microsoft.AspNet.OData.Common.Error.ArgumentNull(nameof (target));
      requiredProperties = requiredProperties ?? (IEnumerable<IEdmProperty>) CapabilitiesVocabularyExtensionMethods.EmptyStructuralProperties;
      nonFilterableProperties = nonFilterableProperties ?? (IEnumerable<IEdmProperty>) CapabilitiesVocabularyExtensionMethods.EmptyStructuralProperties;
      IList<IEdmPropertyConstructor> properties = (IList<IEdmPropertyConstructor>) new List<IEdmPropertyConstructor>()
      {
        (IEdmPropertyConstructor) new EdmPropertyConstructor("Filterable", (IEdmExpression) new EdmBooleanConstant(isFilterable)),
        (IEdmPropertyConstructor) new EdmPropertyConstructor("RequiresFilter", (IEdmExpression) new EdmBooleanConstant(isRequiresFilter)),
        (IEdmPropertyConstructor) new EdmPropertyConstructor("RequiredProperties", (IEdmExpression) new EdmCollectionExpression((IEdmExpression[]) requiredProperties.Select<IEdmProperty, EdmPropertyPathExpression>((Func<IEdmProperty, EdmPropertyPathExpression>) (p => new EdmPropertyPathExpression(p.Name))).ToArray<EdmPropertyPathExpression>())),
        (IEdmPropertyConstructor) new EdmPropertyConstructor("NonFilterableProperties", (IEdmExpression) new EdmCollectionExpression((IEdmExpression[]) nonFilterableProperties.Select<IEdmProperty, EdmPropertyPathExpression>((Func<IEdmProperty, EdmPropertyPathExpression>) (p => new EdmPropertyPathExpression(p.Name))).ToArray<EdmPropertyPathExpression>()))
      };
      model.SetVocabularyAnnotation((IEdmVocabularyAnnotatable) target, properties, "Org.OData.Capabilities.V1.FilterRestrictions");
    }

    public static void SetSortRestrictionsAnnotation(
      this EdmModel model,
      IEdmEntitySet target,
      bool isSortable,
      IEnumerable<IEdmProperty> ascendingOnlyProperties,
      IEnumerable<IEdmProperty> descendingOnlyProperties,
      IEnumerable<IEdmProperty> nonSortableProperties)
    {
      if (model == null)
        throw Microsoft.AspNet.OData.Common.Error.ArgumentNull(nameof (model));
      if (target == null)
        throw Microsoft.AspNet.OData.Common.Error.ArgumentNull(nameof (target));
      ascendingOnlyProperties = ascendingOnlyProperties ?? (IEnumerable<IEdmProperty>) CapabilitiesVocabularyExtensionMethods.EmptyStructuralProperties;
      descendingOnlyProperties = descendingOnlyProperties ?? (IEnumerable<IEdmProperty>) CapabilitiesVocabularyExtensionMethods.EmptyStructuralProperties;
      nonSortableProperties = nonSortableProperties ?? (IEnumerable<IEdmProperty>) CapabilitiesVocabularyExtensionMethods.EmptyStructuralProperties;
      IList<IEdmPropertyConstructor> properties = (IList<IEdmPropertyConstructor>) new List<IEdmPropertyConstructor>()
      {
        (IEdmPropertyConstructor) new EdmPropertyConstructor("Sortable", (IEdmExpression) new EdmBooleanConstant(isSortable)),
        (IEdmPropertyConstructor) new EdmPropertyConstructor("AscendingOnlyProperties", (IEdmExpression) new EdmCollectionExpression((IEdmExpression[]) ascendingOnlyProperties.Select<IEdmProperty, EdmPropertyPathExpression>((Func<IEdmProperty, EdmPropertyPathExpression>) (p => new EdmPropertyPathExpression(p.Name))).ToArray<EdmPropertyPathExpression>())),
        (IEdmPropertyConstructor) new EdmPropertyConstructor("DescendingOnlyProperties", (IEdmExpression) new EdmCollectionExpression((IEdmExpression[]) descendingOnlyProperties.Select<IEdmProperty, EdmPropertyPathExpression>((Func<IEdmProperty, EdmPropertyPathExpression>) (p => new EdmPropertyPathExpression(p.Name))).ToArray<EdmPropertyPathExpression>())),
        (IEdmPropertyConstructor) new EdmPropertyConstructor("NonSortableProperties", (IEdmExpression) new EdmCollectionExpression((IEdmExpression[]) nonSortableProperties.Select<IEdmProperty, EdmPropertyPathExpression>((Func<IEdmProperty, EdmPropertyPathExpression>) (p => new EdmPropertyPathExpression(p.Name))).ToArray<EdmPropertyPathExpression>()))
      };
      model.SetVocabularyAnnotation((IEdmVocabularyAnnotatable) target, properties, "Org.OData.Capabilities.V1.SortRestrictions");
    }

    public static void SetExpandRestrictionsAnnotation(
      this EdmModel model,
      IEdmEntitySet target,
      bool isExpandable,
      IEnumerable<IEdmNavigationProperty> nonExpandableProperties)
    {
      if (model == null)
        throw Microsoft.AspNet.OData.Common.Error.ArgumentNull(nameof (model));
      if (target == null)
        throw Microsoft.AspNet.OData.Common.Error.ArgumentNull(nameof (target));
      nonExpandableProperties = nonExpandableProperties ?? CapabilitiesVocabularyExtensionMethods.EmptyNavigationProperties;
      IList<IEdmPropertyConstructor> properties = (IList<IEdmPropertyConstructor>) new List<IEdmPropertyConstructor>()
      {
        (IEdmPropertyConstructor) new EdmPropertyConstructor("Expandable", (IEdmExpression) new EdmBooleanConstant(isExpandable)),
        (IEdmPropertyConstructor) new EdmPropertyConstructor("NonExpandableProperties", (IEdmExpression) new EdmCollectionExpression((IEdmExpression[]) nonExpandableProperties.Select<IEdmNavigationProperty, EdmNavigationPropertyPathExpression>((Func<IEdmNavigationProperty, EdmNavigationPropertyPathExpression>) (p => new EdmNavigationPropertyPathExpression(p.Name))).ToArray<EdmNavigationPropertyPathExpression>()))
      };
      model.SetVocabularyAnnotation((IEdmVocabularyAnnotatable) target, properties, "Org.OData.Capabilities.V1.ExpandRestrictions");
    }

    private static void SetVocabularyAnnotation(
      this EdmModel model,
      IEdmVocabularyAnnotatable target,
      IList<IEdmPropertyConstructor> properties,
      string qualifiedName)
    {
      IEdmTerm term = model.FindTerm(qualifiedName);
      if (term == null)
        return;
      IEdmRecordExpression recordExpression = (IEdmRecordExpression) new EdmRecordExpression((IEnumerable<IEdmPropertyConstructor>) properties);
      EdmVocabularyAnnotation annotation = new EdmVocabularyAnnotation(target, term, (IEdmExpression) recordExpression);
      annotation.SetSerializationLocation((IEdmModel) model, new EdmVocabularyAnnotationSerializationLocation?(EdmVocabularyAnnotationSerializationLocation.Inline));
      model.SetVocabularyAnnotation((IEdmVocabularyAnnotation) annotation);
    }

    private static IEdmEnumType GetCapabilitiesNavigationType(this EdmModel model) => CapabilitiesVocabularyExtensionMethods._navigationType ?? (CapabilitiesVocabularyExtensionMethods._navigationType = model.FindType("Org.OData.Capabilities.V1.NavigationType") as IEdmEnumType);
  }
}
