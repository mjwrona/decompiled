// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Pipelines.Server.IYamlCacheService
// Assembly: Microsoft.TeamFoundation.DistributedTask.Orchestration.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07FD5059-3D25-415E-AA3A-5372051D7E71
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.dll

using Microsoft.TeamFoundation.Framework.Server;

namespace Microsoft.TeamFoundation.DistributedTask.Pipelines.Server
{
  [DefaultServiceImplementation(typeof (YamlCacheService))]
  internal interface IYamlCacheService : IVssFrameworkService
  {
    void Set(IVssRequestContext requestContext, string key, string value);

    bool TryGetValue(IVssRequestContext requestContext, string key, out string value);
  }
}
