// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Data.DataBinders.ReleaseBinder2
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Data, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A69B0FB7-3028-4162-BA38-794D80D7A49A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Data.dll

using Microsoft.TeamFoundation.DistributedTask.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Components;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Utilities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics.CodeAnalysis;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Data.DataBinders
{
  [SuppressMessage("Microsoft.Naming", "CA1710:IdentifiersShouldHaveCorrectSuffix", Justification = "Binder is the right suffix as per VSSF convention.")]
  public class ReleaseBinder2 : ReleaseBinder
  {
    private SqlColumnBinder variableGroups = new SqlColumnBinder("VariableGroups");

    public ReleaseBinder2(
      IVssRequestContext requestContext,
      ReleaseManagementSqlResourceComponentBase sqlComponent)
      : base(requestContext, sqlComponent)
    {
    }

    protected override void FillVariableGroups(Release release)
    {
      if (release == null)
        throw new ArgumentNullException(nameof (release));
      IList<VariableGroup> values = ServerModelUtility.FromString<IList<VariableGroup>>(this.variableGroups.GetString((IDataReader) this.Reader, true));
      if (values == null)
        return;
      release.VariableGroups.AddRange<VariableGroup, IList<VariableGroup>>((IEnumerable<VariableGroup>) values);
    }
  }
}
