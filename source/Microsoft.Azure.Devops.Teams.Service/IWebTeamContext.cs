// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Devops.Teams.Service.IWebTeamContext
// Assembly: Microsoft.Azure.Devops.Teams.Service, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 643BB522-27FA-4CE7-8A1E-31D95F8AC409
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Devops.Teams.Service.dll

using Microsoft.TeamFoundation.Core.WebApi;

namespace Microsoft.Azure.Devops.Teams.Service
{
  public interface IWebTeamContext
  {
    ProjectInfo Project { get; }

    WebApiTeam Team { get; }
  }
}
