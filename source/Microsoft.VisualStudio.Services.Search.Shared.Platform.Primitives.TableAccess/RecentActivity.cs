// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.TableAccess.RecentActivity
// Assembly: Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.TableAccess, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: D3998EAE-13E8-421A-93CB-363047218BB4
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.TableAccess.dll

using System;

namespace Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.TableAccess
{
  public class RecentActivity
  {
    public string ArtifactId { get; set; }

    public Guid IdentityId { get; set; }

    public Guid ProjectId { get; set; }

    public int AreaId { get; set; }

    public DateTime ActivityDate { get; set; }

    public override string ToString() => "ArtifactId: " + this.ArtifactId.ToString() + "IdentityId: " + this.IdentityId.ToString() + "ProjectId: " + this.ProjectId.ToString() + "AreaId: " + this.AreaId.ToString() + "ActivityDate: " + this.ActivityDate.ToString();
  }
}
