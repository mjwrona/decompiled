// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.AttachmentComparer
// Assembly: Microsoft.VisualStudio.Services.Tcm.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7631C286-897C-44D1-A133-A0BB6CC047F3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Tcm.Server.Common.dll

using System.Collections.Generic;

namespace Microsoft.TeamFoundation.TestManagement.Server
{
  internal class AttachmentComparer : IEqualityComparer<Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.TestResultAttachment>
  {
    public bool Equals(Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.TestResultAttachment attachment1, Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.TestResultAttachment attachment2) => attachment1.Id == attachment2.Id;

    public int GetHashCode(Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.TestResultAttachment attachment) => attachment.Id;
  }
}
