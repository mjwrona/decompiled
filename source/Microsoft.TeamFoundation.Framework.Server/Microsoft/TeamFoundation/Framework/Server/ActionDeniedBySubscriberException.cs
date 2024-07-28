// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.ActionDeniedBySubscriberException
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Common;
using System;

namespace Microsoft.TeamFoundation.Framework.Server
{
  [Serializable]
  public class ActionDeniedBySubscriberException : EventException
  {
    private ExceptionPropertyCollection m_properties;

    public ActionDeniedBySubscriberException(string message, ExceptionPropertyCollection properties)
      : base(message)
    {
      this.m_properties = properties;
    }

    public ExceptionPropertyCollection PropertyCollection => this.m_properties;

    public override void GetExceptionProperties(ExceptionPropertyCollection properties)
    {
      base.GetExceptionProperties(properties);
      if (this.m_properties == null)
        return;
      properties.Copy(this.m_properties);
    }
  }
}
