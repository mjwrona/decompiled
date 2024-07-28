// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.DatabaseServicePrincipal
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System;

namespace Microsoft.TeamFoundation.Framework.Server
{
  internal class DatabaseServicePrincipal
  {
    public int PrincipalId;
    public string PrincipalName;
    public string TypeDesc;
    public string AuthenticationTypeDesc;
    public DateTime CreateDate;
    public DateTime ModifyDate;
    public string StateDesc;
    public string PermissionName;

    public override string ToString() => string.Format("principal_id:{0} principal_name: {1} type_desc:{2} authentication_type_desc:{3}create_date:{4} modify_date:{5} state_desc:{6} permission_name: {7}", (object) this.PrincipalId, (object) this.PrincipalName, (object) this.TypeDesc, (object) this.AuthenticationTypeDesc, (object) this.CreateDate, (object) this.ModifyDate, (object) this.StateDesc, (object) this.PermissionName);
  }
}
