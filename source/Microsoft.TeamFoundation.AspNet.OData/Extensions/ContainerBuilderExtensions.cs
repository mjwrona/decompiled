// Decompiled with JetBrains decompiler
// Type: Microsoft.AspNet.OData.Extensions.ContainerBuilderExtensions
// Assembly: Microsoft.TeamFoundation.AspNet.OData, Version=7.3.1.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 08758328-0988-4E6C-88EC-9BA90EA42587
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.AspNet.OData.dll

using Microsoft.AspNet.OData.Formatter.Deserialization;
using Microsoft.AspNet.OData.Formatter.Serialization;
using Microsoft.AspNet.OData.Query;
using Microsoft.AspNet.OData.Query.Expressions;
using Microsoft.AspNet.OData.Query.Validators;
using Microsoft.AspNet.OData.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OData;
using System;
using System.Net.Http;

namespace Microsoft.AspNet.OData.Extensions
{
  internal static class ContainerBuilderExtensions
  {
    public static IContainerBuilder AddDefaultWebApiServices(this IContainerBuilder builder)
    {
      if (builder == null)
        throw Microsoft.AspNet.OData.Common.Error.ArgumentNull(nameof (builder));
      builder.AddService<IODataPathHandler, DefaultODataPathHandler>(Microsoft.OData.ServiceLifetime.Singleton);
      builder.AddServicePrototype<ODataMessageReaderSettings>(new ODataMessageReaderSettings()
      {
        EnableMessageStreamDisposal = false,
        MessageQuotas = new ODataMessageQuotas()
        {
          MaxReceivedMessageSize = long.MaxValue
        }
      });
      builder.AddServicePrototype<ODataMessageWriterSettings>(new ODataMessageWriterSettings()
      {
        EnableMessageStreamDisposal = false,
        MessageQuotas = new ODataMessageQuotas()
        {
          MaxReceivedMessageSize = long.MaxValue
        }
      });
      builder.AddService<CountQueryValidator>(Microsoft.OData.ServiceLifetime.Singleton);
      builder.AddService<FilterQueryValidator>(Microsoft.OData.ServiceLifetime.Scoped);
      builder.AddService<ODataQueryValidator>(Microsoft.OData.ServiceLifetime.Singleton);
      builder.AddService<OrderByQueryValidator>(Microsoft.OData.ServiceLifetime.Singleton);
      builder.AddService<SelectExpandQueryValidator>(Microsoft.OData.ServiceLifetime.Singleton);
      builder.AddService<SkipQueryValidator>(Microsoft.OData.ServiceLifetime.Singleton);
      builder.AddService<SkipTokenQueryValidator>(Microsoft.OData.ServiceLifetime.Singleton);
      builder.AddService<TopQueryValidator>(Microsoft.OData.ServiceLifetime.Singleton);
      builder.AddService<ComputeQueryValidator>(Microsoft.OData.ServiceLifetime.Scoped);
      builder.AddService<SkipTokenHandler, DefaultSkipTokenHandler>(Microsoft.OData.ServiceLifetime.Singleton);
      builder.AddService<ODataSerializerProvider, DefaultODataSerializerProvider>(Microsoft.OData.ServiceLifetime.Singleton);
      builder.AddService<ODataDeserializerProvider, DefaultODataDeserializerProvider>(Microsoft.OData.ServiceLifetime.Singleton);
      builder.AddService<ODataResourceDeserializer>(Microsoft.OData.ServiceLifetime.Singleton);
      builder.AddService<ODataEnumDeserializer>(Microsoft.OData.ServiceLifetime.Singleton);
      builder.AddService<ODataPrimitiveDeserializer>(Microsoft.OData.ServiceLifetime.Singleton);
      builder.AddService<ODataResourceSetDeserializer>(Microsoft.OData.ServiceLifetime.Singleton);
      builder.AddService<ODataCollectionDeserializer>(Microsoft.OData.ServiceLifetime.Singleton);
      builder.AddService<ODataEntityReferenceLinkDeserializer>(Microsoft.OData.ServiceLifetime.Singleton);
      builder.AddService<ODataActionPayloadDeserializer>(Microsoft.OData.ServiceLifetime.Singleton);
      builder.AddService<ODataEnumSerializer>(Microsoft.OData.ServiceLifetime.Singleton);
      builder.AddService<ODataPrimitiveSerializer>(Microsoft.OData.ServiceLifetime.Singleton);
      builder.AddService<ODataDeltaFeedSerializer>(Microsoft.OData.ServiceLifetime.Singleton);
      builder.AddService<ODataResourceSetSerializer>(Microsoft.OData.ServiceLifetime.Singleton);
      builder.AddService<ODataCollectionSerializer>(Microsoft.OData.ServiceLifetime.Singleton);
      builder.AddService<ODataResourceSerializer>(Microsoft.OData.ServiceLifetime.Singleton);
      builder.AddService<ODataServiceDocumentSerializer>(Microsoft.OData.ServiceLifetime.Singleton);
      builder.AddService<ODataEntityReferenceLinkSerializer>(Microsoft.OData.ServiceLifetime.Singleton);
      builder.AddService<ODataEntityReferenceLinksSerializer>(Microsoft.OData.ServiceLifetime.Singleton);
      builder.AddService<ODataErrorSerializer>(Microsoft.OData.ServiceLifetime.Singleton);
      builder.AddService<ODataMetadataSerializer>(Microsoft.OData.ServiceLifetime.Singleton);
      builder.AddService<ODataRawValueSerializer>(Microsoft.OData.ServiceLifetime.Singleton);
      builder.AddService<ODataQuerySettings>(Microsoft.OData.ServiceLifetime.Scoped);
      builder.AddService<FilterBinder>(Microsoft.OData.ServiceLifetime.Transient);
      builder.AddService<HttpRequestScope>(Microsoft.OData.ServiceLifetime.Scoped);
      builder.AddService<HttpRequestMessage>(Microsoft.OData.ServiceLifetime.Scoped, (Func<IServiceProvider, HttpRequestMessage>) (sp => ServiceProviderServiceExtensions.GetRequiredService<HttpRequestScope>(sp).HttpRequest));
      return builder;
    }
  }
}
