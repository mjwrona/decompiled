// Decompiled with JetBrains decompiler
// Type: Microsoft.AspNet.OData.Formatter.Deserialization.ODataEdmTypeDeserializer
// Assembly: Microsoft.TeamFoundation.AspNet.OData, Version=7.3.1.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 08758328-0988-4E6C-88EC-9BA90EA42587
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.AspNet.OData.dll

using Microsoft.AspNet.OData.Common;
using Microsoft.OData;
using Microsoft.OData.Edm;

namespace Microsoft.AspNet.OData.Formatter.Deserialization
{
  public abstract class ODataEdmTypeDeserializer : ODataDeserializer
  {
    protected ODataEdmTypeDeserializer(ODataPayloadKind payloadKind)
      : base(payloadKind)
    {
    }

    protected ODataEdmTypeDeserializer(
      ODataPayloadKind payloadKind,
      ODataDeserializerProvider deserializerProvider)
      : this(payloadKind)
    {
      this.DeserializerProvider = deserializerProvider != null ? deserializerProvider : throw Microsoft.AspNet.OData.Common.Error.ArgumentNull(nameof (deserializerProvider));
    }

    public ODataDeserializerProvider DeserializerProvider { get; private set; }

    public virtual object ReadInline(
      object item,
      IEdmTypeReference edmType,
      ODataDeserializerContext readContext)
    {
      throw Microsoft.AspNet.OData.Common.Error.NotSupported(SRResources.DoesNotSupportReadInLine, (object) this.GetType().Name);
    }
  }
}
