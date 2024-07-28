// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.Authentication.UserAuthentication.UserAuthenticationConfiguration
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Framework.Common;
using System;

namespace Microsoft.TeamFoundation.Framework.Server.Authentication.UserAuthentication
{
  public class UserAuthenticationConfiguration
  {
    public bool UseSlidingExpiration { get; set; }

    public TimeSpan ExpireTimeSpan { get; set; }

    public TimeSpan? ReIssueTimeSpan { get; set; }

    public string CookieName { get; set; }

    public string Audience { get; set; }

    public string Issuer { get; set; }

    public string CurrentVersion { get; set; }

    public ITimeProvider TimeProvider { get; set; }
  }
}
