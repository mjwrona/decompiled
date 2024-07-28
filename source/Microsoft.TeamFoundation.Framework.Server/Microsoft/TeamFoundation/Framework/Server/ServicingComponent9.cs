// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.ServicingComponent9
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System.Data;
using System.Linq;

namespace Microsoft.TeamFoundation.Framework.Server
{
  internal class ServicingComponent9 : ServicingComponent8
  {
    public override void StartServicingDeployment(string serviceLevels)
    {
      this.PrepareStoredProcedure("prc_StartServicingDeployment");
      this.BindString("@serviceLevel", serviceLevels, (int) byte.MaxValue, false, SqlDbType.VarChar);
      this.ExecuteNonQuery();
    }

    public override void FinishServicingDeployment(string serviceLevels)
    {
      this.PrepareStoredProcedure("prc_FinishServicingDeployment");
      this.BindString("@serviceLevel", serviceLevels, (int) byte.MaxValue, false, SqlDbType.VarChar);
      this.ExecuteNonQuery();
    }

    public override ServicingDeploymentInfo GetServicingDeploymentInfo(string serviceLevels)
    {
      this.PrepareStoredProcedure("prc_GetServicingDeploymentInfo");
      this.BindString("@serviceLevel", serviceLevels, (int) byte.MaxValue, false, SqlDbType.VarChar);
      ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext);
      resultCollection.AddBinder<ServicingDeploymentInfo>((ObjectBinder<ServicingDeploymentInfo>) new ServicingDeploymentInfoColumns());
      return resultCollection.GetCurrent<ServicingDeploymentInfo>().SingleOrDefault<ServicingDeploymentInfo>();
    }

    public override bool BindOperationTarget() => true;

    public override ServicingOperationColumns GetServicingOperationBinder() => (ServicingOperationColumns) new ServicingOperationColumns2();
  }
}
