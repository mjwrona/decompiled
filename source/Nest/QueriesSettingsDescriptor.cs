// Decompiled with JetBrains decompiler
// Type: Nest.QueriesSettingsDescriptor
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;

namespace Nest
{
  public class QueriesSettingsDescriptor : 
    DescriptorBase<QueriesSettingsDescriptor, IQueriesSettings>,
    IQueriesSettings
  {
    IQueriesCacheSettings IQueriesSettings.Cache { get; set; }

    public QueriesSettingsDescriptor Cache(
      Func<QueriesCacheSettingsDescriptor, IQueriesCacheSettings> selector)
    {
      return this.Assign<IQueriesCacheSettings>(selector(new QueriesCacheSettingsDescriptor()), (Action<IQueriesSettings, IQueriesCacheSettings>) ((a, v) => a.Cache = v));
    }
  }
}
