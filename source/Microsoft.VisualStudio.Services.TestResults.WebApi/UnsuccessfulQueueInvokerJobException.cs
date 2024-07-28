// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.TestResults.WebApi.UnsuccessfulQueueInvokerJobException
// Assembly: Microsoft.VisualStudio.Services.TestResults.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A1B70BC6-DD93-426A-A4F2-75066CF77D48
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.TestResults.WebApi.dll

using Microsoft.VisualStudio.Services.Common;

namespace Microsoft.VisualStudio.Services.TestResults.WebApi
{
  public class UnsuccessfulQueueInvokerJobException : VssServiceException
  {
    public UnsuccessfulQueueInvokerJobException()
    {
    }

    public UnsuccessfulQueueInvokerJobException(string message)
      : base(message)
    {
    }
  }
}
