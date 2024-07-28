// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.TeamFoundationTraceConfiguration
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;

namespace Microsoft.TeamFoundation.Framework.Server
{
  internal class TeamFoundationTraceConfiguration
  {
    private CachedRegistryService m_registryService;
    private static readonly string s_isTracingConfigKey = "traceWriter";

    public void Initialize(IVssRequestContext systemRequestContext)
    {
      this.m_registryService = systemRequestContext.GetService<CachedRegistryService>();
      this.UpdateTraceSettings(systemRequestContext);
      systemRequestContext.GetService<CachedRegistryService>().RegisterNotification(systemRequestContext, new RegistrySettingsChangedCallback(this.OnSettingsChanged), "/Diagnostics/Trace/*");
      Trace.TraceInformation("Initialized the Trace Configurations");
    }

    private void UpdateTraceSettings(IVssRequestContext systemRequestContext)
    {
      bool flag1 = false;
      bool flag2 = this.m_registryService.GetValue<bool>(systemRequestContext, (RegistryQuery) "/Diagnostics/Trace/Enabled", false, false);
      string appSetting = ConfigurationManager.AppSettings[TeamFoundationTraceConfiguration.s_isTracingConfigKey];
      if (!string.IsNullOrEmpty(appSetting))
      {
        try
        {
          flag1 = bool.Parse(appSetting);
        }
        catch (FormatException ex)
        {
        }
      }
      if (flag2 | flag1)
      {
        TeamFoundationTraceListener.SetTraceWriterState(true);
        RegistryEntryCollection registryEntryCollection = this.m_registryService.ReadEntries(systemRequestContext, (RegistryQuery) "/Diagnostics/Trace/Names/*", true);
        SortedDictionary<string, TraceLevel> enabledKeywords = TeamFoundationTrace.TraceSettings.GetEnabledKeywords();
        foreach (RegistryEntry registryEntry in registryEntryCollection)
        {
          if (!string.IsNullOrEmpty(registryEntry.Value))
          {
            TraceLevel traceLevel1;
            try
            {
              traceLevel1 = (TraceLevel) Enum.Parse(typeof (TraceLevel), registryEntry.Value, true);
            }
            catch
            {
              continue;
            }
            if (Enum.IsDefined(typeof (TraceLevel), (object) traceLevel1))
            {
              TraceLevel traceLevel2 = TeamFoundationTrace.TraceSettings.OriginalTraceLevel(registryEntry.Name);
              if (traceLevel2 > traceLevel1)
                traceLevel1 = traceLevel2;
              TeamFoundationTrace.TraceSettings[registryEntry.Name] = traceLevel1;
              if (enabledKeywords.ContainsKey(registryEntry.Name))
                enabledKeywords.Remove(registryEntry.Name);
            }
          }
        }
        foreach (string key in enabledKeywords.Keys)
          TeamFoundationTrace.TraceSettings[key] = TeamFoundationTrace.TraceSettings.OriginalTraceLevel(key);
      }
      else
        TeamFoundationTraceListener.SetTraceWriterState(false);
      Trace.TraceInformation("Updated Trace Settings");
    }

    private void OnSettingsChanged(
      IVssRequestContext requestContext,
      RegistryEntryCollection changedEntries)
    {
      this.UpdateTraceSettings(requestContext);
    }
  }
}
