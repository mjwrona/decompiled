// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.UriParser.KeyFinder
// Assembly: Microsoft.TeamFoundation.OData.Core, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6619C7F6-E44A-4143-AE77-6D570F968D9A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Core.dll

using Microsoft.OData.Edm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Microsoft.OData.UriParser
{
  internal static class KeyFinder
  {
    public static SegmentArgumentParser FindAndUseKeysFromRelatedSegment(
      SegmentArgumentParser rawKeyValuesFromUri,
      IEnumerable<IEdmStructuralProperty> targetEntityKeyProperties,
      IEdmNavigationProperty currentNavigationProperty,
      KeySegment keySegmentOfParentEntity)
    {
      ExceptionUtils.CheckArgumentNotNull<IEdmNavigationProperty>(currentNavigationProperty, nameof (currentNavigationProperty));
      ExceptionUtils.CheckArgumentNotNull<SegmentArgumentParser>(rawKeyValuesFromUri, nameof (rawKeyValuesFromUri));
      ReadOnlyCollection<IEdmStructuralProperty> readOnlyCollection = targetEntityKeyProperties != null ? new ReadOnlyCollection<IEdmStructuralProperty>((IList<IEdmStructuralProperty>) targetEntityKeyProperties.ToList<IEdmStructuralProperty>()) : new ReadOnlyCollection<IEdmStructuralProperty>((IList<IEdmStructuralProperty>) new List<IEdmStructuralProperty>());
      bool flag = !rawKeyValuesFromUri.AreValuesNamed;
      if (flag && rawKeyValuesFromUri.ValueCount > 1 || keySegmentOfParentEntity == null)
        return rawKeyValuesFromUri;
      List<EdmReferentialConstraintPropertyPair> list1 = KeyFinder.ExtractMatchingPropertyPairsFromNavProp(currentNavigationProperty, (IEnumerable<IEdmStructuralProperty>) readOnlyCollection).ToList<EdmReferentialConstraintPropertyPair>();
      foreach (EdmReferentialConstraintPropertyPair constraintPropertyPair in list1)
      {
        EdmReferentialConstraintPropertyPair keyFromReferentialIntegrityConstraint = constraintPropertyPair;
        KeyValuePair<string, object> keyValuePair = keySegmentOfParentEntity.Keys.SingleOrDefault<KeyValuePair<string, object>>((Func<KeyValuePair<string, object>, bool>) (x => x.Key == keyFromReferentialIntegrityConstraint.DependentProperty.Name));
        if (keyValuePair.Key != null && readOnlyCollection.Any<IEdmStructuralProperty>((Func<IEdmStructuralProperty, bool>) (x => x.Name == keyFromReferentialIntegrityConstraint.PrincipalProperty.Name)))
          rawKeyValuesFromUri.AddNamedValue(keyFromReferentialIntegrityConstraint.PrincipalProperty.Name, KeyFinder.ConvertKeyValueToUriLiteral(keyValuePair.Value, rawKeyValuesFromUri.KeyAsSegment));
      }
      list1.Clear();
      IEdmNavigationProperty partner = currentNavigationProperty.Partner;
      if (partner != null)
        list1.AddRange(KeyFinder.ExtractMatchingPropertyPairsFromReversedNavProp(partner, (IEnumerable<IEdmStructuralProperty>) readOnlyCollection));
      foreach (EdmReferentialConstraintPropertyPair constraintPropertyPair in list1)
      {
        EdmReferentialConstraintPropertyPair keyFromReferentialIntegrityConstraint = constraintPropertyPair;
        KeyValuePair<string, object> keyValuePair = keySegmentOfParentEntity.Keys.SingleOrDefault<KeyValuePair<string, object>>((Func<KeyValuePair<string, object>, bool>) (x => x.Key == keyFromReferentialIntegrityConstraint.PrincipalProperty.Name));
        if (keyValuePair.Key != null && readOnlyCollection.Any<IEdmStructuralProperty>((Func<IEdmStructuralProperty, bool>) (x => x.Name == keyFromReferentialIntegrityConstraint.DependentProperty.Name)))
          rawKeyValuesFromUri.AddNamedValue(keyFromReferentialIntegrityConstraint.DependentProperty.Name, KeyFinder.ConvertKeyValueToUriLiteral(keyValuePair.Value, rawKeyValuesFromUri.KeyAsSegment));
      }
      if (flag && rawKeyValuesFromUri.NamedValues != null)
      {
        List<IEdmStructuralProperty> list2 = readOnlyCollection.Where<IEdmStructuralProperty>((Func<IEdmStructuralProperty, bool>) (x => !rawKeyValuesFromUri.NamedValues.ContainsKey(x.Name))).ToList<IEdmStructuralProperty>();
        if (list2.Count != 1)
          return rawKeyValuesFromUri;
        rawKeyValuesFromUri.AddNamedValue(list2[0].Name, rawKeyValuesFromUri.PositionalValues[0]);
        rawKeyValuesFromUri.PositionalValues.Clear();
      }
      return rawKeyValuesFromUri;
    }

    private static IEnumerable<EdmReferentialConstraintPropertyPair> ExtractMatchingPropertyPairsFromNavProp(
      IEdmNavigationProperty currentNavigationProperty,
      IEnumerable<IEdmStructuralProperty> targetKeyPropertyList)
    {
      return currentNavigationProperty != null && currentNavigationProperty.ReferentialConstraint != null ? currentNavigationProperty.ReferentialConstraint.PropertyPairs.Where<EdmReferentialConstraintPropertyPair>((Func<EdmReferentialConstraintPropertyPair, bool>) (x => targetKeyPropertyList.Any<IEdmStructuralProperty>((Func<IEdmStructuralProperty, bool>) (y => y == x.PrincipalProperty)))) : (IEnumerable<EdmReferentialConstraintPropertyPair>) new List<EdmReferentialConstraintPropertyPair>();
    }

    private static IEnumerable<EdmReferentialConstraintPropertyPair> ExtractMatchingPropertyPairsFromReversedNavProp(
      IEdmNavigationProperty currentNavigationProperty,
      IEnumerable<IEdmStructuralProperty> targetKeyPropertyList)
    {
      return currentNavigationProperty != null && currentNavigationProperty.ReferentialConstraint != null ? currentNavigationProperty.ReferentialConstraint.PropertyPairs.Where<EdmReferentialConstraintPropertyPair>((Func<EdmReferentialConstraintPropertyPair, bool>) (x => targetKeyPropertyList.Any<IEdmStructuralProperty>((Func<IEdmStructuralProperty, bool>) (y => y == x.DependentProperty)))) : (IEnumerable<EdmReferentialConstraintPropertyPair>) new List<EdmReferentialConstraintPropertyPair>();
    }

    private static string ConvertKeyValueToUriLiteral(object value, bool keyAsSegment)
    {
      string str = value as string;
      return keyAsSegment && str != null ? str : ODataUriConversionUtils.ConvertToUriPrimitiveLiteral(value, ODataVersion.V4);
    }
  }
}
