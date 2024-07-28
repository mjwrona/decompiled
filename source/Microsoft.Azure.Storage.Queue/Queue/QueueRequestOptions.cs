// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Storage.Queue.QueueRequestOptions
// Assembly: Microsoft.Azure.Storage.Queue, Version=11.2.3.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 3D35BFA0-638A-4C3C-8E74-B592D3B60EFD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Storage.Queue.dll

using Microsoft.Azure.Storage.Core.Executor;
using Microsoft.Azure.Storage.Core.Util;
using Microsoft.Azure.Storage.RetryPolicies;
using Microsoft.Azure.Storage.Shared.Protocol;
using System;

namespace Microsoft.Azure.Storage.Queue
{
  public sealed class QueueRequestOptions : IRequestOptions
  {
    private TimeSpan? maximumExecutionTime;
    internal static QueueRequestOptions BaseDefaultRequestOptions = new QueueRequestOptions()
    {
      RetryPolicy = (IRetryPolicy) new NoRetry(),
      EncryptionPolicy = (QueueEncryptionPolicy) null,
      RequireEncryption = new bool?(),
      LocationMode = new Microsoft.Azure.Storage.RetryPolicies.LocationMode?(Microsoft.Azure.Storage.RetryPolicies.LocationMode.PrimaryOnly),
      ServerTimeout = new TimeSpan?(),
      MaximumExecutionTime = new TimeSpan?(),
      NetworkTimeout = new TimeSpan?(Constants.DefaultNetworkTimeout)
    };

    public QueueRequestOptions()
    {
    }

    internal QueueRequestOptions(QueueRequestOptions other)
      : this()
    {
      if (other == null)
        return;
      this.RetryPolicy = other.RetryPolicy;
      this.EncryptionPolicy = other.EncryptionPolicy;
      this.RequireEncryption = other.RequireEncryption;
      this.ServerTimeout = other.ServerTimeout;
      this.LocationMode = other.LocationMode;
      this.MaximumExecutionTime = other.MaximumExecutionTime;
      this.NetworkTimeout = other.NetworkTimeout;
      this.OperationExpiryTime = other.OperationExpiryTime;
    }

    internal static QueueRequestOptions ApplyDefaults(
      QueueRequestOptions options,
      CloudQueueClient serviceClient)
    {
      QueueRequestOptions queueRequestOptions1 = new QueueRequestOptions(options);
      queueRequestOptions1.RetryPolicy = queueRequestOptions1.RetryPolicy ?? serviceClient.DefaultRequestOptions.RetryPolicy ?? QueueRequestOptions.BaseDefaultRequestOptions.RetryPolicy;
      queueRequestOptions1.EncryptionPolicy = queueRequestOptions1.EncryptionPolicy ?? serviceClient.DefaultRequestOptions.EncryptionPolicy ?? QueueRequestOptions.BaseDefaultRequestOptions.EncryptionPolicy;
      queueRequestOptions1.RequireEncryption = queueRequestOptions1.RequireEncryption ?? serviceClient.DefaultRequestOptions.RequireEncryption ?? QueueRequestOptions.BaseDefaultRequestOptions.RequireEncryption;
      queueRequestOptions1.LocationMode = queueRequestOptions1.LocationMode ?? serviceClient.DefaultRequestOptions.LocationMode ?? QueueRequestOptions.BaseDefaultRequestOptions.LocationMode;
      QueueRequestOptions queueRequestOptions2 = queueRequestOptions1;
      TimeSpan? nullable1 = queueRequestOptions1.ServerTimeout;
      TimeSpan? nullable2;
      TimeSpan? nullable3;
      if (!nullable1.HasValue)
      {
        nullable2 = serviceClient.DefaultRequestOptions.ServerTimeout;
        nullable3 = nullable2 ?? QueueRequestOptions.BaseDefaultRequestOptions.ServerTimeout;
      }
      else
        nullable3 = nullable1;
      queueRequestOptions2.ServerTimeout = nullable3;
      QueueRequestOptions queueRequestOptions3 = queueRequestOptions1;
      nullable1 = queueRequestOptions1.NetworkTimeout;
      TimeSpan? nullable4;
      if (!nullable1.HasValue)
      {
        nullable2 = serviceClient.DefaultRequestOptions.NetworkTimeout;
        nullable4 = nullable2 ?? QueueRequestOptions.BaseDefaultRequestOptions.NetworkTimeout;
      }
      else
        nullable4 = nullable1;
      queueRequestOptions3.NetworkTimeout = nullable4;
      QueueRequestOptions queueRequestOptions4 = queueRequestOptions1;
      nullable1 = queueRequestOptions1.MaximumExecutionTime;
      TimeSpan? nullable5;
      if (!nullable1.HasValue)
      {
        nullable2 = serviceClient.DefaultRequestOptions.MaximumExecutionTime;
        nullable5 = nullable2 ?? QueueRequestOptions.BaseDefaultRequestOptions.MaximumExecutionTime;
      }
      else
        nullable5 = nullable1;
      queueRequestOptions4.MaximumExecutionTime = nullable5;
      if (!queueRequestOptions1.OperationExpiryTime.HasValue)
      {
        nullable1 = queueRequestOptions1.MaximumExecutionTime;
        if (nullable1.HasValue)
        {
          QueueRequestOptions queueRequestOptions5 = queueRequestOptions1;
          DateTime now = DateTime.Now;
          nullable1 = queueRequestOptions1.MaximumExecutionTime;
          TimeSpan timeSpan = nullable1.Value;
          DateTime? nullable6 = new DateTime?(now + timeSpan);
          queueRequestOptions5.OperationExpiryTime = nullable6;
        }
      }
      return queueRequestOptions1;
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

    internal void AssertPolicyIfRequired()
    {
      if (this.RequireEncryption.HasValue && this.RequireEncryption.Value && this.EncryptionPolicy == null)
        throw new InvalidOperationException("Encryption Policy is mandatory when RequireEncryption is set to true. If you do not want to encrypt/decrypt data, please set RequireEncryption to false in request options.");
    }

    internal DateTime? OperationExpiryTime { get; set; }

    public IRetryPolicy RetryPolicy { get; set; }

    public QueueEncryptionPolicy EncryptionPolicy { get; set; }

    public bool? RequireEncryption { get; set; }

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
  }
}
