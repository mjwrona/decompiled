// Decompiled with JetBrains decompiler
// Type: Microsoft.Ajax.Utilities.JSParser
// Assembly: WebGrease, Version=1.6.5135.21930, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 86487675-C393-48D4-AFEC-7657DB09B21F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\WebGrease.dll

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Reflection;

namespace Microsoft.Ajax.Utilities
{
  public class JSParser
  {
    private GlobalScope m_globalScope;
    private JSScanner m_scanner;
    private Context m_currentToken;
    private bool m_newModule;
    private CodeSettings m_settings;
    private bool m_foundEndOfLine;
    private IList<Context> m_importantComments;
    private Dictionary<string, LabelInfo> m_labelInfo;
    private long[] m_timingPoints;

    private Context CurrentPositionContext => this.m_currentToken.FlattenToStart();

    public ICollection<string> DebugLookups { get; private set; }

    public ScriptVersion ParsedVersion { get; private set; }

    public CodeSettings Settings
    {
      get
      {
        if (this.m_settings == null)
          this.m_settings = new CodeSettings();
        return this.m_settings;
      }
      set => this.m_settings = value ?? new CodeSettings();
    }

    public TextWriter EchoWriter { get; set; }

    public GlobalScope GlobalScope
    {
      get
      {
        if (this.m_globalScope == null)
          this.m_globalScope = new GlobalScope(this.m_settings);
        return this.m_globalScope;
      }
      set
      {
        this.m_globalScope = value;
        if (this.m_globalScope == null)
          return;
        foreach (ActivationObject childScope in (IEnumerable<ActivationObject>) this.m_globalScope.ChildScopes)
          childScope.Existing = true;
      }
    }

    public IList<long> TimingPoints => (IList<long>) this.m_timingPoints;

    public event EventHandler<ContextErrorEventArgs> CompilerError;

    public event EventHandler<UndefinedReferenceEventArgs> UndefinedReference;

    public JSParser()
    {
      this.m_importantComments = (IList<Context>) new List<Context>();
      this.m_labelInfo = new Dictionary<string, LabelInfo>();
    }

    [Obsolete("This Constructor will be removed in version 6. Please use the default constructor.", false)]
    public JSParser(string source)
      : this()
    {
      this.SetDocumentContext(new DocumentContext(source));
    }

    public Block Parse(DocumentContext sourceContext)
    {
      this.SetDocumentContext(sourceContext);
      if (this.m_settings == null)
        this.m_settings = new CodeSettings();
      this.m_importantComments.Clear();
      this.m_labelInfo.Clear();
      return this.InternalParse();
    }

    public Block Parse(DocumentContext sourceContext, CodeSettings settings)
    {
      this.Settings = settings;
      return this.Parse(sourceContext);
    }

    public Block Parse(string source) => this.Parse(new DocumentContext(source));

    public Block Parse(string source, CodeSettings settings)
    {
      this.Settings = settings;
      return this.Parse(source);
    }

    [Obsolete("This method will be removed in version 6. Please use the default constructor and use a Parse override that is passed the source.", false)]
    public Block Parse(CodeSettings settings)
    {
      if (this.m_scanner == null)
        throw new InvalidOperationException(JScript.NoSource);
      this.m_settings = settings = settings ?? new CodeSettings();
      return this.InternalParse();
    }

    private Block InternalParse()
    {
      this.DebugLookups = (ICollection<string>) new HashSet<string>(this.m_settings.DebugLookupCollection);
      this.m_scanner.DebugLookupCollection = this.DebugLookups;
      this.m_scanner.AllowEmbeddedAspNetBlocks = this.m_settings.AllowEmbeddedAspNetBlocks;
      this.m_scanner.IgnoreConditionalCompilation = this.m_settings.IgnoreConditionalCompilation;
      this.m_scanner.UsePreprocessorDefines = !this.m_settings.IgnorePreprocessorDefines;
      if (this.m_scanner.UsePreprocessorDefines)
        this.m_scanner.SetPreprocessorDefines(this.m_settings.PreprocessorValues);
      this.m_scanner.StripDebugCommentBlocks = this.m_settings.StripDebugStatements;
      this.ParsedVersion = ScriptVersion.EcmaScript5;
      this.GlobalScope.UseStrict = this.m_settings.StrictMode;
      this.GlobalScope.SetAssumedGlobals(this.m_settings);
      this.m_newModule = true;
      long[] numArray = this.m_timingPoints = new long[9];
      int length = numArray.Length;
      Stopwatch stopwatch = new Stopwatch();
      stopwatch.Start();
      this.GetNextToken();
      Block block1;
      Block block2;
      switch (this.m_settings.SourceMode)
      {
        case JavaScriptSourceMode.Program:
          Block block3 = new Block(this.CurrentPositionContext);
          block3.EnclosingScope = (ActivationObject) this.GlobalScope;
          block2 = block1 = this.ParseStatements(block3);
          break;
        case JavaScriptSourceMode.Expression:
          Block block4 = new Block(this.CurrentPositionContext);
          block4.EnclosingScope = (ActivationObject) this.GlobalScope;
          block1 = block2 = block4;
          try
          {
            AstNode expression = this.ParseExpression();
            if (expression != null)
            {
              block2.Append(expression);
              block2.UpdateWith(expression.Context);
              break;
            }
            break;
          }
          catch (EndOfStreamException ex)
          {
            break;
          }
        case JavaScriptSourceMode.EventHandler:
          Block block5 = new Block(this.CurrentPositionContext);
          block5.EnclosingScope = (ActivationObject) this.GlobalScope;
          block2 = block5;
          AstNodeList astNodeList = new AstNodeList(this.CurrentPositionContext);
          astNodeList.Append((AstNode) new ParameterDeclaration(this.CurrentPositionContext)
          {
            Binding = (AstNode) new BindingIdentifier(this.CurrentPositionContext)
            {
              Name = "event",
              RenameNotAllowed = true
            }
          });
          FunctionObject functionObject = new FunctionObject(this.CurrentPositionContext)
          {
            FunctionType = FunctionType.Expression,
            ParameterDeclarations = astNodeList,
            Body = new Block(this.CurrentPositionContext)
          };
          block2.Append((AstNode) functionObject);
          this.ParseFunctionBody(functionObject.Body);
          block1 = functionObject.Body;
          break;
        case JavaScriptSourceMode.Module:
          Block block6 = new Block(this.CurrentPositionContext);
          block6.EnclosingScope = (ActivationObject) this.GlobalScope;
          block1 = block2 = block6;
          ModuleDeclaration moduleDeclaration = new ModuleDeclaration(this.CurrentPositionContext)
          {
            IsImplicit = true,
            Body = new Block(this.CurrentPositionContext)
            {
              IsModule = true
            }
          };
          block2.Append((AstNode) moduleDeclaration);
          this.ParsedVersion = ScriptVersion.EcmaScript6;
          this.ParseStatements(moduleDeclaration.Body);
          break;
        default:
          return (Block) null;
      }
      int num1;
      numArray[num1 = length - 1] = stopwatch.ElapsedTicks;
      ResolutionVisitor.Apply(block2, (ActivationObject) this.GlobalScope, this);
      int num2;
      numArray[num2 = num1 - 1] = stopwatch.ElapsedTicks;
      if (block2 != null && this.Settings.MinifyCode && !this.Settings.PreprocessOnly)
      {
        ReorderScopeVisitor.Apply(block2, this);
        int num3;
        numArray[num3 = num2 - 1] = stopwatch.ElapsedTicks;
        AnalyzeNodeVisitor analyzeNodeVisitor = new AnalyzeNodeVisitor(this);
        block2.Accept((IVisitor) analyzeNodeVisitor);
        int num4;
        numArray[num4 = num3 - 1] = stopwatch.ElapsedTicks;
        this.GlobalScope.AnalyzeScope();
        int num5;
        numArray[num5 = num4 - 1] = stopwatch.ElapsedTicks;
        if (this.m_settings.LocalRenaming != LocalRenaming.KeepAll && this.m_settings.IsModificationAllowed(TreeModifications.LocalRenaming))
          this.GlobalScope.AutoRenameFields();
        int num6;
        numArray[num6 = num5 - 1] = stopwatch.ElapsedTicks;
        if (this.m_settings.EvalLiteralExpressions)
        {
          EvaluateLiteralVisitor evaluateLiteralVisitor = new EvaluateLiteralVisitor(this);
          block2.Accept((IVisitor) evaluateLiteralVisitor);
        }
        int num7;
        numArray[num7 = num6 - 1] = stopwatch.ElapsedTicks;
        FinalPassVisitor.Apply((AstNode) block2, this.m_settings);
        int num8;
        numArray[num8 = num7 - 1] = stopwatch.ElapsedTicks;
        this.GlobalScope.ValidateGeneratedNames();
        int num9;
        numArray[num9 = num8 - 1] = stopwatch.ElapsedTicks;
        stopwatch.Stop();
      }
      foreach (ActivationObject childScope in (IEnumerable<ActivationObject>) this.GlobalScope.ChildScopes)
        childScope.Existing = true;
      if (block1 != block2)
      {
        block1.EnclosingScope = block1.Parent.EnclosingScope;
        block1.Parent = (AstNode) null;
      }
      return block1;
    }

    internal void OnUndefinedReference(Microsoft.Ajax.Utilities.UndefinedReference ex)
    {
      if (this.UndefinedReference == null)
        return;
      this.UndefinedReference((object) this, new UndefinedReferenceEventArgs(ex));
    }

    internal void OnCompilerError(ContextError se)
    {
      if (this.CompilerError == null || this.m_settings.IgnoreAllErrors || this.m_settings == null || this.m_settings.IgnoreErrorCollection.Contains(se.ErrorCode))
        return;
      this.CompilerError((object) this, new ContextErrorEventArgs()
      {
        Error = se
      });
    }

    private Block ParseStatements(Block block)
    {
      Block statements = block;
      try
      {
        bool flag = true;
        int endPosition = this.m_currentToken.EndPosition;
        while (this.m_currentToken.IsNot(JSToken.EndOfFile))
        {
          AstNode astNode = this.ParseStatement(true);
          if (flag)
          {
            if (astNode is ConstantWrapper constantWrapper && constantWrapper.PrimitiveType == PrimitiveType.String)
            {
              if (!(constantWrapper is DirectivePrologue))
              {
                DirectivePrologue directivePrologue = new DirectivePrologue(constantWrapper.Value.ToString(), astNode.Context);
                directivePrologue.MayHaveIssues = constantWrapper.MayHaveIssues;
                astNode = (AstNode) directivePrologue;
              }
            }
            else if (!this.m_newModule)
              flag = false;
          }
          else if (this.m_newModule)
            flag = true;
          if (astNode != null)
          {
            block.Append(astNode);
            if (astNode is ExportNode && !block.IsModule)
            {
              block.IsModule = true;
              if (block.Parent == null)
              {
                Block block1 = new Block(block.Context.Clone());
                block1.EnclosingScope = block.EnclosingScope;
                statements = block1;
                block.EnclosingScope = (ActivationObject) null;
                statements.Append((AstNode) new ModuleDeclaration(new Context(this.m_currentToken.Document))
                {
                  IsImplicit = true,
                  Body = block
                });
              }
            }
            endPosition = this.m_currentToken.EndPosition;
          }
          else if (!this.m_scanner.IsEndOfFile && this.m_currentToken.StartLinePosition == endPosition)
          {
            this.m_currentToken.HandleError(JSError.ApplicationError, true);
            break;
          }
        }
        this.AppendImportantComments(block);
      }
      catch (EndOfStreamException ex)
      {
      }
      block.UpdateWith(this.CurrentPositionContext);
      return statements;
    }

    private AstNode ParseStatement(bool fSourceElement, bool skipImportantComment = false)
    {
      AstNode statement1 = (AstNode) null;
      if (skipImportantComment)
        this.m_importantComments.Clear();
      if (this.m_importantComments.Count > 0 && this.m_settings.PreserveImportantComments && this.m_settings.IsModificationAllowed(TreeModifications.PreserveImportantComments))
      {
        statement1 = (AstNode) new ImportantComment(this.m_importantComments[0]);
        this.m_importantComments.RemoveAt(0);
      }
      else
      {
        switch (this.m_currentToken.Token)
        {
          case JSToken.EndOfFile:
            this.ReportError(JSError.ErrorEndOfFile);
            return (AstNode) null;
          case JSToken.Semicolon:
            AstNode statement2 = (AstNode) new EmptyStatement(this.m_currentToken.Clone());
            this.GetNextToken();
            return statement2;
          case JSToken.RightCurly:
            this.ReportError(JSError.SyntaxError);
            this.GetNextToken();
            goto label_38;
          case JSToken.LeftCurly:
            return (AstNode) this.ParseBlock();
          case JSToken.Debugger:
            return this.ParseDebuggerStatement();
          case JSToken.Var:
          case JSToken.Const:
          case JSToken.Let:
            return this.ParseVariableStatement();
          case JSToken.If:
            return (AstNode) this.ParseIfStatement();
          case JSToken.For:
            return this.ParseForStatement();
          case JSToken.Do:
            return (AstNode) this.ParseDoStatement();
          case JSToken.While:
            return (AstNode) this.ParseWhileStatement();
          case JSToken.Continue:
            return (AstNode) this.ParseContinueStatement();
          case JSToken.Break:
            return (AstNode) this.ParseBreakStatement();
          case JSToken.Return:
            return (AstNode) this.ParseReturnStatement();
          case JSToken.With:
            return (AstNode) this.ParseWithStatement();
          case JSToken.Switch:
            return this.ParseSwitchStatement();
          case JSToken.Throw:
            return this.ParseThrowStatement();
          case JSToken.Try:
            return this.ParseTryStatement();
          case JSToken.Function:
            FunctionObject function = this.ParseFunction(FunctionType.Declaration, this.m_currentToken.Clone());
            function.IsSourceElement = fSourceElement;
            return (AstNode) function;
          case JSToken.Else:
            this.ReportError(JSError.InvalidElse);
            this.GetNextToken();
            goto label_38;
          case JSToken.ConditionalCommentStart:
            return this.ParseStatementLevelConditionalComment(fSourceElement);
          case JSToken.ConditionalCompilationOn:
            ConditionalCompilationOn statement3 = new ConditionalCompilationOn(this.m_currentToken.Clone());
            this.GetNextToken();
            return (AstNode) statement3;
          case JSToken.ConditionalCompilationSet:
            return (AstNode) this.ParseConditionalCompilationSet();
          case JSToken.ConditionalCompilationIf:
            return (AstNode) this.ParseConditionalCompilationIf(false);
          case JSToken.ConditionalCompilationElseIf:
            return (AstNode) this.ParseConditionalCompilationIf(true);
          case JSToken.ConditionalCompilationElse:
            ConditionalCompilationElse statement4 = new ConditionalCompilationElse(this.m_currentToken.Clone());
            this.GetNextToken();
            return (AstNode) statement4;
          case JSToken.ConditionalCompilationEnd:
            ConditionalCompilationEnd statement5 = new ConditionalCompilationEnd(this.m_currentToken.Clone());
            this.GetNextToken();
            return (AstNode) statement5;
          case JSToken.Identifier:
            if (!this.m_currentToken.Is("module"))
              break;
            goto case JSToken.Module;
          case JSToken.Class:
            return (AstNode) this.ParseClassNode(ClassType.Declaration);
          case JSToken.Export:
            return this.ParseExport();
          case JSToken.Import:
            return this.ParseImport();
          case JSToken.Module:
            if (this.PeekCanBeModule())
              return this.ParseModule();
            break;
        }
        statement1 = this.ParseExpressionStatement(fSourceElement);
      }
label_38:
      return statement1;
    }

    private AstNode ParseExpressionStatement(bool fSourceElement)
    {
      bool newModule = this.m_newModule;
      bool isLeftHandSideExpr;
      AstNode expressionStatement = this.ParseUnaryExpression(out isLeftHandSideExpr, false);
      if (expressionStatement != null)
      {
        if (expressionStatement is Lookup lookup1 && this.m_currentToken.Is(JSToken.Colon))
        {
          expressionStatement = (AstNode) this.ParseLabeledStatement(lookup1, fSourceElement);
        }
        else
        {
          expressionStatement = this.ParseExpression(expressionStatement, false, isLeftHandSideExpr, JSToken.None);
          if (newModule && expressionStatement.IsExpression && expressionStatement is ConstantWrapper constantWrapper && constantWrapper.PrimitiveType == PrimitiveType.String && !(expressionStatement is DirectivePrologue))
          {
            DirectivePrologue directivePrologue = new DirectivePrologue(constantWrapper.Value.ToString(), constantWrapper.Context);
            directivePrologue.MayHaveIssues = constantWrapper.MayHaveIssues;
            expressionStatement = (AstNode) directivePrologue;
          }
          if (expressionStatement is BinaryOperator binaryOperator && (binaryOperator.OperatorToken == JSToken.Equal || binaryOperator.OperatorToken == JSToken.StrictEqual))
            binaryOperator.OperatorContext.IfNotNull<Context>((Action<Context>) (c => c.HandleError(JSError.SuspectEquality)));
          if (expressionStatement is Lookup lookup && lookup.Name.StartsWith("<%=", StringComparison.Ordinal) && lookup.Name.EndsWith("%>", StringComparison.Ordinal))
            expressionStatement = (AstNode) new AspNetBlockNode(expressionStatement.Context)
            {
              AspNetBlockText = lookup.Name
            };
          if (expressionStatement is AspNetBlockNode aspNetBlockNode && this.m_currentToken.Is(JSToken.Semicolon))
          {
            aspNetBlockNode.IsTerminatedByExplicitSemicolon = true;
            expressionStatement.IfNotNull<AstNode, Context>((Func<AstNode, Context>) (s => s.TerminatingContext = this.m_currentToken.Clone()));
            this.GetNextToken();
          }
          this.ExpectSemicolon(expressionStatement);
        }
      }
      else
        this.GetNextToken();
      return expressionStatement;
    }

    private LabeledStatement ParseLabeledStatement(Lookup lookup, bool fSourceElement)
    {
      string name = lookup.Name;
      Context context = this.m_currentToken.Clone();
      bool flag = true;
      LabelInfo labelInfo;
      if (this.m_labelInfo.TryGetValue(name, out labelInfo))
      {
        labelInfo.HasIssues = true;
        flag = false;
        lookup.Context.HandleError(JSError.BadLabel, true);
      }
      else
      {
        labelInfo = new LabelInfo()
        {
          NestLevel = this.m_labelInfo.Count,
          RefCount = 0
        };
        this.m_labelInfo.Add(name, labelInfo);
      }
      this.GetNextToken();
      LabeledStatement labeledStatement;
      if (this.m_currentToken.IsNot(JSToken.EndOfFile))
        labeledStatement = new LabeledStatement(lookup.Context.Clone())
        {
          Label = name,
          LabelContext = lookup.Context,
          LabelInfo = labelInfo,
          ColonContext = context,
          Statement = this.ParseStatement(fSourceElement, true)
        };
      else
        labeledStatement = new LabeledStatement(lookup.Context.Clone())
        {
          Label = name,
          LabelContext = lookup.Context,
          LabelInfo = labelInfo,
          ColonContext = context
        };
      if (flag)
        this.m_labelInfo.Remove(name);
      return labeledStatement;
    }

    private AstNode ParseStatementLevelConditionalComment(bool fSourceElement)
    {
      ConditionalCompilationComment compilationComment = new ConditionalCompilationComment(this.m_currentToken.Clone());
      this.GetNextToken();
      while (this.m_currentToken.IsNot(JSToken.ConditionalCommentEnd) && this.m_currentToken.IsNot(JSToken.EndOfFile))
      {
        if (this.m_currentToken.Is(JSToken.ConditionalCommentStart))
          this.GetNextToken();
        else
          compilationComment.Append(this.ParseStatement(fSourceElement));
      }
      this.GetNextToken();
      return compilationComment.Statements.Count <= 0 ? (AstNode) null : (AstNode) compilationComment;
    }

    private ConditionalCompilationSet ParseConditionalCompilationSet()
    {
      Context context = this.m_currentToken.Clone();
      string str = (string) null;
      AstNode astNode = (AstNode) null;
      this.GetNextToken();
      if (this.m_currentToken.Is(JSToken.ConditionalCompilationVariable))
      {
        context.UpdateWith(this.m_currentToken);
        str = this.m_currentToken.Code;
        this.GetNextToken();
        if (this.m_currentToken.Is(JSToken.Assign))
        {
          context.UpdateWith(this.m_currentToken);
          this.GetNextToken();
          astNode = this.ParseExpression();
          if (astNode != null)
            context.UpdateWith(astNode.Context);
          else
            this.m_currentToken.HandleError(JSError.ExpressionExpected);
        }
        else
          this.m_currentToken.HandleError(JSError.NoEqual);
      }
      else
        this.m_currentToken.HandleError(JSError.NoIdentifier);
      return new ConditionalCompilationSet(context)
      {
        VariableName = str,
        Value = astNode
      };
    }

    private ConditionalCompilationStatement ParseConditionalCompilationIf(bool isElseIf)
    {
      Context context = this.m_currentToken.Clone();
      AstNode astNode = (AstNode) null;
      this.GetNextToken();
      if (this.m_currentToken.Is(JSToken.LeftParenthesis))
      {
        context.UpdateWith(this.m_currentToken);
        this.GetNextToken();
        astNode = this.ParseExpression();
        if (astNode != null)
          context.UpdateWith(astNode.Context);
        else
          this.m_currentToken.HandleError(JSError.ExpressionExpected);
        if (this.m_currentToken.Is(JSToken.RightParenthesis))
        {
          context.UpdateWith(this.m_currentToken);
          this.GetNextToken();
        }
        else
          this.m_currentToken.HandleError(JSError.NoRightParenthesis);
      }
      else
        this.m_currentToken.HandleError(JSError.NoLeftParenthesis);
      if (isElseIf)
        return (ConditionalCompilationStatement) new ConditionalCompilationElseIf(context)
        {
          Condition = astNode
        };
      return (ConditionalCompilationStatement) new ConditionalCompilationIf(context)
      {
        Condition = astNode
      };
    }

    private Block ParseBlock()
    {
      Block block = new Block(this.m_currentToken.Clone())
      {
        ForceBraces = true
      };
      block.BraceOnNewLine = this.m_foundEndOfLine;
      this.GetNextToken();
      while (this.m_currentToken.IsNot(JSToken.RightCurly) && this.m_currentToken.IsNot(JSToken.EndOfFile))
        block.Append(this.ParseStatement(false));
      this.AppendImportantComments(block);
      if (this.m_currentToken.IsNot(JSToken.RightCurly))
      {
        this.ReportError(JSError.NoRightCurly);
        if (this.m_currentToken.Is(JSToken.EndOfFile))
          this.ReportError(JSError.ErrorEndOfFile);
      }
      block.TerminatingContext = this.m_currentToken.Clone();
      block.Context.UpdateWith(this.m_currentToken);
      this.GetNextToken();
      return block;
    }

    private AstNode ParseDebuggerStatement()
    {
      DebuggerNode node = new DebuggerNode(this.m_currentToken.Clone());
      this.GetNextToken();
      this.ExpectSemicolon((AstNode) node);
      return (AstNode) node;
    }

    private AstNode ParseVariableStatement()
    {
      Declaration node;
      if (this.m_currentToken.Is(JSToken.Var))
      {
        Var var = new Var(this.m_currentToken.Clone());
        var.StatementToken = this.m_currentToken.Token;
        var.KeywordContext = this.m_currentToken.Clone();
        node = (Declaration) var;
      }
      else
      {
        if (!this.m_currentToken.IsOne(JSToken.Const, JSToken.Let))
          return (AstNode) null;
        if (this.m_currentToken.Is(JSToken.Const) && this.m_settings.ConstStatementsMozilla)
        {
          ConstStatement constStatement = new ConstStatement(this.m_currentToken.Clone());
          constStatement.StatementToken = this.m_currentToken.Token;
          constStatement.KeywordContext = this.m_currentToken.Clone();
          node = (Declaration) constStatement;
        }
        else
        {
          this.ParsedVersion = ScriptVersion.EcmaScript6;
          LexicalDeclaration lexicalDeclaration = new LexicalDeclaration(this.m_currentToken.Clone());
          lexicalDeclaration.StatementToken = this.m_currentToken.Token;
          lexicalDeclaration.KeywordContext = this.m_currentToken.Clone();
          node = (Declaration) lexicalDeclaration;
        }
      }
      do
      {
        this.GetNextToken();
        VariableDeclaration varDecl = this.ParseVarDecl(JSToken.None);
        if (varDecl != null)
        {
          node.Append((AstNode) varDecl);
          node.Context.UpdateWith(varDecl.Context);
        }
      }
      while (this.m_currentToken.Is(JSToken.Comma));
      this.ExpectSemicolon((AstNode) node);
      return (AstNode) node;
    }

    private VariableDeclaration ParseVarDecl(JSToken inToken)
    {
      Context context1 = this.m_currentToken.Clone();
      VariableDeclaration varDecl = (VariableDeclaration) null;
      AstNode binding = this.ParseBinding();
      if (binding != null)
      {
        Context context2 = (Context) null;
        AstNode astNode = (AstNode) null;
        bool flag1 = false;
        bool flag2 = false;
        if (this.m_currentToken.Is(JSToken.ConditionalCommentStart))
        {
          flag1 = true;
          this.GetNextToken();
          if (this.m_currentToken.Is(JSToken.ConditionalCompilationOn))
          {
            this.GetNextToken();
            if (this.m_currentToken.Is(JSToken.ConditionalCommentEnd))
              flag1 = false;
            else
              flag2 = true;
          }
        }
        if (this.m_currentToken.IsOne(JSToken.Assign, JSToken.Equal))
        {
          context2 = this.m_currentToken.Clone();
          if (this.m_currentToken.Is(JSToken.Equal))
            this.ReportError(JSError.NoEqual);
          this.GetNextToken();
          if (this.m_currentToken.Is(JSToken.ConditionalCommentEnd))
          {
            flag1 = false;
            this.m_currentToken.HandleError(JSError.ConditionalCompilationTooComplex);
            this.GetNextToken();
          }
          astNode = this.ParseExpression(true, inToken);
          if (astNode != null)
            context1.UpdateWith(astNode.Context);
        }
        else if (flag1)
        {
          flag1 = false;
          this.m_currentToken.HandleError(JSError.ConditionalCompilationTooComplex);
          while (this.m_currentToken.IsNot(JSToken.EndOfFile) && this.m_currentToken.IsNot(JSToken.ConditionalCommentEnd))
            this.GetNextToken();
          this.GetNextToken();
        }
        if (this.m_currentToken.Is(JSToken.ConditionalCommentEnd))
          this.GetNextToken();
        else if (flag1)
        {
          flag1 = false;
          this.m_currentToken.HandleError(JSError.ConditionalCompilationTooComplex);
          astNode = (AstNode) null;
          while (this.m_currentToken.IsNot(JSToken.EndOfFile) && this.m_currentToken.IsNot(JSToken.ConditionalCommentEnd))
            this.GetNextToken();
          this.GetNextToken();
        }
        VariableDeclaration variableDeclaration = new VariableDeclaration(context1);
        variableDeclaration.Binding = binding;
        variableDeclaration.AssignContext = context2;
        variableDeclaration.Initializer = astNode;
        variableDeclaration.IsCCSpecialCase = flag1;
        variableDeclaration.UseCCOn = flag2;
        varDecl = variableDeclaration;
      }
      return varDecl;
    }

    private AstNode ParseBinding()
    {
      AstNode binding;
      if (this.m_currentToken.Is(JSToken.Identifier))
      {
        binding = (AstNode) new BindingIdentifier(this.m_currentToken.Clone())
        {
          Name = this.m_scanner.Identifier
        };
        this.GetNextToken();
      }
      else if (this.m_currentToken.Is(JSToken.LeftBracket))
      {
        this.ParsedVersion = ScriptVersion.EcmaScript6;
        binding = this.ParseArrayLiteral(true);
      }
      else if (this.m_currentToken.Is(JSToken.LeftCurly))
      {
        this.ParsedVersion = ScriptVersion.EcmaScript6;
        binding = (AstNode) this.ParseObjectLiteral(true);
      }
      else
      {
        string str = JSKeyword.CanBeIdentifier(this.m_currentToken.Token);
        if (str != null)
        {
          binding = (AstNode) new BindingIdentifier(this.m_currentToken.Clone())
          {
            Name = str
          };
          this.GetNextToken();
        }
        else
        {
          string code;
          if (JSScanner.IsValidIdentifier(code = this.m_currentToken.Code))
          {
            this.ReportError(JSError.NoIdentifier);
            binding = (AstNode) new BindingIdentifier(this.m_currentToken.Clone())
            {
              Name = code
            };
            this.GetNextToken();
          }
          else
          {
            this.ReportError(JSError.NoIdentifier);
            return (AstNode) null;
          }
        }
      }
      return binding;
    }

    private IfNode ParseIfStatement()
    {
      Context ifCtx = this.m_currentToken.Clone();
      AstNode node = (AstNode) null;
      Context context = (Context) null;
      this.GetNextToken();
      if (this.m_currentToken.IsNot(JSToken.LeftParenthesis))
        this.ReportError(JSError.NoLeftParenthesis);
      else
        this.GetNextToken();
      AstNode expression = this.ParseExpression();
      if (this.m_currentToken.Is(JSToken.RightParenthesis))
      {
        ifCtx.UpdateWith(this.m_currentToken);
        this.GetNextToken();
      }
      else
      {
        expression.IfNotNull<AstNode, Context>((Func<AstNode, Context>) (c => ifCtx.UpdateWith(c.Context)));
        this.ReportError(JSError.NoRightParenthesis);
      }
      if (expression is BinaryOperator binaryOperator && binaryOperator.OperatorToken == JSToken.Assign)
        expression.Context.HandleError(JSError.SuspectAssignment);
      if (this.m_currentToken.Is(JSToken.Semicolon))
        this.m_currentToken.HandleError(JSError.SuspectSemicolon);
      else if (this.m_currentToken.IsNot(JSToken.LeftCurly))
        this.ReportError(JSError.StatementBlockExpected, this.CurrentPositionContext);
      AstNode statement = this.ParseStatement(false, true);
      if (statement != null)
        ifCtx.UpdateWith(statement.Context);
      if (this.m_currentToken.Is(JSToken.Else))
      {
        context = this.m_currentToken.Clone();
        this.GetNextToken();
        if (this.m_currentToken.Is(JSToken.Semicolon))
          this.m_currentToken.HandleError(JSError.SuspectSemicolon);
        else if (this.m_currentToken.IsNot(JSToken.LeftCurly) && this.m_currentToken.IsNot(JSToken.If))
          this.ReportError(JSError.StatementBlockExpected, this.CurrentPositionContext);
        node = this.ParseStatement(false, true);
        if (node != null)
          ifCtx.UpdateWith(node.Context);
      }
      return new IfNode(ifCtx)
      {
        Condition = expression,
        TrueBlock = AstNode.ForceToBlock(statement),
        ElseContext = context,
        FalseBlock = AstNode.ForceToBlock(node)
      };
    }

    private AstNode ParseForStatement()
    {
      Context context1 = this.m_currentToken.Clone();
      this.GetNextToken();
      if (this.m_currentToken.Is(JSToken.LeftParenthesis))
        this.GetNextToken();
      else
        this.ReportError(JSError.NoLeftParenthesis);
      AstNode astNode1 = (AstNode) null;
      AstNode astNode2 = (AstNode) null;
      AstNode astNode3 = (AstNode) null;
      Context context2 = (Context) null;
      Context context3 = (Context) null;
      Context context4 = (Context) null;
      if (this.m_currentToken.IsOne(JSToken.Var, JSToken.Let, JSToken.Const))
      {
        Declaration declaration;
        if (this.m_currentToken.Is(JSToken.Var))
        {
          Var var = new Var(this.m_currentToken.Clone());
          var.StatementToken = this.m_currentToken.Token;
          var.KeywordContext = this.m_currentToken.Clone();
          declaration = (Declaration) var;
        }
        else
        {
          this.ParsedVersion = ScriptVersion.EcmaScript6;
          LexicalDeclaration lexicalDeclaration = new LexicalDeclaration(this.m_currentToken.Clone());
          lexicalDeclaration.StatementToken = this.m_currentToken.Token;
          lexicalDeclaration.KeywordContext = this.m_currentToken.Clone();
          declaration = (Declaration) lexicalDeclaration;
        }
        this.GetNextToken();
        declaration.Append((AstNode) this.ParseVarDecl(JSToken.In));
        while (this.m_currentToken.Is(JSToken.Comma))
        {
          this.GetNextToken();
          declaration.Append((AstNode) this.ParseVarDecl(JSToken.In));
        }
        astNode1 = (AstNode) declaration;
      }
      else if (this.m_currentToken.IsNot(JSToken.Semicolon))
        astNode1 = this.ParseExpression(inToken: JSToken.In);
      bool flag = this.m_currentToken.Is(JSToken.In) || this.m_currentToken.Is("of");
      if (flag)
      {
        if (this.m_currentToken.IsNot(JSToken.In))
          this.ParsedVersion = ScriptVersion.EcmaScript6;
        context2 = this.m_currentToken.Clone();
        this.GetNextToken();
        astNode2 = this.ParseExpression();
      }
      else
      {
        if (this.m_currentToken.Is(JSToken.Semicolon))
        {
          context3 = this.m_currentToken.Clone();
          this.GetNextToken();
        }
        else
          this.ReportError(JSError.NoSemicolon);
        if (this.m_currentToken.IsNot(JSToken.Semicolon))
          astNode2 = this.ParseExpression();
        if (this.m_currentToken.Is(JSToken.Semicolon))
        {
          context4 = this.m_currentToken.Clone();
          this.GetNextToken();
        }
        else
          this.ReportError(JSError.NoSemicolon);
        if (this.m_currentToken.IsNot(JSToken.RightParenthesis))
          astNode3 = this.ParseExpression();
      }
      if (this.m_currentToken.Is(JSToken.RightParenthesis))
      {
        context1.UpdateWith(this.m_currentToken);
        this.GetNextToken();
      }
      else
        this.ReportError(JSError.NoRightParenthesis);
      if (this.m_currentToken.IsNot(JSToken.LeftCurly))
        this.ReportError(JSError.StatementBlockExpected, this.CurrentPositionContext);
      AstNode statement = this.ParseStatement(false, true);
      AstNode forStatement;
      if (flag)
      {
        ForIn forIn = new ForIn(context1);
        forIn.Variable = astNode1;
        forIn.OperatorContext = context2;
        forIn.Collection = astNode2;
        forIn.Body = AstNode.ForceToBlock(statement);
        forStatement = (AstNode) forIn;
      }
      else
      {
        if (astNode2 is BinaryOperator binaryOperator && binaryOperator.OperatorToken == JSToken.Assign)
          astNode2.Context.HandleError(JSError.SuspectAssignment);
        ForNode forNode = new ForNode(context1);
        forNode.Initializer = astNode1;
        forNode.Separator1Context = context3;
        forNode.Condition = astNode2;
        forNode.Separator2Context = context4;
        forNode.Incrementer = astNode3;
        forNode.Body = AstNode.ForceToBlock(statement);
        forStatement = (AstNode) forNode;
      }
      return forStatement;
    }

    private DoWhile ParseDoStatement()
    {
      Context context1 = this.m_currentToken.Clone();
      Context other = (Context) null;
      Context context2 = (Context) null;
      this.GetNextToken();
      if (this.m_currentToken.IsNot(JSToken.LeftCurly))
        this.ReportError(JSError.StatementBlockExpected, this.CurrentPositionContext);
      AstNode statement = this.ParseStatement(false, true);
      if (this.m_currentToken.IsNot(JSToken.While))
      {
        this.ReportError(JSError.NoWhile);
      }
      else
      {
        other = this.m_currentToken.Clone();
        context1.UpdateWith(other);
        this.GetNextToken();
      }
      if (this.m_currentToken.IsNot(JSToken.LeftParenthesis))
        this.ReportError(JSError.NoLeftParenthesis);
      else
        this.GetNextToken();
      AstNode expression = this.ParseExpression();
      if (this.m_currentToken.IsNot(JSToken.RightParenthesis))
      {
        this.ReportError(JSError.NoRightParenthesis);
        context1.UpdateWith(expression.Context);
      }
      else
      {
        context1.UpdateWith(this.m_currentToken);
        this.GetNextToken();
      }
      if (this.m_currentToken.Is(JSToken.Semicolon))
      {
        context2 = this.m_currentToken.Clone();
        this.GetNextToken();
      }
      if (expression is BinaryOperator binaryOperator && binaryOperator.OperatorToken == JSToken.Assign)
        expression.Context.HandleError(JSError.SuspectAssignment);
      DoWhile doStatement = new DoWhile(context1);
      doStatement.Body = AstNode.ForceToBlock(statement);
      doStatement.WhileContext = other;
      doStatement.Condition = expression;
      doStatement.TerminatingContext = context2;
      return doStatement;
    }

    private WhileNode ParseWhileStatement()
    {
      Context context = this.m_currentToken.Clone();
      this.GetNextToken();
      if (this.m_currentToken.IsNot(JSToken.LeftParenthesis))
        this.ReportError(JSError.NoLeftParenthesis);
      else
        this.GetNextToken();
      AstNode expression = this.ParseExpression();
      if (this.m_currentToken.IsNot(JSToken.RightParenthesis))
      {
        this.ReportError(JSError.NoRightParenthesis);
        context.UpdateWith(expression.Context);
      }
      else
      {
        context.UpdateWith(this.m_currentToken);
        this.GetNextToken();
      }
      if (expression is BinaryOperator binaryOperator && binaryOperator.OperatorToken == JSToken.Assign)
        expression.Context.HandleError(JSError.SuspectAssignment);
      if (this.m_currentToken.IsNot(JSToken.LeftCurly))
        this.ReportError(JSError.StatementBlockExpected, this.CurrentPositionContext);
      AstNode statement = this.ParseStatement(false, true);
      WhileNode whileStatement = new WhileNode(context);
      whileStatement.Condition = expression;
      whileStatement.Body = AstNode.ForceToBlock(statement);
      return whileStatement;
    }

    private ContinueNode ParseContinueStatement()
    {
      ContinueNode node = new ContinueNode(this.m_currentToken.Clone());
      this.GetNextToken();
      string str = (string) null;
      if (!this.m_foundEndOfLine && (this.m_currentToken.Is(JSToken.Identifier) || (str = JSKeyword.CanBeIdentifier(this.m_currentToken.Token)) != null))
      {
        node.UpdateWith(this.m_currentToken);
        node.LabelContext = this.m_currentToken.Clone();
        node.Label = str ?? this.m_scanner.Identifier;
        LabelInfo labelInfo;
        if (this.m_labelInfo.TryGetValue(node.Label, out labelInfo))
        {
          ++labelInfo.RefCount;
          node.LabelInfo = labelInfo;
        }
        else
          node.LabelContext.HandleError(JSError.NoLabel, true);
        this.GetNextToken();
      }
      this.ExpectSemicolon((AstNode) node);
      return node;
    }

    private Break ParseBreakStatement()
    {
      Break node = new Break(this.m_currentToken.Clone());
      this.GetNextToken();
      string str = (string) null;
      if (!this.m_foundEndOfLine && (this.m_currentToken.Is(JSToken.Identifier) || (str = JSKeyword.CanBeIdentifier(this.m_currentToken.Token)) != null))
      {
        node.UpdateWith(this.m_currentToken);
        node.LabelContext = this.m_currentToken.Clone();
        node.Label = str ?? this.m_scanner.Identifier;
        LabelInfo labelInfo;
        if (this.m_labelInfo.TryGetValue(node.Label, out labelInfo))
        {
          ++labelInfo.RefCount;
          node.LabelInfo = labelInfo;
        }
        else
          node.LabelContext.HandleError(JSError.NoLabel, true);
        this.GetNextToken();
      }
      this.ExpectSemicolon((AstNode) node);
      return node;
    }

    private ReturnNode ParseReturnStatement()
    {
      ReturnNode node = new ReturnNode(this.m_currentToken.Clone());
      this.GetNextToken();
      if (!this.m_foundEndOfLine)
      {
        if (this.m_currentToken.IsNot(JSToken.Semicolon) && this.m_currentToken.IsNot(JSToken.RightCurly))
        {
          node.Operand = this.ParseExpression();
          if (node.Operand != null)
            node.UpdateWith(node.Operand.Context);
        }
        this.ExpectSemicolon((AstNode) node);
      }
      else
        this.ReportError(JSError.SemicolonInsertion, node.Context.FlattenToEnd());
      return node;
    }

    private WithNode ParseWithStatement()
    {
      Context context = this.m_currentToken.Clone();
      this.GetNextToken();
      if (this.m_currentToken.IsNot(JSToken.LeftParenthesis))
        this.ReportError(JSError.NoLeftParenthesis);
      else
        this.GetNextToken();
      AstNode expression = this.ParseExpression();
      if (this.m_currentToken.IsNot(JSToken.RightParenthesis))
      {
        context.UpdateWith(expression.Context);
        this.ReportError(JSError.NoRightParenthesis);
      }
      else
      {
        context.UpdateWith(this.m_currentToken);
        this.GetNextToken();
      }
      if (this.m_currentToken.IsNot(JSToken.LeftCurly))
        this.ReportError(JSError.StatementBlockExpected, this.CurrentPositionContext);
      AstNode statement = this.ParseStatement(false, true);
      return new WithNode(context)
      {
        WithObject = expression,
        Body = AstNode.ForceToBlock(statement)
      };
    }

    private AstNode ParseSwitchStatement()
    {
      Context context1 = this.m_currentToken.Clone();
      bool flag1 = false;
      Context context2 = (Context) null;
      this.GetNextToken();
      if (this.m_currentToken.IsNot(JSToken.LeftParenthesis))
        this.ReportError(JSError.NoLeftParenthesis);
      else
        this.GetNextToken();
      AstNode expression = this.ParseExpression();
      if (this.m_currentToken.IsNot(JSToken.RightParenthesis))
        this.ReportError(JSError.NoRightParenthesis);
      else
        this.GetNextToken();
      if (this.m_currentToken.IsNot(JSToken.LeftCurly))
      {
        this.ReportError(JSError.NoLeftCurly);
      }
      else
      {
        flag1 = this.m_foundEndOfLine;
        context2 = this.m_currentToken.Clone();
        this.GetNextToken();
      }
      AstNodeList astNodeList = new AstNodeList(this.CurrentPositionContext);
      bool flag2 = false;
      while (this.m_currentToken.IsNot(JSToken.RightCurly))
      {
        AstNode astNode = (AstNode) null;
        Context context3 = this.m_currentToken.Clone();
        Context context4 = (Context) null;
        if (this.m_currentToken.Is(JSToken.Case))
        {
          this.GetNextToken();
          astNode = this.ParseExpression();
        }
        else if (this.m_currentToken.Is(JSToken.Default))
        {
          if (flag2)
            this.ReportError(JSError.DupDefault);
          else
            flag2 = true;
          this.GetNextToken();
        }
        else
        {
          flag2 = true;
          this.ReportError(JSError.BadSwitch);
        }
        if (this.m_currentToken.IsNot(JSToken.Colon))
        {
          this.ReportError(JSError.NoColon);
        }
        else
        {
          context4 = this.m_currentToken.Clone();
          this.GetNextToken();
        }
        Block block = new Block(this.m_currentToken.Clone());
        while (true)
        {
          if (this.m_currentToken.IsNotAny(JSToken.RightCurly, JSToken.Case, JSToken.Default, JSToken.EndOfFile))
            block.Append(this.ParseStatement(false));
          else
            break;
        }
        context3.UpdateWith(block.Context);
        SwitchCase node = new SwitchCase(context3)
        {
          CaseValue = astNode,
          ColonContext = context4,
          Statements = block
        };
        astNodeList.Append((AstNode) node);
      }
      context1.UpdateWith(this.m_currentToken);
      this.GetNextToken();
      return (AstNode) new Switch(context1)
      {
        Expression = expression,
        BraceContext = context2,
        Cases = astNodeList,
        BraceOnNewLine = flag1
      };
    }

    private AstNode ParseThrowStatement()
    {
      ThrowNode node = new ThrowNode(this.m_currentToken.Clone());
      this.GetNextToken();
      if (!this.m_foundEndOfLine)
      {
        if (this.m_currentToken.IsNot(JSToken.Semicolon))
        {
          node.Operand = this.ParseExpression();
          if (node.Operand != null)
            node.UpdateWith(node.Operand.Context);
        }
        this.ExpectSemicolon((AstNode) node);
      }
      else
        this.ReportError(JSError.SemicolonInsertion, node.Context.FlattenToEnd());
      return (AstNode) node;
    }

    private AstNode ParseTryStatement()
    {
      Context context1 = this.m_currentToken.Clone();
      Context context2 = (Context) null;
      ParameterDeclaration parameterDeclaration = (ParameterDeclaration) null;
      Block block1 = (Block) null;
      Context context3 = (Context) null;
      Block block2 = (Block) null;
      bool flag = false;
      this.GetNextToken();
      if (this.m_currentToken.IsNot(JSToken.LeftCurly))
        this.ReportError(JSError.NoLeftCurly);
      Block block3 = this.ParseBlock();
      if (this.m_currentToken.Is(JSToken.Catch))
      {
        flag = true;
        context2 = this.m_currentToken.Clone();
        this.GetNextToken();
        if (this.m_currentToken.IsNot(JSToken.LeftParenthesis))
          this.ReportError(JSError.NoLeftParenthesis);
        else
          this.GetNextToken();
        AstNode binding = this.ParseBinding();
        if (binding == null)
          this.ReportError(JSError.NoBinding);
        else
          parameterDeclaration = new ParameterDeclaration(binding.Context.Clone())
          {
            Binding = binding
          };
        if (this.m_currentToken.IsNot(JSToken.RightParenthesis))
          this.ReportError(JSError.NoRightParenthesis);
        else
          this.GetNextToken();
        if (this.m_currentToken.IsNot(JSToken.LeftCurly))
          this.ReportError(JSError.NoLeftCurly);
        block1 = this.ParseBlock();
        context1.UpdateWith(block1.Context);
      }
      if (this.m_currentToken.Is(JSToken.Finally))
      {
        flag = true;
        context3 = this.m_currentToken.Clone();
        this.GetNextToken();
        if (this.m_currentToken.IsNot(JSToken.LeftCurly))
          this.ReportError(JSError.NoLeftCurly);
        block2 = this.ParseBlock();
        context1.UpdateWith(block2.Context);
      }
      if (!flag)
        this.ReportError(JSError.NoCatch);
      return (AstNode) new TryNode(context1)
      {
        TryBlock = block3,
        CatchContext = context2,
        CatchParameter = parameterDeclaration,
        CatchBlock = block1,
        FinallyContext = context3,
        FinallyBlock = block2
      };
    }

    private AstNode ParseModule()
    {
      this.ParsedVersion = ScriptVersion.EcmaScript6;
      Context context = this.m_currentToken.Clone();
      this.GetNextToken();
      string str = (string) null;
      Context other1 = (Context) null;
      Block block = (Block) null;
      BindingIdentifier bindingIdentifier = (BindingIdentifier) null;
      Context other2 = (Context) null;
      if (this.m_currentToken.Is(JSToken.StringLiteral))
      {
        if (this.m_foundEndOfLine)
          this.ReportError(JSError.NewLineNotAllowed, forceToError: true);
        str = this.m_scanner.StringLiteralValue;
        other1 = this.m_currentToken.Clone();
        context.UpdateWith(other1);
        this.GetNextToken();
        if (this.m_currentToken.IsNot(JSToken.LeftCurly))
        {
          this.ReportError(JSError.NoLeftCurly);
        }
        else
        {
          block = this.ParseBlock();
          if (block != null)
          {
            context.UpdateWith(block.Context);
            block.IsModule = true;
          }
        }
      }
      else if (this.m_currentToken.Is(JSToken.Identifier) || JSKeyword.CanBeIdentifier(this.m_currentToken.Token) != null)
      {
        bindingIdentifier = (BindingIdentifier) this.ParseBinding();
        context.UpdateWith(bindingIdentifier.Context);
        if (this.m_currentToken.Is("from"))
        {
          other2 = this.m_currentToken.Clone();
          context.UpdateWith(other2);
          this.GetNextToken();
        }
        else
          this.ReportError(JSError.NoExpectedFrom);
        if (this.m_currentToken.Is(JSToken.StringLiteral))
        {
          str = this.m_scanner.StringLiteralValue;
          other1 = this.m_currentToken.Clone();
          context.UpdateWith(other1);
          this.GetNextToken();
        }
        else
          this.ReportError(JSError.NoStringLiteral);
      }
      else
        this.ReportError(JSError.NoIdentifier);
      ModuleDeclaration node = new ModuleDeclaration(context)
      {
        ModuleName = str,
        ModuleContext = other1,
        Body = block,
        Binding = bindingIdentifier,
        FromContext = other2
      };
      if (bindingIdentifier != null)
        this.ExpectSemicolon((AstNode) node);
      return (AstNode) node;
    }

    private AstNode ParseExport()
    {
      this.ParsedVersion = ScriptVersion.EcmaScript6;
      ExportNode exportNode = new ExportNode(this.m_currentToken.Clone());
      exportNode.KeywordContext = this.m_currentToken.Clone();
      ExportNode node1 = exportNode;
      this.GetNextToken();
      if (this.m_currentToken.IsOne(JSToken.Var, JSToken.Const, JSToken.Let, JSToken.Function, JSToken.Class))
      {
        AstNode statement = this.ParseStatement(true, true);
        if (statement != null)
          node1.Append(statement);
        else
          this.ReportError(JSError.SyntaxError);
      }
      else if (this.m_currentToken.Is(JSToken.Default))
      {
        node1.IsDefault = true;
        node1.DefaultContext = this.m_currentToken.Clone();
        node1.Context.UpdateWith(this.m_currentToken);
        this.GetNextToken();
        AstNode expression = this.ParseExpression(true);
        if (expression != null)
          node1.Append(expression);
        else
          this.ReportError(JSError.ExpressionExpected);
        this.ExpectSemicolon((AstNode) node1);
      }
      else
      {
        if (this.m_currentToken.Is(JSToken.Identifier) || JSKeyword.CanBeIdentifier(this.m_currentToken.Token) != null)
        {
          Lookup node2 = new Lookup(this.m_currentToken.Clone())
          {
            Name = this.m_scanner.Identifier
          };
          node1.Append((AstNode) node2);
          this.GetNextToken();
        }
        else if (this.m_currentToken.Is(JSToken.Multiply))
        {
          node1.OpenContext = this.m_currentToken.Clone();
          node1.UpdateWith(node1.OpenContext);
          this.GetNextToken();
        }
        else if (this.m_currentToken.Is(JSToken.LeftCurly))
        {
          node1.OpenContext = this.m_currentToken.Clone();
          node1.UpdateWith(node1.OpenContext);
          do
          {
            this.GetNextToken();
            if (this.m_currentToken.IsNot(JSToken.RightCurly))
            {
              string str1 = (string) null;
              if (this.m_currentToken.Is(JSToken.Identifier) || (str1 = JSKeyword.CanBeIdentifier(this.m_currentToken.Token)) != null)
              {
                Context context = this.m_currentToken.Clone();
                Lookup lookup = new Lookup(this.m_currentToken.Clone())
                {
                  Name = str1 ?? this.m_scanner.Identifier
                };
                this.GetNextToken();
                Context other1 = (Context) null;
                Context other2 = (Context) null;
                string str2 = (string) null;
                if (this.m_currentToken.Is("as"))
                {
                  other1 = this.m_currentToken.Clone();
                  context.UpdateWith(other1);
                  this.GetNextToken();
                  str2 = this.m_scanner.Identifier;
                  if (str2 != null)
                  {
                    other2 = this.m_currentToken.Clone();
                    context.UpdateWith(other2);
                    this.GetNextToken();
                  }
                  else
                    this.ReportError(JSError.NoIdentifier);
                }
                ImportExportSpecifier node3 = new ImportExportSpecifier(context)
                {
                  LocalIdentifier = (AstNode) lookup,
                  AsContext = other1,
                  ExternalName = str2,
                  NameContext = other2
                };
                node1.Append((AstNode) node3);
                if (this.m_currentToken.Is(JSToken.Comma))
                  node3.TerminatingContext = this.m_currentToken.Clone();
              }
              else
                this.ReportError(JSError.NoIdentifier);
            }
          }
          while (this.m_currentToken.Is(JSToken.Comma));
          if (this.m_currentToken.Is(JSToken.RightCurly))
          {
            node1.CloseContext = this.m_currentToken.Clone();
            node1.UpdateWith(node1.CloseContext);
            this.GetNextToken();
          }
          else
            this.ReportError(JSError.NoRightCurly);
        }
        else
          this.ReportError(JSError.NoSpecifierSet);
        if (this.m_currentToken.Is("from"))
        {
          node1.FromContext = this.m_currentToken.Clone();
          node1.UpdateWith(node1.FromContext);
          this.GetNextToken();
          if (this.m_currentToken.Is(JSToken.StringLiteral))
          {
            node1.ModuleContext = this.m_currentToken.Clone();
            node1.UpdateWith(node1.ModuleContext);
            node1.ModuleName = this.m_scanner.StringLiteralValue;
            this.GetNextToken();
          }
          else
            this.ReportError(JSError.NoStringLiteral);
        }
        this.ExpectSemicolon((AstNode) node1);
      }
      return (AstNode) node1;
    }

    private AstNode ParseImport()
    {
      this.ParsedVersion = ScriptVersion.EcmaScript6;
      ImportNode importNode = new ImportNode(this.m_currentToken.Clone());
      importNode.KeywordContext = this.m_currentToken.Clone();
      ImportNode node1 = importNode;
      this.GetNextToken();
      if (this.m_currentToken.Is(JSToken.StringLiteral))
      {
        node1.ModuleName = this.m_scanner.StringLiteralValue;
        node1.ModuleContext = this.m_currentToken.Clone();
        this.GetNextToken();
      }
      else
      {
        if (this.m_currentToken.Is(JSToken.LeftCurly))
        {
          node1.OpenContext = this.m_currentToken.Clone();
          node1.UpdateWith(node1.OpenContext);
          do
          {
            this.GetNextToken();
            if (this.m_currentToken.IsNot(JSToken.RightCurly))
            {
              string str = this.m_scanner.Identifier;
              if (str != null)
              {
                Context context1 = this.m_currentToken.Clone();
                Context context2 = context1.Clone();
                this.GetNextToken();
                Context context3 = (Context) null;
                AstNode astNode = (AstNode) null;
                if (this.m_currentToken.Is("as"))
                {
                  context3 = this.m_currentToken.Clone();
                  this.GetNextToken();
                  if (this.m_currentToken.Is(JSToken.Identifier) || JSKeyword.CanBeIdentifier(this.m_currentToken.Token) != null)
                    astNode = this.ParseBinding();
                  else
                    this.ReportError(JSError.NoIdentifier);
                }
                else
                {
                  astNode = (AstNode) new BindingIdentifier(context1)
                  {
                    Name = str
                  };
                  str = (string) null;
                  context1 = (Context) null;
                }
                ImportExportSpecifier node2 = new ImportExportSpecifier(context2)
                {
                  ExternalName = str,
                  NameContext = context1,
                  AsContext = context3,
                  LocalIdentifier = astNode
                };
                node1.Append((AstNode) node2);
                if (this.m_currentToken.Is(JSToken.Comma))
                  node1.TerminatingContext = this.m_currentToken.Clone();
              }
              else
                this.ReportError(JSError.NoIdentifier);
            }
          }
          while (this.m_currentToken.Is(JSToken.Comma));
          if (this.m_currentToken.Is(JSToken.RightCurly))
          {
            node1.CloseContext = this.m_currentToken.Clone();
            node1.UpdateWith(node1.CloseContext);
            this.GetNextToken();
          }
          else
            this.ReportError(JSError.NoRightCurly);
        }
        else if (this.m_currentToken.Is(JSToken.Identifier) || JSKeyword.CanBeIdentifier(this.m_currentToken.Token) != null)
          node1.Append(this.ParseBinding());
        if (this.m_currentToken.Is("from"))
        {
          node1.FromContext = this.m_currentToken.Clone();
          node1.UpdateWith(node1.FromContext);
          this.GetNextToken();
        }
        else
          this.ReportError(JSError.NoExpectedFrom);
        if (this.m_currentToken.Is(JSToken.StringLiteral))
        {
          node1.ModuleName = this.m_scanner.StringLiteralValue;
          node1.ModuleContext = this.m_currentToken.Clone();
          node1.UpdateWith(node1.ModuleContext);
          this.GetNextToken();
        }
        else
          this.ReportError(JSError.NoStringLiteral);
      }
      this.ExpectSemicolon((AstNode) node1);
      return (AstNode) node1;
    }

    private FunctionObject ParseFunction(FunctionType functionType, Context fncCtx)
    {
      BindingIdentifier bindingIdentifier = (BindingIdentifier) null;
      Block body = (Block) null;
      bool flag1 = functionType == FunctionType.Expression;
      if (functionType != FunctionType.Method)
        this.GetNextToken();
      bool flag2 = this.m_currentToken.Is(JSToken.Multiply);
      if (flag2)
      {
        this.GetNextToken();
        this.ParsedVersion = ScriptVersion.EcmaScript6;
      }
      if (this.m_currentToken.Is(JSToken.Identifier))
      {
        bindingIdentifier = new BindingIdentifier(this.m_currentToken.Clone())
        {
          Name = this.m_scanner.Identifier
        };
        this.GetNextToken();
      }
      else
      {
        string str = JSKeyword.CanBeIdentifier(this.m_currentToken.Token);
        if (str != null)
        {
          bindingIdentifier = new BindingIdentifier(this.m_currentToken.Clone())
          {
            Name = str
          };
          this.GetNextToken();
        }
        else if (!flag1)
        {
          this.ReportError(JSError.NoIdentifier);
          if (this.m_currentToken.IsNot(JSToken.LeftParenthesis) && this.m_currentToken.IsNot(JSToken.LeftCurly))
          {
            string code = this.m_currentToken.Code;
            bindingIdentifier = new BindingIdentifier(this.CurrentPositionContext)
            {
              Name = code
            };
            this.GetNextToken();
          }
        }
      }
      if (this.m_currentToken.IsNot(JSToken.LeftParenthesis))
      {
        bool flag3 = false;
        while (this.m_currentToken.IsNot(JSToken.LeftParenthesis) && this.m_currentToken.IsNot(JSToken.LeftCurly) && this.m_currentToken.IsNot(JSToken.Semicolon) && this.m_currentToken.IsNot(JSToken.EndOfFile))
        {
          bindingIdentifier.Context.UpdateWith(this.m_currentToken);
          this.GetNextToken();
          flag3 = true;
        }
        if (flag3)
        {
          bindingIdentifier.Name = bindingIdentifier.Context.Code;
          bindingIdentifier.Context.HandleError(JSError.FunctionNameMustBeIdentifier);
        }
        else
          this.ReportError(JSError.NoLeftParenthesis);
      }
      AstNodeList formalParameters = this.ParseFormalParameters();
      fncCtx.UpdateWith(formalParameters.IfNotNull<AstNodeList, Context>((Func<AstNodeList, Context>) (p => p.Context)));
      if (this.m_currentToken.IsNot(JSToken.LeftCurly))
        this.ReportError(JSError.NoLeftCurly);
      try
      {
        body = new Block(this.m_currentToken.Clone());
        body.BraceOnNewLine = this.m_foundEndOfLine;
        this.GetNextToken();
        this.ParseFunctionBody(body);
        if (this.m_currentToken.Is(JSToken.RightCurly))
        {
          body.Context.UpdateWith(this.m_currentToken);
          this.GetNextToken();
        }
        else if (this.m_currentToken.Is(JSToken.EndOfFile))
        {
          fncCtx.HandleError(JSError.UnclosedFunction, true);
          this.ReportError(JSError.ErrorEndOfFile);
        }
        else
          this.ReportError(JSError.NoRightCurly);
        fncCtx.UpdateWith(body.Context);
      }
      catch (EndOfStreamException ex)
      {
        fncCtx.HandleError(JSError.UnclosedFunction, true);
      }
      return new FunctionObject(fncCtx)
      {
        FunctionType = functionType,
        Binding = bindingIdentifier,
        ParameterDeclarations = formalParameters,
        Body = body,
        IsGenerator = flag2
      };
    }

    private void ParseFunctionBody(Block body)
    {
      bool flag = true;
      while (this.m_currentToken.IsNot(JSToken.RightCurly) && this.m_currentToken.IsNot(JSToken.EndOfFile))
      {
        AstNode astNode = this.ParseStatement(true);
        if (flag)
        {
          if (astNode is ConstantWrapper constantWrapper && constantWrapper.PrimitiveType == PrimitiveType.String)
          {
            if (!(constantWrapper is DirectivePrologue))
            {
              DirectivePrologue directivePrologue = new DirectivePrologue(constantWrapper.Value.ToString(), constantWrapper.Context);
              directivePrologue.MayHaveIssues = constantWrapper.MayHaveIssues;
              astNode = (AstNode) directivePrologue;
            }
          }
          else if (!this.m_newModule)
            flag = false;
        }
        else if (this.m_newModule)
          flag = true;
        body.Append(astNode);
      }
      this.AppendImportantComments(body);
    }

    private AstNodeList ParseFormalParameters()
    {
      AstNodeList formalParameters = (AstNodeList) null;
      if (this.m_currentToken.Is(JSToken.LeftParenthesis))
      {
        formalParameters = new AstNodeList(this.m_currentToken.Clone());
        JSToken jsToken = JSToken.Comma;
        while (jsToken == JSToken.Comma)
        {
          ParameterDeclaration node = (ParameterDeclaration) null;
          this.GetNextToken();
          if (this.m_currentToken.IsNot(JSToken.RightParenthesis))
          {
            Context context = (Context) null;
            if (this.m_currentToken.Is(JSToken.RestSpread))
            {
              this.ParsedVersion = ScriptVersion.EcmaScript6;
              context = this.m_currentToken.Clone();
              this.GetNextToken();
            }
            AstNode binding = this.ParseBinding();
            if (binding != null)
            {
              node = new ParameterDeclaration(binding.Context.Clone())
              {
                Binding = binding,
                Position = formalParameters.Count,
                HasRest = context != null,
                RestContext = context
              };
              formalParameters.Append((AstNode) node);
            }
            else
              this.ReportError(JSError.NoBinding);
            if (this.m_currentToken.Is(JSToken.Assign))
            {
              this.ParsedVersion = ScriptVersion.EcmaScript6;
              node.IfNotNull<ParameterDeclaration, Context>((Func<ParameterDeclaration, Context>) (p => p.AssignContext = this.m_currentToken.Clone()));
              this.GetNextToken();
              AstNode initializer = this.ParseExpression(true);
              node.IfNotNull<ParameterDeclaration, AstNode>((Func<ParameterDeclaration, AstNode>) (p => p.Initializer = initializer));
            }
          }
          jsToken = this.m_currentToken.Token;
          switch (jsToken)
          {
            case JSToken.Comma:
              node.IfNotNull<ParameterDeclaration, Context>((Func<ParameterDeclaration, Context>) (p => p.TerminatingContext = this.m_currentToken.Clone()));
              continue;
            case JSToken.RightParenthesis:
              continue;
            default:
              this.ReportError(JSError.NoRightParenthesisOrComma);
              continue;
          }
        }
        if (this.m_currentToken.Is(JSToken.RightParenthesis))
        {
          formalParameters.UpdateWith(this.m_currentToken);
          this.GetNextToken();
        }
        else
          this.ReportError(JSError.NoRightParenthesis);
      }
      return formalParameters;
    }

    private ClassNode ParseClassNode(ClassType classType)
    {
      Context context1 = this.m_currentToken.Clone();
      Context context2 = context1.Clone();
      this.GetNextToken();
      AstNode astNode1 = (AstNode) null;
      if (this.m_currentToken.IsNot(JSToken.LeftCurly) && this.m_currentToken.IsNot(JSToken.Extends))
        astNode1 = this.ParseBinding();
      if (!(astNode1 is BindingIdentifier) && classType == ClassType.Declaration)
        this.ReportError(JSError.NoIdentifier, astNode1.IfNotNull<AstNode, Context>((Func<AstNode, Context>) (b => b.Context)));
      Context other1 = (Context) null;
      AstNode astNode2 = (AstNode) null;
      Context other2 = (Context) null;
      Context other3 = (Context) null;
      if (this.m_currentToken.Is(JSToken.Extends))
      {
        other1 = this.m_currentToken.Clone();
        context2.UpdateWith(other1);
        this.GetNextToken();
        astNode2 = this.ParseExpression(true);
        if (astNode2 != null)
          context2.UpdateWith(astNode2.Context);
        else
          this.ReportError(JSError.ExpressionExpected);
      }
      AstNodeList astNodeList = (AstNodeList) null;
      if (this.m_currentToken.Is(JSToken.LeftCurly))
      {
        other2 = this.m_currentToken.Clone();
        context2.UpdateWith(other2);
        this.GetNextToken();
        astNodeList = new AstNodeList(this.m_currentToken.FlattenToStart());
        while (this.m_currentToken.IsNot(JSToken.EndOfFile) && this.m_currentToken.IsNot(JSToken.RightCurly))
        {
          if (this.m_currentToken.Is(JSToken.Semicolon))
          {
            this.GetNextToken();
          }
          else
          {
            AstNode classElement = this.ParseClassElement();
            if (classElement != null)
            {
              astNodeList.Append(classElement);
              context2.UpdateWith(classElement.Context);
            }
            else
              this.ReportError(JSError.ClassElementExpected);
          }
        }
        if (this.m_currentToken.Is(JSToken.RightCurly))
        {
          other3 = this.m_currentToken.Clone();
          context2.UpdateWith(other3);
          this.GetNextToken();
        }
        else
          this.ReportError(JSError.NoRightCurly);
      }
      else
        this.ReportError(JSError.NoLeftCurly);
      return new ClassNode(context2)
      {
        ClassType = classType,
        ClassContext = context1,
        Binding = astNode1,
        ExtendsContext = other1,
        Heritage = astNode2,
        OpenBrace = other2,
        Elements = astNodeList,
        CloseBrace = other3
      };
    }

    private AstNode ParseClassElement()
    {
      Context context = this.m_currentToken.Is(JSToken.Static) ? this.m_currentToken.Clone() : (Context) null;
      if (context != null)
        this.GetNextToken();
      FunctionObject function = this.ParseFunction(this.m_currentToken.Is(JSToken.Get) ? FunctionType.Getter : (this.m_currentToken.Is(JSToken.Set) ? FunctionType.Setter : FunctionType.Method), this.m_currentToken.FlattenToStart());
      if (function != null && context != null)
      {
        function.IsStatic = true;
        function.StaticContext = context;
      }
      return (AstNode) function;
    }

    private AstNode ParseExpression(bool single = false, JSToken inToken = JSToken.None)
    {
      bool isLeftHandSideExpr;
      return this.ParseExpression(this.ParseUnaryExpression(out isLeftHandSideExpr, false), single, isLeftHandSideExpr, inToken);
    }

    private AstNode ParseExpression(
      AstNode leftHandSide,
      bool single,
      bool bCanAssign,
      JSToken inToken)
    {
      Stack<Context> contextStack = new Stack<Context>();
      contextStack.Push((Context) null);
      Stack<AstNode> astNodeStack = new Stack<AstNode>();
      astNodeStack.Push(leftHandSide);
      while (JSScanner.IsProcessableOperator(this.m_currentToken.Token) && this.m_currentToken.IsNot(inToken) && (!single || this.m_currentToken.IsNot(JSToken.Comma)))
      {
        OperatorPrecedence operatorPrecedence1 = JSScanner.GetOperatorPrecedence(this.m_currentToken);
        bool flag = JSScanner.IsRightAssociativeOperator(this.m_currentToken.Token);
        for (OperatorPrecedence operatorPrecedence2 = JSScanner.GetOperatorPrecedence(contextStack.Peek()); operatorPrecedence1 < operatorPrecedence2 || operatorPrecedence1 == operatorPrecedence2 && !flag; operatorPrecedence2 = JSScanner.GetOperatorPrecedence(contextStack.Peek()))
        {
          AstNode operand2 = astNodeStack.Pop();
          AstNode operand1 = astNodeStack.Pop();
          AstNode expressionNode = JSParser.CreateExpressionNode(contextStack.Pop(), operand1, operand2);
          astNodeStack.Push(expressionNode);
        }
        if (this.m_currentToken.Is(JSToken.ConditionalIf))
        {
          AstNode astNode1 = astNodeStack.Pop();
          if (astNode1 is BinaryOperator binaryOperator && binaryOperator.OperatorToken == JSToken.Assign)
            astNode1.Context.HandleError(JSError.SuspectAssignment);
          Context context1 = this.m_currentToken.Clone();
          this.GetNextToken();
          AstNode expression1 = this.ParseExpression(true);
          Context context2 = (Context) null;
          if (this.m_currentToken.IsNot(JSToken.Colon))
            this.ReportError(JSError.NoColon);
          else
            context2 = this.m_currentToken.Clone();
          this.GetNextToken();
          AstNode expression2 = this.ParseExpression(true, inToken);
          AstNode astNode2 = (AstNode) new Conditional(astNode1.Context.CombineWith(expression2.Context))
          {
            Condition = astNode1,
            QuestionContext = context1,
            TrueExpression = expression1,
            ColonContext = context2,
            FalseExpression = expression2
          };
          astNodeStack.Push(astNode2);
        }
        else
        {
          if (JSScanner.IsAssignmentOperator(this.m_currentToken.Token))
          {
            if (!bCanAssign)
              this.ReportError(JSError.IllegalAssignment);
          }
          else
            bCanAssign = this.m_currentToken.Is(JSToken.Comma);
          contextStack.Push(this.m_currentToken.Clone());
          this.GetNextToken();
          if (bCanAssign)
            astNodeStack.Push(this.ParseUnaryExpression(out bCanAssign, false));
          else
            astNodeStack.Push(this.ParseUnaryExpression(out bool _, false));
        }
      }
      while (contextStack.Peek() != null)
      {
        AstNode operand2 = astNodeStack.Pop();
        AstNode operand1 = astNodeStack.Pop();
        AstNode expressionNode = JSParser.CreateExpressionNode(contextStack.Pop(), operand1, operand2);
        astNodeStack.Push(expressionNode);
      }
      AstNode expression3 = astNodeStack.Pop();
      if (expression3 != null && expression3.Context.Token == JSToken.Yield && expression3 is Lookup)
      {
        AstNode expression4 = this.ParseExpression(true);
        if (expression4 != null)
          expression3 = (AstNode) new UnaryOperator(expression3.Context.CombineWith(expression4.Context))
          {
            OperatorToken = JSToken.Yield,
            OperatorContext = expression3.Context,
            Operand = expression4
          };
      }
      return expression3;
    }

    private AstNode ParseUnaryExpression(out bool isLeftHandSideExpr, bool isMinus)
    {
      isLeftHandSideExpr = false;
      bool isLeftHandSideExpr1 = false;
      JSToken token;
      AstNode unaryExpression1;
      Context context1;
      Context context2;
      Context context3;
      while (true)
      {
        token = this.m_currentToken.Token;
        switch (token)
        {
          case JSToken.ConditionalCommentStart:
            context1 = this.m_currentToken.Clone();
            this.GetNextToken();
            if (this.m_currentToken.Is(JSToken.ConditionalCommentEnd))
            {
              this.GetNextToken();
              continue;
            }
            if (this.m_currentToken.Is(JSToken.ConditionalCompilationOn))
            {
              this.GetNextToken();
              if (this.m_currentToken.Is(JSToken.ConditionalCompilationVariable))
              {
                unaryExpression1 = (AstNode) new ConstantWrapperPP(this.m_currentToken.Clone())
                {
                  VarName = this.m_currentToken.Code,
                  ForceComments = true
                };
                this.GetNextToken();
                if (!this.m_currentToken.Is(JSToken.ConditionalCommentEnd))
                {
                  this.CCTooComplicated((Context) null);
                  continue;
                }
                goto label_9;
              }
              else if (this.m_currentToken.Is(JSToken.LogicalNot))
              {
                context2 = this.m_currentToken.Clone();
                this.GetNextToken();
                if (!this.m_currentToken.Is(JSToken.ConditionalCommentEnd))
                {
                  this.CCTooComplicated((Context) null);
                  continue;
                }
                goto label_13;
              }
              else
              {
                this.CCTooComplicated((Context) null);
                continue;
              }
            }
            else if (this.m_currentToken.Is(JSToken.LogicalNot))
            {
              context3 = this.m_currentToken.Clone();
              this.GetNextToken();
              if (!this.m_currentToken.Is(JSToken.ConditionalCommentEnd))
              {
                this.CCTooComplicated((Context) null);
                continue;
              }
              goto label_18;
            }
            else if (this.m_currentToken.Is(JSToken.ConditionalCompilationVariable))
            {
              unaryExpression1 = (AstNode) new ConstantWrapperPP(this.m_currentToken.Clone())
              {
                VarName = this.m_currentToken.Code,
                ForceComments = true
              };
              this.GetNextToken();
              if (!this.m_currentToken.Is(JSToken.ConditionalCommentEnd))
              {
                this.CCTooComplicated((Context) null);
                continue;
              }
              goto label_22;
            }
            else
            {
              this.CCTooComplicated((Context) null);
              continue;
            }
          case JSToken.RestSpread:
            goto label_2;
          case JSToken.FirstOperator:
          case JSToken.Increment:
          case JSToken.Decrement:
          case JSToken.Void:
          case JSToken.TypeOf:
          case JSToken.LogicalNot:
          case JSToken.BitwiseNot:
          case JSToken.FirstBinaryOperator:
          case JSToken.Minus:
            goto label_3;
          default:
            goto label_25;
        }
      }
label_2:
      this.ParsedVersion = ScriptVersion.EcmaScript6;
label_3:
      Context context4 = this.m_currentToken.Clone();
      this.GetNextToken();
      AstNode unaryExpression2 = this.ParseUnaryExpression(out isLeftHandSideExpr1, false);
      unaryExpression1 = (AstNode) new UnaryOperator(context4.CombineWith(unaryExpression2.Context))
      {
        Operand = unaryExpression2,
        OperatorContext = context4,
        OperatorToken = token
      };
      goto label_26;
label_9:
      this.GetNextToken();
      goto label_26;
label_13:
      this.GetNextToken();
      AstNode unaryExpression3 = this.ParseUnaryExpression(out isLeftHandSideExpr1, false);
      context1.UpdateWith(unaryExpression3.Context);
      UnaryOperator unaryOperator1 = new UnaryOperator(context1)
      {
        Operand = unaryExpression3,
        OperatorContext = context2,
        OperatorToken = JSToken.LogicalNot
      };
      unaryOperator1.OperatorInConditionalCompilationComment = true;
      unaryOperator1.ConditionalCommentContainsOn = true;
      unaryExpression1 = (AstNode) unaryOperator1;
      goto label_26;
label_18:
      this.GetNextToken();
      AstNode unaryExpression4 = this.ParseUnaryExpression(out isLeftHandSideExpr1, false);
      context1.UpdateWith(unaryExpression4.Context);
      UnaryOperator unaryOperator2 = new UnaryOperator(context1)
      {
        Operand = unaryExpression4,
        OperatorContext = context3,
        OperatorToken = JSToken.LogicalNot
      };
      unaryOperator2.OperatorInConditionalCompilationComment = true;
      unaryExpression1 = (AstNode) unaryOperator2;
      goto label_26;
label_22:
      this.GetNextToken();
      goto label_26;
label_25:
      unaryExpression1 = this.ParsePostfixExpression(this.ParseLeftHandSideExpression(isMinus), out isLeftHandSideExpr);
label_26:
      return unaryExpression1;
    }

    private AstNode ParsePostfixExpression(AstNode ast, out bool isLeftHandSideExpr)
    {
      isLeftHandSideExpr = true;
      if (ast != null && !this.m_foundEndOfLine)
      {
        if (this.m_currentToken.Is(JSToken.Increment))
        {
          isLeftHandSideExpr = false;
          Context context = ast.Context.Clone();
          context.UpdateWith(this.m_currentToken);
          ast = (AstNode) new UnaryOperator(context)
          {
            Operand = ast,
            OperatorToken = this.m_currentToken.Token,
            OperatorContext = this.m_currentToken.Clone(),
            IsPostfix = true
          };
          this.GetNextToken();
        }
        else if (this.m_currentToken.Is(JSToken.Decrement))
        {
          isLeftHandSideExpr = false;
          Context context = ast.Context.Clone();
          context.UpdateWith(this.m_currentToken);
          ast = (AstNode) new UnaryOperator(context)
          {
            Operand = ast,
            OperatorToken = this.m_currentToken.Token,
            OperatorContext = this.m_currentToken.Clone(),
            IsPostfix = true
          };
          this.GetNextToken();
        }
      }
      return ast;
    }

    private AstNode ParseLeftHandSideExpression(bool isMinus)
    {
      AstNode astNode = (AstNode) null;
      List<Context> newContexts = (List<Context>) null;
      JSToken token;
      while (true)
      {
        while (this.m_currentToken.Is(JSToken.New))
        {
          if (newContexts == null)
            newContexts = new List<Context>(4);
          newContexts.Add(this.m_currentToken.Clone());
          this.GetNextToken();
        }
        token = this.m_currentToken.Token;
        switch (token)
        {
          case JSToken.LeftCurly:
            goto label_54;
          case JSToken.Function:
            goto label_55;
          case JSToken.ConditionalCommentStart:
            this.GetNextToken();
            if (this.m_currentToken.Is(JSToken.ConditionalCompilationVariable))
            {
              astNode = (AstNode) new ConstantWrapperPP(this.m_currentToken.Clone())
              {
                VarName = this.m_currentToken.Code,
                ForceComments = true
              };
              this.GetNextToken();
              if (!this.m_currentToken.Is(JSToken.ConditionalCommentEnd))
              {
                this.CCTooComplicated((Context) null);
                continue;
              }
              goto label_10;
            }
            else
            {
              if (this.m_currentToken.Is(JSToken.ConditionalCommentEnd))
              {
                this.GetNextToken();
                continue;
              }
              this.m_currentToken.HandleError(JSError.ConditionalCompilationTooComplex);
              while (this.m_currentToken.IsNot(JSToken.EndOfFile) && this.m_currentToken.IsNot(JSToken.ConditionalCommentEnd))
                this.GetNextToken();
              this.GetNextToken();
              continue;
            }
          case JSToken.ConditionalCompilationVariable:
            goto label_33;
          case JSToken.Identifier:
            goto label_6;
          case JSToken.Null:
            goto label_32;
          case JSToken.True:
            goto label_30;
          case JSToken.False:
            goto label_31;
          case JSToken.This:
            goto label_18;
          case JSToken.StringLiteral:
            goto label_19;
          case JSToken.IntegerLiteral:
          case JSToken.NumericLiteral:
            goto label_20;
          case JSToken.TemplateLiteral:
            goto label_7;
          case JSToken.LeftParenthesis:
            goto label_36;
          case JSToken.LeftBracket:
            goto label_53;
          case JSToken.Divide:
          case JSToken.DivideAssign:
            goto label_34;
          case JSToken.Modulo:
            goto label_35;
          case JSToken.Class:
            goto label_56;
          case JSToken.Yield:
            goto label_58;
          case JSToken.AspNetBlock:
            goto label_57;
          default:
            goto label_61;
        }
      }
label_6:
      astNode = (AstNode) new Lookup(this.m_currentToken.Clone())
      {
        Name = this.m_scanner.Identifier
      };
      this.GetNextToken();
      goto label_64;
label_7:
      astNode = (AstNode) this.ParseTemplateLiteral();
      goto label_64;
label_10:
      this.GetNextToken();
      goto label_64;
label_18:
      astNode = (AstNode) new ThisLiteral(this.m_currentToken.Clone());
      this.GetNextToken();
      goto label_64;
label_19:
      astNode = (AstNode) new ConstantWrapper((object) this.m_scanner.StringLiteralValue, PrimitiveType.String, this.m_currentToken.Clone())
      {
        MayHaveIssues = this.m_scanner.LiteralHasIssues
      };
      this.GetNextToken();
      goto label_64;
label_20:
      Context context1 = this.m_currentToken.Clone();
      double doubleValue;
      if (this.ConvertNumericLiteralToDouble(this.m_currentToken.Code, token == JSToken.IntegerLiteral, out doubleValue))
      {
        bool literalHasIssues = this.m_scanner.LiteralHasIssues;
        if (doubleValue == double.MaxValue)
          this.ReportError(JSError.NumericMaximum, context1);
        else if (isMinus && -doubleValue == double.MinValue)
          this.ReportError(JSError.NumericMinimum, context1);
        astNode = (AstNode) new ConstantWrapper((object) doubleValue, PrimitiveType.Number, context1)
        {
          MayHaveIssues = literalHasIssues
        };
      }
      else
      {
        if (double.IsInfinity(doubleValue))
          this.ReportError(JSError.NumericOverflow, context1);
        astNode = (AstNode) new ConstantWrapper((object) this.m_currentToken.Code, PrimitiveType.Other, context1)
        {
          MayHaveIssues = true
        };
      }
      this.GetNextToken();
      goto label_64;
label_30:
      astNode = (AstNode) new ConstantWrapper((object) true, PrimitiveType.Boolean, this.m_currentToken.Clone());
      this.GetNextToken();
      goto label_64;
label_31:
      astNode = (AstNode) new ConstantWrapper((object) false, PrimitiveType.Boolean, this.m_currentToken.Clone());
      this.GetNextToken();
      goto label_64;
label_32:
      astNode = (AstNode) new ConstantWrapper((object) null, PrimitiveType.Null, this.m_currentToken.Clone());
      this.GetNextToken();
      goto label_64;
label_33:
      astNode = (AstNode) new ConstantWrapperPP(this.m_currentToken.Clone())
      {
        VarName = this.m_currentToken.Code,
        ForceComments = false
      };
      this.GetNextToken();
      goto label_64;
label_34:
      astNode = (AstNode) this.ScanRegularExpression();
      if (astNode == null)
        goto label_61;
      else
        goto label_64;
label_35:
      astNode = (AstNode) this.ScanReplacementToken();
      if (astNode == null)
        goto label_61;
      else
        goto label_64;
label_36:
      Context context2 = this.m_currentToken.Clone();
      this.GetNextToken();
      if (this.m_currentToken.Is(JSToken.For))
      {
        astNode = (AstNode) this.ParseComprehension(false, context2, (AstNode) null);
        goto label_64;
      }
      else if (this.m_currentToken.Is(JSToken.RightParenthesis))
      {
        astNode = (AstNode) new GroupingOperator(context2);
        astNode.UpdateWith(this.m_currentToken);
        this.GetNextToken();
        goto label_64;
      }
      else if (this.m_currentToken.Is(JSToken.RestSpread))
      {
        Context context3 = this.m_currentToken.Clone();
        this.GetNextToken();
        astNode = this.ParseExpression(true);
        if (astNode != null)
          astNode = (AstNode) new UnaryOperator(context3.CombineWith(astNode.Context))
          {
            OperatorContext = context3,
            OperatorToken = JSToken.RestSpread,
            Operand = astNode
          };
        if (this.m_currentToken.Is(JSToken.Comma))
          astNode = this.ParseExpression(astNode, false, true, JSToken.None);
        if (this.m_currentToken.Is(JSToken.RightParenthesis))
        {
          astNode = (AstNode) new GroupingOperator(context2)
          {
            Operand = astNode
          };
          astNode.UpdateWith(this.m_currentToken);
          this.GetNextToken();
          goto label_64;
        }
        else
        {
          this.ReportError(JSError.NoRightParenthesis);
          goto label_64;
        }
      }
      else
      {
        AstNode expression = this.ParseExpression();
        if (this.m_currentToken.Is(JSToken.For))
        {
          astNode = (AstNode) this.ParseComprehension(false, context2, expression);
          goto label_64;
        }
        else
        {
          astNode = (AstNode) new GroupingOperator(context2)
          {
            Operand = expression
          };
          astNode.UpdateWith(expression.Context);
          if (this.m_currentToken.IsNot(JSToken.RightParenthesis))
          {
            this.ReportError(JSError.NoRightParenthesis);
            goto label_64;
          }
          else
          {
            astNode.UpdateWith(this.m_currentToken);
            this.GetNextToken();
            goto label_64;
          }
        }
      }
label_53:
      astNode = this.ParseArrayLiteral(false);
      goto label_64;
label_54:
      astNode = (AstNode) this.ParseObjectLiteral(false);
      goto label_64;
label_55:
      astNode = (AstNode) this.ParseFunction(FunctionType.Expression, this.m_currentToken.Clone());
      goto label_64;
label_56:
      astNode = (AstNode) this.ParseClassNode(ClassType.Expression);
      goto label_64;
label_57:
      astNode = (AstNode) new AspNetBlockNode(this.m_currentToken.Clone())
      {
        AspNetBlockText = this.m_currentToken.Code
      };
      this.GetNextToken();
      goto label_64;
label_58:
      if (this.ParsedVersion == ScriptVersion.EcmaScript6 || this.m_settings.ScriptVersion == ScriptVersion.EcmaScript6)
      {
        astNode = this.ParseYieldExpression();
        goto label_64;
      }
      else
      {
        astNode = (AstNode) new Lookup(this.m_currentToken.Clone())
        {
          Name = "yield"
        };
        this.GetNextToken();
        goto label_64;
      }
label_61:
      string str = JSKeyword.CanBeIdentifier(this.m_currentToken.Token);
      if (str != null)
      {
        astNode = (AstNode) new Lookup(this.m_currentToken.Clone())
        {
          Name = str
        };
        this.GetNextToken();
      }
      else
        this.ReportError(JSError.ExpressionExpected);
label_64:
      if (this.m_currentToken.Is(JSToken.ArrowFunction))
      {
        this.ParsedVersion = ScriptVersion.EcmaScript6;
        astNode = (AstNode) this.ParseArrowFunction(astNode);
      }
      return this.ParseMemberExpression(astNode, newContexts);
    }

    private RegExpLiteral ScanRegularExpression()
    {
      RegExpLiteral regExpLiteral = (RegExpLiteral) null;
      this.m_currentToken = this.m_scanner.UpdateToken(UpdateHint.RegularExpression);
      if (this.m_currentToken.Is(JSToken.RegularExpression))
      {
        Context context = this.m_currentToken.Clone();
        string code = this.m_currentToken.Code;
        string str1 = code.Substring(1, code.Length - 2);
        this.GetNextToken();
        string str2 = (string) null;
        if (this.m_currentToken.Is(JSToken.Identifier))
        {
          context.UpdateWith(this.m_currentToken);
          str2 = this.m_scanner.Identifier;
          this.GetNextToken();
        }
        regExpLiteral = new RegExpLiteral(this.m_currentToken.Clone())
        {
          Pattern = str1,
          PatternSwitches = str2
        };
      }
      return regExpLiteral;
    }

    private ConstantWrapper ScanReplacementToken()
    {
      ConstantWrapper constantWrapper = (ConstantWrapper) null;
      this.m_currentToken = this.m_scanner.UpdateToken(UpdateHint.ReplacementToken);
      if (this.m_currentToken.Is(JSToken.ReplacementToken))
      {
        constantWrapper = new ConstantWrapper((object) this.m_currentToken.Code, PrimitiveType.Other, this.m_currentToken.Clone());
        this.GetNextToken();
      }
      return constantWrapper;
    }

    private TemplateLiteral ParseTemplateLiteral()
    {
      this.ParsedVersion = ScriptVersion.EcmaScript6;
      Context context1 = this.m_currentToken.Clone();
      Context context2 = this.m_currentToken.Clone();
      Lookup lookup = (Lookup) null;
      string str1 = this.m_scanner.StringLiteralValue;
      int num = str1.IndexOf('`');
      if (num != 0)
      {
        string str2 = str1.Substring(0, num);
        str1 = str1.Substring(num);
        lookup = new Lookup(context2.SplitStart(num))
        {
          Name = str2
        };
      }
      bool flag1 = str1[str1.Length - 1] != '`';
      TemplateLiteral templateLiteral = new TemplateLiteral(context1)
      {
        Function = lookup,
        Text = str1,
        TextContext = context2,
        Expressions = flag1 ? new AstNodeList(context1.FlattenToEnd()) : (AstNodeList) null
      };
      this.GetNextToken();
      if (flag1)
      {
        bool flag2;
        do
        {
          flag2 = false;
          AstNode expression = this.ParseExpression();
          if (this.m_currentToken.Is(JSToken.RightCurly))
          {
            this.m_scanner.UpdateToken(UpdateHint.TemplateLiteral);
            if (this.m_currentToken.Is(JSToken.TemplateLiteral))
            {
              string stringLiteralValue = this.m_scanner.StringLiteralValue;
              TemplateLiteralExpression node = new TemplateLiteralExpression(expression.Context.Clone())
              {
                Expression = expression,
                Text = stringLiteralValue
              };
              templateLiteral.UpdateWith(node.Context);
              templateLiteral.Expressions.Append((AstNode) node);
              this.GetNextToken();
              flag2 = stringLiteralValue[stringLiteralValue.Length - 1] != '`';
            }
          }
          else
            this.ReportError(JSError.NoRightCurly);
        }
        while (flag2);
      }
      return templateLiteral;
    }

    private AstNode ParseYieldExpression()
    {
      this.ParsedVersion = ScriptVersion.EcmaScript6;
      Context context1 = this.m_currentToken.Clone();
      Context context2 = context1.Clone();
      this.GetNextToken();
      bool flag = this.m_currentToken.Is(JSToken.Multiply);
      if (flag)
        this.GetNextToken();
      AstNode expression = this.ParseExpression(true);
      if (expression == null)
        this.ReportError(JSError.ExpressionExpected);
      else
        context1.UpdateWith(expression.Context);
      return (AstNode) new UnaryOperator(context1)
      {
        OperatorContext = context2,
        OperatorToken = JSToken.Yield,
        Operand = expression,
        IsDelegator = flag
      };
    }

    private FunctionObject ParseArrowFunction(AstNode parameters)
    {
      Context context = this.m_currentToken.Clone();
      this.GetNextToken();
      this.ParsedVersion = ScriptVersion.EcmaScript6;
      FunctionObject functionObject = new FunctionObject(parameters.Context.Clone())
      {
        ParameterDeclarations = BindingTransform.ToParameters(parameters),
        FunctionType = FunctionType.ArrowFunction
      };
      functionObject.UpdateWith(context);
      if (this.m_currentToken.Is(JSToken.LeftCurly))
      {
        functionObject.Body = this.ParseBlock();
      }
      else
      {
        functionObject.Body = AstNode.ForceToBlock(this.ParseExpression(true));
        functionObject.Body.IsConcise = true;
      }
      functionObject.Body.IfNotNull<Block>((Action<Block>) (b => functionObject.UpdateWith(b.Context)));
      return functionObject;
    }

    private AstNode ParseArrayLiteral(bool isBindingPattern)
    {
      Context openDelimiter = this.m_currentToken.Clone();
      Context context1 = openDelimiter.Clone();
      AstNodeList astNodeList = new AstNodeList(this.CurrentPositionContext);
      bool flag = false;
      Context commaContext = (Context) null;
      do
      {
        this.GetNextToken();
        AstNode astNode = (AstNode) null;
        if (this.m_currentToken.Is(JSToken.Comma))
          astNode = (AstNode) new ConstantWrapper((object) Missing.Value, PrimitiveType.Other, this.m_currentToken.FlattenToStart());
        else if (this.m_currentToken.Is(JSToken.RightBracket))
        {
          if (astNodeList.Count != 0)
          {
            if (!isBindingPattern)
            {
              flag = true;
              astNode = (AstNode) new ConstantWrapper((object) Missing.Value, PrimitiveType.Other, this.m_currentToken.FlattenToStart());
              commaContext.HandleError(JSError.ArrayLiteralTrailingComma);
            }
          }
          else
            break;
        }
        else
        {
          if (this.m_currentToken.Is(JSToken.For))
            return (AstNode) this.ParseComprehension(true, openDelimiter, (AstNode) null);
          Context context2 = (Context) null;
          if (this.m_currentToken.Is(JSToken.RestSpread))
          {
            this.ParsedVersion = ScriptVersion.EcmaScript6;
            context2 = this.m_currentToken.Clone();
            this.GetNextToken();
          }
          if (isBindingPattern)
          {
            astNode = this.ParseBinding();
            if (this.m_currentToken.Is(JSToken.Assign))
            {
              Context context3 = this.m_currentToken.Clone();
              this.GetNextToken();
              astNode = (AstNode) new InitializerNode(context3.Clone())
              {
                Binding = astNode,
                AssignContext = context3,
                Initializer = this.ParseExpression(true)
              };
            }
          }
          else
            astNode = this.ParseExpression(true);
          if (context2 != null)
            astNode = (AstNode) new UnaryOperator(context2.CombineWith(astNode.Context))
            {
              Operand = astNode,
              OperatorToken = JSToken.RestSpread,
              OperatorContext = context2
            };
        }
        if (this.m_currentToken.Is(JSToken.For))
          return (AstNode) this.ParseComprehension(true, openDelimiter, astNode);
        astNodeList.Append(astNode);
        if (this.m_currentToken.Is(JSToken.Comma))
        {
          commaContext = this.m_currentToken.Clone();
          astNode.IfNotNull<AstNode, Context>((Func<AstNode, Context>) (e => e.TerminatingContext = commaContext));
        }
      }
      while (this.m_currentToken.Is(JSToken.Comma));
      if (this.m_currentToken.Is(JSToken.RightBracket))
      {
        context1.UpdateWith(this.m_currentToken);
        this.GetNextToken();
      }
      else
        this.m_currentToken.HandleError(JSError.NoRightBracketOrComma, true);
      return (AstNode) new ArrayLiteral(context1)
      {
        Elements = astNodeList,
        MayHaveIssues = flag
      };
    }

    private ComprehensionNode ParseComprehension(
      bool isArray,
      Context openDelimiter,
      AstNode expression)
    {
      bool flag = expression != null;
      Context context = openDelimiter.Clone();
      Context other = (Context) null;
      expression.IfNotNull<AstNode, Context>((Func<AstNode, Context>) (e => context.UpdateWith(e.Context)));
      AstNodeList astNodeList = new AstNodeList(this.m_currentToken.Clone());
      Context currentToken;
      JSToken[] jsTokenArray;
      do
      {
        if (this.m_currentToken.IsOne(JSToken.For, JSToken.If))
        {
          ComprehensionClause comprehensionClause = this.ParseComprehensionClause();
          comprehensionClause.IfNotNull<ComprehensionClause, Context>((Func<ComprehensionClause, Context>) (c => context.UpdateWith(c.Context)));
          astNodeList.Append((AstNode) comprehensionClause);
        }
        else
          this.ReportError(JSError.NoForOrIf);
        currentToken = this.m_currentToken;
        jsTokenArray = new JSToken[2]
        {
          JSToken.For,
          JSToken.If
        };
      }
      while (currentToken.IsOne(jsTokenArray));
      context.UpdateWith(astNodeList.Context);
      if (expression == null)
      {
        expression = this.ParseExpression(true);
        expression.IfNotNull<AstNode, Context>((Func<AstNode, Context>) (e => context.UpdateWith(e.Context)));
      }
      if (this.m_currentToken.IsNot(isArray ? JSToken.RightBracket : JSToken.RightParenthesis))
      {
        this.ReportError(isArray ? JSError.NoRightBracket : JSError.NoRightParenthesis);
      }
      else
      {
        other = this.m_currentToken.Clone();
        context.UpdateWith(other);
        this.GetNextToken();
      }
      this.ParsedVersion = ScriptVersion.EcmaScript6;
      return new ComprehensionNode(context)
      {
        OpenDelimiter = openDelimiter,
        Expression = expression,
        Clauses = astNodeList,
        CloseDelimiter = other,
        ComprehensionType = isArray ? ComprehensionType.Array : ComprehensionType.Generator,
        MozillaOrdering = flag
      };
    }

    private ComprehensionClause ParseComprehensionClause()
    {
      Context context = this.m_currentToken.Clone();
      Context clauseContext = context.Clone();
      this.GetNextToken();
      Context other1 = (Context) null;
      if (this.m_currentToken.IsNot(JSToken.LeftParenthesis))
      {
        this.ReportError(JSError.NoLeftParenthesis, context);
      }
      else
      {
        other1 = this.m_currentToken.Clone();
        clauseContext.UpdateWith(other1);
        this.GetNextToken();
      }
      AstNode astNode = (AstNode) null;
      Context other2 = (Context) null;
      bool flag = false;
      AstNode expression;
      if (context.Is(JSToken.For))
      {
        astNode = this.ParseBinding();
        astNode.IfNotNull<AstNode, Context>((Func<AstNode, Context>) (b => clauseContext.UpdateWith(b.Context)));
        if (this.m_currentToken.Is(JSToken.In) || this.m_currentToken.Is("of"))
        {
          flag = this.m_currentToken.Is(JSToken.In);
          other2 = this.m_currentToken.Clone();
          this.GetNextToken();
          clauseContext.UpdateWith(other2);
        }
        else
          this.ReportError(JSError.NoForOrIf);
        expression = this.ParseExpression(true);
        expression.IfNotNull<AstNode, Context>((Func<AstNode, Context>) (e => clauseContext.UpdateWith(e.Context)));
      }
      else
      {
        expression = this.ParseExpression(true);
        expression.IfNotNull<AstNode, Context>((Func<AstNode, Context>) (e => clauseContext.UpdateWith(e.Context)));
      }
      Context other3 = (Context) null;
      if (this.m_currentToken.IsNot(JSToken.RightParenthesis))
      {
        this.ReportError(JSError.NoRightParenthesis);
      }
      else
      {
        other3 = this.m_currentToken.Clone();
        clauseContext.UpdateWith(other3);
        this.GetNextToken();
      }
      if (context.Is(JSToken.For))
      {
        ComprehensionForClause comprehensionClause = new ComprehensionForClause(clauseContext);
        comprehensionClause.OperatorContext = context;
        comprehensionClause.OpenContext = other1;
        comprehensionClause.Binding = astNode;
        comprehensionClause.IsInOperation = flag;
        comprehensionClause.OfContext = other2;
        comprehensionClause.Expression = expression;
        comprehensionClause.CloseContext = other3;
        return (ComprehensionClause) comprehensionClause;
      }
      ComprehensionIfClause comprehensionClause1 = new ComprehensionIfClause(clauseContext);
      comprehensionClause1.OperatorContext = context;
      comprehensionClause1.OpenContext = other1;
      comprehensionClause1.Condition = expression;
      comprehensionClause1.CloseContext = other3;
      return (ComprehensionClause) comprehensionClause1;
    }

    private ObjectLiteral ParseObjectLiteral(bool isBindingPattern)
    {
      Context context = this.m_currentToken.Clone();
      AstNodeList astNodeList = new AstNodeList(this.CurrentPositionContext);
      do
      {
        this.GetNextToken();
        if (this.m_currentToken.IsNot(JSToken.RightCurly))
        {
          ObjectLiteralProperty objectLiteralProperty = this.ParseObjectLiteralProperty(isBindingPattern);
          astNodeList.Append((AstNode) objectLiteralProperty);
        }
      }
      while (this.m_currentToken.Is(JSToken.Comma));
      if (this.m_currentToken.Is(JSToken.RightCurly))
      {
        context.UpdateWith(this.m_currentToken);
        this.GetNextToken();
      }
      else
        this.ReportError(JSError.NoRightCurly);
      return new ObjectLiteral(context)
      {
        Properties = astNodeList
      };
    }

    private ObjectLiteralProperty ParseObjectLiteralProperty(bool isBindingPattern)
    {
      ObjectLiteralProperty objectLiteralProperty = (ObjectLiteralProperty) null;
      ObjectLiteralField objectLiteralField = (ObjectLiteralField) null;
      AstNode astNode = (AstNode) null;
      JSToken jsToken = this.PeekToken();
      Context propertyContext = this.m_currentToken.Clone();
      switch (jsToken)
      {
        case JSToken.RightCurly:
        case JSToken.Comma:
        case JSToken.Assign:
          this.ParsedVersion = ScriptVersion.EcmaScript6;
          astNode = this.ParseObjectPropertyValue(isBindingPattern);
          if (isBindingPattern && this.m_currentToken.Is(JSToken.Assign))
          {
            Context context = this.m_currentToken.Clone();
            this.GetNextToken();
            astNode = (AstNode) new InitializerNode(context.Clone())
            {
              Binding = astNode,
              AssignContext = context,
              Initializer = this.ParseExpression(true)
            };
            break;
          }
          break;
        case JSToken.Colon:
          objectLiteralField = this.ParseObjectLiteralFieldName();
          if (this.m_currentToken.Is(JSToken.Colon))
          {
            objectLiteralField.IfNotNull<ObjectLiteralField, Context>((Func<ObjectLiteralField, Context>) (f => f.ColonContext = this.m_currentToken.Clone()));
            this.GetNextToken();
            astNode = this.ParseObjectPropertyValue(isBindingPattern);
            if (isBindingPattern && this.m_currentToken.Is(JSToken.Assign))
            {
              Context context = this.m_currentToken.Clone();
              this.GetNextToken();
              astNode = (AstNode) new InitializerNode(context.Clone())
              {
                Binding = astNode,
                AssignContext = context,
                Initializer = this.ParseExpression(true)
              };
              break;
            }
            break;
          }
          break;
        default:
          if (this.m_currentToken.IsOne(JSToken.Get, JSToken.Set))
          {
            bool isGetter = this.m_currentToken.Is(JSToken.Get);
            Context fncCtx = this.m_currentToken.Clone();
            FunctionObject function = this.ParseFunction(isGetter ? FunctionType.Getter : FunctionType.Setter, fncCtx);
            if (function != null)
            {
              objectLiteralField = (ObjectLiteralField) new GetterSetter(function.Binding.Name, isGetter, function.Binding.Context.Clone());
              astNode = (AstNode) function;
              if (isBindingPattern)
              {
                fncCtx.HandleError(JSError.MethodsNotAllowedInBindings, true);
                break;
              }
              break;
            }
            this.ReportError(JSError.FunctionExpressionExpected);
            break;
          }
          if (this.m_currentToken.Is(JSToken.Multiply) || jsToken == JSToken.LeftParenthesis)
          {
            astNode = (AstNode) this.ParseFunction(FunctionType.Method, this.m_currentToken.Clone());
            if (astNode != null)
            {
              this.ParsedVersion = ScriptVersion.EcmaScript6;
              break;
            }
            break;
          }
          break;
      }
      if (objectLiteralField != null || astNode != null)
      {
        objectLiteralField.IfNotNull<ObjectLiteralField, Context>((Func<ObjectLiteralField, Context>) (f => propertyContext.UpdateWith(f.Context)));
        astNode.IfNotNull<AstNode, Context>((Func<AstNode, Context>) (v => propertyContext.UpdateWith(v.Context)));
        objectLiteralProperty = new ObjectLiteralProperty(propertyContext)
        {
          Name = objectLiteralField,
          Value = astNode
        };
        if (this.m_currentToken.Is(JSToken.Comma))
          objectLiteralProperty.IfNotNull<ObjectLiteralProperty, Context>((Func<ObjectLiteralProperty, Context>) (p => p.TerminatingContext = this.m_currentToken.Clone()));
      }
      return objectLiteralProperty;
    }

    private ObjectLiteralField ParseObjectLiteralFieldName()
    {
      ObjectLiteralField literalFieldName;
      switch (this.m_currentToken.Token)
      {
        case JSToken.Identifier:
        case JSToken.Get:
        case JSToken.Set:
          literalFieldName = new ObjectLiteralField((object) this.m_scanner.Identifier, PrimitiveType.String, this.m_currentToken.Clone())
          {
            IsIdentifier = true
          };
          break;
        case JSToken.StringLiteral:
          ObjectLiteralField objectLiteralField = new ObjectLiteralField((object) this.m_scanner.StringLiteralValue, PrimitiveType.String, this.m_currentToken.Clone());
          objectLiteralField.MayHaveIssues = this.m_scanner.LiteralHasIssues;
          literalFieldName = objectLiteralField;
          break;
        case JSToken.IntegerLiteral:
        case JSToken.NumericLiteral:
          double doubleValue;
          if (this.ConvertNumericLiteralToDouble(this.m_currentToken.Code, this.m_currentToken.Is(JSToken.IntegerLiteral), out doubleValue))
          {
            literalFieldName = new ObjectLiteralField((object) doubleValue, PrimitiveType.Number, this.m_currentToken.Clone());
            break;
          }
          if (double.IsInfinity(doubleValue))
            this.ReportError(JSError.NumericOverflow);
          literalFieldName = new ObjectLiteralField((object) this.m_currentToken.Code, PrimitiveType.Other, this.m_currentToken.Clone());
          break;
        default:
          string identifier = this.m_scanner.Identifier;
          if (JSScanner.IsValidIdentifier(identifier))
          {
            if (JSKeyword.CanBeIdentifier(this.m_currentToken.Token) == null)
              this.ReportError(JSError.ObjectLiteralKeyword);
            literalFieldName = new ObjectLiteralField((object) identifier, PrimitiveType.String, this.m_currentToken.Clone());
            break;
          }
          this.ReportError(JSError.NoMemberIdentifier);
          literalFieldName = new ObjectLiteralField((object) this.m_currentToken.Code, PrimitiveType.String, this.m_currentToken.Clone());
          break;
      }
      this.GetNextToken();
      return literalFieldName;
    }

    private AstNode ParseObjectPropertyValue(bool isBindingPattern) => isBindingPattern ? this.ParseBinding() : this.ParseExpression(true);

    private AstNode ParseMemberExpression(AstNode expression, List<Context> newContexts)
    {
      while (true)
      {
        switch (this.m_currentToken.Token)
        {
          case JSToken.LeftParenthesis:
            AstNodeList expressionList = this.ParseExpressionList(JSToken.RightParenthesis);
            expression = (AstNode) new CallNode(expression.Context.CombineWith(expressionList.Context))
            {
              Function = expression,
              Arguments = expressionList,
              InBrackets = false
            };
            if (newContexts != null && newContexts.Count > 0)
            {
              newContexts[newContexts.Count - 1].UpdateWith(expression.Context);
              if (!(expression is CallNode))
                expression = (AstNode) new CallNode(newContexts[newContexts.Count - 1])
                {
                  Function = expression,
                  Arguments = new AstNodeList(this.CurrentPositionContext)
                };
              else
                expression.Context = newContexts[newContexts.Count - 1];
              ((CallNode) expression).IsConstructor = true;
              newContexts.RemoveAt(newContexts.Count - 1);
            }
            this.GetNextToken();
            continue;
          case JSToken.LeftBracket:
            this.GetNextToken();
            AstNodeList astNodeList = new AstNodeList(this.CurrentPositionContext);
            AstNode expression1 = this.ParseExpression();
            if (expression1 != null)
              astNodeList.Append(expression1);
            expression = (AstNode) new CallNode(expression.Context.CombineWith(this.m_currentToken.Clone()))
            {
              Function = expression,
              Arguments = astNodeList,
              InBrackets = true
            };
            this.GetNextToken();
            continue;
          case JSToken.AccessField:
            ConstantWrapper constantWrapper = (ConstantWrapper) null;
            Context nameContext = this.m_currentToken.Clone();
            this.GetNextToken();
            string str;
            if (this.m_currentToken.IsNot(JSToken.Identifier))
            {
              str = JSKeyword.CanBeIdentifier(this.m_currentToken.Token);
              if (str != null)
                constantWrapper = new ConstantWrapper((object) str, PrimitiveType.String, this.m_currentToken.Clone());
              else if (JSScanner.IsValidIdentifier(this.m_currentToken.Code))
              {
                this.ReportError(JSError.KeywordUsedAsIdentifier);
                str = this.m_currentToken.Code;
                constantWrapper = new ConstantWrapper((object) str, PrimitiveType.String, this.m_currentToken.Clone());
              }
              else
                this.ReportError(JSError.NoIdentifier);
            }
            else
            {
              str = this.m_scanner.Identifier;
              constantWrapper = new ConstantWrapper((object) str, PrimitiveType.String, this.m_currentToken.Clone());
            }
            if (constantWrapper != null)
              nameContext.UpdateWith(constantWrapper.Context);
            this.GetNextToken();
            expression = (AstNode) new Member(expression.IfNotNull<AstNode, Context>((Func<AstNode, Context>) (e => e.Context.CombineWith(nameContext)), nameContext.Clone()))
            {
              Root = expression,
              Name = str,
              NameContext = nameContext
            };
            continue;
          default:
            goto label_20;
        }
      }
label_20:
      if (newContexts != null)
      {
        while (newContexts.Count > 0)
        {
          newContexts[newContexts.Count - 1].UpdateWith(expression.Context);
          expression = (AstNode) new CallNode(newContexts[newContexts.Count - 1])
          {
            Function = expression,
            Arguments = new AstNodeList(this.CurrentPositionContext)
          };
          ((CallNode) expression).IsConstructor = true;
          newContexts.RemoveAt(newContexts.Count - 1);
        }
      }
      return expression;
    }

    private AstNodeList ParseExpressionList(JSToken terminator)
    {
      AstNodeList expressionList = new AstNodeList(this.m_currentToken.Clone());
      do
      {
        this.GetNextToken();
        AstNode node = (AstNode) null;
        if (this.m_currentToken.Is(JSToken.Comma))
        {
          node = (AstNode) new ConstantWrapper((object) Missing.Value, PrimitiveType.Other, this.m_currentToken.FlattenToStart());
          expressionList.Append(node);
          expressionList.UpdateWith(this.m_currentToken);
        }
        else if (this.m_currentToken.IsNot(terminator))
        {
          Context context = (Context) null;
          if (this.m_currentToken.Is(JSToken.RestSpread))
          {
            this.ParsedVersion = ScriptVersion.EcmaScript6;
            context = this.m_currentToken.Clone();
            this.GetNextToken();
          }
          node = this.ParseExpression(true);
          if (context != null)
            node = (AstNode) new UnaryOperator(context.CombineWith(node.Context))
            {
              Operand = node,
              OperatorToken = JSToken.RestSpread,
              OperatorContext = context
            };
          expressionList.Append(node);
        }
        if (this.m_currentToken.Is(JSToken.Comma))
          node.IfNotNull<AstNode, Context>((Func<AstNode, Context>) (i => i.TerminatingContext = this.m_currentToken.Clone()));
      }
      while (this.m_currentToken.Is(JSToken.Comma));
      if (this.m_currentToken.Is(terminator))
        expressionList.Context.UpdateWith(this.m_currentToken);
      else if (terminator == JSToken.RightParenthesis)
      {
        if (this.m_currentToken.Is(JSToken.Semicolon) && this.PeekToken() == JSToken.RightParenthesis)
        {
          this.ReportError(JSError.UnexpectedSemicolon);
          this.GetNextToken();
        }
        else
          this.ReportError(JSError.NoRightParenthesis);
      }
      else
        this.ReportError(JSError.NoRightBracket);
      return expressionList;
    }

    private void SetDocumentContext(DocumentContext documentContext)
    {
      documentContext.Parser = this;
      this.m_scanner = new JSScanner(documentContext);
      this.m_currentToken = this.m_scanner.CurrentToken;
      this.m_scanner.GlobalDefine += (EventHandler<GlobalDefineEventArgs>) ((sender, ea) =>
      {
        GlobalScope globalScope = this.GlobalScope;
        if (globalScope[ea.Name] != null)
          return;
        JSVariableField field = globalScope.CreateField(ea.Name, (object) null, FieldAttributes.SpecialName);
        globalScope.AddField(field);
      });
      this.m_scanner.NewModule += (EventHandler<NewModuleEventArgs>) ((sender, ea) =>
      {
        this.m_newModule = true;
        this.m_foundEndOfLine = true;
      });
    }

    private static AstNode CreateExpressionNode(
      Context operatorContext,
      AstNode operand1,
      AstNode operand2)
    {
      Context context = (operand1.IfNotNull<AstNode, Context>((Func<AstNode, Context>) (operand => operand.Context)) ?? operatorContext).CombineWith(operand2.IfNotNull<AstNode, Context>((Func<AstNode, Context>) (operand => operand.Context)));
      switch (operatorContext.Token)
      {
        case JSToken.FirstBinaryOperator:
        case JSToken.Minus:
        case JSToken.Multiply:
        case JSToken.Divide:
        case JSToken.Modulo:
        case JSToken.BitwiseAnd:
        case JSToken.BitwiseOr:
        case JSToken.BitwiseXor:
        case JSToken.LeftShift:
        case JSToken.RightShift:
        case JSToken.UnsignedRightShift:
        case JSToken.Equal:
        case JSToken.NotEqual:
        case JSToken.StrictEqual:
        case JSToken.StrictNotEqual:
        case JSToken.LessThan:
        case JSToken.LessThanEqual:
        case JSToken.GreaterThan:
        case JSToken.GreaterThanEqual:
        case JSToken.LogicalAnd:
        case JSToken.LogicalOr:
        case JSToken.InstanceOf:
        case JSToken.In:
        case JSToken.Assign:
        case JSToken.PlusAssign:
        case JSToken.MinusAssign:
        case JSToken.MultiplyAssign:
        case JSToken.DivideAssign:
        case JSToken.ModuloAssign:
        case JSToken.BitwiseAndAssign:
        case JSToken.BitwiseOrAssign:
        case JSToken.BitwiseXorAssign:
        case JSToken.LeftShiftAssign:
        case JSToken.RightShiftAssign:
        case JSToken.UnsignedRightShiftAssign:
          return (AstNode) new BinaryOperator(context)
          {
            Operand1 = operand1,
            Operand2 = operand2,
            OperatorContext = operatorContext,
            OperatorToken = operatorContext.Token
          };
        case JSToken.Comma:
          return CommaOperator.CombineWithComma(context, operand1, operand2);
        default:
          return (AstNode) null;
      }
    }

    private bool ConvertNumericLiteralToDouble(string str, bool isInteger, out double doubleValue)
    {
      try
      {
        if (isInteger)
        {
          if (str[0] == '0' && str.Length > 1)
          {
            if (str[1] == 'x' || str[1] == 'X')
            {
              if (str.Length == 2)
              {
                doubleValue = 0.0;
                return false;
              }
              doubleValue = (double) Convert.ToInt64(str, 16);
            }
            else if (str[1] == 'o' || str[1] == 'O')
            {
              if (str.Length == 2)
              {
                doubleValue = 0.0;
                return false;
              }
              doubleValue = (double) Convert.ToInt64(str.Substring(2), 8);
            }
            else
            {
              if (str[1] != 'b')
              {
                if (str[1] != 'B')
                {
                  try
                  {
                    doubleValue = (double) Convert.ToInt64(str, 8);
                    if ((double) Convert.ToInt64(str, 10) != doubleValue)
                    {
                      this.ReportError(JSError.OctalLiteralsDeprecated);
                      return false;
                    }
                    goto label_19;
                  }
                  catch (FormatException ex)
                  {
                    doubleValue = Convert.ToDouble(str, (IFormatProvider) CultureInfo.InvariantCulture);
                    goto label_19;
                  }
                }
              }
              if (str.Length == 2)
              {
                doubleValue = 0.0;
                return false;
              }
              doubleValue = (double) Convert.ToInt64(str.Substring(2), 2);
            }
          }
          else
            doubleValue = Convert.ToDouble(str, (IFormatProvider) CultureInfo.InvariantCulture);
label_19:
          if (doubleValue < -9.00719925474099E+15 || 9.00719925474099E+15 < doubleValue)
            return false;
        }
        else
          doubleValue = Convert.ToDouble(str, (IFormatProvider) CultureInfo.InvariantCulture);
        return true;
      }
      catch (OverflowException ex)
      {
        doubleValue = str[0] == '-' ? double.NegativeInfinity : double.PositiveInfinity;
        return false;
      }
      catch (FormatException ex)
      {
        doubleValue = double.NaN;
        return false;
      }
    }

    private void AppendImportantComments(Block block)
    {
      if (block == null || this.m_importantComments.Count <= 0 || !this.m_settings.PreserveImportantComments || !this.m_settings.IsModificationAllowed(TreeModifications.PreserveImportantComments))
        return;
      foreach (Context importantComment in (IEnumerable<Context>) this.m_importantComments)
        block.Append((AstNode) new ImportantComment(importantComment));
      this.m_importantComments.Clear();
    }

    private void GetNextToken() => this.m_currentToken = this.ScanNextToken();

    private Context ScanNextToken()
    {
      this.EchoWriter.IfNotNull<TextWriter>((Action<TextWriter>) (w =>
      {
        if (!this.m_currentToken.IsNot(JSToken.None))
          return;
        w.Write(this.m_currentToken.Code);
      }));
      this.m_newModule = false;
      this.m_foundEndOfLine = false;
      this.m_importantComments.Clear();
      Context nextToken = this.m_scanner.ScanNextToken();
      while (true)
      {
        if (nextToken.IsOne(JSToken.WhiteSpace, JSToken.EndOfLine, JSToken.SingleLineComment, JSToken.MultipleLineComment, JSToken.PreprocessorDirective, JSToken.Error))
        {
          if (nextToken.Is(JSToken.EndOfLine))
            this.m_foundEndOfLine = true;
          else if (nextToken.IsOne(JSToken.MultipleLineComment, JSToken.SingleLineComment) && nextToken.HasCode && (nextToken.Code.Length > 2 && nextToken.Code[2] == '!' || nextToken.Code.IndexOf("@preserve", StringComparison.OrdinalIgnoreCase) >= 0 || nextToken.Code.IndexOf("@license", StringComparison.OrdinalIgnoreCase) >= 0))
            this.m_importantComments.Add(nextToken.Clone());
          this.EchoWriter.IfNotNull<TextWriter>((Action<TextWriter>) (w =>
          {
            if (this.Settings.PreprocessOnly && nextToken.Token == JSToken.PreprocessorDirective)
              return;
            w.Write(nextToken.Code);
          }));
          nextToken = this.m_scanner.ScanNextToken();
        }
        else
          break;
      }
      if (nextToken.Is(JSToken.EndOfFile))
        this.m_foundEndOfLine = true;
      return nextToken;
    }

    private JSToken PeekToken()
    {
      JSScanner jsScanner = this.m_scanner.Clone();
      jsScanner.SuppressErrors = true;
      Context context = jsScanner.ScanNextToken();
      while (true)
      {
        if (context.IsOne(JSToken.WhiteSpace, JSToken.EndOfLine, JSToken.Error, JSToken.SingleLineComment, JSToken.MultipleLineComment, JSToken.PreprocessorDirective, JSToken.ConditionalCommentEnd, JSToken.ConditionalCommentStart, JSToken.ConditionalCompilationElse, JSToken.ConditionalCompilationElseIf, JSToken.ConditionalCompilationEnd, JSToken.ConditionalCompilationIf, JSToken.ConditionalCompilationOn, JSToken.ConditionalCompilationSet, JSToken.ConditionalCompilationVariable, JSToken.ConditionalIf))
          context = jsScanner.ScanNextToken();
        else
          break;
      }
      return context.Token;
    }

    private bool PeekCanBeModule()
    {
      if (this.ParsedVersion == ScriptVersion.EcmaScript6 || this.m_settings.ScriptVersion == ScriptVersion.EcmaScript6)
        return true;
      JSScanner jsScanner = this.m_scanner.Clone();
      jsScanner.SuppressErrors = true;
      Context context = jsScanner.ScanNextToken();
      bool flag = false;
      while (true)
      {
        if (context.IsOne(JSToken.WhiteSpace, JSToken.EndOfLine, JSToken.Error, JSToken.SingleLineComment, JSToken.MultipleLineComment, JSToken.PreprocessorDirective, JSToken.ConditionalCommentEnd, JSToken.ConditionalCommentStart, JSToken.ConditionalCompilationElse, JSToken.ConditionalCompilationElseIf, JSToken.ConditionalCompilationEnd, JSToken.ConditionalCompilationIf, JSToken.ConditionalCompilationOn, JSToken.ConditionalCompilationSet, JSToken.ConditionalCompilationVariable, JSToken.ConditionalIf))
        {
          if (context.Is(JSToken.EndOfLine))
            flag = true;
          context = jsScanner.ScanNextToken();
        }
        else
          break;
      }
      return context.Is(JSToken.StringLiteral) && !flag || context.Is(JSToken.Identifier) || JSKeyword.CanBeIdentifier(context.Token) != null;
    }

    private void ExpectSemicolon(AstNode node)
    {
      if (this.m_currentToken.Is(JSToken.Semicolon))
      {
        node.TerminatingContext = this.m_currentToken.Clone();
        this.GetNextToken();
      }
      else
      {
        if (!this.m_foundEndOfLine)
        {
          if (!this.m_currentToken.IsOne(JSToken.RightCurly, JSToken.EndOfFile))
          {
            this.ReportError(JSError.NoSemicolon, node.Context.IfNotNull<Context, Context>((Func<Context, Context>) (c => c.FlattenToEnd())));
            return;
          }
        }
        if (!this.m_currentToken.IsNot(JSToken.RightCurly) || !this.m_currentToken.IsNot(JSToken.EndOfFile))
          return;
        this.ReportError(JSError.SemicolonInsertion, node.Context.IfNotNull<Context, Context>((Func<Context, Context>) (c => c.FlattenToEnd())));
      }
    }

    private void ReportError(JSError errorId, Context context = null, bool forceToError = false)
    {
      context = context ?? this.m_currentToken.Clone();
      if (context.Token == JSToken.EndOfFile)
        context.HandleError(errorId, true);
      else
        context.HandleError(errorId, forceToError);
    }

    private void CCTooComplicated(Context context)
    {
      (context ?? this.m_currentToken).HandleError(JSError.ConditionalCompilationTooComplex);
      while (this.m_currentToken.IsNot(JSToken.EndOfFile) && this.m_currentToken.IsNot(JSToken.ConditionalCommentEnd))
        this.GetNextToken();
      this.GetNextToken();
    }
  }
}
