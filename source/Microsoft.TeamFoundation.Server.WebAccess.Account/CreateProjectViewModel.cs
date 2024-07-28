// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.Account.CreateProjectViewModel
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.Account, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: AC21A176-69BE-407E-B3DD-80612369F784
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.WebAccess.Account.dll

using System.Globalization;

namespace Microsoft.TeamFoundation.Server.WebAccess.Account
{
  public class CreateProjectViewModel
  {
    public string NextUrl { get; set; }

    public bool IsCompact { get; set; }

    public string Scenario { get; set; }

    public string CurrentUICultureRegion
    {
      get
      {
        string name = CultureInfo.CurrentUICulture.Name;
        string[] strArray1;
        if (name == null)
          strArray1 = (string[]) null;
        else
          strArray1 = name.Split('-');
        string[] strArray2 = strArray1;
        return strArray2 == null || strArray2.Length < 2 ? string.Empty : strArray2[1];
      }
    }
  }
}
