// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.ArtifactPagingManager`1
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

namespace Microsoft.TeamFoundation.Framework.Server
{
  internal abstract class ArtifactPagingManager<T> : DbPagingManager<T>
  {
    protected TeamFoundationPropertyService m_propertyService;
    protected PropertyComponent m_component;

    public ArtifactPagingManager(IVssRequestContext requestContext, PropertyComponent component)
      : base(requestContext, false)
    {
      using (requestContext.AcquireExemptionLock())
        this.m_propertyService = requestContext.GetService<TeamFoundationPropertyService>();
      this.m_XmlParameterChunkThreshold = DbPagingManagerSettings.XmlParameterChunkThresholdSettingInBytes;
      this.m_component = component;
    }

    public ArtifactKind PagedArtifactKind { get; private set; }

    protected void UpdatePagedArtifactKind(ArtifactSpec artifactSpec)
    {
      if (this.PagedArtifactKind == null)
      {
        this.PagedArtifactKind = this.m_propertyService.GetArtifactKind(this.m_requestContext, artifactSpec.Kind);
        this.m_dataspaceCategory = this.PagedArtifactKind.DataspaceCategory;
      }
      else
      {
        if (artifactSpec.Kind != this.PagedArtifactKind.Kind)
          throw new ArtifactKindsMustBeUniformException();
        if (!this.PagedArtifactKind.IsMonikerBased && !string.IsNullOrEmpty(artifactSpec.Moniker))
          throw new TeamFoundationValidationException(FrameworkResources.ArtifactKindMonikerDisallowed((object) this.PagedArtifactKind.Kind), nameof (artifactSpec));
      }
    }
  }
}
