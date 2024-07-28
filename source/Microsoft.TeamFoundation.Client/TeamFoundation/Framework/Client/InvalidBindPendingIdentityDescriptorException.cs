// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Client.InvalidBindPendingIdentityDescriptorException
// Assembly: Microsoft.TeamFoundation.Client, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 03892C75-AE2B-482B-8E0D-B14588A2C857
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Client.dll

using System;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.Framework.Client
{
  [Serializable]
  public class InvalidBindPendingIdentityDescriptorException : TeamFoundationIdentityServiceException
  {
    public InvalidBindPendingIdentityDescriptorException(string message)
      : base(message)
    {
    }

    public InvalidBindPendingIdentityDescriptorException(string message, Exception ex)
      : base(message, ex)
    {
    }

    protected InvalidBindPendingIdentityDescriptorException(
      SerializationInfo info,
      StreamingContext context)
      : base(info, context)
    {
    }
  }
}
