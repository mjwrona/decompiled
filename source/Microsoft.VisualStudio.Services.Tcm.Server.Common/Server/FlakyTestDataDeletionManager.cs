// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.FlakyTestDataDeletionManager
// Assembly: Microsoft.VisualStudio.Services.Tcm.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7631C286-897C-44D1-A133-A0BB6CC047F3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Tcm.Server.Common.dll

using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.TestManagement.Server
{
  internal class FlakyTestDataDeletionManager
  {
    private const string JobExtensionPath = "Microsoft.VisualStudio.Services.Tcm.Jobs.TcmDeleteFlakyTestDataJob";
    private const string JobName = "TcmDeleteFlakyTestDataJob";

    public void QueueFlakyTestDataDeletion(
      TestManagementRequestContext tcmRequestContext,
      Guid projectId)
    {
      tcmRequestContext.TraceEnter("FlakyTestDataDeleteJob", "FlakyTestDataDelete.QueueFlakyTestDetection");
      try
      {
        ITeamFoundationJobService service = tcmRequestContext.RequestContext.GetService<ITeamFoundationJobService>();
        TeamFoundationJobDefinition foundationJobDefinition = new TeamFoundationJobDefinition()
        {
          JobId = Guid.NewGuid(),
          ExtensionName = "Microsoft.VisualStudio.Services.Tcm.Jobs.TcmDeleteFlakyTestDataJob",
          Data = TeamFoundationSerializationUtility.SerializeToXml((object) projectId),
          Name = "TcmDeleteFlakyTestDataJob",
          PriorityClass = JobPriorityClass.Normal
        };
        service.UpdateJobDefinitions(tcmRequestContext.RequestContext, (IEnumerable<Guid>) null, (IEnumerable<TeamFoundationJobDefinition>) new List<TeamFoundationJobDefinition>()
        {
          foundationJobDefinition
        });
        service.QueueJobsNow(tcmRequestContext.RequestContext, (IEnumerable<TeamFoundationJobReference>) new List<TeamFoundationJobReference>()
        {
          foundationJobDefinition.ToJobReference()
        });
      }
      catch (Exception ex)
      {
        tcmRequestContext.TraceException("FlakyTestDataDeleteJob", ex);
      }
      tcmRequestContext.TraceLeave("FlakyTestDataDeleteJob", "FlakyTestDataDelete.QueueFlakyTestDetection");
    }
  }
}
