// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ServiceHooks.Server.ActionTaskRunException
// Assembly: Microsoft.VisualStudio.Services.ServiceHooks.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 25B6D63E-3809-4A04-9AE1-1F77D8FFEE67
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ServiceHooks.Server.dll

using Microsoft.VisualStudio.Services.ServiceHooks.Common;
using Microsoft.VisualStudio.Services.ServiceHooks.WebApi;
using System;

namespace Microsoft.VisualStudio.Services.ServiceHooks.Server
{
  [Serializable]
  internal class ActionTaskRunException : ServiceHookException
  {
    public ActionTaskResult ActionTaskResult { get; private set; }

    public ActionTaskRunException(ActionTaskResult actionTaskResult)
      : base(actionTaskResult.ErrorMessage)
    {
      this.ActionTaskResult = actionTaskResult;
    }

    public ActionTaskRunException(ActionTaskResult actionTaskResult, Exception innerException)
      : base(actionTaskResult.ErrorMessage, innerException)
    {
      this.ActionTaskResult = actionTaskResult;
    }
  }
}
