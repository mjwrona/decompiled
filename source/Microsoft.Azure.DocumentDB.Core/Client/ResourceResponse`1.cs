// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Documents.Client.ResourceResponse`1
// Assembly: Microsoft.Azure.DocumentDB.Core, Version=2.10.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 20BB0EB7-1465-494C-8F4E-F898448B85D9
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.DocumentDB.Core.dll

namespace Microsoft.Azure.Documents.Client
{
  public class ResourceResponse<TResource> : 
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
