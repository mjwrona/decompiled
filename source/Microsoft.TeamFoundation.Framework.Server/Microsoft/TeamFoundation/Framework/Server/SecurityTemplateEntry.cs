// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.SecurityTemplateEntry
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.VisualStudio.Services.Common;
using System;

namespace Microsoft.TeamFoundation.Framework.Server
{
  public class SecurityTemplateEntry
  {
    public readonly Guid Id;
    public readonly TeamFoundationHostType HostType;
    public readonly Guid NamespaceId;
    public readonly Guid AclStoreId;
    public readonly string TokenTemplate;
    public readonly string SubjectTemplate;
    public readonly int Allow;
    public readonly int Deny;

    public SecurityTemplateEntry(
      Guid id,
      TeamFoundationHostType hostType,
      Guid namespaceId,
      Guid aclStoreId,
      string tokenTemplate,
      string subjectTemplate,
      int allow,
      int deny)
    {
      ArgumentUtility.CheckForEmptyGuid(id, nameof (id));
      ArgumentUtility.CheckForMultipleBits((int) hostType, nameof (hostType));
      ArgumentUtility.CheckForEmptyGuid(namespaceId, nameof (namespaceId));
      ArgumentUtility.CheckForEmptyGuid(aclStoreId, nameof (aclStoreId));
      ArgumentUtility.CheckForNull<string>(tokenTemplate, nameof (tokenTemplate));
      ArgumentUtility.CheckForNull<string>(subjectTemplate, nameof (subjectTemplate));
      this.Id = id;
      this.HostType = hostType;
      this.NamespaceId = namespaceId;
      this.AclStoreId = aclStoreId;
      this.TokenTemplate = tokenTemplate;
      this.SubjectTemplate = subjectTemplate;
      this.Allow = allow;
      this.Deny = deny;
    }

    internal static SecurityTemplateEntry CreateFromComponentType(
      SecurityTemplateComponent.SecurityTemplateEntry entry)
    {
      return new SecurityTemplateEntry(entry.Id, (TeamFoundationHostType) entry.HostType, entry.NamespaceId, entry.AclStoreId, entry.TokenTemplate, entry.SubjectTemplate, entry.Allow, entry.Deny);
    }
  }
}
