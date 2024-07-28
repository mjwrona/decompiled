// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Identity.IdentityDomain
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Common;
using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using System;

namespace Microsoft.VisualStudio.Services.Identity
{
  public class IdentityDomain
  {
    public readonly IdentityMapper IdentityMapper;
    private readonly string m_toString;
    private object m_identityStore;

    public IdentityDomain(IVssRequestContext requestContext)
      : this(requestContext, (IdentityDomain) null)
    {
    }

    public IdentityDomain(IVssRequestContext requestContext, IdentityDomain parent)
      : this(requestContext.ServiceHost.InstanceId, requestContext.ServiceHost.Name, requestContext.ServiceHost.Is(TeamFoundationHostType.Application))
    {
      this.Parent = parent;
      if (requestContext.ServiceHost.Is(TeamFoundationHostType.Deployment))
        this.HostLevel = TeamFoundationHostType.Deployment;
      else
        this.HostLevel = requestContext.ServiceHost.HostType;
    }

    internal IdentityDomain(Guid domainId, string name = null, bool isMaster = false)
    {
      this.Parent = (IdentityDomain) null;
      this.HostLevel = TeamFoundationHostType.Unknown;
      this.DomainId = domainId;
      this.IdentityMapper = new IdentityMapper(domainId);
      this.GlobalGroupScope = TFCommonUtil.GetIdentityDomainScope(this.DomainId);
      this.Name = name;
      this.DomainRoot = this.IdentityMapper.MapFromWellKnownIdentifier(GroupWellKnownIdentityDescriptors.EveryoneGroup);
      this.DomainAdmin = this.IdentityMapper.MapFromWellKnownIdentifier(GroupWellKnownIdentityDescriptors.NamespaceAdministratorsGroup);
      this.IsMaster = isMaster;
      if (this.Name != null)
        this.m_toString = this.DomainId.ToString() + "-" + this.Name;
      else
        this.m_toString = this.DomainId.ToString();
    }

    public IdentityDomain Parent { get; private set; }

    public TeamFoundationHostType HostLevel { get; private set; }

    public bool IsMaster { get; private set; }

    internal object IdentityStore
    {
      get => this.m_identityStore;
      set => this.m_identityStore = value;
    }

    public bool IsOwner(IdentityDescriptor descriptor)
    {
      IdentityValidation.CheckDescriptor(descriptor, nameof (descriptor));
      if (!string.Equals(descriptor.IdentityType, "Microsoft.TeamFoundation.Identity", StringComparison.OrdinalIgnoreCase))
        return false;
      return descriptor.Identifier.StartsWith(this.DomainSid, StringComparison.OrdinalIgnoreCase) || descriptor.Identifier.StartsWith(SidIdentityHelper.WellKnownSidPrefix, StringComparison.OrdinalIgnoreCase);
    }

    internal Guid DomainId { get; }

    internal string DomainSid => this.IdentityMapper.DomainSid;

    internal IdentityDescriptor DomainRoot { get; private set; }

    internal IdentityDescriptor DomainAdmin { get; private set; }

    public string GlobalGroupScope { get; }

    internal string Name { get; }

    public static IdentityDescriptor MapFromWellKnownIdentifier(
      Guid scopeId,
      IdentityDescriptor descriptor)
    {
      return IdentityMapper.MapFromWellKnownIdentifier(descriptor, scopeId);
    }

    public static SubjectDescriptor MapToWellKnownIdentifier(
      Guid scopeId,
      SubjectDescriptor descriptor)
    {
      return IdentityMapper.MapToWellKnownIdentifier(descriptor, scopeId);
    }

    internal IdentityDescriptor MapFromWellKnownIdentifier(IdentityDescriptor descriptor) => this.IdentityMapper.MapFromWellKnownIdentifier(descriptor);

    internal SubjectDescriptor MapFromWellKnownIdentifier(
      IVssRequestContext requestContext,
      SubjectDescriptor subjectDescriptor)
    {
      return this.IdentityMapper.MapFromWellKnownIdentifier(requestContext, subjectDescriptor);
    }

    internal string MapFromWellKnownIdentifier(string identifier) => this.IdentityMapper.MapFromWellKnownIdentifier(identifier);

    internal IdentityDescriptor MapToWellKnownIdentifier(IdentityDescriptor descriptor) => this.IdentityMapper.MapToWellKnownIdentifier(descriptor);

    internal string MapToWellKnownIdentifier(string identifier) => this.IdentityMapper.MapToWellKnownIdentifier(identifier);

    internal bool MapIfSidFromDifferentHost(string sid, string otherHostSid, out string result)
    {
      bool flag;
      if (sid.StartsWith(otherHostSid, StringComparison.OrdinalIgnoreCase))
      {
        result = this.DomainSid + sid.Substring(otherHostSid.Length);
        flag = true;
      }
      else
      {
        result = sid;
        flag = false;
      }
      return flag;
    }

    public override string ToString() => this.m_toString;
  }
}
