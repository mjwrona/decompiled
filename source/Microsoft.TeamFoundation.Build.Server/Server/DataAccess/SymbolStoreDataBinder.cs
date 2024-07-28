// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build.Server.DataAccess.SymbolStoreDataBinder
// Assembly: Microsoft.TeamFoundation.Build.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 50E8BB1D-C69C-4DD2-83BE-A8FFBFFA6298
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System.Data;

namespace Microsoft.TeamFoundation.Build.Server.DataAccess
{
  internal sealed class SymbolStoreDataBinder : BuildObjectBinder<SymbolStoreData>
  {
    private SqlColumnBinder buildUri = new SqlColumnBinder("BuildUri");
    private SqlColumnBinder storePath = new SqlColumnBinder("StorePath");
    private SqlColumnBinder transactionId = new SqlColumnBinder("TransactionId");

    protected override SymbolStoreData Bind() => new SymbolStoreData()
    {
      BuildUri = this.buildUri.GetArtifactUriFromString(this.Reader, "Build", false),
      StorePath = this.storePath.GetString((IDataReader) this.Reader, false),
      TransactionId = this.transactionId.GetString((IDataReader) this.Reader, false)
    };
  }
}
