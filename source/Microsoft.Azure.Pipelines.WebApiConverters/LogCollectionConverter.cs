// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Pipelines.WebApiConverters.LogCollectionConverter
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
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.Azure.Pipelines.WebApiConverters
{
  public class LogCollectionConverter
  {
    public static readonly VersionedObjectFactory VersionedObjectFactory = new VersionedObjectFactory(new IVersionedObjectCreator[1]
    {
      (IVersionedObjectCreator) new VersionedObjectCreator<LogCollectionConverter>(1)
    });
    private const int ResourceVersion = 1;

    public virtual LogCollection ToWebApiLogCollection(
      IVssRequestContext requestContext,
      Guid projectId,
      IEnumerable<TaskLog> source,
      int pipelineId,
      int runId,
      bool includeSignedContent,
      Action<IVssRequestContext, int> signedContentPermissionCheck)
    {
      if (source == null)
        return (LogCollection) null;
      LogConverter logConverter = requestContext.CreateVersionedObject<LogConverter>(1);
      IPipelinesRouteService service1 = requestContext.GetService<IPipelinesRouteService>();
      LogCollection apiLogCollection = new LogCollection()
      {
        Logs = source.Select<TaskLog, Log>((Func<TaskLog, Log>) (log => logConverter.ToWebApiLog(requestContext, projectId, log, pipelineId, runId, includeSignedContent, signedContentPermissionCheck))),
        Url = service1.GetLogCollectionRestUrl(requestContext, projectId, pipelineId, runId)
      };
      if (includeSignedContent)
      {
        string logsContentRestUrl = service1.GetSignedLogsContentRestUrl(requestContext, projectId, pipelineId, runId);
        signedContentPermissionCheck(requestContext, 0);
        IUrlSigningService service2 = requestContext.GetService<IUrlSigningService>();
        DateTime expires = DateTime.UtcNow.Add(SignedContentConstants.TimeToLive);
        apiLogCollection.SignedContent = new SignedUrl()
        {
          Url = service2.Sign(requestContext, new Uri(logsContentRestUrl), expires),
          SignatureExpires = expires
        };
      }
      return apiLogCollection;
    }
  }
}
