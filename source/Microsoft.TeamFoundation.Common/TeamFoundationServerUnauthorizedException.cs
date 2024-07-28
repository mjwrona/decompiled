// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TeamFoundationServerUnauthorizedException
// Assembly: Microsoft.TeamFoundation.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0E643B42-FE11-4FC2-A9D6-79417E26CF92
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Common.dll

using Microsoft.TeamFoundation.Common.Internal;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Net;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation
{
  [ExceptionMapping("0.0", "3.0", "TeamFoundationServerUnauthorizedException", "Microsoft.TeamFoundation.TeamFoundationServerUnauthorizedException, Microsoft.TeamFoundation.Common, Version=14.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a")]
  [Serializable]
  public class TeamFoundationServerUnauthorizedException : TeamFoundationServerException
  {
    public TeamFoundationServerUnauthorizedException()
      : base(TFCommonResources.UnauthorizedUnknownServer())
    {
    }

    public TeamFoundationServerUnauthorizedException(string message)
      : this(message, (Exception) null)
    {
    }

    public TeamFoundationServerUnauthorizedException(Exception innerException)
      : this(innerException.Message, innerException)
    {
    }

    public TeamFoundationServerUnauthorizedException(string message, Exception innerException)
      : base(message, innerException)
    {
      this.WebException = innerException as WebException;
    }

    public TeamFoundationServerUnauthorizedException(
      string message,
      Exception innerException,
      HttpWebResponse webResponse,
      TeamFoundationAuthenticationError error)
      : this(message, innerException)
    {
      this.AuthenticationError = error;
      this.WebResponse = webResponse;
    }

    protected TeamFoundationServerUnauthorizedException(
      SerializationInfo info,
      StreamingContext context)
      : base(info, context)
    {
    }

    public TeamFoundationAuthenticationError AuthenticationError { get; private set; }

    public WebException WebException { get; private set; }

    public HttpWebResponse WebResponse { get; private set; }
  }
}
