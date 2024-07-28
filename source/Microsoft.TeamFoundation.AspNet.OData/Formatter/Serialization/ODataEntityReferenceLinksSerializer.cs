// Decompiled with JetBrains decompiler
// Type: Microsoft.AspNet.OData.Formatter.Serialization.ODataEntityReferenceLinksSerializer
// Assembly: Microsoft.TeamFoundation.AspNet.OData, Version=7.3.1.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 08758328-0988-4E6C-88EC-9BA90EA42587
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.AspNet.OData.dll

using Microsoft.AspNet.OData.Common;
using Microsoft.OData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace Microsoft.AspNet.OData.Formatter.Serialization
{
  public class ODataEntityReferenceLinksSerializer : ODataSerializer
  {
    public ODataEntityReferenceLinksSerializer()
      : base(ODataPayloadKind.EntityReferenceLinks)
    {
    }

    public override void WriteObject(
      object graph,
      Type type,
      ODataMessageWriter messageWriter,
      ODataSerializerContext writeContext)
    {
      if (messageWriter == null)
        throw Microsoft.AspNet.OData.Common.Error.ArgumentNull(nameof (messageWriter));
      if (writeContext == null)
        throw Microsoft.AspNet.OData.Common.Error.ArgumentNull(nameof (writeContext));
      switch (graph)
      {
        case null:
          break;
        case ODataEntityReferenceLinks links:
label_8:
          messageWriter.WriteEntityReferenceLinks(links);
          break;
        case IEnumerable<Uri> source:
          links = new ODataEntityReferenceLinks()
          {
            Links = source.Select<Uri, ODataEntityReferenceLink>((Func<Uri, ODataEntityReferenceLink>) (uri => new ODataEntityReferenceLink()
            {
              Url = uri
            }))
          };
          if (writeContext.Request != null)
          {
            links.Count = writeContext.InternalRequest.Context.TotalCount;
            goto label_8;
          }
          else
            goto label_8;
        default:
          throw new SerializationException(Microsoft.AspNet.OData.Common.Error.Format(SRResources.CannotWriteType, (object) this.GetType().Name, (object) graph.GetType().FullName));
      }
    }
  }
}
