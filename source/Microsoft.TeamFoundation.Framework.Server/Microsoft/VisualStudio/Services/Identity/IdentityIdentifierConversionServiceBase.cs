// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Identity.IdentityIdentifierConversionServiceBase
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Identity
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  internal abstract class IdentityIdentifierConversionServiceBase : 
    IIdentityIdentifierConversionService,
    IVssFrameworkService
  {
    protected const string TraceArea = "IdentityIdentifierConversion";
    protected const string TraceLayer = "Service";
    private Guid ServiceHostInstanceId;

    protected IEnumerable<IIdentityIdentifierRepository> Repositories { get; private set; }

    protected abstract IEnumerable<IIdentityIdentifierRepository> GetRepositories(
      IVssRequestContext requestContext);

    public virtual void ServiceStart(IVssRequestContext requestContext)
    {
      this.ServiceHostInstanceId = requestContext.ServiceHost.InstanceId;
      this.Repositories = this.GetRepositories(requestContext);
    }

    public virtual void ServiceEnd(IVssRequestContext requestContext)
    {
      foreach (IIdentityIdentifierRepository repository in this.Repositories)
        repository.Unload(requestContext);
    }

    protected void ValidateServiceHost(IVssRequestContext requestContext)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      requestContext.CheckServiceHostId(this.ServiceHostInstanceId, (IVssFrameworkService) this);
    }

    public IdentityDescriptor GetDescriptorByMasterId(
      IVssRequestContext requestContext,
      Guid masterId)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForEmptyGuid(masterId, nameof (masterId));
      this.ValidateServiceHost(requestContext);
      try
      {
        requestContext.TraceEnter(6307306, "IdentityIdentifierConversion", "Service", nameof (GetDescriptorByMasterId));
        requestContext.Trace(6307323, TraceLevel.Verbose, "IdentityIdentifierConversion", "Service", "Requested identityDescriptor by masterId {0}", (object) masterId);
        this.CheckPermission(requestContext);
        if (requestContext.ServiceHost.Is(TeamFoundationHostType.ProjectCollection))
        {
          IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Application);
          return vssRequestContext.GetService<IIdentityIdentifierConversionService>().GetDescriptorByMasterId(vssRequestContext, masterId);
        }
        IIdentityIdentifierRepository repositoryWithData = (IIdentityIdentifierRepository) null;
        IdentityDescriptor identityDescriptor = (IdentityDescriptor) null;
        foreach (IIdentityIdentifierRepository repository in this.Repositories)
        {
          identityDescriptor = repository.GetDescriptorByMasterId(requestContext, masterId);
          if (identityDescriptor != (IdentityDescriptor) null)
          {
            repositoryWithData = repository;
            break;
          }
        }
        if (identityDescriptor == (IdentityDescriptor) null)
          requestContext.Trace(1007072, TraceLevel.Error, "IdentityIdentifierConversion", "Service", string.Format("GetDescriptorByMasterId: identityDescriptor is null, masterId is {0}", (object) masterId));
        requestContext.Trace(6307304, TraceLevel.Verbose, "IdentityIdentifierConversion", "Service", "Received identityDescriptor by masterId {0} - Descriptor Type: [{1}] Identifier Hash: [{2}]", (object) masterId, (object) identityDescriptor?.IdentityType, (object) identityDescriptor?.GetHashCode());
        if (requestContext.IsTracing(6307305, TraceLevel.Verbose, "IdentityIdentifierConversion", "Service"))
        {
          if (repositoryWithData == null)
            requestContext.Trace(6307305, TraceLevel.Verbose, "IdentityIdentifierConversion", "Service", "Received identityDescriptor by masterId. repositoryWithData : null");
          else
            requestContext.Trace(6307305, TraceLevel.Verbose, "IdentityIdentifierConversion", "Service", "Received identityDescriptor by masterId. repositoryWithData : {0} {1}", (object) repositoryWithData.GetType(), (object) repositoryWithData.HostType);
        }
        if (identityDescriptor != (IdentityDescriptor) null)
        {
          IEnumerable<IIdentityIdentifierRepository> identifierRepositories = this.Repositories.Where<IIdentityIdentifierRepository>((Func<IIdentityIdentifierRepository, bool>) (x => x != repositoryWithData && (x.HostType & repositoryWithData.HostType) == repositoryWithData.HostType));
          requestContext.TraceSerializedConditionally(6307321, TraceLevel.Verbose, "IdentityIdentifierConversion", "Service", "Received identityDescriptor by masterId. repositoriesToNotify - {0}", (object) identifierRepositories);
          foreach (IIdentityIdentifierRepository identifierRepository in identifierRepositories)
            identifierRepository.OnDescriptorRetrievedByMasterId(requestContext, masterId, identityDescriptor);
        }
        return identityDescriptor;
      }
      finally
      {
        requestContext.TraceLeave(6307307, "IdentityIdentifierConversion", "Service", nameof (GetDescriptorByMasterId));
      }
    }

    public abstract IdentityDescriptor GetDescriptorByLocalId(
      IVssRequestContext requestContext,
      Guid localId);

    protected virtual void CheckPermission(IVssRequestContext requestContext)
    {
    }
  }
}
