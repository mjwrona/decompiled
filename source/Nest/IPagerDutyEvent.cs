// Decompiled with JetBrains decompiler
// Type: Nest.IPagerDutyEvent
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net.Utf8Json;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Nest
{
  [InterfaceDataContract]
  [ReadAs(typeof (PagerDutyEvent))]
  public interface IPagerDutyEvent
  {
    [DataMember(Name = "account")]
    string Account { get; set; }

    [DataMember(Name = "attach_payload")]
    bool? AttachPayload { get; set; }

    [DataMember(Name = "client")]
    string Client { get; set; }

    [DataMember(Name = "client_url")]
    string ClientUrl { get; set; }

    [DataMember(Name = "context")]
    IEnumerable<IPagerDutyContext> Context { get; set; }

    [DataMember(Name = "description")]
    string Description { get; set; }

    [DataMember(Name = "event_type")]
    PagerDutyEventType? EventType { get; set; }

    [DataMember(Name = "incident_key")]
    string IncidentKey { get; set; }
  }
}
