// Decompiled with JetBrains decompiler
// Type: Nest.DeleteLicenseDescriptor
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net.Specification.LicenseApi;

namespace Nest
{
  public class DeleteLicenseDescriptor : 
    RequestDescriptorBase<DeleteLicenseDescriptor, DeleteLicenseRequestParameters, IDeleteLicenseRequest>,
    IDeleteLicenseRequest,
    IRequest<DeleteLicenseRequestParameters>,
    IRequest
  {
    internal override ApiUrls ApiUrls => ApiUrlsLookups.LicenseDelete;
  }
}
