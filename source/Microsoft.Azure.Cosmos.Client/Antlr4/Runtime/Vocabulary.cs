// Decompiled with JetBrains decompiler
// Type: Antlr4.Runtime.Vocabulary
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using Antlr4.Runtime.Misc;
using System;

namespace Antlr4.Runtime
{
  internal class Vocabulary : IVocabulary
  {
    private static readonly string[] EmptyNames = new string[0];
    [NotNull]
    public static readonly Vocabulary EmptyVocabulary = new Vocabulary(Vocabulary.EmptyNames, Vocabulary.EmptyNames, Vocabulary.EmptyNames);
    [NotNull]
    private readonly string[] literalNames;
    [NotNull]
    private readonly string[] symbolicNames;
    [NotNull]
    private readonly string[] displayNames;
    private readonly int maxTokenType;

    public Vocabulary(string[] literalNames, string[] symbolicNames)
      : this(literalNames, symbolicNames, (string[]) null)
    {
    }

    public Vocabulary(string[] literalNames, string[] symbolicNames, string[] displayNames)
    {
      this.literalNames = literalNames != null ? literalNames : Vocabulary.EmptyNames;
      this.symbolicNames = symbolicNames != null ? symbolicNames : Vocabulary.EmptyNames;
      this.displayNames = displayNames != null ? displayNames : Vocabulary.EmptyNames;
      this.maxTokenType = Math.Max(this.displayNames.Length, Math.Max(this.literalNames.Length, this.symbolicNames.Length)) - 1;
    }

    public virtual int getMaxTokenType() => this.maxTokenType;

    [return: Nullable]
    public virtual string GetLiteralName(int tokenType) => tokenType >= 0 && tokenType < this.literalNames.Length ? this.literalNames[tokenType] : (string) null;

    [return: Nullable]
    public virtual string GetSymbolicName(int tokenType)
    {
      if (tokenType >= 0 && tokenType < this.symbolicNames.Length)
        return this.symbolicNames[tokenType];
      return tokenType == -1 ? "EOF" : (string) null;
    }

    [return: NotNull]
    public virtual string GetDisplayName(int tokenType)
    {
      if (tokenType >= 0 && tokenType < this.displayNames.Length)
      {
        string displayName = this.displayNames[tokenType];
        if (displayName != null)
          return displayName;
      }
      return this.GetLiteralName(tokenType) ?? this.GetSymbolicName(tokenType) ?? tokenType.ToString();
    }
  }
}
