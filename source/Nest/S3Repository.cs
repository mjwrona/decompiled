// Decompiled with JetBrains decompiler
// Type: Nest.S3Repository
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

namespace Nest
{
  public class S3Repository : 
    IS3Repository,
    IRepository<IS3RepositorySettings>,
    IRepositoryWithSettings,
    ISnapshotRepository
  {
    public S3Repository(IS3RepositorySettings settings) => this.Settings = settings;

    public IS3RepositorySettings Settings { get; set; }

    object IRepositoryWithSettings.DelegateSettings => (object) this.Settings;

    public string Type { get; } = "s3";
  }
}
