// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Server.Jobs.Definitions.CodePipelineDocumentId
// Assembly: Microsoft.VisualStudio.Services.Search.Server.Jobs.Definitions, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 10D2EBC4-B606-4155-939F-EEB226A80181
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.Server.Jobs.Definitions.dll

using System;
using System.Runtime.CompilerServices;

namespace Microsoft.VisualStudio.Services.Search.Server.Jobs.Definitions
{
  public class CodePipelineDocumentId : IEquatable<CodePipelineDocumentId>
  {
    public string FilePath { get; }

    public string Branch { get; }

    public CodePipelineDocumentId(string filePath, string branch)
    {
      this.FilePath = filePath ?? throw new ArgumentNullException(nameof (filePath));
      this.Branch = branch ?? throw new ArgumentNullException(nameof (branch));
    }

    public CodePipelineDocumentId(string filePath)
      : this(filePath, string.Empty)
    {
    }

    public static implicit operator CodePipelineDocumentId(string filePath) => new CodePipelineDocumentId(filePath);

    public override bool Equals(object obj) => this.Equals(obj as CodePipelineDocumentId);

    public bool Equals(CodePipelineDocumentId other)
    {
      if (this == other)
        return true;
      return other != null && string.Equals(this.FilePath, other.FilePath, StringComparison.Ordinal) && string.Equals(this.Branch, other.Branch, StringComparison.Ordinal);
    }

    public override int GetHashCode() => (-315410199 * -1521134295 + this.FilePath.GetHashCode()) * -1521134295 + this.Branch.GetHashCode();

    public override string ToString() => string.IsNullOrWhiteSpace(this.Branch) ? FormattableString.Invariant(FormattableStringFactory.Create("FilePath: [{0}]", (object) this.FilePath)) : FormattableString.Invariant(FormattableStringFactory.Create("Branch: [{0}], FilePath: [{1}]", (object) this.Branch, (object) this.FilePath));
  }
}
