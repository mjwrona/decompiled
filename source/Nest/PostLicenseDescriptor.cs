// Decompiled with JetBrains decompiler
// Type: Nest.PostLicenseDescriptor
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net.Specification.LicenseApi;
using System;

namespace Nest
{
  public class PostLicenseDescriptor : 
    RequestDescriptorBase<PostLicenseDescriptor, PostLicenseRequestParameters, IPostLicenseRequest>,
    IPostLicenseRequest,
    IRequest<PostLicenseRequestParameters>,
    IRequest
  {
    internal override ApiUrls ApiUrls => ApiUrlsLookups.LicensePost;

    public PostLicenseDescriptor Acknowledge(bool? acknowledge = true) => this.Qs(nameof (acknowledge), (object) acknowledge);

    Nest.License IPostLicenseRequest.License { get; set; }

    public PostLicenseDescriptor License(Nest.License license) => this.Assign<Nest.License>(license, (Action<IPostLicenseRequest, Nest.License>) ((a, v) => a.License = v));
  }
}
