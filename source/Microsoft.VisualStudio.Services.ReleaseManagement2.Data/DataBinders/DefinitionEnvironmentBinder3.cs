// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Data.DataBinders.DefinitionEnvironmentBinder3
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Data, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A69B0FB7-3028-4162-BA38-794D80D7A49A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Data.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Components;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Utilities;
using Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics.CodeAnalysis;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Data.DataBinders
{
  [SuppressMessage("Microsoft.Naming", "CA1710:IdentifiersShouldHaveCorrectSuffix", Justification = "Binder is the right suffix as per VSSF convention.")]
  public class DefinitionEnvironmentBinder3 : DefinitionEnvironmentBinder2
  {
    private SqlColumnBinder deploymentGates = new SqlColumnBinder("Gates");
    private SqlColumnBinder variableGroups = new SqlColumnBinder("VariableGroups");
    private SqlColumnBinder modifiedOn = new SqlColumnBinder("ModifiedOn");

    public DefinitionEnvironmentBinder3(
      ReleaseManagementSqlResourceComponentBase sqlComponent)
      : base(sqlComponent)
    {
    }

    protected override DefinitionEnvironment Bind()
    {
      string str1 = this.deploymentGates.GetString((IDataReader) this.Reader, (string) null);
      string str2 = this.variableGroups.GetString((IDataReader) this.Reader, (string) null);
      DateTime dateTime = this.modifiedOn.GetDateTime((IDataReader) this.Reader, DateTime.MinValue);
      DefinitionEnvironment definitionEnvironment = base.Bind();
      definitionEnvironment.ModifiedOn = dateTime;
      if (str1 != null)
      {
        IDictionary<string, ReleaseDefinitionGatesStep> definitionEnvironmentGates = ServerModelUtility.FromString<IDictionary<string, ReleaseDefinitionGatesStep>>(str1);
        definitionEnvironment.PopulateDefinitionGates(definitionEnvironmentGates);
      }
      if (!string.IsNullOrWhiteSpace(str2))
      {
        List<int> values = ServerModelUtility.FromString<List<int>>(str2);
        if (values.Count > 0)
          definitionEnvironment.VariableGroups.AddRange<int, IList<int>>((IEnumerable<int>) values);
      }
      return definitionEnvironment;
    }
  }
}
