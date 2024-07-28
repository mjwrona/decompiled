// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DstsHelper.AlternateLoginIdentity
// Assembly: Microsoft.TeamFoundation.DstsHelper, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 08D47267-3A15-4307-BBA0-1792E9C6BDF1
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DstsHelper.dll

using Microsoft.TeamFoundation.Framework.Server;
using System.Security.Principal;

namespace Microsoft.TeamFoundation.DstsHelper
{
  internal sealed class AlternateLoginIdentity : IIdentity
  {
    internal AlternateLoginIdentity(string authenticationType, Microsoft.VisualStudio.Services.Identity.Identity identity)
    {
      this.AuthenticationType = authenticationType;
      this.Identity = identity;
    }

    public string AuthenticationType { get; }

    internal Microsoft.VisualStudio.Services.Identity.Identity Identity { get; }

    public bool IsAuthenticated => true;

    public string Name => IdentityHelper.GetUniqueName(this.Identity);
  }
}
