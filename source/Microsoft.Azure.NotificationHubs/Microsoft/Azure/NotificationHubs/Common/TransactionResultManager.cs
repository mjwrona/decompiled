// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.NotificationHubs.Common.TransactionResultManager
// Assembly: Microsoft.Azure.NotificationHubs, Version=2.16.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1F43328A-44A2-48DE-9CBC-06F3C4A41C2A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.NotificationHubs.dll

using Microsoft.Azure.NotificationHubs.Tracing;
using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.Transactions;

namespace Microsoft.Azure.NotificationHubs.Common
{
  internal sealed class TransactionResultManager
  {
    private static readonly TransactionResultManager instance = new TransactionResultManager();
    private readonly object syncLock;
    private readonly Dictionary<string, TransactionResult> transactionResults;

    private TransactionResultManager()
    {
      this.syncLock = new object();
      this.transactionResults = new Dictionary<string, TransactionResult>();
    }

    public static TransactionResultManager Instance => TransactionResultManager.instance;

    public void RegisterForTransactionalResult(
      Action<object, TransactionEventArgs, TransactionResult> onTransactionCompleted)
    {
      if (onTransactionCompleted == null)
        return;
      Transaction current = Transaction.Current;
      if (!(current != (Transaction) null))
        return;
      string transactionIdentifier = current.TransactionInformation.LocalIdentifier;
      lock (this.syncLock)
      {
        TransactionResult transactionResult;
        if (!this.transactionResults.TryGetValue(transactionIdentifier, out transactionResult))
        {
          transactionResult = new TransactionResult();
          this.transactionResults.Add(transactionIdentifier, transactionResult);
        }
        ++transactionResult.ReferenceCount;
      }
      current.TransactionCompleted += Fx.ThunkTransactionEventHandler((TransactionCompletedEventHandler) ((s, e) =>
      {
        TransactionResult transactionResult;
        lock (this.syncLock)
        {
          transactionResult = this.transactionResults[transactionIdentifier];
          if (--transactionResult.ReferenceCount == 0)
            this.transactionResults.Remove(transactionIdentifier);
        }
        onTransactionCompleted(s, e, transactionResult);
      }));
    }

    public void SetTransactionResult(
      string transactionId,
      Exception completionException,
      TrackingContext trackingContext)
    {
      lock (this.syncLock)
      {
        TransactionResult transactionResult;
        if (!this.transactionResults.TryGetValue(transactionId, out transactionResult))
          return;
        transactionResult.CompletionException = completionException;
        transactionResult.TrackingContext = trackingContext;
      }
    }

    public void SetTransactionResultExtension(
      string transactionId,
      IExtension<TransactionResult> extensionData)
    {
      lock (this.syncLock)
      {
        TransactionResult transactionResult;
        if (!this.transactionResults.TryGetValue(transactionId, out transactionResult))
          return;
        transactionResult.Extensions.Add(extensionData);
      }
    }

    public T FindTransactionResultExtension<T>(string transactionId)
    {
      lock (this.syncLock)
      {
        TransactionResult transactionResult;
        if (this.transactionResults.TryGetValue(transactionId, out transactionResult))
          return transactionResult.Extensions.Find<T>();
        throw Fx.AssertAndFailFastService(SRClient.CannotFindTransactionResult((object) transactionId));
      }
    }
  }
}
