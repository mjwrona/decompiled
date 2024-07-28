// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.TestManagementServiceException
// Assembly: Microsoft.VisualStudio.Services.Tcm.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7631C286-897C-44D1-A133-A0BB6CC047F3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Tcm.Server.Common.dll

using Microsoft.TeamFoundation.Common;
using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.TestManagement.Server
{
  [CLSCompliant(false)]
  [Serializable]
  public class TestManagementServiceException : TeamFoundationServiceException
  {
    public TestManagementServiceException() => this.Initialize();

    public TestManagementServiceException(string message)
      : base(message)
    {
      this.Initialize();
    }

    public TestManagementServiceException(string message, Exception innerException)
      : base(message, innerException)
    {
      this.Initialize();
    }

    protected TestManagementServiceException(SerializationInfo info, StreamingContext context)
      : base(info, context)
    {
      this.Initialize();
    }

    public override void GetExceptionProperties(ExceptionPropertyCollection properties)
    {
      base.GetExceptionProperties(properties);
      properties.Set("ERRORCODE", this.ErrorCode);
    }

    private void Initialize() => this.EventId = TeamFoundationEventId.TestManagementServiceException;

    internal static int GetTestManagementErrorCode(SqlError sqlError)
    {
      List<int> ints = TeamFoundationServiceException.ExtractInts(sqlError, "error");
      return ints == null || ints.Count == 0 ? -1 : ints[0];
    }
  }
}
