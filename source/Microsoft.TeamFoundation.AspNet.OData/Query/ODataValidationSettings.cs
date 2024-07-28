// Decompiled with JetBrains decompiler
// Type: Microsoft.AspNet.OData.Query.ODataValidationSettings
// Assembly: Microsoft.TeamFoundation.AspNet.OData, Version=7.3.1.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 08758328-0988-4E6C-88EC-9BA90EA42587
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.AspNet.OData.dll

using Microsoft.AspNet.OData.Common;
using System.Collections.ObjectModel;

namespace Microsoft.AspNet.OData.Query
{
  public class ODataValidationSettings
  {
    private const int MinMaxSkip = 0;
    private const int MinMaxTop = 0;
    private const int MinMaxExpansionDepth = 0;
    private const int MinMaxNodeCount = 1;
    private const int MinMaxAnyAllExpressionDepth = 1;
    private const int MinMaxOrderByNodeCount = 1;
    internal const int DefaultMaxExpansionDepth = 2;
    private AllowedArithmeticOperators _allowedArithmeticOperators = AllowedArithmeticOperators.All;
    private AllowedFunctions _allowedFunctions = AllowedFunctions.AllFunctions;
    private AllowedLogicalOperators _allowedLogicalOperators = AllowedLogicalOperators.All;
    private AllowedQueryOptions _allowedQueryParameters = AllowedQueryOptions.Supported;
    private Collection<string> _allowedOrderByProperties = new Collection<string>();
    private int? _maxSkip;
    private int? _maxTop;
    private int _maxAnyAllExpressionDepth = 1;
    private int _maxNodeCount = 100;
    private int _maxExpansionDepth = 2;
    private int _maxOrderByNodeCount = 5;

    public AllowedArithmeticOperators AllowedArithmeticOperators
    {
      get => this._allowedArithmeticOperators;
      set => this._allowedArithmeticOperators = value <= AllowedArithmeticOperators.All && value >= AllowedArithmeticOperators.None ? value : throw Error.InvalidEnumArgument(nameof (value), (int) value, typeof (AllowedArithmeticOperators));
    }

    public AllowedFunctions AllowedFunctions
    {
      get => this._allowedFunctions;
      set => this._allowedFunctions = value <= AllowedFunctions.AllFunctions && value >= AllowedFunctions.None ? value : throw Error.InvalidEnumArgument(nameof (value), (int) value, typeof (AllowedFunctions));
    }

    public AllowedLogicalOperators AllowedLogicalOperators
    {
      get => this._allowedLogicalOperators;
      set => this._allowedLogicalOperators = value <= AllowedLogicalOperators.All && value >= AllowedLogicalOperators.None ? value : throw Error.InvalidEnumArgument(nameof (value), (int) value, typeof (AllowedLogicalOperators));
    }

    public Collection<string> AllowedOrderByProperties => this._allowedOrderByProperties;

    public AllowedQueryOptions AllowedQueryOptions
    {
      get => this._allowedQueryParameters;
      set => this._allowedQueryParameters = value <= AllowedQueryOptions.All && value >= AllowedQueryOptions.None ? value : throw Error.InvalidEnumArgument(nameof (value), (int) value, typeof (AllowedQueryOptions));
    }

    public int MaxOrderByNodeCount
    {
      get => this._maxOrderByNodeCount;
      set => this._maxOrderByNodeCount = value >= 1 ? value : throw Error.ArgumentMustBeGreaterThanOrEqualTo(nameof (value), (object) value, (object) 1);
    }

    public int MaxAnyAllExpressionDepth
    {
      get => this._maxAnyAllExpressionDepth;
      set => this._maxAnyAllExpressionDepth = value >= 1 ? value : throw Error.ArgumentMustBeGreaterThanOrEqualTo(nameof (value), (object) value, (object) 1);
    }

    public int MaxNodeCount
    {
      get => this._maxNodeCount;
      set => this._maxNodeCount = value >= 1 ? value : throw Error.ArgumentMustBeGreaterThanOrEqualTo(nameof (value), (object) value, (object) 1);
    }

    public int? MaxSkip
    {
      get => this._maxSkip;
      set
      {
        if (value.HasValue)
        {
          int? nullable = value;
          int num = 0;
          if (nullable.GetValueOrDefault() < num & nullable.HasValue)
            throw Error.ArgumentMustBeGreaterThanOrEqualTo(nameof (value), (object) value, (object) 0);
        }
        this._maxSkip = value;
      }
    }

    public int? MaxTop
    {
      get => this._maxTop;
      set
      {
        if (value.HasValue)
        {
          int? nullable = value;
          int num = 0;
          if (nullable.GetValueOrDefault() < num & nullable.HasValue)
            throw Error.ArgumentMustBeGreaterThanOrEqualTo(nameof (value), (object) value, (object) 0);
        }
        this._maxTop = value;
      }
    }

    public int MaxExpansionDepth
    {
      get => this._maxExpansionDepth;
      set => this._maxExpansionDepth = value >= 0 ? value : throw Error.ArgumentMustBeGreaterThanOrEqualTo(nameof (value), (object) value, (object) 0);
    }
  }
}
