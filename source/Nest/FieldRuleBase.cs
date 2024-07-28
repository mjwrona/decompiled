// Decompiled with JetBrains decompiler
// Type: Nest.FieldRuleBase
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net.Utf8Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace Nest
{
  [JsonFormatter(typeof (FieldRuleBaseFormatter))]
  public abstract class FieldRuleBase : IsADictionaryBase<string, object>
  {
    [IgnoreDataMember]
    protected string DistinguishedName
    {
      get
      {
        object obj;
        return !this.BackingDictionary.TryGetValue("dn", out obj) ? (string) null : (string) obj;
      }
      set => this.BackingDictionary.Add("dn", (object) value);
    }

    [IgnoreDataMember]
    protected IEnumerable<string> DistinguishedNames
    {
      get
      {
        object obj;
        return !this.BackingDictionary.TryGetValue("dn", out obj) ? (IEnumerable<string>) null : (IEnumerable<string>) obj;
      }
      set => this.BackingDictionary.Add("dn", (object) value);
    }

    [IgnoreDataMember]
    protected IEnumerable<string> Groups
    {
      get
      {
        object obj;
        return !this.BackingDictionary.TryGetValue("groups", out obj) ? (IEnumerable<string>) null : (IEnumerable<string>) obj;
      }
      set => this.BackingDictionary.Add("groups", (object) value);
    }

    [IgnoreDataMember]
    protected Tuple<string, object> Metadata
    {
      get
      {
        string key = this.BackingDictionary.Keys.FirstOrDefault<string>((Func<string, bool>) (k => k.StartsWith("metadata.")));
        return !string.IsNullOrEmpty(key) ? Tuple.Create<string, object>(key, this.BackingDictionary[key]) : (Tuple<string, object>) null;
      }
      set => this.BackingDictionary.Add("metadata." + value.Item1, value.Item2);
    }

    [IgnoreDataMember]
    protected string Realm
    {
      get
      {
        object obj;
        return !this.BackingDictionary.TryGetValue("realm.name", out obj) ? (string) null : (string) obj;
      }
      set => this.BackingDictionary.Add("realm.name", (object) value);
    }

    [IgnoreDataMember]
    protected string Username
    {
      get
      {
        object obj;
        return !this.BackingDictionary.TryGetValue("username", out obj) ? (string) null : (string) obj;
      }
      set => this.BackingDictionary.Add("username", (object) value);
    }

    [IgnoreDataMember]
    protected IEnumerable<string> Usernames
    {
      get
      {
        object obj;
        return !this.BackingDictionary.TryGetValue("username", out obj) ? (IEnumerable<string>) null : (IEnumerable<string>) obj;
      }
      set => this.BackingDictionary.Add("username", (object) value);
    }

    public static AnyRoleMappingRule operator |(
      FieldRuleBase leftContainer,
      FieldRuleBase rightContainer)
    {
      return new AnyRoleMappingRule(new RoleMappingRuleBase[2]
      {
        (RoleMappingRuleBase) new FieldRoleMappingRule(leftContainer),
        (RoleMappingRuleBase) new FieldRoleMappingRule(rightContainer)
      });
    }

    public static AllRoleMappingRule operator &(
      FieldRuleBase leftContainer,
      FieldRuleBase rightContainer)
    {
      return new AllRoleMappingRule(new RoleMappingRuleBase[2]
      {
        (RoleMappingRuleBase) new FieldRoleMappingRule(leftContainer),
        (RoleMappingRuleBase) new FieldRoleMappingRule(rightContainer)
      });
    }

    public static AllRoleMappingRule operator +(
      FieldRuleBase leftContainer,
      FieldRuleBase rightContainer)
    {
      return new AllRoleMappingRule(new RoleMappingRuleBase[2]
      {
        (RoleMappingRuleBase) new FieldRoleMappingRule(leftContainer),
        (RoleMappingRuleBase) new FieldRoleMappingRule(rightContainer)
      });
    }

    public static ExceptRoleMappingRole operator !(FieldRuleBase leftContainer) => new ExceptRoleMappingRole((RoleMappingRuleBase) new FieldRoleMappingRule(leftContainer));

    public static bool operator false(FieldRuleBase a) => false;

    public static bool operator true(FieldRuleBase a) => false;
  }
}
