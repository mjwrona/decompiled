// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Directories.DirectoryService.Components.IdentityDirectoryWrapperService`1
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Identity;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Directories.DirectoryService.Components
{
  public abstract class IdentityDirectoryWrapperService<TIdentity> : IVssFrameworkService where TIdentity : IVssIdentity
  {
    public virtual IdentityDirectoryEntityResult<TIdentity> AddMember(
      IVssRequestContext requestContext,
      string member,
      string profile = null,
      string license = null,
      IEnumerable<string> localGroups = null,
      IEnumerable<string> propertiesToReturn = null)
    {
      DirectoryEntityResult entityResult = requestContext.GetService<IDirectoryService>().AddMember(requestContext, member, profile, license, localGroups, IdentityDirectoryWrapperService<TIdentity>.ExtendPropertiesToReturn(propertiesToReturn));
      return this.ExtendWithIdentity(requestContext, entityResult);
    }

    public virtual IdentityDirectoryEntityResult<TIdentity> AddMember(
      IVssRequestContext requestContext,
      IDirectoryEntityDescriptor member,
      string profile = null,
      string license = null,
      IEnumerable<string> localGroups = null,
      IEnumerable<string> propertiesToReturn = null)
    {
      DirectoryEntityResult entityResult = requestContext.GetService<IDirectoryService>().AddMember(requestContext, member, profile, license, localGroups, IdentityDirectoryWrapperService<TIdentity>.ExtendPropertiesToReturn(propertiesToReturn));
      return this.ExtendWithIdentity(requestContext, entityResult);
    }

    public virtual IReadOnlyList<IdentityDirectoryEntityResult<TIdentity>> AddMembers(
      IVssRequestContext requestContext,
      IReadOnlyList<IDirectoryEntityDescriptor> members,
      string profile = null,
      string license = null,
      IEnumerable<string> localGroups = null,
      IEnumerable<string> propertiesToReturn = null)
    {
      IReadOnlyList<DirectoryEntityResult> entityResults = requestContext.GetService<IDirectoryService>().AddMembers(requestContext, members, profile, license, localGroups, IdentityDirectoryWrapperService<TIdentity>.ExtendPropertiesToReturn(propertiesToReturn));
      return this.ExtendWithIdentities(requestContext, entityResults);
    }

    public virtual void ServiceStart(IVssRequestContext systemRequestContext)
    {
    }

    public virtual void ServiceEnd(IVssRequestContext systemRequestContext)
    {
    }

    private static IEnumerable<string> ExtendPropertiesToReturn(
      IEnumerable<string> propertiesToReturn)
    {
      HashSet<string> stringSet = new HashSet<string>(DirectoryEntityExtensions.PropertiesForApplyToIdentity);
      if (propertiesToReturn != null)
        stringSet.UnionWith(propertiesToReturn);
      return (IEnumerable<string>) stringSet;
    }

    protected abstract IList<TIdentity> GetIdentities(
      IVssRequestContext requestContext,
      IList<Guid> identityIds);

    private IReadOnlyList<IdentityDirectoryEntityResult<TIdentity>> ExtendWithIdentities(
      IVssRequestContext requestContext,
      IReadOnlyList<DirectoryEntityResult> entityResults)
    {
      ArgumentUtility.CheckForNull<IReadOnlyList<DirectoryEntityResult>>(entityResults, nameof (entityResults));
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      Guid[] array = entityResults.Select<DirectoryEntityResult, Guid>(IdentityDirectoryWrapperService<TIdentity>.\u003C\u003EO.\u003C0\u003E__GetIdentityId ?? (IdentityDirectoryWrapperService<TIdentity>.\u003C\u003EO.\u003C0\u003E__GetIdentityId = new Func<DirectoryEntityResult, Guid>(IdentityDirectoryWrapperService<TIdentity>.GetIdentityId))).ToArray<Guid>();
      IList<TIdentity> identities = this.GetIdentities(requestContext, (IList<Guid>) array);
      if (identities == null)
        throw new IdentityDirectoryWrapperReadFailedException(IdentityTracing.FormatWithSafeSerialization("Failed to read identities corresponding to entity results => {0}", (object) entityResults));
      if (identities.Count != entityResults.Count)
        throw new IdentityDirectoryWrapperReadCountMismatchException(IdentityTracing.FormatWithSafeSerialization("Found unequal number of identities corresponding to entity results => {0}, identities => {1}", (object) entityResults, (object) identities));
      return (IReadOnlyList<IdentityDirectoryEntityResult<TIdentity>>) entityResults.Zip<DirectoryEntityResult, TIdentity, IdentityDirectoryEntityResult<TIdentity>>((IEnumerable<TIdentity>) identities, (Func<DirectoryEntityResult, TIdentity, IdentityDirectoryEntityResult<TIdentity>>) ((r, i) => IdentityDirectoryWrapperService<TIdentity>.ExtendWithIdentity(r, i, requestContext.RequestTracer))).ToList<IdentityDirectoryEntityResult<TIdentity>>();
    }

    private IdentityDirectoryEntityResult<TIdentity> ExtendWithIdentity(
      IVssRequestContext requestContext,
      DirectoryEntityResult entityResult)
    {
      ArgumentUtility.CheckForNull<DirectoryEntityResult>(entityResult, nameof (entityResult));
      return this.ExtendWithIdentities(requestContext, (IReadOnlyList<DirectoryEntityResult>) new DirectoryEntityResult[1]
      {
        entityResult
      }).Single<IdentityDirectoryEntityResult<TIdentity>>();
    }

    private static IdentityDirectoryEntityResult<TIdentity> ExtendWithIdentity(
      DirectoryEntityResult entityResult,
      TIdentity identity,
      ITraceRequest tracer)
    {
      ArgumentUtility.CheckForNull<DirectoryEntityResult>(entityResult, nameof (entityResult));
      IDirectoryEntity entity = entityResult.Entity;
      if (entity != null)
        entity.TryApplyToIdentity((IVssIdentity) identity, tracer);
      return new IdentityDirectoryEntityResult<TIdentity>(entityResult, identity);
    }

    private static Guid GetIdentityId(DirectoryEntityResult entityResult)
    {
      ArgumentUtility.CheckForNull<DirectoryEntityResult>(entityResult, nameof (entityResult));
      return IdentityDirectoryWrapperService<TIdentity>.GetIdentityId(entityResult.Entity?.LocalId);
    }

    private static Guid GetIdentityId(string localId)
    {
      if (localId == null)
        return Guid.Empty;
      Guid result = new Guid();
      return !Guid.TryParse(localId, out result) ? Guid.Empty : result;
    }
  }
}
