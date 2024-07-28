// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Content.Server.Azure.ASTableComponent2
// Assembly: Microsoft.VisualStudio.Services.Content.Server.Azure, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7823E4AE-BEB6-4A7C-9914-276DEAE1FB1F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Content.Server.Azure.dll

using Microsoft.TeamFoundation.Framework.Server;
using System.Collections.Generic;
using System.Data;

namespace Microsoft.VisualStudio.Services.Content.Server.Azure
{
  internal class ASTableComponent2 : ASTableComponent
  {
    public override List<string> ListPrimaryKeys(int total, string pkMinExclusive)
    {
      SqlColumnBinder pkColumn = new SqlColumnBinder("PartitionKey");
      this.PrepareStoredProcedure("ASTable.prc_ListPartitionKeys");
      this.BindArgumentsForPKListing(total, pkMinExclusive);
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), "ASTable.prc_ListPartitionKeys", this.RequestContext))
      {
        resultCollection.AddBinder<string>((ObjectBinder<string>) new SimpleObjectBinder<string>((System.Func<IDataReader, string>) (reader => pkColumn.GetString(reader, false))));
        return resultCollection.GetCurrent<string>().Items;
      }
    }

    internal List<string> ListPrimaryKeysUsingStmt(int total, string pkMinExclusive) => base.ListPrimaryKeys(total, pkMinExclusive);
  }
}
