// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Pipelines.WebApiConverters.LogConverter
// Assembly: Microsoft.Azure.Pipelines.WebApiConverters, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 42DDCCD8-4E0C-44F8-A5D2-AEF894388AED
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Pipelines.WebApiConverters.dll

using Microsoft.Azure.Pipelines.Routes;
using Microsoft.Azure.Pipelines.Server;
using Microsoft.Azure.Pipelines.WebApi;
using Microsoft.TeamFoundation.DistributedTask.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.WebApi;
using System;

namespace Microsoft.Azure.Pipelines.WebApiConverters
{
  public class LogConverter
  {
    public static readonly VersionedObjectFactory VersionedObjectFactory = new VersionedObjectFactory(new IVersionedObjectCreator[1]
    {
      (IVersionedObjectCreator) new VersionedObjectCreator<LogConverter>(1)
    });

    public virtual Log ToWebApiLog(
      IVssRequestContext requestContext,
      Guid projectId,
      TaskLog source,
      int pipelineId,
      int runId,
      bool includeSignedContent,
      Action<IVssRequestContext, int> signedContentPermissionCheck)
    {
      if (source == null)
        return (Log) null;
      IPipelinesRouteService service1 = requestContext.GetService<IPipelinesRouteService>();
      Log webApiLog = new Log()
      {
        CreatedOn = new DateTime?(source.CreatedOn),
        Id = source.Id,
        LastChangedOn = new DateTime?(source.LastChangedOn),
        LineCount = source.LineCount,
        Url = service1.GetLogRestUrl(requestContext, projectId, pipelineId, runId, source.Id)
      };
      if (includeSignedContent)
      {
        string logContentRestUrl = service1.GetSignedLogContentRestUrl(requestContext, projectId, pipelineId, runId, source.Id);
        signedContentPermissionCheck(requestContext, source.Id);
        IUrlSigningService service2 = requestContext.GetService<IUrlSigningService>();
        DateTime expires = DateTime.UtcNow.Add(SignedContentConstants.TimeToLive);
        webApiLog.SignedContent = new SignedUrl()
        {
          Url = service2.Sign(requestContext, new Uri(logContentRestUrl), expires),
          SignatureExpires = expires
        };
      }
      return webApiLog;
    }
  }
}
