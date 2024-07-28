// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.NotificationHubs.PushOutcomeCollection
// Assembly: Microsoft.Azure.NotificationHubs, Version=2.16.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1F43328A-44A2-48DE-9CBC-06F3C4A41C2A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.NotificationHubs.dll

using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.Azure.NotificationHubs
{
  internal class PushOutcomeCollection
  {
    public PushOutcomeCollection() => this.Outcomes = new Dictionary<string, Dictionary<string, long>>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);

    public Dictionary<string, Dictionary<string, long>> Outcomes { get; internal set; }

    internal bool IsEmpty() => this.Outcomes.Count <= 0;

    internal void Add(string platform, IEnumerable<string> pushOutcomes)
    {
      Dictionary<string, long> dictionary = pushOutcomes.GroupBy<string, string>((Func<string, string>) (pushOutcome => pushOutcome)).Select<IGrouping<string, string>, IGrouping<string, string>>((Func<IGrouping<string, string>, IGrouping<string, string>>) (pushOutcomeGroup => pushOutcomeGroup)).ToDictionary<IGrouping<string, string>, string, long>((Func<IGrouping<string, string>, string>) (s => s.Key), (Func<IGrouping<string, string>, long>) (s => s.LongCount<string>()));
      this.Add(platform, dictionary);
    }

    internal void Add(string platform, Dictionary<string, long> outcomeStat)
    {
      if (this.Outcomes.ContainsKey(platform))
      {
        Dictionary<string, long> outcome = this.Outcomes[platform];
        foreach (KeyValuePair<string, long> keyValuePair in outcomeStat)
          outcome[keyValuePair.Key] = !outcome.ContainsKey(keyValuePair.Key) ? keyValuePair.Value : outcome[keyValuePair.Key] + keyValuePair.Value;
      }
      else
        this.Outcomes[platform] = outcomeStat;
    }

    internal static PushOutcomeCollection Rollup(
      IEnumerable<PushOutcomeCollection> outcomeCollection)
    {
      PushOutcomeCollection outcomeCollection1 = new PushOutcomeCollection();
      Dictionary<string, long> dictionary1 = new Dictionary<string, long>();
      foreach (PushOutcomeCollection outcome1 in outcomeCollection)
      {
        foreach (KeyValuePair<string, Dictionary<string, long>> outcome2 in outcome1.Outcomes)
        {
          if (!outcome2.Key.Equals("AllPNS", StringComparison.OrdinalIgnoreCase))
          {
            Dictionary<string, long> dictionary2;
            if (outcomeCollection1.Outcomes.Keys.Contains<string>(outcome2.Key))
            {
              dictionary2 = outcomeCollection1.Outcomes[outcome2.Key];
            }
            else
            {
              dictionary2 = new Dictionary<string, long>();
              outcomeCollection1.Outcomes.Add(outcome2.Key, dictionary2);
            }
            foreach (KeyValuePair<string, long> keyValuePair in outcome2.Value)
            {
              dictionary2[keyValuePair.Key] = !dictionary2.Keys.Contains<string>(keyValuePair.Key) ? keyValuePair.Value : dictionary2[keyValuePair.Key] + keyValuePair.Value;
              dictionary1[keyValuePair.Key] = !dictionary1.Keys.Contains<string>(keyValuePair.Key) ? keyValuePair.Value : dictionary1[keyValuePair.Key] + keyValuePair.Value;
            }
          }
        }
      }
      outcomeCollection1.Outcomes["AllPNS"] = dictionary1;
      return outcomeCollection1;
    }

    internal static PushOutcomeCollection Aggregate(
      IEnumerable<PushOutcomeCollection> outcomeCollection)
    {
      PushOutcomeCollection outcomeCollection1 = new PushOutcomeCollection();
      foreach (PushOutcomeCollection outcome1 in outcomeCollection)
      {
        foreach (KeyValuePair<string, Dictionary<string, long>> outcome2 in outcome1.Outcomes)
        {
          if (!outcome2.Key.Equals("AllPNS", StringComparison.OrdinalIgnoreCase))
          {
            Dictionary<string, long> dictionary;
            if (outcomeCollection1.Outcomes.Keys.Contains<string>(outcome2.Key))
            {
              dictionary = outcomeCollection1.Outcomes[outcome2.Key];
            }
            else
            {
              dictionary = new Dictionary<string, long>();
              outcomeCollection1.Outcomes.Add(outcome2.Key, dictionary);
            }
            foreach (KeyValuePair<string, long> keyValuePair in outcome2.Value)
              dictionary[keyValuePair.Key] = !dictionary.Keys.Contains<string>(keyValuePair.Key) ? keyValuePair.Value : dictionary[keyValuePair.Key] + keyValuePair.Value;
          }
        }
      }
      return outcomeCollection1;
    }
  }
}
