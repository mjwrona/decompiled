// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TeamFoundationServiceUnavailableException
// Assembly: Microsoft.TeamFoundation.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0E643B42-FE11-4FC2-A9D6-79417E26CF92
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Common.dll

using Microsoft.TeamFoundation.Common.Internal;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation
{
  [ExceptionMapping("0.0", "3.0", "TeamFoundationServiceUnavailableException", "Microsoft.TeamFoundation.TeamFoundationServiceUnavailableException, Microsoft.TeamFoundation.Common, Version=14.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a")]
  [Serializable]
  public class TeamFoundationServiceUnavailableException : TeamFoundationServerException
  {
    public TeamFoundationServiceUnavailableException(string nameOrUrl, string reason)
      : base(TFCommonResources.ServicesUnavailable((object) nameOrUrl, (object) reason))
    {
    }

    public TeamFoundationServiceUnavailableException(string message, Exception innerException)
      : base(message, innerException)
    {
    }

    public TeamFoundationServiceUnavailableException(
      string nameOrUrl,
      string reason,
      Exception innerException)
      : this(TFCommonResources.ServicesUnavailable((object) nameOrUrl, (object) reason), innerException)
    {
    }

    protected TeamFoundationServiceUnavailableException(
      SerializationInfo info,
      StreamingContext context)
      : base(info, context)
    {
    }

    public TeamFoundationServiceUnavailableException(string reason)
      : base(TFCommonResources.ServicesUnavailableNoServer((object) reason))
    {
    }
  }
}
