// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.Common.WorkItemTrackingReadReplicaConfiguration
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Web;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server.Common
{
  public class WorkItemTrackingReadReplicaConfiguration
  {
    public readonly IReadOnlyDictionary<string, string> ReadReplicaUsers;
    public readonly IReadOnlyDictionary<string, string> ReadReplicaUserAgents;
    public readonly IReadOnlyDictionary<string, string> ReadReplicaEnabledCommands;
    public readonly IReadOnlyDictionary<string, string> ForcedReadReplicaCommands;

    public WorkItemTrackingReadReplicaConfiguration(
      RegistryEntryCollection readReplicaUsers,
      RegistryEntryCollection readReplicaUserAgents,
      RegistryEntryCollection readReplicaEnabledCommands,
      RegistryEntryCollection forcedReadReplicaCommands)
    {
      Dictionary<string, string> dictionary1 = new Dictionary<string, string>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
      Dictionary<string, string> dictionary2 = new Dictionary<string, string>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
      Dictionary<string, string> dictionary3 = new Dictionary<string, string>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
      Dictionary<string, string> dictionary4 = new Dictionary<string, string>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
      foreach (RegistryEntry readReplicaUser in readReplicaUsers)
      {
        string key = readReplicaUser.Path.Substring("/Service/WorkItemTracking/Settings/ReadReplicaUsers/".Length);
        if (key != null)
          dictionary1[key] = readReplicaUser.GetValue<string>();
      }
      foreach (RegistryEntry replicaUserAgent in readReplicaUserAgents)
      {
        string str = replicaUserAgent.Path.Substring("/Service/WorkItemTracking/Settings/ReadReplicaUserAgents/".Length);
        if (str != null)
          dictionary2[HttpUtility.UrlDecode(str)] = replicaUserAgent.GetValue<string>();
      }
      foreach (RegistryEntry replicaEnabledCommand in readReplicaEnabledCommands)
      {
        string key = replicaEnabledCommand.Path.Substring("/Service/WorkItemTracking/Settings/ReadReplicaEnabledCommands/".Length);
        if (key != null)
          dictionary3[key] = replicaEnabledCommand.GetValue<string>();
      }
      foreach (RegistryEntry readReplicaCommand in forcedReadReplicaCommands)
      {
        string key = readReplicaCommand.Path.Substring("/Service/WorkItemTracking/Settings/ForcedReadReplicaCommands/".Length);
        if (key != null)
          dictionary4[key] = readReplicaCommand.GetValue<string>();
      }
      this.ReadReplicaUsers = (IReadOnlyDictionary<string, string>) new ReadOnlyDictionary<string, string>((IDictionary<string, string>) dictionary1);
      this.ReadReplicaUserAgents = (IReadOnlyDictionary<string, string>) new ReadOnlyDictionary<string, string>((IDictionary<string, string>) dictionary2);
      this.ReadReplicaEnabledCommands = (IReadOnlyDictionary<string, string>) new ReadOnlyDictionary<string, string>((IDictionary<string, string>) dictionary3);
      this.ForcedReadReplicaCommands = (IReadOnlyDictionary<string, string>) new ReadOnlyDictionary<string, string>((IDictionary<string, string>) dictionary4);
    }
  }
}
