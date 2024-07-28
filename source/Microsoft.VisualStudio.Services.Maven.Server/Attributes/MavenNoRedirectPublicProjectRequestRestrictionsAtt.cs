// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Maven.Server.Attributes.MavenNoRedirectPublicProjectRequestRestrictionsAttribute
// Assembly: Microsoft.VisualStudio.Services.Maven.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3AEBE02E-FDD2-41D8-89F7-5C54445DBFA7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Maven.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.Types;

namespace Microsoft.VisualStudio.Services.Maven.Server.Attributes
{
  public class MavenNoRedirectPublicProjectRequestRestrictionsAttribute : 
    PublicProjectRequestRestrictionsAttribute
  {
    public MavenNoRedirectPublicProjectRequestRestrictionsAttribute()
      : base(false, true, (string) null, RequiredAuthentication.ValidatedUser, false, true, AuthenticationMechanisms.Basic | AuthenticationMechanisms.Windows | AuthenticationMechanisms.Federated | AuthenticationMechanisms.OAuth, "MavenPublicProjectNoRedirect", UserAgentFilterType.Regex, "(Java|Maven)")
    {
    }
  }
}
