// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Audit.AuditLogEntry
// Assembly: Microsoft.VisualStudio.Services.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7B264323-C592-4F23-AB6B-55AEDC85864F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.WebApi.dll

using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Notifications;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.Audit
{
  [DataContract]
  [NotificationEventBindings(EventSerializerType.Json, "ms.vss-notifications.audit-event")]
  public class AuditLogEntry
  {
    public AuditLogEntry()
    {
    }

    public AuditLogEntry(AuditLogEntry entry)
    {
      ArgumentUtility.CheckForNull<AuditLogEntry>(entry, nameof (entry));
      this.Id = entry.Id;
      this.CorrelationId = entry.CorrelationId;
      this.ActionId = entry.ActionId;
      this.ActivityId = entry.ActivityId;
      this.ActorCUID = entry.ActorCUID;
      this.ActorUserId = entry.ActorUserId;
      this.ActorClientId = entry.ActorClientId;
      this.ActorUPN = entry.ActorUPN;
      this.AuthenticationMechanism = entry.AuthenticationMechanism;
      this.Timestamp = entry.Timestamp;
      this.ScopeType = entry.ScopeType;
      this.ScopeId = entry.ScopeId;
      this.ProjectId = entry.ProjectId;
      this.IPAddress = entry.IPAddress;
      this.UserAgent = entry.UserAgent;
      this.Data = entry.Data != null ? (IDictionary<string, object>) new Dictionary<string, object>(entry.Data) : (IDictionary<string, object>) new Dictionary<string, object>();
    }

    [DataMember]
    public Guid Id { get; set; }

    [DataMember]
    public Guid CorrelationId { get; set; }

    [DataMember]
    public Guid ActivityId { get; set; }

    [DataMember]
    public Guid ActorCUID { get; set; }

    [DataMember]
    public Guid ActorUserId { get; set; }

    [DataMember]
    public Guid ActorClientId { get; set; }

    [DataMember]
    public string ActorUPN { get; set; }

    [DataMember]
    public string AuthenticationMechanism { get; set; }

    [DataMember]
    public DateTime Timestamp { get; set; }

    [DataMember]
    public AuditScopeType ScopeType { get; set; }

    [DataMember]
    public Guid ScopeId { get; set; }

    [DataMember]
    public Guid ProjectId { get; set; }

    [DataMember]
    public string IPAddress { get; set; }

    [DataMember]
    public string UserAgent { get; set; }

    [DataMember]
    public string ActionId { get; set; }

    [DataMember]
    public IDictionary<string, object> Data { get; set; }

    public override string ToString() => JsonUtility.ToString((object) this);
  }
}
