// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Content.Common.ExpirationDateOptions
// Assembly: Microsoft.VisualStudio.Services.Content.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: DC45E7D4-4445-41B3-8FA2-C13CD848D0F1
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Content.Common.dll

using System;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Content.Common
{
  public class ExpirationDateOptions
  {
    public readonly DateTime StartDate;
    public readonly DateTime EndDate;
    private const char Separator = '_';

    public ExpirationDateOptions(string expirationDateOptionFilter)
    {
      if (string.IsNullOrWhiteSpace(expirationDateOptionFilter))
        throw new ArgumentException("ExpirationDateOption cannot be Null.");
      string[] strArray1 = expirationDateOptionFilter.Contains<char>('_') ? expirationDateOptionFilter.Split('_') : throw new ArgumentException(string.Format("ExpirationDateOptions should contain {0} , e.g. 2/2/2020_3/2/2020", (object) '_'));
      if (strArray1.Length != 2)
        throw new ArgumentException("Expiration Date filter should follow the format e.g. 2/2/2020_3/2/2020");
      DateTime dateTime;
      if (string.IsNullOrWhiteSpace(strArray1[0]))
      {
        string[] strArray2 = strArray1;
        dateTime = DateTime.MinValue;
        string str = dateTime.ToString();
        strArray2[0] = str;
      }
      if (string.IsNullOrWhiteSpace(strArray1[1]))
      {
        string[] strArray3 = strArray1;
        dateTime = DateTime.MaxValue;
        string str = dateTime.ToString();
        strArray3[1] = str;
      }
      DateTime result1;
      if (!DateTime.TryParse(strArray1[0], out result1))
        throw new ArgumentException("First parameter of ExpirationDateOptions is invalid, e.g. 2/2/2020_3/2/2020");
      DateTime result2;
      if (!DateTime.TryParse(strArray1[1], out result2))
        throw new ArgumentException("Second parameter of ExpirationDateOptions is invalid, e.g. 2/2/2020_3/2/2020");
      this.StartDate = this.IsValid(result1, result2) ? result1.Date : throw new ArgumentException("Minimum value of ExpirationDateOptions must be lesser than Maximum value. Input Bounds: [MinDate:00:00:00 UTC, January 1, 0001 - MaxDate:23:59:59 UTC, December 31, 9999] e.g. 1/1/0001_12/31/9999");
      dateTime = result2.Date;
      dateTime = dateTime.AddMinutes(1439.0);
      this.EndDate = dateTime.AddSeconds(59.0);
    }

    public override string ToString()
    {
      DateTime dateTime = this.StartDate;
      string str1 = dateTime.ToString();
      dateTime = this.EndDate;
      string str2 = dateTime.ToString();
      return str1 + "_" + str2;
    }

    public bool IsEndDateMaxValue()
    {
      DateTime endDate = this.EndDate;
      DateTime dateTime1 = DateTime.MaxValue;
      dateTime1 = dateTime1.Date;
      dateTime1 = dateTime1.AddMinutes(1439.0);
      DateTime dateTime2 = dateTime1.AddSeconds(59.0);
      return endDate == dateTime2;
    }

    private bool IsValid(DateTime startDate, DateTime endDate) => startDate <= endDate;
  }
}
