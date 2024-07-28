// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.InvalidServiceVersionException
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.Framework.Server
{
  [Serializable]
  public class InvalidServiceVersionException : TeamFoundationResourceManagementServiceException
  {
    private readonly string m_serviceName;
    private readonly int m_serviceVersion;
    private readonly int m_minServiceVersion;

    public InvalidServiceVersionException(
      string serviceName,
      int serviceVersion,
      int minServiceVersion)
    {
      this.m_serviceName = serviceName;
      this.m_serviceVersion = serviceVersion;
      this.m_minServiceVersion = minServiceVersion;
    }

    protected InvalidServiceVersionException(SerializationInfo info, StreamingContext context)
      : base(info, context)
    {
    }

    public string ServiceName => this.m_serviceName;

    public int ServiceVersion => this.m_serviceVersion;

    public int MinServiceVersion => this.m_minServiceVersion;
  }
}
