// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Notifications.VssNotificationEvent
// Assembly: Microsoft.VisualStudio.Services.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7B264323-C592-4F23-AB6B-55AEDC85864F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.WebApi.dll

using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.Notifications
{
  [DataContract]
  public class VssNotificationEvent : ICloneable
  {
    protected DateTime m_createdTime;
    private List<EventActor> m_actors;
    private List<EventScope> m_scopes;
    private List<string> m_artifactUris;
    public static readonly TimeSpan ProcessNow = TimeSpan.Zero;
    public static readonly TimeSpan DefaultExpiration = TimeSpan.MinValue;
    public static readonly TimeSpan NeverExpire = TimeSpan.MaxValue;

    public VssNotificationEvent() => this.m_createdTime = DateTime.UtcNow;

    public VssNotificationEvent(object data)
      : this()
    {
      this.InitFromObject(data);
    }

    public VssNotificationEvent(string serializedEvent, string eventType)
      : this()
    {
      this.Data = (object) serializedEvent;
      this.EventType = eventType;
    }

    public void InitFromObject(object data)
    {
      this.Data = !(data is string) ? data : throw new ArgumentException("Must use VssNotificationEvent(string, string) for already serialized events");
      this.EventType = data.GetType().Name;
    }

    [DataMember]
    public string EventType { get; set; }

    [DataMember]
    public object Data { get; set; }

    [DataMember]
    public List<EventActor> Actors
    {
      get
      {
        if (this.m_actors == null)
          this.m_actors = new List<EventActor>();
        return this.m_actors;
      }
    }

    [DataMember]
    public List<EventScope> Scopes
    {
      get
      {
        if (this.m_scopes == null)
          this.m_scopes = new List<EventScope>();
        return this.m_scopes;
      }
    }

    [DataMember]
    public List<string> ArtifactUris
    {
      get
      {
        if (this.m_artifactUris == null)
          this.m_artifactUris = new List<string>();
        return this.m_artifactUris;
      }
    }

    [DataMember]
    public TimeSpan ProcessDelay { get; set; } = VssNotificationEvent.ProcessNow;

    [DataMember]
    public TimeSpan ExpiresIn { get; set; } = VssNotificationEvent.DefaultExpiration;

    [DataMember]
    public DateTime? SourceEventCreatedTime { get; set; }

    [DataMember]
    public string ItemId { get; set; }

    public bool HasActors => this.m_actors != null && this.m_actors.Count > 0;

    public bool HasScopes => this.m_scopes != null && this.m_scopes.Count > 0;

    public bool HasArtifactUris => this.m_artifactUris != null && this.m_artifactUris.Count > 0;

    public void AddActor(string role, Guid id) => this.Actors.Add(new EventActor()
    {
      Role = role,
      Id = id
    });

    public void AddScope(string type, Guid id) => this.AddScope(type, id, (string) null);

    public void AddScope(string type, Guid id, string name) => this.Scopes.Add(new EventScope()
    {
      Type = type,
      Id = id,
      Name = name
    });

    public void AddArtifactUri(string artificatUri) => this.ArtifactUris.Add(artificatUri);

    public void AddSystemInitiatorActor() => this.AddActor(VssNotificationEvent.Roles.Initiator, VssNotificationEvent.KnownInitiators.System);

    public object Clone()
    {
      VssNotificationEvent notificationEvent = new VssNotificationEvent();
      notificationEvent.CloneFrom(this);
      return (object) notificationEvent;
    }

    protected virtual void CloneFrom(VssNotificationEvent other)
    {
      this.Actors.AddRange((IEnumerable<EventActor>) other.Actors);
      this.ArtifactUris.AddRange((IEnumerable<string>) other.ArtifactUris);
      this.Data = other.Data;
      this.EventType = other.EventType;
      this.Scopes.AddRange((IEnumerable<EventScope>) other.Scopes);
      this.ProcessDelay = other.ProcessDelay;
      this.ExpiresIn = other.ExpiresIn;
      this.SourceEventCreatedTime = other.SourceEventCreatedTime;
      this.ItemId = other.ItemId;
      this.m_createdTime = other.m_createdTime;
    }

    public static class Roles
    {
      public static readonly string MainActor = "mainActor";
      public static readonly string Initiator = "initiator";
    }

    public static class ScopeNames
    {
      public static readonly string Project = "project";
      public static readonly string Repository = "repository";
    }

    public static class KnownInitiators
    {
      public static readonly Guid System = new Guid("00d7d880-a761-45b5-bca7-394bc63d0cc3");
    }
  }
}
