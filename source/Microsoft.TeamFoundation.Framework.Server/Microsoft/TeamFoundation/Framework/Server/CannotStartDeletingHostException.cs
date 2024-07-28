// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.CannotStartDeletingHostException
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Data.SqlClient;
using System.Globalization;

namespace Microsoft.TeamFoundation.Framework.Server
{
  [Serializable]
  public class CannotStartDeletingHostException : HostManagementException
  {
    public CannotStartDeletingHostException(Guid hostId)
      : this(hostId.ToString((string) null, (IFormatProvider) CultureInfo.InvariantCulture))
    {
    }

    private CannotStartDeletingHostException(string hostId)
      : base(FrameworkResources.CannotStartDeletingHostException((object) hostId))
    {
    }

    public CannotStartDeletingHostException(
      IVssRequestContext requestContext,
      SqlException ex,
      SqlError sqlError)
      : this(TeamFoundationServiceException.ExtractString(sqlError, "identifier"))
    {
    }

    public CannotStartDeletingHostException(WrappedException wrappedException)
      : base(wrappedException.Message)
    {
    }
  }
}
