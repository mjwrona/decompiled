// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.TestObjectUpdatedException
// Assembly: Microsoft.VisualStudio.Services.Tcm.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7631C286-897C-44D1-A133-A0BB6CC047F3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Tcm.Server.Common.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Globalization;
using System.Runtime.Serialization;
using System.Security;

namespace Microsoft.TeamFoundation.TestManagement.Server
{
  [CLSCompliant(false)]
  [Serializable]
  public class TestObjectUpdatedException : TestManagementServiceException
  {
    private List<int> m_ids;

    public TestObjectUpdatedException(string message)
      : base(message)
    {
    }

    public TestObjectUpdatedException(string message, Exception innerException)
      : base(message, innerException)
    {
    }

    public TestObjectUpdatedException(
      IVssRequestContext requestContext,
      SqlException ex,
      SqlError sqlError)
      : base(TestObjectUpdatedException.FormatMessage(requestContext, sqlError))
    {
      this.m_ids = new List<int>(2);
      this.m_ids.Add(TeamFoundationServiceException.ExtractInt(sqlError, "id0"));
      this.m_ids.Add(TeamFoundationServiceException.ExtractInt(sqlError, "id1"));
    }

    private static string FormatMessage(IVssRequestContext requestContext, SqlError sqlError) => string.Format((IFormatProvider) CultureInfo.CurrentCulture, ServerResources.TestObjectUpdatedError, (object) TeamFoundationServiceException.ExtractInt(sqlError, "id0"));

    [SecurityCritical]
    public override void GetObjectData(SerializationInfo info, StreamingContext context)
    {
      base.GetObjectData(info, context);
      info.AddValue("Ids", (object) this.Ids);
    }

    public List<int> Ids => this.m_ids;
  }
}
