// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Data.DataBinders.DefinitionEnvironmentBinder2
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Data, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A69B0FB7-3028-4162-BA38-794D80D7A49A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Data.dll

using Microsoft.TeamFoundation.DistributedTask.Common.Contracts;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Components;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Utilities;
using System.Data;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Data.DataBinders
{
  public class DefinitionEnvironmentBinder2 : DefinitionEnvironmentBinder
  {
    private SqlColumnBinder runoptions = new SqlColumnBinder("RunOptions");
    private SqlColumnBinder processParameters = new SqlColumnBinder("ProcessParameters");

    public DefinitionEnvironmentBinder2(
      ReleaseManagementSqlResourceComponentBase sqlComponent)
      : base(sqlComponent)
    {
    }

    protected override DefinitionEnvironment Bind()
    {
      EnvironmentOptions environmentOptions = ServerModelUtility.GetServerEnvironmentOptions(this.runoptions.GetString((IDataReader) this.Reader, (string) null));
      DefinitionEnvironment definitionEnvironment = base.Bind();
      definitionEnvironment.EnvironmentOptions = environmentOptions;
      string str = this.processParameters.GetString((IDataReader) this.Reader, (string) null);
      if (str != null)
        definitionEnvironment.ProcessParameters = ServerModelUtility.FromString<ProcessParameters>(str);
      return definitionEnvironment;
    }
  }
}
