// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.CircuitBreaker.ICommandFactory
// Assembly: Microsoft.VisualStudio.Services.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F9C46641-2F1F-44C4-9C2E-4328CD5FFC63
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Common.dll

using System;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.CircuitBreaker
{
  public interface ICommandFactory
  {
    CommandKey CommandKey { get; }

    CommandAsync CreateCommandAsync(
      CommandSetter commandSetter,
      Func<Task> run = null,
      Func<Task> fallback = null,
      bool continueOnCapturedContext = false);

    CommandAsync<TResult> CreateCommandAsync<TResult>(
      CommandSetter commandSetter,
      Func<Task<TResult>> run = null,
      Func<Task<TResult>> fallback = null,
      bool continueOnCapturedContext = false);
  }
}
