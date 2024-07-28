// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.BlobStore.Common.ProjectDomainId
// Assembly: Microsoft.VisualStudio.Services.BlobStore.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: FAFB0281-5CF2-4D3F-992C-49FBB9BEC906
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.BlobStore.Common.dll

using Newtonsoft.Json;
using System;

namespace Microsoft.VisualStudio.Services.BlobStore.Common
{
  [CLSCompliant(false)]
  [Serializable]
  public class ProjectDomainId : IDomainId
  {
    public Guid ProjectId { get; }

    public ByteDomainId PhysicalDomainId { get; }

    private ProjectDomainId()
    {
    }

    [JsonConstructor]
    public ProjectDomainId(Guid projectId, ByteDomainId physicalDomainId)
    {
      this.ProjectId = projectId;
      this.PhysicalDomainId = physicalDomainId;
    }

    public static bool TryParse(string input, out ProjectDomainId result, out string error)
    {
      result = (ProjectDomainId) null;
      if (string.IsNullOrWhiteSpace(input))
      {
        error = Resources.DomainIdNullError();
        return false;
      }
      string[] strArray = input.Split('_');
      if (strArray.Length != 2)
      {
        error = Resources.InvalidProjectDomainIdFormatError((object) input);
        return false;
      }
      Guid result1;
      if (!Guid.TryParse(strArray[0], out result1) || result1 == Guid.Empty)
      {
        error = Resources.InvalidProjectIdError((object) strArray[0]);
        return false;
      }
      byte result2;
      string error1;
      if (!ByteDomainId.TryParse(strArray[1], out result2, out error1, out bool _))
      {
        error = Resources.InvalidPhysicalDomainIdError((object) error1);
        return false;
      }
      error = (string) null;
      result = new ProjectDomainId(result1, new ByteDomainId(result2));
      return true;
    }

    public override bool Equals(IDomainId other) => other is ProjectDomainId projectDomainId && projectDomainId != null && this.ProjectId == projectDomainId.ProjectId && this.PhysicalDomainId.Equals((IDomainId) projectDomainId.PhysicalDomainId);

    public override string Serialize() => string.Format("{0}_{1}", (object) this.ProjectId, (object) this.PhysicalDomainId.Serialize());

    public override int GetHashCode() => this.Serialize().GetHashCode();
  }
}
