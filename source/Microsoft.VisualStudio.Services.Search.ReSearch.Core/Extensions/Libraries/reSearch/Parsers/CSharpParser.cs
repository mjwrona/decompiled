// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Extensions.Libraries.reSearch.Parsers.CSharpParser
// Assembly: Microsoft.VisualStudio.Services.Search.ReSearch.Core, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 657A74AE-F2A6-4615-BB2F-7FA1F961B173
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.ReSearch.Core.dll

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.VisualStudio.Services.Search.Extensions.Libraries.reSearch.Tokenizers.SourceCode;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading;

namespace Microsoft.VisualStudio.Services.Search.Extensions.Libraries.reSearch.Parsers
{
  public class CSharpParser : RoslynParser
  {
    public CSharpParser(int maxFileSizeSupportedInBytes)
      : base(maxFileSizeSupportedInBytes)
    {
    }

    protected override List<CodeSymbol> ParseIntoCodeSymbols()
    {
      if (!string.IsNullOrEmpty(this.Content))
        this.ParseTree(CSharpSyntaxTree.ParseText(this.Content, new CSharpParseOptions(LanguageVersion.Latest), "", (Encoding) null, new CancellationToken()).GetRoot());
      return this.SymbolsList;
    }

    private List<CodeSymbol> ParseTree(SyntaxList<SyntaxNode> syntaxList, int depth)
    {
      List<CodeSymbol> tree = new List<CodeSymbol>();
      foreach (SyntaxNode syntax in syntaxList)
        tree.AddRange((IEnumerable<CodeSymbol>) this.ParseTree(syntax, depth));
      return tree;
    }

    private List<CodeSymbol> ParseTree(
      SeparatedSyntaxList<ExpressionSyntax> separatedSyntaxList,
      int depth)
    {
      List<CodeSymbol> tree = new List<CodeSymbol>();
      foreach (ExpressionSyntax separatedSyntax in separatedSyntaxList)
        tree.AddRange((IEnumerable<CodeSymbol>) this.ParseTree((SyntaxNode) separatedSyntax, depth));
      return tree;
    }

    private List<CodeSymbol> ParseTree(
      SeparatedSyntaxList<AnonymousObjectMemberDeclaratorSyntax> separatedSyntaxList,
      int depth)
    {
      List<CodeSymbol> tree = new List<CodeSymbol>();
      foreach (AnonymousObjectMemberDeclaratorSyntax separatedSyntax in separatedSyntaxList)
        tree.AddRange((IEnumerable<CodeSymbol>) this.ParseTree((SyntaxNode) separatedSyntax, depth));
      return tree;
    }

    private List<CodeSymbol> ParseTree(SyntaxNode node, int depth = 0)
    {
      SyntaxKind syntaxKind = node.Kind();
      if ((uint) syntaxKind <= 8739U)
      {
        if ((uint) syntaxKind <= 8687U)
        {
          switch (syntaxKind)
          {
            case SyntaxKind.IdentifierName:
              return this.ParseAndScope(new RoslynParser.Parser(this.ParseIdentifier), node, depth);
            case SyntaxKind.QualifiedName:
              return this.ParseAndScope(new RoslynParser.Parser(this.ParseQualifiesName), node, depth);
            case SyntaxKind.GenericName:
              return this.ParseAndScope(new RoslynParser.Parser(this.ParseGenericName), (SyntaxNode) (node as GenericNameSyntax), depth);
            case SyntaxKind.AliasQualifiedName:
              return this.ParseAndScope(new RoslynParser.Parser(this.ParseAliasName), (SyntaxNode) (node as AliasQualifiedNameSyntax), depth);
            case SyntaxKind.PredefinedType:
              return this.ParseAndScope(new RoslynParser.Parser(this.ParsePredefinedType), node, depth);
            case SyntaxKind.ArrayType:
              return this.ParseAndScope(new RoslynParser.Parser(this.ParseArrayType), node, depth);
            case SyntaxKind.ArrayRankSpecifier:
              ArrayRankSpecifierSyntax rankSpecifierSyntax = (ArrayRankSpecifierSyntax) node;
              this.ParseTriva(rankSpecifierSyntax.OpenBracketToken, depth + 1);
              this.ParseTriva(rankSpecifierSyntax.CloseBracketToken, depth + 1);
              return this.ParseTree(rankSpecifierSyntax.Sizes, depth);
            case SyntaxKind.InvocationExpression:
              return this.ParseAndScope(new RoslynParser.Parser(this.ParseInvocation), (SyntaxNode) (node as InvocationExpressionSyntax), depth);
            case SyntaxKind.Argument:
              return this.ParseAndScope(new RoslynParser.Parser(this.ParseArgument), (SyntaxNode) (node as ArgumentSyntax), depth);
            case SyntaxKind.NameColon:
              return this.ParseAndScope(new RoslynParser.Parser(this.ParseNameColon), node, depth);
            case SyntaxKind.CastExpression:
              return this.ParseAndScope(new RoslynParser.Parser(this.ParseCast), (SyntaxNode) (node as CastExpressionSyntax), depth);
            case SyntaxKind.AnonymousMethodExpression:
              return this.ParseAndScope(new RoslynParser.Parser(this.ParseAnonymousMethod), (SyntaxNode) (node as AnonymousMethodExpressionSyntax), depth);
            case SyntaxKind.ObjectInitializerExpression:
            case SyntaxKind.CollectionInitializerExpression:
            case SyntaxKind.ArrayInitializerExpression:
            case SyntaxKind.ComplexElementInitializerExpression:
              InitializerExpressionSyntax expressionSyntax = (InitializerExpressionSyntax) node;
              foreach (SyntaxNodeOrToken withSeparator in expressionSyntax.Expressions.GetWithSeparators())
              {
                if (withSeparator.IsToken)
                  this.ParseTriva(withSeparator.AsToken(), depth);
              }
              this.ParseTriva(expressionSyntax.OpenBraceToken, depth + 1);
              this.ParseTriva(expressionSyntax.CloseBraceToken, depth + 1);
              return this.ParseTree(expressionSyntax.Expressions, depth);
            case SyntaxKind.ObjectCreationExpression:
              return this.ParseAndScope(new RoslynParser.Parser(this.ParseObjectCreationExpression), node, depth);
            case SyntaxKind.AnonymousObjectCreationExpression:
              return this.ParseAndScope(new RoslynParser.Parser(this.ParseAnonymousObjectCreationExpression), node, depth);
            case SyntaxKind.ArrayCreationExpression:
              return this.ParseAndScope(new RoslynParser.Parser(this.ParseArrayCreation), node, depth);
            case SyntaxKind.AsExpression:
              return this.ParseAndScope(new RoslynParser.Parser(this.ParseAsExpression), node, depth);
          }
        }
        else
        {
          switch (syntaxKind)
          {
            case SyntaxKind.SimpleMemberAccessExpression:
              return this.ParseAndScope(new RoslynParser.Parser(this.ParseMemberAccess), node, depth);
            case SyntaxKind.SimpleAssignmentExpression:
              return this.ParseAndScope(new RoslynParser.Parser(this.ParseAssign), node, depth);
          }
        }
      }
      else if ((uint) syntaxKind <= 8763U)
      {
        switch (syntaxKind)
        {
          case SyntaxKind.NumericLiteralExpression:
          case SyntaxKind.StringLiteralExpression:
          case SyntaxKind.TrueLiteralExpression:
          case SyntaxKind.FalseLiteralExpression:
            return this.ParseAndScope(new RoslynParser.Parser(this.ParsePrimaryExpression), node, depth);
          case SyntaxKind.CheckedExpression:
          case SyntaxKind.UncheckedExpression:
            return this.ParseAndScope(new RoslynParser.Parser(this.ParsePrimaryFunction), node, depth);
        }
      }
      else if ((uint) syntaxKind <= 8805U)
      {
        switch (syntaxKind)
        {
          case SyntaxKind.VariableDeclaration:
            return this.ParseAndScope(new RoslynParser.Parser(this.ParseVariableDeclaration), (SyntaxNode) (node as VariableDeclarationSyntax), depth);
          case SyntaxKind.LabeledStatement:
            return this.ParseAndScope(new RoslynParser.Parser(this.ParseLabeledStatement), node, depth);
          case SyntaxKind.ReturnStatement:
            return this.ParseAndScope(new RoslynParser.Parser(this.ParseReturnStatement), (SyntaxNode) (node as ReturnStatementSyntax), depth);
        }
      }
      else
      {
        switch (syntaxKind)
        {
          case SyntaxKind.DoStatement:
            return this.ParseAndScope(new RoslynParser.Parser(this.ParseDoStatement), node, depth);
          case SyntaxKind.TryStatement:
            return this.ParseAndScope(new RoslynParser.Parser(this.ParseTryStatement), (SyntaxNode) (node as TryStatementSyntax), depth);
          case SyntaxKind.CatchClause:
            return this.ParseAndScope(new RoslynParser.Parser(this.ParseCatch), node, depth);
          case SyntaxKind.CatchDeclaration:
            return this.ParseAndScope(new RoslynParser.Parser(this.ParseCatchDeclaration), node, depth);
          case SyntaxKind.FinallyClause:
            return this.ParseAndScope(new RoslynParser.Parser(this.ParseFinallyClause), node, depth);
          case SyntaxKind.LocalFunctionStatement:
            return this.ParseAndScope(new RoslynParser.Parser(this.ParseLocalFunctionStatement), node, depth);
          case SyntaxKind.NamespaceDeclaration:
            return this.ParseAndScope(new RoslynParser.Parser(this.ParseNamespaceDeclaration), node, depth);
          case SyntaxKind.UsingDirective:
            return this.ParseAndScope(new RoslynParser.Parser(this.ParseUsingDirective), node, depth);
          case SyntaxKind.ExternAliasDirective:
            return this.ParseAndScope(new RoslynParser.Parser(this.ParseExternAliasDirective), node, depth);
          case SyntaxKind.AttributeTargetSpecifier:
            return this.ParseAndScope(new RoslynParser.Parser(this.ParseAttributeTarget), node, depth);
          case SyntaxKind.Attribute:
            return this.ParseAndScope(new RoslynParser.Parser(this.ParseAttribute), node, depth);
          case SyntaxKind.AttributeArgument:
            return this.ParseAndScope(new RoslynParser.Parser(this.ParseAttributeArgument), node, depth);
          case SyntaxKind.ClassDeclaration:
          case SyntaxKind.InterfaceDeclaration:
            return this.ParseAndScope(new RoslynParser.Parser(this.ParseTypeDeclaration), (SyntaxNode) (node as TypeDeclarationSyntax), depth);
          case SyntaxKind.EnumDeclaration:
            return this.ParseAndScope(new RoslynParser.Parser(this.ParseEnumDeclaration), node, depth);
          case SyntaxKind.DelegateDeclaration:
            return this.ParseAndScope(new RoslynParser.Parser(this.ParseDelegateDeclaration), node, depth);
          case SyntaxKind.BaseList:
            return this.ParseAndScope(new RoslynParser.Parser(this.ParseBaseList), (SyntaxNode) (node as BaseListSyntax), depth);
          case SyntaxKind.TypeParameterConstraintClause:
            return this.ParseAndScope(new RoslynParser.Parser(this.ParseContraintClause), (SyntaxNode) (node as TypeParameterConstraintClauseSyntax), depth);
          case SyntaxKind.ConstructorConstraint:
            return this.ParseAndScope(new RoslynParser.Parser(this.ParseConstructorConstraint), node, depth);
          case SyntaxKind.TypeConstraint:
            return this.ParseAndScope(new RoslynParser.Parser(this.ParseConstraint), (SyntaxNode) (node as TypeConstraintSyntax), depth);
          case SyntaxKind.EnumMemberDeclaration:
            return this.ParseAndScope(new RoslynParser.Parser(this.ParseEnumMemeberDeclaration), node, depth);
          case SyntaxKind.FieldDeclaration:
            return this.ParseAndScope(new RoslynParser.Parser(this.ParseFieldDeclaration), (SyntaxNode) (node as FieldDeclarationSyntax), depth);
          case SyntaxKind.MethodDeclaration:
            return this.ParseAndScope(new RoslynParser.Parser(this.ParseMethodDeclaration), node, depth);
          case SyntaxKind.OperatorDeclaration:
            return this.ParseAndScope(new RoslynParser.Parser(this.ParseOperatorDeclaration), node, depth);
          case SyntaxKind.ConversionOperatorDeclaration:
            return this.ParseAndScope(new RoslynParser.Parser(this.ParseConversionOperatorDeclaration), node, depth);
          case SyntaxKind.ConstructorDeclaration:
            return this.ParseAndScope(new RoslynParser.Parser(this.ParseContructorDeclaration), node, depth);
          case SyntaxKind.BaseConstructorInitializer:
          case SyntaxKind.ThisConstructorInitializer:
            return this.ParseAndScope(new RoslynParser.Parser(this.ParseConstructorInitializer), node, depth);
          case SyntaxKind.DestructorDeclaration:
            return this.ParseAndScope(new RoslynParser.Parser(this.ParseDestructorDeclaration), node, depth);
          case SyntaxKind.PropertyDeclaration:
            return this.ParseAndScope(new RoslynParser.Parser(this.ParsePropertyDeclaration), (SyntaxNode) (node as PropertyDeclarationSyntax), depth);
          case SyntaxKind.EventDeclaration:
            return this.ParseAndScope(new RoslynParser.Parser(this.ParseEventDeclaration), node, depth);
          case SyntaxKind.IndexerDeclaration:
            return this.ParseAndScope(new RoslynParser.Parser(this.ParseIndexerDeclaration), node, depth);
          case SyntaxKind.GetAccessorDeclaration:
          case SyntaxKind.SetAccessorDeclaration:
          case SyntaxKind.AddAccessorDeclaration:
          case SyntaxKind.RemoveAccessorDeclaration:
          case SyntaxKind.UnknownAccessorDeclaration:
            return this.ParseAndScope(new RoslynParser.Parser(this.ParseAccessor), (SyntaxNode) (node as AccessorDeclarationSyntax), depth);
          case SyntaxKind.Parameter:
            return this.ParseAndScope(new RoslynParser.Parser(this.ParseParameter), (SyntaxNode) (node as ParameterSyntax), depth);
          case SyntaxKind.TypeParameter:
            return this.ParseAndScope(new RoslynParser.Parser(this.ParseTypeParameter), (SyntaxNode) (node as TypeParameterSyntax), depth);
        }
      }
      if (node.ChildNodesAndTokens().Count<SyntaxNodeOrToken>() > 0)
      {
        int count = this.SymbolsList.Count;
        foreach (SyntaxNodeOrToken childNodesAndToken in node.ChildNodesAndTokens())
        {
          if (childNodesAndToken.IsNode)
            this.ParseTree(childNodesAndToken.AsNode(), depth + 1);
          else if (!this.IgnoredKind(childNodesAndToken.AsToken().Kind()))
            this.AddSymbol(childNodesAndToken.AsToken(), this.GetTokenKind(childNodesAndToken.Kind()), depth);
          else
            this.ParseTriva(childNodesAndToken.AsToken(), depth + 1);
        }
        return this.SymbolsList.GetRange(count, this.SymbolsList.Count - count);
      }
      foreach (SyntaxToken childToken in node.ChildTokens())
        this.AddSymbol(childToken, this.GetTokenKind(childToken.Kind()), depth);
      return this.SymbolsList.GetRange(this.SymbolsList.Count - 1, 1);
    }

    private void ParseAttributeTarget(SyntaxNode node, int depth)
    {
      AttributeTargetSpecifierSyntax targetSpecifierSyntax = (AttributeTargetSpecifierSyntax) node;
      this.AddSymbol(targetSpecifierSyntax.Identifier, CodeTokenKind.CSAttributeTarget, depth);
      this.ParseTriva(targetSpecifierSyntax.ColonToken, depth + 1);
    }

    private void ParseAttributeArgument(SyntaxNode node, int depth)
    {
      AttributeArgumentSyntax attributeArgumentSyntax = (AttributeArgumentSyntax) node;
      List<CodeSymbol> tree1 = this.ParseTree((SyntaxNode) attributeArgumentSyntax.Expression, depth);
      if (tree1 != null && tree1.Count >= 1)
        tree1[tree1.Count - 1].SymbolType = CodeTokenKind.CSAttributeParameterValue;
      if (attributeArgumentSyntax.NameColon != null)
      {
        List<CodeSymbol> tree2 = this.ParseTree((SyntaxNode) attributeArgumentSyntax.NameColon, depth + 1);
        if (tree2 != null && tree2.Count >= 1)
          tree2[0].SymbolType = CodeTokenKind.CSAttributeParameterName;
      }
      if (attributeArgumentSyntax.NameEquals == null)
        return;
      List<CodeSymbol> tree3 = this.ParseTree((SyntaxNode) attributeArgumentSyntax.NameEquals, depth + 1);
      if (tree3 == null || tree3.Count < 1)
        return;
      tree3[0].SymbolType = CodeTokenKind.CSAttributeParameterName;
    }

    private void ParseAttribute(SyntaxNode node, int depth)
    {
      AttributeSyntax attributeSyntax = (AttributeSyntax) node;
      if (attributeSyntax.ArgumentList != null)
        this.ParseTree((SyntaxNode) attributeSyntax.ArgumentList, depth + 1);
      this.AddSymbol((SyntaxNode) attributeSyntax.Name, CodeTokenKind.CSAttributeName, depth);
    }

    private void ParseAsExpression(SyntaxNode node, int depth)
    {
      BinaryExpressionSyntax expressionSyntax = (BinaryExpressionSyntax) node;
      this.ParseTree((SyntaxNode) expressionSyntax.Left, depth + 1);
      this.AddSymbol(expressionSyntax.OperatorToken, CodeTokenKind.CSAsKeyword, depth);
      List<CodeSymbol> tree = this.ParseTree((SyntaxNode) expressionSyntax.Right, depth);
      if (tree == null || tree.Count < 1)
        return;
      tree[tree.Count - 1].SymbolType = CodeTokenKind.CSCast;
    }

    private void ParseObjectCreationExpression(SyntaxNode node, int depth)
    {
      ObjectCreationExpressionSyntax expressionSyntax = (ObjectCreationExpressionSyntax) node;
      if (expressionSyntax.ArgumentList != null)
        this.ParseTree((SyntaxNode) expressionSyntax.ArgumentList, depth + 1);
      if (expressionSyntax.Initializer != null)
        this.ParseTree((SyntaxNode) expressionSyntax.Initializer, depth + 1);
      this.AddSymbol(expressionSyntax.NewKeyword, CodeTokenKind.CSNewKeyword, depth + 1);
      this.AddSymbol((SyntaxNode) expressionSyntax.Type, CodeTokenKind.CSObjectCreationType, depth);
    }

    private void ParseLabeledStatement(SyntaxNode node, int depth)
    {
      LabeledStatementSyntax labeledStatementSyntax = (LabeledStatementSyntax) node;
      this.AddSymbol(labeledStatementSyntax.Identifier, CodeTokenKind.CSLabeledStatement, depth);
      this.ParseTree((SyntaxNode) labeledStatementSyntax.Statement, depth + 1);
      this.ParseTriva(labeledStatementSyntax.ColonToken, depth + 1);
    }

    private void ParseIndexerDeclaration(SyntaxNode node, int depth)
    {
      IndexerDeclarationSyntax declarationSyntax = (IndexerDeclarationSyntax) node;
      this.ParseTree((SyntaxNode) declarationSyntax.AccessorList, depth + 1);
      this.ParseTree((SyntaxList<SyntaxNode>) declarationSyntax.AttributeLists, depth + 1);
      if (declarationSyntax.ExplicitInterfaceSpecifier != null)
        this.ParseTree((SyntaxNode) declarationSyntax.ExplicitInterfaceSpecifier, depth + 1);
      if (declarationSyntax.ExpressionBody != null)
        this.ParseTree((SyntaxNode) declarationSyntax.ExpressionBody, depth + 1);
      this.ParseModifiers(declarationSyntax.Modifiers, depth + 1);
      this.ParseTree((SyntaxNode) declarationSyntax.ParameterList, depth + 1);
      this.AddSymbol(declarationSyntax.ThisKeyword, CodeTokenKind.CSThisKeyword, depth + 1);
      List<CodeSymbol> tree = this.ParseTree((SyntaxNode) declarationSyntax.Type, depth);
      if (tree.Count <= 0)
        return;
      tree[tree.Count - 1].SymbolType = CodeTokenKind.CSIndexerDeclaration;
    }

    private void ParseEventDeclaration(SyntaxNode node, int depth)
    {
      EventDeclarationSyntax declarationSyntax = (EventDeclarationSyntax) node;
      this.ParseTree((SyntaxNode) declarationSyntax.AccessorList, depth + 1);
      this.ParseTree((SyntaxList<SyntaxNode>) declarationSyntax.AttributeLists, depth + 1);
      this.AddSymbol(declarationSyntax.EventKeyword, CodeTokenKind.CSEventKeyword, depth + 1);
      if (declarationSyntax.ExplicitInterfaceSpecifier != null)
        this.ParseTree((SyntaxNode) declarationSyntax.ExplicitInterfaceSpecifier, depth + 1);
      this.AddSymbol(declarationSyntax.Identifier, CodeTokenKind.CSEventDeclaration, depth);
      this.ParseModifiers(declarationSyntax.Modifiers, depth + 1);
      List<CodeSymbol> tree = this.ParseTree((SyntaxNode) declarationSyntax.Type, depth + 1);
      if (tree.Count <= 0)
        return;
      tree[tree.Count - 1].SymbolType = CodeTokenKind.CSEventDeclarationType;
    }

    private void ParseOperatorDeclaration(SyntaxNode node, int depth)
    {
      OperatorDeclarationSyntax declarationSyntax = (OperatorDeclarationSyntax) node;
      this.ParseTree((SyntaxList<SyntaxNode>) declarationSyntax.AttributeLists, depth + 1);
      CodeTokenKind kind = CodeTokenKind.CSOperatorDeclaration;
      if (declarationSyntax.Body != null)
      {
        this.ParseTree((SyntaxNode) declarationSyntax.Body, depth + 1);
        kind = CodeTokenKind.CSOperatorDefinition;
      }
      if (declarationSyntax.ExpressionBody != null)
      {
        this.ParseTree((SyntaxNode) declarationSyntax.ExpressionBody, depth + 1);
        kind = CodeTokenKind.CSOperatorDefinition;
      }
      this.ParseModifiers(declarationSyntax.Modifiers, depth + 1);
      this.AddSymbol(declarationSyntax.OperatorKeyword, CodeTokenKind.CSOperatorKeyword, depth + 1);
      this.AddSymbol(declarationSyntax.OperatorToken, kind, depth + 1);
      this.ParseTree((SyntaxNode) declarationSyntax.ParameterList, depth + 1);
      this.AddSymbol((SyntaxNode) declarationSyntax.ReturnType, CodeTokenKind.CSReturnTypeDeclaration, depth);
      this.ParseTriva(declarationSyntax.SemicolonToken, depth + 1);
    }

    private void ParseExternAliasDirective(SyntaxNode node, int depth)
    {
      ExternAliasDirectiveSyntax aliasDirectiveSyntax = (ExternAliasDirectiveSyntax) node;
      this.AddSymbol(aliasDirectiveSyntax.AliasKeyword, CodeTokenKind.CSAliasKeyword, depth + 1);
      this.AddSymbol(aliasDirectiveSyntax.ExternKeyword, CodeTokenKind.CSExternKeyword, depth + 1);
      this.AddSymbol(aliasDirectiveSyntax.Identifier, CodeTokenKind.CSExternAliasDirective, depth);
      this.ParseTriva(aliasDirectiveSyntax.SemicolonToken, depth + 1);
    }

    private void ParseUsingDirective(SyntaxNode node, int depth)
    {
      UsingDirectiveSyntax usingDirectiveSyntax = (UsingDirectiveSyntax) node;
      if (usingDirectiveSyntax.Alias != null)
      {
        List<CodeSymbol> tree = this.ParseTree((SyntaxNode) usingDirectiveSyntax.Alias, depth + 1);
        if (tree.Count > 0)
          tree[tree.Count - 1].SymbolType = CodeTokenKind.CSAlias;
      }
      this.AddSymbolForFullyQualifiedName((SyntaxNode) usingDirectiveSyntax.Name, CodeTokenKind.CSUsing, depth);
      this.AddSymbol(usingDirectiveSyntax.UsingKeyword, CodeTokenKind.CSUsingKeyword, depth + 1);
      SyntaxToken staticKeyword = usingDirectiveSyntax.StaticKeyword;
      this.AddSymbol(usingDirectiveSyntax.StaticKeyword, CodeTokenKind.CSStaticKeyword, depth + 1);
      this.ParseTriva(usingDirectiveSyntax.SemicolonToken, depth + 1);
    }

    private void ParseNamespaceDeclaration(SyntaxNode node, int depth)
    {
      NamespaceDeclarationSyntax declarationSyntax = (NamespaceDeclarationSyntax) node;
      this.ParseTree((SyntaxList<SyntaxNode>) declarationSyntax.Externs, depth + 1);
      this.ParseTree((SyntaxList<SyntaxNode>) declarationSyntax.Members, depth + 1);
      this.AddSymbolForFullyQualifiedName((SyntaxNode) declarationSyntax.Name, CodeTokenKind.CSNamespace, depth);
      this.AddSymbol(declarationSyntax.NamespaceKeyword, CodeTokenKind.CSNamespaceKeyword, depth + 1);
      this.ParseTree((SyntaxList<SyntaxNode>) declarationSyntax.Usings, depth + 1);
      this.ParseTriva(declarationSyntax.OpenBraceToken, depth + 1);
      this.ParseTriva(declarationSyntax.CloseBraceToken, depth + 1);
      this.ParseTriva(declarationSyntax.SemicolonToken, depth + 1);
    }

    private void ParseEnumMemeberDeclaration(SyntaxNode node, int depth)
    {
      EnumMemberDeclarationSyntax declarationSyntax = (EnumMemberDeclarationSyntax) node;
      this.ParseTree((SyntaxList<SyntaxNode>) declarationSyntax.AttributeLists, depth + 1);
      if (declarationSyntax.EqualsValue != null)
        this.ParseTree((SyntaxNode) declarationSyntax.EqualsValue, depth + 1);
      this.AddSymbol(declarationSyntax.Identifier, CodeTokenKind.CSEnumMemberDeclaration, depth);
    }

    private void ParseEnumDeclaration(SyntaxNode node, int depth)
    {
      CodeTokenKind kind = CodeTokenKind.CSEnumDefinition;
      EnumDeclarationSyntax declarationSyntax = (EnumDeclarationSyntax) node;
      this.ParseTree((SyntaxList<SyntaxNode>) declarationSyntax.AttributeLists, depth + 1);
      if (declarationSyntax.BaseList != null)
        this.ParseTree((SyntaxNode) declarationSyntax.BaseList, depth + 1);
      this.AddSymbol(declarationSyntax.EnumKeyword, CodeTokenKind.CSEnumKeyword, depth + 1);
      int num = 0;
      foreach (SyntaxNodeOrToken withSeparator in declarationSyntax.Members.GetWithSeparators())
      {
        if (withSeparator.IsNode)
          num += this.ParseTree(withSeparator.AsNode(), depth + 1).Count;
        else
          this.ParseTriva(withSeparator.AsToken(), depth + 1);
      }
      this.AddSymbol(declarationSyntax.Identifier, kind, depth);
      this.ParseModifiers(declarationSyntax.Modifiers, depth + 1);
      this.ParseTriva(declarationSyntax.OpenBraceToken, depth + 1);
      this.ParseTriva(declarationSyntax.CloseBraceToken, depth + 1);
      this.ParseTriva(declarationSyntax.SemicolonToken, depth + 1);
    }

    private void ParseDoStatement(SyntaxNode node, int depth)
    {
      DoStatementSyntax doStatementSyntax = (DoStatementSyntax) node;
      this.ParseTree((SyntaxNode) doStatementSyntax.Condition, depth + 1);
      this.AddSymbol(doStatementSyntax.DoKeyword, CodeTokenKind.CSDoKeyword, depth);
      this.ParseTree((SyntaxNode) doStatementSyntax.Statement, depth + 1);
      this.AddSymbol(doStatementSyntax.WhileKeyword, CodeTokenKind.CSWhileKeyword, depth + 1);
      this.ParseTriva(doStatementSyntax.OpenParenToken, depth + 1);
      this.ParseTriva(doStatementSyntax.CloseParenToken, depth + 1);
      this.ParseTriva(doStatementSyntax.SemicolonToken, depth + 1);
    }

    private void ParseDestructorDeclaration(SyntaxNode node, int depth)
    {
      CodeTokenKind kind = CodeTokenKind.CSDestructorDeclaration;
      DestructorDeclarationSyntax declarationSyntax = (DestructorDeclarationSyntax) node;
      this.ParseTree((SyntaxList<SyntaxNode>) declarationSyntax.AttributeLists, depth + 1);
      if (declarationSyntax.Body != null)
      {
        this.ParseTree((SyntaxNode) declarationSyntax.Body, depth + 1);
        kind = CodeTokenKind.CSDestructorDefinition;
      }
      if (declarationSyntax.ExpressionBody != null)
        this.ParseTree((SyntaxNode) declarationSyntax.ExpressionBody, depth + 1);
      this.AddSymbol(declarationSyntax.Identifier, kind, depth);
      this.ParseModifiers(declarationSyntax.Modifiers, depth + 1);
      this.ParseTree((SyntaxNode) declarationSyntax.ParameterList, depth + 1);
      this.ParseTriva(declarationSyntax.TildeToken, depth + 1);
      this.ParseTriva(declarationSyntax.SemicolonToken, depth + 1);
    }

    private void ParseConstructorConstraint(SyntaxNode node, int depth)
    {
      ConstructorConstraintSyntax constraintSyntax = (ConstructorConstraintSyntax) node;
      this.AddSymbol(constraintSyntax.NewKeyword, CodeTokenKind.CSNewKeyword, depth);
      this.ParseTriva(constraintSyntax.OpenParenToken, depth + 1);
      this.ParseTriva(constraintSyntax.CloseParenToken, depth + 1);
    }

    private void ParseDelegateDeclaration(SyntaxNode node, int depth)
    {
      DelegateDeclarationSyntax declarationSyntax = (DelegateDeclarationSyntax) node;
      this.ParseTree((SyntaxList<SyntaxNode>) declarationSyntax.AttributeLists, depth + 1);
      this.ParseTree((SyntaxList<SyntaxNode>) declarationSyntax.ConstraintClauses, depth + 1);
      this.AddSymbol(declarationSyntax.DelegateKeyword, this.GetTokenKind(declarationSyntax.DelegateKeyword.Kind()), depth + 1);
      this.AddSymbol(declarationSyntax.Identifier, CodeTokenKind.CSDelegateDeclaration, depth);
      this.ParseModifiers(declarationSyntax.Modifiers, depth + 1);
      this.ParseTree((SyntaxNode) declarationSyntax.ParameterList, depth + 1);
      List<CodeSymbol> tree = this.ParseTree((SyntaxNode) declarationSyntax.ReturnType, depth + 1);
      if (tree.Count != 0)
        tree[tree.Count - 1].SymbolType = CodeTokenKind.CSReturnTypeDeclaration;
      if (declarationSyntax.TypeParameterList != null)
        this.ParseTree((SyntaxNode) declarationSyntax.TypeParameterList, depth + 1);
      this.ParseTriva(declarationSyntax.SemicolonToken, depth + 1);
    }

    private void ParseConversionOperatorDeclaration(SyntaxNode node, int depth)
    {
      CodeTokenKind codeTokenKind = CodeTokenKind.CSConversionOperatorTypeDeclaration;
      ConversionOperatorDeclarationSyntax declarationSyntax = (ConversionOperatorDeclarationSyntax) node;
      this.ParseTree((SyntaxList<SyntaxNode>) declarationSyntax.AttributeLists, depth + 1);
      if (declarationSyntax.Body != null)
      {
        this.ParseTree((SyntaxNode) declarationSyntax.Body, depth + 1);
        codeTokenKind = CodeTokenKind.CSConversionOperatorTypeDefinition;
      }
      if (declarationSyntax.ExpressionBody != null)
      {
        this.ParseTree((SyntaxNode) declarationSyntax.ExpressionBody, depth + 1);
        codeTokenKind = CodeTokenKind.CSConversionOperatorTypeDefinition;
      }
      this.AddSymbol(declarationSyntax.ImplicitOrExplicitKeyword, this.GetTokenKind(declarationSyntax.ImplicitOrExplicitKeyword.Kind()), depth + 1);
      this.ParseModifiers(declarationSyntax.Modifiers, depth + 1);
      this.AddSymbol(declarationSyntax.OperatorKeyword, this.GetTokenKind(declarationSyntax.OperatorKeyword.Kind()), depth + 1);
      this.ParseTree((SyntaxNode) declarationSyntax.ParameterList, depth + 1);
      List<CodeSymbol> tree = this.ParseTree((SyntaxNode) declarationSyntax.Type, depth);
      tree[tree.Count - 1].SymbolType = codeTokenKind;
      this.ParseTriva(declarationSyntax.SemicolonToken, depth + 1);
    }

    private void ParsePrimaryFunction(SyntaxNode node, int depth)
    {
      CheckedExpressionSyntax expressionSyntax = (CheckedExpressionSyntax) node;
      List<CodeSymbol> tree = this.ParseTree((SyntaxNode) expressionSyntax.Expression, depth + 1);
      if (tree != null)
      {
        foreach (CodeSymbol codeSymbol in tree)
          codeSymbol.SymbolType = CodeTokenKind.CSArgument;
      }
      this.AddSymbol(expressionSyntax.Keyword, this.GetTokenKind(expressionSyntax.Keyword.Kind()), depth);
    }

    private void ParseConstructorInitializer(SyntaxNode node, int depth)
    {
      ConstructorInitializerSyntax initializerSyntax = (ConstructorInitializerSyntax) node;
      this.ParseTree((SyntaxNode) initializerSyntax.ArgumentList, depth + 1);
      this.AddSymbol(initializerSyntax.ThisOrBaseKeyword, this.GetTokenKind(initializerSyntax.ThisOrBaseKeyword.Kind()), depth);
      this.ParseTriva(initializerSyntax.ColonToken, depth + 1);
    }

    private void ParseContructorDeclaration(SyntaxNode node, int depth)
    {
      CodeTokenKind kind = CodeTokenKind.CSConstructorDeclaration;
      ConstructorDeclarationSyntax declarationSyntax = (ConstructorDeclarationSyntax) node;
      this.ParseTree((SyntaxList<SyntaxNode>) declarationSyntax.AttributeLists, depth + 1);
      if (declarationSyntax.Body != null)
      {
        this.ParseTree((SyntaxNode) declarationSyntax.Body, depth + 1);
        kind = CodeTokenKind.CSConstructorDefinition;
      }
      if (declarationSyntax.ExpressionBody != null)
      {
        this.ParseTree((SyntaxNode) declarationSyntax.ExpressionBody, depth + 1);
        kind = CodeTokenKind.CSConstructorDefinition;
      }
      this.AddSymbol(declarationSyntax.Identifier, kind, depth);
      if (declarationSyntax.Initializer != null)
        this.ParseTree((SyntaxNode) declarationSyntax.Initializer, depth + 1);
      this.ParseModifiers(declarationSyntax.Modifiers, depth + 1);
      this.ParseTree((SyntaxNode) declarationSyntax.ParameterList, depth + 1);
      this.ParseTriva(declarationSyntax.SemicolonToken, depth + 1);
    }

    private void ParseFinallyClause(SyntaxNode node, int depth)
    {
      FinallyClauseSyntax finallyClauseSyntax = (FinallyClauseSyntax) node;
      this.ParseTree((SyntaxNode) finallyClauseSyntax.Block, depth + 1);
      this.AddSymbol(finallyClauseSyntax.FinallyKeyword, CodeTokenKind.CSFinallyKeyword, depth);
    }

    private void ParseCatchDeclaration(SyntaxNode node, int depth)
    {
      CatchDeclarationSyntax declarationSyntax = (CatchDeclarationSyntax) node;
      if (declarationSyntax.Identifier.Kind() != SyntaxKind.None)
        this.AddSymbol(declarationSyntax.Identifier, CodeTokenKind.CSCatchDeclarationIdentifier, depth + 1);
      List<CodeSymbol> tree = this.ParseTree((SyntaxNode) declarationSyntax.Type, depth);
      tree[tree.Count - 1].SymbolType = CodeTokenKind.CSCatchDeclarationType;
      foreach (CodeSymbol codeSymbol in tree)
      {
        if (codeSymbol.SymbolType == CodeTokenKind.CSUnknown || codeSymbol.SymbolType == CodeTokenKind.CSIdentifier)
          codeSymbol.SymbolType = CodeTokenKind.CSCatchDeclarationType;
      }
      this.ParseTriva(declarationSyntax.OpenParenToken, depth + 1);
      this.ParseTriva(declarationSyntax.CloseParenToken, depth + 1);
    }

    private void ParseCatch(SyntaxNode node, int depth)
    {
      CatchClauseSyntax catchClauseSyntax = (CatchClauseSyntax) node;
      this.ParseTree((SyntaxNode) catchClauseSyntax.Block, depth + 1);
      this.AddSymbol(catchClauseSyntax.CatchKeyword, CodeTokenKind.CSCatchKeyword, depth);
      if (catchClauseSyntax.Filter != null)
        this.ParseTree((SyntaxNode) catchClauseSyntax.Filter, depth + 1);
      if (catchClauseSyntax.Declaration == null)
        return;
      this.ParseTree((SyntaxNode) catchClauseSyntax.Declaration, depth + 1);
    }

    private void ParseTryStatement(SyntaxNode node, int depth)
    {
      TryStatementSyntax tryStatementSyntax = (TryStatementSyntax) node;
      this.ParseTree((SyntaxNode) tryStatementSyntax.Block, depth + 1);
      this.ParseTree((SyntaxList<SyntaxNode>) tryStatementSyntax.Catches, depth + 1);
      if (tryStatementSyntax.Finally != null)
        this.ParseTree((SyntaxNode) tryStatementSyntax.Finally, depth + 1);
      this.AddSymbol(tryStatementSyntax.TryKeyword, CodeTokenKind.CSTryKeyword, depth);
    }

    private void ParseMemberAccess(SyntaxNode node, int depth)
    {
      MemberAccessExpressionSyntax expressionSyntax = (MemberAccessExpressionSyntax) node;
      List<CodeSymbol> codeSymbolList = new List<CodeSymbol>();
      codeSymbolList.AddRange((IEnumerable<CodeSymbol>) this.ParseTree((SyntaxNode) expressionSyntax.Expression, depth + 1));
      codeSymbolList.AddRange((IEnumerable<CodeSymbol>) this.ParseTree((SyntaxNode) expressionSyntax.Name, depth));
      foreach (CodeSymbol codeSymbol in codeSymbolList)
      {
        if (codeSymbol.SymbolType == CodeTokenKind.CSUnknown || codeSymbol.SymbolType == CodeTokenKind.CSIdentifier)
          codeSymbol.SymbolType = CodeTokenKind.CSMemberAccess;
      }
      this.ParseTriva(expressionSyntax.OperatorToken, depth + 1);
    }

    private void ParseQualifiesName(SyntaxNode node, int depth)
    {
      QualifiedNameSyntax qualifiedNameSyntax = (QualifiedNameSyntax) node;
      this.ParseTree((SyntaxNode) qualifiedNameSyntax.Left, depth + 1);
      this.ParseTree((SyntaxNode) qualifiedNameSyntax.Right, depth);
      this.ParseTriva(qualifiedNameSyntax.DotToken, depth + 1);
    }

    private void ParseAssign(SyntaxNode node, int depth)
    {
      AssignmentExpressionSyntax expressionSyntax = (AssignmentExpressionSyntax) node;
      List<CodeSymbol> tree = this.ParseTree((SyntaxNode) expressionSyntax.Left, depth + 1);
      tree[tree.Count - 1].SymbolType = CodeTokenKind.CSAssignment;
      this.ParseTriva(expressionSyntax.OperatorToken, depth + 1);
      this.ParseTree((SyntaxNode) expressionSyntax.Right, depth);
    }

    private void ParseNameColon(SyntaxNode node, int depth)
    {
      NameColonSyntax nameColonSyntax = (NameColonSyntax) node;
      this.AddSymbol((SyntaxNode) nameColonSyntax.Name, CodeTokenKind.CSNamedArgument, depth);
      this.ParseTriva(nameColonSyntax.ColonToken, depth + 1);
    }

    private void ParseIdentifier(SyntaxNode node, int depth) => this.AddSymbol(((SimpleNameSyntax) node).Identifier, this.GetTokenKind(node.Kind()), depth);

    private void ParsePrimaryExpression(SyntaxNode node, int depth)
    {
      LiteralExpressionSyntax expressionSyntax = (LiteralExpressionSyntax) node;
      this.AddLiteralSymbol(expressionSyntax.Token, this.GetTokenKind(expressionSyntax.Kind()), depth);
    }

    private void ParsePredefinedType(SyntaxNode node, int depth)
    {
      PredefinedTypeSyntax predefinedTypeSyntax = (PredefinedTypeSyntax) node;
      this.AddSymbol((SyntaxNode) predefinedTypeSyntax, this.GetTokenKind(predefinedTypeSyntax.Keyword.Kind()), depth);
    }

    private void ParseCast(SyntaxNode node, int depth)
    {
      CastExpressionSyntax expressionSyntax = (CastExpressionSyntax) node;
      this.ParseTree((SyntaxNode) expressionSyntax.Expression, depth + 1);
      List<CodeSymbol> tree = this.ParseTree((SyntaxNode) expressionSyntax.Type, depth);
      tree[tree.Count - 1].SymbolType = CodeTokenKind.CSCast;
      this.ParseTriva(expressionSyntax.OpenParenToken, depth + 1);
      this.ParseTriva(expressionSyntax.CloseParenToken, depth + 1);
    }

    private void ParseArrayType(SyntaxNode node, int depth)
    {
      ArrayTypeSyntax arrayTypeSyntax = (ArrayTypeSyntax) node;
      this.ParseTree((SyntaxList<SyntaxNode>) arrayTypeSyntax.RankSpecifiers, depth + 1);
      this.ParseTree((SyntaxNode) arrayTypeSyntax.ElementType, depth);
      this.SymbolsList[this.SymbolsList.Count - 1].SymbolType = CodeTokenKind.CSArrayCreationType;
    }

    private void ParseArrayCreation(SyntaxNode node, int depth)
    {
      ArrayCreationExpressionSyntax expressionSyntax = (ArrayCreationExpressionSyntax) node;
      if (expressionSyntax.Initializer != null)
        this.ParseTree((SyntaxNode) expressionSyntax.Initializer, depth + 1);
      this.AddSymbol(expressionSyntax.NewKeyword, CodeTokenKind.CSArrayCreation, depth + 1);
      this.ParseTree((SyntaxNode) expressionSyntax.Type, depth);
    }

    private void ParseAnonymousObjectCreationExpression(SyntaxNode node, int depth)
    {
      AnonymousObjectCreationExpressionSyntax expressionSyntax = (AnonymousObjectCreationExpressionSyntax) node;
      this.ParseTree(expressionSyntax.Initializers, depth);
      this.AddSymbol(expressionSyntax.NewKeyword, CodeTokenKind.CSAnonymousObjectCreation, depth + 1);
    }

    private void ParseAnonymousMethod(SyntaxNode node, int depth)
    {
      AnonymousMethodExpressionSyntax expressionSyntax = (AnonymousMethodExpressionSyntax) node;
      if (expressionSyntax.Block != null)
        this.ParseTree((SyntaxNode) expressionSyntax.Block, depth + 1);
      if (expressionSyntax.Body != null)
        this.ParseTree((SyntaxNode) expressionSyntax.Body, depth + 1);
      this.AddSymbol(expressionSyntax.DelegateKeyword, this.GetTokenKind(expressionSyntax.DelegateKeyword.Kind()), depth);
      if (expressionSyntax.ParameterList == null)
        return;
      this.ParseTree((SyntaxNode) expressionSyntax.ParameterList, depth + 1);
    }

    private void ParseGenericName(SyntaxNode node, int depth)
    {
      GenericNameSyntax genericNameSyntax = (GenericNameSyntax) node;
      this.ParseTree((SyntaxNode) genericNameSyntax.TypeArgumentList, depth + 1);
      this.AddSymbol(genericNameSyntax.Identifier, this.GetTokenKind(genericNameSyntax.Identifier.Kind()), depth);
    }

    private void ParseAliasName(SyntaxNode node, int depth)
    {
      AliasQualifiedNameSyntax qualifiedNameSyntax = (AliasQualifiedNameSyntax) node;
      foreach (CodeSymbol codeSymbol in this.ParseTree((SyntaxNode) qualifiedNameSyntax.Alias, depth))
      {
        if (codeSymbol.SymbolType == CodeTokenKind.CSIdentifier)
          codeSymbol.SymbolType = CodeTokenKind.CSAlias;
      }
      this.ParseTree((SyntaxNode) qualifiedNameSyntax.Name, depth + 1);
    }

    private void ParseFieldDeclaration(SyntaxNode node, int depth)
    {
      FieldDeclarationSyntax declarationSyntax = (FieldDeclarationSyntax) node;
      this.ParseTree((SyntaxList<SyntaxNode>) declarationSyntax.AttributeLists, depth + 1);
      foreach (CodeSymbol codeSymbol in this.ParseTree((SyntaxNode) declarationSyntax.Declaration, depth))
      {
        switch (codeSymbol.SymbolType)
        {
          case CodeTokenKind.CSVariableDeclarationType:
            codeSymbol.SymbolType = CodeTokenKind.CSFieldDeclarationType;
            continue;
          case CodeTokenKind.CSVariableDeclaration:
            codeSymbol.SymbolType = CodeTokenKind.CSFieldDeclaration;
            continue;
          case CodeTokenKind.CSVariableDeclarationAndAssignment:
            codeSymbol.SymbolType = CodeTokenKind.CSFieldDeclarationAndAssignment;
            continue;
          default:
            continue;
        }
      }
      this.ParseModifiers(declarationSyntax.Modifiers, depth + 1);
      this.ParseTriva(declarationSyntax.SemicolonToken, depth + 1);
    }

    private void ParseBaseList(SyntaxNode node, int depth)
    {
      BaseListSyntax baseListSyntax = (BaseListSyntax) node;
      foreach (SyntaxNode type in baseListSyntax.Types)
        this.AddSymbol(type, CodeTokenKind.CSBaseType, depth);
      foreach (SyntaxNodeOrToken withSeparator in baseListSyntax.Types.GetWithSeparators())
      {
        if (withSeparator.IsToken)
          this.ParseTriva(withSeparator.AsToken(), depth + 1);
      }
      this.ParseTriva(baseListSyntax.ColonToken, depth + 1);
    }

    private void ParseTypeDeclaration(SyntaxNode node, int depth)
    {
      TypeDeclarationSyntax declarationSyntax = (TypeDeclarationSyntax) node;
      this.ParseTree((SyntaxList<SyntaxNode>) declarationSyntax.AttributeLists, depth + 1);
      if (declarationSyntax.BaseList != null)
        this.ParseTree((SyntaxNode) declarationSyntax.BaseList, depth + 1);
      this.ParseTree((SyntaxList<SyntaxNode>) declarationSyntax.ConstraintClauses, depth + 1);
      this.AddSymbol(declarationSyntax.Identifier, this.GetTokenKind(declarationSyntax.Kind()), depth);
      this.AddSymbol(declarationSyntax.Keyword, this.GetTokenKind(declarationSyntax.Keyword.Kind()), depth + 1);
      this.ParseTree((SyntaxList<SyntaxNode>) declarationSyntax.Members, depth + 1);
      this.ParseModifiers(declarationSyntax.Modifiers, depth + 1);
      this.ParseTriva(declarationSyntax.OpenBraceToken, depth + 1);
      this.ParseTriva(declarationSyntax.CloseBraceToken, depth + 1);
      this.ParseTriva(declarationSyntax.SemicolonToken, depth + 1);
      if (declarationSyntax.TypeParameterList == null)
        return;
      this.ParseTree((SyntaxNode) declarationSyntax.TypeParameterList, depth + 1);
    }

    private void ParseArgument(SyntaxNode node, int depth)
    {
      ArgumentSyntax argumentSyntax = (ArgumentSyntax) node;
      if (argumentSyntax.NameColon != null)
        this.ParseTree((SyntaxNode) argumentSyntax.NameColon, depth);
      if (argumentSyntax.RefOrOutKeyword.Kind() != SyntaxKind.None)
        this.AddSymbol(argumentSyntax.RefOrOutKeyword, this.GetTokenKind(argumentSyntax.RefOrOutKeyword.Kind()), depth + 1);
      List<CodeSymbol> tree = this.ParseTree((SyntaxNode) argumentSyntax.Expression, depth);
      if (tree == null || tree.Count <= 0)
        return;
      CodeTokenKind codeTokenKind = tree[tree.Count - 1].SymbolType;
      switch (codeTokenKind)
      {
        case CodeTokenKind.CSStringLiteral:
        case CodeTokenKind.CSInvocation:
          tree[tree.Count - 1].SymbolType = codeTokenKind;
          break;
        default:
          codeTokenKind = CodeTokenKind.CSArgument;
          goto case CodeTokenKind.CSStringLiteral;
      }
    }

    private void ParseInvocation(SyntaxNode node, int depth)
    {
      InvocationExpressionSyntax expressionSyntax = (InvocationExpressionSyntax) node;
      this.ParseTree((SyntaxNode) expressionSyntax.ArgumentList, depth + 1);
      this.ParseTree((SyntaxNode) expressionSyntax.Expression, depth);
      this.SymbolsList[this.SymbolsList.Count - 1].SymbolType = CodeTokenKind.CSInvocation;
    }

    private void ParseVariableDeclaration(SyntaxNode node, int depth)
    {
      VariableDeclarationSyntax declarationSyntax = (VariableDeclarationSyntax) node;
      foreach (SyntaxNodeOrToken withSeparator in declarationSyntax.Variables.GetWithSeparators())
      {
        if (withSeparator.IsNode)
        {
          VariableDeclaratorSyntax declaratorSyntax = withSeparator.AsNode() as VariableDeclaratorSyntax;
          CodeTokenKind kind = CodeTokenKind.CSVariableDeclaration;
          if (declaratorSyntax.Initializer != null)
          {
            kind = CodeTokenKind.CSVariableDeclarationAndAssignment;
            this.ParseTree((SyntaxNode) declaratorSyntax.Initializer, depth + 2);
          }
          if (declaratorSyntax.ArgumentList != null)
            this.ParseTree((SyntaxNode) declaratorSyntax.ArgumentList, depth + 2);
          this.AddSymbol(declaratorSyntax.Identifier, kind, depth + 1);
        }
        else
          this.ParseTriva(withSeparator.AsToken(), depth + 1);
      }
      this.ParseTree((SyntaxNode) declarationSyntax.Type, depth);
      this.SymbolsList[this.SymbolsList.Count - 1].SymbolType = CodeTokenKind.CSVariableDeclarationType;
    }

    private void ParseReturnStatement(SyntaxNode node, int depth)
    {
      ReturnStatementSyntax returnStatementSyntax = (ReturnStatementSyntax) node;
      this.AddSymbol(returnStatementSyntax.ReturnKeyword, this.GetTokenKind(returnStatementSyntax.Kind()), depth);
      if (returnStatementSyntax.Expression != null)
        this.ParseTree((SyntaxNode) returnStatementSyntax.Expression, depth + 1);
      this.ParseTriva(returnStatementSyntax.SemicolonToken, depth + 1);
    }

    private void ParseLocalFunctionStatement(SyntaxNode node, int depth)
    {
      LocalFunctionStatementSyntax functionStatementSyntax = (LocalFunctionStatementSyntax) node;
      CodeTokenKind kind = CodeTokenKind.CSMethodDeclaration;
      if (functionStatementSyntax.Body != null)
      {
        this.ParseTree((SyntaxNode) functionStatementSyntax.Body, depth + 1);
        kind = CodeTokenKind.CSMethodDefinition;
      }
      SyntaxList<TypeParameterConstraintClauseSyntax> constraintClauses = functionStatementSyntax.ConstraintClauses;
      this.ParseTree((SyntaxList<SyntaxNode>) functionStatementSyntax.ConstraintClauses, depth + 1);
      SyntaxTokenList modifiers = functionStatementSyntax.Modifiers;
      this.ParseModifiers(functionStatementSyntax.Modifiers, depth + 1);
      if (functionStatementSyntax.ParameterList != null)
        this.ParseTree((SyntaxNode) functionStatementSyntax.ParameterList, depth + 1);
      if (functionStatementSyntax.TypeParameterList != null)
        this.ParseTree((SyntaxNode) functionStatementSyntax.TypeParameterList, depth + 1);
      if (functionStatementSyntax.ReturnType != null)
        this.AddSymbol((SyntaxNode) functionStatementSyntax.ReturnType, CodeTokenKind.CSReturnTypeDeclaration, depth + 1);
      SyntaxToken identifier = functionStatementSyntax.Identifier;
      this.AddSymbol(functionStatementSyntax.Identifier, kind, depth);
      if (functionStatementSyntax.ExpressionBody != null)
        this.ParseTree((SyntaxNode) functionStatementSyntax.ExpressionBody, depth + 1);
      this.ParseTriva(functionStatementSyntax.SemicolonToken, depth + 1);
    }

    private void ParseAccessor(SyntaxNode node, int depth)
    {
      CodeTokenKind kind = CodeTokenKind.CSAccessorDeclaration;
      AccessorDeclarationSyntax declarationSyntax = (AccessorDeclarationSyntax) node;
      this.ParseTree((SyntaxList<SyntaxNode>) declarationSyntax.AttributeLists, depth + 1);
      if (declarationSyntax.ExpressionBody != null)
      {
        this.ParseTree((SyntaxNode) declarationSyntax.ExpressionBody, depth + 1);
        kind = CodeTokenKind.CSAccessorDefinition;
      }
      if (declarationSyntax.Body != null)
      {
        this.ParseTree((SyntaxNode) declarationSyntax.Body, depth + 1);
        kind = CodeTokenKind.CSAccessorDefinition;
      }
      this.AddSymbol(declarationSyntax.Keyword, kind, depth + 1);
      this.ParseModifiers(declarationSyntax.Modifiers, depth + 1);
      this.ParseTriva(declarationSyntax.SemicolonToken, depth + 1);
    }

    private void ParsePropertyDeclaration(SyntaxNode node, int depth)
    {
      PropertyDeclarationSyntax declarationSyntax = (PropertyDeclarationSyntax) node;
      if (declarationSyntax.AccessorList != null)
        this.ParseTree((SyntaxNode) declarationSyntax.AccessorList, depth + 1);
      if (declarationSyntax.ExpressionBody != null)
        this.ParseTree((SyntaxNode) declarationSyntax.ExpressionBody, depth + 1);
      this.ParseTree((SyntaxList<SyntaxNode>) declarationSyntax.AttributeLists, depth + 1);
      if (declarationSyntax.ExplicitInterfaceSpecifier != null)
        this.ParseTree((SyntaxNode) declarationSyntax.ExplicitInterfaceSpecifier, depth + 1);
      if (declarationSyntax.Initializer != null)
        this.ParseTree((SyntaxNode) declarationSyntax.Initializer, depth + 1);
      this.AddSymbol(declarationSyntax.Identifier, CodeTokenKind.CSPropertyDeclarationName, depth);
      this.ParseModifiers(declarationSyntax.Modifiers, depth + 1);
      this.AddSymbol((SyntaxNode) declarationSyntax.Type, CodeTokenKind.CSPropertyDeclarationType, depth + 1);
    }

    private void ParseTypeParameter(SyntaxNode node, int depth)
    {
      TypeParameterSyntax typeParameterSyntax = (TypeParameterSyntax) node;
      this.ParseTree((SyntaxList<SyntaxNode>) typeParameterSyntax.AttributeLists, depth + 1);
      this.AddSymbol(typeParameterSyntax.Identifier, CodeTokenKind.CSTypeParameter, depth);
      if (typeParameterSyntax.VarianceKeyword.Value == null)
        return;
      this.AddSymbol(typeParameterSyntax.VarianceKeyword, CodeTokenKind.CSVarianceKeyword, depth + 1);
    }

    private void ParseConstraint(SyntaxNode node, int depth) => this.AddSymbol((SyntaxNode) ((TypeConstraintSyntax) node).Type, CodeTokenKind.CSConstraintType, depth);

    private void ParseContraintClause(SyntaxNode node, int depth)
    {
      TypeParameterConstraintClauseSyntax constraintClauseSyntax = (TypeParameterConstraintClauseSyntax) node;
      foreach (SyntaxNode constraint in constraintClauseSyntax.Constraints)
        this.ParseTree(constraint, depth + 1);
      this.AddSymbol((SyntaxNode) constraintClauseSyntax.Name, CodeTokenKind.CSConstraintIdentifier, depth);
      this.AddSymbol(constraintClauseSyntax.WhereKeyword, CodeTokenKind.CSWhereKeyword, depth + 1);
      this.ParseTriva(constraintClauseSyntax.ColonToken, depth + 1);
    }

    private void ParseParameter(SyntaxNode node, int depth)
    {
      ParameterSyntax parameterSyntax = (ParameterSyntax) node;
      this.ParseTree((SyntaxList<SyntaxNode>) parameterSyntax.AttributeLists, depth + 1);
      EqualsValueClauseSyntax valueClauseSyntax = parameterSyntax.Default;
      if (valueClauseSyntax != null)
        this.AddSymbol((SyntaxNode) valueClauseSyntax.Value, CodeTokenKind.CSDefaultParameterValue, depth + 1);
      this.AddSymbol(parameterSyntax.Identifier, CodeTokenKind.CSParameterName, depth + 1);
      this.ParseModifiers(parameterSyntax.Modifiers, depth + 1);
      if (parameterSyntax.Type == null)
        return;
      this.AddSymbol((SyntaxNode) parameterSyntax.Type, CodeTokenKind.CSParameterType, depth);
    }

    private void ParseMethodDeclaration(SyntaxNode node, int depth)
    {
      MethodDeclarationSyntax declarationSyntax = (MethodDeclarationSyntax) node;
      CodeTokenKind kind = CodeTokenKind.CSMethodDeclaration;
      this.ParseTree((SyntaxList<SyntaxNode>) declarationSyntax.AttributeLists, depth + 1);
      this.ParseTree((SyntaxList<SyntaxNode>) declarationSyntax.ConstraintClauses, depth + 1);
      this.AddSymbol((SyntaxNode) declarationSyntax.ReturnType, CodeTokenKind.CSReturnTypeDeclaration, depth + 1);
      this.ParseModifiers(declarationSyntax.Modifiers, depth + 1);
      this.ParseTree((SyntaxNode) declarationSyntax.ParameterList, depth + 1);
      if (declarationSyntax.TypeParameterList != null)
        this.ParseTree((SyntaxNode) declarationSyntax.TypeParameterList, depth + 1);
      if (declarationSyntax.Body != null)
      {
        this.ParseTree((SyntaxNode) declarationSyntax.Body, depth + 1);
        kind = CodeTokenKind.CSMethodDefinition;
      }
      if (declarationSyntax.ExpressionBody != null)
      {
        this.ParseTree((SyntaxNode) declarationSyntax.ExpressionBody, depth + 1);
        kind = CodeTokenKind.CSMethodDefinition;
      }
      this.AddSymbol(declarationSyntax.Identifier, kind, depth);
      if (declarationSyntax.ExplicitInterfaceSpecifier != null)
        this.ParseTree((SyntaxNode) declarationSyntax.ExplicitInterfaceSpecifier, depth + 1);
      this.ParseTriva(declarationSyntax.SemicolonToken, depth + 1);
    }

    private void ParseModifiers(SyntaxTokenList syntaxTokenList, int depth)
    {
      foreach (SyntaxToken syntaxToken in syntaxTokenList)
        this.AddSymbol(syntaxToken, this.GetTokenKind(syntaxToken.Kind()), depth);
    }

    [SuppressMessage("Microsoft.Usage", "CA2233:OperationsShouldNotOverflow", MessageId = "depth+1", Justification = "depth is not likely to exceed int.Max range.")]
    protected override void AddSymbol(SyntaxNode syntaxNode, CodeTokenKind kind, int depth)
    {
      foreach (SyntaxToken descendantToken in syntaxNode.DescendantTokens())
      {
        if (!this.IgnoredKind(descendantToken.Kind()))
        {
          this.AddSymbol(descendantToken, kind, depth);
        }
        else
        {
          if (depth == int.MaxValue)
            throw new ArgumentOutOfRangeException(nameof (depth), "input must be less than Int32.MaxValue");
          this.ParseTriva(descendantToken, depth + 1);
        }
      }
    }

    private void ParseTriva(SyntaxToken token, int depth)
    {
      if (!this.IgnoredKind(token.Kind()))
      {
        this.AddSymbol(token, this.GetTokenKind(token.Kind()), depth);
      }
      else
      {
        if (token.HasLeadingTrivia)
          this.ParseTriva(token.LeadingTrivia, depth);
        if (!token.HasTrailingTrivia)
          return;
        this.ParseTriva(token.TrailingTrivia, depth + 1);
      }
    }

    protected override void ParseTriva(SyntaxTriviaList syntaxTriviaList, int depth)
    {
      foreach (SyntaxTrivia syntaxTrivia in syntaxTriviaList)
      {
        if (!this.IgnoredKind(syntaxTrivia.Kind()))
        {
          switch (syntaxTrivia.Kind())
          {
            case SyntaxKind.SingleLineCommentTrivia:
            case SyntaxKind.MultiLineCommentTrivia:
            case SyntaxKind.SingleLineDocumentationCommentTrivia:
            case SyntaxKind.MultiLineDocumentationCommentTrivia:
              this.ParseCommentTriva(syntaxTrivia, CodeTokenKind.CSComment);
              continue;
            case SyntaxKind.DefineDirectiveTrivia:
              this.ParseDefineDirective(syntaxTrivia, depth);
              continue;
            default:
              using (List<CodeSymbol>.Enumerator enumerator = this.Tokenize(syntaxTrivia, CodeTokenKind.CSUnknown).GetEnumerator())
              {
                while (enumerator.MoveNext())
                  this.AddSymbol(this.SymbolsList, enumerator.Current);
                continue;
              }
          }
        }
      }
    }

    private void ParseDefineDirective(SyntaxTrivia triva, int depth)
    {
      DefineDirectiveTriviaSyntax structure = (DefineDirectiveTriviaSyntax) triva.GetStructure();
      this.AddSymbol(structure.DefineKeyword, CodeTokenKind.CSDefineKeyword, depth + 1);
      this.AddSymbol(structure.Name, CodeTokenKind.CSDefineDirective, depth);
      this.ParseTriva(structure.EndOfDirectiveToken, depth + 1);
    }

    private bool IgnoredKind(SyntaxKind syntaxKind)
    {
      if ((uint) syntaxKind <= 8496U)
      {
        switch (syntaxKind)
        {
          case SyntaxKind.TildeToken:
          case SyntaxKind.ExclamationToken:
          case SyntaxKind.DollarToken:
          case SyntaxKind.PercentToken:
          case SyntaxKind.CaretToken:
          case SyntaxKind.AmpersandToken:
          case SyntaxKind.AsteriskToken:
          case SyntaxKind.OpenParenToken:
          case SyntaxKind.CloseParenToken:
          case SyntaxKind.MinusToken:
          case SyntaxKind.PlusToken:
          case SyntaxKind.EqualsToken:
          case SyntaxKind.OpenBraceToken:
          case SyntaxKind.CloseBraceToken:
          case SyntaxKind.OpenBracketToken:
          case SyntaxKind.CloseBracketToken:
          case SyntaxKind.BarToken:
          case SyntaxKind.BackslashToken:
          case SyntaxKind.ColonToken:
          case SyntaxKind.SemicolonToken:
          case SyntaxKind.DoubleQuoteToken:
          case SyntaxKind.SingleQuoteToken:
          case SyntaxKind.LessThanToken:
          case SyntaxKind.CommaToken:
          case SyntaxKind.GreaterThanToken:
          case SyntaxKind.DotToken:
          case SyntaxKind.QuestionToken:
          case SyntaxKind.HashToken:
          case SyntaxKind.SlashToken:
          case SyntaxKind.SlashGreaterThanToken:
          case SyntaxKind.LessThanSlashToken:
          case SyntaxKind.BarBarToken:
          case SyntaxKind.AmpersandAmpersandToken:
          case SyntaxKind.MinusMinusToken:
          case SyntaxKind.PlusPlusToken:
          case SyntaxKind.ColonColonToken:
          case SyntaxKind.QuestionQuestionToken:
          case SyntaxKind.MinusGreaterThanToken:
          case SyntaxKind.ExclamationEqualsToken:
          case SyntaxKind.EqualsEqualsToken:
          case SyntaxKind.EqualsGreaterThanToken:
          case SyntaxKind.LessThanEqualsToken:
          case SyntaxKind.LessThanLessThanToken:
          case SyntaxKind.LessThanLessThanEqualsToken:
          case SyntaxKind.GreaterThanEqualsToken:
          case SyntaxKind.GreaterThanGreaterThanToken:
          case SyntaxKind.GreaterThanGreaterThanEqualsToken:
          case SyntaxKind.SlashEqualsToken:
          case SyntaxKind.AsteriskEqualsToken:
          case SyntaxKind.BarEqualsToken:
          case SyntaxKind.AmpersandEqualsToken:
          case SyntaxKind.PlusEqualsToken:
          case SyntaxKind.MinusEqualsToken:
          case SyntaxKind.CaretEqualsToken:
          case SyntaxKind.PercentEqualsToken:
          case SyntaxKind.EndOfFileToken:
            break;
          default:
            goto label_4;
        }
      }
      else
      {
        switch (syntaxKind)
        {
          case SyntaxKind.XmlTextLiteralToken:
          case SyntaxKind.XmlTextLiteralNewLineToken:
          case SyntaxKind.EndOfLineTrivia:
          case SyntaxKind.WhitespaceTrivia:
            break;
          default:
            goto label_4;
        }
      }
      return true;
label_4:
      return false;
    }

    private CodeTokenKind GetTokenKind(SyntaxKind syntaxKind)
    {
      if ((uint) syntaxKind <= 8616U)
      {
        if ((uint) syntaxKind <= 8354U)
        {
          if ((uint) syntaxKind <= 8309U)
          {
            if (syntaxKind == SyntaxKind.ByteKeyword)
              return CodeTokenKind.CSByteKeyword;
            if (syntaxKind == SyntaxKind.IntKeyword)
              return CodeTokenKind.CSIntKeyword;
            goto label_50;
          }
          else
          {
            switch (syntaxKind)
            {
              case SyntaxKind.StringKeyword:
                return CodeTokenKind.CSStringKeyword;
              case SyntaxKind.PublicKeyword:
                return CodeTokenKind.CSPublicKeyword;
              case SyntaxKind.PrivateKeyword:
                return CodeTokenKind.CSPrivateKeyword;
              case SyntaxKind.StaticKeyword:
                return CodeTokenKind.CSStaticKeyword;
              case SyntaxKind.NewKeyword:
                return CodeTokenKind.CSNewKeyword;
              default:
                goto label_50;
            }
          }
        }
        else if ((uint) syntaxKind <= 8384U)
        {
          switch (syntaxKind)
          {
            case SyntaxKind.RefKeyword:
              return CodeTokenKind.CSRefKeyword;
            case SyntaxKind.OutKeyword:
              return CodeTokenKind.CSOutKeyword;
            case SyntaxKind.ThisKeyword:
              return CodeTokenKind.CSThisKeyword;
            case SyntaxKind.BaseKeyword:
              return CodeTokenKind.CSBaseKeyword;
            case SyntaxKind.ClassKeyword:
              return CodeTokenKind.CSClassKeyword;
            case SyntaxKind.InterfaceKeyword:
              return CodeTokenKind.CSInterfaceKeyword;
            case SyntaxKind.DelegateKeyword:
              return CodeTokenKind.CSDelegateKeyword;
            case SyntaxKind.CheckedKeyword:
              return CodeTokenKind.CSCheckedKeyword;
            case SyntaxKind.UncheckedKeyword:
              return CodeTokenKind.CSUncheckedKeyword;
            case SyntaxKind.OperatorKeyword:
              return CodeTokenKind.CSOperatorKeyword;
            case SyntaxKind.ExplicitKeyword:
              return CodeTokenKind.CSExplicitKeyword;
            case SyntaxKind.ImplicitKeyword:
              return CodeTokenKind.CSImplicitKeyword;
            default:
              goto label_50;
          }
        }
        else
        {
          switch (syntaxKind)
          {
            case SyntaxKind.IdentifierToken:
            case SyntaxKind.IdentifierName:
              return CodeTokenKind.CSIdentifier;
            case SyntaxKind.InterpolatedStringTextToken:
              break;
            default:
              goto label_50;
          }
        }
      }
      else if ((uint) syntaxKind <= 8714U)
      {
        if ((uint) syntaxKind <= 8638U)
        {
          switch (syntaxKind)
          {
            case SyntaxKind.QualifiedName:
              return CodeTokenKind.CSQualifiedName;
            case SyntaxKind.InvocationExpression:
              return CodeTokenKind.CSInvocation;
            case SyntaxKind.Argument:
              return CodeTokenKind.CSArgument;
            default:
              goto label_50;
          }
        }
        else
        {
          switch (syntaxKind)
          {
            case SyntaxKind.CastExpression:
              return CodeTokenKind.CSCast;
            case SyntaxKind.SimpleMemberAccessExpression:
              return CodeTokenKind.CSMemberAccess;
            case SyntaxKind.SimpleAssignmentExpression:
              return CodeTokenKind.CSAssignment;
            default:
              goto label_50;
          }
        }
      }
      else if ((uint) syntaxKind <= 8855U)
      {
        switch (syntaxKind)
        {
          case SyntaxKind.NumericLiteralExpression:
            return CodeTokenKind.CSNumericLiteral;
          case SyntaxKind.StringLiteralExpression:
            break;
          case SyntaxKind.TrueLiteralExpression:
            return CodeTokenKind.CSTrueKeyword;
          case SyntaxKind.FalseLiteralExpression:
            return CodeTokenKind.CSFalseKeyword;
          case SyntaxKind.ReturnStatement:
            return CodeTokenKind.CSReturnKeyword;
          case SyntaxKind.ClassDeclaration:
            return CodeTokenKind.CSClassDefinition;
          default:
            goto label_50;
        }
      }
      else
      {
        switch (syntaxKind)
        {
          case SyntaxKind.InterfaceDeclaration:
            return CodeTokenKind.CSInterfaceDefinition;
          case SyntaxKind.MethodDeclaration:
            return CodeTokenKind.CSMethodDefinition;
          case SyntaxKind.InterpolatedStringText:
            break;
          default:
            goto label_50;
        }
      }
      return CodeTokenKind.CSStringLiteral;
label_50:
      return CodeTokenKind.CSUnknown;
    }
  }
}
