// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Server.DataAccess.HostedParallelismComponent
// Assembly: Microsoft.TeamFoundation.DistributedTask.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: EDA28B25-3B93-49F7-A194-C85AAF98B83A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace Microsoft.TeamFoundation.DistributedTask.Server.DataAccess
{
  internal class HostedParallelismComponent : TaskSqlComponentBase
  {
    public static readonly ComponentFactory ComponentFactory = new ComponentFactory(new IComponentCreator[1]
    {
      (IComponentCreator) new ComponentCreator<HostedParallelismComponent>(1)
    }, "DistributedTaskHostedParallelismService", "DistributedTask");
    protected const int MaxDescriptionLength = 256;

    public HostedParallelismComponent() => this.SelectedFeatures |= SqlResourceComponentFeatures.BindPartitionIdByDefault;

    public virtual async Task<HostedParallelism> GetHostedParallelismAsync()
    {
      HostedParallelismComponent component = this;
      HostedParallelism parallelismAsync;
      using (new TaskSqlComponentBase.SqlMethodScope((TaskSqlComponentBase) component, nameof (GetHostedParallelismAsync)))
      {
        component.PrepareStoredProcedure("Task.prc_GetHostedParallelism");
        using (ResultCollection resultCollection = new ResultCollection((IDataReader) await component.ExecuteReaderAsync(), component.ProcedureName, component.RequestContext))
        {
          resultCollection.AddBinder<HostedParallelism>(component.GetHostedParallelismBinder());
          parallelismAsync = resultCollection.GetCurrent<HostedParallelism>().Items.FirstOrDefault<HostedParallelism>();
        }
      }
      return parallelismAsync;
    }

    public virtual async Task<HostedParallelism> UpdateHostedParallelismAsync(
      HostedParallelismLevel level,
      HostedParallelismSource source,
      string description)
    {
      HostedParallelismComponent component = this;
      HostedParallelism hostedParallelism;
      using (new TaskSqlComponentBase.SqlMethodScope((TaskSqlComponentBase) component, nameof (UpdateHostedParallelismAsync)))
      {
        component.PrepareStoredProcedure("Task.prc_UpdateHostedParallelism");
        component.BindByte("@level", (byte) level);
        component.BindByte("@source", (byte) source);
        component.BindString("@description", description, 256, BindStringBehavior.EmptyStringToNull, SqlDbType.NVarChar);
        using (ResultCollection resultCollection = new ResultCollection((IDataReader) await component.ExecuteReaderAsync(), component.ProcedureName, component.RequestContext))
        {
          resultCollection.AddBinder<HostedParallelism>(component.GetHostedParallelismBinder());
          hostedParallelism = resultCollection.GetCurrent<HostedParallelism>().Items.FirstOrDefault<HostedParallelism>();
        }
      }
      return hostedParallelism;
    }

    protected virtual ObjectBinder<HostedParallelism> GetHostedParallelismBinder() => (ObjectBinder<HostedParallelism>) new HostedParallelismBinder();
  }
}
