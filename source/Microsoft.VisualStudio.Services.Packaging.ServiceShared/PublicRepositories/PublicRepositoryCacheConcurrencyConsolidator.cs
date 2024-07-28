// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.PublicRepositories.PublicRepositoryCacheConcurrencyConsolidator
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Utils;
using System;


#nullable enable
namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared.PublicRepositories
{
  public static class PublicRepositoryCacheConcurrencyConsolidator
  {
    public static IConcurrencyConsolidator<PublicRepoUpdateConcurrencyKey<TCursor>, TDoc> Bootstrap<TCursor, TDoc>(
      IVssRequestContext deploymentContext,
      Guid key)
    {
      deploymentContext.CheckDeploymentRequestContext();
      return (IConcurrencyConsolidator<PublicRepoUpdateConcurrencyKey<TCursor>, TDoc>) ServiceHostLevelSharedObjectHolder.Get<ConcurrencyConsolidator<PublicRepoUpdateConcurrencyKey<TCursor>, TDoc>>(deploymentContext, key, (Func<Guid, ConcurrencyConsolidator<PublicRepoUpdateConcurrencyKey<TCursor>, TDoc>>) (_ => new ConcurrencyConsolidator<PublicRepoUpdateConcurrencyKey<TCursor>, TDoc>(false, 1)));
    }
  }
}
