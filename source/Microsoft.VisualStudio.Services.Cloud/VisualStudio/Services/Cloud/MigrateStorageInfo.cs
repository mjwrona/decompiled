// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Cloud.MigrateStorageInfo
// Assembly: Microsoft.VisualStudio.Services.Cloud, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F2D48A0E-C4C9-4233-BA34-E8461823F6F2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cloud.dll

namespace Microsoft.VisualStudio.Services.Cloud
{
  public class MigrateStorageInfo
  {
    public MigrateStorageInfo(string name, string drawer, string lookupKey)
    {
      this.Name = name;
      this.Drawer = drawer;
      this.LookupKey = lookupKey;
    }

    public string Name { get; set; }

    public string Drawer { get; set; }

    public string LookupKey { get; set; }
  }
}
