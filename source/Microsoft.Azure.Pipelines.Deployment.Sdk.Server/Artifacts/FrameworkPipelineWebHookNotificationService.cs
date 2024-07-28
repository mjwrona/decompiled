// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Pipelines.Deployment.Sdk.Server.Artifacts.FrameworkPipelineWebHookNotificationService
// Assembly: Microsoft.Azure.Pipelines.Deployment.Sdk.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 2CF55160-AB9F-45A3-BD33-54D24F269988
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.Azure.Pipelines.Deployment.Sdk.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Notifications;
using System;

namespace Microsoft.Azure.Pipelines.Deployment.Sdk.Server.Artifacts
{
  public class FrameworkPipelineWebHookNotificationService : 
    IPipelineWebHookNotificationService,
    IVssFrameworkService
  {
    public void PublishEvent(IVssRequestContext requestContext, VssNotificationEvent notification) => throw new NotImplementedException();

    public void ServiceStart(IVssRequestContext systemRequestContext)
    {
    }

    public void ServiceEnd(IVssRequestContext systemRequestContext)
    {
    }
  }
}
