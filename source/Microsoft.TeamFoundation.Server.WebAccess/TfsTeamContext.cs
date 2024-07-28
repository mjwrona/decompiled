// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.TfsTeamContext
// Assembly: Microsoft.TeamFoundation.Server.WebAccess, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A2CCA8C5-6910-48A5-82D9-BDC1350B5B4D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.dll

using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.Server.WebAccess
{
  [DataContract]
  public class TfsTeamContext : TeamContext
  {
    private IVssRequestContext m_requestContext;
    private WebApiTeam m_team;

    public TfsTeamContext(IVssRequestContext requestContext, WebApiTeam team)
    {
      this.m_requestContext = requestContext;
      this.m_team = team;
      this.Name = team.Name;
      this.Id = team.Id;
    }
  }
}
