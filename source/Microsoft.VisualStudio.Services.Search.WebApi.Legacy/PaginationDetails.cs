// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.WebApi.PaginationDetails
// Assembly: Microsoft.VisualStudio.Services.Search.WebApi.Legacy, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 5D4CB2D3-3C08-46C7-B9C5-51E638F57F9E
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Search.WebApi.Legacy.dll

using Microsoft.VisualStudio.Services.Search.Shared.WebApi.Legacy;
using System;
using System.Runtime.Serialization;
using System.Text;

namespace Microsoft.VisualStudio.Services.Search.WebApi
{
  [DataContract]
  [Serializable]
  public class PaginationDetails
  {
    [DataMember(Name = "pageId")]
    public string PageId { get; set; }

    public string ToString(int indentLevel)
    {
      StringBuilder sb = new StringBuilder();
      sb.Append(Extensions.GetIndentSpacing(indentLevel), "PageId: ").AppendLine(this.PageId.ToString());
      return sb.ToString();
    }

    public override string ToString() => this.ToString(0);
  }
}
