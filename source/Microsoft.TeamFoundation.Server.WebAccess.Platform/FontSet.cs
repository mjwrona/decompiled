// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.FontSet
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.Platform, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A6A2C403-5081-466C-A570-9B50BFA8E213
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.Platform.dll

namespace Microsoft.TeamFoundation.Server.WebAccess
{
  internal struct FontSet
  {
    public readonly string Name;
    public readonly string Weight;
    public static readonly FontSet Normal = new FontSet("normal", "normal");
    public static readonly FontSet Light = new FontSet("light", "100");
    public static readonly FontSet SemiLight = new FontSet("semilight", "300");
    public static readonly FontSet Regular = new FontSet("regular", "400");
    public static readonly FontSet SemiBold = new FontSet("semibold", "600");

    public FontSet(string name, string weight)
    {
      this.Name = name;
      this.Weight = weight;
    }
  }
}
