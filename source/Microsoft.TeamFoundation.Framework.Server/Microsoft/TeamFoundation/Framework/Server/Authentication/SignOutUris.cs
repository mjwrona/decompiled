// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.Authentication.SignOutUris
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.Framework.Server.Authentication
{
  public class SignOutUris
  {
    public SignOutUris()
      : this((ISet<Uri>) null, (ISet<Uri>) null)
    {
    }

    public SignOutUris(ISet<Uri> cookieSignOutUris, ISet<Uri> msalJsSignOutUris)
    {
      this.CookieSignOutUris = cookieSignOutUris ?? (ISet<Uri>) new HashSet<Uri>();
      this.MsalJsSignOutUris = msalJsSignOutUris ?? (ISet<Uri>) new HashSet<Uri>();
    }

    public ISet<Uri> CookieSignOutUris { get; }

    public ISet<Uri> MsalJsSignOutUris { get; }
  }
}
