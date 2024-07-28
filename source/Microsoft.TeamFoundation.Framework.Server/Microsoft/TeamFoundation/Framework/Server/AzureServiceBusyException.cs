// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.AzureServiceBusyException
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.Framework.Server
{
  [Serializable]
  public class AzureServiceBusyException : SqlAzureException
  {
    private SqlAzureReason m_reason = new SqlAzureReason();

    public AzureServiceBusyException(SqlException sqlException)
      : base(FrameworkResources.ErrorWhileProcessingResults(), sqlException)
    {
      this.LogLevel = EventLogEntryType.Information;
      this.EventId = TeamFoundationEventId.SqlAzureTooBusy;
      this.m_reason = this.ParseReasonCode(sqlException.Message);
    }

    public AzureServiceBusyException(
      IVssRequestContext requestContext,
      SqlException sqlException,
      SqlError sqlError)
      : this(sqlException)
    {
    }

    protected AzureServiceBusyException(SerializationInfo info, StreamingContext context)
      : base(info, context)
    {
      this.m_reason = (SqlAzureReason) info.GetValue(nameof (Reason), typeof (SqlAzureReason));
    }

    public override void GetObjectData(SerializationInfo info, StreamingContext context)
    {
      base.GetObjectData(info, context);
      info.AddValue("Reason", (object) this.Reason, typeof (SqlAzureReason));
    }

    public SqlAzureReason Reason => this.m_reason;

    public override string Message => base.Message + Environment.NewLine + this.m_reason.ToString();

    private SqlAzureReason ParseReasonCode(string errorMessage)
    {
      SqlAzureReason reasonCode = new SqlAzureReason();
      try
      {
        string str = "Code: ";
        int num1;
        if ((num1 = errorMessage.IndexOf(str, StringComparison.OrdinalIgnoreCase)) != -1)
        {
          int num2;
          if ((num2 = errorMessage.IndexOf(" ", num1 + str.Length, StringComparison.OrdinalIgnoreCase)) != -1)
          {
            int result;
            if (int.TryParse(errorMessage.Substring(num1 + str.Length, num2 - num1 - str.Length), out result))
            {
              reasonCode.ThrottlingMode = (SqlAzureThrottlingMode) (result % 4);
              reasonCode.MildResourceType = (SqlAzureResourceType) (result >> 8 & (int) byte.MaxValue);
              reasonCode.SignificantResourceType = (SqlAzureResourceType) (result >> 16 & (int) byte.MaxValue);
            }
          }
        }
      }
      catch (Exception ex)
      {
        TeamFoundationTrace.TraceException(ex);
      }
      return reasonCode;
    }
  }
}
