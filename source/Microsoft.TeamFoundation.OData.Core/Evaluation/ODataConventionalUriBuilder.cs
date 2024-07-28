// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.Evaluation.ODataConventionalUriBuilder
// Assembly: Microsoft.TeamFoundation.OData.Core, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6619C7F6-E44A-4143-AE77-6D570F968D9A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Core.dll

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace Microsoft.OData.Evaluation
{
  internal sealed class ODataConventionalUriBuilder : ODataUriBuilder
  {
    private readonly Uri serviceBaseUri;
    private readonly KeySerializer keySerializer;

    internal ODataConventionalUriBuilder(Uri serviceBaseUri, ODataUrlKeyDelimiter urlKeyDelimiter)
    {
      this.serviceBaseUri = serviceBaseUri;
      this.keySerializer = KeySerializer.Create(urlKeyDelimiter.EnableKeyAsSegment);
    }

    internal override Uri BuildBaseUri() => this.serviceBaseUri;

    internal override Uri BuildEntitySetUri(Uri baseUri, string entitySetName)
    {
      ExceptionUtils.CheckArgumentStringNotNullOrEmpty(entitySetName, nameof (entitySetName));
      return ODataConventionalUriBuilder.AppendSegment(baseUri, entitySetName, true);
    }

    internal override Uri BuildEntityInstanceUri(
      Uri baseUri,
      ICollection<KeyValuePair<string, object>> keyProperties,
      string entityTypeName)
    {
      StringBuilder builder = new StringBuilder(UriUtils.UriToString(baseUri));
      this.AppendKeyExpression(builder, keyProperties, entityTypeName);
      return new Uri(builder.ToString(), UriKind.Absolute);
    }

    internal override Uri BuildStreamEditLinkUri(Uri baseUri, string streamPropertyName)
    {
      ExceptionUtils.CheckArgumentStringNotEmpty(streamPropertyName, nameof (streamPropertyName));
      return streamPropertyName == null ? ODataConventionalUriBuilder.AppendSegment(baseUri, "$value", false) : ODataConventionalUriBuilder.AppendSegment(baseUri, streamPropertyName, true);
    }

    internal override Uri BuildStreamReadLinkUri(Uri baseUri, string streamPropertyName)
    {
      ExceptionUtils.CheckArgumentStringNotEmpty(streamPropertyName, nameof (streamPropertyName));
      return streamPropertyName == null ? ODataConventionalUriBuilder.AppendSegment(baseUri, "$value", false) : ODataConventionalUriBuilder.AppendSegment(baseUri, streamPropertyName, true);
    }

    internal override Uri BuildNavigationLinkUri(Uri baseUri, string navigationPropertyName)
    {
      ExceptionUtils.CheckArgumentStringNotNullOrEmpty(navigationPropertyName, nameof (navigationPropertyName));
      return ODataConventionalUriBuilder.AppendSegment(baseUri, navigationPropertyName, true);
    }

    internal override Uri BuildAssociationLinkUri(Uri baseUri, string navigationPropertyName)
    {
      ExceptionUtils.CheckArgumentStringNotNullOrEmpty(navigationPropertyName, nameof (navigationPropertyName));
      return ODataConventionalUriBuilder.AppendSegment(ODataConventionalUriBuilder.AppendSegment(baseUri, navigationPropertyName, true), "$ref", false);
    }

    internal override Uri BuildOperationTargetUri(
      Uri baseUri,
      string operationName,
      string bindingParameterTypeName,
      string parameterNames)
    {
      ExceptionUtils.CheckArgumentStringNotNullOrEmpty(operationName, nameof (operationName));
      Uri baseUri1 = baseUri;
      if (!string.IsNullOrEmpty(bindingParameterTypeName))
        baseUri1 = ODataConventionalUriBuilder.AppendSegment(baseUri, bindingParameterTypeName, true);
      if (!string.IsNullOrEmpty(parameterNames))
      {
        operationName += "(";
        operationName += string.Join(",", ((IEnumerable<string>) parameterNames.Split(',')).Select<string, string>((Func<string, string>) (p => p + "=@" + p)).ToArray<string>());
        operationName += ")";
      }
      return ODataConventionalUriBuilder.AppendSegment(baseUri1, operationName, false);
    }

    internal override Uri AppendTypeSegment(Uri baseUri, string typeName)
    {
      ExceptionUtils.CheckArgumentStringNotNullOrEmpty(typeName, nameof (typeName));
      return ODataConventionalUriBuilder.AppendSegment(baseUri, typeName, true);
    }

    [Conditional("DEBUG")]
    private static void ValidateBaseUri(Uri baseUri)
    {
    }

    private static Uri AppendSegment(Uri baseUri, string segment, bool escapeSegment)
    {
      string str = UriUtils.UriToString(baseUri);
      if (escapeSegment)
        segment = Uri.EscapeDataString(segment);
      return str[str.Length - 1] != '/' ? new Uri(str + "/" + segment, UriKind.RelativeOrAbsolute) : new Uri(baseUri, segment);
    }

    private static object ValidateKeyValue(
      string keyPropertyName,
      object keyPropertyValue,
      string entityTypeName)
    {
      return keyPropertyValue != null ? keyPropertyValue : throw new ODataException(Strings.ODataConventionalUriBuilder_NullKeyValue((object) keyPropertyName, (object) entityTypeName));
    }

    private void AppendKeyExpression(
      StringBuilder builder,
      ICollection<KeyValuePair<string, object>> keyProperties,
      string entityTypeName)
    {
      if (!keyProperties.Any<KeyValuePair<string, object>>())
        throw new ODataException(Strings.ODataConventionalUriBuilder_EntityTypeWithNoKeyProperties((object) entityTypeName));
      this.keySerializer.AppendKeyExpression<KeyValuePair<string, object>>(builder, keyProperties, (Func<KeyValuePair<string, object>, string>) (p => p.Key), (Func<KeyValuePair<string, object>, object>) (p => ODataConventionalUriBuilder.ValidateKeyValue(p.Key, p.Value, entityTypeName)));
    }
  }
}
