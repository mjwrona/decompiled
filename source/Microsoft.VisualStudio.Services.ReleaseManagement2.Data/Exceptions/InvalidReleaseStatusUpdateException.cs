// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Data.Exceptions.InvalidReleaseStatusUpdateException
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Data, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A69B0FB7-3028-4162-BA38-794D80D7A49A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Data.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Data.Exceptions
{
  [Serializable]
  public class InvalidReleaseStatusUpdateException : TeamFoundationServiceException
  {
    public InvalidReleaseStatusUpdateException()
    {
    }

    public InvalidReleaseStatusUpdateException(string message)
      : base(message)
    {
    }

    protected InvalidReleaseStatusUpdateException(SerializationInfo info, StreamingContext context)
      : base(info, context)
    {
    }

    public InvalidReleaseStatusUpdateException(string message, Exception innerException)
      : base(message, innerException)
    {
    }
  }
}
