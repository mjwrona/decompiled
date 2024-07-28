// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.Types.IdentityImageFailureException
// Assembly: Microsoft.TeamFoundation.Server.WebAccess, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A2CCA8C5-6910-48A5-82D9-BDC1350B5B4D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.WebAccess;
using System;

namespace Microsoft.TeamFoundation.Server.Types
{
  public class IdentityImageFailureException : TeamFoundationServiceException
  {
    public IdentityImageFailureException(Exception innerException)
      : base(WebAccessServerResources.IdentityImageFailureExceptionMessage, innerException)
    {
    }
  }
}
