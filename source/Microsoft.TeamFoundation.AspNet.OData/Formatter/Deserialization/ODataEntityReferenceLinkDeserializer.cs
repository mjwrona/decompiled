// Decompiled with JetBrains decompiler
// Type: Microsoft.AspNet.OData.Formatter.Deserialization.ODataEntityReferenceLinkDeserializer
// Assembly: Microsoft.TeamFoundation.AspNet.OData, Version=7.3.1.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 08758328-0988-4E6C-88EC-9BA90EA42587
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.AspNet.OData.dll

using Microsoft.OData;
using System;
using System.Collections.Generic;

namespace Microsoft.AspNet.OData.Formatter.Deserialization
{
  public class ODataEntityReferenceLinkDeserializer : ODataDeserializer
  {
    public ODataEntityReferenceLinkDeserializer()
      : base(ODataPayloadKind.EntityReferenceLink)
    {
    }

    public override object Read(
      ODataMessageReader messageReader,
      Type type,
      ODataDeserializerContext readContext)
    {
      if (messageReader == null)
        throw Microsoft.AspNet.OData.Common.Error.ArgumentNull(nameof (messageReader));
      if (readContext == null)
        throw Microsoft.AspNet.OData.Common.Error.ArgumentNull(nameof (readContext));
      ODataEntityReferenceLink entityReferenceLink = messageReader.ReadEntityReferenceLink();
      return entityReferenceLink != null ? (object) ODataEntityReferenceLinkDeserializer.ResolveContentId(entityReferenceLink.Url, readContext) : (object) null;
    }

    private static Uri ResolveContentId(Uri uri, ODataDeserializerContext readContext)
    {
      if (uri != (Uri) null)
      {
        IDictionary<string, string> contentIdMapping = readContext.InternalRequest.ODataContentIdMapping;
        if (contentIdMapping != null)
        {
          Uri baseUri = new Uri(readContext.InternalUrlHelper.CreateODataLink());
          Uri uri1 = new Uri(ContentIdHelpers.ResolveContentId(uri.IsAbsoluteUri ? baseUri.MakeRelativeUri(uri).OriginalString : uri.OriginalString, contentIdMapping), UriKind.RelativeOrAbsolute);
          if (!uri1.IsAbsoluteUri)
            uri1 = new Uri(baseUri, uri);
          return uri1;
        }
      }
      return uri;
    }
  }
}
