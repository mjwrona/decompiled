// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Data.Exceptions.InvalidRequestException
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Data, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A69B0FB7-3028-4162-BA38-794D80D7A49A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Data.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Data.SqlClient;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Data.Exceptions
{
  [Serializable]
  public class InvalidRequestException : TeamFoundationServiceException
  {
    public InvalidRequestException()
    {
    }

    public InvalidRequestException(string message)
      : base(message)
    {
    }

    public InvalidRequestException(string message, Exception innerException)
      : base(message, innerException)
    {
    }

    [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "requestContext", Justification = "Needed by the framework")]
    [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "errorNumber", Justification = "Needed by the framework")]
    [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "sqlError", Justification = "Needed by the framework")]
    public InvalidRequestException(
      IVssRequestContext requestContext,
      int errorNumber,
      SqlException ex,
      SqlError sqlError)
      : base(InvalidRequestException.GetErrorMessage(ex), 700003)
    {
    }

    protected InvalidRequestException(SerializationInfo info, StreamingContext context)
      : base(info, context)
    {
    }

    private static string GetErrorMessage(SqlException ex) => ex != null ? ex.Message : string.Empty;
  }
}
