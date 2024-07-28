// Decompiled with JetBrains decompiler
// Type: Microsoft.AspNet.OData.Query.ODataQuerySettings
// Assembly: Microsoft.TeamFoundation.AspNet.OData, Version=7.3.1.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 08758328-0988-4E6C-88EC-9BA90EA42587
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.AspNet.OData.dll

using Microsoft.AspNet.OData.Common;

namespace Microsoft.AspNet.OData.Query
{
  public class ODataQuerySettings
  {
    private HandleNullPropagationOption _handleNullPropagationOption;
    private int? _pageSize;
    private int? _modelBoundPageSize;

    public ODataQuerySettings()
    {
      this.EnsureStableOrdering = true;
      this.EnableConstantParameterization = true;
    }

    internal int? ModelBoundPageSize
    {
      get => this._modelBoundPageSize;
      set
      {
        if (value.HasValue)
        {
          int? nullable = value;
          int num = 0;
          if (nullable.GetValueOrDefault() <= num & nullable.HasValue)
            throw Error.ArgumentMustBeGreaterThanOrEqualTo(nameof (value), (object) value, (object) 1);
        }
        this._modelBoundPageSize = value;
      }
    }

    public bool EnsureStableOrdering { get; set; }

    public HandleNullPropagationOption HandleNullPropagation
    {
      get => this._handleNullPropagationOption;
      set
      {
        HandleNullPropagationOptionHelper.Validate(value, nameof (value));
        this._handleNullPropagationOption = value;
      }
    }

    public bool EnableConstantParameterization { get; set; }

    public bool EnableCorrelatedSubqueryBuffering { get; set; }

    public int? PageSize
    {
      get => this._pageSize;
      set
      {
        if (value.HasValue)
        {
          int? nullable = value;
          int num = 0;
          if (nullable.GetValueOrDefault() <= num & nullable.HasValue)
            throw Error.ArgumentMustBeGreaterThanOrEqualTo(nameof (value), (object) value, (object) 1);
        }
        this._pageSize = value;
      }
    }

    public bool HandleReferenceNavigationPropertyExpandFilter { get; set; }

    public bool PostponePaging { get; set; }

    internal void CopyFrom(ODataQuerySettings settings)
    {
      this.EnsureStableOrdering = settings.EnsureStableOrdering;
      this.EnableConstantParameterization = settings.EnableConstantParameterization;
      this.HandleNullPropagation = settings.HandleNullPropagation;
      this.PageSize = settings.PageSize;
      this.ModelBoundPageSize = settings.ModelBoundPageSize;
      this.HandleReferenceNavigationPropertyExpandFilter = settings.HandleReferenceNavigationPropertyExpandFilter;
      this.EnableCorrelatedSubqueryBuffering = settings.EnableCorrelatedSubqueryBuffering;
    }
  }
}
