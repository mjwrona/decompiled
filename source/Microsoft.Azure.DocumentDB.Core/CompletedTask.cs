// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Documents.CompletedTask
// Assembly: Microsoft.Azure.DocumentDB.Core, Version=2.10.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 20BB0EB7-1465-494C-8F4E-F898448B85D9
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.DocumentDB.Core.dll

using System;
using System.Threading.Tasks;

namespace Microsoft.Azure.Documents
{
  internal static class CompletedTask
  {
    private static Task instance;

    public static Task Instance
    {
      get
      {
        if (CompletedTask.instance == null)
        {
          TaskCompletionSource<bool> completionSource = new TaskCompletionSource<bool>();
          completionSource.SetResult(true);
          CompletedTask.instance = (Task) completionSource.Task;
        }
        return CompletedTask.instance;
      }
    }

    public static Task<T> SetException<T>(Exception exception)
    {
      TaskCompletionSource<T> completionSource = new TaskCompletionSource<T>();
      completionSource.SetException(exception);
      return completionSource.Task;
    }
  }
}
