// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.SqlComparer
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System;
using System.IO;

namespace Microsoft.TeamFoundation.Framework.Server
{
  public abstract class SqlComparer
  {
    public SqlComparer(string db1, string db2)
    {
      this.RootDir = Path.Combine(Path.GetTempPath(), "TfsDbCompare");
      this.Db1Dir = Path.Combine(this.RootDir, db1);
      this.Db2Dir = Path.Combine(this.RootDir, db2);
    }

    public string RootDir { get; private set; }

    public string Db1Dir { get; private set; }

    public string Db2Dir { get; private set; }

    public bool AreEqual(string itemName, string value1, string value2)
    {
      if (string.Equals(this.PrepareForOrdinalComparison(value1), this.PrepareForOrdinalComparison(value2), StringComparison.Ordinal))
        return true;
      Directory.CreateDirectory(this.Db1Dir);
      Directory.CreateDirectory(this.Db2Dir);
      File.WriteAllText(Path.Combine(this.Db1Dir, itemName + ".sql"), value1);
      File.WriteAllText(Path.Combine(this.Db2Dir, itemName + ".sql"), value2);
      return false;
    }

    protected abstract string PrepareForOrdinalComparison(string value);
  }
}
