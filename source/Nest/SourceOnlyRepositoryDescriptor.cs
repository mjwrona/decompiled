// Decompiled with JetBrains decompiler
// Type: Nest.SourceOnlyRepositoryDescriptor
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;

namespace Nest
{
  public class SourceOnlyRepositoryDescriptor : 
    DescriptorBase<SourceOnlyRepositoryDescriptor, ISourceOnlyRepository>,
    ISourceOnlyRepository,
    IRepositoryWithSettings,
    ISnapshotRepository
  {
    private object _delegateSettings;
    private string _delegateType;

    object IRepositoryWithSettings.DelegateSettings => this._delegateSettings;

    string ISourceOnlyRepository.DelegateType => this._delegateType;

    string ISnapshotRepository.Type { get; } = "source";

    private SourceOnlyRepositoryDescriptor DelegateTo<TDescriptor>(
      Func<TDescriptor, IRepositoryWithSettings> selector)
      where TDescriptor : IRepositoryWithSettings, new()
    {
      return this.Custom(selector != null ? selector(new TDescriptor()) : (IRepositoryWithSettings) null);
    }

    public SourceOnlyRepositoryDescriptor FileSystem(
      Func<FileSystemRepositoryDescriptor, IFileSystemRepository> selector)
    {
      return this.DelegateTo<FileSystemRepositoryDescriptor>((Func<FileSystemRepositoryDescriptor, IRepositoryWithSettings>) selector);
    }

    public SourceOnlyRepositoryDescriptor ReadOnlyUrl(
      Func<ReadOnlyUrlRepositoryDescriptor, IReadOnlyUrlRepository> selector)
    {
      return this.DelegateTo<ReadOnlyUrlRepositoryDescriptor>((Func<ReadOnlyUrlRepositoryDescriptor, IRepositoryWithSettings>) selector);
    }

    public SourceOnlyRepositoryDescriptor Azure(
      Func<AzureRepositoryDescriptor, IAzureRepository> selector = null)
    {
      return this.DelegateTo<AzureRepositoryDescriptor>((Func<AzureRepositoryDescriptor, IRepositoryWithSettings>) selector);
    }

    public SourceOnlyRepositoryDescriptor Hdfs(
      Func<HdfsRepositoryDescriptor, IHdfsRepository> selector)
    {
      return this.DelegateTo<HdfsRepositoryDescriptor>((Func<HdfsRepositoryDescriptor, IRepositoryWithSettings>) selector);
    }

    public SourceOnlyRepositoryDescriptor S3(
      Func<S3RepositoryDescriptor, IS3Repository> selector)
    {
      return this.DelegateTo<S3RepositoryDescriptor>((Func<S3RepositoryDescriptor, IRepositoryWithSettings>) selector);
    }

    public SourceOnlyRepositoryDescriptor Custom(IRepositoryWithSettings repository)
    {
      this._delegateType = repository?.Type;
      this._delegateSettings = repository?.DelegateSettings;
      return this;
    }
  }
}
