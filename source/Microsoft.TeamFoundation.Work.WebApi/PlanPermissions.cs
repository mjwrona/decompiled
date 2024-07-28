// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Work.WebApi.PlanPermissions
// Assembly: Microsoft.TeamFoundation.Work.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0C4CCFA0-0616-4E48-A4F0-952E1CB10B12
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Work.WebApi.dll

using System;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.Work.WebApi
{
  [Obsolete("This class is deprecated and will not be populated. Use PlanUserPermissions instead")]
  public class PlanPermissions
  {
    [DataMember(Name = "edit")]
    public bool Edit { get; set; }

    [DataMember(Name = "delete")]
    public bool Delete { get; set; }
  }
}
