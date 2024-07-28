// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.DelegatedAuthorization.AuthorizationScopeDescription
// Assembly: Microsoft.VisualStudio.Services.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7B264323-C592-4F23-AB6B-55AEDC85864F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.WebApi.dll

using System;

namespace Microsoft.VisualStudio.Services.DelegatedAuthorization
{
  public class AuthorizationScopeDescription
  {
    public static readonly string FallbackMarket = string.Empty;

    public AuthorizationScopeDescription(string market, string title, string description)
    {
      if (market == null)
        throw new ArgumentNullException(nameof (market));
      if (market != AuthorizationScopeDescription.FallbackMarket && string.IsNullOrWhiteSpace(market))
        throw new ArgumentException("Market is required: '" + market + "'");
      if (market != AuthorizationScopeDescription.FallbackMarket && string.IsNullOrWhiteSpace(market))
        throw new ArgumentException("Market is required: '" + market + "'");
      if (title == null)
        throw new ArgumentNullException(nameof (title));
      if (string.IsNullOrWhiteSpace(title))
        throw new ArgumentException("Title is required: '" + title + "'");
      if (description == null)
        throw new ArgumentNullException(nameof (description));
      if (string.IsNullOrWhiteSpace(description))
        throw new ArgumentException("Description is required: '" + description + "'");
      this.Market = market;
      this.Title = title;
      this.Description = description;
    }

    public string Market { get; protected set; }

    public string Title { get; protected set; }

    public string Description { get; protected set; }
  }
}
