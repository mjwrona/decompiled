// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.WebApi.Legacy.CodeResults
// Assembly: Microsoft.VisualStudio.Services.Search.WebApi.Legacy, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 5D4CB2D3-3C08-46C7-B9C5-51E638F57F9E
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Search.WebApi.Legacy.dll

using Microsoft.VisualStudio.Services.Search.Shared.WebApi.Legacy;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Microsoft.VisualStudio.Services.Search.WebApi.Legacy
{
  [DataContract]
  public class CodeResults : SearchSecuredObject
  {
    public CodeResults(int count, IEnumerable<CodeResult> values)
    {
      this.Count = count;
      this.Values = values;
    }

    [DataMember(Name = "count")]
    public int Count { get; set; }

    [DataMember(Name = "values")]
    public IEnumerable<CodeResult> Values { get; set; }

    [SuppressMessage("Microsoft.Usage", "CA2233:OperationsShouldNotOverflow", MessageId = "indentLevel+1", Justification = "indentLevel is not likely to exceed int.Max range.")]
    public string ToString(int indentLevel)
    {
      StringBuilder sb = new StringBuilder();
      string indentSpacing = Extensions.GetIndentSpacing(indentLevel);
      sb.Append(indentSpacing, "Count: ").AppendLine(this.Count.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      foreach (CodeResult codeResult in this.Values)
        sb.Append(codeResult.ToString(indentLevel + 1));
      return sb.ToString();
    }

    public override string ToString() => this.ToString(0);

    internal override void SetSecuredObject(
      Guid namespaceId,
      int requiredPermissions,
      string token)
    {
      base.SetSecuredObject(namespaceId, requiredPermissions, token);
      IEnumerable<CodeResult> values = this.Values;
      this.Values = values != null ? (IEnumerable<CodeResult>) values.Select<CodeResult, CodeResult>((Func<CodeResult, CodeResult>) (i =>
      {
        i.SetSecuredObject(namespaceId, requiredPermissions, token);
        return i;
      })).ToList<CodeResult>() : (IEnumerable<CodeResult>) null;
    }
  }
}
