// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.TestManagement.SharedStepModel
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.TestManagement, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 2E4165D5-898A-42D9-B816-9FABF135E4DA
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.TestManagement.dll

using Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems.DataModels;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.Server.WebAccess.TestManagement
{
  [DataContract]
  internal class SharedStepModel : TestActionModel
  {
    [DataMember(Name = "refe")]
    public int Refe { get; set; }

    public static SharedStepModel CreateSharedStep(int id, int refe)
    {
      SharedStepModel sharedStep = new SharedStepModel();
      sharedStep.Id = id;
      sharedStep.Refe = refe;
      return sharedStep;
    }

    public override bool ProcessTestAttachments(List<WorkItemResourceLinkInfo> links) => false;
  }
}
