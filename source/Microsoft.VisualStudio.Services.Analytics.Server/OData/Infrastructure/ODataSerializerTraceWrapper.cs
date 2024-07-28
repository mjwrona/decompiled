// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Analytics.OData.Infrastructure.ODataSerializerTraceWrapper
// Assembly: Microsoft.VisualStudio.Services.Analytics.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9C06769D-4EB9-467A-8965-10A4FD97C7AD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Analytics.Server.dll

using Microsoft.AspNet.OData.Formatter.Serialization;
using Microsoft.OData;
using Microsoft.TeamFoundation.Framework.Server;
using System;

namespace Microsoft.VisualStudio.Services.Analytics.OData.Infrastructure
{
  public class ODataSerializerTraceWrapper : Microsoft.AspNet.OData.Formatter.Serialization.ODataSerializer
  {
    private readonly Microsoft.AspNet.OData.Formatter.Serialization.ODataSerializer _serializer;
    private static readonly string s_layer = "ODataSerialize";

    public ODataSerializerTraceWrapper(Microsoft.AspNet.OData.Formatter.Serialization.ODataSerializer serializer)
      : base(serializer.ODataPayloadKind)
    {
      this._serializer = serializer;
    }

    public override void WriteObject(
      object graph,
      Type type,
      ODataMessageWriter messageWriter,
      ODataSerializerContext writeContext)
    {
      IVssRequestContext property = (IVssRequestContext) writeContext.Request.Properties[TfsApiPropertyKeys.TfsRequestContext];
      property.TraceEnter(12013009, this.Area, this.Layer, nameof (WriteObject));
      try
      {
        this._serializer.WriteObject(graph, type, messageWriter, writeContext);
      }
      finally
      {
        property.TraceLeave(12013010, this.Area, this.Layer, nameof (WriteObject));
      }
    }

    protected string Area => "AnalyticsModel";

    protected string Layer => ODataSerializerTraceWrapper.s_layer;
  }
}
