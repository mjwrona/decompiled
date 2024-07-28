// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Storage.Blob.BlobRequestOptions
// Assembly: Microsoft.Azure.Storage.Blob, Version=11.2.3.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: A04A3512-352A-442F-A95B-BC1B94EF8840
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Storage.Blob.dll

using Microsoft.Azure.Storage.Core.Executor;
using Microsoft.Azure.Storage.Core.Util;
using Microsoft.Azure.Storage.RetryPolicies;
using Microsoft.Azure.Storage.Shared.Protocol;
using System;

namespace Microsoft.Azure.Storage.Blob
{
  public sealed class BlobRequestOptions : IRequestOptions
  {
    private int? parallelOperationThreadCount;
    private long? singleBlobUploadThresholdInBytes;
    private TimeSpan? maximumExecutionTime;
    private BlobCustomerProvidedKey blobCustomerProvidedKey;
    private string encryptionScope;
    internal static BlobRequestOptions BaseDefaultRequestOptions = new BlobRequestOptions()
    {
      RetryPolicy = (IRetryPolicy) new NoRetry(),
      AbsorbConditionalErrorsOnRetry = new bool?(false),
      EncryptionPolicy = (BlobEncryptionPolicy) null,
      RequireEncryption = new bool?(),
      LocationMode = new Microsoft.Azure.Storage.RetryPolicies.LocationMode?(Microsoft.Azure.Storage.RetryPolicies.LocationMode.PrimaryOnly),
      ServerTimeout = new TimeSpan?(),
      MaximumExecutionTime = new TimeSpan?(),
      NetworkTimeout = new TimeSpan?(Constants.DefaultNetworkTimeout),
      ParallelOperationThreadCount = new int?(1),
      SingleBlobUploadThresholdInBytes = new long?(134217728L),
      ChecksumOptions = new ChecksumOptions()
      {
        DisableContentMD5Validation = new bool?(false),
        UseTransactionalMD5 = new bool?(false),
        DisableContentCRC64Validation = new bool?(false),
        UseTransactionalCRC64 = new bool?(false)
      }
    };

    public BlobRequestOptions()
    {
    }

    internal BlobRequestOptions(BlobRequestOptions other)
    {
      if (other == null)
        return;
      this.RetryPolicy = other.RetryPolicy;
      this.AbsorbConditionalErrorsOnRetry = other.AbsorbConditionalErrorsOnRetry;
      this.EncryptionPolicy = other.EncryptionPolicy;
      this.RequireEncryption = other.RequireEncryption;
      this.SkipEncryptionPolicyValidation = other.SkipEncryptionPolicyValidation;
      this.LocationMode = other.LocationMode;
      this.ServerTimeout = other.ServerTimeout;
      this.MaximumExecutionTime = other.MaximumExecutionTime;
      this.NetworkTimeout = other.NetworkTimeout;
      this.OperationExpiryTime = other.OperationExpiryTime;
      this.ChecksumOptions.CopyFrom(other.ChecksumOptions);
      this.ParallelOperationThreadCount = other.ParallelOperationThreadCount;
      this.SingleBlobUploadThresholdInBytes = other.SingleBlobUploadThresholdInBytes;
    }

    internal static BlobRequestOptions ApplyDefaults(
      BlobRequestOptions options,
      BlobType blobType,
      CloudBlobClient serviceClient,
      bool applyExpiry = true)
    {
      BlobRequestOptions blobRequestOptions1 = new BlobRequestOptions(options);
      blobRequestOptions1.RetryPolicy = blobRequestOptions1.RetryPolicy ?? serviceClient.DefaultRequestOptions.RetryPolicy ?? BlobRequestOptions.BaseDefaultRequestOptions.RetryPolicy;
      BlobRequestOptions blobRequestOptions2 = blobRequestOptions1;
      bool? nullable1 = blobRequestOptions1.AbsorbConditionalErrorsOnRetry;
      bool? nullable2;
      bool? nullable3;
      if (!nullable1.HasValue)
      {
        nullable2 = serviceClient.DefaultRequestOptions.AbsorbConditionalErrorsOnRetry;
        nullable3 = nullable2 ?? BlobRequestOptions.BaseDefaultRequestOptions.AbsorbConditionalErrorsOnRetry;
      }
      else
        nullable3 = nullable1;
      blobRequestOptions2.AbsorbConditionalErrorsOnRetry = nullable3;
      blobRequestOptions1.EncryptionPolicy = blobRequestOptions1.EncryptionPolicy ?? serviceClient.DefaultRequestOptions.EncryptionPolicy ?? BlobRequestOptions.BaseDefaultRequestOptions.EncryptionPolicy;
      BlobRequestOptions blobRequestOptions3 = blobRequestOptions1;
      nullable1 = blobRequestOptions1.RequireEncryption;
      bool? nullable4;
      if (!nullable1.HasValue)
      {
        nullable2 = serviceClient.DefaultRequestOptions.RequireEncryption;
        nullable4 = nullable2 ?? BlobRequestOptions.BaseDefaultRequestOptions.RequireEncryption;
      }
      else
        nullable4 = nullable1;
      blobRequestOptions3.RequireEncryption = nullable4;
      blobRequestOptions1.LocationMode = blobRequestOptions1.LocationMode ?? serviceClient.DefaultRequestOptions.LocationMode ?? BlobRequestOptions.BaseDefaultRequestOptions.LocationMode;
      BlobRequestOptions blobRequestOptions4 = blobRequestOptions1;
      TimeSpan? nullable5 = blobRequestOptions1.ServerTimeout;
      TimeSpan? nullable6;
      TimeSpan? nullable7;
      if (!nullable5.HasValue)
      {
        nullable6 = serviceClient.DefaultRequestOptions.ServerTimeout;
        nullable7 = nullable6 ?? BlobRequestOptions.BaseDefaultRequestOptions.ServerTimeout;
      }
      else
        nullable7 = nullable5;
      blobRequestOptions4.ServerTimeout = nullable7;
      BlobRequestOptions blobRequestOptions5 = blobRequestOptions1;
      nullable5 = blobRequestOptions1.MaximumExecutionTime;
      TimeSpan? nullable8;
      if (!nullable5.HasValue)
      {
        nullable6 = serviceClient.DefaultRequestOptions.MaximumExecutionTime;
        nullable8 = nullable6 ?? BlobRequestOptions.BaseDefaultRequestOptions.MaximumExecutionTime;
      }
      else
        nullable8 = nullable5;
      blobRequestOptions5.MaximumExecutionTime = nullable8;
      BlobRequestOptions blobRequestOptions6 = blobRequestOptions1;
      nullable5 = blobRequestOptions1.NetworkTimeout;
      TimeSpan? nullable9;
      if (!nullable5.HasValue)
      {
        nullable6 = serviceClient.DefaultRequestOptions.NetworkTimeout;
        nullable9 = nullable6 ?? BlobRequestOptions.BaseDefaultRequestOptions.NetworkTimeout;
      }
      else
        nullable9 = nullable5;
      blobRequestOptions6.NetworkTimeout = nullable9;
      blobRequestOptions1.ParallelOperationThreadCount = blobRequestOptions1.ParallelOperationThreadCount ?? serviceClient.DefaultRequestOptions.ParallelOperationThreadCount ?? BlobRequestOptions.BaseDefaultRequestOptions.ParallelOperationThreadCount;
      blobRequestOptions1.SingleBlobUploadThresholdInBytes = blobRequestOptions1.SingleBlobUploadThresholdInBytes ?? serviceClient.DefaultRequestOptions.SingleBlobUploadThresholdInBytes ?? BlobRequestOptions.BaseDefaultRequestOptions.SingleBlobUploadThresholdInBytes;
      if (applyExpiry && !blobRequestOptions1.OperationExpiryTime.HasValue)
      {
        nullable5 = blobRequestOptions1.MaximumExecutionTime;
        if (nullable5.HasValue)
        {
          BlobRequestOptions blobRequestOptions7 = blobRequestOptions1;
          DateTime now = DateTime.Now;
          nullable5 = blobRequestOptions1.MaximumExecutionTime;
          TimeSpan timeSpan = nullable5.Value;
          DateTime? nullable10 = new DateTime?(now + timeSpan);
          blobRequestOptions7.OperationExpiryTime = nullable10;
        }
      }
      ChecksumOptions checksumOptions1 = blobRequestOptions1.ChecksumOptions;
      nullable1 = blobRequestOptions1.ChecksumOptions.DisableContentMD5Validation;
      bool? nullable11;
      if (!nullable1.HasValue)
      {
        nullable2 = serviceClient.DefaultRequestOptions.ChecksumOptions.DisableContentMD5Validation;
        nullable11 = nullable2 ?? BlobRequestOptions.BaseDefaultRequestOptions.ChecksumOptions.DisableContentMD5Validation;
      }
      else
        nullable11 = nullable1;
      checksumOptions1.DisableContentMD5Validation = nullable11;
      ChecksumOptions checksumOptions2 = blobRequestOptions1.ChecksumOptions;
      nullable1 = blobRequestOptions1.ChecksumOptions.StoreContentMD5;
      int num1;
      if (!nullable1.HasValue)
      {
        nullable2 = serviceClient.DefaultRequestOptions.ChecksumOptions.StoreContentMD5;
        num1 = (int) nullable2 ?? (blobType == BlobType.BlockBlob ? 1 : 0);
      }
      else
        num1 = nullable1.GetValueOrDefault() ? 1 : 0;
      bool? nullable12 = new bool?(num1 != 0);
      checksumOptions2.StoreContentMD5 = nullable12;
      ChecksumOptions checksumOptions3 = blobRequestOptions1.ChecksumOptions;
      nullable1 = blobRequestOptions1.ChecksumOptions.UseTransactionalMD5;
      bool? nullable13;
      if (!nullable1.HasValue)
      {
        nullable2 = serviceClient.DefaultRequestOptions.ChecksumOptions.UseTransactionalMD5;
        nullable13 = nullable2 ?? BlobRequestOptions.BaseDefaultRequestOptions.ChecksumOptions.UseTransactionalMD5;
      }
      else
        nullable13 = nullable1;
      checksumOptions3.UseTransactionalMD5 = nullable13;
      ChecksumOptions checksumOptions4 = blobRequestOptions1.ChecksumOptions;
      nullable1 = blobRequestOptions1.ChecksumOptions.DisableContentCRC64Validation;
      bool? nullable14;
      if (!nullable1.HasValue)
      {
        nullable2 = serviceClient.DefaultRequestOptions.ChecksumOptions.DisableContentCRC64Validation;
        nullable14 = nullable2 ?? BlobRequestOptions.BaseDefaultRequestOptions.ChecksumOptions.DisableContentCRC64Validation;
      }
      else
        nullable14 = nullable1;
      checksumOptions4.DisableContentCRC64Validation = nullable14;
      ChecksumOptions checksumOptions5 = blobRequestOptions1.ChecksumOptions;
      nullable1 = blobRequestOptions1.ChecksumOptions.StoreContentCRC64;
      int num2;
      if (!nullable1.HasValue)
      {
        nullable2 = serviceClient.DefaultRequestOptions.ChecksumOptions.StoreContentCRC64;
        num2 = (int) nullable2 ?? (blobType == BlobType.BlockBlob ? 1 : 0);
      }
      else
        num2 = nullable1.GetValueOrDefault() ? 1 : 0;
      bool? nullable15 = new bool?(num2 != 0);
      checksumOptions5.StoreContentCRC64 = nullable15;
      ChecksumOptions checksumOptions6 = blobRequestOptions1.ChecksumOptions;
      nullable1 = blobRequestOptions1.ChecksumOptions.UseTransactionalCRC64;
      bool? nullable16;
      if (!nullable1.HasValue)
      {
        nullable2 = serviceClient.DefaultRequestOptions.ChecksumOptions.UseTransactionalCRC64;
        nullable16 = nullable2 ?? BlobRequestOptions.BaseDefaultRequestOptions.ChecksumOptions.UseTransactionalCRC64;
      }
      else
        nullable16 = nullable1;
      checksumOptions6.UseTransactionalCRC64 = nullable16;
      blobRequestOptions1.CustomerProvidedKey = options?.CustomerProvidedKey;
      blobRequestOptions1.EncryptionScope = options?.EncryptionScope;
      return blobRequestOptions1;
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

    internal void AssertNoEncryptionPolicyOrStrictMode()
    {
      if (this.EncryptionPolicy != null && !this.SkipEncryptionPolicyValidation)
        throw new InvalidOperationException("Encryption is not supported for the current operation. Please ensure that EncryptionPolicy is not set on RequestOptions.");
      this.AssertPolicyIfRequired();
    }

    internal void AssertPolicyIfRequired()
    {
      if (this.RequireEncryption.HasValue && this.RequireEncryption.Value && this.EncryptionPolicy == null)
        throw new InvalidOperationException("Encryption Policy is mandatory when RequireEncryption is set to true. If you do not want to encrypt/decrypt data, please set RequireEncryption to false in request options.");
    }

    internal DateTime? OperationExpiryTime { get; set; }

    public IRetryPolicy RetryPolicy { get; set; }

    public BlobEncryptionPolicy EncryptionPolicy { get; set; }

    public bool? RequireEncryption { get; set; }

    internal bool SkipEncryptionPolicyValidation { get; set; }

    public BlobCustomerProvidedKey CustomerProvidedKey
    {
      get => this.blobCustomerProvidedKey;
      set => this.blobCustomerProvidedKey = value == null || this.EncryptionScope == null ? value : throw new StorageException("");
    }

    public string EncryptionScope
    {
      get => this.encryptionScope;
      set => this.encryptionScope = value == null || this.CustomerProvidedKey == null ? value : throw new StorageException("");
    }

    public bool? AbsorbConditionalErrorsOnRetry { get; set; }

    public Microsoft.Azure.Storage.RetryPolicies.LocationMode? LocationMode { get; set; }

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

    public long? SingleBlobUploadThresholdInBytes
    {
      get => this.singleBlobUploadThresholdInBytes;
      set
      {
        if (value.HasValue)
          CommonUtility.AssertInBounds<long>(nameof (SingleBlobUploadThresholdInBytes), value.Value, 1048576L, 268435456L);
        this.singleBlobUploadThresholdInBytes = value;
      }
    }

    public bool? UseTransactionalMD5
    {
      get => this.ChecksumOptions.UseTransactionalMD5;
      set => this.ChecksumOptions.UseTransactionalMD5 = value;
    }

    public bool? StoreBlobContentMD5
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
