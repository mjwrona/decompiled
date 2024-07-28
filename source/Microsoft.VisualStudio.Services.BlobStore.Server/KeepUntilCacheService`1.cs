// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.BlobStore.Server.KeepUntilCacheService`1
// Assembly: Microsoft.VisualStudio.Services.BlobStore.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: D3AB5C9B-EB54-4477-A304-63BB297414A3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.BlobStore.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.BlobStore.Common;
using System;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.BlobStore.Server
{
  public class KeepUntilCacheService<T> where T : IEquatable<T>
  {
    public bool TryGetKeepUntil(
      IVssRequestContext requestContext,
      IDomainId domainId,
      T identifier,
      out DateTime keepUntil)
    {
      Guid instanceId = requestContext.ServiceHost.InstanceId;
      requestContext = requestContext.To(TeamFoundationHostType.Deployment);
      return requestContext.GetService<KeepUntilCacheService<T>.DeploymentKeepUntilCacheService>().TryGetKeepUntil(requestContext, domainId, instanceId, identifier, out keepUntil);
    }

    public void SetKeepUntil(
      IVssRequestContext requestContext,
      IDomainId domainId,
      T identifier,
      DateTime newKeepUntil)
    {
      Guid instanceId = requestContext.ServiceHost.InstanceId;
      requestContext = requestContext.To(TeamFoundationHostType.Deployment);
      requestContext.GetService<KeepUntilCacheService<T>.DeploymentKeepUntilCacheService>().SetKeepUntil(requestContext, domainId, instanceId, identifier, newKeepUntil);
    }

    public void ServiceStart(IVssRequestContext systemRequestContext)
    {
    }

    public void ServiceEnd(IVssRequestContext systemRequestContext)
    {
    }

    private struct HostIdAndIdentifier : 
      IEqualityComparer<KeepUntilCacheService<T>.HostIdAndIdentifier>
    {
      public static readonly IEqualityComparer<KeepUntilCacheService<T>.HostIdAndIdentifier> Comparer = (IEqualityComparer<KeepUntilCacheService<T>.HostIdAndIdentifier>) new KeepUntilCacheService<T>.HostIdAndIdentifier();
      private readonly T Identifier;
      private readonly Guid HostId;
      private readonly IDomainId DomainId;

      public HostIdAndIdentifier(Guid hostId, IDomainId domainId, T identifier)
      {
        this.HostId = hostId;
        this.DomainId = domainId;
        this.Identifier = identifier;
      }

      public bool Equals(
        KeepUntilCacheService<T>.HostIdAndIdentifier x,
        KeepUntilCacheService<T>.HostIdAndIdentifier y)
      {
        return x.HostId.Equals(y.HostId) && x.DomainId.Equals(y.DomainId) && x.Identifier.Equals(y.Identifier);
      }

      public int GetHashCode(KeepUntilCacheService<T>.HostIdAndIdentifier obj) => obj.HostId.GetHashCode() ^ obj.DomainId.GetHashCode() ^ obj.Identifier.GetHashCode();
    }

    private class DeploymentKeepUntilCacheService : 
      VssMemoryCacheService<KeepUntilCacheService<T>.HostIdAndIdentifier, DateTime>
    {
      private static readonly MemoryCacheConfiguration<KeepUntilCacheService<T>.HostIdAndIdentifier, DateTime> Configuration = new MemoryCacheConfiguration<KeepUntilCacheService<T>.HostIdAndIdentifier, DateTime>().WithMaxElements(1000000);

      public DeploymentKeepUntilCacheService()
        : base(KeepUntilCacheService<T>.HostIdAndIdentifier.Comparer, KeepUntilCacheService<T>.DeploymentKeepUntilCacheService.Configuration)
      {
      }

      protected override void ServiceStart(IVssRequestContext systemRequestContext)
      {
        systemRequestContext.CheckDeploymentRequestContext();
        base.ServiceStart(systemRequestContext);
      }

      public bool TryGetKeepUntil(
        IVssRequestContext deploymentRequestContext,
        IDomainId domainId,
        Guid hostId,
        T identifier,
        out DateTime keepUntil)
      {
        return this.TryGetValue(deploymentRequestContext, new KeepUntilCacheService<T>.HostIdAndIdentifier(hostId, domainId, identifier), out keepUntil);
      }

      public void SetKeepUntil(
        IVssRequestContext deploymentRequestContext,
        IDomainId domainId,
        Guid hostId,
        T identifier,
        DateTime newKeepUntil)
      {
        this.Set(deploymentRequestContext, new KeepUntilCacheService<T>.HostIdAndIdentifier(hostId, domainId, identifier), newKeepUntil);
      }
    }
  }
}
