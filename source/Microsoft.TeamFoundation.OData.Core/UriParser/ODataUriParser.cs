// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.UriParser.ODataUriParser
// Assembly: Microsoft.TeamFoundation.OData.Core, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6619C7F6-E44A-4143-AE77-6D570F968D9A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Core.dll

using Microsoft.OData.Edm;
using Microsoft.OData.UriParser.Aggregation;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Microsoft.OData.UriParser
{
  public sealed class ODataUriParser
  {
    private readonly ODataUriParserConfiguration configuration;
    private readonly Uri serviceRoot;
    private readonly Uri uri;
    private readonly List<CustomQueryOptionToken> queryOptions;
    private IDictionary<string, string> queryOptionDic;
    private IList<KeyValuePair<string, string>> customQueryOptions;
    private ODataQueryOptionParser queryOptionParser;
    private ODataPath odataPath;
    private EntityIdSegment entityIdSegment;

    public ODataUriParser(IEdmModel model, Uri serviceRoot, Uri uri)
      : this(model, serviceRoot, uri, (IServiceProvider) null)
    {
    }

    public ODataUriParser(IEdmModel model, Uri serviceRoot, Uri uri, IServiceProvider container)
    {
      ExceptionUtils.CheckArgumentNotNull<Uri>(uri, nameof (uri));
      if (serviceRoot == (Uri) null)
        throw new ODataException(Microsoft.OData.Strings.UriParser_NeedServiceRootForThisOverload);
      if (!serviceRoot.IsAbsoluteUri)
        throw new ODataException(Microsoft.OData.Strings.UriParser_UriMustBeAbsolute((object) serviceRoot));
      this.configuration = new ODataUriParserConfiguration(model, container);
      this.serviceRoot = UriUtils.EnsureTaillingSlash(serviceRoot);
      this.uri = uri.IsAbsoluteUri ? uri : UriUtils.UriToAbsoluteUri(this.ServiceRoot, uri);
      this.queryOptions = QueryOptionUtils.ParseQueryOptions(this.uri);
    }

    public ODataUriParser(IEdmModel model, Uri relativeUri)
      : this(model, relativeUri, (IServiceProvider) null)
    {
    }

    public ODataUriParser(IEdmModel model, Uri relativeUri, IServiceProvider container)
    {
      ExceptionUtils.CheckArgumentNotNull<Uri>(relativeUri, nameof (relativeUri));
      if (relativeUri.IsAbsoluteUri)
        throw new ODataException(Microsoft.OData.Strings.UriParser_RelativeUriMustBeRelative);
      this.configuration = new ODataUriParserConfiguration(model, container);
      this.uri = relativeUri;
      this.queryOptions = QueryOptionUtils.ParseQueryOptions(UriUtils.CreateMockAbsoluteUri(this.uri));
    }

    public ODataUriParserSettings Settings => this.configuration.Settings;

    public IEdmModel Model => this.configuration.Model;

    public IServiceProvider Container => this.configuration.Container;

    public Uri ServiceRoot => this.serviceRoot;

    public ODataUrlKeyDelimiter UrlKeyDelimiter
    {
      get => this.configuration.UrlKeyDelimiter;
      set => this.configuration.UrlKeyDelimiter = value;
    }

    public Func<string, BatchReferenceSegment> BatchReferenceCallback
    {
      get => this.configuration.BatchReferenceCallback;
      set => this.configuration.BatchReferenceCallback = value;
    }

    public bool EnableNoDollarQueryOptions
    {
      get => this.configuration.EnableNoDollarQueryOptions;
      set => this.configuration.EnableNoDollarQueryOptions = value;
    }

    public bool EnableUriTemplateParsing
    {
      get => this.configuration.EnableUriTemplateParsing;
      set => this.configuration.EnableUriTemplateParsing = value;
    }

    public ODataUriResolver Resolver
    {
      get => this.configuration.Resolver;
      set => this.configuration.Resolver = value;
    }

    public ParseDynamicPathSegment ParseDynamicPathSegmentFunc
    {
      get => this.configuration.ParseDynamicPathSegmentFunc;
      set => this.configuration.ParseDynamicPathSegmentFunc = value;
    }

    public IDictionary<string, SingleValueNode> ParameterAliasNodes
    {
      get
      {
        if (this.ParameterAliasValueAccessor == null)
          this.Initialize();
        return this.ParameterAliasValueAccessor.ParameterAliasValueNodesCached;
      }
    }

    public IList<KeyValuePair<string, string>> CustomQueryOptions
    {
      get
      {
        if (this.customQueryOptions == null)
          this.InitQueryOptionDic();
        return this.customQueryOptions;
      }
    }

    internal ParameterAliasValueAccessor ParameterAliasValueAccessor
    {
      get => this.configuration.ParameterAliasValueAccessor;
      set => this.configuration.ParameterAliasValueAccessor = value;
    }

    public ODataPath ParsePath()
    {
      this.Initialize();
      return this.odataPath;
    }

    public FilterClause ParseFilter()
    {
      this.Initialize();
      return this.queryOptionParser.ParseFilter();
    }

    public OrderByClause ParseOrderBy()
    {
      this.Initialize();
      return this.queryOptionParser.ParseOrderBy();
    }

    public SelectExpandClause ParseSelectAndExpand()
    {
      this.Initialize();
      return this.queryOptionParser.ParseSelectAndExpand();
    }

    public EntityIdSegment ParseEntityId()
    {
      if (this.entityIdSegment != null)
        return this.entityIdSegment;
      this.InitQueryOptionDic();
      string uriString = (string) null;
      if (!this.queryOptionDic.TryGetValue("$id", out uriString) && !this.Resolver.EnableCaseInsensitive)
        return (EntityIdSegment) null;
      if (uriString == null && this.Resolver.EnableCaseInsensitive)
      {
        List<KeyValuePair<string, string>> list = this.queryOptionDic.Where<KeyValuePair<string, string>>((Func<KeyValuePair<string, string>, bool>) (pair => string.Equals("$id", pair.Key, StringComparison.OrdinalIgnoreCase))).ToList<KeyValuePair<string, string>>();
        if (list.Count == 0)
          return (EntityIdSegment) null;
        uriString = list.Count == 1 ? list.First<KeyValuePair<string, string>>().Value : throw new ODataException(Microsoft.OData.Strings.QueryOptionUtils_QueryParameterMustBeSpecifiedOnce((object) "$id"));
      }
      Uri uri = new Uri(uriString, UriKind.RelativeOrAbsolute);
      if (!uri.IsAbsoluteUri)
        uri = this.uri.IsAbsoluteUri ? new Uri(this.uri, uri) : UriUtils.CreateMockAbsoluteUri().MakeRelativeUri(new Uri(UriUtils.CreateMockAbsoluteUri(this.uri), uri));
      this.entityIdSegment = new EntityIdSegment(uri);
      return this.entityIdSegment;
    }

    public long? ParseTop()
    {
      this.Initialize();
      return this.queryOptionParser.ParseTop();
    }

    public long? ParseSkip()
    {
      this.Initialize();
      return this.queryOptionParser.ParseSkip();
    }

    public long? ParseIndex()
    {
      this.Initialize();
      return this.queryOptionParser.ParseIndex();
    }

    public bool? ParseCount()
    {
      this.Initialize();
      return this.queryOptionParser.ParseCount();
    }

    public SearchClause ParseSearch()
    {
      this.Initialize();
      return this.queryOptionParser.ParseSearch();
    }

    public ApplyClause ParseApply()
    {
      this.Initialize();
      return this.queryOptionParser.ParseApply();
    }

    public string ParseSkipToken()
    {
      this.Initialize();
      return this.queryOptionParser.ParseSkipToken();
    }

    public string ParseDeltaToken()
    {
      this.Initialize();
      return this.queryOptionParser.ParseDeltaToken();
    }

    public ComputeClause ParseCompute()
    {
      this.Initialize();
      return this.queryOptionParser.ParseCompute();
    }

    public ODataUri ParseUri()
    {
      ExceptionUtils.CheckArgumentNotNull<IEdmModel>(this.configuration.Model, "model");
      ExceptionUtils.CheckArgumentNotNull<Uri>(this.uri, "uri");
      ODataPath path = this.ParsePath();
      SelectExpandClause selectAndExpand = this.ParseSelectAndExpand();
      FilterClause filter = this.ParseFilter();
      OrderByClause orderBy = this.ParseOrderBy();
      SearchClause search = this.ParseSearch();
      ApplyClause apply = this.ParseApply();
      ComputeClause compute = this.ParseCompute();
      long? top = this.ParseTop();
      long? skip = this.ParseSkip();
      long? index = this.ParseIndex();
      bool? count = this.ParseCount();
      string skipToken = this.ParseSkipToken();
      string deltaToken = this.ParseDeltaToken();
      List<QueryNode> customQueryOptions = new List<QueryNode>();
      return new ODataUri(this.ParameterAliasValueAccessor, path, (IEnumerable<QueryNode>) customQueryOptions, selectAndExpand, filter, orderBy, search, apply, skip, top, index, count, compute)
      {
        ServiceRoot = this.serviceRoot,
        SkipToken = skipToken,
        DeltaToken = deltaToken
      };
    }

    private ODataPath ParsePathImplementation()
    {
      Uri uri = this.uri;
      ExceptionUtils.CheckArgumentNotNull<Uri>(uri, "pathUri");
      return ODataPathFactory.BindPath((this.Container != null ? this.Container.GetService<UriPathParser>() : new UriPathParser(this.Settings)).ParsePathIntoSegments(uri, this.ServiceRoot), this.configuration);
    }

    private void Initialize()
    {
      if (this.odataPath != null)
        return;
      if (this.ParameterAliasValueAccessor == null)
        this.ParameterAliasValueAccessor = new ParameterAliasValueAccessor((IDictionary<string, string>) this.queryOptions.GetParameterAliases());
      this.odataPath = this.ParsePathImplementation();
      this.InitQueryOptionDic();
      this.queryOptionParser = new ODataQueryOptionParser(this.Model, this.odataPath, this.queryOptionDic)
      {
        Configuration = this.configuration
      };
    }

    private void InitQueryOptionDic()
    {
      if (this.queryOptionDic != null)
        return;
      this.queryOptionDic = (IDictionary<string, string>) new Dictionary<string, string>((IEqualityComparer<string>) StringComparer.Ordinal);
      this.customQueryOptions = (IList<KeyValuePair<string, string>>) new List<KeyValuePair<string, string>>();
      if (this.queryOptions == null)
        return;
      foreach (CustomQueryOptionToken queryOption in this.queryOptions)
      {
        string name = queryOption.Name;
        if (name != null)
        {
          string str = this.EnableNoDollarQueryOptions && !queryOption.Name.StartsWith("$", StringComparison.Ordinal) ? "$" + name : name;
          if (this.IsODataQueryOption(str))
          {
            if (this.queryOptionDic.ContainsKey(str))
            {
              string p0;
              if (!this.EnableNoDollarQueryOptions)
                p0 = str;
              else
                p0 = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "${0}/{0}", new object[1]
                {
                  (object) str.TrimStart('$')
                });
              throw new ODataException(Microsoft.OData.Strings.QueryOptionUtils_QueryParameterMustBeSpecifiedOnce((object) p0));
            }
            this.queryOptionDic.Add(str, queryOption.Value);
          }
          else
            this.customQueryOptions.Add(new KeyValuePair<string, string>(name, queryOption.Value));
        }
      }
    }

    private bool IsODataQueryOption(string optionName)
    {
      switch (this.Resolver.EnableCaseInsensitive ? optionName.ToLowerInvariant() : optionName)
      {
        case "$apply":
        case "$compute":
        case "$count":
        case "$deltatoken":
        case "$expand":
        case "$filter":
        case "$format":
        case "$id":
        case "$index":
        case "$orderby":
        case "$search":
        case "$select":
        case "$skip":
        case "$skiptoken":
        case "$top":
          return true;
        default:
          return false;
      }
    }
  }
}
