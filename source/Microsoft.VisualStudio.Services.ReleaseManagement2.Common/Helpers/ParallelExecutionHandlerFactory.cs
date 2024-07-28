// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Common.Helpers.ParallelExecutionHandlerFactory
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: C3F75541-7C8A-4AF6-A47E-709CEEE7550D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Common.dll

using Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts;
using System;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Common.Helpers
{
  public static class ParallelExecutionHandlerFactory
  {
    public static ExecutionHandler GetHandler(ExecutionInput input, Action<string> traceMethod)
    {
      if (input == null)
        throw new ArgumentNullException(nameof (input));
      switch (input.ParallelExecutionType)
      {
        case ParallelExecutionTypes.None:
          return (ExecutionHandler) new NoneExecutionHandler((NoneExecutionInput) input, traceMethod);
        case ParallelExecutionTypes.MultiConfiguration:
          return (ExecutionHandler) new MultiConfigHandler((MultiConfigInput) input, traceMethod);
        case ParallelExecutionTypes.MultiMachine:
          return (ExecutionHandler) new MultiMachineHandler((MultiMachineInput) input, traceMethod);
        default:
          throw new NotImplementedException();
      }
    }
  }
}
