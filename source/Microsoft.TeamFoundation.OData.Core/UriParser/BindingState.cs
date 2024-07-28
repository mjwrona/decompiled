// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.UriParser.BindingState
// Assembly: Microsoft.TeamFoundation.OData.Core, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6619C7F6-E44A-4143-AE77-6D570F968D9A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Core.dll

using Microsoft.OData.Edm;
using System.Collections.Generic;

namespace Microsoft.OData.UriParser
{
  internal sealed class BindingState
  {
    private readonly ODataUriParserConfiguration configuration;
    private readonly Stack<RangeVariable> rangeVariables = new Stack<RangeVariable>();
    private RangeVariable implicitRangeVariable;
    private int BindingRecursionDepth;
    private List<CustomQueryOptionToken> queryOptions;
    private List<ODataPathSegment> parsedSegments = new List<ODataPathSegment>();

    internal BindingState(ODataUriParserConfiguration configuration)
    {
      ExceptionUtils.CheckArgumentNotNull<ODataUriParserConfiguration>(configuration, nameof (configuration));
      this.configuration = configuration;
      this.BindingRecursionDepth = 0;
    }

    internal BindingState(
      ODataUriParserConfiguration configuration,
      List<ODataPathSegment> parsedSegments)
    {
      ExceptionUtils.CheckArgumentNotNull<ODataUriParserConfiguration>(configuration, nameof (configuration));
      this.configuration = configuration;
      this.BindingRecursionDepth = 0;
      this.parsedSegments = parsedSegments;
    }

    internal IEdmModel Model => this.configuration.Model;

    internal ODataUriParserConfiguration Configuration => this.configuration;

    internal RangeVariable ImplicitRangeVariable
    {
      get => this.implicitRangeVariable;
      set => this.implicitRangeVariable = value;
    }

    internal Stack<RangeVariable> RangeVariables => this.rangeVariables;

    internal List<CustomQueryOptionToken> QueryOptions
    {
      get => this.queryOptions;
      set => this.queryOptions = value;
    }

    internal HashSet<EndPathToken> AggregatedPropertyNames { get; set; }

    internal bool IsCollapsed { get; set; }

    internal bool InEntitySetAggregation { get; set; }

    internal List<ODataPathSegment> ParsedSegments => this.parsedSegments;

    internal void RecurseEnter()
    {
      ++this.BindingRecursionDepth;
      if (this.BindingRecursionDepth > this.configuration.Settings.FilterLimit)
        throw new ODataException(Microsoft.OData.Strings.UriQueryExpressionParser_TooDeep);
    }

    internal void RecurseLeave() => --this.BindingRecursionDepth;
  }
}
