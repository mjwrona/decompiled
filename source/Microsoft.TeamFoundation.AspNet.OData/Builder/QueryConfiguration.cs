// Decompiled with JetBrains decompiler
// Type: Microsoft.AspNet.OData.Builder.QueryConfiguration
// Assembly: Microsoft.TeamFoundation.AspNet.OData, Version=7.3.1.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 08758328-0988-4E6C-88EC-9BA90EA42587
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.AspNet.OData.dll

using Microsoft.AspNet.OData.Query;
using System.Collections.Generic;

namespace Microsoft.AspNet.OData.Builder
{
  public class QueryConfiguration
  {
    private ModelBoundQuerySettings _querySettings;

    public ModelBoundQuerySettings ModelBoundQuerySettings
    {
      get => this._querySettings;
      set => this._querySettings = value;
    }

    public virtual void SetCount(bool enableCount) => this.GetModelBoundQuerySettingsOrDefault().Countable = new bool?(enableCount);

    public virtual void SetMaxTop(int? maxTop) => this.GetModelBoundQuerySettingsOrDefault().MaxTop = maxTop;

    public virtual void SetPageSize(int? pageSize) => this.GetModelBoundQuerySettingsOrDefault().PageSize = pageSize;

    public virtual void SetExpand(
      IEnumerable<string> properties,
      int? maxDepth,
      SelectExpandType expandType)
    {
      this.GetModelBoundQuerySettingsOrDefault();
      if (properties == null)
      {
        this.ModelBoundQuerySettings.DefaultExpandType = new SelectExpandType?(expandType);
        this.ModelBoundQuerySettings.DefaultMaxDepth = maxDepth ?? 2;
      }
      else
      {
        foreach (string property in properties)
          this.ModelBoundQuerySettings.ExpandConfigurations[property] = new ExpandConfiguration()
          {
            ExpandType = expandType,
            MaxDepth = maxDepth ?? 2
          };
      }
    }

    public virtual void SetSelect(IEnumerable<string> properties, SelectExpandType selectType)
    {
      this.GetModelBoundQuerySettingsOrDefault();
      if (properties == null)
      {
        this.ModelBoundQuerySettings.DefaultSelectType = new SelectExpandType?(selectType);
      }
      else
      {
        foreach (string property in properties)
          this.ModelBoundQuerySettings.SelectConfigurations[property] = selectType;
      }
    }

    public virtual void SetOrderBy(IEnumerable<string> properties, bool enableOrderBy)
    {
      this.GetModelBoundQuerySettingsOrDefault();
      if (properties == null)
      {
        this.ModelBoundQuerySettings.DefaultEnableOrderBy = new bool?(enableOrderBy);
      }
      else
      {
        foreach (string property in properties)
          this.ModelBoundQuerySettings.OrderByConfigurations[property] = enableOrderBy;
      }
    }

    public virtual void SetFilter(IEnumerable<string> properties, bool enableFilter)
    {
      this.GetModelBoundQuerySettingsOrDefault();
      if (properties == null)
      {
        this.ModelBoundQuerySettings.DefaultEnableFilter = new bool?(enableFilter);
      }
      else
      {
        foreach (string property in properties)
          this.ModelBoundQuerySettings.FilterConfigurations[property] = enableFilter;
      }
    }

    internal ModelBoundQuerySettings GetModelBoundQuerySettingsOrDefault()
    {
      if (this._querySettings == null)
        this._querySettings = new ModelBoundQuerySettings(ModelBoundQuerySettings.DefaultModelBoundQuerySettings);
      return this._querySettings;
    }
  }
}
