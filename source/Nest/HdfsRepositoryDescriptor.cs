// Decompiled with JetBrains decompiler
// Type: Nest.HdfsRepositoryDescriptor
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;

namespace Nest
{
  public class HdfsRepositoryDescriptor : 
    DescriptorBase<HdfsRepositoryDescriptor, IHdfsRepository>,
    IHdfsRepository,
    IRepository<IHdfsRepositorySettings>,
    IRepositoryWithSettings,
    ISnapshotRepository
  {
    IHdfsRepositorySettings IRepository<IHdfsRepositorySettings>.Settings { get; set; }

    object IRepositoryWithSettings.DelegateSettings => (object) this.Self.Settings;

    string ISnapshotRepository.Type => "hdfs";

    public HdfsRepositoryDescriptor Settings(
      string path,
      Func<HdfsRepositorySettingsDescriptor, IHdfsRepositorySettings> settingsSelector = null)
    {
      return this.Assign<IHdfsRepositorySettings>(settingsSelector.InvokeOrDefault<HdfsRepositorySettingsDescriptor, IHdfsRepositorySettings>(new HdfsRepositorySettingsDescriptor().Path(path)), (Action<IHdfsRepository, IHdfsRepositorySettings>) ((a, v) => a.Settings = v));
    }
  }
}
