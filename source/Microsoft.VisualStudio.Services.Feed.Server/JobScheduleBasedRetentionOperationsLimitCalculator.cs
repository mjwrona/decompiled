// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Feed.Server.JobScheduleBasedRetentionOperationsLimitCalculator
// Assembly: Microsoft.VisualStudio.Services.Feed.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 55555083-3B79-4F4F-AA85-92D66019974E
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Feed.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Feed.Server
{
  public class JobScheduleBasedRetentionOperationsLimitCalculator : 
    IRetentionOperationsLimitCalculator
  {
    public const int MaxDailyRetentionDeleteOperationsDefault = 40000;
    public const string MaxDailyRetentionDeleteOperationsKey = "/Configuration/Feed/MaxDailyRetentionDeleteOperations";
    private const int SecondsPerDay = 86400;
    private readonly TeamFoundationJobDefinition retentionJobDefinition;

    public JobScheduleBasedRetentionOperationsLimitCalculator(
      TeamFoundationJobDefinition retentionJobDefinition)
    {
      this.retentionJobDefinition = retentionJobDefinition;
    }

    public int GetFeedProtocolOperationsLimit(IVssRequestContext requestContext)
    {
      int num1 = requestContext.GetService<IVssRegistryService>().GetValue<int>(requestContext, (RegistryQuery) "/Configuration/Feed/MaxDailyRetentionDeleteOperations", true, 40000);
      TeamFoundationJobSchedule foundationJobSchedule = this.retentionJobDefinition.Schedule.FirstOrDefault<TeamFoundationJobSchedule>();
      int num2 = foundationJobSchedule != null ? foundationJobSchedule.Interval : 86400;
      int num3 = 86400 / (num2 > 86400 ? 86400 : num2);
      return num1 / num3;
    }
  }
}
