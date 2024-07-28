// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Documents.Client.ResourceResponse`1
// Assembly: Microsoft.Azure.Cosmos.Direct, Version=3.29.4.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: FFE3C00D-4333-4294-8947-B1C93A852E2F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Direct.dll

namespace Microsoft.Azure.Documents.Client
{
  internal class ResourceResponse<TResource> : 
    ResourceResponseBase,
    IResourceResponse<TResource>,
    IResourceResponseBase
    where TResource : Microsoft.Azure.Documents.Resource, new()
  {
    private TResource resource;
    private ITypeResolver<TResource> typeResolver;

    public ResourceResponse()
    {
    }

    public ResourceResponse(TResource resource)
      : this()
    {
      this.resource = resource;
    }

    internal ResourceResponse(
      DocumentServiceResponse response,
      ITypeResolver<TResource> typeResolver = null)
      : base(response)
    {
      this.typeResolver = typeResolver;
    }

    public TResource Resource
    {
      get
      {
        if ((object) this.resource == null)
          this.resource = this.response.GetResource<TResource>(this.typeResolver);
        return this.resource;
      }
    }

    public static implicit operator TResource(ResourceResponse<TResource> source) => source.Resource;
  }
}
