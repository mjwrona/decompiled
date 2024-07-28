// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.Core.Project.WebServer.ProjectsContinuationToken
// Assembly: Microsoft.TeamFoundation.Server.Core, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9DD3208E-87CF-4F7C-8D96-8880BDAD13B2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.Core.dll

namespace Microsoft.TeamFoundation.Server.Core.Project.WebServer
{
  public sealed class ProjectsContinuationToken
  {
    public const string ContinuationTokenHeaderName = "x-ms-continuationtoken";
    private readonly int? _take;
    private readonly int? _skip;
    private readonly int? _continuationToken;
    private readonly int _projectsCount;

    internal ProjectsContinuationToken(
      int? skip,
      int? take,
      int? continuationToken,
      int projectsCount)
    {
      this._take = take;
      this._skip = skip;
      this._continuationToken = continuationToken;
      this._projectsCount = projectsCount;
    }

    internal bool IsValid
    {
      get
      {
        int? nullable;
        if (this._take.HasValue)
        {
          nullable = this._take;
          int num = 0;
          if (nullable.GetValueOrDefault() >= num & nullable.HasValue)
          {
            nullable = this._take;
            int projectsCount = this._projectsCount;
            if (nullable.GetValueOrDefault() <= projectsCount & nullable.HasValue)
              goto label_10;
          }
        }
        nullable = this._skip;
        if (nullable.HasValue)
        {
          nullable = this._skip;
          int num = 0;
          if (nullable.GetValueOrDefault() > num & nullable.HasValue)
          {
            nullable = this._skip;
            int projectsCount = this._projectsCount;
            if (nullable.GetValueOrDefault() <= projectsCount & nullable.HasValue)
              goto label_10;
          }
        }
        nullable = this._continuationToken;
        if (nullable.HasValue)
        {
          nullable = this._continuationToken;
          int num = 0;
          if (nullable.GetValueOrDefault() > num & nullable.HasValue)
          {
            nullable = this._continuationToken;
            int projectsCount = this._projectsCount;
            return nullable.GetValueOrDefault() <= projectsCount & nullable.HasValue;
          }
        }
        return false;
label_10:
        return true;
      }
    }

    internal int NextToken
    {
      get
      {
        (int take, int skip) paginationValues = this.CalculatePaginationValues();
        return paginationValues.take + paginationValues.skip;
      }
    }

    internal bool AllTheProjectsBeenFetched => this.NextToken >= this._projectsCount;

    internal (int take, int skip) CalculatePaginationValues()
    {
      int actualSkipValue = this.GetActualSkipValue(this._skip, this._continuationToken);
      return (this.GetActualTakeValue(this._take, actualSkipValue), actualSkipValue);
    }

    private int GetActualSkipValue(int? skipFromRequest, int? continuationToken)
    {
      int? nullable = continuationToken;
      int num = 0;
      return nullable.GetValueOrDefault() > num & nullable.HasValue ? continuationToken.Value : skipFromRequest.GetValueOrDefault();
    }

    private int GetActualTakeValue(int? take, int actualSkipValue)
    {
      int num = 0;
      if (actualSkipValue > 0)
        num = this._projectsCount - actualSkipValue;
      return take ?? num;
    }
  }
}
