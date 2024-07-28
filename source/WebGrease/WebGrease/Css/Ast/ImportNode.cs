// Decompiled with JetBrains decompiler
// Type: WebGrease.Css.Ast.ImportNode
// Assembly: WebGrease, Version=1.6.5135.21930, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 86487675-C393-48D4-AFEC-7657DB09B21F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\WebGrease.dll

using System.Collections.Generic;
using System.Collections.ObjectModel;
using WebGrease.Css.Ast.MediaQuery;
using WebGrease.Css.Visitor;

namespace WebGrease.Css.Ast
{
  public sealed class ImportNode : AstNode
  {
    public ImportNode(
      AllowedImportData allowedImportDataType,
      string importDataValue,
      ReadOnlyCollection<MediaQueryNode> mediaQueries)
    {
      this.AllowedImportDataType = allowedImportDataType;
      this.ImportDataValue = importDataValue;
      this.MediaQueries = mediaQueries ?? new List<MediaQueryNode>(0).AsReadOnly();
    }

    public AllowedImportData AllowedImportDataType { get; private set; }

    public string ImportDataValue { get; private set; }

    public ReadOnlyCollection<MediaQueryNode> MediaQueries { get; private set; }

    public override AstNode Accept(NodeVisitor nodeVisitor) => nodeVisitor.VisitImportNode(this);
  }
}
