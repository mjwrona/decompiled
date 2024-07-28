// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.BoardRowsValidator
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8CB058E6-FA40-46B0-B867-53112DFAAB81
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.dll

using Microsoft.Azure.Boards.Agile.Common.Exceptions;
using Microsoft.TeamFoundation.Common;
using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.Models;
using Microsoft.TeamFoundation.WorkItemTracking.Server;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common
{
  public class BoardRowsValidator : BoardValidatorBase
  {
    internal const int MaxRowLimit = 50;
    internal const int MaxRowNameLength = 255;

    protected BoardSettings BoardSettings { get; private set; }

    protected Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.Models.BacklogConfiguration BacklogConfiguration { get; private set; }

    public BoardRowsValidator(
      IVssRequestContext requestContext,
      BoardSettings boardSettings,
      Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.Models.BacklogConfiguration backlogConfiguration)
      : base(requestContext)
    {
      ArgumentUtility.CheckForNull<BoardSettings>(boardSettings, nameof (boardSettings));
      ArgumentUtility.CheckForNull<Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.Models.BacklogConfiguration>(backlogConfiguration, nameof (backlogConfiguration));
      this.BoardSettings = boardSettings;
      this.BacklogConfiguration = backlogConfiguration;
    }

    public override bool Validate(bool throwOnFail = false) => this.ExecuteValidator((Action) (() => this.ValidateInternal()), throwOnFail);

    private void ValidateInternal()
    {
      BacklogLevelConfiguration backlogLevel = (BacklogLevelConfiguration) null;
      this.Assert((Func<bool>) (() => this.BacklogConfiguration.TryGetBacklogLevelConfiguration(this.BoardSettings.BacklogLevelId, out backlogLevel)), Resources.BoardValidator_CannotGetBacklogConfig);
      List<BoardRow> rows = this.BoardSettings.Rows.ToList<BoardRow>();
      int rowCount = rows.Count;
      this.AssertException((Func<bool>) (() => rowCount <= 50), (Func<BoardValidatorExceptionBase>) (() => (BoardValidatorExceptionBase) new BoardValidatorRowCountInvalidException(Resources.BoardValidator_RowsCountInvalid)));
      this.Assert((Func<bool>) (() => rows.Count<BoardRow>((Func<BoardRow, bool>) (r => r.IsDefault)) == 1), Resources.BoardValidator_MustHaveOneDefaultRow);
      this.Assert((Func<bool>) (() => rows.Find((Predicate<BoardRow>) (r => r.IsDefault)).Color.IsNullOrEmpty<char>()), Resources.BoardValidator_DefaultRowMustNotHaveColor);
      for (int index = 0; index < rowCount; ++index)
      {
        BoardRow row = rows[index];
        string rowNameInErrorMessage = row.Name ?? string.Empty;
        if (!row.Color.IsNullOrEmpty<char>())
          CommonWITUtils.CheckColorWithHashSymbol(row.Color);
        if (!row.IsDefault)
          this.Assert((Func<bool>) (() => !row.Name.IsNullOrEmpty<char>()), Resources.BoardValidator_RowInvalidNonDefaultName);
        if (!row.Name.IsNullOrEmpty<char>())
        {
          this.AssertException((Func<bool>) (() => row.Name.Length <= (int) byte.MaxValue), (Func<BoardValidatorExceptionBase>) (() => (BoardValidatorExceptionBase) new BoardValidatorRowNameLengthInvalidException(string.Format(Resources.BoardValidator_RowNameLengthInvalid, (object) rowNameInErrorMessage), rowNameInErrorMessage)));
          this.AssertException((Func<bool>) (() => !ArgumentUtility.IsInvalidString(row.Name, false)), (Func<BoardValidatorExceptionBase>) (() => (BoardValidatorExceptionBase) new BoardValidatorInvalidCharException(string.Format(Resources.BoardValidator_RowNameContainsInvalidChar, (object) rowNameInErrorMessage), rowNameInErrorMessage)));
        }
        this.AssertException((Func<bool>) (() => rows.Count<BoardRow>((Func<BoardRow, bool>) (r => TFStringComparer.BoardRowName.Equals(r.Name, row.Name))) == 1), (Func<BoardValidatorExceptionBase>) (() => (BoardValidatorExceptionBase) new BoardValidatorDuplicateRowNameException(string.Format(Resources.BoardValidator_MustNotHaveDuplicateRowName, (object) rowNameInErrorMessage), rowNameInErrorMessage)));
        this.Assert((Func<bool>) (() => !row.Id.HasValue || rows.Count<BoardRow>((Func<BoardRow, bool>) (r =>
        {
          Guid? id1 = r.Id;
          Guid? id2 = row.Id;
          if (id1.HasValue != id2.HasValue)
            return false;
          return !id1.HasValue || id1.GetValueOrDefault() == id2.GetValueOrDefault();
        })) == 1), Resources.BoardValidator_MustNotHaveDuplicateRowId, (object) rowNameInErrorMessage);
      }
    }
  }
}
