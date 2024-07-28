// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.BoardColumnsValidator
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8CB058E6-FA40-46B0-B867-53112DFAAB81
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.dll

using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.Models;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common
{
  public class BoardColumnsValidator : BoardValidatorBase
  {
    protected BoardSettings BoardSettings { get; private set; }

    protected ProjectProcessConfiguration ProcessSettings { get; private set; }

    protected Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.Models.BacklogConfiguration BacklogConfiguration { get; private set; }

    protected string ProjectName { get; private set; }

    protected Guid ProjectId { get; private set; }

    protected ITeamSettings TeamSettings { get; private set; }

    protected BoardSettings ExistingBoardSettings { get; private set; }

    public BoardColumnsValidator(
      IVssRequestContext requestContext,
      BoardSettings boardSettings,
      ProjectProcessConfiguration processSettings,
      Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.Models.BacklogConfiguration backlogConfiguration,
      string projectName,
      ITeamSettings teamSettings,
      BoardSettings existingBoardSettings = null)
      : base(requestContext)
    {
      ArgumentUtility.CheckForNull<BoardSettings>(boardSettings, nameof (boardSettings));
      ArgumentUtility.CheckForNull<ProjectProcessConfiguration>(processSettings, nameof (processSettings));
      ArgumentUtility.CheckForNull<Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.Models.BacklogConfiguration>(backlogConfiguration, nameof (backlogConfiguration));
      ArgumentUtility.CheckForNull<string>(projectName, nameof (projectName));
      ArgumentUtility.CheckForNull<ITeamSettings>(teamSettings, nameof (teamSettings));
      this.BoardSettings = boardSettings;
      this.ProcessSettings = processSettings;
      this.BacklogConfiguration = backlogConfiguration;
      this.ProjectName = projectName;
      this.TeamSettings = teamSettings;
      this.ExistingBoardSettings = existingBoardSettings;
    }

    public override bool Validate(bool throwOnFail = false) => this.ExecuteValidator((Action) (() =>
    {
      BacklogLevelConfiguration backlogLevel = (BacklogLevelConfiguration) null;
      this.Assert((Func<bool>) (() => this.BacklogConfiguration.TryGetBacklogLevelConfiguration(this.BoardSettings.BacklogLevelId, out backlogLevel)), Resources.BoardValidator_CannotGetBacklogConfig);
      IEnumerable<BoardColumn> columns = this.BoardSettings.Columns;
      this.Assert((Func<bool>) (() => columns.Count<BoardColumn>() >= 2 && columns.Count<BoardColumn>() <= 100), Resources.BoardValidator_ColumnsCountInvalid);
      this.Assert((Func<bool>) (() => columns.First<BoardColumn>().ColumnType == BoardColumnType.Incoming), Resources.BoardValidator_FirstColumnMustBeIncoming);
      this.Assert((Func<bool>) (() => columns.Last<BoardColumn>().ColumnType == BoardColumnType.Outgoing), Resources.BoardValidator_LastColumnMustBeOutgoing);
      this.Assert((Func<bool>) (() => columns.Count<BoardColumn>((Func<BoardColumn, bool>) (c => c.ColumnType == BoardColumnType.InProgress)) == columns.Count<BoardColumn>() - 2), Resources.BoardValidator_MustNotHaveIncomingOutgoingIntheMiddle);
      this.Assert((Func<bool>) (() => !columns.Any<BoardColumn>((Func<BoardColumn, bool>) (c => c.IsDeleted))), Resources.BoardValidator_MustNotSetIsDeletedTrue);
      if (this.ExistingBoardSettings != null)
      {
        this.Assert((Func<bool>) (() =>
        {
          Guid? id9 = this.BoardSettings.Columns.First<BoardColumn>((Func<BoardColumn, bool>) (c => c.ColumnType == BoardColumnType.Incoming)).Id;
          Guid? id10 = this.ExistingBoardSettings.Columns.First<BoardColumn>((Func<BoardColumn, bool>) (c => c.ColumnType == BoardColumnType.Incoming)).Id;
          if (id9.HasValue != id10.HasValue)
            return false;
          return !id9.HasValue || id9.GetValueOrDefault() == id10.GetValueOrDefault();
        }), Resources.BoardValidator_CannotRecreateIncomingColumn);
        this.Assert((Func<bool>) (() =>
        {
          Guid? id11 = this.BoardSettings.Columns.First<BoardColumn>((Func<BoardColumn, bool>) (c => c.ColumnType == BoardColumnType.Outgoing)).Id;
          Guid? id12 = this.ExistingBoardSettings.Columns.First<BoardColumn>((Func<BoardColumn, bool>) (c => c.ColumnType == BoardColumnType.Outgoing)).Id;
          if (id11.HasValue != id12.HasValue)
            return false;
          return !id11.HasValue || id11.GetValueOrDefault() == id12.GetValueOrDefault();
        }), Resources.BoardValidator_CannotRecreateOutgoingColumn);
      }
      IDictionary<string, IDictionary<string, string[]>> allowedStateMappings = this.GetColumnTypeAllowedStateMappings(this.BacklogConfiguration, backlogLevel);
      foreach (BoardColumn boardColumn in columns)
      {
        BoardColumn column = boardColumn;
        this.Assert((Func<bool>) (() => !string.IsNullOrWhiteSpace(column.Name) && column.Name.Length <= (int) byte.MaxValue), Resources.BoardValidator_ColumnNameLengthInvalid, (object) (column.Name ?? string.Empty));
        this.Assert((Func<bool>) (() => !ArgumentUtility.IsInvalidString(column.Name, false, OSDetails.IsChineseOS)), Resources.BoardValidator_ColumnNameContainsInvalidChar, (object) column.Name);
        this.Assert((Func<bool>) (() => !column.Id.HasValue || column.Id.Value != Guid.Empty), Resources.BoardValidator_MustNotHaveEmptyGuidForColumnId, (object) column.Name);
        this.Assert((Func<bool>) (() => !column.Id.HasValue || this.ExistingBoardSettings == null || this.ExistingBoardSettings.Columns.Count<BoardColumn>((Func<BoardColumn, bool>) (c =>
        {
          Guid? id13 = c.Id;
          Guid? id14 = column.Id;
          if (id13.HasValue != id14.HasValue)
            return false;
          return !id13.HasValue || id13.GetValueOrDefault() == id14.GetValueOrDefault();
        })) == 1), Resources.BoardValidator_ColumnIdMustExist, (object) column.Name);
        this.Assert((Func<bool>) (() => column.Order >= 0), Resources.BoardValidator_ColumnOrderMustNotNegative, (object) column.Name);
        this.Assert((Func<bool>) (() => column.ItemLimit >= 0 && column.ItemLimit <= 999), Resources.BoardValidator_ItemLimitInvalid, (object) column.Name);
        if (column.ColumnType == BoardColumnType.Incoming || column.ColumnType == BoardColumnType.Outgoing)
        {
          this.Assert((Func<bool>) (() => !column.IsSplit), Resources.BoardValidator_IncomingOutgoingCannotBeSplit, (object) column.Name);
          this.Assert((Func<bool>) (() => string.IsNullOrEmpty(column.Description)), Resources.BoardValidator_MustNotHaveDoDOnIncomingOutgoing);
        }
        this.Assert((Func<bool>) (() => column.StateMappings != null), Resources.BoardValidator_StateMappingMustNotBeNull, (object) column.Name);
        this.Assert((Func<bool>) (() => column.Description == null || column.Description.Length <= 2000), Resources.BoardValidator_DescriptionTooLong, (object) column.Name);
        this.Assert((Func<bool>) (() => columns.Count<BoardColumn>((Func<BoardColumn, bool>) (c => TFStringComparer.BoardColumnName.Equals(c.Name, column.Name))) == 1), Resources.BoardValidator_MustNotHaveDuplicateColumnName, (object) column.Name);
        this.Assert((Func<bool>) (() => !column.Id.HasValue || columns.Count<BoardColumn>((Func<BoardColumn, bool>) (c =>
        {
          Guid? id15 = c.Id;
          Guid? id16 = column.Id;
          if (id15.HasValue != id16.HasValue)
            return false;
          return !id15.HasValue || id15.GetValueOrDefault() == id16.GetValueOrDefault();
        })) == 1), Resources.BoardValidator_MustNotHaveDuplicateColumnId, (object) column.Name);
        this.Assert((Func<bool>) (() => columns.Count<BoardColumn>((Func<BoardColumn, bool>) (c => c.Order == column.Order)) == 1), Resources.BoardValidator_MustNotHaveDuplicateColumnOrder, (object) column.Name);
        IDictionary<string, string[]> witColumnMappings = allowedStateMappings[((int) column.ColumnType).ToString()];
        this.Assert((Func<bool>) (() =>
        {
          List<string> list3 = witColumnMappings.Keys.ToList<string>();
          List<string> list4 = column.StateMappings.Keys.ToList<string>();
          list3.Sort();
          list4.Sort();
          return list3.SequenceEqual<string>((IEnumerable<string>) list4, (IEqualityComparer<string>) TFStringComparer.WorkItemTypeName);
        }), Resources.BoardValidator_MustDefineAllWITStateMapping, (object) column.Name, (object) backlogLevel.Id);
        foreach (KeyValuePair<string, string> stateMapping in column.StateMappings)
        {
          string workItemTypeName = stateMapping.Key;
          string state = stateMapping.Value;
          string normalizedWorkitemTypeName = witColumnMappings.Keys.FirstOrDefault<string>((Func<string, bool>) (k => TFStringComparer.WorkItemTypeName.Equals(workItemTypeName, k)));
          this.Assert((Func<bool>) (() => normalizedWorkitemTypeName != null), string.Format(Resources.BoardValidator_WorkItemTypeNameInStateMappingIsNotValid, (object) workItemTypeName));
          this.Assert((Func<bool>) (() => ((IEnumerable<string>) witColumnMappings[normalizedWorkitemTypeName]).Contains<string>(state, (IEqualityComparer<string>) TFStringComparer.WorkItemStateName)), Resources.BoardValidator_StateIsNotValid, (object) column.Name, (object) state, (object) column.ColumnType, (object) workItemTypeName);
        }
      }
    }), throwOnFail);

    public virtual IDictionary<string, IDictionary<string, string[]>> GetColumnTypeAllowedStateMappings(
      Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.Models.BacklogConfiguration backlogConfiguration,
      BacklogLevelConfiguration backlogLevel)
    {
      return KanbanUtils.Instance.GetColumnTypeAllowedStateMappings(this.RequestContext, this.ProjectName, backlogConfiguration, backlogLevel, this.TeamSettings.BugsBehavior == BugsBehavior.AsRequirements);
    }
  }
}
