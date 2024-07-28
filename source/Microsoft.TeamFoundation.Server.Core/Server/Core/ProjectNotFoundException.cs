// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.Core.ProjectNotFoundException
// Assembly: Microsoft.TeamFoundation.Server.Core, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9DD3208E-87CF-4F7C-8D96-8880BDAD13B2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.Core.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Identity;
using System;

namespace Microsoft.TeamFoundation.Server.Core
{
  [Serializable]
  public class ProjectNotFoundException : IdentityServiceException
  {
    public ProjectNotFoundException()
      : base(FrameworkResources.PROJECT_NOT_FOUND())
    {
    }

    public ProjectNotFoundException(string message)
      : base(message)
    {
    }

    public ProjectNotFoundException(string message, Exception innerException)
      : base(message, innerException)
    {
    }
  }
}
