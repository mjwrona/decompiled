// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Storage.Blob.CloudBlobStream
// Assembly: Microsoft.Azure.Storage.Blob, Version=11.2.3.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: A04A3512-352A-442F-A95B-BC1B94EF8840
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Storage.Blob.dll

using System;
using System.IO;
using System.Threading.Tasks;

namespace Microsoft.Azure.Storage.Blob
{
  public abstract class CloudBlobStream : Stream
  {
    public abstract void Commit();

    public abstract ICancellableAsyncResult BeginCommit(AsyncCallback callback, object state);

    public abstract void EndCommit(IAsyncResult asyncResult);

    public abstract ICancellableAsyncResult BeginFlush(AsyncCallback callback, object state);

    public abstract void EndFlush(IAsyncResult asyncResult);

    public abstract Task CommitAsync();
  }
}
