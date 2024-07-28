// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ServiceHooks.Common.ActionTaskResult
// Assembly: Microsoft.VisualStudio.Services.ServiceHooks.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E36C8A02-D97F-45E0-9F96-E7385D8CA092
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.ServiceHooks.Common.dll

using System;

namespace Microsoft.VisualStudio.Services.ServiceHooks.Common
{
  public class ActionTaskResult
  {
    public Exception Exception { get; private set; }

    public string ErrorMessage { get; private set; }

    public ActionTaskResultLevel ResultLevel { get; private set; }

    public ActionTaskResult(
      ActionTaskResultLevel resultLevel,
      Exception exception = null,
      string errorMessage = null)
    {
      this.Exception = exception;
      this.ResultLevel = resultLevel;
      this.ErrorMessage = errorMessage;
    }
  }
}
