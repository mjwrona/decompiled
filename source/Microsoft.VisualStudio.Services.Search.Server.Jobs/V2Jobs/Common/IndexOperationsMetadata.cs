// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.V2Jobs.Common.IndexOperationsMetadata
// Assembly: Microsoft.VisualStudio.Services.Search.Server.Jobs, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 23ACAECB-A4CB-4AA5-8366-092C41D8D4A8
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Search.Server.Jobs.dll

using Microsoft.VisualStudio.Services.Search.Common;
using Microsoft.VisualStudio.Services.Search.Common.Arriba;
using Microsoft.VisualStudio.Services.Search.Common.Arriba.Expressions;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Search.V2Jobs.Common
{
  public class IndexOperationsMetadata
  {
    public string RepositoryId { get; set; }

    public string ProjectName { get; set; }

    public string BranchName { get; set; }

    public string RepositoryName { get; set; }

    public string CollectionName { get; set; }

    public IExpression GetQueryExpression()
    {
      List<IExpression> set = new List<IExpression>();
      if (!string.IsNullOrEmpty(this.RepositoryId))
        set.Add((IExpression) new TermExpression("repositoryId", Operator.Equals, this.RepositoryId));
      if (!string.IsNullOrEmpty(this.ProjectName))
        set.Add((IExpression) new TermExpression("projectNameOriginal", Operator.Equals, this.ProjectName));
      if (!string.IsNullOrEmpty(this.BranchName))
      {
        string nameWithoutPrefix = CustomUtils.GetBranchNameWithoutPrefix("refs/heads/", this.BranchName);
        set.Add((IExpression) new TermExpression("branchNameOriginal", Operator.Equals, nameWithoutPrefix));
      }
      if (!string.IsNullOrEmpty(this.RepositoryName))
        set.Add((IExpression) new TermExpression("repoNameOriginal", Operator.Equals, this.RepositoryName));
      if (!string.IsNullOrEmpty(this.CollectionName))
        set.Add((IExpression) new TermExpression("collectionNameOriginal", Operator.Equals, this.CollectionName));
      return set.Count > 0 ? (IExpression) new AndExpression((IEnumerable<IExpression>) set) : (IExpression) new EmptyExpression();
    }
  }
}
