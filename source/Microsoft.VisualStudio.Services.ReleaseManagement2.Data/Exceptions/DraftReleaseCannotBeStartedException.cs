// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Data.Exceptions.DraftReleaseCannotBeStartedException
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Data, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A69B0FB7-3028-4162-BA38-794D80D7A49A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Data.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Data.Exceptions
{
  [Serializable]
  public class DraftReleaseCannotBeStartedException : TeamFoundationServiceException
  {
    public DraftReleaseCannotBeStartedException()
    {
    }

    public DraftReleaseCannotBeStartedException(string message)
      : base(message)
    {
    }

    protected DraftReleaseCannotBeStartedException(SerializationInfo info, StreamingContext context)
      : base(info, context)
    {
    }

    public DraftReleaseCannotBeStartedException(string message, Exception innerException)
      : base(message, innerException)
    {
    }
  }
}
