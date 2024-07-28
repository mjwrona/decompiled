// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.PermissionsReport.PermissionsReportRequestInvalidException
// Assembly: Microsoft.TeamFoundation.PermissionsReport, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 5F2A073E-CE9C-4394-ABB1-512D7FB702F5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.PermissionsReport.dll

using System;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.PermissionsReport
{
  [Serializable]
  public class PermissionsReportRequestInvalidException : PermissionsReportException
  {
    public PermissionsReportRequestInvalidException()
    {
    }

    public PermissionsReportRequestInvalidException(string message)
      : base(message)
    {
    }

    public PermissionsReportRequestInvalidException(string message, Exception innerException)
      : base(message, innerException)
    {
    }

    protected PermissionsReportRequestInvalidException(
      SerializationInfo info,
      StreamingContext context)
      : base(info, context)
    {
    }
  }
}
