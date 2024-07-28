// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.ProjectAnalysis.Server.ProjectAnalysisComponentBase
// Assembly: Microsoft.TeamFoundation.ProjectAnalysis.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 076482BC-74A4-4A35-9427-1E61C33D1FA6
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.ProjectAnalysis.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.ProjectAnalysis.Server
{
  public class ProjectAnalysisComponentBase : TeamFoundationSqlResourceComponent
  {
    private static Dictionary<int, SqlExceptionFactory> s_translatedExceptions = new Dictionary<int, SqlExceptionFactory>()
    {
      {
        1760001,
        new SqlExceptionFactory(typeof (LanguageMetadataExistsException))
      },
      {
        1760002,
        new SqlExceptionFactory(typeof (LanguageMetadataNotFoundException))
      }
    };

    public ProjectAnalysisComponentBase()
    {
      this.SelectedFeatures |= SqlResourceComponentFeatures.BindPartitionIdByDefault;
      this.ContainerErrorCode = 50000;
    }

    protected override IDictionary<int, SqlExceptionFactory> TranslatedExceptions => (IDictionary<int, SqlExceptionFactory>) ProjectAnalysisComponentBase.s_translatedExceptions;
  }
}
