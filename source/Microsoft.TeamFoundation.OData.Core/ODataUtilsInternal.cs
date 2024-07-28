// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.ODataUtilsInternal
// Assembly: Microsoft.TeamFoundation.OData.Core, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6619C7F6-E44A-4143-AE77-6D570F968D9A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Core.dll

using Microsoft.OData.Edm;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.OData
{
  internal static class ODataUtilsInternal
  {
    internal static void SetODataVersion(ODataMessage message, ODataMessageWriterSettings settings)
    {
      string headerValue = ODataUtils.ODataVersionToString(settings.Version.Value);
      message.SetHeader("OData-Version", headerValue);
    }

    internal static ODataVersion GetODataVersion(ODataMessage message, ODataVersion defaultVersion)
    {
      string header = message.GetHeader("OData-Version");
      return !string.IsNullOrEmpty(header) ? ODataUtils.StringToODataVersion(header) : defaultVersion;
    }

    internal static bool IsPayloadKindSupported(ODataPayloadKind payloadKind, bool inRequest)
    {
      switch (payloadKind)
      {
        case ODataPayloadKind.ResourceSet:
        case ODataPayloadKind.EntityReferenceLinks:
        case ODataPayloadKind.Collection:
        case ODataPayloadKind.ServiceDocument:
        case ODataPayloadKind.MetadataDocument:
        case ODataPayloadKind.Error:
        case ODataPayloadKind.IndividualProperty:
        case ODataPayloadKind.Delta:
        case ODataPayloadKind.Asynchronous:
          return !inRequest;
        case ODataPayloadKind.Resource:
        case ODataPayloadKind.Property:
        case ODataPayloadKind.EntityReferenceLink:
        case ODataPayloadKind.Value:
        case ODataPayloadKind.BinaryValue:
        case ODataPayloadKind.Batch:
          return true;
        case ODataPayloadKind.Parameter:
          return inRequest;
        default:
          throw new ODataException(Strings.General_InternalError((object) InternalErrorCodes.ODataUtilsInternal_IsPayloadKindSupported_UnreachableCodePath));
      }
    }

    internal static IEnumerable<T> ConcatEnumerables<T>(
      IEnumerable<T> enumerable1,
      IEnumerable<T> enumerable2)
    {
      if (enumerable1 == null)
        return enumerable2;
      return enumerable2 == null ? enumerable1 : enumerable1.Concat<T>(enumerable2);
    }

    internal static bool IsNullable(this IEdmTypeReference type) => type == null || type.IsNullable;
  }
}
