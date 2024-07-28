// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.ODataConstants
// Assembly: Microsoft.TeamFoundation.OData.Core, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6619C7F6-E44A-4143-AE77-6D570F968D9A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Core.dll

using System.Diagnostics.CodeAnalysis;

namespace Microsoft.OData
{
  public static class ODataConstants
  {
    public const string MethodGet = "GET";
    public const string MethodPost = "POST";
    public const string MethodPut = "PUT";
    public const string MethodDelete = "DELETE";
    public const string MethodPatch = "PATCH";
    public const string ContentTypeHeader = "Content-Type";
    public const string ODataVersionHeader = "OData-Version";
    public const string ContentIdHeader = "Content-ID";
    public const string OmitValuesNulls = "nulls";
    internal const string OmitValuesDefaults = "defaults";
    internal const string ContentLengthHeader = "Content-Length";
    internal const string HttpQValueParameter = "q";
    internal const string HttpVersionInBatching = "HTTP/1.1";
    internal const string HttpVersionInAsync = "HTTP/1.1";
    [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Charset", Justification = "Member name chosen based on HTTP header name.")]
    internal const string Charset = "charset";
    internal const string HttpMultipartBoundary = "boundary";
    internal const string ContentTransferEncoding = "Content-Transfer-Encoding";
    internal const string BatchContentTransferEncoding = "binary";
    internal const ODataVersion ODataDefaultProtocolVersion = ODataVersion.V4;
    internal const string BatchRequestBoundaryTemplate = "batch_{0}";
    internal const string BatchResponseBoundaryTemplate = "batchresponse_{0}";
    internal const string RequestChangeSetBoundaryTemplate = "changeset_{0}";
    internal const string ResponseChangeSetBoundaryTemplate = "changesetresponse_{0}";
    internal const string HttpWeakETagPrefix = "W/\"";
    internal const string HttpWeakETagSuffix = "\"";
    internal const int DefaultMaxRecursionDepth = 100;
    internal const long DefaultMaxReadMessageSize = 1048576;
    internal const int DefaultMaxPartsPerBatch = 100;
    internal const int DefaultMaxOperationsPerChangeset = 1000;
    internal const string UriSegmentSeparator = "/";
    internal const char UriSegmentSeparatorChar = '/';
    internal const string EntityReferenceSegmentName = "$ref";
    internal const string CollectionPrefix = "Collection";
    internal const string DefaultStreamSegmentName = "$value";
    internal const string TypeNamePrefix = "#";
    internal const string UriMetadataSegment = "$metadata";
    internal const string ODataPrefix = "odata";
    internal const string CollectionOfEntityReferencesContextUrlSegment = "#Collection($ref)";
    internal const string SingleEntityReferencesContextUrlSegment = "#$ref";
    internal const char ContextUriFragmentIndicator = '#';
    internal const string ContextUriFragmentItemSelector = "/$entity";
    internal const char ContextUriProjectionStart = '(';
    internal const char ContextUriProjectionEnd = ')';
    internal const string ContextUriProjectionPropertySeparator = ",";
    internal const string ContextUriFragmentNull = "Edm.Null";
    internal const string ContextUriFragmentUntyped = "Edm.Untyped";
    internal const string DeltaResourceSet = "$delta";
    internal const string ContextUriDeltaResourceSet = "/$delta";
    internal const string DeletedEntry = "$deletedEntity";
    internal const string ContextUriDeletedEntry = "/$deletedEntity";
    internal const string DeltaLink = "$link";
    internal const string ContextUriDeltaLink = "/$link";
    internal const string DeletedLink = "$deletedLink";
    internal const string ContextUriDeletedLink = "/$deletedLink";
  }
}
