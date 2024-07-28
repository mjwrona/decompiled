// Decompiled with JetBrains decompiler
// Type: Nest.SortingSettingsDescriptor`1
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;

namespace Nest
{
  public class SortingSettingsDescriptor<T> : 
    DescriptorBase<SortingSettingsDescriptor<T>, ISortingSettings>,
    ISortingSettings
    where T : class
  {
    Nest.Fields ISortingSettings.Fields { get; set; }

    IndexSortMissing[] ISortingSettings.Missing { get; set; }

    IndexSortMode[] ISortingSettings.Mode { get; set; }

    IndexSortOrder[] ISortingSettings.Order { get; set; }

    public SortingSettingsDescriptor<T> Fields(Func<FieldsDescriptor<T>, IPromise<Nest.Fields>> fields) => this.Assign<Func<FieldsDescriptor<T>, IPromise<Nest.Fields>>>(fields, (Action<ISortingSettings, Func<FieldsDescriptor<T>, IPromise<Nest.Fields>>>) ((a, v) => a.Fields = v != null ? v(new FieldsDescriptor<T>())?.Value : (Nest.Fields) null));

    public SortingSettingsDescriptor<T> Fields(Nest.Fields fields) => this.Assign<Nest.Fields>(fields, (Action<ISortingSettings, Nest.Fields>) ((a, v) => a.Fields = v));

    public SortingSettingsDescriptor<T> Order(params IndexSortOrder[] order) => this.Assign<IndexSortOrder[]>(order, (Action<ISortingSettings, IndexSortOrder[]>) ((a, v) => a.Order = v));

    public SortingSettingsDescriptor<T> Mode(params IndexSortMode[] mode) => this.Assign<IndexSortMode[]>(mode, (Action<ISortingSettings, IndexSortMode[]>) ((a, v) => a.Mode = v));

    public SortingSettingsDescriptor<T> Missing(params IndexSortMissing[] missing) => this.Assign<IndexSortMissing[]>(missing, (Action<ISortingSettings, IndexSortMissing[]>) ((a, v) => a.Missing = v));
  }
}
