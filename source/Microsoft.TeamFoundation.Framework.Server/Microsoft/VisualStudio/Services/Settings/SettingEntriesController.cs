// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Settings.SettingEntriesController
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using System.Collections.Generic;
using System.Web.Http;

namespace Microsoft.VisualStudio.Services.Settings
{
  [VersionedApiControllerCustomName(Area = "Settings", ResourceName = "Entries")]
  public class SettingEntriesController : TfsApiController
  {
    private const int c_maxSetEntriesCount = 100;

    public override string TraceArea => "Settings";

    public override string ActivityLogArea => "Settings";

    [HttpGet]
    [ClientLocationId("CD006711-163D-4CD4-A597-B05BAD2556FF")]
    public IDictionary<string, object> GetEntries(string userScope, string key = null) => this.GetEntriesForScope(userScope, (string) null, (string) null, key);

    [HttpGet]
    [ClientLocationId("4CBAAFAF-E8AF-4570-98D1-79EE99C56327")]
    public IDictionary<string, object> GetEntriesForScope(
      string userScope,
      string scopeName,
      string scopeValue,
      string key = null)
    {
      ArgumentUtility.CheckStringForNullOrEmpty(userScope, nameof (userScope));
      SettingsUserScope userScope1 = SettingsUserScope.Parse(userScope);
      return this.TfsRequestContext.GetService<ISettingsService>().GetValues(this.TfsRequestContext, userScope1, scopeName, scopeValue, key);
    }

    [HttpDelete]
    [ClientLocationId("CD006711-163D-4CD4-A597-B05BAD2556FF")]
    public void RemoveEntries(string userScope, string key) => this.RemoveEntriesForScope(userScope, (string) null, (string) null, key);

    [HttpDelete]
    [ClientLocationId("4CBAAFAF-E8AF-4570-98D1-79EE99C56327")]
    public void RemoveEntriesForScope(
      string userScope,
      string scopeName,
      string scopeValue,
      string key)
    {
      ArgumentUtility.CheckStringForNullOrEmpty(userScope, nameof (userScope));
      ArgumentUtility.CheckStringForNullOrEmpty(key, nameof (key));
      SettingsUserScope userScope1 = SettingsUserScope.Parse(userScope);
      this.TfsRequestContext.GetService<ISettingsService>().RemoveValue(this.TfsRequestContext, userScope1, scopeName, scopeValue, key, true);
    }

    [HttpPatch]
    [ClientLocationId("CD006711-163D-4CD4-A597-B05BAD2556FF")]
    public void SetEntries(IDictionary<string, object> entries, string userScope) => this.SetEntriesForScope(entries, userScope, (string) null, (string) null);

    [HttpPatch]
    [ClientLocationId("4CBAAFAF-E8AF-4570-98D1-79EE99C56327")]
    public void SetEntriesForScope(
      IDictionary<string, object> entries,
      string userScope,
      string scopeName,
      string scopeValue)
    {
      ArgumentUtility.CheckForNull<IDictionary<string, object>>(entries, nameof (entries));
      ArgumentUtility.CheckStringForNullOrEmpty(userScope, nameof (userScope));
      ArgumentUtility.CheckForOutOfRange(entries.Keys.Count, "entries.Count", 1, 100);
      SettingsUserScope userScope1 = SettingsUserScope.Parse(userScope);
      ISettingsService service = this.TfsRequestContext.GetService<ISettingsService>();
      foreach (KeyValuePair<string, object> entry in (IEnumerable<KeyValuePair<string, object>>) entries)
        service.SetValue(this.TfsRequestContext, userScope1, scopeName, scopeValue, entry.Key, entry.Value);
    }
  }
}
