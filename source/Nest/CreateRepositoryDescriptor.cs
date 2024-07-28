// Decompiled with JetBrains decompiler
// Type: Nest.CreateRepositoryDescriptor
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net;
using Elasticsearch.Net.Specification.SnapshotApi;
using Elasticsearch.Net.Utf8Json;
using System;

namespace Nest
{
  public class CreateRepositoryDescriptor : 
    RequestDescriptorBase<CreateRepositoryDescriptor, CreateRepositoryRequestParameters, ICreateRepositoryRequest>,
    ICreateRepositoryRequest,
    IRequest<CreateRepositoryRequestParameters>,
    IRequest
  {
    internal override ApiUrls ApiUrls => ApiUrlsLookups.SnapshotCreateRepository;

    public CreateRepositoryDescriptor(Name repository)
      : base((Func<RouteValues, RouteValues>) (r => r.Required(nameof (repository), (IUrlParameter) repository)))
    {
    }

    [SerializationConstructor]
    protected CreateRepositoryDescriptor()
    {
    }

    Name ICreateRepositoryRequest.RepositoryName => this.Self.RouteValues.Get<Name>("repository");

    public CreateRepositoryDescriptor MasterTimeout(Time mastertimeout) => this.Qs("master_timeout", (object) mastertimeout);

    public CreateRepositoryDescriptor Timeout(Time timeout) => this.Qs(nameof (timeout), (object) timeout);

    public CreateRepositoryDescriptor Verify(bool? verify = true) => this.Qs(nameof (verify), (object) verify);

    ISnapshotRepository ICreateRepositoryRequest.Repository { get; set; }

    public CreateRepositoryDescriptor FileSystem(
      Func<FileSystemRepositoryDescriptor, IFileSystemRepository> selector)
    {
      return this.Assign<Func<FileSystemRepositoryDescriptor, IFileSystemRepository>>(selector, (Action<ICreateRepositoryRequest, Func<FileSystemRepositoryDescriptor, IFileSystemRepository>>) ((a, v) => a.Repository = v != null ? (ISnapshotRepository) v(new FileSystemRepositoryDescriptor()) : (ISnapshotRepository) null));
    }

    public CreateRepositoryDescriptor ReadOnlyUrl(
      Func<ReadOnlyUrlRepositoryDescriptor, IReadOnlyUrlRepository> selector)
    {
      return this.Assign<Func<ReadOnlyUrlRepositoryDescriptor, IReadOnlyUrlRepository>>(selector, (Action<ICreateRepositoryRequest, Func<ReadOnlyUrlRepositoryDescriptor, IReadOnlyUrlRepository>>) ((a, v) => a.Repository = v != null ? (ISnapshotRepository) v(new ReadOnlyUrlRepositoryDescriptor()) : (ISnapshotRepository) null));
    }

    public CreateRepositoryDescriptor Azure(
      Func<AzureRepositoryDescriptor, IAzureRepository> selector = null)
    {
      return this.Assign<IAzureRepository>(selector.InvokeOrDefault<AzureRepositoryDescriptor, IAzureRepository>(new AzureRepositoryDescriptor()), (Action<ICreateRepositoryRequest, IAzureRepository>) ((a, v) => a.Repository = (ISnapshotRepository) v));
    }

    public CreateRepositoryDescriptor Hdfs(
      Func<HdfsRepositoryDescriptor, IHdfsRepository> selector)
    {
      return this.Assign<Func<HdfsRepositoryDescriptor, IHdfsRepository>>(selector, (Action<ICreateRepositoryRequest, Func<HdfsRepositoryDescriptor, IHdfsRepository>>) ((a, v) => a.Repository = v != null ? (ISnapshotRepository) v(new HdfsRepositoryDescriptor()) : (ISnapshotRepository) null));
    }

    public CreateRepositoryDescriptor S3(
      Func<S3RepositoryDescriptor, IS3Repository> selector)
    {
      return this.Assign<Func<S3RepositoryDescriptor, IS3Repository>>(selector, (Action<ICreateRepositoryRequest, Func<S3RepositoryDescriptor, IS3Repository>>) ((a, v) => a.Repository = v != null ? (ISnapshotRepository) v(new S3RepositoryDescriptor()) : (ISnapshotRepository) null));
    }

    public CreateRepositoryDescriptor SourceOnly(
      Func<SourceOnlyRepositoryDescriptor, ISourceOnlyRepository> selector)
    {
      return this.Assign<Func<SourceOnlyRepositoryDescriptor, ISourceOnlyRepository>>(selector, (Action<ICreateRepositoryRequest, Func<SourceOnlyRepositoryDescriptor, ISourceOnlyRepository>>) ((a, v) => a.Repository = v != null ? (ISnapshotRepository) v(new SourceOnlyRepositoryDescriptor()) : (ISnapshotRepository) null));
    }

    public CreateRepositoryDescriptor Custom(ISnapshotRepository repository) => this.Assign<ISnapshotRepository>(repository, (Action<ICreateRepositoryRequest, ISnapshotRepository>) ((a, v) => a.Repository = v));
  }
}
