// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Storage.File.FileRequestOptions
// Assembly: Microsoft.Azure.Storage.File, Version=11.2.3.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: C68E95B0-8DFB-410C-8E70-706406D1A279
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Storage.File.dll

using Microsoft.Azure.Storage.Core.Executor;
using Microsoft.Azure.Storage.Core.Util;
using Microsoft.Azure.Storage.RetryPolicies;
using Microsoft.Azure.Storage.Shared.Protocol;
using System;

namespace Microsoft.Azure.Storage.File
{
  public sealed class FileRequestOptions : IRequestOptions
  {
    private int? parallelOperationThreadCount;
    private TimeSpan? maximumExecutionTime;
    internal static FileRequestOptions BaseDefaultRequestOptions = new FileRequestOptions()
    {
      RetryPolicy = (IRetryPolicy) new NoRetry(),
      LocationMode = new Microsoft.Azure.Storage.RetryPolicies.LocationMode?(Microsoft.Azure.Storage.RetryPolicies.LocationMode.PrimaryOnly),
      RequireEncryption = new bool?(),
      ServerTimeout = new TimeSpan?(),
      MaximumExecutionTime = new TimeSpan?(),
      NetworkTimeout = new TimeSpan?(Constants.DefaultNetworkTimeout),
      ParallelOperationThreadCount = new int?(1),
      ChecksumOptions = new ChecksumOptions()
      {
        DisableContentMD5Validation = new bool?(false),
        StoreContentMD5 = new bool?(false),
        UseTransactionalMD5 = new bool?(false),
        DisableContentCRC64Validation = new bool?(false),
        StoreContentCRC64 = new bool?(false),
        UseTransactionalCRC64 = new bool?(false)
      }
    };

    public FileRequestOptions()
    {
    }

    internal FileRequestOptions(FileRequestOptions other)
      : this()
    {
      if (other == null)
        return;
      this.RetryPolicy = other.RetryPolicy;
      this.RequireEncryption = other.RequireEncryption;
      this.LocationMode = other.LocationMode;
      this.ServerTimeout = other.ServerTimeout;
      this.MaximumExecutionTime = other.MaximumExecutionTime;
      this.NetworkTimeout = other.NetworkTimeout;
      this.OperationExpiryTime = other.OperationExpiryTime;
      this.ChecksumOptions.CopyFrom(other.ChecksumOptions);
      this.ParallelOperationThreadCount = other.ParallelOperationThreadCount;
    }

    internal static FileRequestOptions ApplyDefaults(
      FileRequestOptions options,
      CloudFileClient serviceClient,
      bool applyExpiry = true)
    {
      FileRequestOptions fileRequestOptions1 = new FileRequestOptions(options);
      fileRequestOptions1.RetryPolicy = fileRequestOptions1.RetryPolicy ?? serviceClient.DefaultRequestOptions.RetryPolicy ?? FileRequestOptions.BaseDefaultRequestOptions.RetryPolicy;
      fileRequestOptions1.LocationMode = fileRequestOptions1.LocationMode ?? serviceClient.DefaultRequestOptions.LocationMode ?? FileRequestOptions.BaseDefaultRequestOptions.LocationMode;
      FileRequestOptions fileRequestOptions2 = fileRequestOptions1;
      bool? nullable1 = fileRequestOptions1.RequireEncryption;
      bool? nullable2;
      bool? nullable3;
      if (!nullable1.HasValue)
      {
        nullable2 = serviceClient.DefaultRequestOptions.RequireEncryption;
        nullable3 = nullable2 ?? FileRequestOptions.BaseDefaultRequestOptions.RequireEncryption;
      }
      else
        nullable3 = nullable1;
      fileRequestOptions2.RequireEncryption = nullable3;
      FileRequestOptions fileRequestOptions3 = fileRequestOptions1;
      TimeSpan? nullable4 = fileRequestOptions1.ServerTimeout;
      TimeSpan? nullable5;
      TimeSpan? nullable6;
      if (!nullable4.HasValue)
      {
        nullable5 = serviceClient.DefaultRequestOptions.ServerTimeout;
        nullable6 = nullable5 ?? FileRequestOptions.BaseDefaultRequestOptions.ServerTimeout;
      }
      else
        nullable6 = nullable4;
      fileRequestOptions3.ServerTimeout = nullable6;
      FileRequestOptions fileRequestOptions4 = fileRequestOptions1;
      nullable4 = fileRequestOptions1.MaximumExecutionTime;
      TimeSpan? nullable7;
      if (!nullable4.HasValue)
      {
        nullable5 = serviceClient.DefaultRequestOptions.MaximumExecutionTime;
        nullable7 = nullable5 ?? FileRequestOptions.BaseDefaultRequestOptions.MaximumExecutionTime;
      }
      else
        nullable7 = nullable4;
      fileRequestOptions4.MaximumExecutionTime = nullable7;
      FileRequestOptions fileRequestOptions5 = fileRequestOptions1;
      nullable4 = fileRequestOptions1.NetworkTimeout;
      TimeSpan? nullable8;
      if (!nullable4.HasValue)
      {
        nullable5 = serviceClient.DefaultRequestOptions.NetworkTimeout;
        nullable8 = nullable5 ?? FileRequestOptions.BaseDefaultRequestOptions.NetworkTimeout;
      }
      else
        nullable8 = nullable4;
      fileRequestOptions5.NetworkTimeout = nullable8;
      fileRequestOptions1.ParallelOperationThreadCount = fileRequestOptions1.ParallelOperationThreadCount ?? serviceClient.DefaultRequestOptions.ParallelOperationThreadCount ?? FileRequestOptions.BaseDefaultRequestOptions.ParallelOperationThreadCount;
      if (applyExpiry && !fileRequestOptions1.OperationExpiryTime.HasValue)
      {
        nullable4 = fileRequestOptions1.MaximumExecutionTime;
        if (nullable4.HasValue)
        {
          FileRequestOptions fileRequestOptions6 = fileRequestOptions1;
          DateTime now = DateTime.Now;
          nullable4 = fileRequestOptions1.MaximumExecutionTime;
          TimeSpan timeSpan = nullable4.Value;
          DateTime? nullable9 = new DateTime?(now + timeSpan);
          fileRequestOptions6.OperationExpiryTime = nullable9;
        }
      }
      ChecksumOptions checksumOptions1 = fileRequestOptions1.ChecksumOptions;
      nullable1 = fileRequestOptions1.ChecksumOptions.DisableContentMD5Validation;
      bool? nullable10;
      if (!nullable1.HasValue)
      {
        nullable2 = serviceClient.DefaultRequestOptions.ChecksumOptions.DisableContentMD5Validation;
        nullable10 = nullable2 ?? FileRequestOptions.BaseDefaultRequestOptions.ChecksumOptions.DisableContentMD5Validation;
      }
      else
        nullable10 = nullable1;
      checksumOptions1.DisableContentMD5Validation = nullable10;
      ChecksumOptions checksumOptions2 = fileRequestOptions1.ChecksumOptions;
      nullable1 = fileRequestOptions1.ChecksumOptions.StoreContentMD5;
      bool? nullable11;
      if (!nullable1.HasValue)
      {
        nullable2 = serviceClient.DefaultRequestOptions.ChecksumOptions.StoreContentMD5;
        nullable11 = nullable2 ?? FileRequestOptions.BaseDefaultRequestOptions.ChecksumOptions.StoreContentMD5;
      }
      else
        nullable11 = nullable1;
      checksumOptions2.StoreContentMD5 = nullable11;
      ChecksumOptions checksumOptions3 = fileRequestOptions1.ChecksumOptions;
      nullable1 = fileRequestOptions1.ChecksumOptions.UseTransactionalMD5;
      bool? nullable12;
      if (!nullable1.HasValue)
      {
        nullable2 = serviceClient.DefaultRequestOptions.ChecksumOptions.UseTransactionalMD5;
        nullable12 = nullable2 ?? FileRequestOptions.BaseDefaultRequestOptions.ChecksumOptions.UseTransactionalMD5;
      }
      else
        nullable12 = nullable1;
      checksumOptions3.UseTransactionalMD5 = nullable12;
      ChecksumOptions checksumOptions4 = fileRequestOptions1.ChecksumOptions;
      nullable1 = fileRequestOptions1.ChecksumOptions.DisableContentCRC64Validation;
      bool? nullable13;
      if (!nullable1.HasValue)
      {
        nullable2 = serviceClient.DefaultRequestOptions.ChecksumOptions.DisableContentCRC64Validation;
        nullable13 = nullable2 ?? FileRequestOptions.BaseDefaultRequestOptions.ChecksumOptions.DisableContentCRC64Validation;
      }
      else
        nullable13 = nullable1;
      checksumOptions4.DisableContentCRC64Validation = nullable13;
      ChecksumOptions checksumOptions5 = fileRequestOptions1.ChecksumOptions;
      nullable1 = fileRequestOptions1.ChecksumOptions.StoreContentCRC64;
      bool? nullable14;
      if (!nullable1.HasValue)
      {
        nullable2 = serviceClient.DefaultRequestOptions.ChecksumOptions.StoreContentCRC64;
        nullable14 = nullable2 ?? FileRequestOptions.BaseDefaultRequestOptions.ChecksumOptions.StoreContentCRC64;
      }
      else
        nullable14 = nullable1;
      checksumOptions5.StoreContentCRC64 = nullable14;
      ChecksumOptions checksumOptions6 = fileRequestOptions1.ChecksumOptions;
      nullable1 = fileRequestOptions1.ChecksumOptions.UseTransactionalCRC64;
      bool? nullable15;
      if (!nullable1.HasValue)
      {
        nullable2 = serviceClient.DefaultRequestOptions.ChecksumOptions.UseTransactionalCRC64;
        nullable15 = nullable2 ?? FileRequestOptions.BaseDefaultRequestOptions.ChecksumOptions.UseTransactionalCRC64;
      }
      else
        nullable15 = nullable1;
      checksumOptions6.UseTransactionalCRC64 = nullable15;
      return fileRequestOptions1;
    }

    internal void ApplyToStorageCommand<T>(RESTCommand<T> cmd)
    {
      if (this.LocationMode.HasValue)
        cmd.LocationMode = this.LocationMode.Value;
      if (this.ServerTimeout.HasValue)
        cmd.ServerTimeoutInSeconds = new int?((int) this.ServerTimeout.Value.TotalSeconds);
      if (this.OperationExpiryTime.HasValue)
        cmd.OperationExpiryTime = this.OperationExpiryTime;
      else if (this.MaximumExecutionTime.HasValue)
        cmd.OperationExpiryTime = new DateTime?(DateTime.Now + this.MaximumExecutionTime.Value);
      cmd.NetworkTimeout = this.NetworkTimeout;
    }

    internal DateTime? OperationExpiryTime { get; set; }

    public IRetryPolicy RetryPolicy { get; set; }

    public Microsoft.Azure.Storage.RetryPolicies.LocationMode? LocationMode
    {
      get => new Microsoft.Azure.Storage.RetryPolicies.LocationMode?(Microsoft.Azure.Storage.RetryPolicies.LocationMode.PrimaryOnly);
      set
      {
        Microsoft.Azure.Storage.RetryPolicies.LocationMode? nullable = value;
        Microsoft.Azure.Storage.RetryPolicies.LocationMode locationMode = Microsoft.Azure.Storage.RetryPolicies.LocationMode.PrimaryOnly;
        if (!(nullable.GetValueOrDefault() == locationMode & nullable.HasValue))
          throw new NotSupportedException("This operation can only be executed against the primary storage location.");
      }
    }

    public bool? RequireEncryption
    {
      get => new bool?(false);
      set
      {
        if (value.HasValue && value.Value)
          throw new NotSupportedException("Encryption is not supported for files.");
      }
    }

    public TimeSpan? ServerTimeout { get; set; }

    public TimeSpan? MaximumExecutionTime
    {
      get => this.maximumExecutionTime;
      set
      {
        if (value.HasValue)
          CommonUtility.AssertInBounds<TimeSpan>(nameof (MaximumExecutionTime), value.Value, TimeSpan.Zero, Constants.MaxMaximumExecutionTime);
        this.maximumExecutionTime = value;
      }
    }

    public TimeSpan? NetworkTimeout { get; set; }

    public int? ParallelOperationThreadCount
    {
      get => this.parallelOperationThreadCount;
      set
      {
        if (value.HasValue)
          CommonUtility.AssertInBounds<int>(nameof (ParallelOperationThreadCount), value.Value, 1, 64);
        this.parallelOperationThreadCount = value;
      }
    }

    public bool? UseTransactionalMD5
    {
      get => this.ChecksumOptions.UseTransactionalMD5;
      set => this.ChecksumOptions.UseTransactionalMD5 = value;
    }

    public bool? StoreFileContentMD5
    {
      get => this.ChecksumOptions.StoreContentMD5;
      set => this.ChecksumOptions.StoreContentMD5 = value;
    }

    public bool? DisableContentMD5Validation
    {
      get => this.ChecksumOptions.DisableContentMD5Validation;
      set => this.ChecksumOptions.DisableContentMD5Validation = value;
    }

    public ChecksumOptions ChecksumOptions { get; set; } = new ChecksumOptions();
  }
}
