// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build2.WebServer.RepositoryWebhookController
// Assembly: Microsoft.TeamFoundation.Build2.WebServer, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: FDDA87C8-3548-4A75-AA18-4FB488450659
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build2.WebServer.dll

using Microsoft.TeamFoundation.Build2.Server;
using Microsoft.TeamFoundation.Build2.WebApiConverters;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace Microsoft.TeamFoundation.Build2.WebServer
{
  [VersionedApiControllerCustomName(Area = "build", ResourceName = "webhooks")]
  [ClientGroupByResource("sourceProviders")]
  public class RepositoryWebhookController : BuildApiController
  {
    [HttpGet]
    [ClientLocationId("8F20FF82-9498-4812-9F6E-9C01BDC50E99")]
    public IEnumerable<Microsoft.TeamFoundation.Build.WebApi.RepositoryWebhook> ListWebhooks(
      string providerName,
      [ClientQueryParameter] Guid? serviceEndpointId = null,
      [ClientQueryParameter] string repository = null)
    {
      ArgumentUtility.CheckStringForNullOrEmpty(providerName, nameof (providerName));
      return this.GetSourceProvider(providerName).GetRepositoryWebhooks(this.TfsRequestContext, this.ProjectId, serviceEndpointId, repository).Select<Microsoft.TeamFoundation.Build2.Server.RepositoryWebhook, Microsoft.TeamFoundation.Build.WebApi.RepositoryWebhook>((Func<Microsoft.TeamFoundation.Build2.Server.RepositoryWebhook, Microsoft.TeamFoundation.Build.WebApi.RepositoryWebhook>) (h => h.ToWebApiRepositoryWebhook()));
    }

    [HttpPost]
    [ClientLocationId("793BCEB8-9736-4030-BD2F-FB3CE6D6B478")]
    public void RestoreWebhooks(
      string providerName,
      [FromBody] ICollection<Microsoft.TeamFoundation.Build.WebApi.DefinitionTriggerType> triggerTypes,
      [ClientQueryParameter] Guid? serviceEndpointId = null,
      [ClientQueryParameter] string repository = null)
    {
      ArgumentUtility.CheckStringForNullOrEmpty(providerName, nameof (providerName));
      this.CheckRequestContent((object) triggerTypes);
      ArgumentUtility.CheckEnumerableForEmpty((IEnumerable) triggerTypes, nameof (triggerTypes));
      this.GetSourceProvider(providerName).RecreateSubscription(this.TfsRequestContext, this.ProjectId, serviceEndpointId, repository, (ICollection<Microsoft.TeamFoundation.Build2.Server.DefinitionTriggerType>) triggerTypes.Select<Microsoft.TeamFoundation.Build.WebApi.DefinitionTriggerType, Microsoft.TeamFoundation.Build2.Server.DefinitionTriggerType>((Func<Microsoft.TeamFoundation.Build.WebApi.DefinitionTriggerType, Microsoft.TeamFoundation.Build2.Server.DefinitionTriggerType>) (tt => tt.ToBuildServerDefinitionTriggerType())).ToList<Microsoft.TeamFoundation.Build2.Server.DefinitionTriggerType>());
    }

    private IBuildSourceProvider GetSourceProvider(string repositoryType) => this.TfsRequestContext.GetService<IBuildSourceProviderService>().GetSourceProvider(this.TfsRequestContext, repositoryType);
  }
}
