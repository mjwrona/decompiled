// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Common.AbstractJobYieldData
// Assembly: Microsoft.VisualStudio.Services.Search.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8E09DCBA-148E-4EB7-BB73-B53B030BE93E
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.Common.dll

using System.Runtime.Serialization;
using System.Text;

namespace Microsoft.VisualStudio.Services.Search.Common
{
  [DataContract]
  public abstract class AbstractJobYieldData
  {
    protected AbstractJobYieldData()
    {
      this.IsPrimaryCrawlTraversal = true;
      this.IncompleteTreeCrawl = false;
    }

    public void InitCrawlResumeCounters()
    {
      this.IsPrimaryCrawlTraversal = false;
      this.IncompleteTreeCrawl = false;
    }

    [DataMember]
    public bool IsPrimaryCrawlTraversal { get; set; }

    [DataMember]
    public bool IncompleteTreeCrawl { get; set; }

    public abstract bool HasData();

    public abstract AbstractJobYieldData Clone();

    public override string ToString()
    {
      StringBuilder stringBuilder = new StringBuilder();
      stringBuilder.Append("[IsPrimaryCrawlTraversal: ");
      stringBuilder.Append(this.IsPrimaryCrawlTraversal);
      stringBuilder.Append(", IncompleteTreeCrawl: ");
      stringBuilder.Append(this.IncompleteTreeCrawl);
      stringBuilder.Append("]");
      return stringBuilder.ToString();
    }
  }
}
