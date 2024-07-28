// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.DatabaseRoles
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

namespace Microsoft.TeamFoundation.Framework.Server
{
  public static class DatabaseRoles
  {
    public static readonly string DbOwner = "db_owner";
    public static readonly string TfsAdminRole = "TFSADMINROLE";
    public static readonly string VsoDboRole = "VSODBOROLE";
    public static readonly string TfsExecRole = "TFSEXECROLE";
    public static readonly string TfsBuildExecRole = "TFSBUILDEXECROLE";
    public static readonly string TfsReaderRole = "TFSREADERROLE";
    public static readonly string VsoDiagRole = "VSODIAGROLE";
    internal static readonly string TfsWarehouseDataReader = nameof (TfsWarehouseDataReader);
    internal static readonly string TfsWarehouseAdministrator = nameof (TfsWarehouseAdministrator);
  }
}
