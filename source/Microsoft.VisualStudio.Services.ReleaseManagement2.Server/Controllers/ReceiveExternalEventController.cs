// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Server.Controllers.ReceiveExternalEventController
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 134B1041-BFA6-49C6-8C6D-CA5ADF31AF54
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Server.dll

using Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.Artifacts;
using Microsoft.TeamFoundation.DistributedTask.Pipelines.Artifacts.WebHooks;
using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.ReleaseManagement.Artifact.Extensions;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Exceptions;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model;
using Microsoft.VisualStudio.Services.ReleaseManagement.Server.Utilities;
using Microsoft.VisualStudio.Services.WebApi.Internal;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Server.Controllers
{
  [ControllerApiVersion(4.1)]
  [VersionedApiControllerCustomName(Area = "Release", ResourceName = "receiveExternalEvent")]
  [ClientIgnore]
  public class ReceiveExternalEventController : ReleaseManagementPublicControllerBase
  {
    private const int MaxPostRequestSize = 5242880;

    [SuppressMessage("Microsoft.Maintainability", "CA1506:AvoidExcessiveClassCoupling", Justification = "Required")]
    [ClientResponseType(typeof (void), null, null)]
    [HttpPost]
    public virtual HttpResponseMessage ReceiveExternalEvent(string webHookId)
    {
      Guid result;
      if (!Guid.TryParse(webHookId, out result))
        throw new ArgumentException(string.Format((IFormatProvider) CultureInfo.InvariantCulture, Microsoft.VisualStudio.Services.ReleaseManagement.Server.Properties.Resources.InvalidWebHookId, (object) webHookId));
      string eventPayload = string.Empty;
      using (RestrictedStream restrictedStream = new RestrictedStream(this.Request.Content.ReadAsStreamAsync().Result, 0L, 5242881L, true))
        eventPayload = new StreamReader((Stream) restrictedStream).ReadToEnd();
      WebHook webHook = this.TfsRequestContext.GetService<IWebHookService>().GetWebHook(this.TfsRequestContext, result, true);
      if (webHook == null)
        throw new WebHookException(string.Format((IFormatProvider) CultureInfo.InvariantCulture, Microsoft.VisualStudio.Services.ReleaseManagement.Server.Properties.Resources.WebHookIdNotFound, (object) webHookId));
      IArtifactType artifactType = (IArtifactType) ExtensionArtifactsRetriever.GetExtensionArtifacts(this.TfsRequestContext.Elevate()).FirstOrDefault<ArtifactTypeBase>((Func<ArtifactTypeBase, bool>) (x => x.Name.Equals(webHook.ArtifactType, StringComparison.OrdinalIgnoreCase)));
      if (artifactType == null)
      {
        this.TfsRequestContext.Trace(1976474, TraceLevel.Error, "ReleaseManagementService", "ArtifactTrigger", string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Cannot fetch artifact type definition for artifact type {0}", (object) webHook.ArtifactType));
        throw new WebHookException(string.Format((IFormatProvider) CultureInfo.InvariantCulture, Microsoft.VisualStudio.Services.ReleaseManagement.Server.Properties.Resources.CannotFetchArtifactTypeDefinition, (object) webHook.ArtifactType));
      }
      if (!WebHookHelper.ValidateWebHookPayload(this.TfsRequestContext, this.ActionContext.Request, eventPayload, webHook, artifactType))
        throw new ReleaseWebHookException(string.Format((IFormatProvider) CultureInfo.InvariantCulture, Microsoft.VisualStudio.Services.ReleaseManagement.Server.Properties.Resources.WebHookPayloadHashDoesNotMatch, (object) eventPayload));
      CustomArtifactTriggerEvent eventFromPayload = ReleaseWebHookHelper.GetCustomArtifactTriggerEventFromPayload(eventPayload, artifactType);
      foreach (IWebHookSubscription subscription in (IEnumerable<IWebHookSubscription>) webHook.Subscriptions)
      {
        eventFromPayload.UniqueSourceIdentifiers = new List<string>();
        ReleaseWebHookSubscription hookSubscription = subscription as ReleaseWebHookSubscription;
        if (hookSubscription.ArtifactUniqueSourceIdentifier != null)
          eventFromPayload.UniqueSourceIdentifiers.Add(hookSubscription.ArtifactUniqueSourceIdentifier);
      }
      ArtifactTriggerHelper.QueueJobToTriggerReleases(this.TfsRequestContext, eventFromPayload);
      return this.Request.CreateResponse(HttpStatusCode.OK);
    }
  }
}
