// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ScheduledReleaseTrigger
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Data, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A69B0FB7-3028-4162-BA38-794D80D7A49A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Data.dll

using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Converters;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Enumerations;
using Newtonsoft.Json;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model
{
  public class ScheduledReleaseTrigger : ReleaseTriggerBase
  {
    private ReleaseSchedule schedule;

    public ScheduledReleaseTrigger() => this.TriggerType = ReleaseTriggerType.Schedule;

    public ReleaseSchedule Schedule
    {
      get
      {
        if (this.schedule == null)
          this.schedule = JsonConvert.DeserializeObject<ReleaseSchedule>(this.TriggerContent);
        return this.schedule;
      }
      set
      {
        this.schedule = value;
        this.TriggerContent = JsonConvert.SerializeObject((object) this.schedule);
      }
    }

    public string TriggerContent { get; set; }

    public override ReleaseTriggerBase DeepClone()
    {
      ScheduledReleaseTrigger scheduledReleaseTrigger = base.DeepClone().ToScheduledReleaseTrigger();
      scheduledReleaseTrigger.schedule = this.schedule == null ? (ReleaseSchedule) null : this.schedule.DeepClone();
      return (ReleaseTriggerBase) scheduledReleaseTrigger;
    }
  }
}
