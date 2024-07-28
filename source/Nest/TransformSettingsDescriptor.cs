// Decompiled with JetBrains decompiler
// Type: Nest.TransformSettingsDescriptor
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;

namespace Nest
{
  public class TransformSettingsDescriptor : 
    DescriptorBase<TransformSettingsDescriptor, ITransformSettings>,
    ITransformSettings
  {
    float? ITransformSettings.DocsPerSecond { get; set; }

    int? ITransformSettings.MaxPageSearchSize { get; set; }

    bool? ITransformSettings.DatesAsEpochMilliseconds { get; set; }

    public TransformSettingsDescriptor DocsPerSecond(float? docsPerSecond) => this.Assign<float?>(docsPerSecond, (Action<ITransformSettings, float?>) ((a, v) => a.DocsPerSecond = v));

    public TransformSettingsDescriptor MaxPageSearchSize(int? maxPageSearchSize) => this.Assign<int?>(maxPageSearchSize, (Action<ITransformSettings, int?>) ((a, v) => a.MaxPageSearchSize = v));

    public TransformSettingsDescriptor DatesAsEpochMilliseconds(bool? datesAsEpochMillis = true) => this.Assign<bool?>(datesAsEpochMillis, (Action<ITransformSettings, bool?>) ((a, v) => a.DatesAsEpochMilliseconds = v));
  }
}
