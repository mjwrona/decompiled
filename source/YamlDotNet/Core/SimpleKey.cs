// Decompiled with JetBrains decompiler
// Type: YamlDotNet.Core.SimpleKey
// Assembly: YamlDotNet, Version=5.0.0.0, Culture=neutral, PublicKeyToken=ec19458f3c15af5e
// MVID: 5F9DD5C4-A41D-46B2-A793-8157A0D55AB5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\YamlDotNet.dll

using System;

namespace YamlDotNet.Core
{
  [Serializable]
  internal class SimpleKey
  {
    private readonly Cursor cursor;

    public bool IsPossible { get; set; }

    public bool IsRequired { get; private set; }

    public int TokenNumber { get; private set; }

    public int Index => this.cursor.Index;

    public int Line => this.cursor.Line;

    public int LineOffset => this.cursor.LineOffset;

    public Mark Mark => this.cursor.Mark();

    public SimpleKey() => this.cursor = new Cursor();

    public SimpleKey(bool isPossible, bool isRequired, int tokenNumber, Cursor cursor)
    {
      this.IsPossible = isPossible;
      this.IsRequired = isRequired;
      this.TokenNumber = tokenNumber;
      this.cursor = new Cursor(cursor);
    }
  }
}
