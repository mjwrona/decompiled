// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.Models.AccountTrialModeModel
// Assembly: Microsoft.TeamFoundation.Server.WebAccess, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A2CCA8C5-6910-48A5-82D9-BDC1350B5B4D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.dll

using System;

namespace Microsoft.TeamFoundation.Server.WebAccess.Models
{
  public class AccountTrialModeModel
  {
    public AccountTrialModeModel(DateTime? startDate, DateTime? endDate)
    {
      if (startDate.HasValue && endDate.HasValue)
      {
        DateTime utcNow = DateTime.UtcNow;
        this.StartDate = startDate.Value;
        this.EndDate = endDate.Value;
        DateTime dateTime1 = utcNow;
        DateTime? nullable = startDate;
        if ((nullable.HasValue ? (dateTime1 > nullable.GetValueOrDefault() ? 1 : 0) : 0) != 0)
        {
          DateTime dateTime2 = utcNow;
          nullable = endDate;
          if ((nullable.HasValue ? (dateTime2 < nullable.GetValueOrDefault() ? 1 : 0) : 0) != 0)
          {
            this.IsAccountEligibleForTrialMode = false;
            this.IsAccountInTrialMode = true;
            this.DaysLeftOnTrialMode = (endDate.Value - utcNow).Days;
            return;
          }
        }
        DateTime dateTime3 = utcNow;
        nullable = endDate;
        if ((nullable.HasValue ? (dateTime3 > nullable.GetValueOrDefault() ? 1 : 0) : 0) == 0)
          return;
        this.IsAccountTrialModeExpired = true;
        this.IsAccountEligibleForTrialMode = false;
        this.IsAccountInTrialMode = false;
      }
      else
      {
        this.IsAccountEligibleForTrialMode = true;
        this.IsAccountInTrialMode = false;
        this.StartDate = DateTime.UtcNow;
        this.EndDate = DateTime.UtcNow;
      }
    }

    public DateTime StartDate { get; private set; }

    public DateTime EndDate { get; private set; }

    public bool IsAccountInTrialMode { get; private set; }

    public bool IsAccountEligibleForTrialMode { get; private set; }

    public bool IsAccountTrialModeExpired { get; private set; }

    public int DaysLeftOnTrialMode { get; private set; }
  }
}
