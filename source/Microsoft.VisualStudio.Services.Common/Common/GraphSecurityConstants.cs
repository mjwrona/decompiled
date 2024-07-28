// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Common.GraphSecurityConstants
// Assembly: Microsoft.VisualStudio.Services.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F9C46641-2F1F-44C4-9C2E-4328CD5FFC63
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Common.dll

using System;
using System.ComponentModel;

namespace Microsoft.VisualStudio.Services.Common
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  public class GraphSecurityConstants
  {
    public static readonly Guid NamespaceId = new Guid("C2EE56C9-E8FA-4CDD-9D48-2C44F697A58E");
    public static readonly string RefsToken = "Refs";
    public static readonly string SubjectsToken = "Subjects";
    public const int ReadByPublicIdentifier = 1;
    public const int ReadByPersonalIdentifier = 2;
  }
}
