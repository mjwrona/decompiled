// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Analytics.OData.Infrastructure.AnalyticsODataSerializerProvider
// Assembly: Microsoft.VisualStudio.Services.Analytics.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9C06769D-4EB9-467A-8965-10A4FD97C7AD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Analytics.Server.dll

using Microsoft.AspNet.OData.Formatter.Serialization;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OData.Edm;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Net.Http;

namespace Microsoft.VisualStudio.Services.Analytics.OData.Infrastructure
{
  public class AnalyticsODataSerializerProvider : DefaultODataSerializerProvider
  {
    private static readonly WrappedExceptionErrorSerializer _errorSerializer = new WrappedExceptionErrorSerializer();
    private ODataEdmTypeSerializer _primitiveTypeSerializer;
    private ODataEdmTypeSerializer _resourceTypeSerializer;
    private readonly IServiceProvider _rootContainer;

    public AnalyticsODataSerializerProvider(IServiceProvider rootContainer)
      : base(rootContainer)
    {
      this._rootContainer = rootContainer;
    }

    public override ODataSerializer GetODataPayloadSerializer(Type type, HttpRequestMessage request) => type == typeof (WrappedException) ? (ODataSerializer) AnalyticsODataSerializerProvider._errorSerializer : (ODataSerializer) new ODataSerializerTraceWrapper(base.GetODataPayloadSerializer(type, request));

    public override ODataEdmTypeSerializer GetEdmTypeSerializer(IEdmTypeReference edmType)
    {
      switch (edmType.TypeKind())
      {
        case EdmTypeKind.Primitive:
          return this._primitiveTypeSerializer ?? (this._primitiveTypeSerializer = (ODataEdmTypeSerializer) this._rootContainer.GetRequiredService<ODataPrimitiveSerializer>());
        case EdmTypeKind.Entity:
          return this._resourceTypeSerializer ?? (this._resourceTypeSerializer = (ODataEdmTypeSerializer) this._rootContainer.GetRequiredService<ODataResourceSerializer>());
        default:
          return base.GetEdmTypeSerializer(edmType);
      }
    }
  }
}
