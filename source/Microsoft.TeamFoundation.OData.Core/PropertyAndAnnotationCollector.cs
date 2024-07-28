// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.PropertyAndAnnotationCollector
// Assembly: Microsoft.TeamFoundation.OData.Core, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6619C7F6-E44A-4143-AE77-6D570F968D9A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Core.dll

using Microsoft.OData.JsonLight;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.OData
{
  internal sealed class PropertyAndAnnotationCollector
  {
    private static readonly IDictionary<string, object> emptyDictionary = (IDictionary<string, object>) new Dictionary<string, object>();
    private readonly bool throwOnDuplicateProperty;
    private IDictionary<string, object> odataScopeAnnotations = (IDictionary<string, object>) new Dictionary<string, object>();
    private IList<KeyValuePair<string, object>> customScopeAnnotations = (IList<KeyValuePair<string, object>>) new List<KeyValuePair<string, object>>();
    private IDictionary<string, PropertyAndAnnotationCollector.PropertyData> propertyData = (IDictionary<string, PropertyAndAnnotationCollector.PropertyData>) new Dictionary<string, PropertyAndAnnotationCollector.PropertyData>();

    internal PropertyAndAnnotationCollector(bool throwOnDuplicateProperty) => this.throwOnDuplicateProperty = throwOnDuplicateProperty;

    internal void CheckForDuplicatePropertyNames(ODataProperty property)
    {
      if (!this.throwOnDuplicateProperty)
        return;
      string name = property.Name;
      PropertyAndAnnotationCollector.PropertyData propertyData;
      if (!this.propertyData.TryGetValue(name, out propertyData))
        this.propertyData[name] = new PropertyAndAnnotationCollector.PropertyData(PropertyAndAnnotationCollector.PropertyState.SimpleProperty);
      else
        propertyData.State = propertyData.State == PropertyAndAnnotationCollector.PropertyState.AnnotationSeen ? PropertyAndAnnotationCollector.PropertyState.SimpleProperty : throw new ODataException(Strings.DuplicatePropertyNamesNotAllowed((object) name));
    }

    internal void ValidatePropertyUniquenessOnNestedResourceInfoStart(
      ODataNestedResourceInfo nestedResourceInfo)
    {
      if (!this.throwOnDuplicateProperty)
        return;
      string name = nestedResourceInfo.Name;
      PropertyAndAnnotationCollector.PropertyData propertyData;
      if (!this.propertyData.TryGetValue(name, out propertyData))
        return;
      PropertyAndAnnotationCollector.CheckNestedResourceInfoDuplicateNameForExistingDuplicationRecord(name, propertyData);
    }

    internal Uri ValidatePropertyUniquenessAndGetAssociationLink(
      ODataNestedResourceInfo nestedResourceInfo)
    {
      string name = nestedResourceInfo.Name;
      PropertyAndAnnotationCollector.PropertyData propertyData;
      if (!this.propertyData.TryGetValue(name, out propertyData))
      {
        this.propertyData[name] = new PropertyAndAnnotationCollector.PropertyData(PropertyAndAnnotationCollector.PropertyState.NavigationProperty)
        {
          NestedResourceInfo = nestedResourceInfo
        };
        return (Uri) null;
      }
      if (this.throwOnDuplicateProperty)
        PropertyAndAnnotationCollector.CheckNestedResourceInfoDuplicateNameForExistingDuplicationRecord(name, propertyData);
      propertyData.State = PropertyAndAnnotationCollector.PropertyState.NavigationProperty;
      propertyData.NestedResourceInfo = nestedResourceInfo;
      return propertyData.AssociationLinkUrl;
    }

    internal ODataNestedResourceInfo ValidatePropertyOpenForAssociationLinkAndGetNestedResourceInfo(
      string propertyName,
      Uri associationLinkUrl)
    {
      PropertyAndAnnotationCollector.PropertyData propertyData;
      if (!this.propertyData.TryGetValue(propertyName, out propertyData))
      {
        this.propertyData[propertyName] = new PropertyAndAnnotationCollector.PropertyData(PropertyAndAnnotationCollector.PropertyState.NavigationProperty)
        {
          AssociationLinkUrl = associationLinkUrl
        };
        return (ODataNestedResourceInfo) null;
      }
      if (propertyData.State != PropertyAndAnnotationCollector.PropertyState.AnnotationSeen && (propertyData.State != PropertyAndAnnotationCollector.PropertyState.NavigationProperty || !(propertyData.AssociationLinkUrl == (Uri) null)))
        throw new ODataException(Strings.DuplicateAnnotationNotAllowed((object) "odata.associationLink"));
      propertyData.State = PropertyAndAnnotationCollector.PropertyState.NavigationProperty;
      propertyData.AssociationLinkUrl = associationLinkUrl;
      return propertyData.NestedResourceInfo;
    }

    internal void Reset() => this.propertyData.Clear();

    internal void AddODataScopeAnnotation(string annotationName, object annotationValue)
    {
      if (annotationValue == null)
        return;
      try
      {
        this.odataScopeAnnotations.Add(annotationName, annotationValue);
      }
      catch (ArgumentException ex)
      {
        throw new ODataException(Strings.DuplicateAnnotationNotAllowed((object) annotationName));
      }
    }

    internal void AddCustomScopeAnnotation(string annotationName, object annotationValue)
    {
      if (annotationValue == null)
        return;
      this.customScopeAnnotations.Add(new KeyValuePair<string, object>(annotationName, annotationValue));
    }

    internal IDictionary<string, object> GetODataScopeAnnotation() => this.odataScopeAnnotations;

    internal IEnumerable<KeyValuePair<string, object>> GetCustomScopeAnnotation() => (IEnumerable<KeyValuePair<string, object>>) this.customScopeAnnotations;

    internal void AddODataPropertyAnnotation(
      string propertyName,
      string annotationName,
      object annotationValue)
    {
      if (annotationValue == null)
        return;
      PropertyAndAnnotationCollector.PropertyData propertyData;
      if (!this.propertyData.TryGetValue(propertyName, out propertyData))
        this.propertyData[propertyName] = propertyData = new PropertyAndAnnotationCollector.PropertyData(PropertyAndAnnotationCollector.PropertyState.AnnotationSeen);
      else if (propertyData.Processed)
        throw new ODataException(Strings.PropertyAnnotationAfterTheProperty((object) annotationName, (object) propertyName));
      try
      {
        propertyData.ODataAnnotations.Add(annotationName, annotationValue);
      }
      catch (ArgumentException ex)
      {
        throw new ODataException(ODataJsonLightReaderUtils.IsAnnotationProperty(propertyName) ? Strings.DuplicateAnnotationForInstanceAnnotationNotAllowed((object) annotationName, (object) propertyName) : Strings.DuplicateAnnotationForPropertyNotAllowed((object) annotationName, (object) propertyName));
      }
    }

    internal void AddCustomPropertyAnnotation(
      string propertyName,
      string annotationName,
      object annotationValue)
    {
      if (annotationValue == null)
        return;
      PropertyAndAnnotationCollector.PropertyData propertyData;
      if (!this.propertyData.TryGetValue(propertyName, out propertyData))
        this.propertyData[propertyName] = propertyData = new PropertyAndAnnotationCollector.PropertyData(PropertyAndAnnotationCollector.PropertyState.AnnotationSeen);
      else if (propertyData.Processed)
        throw new ODataException(Strings.PropertyAnnotationAfterTheProperty((object) annotationName, (object) propertyName));
      propertyData.CustomAnnotations.Add(new KeyValuePair<string, object>(annotationName, annotationValue));
    }

    internal void SetDerivedTypeValidator(string propertyName, DerivedTypeValidator validator)
    {
      if (validator == null)
        return;
      PropertyAndAnnotationCollector.PropertyData propertyData;
      if (!this.propertyData.TryGetValue(propertyName, out propertyData))
        this.propertyData[propertyName] = propertyData = new PropertyAndAnnotationCollector.PropertyData(PropertyAndAnnotationCollector.PropertyState.AnnotationSeen);
      propertyData.DerivedTypeValidator = validator;
    }

    internal DerivedTypeValidator GetDerivedTypeValidator(string propertyName)
    {
      PropertyAndAnnotationCollector.PropertyData propertyData;
      return !this.propertyData.TryGetValue(propertyName, out propertyData) ? (DerivedTypeValidator) null : propertyData.DerivedTypeValidator;
    }

    internal IDictionary<string, object> GetODataPropertyAnnotations(string propertyName)
    {
      PropertyAndAnnotationCollector.PropertyData propertyData;
      return !this.propertyData.TryGetValue(propertyName, out propertyData) ? PropertyAndAnnotationCollector.emptyDictionary : propertyData.ODataAnnotations;
    }

    internal IEnumerable<KeyValuePair<string, object>> GetCustomPropertyAnnotations(
      string propertyName)
    {
      PropertyAndAnnotationCollector.PropertyData propertyData;
      return !this.propertyData.TryGetValue(propertyName, out propertyData) ? Enumerable.Empty<KeyValuePair<string, object>>() : (IEnumerable<KeyValuePair<string, object>>) propertyData.CustomAnnotations;
    }

    internal void MarkPropertyAsProcessed(string propertyName)
    {
      PropertyAndAnnotationCollector.PropertyData propertyData;
      if (!this.propertyData.TryGetValue(propertyName, out propertyData))
        this.propertyData[propertyName] = propertyData = new PropertyAndAnnotationCollector.PropertyData(PropertyAndAnnotationCollector.PropertyState.AnnotationSeen);
      propertyData.Processed = !propertyData.Processed ? true : throw new ODataException(!ODataJsonLightReaderUtils.IsAnnotationProperty(propertyName) || ODataJsonLightUtils.IsMetadataReferenceProperty(propertyName) ? Strings.DuplicatePropertyNamesNotAllowed((object) propertyName) : Strings.DuplicateAnnotationNotAllowed((object) propertyName));
    }

    internal void CheckIfPropertyOpenForAnnotations(string propertyName, string annotationName)
    {
      PropertyAndAnnotationCollector.PropertyData propertyData;
      if (this.propertyData.TryGetValue(propertyName, out propertyData) && propertyData.Processed)
        throw new ODataException(Strings.PropertyAnnotationAfterTheProperty((object) annotationName, (object) propertyName));
    }

    private static void CheckNestedResourceInfoDuplicateNameForExistingDuplicationRecord(
      string propertyName,
      PropertyAndAnnotationCollector.PropertyData propertyData)
    {
      if ((propertyData.State != PropertyAndAnnotationCollector.PropertyState.NavigationProperty || !(propertyData.AssociationLinkUrl != (Uri) null) || propertyData.NestedResourceInfo != null) && propertyData.State != PropertyAndAnnotationCollector.PropertyState.AnnotationSeen)
        throw new ODataException(Strings.DuplicatePropertyNamesNotAllowed((object) propertyName));
    }

    private enum PropertyState
    {
      AnnotationSeen,
      SimpleProperty,
      NavigationProperty,
    }

    private sealed class PropertyData
    {
      internal PropertyData(
        PropertyAndAnnotationCollector.PropertyState propertyState)
      {
        this.State = propertyState;
        this.ODataAnnotations = (IDictionary<string, object>) new Dictionary<string, object>();
        this.CustomAnnotations = (IList<KeyValuePair<string, object>>) new List<KeyValuePair<string, object>>();
      }

      internal PropertyAndAnnotationCollector.PropertyState State { get; set; }

      internal ODataNestedResourceInfo NestedResourceInfo { get; set; }

      internal Uri AssociationLinkUrl { get; set; }

      internal IDictionary<string, object> ODataAnnotations { get; private set; }

      internal IList<KeyValuePair<string, object>> CustomAnnotations { get; private set; }

      internal bool Processed { get; set; }

      internal DerivedTypeValidator DerivedTypeValidator { get; set; }
    }
  }
}
