// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Maven.Server.Version.Items.MavenVersionList
// Assembly: Microsoft.VisualStudio.Services.Maven.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3AEBE02E-FDD2-41D8-89F7-5C54445DBFA7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Maven.Server.dll

using Microsoft.VisualStudio.Services.Maven.Server.Version.Helpers;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.VisualStudio.Services.Maven.Server.Version.Items
{
  public class MavenVersionList : List<IMavenVersionItem>, IMavenVersionItem
  {
    public bool IsEquivalentToNull() => this.Count == 0;

    public void Normalize()
    {
      for (int index = this.Count - 1; index >= 0; --index)
      {
        IMavenVersionItem mavenVersionItem = this[index];
        if (mavenVersionItem.IsEquivalentToNull())
          this.Remove(mavenVersionItem);
        else if (!(mavenVersionItem is MavenVersionList))
          break;
      }
    }

    public override string ToString()
    {
      StringBuilder stringBuilder = new StringBuilder();
      foreach (IMavenVersionItem mavenVersionItem in (List<IMavenVersionItem>) this)
      {
        if (stringBuilder.Length > 0)
          stringBuilder.Append(mavenVersionItem is MavenVersionList ? '-' : '.');
        stringBuilder.Append((object) mavenVersionItem);
      }
      return stringBuilder.ToString().ToLowerInvariant();
    }
  }
}
