// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.SecuritySubjectComponent
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.Framework.Server
{
  internal class SecuritySubjectComponent : TeamFoundationSqlResourceComponent
  {
    public static readonly ComponentFactory ComponentFactory = new ComponentFactory(new IComponentCreator[3]
    {
      (IComponentCreator) new ComponentCreator<SecuritySubjectComponent>(1, true),
      (IComponentCreator) new ComponentCreator<SecuritySubjectComponent2>(2),
      (IComponentCreator) new ComponentCreator<SecuritySubjectComponent3>(3)
    }, "SecuritySubject");

    public virtual IReadOnlyList<SecuritySubjectComponent.SecuritySubjectEntry> QuerySecuritySubjectEntries(
      out long sequenceId)
    {
      sequenceId = 0L;
      List<SecuritySubjectComponent.SecuritySubjectEntry> securitySubjectEntryList = new List<SecuritySubjectComponent.SecuritySubjectEntry>();
      if (this.RequestContext.ServiceHost.DeploymentServiceHost.IsHosted)
      {
        SecuritySubjectComponent.SecuritySubjectEntry securitySubjectEntry = new SecuritySubjectComponent.SecuritySubjectEntry(HostedLicenseName.Stakeholder, 3, (string) null, "Hosted Stakeholder License Security Subject");
        securitySubjectEntryList.Add(securitySubjectEntry);
      }
      else
      {
        SecuritySubjectComponent.SecuritySubjectEntry securitySubjectEntry = new SecuritySubjectComponent.SecuritySubjectEntry(OnPremLicenseName.Limited, 3, (string) null, "On-premises Limited License Security Subject");
        securitySubjectEntryList.Add(securitySubjectEntry);
      }
      return (IReadOnlyList<SecuritySubjectComponent.SecuritySubjectEntry>) securitySubjectEntryList.AsReadOnly();
    }

    public virtual long UpdateSecuritySubjectEntries(
      IEnumerable<SecuritySubjectComponent.SecuritySubjectEntry> entries)
    {
      throw new NotImplementedException();
    }

    public virtual long DeleteSecuritySubjectEntries(IEnumerable<Guid> ids) => throw new NotImplementedException();

    internal class SecuritySubjectEntry
    {
      public readonly Guid Id;
      public readonly int SubjectType;
      public readonly string Identifier;
      public readonly string Description;
      internal const int MaxIdentifierStringLength = 512;
      internal const int MaxDescriptionStringLength = 1024;

      public SecuritySubjectEntry(Guid id, int subjectType, string identifier, string description)
      {
        this.Id = id;
        this.SubjectType = subjectType;
        this.Identifier = string.IsNullOrEmpty(identifier) ? id.ToString("D") : identifier;
        this.Description = description;
      }
    }
  }
}
