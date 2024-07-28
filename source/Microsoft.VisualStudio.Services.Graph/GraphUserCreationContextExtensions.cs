// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Graph.GraphUserCreationContextExtensions
// Assembly: Microsoft.VisualStudio.Services.Graph, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 00390AA0-D8BB-45EB-AEF5-70DC8BFC765D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Graph.dll

using Microsoft.VisualStudio.Services.Directories;
using Microsoft.VisualStudio.Services.Graph.Client;

namespace Microsoft.VisualStudio.Services.Graph
{
  public static class GraphUserCreationContextExtensions
  {
    public static IDirectoryEntityDescriptor ToDirectoryEntityDescriptor(
      this GraphUserOriginIdUpdateContext updateContext,
      string newLocalId = null)
    {
      return (IDirectoryEntityDescriptor) new DirectoryEntityDescriptor("User", originId: updateContext.OriginId, localId: newLocalId, properties: GraphToDirectoryService.CommonUpdateUserProperties);
    }

    public static IDirectoryEntityDescriptor ToDirectoryEntityDescriptor(
      this GraphUserPrincipalNameUpdateContext updateContext,
      string newLocalId = null)
    {
      string principalName = updateContext.PrincipalName;
      return (IDirectoryEntityDescriptor) new DirectoryEntityDescriptor("User", localId: newLocalId, principalName: principalName, properties: GraphToDirectoryService.CommonUpdateUserProperties);
    }
  }
}
