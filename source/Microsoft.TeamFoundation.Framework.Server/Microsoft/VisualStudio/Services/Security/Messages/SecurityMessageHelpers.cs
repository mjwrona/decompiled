// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Security.Messages.SecurityMessageHelpers
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.VisualStudio.Services.Security.Server;
using System;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Security.Messages
{
  public static class SecurityMessageHelpers
  {
    public static void CollapseMessages(IList<SecurityMessage2> messages)
    {
      if (messages.Count <= 1)
        return;
      Dictionary<SecurityMessageHelpers.DataChangedKey, TokenStoreSequenceId> dictionary = new Dictionary<SecurityMessageHelpers.DataChangedKey, TokenStoreSequenceId>(SecurityMessageHelpers.DataChangedKey.Comparer);
      SecurityMessage2 message1 = messages[messages.Count - 1];
      if (message1.NamespaceId != Guid.Empty)
        dictionary[new SecurityMessageHelpers.DataChangedKey(message1)] = new TokenStoreSequenceId(message1.NewSequenceId);
      for (int index = messages.Count - 2; index >= 0; --index)
      {
        SecurityMessage2 message2 = messages[index];
        if (message2.NamespaceId == Guid.Empty)
        {
          if (message1.InstanceId == message2.InstanceId && message1.NamespaceId == Guid.Empty)
          {
            messages.RemoveAt(index);
            continue;
          }
          dictionary.Clear();
        }
        else
        {
          SecurityMessageHelpers.DataChangedKey key = new SecurityMessageHelpers.DataChangedKey(message2);
          TokenStoreSequenceId tokenStoreSequenceId = new TokenStoreSequenceId(message2.NewSequenceId);
          TokenStoreSequenceId newer;
          if (dictionary.TryGetValue(key, out newer) && tokenStoreSequenceId.IsSupersededBy(newer, true, false))
          {
            messages.RemoveAt(index);
            continue;
          }
          dictionary[key] = tokenStoreSequenceId;
        }
        message1 = message2;
      }
    }

    private struct DataChangedKey
    {
      public readonly Guid InstanceId;
      public readonly Guid NamespaceId;
      public readonly Guid AclStoreId;
      public static readonly IEqualityComparer<SecurityMessageHelpers.DataChangedKey> Comparer = (IEqualityComparer<SecurityMessageHelpers.DataChangedKey>) new SecurityMessageHelpers.DataChangedKey.DataChangedKeyComparer();

      public DataChangedKey(SecurityMessage2 message)
        : this(message.InstanceId, message.NamespaceId, message.AclStoreId)
      {
      }

      public DataChangedKey(Guid instanceId, Guid namespaceId, Guid aclStoreId)
      {
        this.InstanceId = instanceId;
        this.NamespaceId = namespaceId;
        this.AclStoreId = aclStoreId;
      }

      private class DataChangedKeyComparer : IEqualityComparer<SecurityMessageHelpers.DataChangedKey>
      {
        public bool Equals(
          SecurityMessageHelpers.DataChangedKey x,
          SecurityMessageHelpers.DataChangedKey y)
        {
          return x.InstanceId.Equals(y.InstanceId) && x.NamespaceId.Equals(y.NamespaceId) && x.AclStoreId.Equals(y.AclStoreId);
        }

        public int GetHashCode(SecurityMessageHelpers.DataChangedKey obj)
        {
          Guid guid = obj.InstanceId;
          int hashCode1 = guid.GetHashCode();
          guid = obj.NamespaceId;
          int hashCode2 = guid.GetHashCode();
          int num = hashCode1 ^ hashCode2;
          guid = obj.AclStoreId;
          int hashCode3 = guid.GetHashCode();
          return num ^ hashCode3;
        }
      }
    }
  }
}
