// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.DatabasePrincipal
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System;

namespace Microsoft.TeamFoundation.Framework.Server
{
  internal class DatabasePrincipal
  {
    public int PrincipalId;
    public string PrincipalName;
    public string RoleName;
    public string Permissions;
    public string TypeDesc;
    public DateTime CreateDate;
    public DateTime ModifyDate;

    public override string ToString() => string.Format("principal_id:{0} principal_name: {1} role_name:{2} permissions:{3}type_desc:{4} create_date:{5} modify_date:{6}", (object) this.PrincipalId, (object) this.PrincipalName, (object) this.RoleName, (object) this.Permissions, (object) this.TypeDesc, (object) this.CreateDate, (object) this.ModifyDate);
  }
}
