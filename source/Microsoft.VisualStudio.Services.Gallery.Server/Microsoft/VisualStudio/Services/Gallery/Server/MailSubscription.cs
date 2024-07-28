// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Gallery.Server.MailSubscription
// Assembly: Microsoft.VisualStudio.Services.Gallery.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B9EBBED5-135E-45CD-B0B4-F747360599CD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Gallery.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Identity;
using Microsoft.VisualStudio.Services.Settings;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace Microsoft.VisualStudio.Services.Gallery.Server
{
  internal class MailSubscription : IMailSubscription
  {
    private const string s_area = "gallery";
    private const string s_layer = "MailSubscription";

    public bool HasUserUnsubscribed(
      IVssRequestContext requestContext,
      Guid userId,
      string settingKey)
    {
      bool flag = false;
      try
      {
        requestContext.TraceEnter(12062022, "gallery", nameof (MailSubscription), nameof (HasUserUnsubscribed));
        IList<Microsoft.VisualStudio.Services.Identity.Identity> identityList = requestContext.GetService<IdentityService>().ReadIdentities(requestContext, (IList<Guid>) new List<Guid>()
        {
          userId
        }, QueryMembership.None, (IEnumerable<string>) null);
        if (identityList == null || identityList.Count == 0 || identityList[0] == null)
          throw new ArgumentException(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "User identity not found. Unable to create impersonation context for the user with id {0}.", (object) userId));
        using (IVssRequestContext userContext = requestContext.CreateUserContext(identityList[0].Descriptor))
        {
          IDictionary<string, object> values = userContext.GetService<ISettingsService>().GetValues(userContext, SettingsUserScope.User, (string) null, (string) null, (string) null);
          if (values != null)
          {
            if (values.ContainsKey(settingKey))
            {
              bool? nullable = values[settingKey] as bool?;
              if (nullable.HasValue)
                flag = nullable.Value;
            }
          }
        }
      }
      catch (Exception ex)
      {
        requestContext.TraceException(12062023, "gallery", nameof (MailSubscription), ex);
        throw;
      }
      finally
      {
        requestContext.TraceLeave(12062022, "gallery", nameof (MailSubscription), nameof (HasUserUnsubscribed));
      }
      return flag;
    }
  }
}
