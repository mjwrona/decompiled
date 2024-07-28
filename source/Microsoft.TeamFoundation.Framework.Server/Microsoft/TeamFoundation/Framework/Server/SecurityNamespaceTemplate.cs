// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.SecurityNamespaceTemplate
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.Framework.Server
{
  public class SecurityNamespaceTemplate
  {
    public SecurityNamespaceTemplate(
      TeamFoundationHostType hostType,
      Guid namespaceId,
      NamespaceDescription description)
    {
      this.HostType = hostType;
      this.NamespaceId = namespaceId;
      this.Description = description;
    }

    public TeamFoundationHostType HostType { get; }

    public Guid NamespaceId { get; }

    public NamespaceDescription Description { get; }

    public int Signature => Tuple.Create<TeamFoundationHostType, Guid>(this.HostType, this.NamespaceId).GetHashCode();

    private void CheckHostType(
      TeamFoundationHostType hostType,
      List<SecurityNamespaceTemplate> templates)
    {
      if (!this.HostType.HasFlag((Enum) hostType))
        return;
      templates.Add(new SecurityNamespaceTemplate(hostType, this.NamespaceId, this.Description));
    }

    internal IList<SecurityNamespaceTemplate> DecomposeHostType()
    {
      List<SecurityNamespaceTemplate> templates = new List<SecurityNamespaceTemplate>();
      if (this.HostType == TeamFoundationHostType.Parent || this.HostType == TeamFoundationHostType.Unknown)
        return (IList<SecurityNamespaceTemplate>) templates;
      this.CheckHostType(TeamFoundationHostType.Deployment, templates);
      this.CheckHostType(TeamFoundationHostType.Application, templates);
      this.CheckHostType(TeamFoundationHostType.ProjectCollection, templates);
      return (IList<SecurityNamespaceTemplate>) templates;
    }

    internal void ValidateSecurityNamespaceTemplate(IVssRequestContext requestContext)
    {
      ArgumentUtility.CheckForOutOfRange((int) this.HostType, "HostType", 1, 7);
      ArgumentUtility.CheckForEmptyGuid(this.NamespaceId, "NamespaceId");
      ArgumentUtility.CheckBool(this.Description.IsProjected, true, "IsProjected");
      if (this.Description is SecurityNamespaceDescription description)
      {
        ArgumentUtility.CheckForNull<string>(description.Name, "Name");
        ArgumentUtility.CheckStringForNullOrEmpty(description.DataspaceCategory, "DataspaceCategory");
        ArgumentUtility.CheckForEmptyGuid(description.NamespaceId, "NamespaceId");
        description.ValidateActions();
      }
      else
        this.Description.Validate(requestContext);
    }
  }
}
