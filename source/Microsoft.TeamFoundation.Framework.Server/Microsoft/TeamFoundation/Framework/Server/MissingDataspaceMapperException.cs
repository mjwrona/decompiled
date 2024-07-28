// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.MissingDataspaceMapperException
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.ComponentModel;

namespace Microsoft.TeamFoundation.Framework.Server
{
  [Serializable]
  public class MissingDataspaceMapperException : TeamFoundationSecurityServiceException
  {
    public MissingDataspaceMapperException(Guid namespaceId)
      : base(FrameworkResources.MissingDataspaceMapper((object) namespaceId))
    {
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public MissingDataspaceMapperException(WrappedException wrappedException)
      : base(wrappedException.Message)
    {
    }
  }
}
