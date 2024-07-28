// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.SocialServer.Server.SocialActivityNewPartitionCreationJob
// Assembly: Microsoft.VisualStudio.Services.Social.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6878458A-724A-4C44-954E-B2170F10219E
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Social.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.JobService.Extensions.Core;
using Microsoft.VisualStudio.Services.SocialServer.Server.DataAccess;
using System;
using System.Diagnostics;

namespace Microsoft.VisualStudio.Services.SocialServer.Server
{
  public class SocialActivityNewPartitionCreationJob : DatabaseWideJob
  {
    private int m_minPartitionSpanInSec;

    protected override bool ConsiderServicingDatabasesOnline { get; } = true;

    protected override string JobName { get; } = "Social Activity Partition Creation Job";

    protected override string Area { get; } = "SocialActivityAggregation";

    protected override string Layer { get; } = nameof (SocialActivityNewPartitionCreationJob);

    public override TeamFoundationJobExecutionResult Run(
      IVssRequestContext requestContext,
      TeamFoundationJobDefinition jobDefinition,
      DateTime queueTime,
      out string resultMessage)
    {
      this.m_minPartitionSpanInSec = RegistryHelper.GetRegistryValue<int>(requestContext, "/Configuration/SocialSDK/SocialActivity/SocialActivityMinimumPartitionSpanInSec", 72000);
      return base.Run(requestContext, jobDefinition, queueTime, out resultMessage);
    }

    protected override void ProcessOnlineDatabase(
      IVssRequestContext requestContext,
      ITeamFoundationDatabaseProperties database)
    {
      if (database.DatabaseId == requestContext.ServiceHost.DeploymentServiceHost.DatabaseProperties.DatabaseId)
      {
        requestContext.Trace(11000002, TraceLevel.Info, this.Area, this.Layer, (string[]) null, "Skipping config db");
      }
      else
      {
        using (SocialActivityDatabaseWideComponent componentRaw = database.GetDboConnectionInfo().CreateComponentRaw<SocialActivityDatabaseWideComponent>())
        {
          DateTime activityPartition = componentRaw.CreateNewSocialActivityPartition(this.m_minPartitionSpanInSec);
          CustomerIntelligenceService service = requestContext.GetService<CustomerIntelligenceService>();
          CustomerIntelligenceData intelligenceData = new CustomerIntelligenceData();
          intelligenceData.Add("DatabaseId", (double) database.DatabaseId);
          intelligenceData.Add("CreatedPartitionBoundaryTime", (object) activityPartition);
          IVssRequestContext requestContext1 = requestContext;
          string area = this.Area;
          string layer = this.Layer;
          CustomerIntelligenceData properties = intelligenceData;
          service.Publish(requestContext1, area, layer, properties);
        }
      }
    }
  }
}
