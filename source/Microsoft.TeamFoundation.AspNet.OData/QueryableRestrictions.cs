// Decompiled with JetBrains decompiler
// Type: Microsoft.AspNet.OData.QueryableRestrictions
// Assembly: Microsoft.TeamFoundation.AspNet.OData, Version=7.3.1.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 08758328-0988-4E6C-88EC-9BA90EA42587
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.AspNet.OData.dll

using Microsoft.AspNet.OData.Builder;

namespace Microsoft.AspNet.OData
{
  public class QueryableRestrictions
  {
    private bool _autoExpand;

    public QueryableRestrictions()
    {
    }

    public QueryableRestrictions(PropertyConfiguration propertyConfiguration)
    {
      this.NotFilterable = propertyConfiguration.NotFilterable;
      this.NotSortable = propertyConfiguration.NotSortable;
      this.NotNavigable = propertyConfiguration.NotNavigable;
      this.NotExpandable = propertyConfiguration.NotExpandable;
      this.NotCountable = propertyConfiguration.NotCountable;
      this.DisableAutoExpandWhenSelectIsPresent = propertyConfiguration.DisableAutoExpandWhenSelectIsPresent;
      this._autoExpand = propertyConfiguration.AutoExpand;
    }

    public bool NotFilterable { get; set; }

    public bool NonFilterable
    {
      get => this.NotFilterable;
      set => this.NotFilterable = value;
    }

    public bool NotSortable { get; set; }

    public bool Unsortable
    {
      get => this.NotSortable;
      set => this.NotSortable = value;
    }

    public bool NotNavigable { get; set; }

    public bool NotExpandable { get; set; }

    public bool NotCountable { get; set; }

    public bool AutoExpand
    {
      get => !this.NotExpandable && this._autoExpand;
      set => this._autoExpand = value;
    }

    public bool DisableAutoExpandWhenSelectIsPresent { get; set; }
  }
}
