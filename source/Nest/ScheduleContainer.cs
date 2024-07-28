// Decompiled with JetBrains decompiler
// Type: Nest.ScheduleContainer
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

namespace Nest
{
  public class ScheduleContainer : TriggerBase, IScheduleContainer
  {
    public ScheduleContainer()
    {
    }

    public ScheduleContainer(ScheduleBase schedule)
    {
      schedule.ThrowIfNull<ScheduleBase>(nameof (schedule));
      schedule.WrapInContainer((IScheduleContainer) this);
    }

    public CronExpression Cron { get; set; }

    public ICronExpressions CronExpressions { get; set; }

    public IDailySchedule Daily { get; set; }

    public IHourlySchedule Hourly { get; set; }

    public Interval Interval { get; set; }

    public IMonthlySchedule Monthly { get; set; }

    public IWeeklySchedule Weekly { get; set; }

    public IYearlySchedule Yearly { get; set; }

    internal override void WrapInContainer(ITriggerContainer container) => container.Schedule = (IScheduleContainer) this;

    public static implicit operator ScheduleContainer(ScheduleBase scheduleBase) => scheduleBase != null ? new ScheduleContainer(scheduleBase) : (ScheduleContainer) null;
  }
}
