// Decompiled with JetBrains decompiler
// Type: Nest.S3RepositoryDescriptor
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;

namespace Nest
{
  public class S3RepositoryDescriptor : 
    DescriptorBase<S3RepositoryDescriptor, IS3Repository>,
    IS3Repository,
    IRepository<IS3RepositorySettings>,
    IRepositoryWithSettings,
    ISnapshotRepository
  {
    IS3RepositorySettings IRepository<IS3RepositorySettings>.Settings { get; set; }

    object IRepositoryWithSettings.DelegateSettings => (object) this.Self.Settings;

    string ISnapshotRepository.Type { get; } = "s3";

    public S3RepositoryDescriptor Settings(
      string bucket,
      Func<S3RepositorySettingsDescriptor, IS3RepositorySettings> settingsSelector = null)
    {
      return this.Assign<IS3RepositorySettings>(settingsSelector.InvokeOrDefault<S3RepositorySettingsDescriptor, IS3RepositorySettings>(new S3RepositorySettingsDescriptor(bucket)), (Action<IS3Repository, IS3RepositorySettings>) ((a, v) => a.Settings = v));
    }
  }
}
