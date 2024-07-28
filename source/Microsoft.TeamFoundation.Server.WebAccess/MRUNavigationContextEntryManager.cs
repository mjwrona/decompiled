// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.MRUNavigationContextEntryManager
// Assembly: Microsoft.TeamFoundation.Server.WebAccess, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A2CCA8C5-6910-48A5-82D9-BDC1350B5B4D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.dll

using Microsoft.Azure.Boards.Settings;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Settings;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamFoundation.Server.WebAccess
{
  public static class MRUNavigationContextEntryManager
  {
    private const string c_mruNavSettingsKey = "WebAccess/Navigation/MRU";
    private const int c_mruStorageCount = 8;

    public static MRUNavigationContextEntry[] ReadMRUNavigationContexts(
      IVssRequestContext requestContext)
    {
      MRUNavigationContextEntry[] navigationContextEntryArray = MRUNavigationContextEntryManager.ReadMRUNavigationContextData(requestContext);
      if (navigationContextEntryArray != null)
      {
        foreach (MRUNavigationContextEntry navigationContextEntry in navigationContextEntryArray)
          navigationContextEntry.FixServiceHost(requestContext);
      }
      return navigationContextEntryArray;
    }

    internal static MRUNavigationContextEntry[] ReadMRUNavigationContexts(
      TfsWebContext tfsWebContext)
    {
      return MRUNavigationContextEntryManager.ReadMRUNavigationContexts(tfsWebContext.TfsRequestContext);
    }

    private static MRUNavigationContextEntry[] ReadMRUNavigationContextData(
      IVssRequestContext tfsRequestContext,
      Guid? userTeamFoundationId = null)
    {
      tfsRequestContext.GetService<ISettingsService>();
      MRUNavigationContextEntry[] navigationContextEntryArray = TfsWebContextExtensions.GetAccountSetting<MRUNavigationContextEntry[]>(tfsRequestContext, !userTeamFoundationId.HasValue ? SettingsUserScope.User : SettingsUserScope.SpecificUser(userTeamFoundationId.Value), "WebAccess/Navigation/MRU");
      if (navigationContextEntryArray == null)
      {
        AccountUserSettingsHive userSettingsHive = !userTeamFoundationId.HasValue ? new AccountUserSettingsHive(tfsRequestContext) : new AccountUserSettingsHive(tfsRequestContext, userTeamFoundationId.Value);
        using (userSettingsHive)
          navigationContextEntryArray = userSettingsHive.ReadEnumerableSetting<MRUNavigationContextEntry>("/WebAccess/MRU/Navigation", Enumerable.Empty<MRUNavigationContextEntry>()).ToArray<MRUNavigationContextEntry>();
      }
      return navigationContextEntryArray;
    }

    internal static void WriteMRUNavigationContexts(
      IVssRequestContext tfsRequestContext,
      IEnumerable<MRUNavigationContextEntry> entries,
      Guid? userTeamFoundationId = null)
    {
      TfsWebContextExtensions.SetAccountSetting(tfsRequestContext, !userTeamFoundationId.HasValue ? SettingsUserScope.User : SettingsUserScope.SpecificUser(userTeamFoundationId.Value), "WebAccess/Navigation/MRU", (object) entries.ToArray<MRUNavigationContextEntry>());
    }

    internal static void RemoveMRUNavigationContextsForProject(
      IVssRequestContext tfsRequestContext,
      string projectName,
      string teamName = null)
    {
      string serviceHost = tfsRequestContext.ServiceHost.Name;
      MRUNavigationContextEntry[] source = MRUNavigationContextEntryManager.ReadMRUNavigationContexts(tfsRequestContext);
      IEnumerable<MRUNavigationContextEntry> navigationContextEntries = ((IEnumerable<MRUNavigationContextEntry>) source).Where<MRUNavigationContextEntry>((Func<MRUNavigationContextEntry, bool>) (x => (string.IsNullOrEmpty(teamName) ? 1 : (teamName.Equals(x.Team, StringComparison.OrdinalIgnoreCase) ? 1 : 0)) == 0 || string.IsNullOrEmpty(projectName) || !projectName.Equals(x.Project, StringComparison.OrdinalIgnoreCase) || string.IsNullOrEmpty(serviceHost) || !serviceHost.Equals(x.ServiceHost.Name, StringComparison.OrdinalIgnoreCase)));
      if (source.Length == navigationContextEntries.Count<MRUNavigationContextEntry>())
        return;
      MRUNavigationContextEntryManager.WriteMRUNavigationContexts(tfsRequestContext, navigationContextEntries);
    }

    public static void UpdateMRUNavigationContextAsync(TfsWebContext tfsWebContext)
    {
      if (tfsWebContext.NavigationContext.TopMostLevel < NavigationContextLevels.Project)
        return;
      Guid userId = tfsWebContext.TfsRequestContext.GetUserId(true);
      tfsWebContext.TfsRequestContext.To(TeamFoundationHostType.Deployment).GetService<TeamFoundationTaskService>().AddTask(tfsWebContext.TfsRequestContext, new TeamFoundationTask((TeamFoundationTaskCallback) ((request, newMruEntry) => MRUNavigationContextEntryManager.UpdateMRUNavigationContexts(request, MRUNavigationContextEntryManager.ReadMRUNavigationContextData(request, new Guid?(userId)), (MRUNavigationContextEntry) newMruEntry, new Guid?(userId))), (object) new MRUNavigationContextEntry(tfsWebContext), 0));
    }

    internal static MRUNavigationContextEntry[] UpdateMRUNavigationContexts(
      IVssRequestContext requestContext,
      MRUNavigationContextEntry[] existingEntries,
      MRUNavigationContextEntry currentEntry,
      Guid? userId = null)
    {
      requestContext.TraceEnter(0, "WebAccess", TfsTraceLayers.Content, "UpdateMRUNavigationContext");
      try
      {
        MRUNavigationContextEntry navigationContextEntry = ((IEnumerable<MRUNavigationContextEntry>) existingEntries).FirstOrDefault<MRUNavigationContextEntry>();
        MRUNavigationContextEntry[] entries = (MRUNavigationContextEntry[]) null;
        if (navigationContextEntry == null)
        {
          entries = new MRUNavigationContextEntry[1]
          {
            currentEntry
          };
        }
        else
        {
          if (navigationContextEntry.Equals(currentEntry) && navigationContextEntry.LastAccessedByUser.HasValue)
          {
            DateTime? lastAccessedByUser = currentEntry.LastAccessedByUser;
            if (lastAccessedByUser.HasValue)
            {
              lastAccessedByUser = navigationContextEntry.LastAccessedByUser;
              DateTime dateTime1 = lastAccessedByUser.Value;
              ref DateTime local = ref dateTime1;
              lastAccessedByUser = currentEntry.LastAccessedByUser;
              DateTime dateTime2 = lastAccessedByUser.Value;
              if (Math.Abs(local.Subtract(dateTime2).TotalMinutes) <= 1.0)
                goto label_7;
            }
          }
          entries = ((IEnumerable<MRUNavigationContextEntry>) new MRUNavigationContextEntry[1]
          {
            currentEntry
          }).Concat<MRUNavigationContextEntry>(((IEnumerable<MRUNavigationContextEntry>) existingEntries).Where<MRUNavigationContextEntry>((Func<MRUNavigationContextEntry, bool>) (entry => !entry.Equals(currentEntry)))).Take<MRUNavigationContextEntry>(8).ToArray<MRUNavigationContextEntry>();
        }
label_7:
        if (entries == null)
          return existingEntries;
        MRUNavigationContextEntryManager.WriteMRUNavigationContexts(requestContext, (IEnumerable<MRUNavigationContextEntry>) entries, userId);
        return entries;
      }
      finally
      {
        requestContext.TraceLeave(0, "WebAccess", TfsTraceLayers.Content, "UpdateMRUNavigationContext");
      }
    }
  }
}
