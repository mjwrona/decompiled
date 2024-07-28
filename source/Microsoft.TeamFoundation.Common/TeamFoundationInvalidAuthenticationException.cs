// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TeamFoundationInvalidAuthenticationException
// Assembly: Microsoft.TeamFoundation.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0E643B42-FE11-4FC2-A9D6-79417E26CF92
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Common.dll

using Microsoft.VisualStudio.Services.Common;
using System;
using System.Net;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation
{
  [ExceptionMapping("0.0", "3.0", "TeamFoundationInvalidAuthenticationException", "Microsoft.TeamFoundation.TeamFoundationInvalidAuthenticationException, Microsoft.TeamFoundation.Common, Version=14.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a")]
  [Serializable]
  public class TeamFoundationInvalidAuthenticationException : 
    TeamFoundationServerUnauthorizedException
  {
    public TeamFoundationInvalidAuthenticationException(string message)
      : base(message, (Exception) null)
    {
    }

    public TeamFoundationInvalidAuthenticationException(
      string message,
      TeamFoundationAuthenticationError error)
      : base(message, (Exception) null, (HttpWebResponse) null, error)
    {
    }

    protected TeamFoundationInvalidAuthenticationException(
      SerializationInfo info,
      StreamingContext context)
      : base(info, context)
    {
    }
  }
}
