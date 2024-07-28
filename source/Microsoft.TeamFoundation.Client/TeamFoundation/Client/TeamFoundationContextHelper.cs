// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Client.TeamFoundationContextHelper
// Assembly: Microsoft.TeamFoundation.Client, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 03892C75-AE2B-482B-8E0D-B14588A2C857
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Client.dll

using Microsoft.TeamFoundation.Core.WebApi;
using System;
using System.ComponentModel;

namespace Microsoft.TeamFoundation.Client
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  public static class TeamFoundationContextHelper
  {
    public static bool IsProjectActive(
      IServiceProvider serviceProvider,
      Guid collectionId,
      string projectUri)
    {
      ITeamFoundationContextManager service = (ITeamFoundationContextManager) serviceProvider.GetService(typeof (ITeamFoundationContextManager));
      return service != null && service.CurrentContext != null && service.CurrentContext.HasCollection && service.CurrentContext.HasTeamProject && service.CurrentContext.TeamProjectCollection.InstanceId == collectionId && TFStringComparer.ProjectUri.Equals(service.CurrentContext.TeamProjectUri.AbsoluteUri, projectUri);
    }
  }
}
