// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.Core.EmailEventComponent2
// Assembly: Microsoft.TeamFoundation.Server.Core, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9DD3208E-87CF-4F7C-8D96-8880BDAD13B2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.Core.dll

using Microsoft.SqlServer.Server;
using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace Microsoft.TeamFoundation.Server.Core
{
  internal class EmailEventComponent2 : EmailEventComponent
  {
    private static readonly SqlMetaData[] typ_EmailConfirmationTable = new SqlMetaData[2]
    {
      new SqlMetaData("TfId", SqlDbType.UniqueIdentifier),
      new SqlMetaData("PreferredEmailAddress", SqlDbType.NVarChar, 256L)
    };

    internal override void AddPendingEmailConfirmation(Guid tfId, string preferredEmailAddress)
    {
      this.PrepareStoredProcedure("prc_AddPendingEmailConfirmation");
      this.BindGuid("@tfId", tfId);
      this.BindString("@preferredEmailAddress", preferredEmailAddress, 256, true, SqlDbType.NVarChar);
      this.ExecuteNonQuery();
    }

    internal override ResultCollection QueryEmailConfirmations()
    {
      this.PrepareStoredProcedure("prc_QueryEmailConfirmations");
      ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext);
      resultCollection.AddBinder<PreferredEmailConfirmationEntry>((ObjectBinder<PreferredEmailConfirmationEntry>) new EmailConfirmationQueueBinder());
      return resultCollection;
    }

    internal override void EmailConfirmationSent(
      IEnumerable<PreferredEmailConfirmationEntry> sentConfirmations)
    {
      this.PrepareStoredProcedure("prc_EmailConfirmationSent");
      this.BindEmailConfirmationTable("@sentConfirmations", sentConfirmations);
      this.ExecuteNonQuery();
    }

    protected SqlParameter BindEmailConfirmationTable(
      string parameterName,
      IEnumerable<PreferredEmailConfirmationEntry> rows)
    {
      rows = rows ?? Enumerable.Empty<PreferredEmailConfirmationEntry>();
      System.Func<PreferredEmailConfirmationEntry, SqlDataRecord> selector = (System.Func<PreferredEmailConfirmationEntry, SqlDataRecord>) (entry =>
      {
        SqlDataRecord sqlDataRecord = new SqlDataRecord(EmailEventComponent2.typ_EmailConfirmationTable);
        sqlDataRecord.SetGuid(0, entry.TfId);
        sqlDataRecord.SetString(1, entry.PreferredEmailAddress);
        return sqlDataRecord;
      });
      return this.BindTable(parameterName, "typ_EmailConfirmationTable", rows.Select<PreferredEmailConfirmationEntry, SqlDataRecord>(selector));
    }
  }
}
