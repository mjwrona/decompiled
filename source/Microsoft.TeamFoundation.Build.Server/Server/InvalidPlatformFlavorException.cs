// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build.Server.InvalidPlatformFlavorException
// Assembly: Microsoft.TeamFoundation.Build.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 50E8BB1D-C69C-4DD2-83BE-A8FFBFFA6298
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Data.SqlClient;
using System.Web.Services.Protocols;

namespace Microsoft.TeamFoundation.Build.Server
{
  [Serializable]
  public class InvalidPlatformFlavorException : BuildServerException
  {
    internal InvalidPlatformFlavorException(string platformName, string flavorName)
      : base(ResourceStrings.InvalidPlatformFlavorSpecified((object) platformName, (object) flavorName))
    {
      this.SetExceptionProperties();
    }

    public InvalidPlatformFlavorException(
      IVssRequestContext context,
      SqlException ex,
      SqlError err)
      : this(TeamFoundationServiceException.ExtractString(err, "platformName"), TeamFoundationServiceException.ExtractString(err, "flavorName"))
    {
    }

    protected void SetExceptionProperties() => this.FaultCode = Soap12FaultCodes.SenderFaultCode;
  }
}
