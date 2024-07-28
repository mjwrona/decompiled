// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Aad.GetDirectoryRolesWithIdsResponse
// Assembly: Microsoft.VisualStudio.Services.Cloud, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F2D48A0E-C4C9-4233-BA34-E8461823F6F2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cloud.dll

using Microsoft.VisualStudio.Services.Cloud.AzureActiveDirectory;
using System;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Aad
{
  public class GetDirectoryRolesWithIdsResponse : AadServiceResponse
  {
    public IDictionary<Guid, AadDirectoryRole> DirectoryRoles { get; set; }

    public override string CompareAndGetDifference(AadServiceResponse anotherResponse) => !(anotherResponse is GetDirectoryRolesWithIdsResponse rolesWithIdsResponse) ? string.Format("Target response is null or not of type '{0}'.", (object) typeof (GetDirectoryRolesWithIdsResponse)) : AadObjectCompareHelpers.CompareAndGetDifferenceAadDirectoryRolesDict(this.DirectoryRoles, rolesWithIdsResponse.DirectoryRoles);
  }
}
