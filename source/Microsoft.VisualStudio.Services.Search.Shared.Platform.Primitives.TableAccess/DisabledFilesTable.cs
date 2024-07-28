// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.TableAccess.DisabledFilesTable
// Assembly: Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.TableAccess, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: D3998EAE-13E8-421A-93CB-363047218BB4
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.TableAccess.dll

using Microsoft.SqlServer.Server;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Search.Common;
using Microsoft.VisualStudio.Services.Search.Common.Enums;
using Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.TableAccess.Common;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.TableAccess
{
  internal class DisabledFilesTable : SQLTable<DisabledFile>
  {
    private const string ServiceName = "Search_DisabledFiles";
    public static readonly ComponentFactory ComponentFactory = new ComponentFactory(new IComponentCreator[2]
    {
      (IComponentCreator) new ComponentCreator<DisabledFilesTable>(1, true),
      (IComponentCreator) new ComponentCreator<DisabledFilesTableV2>(2, false)
    }, "Search_DisabledFiles");
    private DisabledFilesTable.Columns m_columns = new DisabledFilesTable.Columns();
    private static readonly SqlMetaData[] s_gitDisabledFileSqlTableEntityLookupTable = new SqlMetaData[7]
    {
      new SqlMetaData("CollectionId", SqlDbType.UniqueIdentifier),
      new SqlMetaData("RepositoryId", SqlDbType.UniqueIdentifier),
      new SqlMetaData("BranchName", SqlDbType.NVarChar, 256L),
      new SqlMetaData("FilePath", SqlDbType.NVarChar, 512L),
      new SqlMetaData("DisableReason", SqlDbType.TinyInt),
      new SqlMetaData("ContentHash", SqlDbType.VarChar, 40L),
      new SqlMetaData("LastUpdatedTimeStamp", SqlDbType.DateTime2)
    };

    public DisabledFilesTable()
      : base()
    {
    }

    internal DisabledFilesTable(string connectionString, int partitionId)
      : base(connectionString, partitionId)
    {
    }

    public override DisabledFile Insert(DisabledFile disabledFile)
    {
      this.ValidateNotNull<DisabledFile>(nameof (disabledFile), disabledFile);
      try
      {
        this.PrepareStoredProcedure("Search.prc_AddEntryForDisabledFilesTable");
        IList<DisabledFile> rows = (IList<DisabledFile>) new List<DisabledFile>();
        rows.Add(disabledFile);
        this.BindDisabledFileEntityLookupTable("@itemList", (IEnumerable<DisabledFile>) rows);
        this.ExecuteNonQuery(false);
      }
      catch (Exception ex)
      {
        throw new TableAccessException(TableAcessErrorCodeEnum.UNEXPECTED_ERROR, ex, string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Failed to Add entity with File Path {0}  with SQL Azure platform", (object) disabledFile.FilePath));
      }
      return (DisabledFile) null;
    }

    public override List<DisabledFile> RetriveTableEntityList(
      int count,
      TableEntityFilterList filterList)
    {
      if (filterList != null)
      {
        if (filterList.Count == 2)
        {
          try
          {
            this.PrepareStoredProcedure("Search.prc_QueryOnDisabledFilesTable");
            this.BindInt("@count", count);
            this.BindGuid("@collectionId", new Guid(TableEntityHelper.RetrieveFilterValue(filterList, "CollectionId")));
            this.BindGuid("@repositoryId", new Guid(TableEntityHelper.RetrieveFilterValue(filterList, "RepositoryId")));
            using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
            {
              resultCollection.AddBinder<DisabledFile>((ObjectBinder<DisabledFile>) new ProjectionBinder<DisabledFile>((System.Func<SqlDataReader, DisabledFile>) (r => new DisabledFile()
              {
                FilePath = this.m_columns.GetFilePath.GetString((IDataReader) r, true),
                DisableReason = (RejectionCode) this.m_columns.GetDisableReason.GetByte((IDataReader) r)
              })));
              ObjectBinder<DisabledFile> current = resultCollection.GetCurrent<DisabledFile>();
              return current != null && current.Items != null && current.Items.Count > 0 ? current.Items : new List<DisabledFile>();
            }
          }
          catch (Exception ex)
          {
            throw new TableAccessException(TableAcessErrorCodeEnum.UNEXPECTED_ERROR, ex, "Failed to retrieve Entity List with SQL Azure Platform");
          }
        }
      }
      throw new TableAccessException(TableAcessErrorCodeEnum.INVALID_ARGUMENTS, (Exception) new ArgumentException("Collection Id and Repository Id are mandatory filters for this Operation"));
    }

    protected SqlParameter BindDisabledFileEntityLookupTable(
      string parameterName,
      IEnumerable<DisabledFile> rows)
    {
      rows = rows ?? Enumerable.Empty<DisabledFile>();
      System.Func<DisabledFile, SqlDataRecord> selector = (System.Func<DisabledFile, SqlDataRecord>) (entity =>
      {
        SqlDataRecord sqlDataRecord = new SqlDataRecord(DisabledFilesTable.s_gitDisabledFileSqlTableEntityLookupTable);
        sqlDataRecord.SetString(3, entity.FilePath);
        sqlDataRecord.SetByte(4, (byte) entity.DisableReason);
        return sqlDataRecord;
      });
      return this.BindTable(parameterName, "Search.typ_DisabledFilesDescriptor", rows.Select<DisabledFile, SqlDataRecord>(selector));
    }

    private class Columns : Dictionary<string, SqlColumnBinder>
    {
      public SqlColumnBinder GetCollectionId = new SqlColumnBinder("CollectionId");
      public SqlColumnBinder GetRepositoryId = new SqlColumnBinder("RepositoryId");
      public SqlColumnBinder GetBranchName = new SqlColumnBinder("BranchName");
      public SqlColumnBinder GetFilePath = new SqlColumnBinder("FilePath");
      public SqlColumnBinder GetDisableReason = new SqlColumnBinder("DisableReason");
      public SqlColumnBinder GetContentHash = new SqlColumnBinder("ContentHash");
      public SqlColumnBinder GetLastUpdatedTimeStamp = new SqlColumnBinder("LastUpdatedTimeStamp");
    }
  }
}
