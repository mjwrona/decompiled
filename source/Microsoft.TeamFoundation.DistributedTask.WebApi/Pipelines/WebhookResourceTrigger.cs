// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Pipelines.WebhookResourceTrigger
// Assembly: Microsoft.TeamFoundation.DistributedTask.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9201F3B5-DEAF-44A3-860C-DB7B277BB5C6
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.WebApi.dll

using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.DistributedTask.Pipelines
{
  [DataContract]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public class WebhookResourceTrigger : PipelineTrigger, IEquatable<WebhookResourceTrigger>
  {
    [DataMember(Name = "Filters", EmitDefaultValue = false)]
    private IDictionary<string, string> m_filters;

    public WebhookResourceTrigger()
      : base(PipelineTriggerType.WebhookTriggeredEvent)
    {
    }

    private WebhookResourceTrigger(WebhookResourceTrigger triggerToClone)
      : base(PipelineTriggerType.WebhookTriggeredEvent)
    {
      this.Filters.AddRange<KeyValuePair<string, string>, IDictionary<string, string>>((IEnumerable<KeyValuePair<string, string>>) triggerToClone.Filters);
    }

    public IDictionary<string, string> Filters
    {
      get
      {
        if (this.m_filters == null)
          this.m_filters = (IDictionary<string, string>) new Dictionary<string, string>();
        return this.m_filters;
      }
    }

    public WebhookResourceTrigger Clone() => new WebhookResourceTrigger(this);

    public bool Equals(WebhookResourceTrigger other) => other != null && this.TriggerType == other.TriggerType && this.Enabled == other.Enabled && !this.Filters.Any<KeyValuePair<string, string>>((Func<KeyValuePair<string, string>, bool>) (x => !other.Filters.ContainsKey(x.Key) || other.Filters[x.Key] != x.Value)) && !other.Filters.Any<KeyValuePair<string, string>>((Func<KeyValuePair<string, string>, bool>) (x => !this.Filters.ContainsKey(x.Key) || this.Filters[x.Key] != x.Value));

    public override bool Equals(object obj)
    {
      if (obj == null)
        return false;
      if (this == obj)
        return true;
      return !(obj.GetType() != this.GetType()) && this.Equals((WebhookResourceTrigger) obj);
    }

    public override int GetHashCode() => this.Enabled.GetHashCode() * 397 ^ this.TriggerType.GetHashCode();
  }
}
