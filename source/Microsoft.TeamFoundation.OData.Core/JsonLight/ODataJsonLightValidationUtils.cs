// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.JsonLight.ODataJsonLightValidationUtils
// Assembly: Microsoft.TeamFoundation.OData.Core, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6619C7F6-E44A-4143-AE77-6D570F968D9A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Core.dll

using System;

namespace Microsoft.OData.JsonLight
{
  internal static class ODataJsonLightValidationUtils
  {
    internal static void ValidateMetadataReferencePropertyName(
      Uri metadataDocumentUri,
      string propertyName)
    {
      string uriString = propertyName;
      if (propertyName[0] == '#')
        uriString = UriUtils.UriToString(metadataDocumentUri) + UriUtils.EnsureEscapedFragment(propertyName);
      if (!Uri.IsWellFormedUriString(uriString, UriKind.Absolute) || !ODataJsonLightUtils.IsMetadataReferenceProperty(propertyName) || propertyName[propertyName.Length - 1] == '#')
        throw new ODataException(Strings.ValidationUtils_InvalidMetadataReferenceProperty((object) propertyName));
      if (ODataJsonLightValidationUtils.IsOpenMetadataReferencePropertyName(metadataDocumentUri, propertyName))
        throw new ODataException(Strings.ODataJsonLightValidationUtils_OpenMetadataReferencePropertyNotSupported((object) propertyName, (object) UriUtils.UriToString(metadataDocumentUri)));
    }

    internal static void ValidateOperation(Uri metadataDocumentUri, ODataOperation operation)
    {
      ValidationUtils.ValidateOperationMetadataNotNull(operation);
      string propertyName = UriUtils.UriToString(operation.Metadata);
      if (!(metadataDocumentUri != (Uri) null))
        return;
      ODataJsonLightValidationUtils.ValidateMetadataReferencePropertyName(metadataDocumentUri, propertyName);
    }

    internal static bool IsOpenMetadataReferencePropertyName(
      Uri metadataDocumentUri,
      string propertyName)
    {
      return ODataJsonLightUtils.IsMetadataReferenceProperty(propertyName) && propertyName[0] != '#' && !propertyName.StartsWith(UriUtils.UriToString(metadataDocumentUri), StringComparison.OrdinalIgnoreCase);
    }

    internal static void ValidateOperationPropertyValueIsNotNull(
      object propertyValue,
      string propertyName,
      string metadata)
    {
      if (propertyValue == null)
        throw new ODataException(Strings.ODataJsonLightValidationUtils_OperationPropertyCannotBeNull((object) propertyName, (object) metadata));
    }
  }
}
