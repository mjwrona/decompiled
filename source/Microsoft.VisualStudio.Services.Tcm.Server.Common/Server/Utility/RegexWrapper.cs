// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.Utility.RegexWrapper
// Assembly: Microsoft.VisualStudio.Services.Tcm.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7631C286-897C-44D1-A133-A0BB6CC047F3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Tcm.Server.Common.dll

using System.Text.RegularExpressions;

namespace Microsoft.TeamFoundation.TestManagement.Server.Utility
{
  public class RegexWrapper : IRegexWrapper
  {
    private Regex regex;

    public RegexWrapper(Regex regex) => this.regex = regex;

    public bool IsMatch(string actualString) => this.regex.IsMatch(actualString);

    public System.Text.RegularExpressions.Match Match(string actualString) => this.regex.Match(actualString);

    public string Replace(string actualString, string replacementString) => this.regex.Replace(actualString, replacementString);
  }
}
