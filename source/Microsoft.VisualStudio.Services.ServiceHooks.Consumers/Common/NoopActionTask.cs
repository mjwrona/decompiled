// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ServiceHooks.Consumers.Common.NoopActionTask
// Assembly: Microsoft.VisualStudio.Services.ServiceHooks.Consumers, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 5BEF54E7-7304-4071-B5F1-22428BB21801
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.ServiceHooks.Consumers.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.ServiceHooks.Common;
using System;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.ServiceHooks.Consumers.Common
{
  public class NoopActionTask : ActionTask
  {
    public string RequestMessage { get; private set; }

    public string ResponseMessage { get; private set; }

    public NoopActionTask(string requestMessage = null, string responseMessage = null)
    {
      this.RequestMessage = requestMessage;
      this.ResponseMessage = responseMessage;
    }

    public override Task<ActionTaskResult> RunAsync(
      IVssRequestContext requestContext,
      TimeSpan timeout)
    {
      if (this.RequestMessage != null)
        this.UpdateNotificationForRequest(requestContext, this.RequestMessage);
      if (this.ResponseMessage != null)
        this.UpdateNotificationForResponse(requestContext, this.ResponseMessage);
      return Task.FromResult<ActionTaskResult>(new ActionTaskResult(ActionTaskResultLevel.Success));
    }
  }
}
