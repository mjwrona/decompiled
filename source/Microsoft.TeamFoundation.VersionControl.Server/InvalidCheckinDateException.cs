// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.VersionControl.Server.InvalidCheckinDateException
// Assembly: Microsoft.TeamFoundation.VersionControl.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: C5D9EE74-8805-4E00-959F-39760D2358B5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.VersionControl.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Data.SqlClient;
using System.Globalization;

namespace Microsoft.TeamFoundation.VersionControl.Server
{
  [Serializable]
  public class InvalidCheckinDateException : ServerException
  {
    private string m_message;

    public InvalidCheckinDateException(
      IVssRequestContext requestContext,
      SqlException ex,
      SqlError sqlError)
      : base(string.Empty)
    {
      foreach (int num in TeamFoundationServiceException.ExtractInts(sqlError, "error"))
      {
        switch (num)
        {
          case 500210:
            this.m_message = Resources.Format("CheckinDateOlderThanLastCheckin", (object) DateTime.Parse(TeamFoundationServiceException.ExtractString(sqlError, "date"), (IFormatProvider) CultureInfo.CurrentCulture, DateTimeStyles.AssumeUniversal));
            break;
          case 500211:
            this.m_message = Resources.Format("CheckinDateNewerThanServerTime", (object) DateTime.Parse(TeamFoundationServiceException.ExtractString(sqlError, "date"), (IFormatProvider) CultureInfo.CurrentCulture, DateTimeStyles.AssumeUniversal));
            break;
          case 500213:
            this.m_message = Resources.Get("CheckinDateOverflow");
            break;
        }
        if (!string.IsNullOrEmpty(this.m_message))
          break;
      }
    }

    public override string Message => this.m_message;
  }
}
