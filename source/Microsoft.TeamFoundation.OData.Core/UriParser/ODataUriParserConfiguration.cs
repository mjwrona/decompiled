// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.UriParser.ODataUriParserConfiguration
// Assembly: Microsoft.TeamFoundation.OData.Core, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6619C7F6-E44A-4143-AE77-6D570F968D9A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Core.dll

using Microsoft.OData.Edm;
using System;

namespace Microsoft.OData.UriParser
{
  internal sealed class ODataUriParserConfiguration
  {
    private ODataUrlKeyDelimiter urlKeyDelimiter;
    private ODataUriResolver uriResolver;

    public ODataUriParserConfiguration(IEdmModel model, IServiceProvider container)
    {
      ExceptionUtils.CheckArgumentNotNull<IEdmModel>(model, nameof (model));
      this.Model = model;
      this.Container = container;
      this.Resolver = ODataUriResolver.GetUriResolver(container);
      this.urlKeyDelimiter = ODataUrlKeyDelimiter.GetODataUrlKeyDelimiter(container);
      this.Settings = this.Container != null ? this.Container.GetRequiredService<ODataUriParserSettings>() : new ODataUriParserSettings();
      this.EnableUriTemplateParsing = false;
    }

    internal ODataUriParserConfiguration(IEdmModel model)
      : this(model, (IServiceProvider) null)
    {
    }

    public ODataUriParserSettings Settings { get; private set; }

    public IEdmModel Model { get; private set; }

    public IServiceProvider Container { get; private set; }

    public ODataUrlKeyDelimiter UrlKeyDelimiter
    {
      get => this.urlKeyDelimiter;
      set
      {
        ExceptionUtils.CheckArgumentNotNull<ODataUrlKeyDelimiter>(value, nameof (UrlKeyDelimiter));
        this.urlKeyDelimiter = value;
      }
    }

    public Func<string, BatchReferenceSegment> BatchReferenceCallback { get; set; }

    public ParseDynamicPathSegment ParseDynamicPathSegmentFunc { get; set; }

    internal bool EnableCaseInsensitiveUriFunctionIdentifier
    {
      get => this.Resolver.EnableCaseInsensitive;
      set => this.Resolver.EnableCaseInsensitive = value;
    }

    internal bool EnableNoDollarQueryOptions
    {
      get => this.Resolver.EnableNoDollarQueryOptions;
      set => this.Resolver.EnableNoDollarQueryOptions = value;
    }

    internal bool EnableUriTemplateParsing { get; set; }

    internal ParameterAliasValueAccessor ParameterAliasValueAccessor { get; set; }

    internal ODataUriResolver Resolver
    {
      get => this.uriResolver;
      set
      {
        ExceptionUtils.CheckArgumentNotNull<ODataUriResolver>(value, nameof (Resolver));
        this.uriResolver = value;
      }
    }
  }
}
