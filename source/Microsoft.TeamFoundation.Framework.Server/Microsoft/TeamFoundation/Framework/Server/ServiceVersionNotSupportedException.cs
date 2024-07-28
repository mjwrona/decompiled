// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.ServiceVersionNotSupportedException
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.Framework.Server
{
  [Serializable]
  public class ServiceVersionNotSupportedException : TeamFoundationResourceManagementServiceException
  {
    public ServiceVersionNotSupportedException(string serviceName, int version, int minVersion)
      : this(FrameworkResources.ServiceVersionNotSupported((object) serviceName, (object) version, (object) minVersion))
    {
      this.Version = new int?(version);
      this.MinVersion = new int?(minVersion);
    }

    public ServiceVersionNotSupportedException()
    {
    }

    public ServiceVersionNotSupportedException(string message)
      : base(message)
    {
    }

    public ServiceVersionNotSupportedException(string message, Exception inner)
      : base(message, inner)
    {
    }

    protected ServiceVersionNotSupportedException(SerializationInfo info, StreamingContext context)
      : base(info, context)
    {
    }

    public int? Version { get; }

    public int? MinVersion { get; }
  }
}
