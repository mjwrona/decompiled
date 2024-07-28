// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Gallery.Server.GalleryUserSettingsService
// Assembly: Microsoft.VisualStudio.Services.Gallery.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B9EBBED5-135E-45CD-B0B4-F747360599CD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Gallery.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Gallery.WebApi;
using Microsoft.VisualStudio.Services.Settings;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Gallery.Server
{
  public class GalleryUserSettingsService : IGalleryUserSettingsService, IVssFrameworkService
  {
    public void ServiceStart(IVssRequestContext systemRequestContext)
    {
    }

    public void ServiceEnd(IVssRequestContext systemRequestContext)
    {
    }

    public void SetGalleryUserSettings(
      IVssRequestContext requestContext,
      IDictionary<string, object> entries,
      string userScope)
    {
      ArgumentUtility.CheckForNull<IDictionary<string, object>>(entries, nameof (entries));
      ArgumentUtility.CheckStringForNullOrEmpty(userScope, nameof (userScope));
      SettingsUserScope userScope1 = string.Equals("me", userScope, StringComparison.OrdinalIgnoreCase) ? SettingsUserScope.User : SettingsUserScope.AllUsers;
      ISettingsService service = requestContext.GetService<ISettingsService>();
      foreach (KeyValuePair<string, object> entry in (IEnumerable<KeyValuePair<string, object>>) entries)
      {
        this.ValidateSettingKey(entry.Key);
        service.SetValue(requestContext, userScope1, (string) null, (string) null, entry.Key, entry.Value);
      }
    }

    public IDictionary<string, object> GetGalleryUserSettings(
      IVssRequestContext requestContext,
      string userScope,
      string key)
    {
      ArgumentUtility.CheckStringForNullOrEmpty(userScope, nameof (userScope));
      if (key != null)
        this.ValidateSettingKey(key);
      SettingsUserScope userScope1 = string.Equals("me", userScope, StringComparison.OrdinalIgnoreCase) ? SettingsUserScope.User : SettingsUserScope.AllUsers;
      IDictionary<string, object> values = requestContext.GetService<ISettingsService>().GetValues(requestContext, userScope1, (string) null, (string) null, (string) null);
      if (key == null || !values.ContainsKey(key))
        return values;
      return (IDictionary<string, object>) new Dictionary<string, object>()
      {
        {
          key,
          values[key]
        }
      };
    }

    private void ValidateSettingKey(string key)
    {
      ArgumentUtility.CheckStringForNullOrEmpty(key, nameof (key));
      if (!GalleryServiceConstants.SupportedSettings.Contains<string>(key, (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase))
        throw new InvalidSettingKeyException(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Setting key:{0} is not valid.", (object) key));
    }
  }
}
