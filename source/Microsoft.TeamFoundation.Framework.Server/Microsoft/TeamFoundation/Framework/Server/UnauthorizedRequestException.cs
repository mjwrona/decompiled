// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.UnauthorizedRequestException
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.VisualStudio.Services.Common;
using System;
using System.Net;

namespace Microsoft.TeamFoundation.Framework.Server
{
  [Serializable]
  public class UnauthorizedRequestException : RequestFilterException
  {
    public UnauthorizedRequestException()
      : base(FrameworkResources.AuthenticationRequiredError(), HttpStatusCode.Unauthorized)
    {
    }

    public UnauthorizedRequestException(Guid id)
      : base(FrameworkResources.UnauthorizedUserError((object) id.ToString()), HttpStatusCode.Unauthorized)
    {
    }

    public UnauthorizedRequestException(SubjectDescriptor subjectDescriptor)
      : base(FrameworkResources.UnauthorizedUserError((object) subjectDescriptor.ToString()), HttpStatusCode.Unauthorized)
    {
    }

    public UnauthorizedRequestException(string exceptionMessage, HttpStatusCode statusCode)
      : base(exceptionMessage, statusCode)
    {
    }

    public UnauthorizedRequestException(string exceptionMessage, string resolutionUrl)
      : base(FrameworkResources.NoncompliantUserResolutionError((object) exceptionMessage, (object) resolutionUrl), HttpStatusCode.Forbidden)
    {
    }

    public UnauthorizedRequestException(string exceptionMessage, Exception innerException)
      : base(exceptionMessage, HttpStatusCode.Forbidden, innerException)
    {
    }
  }
}
