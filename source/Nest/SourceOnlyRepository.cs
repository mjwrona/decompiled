// Decompiled with JetBrains decompiler
// Type: Nest.SourceOnlyRepository
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;

namespace Nest
{
  public class SourceOnlyRepository : 
    ISourceOnlyRepository,
    IRepositoryWithSettings,
    ISnapshotRepository
  {
    private readonly object _delegateSettings;
    private readonly string _delegateType;

    internal SourceOnlyRepository()
    {
    }

    internal SourceOnlyRepository(string delegateType, object settings)
    {
      this._delegateType = delegateType;
      this._delegateSettings = settings;
    }

    public SourceOnlyRepository(IRepositoryWithSettings repositoryToDelegateTo)
    {
      this._delegateType = repositoryToDelegateTo != null ? repositoryToDelegateTo.Type : throw new ArgumentNullException(nameof (repositoryToDelegateTo));
      this._delegateSettings = repositoryToDelegateTo.DelegateSettings;
    }

    object IRepositoryWithSettings.DelegateSettings => this._delegateSettings;

    string ISourceOnlyRepository.DelegateType => this._delegateType;

    string ISnapshotRepository.Type { get; } = "source";
  }
}
