// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Graph.Client.GraphUserOriginIdUpdateContext
// Assembly: Microsoft.VisualStudio.Services.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7B264323-C592-4F23-AB6B-55AEDC85864F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.WebApi.dll

using Microsoft.VisualStudio.Services.Directories;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.Graph.Client
{
  [DataContract]
  public class GraphUserOriginIdUpdateContext : 
    GraphUserUpdateContext,
    IGraphMemberOriginIdUpdateContext,
    IGraphMemberUpdateContext
  {
    [DataMember(IsRequired = true)]
    public string OriginId { get; set; }

    public IDirectoryEntityDescriptor ToDirectoryEntityDescriptor(string newLocalId = null) => (IDirectoryEntityDescriptor) new DirectoryEntityDescriptor("User", originId: this.OriginId, localId: newLocalId, properties: GraphToDirectoryService.CommonUpdateUserProperties);
  }
}
