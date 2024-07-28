// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.Evaluation.ODataResourceMetadataBuilder
// Assembly: Microsoft.TeamFoundation.OData.Core, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6619C7F6-E44A-4143-AE77-6D570F968D9A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Core.dll

using Microsoft.OData.JsonLight;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace Microsoft.OData.Evaluation
{
  internal abstract class ODataResourceMetadataBuilder
  {
    internal static ODataResourceMetadataBuilder Null => (ODataResourceMetadataBuilder) ODataResourceMetadataBuilder.NullResourceMetadataBuilder.Instance;

    internal ODataResourceMetadataBuilder ParentMetadataBuilder { get; set; }

    internal bool IsFromCollection { get; set; }

    internal string NameAsProperty { get; set; }

    [SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate", Justification = "A method for consistency with the rest of the API.")]
    internal abstract Uri GetEditLink();

    [SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate", Justification = "A method for consistency with the rest of the API.")]
    internal abstract Uri GetReadLink();

    [SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate", Justification = "A method for consistency with the rest of the API.")]
    internal abstract Uri GetId();

    internal abstract bool TryGetIdForSerialization(out Uri id);

    [SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate", Justification = "A method for consistency with the rest of the API.")]
    internal abstract string GetETag();

    [SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate", Justification = "A method for consistency with the rest of the API.")]
    internal virtual ODataStreamReferenceValue GetMediaResource() => (ODataStreamReferenceValue) null;

    internal virtual IEnumerable<ODataProperty> GetProperties(
      IEnumerable<ODataProperty> nonComputedProperties)
    {
      return nonComputedProperties != null ? nonComputedProperties.Where<ODataProperty>((Func<ODataProperty, bool>) (p =>
      {
        if (p.ODataValue is ODataStreamReferenceValue)
          return false;
        if (p.ODataValue is ODataResourceValue)
          throw new ODataException(Strings.ODataResource_PropertyValueCannotBeODataResourceValue((object) p.Name));
        if (p.ODataValue is ODataCollectionValue odataValue2 && odataValue2.Items != null && odataValue2.Items.Any<object>((Func<object, bool>) (t => t is ODataResourceValue)))
          throw new ODataException(Strings.ODataResource_PropertyValueCannotBeODataResourceValue((object) p.Name));
        return true;
      })) : (IEnumerable<ODataProperty>) null;
    }

    [SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate", Justification = "A method for consistency with the rest of the API.")]
    internal virtual IEnumerable<ODataAction> GetActions() => (IEnumerable<ODataAction>) null;

    [SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate", Justification = "A method for consistency with the rest of the API.")]
    internal virtual IEnumerable<ODataFunction> GetFunctions() => (IEnumerable<ODataFunction>) null;

    internal virtual void MarkNestedResourceInfoProcessed(string navigationPropertyName)
    {
    }

    [SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate", Justification = "A method for consistency with the rest of the API.")]
    internal virtual ODataProperty GetNextUnprocessedStreamProperty() => (ODataProperty) null;

    internal virtual void MarkStreamPropertyProcessed(string streamPropertyName)
    {
    }

    [SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate", Justification = "A method for consistency with the rest of the API.")]
    internal virtual ODataJsonLightReaderNestedResourceInfo GetNextUnprocessedNavigationLink() => (ODataJsonLightReaderNestedResourceInfo) null;

    internal virtual Uri GetStreamEditLink(string streamPropertyName)
    {
      ExceptionUtils.CheckArgumentStringNotEmpty(streamPropertyName, nameof (streamPropertyName));
      return (Uri) null;
    }

    internal virtual Uri GetStreamReadLink(string streamPropertyName)
    {
      ExceptionUtils.CheckArgumentStringNotEmpty(streamPropertyName, nameof (streamPropertyName));
      return (Uri) null;
    }

    internal virtual Uri GetNavigationLinkUri(
      string navigationPropertyName,
      Uri navigationLinkUrl,
      bool hasNestedResourceInfoUrl)
    {
      ExceptionUtils.CheckArgumentStringNotNullOrEmpty(navigationPropertyName, nameof (navigationPropertyName));
      return (Uri) null;
    }

    internal virtual Uri GetAssociationLinkUri(
      string navigationPropertyName,
      Uri associationLinkUrl,
      bool hasAssociationLinkUrl)
    {
      ExceptionUtils.CheckArgumentStringNotNullOrEmpty(navigationPropertyName, nameof (navigationPropertyName));
      return (Uri) null;
    }

    internal virtual Uri GetOperationTargetUri(
      string operationName,
      string bindingParameterTypeName,
      string parameterNames)
    {
      ExceptionUtils.CheckArgumentStringNotNullOrEmpty(operationName, nameof (operationName));
      return (Uri) null;
    }

    internal virtual string GetOperationTitle(string operationName)
    {
      ExceptionUtils.CheckArgumentStringNotNullOrEmpty(operationName, nameof (operationName));
      return (string) null;
    }

    private sealed class NullResourceMetadataBuilder : ODataResourceMetadataBuilder
    {
      internal static readonly ODataResourceMetadataBuilder.NullResourceMetadataBuilder Instance = new ODataResourceMetadataBuilder.NullResourceMetadataBuilder();

      private NullResourceMetadataBuilder()
      {
      }

      internal override Uri GetEditLink() => (Uri) null;

      internal override Uri GetReadLink() => (Uri) null;

      internal override Uri GetId() => (Uri) null;

      internal override string GetETag() => (string) null;

      internal override bool TryGetIdForSerialization(out Uri id)
      {
        id = (Uri) null;
        return false;
      }
    }
  }
}
