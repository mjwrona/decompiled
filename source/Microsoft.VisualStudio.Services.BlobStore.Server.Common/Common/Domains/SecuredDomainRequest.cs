// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.BlobStore.Server.Common.Domains.SecuredDomainRequest
// Assembly: Microsoft.VisualStudio.Services.BlobStore.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: CB48D0BF-32A2-483C-A1D4-2F10DEBB3D56
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.BlobStore.Server.Common.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.BlobStore.Common;
using System;

namespace Microsoft.VisualStudio.Services.BlobStore.Server.Common.Domains
{
  public class SecuredDomainRequest : ISecuredDomainRequest, IEquatable<ISecuredDomainRequest>
  {
    public IDomainId DomainId { get; }

    public DomainRole Role { get; }

    public SecuredDomainRequest(IVssRequestContext requestContext, IDomainId domainId)
      : this((IDomainSecurityValidator) new DomainSecurityValidator(requestContext), domainId)
    {
    }

    public SecuredDomainRequest(IDomainSecurityValidator validator, IDomainId domainId)
    {
      validator.AssertWritePermissionToDomain(domainId);
      this.DomainId = domainId;
      this.Role = DomainRole.Writer;
    }

    public bool Equals(ISecuredDomainRequest other)
    {
      if (!this.DomainId.Equals(other?.DomainId))
        return false;
      int role1 = (int) this.Role;
      DomainRole? role2 = other?.Role;
      int valueOrDefault = (int) role2.GetValueOrDefault();
      return role1 == valueOrDefault & role2.HasValue;
    }

    public override bool Equals(object obj) => this.Equals(obj as ISecuredDomainRequest);

    public override int GetHashCode() => this.ToString().GetHashCode();

    public override string ToString() => string.Format("{0}, {1}", (object) this.DomainId.Serialize(), (object) this.Role);
  }
}
