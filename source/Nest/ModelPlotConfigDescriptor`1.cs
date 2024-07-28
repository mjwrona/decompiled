// Decompiled with JetBrains decompiler
// Type: Nest.ModelPlotConfigDescriptor`1
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;

namespace Nest
{
  public class ModelPlotConfigDescriptor<T> : 
    DescriptorBase<ModelPlotConfigDescriptor<T>, IModelPlotConfig>,
    IModelPlotConfig,
    IModelPlotConfigEnabled
    where T : class
  {
    bool? IModelPlotConfigEnabled.Enabled { get; set; }

    Fields IModelPlotConfig.Terms { get; set; }

    bool? IModelPlotConfig.AnnotationsEnabled { get; set; }

    public ModelPlotConfigDescriptor<T> Enabled(bool? enabled = true) => this.Assign<bool?>(enabled, (Action<IModelPlotConfig, bool?>) ((a, v) => a.Enabled = v));

    public ModelPlotConfigDescriptor<T> Terms(Func<FieldsDescriptor<T>, IPromise<Fields>> fields) => this.Assign<Func<FieldsDescriptor<T>, IPromise<Fields>>>(fields, (Action<IModelPlotConfig, Func<FieldsDescriptor<T>, IPromise<Fields>>>) ((a, v) => a.Terms = v != null ? v(new FieldsDescriptor<T>())?.Value : (Fields) null));

    public ModelPlotConfigDescriptor<T> Terms(Fields fields) => this.Assign<Fields>(fields, (Action<IModelPlotConfig, Fields>) ((a, v) => a.Terms = v));

    public ModelPlotConfigDescriptor<T> AnnotationsEnabled(bool? enabled = true) => this.Assign<bool?>(enabled, (Action<IModelPlotConfig, bool?>) ((a, v) => a.AnnotationsEnabled = v));
  }
}
