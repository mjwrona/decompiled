// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.BoardValidator
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8CB058E6-FA40-46B0-B867-53112DFAAB81
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.Models;
using System;

namespace Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common
{
  public class BoardValidator : BoardValidatorBase
  {
    protected BoardSettings BoardSettings { get; set; }

    protected Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.Models.BacklogConfiguration BacklogConfiguration { get; set; }

    public BoardValidator(
      IVssRequestContext requestContext,
      BoardSettings boardSettings,
      Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.Models.BacklogConfiguration backlogConfiguration)
      : base(requestContext)
    {
      this.BoardSettings = boardSettings;
      this.BacklogConfiguration = backlogConfiguration;
    }

    public override bool Validate(bool throwOnFail = false) => this.ExecuteValidator((Action) (() =>
    {
      BoardSettings board = this.BoardSettings;
      BacklogLevelConfiguration backlogLevel = (BacklogLevelConfiguration) null;
      this.Assert((Func<bool>) (() =>
      {
        Guid? extensionId = board.ExtensionId;
        Guid empty = Guid.Empty;
        if (!extensionId.HasValue)
          return true;
        return extensionId.HasValue && extensionId.GetValueOrDefault() != empty;
      }), Resources.BoardValidator_WITExtensionMustNotBeNull);
      this.Assert((Func<bool>) (() =>
      {
        Guid? id = board.Id;
        Guid empty = Guid.Empty;
        if (!id.HasValue)
          return true;
        return id.HasValue && id.GetValueOrDefault() != empty;
      }), Resources.BoardValidator_BoardIdEmpty);
      this.Assert((Func<bool>) (() => board.TeamId != Guid.Empty), Resources.BoardValidator_TeamIdMustNotBeEmpty);
      this.Assert((Func<bool>) (() => this.BacklogConfiguration.TryGetBacklogLevelConfiguration(board.BacklogLevelId, out backlogLevel)), Resources.BoardValidator_BoardCategoryRefNameInvalid, (object) board.BacklogLevelId);
    }), throwOnFail);
  }
}
