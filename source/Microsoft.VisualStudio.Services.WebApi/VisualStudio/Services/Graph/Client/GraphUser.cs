// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Graph.Client.GraphUser
// Assembly: Microsoft.VisualStudio.Services.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7B264323-C592-4F23-AB6B-55AEDC85864F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.WebApi.dll

using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Identity;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.ComponentModel;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.Graph.Client
{
  [DataContract]
  public class GraphUser : AadGraphMember
  {
    public override string SubjectKind => "user";

    [EditorBrowsable(EditorBrowsableState.Never)]
    internal GraphUser(
      string origin,
      string originId,
      SubjectDescriptor descriptor,
      IdentityDescriptor legacyDescriptor,
      string displayName,
      ReferenceLinks links,
      string url,
      string domain,
      string principalName,
      string mailAddress,
      string metaType,
      DateTime metadataUpdateDate,
      bool isDeletedInOrigin,
      string directoryAlias)
      : base(origin, originId, descriptor, legacyDescriptor, displayName, links, url, domain, principalName, mailAddress, metaType, metadataUpdateDate, isDeletedInOrigin, directoryAlias)
    {
    }

    internal GraphUser(
      GraphUser user,
      string origin = null,
      string originId = null,
      SubjectDescriptor? descriptor = null,
      IdentityDescriptor legacyDescriptor = null,
      string displayName = null,
      ReferenceLinks links = null,
      string url = null,
      string domain = null,
      string principalName = null,
      string mailAddress = null,
      string metaType = null,
      DateTime? metadataUpdateDate = null,
      bool? isDeletedInOrigin = false,
      string directoryAlias = null)
    {
      string origin1 = origin ?? user?.Origin;
      string originId1 = originId ?? user?.OriginId;
      SubjectDescriptor descriptor1 = descriptor ?? (user != null ? user.Descriptor : new SubjectDescriptor());
      IdentityDescriptor legacyDescriptor1 = legacyDescriptor;
      if ((object) legacyDescriptor1 == null)
        legacyDescriptor1 = user?.LegacyDescriptor ?? (IdentityDescriptor) null;
      string displayName1 = displayName ?? user?.DisplayName;
      ReferenceLinks links1 = links ?? user?.Links;
      string url1 = url ?? user?.Url;
      string domain1 = domain ?? user?.Domain;
      string principalName1 = principalName ?? user?.PrincipalName;
      string mailAddress1 = mailAddress ?? user?.MailAddress;
      string metaType1 = metaType ?? user?.MetaType;
      DateTime metadataUpdateDate1 = metadataUpdateDate ?? (user != null ? user.MetadataUpdateDate : DateTime.MinValue);
      int num = (int) isDeletedInOrigin ?? (user != null ? (user.IsDeletedInOrigin ? 1 : 0) : 0);
      string directoryAlias1 = directoryAlias ?? user?.DirectoryAlias;
      // ISSUE: explicit constructor call
      this.\u002Ector(origin1, originId1, descriptor1, legacyDescriptor1, displayName1, links1, url1, domain1, principalName1, mailAddress1, metaType1, metadataUpdateDate1, num != 0, directoryAlias1);
    }

    protected GraphUser()
    {
    }
  }
}
