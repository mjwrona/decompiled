// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Pipelines.Artifacts.WebHooks.IWebHookExtension
// Assembly: Microsoft.TeamFoundation.DistributedTask.Orchestration.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07FD5059-3D25-415E-AA3A-5372051D7E71
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.dll

using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.FormInput;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;

namespace Microsoft.TeamFoundation.DistributedTask.Pipelines.Artifacts.WebHooks
{
  [InheritedExport]
  public interface IWebHookExtension
  {
    string ServiceName { get; }

    void AddWebHookSubscription(
      IVssRequestContext requestContext,
      Guid webHookId,
      string uniqueArtifactIdentifier);

    Guid CreateWebHookName(IVssRequestContext requestContext, Guid webHookId, string webHookName);

    void DeleteWebHookName(IVssRequestContext requestContext, WebHook webHook);

    WebHook CreateWebHook(IVssRequestContext requestContext, WebHook webHook);

    void DeleteWebHook(IVssRequestContext requestContext, WebHook webHook);

    void DeleteWebHookSubscription(
      IVssRequestContext requestContext,
      Guid webHookId,
      string uniqueArtifactIdentifier);

    WebHook GetWebHook(
      IVssRequestContext requestContext,
      string uniqueArtifactIdentifier,
      bool includeSubscriptions);

    WebHook GetWebHook(
      IVssRequestContext requestContext,
      Guid webHookId,
      bool includeSubscriptions);

    string GetWebHookPayloadUrl(IVssRequestContext requestContext, WebHook webHook);

    InputValues GetWebHookPublisherInputValues(
      IVssRequestContext requestContext,
      ProjectInfo projectInfo,
      string inputId,
      IDictionary<string, string> inputValues);

    void QueueJobToTriggerPipeline(
      IVssRequestContext requestContext,
      Guid webHookId,
      string eventPayload);

    WebHook GetIncomingWebHook(
      IVssRequestContext requestContext,
      string webHookName,
      bool includeSubscriptions);
  }
}
