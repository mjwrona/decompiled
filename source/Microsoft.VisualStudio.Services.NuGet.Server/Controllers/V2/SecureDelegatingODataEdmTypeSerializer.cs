// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.NuGet.Server.Controllers.V2.SecureDelegatingODataEdmTypeSerializer
// Assembly: Microsoft.VisualStudio.Services.NuGet.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1B6DD8F0-A807-4AA3-9A6E-1E5CDBF27B34
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.NuGet.Server.dll

using Microsoft.Data.Edm;
using Microsoft.Data.OData;
using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Web.Http.OData.Formatter.Serialization;

namespace Microsoft.VisualStudio.Services.NuGet.Server.Controllers.V2
{
  public class SecureDelegatingODataEdmTypeSerializer : ODataEdmTypeSerializer
  {
    private readonly ODataEdmTypeSerializer innerEdmTypeSerializer;

    public SecureDelegatingODataEdmTypeSerializer(ODataEdmTypeSerializer innerEdmTypeSerializer)
      : base(innerEdmTypeSerializer.ODataPayloadKind)
    {
      this.innerEdmTypeSerializer = innerEdmTypeSerializer;
    }

    public override ODataValue CreateODataValue(
      object graph,
      IEdmTypeReference expectedType,
      ODataSerializerContext writeContext)
    {
      return this.innerEdmTypeSerializer.CreateODataValue(graph, expectedType, writeContext);
    }

    public override void WriteObject(
      object graph,
      Type type,
      ODataMessageWriter messageWriter,
      ODataSerializerContext writeContext)
    {
      ServerJsonSerializationHelper.ValidateSecurity(graph);
      this.innerEdmTypeSerializer.WriteObject(graph, type, messageWriter, writeContext);
    }

    public override void WriteObjectInline(
      object graph,
      IEdmTypeReference expectedType,
      ODataWriter writer,
      ODataSerializerContext writeContext)
    {
      ServerJsonSerializationHelper.ValidateSecurity(graph);
      this.innerEdmTypeSerializer.WriteObjectInline(graph, expectedType, writer, writeContext);
    }
  }
}
