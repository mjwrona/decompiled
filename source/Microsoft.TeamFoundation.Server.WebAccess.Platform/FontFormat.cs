// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.FontFormat
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.Platform, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A6A2C403-5081-466C-A570-9B50BFA8E213
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.Platform.dll

namespace Microsoft.TeamFoundation.Server.WebAccess
{
  internal struct FontFormat
  {
    public readonly string Name;
    public readonly string Extension;
    public static readonly FontFormat Woff2 = new FontFormat("woff2", ".woff2");
    public static readonly FontFormat Woff = new FontFormat("woff", ".woff");
    public static readonly FontFormat TrueType = new FontFormat("truetype", ".ttf");

    public FontFormat(string name, string extension)
    {
      this.Name = name;
      this.Extension = extension;
    }
  }
}
