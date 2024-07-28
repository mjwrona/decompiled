// Decompiled with JetBrains decompiler
// Type: Nest.ScheduleDescriptor
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;
using System.Collections.Generic;

namespace Nest
{
  public class ScheduleDescriptor : 
    DescriptorBase<ScheduleDescriptor, IScheduleContainer>,
    IScheduleContainer
  {
    CronExpression IScheduleContainer.Cron { get; set; }

    ICronExpressions IScheduleContainer.CronExpressions { get; set; }

    IDailySchedule IScheduleContainer.Daily { get; set; }

    IHourlySchedule IScheduleContainer.Hourly { get; set; }

    Nest.Interval IScheduleContainer.Interval { get; set; }

    IMonthlySchedule IScheduleContainer.Monthly { get; set; }

    IWeeklySchedule IScheduleContainer.Weekly { get; set; }

    IYearlySchedule IScheduleContainer.Yearly { get; set; }

    public ScheduleDescriptor Daily(
      Func<DailyScheduleDescriptor, IDailySchedule> selector)
    {
      return this.Assign<IDailySchedule>(selector(new DailyScheduleDescriptor()), (Action<IScheduleContainer, IDailySchedule>) ((a, v) => a.Daily = v));
    }

    public ScheduleDescriptor Hourly(
      Func<HourlyScheduleDescriptor, IHourlySchedule> selector)
    {
      return this.Assign<IHourlySchedule>(selector(new HourlyScheduleDescriptor()), (Action<IScheduleContainer, IHourlySchedule>) ((a, v) => a.Hourly = v));
    }

    public ScheduleDescriptor Monthly(
      Func<MonthlyScheduleDescriptor, IPromise<IMonthlySchedule>> selector)
    {
      return this.Assign<IMonthlySchedule>(selector(new MonthlyScheduleDescriptor())?.Value, (Action<IScheduleContainer, IMonthlySchedule>) ((a, v) => a.Monthly = v));
    }

    public ScheduleDescriptor Weekly(
      Func<WeeklyScheduleDescriptor, IPromise<IWeeklySchedule>> selector)
    {
      return this.Assign<IWeeklySchedule>(selector(new WeeklyScheduleDescriptor())?.Value, (Action<IScheduleContainer, IWeeklySchedule>) ((a, v) => a.Weekly = v));
    }

    public ScheduleDescriptor Yearly(
      Func<YearlyScheduleDescriptor, IPromise<IYearlySchedule>> selector)
    {
      return this.Assign<IYearlySchedule>(selector(new YearlyScheduleDescriptor())?.Value, (Action<IScheduleContainer, IYearlySchedule>) ((a, v) => a.Yearly = v));
    }

    public ScheduleDescriptor Cron(CronExpression cron) => this.Assign<CronExpression>(cron, (Action<IScheduleContainer, CronExpression>) ((a, v) =>
    {
      if (a.CronExpressions != null)
        a.CronExpressions.Add(v);
      else if ((object) a.Cron == null)
      {
        a.Cron = v;
      }
      else
      {
        a.Cron = (CronExpression) null;
        a.CronExpressions = (ICronExpressions) new Nest.CronExpressions(new CronExpression[1]
        {
          v
        });
      }
    }));

    public ScheduleDescriptor Cron(Nest.CronExpressions expressions) => this.Assign<Nest.CronExpressions>(expressions, (Action<IScheduleContainer, Nest.CronExpressions>) ((a, v) => a.CronExpressions = (ICronExpressions) v));

    public ScheduleDescriptor Cron(params CronExpression[] expressions) => this.Assign<CronExpression[]>(expressions, (Action<IScheduleContainer, CronExpression[]>) ((a, v) => a.CronExpressions = (ICronExpressions) new Nest.CronExpressions(expressions)));

    public ScheduleDescriptor Cron(IEnumerable<CronExpression> expressions) => this.Assign<IEnumerable<CronExpression>>(expressions, (Action<IScheduleContainer, IEnumerable<CronExpression>>) ((a, v) => a.CronExpressions = (ICronExpressions) new Nest.CronExpressions(v)));

    public ScheduleDescriptor CronExpressions(
      Func<CronExpressionsDescriptor, IPromise<ICronExpressions>> selector)
    {
      return this.Assign<ICronExpressions>(selector(new CronExpressionsDescriptor())?.Value, (Action<IScheduleContainer, ICronExpressions>) ((a, v) => a.CronExpressions = v));
    }

    public ScheduleDescriptor Interval(Nest.Interval interval) => this.Assign<Nest.Interval>(interval, (Action<IScheduleContainer, Nest.Interval>) ((a, v) => a.Interval = v));
  }
}
