// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Content.Server.Common.Result`2
// Assembly: Microsoft.VisualStudio.Services.Content.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 203E0171-FB50-4FDE-9B1F-EFC6366423BC
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Content.Server.Common.dll

using Microsoft.VisualStudio.Services.Content.Common;
using System;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.Content.Server.Common
{
  public abstract class Result<TSuccess, TError> : TaggedUnion<TSuccess, TError>
  {
    protected Result(TSuccess success)
      : base(success)
    {
    }

    protected Result(TError error)
      : base(error)
    {
    }

    public void OnError(Action<TError> onError) => this.Match((Action<TSuccess>) (_ => { }), onError);

    public Task OnErrorAsync(Func<TError, Task> onErrorAsync) => this.Match<Task>((Func<TSuccess, Task>) (_ => CompletedTask.Instance), onErrorAsync);
  }
}
