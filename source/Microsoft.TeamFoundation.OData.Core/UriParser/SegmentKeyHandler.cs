// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.UriParser.SegmentKeyHandler
// Assembly: Microsoft.TeamFoundation.OData.Core, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6619C7F6-E44A-4143-AE77-6D570F968D9A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Core.dll

using Microsoft.OData.Edm;
using Microsoft.OData.Metadata;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.OData.UriParser
{
  internal static class SegmentKeyHandler
  {
    internal static bool TryCreateKeySegmentFromParentheses(
      ODataPathSegment previous,
      KeySegment previousKeySegment,
      string parenthesisExpression,
      ODataUriResolver resolver,
      out ODataPathSegment keySegment,
      bool enableUriTemplateParsing = false)
    {
      if (previous.SingleResult)
        throw ExceptionUtil.CreateSyntaxError();
      SegmentArgumentParser instance;
      if (!SegmentArgumentParser.TryParseKeysFromUri(parenthesisExpression, out instance, enableUriTemplateParsing))
        throw ExceptionUtil.CreateSyntaxError();
      if (instance.IsEmpty)
      {
        keySegment = (ODataPathSegment) null;
        return false;
      }
      keySegment = (ODataPathSegment) SegmentKeyHandler.CreateKeySegment(previous, previousKeySegment, instance, resolver);
      return true;
    }

    internal static bool TryHandleSegmentAsKey(
      string segmentText,
      ODataPathSegment previous,
      KeySegment previousKeySegment,
      ODataUrlKeyDelimiter odataUrlKeyDelimiter,
      ODataUriResolver resolver,
      out KeySegment keySegment,
      bool enableUriTemplateParsing = false)
    {
      keySegment = (KeySegment) null;
      if (!odataUrlKeyDelimiter.EnableKeyAsSegment || previous.SingleResult || SegmentKeyHandler.IsSystemSegment(segmentText) || previous.TargetEdmType == null || !previous.TargetEdmType.IsEntityOrEntityCollectionType(out IEdmEntityType _))
        return false;
      keySegment = SegmentKeyHandler.CreateKeySegment(previous, previousKeySegment, SegmentArgumentParser.FromSegment(segmentText, enableUriTemplateParsing), resolver);
      return true;
    }

    private static bool IsSystemSegment(string segmentText)
    {
      if (segmentText[0] != '$')
        return false;
      return segmentText.Length < 2 || segmentText[1] != '$';
    }

    private static KeySegment CreateKeySegment(
      ODataPathSegment segment,
      KeySegment previousKeySegment,
      SegmentArgumentParser key,
      ODataUriResolver resolver)
    {
      IEdmEntityType entityType = (IEdmEntityType) null;
      if (segment.TargetEdmType == null || !segment.TargetEdmType.IsEntityOrEntityCollectionType(out entityType))
        throw ExceptionUtil.CreateSyntaxError();
      List<IEdmStructuralProperty> list = entityType.Key().ToList<IEdmStructuralProperty>();
      if (list.Count != key.ValueCount && segment is NavigationPropertySegment navigationPropertySegment)
        key = KeyFinder.FindAndUseKeysFromRelatedSegment(key, (IEnumerable<IEdmStructuralProperty>) list, navigationPropertySegment.NavigationProperty, previousKeySegment);
      if (!key.AreValuesNamed && key.ValueCount > 1 && resolver.GetType() == typeof (ODataUriResolver))
        throw ExceptionUtil.CreateBadRequestError(Microsoft.OData.Strings.RequestUriProcessor_KeysMustBeNamed);
      IEnumerable<KeyValuePair<string, object>> keyPairs;
      if (!key.TryConvertValues(entityType, out keyPairs, resolver))
        throw ExceptionUtil.CreateSyntaxError();
      return new KeySegment(segment, keyPairs, entityType, segment.TargetEdmNavigationSource);
    }
  }
}
