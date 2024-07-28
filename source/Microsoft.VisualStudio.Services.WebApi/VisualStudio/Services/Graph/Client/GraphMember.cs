// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Graph.Client.GraphMember
// Assembly: Microsoft.VisualStudio.Services.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7B264323-C592-4F23-AB6B-55AEDC85864F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.WebApi.dll

using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Identity;
using Microsoft.VisualStudio.Services.WebApi;
using System.ComponentModel;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.Graph.Client
{
  [DataContract]
  public abstract class GraphMember : GraphSubject
  {
    [DataMember(IsRequired = false, EmitDefaultValue = true)]
    public string Domain { get; private set; }

    [DataMember]
    public string PrincipalName { get; private set; }

    [DataMember(IsRequired = false, EmitDefaultValue = true)]
    public string MailAddress { get; private set; }

    [EditorBrowsable(EditorBrowsableState.Never)]
    protected GraphMember(
      string origin,
      string originId,
      SubjectDescriptor descriptor,
      IdentityDescriptor legacyDescriptor,
      string displayName,
      ReferenceLinks links,
      string url,
      string domain,
      string principalName,
      string mailAddress)
      : base(origin, originId, descriptor, legacyDescriptor, displayName, links, url)
    {
      this.Domain = domain;
      this.PrincipalName = principalName;
      this.MailAddress = mailAddress;
    }

    protected GraphMember()
    {
    }
  }
}
