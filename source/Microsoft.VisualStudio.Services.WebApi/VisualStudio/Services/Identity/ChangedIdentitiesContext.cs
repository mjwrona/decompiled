// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Identity.ChangedIdentitiesContext
// Assembly: Microsoft.VisualStudio.Services.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7B264323-C592-4F23-AB6B-55AEDC85864F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.WebApi.dll

using Newtonsoft.Json;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.Identity
{
  [DataContract]
  public class ChangedIdentitiesContext
  {
    private static int UnspecifiedSequenceId = -1;

    [JsonConstructor]
    private ChangedIdentitiesContext()
    {
    }

    public ChangedIdentitiesContext(int identitySequenceId, int groupSequenceId)
      : this(identitySequenceId, groupSequenceId, ChangedIdentitiesContext.UnspecifiedSequenceId)
    {
    }

    public ChangedIdentitiesContext(
      int identitySequenceId,
      int groupSequenceId,
      int organizationIdentitySequenceId)
      : this(identitySequenceId, groupSequenceId, organizationIdentitySequenceId, 0)
    {
    }

    public ChangedIdentitiesContext(
      int identitySequenceId,
      int groupSequenceId,
      int organizationIdentitySequenceId,
      int pageSize)
    {
      this.IdentitySequenceId = identitySequenceId;
      this.GroupSequenceId = groupSequenceId;
      this.OrganizationIdentitySequenceId = organizationIdentitySequenceId;
      this.PageSize = pageSize;
    }

    [DataMember]
    public int IdentitySequenceId { get; private set; }

    [DataMember]
    public int GroupSequenceId { get; private set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public int OrganizationIdentitySequenceId { get; private set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public int PageSize { get; private set; }
  }
}
