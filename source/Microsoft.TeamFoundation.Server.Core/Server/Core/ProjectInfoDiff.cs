// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.Core.ProjectInfoDiff
// Assembly: Microsoft.TeamFoundation.Server.Core, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9DD3208E-87CF-4F7C-8D96-8880BDAD13B2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.Core.dll

using Microsoft.TeamFoundation.Core.WebApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Microsoft.TeamFoundation.Server.Core
{
  internal class ProjectInfoDiff
  {
    private readonly ProjectInfo m_project1;
    private readonly ProjectInfo m_project2;
    private static readonly HashSet<string> s_diffFields = new HashSet<string>()
    {
      "Name",
      "State",
      "Abbreviation",
      "Description",
      "Visibility"
    };

    public ProjectInfoDiff(ProjectInfo project1, ProjectInfo project2)
    {
      this.m_project1 = project1;
      this.m_project2 = project2;
    }

    public static DiffSpec Diff(ProjectInfo project1, ProjectInfo project2) => new ProjectInfoDiff(project1, project2).Diff();

    public DiffSpec Diff()
    {
      ProjectInfo projectInfo1 = this.m_project1 ?? new ProjectInfo();
      ProjectInfo projectInfo2 = this.m_project2 ?? new ProjectInfo();
      List<DiffSpec> items = new List<DiffSpec>();
      foreach (PropertyInfo propertyInfo in ((IEnumerable<MemberInfo>) typeof (ProjectInfo).GetMembers()).Where<MemberInfo>((Func<MemberInfo, bool>) (m => ProjectInfoDiff.s_diffFields.Contains(m.Name))))
      {
        object obj1 = propertyInfo.GetValue((object) projectInfo1);
        object obj2 = propertyInfo.GetValue((object) projectInfo2);
        if (!object.Equals(obj1, obj2))
          items.Add((DiffSpec) new DiffItem(propertyInfo.Name, (DiffSpec) new DiffSingle(obj1, obj2)));
      }
      return (DiffSpec) new DiffComposite((IEnumerable<DiffSpec>) items);
    }

    internal static IEnumerable<string> DiffFields => (IEnumerable<string>) ProjectInfoDiff.s_diffFields;
  }
}
