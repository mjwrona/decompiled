// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Storage.Core.SyncMemoryStream
// Assembly: Microsoft.Azure.Storage.Common, Version=11.2.3.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 0978DA65-6954-4A99-9ACB-2EF3D979A5D5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Storage.Common.dll

using Microsoft.Azure.Storage.Core.Util;
using System;
using System.IO;

namespace Microsoft.Azure.Storage.Core
{
  public class SyncMemoryStream : MemoryStream
  {
    public SyncMemoryStream()
    {
    }

    public SyncMemoryStream(byte[] buffer)
      : base(buffer)
    {
    }

    public SyncMemoryStream(byte[] buffer, int index)
      : base(buffer, index, buffer.Length - index)
    {
    }

    public SyncMemoryStream(byte[] buffer, int index, int count)
      : base(buffer, index, count)
    {
    }

    public override IAsyncResult BeginRead(
      byte[] buffer,
      int offset,
      int count,
      AsyncCallback callback,
      object state)
    {
      CommonUtility.AssertNotNull(nameof (buffer), (object) buffer);
      CommonUtility.AssertInBounds<int>(nameof (offset), offset, 0, buffer.Length);
      CommonUtility.AssertInBounds<int>(nameof (count), count, 0, buffer.Length - offset);
      StorageAsyncResult<int> storageAsyncResult = new StorageAsyncResult<int>(callback, state);
      try
      {
        storageAsyncResult.Result = this.Read(buffer, offset, count);
        storageAsyncResult.OnComplete();
      }
      catch (Exception ex)
      {
        storageAsyncResult.OnComplete(ex);
      }
      return (IAsyncResult) storageAsyncResult;
    }

    public override int EndRead(IAsyncResult asyncResult)
    {
      StorageAsyncResult<int> storageAsyncResult = (StorageAsyncResult<int>) asyncResult;
      storageAsyncResult.End();
      return storageAsyncResult.Result;
    }

    public override IAsyncResult BeginWrite(
      byte[] buffer,
      int offset,
      int count,
      AsyncCallback callback,
      object state)
    {
      CommonUtility.AssertNotNull(nameof (buffer), (object) buffer);
      CommonUtility.AssertInBounds<int>(nameof (offset), offset, 0, buffer.Length);
      CommonUtility.AssertInBounds<int>(nameof (count), count, 0, buffer.Length - offset);
      StorageAsyncResult<NullType> storageAsyncResult = new StorageAsyncResult<NullType>(callback, state);
      try
      {
        this.Write(buffer, offset, count);
        storageAsyncResult.OnComplete();
      }
      catch (Exception ex)
      {
        storageAsyncResult.OnComplete(ex);
      }
      return (IAsyncResult) storageAsyncResult;
    }

    public override void EndWrite(IAsyncResult asyncResult) => ((StorageCommandAsyncResult) asyncResult).End();
  }
}
