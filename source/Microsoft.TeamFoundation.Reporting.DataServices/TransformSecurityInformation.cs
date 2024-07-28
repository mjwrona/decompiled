// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Reporting.DataServices.TransformSecurityInformation
// Assembly: Microsoft.TeamFoundation.Reporting.DataServices, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0871DF71-209E-4628-905A-D95195A70FEC
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Reporting.DataServices.dll

using Microsoft.VisualStudio.Services.WebApi;
using System;

namespace Microsoft.TeamFoundation.Reporting.DataServices
{
  public class TransformSecurityInformation
  {
    public TransformSecurityInformation(Guid? projectId, ISecuredObject sharedSecuredObject)
    {
      this.ProjectId = projectId;
      this.SharedSecuredObject = sharedSecuredObject;
    }

    public Guid? ProjectId { get; }

    public ISecuredObject SharedSecuredObject { get; }
  }
}
