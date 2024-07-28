// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Elastic.Controllers.ElasticApiController
// Assembly: Microsoft.TeamFoundation.DistributedTask.Elastic, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6202E83A-3164-4101-8FDA-8C4FB25E62EC
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.DistributedTask.Elastic.dll

using Microsoft.TeamFoundation.DistributedTask.Server.Controllers;
using Microsoft.TeamFoundation.Framework.Server;

namespace Microsoft.TeamFoundation.DistributedTask.Elastic.Controllers
{
  public abstract class ElasticApiController : DistributedTaskApiController
  {
    public IElasticPoolService ElasticPoolService => this.TfsRequestContext.GetService<IElasticPoolService>();

    public IElasticPoolLogService ElasticPoolLogService => this.TfsRequestContext.GetService<IElasticPoolLogService>();

    public IElasticNodeService ElasticNodeService => this.TfsRequestContext.GetService<IElasticNodeService>();

    protected override void InitializeExceptionMap(ApiExceptionMapping exceptionMap)
    {
      base.InitializeExceptionMap(exceptionMap);
      ElasticExceptionMapper.Map(exceptionMap);
    }
  }
}
