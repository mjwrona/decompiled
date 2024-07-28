// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.UserActivity
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamFoundation.Framework.Server
{
  public class UserActivity
  {
    private HashSet<string> m_activeUsers;
    private HashSet<string> m_engagedUsers;
    private HashSet<string> m_dedicatedUsers;
    private List<UserActivityEntry> m_entries;
    private Dictionary<UserAgentDetails, Tuple<int, int, int>> m_userAgentStatistics;
    private static readonly int s_daysForActive = 1;
    private static readonly int s_daysForEngaged = 2;
    private static readonly int s_daysForDedicated = 12;

    public UserActivity(List<UserActivityEntry> entries)
    {
      this.m_entries = entries;
      this.CalculateUserActivityStatistics();
      this.CalculateUserAgentData();
    }

    public int ActiveUsers => this.m_activeUsers.Count + this.m_engagedUsers.Count + this.m_dedicatedUsers.Count;

    public int EngagedUsers => this.m_engagedUsers.Count + this.m_dedicatedUsers.Count;

    public int DedicatedUsers => this.m_dedicatedUsers.Count;

    public IEnumerable<UserAgentDetails> UserAgents => (IEnumerable<UserAgentDetails>) this.m_userAgentStatistics.Keys;

    public int GetUserAgentActiveUsers(UserAgentDetails userAgent)
    {
      Tuple<int, int, int> tuple;
      return this.m_userAgentStatistics.TryGetValue(userAgent, out tuple) ? tuple.Item1 : 0;
    }

    public int GetUserAgentEngagedUsers(UserAgentDetails userAgent)
    {
      Tuple<int, int, int> tuple;
      return this.m_userAgentStatistics.TryGetValue(userAgent, out tuple) ? tuple.Item2 : 0;
    }

    public int GetUserAgentDedicatedUsers(UserAgentDetails userAgent)
    {
      Tuple<int, int, int> tuple;
      return this.m_userAgentStatistics.TryGetValue(userAgent, out tuple) ? tuple.Item3 : 0;
    }

    private void CalculateUserActivityStatistics()
    {
      this.m_activeUsers = new HashSet<string>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
      this.m_engagedUsers = new HashSet<string>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
      this.m_dedicatedUsers = new HashSet<string>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
      foreach (IGrouping<string, UserActivityEntry> source in this.m_entries.GroupBy<UserActivityEntry, string>((Func<UserActivityEntry, string>) (ua => ua.IdentityName.ToLowerInvariant())))
      {
        string key = source.Key;
        int num = source.Select<UserActivityEntry, DateTime>((Func<UserActivityEntry, DateTime>) (e => e.ActivityDate)).Distinct<DateTime>().Count<DateTime>();
        if (num >= UserActivity.s_daysForDedicated)
          this.m_dedicatedUsers.Add(key);
        else if (num >= UserActivity.s_daysForEngaged)
          this.m_engagedUsers.Add(key);
        else if (num >= UserActivity.s_daysForActive)
          this.m_activeUsers.Add(key);
      }
    }

    private void CalculateUserAgentData()
    {
      Dictionary<UserAgentDetails, HashSet<string>> dictionary1 = new Dictionary<UserAgentDetails, HashSet<string>>();
      Dictionary<string, UserAgentDetails> dictionary2 = new Dictionary<string, UserAgentDetails>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
      foreach (UserActivityEntry entry in this.m_entries)
      {
        UserAgentDetails key;
        if (!dictionary2.TryGetValue(entry.UserAgent, out key))
        {
          key = new UserAgentDetails(entry.UserAgent);
          dictionary2.Add(entry.UserAgent, key);
        }
        HashSet<string> stringSet;
        if (!dictionary1.TryGetValue(key, out stringSet))
        {
          stringSet = new HashSet<string>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
          dictionary1.Add(key, stringSet);
        }
        stringSet.Add(entry.IdentityName);
      }
      this.m_userAgentStatistics = new Dictionary<UserAgentDetails, Tuple<int, int, int>>();
      foreach (KeyValuePair<UserAgentDetails, HashSet<string>> keyValuePair in dictionary1)
      {
        int num1 = 0;
        int num2 = 0;
        int num3 = 0;
        foreach (string str in keyValuePair.Value)
        {
          if (this.m_activeUsers.Contains(str))
            ++num1;
          else if (this.m_engagedUsers.Contains(str))
          {
            ++num1;
            ++num2;
          }
          else if (this.m_dedicatedUsers.Contains(str))
          {
            ++num1;
            ++num2;
            ++num3;
          }
        }
        this.m_userAgentStatistics.Add(keyValuePair.Key, Tuple.Create<int, int, int>(num1, num2, num3));
      }
    }
  }
}
