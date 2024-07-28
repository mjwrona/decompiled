// Decompiled with JetBrains decompiler
// Type: Microsoft.AspNet.OData.Formatter.Serialization.ODataRawValueSerializer
// Assembly: Microsoft.TeamFoundation.AspNet.OData, Version=7.3.1.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 08758328-0988-4E6C-88EC-9BA90EA42587
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.AspNet.OData.dll

using Microsoft.OData;
using System;

namespace Microsoft.AspNet.OData.Formatter.Serialization
{
  public class ODataRawValueSerializer : ODataSerializer
  {
    public ODataRawValueSerializer()
      : base(ODataPayloadKind.Value)
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
      if (graph == null)
        throw Microsoft.AspNet.OData.Common.Error.ArgumentNull(nameof (graph));
      if (TypeHelper.IsEnum(graph.GetType()))
        messageWriter.WriteValue((object) graph.ToString());
      else
        messageWriter.WriteValue(ODataPrimitiveSerializer.ConvertUnsupportedPrimitives(graph));
    }
  }
}
