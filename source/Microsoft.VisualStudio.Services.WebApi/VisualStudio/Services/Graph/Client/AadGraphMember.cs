// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Graph.Client.AadGraphMember
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
  public abstract class AadGraphMember : GraphMember
  {
    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public string MetaType { get; private set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    [ClientInternalUseOnly(true)]
    internal DateTime MetadataUpdateDate { get; private set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public string DirectoryAlias { get; private set; }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual bool ShouldSerializeMetadataUpdateDate() => this.ShoudSerializeInternals;

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public bool IsDeletedInOrigin { get; private set; }

    [EditorBrowsable(EditorBrowsableState.Never)]
    internal AadGraphMember(
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
      : base(origin, originId, descriptor, legacyDescriptor, displayName, links, url, domain, principalName, mailAddress)
    {
      this.MetaType = metaType;
      this.MetadataUpdateDate = metadataUpdateDate;
      this.IsDeletedInOrigin = isDeletedInOrigin;
      this.DirectoryAlias = directoryAlias;
    }

    protected AadGraphMember()
    {
    }
  }
}
