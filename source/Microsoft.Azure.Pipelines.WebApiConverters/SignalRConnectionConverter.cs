// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Pipelines.WebApiConverters.SignalRConnectionConverter
// Assembly: Microsoft.Azure.Pipelines.WebApiConverters, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 42DDCCD8-4E0C-44F8-A5D2-AEF894388AED
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Pipelines.WebApiConverters.dll

using Microsoft.Azure.Pipelines.Routes;
using Microsoft.Azure.Pipelines.Server;
using Microsoft.Azure.Pipelines.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.WebApi;
using System;

namespace Microsoft.Azure.Pipelines.WebApiConverters
{
  public class SignalRConnectionConverter
  {
    public static readonly VersionedObjectFactory VersionedObjectFactory = new VersionedObjectFactory(new IVersionedObjectCreator[1]
    {
      (IVersionedObjectCreator) new VersionedObjectCreator<SignalRConnectionConverter>(1)
    });

    public virtual SignalRConnection ToWebApiUrl(
      IVssRequestContext requestContext,
      Guid projectId,
      int pipelineId,
      int runId)
    {
      string rwebsocketRestUrl = requestContext.GetService<IPipelinesRouteService>().GetSignedSignalRWebsocketRestUrl(requestContext, projectId, pipelineId, runId);
      IUrlSigningService service = requestContext.GetService<IUrlSigningService>();
      DateTime expires = DateTime.UtcNow.Add(SignedContentConstants.TimeToLive);
      return new SignalRConnection()
      {
        SignedContent = new SignedUrl()
        {
          Url = service.Sign(requestContext, new Uri(rwebsocketRestUrl), expires),
          SignatureExpires = expires
        }
      };
    }
  }
}
