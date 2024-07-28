// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.Evaluation.ODataConventionalResourceMetadataBuilder
// Assembly: Microsoft.TeamFoundation.OData.Core, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6619C7F6-E44A-4143-AE77-6D570F968D9A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Core.dll

using Microsoft.OData.Edm;
using Microsoft.OData.JsonLight;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace Microsoft.OData.Evaluation
{
  internal class ODataConventionalResourceMetadataBuilder : ODataResourceMetadataBuilder
  {
    public readonly IODataResourceMetadataContext ResourceMetadataContext;
    protected readonly ODataUriBuilder UriBuilder;
    protected readonly IODataMetadataContext MetadataContext;
    protected readonly HashSet<string> ProcessedNestedResourceInfos;
    protected readonly HashSet<string> ProcessedStreamProperties;
    private IEnumerator<ODataJsonLightReaderNestedResourceInfo> unprocessedNavigationLinks;
    private IEnumerator<string> unprocessedStreamProperties;
    private Uri readUrl;
    private Uri editUrl;
    private Uri canonicalUrl;
    private ODataResourceBase resource;

    internal ODataConventionalResourceMetadataBuilder(
      IODataResourceMetadataContext resourceMetadataContext,
      IODataMetadataContext metadataContext,
      ODataUriBuilder uriBuilder)
    {
      this.ResourceMetadataContext = resourceMetadataContext;
      this.UriBuilder = uriBuilder;
      this.MetadataContext = metadataContext;
      this.ProcessedNestedResourceInfos = new HashSet<string>((IEqualityComparer<string>) StringComparer.Ordinal);
      this.ProcessedStreamProperties = new HashSet<string>((IEqualityComparer<string>) StringComparer.Ordinal);
      this.resource = resourceMetadataContext.Resource;
    }

    internal ODataUri ODataUri { get; set; }

    public virtual Uri GetEditUrl()
    {
      if (this.editUrl != (Uri) null)
        return this.editUrl;
      if (this.resource.HasNonComputedEditLink)
        return this.editUrl = this.resource.NonComputedEditLink;
      Uri canonicalUrl = this.GetCanonicalUrl();
      if (canonicalUrl != (Uri) null)
      {
        this.editUrl = canonicalUrl;
      }
      else
      {
        ODataConventionalResourceMetadataBuilder parentMetadataBuilder = this.ParentMetadataBuilder as ODataConventionalResourceMetadataBuilder;
        if (this.NameAsProperty != null && parentMetadataBuilder != null && parentMetadataBuilder.GetEditUrl() != (Uri) null)
        {
          if (parentMetadataBuilder.IsFromCollection && !(parentMetadataBuilder is ODataConventionalEntityMetadataBuilder))
            return this.editUrl = (Uri) null;
          this.editUrl = new Uri(parentMetadataBuilder.GetEditUrl().ToString() + "/" + this.NameAsProperty, UriKind.RelativeOrAbsolute);
        }
      }
      if (this.editUrl != (Uri) null && this.ResourceMetadataContext.ActualResourceTypeName != this.ResourceMetadataContext.TypeContext.ExpectedResourceTypeName)
        this.editUrl = this.UriBuilder.AppendTypeSegment(this.editUrl, this.ResourceMetadataContext.ActualResourceTypeName);
      return this.editUrl;
    }

    public virtual Uri GetReadUrl()
    {
      if (this.readUrl != (Uri) null)
        return this.readUrl;
      if (this.resource.HasNonComputedReadLink)
        return this.readUrl = this.resource.NonComputedReadLink;
      Uri editUrl = this.GetEditUrl();
      if (editUrl != (Uri) null)
        return this.readUrl = editUrl;
      ODataConventionalResourceMetadataBuilder parentMetadataBuilder = this.ParentMetadataBuilder as ODataConventionalResourceMetadataBuilder;
      if (this.NameAsProperty != null && parentMetadataBuilder != null && parentMetadataBuilder.GetReadUrl() != (Uri) null)
      {
        if (parentMetadataBuilder.IsFromCollection && !(parentMetadataBuilder is ODataConventionalEntityMetadataBuilder))
          return this.readUrl = (Uri) null;
        this.readUrl = new Uri(parentMetadataBuilder.GetReadUrl().ToString() + "/" + this.NameAsProperty, UriKind.RelativeOrAbsolute);
        if (this.ResourceMetadataContext.ActualResourceTypeName != this.ResourceMetadataContext.TypeContext.ExpectedResourceTypeName)
          this.readUrl = this.UriBuilder.AppendTypeSegment(this.readUrl, this.ResourceMetadataContext.ActualResourceTypeName);
      }
      return this.readUrl;
    }

    public virtual Uri GetCanonicalUrl()
    {
      if (this.canonicalUrl != (Uri) null)
        return this.canonicalUrl;
      if (this.resource.HasNonComputedId)
        return this.canonicalUrl = this.resource.NonComputedId;
      ODataConventionalResourceMetadataBuilder parentMetadataBuilder = this.ParentMetadataBuilder as ODataConventionalResourceMetadataBuilder;
      if (this.NameAsProperty != null && parentMetadataBuilder != null && parentMetadataBuilder.GetCanonicalUrl() != (Uri) null)
      {
        if (parentMetadataBuilder.IsFromCollection && !(parentMetadataBuilder is ODataConventionalEntityMetadataBuilder))
          return this.canonicalUrl = (Uri) null;
        this.canonicalUrl = parentMetadataBuilder.GetCanonicalUrl();
        IODataResourceTypeContext typeContext = parentMetadataBuilder.ResourceMetadataContext.TypeContext;
        if (parentMetadataBuilder.ResourceMetadataContext.ActualResourceTypeName != typeContext.ExpectedResourceTypeName && (!(typeContext is ODataResourceTypeContext.ODataResourceTypeContextWithModel contextWithModel) || contextWithModel.ExpectedResourceType.FindProperty(this.NameAsProperty) == null))
          this.canonicalUrl = this.UriBuilder.AppendTypeSegment(this.canonicalUrl, parentMetadataBuilder.ResourceMetadataContext.ActualResourceTypeName);
        this.canonicalUrl = new Uri(this.canonicalUrl.ToString() + "/" + this.NameAsProperty, UriKind.RelativeOrAbsolute);
      }
      else if (this.ODataUri != null && this.ODataUri.Path.Count != 0)
        this.canonicalUrl = this.ODataUri.BuildUri(ODataUrlKeyDelimiter.Parentheses);
      return this.canonicalUrl;
    }

    internal virtual void StartResource()
    {
    }

    internal virtual void EndResource()
    {
    }

    internal override Uri GetEditLink() => this.resource.NonComputedEditLink;

    internal override Uri GetReadLink() => this.resource.NonComputedReadLink;

    internal override Uri GetId() => !this.resource.IsTransient ? this.resource.NonComputedId : (Uri) null;

    internal override bool TryGetIdForSerialization(out Uri id)
    {
      if (this.resource.IsTransient)
      {
        id = (Uri) null;
        return true;
      }
      id = this.GetId();
      return id != (Uri) null;
    }

    internal override string GetETag() => this.resource.NonComputedETag;

    internal override IEnumerable<ODataProperty> GetProperties(
      IEnumerable<ODataProperty> nonComputedProperties)
    {
      return nonComputedProperties;
    }

    [SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate", Justification = "A method for consistency with the rest of the API.")]
    internal override IEnumerable<ODataAction> GetActions() => this.resource.NonComputedActions;

    [SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate", Justification = "A method for consistency with the rest of the API.")]
    internal override IEnumerable<ODataFunction> GetFunctions() => this.resource.NonComputedFunctions;

    internal override void MarkNestedResourceInfoProcessed(string navigationPropertyName) => this.ProcessedNestedResourceInfos.Add(navigationPropertyName);

    [SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate", Justification = "A method for consistency with the rest of the API.")]
    internal override ODataJsonLightReaderNestedResourceInfo GetNextUnprocessedNavigationLink()
    {
      if (this.ResourceMetadataContext.Resource.IsTransient)
        return (ODataJsonLightReaderNestedResourceInfo) null;
      if (this.unprocessedNavigationLinks == null)
        this.unprocessedNavigationLinks = this.ResourceMetadataContext.SelectedNavigationProperties.Where<IEdmNavigationProperty>((Func<IEdmNavigationProperty, bool>) (p => !this.ProcessedNestedResourceInfos.Contains(p.Name))).Select<IEdmNavigationProperty, ODataJsonLightReaderNestedResourceInfo>(new Func<IEdmNavigationProperty, ODataJsonLightReaderNestedResourceInfo>(ODataJsonLightReaderNestedResourceInfo.CreateProjectedNestedResourceInfo)).GetEnumerator();
      return this.unprocessedNavigationLinks.MoveNext() ? this.unprocessedNavigationLinks.Current : (ODataJsonLightReaderNestedResourceInfo) null;
    }

    internal override void MarkStreamPropertyProcessed(string streamPropertyName) => this.ProcessedStreamProperties.Add(streamPropertyName);

    [SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate", Justification = "A method for consistency with the rest of the API.")]
    internal override ODataProperty GetNextUnprocessedStreamProperty()
    {
      if (this.unprocessedStreamProperties == null)
        this.unprocessedStreamProperties = this.ResourceMetadataContext.SelectedStreamProperties.Where<KeyValuePair<string, IEdmStructuralProperty>>((Func<KeyValuePair<string, IEdmStructuralProperty>, bool>) (p => !this.ProcessedStreamProperties.Contains(p.Key))).Select<KeyValuePair<string, IEdmStructuralProperty>, string>((Func<KeyValuePair<string, IEdmStructuralProperty>, string>) (p => p.Key)).GetEnumerator();
      if (!this.unprocessedStreamProperties.MoveNext())
        return (ODataProperty) null;
      string current = this.unprocessedStreamProperties.Current;
      ODataStreamReferenceValue streamReferenceValue = new ODataStreamReferenceValue();
      streamReferenceValue.SetMetadataBuilder((ODataResourceMetadataBuilder) this, current);
      ODataProperty unprocessedStreamProperty = new ODataProperty();
      unprocessedStreamProperty.Name = current;
      unprocessedStreamProperty.Value = (object) streamReferenceValue;
      return unprocessedStreamProperty;
    }

    internal override Uri GetNavigationLinkUri(
      string navigationPropertyName,
      Uri navigationLinkUrl,
      bool hasNestedResourceInfoUrl)
    {
      ExceptionUtils.CheckArgumentStringNotNullOrEmpty(navigationPropertyName, nameof (navigationPropertyName));
      Uri readUrl = this.GetReadUrl();
      if (readUrl == (Uri) null || this.IsFromCollection)
        return (Uri) null;
      return !hasNestedResourceInfoUrl ? this.UriBuilder.BuildNavigationLinkUri(readUrl, navigationPropertyName) : navigationLinkUrl;
    }

    internal override Uri GetAssociationLinkUri(
      string navigationPropertyName,
      Uri associationLinkUrl,
      bool hasAssociationLinkUrl)
    {
      ExceptionUtils.CheckArgumentStringNotNullOrEmpty(navigationPropertyName, nameof (navigationPropertyName));
      Uri readUrl = this.GetReadUrl();
      if (readUrl == (Uri) null || this.IsFromCollection)
        return (Uri) null;
      return !hasAssociationLinkUrl ? this.UriBuilder.BuildAssociationLinkUri(readUrl, navigationPropertyName) : associationLinkUrl;
    }

    internal override Uri GetStreamEditLink(string streamPropertyName)
    {
      ExceptionUtils.CheckArgumentStringNotEmpty(streamPropertyName, nameof (streamPropertyName));
      return this.UriBuilder.BuildStreamEditLinkUri(this.GetEditUrl(), streamPropertyName);
    }

    internal override Uri GetStreamReadLink(string streamPropertyName)
    {
      ExceptionUtils.CheckArgumentStringNotEmpty(streamPropertyName, nameof (streamPropertyName));
      return this.UriBuilder.BuildStreamReadLinkUri(this.GetReadUrl(), streamPropertyName);
    }

    internal override Uri GetOperationTargetUri(
      string operationName,
      string bindingParameterTypeName,
      string parameterNames)
    {
      ExceptionUtils.CheckArgumentStringNotNullOrEmpty(operationName, nameof (operationName));
      return this.UriBuilder.BuildOperationTargetUri(string.IsNullOrEmpty(bindingParameterTypeName) || this.ResourceMetadataContext.Resource.NonComputedEditLink != (Uri) null ? this.GetEditLink() : this.GetId(), operationName, bindingParameterTypeName, parameterNames);
    }

    internal override string GetOperationTitle(string operationName)
    {
      ExceptionUtils.CheckArgumentStringNotNullOrEmpty(operationName, nameof (operationName));
      return operationName;
    }
  }
}
