// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Gallery.Server.Components.ExtensionVersionValidationStepBinder
// Assembly: Microsoft.VisualStudio.Services.Gallery.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B9EBBED5-135E-45CD-B0B4-F747360599CD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Gallery.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System.Data;

namespace Microsoft.VisualStudio.Services.Gallery.Server.Components
{
  internal class ExtensionVersionValidationStepBinder : ObjectBinder<ExtensionVersionValidationStep>
  {
    protected SqlColumnBinder stepIdColumn = new SqlColumnBinder("StepId");
    protected SqlColumnBinder parentIdColumn = new SqlColumnBinder("ParentId");
    protected SqlColumnBinder stepTypeColumn = new SqlColumnBinder("StepType");
    protected SqlColumnBinder stepStatusColumn = new SqlColumnBinder("StepStatus");
    protected SqlColumnBinder startTimeColumn = new SqlColumnBinder("StartTime");
    protected SqlColumnBinder lastUpdatedColumn = new SqlColumnBinder("LastUpdated");
    protected SqlColumnBinder validationContextColumn = new SqlColumnBinder("ValidationContext");
    protected SqlColumnBinder resultFileIdColumn = new SqlColumnBinder("ResultFileId");

    protected override ExtensionVersionValidationStep Bind() => new ExtensionVersionValidationStep()
    {
      StepId = this.stepIdColumn.GetGuid((IDataReader) this.Reader),
      ParentId = this.parentIdColumn.GetGuid((IDataReader) this.Reader),
      StepType = this.stepTypeColumn.GetInt32((IDataReader) this.Reader),
      StepStatus = this.stepStatusColumn.GetInt32((IDataReader) this.Reader),
      StartTime = this.startTimeColumn.GetDateTime((IDataReader) this.Reader),
      LastUpdated = this.lastUpdatedColumn.GetDateTime((IDataReader) this.Reader),
      ValidationContext = this.validationContextColumn.GetString((IDataReader) this.Reader, true),
      ResultFileId = this.resultFileIdColumn.GetInt32((IDataReader) this.Reader, -1)
    };
  }
}
