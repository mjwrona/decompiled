// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Identity.IdentityMapper
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Graph;
using System;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Identity
{
  public struct IdentityMapper
  {
    public readonly string DomainSid;

    public IdentityMapper(Guid domainId) => this.DomainSid = IdentityMapper.GetDomainSid(domainId);

    public static string GetDomainSid(Guid domainId) => WellKnownIdentifierMapper.GetDomainSid(domainId);

    public void MapFromWellKnownIdentifier(Microsoft.VisualStudio.Services.Identity.Identity identity)
    {
      if (identity == null)
        return;
      identity.Descriptor = this.MapFromWellKnownIdentifier(identity.Descriptor);
    }

    public void MapFromWellKnownIdentifiers(IEnumerable<Microsoft.VisualStudio.Services.Identity.Identity> identities)
    {
      if (identities == null)
        return;
      foreach (Microsoft.VisualStudio.Services.Identity.Identity identity in identities)
        this.MapFromWellKnownIdentifier(identity);
    }

    public SubjectDescriptor MapFromWellKnownIdentifier(
      IVssRequestContext requestContext,
      SubjectDescriptor subjectDescriptor)
    {
      if (subjectDescriptor.SubjectType == "vssgp")
      {
        string identifier = this.MapFromWellKnownIdentifier(subjectDescriptor.Identifier);
        if ((object) subjectDescriptor.Identifier != (object) identifier)
          subjectDescriptor = new SubjectDescriptor(subjectDescriptor.SubjectType, identifier);
      }
      return subjectDescriptor;
    }

    public SubjectDescriptor MapToWellKnownIdentifier(SubjectDescriptor subjectDescriptor)
    {
      if (subjectDescriptor.SubjectType == "vssgp")
      {
        string wellKnownIdentifier = this.MapToWellKnownIdentifier(subjectDescriptor.Identifier);
        if ((object) subjectDescriptor.Identifier != (object) wellKnownIdentifier)
          subjectDescriptor = new SubjectDescriptor(subjectDescriptor.SubjectType, wellKnownIdentifier);
      }
      return subjectDescriptor;
    }

    public IdentityDescriptor MapFromWellKnownIdentifier(IdentityDescriptor descriptor)
    {
      if (descriptor.IsTeamFoundationType())
      {
        string identifier = this.MapFromWellKnownIdentifier(descriptor.Identifier);
        if ((object) descriptor.Identifier != (object) identifier)
          descriptor = new IdentityDescriptor("Microsoft.TeamFoundation.Identity", identifier);
      }
      return descriptor;
    }

    public string MapFromWellKnownIdentifier(string identifier) => WellKnownIdentifierMapper.MapFromWellKnownIdentifier(identifier, this.DomainSid);

    public void MapToWellKnownIdentifier(Microsoft.VisualStudio.Services.Identity.Identity identity)
    {
      if (identity == null)
        return;
      identity.Descriptor = this.MapToWellKnownIdentifier(identity.Descriptor);
    }

    public void MapToWellKnownIdentifiers(IEnumerable<Microsoft.VisualStudio.Services.Identity.Identity> identities)
    {
      if (identities == null)
        return;
      foreach (Microsoft.VisualStudio.Services.Identity.Identity identity in identities)
        this.MapToWellKnownIdentifier(identity);
    }

    public IdentityDescriptor MapToWellKnownIdentifier(IdentityDescriptor descriptor)
    {
      if (string.Equals(descriptor.IdentityType, "Microsoft.TeamFoundation.Identity", StringComparison.OrdinalIgnoreCase))
      {
        string wellKnownIdentifier = this.MapToWellKnownIdentifier(descriptor.Identifier);
        if ((object) descriptor.Identifier != (object) wellKnownIdentifier)
          descriptor = new IdentityDescriptor("Microsoft.TeamFoundation.Identity", wellKnownIdentifier);
      }
      return descriptor;
    }

    public string MapToWellKnownIdentifier(string identifier)
    {
      if (identifier.StartsWith(this.DomainSid, StringComparison.OrdinalIgnoreCase) && identifier.Substring(this.DomainSid.Length).StartsWith(SidIdentityHelper.WellKnownSidType, StringComparison.Ordinal))
        identifier = SidIdentityHelper.WellKnownDomainSid + identifier.Substring(this.DomainSid.Length);
      return identifier;
    }

    public static void MapFromWellKnownIdentifier(Microsoft.VisualStudio.Services.Identity.Identity identity, Guid domainId) => new IdentityMapper(domainId).MapFromWellKnownIdentifier(identity);

    public static void MapFromWellKnownIdentifiers(IEnumerable<Microsoft.VisualStudio.Services.Identity.Identity> identities, Guid domainId) => new IdentityMapper(domainId).MapFromWellKnownIdentifiers(identities);

    public static IdentityDescriptor MapFromWellKnownIdentifier(
      IdentityDescriptor descriptor,
      Guid domainId)
    {
      return new IdentityMapper(domainId).MapFromWellKnownIdentifier(descriptor);
    }

    public static string MapFromWellKnownIdentifier(string identifier, Guid domainId) => new IdentityMapper(domainId).MapFromWellKnownIdentifier(identifier);

    public static void MapToWellKnownIdentifier(Microsoft.VisualStudio.Services.Identity.Identity identity, Guid domainId) => new IdentityMapper(domainId).MapToWellKnownIdentifier(identity);

    public static void MapToWellKnownIdentifiers(IEnumerable<Microsoft.VisualStudio.Services.Identity.Identity> identities, Guid domainId) => new IdentityMapper(domainId).MapToWellKnownIdentifiers(identities);

    public static IdentityDescriptor MapToWellKnownIdentifier(
      IdentityDescriptor descriptor,
      Guid domainId)
    {
      return new IdentityMapper(domainId).MapToWellKnownIdentifier(descriptor);
    }

    public static string MapToWellKnownIdentifier(string identifier, Guid domainId) => new IdentityMapper(domainId).MapToWellKnownIdentifier(identifier);

    public static SubjectDescriptor MapToWellKnownIdentifier(
      SubjectDescriptor identifier,
      Guid domainId)
    {
      return new IdentityMapper(domainId).MapToWellKnownIdentifier(identifier);
    }
  }
}
