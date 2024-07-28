// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.Account.PersonalAccessTokenPageData
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.Account, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: AC21A176-69BE-407E-B3DD-80612369F784
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.WebAccess.Account.dll

using System.Collections.Generic;

namespace Microsoft.TeamFoundation.Server.WebAccess.Account
{
  public class PersonalAccessTokenPageData
  {
    public IList<PersonalAccessTokenDetailsModel> PersonalAccessTokenDetailsModelList { get; set; }

    public int NextRowNumber { get; set; }
  }
}
