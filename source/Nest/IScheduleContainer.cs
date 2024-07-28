// Decompiled with JetBrains decompiler
// Type: Nest.IScheduleContainer
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net.Utf8Json;

namespace Nest
{
  [JsonFormatter(typeof (ScheduleContainerFormatter))]
  public interface IScheduleContainer
  {
    CronExpression Cron { get; set; }

    ICronExpressions CronExpressions { get; set; }

    IDailySchedule Daily { get; set; }

    IHourlySchedule Hourly { get; set; }

    Interval Interval { get; set; }

    IMonthlySchedule Monthly { get; set; }

    IWeeklySchedule Weekly { get; set; }

    IYearlySchedule Yearly { get; set; }
  }
}
