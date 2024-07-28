// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.DataAccess.BoardSettingsComponent
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8CB058E6-FA40-46B0-B867-53112DFAAB81
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.dll

using Microsoft.SqlServer.Server;
using Microsoft.TeamFoundation.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.DataAccess
{
  internal class BoardSettingsComponent : TeamFoundationSqlResourceComponent
  {
    public static readonly ComponentFactory ComponentFactory = new ComponentFactory(new IComponentCreator[20]
    {
      (IComponentCreator) new ComponentCreator<BoardSettingsComponent>(1),
      (IComponentCreator) new ComponentCreator<BoardSettingsComponent>(2),
      (IComponentCreator) new ComponentCreator<BoardSettingsComponent2>(3),
      (IComponentCreator) new ComponentCreator<BoardSettingsComponent3>(4),
      (IComponentCreator) new ComponentCreator<BoardSettingsComponent4>(5),
      (IComponentCreator) new ComponentCreator<BoardSettingsComponent5>(6),
      (IComponentCreator) new ComponentCreator<BoardSettingsComponent6>(7),
      (IComponentCreator) new ComponentCreator<BoardSettingsComponent7>(8),
      (IComponentCreator) new ComponentCreator<BoardSettingsComponent8>(9),
      (IComponentCreator) new ComponentCreator<BoardSettingsComponent9>(10),
      (IComponentCreator) new ComponentCreator<BoardSettingsComponent10>(11),
      (IComponentCreator) new ComponentCreator<BoardSettingsComponent11>(12),
      (IComponentCreator) new ComponentCreator<BoardSettingsComponent12>(13),
      (IComponentCreator) new ComponentCreator<BoardSettingsComponent13>(14),
      (IComponentCreator) new ComponentCreator<BoardSettingsComponent14>(15),
      (IComponentCreator) new ComponentCreator<BoardSettingsComponent15>(16),
      (IComponentCreator) new ComponentCreator<BoardSettingsComponent16>(17),
      (IComponentCreator) new ComponentCreator<BoardSettingsComponent17>(18),
      (IComponentCreator) new ComponentCreator<BoardSettingsComponent18>(19),
      (IComponentCreator) new ComponentCreator<BoardSettingsComponent19>(20)
    }, "BoardSettings", "WorkItem");
    private static Dictionary<int, SqlExceptionFactory> s_sqlExceptionFactories = new Dictionary<int, SqlExceptionFactory>()
    {
      {
        1300002,
        new SqlExceptionFactory(typeof (BoardExistsException))
      },
      {
        1300003,
        new SqlExceptionFactory(typeof (BoardChangedException))
      }
    };
    private static SqlMetaData[] typ_BoardColumnTable = new SqlMetaData[8]
    {
      new SqlMetaData("Id", SqlDbType.UniqueIdentifier),
      new SqlMetaData("BoardId", SqlDbType.UniqueIdentifier),
      new SqlMetaData("Name", SqlDbType.NVarChar, 256L),
      new SqlMetaData("Order", SqlDbType.Int),
      new SqlMetaData("ItemLimit", SqlDbType.Int),
      new SqlMetaData("ColumnType", SqlDbType.Int),
      new SqlMetaData("RevisedDate", SqlDbType.DateTime),
      new SqlMetaData("Deleted", SqlDbType.Bit)
    };
    private System.Func<int, Guid> m_getDataSpaceIdentifierFunc;
    private System.Func<Guid, int> m_getDataSpaceIdFunc;
    private IVssRequestContext m_testRequestContext;

    public BoardSettingsComponent()
    {
      this.ContainerErrorCode = 50000;
      this.SelectedFeatures |= SqlResourceComponentFeatures.BindPartitionIdByDefault;
    }

    public static BoardSettingsComponent GetBoardSettingsComponent(
      string connectionString,
      int partitionId,
      System.Func<int, Guid> getDataSpaceIdentifierFunc,
      System.Func<Guid, int> getDataSpaceIdFunc,
      IVssRequestContext testRequestContext = null)
    {
      ISqlConnectionInfo connectionInfo = SqlConnectionInfoFactory.Create(connectionString);
      IComponentCreator componentCreator = BoardSettingsComponent.ComponentFactory.GetLastComponentCreator();
      if (componentCreator == null)
        return (BoardSettingsComponent) null;
      BoardSettingsComponent settingsComponent = componentCreator.Create(connectionInfo, 3600, 20, 1, (ITFLogger) new NullLogger(), (CircuitBreakerDatabaseProperties) null) as BoardSettingsComponent;
      settingsComponent.PartitionId = partitionId;
      settingsComponent.m_getDataSpaceIdentifierFunc = getDataSpaceIdentifierFunc;
      settingsComponent.m_getDataSpaceIdFunc = getDataSpaceIdFunc;
      settingsComponent.m_testRequestContext = testRequestContext;
      return settingsComponent;
    }

    public override int GetDataspaceId(Guid dataspaceIdentifier) => this.m_getDataSpaceIdFunc != null ? this.m_getDataSpaceIdFunc(dataspaceIdentifier) : base.GetDataspaceId(dataspaceIdentifier);

    protected override IVssRequestContext RequestContext => this.m_testRequestContext != null ? this.m_testRequestContext : base.RequestContext;

    public override Guid GetDataspaceIdentifier(int dataspaceId) => this.m_getDataSpaceIdentifierFunc != null ? this.m_getDataSpaceIdentifierFunc(dataspaceId) : base.GetDataspaceIdentifier(dataspaceId);

    public static BoardSettingsComponent CreateComponent(IVssRequestContext requestContext) => requestContext.CreateComponent<BoardSettingsComponent>();

    public virtual BoardSettingsDTO CreateBoard(Guid projectId, BoardSettings board)
    {
      ArgumentUtility.CheckForNull<BoardSettings>(board, nameof (board));
      this.PrepareStoredProcedure("prc_CreateBoard");
      this.BindDataspace(projectId);
      this.BindGuid("@teamId", board.TeamId);
      if (board.ExtensionId.HasValue)
        this.BindGuid("@extensionId", board.ExtensionId.Value);
      else
        this.BindNullValue("@extensionId", SqlDbType.UniqueIdentifier);
      this.BindBoardColumnRowTable("@boardColumnTable", this.GetBoardColumnRows(board.Columns));
      board.Id = new Guid?((Guid) this.ExecuteScalar());
      return this.GetBoard(projectId, board.TeamId, board.BacklogLevelId);
    }

    public virtual BoardSettingsDTO GetBoard(Guid projectId, Guid teamId, string backlogLevelId)
    {
      this.PrepareStoredProcedure("prc_GetBoard");
      this.BindDataspace(projectId);
      this.BindGuid("@teamId", teamId);
      BoardSettingsDTO board = (BoardSettingsDTO) null;
      using (ResultCollection rc = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        board = this.GetBoardSettings(rc);
        if (board != null)
          board.BacklogLevelId = backlogLevelId;
      }
      return board;
    }

    public virtual IList<BoardColumnRevisionForReporting> GetBoardColumnRevisions(int watermark) => (IList<BoardColumnRevisionForReporting>) new List<BoardColumnRevisionForReporting>();

    public virtual IEnumerable<BoardColumnRevision> GetBoardRevisions(
      Guid projectId,
      Guid teamId,
      string backlogLevelId)
    {
      this.PrepareStoredProcedure("prc_GetBoardColumnRevisions");
      this.BindDataspace(projectId);
      this.BindGuid("@teamId", teamId);
      using (ResultCollection rc = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
        return (IEnumerable<BoardColumnRevision>) this.GetBoardColumnRevisions(rc);
    }

    public virtual IList<BoardRowRevisionForReporting> GetBoardRowRevisions(int watermark) => (IList<BoardRowRevisionForReporting>) new List<BoardRowRevisionForReporting>();

    public virtual IEnumerable<BoardRowRevision> GetBoardRowRevisions(
      Guid projectId,
      Guid teamId,
      string backlogLevelId)
    {
      return Enumerable.Empty<BoardRowRevision>();
    }

    public virtual void UpdateBoard(Guid projectId, BoardSettings board)
    {
      ArgumentUtility.CheckForNull<BoardSettings>(board, nameof (board));
      this.PrepareStoredProcedure("prc_UpdateBoard");
      this.BindDataspace(projectId);
      this.BindGuid("@boardId", board.Id.Value);
      this.BindBoardColumnRowTable("@boardColumnTable", this.GetBoardColumnRows(board.Columns));
    }

    protected virtual void AddBoardSettingsBinder(ResultCollection rc) => rc.AddBinder<BoardSettingsDTO>((ObjectBinder<BoardSettingsDTO>) new BoardSettingsDTOBinder());

    protected virtual void AddBoardRowRowBinder(ResultCollection rc) => rc.AddBinder<BoardRowTable>((ObjectBinder<BoardRowTable>) new BoardRowRowBinder());

    protected virtual BoardSettingsDTO GetBoardSettings(ResultCollection rc)
    {
      BoardSettingsDTO boardSettings = (BoardSettingsDTO) null;
      this.AddBoardSettingsBinder(rc);
      rc.AddBinder<BoardColumnRow>((ObjectBinder<BoardColumnRow>) new BoardColumnRowBinder());
      List<BoardSettingsDTO> items = rc.GetCurrent<BoardSettingsDTO>().Items;
      if (items.Any<BoardSettingsDTO>())
      {
        boardSettings = items[0];
        rc.NextResult();
      }
      return boardSettings;
    }

    protected virtual void BindDataspace(Guid dataspaceIdentifier)
    {
    }

    public virtual Dictionary<Guid, SortedSet<string>> GetBoardColumnSuggestedValues(Guid? projectId = null) => new Dictionary<Guid, SortedSet<string>>();

    public virtual Dictionary<Guid, SortedSet<string>> GetBoardRowSuggestedValues(Guid? projectId = null) => new Dictionary<Guid, SortedSet<string>>();

    public virtual BoardInput GetBoardInput(Guid projectId, Guid boardId) => (BoardInput) null;

    protected virtual IList<BoardColumnRevision> GetBoardColumnRevisions(ResultCollection rc)
    {
      rc.AddBinder<BoardColumnRow>((ObjectBinder<BoardColumnRow>) new BoardColumnRowBinder());
      return (IList<BoardColumnRevision>) rc.GetCurrent<BoardColumnRow>().Items.Select<BoardColumnRow, BoardColumnRevision>((System.Func<BoardColumnRow, BoardColumnRevision>) (bcr => new BoardColumnRevision()
      {
        Id = bcr.Id,
        Name = bcr.Name,
        Order = bcr.Order,
        ItemLimit = bcr.ItemLimit,
        ColumnType = (BoardColumnType) bcr.ColumnType,
        RevisedDate = bcr.RevisedDate,
        Deleted = bcr.Deleted
      })).ToList<BoardColumnRevision>();
    }

    protected virtual IEnumerable<BoardColumnRow> GetBoardColumnRows(
      IEnumerable<BoardColumn> columns)
    {
      return columns.Select<BoardColumn, BoardColumnRow>((System.Func<BoardColumn, BoardColumnRow>) (c => new BoardColumnRow()
      {
        Id = new Guid?(c.Id.HasValue ? c.Id.Value : Guid.NewGuid()),
        Name = c.Name,
        Order = c.Order,
        ItemLimit = c.ItemLimit,
        ColumnType = (int) c.ColumnType,
        RevisedDate = new DateTime?(),
        Deleted = false
      }));
    }

    public virtual void DeleteBoards(IEnumerable<Guid> boardIds)
    {
    }

    public virtual void UpdateBoardExtension(Guid projectId, Guid boardId, Guid extensionId)
    {
    }

    public virtual void UpdateBoardColumns(
      Guid projectId,
      Guid boardId,
      IEnumerable<BoardColumn> columns)
    {
    }

    public virtual void UpdateBoardRows(Guid projectId, Guid boardId, IEnumerable<BoardRow> rows)
    {
    }

    public virtual List<BoardRecord> GetTeamBoards(IEnumerable<Guid> teamIds) => new List<BoardRecord>();

    protected override IDictionary<int, SqlExceptionFactory> TranslatedExceptions => (IDictionary<int, SqlExceptionFactory>) BoardSettingsComponent.s_sqlExceptionFactories;

    protected virtual SqlParameter BindBoardColumnRowTable(
      string parameterName,
      IEnumerable<BoardColumnRow> rows)
    {
      return this.BindTable(parameterName, "typ_BoardColumnTable", rows.Select<BoardColumnRow, SqlDataRecord>((System.Func<BoardColumnRow, SqlDataRecord>) (row =>
      {
        SqlDataRecord sqlDataRecord1 = new SqlDataRecord(BoardSettingsComponent.typ_BoardColumnTable);
        Guid? nullable;
        if (row.Id.HasValue)
        {
          SqlDataRecord sqlDataRecord2 = sqlDataRecord1;
          nullable = row.Id;
          Guid guid = nullable.Value;
          sqlDataRecord2.SetGuid(0, guid);
        }
        else
          sqlDataRecord1.SetDBNull(0);
        nullable = row.BoardId;
        if (nullable.HasValue)
        {
          SqlDataRecord sqlDataRecord3 = sqlDataRecord1;
          nullable = row.BoardId;
          Guid guid = nullable.Value;
          sqlDataRecord3.SetGuid(1, guid);
        }
        else
          sqlDataRecord1.SetDBNull(1);
        sqlDataRecord1.SetString(2, row.Name);
        sqlDataRecord1.SetInt32(3, row.Order);
        sqlDataRecord1.SetInt32(4, row.ItemLimit);
        sqlDataRecord1.SetValue(5, (object) row.ColumnType);
        if (row.RevisedDate.HasValue)
          sqlDataRecord1.SetDateTime(6, row.RevisedDate.Value);
        else
          sqlDataRecord1.SetDBNull(6);
        sqlDataRecord1.SetValue(7, (object) row.Deleted);
        return sqlDataRecord1;
      })));
    }

    public virtual IEnumerable<BoardCardSettingRow> GetBoardCardSettings(
      Guid projectId,
      BoardCardSettings.ScopeType scopeType,
      Guid scopeId)
    {
      return Enumerable.Empty<BoardCardSettingRow>();
    }

    public virtual void SetBoardCardSettings(Guid projectId, BoardCardSettings cardSettings)
    {
    }

    public virtual BoardCardRulesDTO GetBoardCardRules(
      Guid projectId,
      BoardCardSettings.ScopeType scopeType,
      Guid scopeId)
    {
      return new BoardCardRulesDTO();
    }

    public virtual void SetCardRules(
      Guid projectId,
      string scope,
      Guid boardId,
      IEnumerable<BoardCardRuleRow> rules,
      IEnumerable<RuleAttributeRow> attributes)
    {
    }

    public virtual void UpdateCardRules(
      Guid projectId,
      string scope,
      Guid boardId,
      IEnumerable<BoardCardRuleRow> rules,
      IEnumerable<RuleAttributeRow> attributes,
      List<string> typesToBeDeleted)
    {
    }

    public virtual void UpdateBoardOptions(
      Guid projectId,
      Guid boardId,
      int cardReordering,
      bool statusBadgeIsPublic = false)
    {
    }

    public virtual Dictionary<string, string> GetBoardOptions(Guid projectId, Guid boardId) => new Dictionary<string, string>();

    public virtual BoardOptionRecord GetBoardOptionsRecord(Guid projectId, Guid boardId) => (BoardOptionRecord) null;

    public virtual IEnumerable<BoardRecord> GetAllBoards(Guid? projectId = null, Guid? teamId = null) => Enumerable.Empty<BoardRecord>();

    public virtual IEnumerable<BoardRecord> GetBoardsByIds(IEnumerable<Guid> boardIds) => Enumerable.Empty<BoardRecord>();

    public virtual IEnumerable<BoardRecord> GetBoardsByCategoryReferenceNames(
      IEnumerable<string> categoryReferenceNames)
    {
      return Enumerable.Empty<BoardRecord>();
    }

    public virtual void SoftDeleteBoards(IEnumerable<Guid> boardIds)
    {
    }

    public virtual BoardSettingsDTO RestoreBoard(
      Guid projectId,
      Guid teamId,
      string backlogLevelId)
    {
      return (BoardSettingsDTO) null;
    }
  }
}
