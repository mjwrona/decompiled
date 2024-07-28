// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Licensing.UserClaims
// Assembly: Microsoft.VisualStudio.Services.Licensing, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1B3CC7E6-129F-4267-B8E5-2210320176C7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Licensing.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Graph;
using Microsoft.VisualStudio.Services.Identity;
using System;
using System.Diagnostics;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Licensing
{
  internal class UserClaims
  {
    public const string MsaType = "MSID";
    public const string OrgIdType = "ORGID";
    private const string s_area = "VisualStudio.Services.LicensingService.UserClaims";
    private const string s_layer = "BusinessLogic";

    public string Type { get; set; }

    public string Identifier { get; set; }

    public string Domain { get; set; }

    public string LoginUpn { get; set; }

    public UserClaims(string type, string identifier, string domain, string loginUpn)
    {
      this.Type = type;
      this.Identifier = identifier;
      this.Domain = domain;
      this.LoginUpn = loginUpn;
    }

    internal static UserClaims GetUserClaims(
      IVssRequestContext requestContext,
      Microsoft.VisualStudio.Services.Identity.Identity userIdentity)
    {
      string userPuid = UserClaims.GetUserPuid(requestContext, userIdentity);
      string str = (string) null;
      if (userIdentity.HasUpn())
        str = !userIdentity.HasEmaillessUpn() ? userIdentity.GetProperty<string>("Account", string.Empty) : IdentityHelper.GetPreferredEmailAddress(requestContext, userIdentity.Id);
      if (string.IsNullOrEmpty(str))
      {
        requestContext.Trace(1031082, TraceLevel.Warning, "VisualStudio.Services.LicensingService.UserClaims", "BusinessLogic", "Unexpected: (user identity) accountName is null.");
        return (UserClaims) null;
      }
      if (!string.IsNullOrEmpty(userPuid))
        return new UserClaims("MSID", userPuid, "Windows Live ID", str);
      if (!userIdentity.IsExternalUser)
        return new UserClaims("ORGID", str, string.Empty, str);
      string property = userIdentity.GetProperty<string>("Domain", string.Empty);
      if (!string.IsNullOrEmpty(property))
        return new UserClaims("ORGID", str, property, str);
      requestContext.Trace(1031080, TraceLevel.Warning, "VisualStudio.Services.LicensingService.UserClaims", "BusinessLogic", "Unexpected: (user udentity) domain is null.");
      return (UserClaims) null;
    }

    private static bool IsMsaPuid(string userPuid) => !string.IsNullOrEmpty(userPuid) && userPuid.All<char>((Func<char, bool>) (c => char.IsLetterOrDigit(c)));

    private static string GetUserPuid(IVssRequestContext requestContext, Microsoft.VisualStudio.Services.Identity.Identity userIdentity)
    {
      string userPuidFromSid = UserClaims.GetUserPuidFromSid(requestContext, userIdentity);
      if (UserClaims.IsMsaPuid(userPuidFromSid))
        return userPuidFromSid;
      string property = userIdentity.GetProperty<string>("PUID", string.Empty);
      return UserClaims.IsMsaPuid(property) ? property : (string) null;
    }

    private static string GetUserPuidFromSid(
      IVssRequestContext requestContext,
      Microsoft.VisualStudio.Services.Identity.Identity userIdentity)
    {
      if (userIdentity == null)
      {
        requestContext.Trace(1031030, TraceLevel.Warning, "VisualStudio.Services.LicensingService.UserClaims", "BusinessLogic", "Unexpected: userIdentity is null.");
        return (string) null;
      }
      if (userIdentity.Descriptor == (IdentityDescriptor) null)
      {
        requestContext.Trace(1031031, TraceLevel.Warning, "VisualStudio.Services.LicensingService.UserClaims", "BusinessLogic", "Unexpected: userIdentity.Descriptor is null.");
        return (string) null;
      }
      if (string.IsNullOrEmpty(userIdentity.Descriptor.Identifier))
      {
        requestContext.Trace(1031032, TraceLevel.Warning, "VisualStudio.Services.LicensingService.UserClaims", "BusinessLogic", "Unexpected: userIdentity.Descriptor.Identifier is empty.");
        return (string) null;
      }
      string[] strArray = userIdentity.Descriptor.Identifier.Split(new char[1]
      {
        '@'
      }, StringSplitOptions.RemoveEmptyEntries);
      if (strArray.Length != 2)
        return (string) null;
      string str = strArray[0];
      return !string.Equals(strArray[1], "live.com", StringComparison.OrdinalIgnoreCase) ? (string) null : str;
    }
  }
}
