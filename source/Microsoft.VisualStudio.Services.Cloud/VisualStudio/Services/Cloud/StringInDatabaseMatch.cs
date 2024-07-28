// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Cloud.StringInDatabaseMatch
// Assembly: Microsoft.VisualStudio.Services.Cloud, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F2D48A0E-C4C9-4233-BA34-E8461823F6F2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cloud.dll

namespace Microsoft.VisualStudio.Services.Cloud
{
  internal class StringInDatabaseMatch
  {
    public string DatabaseName { get; set; }

    public string Text { get; set; }

    public string Table { get; set; }

    public string Column { get; set; }

    public override string ToString() => string.Format("Database {0}, Table {1}, Column {2}, String {3}", (object) this.DatabaseName, (object) this.Table, (object) this.Column, (object) this.Text);
  }
}
