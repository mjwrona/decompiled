// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Redis.V2.IRedisConnectionInternal
// Assembly: Microsoft.VisualStudio.Services.Cloud, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F2D48A0E-C4C9-4233-BA34-E8461823F6F2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cloud.dll

using Microsoft.TeamFoundation.Framework.Server;
using StackExchange.Redis;
using System;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.Redis.V2
{
  internal interface IRedisConnectionInternal
  {
    void Call(IVssRequestContext requestContext, Microsoft.VisualStudio.Services.Redis.Tracer tracer, Action<IRedisDatabase> action);

    void Call(IVssRequestContext requestContext, Microsoft.VisualStudio.Services.Redis.Tracer tracer, Action<IServer> action);

    void Call(IVssRequestContext requestContext, Microsoft.VisualStudio.Services.Redis.Tracer tracer, Action<IServer, IDatabase> action);

    Task CallAsync(
      IVssRequestContext requestContext,
      Microsoft.VisualStudio.Services.Redis.Tracer tracer,
      Func<IRedisDatabase, Task> action);
  }
}
