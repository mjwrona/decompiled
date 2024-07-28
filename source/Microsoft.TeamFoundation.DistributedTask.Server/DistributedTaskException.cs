// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Server.DistributedTaskException
// Assembly: Microsoft.TeamFoundation.DistributedTask.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: EDA28B25-3B93-49F7-A194-C85AAF98B83A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Server.dll

using Microsoft.VisualStudio.Services.Common;
using System;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.DistributedTask.Server
{
  [Serializable]
  public class DistributedTaskException : VssServiceException
  {
    public DistributedTaskException(string message)
      : base(message)
    {
    }

    public DistributedTaskException(string message, Exception innerException)
      : base(message, innerException)
    {
    }

    protected DistributedTaskException(SerializationInfo info, StreamingContext context)
      : base(info, context)
    {
    }
  }
}
