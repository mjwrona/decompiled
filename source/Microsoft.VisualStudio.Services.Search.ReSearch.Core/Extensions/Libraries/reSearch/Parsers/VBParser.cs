// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Extensions.Libraries.reSearch.Parsers.VBParser
// Assembly: Microsoft.VisualStudio.Services.Search.ReSearch.Core, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 657A74AE-F2A6-4615-BB2F-7FA1F961B173
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.ReSearch.Core.dll

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.VisualBasic;
using Microsoft.CodeAnalysis.VisualBasic.Syntax;
using Microsoft.VisualStudio.Services.Search.Extensions.Libraries.reSearch.Tokenizers.SourceCode;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading;

namespace Microsoft.VisualStudio.Services.Search.Extensions.Libraries.reSearch.Parsers
{
  public class VBParser : RoslynParser
  {
    public VBParser(int maxFileSizeSupportedInBytes)
      : base(maxFileSizeSupportedInBytes)
    {
    }

    protected override List<CodeSymbol> ParseIntoCodeSymbols()
    {
      if (!string.IsNullOrEmpty(this.Content))
        this.ParseTree(VisualBasicSyntaxTree.ParseText(this.Content, new VisualBasicParseOptions(LanguageVersion.VisualBasic14), "", (Encoding) null, new CancellationToken()).GetRoot());
      return this.SymbolsList;
    }

    private void ParseInterfaceStatement(SyntaxNode node, int depth)
    {
      InterfaceStatementSyntax interfaceStatementSyntax = (InterfaceStatementSyntax) node;
      SyntaxList<AttributeListSyntax> attributeLists = interfaceStatementSyntax.AttributeLists;
      this.ParseTree((SyntaxList<SyntaxNode>) interfaceStatementSyntax.AttributeLists, depth + 1);
      this.AddSymbol(interfaceStatementSyntax.Identifier, CodeTokenKind.VBInterfaceStatement, depth + 1);
      this.ParseModifiers(interfaceStatementSyntax.Modifiers, depth + 1);
      if (interfaceStatementSyntax.TypeParameterList == null)
        return;
      this.ParseTree((SyntaxNode) interfaceStatementSyntax.TypeParameterList, depth + 1);
    }

    private void ParseNameSpaceStatement(SyntaxNode node, int depth) => this.AddSymbolForFullyQualifiedName((SyntaxNode) ((NamespaceStatementSyntax) node).Name, CodeTokenKind.VBNamespaceStatement, depth + 1);

    private void ParseInheritsStatement(SyntaxNode node, int depth)
    {
      foreach (CodeSymbol codeSymbol in this.ParseTree(((InheritsStatementSyntax) node).Types, depth))
      {
        if (codeSymbol.SymbolType == CodeTokenKind.VBUnKnown || codeSymbol.SymbolType == CodeTokenKind.VBIdentifier)
          codeSymbol.SymbolType = CodeTokenKind.VBInheritsStatement;
      }
    }

    private void ParseImplementsStatement(SyntaxNode node, int depth)
    {
      List<CodeSymbol> tree = this.ParseTree(((ImplementsStatementSyntax) node).Types, depth);
      tree[tree.Count - 1].SymbolType = CodeTokenKind.VBClassImplements;
      foreach (CodeSymbol codeSymbol in tree)
      {
        if (codeSymbol.SymbolType == CodeTokenKind.VBUnKnown || codeSymbol.SymbolType == CodeTokenKind.VBIdentifier)
          codeSymbol.SymbolType = CodeTokenKind.VBClassImplements;
      }
    }

    private void ParseStructureStatement(SyntaxNode node, int depth)
    {
      StructureStatementSyntax structureStatementSyntax = (StructureStatementSyntax) node;
      this.ParseTree((SyntaxList<SyntaxNode>) structureStatementSyntax.AttributeLists, depth + 1);
      this.AddSymbol(structureStatementSyntax.Identifier, CodeTokenKind.VBStructureStatement, depth + 1);
      this.ParseModifiers(structureStatementSyntax.Modifiers, depth + 1);
      if (structureStatementSyntax.TypeParameterList == null)
        return;
      this.ParseTree((SyntaxNode) structureStatementSyntax.TypeParameterList, depth + 1);
    }

    private void ParseClassStatement(SyntaxNode node, int depth)
    {
      ClassStatementSyntax classStatementSyntax = (ClassStatementSyntax) node;
      this.ParseTree((SyntaxList<SyntaxNode>) classStatementSyntax.AttributeLists, depth + 1);
      this.AddSymbol(classStatementSyntax.Identifier, CodeTokenKind.VBClassStatement, depth + 1);
      this.ParseModifiers(classStatementSyntax.Modifiers, depth + 1);
      if (classStatementSyntax.TypeParameterList == null)
        return;
      this.ParseTree((SyntaxNode) classStatementSyntax.TypeParameterList, depth + 1);
    }

    private void ParseFunctionAggregation(SyntaxNode node, int depth)
    {
      FunctionAggregationSyntax aggregationSyntax = (FunctionAggregationSyntax) node;
      if (aggregationSyntax.Argument != null)
        this.AddSymbol((SyntaxNode) aggregationSyntax.Argument, CodeTokenKind.VBParameter, depth + 1);
      this.ParseTriva(aggregationSyntax.CloseParenToken, depth + 1);
      this.AddSymbol(aggregationSyntax.FunctionName, CodeTokenKind.VBFunctionAggregation, depth + 1);
      this.ParseTriva(aggregationSyntax.OpenParenToken, depth + 1);
    }

    private void ParseParameter(SyntaxNode node, int depth)
    {
      ParameterSyntax parameterSyntax = (ParameterSyntax) node;
      SyntaxList<AttributeListSyntax> attributeLists = parameterSyntax.AttributeLists;
      this.ParseTree((SyntaxList<SyntaxNode>) parameterSyntax.AttributeLists, depth + 1);
      if (parameterSyntax.AsClause != null)
        this.ParseTree((SyntaxNode) parameterSyntax.AsClause, depth + 1);
      this.ParseModifiers(parameterSyntax.Modifiers, depth + 1);
      this.AddSymbol((SyntaxNode) parameterSyntax.Identifier, CodeTokenKind.VBParameter, depth + 1);
    }

    private void ParseVariableDeclarator(SyntaxNode node, int depth)
    {
      VariableDeclaratorSyntax declaratorSyntax = (VariableDeclaratorSyntax) node;
      CodeTokenKind codeTokenKind = CodeTokenKind.VBVariableDeclarator;
      if (declaratorSyntax.AsClause != null)
        this.ParseTree((SyntaxNode) declaratorSyntax.AsClause, depth + 1);
      if (declaratorSyntax.Initializer != null)
      {
        codeTokenKind = CodeTokenKind.VBVariableDeclarationAndAssignment;
        this.ParseTree((SyntaxNode) declaratorSyntax.Initializer, depth + 1);
      }
      foreach (CodeSymbol codeSymbol in this.ParseTree(declaratorSyntax.Names, depth))
      {
        if (codeSymbol.SymbolType == CodeTokenKind.VBUnKnown || codeSymbol.SymbolType == CodeTokenKind.VBIdentifier)
          codeSymbol.SymbolType = codeTokenKind;
      }
    }

    private void ParseAttribute(SyntaxNode node, int depth)
    {
      AttributeSyntax attributeSyntax = (AttributeSyntax) node;
      CodeTokenKind kind = CodeTokenKind.VBAttribute;
      if (attributeSyntax.ArgumentList != null)
        this.ParseTree((SyntaxNode) attributeSyntax.ArgumentList, depth + 1);
      this.AddSymbol((SyntaxNode) attributeSyntax.Name, kind, depth + 1);
    }

    private void ParseReturnStatement(SyntaxNode node, int depth)
    {
      ReturnStatementSyntax returnStatementSyntax = (ReturnStatementSyntax) node;
      if (returnStatementSyntax.Expression == null)
        return;
      this.ParseTree((SyntaxNode) returnStatementSyntax.Expression, depth + 1);
    }

    private void ParseDeclareSubStatement(SyntaxNode node, int depth)
    {
      DeclareStatementSyntax declareStatementSyntax = (DeclareStatementSyntax) node;
      this.ParseTree((SyntaxNode) declareStatementSyntax.AliasName, depth + 1);
      if (declareStatementSyntax.AsClause != null)
        this.ParseTree((SyntaxNode) declareStatementSyntax.AsClause, depth + 1);
      this.ParseTree((SyntaxList<SyntaxNode>) declareStatementSyntax.AttributeLists, depth + 1);
      this.AddSymbol(declareStatementSyntax.Identifier, CodeTokenKind.VBDeclareSubStatement, depth + 1);
      this.ParseModifiers(declareStatementSyntax.Modifiers, depth + 1);
      if (declareStatementSyntax.ParameterList == null)
        return;
      this.ParseTree((SyntaxNode) declareStatementSyntax.ParameterList, depth + 1);
    }

    private void ParseDelegateSubStatement(SyntaxNode node, int depth)
    {
      DelegateStatementSyntax delegateStatementSyntax = (DelegateStatementSyntax) node;
      if (delegateStatementSyntax.AsClause != null)
        this.ParseTree((SyntaxNode) delegateStatementSyntax.AsClause, depth + 1);
      SyntaxList<AttributeListSyntax> attributeLists = delegateStatementSyntax.AttributeLists;
      this.ParseTree((SyntaxList<SyntaxNode>) delegateStatementSyntax.AttributeLists, depth + 1);
      this.AddSymbol(delegateStatementSyntax.Identifier, CodeTokenKind.VBDelegateSubStatement, depth + 1);
      this.ParseModifiers(delegateStatementSyntax.Modifiers, depth + 1);
      if (delegateStatementSyntax.ParameterList == null)
        return;
      this.ParseTree((SyntaxNode) delegateStatementSyntax.ParameterList, depth + 1);
    }

    private void ParseSubNewStatement(SyntaxNode node, int depth)
    {
      SubNewStatementSyntax newStatementSyntax = (SubNewStatementSyntax) node;
      SyntaxList<AttributeListSyntax> attributeLists = newStatementSyntax.AttributeLists;
      this.ParseTree((SyntaxList<SyntaxNode>) newStatementSyntax.AttributeLists, depth + 1);
      this.AddSymbol(newStatementSyntax.NewKeyword, CodeTokenKind.VBSubNewStatement, depth + 1);
    }

    private void ParseMethodStatement(SyntaxNode node, int depth)
    {
      MethodStatementSyntax methodStatementSyntax = (MethodStatementSyntax) node;
      if (methodStatementSyntax.ImplementsClause != null)
        this.ParseTree((SyntaxNode) methodStatementSyntax.ImplementsClause, depth + 1);
      if (methodStatementSyntax.AsClause != null)
        this.ParseTree((SyntaxNode) methodStatementSyntax.AsClause, depth + 1);
      SyntaxList<AttributeListSyntax> attributeLists = methodStatementSyntax.AttributeLists;
      this.ParseTree((SyntaxList<SyntaxNode>) methodStatementSyntax.AttributeLists, depth + 1);
      if (methodStatementSyntax.HandlesClause != null)
        this.ParseTree((SyntaxNode) methodStatementSyntax.HandlesClause, depth + 1);
      this.ParseModifiers(methodStatementSyntax.Modifiers, depth + 1);
      if (methodStatementSyntax.ParameterList != null)
        this.ParseTree((SyntaxNode) methodStatementSyntax.ParameterList, depth + 1);
      if (((SyntaxNode) methodStatementSyntax).Parent.Kind() == SyntaxKind.InterfaceBlock)
        this.AddSymbol(methodStatementSyntax.Identifier, CodeTokenKind.VBInterfaceMethodDeclaration, depth + 1);
      else if (((SyntaxNode) methodStatementSyntax).Parent.Kind() == SyntaxKind.SubBlock)
      {
        this.AddSymbol(methodStatementSyntax.Identifier, CodeTokenKind.VBSubStatement, depth + 1);
      }
      else
      {
        if (((SyntaxNode) methodStatementSyntax).Parent.Kind() != SyntaxKind.FunctionBlock)
          return;
        this.AddSymbol(methodStatementSyntax.Identifier, CodeTokenKind.VBFuctionStatement, depth + 1);
      }
    }

    private void ParseAddRemoveHandlerStatement(SyntaxNode node, int depth)
    {
      AddRemoveHandlerStatementSyntax handlerStatementSyntax = (AddRemoveHandlerStatementSyntax) node;
      this.ParseTriva(handlerStatementSyntax.CommaToken, depth + 1);
      if (handlerStatementSyntax.DelegateExpression != null)
        this.ParseTree((SyntaxNode) handlerStatementSyntax.DelegateExpression, depth + 1);
      if (handlerStatementSyntax.EventExpression == null)
        return;
      this.ParseTree((SyntaxNode) handlerStatementSyntax.EventExpression, depth + 1);
    }

    private void ParseRaiseEventStatement(SyntaxNode node, int depth)
    {
      RaiseEventStatementSyntax eventStatementSyntax = (RaiseEventStatementSyntax) node;
      this.ParseTree((SyntaxNode) eventStatementSyntax.ArgumentList, depth + 1);
      this.AddSymbol((SyntaxNode) eventStatementSyntax.Name, CodeTokenKind.VBRaiseEventStatement, depth + 1);
    }

    private void ParsePropertyStatement(SyntaxNode node, int depth)
    {
      PropertyStatementSyntax propertyStatementSyntax = (PropertyStatementSyntax) node;
      if (propertyStatementSyntax.AsClause != null)
        this.ParseTree((SyntaxNode) propertyStatementSyntax.AsClause, depth + 1);
      this.ParseTree((SyntaxList<SyntaxNode>) propertyStatementSyntax.AttributeLists, depth + 1);
      this.AddSymbol(propertyStatementSyntax.Identifier, CodeTokenKind.VBPropertyStatement, depth + 1);
      if (propertyStatementSyntax.ImplementsClause != null)
        this.ParseTree((SyntaxNode) propertyStatementSyntax.ImplementsClause, depth + 1);
      if (propertyStatementSyntax.Initializer != null)
        this.ParseTree((SyntaxNode) propertyStatementSyntax.Initializer, depth + 1);
      this.ParseModifiers(propertyStatementSyntax.Modifiers, depth + 1);
      if (propertyStatementSyntax.ParameterList == null)
        return;
      this.ParseTree((SyntaxNode) propertyStatementSyntax.ParameterList, depth + 1);
    }

    private void ParseOperatorStatement(SyntaxNode node, int depth)
    {
      OperatorStatementSyntax operatorStatementSyntax = (OperatorStatementSyntax) node;
      if (operatorStatementSyntax.AsClause != null)
        this.ParseTree((SyntaxNode) operatorStatementSyntax.AsClause, depth + 1);
      SyntaxList<AttributeListSyntax> attributeLists = operatorStatementSyntax.AttributeLists;
      this.ParseTree((SyntaxList<SyntaxNode>) operatorStatementSyntax.AttributeLists, depth + 1);
      this.AddSymbol(operatorStatementSyntax.OperatorToken, CodeTokenKind.VBOperatorStatement, depth + 1);
      this.ParseModifiers(operatorStatementSyntax.Modifiers, depth + 1);
      if (operatorStatementSyntax.ParameterList == null)
        return;
      this.ParseTree((SyntaxNode) operatorStatementSyntax.ParameterList, depth + 1);
    }

    private void ParseAccessorStatement(SyntaxNode node, int depth)
    {
      AccessorStatementSyntax accessorStatementSyntax = (AccessorStatementSyntax) node;
      SyntaxList<AttributeListSyntax> attributeLists = accessorStatementSyntax.AttributeLists;
      this.ParseTree((SyntaxList<SyntaxNode>) accessorStatementSyntax.AttributeLists, depth + 1);
      this.AddSymbol(accessorStatementSyntax.AccessorKeyword, this.GetTokenKind(node.Kind()), depth + 1);
      this.ParseModifiers(accessorStatementSyntax.Modifiers, depth + 1);
      if (accessorStatementSyntax.ParameterList == null)
        return;
      this.ParseTree((SyntaxNode) accessorStatementSyntax.ParameterList, depth + 1);
    }

    private void ParseImplementsClause(SyntaxNode node, int depth)
    {
      ImplementsClauseSyntax implementsClauseSyntax = (ImplementsClauseSyntax) node;
      SeparatedSyntaxList<QualifiedNameSyntax> interfaceMembers = implementsClauseSyntax.InterfaceMembers;
      List<CodeSymbol> tree = this.ParseTree(implementsClauseSyntax.InterfaceMembers, depth);
      tree[tree.Count - 1].SymbolType = CodeTokenKind.VBImplements;
      foreach (CodeSymbol codeSymbol in tree)
      {
        if (codeSymbol.SymbolType == CodeTokenKind.VBUnKnown || codeSymbol.SymbolType == CodeTokenKind.VBIdentifier)
          codeSymbol.SymbolType = CodeTokenKind.VBImplements;
      }
    }

    private void ParseHandlesClauseItem(SyntaxNode node, int depth)
    {
      HandlesClauseItemSyntax clauseItemSyntax = (HandlesClauseItemSyntax) node;
      if (clauseItemSyntax == null || clauseItemSyntax.EventMember == null)
        return;
      this.AddSymbol(clauseItemSyntax.EventMember.Identifier, CodeTokenKind.VBHandlesItem, depth + 1);
    }

    private void ParseSimpleImportsClause(SyntaxNode node, int depth)
    {
      SimpleImportsClauseSyntax importsClauseSyntax = (SimpleImportsClauseSyntax) node;
      CodeTokenKind kind = CodeTokenKind.VBImport;
      this.AddSymbol((SyntaxNode) importsClauseSyntax.Name, kind, depth + 1);
      if (importsClauseSyntax.Alias == null)
        return;
      this.ParseTree((SyntaxNode) importsClauseSyntax.Alias, depth + 1);
    }

    private void ParseImportAliasClause(SyntaxNode node, int depth)
    {
      ImportAliasClauseSyntax aliasClauseSyntax = (ImportAliasClauseSyntax) node;
      this.AddSymbol(aliasClauseSyntax.Identifier, CodeTokenKind.VBImportAliasClause, depth + 1);
      this.ParseTriva(aliasClauseSyntax.EqualsToken, depth + 1);
    }

    private void ParsePredefinedType(SyntaxNode node, int depth) => this.AddSymbol(node, CodeTokenKind.VBPredefinedType, depth);

    private void ParseEnumStatement(SyntaxNode node, int depth)
    {
      EnumStatementSyntax enumStatementSyntax = (EnumStatementSyntax) node;
      SyntaxList<AttributeListSyntax> attributeLists = enumStatementSyntax.AttributeLists;
      this.ParseTree((SyntaxList<SyntaxNode>) enumStatementSyntax.AttributeLists, depth + 1);
      this.AddSymbol(enumStatementSyntax.Identifier, CodeTokenKind.VBEnumStatement, depth + 1);
      this.ParseModifiers(enumStatementSyntax.Modifiers, depth + 1);
      if (enumStatementSyntax.UnderlyingType == null)
        return;
      this.ParseTree((SyntaxNode) enumStatementSyntax.UnderlyingType, depth + 1);
    }

    private void ParseEnumMemberDeclaration(SyntaxNode node, int depth)
    {
      EnumMemberDeclarationSyntax declarationSyntax = (EnumMemberDeclarationSyntax) node;
      SyntaxList<AttributeListSyntax> attributeLists = declarationSyntax.AttributeLists;
      this.ParseTree((SyntaxList<SyntaxNode>) declarationSyntax.AttributeLists, depth + 1);
      this.AddSymbol(declarationSyntax.Identifier, CodeTokenKind.VBEnumMemberDeclaration, depth + 1);
    }

    private void ParseEventStatement(SyntaxNode node, int depth)
    {
      EventStatementSyntax eventStatementSyntax = (EventStatementSyntax) node;
      if (eventStatementSyntax.AsClause != null)
        this.ParseTree((SyntaxNode) eventStatementSyntax.AsClause, depth + 1);
      SyntaxList<AttributeListSyntax> attributeLists = eventStatementSyntax.AttributeLists;
      this.ParseTree((SyntaxList<SyntaxNode>) eventStatementSyntax.AttributeLists, depth + 1);
      this.AddSymbol(eventStatementSyntax.Identifier, CodeTokenKind.VBEventStatement, depth + 1);
      this.ParseModifiers(eventStatementSyntax.Modifiers, depth + 1);
      if (eventStatementSyntax.ParameterList != null)
        this.ParseTree((SyntaxNode) eventStatementSyntax.ParameterList, depth + 1);
      if (eventStatementSyntax.ImplementsClause == null)
        return;
      this.ParseTree((SyntaxNode) eventStatementSyntax.ImplementsClause, depth + 1);
    }

    private void ParseTypeParameterMultipleConstraintClause(SyntaxNode node, int depth) => this.ParseTree(((TypeParameterMultipleConstraintClauseSyntax) node).Constraints, depth + 1);

    private void ParseTypeParameterSingleConstraintClause(SyntaxNode node, int depth) => this.ParseTree((SyntaxNode) ((TypeParameterSingleConstraintClauseSyntax) node).Constraint, depth + 1);

    private void ParseTypeConstraint(SyntaxNode node, int depth) => this.AddSymbol((SyntaxNode) ((TypeConstraintSyntax) node).Type, CodeTokenKind.VBTypeConstraint, depth + 1);

    private void ParseModuleStatement(SyntaxNode node, int depth)
    {
      ModuleStatementSyntax moduleStatementSyntax = (ModuleStatementSyntax) node;
      this.ParseTree((SyntaxList<SyntaxNode>) moduleStatementSyntax.AttributeLists, depth + 1);
      this.AddSymbol(moduleStatementSyntax.Identifier, CodeTokenKind.VBModuleStatement, depth + 1);
      this.ParseModifiers(moduleStatementSyntax.Modifiers, depth + 1);
      if (moduleStatementSyntax.TypeParameterList == null)
        return;
      this.ParseTree((SyntaxNode) moduleStatementSyntax.TypeParameterList, depth + 1);
    }

    private void ParseInvocationExpression(SyntaxNode node, int depth)
    {
      InvocationExpressionSyntax expressionSyntax = (InvocationExpressionSyntax) node;
      if (expressionSyntax.ArgumentList != null)
        this.ParseTree((SyntaxNode) expressionSyntax.ArgumentList, depth + 1);
      if (expressionSyntax.Expression != null)
        this.ParseTree((SyntaxNode) expressionSyntax.Expression, depth + 1);
      this.SymbolsList[this.SymbolsList.Count - 1].SymbolType = CodeTokenKind.VBInvocationExpression;
    }

    private void ParseIdentifierName(SyntaxNode node, int depth) => this.AddSymbol(((IdentifierNameSyntax) node).Identifier, this.GetTokenKind(node.Kind()), depth + 1);

    private void ParseModifiedIdentifierName(SyntaxNode node, int depth) => this.AddSymbol(((ModifiedIdentifierSyntax) node).Identifier, this.GetTokenKind(node.Kind()), depth + 1);

    private void ParseNameColonEquals(SyntaxNode node, int depth)
    {
      NameColonEqualsSyntax colonEqualsSyntax = (NameColonEqualsSyntax) node;
      this.AddSymbol((SyntaxNode) colonEqualsSyntax.Name, CodeTokenKind.VBNameColonEquals, depth + 1);
      this.ParseTriva(colonEqualsSyntax.ColonEqualsToken, depth + 1);
    }

    private void ParseFieldDeclaration(SyntaxNode node, int depth)
    {
      FieldDeclarationSyntax declarationSyntax = (FieldDeclarationSyntax) node;
      this.ParseTree((SyntaxList<SyntaxNode>) declarationSyntax.AttributeLists, depth + 1);
      foreach (CodeSymbol codeSymbol in this.ParseTree(declarationSyntax.Declarators, depth))
      {
        switch (codeSymbol.SymbolType)
        {
          case CodeTokenKind.VBVariableDeclarator:
            codeSymbol.SymbolType = CodeTokenKind.VBFieldDeclaration;
            continue;
          case CodeTokenKind.VBVariableDeclarationAndAssignment:
            codeSymbol.SymbolType = CodeTokenKind.VBFieldDeclarationAndAssignment;
            continue;
          default:
            continue;
        }
      }
      this.ParseModifiers(declarationSyntax.Modifiers, depth + 1);
    }

    private void ParseQualifiedName(SyntaxNode node, int depth)
    {
      QualifiedNameSyntax qualifiedNameSyntax = (QualifiedNameSyntax) node;
      this.ParseTriva(qualifiedNameSyntax.DotToken, depth + 1);
      this.ParseTree((SyntaxNode) qualifiedNameSyntax.Left, depth + 1);
      this.ParseTree((SyntaxNode) qualifiedNameSyntax.Right, depth + 1);
    }

    private void ParseGenericName(SyntaxNode node, int depth)
    {
      GenericNameSyntax genericNameSyntax = (GenericNameSyntax) node;
      this.AddSymbol(genericNameSyntax.Identifier, CodeTokenKind.VBGenericName, depth + 1);
      if (genericNameSyntax.TypeArgumentList == null)
        return;
      this.ParseTree((SyntaxNode) genericNameSyntax.TypeArgumentList, depth + 1);
    }

    private void ParseAnonymousObjectCreationExpression(SyntaxNode node, int depth)
    {
      AnonymousObjectCreationExpressionSyntax expressionSyntax = (AnonymousObjectCreationExpressionSyntax) node;
      this.ParseTree((SyntaxList<SyntaxNode>) expressionSyntax.AttributeLists, depth + 1);
      if (expressionSyntax.Initializer == null)
        return;
      this.ParseTree((SyntaxNode) expressionSyntax.Initializer, depth + 1);
    }

    private void ParseCollectionInitializer(SyntaxNode node, int depth)
    {
      CollectionInitializerSyntax initializerSyntax = (CollectionInitializerSyntax) node;
      this.ParseTriva(initializerSyntax.CloseBraceToken, depth + 1);
      this.ParseTriva(initializerSyntax.OpenBraceToken, depth + 1);
      SeparatedSyntaxList<ExpressionSyntax> initializers = initializerSyntax.Initializers;
      this.ParseTree(initializerSyntax.Initializers, depth + 1);
    }

    private void ParseObjectCollectionInitializer(SyntaxNode node, int depth)
    {
      ObjectCollectionInitializerSyntax initializerSyntax = (ObjectCollectionInitializerSyntax) node;
      if (initializerSyntax.Initializer == null)
        return;
      this.ParseTree((SyntaxNode) initializerSyntax.Initializer, depth + 1);
    }

    private void ParseObjectMemberInitializer(SyntaxNode node, int depth)
    {
      ObjectMemberInitializerSyntax initializerSyntax = (ObjectMemberInitializerSyntax) node;
      this.ParseTriva(initializerSyntax.CloseBraceToken, depth + 1);
      this.ParseTriva(initializerSyntax.OpenBraceToken, depth + 1);
      SeparatedSyntaxList<FieldInitializerSyntax> initializers = initializerSyntax.Initializers;
      this.ParseTree(initializerSyntax.Initializers, depth + 1);
    }

    private void ParseArrayCreationExpression(SyntaxNode node, int depth)
    {
      ArrayCreationExpressionSyntax expressionSyntax = (ArrayCreationExpressionSyntax) node;
      if (expressionSyntax.Initializer != null)
        this.ParseTree((SyntaxNode) expressionSyntax.Initializer, depth + 1);
      this.ParseTree((SyntaxNode) expressionSyntax.Type, depth);
    }

    private void ParseArrayType(SyntaxNode node, int depth)
    {
      ArrayTypeSyntax arrayTypeSyntax = (ArrayTypeSyntax) node;
      this.ParseTree((SyntaxList<SyntaxNode>) arrayTypeSyntax.RankSpecifiers, depth + 1);
      this.ParseTree((SyntaxNode) arrayTypeSyntax.ElementType, depth);
      this.SymbolsList[this.SymbolsList.Count - 1].SymbolType = CodeTokenKind.VBArrayType;
    }

    private void ParseArrayRankSpecifier(SyntaxNode node, int depth)
    {
      ArrayRankSpecifierSyntax rankSpecifierSyntax = (ArrayRankSpecifierSyntax) node;
      this.ParseTriva(rankSpecifierSyntax.OpenParenToken, depth + 1);
      this.ParseTriva(rankSpecifierSyntax.CloseParenToken, depth + 1);
      this.ParseModifiers(rankSpecifierSyntax.CommaTokens, depth + 1);
    }

    private void ParseDirectCastExpression(SyntaxNode node, int depth)
    {
      DirectCastExpressionSyntax expressionSyntax = (DirectCastExpressionSyntax) node;
      this.ParseTriva(expressionSyntax.OpenParenToken, depth + 1);
      this.ParseTriva(expressionSyntax.CloseParenToken, depth + 1);
      this.ParseTree((SyntaxNode) expressionSyntax.Expression, depth + 1);
      List<CodeSymbol> tree = this.ParseTree((SyntaxNode) expressionSyntax.Type, depth);
      tree[tree.Count - 1].SymbolType = CodeTokenKind.VBDirectCastExpression;
    }

    private void ParseCatchExpression(SyntaxNode node, int depth)
    {
      CatchStatementSyntax catchStatementSyntax = (CatchStatementSyntax) node;
      if (catchStatementSyntax.AsClause != null)
        this.ParseTree((SyntaxNode) catchStatementSyntax.AsClause, depth + 1);
      if (catchStatementSyntax.IdentifierName != null)
        this.AddSymbol((SyntaxNode) catchStatementSyntax.IdentifierName, CodeTokenKind.VBCatchStatement, depth + 1);
      if (catchStatementSyntax.WhenClause == null)
        return;
      this.ParseTree((SyntaxNode) catchStatementSyntax.WhenClause, depth + 1);
    }

    private void ParseObjectCreationExpression(SyntaxNode node, int depth)
    {
      ObjectCreationExpressionSyntax expressionSyntax = (ObjectCreationExpressionSyntax) node;
      SyntaxList<AttributeListSyntax> attributeLists = expressionSyntax.AttributeLists;
      this.ParseTree((SyntaxList<SyntaxNode>) expressionSyntax.AttributeLists, depth + 1);
      if (expressionSyntax.ArgumentList != null)
        this.ParseTree((SyntaxNode) expressionSyntax.ArgumentList, depth + 1);
      if (expressionSyntax.Initializer != null)
        this.ParseTree((SyntaxNode) expressionSyntax.Initializer, depth + 1);
      this.ParseTree((SyntaxNode) expressionSyntax.Type, depth + 1);
    }

    private void ParseAttributeStatement(SyntaxNode node, int depth)
    {
      AttributesStatementSyntax attributesStatementSyntax = (AttributesStatementSyntax) node;
      SyntaxList<AttributeListSyntax> attributeLists = attributesStatementSyntax.AttributeLists;
      this.ParseTree((SyntaxList<SyntaxNode>) attributesStatementSyntax.AttributeLists, depth + 1);
    }

    private void ParseMemberAccessExpressionStatement(SyntaxNode node, int depth)
    {
      MemberAccessExpressionSyntax expressionSyntax = (MemberAccessExpressionSyntax) node;
      if (expressionSyntax.Expression != null)
        this.ParseTree((SyntaxNode) expressionSyntax.Expression, depth + 1);
      this.ParseTree((SyntaxNode) expressionSyntax.Name, depth);
    }

    private void ParseSimpleAssignment(SyntaxNode node, int depth)
    {
      AssignmentStatementSyntax assignmentStatementSyntax = (AssignmentStatementSyntax) node;
      List<CodeSymbol> tree = this.ParseTree((SyntaxNode) assignmentStatementSyntax.Left, depth + 1);
      tree[tree.Count - 1].SymbolType = CodeTokenKind.VBAssignment;
      this.ParseTriva(assignmentStatementSyntax.OperatorToken, depth + 1);
      this.ParseTree((SyntaxNode) assignmentStatementSyntax.Right, depth);
    }

    private void ParsePrimaryExpression(SyntaxNode node, int depth)
    {
      LiteralExpressionSyntax expressionSyntax = (LiteralExpressionSyntax) node;
      this.AddLiteralSymbol(expressionSyntax.Token, this.GetTokenKind(expressionSyntax.Kind()), depth);
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
            case SyntaxKind.DocumentationCommentTrivia:
            case SyntaxKind.CommentTrivia:
              this.ParseCommentTriva(syntaxTrivia, CodeTokenKind.VBComment);
              continue;
            default:
              using (List<CodeSymbol>.Enumerator enumerator = this.Tokenize(syntaxTrivia, CodeTokenKind.VBUnKnown).GetEnumerator())
              {
                while (enumerator.MoveNext())
                  this.AddSymbol(this.SymbolsList, enumerator.Current);
                continue;
              }
          }
        }
      }
    }

    private CodeTokenKind GetTokenKind(SyntaxKind syntaxKind)
    {
      if ((uint) syntaxKind <= 264U)
      {
        if ((uint) syntaxKind <= 111U)
        {
          if ((uint) syntaxKind <= 63U)
          {
            switch (syntaxKind)
            {
              case SyntaxKind.NamespaceStatement:
                return CodeTokenKind.VBNamespaceStatement;
              case SyntaxKind.ModuleStatement:
                return CodeTokenKind.VBModuleStatement;
              case SyntaxKind.StructureStatement:
                return CodeTokenKind.VBStructureStatement;
              case SyntaxKind.InterfaceStatement:
                return CodeTokenKind.VBInterfaceStatement;
              case SyntaxKind.ClassStatement:
                return CodeTokenKind.VBClassStatement;
              case SyntaxKind.EnumStatement:
                return CodeTokenKind.VBEnumStatement;
              default:
                goto label_53;
            }
          }
          else
          {
            switch (syntaxKind)
            {
              case SyntaxKind.TypeConstraint:
                return CodeTokenKind.VBTypeConstraint;
              case SyntaxKind.EnumMemberDeclaration:
                return CodeTokenKind.VBEnumMemberDeclaration;
              case SyntaxKind.SubStatement:
                return CodeTokenKind.VBSubStatement;
              case SyntaxKind.DeclareSubStatement:
                return CodeTokenKind.VBDeclareSubStatement;
              case SyntaxKind.DeclareFunctionStatement:
              case SyntaxKind.DelegateSubStatement:
                return CodeTokenKind.VBDelegateSubStatement;
              case SyntaxKind.EventStatement:
                return CodeTokenKind.VBEventStatement;
              case SyntaxKind.OperatorStatement:
                return CodeTokenKind.VBOperatorStatement;
              case SyntaxKind.PropertyStatement:
                return CodeTokenKind.VBPropertyStatement;
              case SyntaxKind.GetAccessorStatement:
                return CodeTokenKind.VBGetAccessor;
              case SyntaxKind.SetAccessorStatement:
                return CodeTokenKind.VBSetAccessor;
              case SyntaxKind.RaiseEventAccessorStatement:
                return CodeTokenKind.VBRaiseEventAccessorStatement;
              default:
                goto label_53;
            }
          }
        }
        else if ((uint) syntaxKind <= 169U)
        {
          switch (syntaxKind)
          {
            case SyntaxKind.Parameter:
              return CodeTokenKind.VBParameter;
            case SyntaxKind.ModifiedIdentifier:
              break;
            case SyntaxKind.ArrayRankSpecifier:
              return CodeTokenKind.VBArrayRankSpecifier;
            case SyntaxKind.Attribute:
              return CodeTokenKind.VBAttribute;
            case SyntaxKind.ExpressionStatement:
              return CodeTokenKind.VBExpressionStatement;
            case SyntaxKind.ReturnStatement:
              return CodeTokenKind.VBReturnStatement;
            default:
              goto label_53;
          }
        }
        else
        {
          switch (syntaxKind)
          {
            case SyntaxKind.CatchStatement:
              return CodeTokenKind.VBCatchStatement;
            case SyntaxKind.SimpleAssignmentStatement:
              return CodeTokenKind.VBAssignment;
            case SyntaxKind.RaiseEventStatement:
              return CodeTokenKind.VBRaiseEventStatement;
            default:
              goto label_53;
          }
        }
      }
      else if ((uint) syntaxKind <= 357U)
      {
        if ((uint) syntaxKind <= 291U)
        {
          if (syntaxKind == SyntaxKind.StringLiteralExpression)
            return CodeTokenKind.VBStringLiteral;
          if (syntaxKind == SyntaxKind.SimpleMemberAccessExpression)
            return CodeTokenKind.VBSimpleMemberAccessExpression;
          goto label_53;
        }
        else
        {
          switch (syntaxKind)
          {
            case SyntaxKind.InvocationExpression:
              return CodeTokenKind.VBInvocationExpression;
            case SyntaxKind.DirectCastExpression:
              return CodeTokenKind.VBDirectCastExpression;
            case SyntaxKind.FunctionAggregation:
              return CodeTokenKind.VBFunctionAggregation;
            default:
              goto label_53;
          }
        }
      }
      else
      {
        if ((uint) syntaxKind <= 710U)
        {
          switch (syntaxKind)
          {
            case SyntaxKind.ArrayType:
              return CodeTokenKind.VBArrayType;
            case SyntaxKind.PredefinedType:
              return CodeTokenKind.VBPredefinedType;
            case SyntaxKind.IdentifierName:
            case SyntaxKind.IdentifierToken:
              goto label_31;
            case SyntaxKind.GenericName:
              return CodeTokenKind.VBGenericName;
            case SyntaxKind.DocumentationCommentTrivia:
              break;
            default:
              goto label_53;
          }
        }
        else
        {
          switch (syntaxKind)
          {
            case SyntaxKind.CommentTrivia:
              break;
            case SyntaxKind.ImportAliasClause:
              return CodeTokenKind.VBImportAliasClause;
            case SyntaxKind.NameColonEquals:
              return CodeTokenKind.VBNameColonEquals;
            default:
              goto label_53;
          }
        }
        return CodeTokenKind.VBComment;
      }
label_31:
      return CodeTokenKind.VBIdentifier;
label_53:
      return CodeTokenKind.CSUnknown;
    }

    private void ParseModifiers(SyntaxTokenList syntaxTokenList, int depth)
    {
      foreach (SyntaxToken syntaxToken in syntaxTokenList)
      {
        if (!this.IgnoredKind(syntaxToken.Kind()))
          this.AddSymbol(syntaxToken, this.GetTokenKind(syntaxToken.Kind()), depth);
      }
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

    private List<CodeSymbol> ParseTree(SyntaxList<SyntaxNode> syntaxList, int depth)
    {
      List<CodeSymbol> tree = new List<CodeSymbol>();
      foreach (SyntaxNode syntax in syntaxList)
        tree.AddRange((IEnumerable<CodeSymbol>) this.ParseTree(syntax, depth));
      return tree;
    }

    [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
    private List<CodeSymbol> ParseTree(
      SeparatedSyntaxList<ImportsClauseSyntax> syntaxList,
      int depth)
    {
      List<CodeSymbol> tree = new List<CodeSymbol>();
      foreach (ImportsClauseSyntax syntax in syntaxList)
        tree.AddRange((IEnumerable<CodeSymbol>) this.ParseTree((SyntaxNode) syntax, depth));
      return tree;
    }

    private List<CodeSymbol> ParseTree(SeparatedSyntaxList<TypeSyntax> syntaxList, int depth)
    {
      List<CodeSymbol> tree = new List<CodeSymbol>();
      foreach (TypeSyntax syntax in syntaxList)
        tree.AddRange((IEnumerable<CodeSymbol>) this.ParseTree((SyntaxNode) syntax, depth));
      return tree;
    }

    private List<CodeSymbol> ParseTree(
      SeparatedSyntaxList<ModifiedIdentifierSyntax> syntaxList,
      int depth)
    {
      List<CodeSymbol> tree = new List<CodeSymbol>();
      foreach (ModifiedIdentifierSyntax syntax in syntaxList)
        tree.AddRange((IEnumerable<CodeSymbol>) this.ParseTree((SyntaxNode) syntax, depth));
      return tree;
    }

    private List<CodeSymbol> ParseTree(
      SeparatedSyntaxList<QualifiedNameSyntax> syntaxList,
      int depth)
    {
      List<CodeSymbol> tree = new List<CodeSymbol>();
      foreach (QualifiedNameSyntax syntax in syntaxList)
        tree.AddRange((IEnumerable<CodeSymbol>) this.ParseTree((SyntaxNode) syntax, depth));
      return tree;
    }

    private List<CodeSymbol> ParseTree(
      SeparatedSyntaxList<HandlesClauseItemSyntax> syntaxList,
      int depth)
    {
      List<CodeSymbol> tree = new List<CodeSymbol>();
      foreach (HandlesClauseItemSyntax syntax in syntaxList)
        tree.AddRange((IEnumerable<CodeSymbol>) this.ParseTree((SyntaxNode) syntax, depth));
      return tree;
    }

    private List<CodeSymbol> ParseTree(SeparatedSyntaxList<ConstraintSyntax> syntaxList, int depth)
    {
      List<CodeSymbol> tree = new List<CodeSymbol>();
      foreach (ConstraintSyntax syntax in syntaxList)
        tree.AddRange((IEnumerable<CodeSymbol>) this.ParseTree((SyntaxNode) syntax, depth));
      return tree;
    }

    private List<CodeSymbol> ParseTree(
      SeparatedSyntaxList<VariableDeclaratorSyntax> syntaxList,
      int depth)
    {
      List<CodeSymbol> tree = new List<CodeSymbol>();
      foreach (VariableDeclaratorSyntax syntax in syntaxList)
        tree.AddRange((IEnumerable<CodeSymbol>) this.ParseTree((SyntaxNode) syntax, depth));
      return tree;
    }

    private List<CodeSymbol> ParseTree(SeparatedSyntaxList<ExpressionSyntax> syntaxList, int depth)
    {
      List<CodeSymbol> tree = new List<CodeSymbol>();
      foreach (ExpressionSyntax syntax in syntaxList)
        tree.AddRange((IEnumerable<CodeSymbol>) this.ParseTree((SyntaxNode) syntax, depth));
      return tree;
    }

    private List<CodeSymbol> ParseTree(
      SeparatedSyntaxList<FieldInitializerSyntax> syntaxList,
      int depth)
    {
      List<CodeSymbol> tree = new List<CodeSymbol>();
      foreach (FieldInitializerSyntax syntax in syntaxList)
        tree.AddRange((IEnumerable<CodeSymbol>) this.ParseTree((SyntaxNode) syntax, depth));
      return tree;
    }

    private List<CodeSymbol> ParseTree(SyntaxNode node, int depth = 0)
    {
      SyntaxKind syntaxKind = node.Kind();
      if ((uint) syntaxKind <= 275U)
      {
        if ((uint) syntaxKind <= 189U)
        {
          if ((uint) syntaxKind <= 167U)
          {
            switch (syntaxKind)
            {
              case SyntaxKind.SimpleImportsClause:
                return this.ParseAndScope(new RoslynParser.Parser(this.ParseSimpleImportsClause), node, depth);
              case SyntaxKind.NamespaceStatement:
                return this.ParseAndScope(new RoslynParser.Parser(this.ParseNameSpaceStatement), node, depth);
              case SyntaxKind.InheritsStatement:
                return this.ParseAndScope(new RoslynParser.Parser(this.ParseInheritsStatement), node, depth);
              case SyntaxKind.ImplementsStatement:
                return this.ParseAndScope(new RoslynParser.Parser(this.ParseImplementsStatement), node, depth);
              case SyntaxKind.ModuleStatement:
                return this.ParseAndScope(new RoslynParser.Parser(this.ParseModuleStatement), node, depth);
              case SyntaxKind.StructureStatement:
                return this.ParseAndScope(new RoslynParser.Parser(this.ParseStructureStatement), node, depth);
              case SyntaxKind.InterfaceStatement:
                return this.ParseAndScope(new RoslynParser.Parser(this.ParseInterfaceStatement), node, depth);
              case SyntaxKind.ClassStatement:
                return this.ParseAndScope(new RoslynParser.Parser(this.ParseClassStatement), node, depth);
              case SyntaxKind.EnumStatement:
                return this.ParseAndScope(new RoslynParser.Parser(this.ParseEnumStatement), node, depth);
              case SyntaxKind.TypeParameterSingleConstraintClause:
                return this.ParseAndScope(new RoslynParser.Parser(this.ParseTypeParameterSingleConstraintClause), node, depth);
              case SyntaxKind.TypeParameterMultipleConstraintClause:
                return this.ParseAndScope(new RoslynParser.Parser(this.ParseTypeParameterMultipleConstraintClause), node, depth);
              case SyntaxKind.TypeConstraint:
                return this.ParseAndScope(new RoslynParser.Parser(this.ParseTypeConstraint), node, depth);
              case SyntaxKind.EnumMemberDeclaration:
                return this.ParseAndScope(new RoslynParser.Parser(this.ParseEnumMemberDeclaration), node, depth);
              case SyntaxKind.SubStatement:
              case SyntaxKind.FunctionStatement:
                return this.ParseAndScope(new RoslynParser.Parser(this.ParseMethodStatement), node, depth);
              case SyntaxKind.SubNewStatement:
                return this.ParseAndScope(new RoslynParser.Parser(this.ParseSubNewStatement), node, depth);
              case SyntaxKind.DeclareSubStatement:
                return this.ParseAndScope(new RoslynParser.Parser(this.ParseDeclareSubStatement), node, depth);
              case SyntaxKind.DelegateSubStatement:
                return this.ParseAndScope(new RoslynParser.Parser(this.ParseDelegateSubStatement), node, depth);
              case SyntaxKind.EventStatement:
                return this.ParseAndScope(new RoslynParser.Parser(this.ParseEventStatement), node, depth);
              case SyntaxKind.OperatorStatement:
                return this.ParseAndScope(new RoslynParser.Parser(this.ParseOperatorStatement), node, depth);
              case SyntaxKind.PropertyStatement:
                return this.ParseAndScope(new RoslynParser.Parser(this.ParsePropertyStatement), node, depth);
              case SyntaxKind.GetAccessorStatement:
              case SyntaxKind.SetAccessorStatement:
                return this.ParseAndScope(new RoslynParser.Parser(this.ParseAccessorStatement), node, depth);
              case SyntaxKind.AddHandlerAccessorStatement:
              case SyntaxKind.RemoveHandlerAccessorStatement:
              case SyntaxKind.RaiseEventAccessorStatement:
                return this.ParseAndScope(new RoslynParser.Parser(this.ParseAccessorStatement), node, depth);
              case SyntaxKind.ImplementsClause:
                return this.ParseAndScope(new RoslynParser.Parser(this.ParseImplementsClause), node, depth);
              case SyntaxKind.HandlesClause:
                return this.ParseTree(((HandlesClauseSyntax) node).Events, depth);
              case SyntaxKind.HandlesClauseItem:
                return this.ParseAndScope(new RoslynParser.Parser(this.ParseHandlesClauseItem), node, depth);
              case SyntaxKind.FieldDeclaration:
                return this.ParseAndScope(new RoslynParser.Parser(this.ParseFieldDeclaration), node, depth);
              case SyntaxKind.VariableDeclarator:
                return this.ParseAndScope(new RoslynParser.Parser(this.ParseVariableDeclarator), node, depth);
              case SyntaxKind.ObjectMemberInitializer:
                return this.ParseAndScope(new RoslynParser.Parser(this.ParseObjectMemberInitializer), node, depth);
              case SyntaxKind.ObjectCollectionInitializer:
                return this.ParseAndScope(new RoslynParser.Parser(this.ParseObjectCollectionInitializer), node, depth);
              case SyntaxKind.EqualsValue:
                return this.ParseTree((SyntaxNode) ((EqualsValueSyntax) node).Value, depth + 1);
              case SyntaxKind.Parameter:
                return this.ParseAndScope(new RoslynParser.Parser(this.ParseParameter), node, depth);
              case SyntaxKind.ModifiedIdentifier:
                return this.ParseAndScope(new RoslynParser.Parser(this.ParseModifiedIdentifierName), node, depth);
              case SyntaxKind.ArrayRankSpecifier:
                return this.ParseAndScope(new RoslynParser.Parser(this.ParseArrayRankSpecifier), node, depth);
              case SyntaxKind.Attribute:
                return this.ParseAndScope(new RoslynParser.Parser(this.ParseAttribute), node, depth);
              case SyntaxKind.AttributesStatement:
                return this.ParseAndScope(new RoslynParser.Parser(this.ParseAttributeStatement), node, depth);
              case SyntaxKind.ExpressionStatement:
                return this.ParseTree((SyntaxNode) ((ExpressionStatementSyntax) node).Expression, depth + 1);
              default:
                goto label_73;
            }
          }
          else
          {
            if (syntaxKind == SyntaxKind.ReturnStatement)
              return this.ParseAndScope(new RoslynParser.Parser(this.ParseReturnStatement), (SyntaxNode) (node as ReturnStatementSyntax), depth);
            if (syntaxKind == SyntaxKind.IfStatement || syntaxKind == SyntaxKind.TryStatement)
              goto label_73;
            else
              goto label_73;
          }
        }
        else if ((uint) syntaxKind <= 247U)
        {
          switch (syntaxKind)
          {
            case SyntaxKind.CatchStatement:
              return this.ParseAndScope(new RoslynParser.Parser(this.ParseCatchExpression), node, depth);
            case SyntaxKind.SimpleAssignmentStatement:
              return this.ParseAndScope(new RoslynParser.Parser(this.ParseSimpleAssignment), node, depth);
            default:
              goto label_73;
          }
        }
        else
        {
          switch (syntaxKind)
          {
            case SyntaxKind.AddHandlerStatement:
            case SyntaxKind.RemoveHandlerStatement:
              return this.ParseAndScope(new RoslynParser.Parser(this.ParseAddRemoveHandlerStatement), node, depth);
            case SyntaxKind.RaiseEventStatement:
              return this.ParseAndScope(new RoslynParser.Parser(this.ParseRaiseEventStatement), node, depth);
            case SyntaxKind.TrueLiteralExpression:
            case SyntaxKind.FalseLiteralExpression:
            case SyntaxKind.NumericLiteralExpression:
              break;
            default:
              goto label_73;
          }
        }
      }
      else if ((uint) syntaxKind <= 351U)
      {
        if ((uint) syntaxKind <= 307U)
        {
          switch (syntaxKind)
          {
            case SyntaxKind.StringLiteralExpression:
              break;
            case SyntaxKind.SimpleMemberAccessExpression:
              return this.ParseAndScope(new RoslynParser.Parser(this.ParseMemberAccessExpressionStatement), node, depth);
            case SyntaxKind.InvocationExpression:
              return this.ParseAndScope(new RoslynParser.Parser(this.ParseInvocationExpression), node, depth);
            case SyntaxKind.ObjectCreationExpression:
              return this.ParseAndScope(new RoslynParser.Parser(this.ParseObjectCreationExpression), node, depth);
            case SyntaxKind.AnonymousObjectCreationExpression:
              return this.ParseAndScope(new RoslynParser.Parser(this.ParseAnonymousObjectCreationExpression), node, depth);
            case SyntaxKind.ArrayCreationExpression:
              return this.ParseAndScope(new RoslynParser.Parser(this.ParseArrayCreationExpression), node, depth);
            case SyntaxKind.CollectionInitializer:
              return this.ParseAndScope(new RoslynParser.Parser(this.ParseCollectionInitializer), node, depth);
            case SyntaxKind.DirectCastExpression:
              return this.ParseAndScope(new RoslynParser.Parser(this.ParseDirectCastExpression), node, depth);
            default:
              goto label_73;
          }
        }
        else if (syntaxKind == SyntaxKind.IsExpression || syntaxKind == SyntaxKind.ArgumentList || syntaxKind == SyntaxKind.RangeArgument)
          goto label_73;
        else
          goto label_73;
      }
      else if ((uint) syntaxKind <= 506U)
      {
        switch (syntaxKind)
        {
          case SyntaxKind.FunctionAggregation:
            return this.ParseAndScope(new RoslynParser.Parser(this.ParseFunctionAggregation), (SyntaxNode) (node as FunctionAggregationSyntax), depth);
          case SyntaxKind.ArrayType:
            return this.ParseAndScope(new RoslynParser.Parser(this.ParseArrayType), node, depth);
          case SyntaxKind.PredefinedType:
            return this.ParseAndScope(new RoslynParser.Parser(this.ParsePredefinedType), node, depth);
          case SyntaxKind.IdentifierName:
            return this.ParseAndScope(new RoslynParser.Parser(this.ParseIdentifierName), node, depth);
          case SyntaxKind.GenericName:
            return this.ParseAndScope(new RoslynParser.Parser(this.ParseGenericName), node, depth);
          case SyntaxKind.QualifiedName:
            return this.ParseAndScope(new RoslynParser.Parser(this.ParseQualifiedName), node, depth);
          default:
            goto label_73;
        }
      }
      else
      {
        switch (syntaxKind)
        {
          case SyntaxKind.ImportAliasClause:
            return this.ParseAndScope(new RoslynParser.Parser(this.ParseImportAliasClause), node, depth);
          case SyntaxKind.NameColonEquals:
            return this.ParseAndScope(new RoslynParser.Parser(this.ParseNameColonEquals), node, depth);
          default:
            goto label_73;
        }
      }
      return this.ParseAndScope(new RoslynParser.Parser(this.ParsePrimaryExpression), (SyntaxNode) (node as LiteralExpressionSyntax), depth);
label_73:
      if (node.ChildNodesAndTokens().Count<SyntaxNodeOrToken>() > 0)
      {
        int count = this.SymbolsList.Count;
        foreach (SyntaxNodeOrToken childNodesAndToken in node.ChildNodesAndTokens())
        {
          if (childNodesAndToken.IsNode)
            this.ParseTree(childNodesAndToken.AsNode(), depth + 1);
          else if (!this.IgnoredKind(childNodesAndToken.AsToken().Kind()))
            this.AddSymbol(childNodesAndToken.AsToken(), CodeTokenKind.CSUnknown, depth);
          else
            this.ParseTriva(childNodesAndToken.AsToken(), depth + 1);
        }
        return this.SymbolsList.GetRange(count, this.SymbolsList.Count - count);
      }
      foreach (SyntaxToken childToken in node.ChildTokens())
        this.AddSymbol(childToken, this.GetTokenKind(childToken.Kind()), depth);
      return this.SymbolsList.GetRange(this.SymbolsList.Count - 1, 1);
    }

    private bool IgnoredKind(SyntaxKind syntaxKind)
    {
      if ((uint) syntaxKind <= 695U)
      {
        switch (syntaxKind)
        {
          case SyntaxKind.CompilationUnit:
          case SyntaxKind.ExclamationToken:
          case SyntaxKind.CommaToken:
          case SyntaxKind.HashToken:
          case SyntaxKind.AmpersandToken:
          case SyntaxKind.SingleQuoteToken:
          case SyntaxKind.OpenParenToken:
          case SyntaxKind.CloseParenToken:
          case SyntaxKind.OpenBraceToken:
          case SyntaxKind.CloseBraceToken:
          case SyntaxKind.SemicolonToken:
          case SyntaxKind.AsteriskToken:
          case SyntaxKind.PlusToken:
          case SyntaxKind.MinusToken:
          case SyntaxKind.DotToken:
          case SyntaxKind.SlashToken:
          case SyntaxKind.ColonToken:
          case SyntaxKind.LessThanToken:
          case SyntaxKind.LessThanEqualsToken:
          case SyntaxKind.EqualsToken:
          case SyntaxKind.GreaterThanToken:
          case SyntaxKind.GreaterThanEqualsToken:
          case SyntaxKind.BackslashToken:
          case SyntaxKind.CaretToken:
          case SyntaxKind.ColonEqualsToken:
          case SyntaxKind.AmpersandEqualsToken:
          case SyntaxKind.AsteriskEqualsToken:
          case SyntaxKind.PlusEqualsToken:
          case SyntaxKind.MinusEqualsToken:
          case SyntaxKind.SlashEqualsToken:
          case SyntaxKind.CaretEqualsToken:
          case SyntaxKind.LessThanLessThanToken:
          case SyntaxKind.GreaterThanGreaterThanToken:
          case SyntaxKind.LessThanLessThanEqualsToken:
          case SyntaxKind.GreaterThanGreaterThanEqualsToken:
          case SyntaxKind.QuestionToken:
          case SyntaxKind.DoubleQuoteToken:
          case SyntaxKind.EndOfFileToken:
          case SyntaxKind.SlashGreaterThanToken:
          case SyntaxKind.LessThanSlashToken:
          case SyntaxKind.XmlTextLiteralToken:
            break;
          default:
            goto label_4;
        }
      }
      else
      {
        switch (syntaxKind)
        {
          case SyntaxKind.WhitespaceTrivia:
          case SyntaxKind.EndOfLineTrivia:
          case SyntaxKind.EndIfDirectiveTrivia:
          case SyntaxKind.EndExternalSourceDirectiveTrivia:
            break;
          default:
            goto label_4;
        }
      }
      return true;
label_4:
      return false;
    }
  }
}
