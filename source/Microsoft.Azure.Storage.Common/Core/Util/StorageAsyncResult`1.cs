// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Storage.Core.Util.StorageAsyncResult`1
// Assembly: Microsoft.Azure.Storage.Common, Version=11.2.3.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 0978DA65-6954-4A99-9ACB-2EF3D979A5D5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Storage.Common.dll

using System;
using System.Diagnostics;

namespace Microsoft.Azure.Storage.Core.Util
{
  internal class StorageAsyncResult<T> : StorageCommandAsyncResult
  {
    internal T Result { get; set; }

    internal OperationContext OperationContext { get; set; }

    internal IRequestOptions RequestOptions { get; set; }

    internal object OperationState { get; set; }

    internal Exception ExceptionRef { get; private set; }

    internal StorageAsyncResult(AsyncCallback callback, object state)
      : base(callback, state)
    {
    }

    [DebuggerNonUserCode]
    internal void OnComplete(Exception exception)
    {
      this.ExceptionRef = exception;
      this.OnComplete();
    }

    [DebuggerNonUserCode]
    internal override void End()
    {
      base.End();
      if (this.ExceptionRef != null)
        throw this.ExceptionRef;
    }
  }
}
