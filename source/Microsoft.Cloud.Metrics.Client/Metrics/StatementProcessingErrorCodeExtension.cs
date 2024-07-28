// Decompiled with JetBrains decompiler
// Type: Microsoft.Cloud.Metrics.Client.Metrics.StatementProcessingErrorCodeExtension
// Assembly: Microsoft.Cloud.Metrics.Client, Version=2.2023.705.2051, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 06B39E1C-7DF0-4BC1-AFBA-9AD635E73CB0
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Cloud.Metrics.Client.dll

using System;
using System.Linq;

namespace Microsoft.Cloud.Metrics.Client.Metrics
{
  public static class StatementProcessingErrorCodeExtension
  {
    public static string ToStringRepresentation(this StatementProcessingErrorCode errorCode) => (errorCode >= StatementProcessingErrorCode.GeneralRuntimeMessage ? (errorCode >= (StatementProcessingErrorCode) 6000 ? (errorCode >= StatementProcessingErrorCode.Information ? "RI" : "RW") : "RE") : "CE") + ((int) errorCode).ToString().PadLeft(4, '0');

    public static StatementProcessingErrorCode ToErrorCodeRepresentation(string stringRepresentation)
    {
      string s = new string(stringRepresentation.Where<char>(new Func<char, bool>(char.IsDigit)).ToArray<char>());
      int codeRepresentation = 0;
      ref int local = ref codeRepresentation;
      int.TryParse(s, out local);
      return (StatementProcessingErrorCode) codeRepresentation;
    }
  }
}
