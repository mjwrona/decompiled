// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.TrendDataRebuildJobExtension
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server
{
  public class TrendDataRebuildJobExtension : ITeamFoundationJobExtension
  {
    public TeamFoundationJobExecutionResult Run(
      IVssRequestContext requestContext,
      TeamFoundationJobDefinition jobDefinition,
      DateTime jobQueueTime,
      out string resultMessage)
    {
      resultMessage = (string) null;
      if (jobDefinition.Data == null)
        throw new TeamFoundationServerException("Job data is empty");
      if (jobDefinition.JobId == TrendDataRebuildJobExtension.JobId)
      {
        ExtensionTrendDataRebuildData[] trendDataRebuildDataArray = TeamFoundationSerializationUtility.Deserialize<ExtensionTrendDataRebuildData[]>(jobDefinition.Data);
        if (trendDataRebuildDataArray.Length != 0)
        {
          IWorkItemTypeExtensionService service = requestContext.GetService<IWorkItemTypeExtensionService>();
          foreach (ExtensionTrendDataRebuildData trendDataRebuildData in trendDataRebuildDataArray)
          {
            ExtensionTrendDataRebuildData rebuildData = trendDataRebuildData;
            WorkItemTypeExtension itemTypeExtension = service.GetExtensions(requestContext, (IEnumerable<Guid>) new Guid[1]
            {
              rebuildData.ExtensionId
            }).FirstOrDefault<WorkItemTypeExtension>();
            if (itemTypeExtension == null)
              throw new TeamFoundationServerException("Extension does not exist");
            requestContext.GetService<WorkItemTrendService>().RebuildTrendData(requestContext, (itemTypeExtension.Fields.FirstOrDefault<WorkItemTypeExtensionFieldEntry>((Func<WorkItemTypeExtensionFieldEntry, bool>) (f => f.Field.FieldId == rebuildData.FieldId)) ?? throw new TeamFoundationServerException("Field does not exist")).Field.ReferenceName, itemTypeExtension.MarkerField.Field.ReferenceName, DateTime.MinValue, true);
          }
        }
      }
      else
      {
        TrendDataRebuildJobData jobData = TeamFoundationSerializationUtility.Deserialize<TrendDataRebuildJobData>(jobDefinition.Data);
        requestContext.GetService<WorkItemTrendService>().EndTrendDataRebuild(requestContext, jobData);
      }
      return TeamFoundationJobExecutionResult.Succeeded;
    }

    internal static string JobName => "Trend Data Rebuild Job";

    internal static string ExtensionName => typeof (TrendDataRebuildJobExtension).ToString();

    internal static Guid JobId => new Guid("92B0B9FA-2C1A-41A3-907B-418C712F8F84");
  }
}
