// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.WebApi.VssConnectionHelper
// Assembly: Microsoft.VisualStudio.Services.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7B264323-C592-4F23-AB6B-55AEDC85864F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.WebApi.dll

using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Location;
using Microsoft.VisualStudio.Services.Organization;
using System;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.WebApi
{
  public static class VssConnectionHelper
  {
    private static readonly VssConnectionHelper.OrganizationHelper s_organizationHelper = new VssConnectionHelper.OrganizationHelper("https://app.vssps.visualstudio.com", Guid.Parse("79134C72-4A58-4B42-976C-04E7115F32BF"));

    public static async Task<Uri> GetOrganizationUrlAsync(
      string organizationName,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return await VssConnectionHelper.s_organizationHelper.GetUrlAsync(organizationName, cancellationToken);
    }

    public static async Task<Uri> GetOrganizationUrlAsync(
      Guid organizationId,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return await VssConnectionHelper.s_organizationHelper.GetUrlAsync(organizationId, cancellationToken);
    }

    internal class OrganizationHelper
    {
      private readonly HttpClient m_client;
      private readonly string m_locationServiceUrl;
      private readonly Guid m_resourceAreaId;

      public OrganizationHelper(string locationServiceUrl, Guid resourceAreaId)
      {
        this.m_locationServiceUrl = locationServiceUrl;
        this.m_resourceAreaId = resourceAreaId;
        this.m_client = new HttpClient();
      }

      public async Task<Uri> GetUrlAsync(
        string organizationName,
        CancellationToken cancellationToken = default (CancellationToken))
      {
        ArgumentUtility.CheckStringForNullOrEmpty(organizationName, nameof (organizationName));
        HttpResponseMessage async = await this.m_client.GetAsync(string.Format("{0}/_apis/resourceAreas/{1}/?accountName={2}&api-version=5.0-preview.1", (object) this.m_locationServiceUrl, (object) this.m_resourceAreaId, (object) organizationName), cancellationToken);
        if (async.StatusCode == HttpStatusCode.OK)
        {
          ResourceAreaInfo resourceAreaInfo = await async.Content.ReadAsAsync<ResourceAreaInfo>(cancellationToken);
          if (resourceAreaInfo != null)
            return new Uri(resourceAreaInfo.LocationUrl);
        }
        throw new OrganizationNotFoundException(organizationName);
      }

      public async Task<Uri> GetUrlAsync(Guid organizationId, CancellationToken cancellationToken = default (CancellationToken))
      {
        ArgumentUtility.CheckForEmptyGuid(organizationId, nameof (organizationId));
        HttpResponseMessage async = await this.m_client.GetAsync(string.Format("{0}/_apis/resourceAreas/{1}/?hostId={2}&api-version=5.0-preview.1", (object) this.m_locationServiceUrl, (object) this.m_resourceAreaId, (object) organizationId), cancellationToken);
        if (async.StatusCode == HttpStatusCode.OK)
        {
          ResourceAreaInfo resourceAreaInfo = await async.Content.ReadAsAsync<ResourceAreaInfo>(cancellationToken);
          if (resourceAreaInfo != null)
            return new Uri(resourceAreaInfo.LocationUrl);
        }
        throw new OrganizationNotFoundException(organizationId.ToString());
      }
    }
  }
}
