// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Client.RegisterObjectBadParentException
// Assembly: Microsoft.TeamFoundation.Client, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 03892C75-AE2B-482B-8E0D-B14588A2C857
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Client.dll

using System;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.Framework.Client
{
  [Serializable]
  public class RegisterObjectBadParentException : AuthorizationSubsystemServiceException
  {
    public RegisterObjectBadParentException(string message)
      : base(message)
    {
    }

    public RegisterObjectBadParentException(string message, Exception ex)
      : base(message, ex)
    {
    }

    protected RegisterObjectBadParentException(SerializationInfo info, StreamingContext context)
      : base(info, context)
    {
    }
  }
}
