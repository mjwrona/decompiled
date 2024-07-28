// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.JsonLight.ODataAnnotationNames
// Assembly: Microsoft.TeamFoundation.OData.Core, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6619C7F6-E44A-4143-AE77-6D570F968D9A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Core.dll

using System;
using System.Collections.Generic;

namespace Microsoft.OData.JsonLight
{
  internal static class ODataAnnotationNames
  {
    internal static readonly HashSet<string> KnownODataAnnotationNames = new HashSet<string>((IEnumerable<string>) new string[19]
    {
      "odata.context",
      "odata.type",
      "odata.id",
      "odata.etag",
      "odata.editLink",
      "odata.readLink",
      "odata.mediaEditLink",
      "odata.mediaReadLink",
      "odata.mediaContentType",
      "odata.mediaEtag",
      "odata.count",
      "odata.nextLink",
      "odata.bind",
      "odata.associationLink",
      "odata.navigationLink",
      "odata.deltaLink",
      "odata.removed",
      "odata.delta",
      "odata.null"
    }, (IEqualityComparer<string>) StringComparer.Ordinal);
    internal const string ODataContext = "odata.context";
    internal const string ODataType = "odata.type";
    internal const string ODataId = "odata.id";
    internal const string ODataETag = "odata.etag";
    internal const string ODataEditLink = "odata.editLink";
    internal const string ODataReadLink = "odata.readLink";
    internal const string ODataMediaEditLink = "odata.mediaEditLink";
    internal const string ODataMediaReadLink = "odata.mediaReadLink";
    internal const string ODataMediaContentType = "odata.mediaContentType";
    internal const string ODataMediaETag = "odata.mediaEtag";
    internal const string ODataCount = "odata.count";
    internal const string ODataNextLink = "odata.nextLink";
    internal const string ODataNavigationLinkUrl = "odata.navigationLink";
    internal const string ODataBind = "odata.bind";
    internal const string ODataAssociationLinkUrl = "odata.associationLink";
    internal const string ODataDeltaLink = "odata.deltaLink";
    internal const string ODataRemoved = "odata.removed";
    internal const string ODataDelta = "odata.delta";
    internal const string ODataNull = "odata.null";

    internal static bool IsODataAnnotationName(string annotationName) => annotationName.StartsWith("odata.", StringComparison.Ordinal);

    internal static bool IsUnknownODataAnnotationName(string annotationName) => ODataAnnotationNames.IsODataAnnotationName(annotationName) && !ODataAnnotationNames.KnownODataAnnotationNames.Contains(annotationName);

    internal static void ValidateIsCustomAnnotationName(string annotationName)
    {
      if (ODataAnnotationNames.KnownODataAnnotationNames.Contains(annotationName))
        throw new ODataException(Strings.ODataJsonLightPropertyAndValueDeserializer_UnexpectedAnnotationProperties((object) annotationName));
    }

    internal static string RemoveAnnotationPrefix(string annotationName) => !string.IsNullOrEmpty(annotationName) && annotationName[0] == '@' ? annotationName.Substring(1) : annotationName;
  }
}
