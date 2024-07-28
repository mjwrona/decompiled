// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.UnexpectedHostTypeException
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Common;
using System;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.Framework.Server
{
  [Serializable]
  public class UnexpectedHostTypeException : TeamFoundationServiceException
  {
    private TeamFoundationHostType m_actualHostType;

    public UnexpectedHostTypeException(TeamFoundationHostType hostType)
      : base(FrameworkResources.UnexpectedHostType((object) hostType.ToString()))
    {
      this.m_actualHostType = hostType;
    }

    protected UnexpectedHostTypeException(SerializationInfo info, StreamingContext context)
      : base(info, context)
    {
    }

    public TeamFoundationHostType AcutalHostType => this.m_actualHostType;

    public override void GetExceptionProperties(ExceptionPropertyCollection properties) => properties.Set("HostType", this.m_actualHostType.ToString());
  }
}
