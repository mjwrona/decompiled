// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.SecuritySubjectEntry
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Identity;
using System;

namespace Microsoft.TeamFoundation.Framework.Server
{
  public class SecuritySubjectEntry
  {
    public readonly Guid Id;
    public readonly SecuritySubjectType SubjectType;
    public readonly string Identifier;
    public readonly string Description;

    public SecuritySubjectEntry(
      Guid id,
      SecuritySubjectType subjectType,
      string identifier,
      string description)
    {
      ArgumentUtility.CheckForEmptyGuid(id, nameof (id));
      ArgumentUtility.CheckForOutOfRange((int) subjectType, nameof (subjectType), 1, 6);
      ArgumentUtility.CheckStringForNullOrEmpty(identifier, nameof (identifier));
      ArgumentUtility.CheckStringLength(identifier, nameof (identifier), 512);
      if (description != null)
        ArgumentUtility.CheckStringLength(description, nameof (description), 1024);
      this.Id = id;
      this.SubjectType = subjectType;
      this.Identifier = identifier;
      this.Description = description;
    }

    internal static SecuritySubjectEntry CreateFromComponentType(
      SecuritySubjectComponent.SecuritySubjectEntry entry)
    {
      return new SecuritySubjectEntry(entry.Id, (SecuritySubjectType) entry.SubjectType, entry.Identifier, entry.Description);
    }

    internal IdentityDescriptor ToDescriptor() => new IdentityDescriptor("System:" + this.SubjectType.ToString(), this.Identifier);

    internal SubjectDescriptor ToSubjectDescriptor()
    {
      string subjectType;
      switch (this.SubjectType)
      {
        case SecuritySubjectType.ServicePrincipal:
          subjectType = "s2s";
          break;
        case SecuritySubjectType.WellKnownGroup:
          subjectType = "vssgp";
          break;
        case SecuritySubjectType.License:
          subjectType = "slic";
          break;
        case SecuritySubjectType.Scope:
          subjectType = "sscp";
          break;
        case SecuritySubjectType.CspPartner:
          subjectType = "csp";
          break;
        case SecuritySubjectType.PublicAccess:
          subjectType = "spa";
          break;
        default:
          throw new NotSupportedException(string.Format("The subject type '{0}' is not a supported value for security subject entries.", (object) this.SubjectType));
      }
      return new SubjectDescriptor(subjectType, this.Identifier);
    }
  }
}
