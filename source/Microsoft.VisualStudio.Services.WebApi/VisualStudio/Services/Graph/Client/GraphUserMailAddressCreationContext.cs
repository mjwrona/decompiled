// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Graph.Client.GraphUserMailAddressCreationContext
// Assembly: Microsoft.VisualStudio.Services.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7B264323-C592-4F23-AB6B-55AEDC85864F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.WebApi.dll

using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Directories;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.Graph.Client
{
  [DataContract]
  public class GraphUserMailAddressCreationContext : 
    GraphUserCreationContext,
    IGraphMemberMailAddressCreationContext,
    IGraphMemberCreationContext
  {
    [DataMember(IsRequired = true)]
    public string MailAddress { get; set; }

    public IDirectoryEntityDescriptor ToDirectoryEntityDescriptor() => (IDirectoryEntityDescriptor) new DirectoryEntityDescriptor("User", localId: this.LocalId, properties: (IReadOnlyDictionary<string, object>) new Dictionary<string, object>()
    {
      {
        "Mail",
        (object) this.MailAddress
      }
    }.AddRange<KeyValuePair<string, object>, Dictionary<string, object>>((IEnumerable<KeyValuePair<string, object>>) GraphToDirectoryService.CommonMemberMaterializationProperties));
  }
}
