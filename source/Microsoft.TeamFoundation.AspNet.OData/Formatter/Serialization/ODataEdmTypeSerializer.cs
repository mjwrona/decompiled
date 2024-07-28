// Decompiled with JetBrains decompiler
// Type: Microsoft.AspNet.OData.Formatter.Serialization.ODataEdmTypeSerializer
// Assembly: Microsoft.TeamFoundation.AspNet.OData, Version=7.3.1.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 08758328-0988-4E6C-88EC-9BA90EA42587
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.AspNet.OData.dll

using Microsoft.AspNet.OData.Common;
using Microsoft.OData;
using Microsoft.OData.Edm;

namespace Microsoft.AspNet.OData.Formatter.Serialization
{
  public abstract class ODataEdmTypeSerializer : ODataSerializer
  {
    protected ODataEdmTypeSerializer(ODataPayloadKind payloadKind)
      : base(payloadKind)
    {
    }

    protected ODataEdmTypeSerializer(
      ODataPayloadKind payloadKind,
      ODataSerializerProvider serializerProvider)
      : this(payloadKind)
    {
      this.SerializerProvider = serializerProvider != null ? serializerProvider : throw Microsoft.AspNet.OData.Common.Error.ArgumentNull(nameof (serializerProvider));
    }

    public ODataSerializerProvider SerializerProvider { get; private set; }

    public virtual void WriteObjectInline(
      object graph,
      IEdmTypeReference expectedType,
      ODataWriter writer,
      ODataSerializerContext writeContext)
    {
      throw Microsoft.AspNet.OData.Common.Error.NotSupported(SRResources.WriteObjectInlineNotSupported, (object) this.GetType().Name);
    }

    public virtual ODataValue CreateODataValue(
      object graph,
      IEdmTypeReference expectedType,
      ODataSerializerContext writeContext)
    {
      throw Microsoft.AspNet.OData.Common.Error.NotSupported(SRResources.CreateODataValueNotSupported, (object) this.GetType().Name);
    }

    internal virtual ODataProperty CreateProperty(
      object graph,
      IEdmTypeReference expectedType,
      string elementName,
      ODataSerializerContext writeContext)
    {
      ODataProperty property = new ODataProperty();
      property.Name = elementName;
      property.Value = (object) this.CreateODataValue(graph, expectedType, writeContext);
      return property;
    }
  }
}
