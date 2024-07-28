// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.BasicAuthPasswordChangeLimitException
// Assembly: Microsoft.VisualStudio.Services.Cloud, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F2D48A0E-C4C9-4233-BA34-E8461823F6F2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cloud.dll

using System;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.Framework.Server
{
  [Serializable]
  public class BasicAuthPasswordChangeLimitException : TeamFoundationServiceException
  {
    public BasicAuthPasswordChangeLimitException(string message, Exception innerException)
      : base(message, innerException)
    {
    }

    public BasicAuthPasswordChangeLimitException(int limit, TimeSpan interval)
      : base(FrameworkResources.BasicAuthPasswordChangeLimitError((object) limit, (object) (int) Math.Ceiling(interval.TotalHours)))
    {
    }

    protected BasicAuthPasswordChangeLimitException(
      SerializationInfo info,
      StreamingContext context)
      : base(info, context)
    {
    }
  }
}
