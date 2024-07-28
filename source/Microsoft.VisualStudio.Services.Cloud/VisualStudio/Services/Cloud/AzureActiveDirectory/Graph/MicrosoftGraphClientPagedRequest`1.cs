// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Cloud.AzureActiveDirectory.Graph.MicrosoftGraphClientPagedRequest`1
// Assembly: Microsoft.VisualStudio.Services.Cloud, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F2D48A0E-C4C9-4233-BA34-E8461823F6F2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cloud.dll

using Microsoft.Graph;
using Microsoft.VisualStudio.Services.Common;
using System;

namespace Microsoft.VisualStudio.Services.Cloud.AzureActiveDirectory.Graph
{
  public abstract class MicrosoftGraphClientPagedRequest<T> : MicrosoftGraphClientRequest<T> where T : MicrosoftGraphClientResponse
  {
    public string PagingToken { get; set; }

    public int? PageSize { get; set; }

    internal string GetSecureNextRequestUrlFromPageToken(GraphServiceClient graphClient)
    {
      Uri result;
      return !string.IsNullOrEmpty(this.PagingToken) && Uri.TryCreate(this.PagingToken, UriKind.RelativeOrAbsolute, out result) ? new Uri(new Uri(((BaseClient) graphClient).BaseUrl, UriKind.Absolute), result.PathAndQuery).AbsoluteUri : (string) null;
    }

    internal override void Validate()
    {
      base.Validate();
      ArgumentUtility.CheckBoundsInclusive(this.PageSize.GetValueOrDefault(), 0, 999, "PageSize");
    }
  }
}
