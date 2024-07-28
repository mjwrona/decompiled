// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Users.Server.UserSyncHelper
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Identity;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Users.Server
{
  internal static class UserSyncHelper
  {
    private const string s_area = "VisualStudio.Services.Users.Server";
    private const string s_layer = "UserSync";

    internal static void SyncUserToIdentity(
      IVssRequestContext requestContext,
      Guid identityId,
      User user)
    {
      if (requestContext.ServiceHost.Is(TeamFoundationHostType.Deployment))
        return;
      IdentityService service = requestContext.GetService<IdentityService>();
      Microsoft.VisualStudio.Services.Identity.Identity identity = service.ReadIdentities(requestContext, (IList<SubjectDescriptor>) new SubjectDescriptor[1]
      {
        user.Descriptor
      }, QueryMembership.None, (IEnumerable<string>) new string[2]
      {
        "ConfirmedNotificationAddress",
        "CustomNotificationAddresses"
      }).Single<Microsoft.VisualStudio.Services.Identity.Identity>();
      if (identity == null)
        return;
      string displayName = identity.DisplayName;
      string property1 = identity.GetProperty<string>("ConfirmedNotificationAddress", (string) null);
      string property2 = identity.GetProperty<string>("CustomNotificationAddresses", (string) null);
      bool flag = false;
      if (!StringComparer.Ordinal.Equals(user.DisplayName, displayName))
      {
        identity.CustomDisplayName = user.DisplayName;
        flag = true;
      }
      if (!StringComparer.OrdinalIgnoreCase.Equals(user.Mail, property1))
      {
        identity.SetProperty("ConfirmedNotificationAddress", (object) user.Mail);
        flag = true;
      }
      if (!StringComparer.OrdinalIgnoreCase.Equals(user.Mail, property2))
      {
        identity.SetProperty("CustomNotificationAddresses", (object) user.Mail);
        flag = true;
      }
      if (!flag)
        return;
      requestContext.Trace(20458, TraceLevel.Info, "VisualStudio.Services.Users.Server", "UserSync", string.Format("Identity has updates. ServiceHost ID: {0}, ", (object) requestContext.ServiceHost.InstanceId) + string.Format("VSID: {0}, ", (object) identity.Id) + "Original customDisplayName: " + displayName + ", Original confirmedNotificationAddressIdentityProperty: " + property1 + ", Original customNotificationAddressIdentityProperty: " + property2 + "\nNew customDisplayName: " + identity.CustomDisplayName + ", " + string.Format("New confirmedNotificationAddressIdentityProperty: {0}, ", identity.Properties.GetValueOrDefault<string, object>("ConfirmedNotificationAddress")) + string.Format("New customNotificationAddressIdentityProperty: {0}", identity.Properties.GetValueOrDefault<string, object>("CustomNotificationAddresses")));
      service.UpdateIdentities(requestContext, (IList<Microsoft.VisualStudio.Services.Identity.Identity>) new Microsoft.VisualStudio.Services.Identity.Identity[1]
      {
        identity
      });
    }
  }
}
