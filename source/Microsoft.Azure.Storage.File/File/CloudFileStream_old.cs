// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Storage.File.CloudFileStream_old
// Assembly: Microsoft.Azure.Storage.File, Version=11.2.3.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: C68E95B0-8DFB-410C-8E70-706406D1A279
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Storage.File.dll

using System;
using System.IO;

namespace Microsoft.Azure.Storage.File
{
  public abstract class CloudFileStream_old : Stream
  {
    public abstract void Commit();

    public abstract ICancellableAsyncResult BeginCommit(AsyncCallback callback, object state);

    public abstract void EndCommit(IAsyncResult asyncResult);

    public abstract ICancellableAsyncResult BeginFlush(AsyncCallback callback, object state);

    public abstract void EndFlush(IAsyncResult asyncResult);
  }
}
