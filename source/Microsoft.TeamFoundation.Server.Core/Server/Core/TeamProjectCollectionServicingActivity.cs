// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.Core.TeamProjectCollectionServicingActivity
// Assembly: Microsoft.TeamFoundation.Server.Core, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9DD3208E-87CF-4F7C-8D96-8880BDAD13B2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.Core.dll

using Microsoft.TeamFoundation.Framework.Server;

namespace Microsoft.TeamFoundation.Server.Core
{
  public class TeamProjectCollectionServicingActivity
  {
    public TeamProjectCollectionServicingActivity()
    {
    }

    public TeamProjectCollectionServicingActivity(
      TeamFoundationJobExecutionEntry latestJobExecutionEntry,
      string[] latestServicingOperations,
      ServicingJobData servicingJobData)
    {
      this.LatestJobExecutionEntry = latestJobExecutionEntry;
      this.LatestServicingOperations = latestServicingOperations;
      this.ServicingJobData = servicingJobData;
    }

    public TeamFoundationJobExecutionEntry LatestJobExecutionEntry { get; set; }

    internal ServicingJobData ServicingJobData { get; set; }

    [ClientProperty(ClientVisibility.Private, ClientVisibility.Private)]
    public string[] LatestServicingOperations { get; set; }
  }
}
