// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems.DataModels.WorkItemIdentity
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

using Microsoft.VisualStudio.Services.WebApi;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems.DataModels
{
  public class WorkItemIdentity
  {
    public string DistinctDisplayName { get; set; }

    public IdentityRef IdentityRef { get; set; }

    public string SecurityToken { get; internal set; }

    public bool HasPermission { get; internal set; }

    public override bool Equals(object obj)
    {
      if (!(obj is WorkItemIdentity workItemIdentity) || !string.Equals(this.DistinctDisplayName, workItemIdentity.DistinctDisplayName))
        return false;
      if (this.IdentityRef == null && workItemIdentity.IdentityRef == null)
        return true;
      return this.IdentityRef != null && workItemIdentity.IdentityRef != null && object.Equals((object) this.IdentityRef.Descriptor, (object) workItemIdentity.IdentityRef.Descriptor);
    }

    public override int GetHashCode()
    {
      string str = this.ToString();
      return str == null ? -1 : str.GetHashCode();
    }

    public override string ToString()
    {
      if (string.IsNullOrEmpty(this.DistinctDisplayName) && this.IdentityRef != null)
      {
        if (this.IdentityRef.Descriptor.ToString() != null)
          return string.Format("{0}", (object) this.IdentityRef.Descriptor);
        if (!string.IsNullOrEmpty(this.IdentityRef.DisplayName))
          return this.IdentityRef.DisplayName;
      }
      return this.DistinctDisplayName;
    }
  }
}
