// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.JsonLight.ODataJsonLightBatchAtomicGroupCache
// Assembly: Microsoft.TeamFoundation.OData.Core, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6619C7F6-E44A-4143-AE77-6D570F968D9A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Core.dll

using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.OData.JsonLight
{
  internal sealed class ODataJsonLightBatchAtomicGroupCache
  {
    private readonly Dictionary<string, IList<string>> groupToMessageIds = new Dictionary<string, IList<string>>();
    private string precedingMessageGroupId;
    private bool isWithinAtomicGroup;

    internal bool IsWithinAtomicGroup
    {
      get => this.isWithinAtomicGroup;
      set => this.isWithinAtomicGroup = value;
    }

    internal bool IsChangesetEnd(string groupId)
    {
      if (!this.isWithinAtomicGroup || this.precedingMessageGroupId != null && this.precedingMessageGroupId.Equals(groupId))
        return false;
      this.isWithinAtomicGroup = false;
      this.precedingMessageGroupId = (string) null;
      return true;
    }

    internal bool AddMessageIdAndGroupId(string messageId, string groupId)
    {
      bool flag = false;
      if (groupId.Equals(this.precedingMessageGroupId, StringComparison.Ordinal))
      {
        this.groupToMessageIds[groupId].Add(messageId);
      }
      else
      {
        if (this.groupToMessageIds.ContainsKey(groupId))
          throw new ODataException(Strings.ODataBatchReader_MessageIdPositionedIncorrectly((object) messageId, (object) groupId));
        this.groupToMessageIds.Add(groupId, (IList<string>) new List<string>()
        {
          messageId
        });
        this.precedingMessageGroupId = groupId;
        this.isWithinAtomicGroup = true;
        flag = true;
      }
      return flag;
    }

    internal string GetGroupId(string targetMessageId)
    {
      foreach (KeyValuePair<string, IList<string>> groupToMessageId in this.groupToMessageIds)
      {
        IList<string> stringList = groupToMessageId.Value;
        if (stringList != null && stringList.Contains(targetMessageId))
          return groupToMessageId.Key;
      }
      return (string) null;
    }

    internal bool IsGroupId(string id) => this.groupToMessageIds.Keys.Contains<string>(id);

    internal IList<string> GetFlattenedMessageIds(IList<string> ids)
    {
      List<string> flattenedMessageIds = new List<string>();
      if (ids.Count == 0)
        return (IList<string>) flattenedMessageIds;
      foreach (string id in (IEnumerable<string>) ids)
      {
        IList<string> collection;
        if (this.groupToMessageIds.TryGetValue(id, out collection))
          flattenedMessageIds.AddRange((IEnumerable<string>) collection);
        else
          flattenedMessageIds.Add(id);
      }
      return (IList<string>) flattenedMessageIds;
    }
  }
}
