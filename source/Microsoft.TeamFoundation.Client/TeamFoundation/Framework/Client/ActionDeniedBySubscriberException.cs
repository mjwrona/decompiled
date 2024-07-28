// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Client.ActionDeniedBySubscriberException
// Assembly: Microsoft.TeamFoundation.Client, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 03892C75-AE2B-482B-8E0D-B14588A2C857
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Client.dll

using System;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.Framework.Client
{
  [Serializable]
  public class ActionDeniedBySubscriberException : EventException
  {
    public int StatusCode => this.GetProperty<int>("Microsoft.TeamFoundation.StatusCode");

    public string SubscriberName => this.GetProperty<string>("Microsoft.TeamFoundation.SubscriberName");

    public string SubscriberType => this.GetProperty<string>("Microsoft.TeamFoundation.SubscriberType");

    public ActionDeniedBySubscriberException(string message)
      : base(message)
    {
    }

    public ActionDeniedBySubscriberException(string message, Exception ex)
      : base(message, ex)
    {
    }

    protected ActionDeniedBySubscriberException(SerializationInfo info, StreamingContext context)
      : base(info, context)
    {
    }
  }
}
