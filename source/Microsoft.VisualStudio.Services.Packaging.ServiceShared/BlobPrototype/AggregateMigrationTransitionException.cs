// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype.AggregateMigrationTransitionException
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using Microsoft.VisualStudio.Services.Common;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype
{
  public class AggregateMigrationTransitionException : VssServiceException
  {
    public AggregateMigrationTransitionException(
      IReadOnlyCollection<MigrationTransitionException> exceptions)
      : base(AggregateMigrationTransitionException.GenerateMessage(exceptions))
    {
      this.Exceptions = exceptions;
    }

    public IReadOnlyCollection<MigrationTransitionException> Exceptions { get; set; }

    private static string GenerateMessage(
      IReadOnlyCollection<MigrationTransitionException> exceptions)
    {
      return exceptions.Count == 1 ? exceptions.First<MigrationTransitionException>().Message : string.Format("{0} exceptions occurred. first example: {1}", (object) exceptions.Count, (object) exceptions.First<MigrationTransitionException>().Message);
    }
  }
}
