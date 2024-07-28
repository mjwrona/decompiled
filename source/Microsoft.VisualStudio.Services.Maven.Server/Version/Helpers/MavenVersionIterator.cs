// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Maven.Server.Version.Helpers.MavenVersionIterator
// Assembly: Microsoft.VisualStudio.Services.Maven.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3AEBE02E-FDD2-41D8-89F7-5C54445DBFA7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Maven.Server.dll

using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Maven.Server.Version.Helpers
{
  public class MavenVersionIterator
  {
    private List<IMavenVersionItem>.Enumerator enumerator;
    private IMavenVersionItem next;
    private bool nextAvailable;
    private bool nextFetched;

    public MavenVersionIterator(List<IMavenVersionItem>.Enumerator enumerator) => this.enumerator = enumerator;

    public MavenVersionIterator(List<IMavenVersionItem> list)
      : this(list.GetEnumerator())
    {
    }

    public void Dispose() => this.enumerator.Dispose();

    public bool HasNext()
    {
      this.CheckNext();
      return this.nextAvailable;
    }

    public IMavenVersionItem Next()
    {
      this.CheckNext();
      if (!this.nextAvailable)
        return (IMavenVersionItem) null;
      this.nextFetched = false;
      return this.next;
    }

    private void CheckNext()
    {
      if (this.nextFetched)
        return;
      this.nextAvailable = this.enumerator.MoveNext();
      if (this.nextAvailable)
        this.next = this.enumerator.Current;
      this.nextFetched = true;
    }
  }
}
