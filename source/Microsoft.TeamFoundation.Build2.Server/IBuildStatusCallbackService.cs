// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build2.Server.IBuildStatusCallbackService
// Assembly: Microsoft.TeamFoundation.Build2.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 680FF5F5-CB5D-4078-8EFA-56C292BFDBB7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build2.Server.dll

using Microsoft.Azure.Pipelines.Server.ObjectModel;
using Microsoft.TeamFoundation.Framework.Server;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.Build2.Server
{
  [DefaultServiceImplementation(typeof (BuildStatusCallbackService))]
  internal interface IBuildStatusCallbackService : IVssFrameworkService
  {
    void Store(
      IVssRequestContext requestContext,
      IReadOnlyBuildData buildData,
      StatusCallbackInfo statusCallbackInfo);

    void Store(
      IVssRequestContext requestContext,
      int buildId,
      StatusCallbackInfo statusCallbackInfo);

    bool TryGet(
      IVssRequestContext requestContext,
      IReadOnlyBuildData buildData,
      out StatusCallbackInfo statusCallbackInfo);

    bool TryGet(
      IVssRequestContext requestContext,
      int buildId,
      out StatusCallbackInfo statusCallbackInfo);

    void Delete(IVssRequestContext requestContext, IReadOnlyBuildData buildData);

    void Delete(IVssRequestContext requestContext, IEnumerable<IReadOnlyBuildData> builds);

    void Delete(IVssRequestContext requestContext, IEnumerable<int> buildIds);
  }
}
