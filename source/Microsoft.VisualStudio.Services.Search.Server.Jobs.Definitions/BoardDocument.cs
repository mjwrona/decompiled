// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Server.Jobs.Definitions.BoardDocument
// Assembly: Microsoft.VisualStudio.Services.Search.Server.Jobs.Definitions, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 10D2EBC4-B606-4155-939F-EEB226A80181
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.Server.Jobs.Definitions.dll

using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.VisualStudio.Services.Search.Common.Enums;
using System;
using System.Runtime.CompilerServices;

namespace Microsoft.VisualStudio.Services.Search.Server.Jobs.Definitions
{
  public class BoardDocument : CorePipelineDocument<string>
  {
    public BoardType Type { get; }

    public WebApiTeam Team { get; }

    public BoardDocument(BoardType boardType, WebApiTeam team)
      : base(team.Id.ToString() + "_" + boardType.ToString())
    {
      this.Type = boardType;
      this.Team = team ?? throw new ArgumentNullException(nameof (team));
    }

    public override string ToString() => FormattableString.Invariant(FormattableStringFactory.Create("Board Type: [{0}], Team Url: [{1}]", (object) this.Type, (object) this.Team.IdentityUrl));
  }
}
