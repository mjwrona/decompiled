// Decompiled with JetBrains decompiler
// Type: Nest.ConnectionSettings
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net;
using System;
using System.Reflection;

namespace Nest
{
  public class ConnectionSettings : ConnectionSettingsBase<ConnectionSettings>
  {
    public static readonly string DefaultUserAgent = "elasticsearch-net/" + typeof (IConnectionSettingsValues).Assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>().InformationalVersion + " (" + RuntimeInformation.OSDescription + "; " + RuntimeInformation.FrameworkDescription + "; Nest)";

    public ConnectionSettings(Uri uri = null)
    {
      Uri uri1 = uri;
      if ((object) uri1 == null)
        uri1 = new Uri("http://localhost:9200");
      // ISSUE: explicit constructor call
      this.\u002Ector((IConnectionPool) new SingleNodeConnectionPool(uri1));
    }

    public ConnectionSettings(string cloudId, BasicAuthenticationCredentials credentials)
      : this((IConnectionPool) new CloudConnectionPool(cloudId, credentials))
    {
    }

    public ConnectionSettings(string cloudId, ApiKeyAuthenticationCredentials credentials)
      : this((IConnectionPool) new CloudConnectionPool(cloudId, credentials))
    {
    }

    public ConnectionSettings(InMemoryConnection connection)
      : this((IConnectionPool) new SingleNodeConnectionPool(new Uri("http://localhost:9200")), (IConnection) connection)
    {
    }

    public ConnectionSettings(IConnectionPool connectionPool)
      : this(connectionPool, (IConnection) null, (ConnectionSettings.SourceSerializerFactory) null)
    {
    }

    public ConnectionSettings(
      IConnectionPool connectionPool,
      ConnectionSettings.SourceSerializerFactory sourceSerializer)
      : this(connectionPool, (IConnection) null, sourceSerializer)
    {
    }

    public ConnectionSettings(IConnectionPool connectionPool, IConnection connection)
      : this(connectionPool, connection, (ConnectionSettings.SourceSerializerFactory) null)
    {
    }

    public ConnectionSettings(
      IConnectionPool connectionPool,
      IConnection connection,
      ConnectionSettings.SourceSerializerFactory sourceSerializer)
      : this(connectionPool, connection, sourceSerializer, (IPropertyMappingProvider) null)
    {
    }

    public ConnectionSettings(
      IConnectionPool connectionPool,
      IConnection connection,
      ConnectionSettings.SourceSerializerFactory sourceSerializer,
      IPropertyMappingProvider propertyMappingProvider)
      : base(connectionPool, connection, sourceSerializer, propertyMappingProvider)
    {
    }

    public delegate IElasticsearchSerializer SourceSerializerFactory(
      IElasticsearchSerializer builtIn,
      IConnectionSettingsValues values);
  }
}
