// Decompiled with JetBrains decompiler
// Type: Nest.ConnectionSettingsBase`1
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net;
using Elasticsearch.Net.Utf8Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Nest
{
  [Browsable(false)]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public abstract class ConnectionSettingsBase<TConnectionSettings> : 
    ConnectionConfiguration<TConnectionSettings>,
    IConnectionSettingsValues,
    IConnectionConfigurationValues,
    IDisposable
    where TConnectionSettings : ConnectionSettingsBase<TConnectionSettings>, IConnectionSettingsValues
  {
    private readonly FluentDictionary<Type, string> _defaultIndices;
    private readonly FluentDictionary<Type, string> _defaultRelationNames;
    private readonly HashSet<Type> _disableIdInference = new HashSet<Type>();
    private readonly FluentDictionary<Type, string> _idProperties = new FluentDictionary<Type, string>();
    private readonly Inferrer _inferrer;
    private readonly IPropertyMappingProvider _propertyMappingProvider;
    private readonly FluentDictionary<MemberInfo, IPropertyMapping> _propertyMappings = new FluentDictionary<MemberInfo, IPropertyMapping>();
    private readonly FluentDictionary<Type, string> _routeProperties = new FluentDictionary<Type, string>();
    private readonly IElasticsearchSerializer _sourceSerializer;
    private bool _defaultDisableAllInference;
    private Func<string, string> _defaultFieldNameInferrer;
    private string _defaultIndex;

    protected ConnectionSettingsBase(
      IConnectionPool connectionPool,
      IConnection connection,
      ConnectionSettings.SourceSerializerFactory sourceSerializerFactory,
      IPropertyMappingProvider propertyMappingProvider)
      : base(connectionPool, connection, (IElasticsearchSerializer) null)
    {
      DefaultHighLevelSerializer highLevelSerializer = new DefaultHighLevelSerializer((IJsonFormatterResolver) new NestFormatterResolver((IConnectionSettingsValues) this));
      IElasticsearchSerializer serializer = (sourceSerializerFactory != null ? sourceSerializerFactory((IElasticsearchSerializer) highLevelSerializer, (IConnectionSettingsValues) this) : (IElasticsearchSerializer) null) ?? (IElasticsearchSerializer) highLevelSerializer;
      IPropertyMappingProvider propertyMappingProvider1 = serializer as IPropertyMappingProvider;
      this._propertyMappingProvider = propertyMappingProvider ?? propertyMappingProvider1 ?? (IPropertyMappingProvider) new PropertyMappingProvider();
      this._sourceSerializer = (IElasticsearchSerializer) new DiagnosticsSerializerProxy(serializer, "source");
      this.UseThisRequestResponseSerializer = (IElasticsearchSerializer) new DiagnosticsSerializerProxy((IElasticsearchSerializer) highLevelSerializer);
      this._defaultFieldNameInferrer = (Func<string, string>) (p => p.ToCamelCase());
      this._defaultIndices = new FluentDictionary<Type, string>();
      this._defaultRelationNames = new FluentDictionary<Type, string>();
      this._inferrer = new Inferrer((IConnectionSettingsValues) this);
      this.UserAgent(ConnectionSettings.DefaultUserAgent);
    }

    bool IConnectionSettingsValues.DefaultDisableIdInference => this._defaultDisableAllInference;

    Func<string, string> IConnectionSettingsValues.DefaultFieldNameInferrer => this._defaultFieldNameInferrer;

    string IConnectionSettingsValues.DefaultIndex => this._defaultIndex;

    FluentDictionary<Type, string> IConnectionSettingsValues.DefaultIndices => this._defaultIndices;

    HashSet<Type> IConnectionSettingsValues.DisableIdInference => this._disableIdInference;

    FluentDictionary<Type, string> IConnectionSettingsValues.DefaultRelationNames => this._defaultRelationNames;

    FluentDictionary<Type, string> IConnectionSettingsValues.IdProperties => this._idProperties;

    Inferrer IConnectionSettingsValues.Inferrer => this._inferrer;

    IPropertyMappingProvider IConnectionSettingsValues.PropertyMappingProvider => this._propertyMappingProvider;

    FluentDictionary<MemberInfo, IPropertyMapping> IConnectionSettingsValues.PropertyMappings => this._propertyMappings;

    FluentDictionary<Type, string> IConnectionSettingsValues.RouteProperties => this._routeProperties;

    IElasticsearchSerializer IConnectionSettingsValues.SourceSerializer => this._sourceSerializer;

    public TConnectionSettings DefaultIndex(string defaultIndex) => this.Assign<string>(defaultIndex, (Action<TConnectionSettings, string>) ((a, v) => a._defaultIndex = v));

    public TConnectionSettings DefaultFieldNameInferrer(Func<string, string> fieldNameInferrer) => this.Assign<Func<string, string>>(fieldNameInferrer, (Action<TConnectionSettings, Func<string, string>>) ((a, v) => a._defaultFieldNameInferrer = v));

    public TConnectionSettings DefaultDisableIdInference(bool disable = true) => this.Assign<bool>(disable, (Action<TConnectionSettings, bool>) ((a, v) => a._defaultDisableAllInference = v));

    private void MapIdPropertyFor<TDocument>(Expression<Func<TDocument, object>> objectPath)
    {
      objectPath.ThrowIfNull<Expression<Func<TDocument, object>>>(nameof (objectPath));
      string name = new MemberInfoResolver((Expression) objectPath).Members.Single<MemberInfo>().Name;
      string str;
      if (this._idProperties.TryGetValue(typeof (TDocument), out str))
      {
        if (!str.Equals(name))
          throw new ArgumentException("Cannot map '" + name + "' as the id property for type '" + typeof (TDocument).Name + "': it already has '" + this._idProperties[typeof (TDocument)] + "' mapped.");
      }
      else
        this._idProperties.Add(typeof (TDocument), name);
    }

    private void MapRoutePropertyFor<TDocument>(Expression<Func<TDocument, object>> objectPath)
    {
      objectPath.ThrowIfNull<Expression<Func<TDocument, object>>>(nameof (objectPath));
      string name = new MemberInfoResolver((Expression) objectPath).Members.Single<MemberInfo>().Name;
      string str;
      if (this._routeProperties.TryGetValue(typeof (TDocument), out str))
      {
        if (!str.Equals(name))
          throw new ArgumentException("Cannot map '" + name + "' as the route property for type '" + typeof (TDocument).Name + "': it already has '" + this._routeProperties[typeof (TDocument)] + "' mapped.");
      }
      else
        this._routeProperties.Add(typeof (TDocument), name);
    }

    private void ApplyPropertyMappings<TDocument>(IList<IClrPropertyMapping<TDocument>> mappings) where TDocument : class
    {
      foreach (IClrPropertyMapping<TDocument> mapping in (IEnumerable<IClrPropertyMapping<TDocument>>) mappings)
      {
        Expression<Func<TDocument, object>> property = mapping.Property;
        MemberInfoResolver memberInfoResolver = new MemberInfoResolver((Expression) property);
        if (memberInfoResolver.Members.Count > 1)
          throw new ArgumentException("ApplyPropertyMappings can only map direct properties");
        if (memberInfoResolver.Members.Count == 0)
          throw new ArgumentException(string.Format("Expression {0} does contain any member access", (object) property));
        MemberInfo member = memberInfoResolver.Members[0];
        IPropertyMapping propertyMapping;
        if (this._propertyMappings.TryGetValue(member, out propertyMapping))
        {
          string newName = mapping.NewName;
          string name1 = propertyMapping.Name;
          string name2 = typeof (TDocument).Name;
          if (name1.IsNullOrEmpty() && newName.IsNullOrEmpty())
            throw new ArgumentException(string.Format("Property mapping '{0}' on type is already ignored", (object) property));
          if (name1.IsNullOrEmpty())
            throw new ArgumentException(string.Format("Property mapping '{0}' on type {1} can not be mapped to '{2}' it already has an ignore mapping", (object) property, (object) name2, (object) newName));
          if (newName.IsNullOrEmpty())
            throw new ArgumentException(string.Format("Property mapping '{0}' on type {1} can not be ignored it already has a mapping to '{2}'", (object) property, (object) name2, (object) name1));
          throw new ArgumentException(string.Format("Property mapping '{0}' on type {1} can not be mapped to '{2}' already mapped as '{3}'", (object) property, (object) name2, (object) newName, (object) name1));
        }
        this._propertyMappings[member] = mapping.ToPropertyMapping();
      }
    }

    public TConnectionSettings DefaultMappingFor<TDocument>(
      Func<ClrTypeMappingDescriptor<TDocument>, IClrTypeMapping<TDocument>> selector)
      where TDocument : class
    {
      IClrTypeMapping<TDocument> clrTypeMapping = selector(new ClrTypeMappingDescriptor<TDocument>());
      if (!clrTypeMapping.IndexName.IsNullOrEmpty())
        this._defaultIndices[clrTypeMapping.ClrType] = clrTypeMapping.IndexName;
      if (!clrTypeMapping.RelationName.IsNullOrEmpty())
        this._defaultRelationNames[clrTypeMapping.ClrType] = clrTypeMapping.RelationName;
      if (!string.IsNullOrWhiteSpace(clrTypeMapping.IdPropertyName))
        this._idProperties[clrTypeMapping.ClrType] = clrTypeMapping.IdPropertyName;
      if (clrTypeMapping.IdProperty != null)
        this.MapIdPropertyFor<TDocument>(clrTypeMapping.IdProperty);
      if (clrTypeMapping.RoutingProperty != null)
        this.MapRoutePropertyFor<TDocument>(clrTypeMapping.RoutingProperty);
      if (clrTypeMapping.Properties != null)
        this.ApplyPropertyMappings<TDocument>(clrTypeMapping.Properties);
      if (clrTypeMapping.DisableIdInference)
        this._disableIdInference.Add(clrTypeMapping.ClrType);
      else
        this._disableIdInference.Remove(clrTypeMapping.ClrType);
      return (TConnectionSettings) this;
    }

    public TConnectionSettings DefaultMappingFor(
      Type documentType,
      Func<ClrTypeMappingDescriptor, IClrTypeMapping> selector)
    {
      IClrTypeMapping clrTypeMapping = selector(new ClrTypeMappingDescriptor(documentType));
      if (!clrTypeMapping.IndexName.IsNullOrEmpty())
        this._defaultIndices[clrTypeMapping.ClrType] = clrTypeMapping.IndexName;
      if (!clrTypeMapping.RelationName.IsNullOrEmpty())
        this._defaultRelationNames[clrTypeMapping.ClrType] = clrTypeMapping.RelationName;
      if (!string.IsNullOrWhiteSpace(clrTypeMapping.IdPropertyName))
        this._idProperties[clrTypeMapping.ClrType] = clrTypeMapping.IdPropertyName;
      return (TConnectionSettings) this;
    }

    public TConnectionSettings DefaultMappingFor(IEnumerable<IClrTypeMapping> typeMappings)
    {
      if (typeMappings == null)
        return (TConnectionSettings) this;
      foreach (IClrTypeMapping typeMapping in typeMappings)
      {
        if (!typeMapping.IndexName.IsNullOrEmpty())
          this._defaultIndices[typeMapping.ClrType] = typeMapping.IndexName;
        if (!typeMapping.RelationName.IsNullOrEmpty())
          this._defaultRelationNames[typeMapping.ClrType] = typeMapping.RelationName;
      }
      return (TConnectionSettings) this;
    }

    protected override bool HttpStatusCodeClassifier(HttpMethod method, int statusCode) => statusCode >= 200 && statusCode < 300 || statusCode == 404;
  }
}
