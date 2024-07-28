// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Notifications.Server.IgnoreSubscription
// Assembly: Microsoft.VisualStudio.Services.Notifications.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8ED1F52D-E567-4EFA-AAED-F2D9262BE9C2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Notifications.Server.dll

using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Notifications.Server
{
  public class IgnoreSubscription
  {
    private Guid id;
    private Guid[] m_Actors;
    private string m_ActorsValue;
    public static readonly string s_eventTypeFieldName = "eventtype";
    public static readonly string s_scopeIdentifierFieldName = "scopeidentifier";
    public static readonly string s_sourceIdentityFieldName = "sourceidentity";
    public static readonly string s_conditionFieldName = "condition";
    public static readonly string s_allowedChannels = "allowedchannels";

    public string EventType { get; set; }

    public string ScopeIdentifier { get; set; }

    public Guid[] Actors
    {
      get
      {
        if (this.m_Actors == null)
          this.m_Actors = new Guid[0];
        return this.m_Actors;
      }
      set
      {
        this.m_ActorsValue = (string) null;
        this.m_Actors = value;
      }
    }

    public HashSet<string> AllowedChannels { get; set; }

    public string ActorsValue
    {
      get
      {
        if (this.m_ActorsValue == null)
          this.m_ActorsValue = string.Join(",", ((IEnumerable<Guid>) this.Actors).Select<Guid, string>((Func<Guid, string>) (a => a.ToString())));
        return this.m_ActorsValue;
      }
    }

    public Condition Condition { get; set; }

    public string Matcher { get; set; }

    public IgnoreSubscription(Guid id)
    {
      this.id = id;
      this.EventType = string.Empty;
      this.ScopeIdentifier = string.Empty;
      this.Condition = (Condition) null;
      this.Actors = new Guid[0];
      this.Matcher = "XPathMatcher";
    }
  }
}
