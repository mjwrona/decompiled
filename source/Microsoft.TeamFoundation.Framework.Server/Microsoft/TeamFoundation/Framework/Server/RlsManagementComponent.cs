// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.RlsManagementComponent
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System;
using System.Data;

namespace Microsoft.TeamFoundation.Framework.Server
{
  internal class RlsManagementComponent : TeamFoundationSqlResourceComponent
  {
    public static readonly ComponentFactory ComponentFactory = new ComponentFactory(new IComponentCreator[3]
    {
      (IComponentCreator) new ComponentCreator<RlsManagementComponent>(1, true),
      (IComponentCreator) new ComponentCreator<RlsManagementComponent2>(2),
      (IComponentCreator) new ComponentCreator<RlsManagementComponent3>(3)
    }, "RlsManagement");

    public RlsManagementComponent() => this.SelectedFeatures &= ~SqlResourceComponentFeatures.BindPartitionIdByDefault;

    public bool IsRlsSupported()
    {
      this.PrepareStoredProcedure("prc_IsRlsSupported");
      return (bool) this.ExecuteScalar();
    }

    public virtual bool IsRlsEnabled() => throw new NotImplementedException("IsRlsEnabled in not implemented in " + this.GetType().Name + " was first implemented RlsManagementComponent3");

    public virtual void EnableRls(RlsOptions options)
    {
      this.PrepareStoredProcedure("prc_EnableRls");
      this.ExecuteNonQuery();
    }

    public void DisableRls()
    {
      this.PrepareStoredProcedure("prc_DisableRls");
      this.BindString("@schemaName", "%", 128, BindStringBehavior.Unchanged, SqlDbType.NVarChar);
      this.BindString("@tableName", "%", 128, BindStringBehavior.Unchanged, SqlDbType.NVarChar);
      this.ExecuteNonQuery();
    }
  }
}
