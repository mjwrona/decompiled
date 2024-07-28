// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DstsHelper.AuthenticationError
// Assembly: Microsoft.TeamFoundation.DstsHelper, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 08D47267-3A15-4307-BBA0-1792E9C6BDF1
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DstsHelper.dll

using Microsoft.VisualStudio.Services.Common;
using System.Text;

namespace Microsoft.TeamFoundation.DstsHelper
{
  internal class AuthenticationError
  {
    public string Error { get; }

    public string ErrorDescription { get; }

    public AuthenticationError(string error, string errorDescription = null)
    {
      ArgumentUtility.CheckStringForNullOrEmpty(error, nameof (error));
      this.Error = error;
      this.ErrorDescription = errorDescription;
    }

    public override string ToString() => string.IsNullOrEmpty(this.ErrorDescription) ? "error=" + AuthenticationError.EscapeAsQuotedString(this.Error) : "error=" + AuthenticationError.EscapeAsQuotedString(this.Error) + ", error_description=" + AuthenticationError.EscapeAsQuotedString(this.ErrorDescription);

    private static string EscapeAsQuotedString(string str)
    {
      StringBuilder stringBuilder = new StringBuilder();
      stringBuilder.Append('"');
      foreach (char ch in str)
      {
        if ((ch > '\u001F' || ch == '\t') && ch != '\u007F')
        {
          if (ch == '\\' || ch == '"')
            stringBuilder.Append('\\');
          stringBuilder.Append(ch);
        }
      }
      stringBuilder.Append('"');
      return stringBuilder.ToString();
    }
  }
}
