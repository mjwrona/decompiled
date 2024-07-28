// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Server.Controllers.DistributedTaskApiController
// Assembly: Microsoft.TeamFoundation.DistributedTask.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: EDA28B25-3B93-49F7-A194-C85AAF98B83A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Server.dll

using Microsoft.TeamFoundation.DistributedTask.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Framework.Server.WebApis;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;

namespace Microsoft.TeamFoundation.DistributedTask.Server.Controllers
{
  [TaskYieldOnException]
  public abstract class DistributedTaskApiController : TfsApiController
  {
    public override string TraceArea => "DistributedTask";

    public override string ActivityLogArea => "DistributedTask";

    public IAgentCloudService AgentCloudService => this.TfsRequestContext.GetService<IAgentCloudService>();

    public IDistributedTaskResourceService ResourceService => this.TfsRequestContext.GetService<IDistributedTaskResourceService>();

    public IDistributedTaskService TaskService => this.TfsRequestContext.GetService<IDistributedTaskService>();

    protected override void InitializeExceptionMap(ApiExceptionMapping exceptionMap)
    {
      base.InitializeExceptionMap(exceptionMap);
      DistributedTaskExceptionMapper.Map(exceptionMap);
    }

    protected IList<int> ParseArray(string array, char delimiter = ',') => DistributedTaskApiControllerHelper.ParseArray(array, delimiter);

    protected bool ZipFileRequested()
    {
      bool flag1 = false;
      double num1 = 0.0;
      bool flag2 = false;
      double num2 = 0.0;
      foreach (MediaTypeWithQualityHeaderValue qualityHeaderValue in this.Request.Headers.Accept)
      {
        double? quality;
        if (qualityHeaderValue.MediaType.Equals("application/zip", StringComparison.OrdinalIgnoreCase))
        {
          flag1 = true;
          quality = qualityHeaderValue.Quality;
          if (quality.HasValue)
          {
            quality = qualityHeaderValue.Quality;
            if (quality.HasValue)
            {
              quality = qualityHeaderValue.Quality;
              num1 = quality.Value;
            }
          }
        }
        if (qualityHeaderValue.MediaType.Equals("application/json", StringComparison.OrdinalIgnoreCase))
        {
          flag2 = true;
          quality = qualityHeaderValue.Quality;
          if (quality.HasValue)
          {
            quality = qualityHeaderValue.Quality;
            if (quality.HasValue)
            {
              quality = qualityHeaderValue.Quality;
              num2 = quality.Value;
            }
          }
        }
      }
      return flag2 & flag1 && num1 > num2 || !flag2 & flag1 || !flag2 && !flag1;
    }

    protected static void FixAgentPlatform(TaskAgentReference agent, string userAgent)
    {
      if (agent == null || string.IsNullOrEmpty(userAgent) || !string.IsNullOrEmpty(agent.OSDescription))
        return;
      agent.OSDescription = DistributedTaskResourceServiceHelper.GetAgentOSDescription(userAgent);
    }

    protected static void SetContinuationToken(
      HttpResponseMessage responseMessage,
      string tokenValue)
    {
      if (responseMessage == null)
        throw new ArgumentNullException(nameof (responseMessage));
      if (string.IsNullOrWhiteSpace(tokenValue))
        return;
      responseMessage.Headers.Add("X-MS-ContinuationToken", tokenValue);
    }
  }
}
