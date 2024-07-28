// Decompiled with JetBrains decompiler
// Type: Microsoft.AspNet.OData.Formatter.Serialization.ODataSerializer
// Assembly: Microsoft.TeamFoundation.AspNet.OData, Version=7.3.1.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 08758328-0988-4E6C-88EC-9BA90EA42587
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.AspNet.OData.dll

using Microsoft.AspNet.OData.Common;
using Microsoft.OData;
using System;

namespace Microsoft.AspNet.OData.Formatter.Serialization
{
  public abstract class ODataSerializer
  {
    protected ODataSerializer(ODataPayloadKind payloadKind)
    {
      ODataPayloadKindHelper.Validate(payloadKind, nameof (payloadKind));
      this.ODataPayloadKind = payloadKind;
    }

    public ODataPayloadKind ODataPayloadKind { get; private set; }

    public virtual void WriteObject(
      object graph,
      Type type,
      ODataMessageWriter messageWriter,
      ODataSerializerContext writeContext)
    {
      throw Microsoft.AspNet.OData.Common.Error.NotSupported(SRResources.WriteObjectNotSupported, (object) this.GetType().Name);
    }
  }
}
