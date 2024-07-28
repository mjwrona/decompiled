// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Elastic.IVssRequestContextExtensions
// Assembly: Microsoft.TeamFoundation.DistributedTask.Elastic, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6202E83A-3164-4101-8FDA-8C4FB25E62EC
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.DistributedTask.Elastic.dll

using Microsoft.TeamFoundation.DistributedTask.Server.DataAccess;
using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Threading.Tasks;

namespace Microsoft.TeamFoundation.DistributedTask.Elastic
{
  public static class IVssRequestContextExtensions
  {
    internal static async Task UsingElasticPoolComponent(
      this IVssRequestContext requestContext,
      Func<ElasticPoolComponent, Task> method)
    {
      using (ElasticPoolComponent component = requestContext.CreateComponent<ElasticPoolComponent>())
        await method(component);
    }

    internal static async Task<TResult> UsingElasticPoolComponent<TResult>(
      this IVssRequestContext requestContext,
      Func<ElasticPoolComponent, Task<TResult>> method)
    {
      TResult result;
      using (ElasticPoolComponent component = requestContext.CreateComponent<ElasticPoolComponent>())
        result = await method(component);
      return result;
    }
  }
}
