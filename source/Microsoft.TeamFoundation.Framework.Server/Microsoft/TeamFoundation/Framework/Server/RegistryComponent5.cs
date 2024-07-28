// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.RegistryComponent5
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Microsoft.TeamFoundation.Framework.Server
{
  internal class RegistryComponent5 : RegistryComponent4
  {
    public override IEnumerable<RegistryItem> QueryRegistry(
      string registryPath,
      int depth,
      out long sequenceId)
    {
      this.PrepareStoredProcedure("prc_QueryRegistry");
      this.BindString("@registryPath", RegistryComponent.RegistryToDatabasePath(registryPath), 260, false, SqlDbType.NVarChar);
      this.BindByte("@depth", RegistryComponent4.GetSqlDepth(depth));
      ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), "prc_QueryRegistry", this.RequestContext);
      resultCollection.AddBinder<long>((ObjectBinder<long>) new RegistryComponent5.SequenceIdColumns());
      resultCollection.AddBinder<RegistryItem>((ObjectBinder<RegistryItem>) new RegistryComponent.RegistryItemColumns());
      sequenceId = resultCollection.GetCurrent<long>().FirstOrDefault<long>();
      resultCollection.NextResult();
      return (IEnumerable<RegistryItem>) resultCollection.GetCurrent<RegistryItem>();
    }

    public override long UpdateRegistry(
      string identityName,
      long currentSequenceId,
      IEnumerable<RegistryItem> entriesToUpdate,
      bool overwriteExisting,
      out IList<RegistryUpdateRecord> updateRecords)
    {
      if (!overwriteExisting)
        throw new ArgumentOutOfRangeException(nameof (overwriteExisting));
      this.PrepareStoredProcedure("prc_UpdateRegistry");
      this.BindKeyValuePairStringTableNullable("@registryUpdates", entriesToUpdate.Select<RegistryItem, KeyValuePair<string, string>>((System.Func<RegistryItem, KeyValuePair<string, string>>) (s => new KeyValuePair<string, string>(RegistryComponent.RegistryToDatabasePath(s.Path), s.Value))));
      this.BindLong("@currentSequenceId", currentSequenceId);
      if (!string.IsNullOrEmpty(identityName))
        this.BindString("@identityName", identityName, 1024, false, SqlDbType.NVarChar);
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), "prc_UpdateRegistry", this.RequestContext))
      {
        resultCollection.AddBinder<long>((ObjectBinder<long>) new RegistryComponent5.SequenceIdColumns());
        resultCollection.AddBinder<RegistryUpdateRecord>((ObjectBinder<RegistryUpdateRecord>) new RegistryComponent5.RegistryUpdateRecordColumns());
        long num = resultCollection.GetCurrent<long>().First<long>();
        resultCollection.NextResult();
        updateRecords = (IList<RegistryUpdateRecord>) resultCollection.GetCurrent<RegistryUpdateRecord>().Items;
        return num;
      }
    }

    public override void InitializeRegistrySequenceId()
    {
      this.PrepareStoredProcedure("prc_InitializeRegistrySequenceId");
      this.ExecuteNonQuery();
    }

    protected class SequenceIdColumns : ObjectBinder<long>
    {
      private SqlColumnBinder m_sequenceIdColumn = new SqlColumnBinder("SequenceId");

      protected override long Bind() => this.m_sequenceIdColumn.GetInt64((IDataReader) this.Reader);
    }

    protected class RegistryUpdateRecordColumns : ObjectBinder<RegistryUpdateRecord>
    {
      private SqlColumnBinder m_sequenceIdColumn = new SqlColumnBinder("SequenceId");
      private SqlColumnBinder m_registryPathColumn = new SqlColumnBinder("RegistryPath");
      private SqlColumnBinder m_regValueColumn = new SqlColumnBinder("RegValue");

      protected override RegistryUpdateRecord Bind() => new RegistryUpdateRecord(this.m_sequenceIdColumn.GetInt64((IDataReader) this.Reader), RegistryComponent.DatabaseToRegistryPath(this.m_registryPathColumn.GetString((IDataReader) this.Reader, false)), this.m_regValueColumn.GetString((IDataReader) this.Reader, true));
    }
  }
}
