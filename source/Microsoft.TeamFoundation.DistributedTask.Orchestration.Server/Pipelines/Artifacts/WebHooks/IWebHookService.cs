// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Pipelines.Artifacts.WebHooks.IWebHookService
// Assembly: Microsoft.TeamFoundation.DistributedTask.Orchestration.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07FD5059-3D25-415E-AA3A-5372051D7E71
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.dll

using Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.Artifacts;
using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.DistributedTask.Pipelines.Artifacts.WebHooks
{
  [DefaultServiceImplementation(typeof (WebHookService))]
  public interface IWebHookService : IVssFrameworkService
  {
    WebHook CreateIncomingWebHook(
      IVssRequestContext requestContext,
      WebHook webHook,
      string webHookName);

    WebHook GetIncomingWebHook(
      IVssRequestContext requestContext,
      string webHookName,
      bool includeSubscriptions = false);

    WebHook GetWebHook(
      IVssRequestContext requestContext,
      Guid webHookId,
      bool includeSubscriptions = false);

    WebHook GetWebHook(
      IVssRequestContext requestContext,
      string uniqueArtifactIdentifier,
      bool includeSubscriptions = false);

    void DeleteWebHook(
      IVssRequestContext requestContext,
      Guid projectId,
      IArtifactType artifactType,
      WebHook webHook,
      IDictionary<string, string> parameters,
      out string resultMessage);

    WebHook EnsureIncomingWebHookExists(
      IVssRequestContext requestContext,
      Guid projectId,
      IArtifactType artifactType,
      string uniqueArtifactIdentifier,
      IDictionary<string, string> parameters,
      out string resultMessage);

    WebHook EnsureWebHookExists(
      IVssRequestContext requestContext,
      Guid projectId,
      IArtifactType artifactType,
      string uniqueArtifactIdentifier,
      IDictionary<string, string> parameters,
      out string resultMessage);

    void AddWebHookSubscription(
      IVssRequestContext requestContext,
      Guid webHookId,
      string uniqueArtifactIdentifier);

    void DeleteWebHookSubscription(
      IVssRequestContext requestContext,
      Guid webHookId,
      string uniqueArtifactIdentifier);

    void LinkWebHook(
      IVssRequestContext requestContext,
      Guid projectId,
      IArtifactType artifactType,
      WebHook webHook,
      IDictionary<string, string> properties);

    void UnlinkWebHook(
      IVssRequestContext requestContext,
      Guid projectId,
      IArtifactType artifactType,
      WebHook webHook,
      IDictionary<string, string> properties);

    IWebHookExtension GetWebHookExtension(IVssRequestContext requestContext);
  }
}
