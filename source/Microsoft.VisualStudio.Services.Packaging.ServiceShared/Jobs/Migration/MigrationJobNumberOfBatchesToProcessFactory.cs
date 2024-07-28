// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.Jobs.Migration.MigrationJobNumberOfBatchesToProcessFactory
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns.Interfaces;

namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared.Jobs.Migration
{
  public class MigrationJobNumberOfBatchesToProcessFactory : IFactory<int>
  {
    public const int HostedNumBatchesPerJob = 1;
    public const int OnPremNumBatchesPerJob = 2147483647;
    private readonly IExecutionEnvironment executionEnvironment;

    public MigrationJobNumberOfBatchesToProcessFactory(IExecutionEnvironment executionEnvironment) => this.executionEnvironment = executionEnvironment;

    public int Get() => !this.executionEnvironment.IsHosted() ? int.MaxValue : 1;
  }
}
