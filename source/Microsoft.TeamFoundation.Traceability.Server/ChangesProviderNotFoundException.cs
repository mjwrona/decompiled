// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Traceability.Server.ChangesProviderNotFoundException
// Assembly: Microsoft.TeamFoundation.Traceability.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: C62AF110-A283-470F-B32B-FE03F2A1E0D2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Traceability.Server.dll

using Microsoft.VisualStudio.Services.Common;
using System;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.Traceability.Server
{
  [Serializable]
  public sealed class ChangesProviderNotFoundException : VssServiceException
  {
    public ChangesProviderNotFoundException(string message)
      : base(message)
    {
    }

    public ChangesProviderNotFoundException(string message, Exception innerException)
      : base(message, innerException)
    {
    }

    private ChangesProviderNotFoundException(SerializationInfo info, StreamingContext context)
      : base(info, context)
    {
    }
  }
}
