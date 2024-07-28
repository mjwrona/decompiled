// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Git.Server.GitPublicProjectRequestRestrictionsAttribute
// Assembly: Microsoft.TeamFoundation.Git.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1F0714E-7EF5-4D28-9AF2-C8D8620EA079
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Git.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.Types;

namespace Microsoft.TeamFoundation.Git.Server
{
  internal class GitPublicProjectRequestRestrictionsAttribute : 
    PublicProjectRequestRestrictionsAttribute
  {
    public GitPublicProjectRequestRestrictionsAttribute()
    {
    }

    public GitPublicProjectRequestRestrictionsAttribute(
      AuthenticationMechanisms mechanisms,
      UserAgentFilterType agentFilterType,
      string agentFilter)
      : base(false, false, (string) null, RequestRestrictions.DefaultRequestRestrictions.RequiredAuthentication, RequestRestrictions.DefaultRequestRestrictions.AllowNonSsl, RequestRestrictions.DefaultRequestRestrictions.AllowCORS, mechanisms, (string) null, agentFilterType, agentFilter)
    {
    }
  }
}
