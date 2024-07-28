// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Data.Components.DefinitionEnvironmentSqlComponent
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Data, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A69B0FB7-3028-4162-BA38-794D80D7A49A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Data.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model;
using System;
using System.Diagnostics.CodeAnalysis;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Data.Components
{
  public class DefinitionEnvironmentSqlComponent : ReleaseManagementSqlResourceComponentBase
  {
    [SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification = "This signature is required by VSSF. Don't change.")]
    public static readonly ComponentFactory ComponentFactory = new ComponentFactory(new IComponentCreator[4]
    {
      (IComponentCreator) new ComponentCreator<DefinitionEnvironmentSqlComponent>(0),
      (IComponentCreator) new ComponentCreator<DefinitionEnvironmentSqlComponent1>(1),
      (IComponentCreator) new ComponentCreator<DefinitionEnvironmentSqlComponent2>(2),
      (IComponentCreator) new ComponentCreator<DefinitionEnvironmentSqlComponent3>(3)
    }, "ReleaseManagementDefinitionEnvironment", "ReleaseManagement");

    public virtual DefinitionEnvironment GetDefinitionEnvironment(
      Guid projectId,
      int releaseDefinitionId,
      int definitionEnvironmentId)
    {
      return (DefinitionEnvironment) null;
    }
  }
}
