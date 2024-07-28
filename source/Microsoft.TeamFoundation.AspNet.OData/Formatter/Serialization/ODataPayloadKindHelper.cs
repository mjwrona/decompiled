// Decompiled with JetBrains decompiler
// Type: Microsoft.AspNet.OData.Formatter.Serialization.ODataPayloadKindHelper
// Assembly: Microsoft.TeamFoundation.AspNet.OData, Version=7.3.1.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 08758328-0988-4E6C-88EC-9BA90EA42587
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.AspNet.OData.dll

using Microsoft.OData;

namespace Microsoft.AspNet.OData.Formatter.Serialization
{
  internal static class ODataPayloadKindHelper
  {
    public static bool IsDefined(ODataPayloadKind payloadKind) => payloadKind == ODataPayloadKind.Batch || payloadKind == ODataPayloadKind.BinaryValue || payloadKind == ODataPayloadKind.Collection || payloadKind == ODataPayloadKind.EntityReferenceLink || payloadKind == ODataPayloadKind.EntityReferenceLinks || payloadKind == ODataPayloadKind.Resource || payloadKind == ODataPayloadKind.Error || payloadKind == ODataPayloadKind.ResourceSet || payloadKind == ODataPayloadKind.MetadataDocument || payloadKind == ODataPayloadKind.Parameter || payloadKind == ODataPayloadKind.Property || payloadKind == ODataPayloadKind.ServiceDocument || payloadKind == ODataPayloadKind.Value || payloadKind == ODataPayloadKind.IndividualProperty || payloadKind == ODataPayloadKind.Delta || payloadKind == ODataPayloadKind.Asynchronous || payloadKind == ODataPayloadKind.Unsupported;

    public static void Validate(ODataPayloadKind payloadKind, string parameterName)
    {
      if (!ODataPayloadKindHelper.IsDefined(payloadKind))
        throw Microsoft.AspNet.OData.Common.Error.InvalidEnumArgument(parameterName, (int) payloadKind, typeof (ODataPayloadKind));
    }
  }
}
