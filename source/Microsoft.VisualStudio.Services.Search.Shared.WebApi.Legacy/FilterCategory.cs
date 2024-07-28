// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Shared.WebApi.Legacy.FilterCategory
// Assembly: Microsoft.VisualStudio.Services.Search.Shared.WebApi.Legacy, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8C7C9E57-44B4-4654-9458-CC8B59C2B681
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Search.Shared.WebApi.Legacy.dll

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Microsoft.VisualStudio.Services.Search.Shared.WebApi.Legacy
{
  [DataContract]
  public class FilterCategory : SearchSecuredObject
  {
    [DataMember(Name = "name")]
    public string Name { get; set; }

    [DataMember(Name = "filters")]
    public IEnumerable<Filter> Filters { get; set; }

    public string ToString(int indentLevel)
    {
      StringBuilder sb = new StringBuilder();
      string indentSpacing = Extensions.GetIndentSpacing(indentLevel);
      sb.Append(indentSpacing, "Name: ").AppendLine(this.Name).Append(indentSpacing, string.Format((IFormatProvider) CultureInfo.InvariantCulture, "FilterCount: {0}", (object) this.Filters.Count<Filter>()));
      return sb.ToString();
    }

    public override string ToString() => this.ToString(0);

    internal override void SetSecuredObject(
      Guid namespaceId,
      int requiredPermissions,
      string token)
    {
      base.SetSecuredObject(namespaceId, requiredPermissions, token);
      IEnumerable<Filter> filters = this.Filters;
      this.Filters = filters != null ? (IEnumerable<Filter>) filters.Select<Filter, Filter>((Func<Filter, Filter>) (i =>
      {
        i.SetSecuredObject(namespaceId, requiredPermissions, token);
        return i;
      })).ToList<Filter>() : (IEnumerable<Filter>) null;
    }
  }
}
