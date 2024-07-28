// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.Evaluation.ODataConventionalEntityMetadataBuilder
// Assembly: Microsoft.TeamFoundation.OData.Core, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6619C7F6-E44A-4143-AE77-6D570F968D9A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Core.dll

using Microsoft.OData.Edm;
using Microsoft.OData.Metadata;
using Microsoft.OData.UriParser;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;

namespace Microsoft.OData.Evaluation
{
  internal sealed class ODataConventionalEntityMetadataBuilder : 
    ODataConventionalResourceMetadataBuilder
  {
    private Uri computedEditLink;
    private Uri computedReadLink;
    private string computedETag;
    private bool etagComputed;
    private Uri computedId;
    private ODataStreamReferenceValue computedMediaResource;
    private List<ODataProperty> computedStreamProperties;
    private bool isResourceEnd;
    private ODataMissingOperationGenerator missingOperationGenerator;
    private ICollection<KeyValuePair<string, object>> computedKeyProperties;

    internal ODataConventionalEntityMetadataBuilder(
      IODataResourceMetadataContext resourceMetadataContext,
      IODataMetadataContext metadataContext,
      ODataUriBuilder uriBuilder)
      : base(resourceMetadataContext, metadataContext, uriBuilder)
    {
      this.isResourceEnd = true;
    }

    private Uri ComputedId
    {
      get
      {
        this.ComputeAndCacheId();
        return this.computedId;
      }
    }

    private ODataMissingOperationGenerator MissingOperationGenerator => this.missingOperationGenerator ?? (this.missingOperationGenerator = new ODataMissingOperationGenerator(this.ResourceMetadataContext, this.MetadataContext));

    private ICollection<KeyValuePair<string, object>> ComputedKeyProperties
    {
      get
      {
        if (this.computedKeyProperties == null)
        {
          this.computedKeyProperties = (ICollection<KeyValuePair<string, object>>) new List<KeyValuePair<string, object>>();
          foreach (KeyValuePair<string, object> keyProperty in (IEnumerable<KeyValuePair<string, object>>) this.ResourceMetadataContext.KeyProperties)
          {
            object underlyingTypeIfUintValue = this.MetadataContext.Model.ConvertToUnderlyingTypeIfUIntValue(keyProperty.Value);
            this.computedKeyProperties.Add(new KeyValuePair<string, object>(keyProperty.Key, underlyingTypeIfUintValue));
          }
        }
        return this.computedKeyProperties;
      }
    }

    public override Uri GetCanonicalUrl() => this.GetId();

    public override Uri GetEditUrl() => this.GetEditLink();

    public override Uri GetReadUrl() => this.GetReadLink();

    [SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate", Justification = "A method for consistency with the rest of the API.")]
    internal override Uri GetEditLink()
    {
      if (this.ResourceMetadataContext.Resource.HasNonComputedEditLink)
        return this.ResourceMetadataContext.Resource.NonComputedEditLink;
      if (this.ResourceMetadataContext.Resource.IsTransient || this.ResourceMetadataContext.Resource.HasNonComputedReadLink)
        return (Uri) null;
      return this.computedEditLink != (Uri) null ? this.computedEditLink : (this.computedEditLink = this.ComputeEditLink());
    }

    [SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate", Justification = "A method for consistency with the rest of the API.")]
    internal override Uri GetReadLink()
    {
      if (this.ResourceMetadataContext.Resource.HasNonComputedReadLink)
        return this.ResourceMetadataContext.Resource.NonComputedReadLink;
      return this.computedReadLink != (Uri) null ? this.computedReadLink : (this.computedReadLink = this.GetEditLink());
    }

    [SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate", Justification = "A method for consistency with the rest of the API.")]
    internal override Uri GetId()
    {
      if (this.ResourceMetadataContext.Resource.HasNonComputedId)
        return this.ResourceMetadataContext.Resource.NonComputedId;
      return !this.ResourceMetadataContext.Resource.IsTransient ? this.ComputedId : (Uri) null;
    }

    [SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate", Justification = "A method for consistency with the rest of the API.")]
    internal override string GetETag()
    {
      if (this.ResourceMetadataContext.Resource.HasNonComputedETag)
        return this.ResourceMetadataContext.Resource.NonComputedETag;
      if (!this.etagComputed)
      {
        StringBuilder stringBuilder = new StringBuilder();
        foreach (KeyValuePair<string, object> etagProperty in this.ResourceMetadataContext.ETagProperties)
        {
          if (stringBuilder.Length > 0)
            stringBuilder.Append(',');
          else
            stringBuilder.Append("W/\"");
          string str = etagProperty.Value != null ? LiteralFormatter.ForConstants.Format(etagProperty.Value) : "null";
          stringBuilder.Append(str);
        }
        if (stringBuilder.Length > 0)
        {
          stringBuilder.Append('"');
          this.computedETag = stringBuilder.ToString();
        }
        this.etagComputed = true;
      }
      return this.computedETag;
    }

    [SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate", Justification = "A method for consistency with the rest of the API.")]
    internal override ODataStreamReferenceValue GetMediaResource()
    {
      if (this.ResourceMetadataContext.Resource.NonComputedMediaResource != null)
        return this.ResourceMetadataContext.Resource.NonComputedMediaResource;
      if (this.computedMediaResource == null && this.ResourceMetadataContext.TypeContext.IsMediaLinkEntry)
      {
        this.computedMediaResource = new ODataStreamReferenceValue();
        this.computedMediaResource.SetMetadataBuilder((ODataResourceMetadataBuilder) this, (string) null);
      }
      return this.computedMediaResource;
    }

    internal override IEnumerable<ODataProperty> GetProperties(
      IEnumerable<ODataProperty> nonComputedProperties)
    {
      return !this.isResourceEnd ? nonComputedProperties : ODataUtilsInternal.ConcatEnumerables<ODataProperty>(nonComputedProperties, this.GetComputedStreamProperties(nonComputedProperties));
    }

    internal override void StartResource() => this.isResourceEnd = false;

    internal override void EndResource() => this.isResourceEnd = true;

    [SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate", Justification = "A method for consistency with the rest of the API.")]
    internal override IEnumerable<ODataAction> GetActions() => ODataUtilsInternal.ConcatEnumerables<ODataAction>(this.ResourceMetadataContext.Resource.NonComputedActions, this.MissingOperationGenerator.GetComputedActions());

    [SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate", Justification = "A method for consistency with the rest of the API.")]
    internal override IEnumerable<ODataFunction> GetFunctions() => ODataUtilsInternal.ConcatEnumerables<ODataFunction>(this.ResourceMetadataContext.Resource.NonComputedFunctions, this.MissingOperationGenerator.GetComputedFunctions());

    internal override Uri GetNavigationLinkUri(
      string navigationPropertyName,
      Uri navigationLinkUrl,
      bool hasNestedResourceInfoUrl)
    {
      ExceptionUtils.CheckArgumentStringNotNullOrEmpty(navigationPropertyName, nameof (navigationPropertyName));
      return !hasNestedResourceInfoUrl ? this.UriBuilder.BuildNavigationLinkUri(this.GetReadLink(), navigationPropertyName) : navigationLinkUrl;
    }

    internal override Uri GetAssociationLinkUri(
      string navigationPropertyName,
      Uri associationLinkUrl,
      bool hasAssociationLinkUrl)
    {
      ExceptionUtils.CheckArgumentStringNotNullOrEmpty(navigationPropertyName, nameof (navigationPropertyName));
      return !hasAssociationLinkUrl ? this.UriBuilder.BuildAssociationLinkUri(this.GetReadLink(), navigationPropertyName) : associationLinkUrl;
    }

    internal override Uri GetOperationTargetUri(
      string operationName,
      string bindingParameterTypeName,
      string parameterNames)
    {
      ExceptionUtils.CheckArgumentStringNotNullOrEmpty(operationName, nameof (operationName));
      return this.ResourceMetadataContext.Resource.IsTransient ? (Uri) null : this.UriBuilder.BuildOperationTargetUri(string.IsNullOrEmpty(bindingParameterTypeName) || this.ResourceMetadataContext.Resource.NonComputedEditLink != (Uri) null ? this.GetEditLink() : this.GetId(), operationName, bindingParameterTypeName, parameterNames);
    }

    internal override string GetOperationTitle(string operationName)
    {
      ExceptionUtils.CheckArgumentStringNotNullOrEmpty(operationName, nameof (operationName));
      return operationName;
    }

    internal override bool TryGetIdForSerialization(out Uri id)
    {
      id = this.ResourceMetadataContext.Resource.IsTransient ? (Uri) null : this.GetId();
      return true;
    }

    private Uri ComputeEditLink()
    {
      Uri baseUri = this.ResourceMetadataContext.Resource.HasNonComputedId ? this.ResourceMetadataContext.Resource.NonComputedId : this.ComputedId;
      if (this.ResourceMetadataContext.ActualResourceTypeName != this.ResourceMetadataContext.TypeContext.NavigationSourceEntityTypeName)
        baseUri = this.UriBuilder.AppendTypeSegment(baseUri, this.ResourceMetadataContext.ActualResourceTypeName);
      return baseUri;
    }

    private void ComputeAndCacheId()
    {
      if (this.computedId != (Uri) null)
        return;
      Uri uri;
      switch (this.ResourceMetadataContext.TypeContext.NavigationSourceKind)
      {
        case EdmNavigationSourceKind.Singleton:
          uri = this.ComputeIdForSingleton();
          break;
        case EdmNavigationSourceKind.ContainedEntitySet:
          uri = this.ComputeIdForContainment();
          break;
        case EdmNavigationSourceKind.UnknownEntitySet:
          throw new ODataException(Microsoft.OData.Strings.ODataMetadataBuilder_UnknownEntitySet((object) this.ResourceMetadataContext.TypeContext.NavigationSourceName));
        default:
          uri = this.ComputeId();
          break;
      }
      this.computedId = uri;
    }

    private Uri ComputeId() => this.UriBuilder.BuildEntityInstanceUri(this.UriBuilder.BuildEntitySetUri(this.UriBuilder.BuildBaseUri(), this.ResourceMetadataContext.TypeContext.NavigationSourceName), this.ComputedKeyProperties, this.ResourceMetadataContext.ActualResourceTypeName);

    private Uri ComputeIdForContainment()
    {
      Uri uri;
      if (!this.TryComputeIdFromParent(out uri))
      {
        Uri baseUri = this.UriBuilder.BuildBaseUri();
        ODataUri odataUri = this.ODataUri ?? this.MetadataContext.ODataUri;
        if (odataUri == null || odataUri.Path == null || odataUri.Path.Count == 0)
          throw new ODataException(Microsoft.OData.Strings.ODataMetadataBuilder_MissingParentIdOrContextUrl);
        uri = this.GetContainingEntitySetUri(baseUri, odataUri);
      }
      Uri baseUri1 = this.UriBuilder.BuildEntitySetUri(uri, this.ResourceMetadataContext.TypeContext.NavigationSourceName);
      if (this.ResourceMetadataContext.TypeContext.IsFromCollection)
        baseUri1 = this.UriBuilder.BuildEntityInstanceUri(baseUri1, this.ComputedKeyProperties, this.ResourceMetadataContext.ActualResourceTypeName);
      return baseUri1;
    }

    private bool TryComputeIdFromParent(out Uri uri)
    {
      try
      {
        if (this.ParentMetadataBuilder is ODataConventionalResourceMetadataBuilder parentMetadataBuilder)
        {
          if (parentMetadataBuilder != this)
          {
            uri = parentMetadataBuilder.GetCanonicalUrl();
            if (uri != (Uri) null)
            {
              IODataResourceTypeContext typeContext = parentMetadataBuilder.ResourceMetadataContext.TypeContext;
              if (parentMetadataBuilder.ResourceMetadataContext.ActualResourceTypeName != typeContext.ExpectedResourceTypeName && (!(typeContext is ODataResourceTypeContext.ODataResourceTypeContextWithModel contextWithModel) || contextWithModel.ExpectedResourceType.FindProperty(this.ResourceMetadataContext.TypeContext.NavigationSourceName) == null))
                uri = new Uri(UriUtils.EnsureTaillingSlash(uri), parentMetadataBuilder.ResourceMetadataContext.ActualResourceTypeName);
              return true;
            }
          }
        }
      }
      catch (ODataException ex)
      {
      }
      uri = (Uri) null;
      return false;
    }

    private Uri ComputeIdForSingleton() => this.UriBuilder.BuildEntitySetUri(this.UriBuilder.BuildBaseUri(), this.ResourceMetadataContext.TypeContext.NavigationSourceName);

    private IEnumerable<ODataProperty> GetComputedStreamProperties(
      IEnumerable<ODataProperty> nonComputedProperties)
    {
      if (this.computedStreamProperties == null)
      {
        IDictionary<string, IEdmStructuralProperty> streamProperties1 = this.ResourceMetadataContext.SelectedStreamProperties;
        if (nonComputedProperties != null)
        {
          foreach (ODataProperty computedProperty in nonComputedProperties)
            streamProperties1.Remove(computedProperty.Name);
        }
        this.computedStreamProperties = new List<ODataProperty>();
        if (streamProperties1.Count > 0)
        {
          foreach (string key in (IEnumerable<string>) streamProperties1.Keys)
          {
            ODataStreamReferenceValue streamReferenceValue = new ODataStreamReferenceValue();
            streamReferenceValue.SetMetadataBuilder((ODataResourceMetadataBuilder) this, key);
            List<ODataProperty> streamProperties2 = this.computedStreamProperties;
            ODataProperty odataProperty = new ODataProperty();
            odataProperty.Name = key;
            odataProperty.Value = (object) streamReferenceValue;
            streamProperties2.Add(odataProperty);
          }
        }
      }
      return (IEnumerable<ODataProperty>) this.computedStreamProperties;
    }

    private Uri GetContainingEntitySetUri(Uri baseUri, ODataUri odataUri)
    {
      List<ODataPathSegment> list = odataUri.Path.ToList<ODataPathSegment>();
      ODataPathSegment odataPathSegment1 = list.Last<ODataPathSegment>();
      while (true)
      {
        switch (odataPathSegment1)
        {
          case NavigationPropertySegment _:
          case OperationSegment _:
            goto label_3;
          default:
            list.Remove(odataPathSegment1);
            odataPathSegment1 = list.Last<ODataPathSegment>();
            continue;
        }
      }
label_3:
      list.Remove(odataPathSegment1);
      for (ODataPathSegment odataPathSegment2 = list.Last<ODataPathSegment>(); odataPathSegment2 is TypeSegment && list[list.Count - 2].TargetEdmType is IEdmStructuredType targetEdmType && targetEdmType.FindProperty(odataPathSegment1.Identifier) != null; odataPathSegment2 = list.Last<ODataPathSegment>())
        list.Remove(odataPathSegment2);
      Uri baseUri1 = baseUri;
      foreach (ODataPathSegment odataPathSegment3 in list)
        baseUri1 = odataPathSegment3 is KeySegment keySegment ? this.UriBuilder.BuildEntityInstanceUri(baseUri1, (ICollection<KeyValuePair<string, object>>) keySegment.Keys.ToList<KeyValuePair<string, object>>(), keySegment.EdmType.FullTypeName()) : this.UriBuilder.BuildEntitySetUri(baseUri1, odataPathSegment3.Identifier);
      return baseUri1;
    }
  }
}
