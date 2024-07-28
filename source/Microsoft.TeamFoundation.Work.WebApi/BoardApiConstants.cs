// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Work.WebApi.BoardApiConstants
// Assembly: Microsoft.TeamFoundation.Work.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0C4CCFA0-0616-4E48-A4F0-952E1CB10B12
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Work.WebApi.dll

using System;
using System.ComponentModel;

namespace Microsoft.TeamFoundation.Work.WebApi
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  public class BoardApiConstants
  {
    public static readonly Guid BoardsLocationId = new Guid("23ad19fc-3b8e-4877-8462-b3f92bc06b40");
    public static readonly Guid BoardColumnsLocationId = new Guid("c555d7ff-84e1-47df-9923-a3fe0cd8751b");
    public static readonly Guid BoardRowsLocationId = new Guid("0863355d-aefd-4d63-8669-984c9b7b0e78");
    public static readonly Guid BoardChartsLocationId = new Guid("45fe888c-239e-49fd-958c-df1a1ab21d97");
    public static readonly Guid BoardCardSettingsLocationId = new Guid("07C3B467-BC60-4F05-8E34-599CE288FAFC");
    public const string BoardCardSettingsLocationString = "07C3B467-BC60-4F05-8E34-599CE288FAFC";
    public static readonly Guid BoardCardStyleSettingsLocationId = new Guid("B044A3D9-02EA-49C7-91A1-B730949CC896");
    public const string BoardCardStyleSettingsLocationString = "B044A3D9-02EA-49C7-91A1-B730949CC896";
    public static readonly Guid BoardColumnSuggestedValuesLocationId = new Guid("eb7ec5a3-1ba3-4fd1-b834-49a5a387e57d");
    public static readonly Guid BoardRowSuggestedValuesLocationId = new Guid("bb494cc6-a0f5-4c6c-8dca-ea6912e79eb9");
    public static readonly Guid BoardFilterSettingsLocationId = new Guid("cfe2d81b-12ba-4083-9e5a-859818c763e4");
    public static readonly Guid BoardParentsLocationId = new Guid("186abea3-5c35-432f-9e28-7a15b4312a0e");
    public static readonly Guid BoardUserSettingsLocationId = new Guid("b30d9f58-1891-4b0a-b168-c46408f919b0");
    public static readonly Guid BoardBadgeLocationId = new Guid("0120b002-ab6c-4ca0-98cf-a8d7492f865c");
    public static string Boards = "boards";
  }
}
