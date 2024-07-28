// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Data.Converters.RetentionPolicyConverter
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Data, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A69B0FB7-3028-4162-BA38-794D80D7A49A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Data.dll

using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model;
using Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts;
using System;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Data.Converters
{
  public static class RetentionPolicyConverter
  {
    public static RetentionPolicy ToWebApiRetentionPolicy(ReleaseDefinition releaseDefinition)
    {
      if (releaseDefinition == null)
        throw new ArgumentNullException(nameof (releaseDefinition));
      if (releaseDefinition.Environments == null || releaseDefinition.Environments.Count == 0 || releaseDefinition.Environments[0].RetentionPolicy == null)
        return (RetentionPolicy) null;
      return new RetentionPolicy()
      {
        DaysToKeep = releaseDefinition.Environments[0].RetentionPolicy.DaysToKeep
      };
    }
  }
}
