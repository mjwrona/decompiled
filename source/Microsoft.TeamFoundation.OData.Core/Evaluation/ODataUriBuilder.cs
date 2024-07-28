// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.Evaluation.ODataUriBuilder
// Assembly: Microsoft.TeamFoundation.OData.Core, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6619C7F6-E44A-4143-AE77-6D570F968D9A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Core.dll

using System;
using System.Collections.Generic;

namespace Microsoft.OData.Evaluation
{
  internal abstract class ODataUriBuilder
  {
    internal virtual Uri BuildBaseUri() => (Uri) null;

    internal virtual Uri BuildEntitySetUri(Uri baseUri, string entitySetName)
    {
      ExceptionUtils.CheckArgumentStringNotNullOrEmpty(entitySetName, nameof (entitySetName));
      return (Uri) null;
    }

    internal virtual Uri BuildEntityInstanceUri(
      Uri baseUri,
      ICollection<KeyValuePair<string, object>> keyProperties,
      string entityTypeName)
    {
      ExceptionUtils.CheckArgumentNotNull<ICollection<KeyValuePair<string, object>>>(keyProperties, nameof (keyProperties));
      ExceptionUtils.CheckArgumentStringNotNullOrEmpty(entityTypeName, nameof (entityTypeName));
      return (Uri) null;
    }

    internal virtual Uri BuildStreamEditLinkUri(Uri baseUri, string streamPropertyName)
    {
      ExceptionUtils.CheckArgumentStringNotEmpty(streamPropertyName, nameof (streamPropertyName));
      return (Uri) null;
    }

    internal virtual Uri BuildStreamReadLinkUri(Uri baseUri, string streamPropertyName)
    {
      ExceptionUtils.CheckArgumentStringNotEmpty(streamPropertyName, nameof (streamPropertyName));
      return (Uri) null;
    }

    internal virtual Uri BuildNavigationLinkUri(Uri baseUri, string navigationPropertyName)
    {
      ExceptionUtils.CheckArgumentStringNotNullOrEmpty(navigationPropertyName, nameof (navigationPropertyName));
      return (Uri) null;
    }

    internal virtual Uri BuildAssociationLinkUri(Uri baseUri, string navigationPropertyName)
    {
      ExceptionUtils.CheckArgumentStringNotNullOrEmpty(navigationPropertyName, nameof (navigationPropertyName));
      return (Uri) null;
    }

    internal virtual Uri BuildOperationTargetUri(
      Uri baseUri,
      string operationName,
      string bindingParameterTypeName,
      string parameterNames)
    {
      ExceptionUtils.CheckArgumentStringNotNullOrEmpty(operationName, nameof (operationName));
      return (Uri) null;
    }

    internal virtual Uri AppendTypeSegment(Uri baseUri, string typeName)
    {
      ExceptionUtils.CheckArgumentStringNotNullOrEmpty(typeName, nameof (typeName));
      return (Uri) null;
    }
  }
}
