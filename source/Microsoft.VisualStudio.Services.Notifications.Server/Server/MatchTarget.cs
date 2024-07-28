// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Notifications.Server.MatchTarget
// Assembly: Microsoft.VisualStudio.Services.Notifications.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8ED1F52D-E567-4EFA-AAED-F2D9262BE9C2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Notifications.Server.dll

using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Notifications.Server
{
  public class MatchTarget
  {
    private TeamFoundationEvent m_event;
    private HashSet<NotificationRecipient> m_recipients = new HashSet<NotificationRecipient>();
    private string m_channel;
    private string m_matchChannel;
    private bool m_isStable;
    private int? m_hashCode;

    public MatchTarget() => this.m_recipients = new HashSet<NotificationRecipient>();

    public bool IsStable
    {
      get => this.m_isStable;
      set
      {
        if (!value)
          throw new ArgumentException("value must be true!");
        this.m_isStable = true;
      }
    }

    public TeamFoundationEvent Event
    {
      get => this.m_event;
      set
      {
        this.ThrowIfStable();
        this.m_event = value;
        this.m_hashCode = new int?();
      }
    }

    public string Channel
    {
      get => this.m_channel;
      set
      {
        this.ThrowIfStable();
        this.m_channel = value;
        this.m_matchChannel = !string.Equals(value, "User") ? value : "EmailHtml";
        this.m_hashCode = new int?();
      }
    }

    public void AddRecipient(NotificationRecipient recipient)
    {
      this.ThrowIfStable();
      this.m_recipients.Add(recipient);
      this.m_hashCode = new int?();
    }

    public void AddRecipients(IEnumerable<NotificationRecipient> recipients)
    {
      this.ThrowIfStable();
      foreach (NotificationRecipient recipient in recipients)
        this.m_recipients.Add(recipient);
      this.m_hashCode = new int?();
    }

    public void AddRecipient(Guid id)
    {
      this.ThrowIfStable();
      this.m_recipients.Add(new NotificationRecipient()
      {
        Id = id
      });
      this.m_hashCode = new int?();
    }

    public void AddRecipients(IEnumerable<Guid> ids)
    {
      this.ThrowIfStable();
      foreach (Guid id in ids)
        this.m_recipients.Add(new NotificationRecipient()
        {
          Id = id
        });
      this.m_hashCode = new int?();
    }

    public void AddRecipients(IEnumerable<Microsoft.VisualStudio.Services.Identity.Identity> identities)
    {
      this.ThrowIfStable();
      foreach (Microsoft.VisualStudio.Services.Identity.Identity identity in identities)
        this.m_recipients.Add(new NotificationRecipient()
        {
          Id = identity.Id,
          Identity = identity
        });
      this.m_hashCode = new int?();
    }

    public IEnumerable<NotificationRecipient> GetRecipients() => (IEnumerable<NotificationRecipient>) this.m_recipients;

    public override bool Equals(object obj)
    {
      bool flag = base.Equals(obj);
      MatchTarget matchTarget = obj as MatchTarget;
      if (!flag && matchTarget != null && this.m_event != null && matchTarget.m_event != null && this.m_event.Id == matchTarget.m_event.Id && string.Equals(this.m_matchChannel, matchTarget.m_matchChannel) && this.m_recipients.SetEquals((IEnumerable<NotificationRecipient>) matchTarget.m_recipients))
        flag = true;
      return flag;
    }

    public override int GetHashCode()
    {
      if (!this.m_hashCode.HasValue)
      {
        this.m_hashCode = new int?(0);
        int? hashCode1 = this.m_hashCode;
        int hashCode2 = this.m_event.SafeGetHashCode<TeamFoundationEvent>();
        this.m_hashCode = hashCode1.HasValue ? new int?(hashCode1.GetValueOrDefault() + hashCode2) : new int?();
        hashCode1 = this.m_hashCode;
        int hashCode3 = this.m_matchChannel.SafeGetHashCode<string>();
        this.m_hashCode = hashCode1.HasValue ? new int?(hashCode1.GetValueOrDefault() + hashCode3) : new int?();
        this.m_recipients.ForEach<NotificationRecipient>((Action<NotificationRecipient>) (r =>
        {
          int? hashCode4 = this.m_hashCode;
          int hashCode5 = r.GetHashCode();
          this.m_hashCode = hashCode4.HasValue ? new int?(hashCode4.GetValueOrDefault() + hashCode5) : new int?();
        }));
      }
      return this.m_hashCode.Value;
    }

    private void ThrowIfStable()
    {
      if (this.m_isStable)
        throw new ArgumentException("MatchTarget cannot be changed once stable");
    }
  }
}
