// Decompiled with JetBrains decompiler
// Type: Nest.DailySchedule
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System.Collections.Generic;

namespace Nest
{
  public class DailySchedule : ScheduleBase, IDailySchedule, ISchedule
  {
    public DailySchedule()
    {
    }

    public DailySchedule(string at) => this.At = (Union<IEnumerable<string>, IEnumerable<ITimeOfDay>>) (IEnumerable<string>) new string[1]
    {
      at
    };

    public DailySchedule(ITimeOfDay at) => this.At = (Union<IEnumerable<string>, IEnumerable<ITimeOfDay>>) (IEnumerable<ITimeOfDay>) new ITimeOfDay[1]
    {
      at
    };

    public Union<IEnumerable<string>, IEnumerable<ITimeOfDay>> At { get; set; }

    internal override void WrapInContainer(IScheduleContainer container) => container.Daily = (IDailySchedule) this;
  }
}
