// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.ExternalProviders.GitHub.WebApi.GraphQLQuery
// Assembly: Microsoft.VisualStudio.ExternalProviders.GitHub.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4FE25D33-B783-4B98-BAFC-7E522D8D8D08
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.ExternalProviders.GitHub.WebApi.dll

using System.Text.RegularExpressions;
using System.Web;

namespace Microsoft.VisualStudio.ExternalProviders.GitHub.WebApi
{
  internal class GraphQLQuery
  {
    private static readonly Regex whitespacePattern = new Regex("\\s+", RegexOptions.Compiled);

    public GraphQLQuery(string query) => this.SingleLineValue = query.Contains("#") ? query : GraphQLQuery.whitespacePattern.Replace(query, " ");

    public string SingleLineValue { get; }

    public static string EscapeValue(string value) => HttpUtility.JavaScriptStringEncode(value);
  }
}
