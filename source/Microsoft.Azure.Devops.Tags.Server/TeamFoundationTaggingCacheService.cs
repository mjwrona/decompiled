// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Devops.Tags.Server.TeamFoundationTaggingCacheService
// Assembly: Microsoft.Azure.Devops.Tags.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 10FDC8E3-D1DB-4668-B2F2-04DAA10A7143
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Devops.Tags.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.Azure.Devops.Tags.Server
{
  internal class TeamFoundationTaggingCacheService : VssMemoryCacheService<Guid, TagDefinition>
  {
    private IVssMemoryCacheGrouping<Guid, TagDefinition, Guid> m_scopeGrouping;
    private IVssMemoryCacheGrouping<Guid, TagDefinition, TeamFoundationTaggingCacheService.TagNamedKey> m_nameGrouping;
    private IVssMemoryCacheGrouping<Guid, TagDefinition, int> m_allGrouping;
    private readonly int AllGroupKey;

    protected override void ServiceStart(IVssRequestContext requestContext)
    {
      base.ServiceStart(requestContext);
      this.m_scopeGrouping = VssMemoryCacheGroupingFactory.Create<Guid, TagDefinition, Guid>(requestContext, this.MemoryCache, (Func<Guid, TagDefinition, IEnumerable<Guid>>) ((k, v) => (IEnumerable<Guid>) new Guid[1]
      {
        v.Scope
      }));
      this.m_nameGrouping = VssMemoryCacheGroupingFactory.Create<Guid, TagDefinition, TeamFoundationTaggingCacheService.TagNamedKey>(requestContext, this.MemoryCache, (Func<Guid, TagDefinition, IEnumerable<TeamFoundationTaggingCacheService.TagNamedKey>>) ((k, v) => (IEnumerable<TeamFoundationTaggingCacheService.TagNamedKey>) new TeamFoundationTaggingCacheService.TagNamedKey[1]
      {
        new TeamFoundationTaggingCacheService.TagNamedKey(v.Scope, v.Name)
      }), (IEqualityComparer<TeamFoundationTaggingCacheService.TagNamedKey>) new TeamFoundationTaggingCacheService.TagNamedKey.Comparer());
      this.m_allGrouping = VssMemoryCacheGroupingFactory.Create<Guid, TagDefinition, int>(requestContext, this.MemoryCache, (Func<Guid, TagDefinition, IEnumerable<int>>) ((k, v) => (IEnumerable<int>) new int[1]
      {
        this.AllGroupKey
      }));
    }

    public bool TryGetValue(
      IVssRequestContext requestContext,
      Guid scope,
      string name,
      out TagDefinition tagDefinition)
    {
      IEnumerable<Guid> keys;
      Guid key;
      if (this.m_nameGrouping.TryGetKeys(new TeamFoundationTaggingCacheService.TagNamedKey(scope, name), out keys) && (key = keys.FirstOrDefault<Guid>()) != Guid.Empty)
        return this.TryGetValue(requestContext, key, out tagDefinition);
      tagDefinition = (TagDefinition) null;
      return false;
    }

    public IEnumerable<TagDefinition> GetAllValues(IVssRequestContext requestContext)
    {
      List<TagDefinition> allValues = new List<TagDefinition>();
      IEnumerable<Guid> keys;
      if (this.m_allGrouping.TryGetKeys(this.AllGroupKey, out keys))
      {
        foreach (Guid key in keys)
        {
          TagDefinition tagDefinition;
          if (this.TryGetValue(requestContext, key, out tagDefinition))
            allValues.Add(tagDefinition);
        }
      }
      return (IEnumerable<TagDefinition>) allValues;
    }

    public int GetAllTagCount(IVssRequestContext requestContext)
    {
      IEnumerable<Guid> keys;
      return this.m_allGrouping.TryGetKeys(this.AllGroupKey, out keys) ? keys.Count<Guid>() : 0;
    }

    public IEnumerable<TagDefinition> GetValues(IVssRequestContext requestContext, Guid scope)
    {
      List<TagDefinition> values = new List<TagDefinition>();
      IEnumerable<Guid> keys;
      if (this.m_scopeGrouping.TryGetKeys(scope, out keys))
      {
        foreach (Guid key in keys)
        {
          TagDefinition tagDefinition;
          if (this.TryGetValue(requestContext, key, out tagDefinition))
            values.Add(tagDefinition);
        }
      }
      return (IEnumerable<TagDefinition>) values;
    }

    private struct TagNamedKey
    {
      public Guid Scope { get; private set; }

      public string Name { get; private set; }

      public TagNamedKey(Guid scope, string name)
        : this()
      {
        this.Scope = scope;
        this.Name = name;
      }

      public class Comparer : EqualityComparer<TeamFoundationTaggingCacheService.TagNamedKey>
      {
        public override bool Equals(
          TeamFoundationTaggingCacheService.TagNamedKey x,
          TeamFoundationTaggingCacheService.TagNamedKey y)
        {
          return x.Scope == y.Scope && VssStringComparer.TagName.Equals(x.Name, y.Name);
        }

        public override int GetHashCode(TeamFoundationTaggingCacheService.TagNamedKey obj) => obj.Scope.GetHashCode() ^ VssStringComparer.TagName.GetHashCode(obj.Name);
      }
    }
  }
}
