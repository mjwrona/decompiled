// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.NotificationHubs.Messaging.AuthorizationRules
// Assembly: Microsoft.Azure.NotificationHubs, Version=2.16.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1F43328A-44A2-48DE-9CBC-06F3C4A41C2A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.NotificationHubs.dll

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace Microsoft.Azure.NotificationHubs.Messaging
{
  [CollectionDataContract(Name = "AuthorizationRules", ItemName = "AuthorizationRule", Namespace = "http://schemas.microsoft.com/netservices/2010/10/servicebus/connect")]
  public class AuthorizationRules : 
    ICollection<AuthorizationRule>,
    IEnumerable<AuthorizationRule>,
    IEnumerable
  {
    public static readonly DataContractSerializer Serializer = new DataContractSerializer(typeof (AuthorizationRules));
    public readonly ICollection<AuthorizationRule> innerCollection;
    private readonly IDictionary<string, SharedAccessAuthorizationRule> nameToSharedAccessAuthorizationRuleMap;
    private bool duplicateAddForSharedAccessAuthorizationRule;

    public AuthorizationRules()
    {
      this.nameToSharedAccessAuthorizationRuleMap = (IDictionary<string, SharedAccessAuthorizationRule>) new Dictionary<string, SharedAccessAuthorizationRule>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
      this.innerCollection = (ICollection<AuthorizationRule>) new List<AuthorizationRule>();
    }

    public AuthorizationRules(IEnumerable<AuthorizationRule> enumerable)
    {
      if (enumerable == null)
        throw new ArgumentNullException(nameof (enumerable));
      this.nameToSharedAccessAuthorizationRuleMap = (IDictionary<string, SharedAccessAuthorizationRule>) new Dictionary<string, SharedAccessAuthorizationRule>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
      this.innerCollection = (ICollection<AuthorizationRule>) new List<AuthorizationRule>();
      foreach (AuthorizationRule authorizationRule in enumerable)
        this.Add(authorizationRule);
    }

    public IEnumerator<AuthorizationRule> GetEnumerator() => this.innerCollection.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => this.innerCollection.GetEnumerator();

    public void Add(AuthorizationRule item)
    {
      if (item is SharedAccessAuthorizationRule)
      {
        SharedAccessAuthorizationRule authorizationRule1 = item as SharedAccessAuthorizationRule;
        SharedAccessAuthorizationRule authorizationRule2;
        if (this.nameToSharedAccessAuthorizationRuleMap.TryGetValue(authorizationRule1.KeyName, out authorizationRule2))
        {
          this.nameToSharedAccessAuthorizationRuleMap.Remove(authorizationRule1.KeyName);
          this.innerCollection.Remove((AuthorizationRule) authorizationRule2);
          this.duplicateAddForSharedAccessAuthorizationRule = true;
        }
        this.nameToSharedAccessAuthorizationRuleMap.Add(authorizationRule1.KeyName, authorizationRule1);
      }
      this.innerCollection.Add(item);
    }

    public void Clear()
    {
      this.nameToSharedAccessAuthorizationRuleMap.Clear();
      this.innerCollection.Clear();
    }

    public bool Contains(AuthorizationRule item) => this.innerCollection.Contains(item);

    public void CopyTo(AuthorizationRule[] array, int arrayIndex) => this.innerCollection.CopyTo(array, arrayIndex);

    public bool Remove(AuthorizationRule item) => this.innerCollection.Remove(item);

    public int Count => this.innerCollection.Count;

    public List<AuthorizationRule> GetRules(Predicate<AuthorizationRule> match) => ((List<AuthorizationRule>) this.innerCollection).FindAll(match);

    public bool TryGetSharedAccessAuthorizationRule(
      string keyName,
      out SharedAccessAuthorizationRule rule)
    {
      return this.nameToSharedAccessAuthorizationRuleMap.TryGetValue(keyName, out rule);
    }

    public List<AuthorizationRule> GetRules(string claimValue) => ((List<AuthorizationRule>) this.innerCollection).FindAll((Predicate<AuthorizationRule>) (rule => string.Equals(claimValue, rule.ClaimValue, StringComparison.OrdinalIgnoreCase)));

    public bool IsReadOnly => this.innerCollection.IsReadOnly;

    public bool HasEqualRuntimeBehavior(AuthorizationRules comparand)
    {
      if (comparand == null)
        return false;
      AuthorizationRuleEqualityComparer comparer = new AuthorizationRuleEqualityComparer();
      HashSet<AuthorizationRule> authorizationRuleSet1 = new HashSet<AuthorizationRule>((IEnumerable<AuthorizationRule>) this.innerCollection, (IEqualityComparer<AuthorizationRule>) comparer);
      HashSet<AuthorizationRule> authorizationRuleSet2 = new HashSet<AuthorizationRule>((IEnumerable<AuthorizationRule>) comparand.innerCollection, (IEqualityComparer<AuthorizationRule>) comparer);
      if (authorizationRuleSet1.Count != authorizationRuleSet2.Count)
        return false;
      foreach (AuthorizationRule authorizationRule in authorizationRuleSet1)
      {
        if (!authorizationRuleSet2.Contains(authorizationRule))
          return false;
      }
      return true;
    }

    public bool RequiresEncryption => this.nameToSharedAccessAuthorizationRuleMap.Any<KeyValuePair<string, SharedAccessAuthorizationRule>>();

    internal void Validate()
    {
      foreach (AuthorizationRule inner in (IEnumerable<AuthorizationRule>) this.innerCollection)
        inner.Validate();
      if (this.duplicateAddForSharedAccessAuthorizationRule)
        throw new InvalidDataContractException(SRClient.CannotHaveDuplicateSAARule);
    }

    internal void UpdateForVersion(
      ApiVersion version,
      AuthorizationRules existingAuthorizationRules = null)
    {
      if (version >= ApiVersion.Three)
        return;
      foreach (AuthorizationRule authorizationRule in (IEnumerable<SharedAccessAuthorizationRule>) this.nameToSharedAccessAuthorizationRuleMap.Values)
        this.innerCollection.Remove(authorizationRule);
      this.nameToSharedAccessAuthorizationRuleMap.Clear();
      if (existingAuthorizationRules == null)
        return;
      foreach (AuthorizationRule authorizationRule in (IEnumerable<SharedAccessAuthorizationRule>) existingAuthorizationRules.nameToSharedAccessAuthorizationRuleMap.Values)
        this.Add(authorizationRule);
    }

    internal bool IsValidForVersion(ApiVersion version) => version >= ApiVersion.Three || !this.nameToSharedAccessAuthorizationRuleMap.Any<KeyValuePair<string, SharedAccessAuthorizationRule>>();
  }
}
