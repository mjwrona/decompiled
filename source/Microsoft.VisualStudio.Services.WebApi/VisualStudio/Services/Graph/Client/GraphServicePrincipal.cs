// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Graph.Client.GraphServicePrincipal
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
  public class GraphServicePrincipal : AadGraphMember
  {
    public override string SubjectKind => "servicePrincipal";

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public string ApplicationId { get; private set; }

    [EditorBrowsable(EditorBrowsableState.Never)]
    internal GraphServicePrincipal(
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
      string directoryAlias,
      string applicationId)
      : base(origin, originId, descriptor, legacyDescriptor, displayName, links, url, domain, principalName, mailAddress, metaType, metadataUpdateDate, isDeletedInOrigin, directoryAlias)
    {
      this.ApplicationId = applicationId;
    }

    internal GraphServicePrincipal(
      GraphServicePrincipal servicePrincipal,
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
      string directoryAlias = null,
      string applicationId = null)
    {
      string origin1 = origin ?? servicePrincipal?.Origin;
      string originId1 = originId ?? servicePrincipal?.OriginId;
      SubjectDescriptor descriptor1 = descriptor ?? (servicePrincipal != null ? servicePrincipal.Descriptor : new SubjectDescriptor());
      IdentityDescriptor legacyDescriptor1 = legacyDescriptor;
      if ((object) legacyDescriptor1 == null)
        legacyDescriptor1 = servicePrincipal?.LegacyDescriptor ?? (IdentityDescriptor) null;
      string displayName1 = displayName ?? servicePrincipal?.DisplayName;
      ReferenceLinks links1 = links ?? servicePrincipal?.Links;
      string url1 = url ?? servicePrincipal?.Url;
      string domain1 = domain ?? servicePrincipal?.Domain;
      string principalName1 = principalName ?? servicePrincipal?.PrincipalName;
      string mailAddress1 = mailAddress ?? servicePrincipal?.MailAddress;
      string metaType1 = metaType ?? servicePrincipal?.MetaType;
      DateTime metadataUpdateDate1 = metadataUpdateDate ?? (servicePrincipal != null ? servicePrincipal.MetadataUpdateDate : DateTime.MinValue);
      int num = (int) isDeletedInOrigin ?? (servicePrincipal != null ? (servicePrincipal.IsDeletedInOrigin ? 1 : 0) : 0);
      string directoryAlias1 = directoryAlias ?? servicePrincipal?.DirectoryAlias;
      string applicationId1 = applicationId ?? servicePrincipal?.ApplicationId;
      // ISSUE: explicit constructor call
      this.\u002Ector(origin1, originId1, descriptor1, legacyDescriptor1, displayName1, links1, url1, domain1, principalName1, mailAddress1, metaType1, metadataUpdateDate1, num != 0, directoryAlias1, applicationId1);
    }

    protected GraphServicePrincipal()
    {
    }
  }
}
