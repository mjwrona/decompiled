// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.VersionControl.Server.VersionControlObjectBinder`1
// Assembly: Microsoft.TeamFoundation.VersionControl.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: C5D9EE74-8805-4E00-959F-39760D2358B5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.VersionControl.Server.dll

using Microsoft.TeamFoundation.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Identity;
using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.VersionControl.Server
{
  internal abstract class VersionControlObjectBinder<T> : ObjectBinder<T>
  {
    private static readonly byte[] s_nullHash = new byte[16];
    protected VersionControlSqlResourceComponent m_component;

    public VersionControlObjectBinder() => this.m_component = (VersionControlSqlResourceComponent) null;

    public VersionControlObjectBinder(VersionControlSqlResourceComponent component) => this.m_component = component;

    protected Guid GetDataspaceIdentifier(int dataspaceId) => dataspaceId == 0 ? Guid.Empty : this.m_component.GetDataspaceIdentifier(dataspaceId);

    protected string GetServerItemProjectNamePath(string serverItemProjectPath) => this.m_component.ConvertToPathWithProjectName(serverItemProjectPath);

    protected string BestEffortGetServerItemProjectNamePath(string serverItemProjectIdPath) => this.m_component.BestEffortConvertToPathWithProjectName(serverItemProjectIdPath);

    protected ItemPathPair GetPreDataspaceItemPathPair(string itemProjectNamePath) => ItemPathPair.FromServerItem(itemProjectNamePath);

    protected ItemPathPair GetItemPathPair(string itemProjectGuidPath) => new ItemPathPair(this.m_component.ConvertToPathWithProjectName(itemProjectGuidPath), itemProjectGuidPath);

    protected ItemPathPair BestEffortGetItemPathPair(string itemProjectGuidPath) => new ItemPathPair(this.m_component.BestEffortConvertToPathWithProjectName(itemProjectGuidPath), itemProjectGuidPath);

    protected static Microsoft.VisualStudio.Services.Identity.Identity GetIdentity(
      IVssRequestContext requestContext,
      IdentityService identityService,
      Guid vsid)
    {
      if (vsid.Equals(Guid.Empty))
        return (Microsoft.VisualStudio.Services.Identity.Identity) null;
      return identityService.ReadIdentities(requestContext, (IList<Guid>) new List<Guid>()
      {
        vsid
      }, QueryMembership.None, (IEnumerable<string>) null)[0];
    }

    protected static string GetIdentityDisplayName(
      IVssRequestContext requestContext,
      IdentityService identityService,
      Guid teamFoundationId)
    {
      Microsoft.VisualStudio.Services.Identity.Identity identity = VersionControlObjectBinder<T>.GetIdentity(requestContext, identityService, teamFoundationId);
      return identity == null ? teamFoundationId.ToString() : identity.DisplayName;
    }

    protected static byte[] NormalizeHashValue(byte[] hashValue) => hashValue == null || hashValue.Length == 0 || ArrayUtil.Equals(hashValue, VersionControlObjectBinder<T>.s_nullHash) ? (byte[]) null : hashValue;
  }
}
