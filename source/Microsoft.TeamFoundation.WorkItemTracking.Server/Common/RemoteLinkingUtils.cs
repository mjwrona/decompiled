// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.Common.RemoteLinkingUtils
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server.Common
{
  public class RemoteLinkingUtils
  {
    public static void WriteRemoteDeletedProjectsToRegistry(
      IVssRequestContext requestContext,
      Guid remoteHostId,
      IEnumerable<Guid> remoteProjectIds)
    {
      SqlRegistryService service = requestContext.GetService<SqlRegistryService>();
      IEnumerable<RegistryEntry> registryEntries1 = remoteProjectIds.Select<Guid, RegistryEntry>((Func<Guid, RegistryEntry>) (remoteProjectId => new RegistryEntry(string.Format("{0}/{1}/{2}", (object) RemoteLinkConstants.RegistryPathRemoteDeletedRoot, (object) remoteHostId, (object) remoteProjectId), string.Empty)));
      IVssRequestContext requestContext1 = requestContext;
      IEnumerable<RegistryEntry> registryEntries2 = registryEntries1;
      service.WriteEntries(requestContext1, registryEntries2);
    }

    public static void WriteLocalDeletedProjectsToRegistry(
      IVssRequestContext requestContext,
      IEnumerable<DeletedProjectWithRemoteLink> localProjectAndRemoteHostPairs)
    {
      SqlRegistryService service = requestContext.GetService<SqlRegistryService>();
      IEnumerable<RegistryEntry> registryEntries1 = localProjectAndRemoteHostPairs.Select<DeletedProjectWithRemoteLink, RegistryEntry>((Func<DeletedProjectWithRemoteLink, RegistryEntry>) (x => new RegistryEntry(string.Format("{0}/{1}/{2}", (object) RemoteLinkConstants.RegistryPathLocalDeletedRoot, (object) x.RemoteHostId, (object) x.LocalProjectId), string.Empty)));
      IVssRequestContext requestContext1 = requestContext;
      IEnumerable<RegistryEntry> registryEntries2 = registryEntries1;
      service.WriteEntries(requestContext1, registryEntries2);
    }

    public static IEnumerable<(Guid HostId, Guid ProjectId, RegistryEntry RegEntry)> ReadLocalDeletedProjectsFromRegistry(
      IVssRequestContext requestContext)
    {
      return RemoteLinkingUtils.ParseRegistryEntryToHostAndProjectId(requestContext.GetService<SqlRegistryService>().ReadEntries(requestContext, (RegistryQuery) RemoteLinkConstants.RegistryPathLocalDeletedAll));
    }

    public static IEnumerable<(Guid HostId, Guid ProjectId, RegistryEntry RegEntry)> ReadRemoteDeletedProjectsFromRegistry(
      IVssRequestContext requestContext)
    {
      return RemoteLinkingUtils.ParseRegistryEntryToHostAndProjectId(requestContext.GetService<SqlRegistryService>().ReadEntries(requestContext, (RegistryQuery) RemoteLinkConstants.RegistryPathRemoteDeletedAll));
    }

    public static void DeleteEntries(
      IVssRequestContext requestContext,
      IEnumerable<RegistryEntry> entries)
    {
      requestContext.GetService<SqlRegistryService>().DeleteEntries(requestContext, entries.Select<RegistryEntry, string>((Func<RegistryEntry, string>) (x => x.Path)));
    }

    private static IEnumerable<(Guid HostId, Guid ProjectId, RegistryEntry RegEntry)> ParseRegistryEntryToHostAndProjectId(
      RegistryEntryCollection entries)
    {
      foreach (RegistryEntry entry in entries)
      {
        string[] strArray = entry.Path.Split('/');
        if (strArray.Length > 2)
        {
          string input1 = strArray[strArray.Length - 2];
          string input2 = strArray[strArray.Length - 1];
          Guid guid;
          ref Guid local = ref guid;
          Guid result;
          if (Guid.TryParse(input1, out local) && Guid.TryParse(input2, out result))
            yield return (guid, result, entry);
        }
      }
    }
  }
}
