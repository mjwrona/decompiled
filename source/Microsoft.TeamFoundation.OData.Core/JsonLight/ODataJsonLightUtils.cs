// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.JsonLight.ODataJsonLightUtils
// Assembly: Microsoft.TeamFoundation.OData.Core, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6619C7F6-E44A-4143-AE77-6D570F968D9A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Core.dll

using Microsoft.OData.Edm;
using Microsoft.OData.Metadata;
using System;
using System.Linq;

namespace Microsoft.OData.JsonLight
{
  internal static class ODataJsonLightUtils
  {
    private static readonly char[] CharactersToTrimFromParameters = new char[2]
    {
      '(',
      ')'
    };

    internal static bool IsMetadataReferenceProperty(string propertyName) => propertyName.IndexOf('#') >= 0;

    internal static string GetFullyQualifiedOperationName(
      Uri metadataDocumentUri,
      string metadataReferencePropertyName,
      out string parameterNames)
    {
      string qualifiedOperationName = ODataJsonLightUtils.GetUriFragmentFromMetadataReferencePropertyName(metadataDocumentUri, metadataReferencePropertyName);
      parameterNames = (string) null;
      int length = qualifiedOperationName.IndexOf('(');
      if (length > -1)
      {
        string str = qualifiedOperationName.Substring(length + 1);
        qualifiedOperationName = qualifiedOperationName.Substring(0, length);
        parameterNames = str.Trim(ODataJsonLightUtils.CharactersToTrimFromParameters);
      }
      return qualifiedOperationName;
    }

    internal static string GetUriFragmentFromMetadataReferencePropertyName(
      Uri metadataDocumentUri,
      string propertyName)
    {
      return ODataJsonLightUtils.GetAbsoluteUriFromMetadataReferencePropertyName(metadataDocumentUri, propertyName).GetComponents(UriComponents.Fragment, UriFormat.Unescaped);
    }

    internal static Uri GetAbsoluteUriFromMetadataReferencePropertyName(
      Uri metadataDocumentUri,
      string propertyName)
    {
      if (propertyName[0] != '#')
        return new Uri(propertyName, UriKind.Absolute);
      propertyName = UriUtils.EnsureEscapedFragment(propertyName);
      return new Uri(metadataDocumentUri, propertyName);
    }

    internal static string GetMetadataReferenceName(IEdmModel model, IEdmOperation operation)
    {
      string metadataReferenceName = operation.FullName();
      if (model.FindDeclaredOperations(operation.FullName()).Take<IEdmOperation>(2).Count<IEdmOperation>() > 1 && operation is IEdmFunction)
        metadataReferenceName = operation.FullNameWithNonBindingParameters();
      return metadataReferenceName;
    }

    internal static ODataOperation CreateODataOperation(
      Uri metadataDocumentUri,
      string metadataReferencePropertyName,
      IEdmOperation edmOperation,
      out bool isAction)
    {
      isAction = edmOperation.IsAction();
      ODataOperation odataOperation = isAction ? (ODataOperation) new ODataAction() : (ODataOperation) new ODataFunction();
      int length;
      if (isAction && (length = metadataReferencePropertyName.IndexOf('(')) > 0)
        metadataReferencePropertyName = metadataReferencePropertyName.Substring(0, length);
      odataOperation.Metadata = ODataJsonLightUtils.GetAbsoluteUriFromMetadataReferencePropertyName(metadataDocumentUri, metadataReferencePropertyName);
      return odataOperation;
    }
  }
}
