// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Data.Utilities.UnknownReleaseRevisionChangeDetails
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Data, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A69B0FB7-3028-4162-BA38-794D80D7A49A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Data.dll

using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Enumerations;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Data.Utilities
{
  public class UnknownReleaseRevisionChangeDetails : ReleaseRevisionChangeDetails
  {
    public UnknownReleaseRevisionChangeDetails(string message)
    {
      this.Id = ReleaseHistoryMessageId.UnknownChange;
      this.Message = message;
    }

    public string Message { get; set; }

    public override string ToString() => this.Message;
  }
}
