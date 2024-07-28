// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.DatabaseReadReplicaSettingsService
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Web;

namespace Microsoft.TeamFoundation.Framework.Server
{
  public class DatabaseReadReplicaSettingsService : 
    IVssDatabaseReadReplicaSettingsService,
    IVssFrameworkService
  {
    private const string ReadReplicaUsersBucket = "ReadReplicaUsers";
    private const string ReadReplicaUserAgentsBucket = "ReadReplicaUserAgents";
    private const string ReadReplicaEnabledCommandsBucket = "ReadReplicaEnabledCommands";
    private const string ForcedReadReplicaCommandsBucket = "ForcedReadReplicaCommands";
    private const string ReadReplicaFeatureFlagPattern = "{0}.Server.ReadFromReadReplica";
    private static readonly string RegistrySettingsPath = "/Configuration/Settings/ReadReplica/";
    private static readonly RegistryQuery RegistrySettingsQuery = (RegistryQuery) (DatabaseReadReplicaSettingsService.RegistrySettingsPath + "...");
    private IDictionary<string, DatabaseReadReplicaSettingsService.ReadReplicaSettings> settings;

    public void ServiceStart(IVssRequestContext requestContext)
    {
      requestContext.GetService<IVssRegistryService>().RegisterNotification(requestContext, new RegistrySettingsChangedCallback(this.OnReadReplicaSettingsChanged), in DatabaseReadReplicaSettingsService.RegistrySettingsQuery);
      Interlocked.CompareExchange<IDictionary<string, DatabaseReadReplicaSettingsService.ReadReplicaSettings>>(ref this.settings, this.CreateFromRegistry(requestContext), (IDictionary<string, DatabaseReadReplicaSettingsService.ReadReplicaSettings>) null);
    }

    public void ServiceEnd(IVssRequestContext requestContext) => requestContext.GetService<IVssRegistryService>().UnregisterNotification(requestContext, new RegistrySettingsChangedCallback(this.OnReadReplicaSettingsChanged));

    public bool IsReadReplicaEnabled(IVssRequestContext requestContext, string serviceName)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckStringForNullOrWhiteSpace(serviceName, nameof (serviceName));
      requestContext.TraceEnter(99251, nameof (DatabaseReadReplicaSettingsService), nameof (IsReadReplicaEnabled), nameof (IsReadReplicaEnabled));
      try
      {
        if (this.settings != null)
        {
          DatabaseReadReplicaSettingsService.ReadReplicaSettings readReplicaSettings;
          if (this.settings.TryGetValue(serviceName, out readReplicaSettings))
          {
            string featureName = string.Format("{0}.Server.ReadFromReadReplica", (object) serviceName);
            if (!requestContext.IsFeatureEnabled(featureName) || string.IsNullOrEmpty(requestContext.Method?.Name))
              return false;
            if (readReplicaSettings.ForcedReadReplicaCommands.ContainsKey(requestContext.Method.Name))
              return true;
            if (readReplicaSettings.ReadReplicaEnabledCommands.ContainsKey(requestContext.Method.Name))
              return this.IsReadReplicaUserOrUserAgent(requestContext, readReplicaSettings);
          }
        }
      }
      catch (Exception ex)
      {
        requestContext.TraceException(99253, nameof (DatabaseReadReplicaSettingsService), nameof (IsReadReplicaEnabled), ex);
      }
      finally
      {
        requestContext.TraceLeave(99252, nameof (DatabaseReadReplicaSettingsService), nameof (IsReadReplicaEnabled), nameof (IsReadReplicaEnabled));
      }
      return false;
    }

    private bool IsReadReplicaUserOrUserAgent(
      IVssRequestContext requestContext,
      DatabaseReadReplicaSettingsService.ReadReplicaSettings readReplicaSettings)
    {
      return readReplicaSettings.ReadReplicaUsers.ContainsKey(requestContext.GetUserId().ToString()) || !string.IsNullOrEmpty(requestContext.UserAgent) && (readReplicaSettings.ReadReplicaUserAgents.ContainsKey(requestContext.UserAgent) || readReplicaSettings.ReadReplicaUserAgents.Any<KeyValuePair<string, string>>((Func<KeyValuePair<string, string>, bool>) (ua => requestContext.UserAgent.IndexOf(ua.Key, StringComparison.OrdinalIgnoreCase) != -1)));
    }

    private void OnReadReplicaSettingsChanged(
      IVssRequestContext requestContext,
      RegistryEntryCollection changedEntries)
    {
      Volatile.Write<IDictionary<string, DatabaseReadReplicaSettingsService.ReadReplicaSettings>>(ref this.settings, this.CreateFromRegistry(requestContext));
    }

    private IDictionary<string, DatabaseReadReplicaSettingsService.ReadReplicaSettings> CreateFromRegistry(
      IVssRequestContext requestContext)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      requestContext.TraceEnter(99254, nameof (DatabaseReadReplicaSettingsService), nameof (CreateFromRegistry), nameof (CreateFromRegistry));
      try
      {
        return DatabaseReadReplicaSettingsService.CreateFromRegistryInternal(requestContext);
      }
      catch (Exception ex)
      {
        requestContext.TraceException(99256, nameof (DatabaseReadReplicaSettingsService), nameof (CreateFromRegistry), ex);
        return (IDictionary<string, DatabaseReadReplicaSettingsService.ReadReplicaSettings>) new Dictionary<string, DatabaseReadReplicaSettingsService.ReadReplicaSettings>();
      }
      finally
      {
        requestContext.TraceLeave(99255, nameof (DatabaseReadReplicaSettingsService), nameof (CreateFromRegistry), nameof (CreateFromRegistry));
      }
    }

    private static IDictionary<string, DatabaseReadReplicaSettingsService.ReadReplicaSettings> CreateFromRegistryInternal(
      IVssRequestContext requestContext)
    {
      IDictionary<string, DatabaseReadReplicaSettingsService.ReadReplicaSettings> registryInternal = (IDictionary<string, DatabaseReadReplicaSettingsService.ReadReplicaSettings>) new Dictionary<string, DatabaseReadReplicaSettingsService.ReadReplicaSettings>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
      foreach (RegistryEntry registryEntry in requestContext.GetService<IVssRegistryService>().ReadEntriesFallThru(requestContext, in DatabaseReadReplicaSettingsService.RegistrySettingsQuery))
      {
        string str1 = registryEntry.Path.Substring(DatabaseReadReplicaSettingsService.RegistrySettingsPath.Length);
        if (!string.IsNullOrWhiteSpace(str1))
        {
          string[] strArray = str1.Split(new char[1]{ '/' }, StringSplitOptions.RemoveEmptyEntries);
          if (strArray.Length == 3)
          {
            string key1 = strArray[0];
            string str2 = strArray[1];
            string str3 = strArray[2];
            DatabaseReadReplicaSettingsService.ReadReplicaSettings readReplicaSettings;
            if (!registryInternal.TryGetValue(key1, out readReplicaSettings))
              registryInternal[key1] = readReplicaSettings = new DatabaseReadReplicaSettingsService.ReadReplicaSettings();
            switch (str2)
            {
              case "ReadReplicaUsers":
                readReplicaSettings.ReadReplicaUsers.TryAdd<string, string>(str3, registryEntry.GetValue<string>());
                continue;
              case "ReadReplicaUserAgents":
                string key2 = HttpUtility.UrlDecode(str3);
                readReplicaSettings.ReadReplicaUserAgents.TryAdd<string, string>(key2, registryEntry.GetValue<string>());
                continue;
              case "ReadReplicaEnabledCommands":
                readReplicaSettings.ReadReplicaEnabledCommands.TryAdd<string, string>(str3, registryEntry.GetValue<string>());
                continue;
              case "ForcedReadReplicaCommands":
                readReplicaSettings.ForcedReadReplicaCommands.TryAdd<string, string>(str3, registryEntry.GetValue<string>());
                continue;
              default:
                continue;
            }
          }
        }
      }
      return registryInternal;
    }

    private class ReadReplicaSettings
    {
      public IDictionary<string, string> ReadReplicaUsers { get; } = (IDictionary<string, string>) new Dictionary<string, string>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);

      public IDictionary<string, string> ReadReplicaUserAgents { get; } = (IDictionary<string, string>) new Dictionary<string, string>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);

      public IDictionary<string, string> ReadReplicaEnabledCommands { get; } = (IDictionary<string, string>) new Dictionary<string, string>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);

      public IDictionary<string, string> ForcedReadReplicaCommands { get; } = (IDictionary<string, string>) new Dictionary<string, string>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
    }
  }
}
