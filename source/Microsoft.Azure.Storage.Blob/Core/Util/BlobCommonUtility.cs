// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Storage.Core.Util.BlobCommonUtility
// Assembly: Microsoft.Azure.Storage.Blob, Version=11.2.3.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: A04A3512-352A-442F-A95B-BC1B94EF8840
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Storage.Blob.dll

using Microsoft.Azure.Storage.Auth;
using Microsoft.Azure.Storage.Blob;
using Microsoft.Azure.Storage.Core.Executor;
using System.Net.Http;

namespace Microsoft.Azure.Storage.Core.Util
{
  internal static class BlobCommonUtility
  {
    internal static ExecutionState<NullType> CreateTemporaryExecutionState(
      BlobRequestOptions options)
    {
      RESTCommand<NullType> cmd = new RESTCommand<NullType>(new StorageCredentials(), (StorageUri) null, (HttpClient) null);
      options?.ApplyToStorageCommand<NullType>(cmd);
      return new ExecutionState<NullType>((StorageCommandBase<NullType>) cmd, options?.RetryPolicy, new OperationContext());
    }
  }
}
