// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.ODataUri
// Assembly: Microsoft.TeamFoundation.OData.Core, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6619C7F6-E44A-4143-AE77-6D570F968D9A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Core.dll

using Microsoft.OData.UriParser;
using Microsoft.OData.UriParser.Aggregation;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Microsoft.OData
{
  public sealed class ODataUri
  {
    private static readonly Uri MetadataSegment = new Uri("$metadata", UriKind.Relative);
    private Uri serviceRoot;

    public ODataUri() => this.ParameterAliasValueAccessor = new ParameterAliasValueAccessor((IDictionary<string, string>) new Dictionary<string, string>((IEqualityComparer<string>) StringComparer.Ordinal));

    internal ODataUri(
      ParameterAliasValueAccessor parameterAliasValueAccessor,
      ODataPath path,
      IEnumerable<QueryNode> customQueryOptions,
      SelectExpandClause selectAndExpand,
      FilterClause filter,
      OrderByClause orderby,
      SearchClause search,
      ApplyClause apply,
      long? skip,
      long? top,
      long? index,
      bool? queryCount,
      ComputeClause compute = null)
    {
      this.ParameterAliasValueAccessor = parameterAliasValueAccessor;
      this.Path = path;
      this.CustomQueryOptions = (IEnumerable<QueryNode>) new ReadOnlyCollection<QueryNode>((IList<QueryNode>) customQueryOptions.ToList<QueryNode>());
      this.SelectAndExpand = selectAndExpand;
      this.Filter = filter;
      this.OrderBy = orderby;
      this.Search = search;
      this.Apply = apply;
      this.Skip = skip;
      this.Top = top;
      this.Index = index;
      this.QueryCount = queryCount;
      this.Compute = compute;
    }

    public Uri RequestUri { get; set; }

    public Uri ServiceRoot
    {
      get => this.serviceRoot;
      set
      {
        if (value == (Uri) null)
        {
          this.serviceRoot = (Uri) null;
          this.MetadataDocumentUri = (Uri) null;
        }
        else
        {
          this.serviceRoot = value.IsAbsoluteUri ? UriUtils.EnsureTaillingSlash(value) : throw new ODataException(Strings.WriterValidationUtils_MessageWriterSettingsServiceDocumentUriMustBeNullOrAbsolute((object) UriUtils.UriToString(value)));
          this.MetadataDocumentUri = new Uri(this.serviceRoot, ODataUri.MetadataSegment);
        }
      }
    }

    public IDictionary<string, SingleValueNode> ParameterAliasNodes => this.ParameterAliasValueAccessor == null ? (IDictionary<string, SingleValueNode>) null : this.ParameterAliasValueAccessor.ParameterAliasValueNodesCached;

    public ODataPath Path { get; set; }

    public IEnumerable<QueryNode> CustomQueryOptions { get; set; }

    public SelectExpandClause SelectAndExpand { get; set; }

    public FilterClause Filter { get; set; }

    public OrderByClause OrderBy { get; set; }

    public SearchClause Search { get; set; }

    public ApplyClause Apply { get; set; }

    public ComputeClause Compute { get; set; }

    public long? Skip { get; set; }

    public long? Top { get; set; }

    public long? Index { get; set; }

    public bool? QueryCount { get; set; }

    public string SkipToken { get; set; }

    public string DeltaToken { get; set; }

    internal Uri MetadataDocumentUri { get; private set; }

    internal ParameterAliasValueAccessor ParameterAliasValueAccessor { get; set; }

    public ODataUri Clone() => new ODataUri()
    {
      RequestUri = this.RequestUri,
      serviceRoot = this.ServiceRoot,
      MetadataDocumentUri = this.MetadataDocumentUri,
      ParameterAliasValueAccessor = this.ParameterAliasValueAccessor,
      Path = this.Path,
      CustomQueryOptions = this.CustomQueryOptions,
      SelectAndExpand = this.SelectAndExpand,
      Apply = this.Apply,
      Filter = this.Filter,
      OrderBy = this.OrderBy,
      Search = this.Search,
      Skip = this.Skip,
      Top = this.Top,
      Index = this.Index,
      QueryCount = this.QueryCount,
      SkipToken = this.SkipToken,
      DeltaToken = this.DeltaToken
    };
  }
}
