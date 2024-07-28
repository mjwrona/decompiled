// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.CodeReview.Server.DiffCacheEntry
// Assembly: Microsoft.VisualStudio.Services.CodeReview.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 2BCB2866-BDCB-4FDE-9EA3-48DFA660C131
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.CodeReview.Server.dll

using Microsoft.VisualStudio.Services.CodeReview.Server.Common;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.CodeReview.Server
{
  [DataContract]
  public class DiffCacheEntry
  {
    [DataMember]
    public DiffChangeType ChangeType;
    [DataMember]
    public int OrigStartLine;
    [DataMember]
    public int OrigStartOffset;
    [DataMember]
    public int OrigEndLine;
    [DataMember]
    public int OrigEndOffset;
    [DataMember]
    public int ModStartLine;
    [DataMember]
    public int ModStartOffset;
    [DataMember]
    public int ModEndLine;
    [DataMember]
    public int ModEndOffset;
  }
}
