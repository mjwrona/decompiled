// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.BasicAuthPasswordReenableUnexpectedFailureException
// Assembly: Microsoft.VisualStudio.Services.Cloud, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F2D48A0E-C4C9-4233-BA34-E8461823F6F2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cloud.dll

using System;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.Framework.Server
{
  [Serializable]
  public class BasicAuthPasswordReenableUnexpectedFailureException : TeamFoundationServiceException
  {
    public BasicAuthPasswordReenableUnexpectedFailureException(string message)
      : base(message)
    {
    }

    public BasicAuthPasswordReenableUnexpectedFailureException(
      string message,
      Exception innerException)
      : base(message, innerException)
    {
    }

    protected BasicAuthPasswordReenableUnexpectedFailureException(
      SerializationInfo info,
      StreamingContext context)
      : base(info, context)
    {
    }
  }
}
