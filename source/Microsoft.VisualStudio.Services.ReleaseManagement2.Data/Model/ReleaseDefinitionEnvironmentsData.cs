// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ReleaseDefinitionEnvironmentsData
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Data, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A69B0FB7-3028-4162-BA38-794D80D7A49A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Data.dll

using Microsoft.VisualStudio.Services.ReleaseManagement.WebApi;
using System.Diagnostics.CodeAnalysis;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model
{
  public class ReleaseDefinitionEnvironmentsData
  {
    public int ReleaseDefinitionId { get; set; }

    public string ReleaseDefinitionName { get; set; }

    public int DefinitionEnvironmentId { get; set; }

    public string DefinitionEnvironmentName { get; set; }

    [SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate", Justification = "Required to be a method.")]
    public DefinitionEnvironmentReference GetDefinitionEnvironmentReference() => new DefinitionEnvironmentReference()
    {
      ReleaseDefinitionId = this.ReleaseDefinitionId,
      ReleaseDefinitionName = this.ReleaseDefinitionName,
      DefinitionEnvironmentId = this.DefinitionEnvironmentId,
      DefinitionEnvironmentName = this.DefinitionEnvironmentName
    };
  }
}
