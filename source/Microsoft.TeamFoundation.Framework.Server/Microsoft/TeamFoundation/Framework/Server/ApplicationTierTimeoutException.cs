// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.ApplicationTierTimeoutException
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.Framework.Server
{
  [Serializable]
  public class ApplicationTierTimeoutException : ProxyException
  {
    public ApplicationTierTimeoutException()
      : this(string.Empty)
    {
    }

    public ApplicationTierTimeoutException(string message)
      : this(message, (Exception) null)
    {
    }

    public ApplicationTierTimeoutException(Exception innerException)
      : this(FrameworkResources.ProxyCacheMissBecameHitException(), innerException)
    {
    }

    public ApplicationTierTimeoutException(string message, Exception innerException)
      : base(message, innerException)
    {
      this.LogException = true;
    }

    protected ApplicationTierTimeoutException(SerializationInfo info, StreamingContext context)
      : base(info, context)
    {
    }
  }
}
