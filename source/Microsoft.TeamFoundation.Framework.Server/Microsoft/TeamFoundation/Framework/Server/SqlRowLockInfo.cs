// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.SqlRowLockInfo
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

namespace Microsoft.TeamFoundation.Framework.Server
{
  internal class SqlRowLockInfo
  {
    public string SchemaName;
    public string TableName;
    public string IndexName;
    public long HobtId;

    public override string ToString() => string.Format("schema_name:{0} table_name: {1} index_name: {2} hobt_id: {3}", (object) this.SchemaName, (object) this.TableName, (object) this.IndexName, (object) this.HobtId);
  }
}
