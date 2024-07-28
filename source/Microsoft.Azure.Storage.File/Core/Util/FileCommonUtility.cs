// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Storage.Core.Util.FileCommonUtility
// Assembly: Microsoft.Azure.Storage.File, Version=11.2.3.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: C68E95B0-8DFB-410C-8E70-706406D1A279
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Storage.File.dll

using Microsoft.Azure.Storage.Auth;
using Microsoft.Azure.Storage.Core.Executor;
using Microsoft.Azure.Storage.File;
using System.Net.Http;

namespace Microsoft.Azure.Storage.Core.Util
{
  internal static class FileCommonUtility
  {
    internal static ExecutionState<NullType> CreateTemporaryExecutionState(
      FileRequestOptions options)
    {
      RESTCommand<NullType> cmd = new RESTCommand<NullType>(new StorageCredentials(), (StorageUri) null, (HttpClient) null);
      options?.ApplyToStorageCommand<NullType>(cmd);
      return new ExecutionState<NullType>((StorageCommandBase<NullType>) cmd, options?.RetryPolicy, new OperationContext());
    }
  }
}
