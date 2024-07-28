// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.FeatureFlagAvailabilityColumns
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System.Data;

namespace Microsoft.TeamFoundation.Framework.Server
{
  internal class FeatureFlagAvailabilityColumns : ObjectBinder<FeatureFlagAvailabilityContainer>
  {
    private SqlColumnBinder RegistryFeaturePathRaw = new SqlColumnBinder(nameof (RegistryFeaturePathRaw));
    private SqlColumnBinder FeatureState = new SqlColumnBinder(nameof (FeatureState));
    private SqlColumnBinder HostId = new SqlColumnBinder(nameof (HostId));

    protected override FeatureFlagAvailabilityContainer Bind() => new FeatureFlagAvailabilityContainer()
    {
      RegistryFeaturePathRaw = this.RegistryFeaturePathRaw.GetString((IDataReader) this.Reader, false),
      FeatureState = this.FeatureState.GetString((IDataReader) this.Reader, false),
      HostId = this.HostId.GetGuid((IDataReader) this.Reader, false)
    };
  }
}
