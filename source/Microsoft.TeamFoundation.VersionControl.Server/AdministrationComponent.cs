// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.VersionControl.Server.AdministrationComponent
// Assembly: Microsoft.TeamFoundation.VersionControl.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: C5D9EE74-8805-4E00-959F-39760D2358B5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.VersionControl.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System.Data;

namespace Microsoft.TeamFoundation.VersionControl.Server
{
  internal class AdministrationComponent : VersionControlSqlResourceComponent
  {
    public static readonly ComponentFactory ComponentFactory = new ComponentFactory(new IComponentCreator[2]
    {
      (IComponentCreator) new ComponentCreator<AdministrationComponent>(1, true),
      (IComponentCreator) new ComponentCreator<AdministrationComponent2>(2)
    }, "VCAdministration");

    public AdminRepositoryInfo QueryRepositoryInformation()
    {
      this.PrepareStoredProcedure("prc_QueryRepositoryInformation");
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.MaxItemsPerRequest, this.ProcedureName, this.m_resultExHandler, this.RequestContext))
      {
        resultCollection.AddBinder<AdminRepositoryInfo>((ObjectBinder<AdminRepositoryInfo>) new AdminRepositoryInfoColumns());
        ObjectBinder<AdminRepositoryInfo> current = resultCollection.GetCurrent<AdminRepositoryInfo>();
        current.MoveNext();
        return current.Current;
      }
    }

    public void OptimizeDatabase()
    {
      this.PrepareStoredProcedure("prc_DeleteUnusedContent", 3600);
      this.ExecuteNonQuery();
    }
  }
}
