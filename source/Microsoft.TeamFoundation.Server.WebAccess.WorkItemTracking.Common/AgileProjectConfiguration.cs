// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.AgileProjectConfiguration
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8CB058E6-FA40-46B0-B867-53112DFAAB81
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.dll

using Microsoft.TeamFoundation.Framework.Server;

namespace Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common
{
  [Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.SettingsPropertyName("TFS.Agile.ProjectConfiguration")]
  [ClassVisibility(ClientVisibility.Public, ClientVisibility.Public)]
  public class AgileProjectConfiguration : ISupportValidation
  {
    public const string SettingsPropertyName = "TFS.Agile.ProjectConfiguration";

    public AgileProjectConfiguration()
    {
      this.ProductBacklog = new ProductBacklogConfiguration();
      this.IterationBacklog = new IterationBacklogConfiguration();
    }

    public ProductBacklogConfiguration ProductBacklog { get; set; }

    public IterationBacklogConfiguration IterationBacklog { get; set; }

    public void Validate(
      IVssRequestContext requestContext,
      string projectUri,
      bool correctWarnings)
    {
      new AgileSettingsValidator(this).Validate(requestContext, projectUri, false);
    }

    public void ValidateStructure() => new AgileSettingsValidator(this).ValidateBasicStructure();

    internal bool IsDefault => (this.IterationBacklog.Columns == null || this.IterationBacklog.Columns.Length == 0) && (this.ProductBacklog.Columns == null || this.ProductBacklog.Columns.Length == 0);
  }
}
