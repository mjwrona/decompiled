// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ReleaseRevisionChangeDetails
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Data, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A69B0FB7-3028-4162-BA38-794D80D7A49A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Data.dll

using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Enumerations;
using System;
using System.Diagnostics.CodeAnalysis;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model
{
  public class ReleaseRevisionChangeDetails
  {
    public ReleaseHistoryMessageId Id { get; set; }

    [SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate", Justification = "Leave this as method.")]
    public virtual string GetServerFormat() => throw new InvalidOperationException("Should be implemented by child class.");

    [SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate", Justification = "Leave this as method.")]
    public virtual string GetStageName() => string.Empty;

    [SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate", Justification = "Leave this as method.")]
    public virtual ReleaseEnvironmentStatus GetReleaseEnvironmentStatus() => ReleaseEnvironmentStatus.Undefined;
  }
}
