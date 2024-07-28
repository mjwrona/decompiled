// Decompiled with JetBrains decompiler
// Type: Microsoft.AspNet.SignalR.Infrastructure.PrefixHelper
// Assembly: Microsoft.AspNet.SignalR.Core, Version=2.4.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 095D5FBC-6474-494D-BE26-4EBE2B9AD3D0
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.AspNet.SignalR.Core.dll

using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.AspNet.SignalR.Infrastructure
{
  internal static class PrefixHelper
  {
    internal const string HubPrefix = "h-";
    internal const string HubGroupPrefix = "hg-";
    internal const string HubConnectionIdPrefix = "hc-";
    internal const string HubUserPrefix = "hu-";
    internal const string PersistentConnectionPrefix = "pc-";
    internal const string PersistentConnectionGroupPrefix = "pcg-";
    internal const string ConnectionIdPrefix = "c-";

    public static bool HasGroupPrefix(string value) => value.StartsWith("hg-", StringComparison.Ordinal) || value.StartsWith("pcg-", StringComparison.Ordinal);

    public static string GetConnectionId(string connectionId) => "c-" + connectionId;

    public static string GetHubConnectionId(string connectionId) => "hc-" + connectionId;

    public static string GetHubName(string connectionId) => "h-" + connectionId;

    public static string GetHubGroupName(string groupName) => "hg-" + groupName;

    public static string GetHubUserId(string userId) => "hu-" + userId;

    public static string GetPersistentConnectionGroupName(string groupName) => "pcg-" + groupName;

    public static string GetPersistentConnectionName(string connectionName) => "pc-" + connectionName;

    public static IList<string> GetPrefixedConnectionIds(IList<string> connectionIds) => connectionIds.Count == 0 ? ListHelper<string>.Empty : (IList<string>) connectionIds.Select<string, string>(new Func<string, string>(PrefixHelper.GetConnectionId)).ToList<string>();

    public static IEnumerable<string> RemoveGroupPrefixes(IEnumerable<string> groups) => groups.Select<string, string>(new Func<string, string>(PrefixHelper.RemoveGroupPrefix));

    public static string RemoveGroupPrefix(string name)
    {
      if (name.StartsWith("hg-", StringComparison.Ordinal))
        return name.Substring("hg-".Length);
      return name.StartsWith("pcg-", StringComparison.Ordinal) ? name.Substring("pcg-".Length) : name;
    }
  }
}
