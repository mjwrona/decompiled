// Decompiled with JetBrains decompiler
// Type: Nest.PagerDutyAction
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System.Collections.Generic;

namespace Nest
{
  public class PagerDutyAction : ActionBase, IPagerDutyAction, IAction, IPagerDutyEvent
  {
    public PagerDutyAction(string name)
      : base(name)
    {
    }

    public string Account { get; set; }

    public override ActionType ActionType => ActionType.PagerDuty;

    public bool? AttachPayload { get; set; }

    public string Client { get; set; }

    public string ClientUrl { get; set; }

    public IEnumerable<IPagerDutyContext> Context { get; set; }

    public string Description { get; set; }

    public PagerDutyEventType? EventType { get; set; }

    public string IncidentKey { get; set; }
  }
}
