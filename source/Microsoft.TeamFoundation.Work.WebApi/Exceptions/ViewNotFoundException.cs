// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Work.WebApi.Exceptions.ViewNotFoundException
// Assembly: Microsoft.TeamFoundation.Work.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0C4CCFA0-0616-4E48-A4F0-952E1CB10B12
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Work.WebApi.dll

using Microsoft.VisualStudio.Services.Common;
using System;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.Work.WebApi.Exceptions
{
  public class ViewNotFoundException : VssServiceException
  {
    public ViewNotFoundException()
      : base(AgileResources.ViewNotFoundExceptionMessage)
    {
    }

    public ViewNotFoundException(string message)
      : base(message)
    {
    }

    public ViewNotFoundException(Exception innerException)
      : base(AgileResources.ViewNotFoundExceptionMessage, innerException)
    {
    }

    public ViewNotFoundException(string message, Exception innerException)
      : base(message, innerException)
    {
    }

    public ViewNotFoundException(SerializationInfo info, StreamingContext context)
      : base(info, context)
    {
    }
  }
}
