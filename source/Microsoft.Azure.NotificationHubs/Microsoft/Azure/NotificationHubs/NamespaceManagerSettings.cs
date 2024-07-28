// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.NotificationHubs.NamespaceManagerSettings
// Assembly: Microsoft.Azure.NotificationHubs, Version=2.16.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1F43328A-44A2-48DE-9CBC-06F3C4A41C2A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.NotificationHubs.dll

using Microsoft.Azure.NotificationHubs.Common;
using Microsoft.Azure.NotificationHubs.Messaging;
using System;

namespace Microsoft.Azure.NotificationHubs
{
  public sealed class NamespaceManagerSettings
  {
    private int getEntitiesPageSize;
    private TimeSpan operationTimeout;
    private RetryPolicy retryPolicy;

    public NamespaceManagerSettings()
    {
      this.operationTimeout = Constants.DefaultOperationTimeout;
      this.retryPolicy = RetryPolicy.Default;
      this.getEntitiesPageSize = int.MaxValue;
      this.TokenProvider = (TokenProvider) null;
    }

    public TimeSpan OperationTimeout
    {
      get => this.operationTimeout;
      set
      {
        TimeoutHelper.ThrowIfNonPositiveArgument(value, nameof (OperationTimeout));
        this.operationTimeout = value;
      }
    }

    public RetryPolicy RetryPolicy
    {
      get => this.retryPolicy;
      set => this.retryPolicy = value != null ? value : throw FxTrace.Exception.ArgumentNull(nameof (RetryPolicy));
    }

    public TokenProvider TokenProvider { get; set; }

    internal TimeSpan InternalOperationTimeout => !(this.operationTimeout > Constants.MaxOperationTimeout) ? this.operationTimeout : Constants.MaxOperationTimeout;

    internal int GetEntitiesPageSize
    {
      get => this.getEntitiesPageSize;
      set => this.getEntitiesPageSize = value > 0 ? value : throw FxTrace.Exception.ArgumentOutOfRange(nameof (GetEntitiesPageSize), (object) value, "GetEntitiesPageSize has to be positive value");
    }
  }
}
