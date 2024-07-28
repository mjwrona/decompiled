// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.BasicAuthService
// Assembly: Microsoft.VisualStudio.Services.Cloud, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F2D48A0E-C4C9-4233-BA34-E8461823F6F2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cloud.dll

using System;
using System.Web;

namespace Microsoft.TeamFoundation.Framework.Server
{
  public abstract class BasicAuthService : ITeamFoundationBasicAuthService, IVssFrameworkService
  {
    public abstract void ServiceStart(IVssRequestContext systemRequestContext);

    public abstract void ServiceEnd(IVssRequestContext systemRequestContext);

    public abstract void DeleteBasicCredential(IVssRequestContext requestContext, Microsoft.VisualStudio.Services.Identity.Identity identity);

    public abstract bool IsBasicAuthDisabled(IVssRequestContext requestContext, Microsoft.VisualStudio.Services.Identity.Identity identity);

    public abstract bool HasBasicCredential(IVssRequestContext requestContext, Microsoft.VisualStudio.Services.Identity.Identity identity);

    public abstract void SetBasicCredential(
      IVssRequestContext requestContext,
      Microsoft.VisualStudio.Services.Identity.Identity identity,
      string password);

    public abstract bool IsValidBasicCredential(
      IVssRequestContext requestContext,
      Microsoft.VisualStudio.Services.Identity.Identity identity,
      string password);

    public abstract void EnableDisabledAccount(IVssRequestContext requestContext, Guid identityId);

    protected static void CheckAlternateCredentials()
    {
      HttpContextBase current = HttpContextFactory.Current;
      if (current?.User?.Identity != null && string.Equals(current.User.Identity.GetType().FullName, "Microsoft.VisualStudio.Services.Cloud.AlternateLoginIdentity", StringComparison.OrdinalIgnoreCase))
        throw new InvalidOperationException(FrameworkResources.AlternateLoginRestriction());
    }

    protected static bool IsValidBasicPassword(string password)
    {
      if (password == null || password.Length < 8 || password.Length > 32)
        return false;
      bool flag1 = false;
      bool flag2 = false;
      bool flag3 = false;
      bool flag4 = false;
      foreach (char c in password)
      {
        flag1 |= char.IsDigit(c);
        flag2 |= char.IsUpper(c);
        flag3 |= char.IsLower(c);
        flag4 |= !char.IsLetterOrDigit(c);
      }
      return (flag1 ? 1 : 0) + (flag2 ? 1 : 0) + (flag3 ? 1 : 0) + (flag4 ? 1 : 0) >= 3;
    }
  }
}
