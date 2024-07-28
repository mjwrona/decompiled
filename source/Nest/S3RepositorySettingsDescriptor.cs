// Decompiled with JetBrains decompiler
// Type: Nest.S3RepositorySettingsDescriptor
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;

namespace Nest
{
  public class S3RepositorySettingsDescriptor : 
    DescriptorBase<S3RepositorySettingsDescriptor, IS3RepositorySettings>,
    IS3RepositorySettings,
    IRepositorySettings
  {
    public S3RepositorySettingsDescriptor(string bucket) => this.Self.Bucket = bucket;

    string IS3RepositorySettings.BasePath { get; set; }

    string IS3RepositorySettings.Bucket { get; set; }

    string IS3RepositorySettings.BufferSize { get; set; }

    string IS3RepositorySettings.CannedAcl { get; set; }

    string IS3RepositorySettings.ChunkSize { get; set; }

    string IS3RepositorySettings.Client { get; set; }

    bool? IS3RepositorySettings.Compress { get; set; }

    bool? IS3RepositorySettings.ServerSideEncryption { get; set; }

    string IS3RepositorySettings.StorageClass { get; set; }

    bool? IS3RepositorySettings.PathStyleAccess { get; set; }

    bool? IS3RepositorySettings.DisableChunkedEncoding { get; set; }

    bool? IS3RepositorySettings.ReadOnly { get; set; }

    string IS3RepositorySettings.MaxRestoreBytesPerSecond { get; set; }

    string IS3RepositorySettings.MaxSnapshotBytesPerSecond { get; set; }

    public S3RepositorySettingsDescriptor Bucket(string bucket) => this.Assign<string>(bucket, (Action<IS3RepositorySettings, string>) ((a, v) => a.Bucket = v));

    public S3RepositorySettingsDescriptor Client(string client) => this.Assign<string>(client, (Action<IS3RepositorySettings, string>) ((a, v) => a.Client = v));

    public S3RepositorySettingsDescriptor BasePath(string basePath) => this.Assign<string>(basePath, (Action<IS3RepositorySettings, string>) ((a, v) => a.BasePath = v));

    public S3RepositorySettingsDescriptor Compress(bool? compress = true) => this.Assign<bool?>(compress, (Action<IS3RepositorySettings, bool?>) ((a, v) => a.Compress = v));

    public S3RepositorySettingsDescriptor ChunkSize(string chunkSize) => this.Assign<string>(chunkSize, (Action<IS3RepositorySettings, string>) ((a, v) => a.ChunkSize = v));

    public S3RepositorySettingsDescriptor ServerSideEncryption(bool? serverSideEncryption = true) => this.Assign<bool?>(serverSideEncryption, (Action<IS3RepositorySettings, bool?>) ((a, v) => a.ServerSideEncryption = v));

    public S3RepositorySettingsDescriptor BufferSize(string bufferSize) => this.Assign<string>(bufferSize, (Action<IS3RepositorySettings, string>) ((a, v) => a.BufferSize = v));

    public S3RepositorySettingsDescriptor CannedAcl(string cannedAcl) => this.Assign<string>(cannedAcl, (Action<IS3RepositorySettings, string>) ((a, v) => a.CannedAcl = v));

    public S3RepositorySettingsDescriptor StorageClass(string storageClass) => this.Assign<string>(storageClass, (Action<IS3RepositorySettings, string>) ((a, v) => a.StorageClass = v));

    public S3RepositorySettingsDescriptor PathStyleAccess(bool? pathStyleAccess = true) => this.Assign<bool?>(pathStyleAccess, (Action<IS3RepositorySettings, bool?>) ((a, v) => a.PathStyleAccess = v));

    public S3RepositorySettingsDescriptor DisableChunkedEncoding(bool? disableChunkedEncoding = true) => this.Assign<bool?>(disableChunkedEncoding, (Action<IS3RepositorySettings, bool?>) ((a, v) => a.DisableChunkedEncoding = v));

    public S3RepositorySettingsDescriptor ReadOnly(bool? readOnly = true) => this.Assign<bool?>(readOnly, (Action<IS3RepositorySettings, bool?>) ((a, v) => a.ReadOnly = v));

    public S3RepositorySettingsDescriptor MaxRestoreBytesPerSecond(string maxRestoreBytesPerSecond) => this.Assign<string>(maxRestoreBytesPerSecond, (Action<IS3RepositorySettings, string>) ((a, v) => a.MaxRestoreBytesPerSecond = v));

    public S3RepositorySettingsDescriptor MaxSnapshotBytesPerSecond(string maxSnapshotBytesPerSecond) => this.Assign<string>(maxSnapshotBytesPerSecond, (Action<IS3RepositorySettings, string>) ((a, v) => a.MaxSnapshotBytesPerSecond = v));
  }
}
