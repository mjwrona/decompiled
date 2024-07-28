// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Pipelines.Yaml.ObjectTemplating.TemplateUnraveler
// Assembly: Microsoft.TeamFoundation.DistributedTask.Pipelines.Yaml, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 2DC134C9-663D-46C7-A414-3ADCC50BB112
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Pipelines.Yaml.dll

using Microsoft.TeamFoundation.DistributedTask.Pipelines.Yaml.ObjectTemplating.Tokens;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.TeamFoundation.DistributedTask.Pipelines.Yaml.ObjectTemplating
{
  internal sealed class TemplateUnraveler
  {
    private readonly TemplateContext m_context;
    private readonly TemplateMemory m_memory;
    private TemplateUnraveler.ReaderState m_last;
    private TemplateUnraveler.ReaderState m_current;

    internal TemplateUnraveler(TemplateContext context, TemplateToken template, int removeBytes)
    {
      this.m_context = context;
      this.m_memory = context.Memory;
      this.MoveFirst(template, removeBytes);
    }

    internal bool AllowLiteral(out LiteralToken literal, bool moveNext = true)
    {
      this.m_memory.IncrementEvents();
      if (this.m_current is TemplateUnraveler.LiteralState current)
      {
        literal = current.Value;
        if (moveNext)
          this.MoveNext();
        return true;
      }
      literal = (LiteralToken) null;
      return false;
    }

    internal bool AllowSequenceStart(out SequenceToken sequence)
    {
      this.m_memory.IncrementEvents();
      if (this.m_current is TemplateUnraveler.SequenceState current && current.IsStart)
      {
        sequence = new SequenceToken(current.Value.FileId, current.Value.Line, current.Value.Column);
        this.MoveNext();
        return true;
      }
      sequence = (SequenceToken) null;
      return false;
    }

    internal bool AllowSequenceEnd()
    {
      this.m_memory.IncrementEvents();
      if (!(this.m_current is TemplateUnraveler.SequenceState current) || !current.IsEnd)
        return false;
      this.MoveNext();
      return true;
    }

    internal bool AllowMappingStart(out MappingToken mapping)
    {
      this.m_memory.IncrementEvents();
      if (this.m_current is TemplateUnraveler.MappingState current && current.IsStart)
      {
        mapping = new MappingToken(current.Value.FileId, current.Value.Line, current.Value.Column);
        this.MoveNext();
        return true;
      }
      mapping = (MappingToken) null;
      return false;
    }

    internal bool AllowMappingEnd()
    {
      this.m_memory.IncrementEvents();
      if (!(this.m_current is TemplateUnraveler.MappingState current) || !current.IsEnd)
        return false;
      this.MoveNext();
      return true;
    }

    internal void ReadEnd()
    {
      this.m_memory.IncrementEvents();
      if (this.m_current != null)
        throw new InvalidOperationException("Expected end of template object. " + this.DumpState());
    }

    internal LiteralToken ReadLiteral()
    {
      LiteralToken literal;
      if (this.AllowLiteral(out literal))
        return literal;
      throw new InvalidOperationException("Unexpected state while attempting to read a scalar. " + this.DumpState());
    }

    internal void ReadMappingEnd()
    {
      if (!this.AllowMappingEnd())
        throw new InvalidOperationException("Unexpected state while attempting to read the mapping end. " + this.DumpState());
    }

    internal void SkipSequenceItem()
    {
      this.m_memory.IncrementEvents();
      TemplateUnraveler.ReaderState parent = this.m_current?.Parent;
      while (true)
      {
        switch (parent)
        {
          case null:
          case TemplateUnraveler.MappingState _:
          case TemplateUnraveler.SequenceState _:
            goto label_3;
          default:
            parent = parent.Parent;
            continue;
        }
      }
label_3:
      if (!(parent is TemplateUnraveler.SequenceState))
        throw new InvalidOperationException("Unexpected state while attempting to skip the current sequence item. " + this.DumpState());
      this.MoveNext(true);
    }

    internal void SkipMappingValue()
    {
      this.m_memory.IncrementEvents();
      TemplateUnraveler.ReaderState parent = this.m_current?.Parent;
      while (true)
      {
        switch (parent)
        {
          case null:
          case TemplateUnraveler.MappingState _:
          case TemplateUnraveler.SequenceState _:
            goto label_3;
          default:
            parent = parent.Parent;
            continue;
        }
      }
label_3:
      if (!(parent is TemplateUnraveler.MappingState mappingState) || mappingState.IsKey)
        throw new InvalidOperationException("Unexpected state while attempting to skip the current mapping value. " + this.DumpState());
      this.MoveNext(true);
    }

    private string DumpState()
    {
      StringBuilder stringBuilder = new StringBuilder();
      if (this.m_current == null)
      {
        stringBuilder.AppendLine("State: (null)");
      }
      else
      {
        stringBuilder.AppendLine("State:");
        stringBuilder.AppendLine();
        Stack<TemplateUnraveler.ReaderState> readerStateStack = new Stack<TemplateUnraveler.ReaderState>();
        for (TemplateUnraveler.ReaderState readerState = this.m_current; readerState != null; readerState = readerState.Parent)
          stringBuilder.AppendLine(readerState.ToString());
      }
      return stringBuilder.ToString();
    }

    private void MoveFirst(TemplateToken value, int removeBytes)
    {
      switch (value)
      {
        case LiteralToken _:
        case SequenceToken _:
        case MappingToken _:
          this.m_memory.IncrementEvents();
          this.m_current = TemplateUnraveler.ReaderState.CreateState((TemplateUnraveler.ReaderState) null, value, this.m_context, removeBytes);
          this.Unravel();
          break;
        default:
          throw new NotSupportedException("Unexpected type '" + value?.GetType().Name + "' when initializing object reader state");
      }
    }

    internal void MoveNext(bool skipNestedEvents = false)
    {
      this.m_memory.IncrementEvents();
      if (this.m_current == null)
        return;
      if (this.m_current is TemplateUnraveler.SequenceState current2 && current2.IsStart && !skipNestedEvents)
        this.m_current = current2.Next();
      else if (this.m_current is TemplateUnraveler.MappingState current1 && current1.IsStart && !skipNestedEvents)
        this.m_current = current1.Next();
      else if (this.m_current.Parent is TemplateUnraveler.SequenceState parent2)
      {
        this.m_current.Remove();
        this.m_current = parent2.Next();
      }
      else if (this.m_current.Parent is TemplateUnraveler.MappingState parent1)
      {
        this.m_current.Remove();
        this.m_current = parent1.Next();
      }
      else if (this.m_current.Parent != null)
      {
        this.m_current.Remove();
        this.m_current = this.m_current.Parent;
      }
      else
      {
        this.m_current.Remove();
        this.m_current = (TemplateUnraveler.ReaderState) null;
      }
      this.Unravel();
    }

    private void Unravel()
    {
      while (this.m_current != null)
      {
        if (this.m_current is TemplateUnraveler.LiteralState current1)
        {
          this.m_memory.AddBytes(current1.Value);
          break;
        }
        if (this.m_current is TemplateUnraveler.BasicExpressionState current9)
        {
          if (current9.IsStart && this.m_current.Parent is TemplateUnraveler.SequenceState)
            this.SequenceItemBasicExpression();
          else if (current9.IsStart && this.m_current.Parent is TemplateUnraveler.MappingState parent2 && parent2.IsKey)
            this.MappingKeyBasicExpression();
          else if (current9.IsStart && this.m_current.Parent is TemplateUnraveler.MappingState parent1 && !parent1.IsKey)
            this.MappingValueBasicExpression();
          else if (current9.IsEnd)
            this.EndExpression();
          else
            this.UnexpectedState();
        }
        else if (this.m_current is TemplateUnraveler.MappingState current8)
        {
          if (current8.IsStart && current8.Value.Count == 1 && current8.Value[0].Key is IfExpressionToken && current8.Value[0].Value is SequenceToken && this.m_current.Parent is TemplateUnraveler.SequenceState)
            this.StartIfSequenceInsertion();
          else if (current8.IsStart && current8.Value.Count == 1 && current8.Value[0].Key is ElseIfExpressionToken && current8.Value[0].Value is SequenceToken && (this.m_last is TemplateUnraveler.IfExpressionState || this.m_last is TemplateUnraveler.ElseIfExpressionState) && this.m_current.Parent is TemplateUnraveler.SequenceState)
            this.StartElseIfSequenceInsertion();
          else if (current8.IsStart && current8.Value.Count == 1 && current8.Value[0].Key is ElseExpressionToken && current8.Value[0].Value is SequenceToken && (this.m_last is TemplateUnraveler.IfExpressionState || this.m_last is TemplateUnraveler.ElseIfExpressionState) && this.m_current.Parent is TemplateUnraveler.SequenceState)
            this.StartElseSequenceInsertion();
          else if (current8.IsStart && current8.Value.Count == 1 && current8.Value[0].Key is EachExpressionToken && current8.Value[0].Value is SequenceToken && this.m_current.Parent is TemplateUnraveler.SequenceState)
            this.StartEachSequenceInsertion();
          else if (current8.IsEnd && this.m_current.Parent is TemplateUnraveler.InsertExpressionState)
          {
            this.m_current.Remove();
            this.m_current = this.m_current.Parent;
          }
          else if (current8.IsEnd && this.m_current.Parent is TemplateUnraveler.IfExpressionState)
          {
            this.m_current.Remove();
            this.m_current = this.m_current.Parent;
          }
          else if (current8.IsEnd && this.m_current.Parent is TemplateUnraveler.ElseExpressionState)
          {
            this.m_current.Remove();
            this.m_current = this.m_current.Parent;
          }
          else if (current8.IsEnd && this.m_current.Parent is TemplateUnraveler.ElseIfExpressionState)
          {
            this.m_current.Remove();
            this.m_current = this.m_current.Parent;
          }
          else if (current8.IsEnd && this.m_current.Parent is TemplateUnraveler.EachExpressionState)
          {
            this.NextEachMappingInsertion();
          }
          else
          {
            if (current8.IsStart)
            {
              this.m_memory.AddBytes(current8.Value);
              break;
            }
            if (current8.IsEnd)
              break;
            this.UnexpectedState();
          }
        }
        else if (this.m_current is TemplateUnraveler.SequenceState current7)
        {
          if (current7.IsEnd && this.m_current.Parent is TemplateUnraveler.BasicExpressionState && this.m_current.Parent.Parent is TemplateUnraveler.SequenceState)
          {
            this.m_current.Remove();
            this.m_current = this.m_current.Parent;
          }
          else if (current7.IsEnd && current7.Parent is TemplateUnraveler.IfExpressionState && current7.Parent.Parent is TemplateUnraveler.MappingState parent6 && parent6.IsKey && parent6.Value.Count == 1 && parent6.Parent is TemplateUnraveler.SequenceState)
          {
            this.m_current.Remove();
            this.m_current = this.m_current.Parent;
          }
          else if (current7.IsEnd && current7.Parent is TemplateUnraveler.ElseExpressionState && current7.Parent.Parent is TemplateUnraveler.MappingState parent5 && parent5.IsKey && parent5.Value.Count == 1 && parent5.Parent is TemplateUnraveler.SequenceState)
          {
            this.m_current.Remove();
            this.m_current = this.m_current.Parent;
          }
          else if (current7.IsEnd && current7.Parent is TemplateUnraveler.ElseIfExpressionState && current7.Parent.Parent is TemplateUnraveler.MappingState parent4 && parent4.IsKey && parent4.Value.Count == 1 && parent4.Parent is TemplateUnraveler.SequenceState)
          {
            this.m_current.Remove();
            this.m_current = this.m_current.Parent;
          }
          else if (current7.IsEnd && current7.Parent is TemplateUnraveler.EachExpressionState && current7.Parent.Parent is TemplateUnraveler.MappingState parent3 && parent3.IsKey && parent3.Value.Count == 1 && parent3.Parent is TemplateUnraveler.SequenceState)
          {
            this.NextEachSequenceInsertion();
          }
          else
          {
            if (current7.IsStart)
            {
              this.m_memory.AddBytes(current7.Value);
              break;
            }
            if (current7.IsEnd)
              break;
            this.UnexpectedState();
          }
        }
        else if (this.m_current is TemplateUnraveler.InsertExpressionState current6)
        {
          if (current6.IsStart && this.m_current.Parent is TemplateUnraveler.MappingState parent7 && parent7.IsKey)
            this.StartMappingInsertion();
          else if (current6.IsEnd)
            this.EndExpression();
          else if (current6.IsStart)
          {
            this.m_context.Error((TemplateToken) current6.Value, YamlStrings.DirectiveNotAllowed((object) current6.Value.Directive));
            this.m_current.Remove();
            this.m_current = current6.ToLiteral();
          }
          else
            this.UnexpectedState();
        }
        else if (this.m_current is TemplateUnraveler.IfExpressionState current5)
        {
          if (current5.IsStart && this.m_current.Parent is TemplateUnraveler.MappingState parent8 && parent8.IsKey)
            this.StartIfMappingInsertion();
          else if (current5.IsEnd)
            this.EndExpression();
          else if (current5.IsStart)
          {
            this.m_context.Error((TemplateToken) current5.Value, YamlStrings.DirectiveNotAllowed((object) current5.Value.Directive));
            this.m_current.Remove();
            this.m_current = current5.ToLiteral();
          }
          else
            this.UnexpectedState();
        }
        else if (this.m_current is TemplateUnraveler.ElseIfExpressionState current4)
        {
          if (current4.IsStart && this.m_current.Parent is TemplateUnraveler.MappingState parent9 && (this.m_last is TemplateUnraveler.IfExpressionState || this.m_last is TemplateUnraveler.ElseIfExpressionState) && parent9.IsKey)
            this.StartElseIfMappingInsertion();
          else if (current4.IsEnd)
            this.EndExpression();
          else if (current4.IsStart)
          {
            this.m_context.Error((TemplateToken) current4.Value, YamlStrings.DirectiveNotAllowed((object) current4.Value.Directive));
            this.m_current.Remove();
            this.m_current = current4.ToLiteral();
          }
          else
            this.UnexpectedState();
        }
        else if (this.m_current is TemplateUnraveler.ElseExpressionState current3)
        {
          if (current3.IsStart && this.m_current.Parent is TemplateUnraveler.MappingState parent10 && (this.m_last is TemplateUnraveler.IfExpressionState || this.m_last is TemplateUnraveler.ElseIfExpressionState) && parent10.IsKey)
            this.StartElseMappingInsertion();
          else if (current3.IsEnd)
            this.EndExpression();
          else if (current3.IsStart)
          {
            this.m_context.Error((TemplateToken) current3.Value, YamlStrings.DirectiveNotAllowed((object) current3.Value.Directive));
            this.m_current.Remove();
            this.m_current = current3.ToLiteral();
          }
          else
            this.UnexpectedState();
        }
        else if (this.m_current is TemplateUnraveler.EachExpressionState current2)
        {
          if (current2.IsStart && this.m_current.Parent is TemplateUnraveler.MappingState parent11 && parent11.IsKey)
            this.StartEachMappingInsertion();
          else if (current2.IsEnd)
            this.EndExpression();
          else if (current2.IsStart)
          {
            this.m_context.Error((TemplateToken) current2.Value, YamlStrings.DirectiveNotAllowed((object) current2.Value.Directive));
            this.m_current.Remove();
            this.m_current = current2.ToLiteral();
          }
          else
            this.UnexpectedState();
        }
        else
          this.UnexpectedState();
        this.m_memory.IncrementEvents();
      }
    }

    private void SequenceItemBasicExpression()
    {
      TemplateUnraveler.BasicExpressionState current = this.m_current as TemplateUnraveler.BasicExpressionState;
      BasicExpressionToken basicExpressionToken = current.Value;
      int bytes = 0;
      TemplateToken templateToken;
      try
      {
        templateToken = basicExpressionToken.EvaluateTemplateToken(current.Context, out bytes);
      }
      catch (Exception ex)
      {
        this.m_context.Error((TemplateToken) basicExpressionToken, ex);
        templateToken = (TemplateToken) null;
      }
      if (templateToken is SequenceToken sequenceToken)
        this.m_current = current.Next(sequenceToken, true, bytes);
      else if (templateToken != null)
      {
        this.m_current = current.Next(templateToken, bytes);
      }
      else
      {
        if (templateToken != null)
          return;
        current.End();
      }
    }

    private void MappingKeyBasicExpression()
    {
      TemplateUnraveler.BasicExpressionState current = this.m_current as TemplateUnraveler.BasicExpressionState;
      BasicExpressionToken basicExpressionToken = current.Value;
      int bytes = 0;
      LiteralToken literalToken;
      try
      {
        literalToken = basicExpressionToken.EvaluateLiteralToken(current.Context, out bytes);
      }
      catch (Exception ex)
      {
        this.m_context.Error((TemplateToken) basicExpressionToken, ex);
        literalToken = (LiteralToken) null;
      }
      if (literalToken != null)
      {
        this.m_current = current.Next((TemplateToken) literalToken, bytes);
      }
      else
      {
        this.m_current.Remove();
        TemplateUnraveler.MappingState parent = this.m_current.Parent as TemplateUnraveler.MappingState;
        parent.Next().Remove();
        this.m_current = parent.Next();
      }
    }

    private void MappingValueBasicExpression()
    {
      TemplateUnraveler.BasicExpressionState current = this.m_current as TemplateUnraveler.BasicExpressionState;
      BasicExpressionToken basicExpressionToken = current.Value;
      int bytes = 0;
      TemplateToken templateToken;
      try
      {
        templateToken = basicExpressionToken.EvaluateTemplateToken(current.Context, out bytes);
      }
      catch (Exception ex)
      {
        this.m_context.Error((TemplateToken) basicExpressionToken, ex);
        templateToken = (TemplateToken) new LiteralToken(basicExpressionToken.FileId, basicExpressionToken.Line, basicExpressionToken.Column, string.Empty);
      }
      this.m_current = current.Next(templateToken, bytes);
    }

    private void StartMappingInsertion()
    {
      TemplateUnraveler.InsertExpressionState current = this.m_current as TemplateUnraveler.InsertExpressionState;
      TemplateUnraveler.MappingState parent = current.Parent as TemplateUnraveler.MappingState;
      TemplateToken templateToken = parent.Value[parent.Index].Value;
      MappingToken mappingToken = templateToken as MappingToken;
      int bytes = 0;
      if (mappingToken == null)
      {
        if (templateToken is BasicExpressionToken basicExpressionToken)
        {
          try
          {
            mappingToken = basicExpressionToken.EvaluateMappingToken(current.Context, out bytes);
          }
          catch (Exception ex)
          {
            this.m_context.Error((TemplateToken) basicExpressionToken, ex);
            mappingToken = (MappingToken) null;
          }
        }
        else
        {
          this.m_context.Error(templateToken, YamlStrings.ExpectedMapping());
          mappingToken = (MappingToken) null;
        }
      }
      if (mappingToken != null && mappingToken.Count > 0)
      {
        this.m_current = current.Next(mappingToken, bytes);
      }
      else
      {
        if (bytes > 0)
          this.m_memory.SubtractBytes(bytes);
        current.End();
      }
    }

    private void StartIfMappingInsertion()
    {
      TemplateUnraveler.IfExpressionState current = this.m_current as TemplateUnraveler.IfExpressionState;
      IfExpressionToken ifExpressionToken = current.Value;
      bool flag;
      try
      {
        flag = ifExpressionToken.Evaluate(current.Context);
      }
      catch (Exception ex)
      {
        this.m_context.Error((TemplateToken) ifExpressionToken, ex);
        flag = false;
      }
      current.EvaluationResult = flag;
      int bytes = 0;
      if (!flag)
      {
        mappingToken = (MappingToken) null;
      }
      else
      {
        TemplateUnraveler.MappingState parent = current.Parent as TemplateUnraveler.MappingState;
        TemplateToken templateToken = parent.Value[parent.Index].Value;
        switch (templateToken)
        {
          case MappingToken mappingToken:
            break;
          case BasicExpressionToken basicExpressionToken:
            try
            {
              mappingToken = basicExpressionToken.EvaluateMappingToken(current.Context, out bytes);
              break;
            }
            catch (Exception ex)
            {
              this.m_context.Error((TemplateToken) basicExpressionToken, ex);
              mappingToken = (MappingToken) null;
              break;
            }
          default:
            this.m_context.Error(templateToken, YamlStrings.ExpectedMapping());
            mappingToken = (MappingToken) null;
            break;
        }
      }
      if (mappingToken != null && mappingToken.Count > 0)
      {
        this.m_current = current.Next(mappingToken, bytes);
      }
      else
      {
        if (bytes > 0)
          this.m_memory.SubtractBytes(bytes);
        current.End();
      }
    }

    private void StartElseIfMappingInsertion()
    {
      TemplateUnraveler.ElseIfExpressionState current = this.m_current as TemplateUnraveler.ElseIfExpressionState;
      int bytes = 0;
      bool flag1 = false;
      if (this.m_last is TemplateUnraveler.IfExpressionState last2)
        flag1 = last2.EvaluationResult;
      else if (this.m_last is TemplateUnraveler.ElseIfExpressionState last1)
        flag1 = last1.EvaluationResult;
      if (flag1)
      {
        mappingToken = (MappingToken) null;
        current.EvaluationResult = true;
      }
      else
      {
        ElseIfExpressionToken ifExpressionToken = current.Value;
        bool flag2;
        try
        {
          flag2 = ifExpressionToken.Evaluate(current.Context);
        }
        catch (Exception ex)
        {
          this.m_context.Error((TemplateToken) ifExpressionToken, ex);
          flag2 = false;
        }
        current.EvaluationResult = flag2;
        if (!flag2)
        {
          mappingToken = (MappingToken) null;
        }
        else
        {
          TemplateUnraveler.MappingState parent = current.Parent as TemplateUnraveler.MappingState;
          TemplateToken templateToken = parent.Value[parent.Index].Value;
          switch (templateToken)
          {
            case MappingToken mappingToken:
              break;
            case BasicExpressionToken basicExpressionToken:
              try
              {
                mappingToken = basicExpressionToken.EvaluateMappingToken(current.Context, out bytes);
                break;
              }
              catch (Exception ex)
              {
                this.m_context.Error((TemplateToken) basicExpressionToken, ex);
                mappingToken = (MappingToken) null;
                break;
              }
            default:
              this.m_context.Error(templateToken, YamlStrings.ExpectedMapping());
              mappingToken = (MappingToken) null;
              break;
          }
        }
      }
      if (mappingToken != null && mappingToken.Count > 0)
      {
        this.m_current = current.Next(mappingToken, bytes);
      }
      else
      {
        if (bytes > 0)
          this.m_memory.SubtractBytes(bytes);
        current.End();
      }
    }

    private void StartElseMappingInsertion()
    {
      TemplateUnraveler.ElseExpressionState current = this.m_current as TemplateUnraveler.ElseExpressionState;
      bool flag = false;
      if (this.m_last is TemplateUnraveler.IfExpressionState last2)
        flag = last2.EvaluationResult;
      else if (this.m_last is TemplateUnraveler.ElseIfExpressionState last1)
        flag = last1.EvaluationResult;
      int bytes = 0;
      if (flag)
      {
        mappingToken = (MappingToken) null;
      }
      else
      {
        TemplateUnraveler.MappingState parent = current.Parent as TemplateUnraveler.MappingState;
        TemplateToken templateToken = parent.Value[parent.Index].Value;
        switch (templateToken)
        {
          case MappingToken mappingToken:
            break;
          case BasicExpressionToken basicExpressionToken:
            try
            {
              mappingToken = basicExpressionToken.EvaluateMappingToken(current.Context, out bytes);
              break;
            }
            catch (Exception ex)
            {
              this.m_context.Error((TemplateToken) basicExpressionToken, ex);
              mappingToken = (MappingToken) null;
              break;
            }
          default:
            this.m_context.Error(templateToken, YamlStrings.ExpectedMapping());
            mappingToken = (MappingToken) null;
            break;
        }
      }
      if (mappingToken != null && mappingToken.Count > 0)
      {
        this.m_current = current.Next(mappingToken, bytes);
      }
      else
      {
        if (bytes > 0)
          this.m_memory.SubtractBytes(bytes);
        current.End();
      }
    }

    private void StartEachMappingInsertion()
    {
      TemplateUnraveler.EachExpressionState current = this.m_current as TemplateUnraveler.EachExpressionState;
      EachExpressionToken eachExpressionToken = current.Value;
      int bytes1 = 0;
      IList collection1;
      try
      {
        collection1 = eachExpressionToken.Evaluate(current.Context, out bytes1);
      }
      catch (Exception ex)
      {
        this.m_context.Error((TemplateToken) eachExpressionToken, ex);
        collection1 = (IList) null;
      }
      current.SetCollection(collection1, bytes1);
      IList collection2 = current.Collection;
      if ((collection2 != null ? (collection2.Count > 0 ? 1 : 0) : 0) != 0)
      {
        current.SetContext(0);
        TemplateUnraveler.MappingState parent = current.Parent as TemplateUnraveler.MappingState;
        TemplateToken templateToken = parent.Value[parent.Index].Value;
        MappingToken mappingToken = templateToken as MappingToken;
        int bytes2 = 0;
        if (mappingToken == null)
        {
          if (templateToken is BasicExpressionToken basicExpressionToken)
          {
            try
            {
              mappingToken = basicExpressionToken.EvaluateMappingToken(current.Context, out bytes2);
            }
            catch (Exception ex)
            {
              this.m_context.Error((TemplateToken) basicExpressionToken, ex);
              mappingToken = new MappingToken(new int?(), new int?(), new int?());
            }
          }
          else
          {
            this.m_context.Error(templateToken, YamlStrings.ExpectedMapping());
            mappingToken = new MappingToken(new int?(), new int?(), new int?());
          }
        }
        this.m_current = current.Next(mappingToken, bytes2);
      }
      else
        current.End();
    }

    private void NextEachMappingInsertion()
    {
      this.m_current.Remove();
      TemplateUnraveler.EachExpressionState parent1 = this.m_current.Parent as TemplateUnraveler.EachExpressionState;
      this.m_current = (TemplateUnraveler.ReaderState) parent1;
      if (parent1.Index + 1 < parent1.Collection.Count)
      {
        parent1.SetContext(parent1.Index + 1);
        TemplateUnraveler.MappingState parent2 = parent1.Parent as TemplateUnraveler.MappingState;
        TemplateToken templateToken = parent2.Value[parent2.Index].Value;
        MappingToken mappingToken = templateToken as MappingToken;
        int bytes = 0;
        if (mappingToken == null)
        {
          if (templateToken is BasicExpressionToken basicExpressionToken)
          {
            try
            {
              mappingToken = basicExpressionToken.EvaluateMappingToken(parent1.Context, out bytes);
            }
            catch (Exception ex)
            {
              this.m_context.Error((TemplateToken) basicExpressionToken, ex);
              mappingToken = new MappingToken(new int?(), new int?(), new int?());
            }
          }
          else
          {
            this.m_context.Error(templateToken, YamlStrings.ExpectedMapping());
            mappingToken = new MappingToken(new int?(), new int?(), new int?());
          }
        }
        this.m_current = parent1.Next(mappingToken, bytes);
      }
      else
        parent1.End();
    }

    private void StartIfSequenceInsertion()
    {
      TemplateUnraveler.MappingState current = this.m_current as TemplateUnraveler.MappingState;
      TemplateUnraveler.IfExpressionState ifExpressionState = current.Next() as TemplateUnraveler.IfExpressionState;
      IfExpressionToken ifExpressionToken = ifExpressionState.Value;
      this.m_current = (TemplateUnraveler.ReaderState) ifExpressionState;
      bool flag;
      try
      {
        flag = ifExpressionToken.Evaluate(ifExpressionState.Context);
      }
      catch (Exception ex)
      {
        this.m_context.Error((TemplateToken) ifExpressionToken, ex);
        flag = false;
      }
      ifExpressionState.EvaluationResult = flag;
      if (flag)
      {
        SequenceToken sequenceToken = current.Value[0].Value as SequenceToken;
        this.m_current = ifExpressionState.Next(sequenceToken);
      }
      else
        ifExpressionState.End();
    }

    private void StartElseIfSequenceInsertion()
    {
      TemplateUnraveler.MappingState current = this.m_current as TemplateUnraveler.MappingState;
      TemplateUnraveler.ElseIfExpressionState ifExpressionState = current.Next() as TemplateUnraveler.ElseIfExpressionState;
      ElseIfExpressionToken ifExpressionToken = ifExpressionState.Value;
      this.m_current = (TemplateUnraveler.ReaderState) ifExpressionState;
      bool flag1 = false;
      if (this.m_last is TemplateUnraveler.IfExpressionState last2)
        flag1 = last2.EvaluationResult;
      else if (this.m_last is TemplateUnraveler.ElseIfExpressionState last1)
        flag1 = last1.EvaluationResult;
      if (flag1)
      {
        ifExpressionState.EvaluationResult = true;
        ifExpressionState.End();
      }
      else
      {
        bool flag2;
        try
        {
          flag2 = ifExpressionToken.Evaluate(ifExpressionState.Context);
        }
        catch (Exception ex)
        {
          this.m_context.Error((TemplateToken) ifExpressionToken, ex);
          flag2 = false;
        }
        ifExpressionState.EvaluationResult = flag2;
        if (flag2)
        {
          SequenceToken sequenceToken = current.Value[0].Value as SequenceToken;
          this.m_current = ifExpressionState.Next(sequenceToken);
        }
        else
          ifExpressionState.End();
      }
    }

    private void StartElseSequenceInsertion()
    {
      TemplateUnraveler.MappingState current = this.m_current as TemplateUnraveler.MappingState;
      TemplateUnraveler.ElseExpressionState elseExpressionState = current.Next() as TemplateUnraveler.ElseExpressionState;
      this.m_current = (TemplateUnraveler.ReaderState) elseExpressionState;
      bool flag = false;
      if (this.m_last is TemplateUnraveler.IfExpressionState last2)
        flag = last2.EvaluationResult;
      else if (this.m_last is TemplateUnraveler.ElseIfExpressionState last1)
        flag = last1.EvaluationResult;
      if (!flag)
      {
        SequenceToken sequenceToken = current.Value[0].Value as SequenceToken;
        this.m_current = elseExpressionState.Next(sequenceToken);
      }
      else
        elseExpressionState.End();
    }

    private void StartEachSequenceInsertion()
    {
      TemplateUnraveler.MappingState current = this.m_current as TemplateUnraveler.MappingState;
      TemplateUnraveler.EachExpressionState eachExpressionState = current.Next() as TemplateUnraveler.EachExpressionState;
      EachExpressionToken eachExpressionToken = eachExpressionState.Value;
      this.m_current = (TemplateUnraveler.ReaderState) eachExpressionState;
      int bytes = 0;
      IList collection1;
      try
      {
        collection1 = eachExpressionToken.Evaluate(eachExpressionState.Context, out bytes);
      }
      catch (Exception ex)
      {
        this.m_context.Error((TemplateToken) eachExpressionToken, ex);
        collection1 = (IList) null;
      }
      eachExpressionState.SetCollection(collection1, bytes);
      SequenceToken sequenceToken = current.Value[0].Value as SequenceToken;
      IList collection2 = eachExpressionState.Collection;
      if ((collection2 != null ? (collection2.Count > 0 ? 1 : 0) : 0) != 0 && sequenceToken.Count > 0)
      {
        eachExpressionState.SetContext(0);
        this.m_current = eachExpressionState.Next(sequenceToken);
      }
      else
        eachExpressionState.End();
    }

    private void NextEachSequenceInsertion()
    {
      SequenceToken sequenceToken = (this.m_current as TemplateUnraveler.SequenceState).Value;
      this.m_current.Remove();
      TemplateUnraveler.EachExpressionState parent = this.m_current.Parent as TemplateUnraveler.EachExpressionState;
      this.m_current = (TemplateUnraveler.ReaderState) parent;
      if (parent.Index + 1 < parent.Collection.Count)
      {
        parent.SetContext(parent.Index + 1);
        this.m_current = parent.Next(sequenceToken);
      }
      else
        parent.End();
    }

    private void EndExpression()
    {
      if (this.m_current.Parent == null)
      {
        this.m_current.Remove();
        this.m_current = (TemplateUnraveler.ReaderState) null;
      }
      else if (this.m_current is TemplateUnraveler.BasicExpressionState)
      {
        if (this.m_current.Parent is TemplateUnraveler.SequenceState parent1)
        {
          this.m_current.Remove();
          this.m_current = parent1.Next();
        }
        else
        {
          this.m_current.Remove();
          this.m_current = (this.m_current.Parent as TemplateUnraveler.MappingState).Next();
        }
      }
      else if (this.m_current is TemplateUnraveler.IfExpressionState current1 && current1.IsSequenceInsertion || this.m_current is TemplateUnraveler.ElseExpressionState current2 && current2.IsSequenceInsertion || this.m_current is TemplateUnraveler.ElseIfExpressionState current3 && current3.IsSequenceInsertion || this.m_current is TemplateUnraveler.EachExpressionState current4 && current4.IsSequenceInsertion)
      {
        this.m_current.Remove();
        this.m_current.Parent.Remove();
        TemplateUnraveler.SequenceState parent2 = this.m_current.Parent.Parent as TemplateUnraveler.SequenceState;
        this.m_last = this.m_current;
        this.m_current = parent2.Next();
      }
      else
      {
        this.m_current.Remove();
        TemplateUnraveler.MappingState parent3 = this.m_current.Parent as TemplateUnraveler.MappingState;
        parent3.Next().Remove();
        this.m_last = this.m_current;
        this.m_current = parent3.Next();
      }
    }

    private void UnexpectedState() => throw new InvalidOperationException("Expected state while unraveling expressions. " + this.DumpState());

    private abstract class ReaderState
    {
      public ReaderState(
        TemplateUnraveler.ReaderState parent,
        TemplateToken value,
        TemplateContext context)
      {
        this.Parent = parent;
        this.Value = value;
        this.Context = context;
      }

      public static TemplateUnraveler.ReaderState CreateState(
        TemplateUnraveler.ReaderState parent,
        TemplateToken value,
        TemplateContext context,
        int removeBytes = 0)
      {
        switch (value.Type)
        {
          case 0:
            return (TemplateUnraveler.ReaderState) new TemplateUnraveler.LiteralState(parent, value as LiteralToken, context, removeBytes);
          case 1:
            return (TemplateUnraveler.ReaderState) new TemplateUnraveler.SequenceState(parent, value as SequenceToken, context, removeBytes);
          case 2:
            return (TemplateUnraveler.ReaderState) new TemplateUnraveler.MappingState(parent, value as MappingToken, context, removeBytes);
          case 3:
            return (TemplateUnraveler.ReaderState) new TemplateUnraveler.BasicExpressionState(parent, value as BasicExpressionToken, context, removeBytes);
          case 4:
            if (removeBytes > 0)
              throw new InvalidOperationException("Unexpected removeBytes");
            return (TemplateUnraveler.ReaderState) new TemplateUnraveler.InsertExpressionState(parent, value as InsertExpressionToken, context);
          case 5:
            if (removeBytes > 0)
              throw new InvalidOperationException("Unexpected removeBytes");
            return (TemplateUnraveler.ReaderState) new TemplateUnraveler.IfExpressionState(parent, value as IfExpressionToken, context);
          case 6:
            if (removeBytes > 0)
              throw new InvalidOperationException("Unexpected removeBytes");
            return (TemplateUnraveler.ReaderState) new TemplateUnraveler.ElseIfExpressionState(parent, value as ElseIfExpressionToken, context);
          case 7:
            if (removeBytes > 0)
              throw new InvalidOperationException("Unexpected removeBytes");
            return (TemplateUnraveler.ReaderState) new TemplateUnraveler.ElseExpressionState(parent, value as ElseExpressionToken, context);
          case 8:
            if (removeBytes > 0)
              throw new InvalidOperationException("Unexpected removeBytes");
            return (TemplateUnraveler.ReaderState) new TemplateUnraveler.EachExpressionState(parent, value as EachExpressionToken, context);
          default:
            throw new NotSupportedException("Unexpected ReaderState type: " + value?.GetType().Name);
        }
      }

      public TemplateUnraveler.ReaderState Parent { get; }

      public TemplateContext Context { get; protected set; }

      public TemplateToken Value { get; }

      public abstract void Remove();
    }

    private abstract class ReaderState<T> : TemplateUnraveler.ReaderState where T : class
    {
      private T m_value;

      public ReaderState(
        TemplateUnraveler.ReaderState parent,
        TemplateToken value,
        TemplateContext context)
        : base(parent, value, context)
      {
      }

      public T Value
      {
        get
        {
          if (base.Value != (object) this.m_value)
            this.m_value = base.Value as T;
          return this.m_value;
        }
      }
    }

    private sealed class LiteralState : TemplateUnraveler.ReaderState<LiteralToken>
    {
      private int m_removeBytes;

      public LiteralState(
        TemplateUnraveler.ReaderState parent,
        LiteralToken literal,
        TemplateContext context,
        int removeBytes)
        : base(parent, (TemplateToken) literal, context)
      {
        context.Memory.AddBytes(literal);
        context.Memory.IncrementDepth();
        this.m_removeBytes = removeBytes;
      }

      public override void Remove()
      {
        this.Context.Memory.SubtractBytes(this.Value);
        this.Context.Memory.DecrementDepth();
        if (this.m_removeBytes <= 0)
          return;
        this.Context.Memory.SubtractBytes(this.m_removeBytes);
      }

      public override string ToString()
      {
        StringBuilder stringBuilder = new StringBuilder();
        stringBuilder.AppendLine(this.GetType().Name ?? "");
        return stringBuilder.ToString();
      }
    }

    private sealed class SequenceState : TemplateUnraveler.ReaderState<SequenceToken>
    {
      private int m_removeBytes;

      public SequenceState(
        TemplateUnraveler.ReaderState parent,
        SequenceToken sequence,
        TemplateContext context,
        int removeBytes)
        : base(parent, (TemplateToken) sequence, context)
      {
        context.Memory.AddBytes(sequence);
        context.Memory.IncrementDepth();
        this.m_removeBytes = removeBytes;
      }

      public bool IsStart { get; private set; } = true;

      public int Index { get; private set; }

      public bool IsEnd => !this.IsStart && this.Index >= this.Value.Count;

      public TemplateUnraveler.ReaderState Next()
      {
        if (this.IsStart)
          this.IsStart = false;
        else
          ++this.Index;
        return !this.IsEnd ? TemplateUnraveler.ReaderState.CreateState((TemplateUnraveler.ReaderState) this, this.Value[this.Index], this.Context) : (TemplateUnraveler.ReaderState) this;
      }

      public TemplateUnraveler.ReaderState End()
      {
        this.IsStart = false;
        this.Index = this.Value.Count;
        return (TemplateUnraveler.ReaderState) this;
      }

      public override void Remove()
      {
        this.Context.Memory.SubtractBytes(this.Value, false);
        this.Context.Memory.DecrementDepth();
        if (this.m_removeBytes <= 0)
          return;
        this.Context.Memory.SubtractBytes(this.m_removeBytes);
      }

      public override string ToString()
      {
        StringBuilder stringBuilder = new StringBuilder();
        stringBuilder.AppendLine(this.GetType().Name + ":");
        stringBuilder.AppendLine(string.Format("  IsStart: {0}", (object) this.IsStart));
        stringBuilder.AppendLine(string.Format("  Index: {0}", (object) this.Index));
        stringBuilder.AppendLine(string.Format("  IsEnd: {0}", (object) this.IsEnd));
        return stringBuilder.ToString();
      }
    }

    private sealed class MappingState : TemplateUnraveler.ReaderState<MappingToken>
    {
      private int m_removeBytes;

      public MappingState(
        TemplateUnraveler.ReaderState parent,
        MappingToken mapping,
        TemplateContext context,
        int removeBytes)
        : base(parent, (TemplateToken) mapping, context)
      {
        context.Memory.AddBytes(mapping);
        context.Memory.IncrementDepth();
        this.m_removeBytes = removeBytes;
      }

      public bool IsStart { get; private set; } = true;

      public int Index { get; private set; }

      public bool IsKey { get; private set; }

      public bool IsEnd => !this.IsStart && this.Index >= this.Value.Count;

      public TemplateUnraveler.ReaderState Next()
      {
        if (this.IsStart)
        {
          this.IsStart = false;
          this.IsKey = true;
        }
        else if (this.IsKey)
        {
          this.IsKey = false;
        }
        else
        {
          ++this.Index;
          this.IsKey = true;
        }
        if (this.IsEnd)
          return (TemplateUnraveler.ReaderState) this;
        return this.IsKey ? TemplateUnraveler.ReaderState.CreateState((TemplateUnraveler.ReaderState) this, (TemplateToken) this.Value[this.Index].Key, this.Context) : TemplateUnraveler.ReaderState.CreateState((TemplateUnraveler.ReaderState) this, this.Value[this.Index].Value, this.Context);
      }

      public TemplateUnraveler.ReaderState End()
      {
        this.IsStart = false;
        this.Index = this.Value.Count;
        return (TemplateUnraveler.ReaderState) this;
      }

      public override void Remove()
      {
        this.Context.Memory.SubtractBytes(this.Value, false);
        this.Context.Memory.DecrementDepth();
        if (this.m_removeBytes <= 0)
          return;
        this.Context.Memory.SubtractBytes(this.m_removeBytes);
      }

      public override string ToString()
      {
        StringBuilder stringBuilder = new StringBuilder();
        stringBuilder.AppendLine(this.GetType().Name + ":");
        stringBuilder.AppendLine(string.Format("  IsStart: {0}", (object) this.IsStart));
        stringBuilder.AppendLine(string.Format("  Index: {0}", (object) this.Index));
        stringBuilder.AppendLine(string.Format("  IsKey: {0}", (object) this.IsKey));
        stringBuilder.AppendLine(string.Format("  IsEnd: {0}", (object) this.IsEnd));
        return stringBuilder.ToString();
      }
    }

    private sealed class BasicExpressionState : TemplateUnraveler.ReaderState<BasicExpressionToken>
    {
      private int m_removeBytes;

      public BasicExpressionState(
        TemplateUnraveler.ReaderState parent,
        BasicExpressionToken expression,
        TemplateContext context,
        int removeBytes)
        : base(parent, (TemplateToken) expression, context)
      {
        context.Memory.AddBytes(expression);
        context.Memory.IncrementDepth();
        this.m_removeBytes = removeBytes;
      }

      public bool IsStart { get; private set; } = true;

      public bool IsEnd => !this.IsStart;

      public TemplateUnraveler.ReaderState Next(TemplateToken value, int removeBytes = 0)
      {
        this.IsStart = false;
        return TemplateUnraveler.ReaderState.CreateState((TemplateUnraveler.ReaderState) this, value, this.Context, removeBytes);
      }

      public TemplateUnraveler.ReaderState Next(
        SequenceToken value,
        bool isSequenceInsertion = false,
        int removeBytes = 0)
      {
        this.IsStart = false;
        TemplateUnraveler.ReaderState state = TemplateUnraveler.ReaderState.CreateState((TemplateUnraveler.ReaderState) this, (TemplateToken) value, this.Context, removeBytes);
        return isSequenceInsertion ? (state as TemplateUnraveler.SequenceState).Next() : state;
      }

      public TemplateUnraveler.ReaderState End()
      {
        this.IsStart = false;
        return (TemplateUnraveler.ReaderState) this;
      }

      public override void Remove()
      {
        this.Context.Memory.SubtractBytes(this.Value);
        this.Context.Memory.DecrementDepth();
        if (this.m_removeBytes <= 0)
          return;
        this.Context.Memory.SubtractBytes(this.m_removeBytes);
      }

      public override string ToString()
      {
        StringBuilder stringBuilder = new StringBuilder();
        stringBuilder.AppendLine(this.GetType().Name + ":");
        stringBuilder.AppendLine(string.Format("  IsStart: {0}", (object) this.IsStart));
        return stringBuilder.ToString();
      }
    }

    private sealed class InsertExpressionState : TemplateUnraveler.ReaderState<InsertExpressionToken>
    {
      public InsertExpressionState(
        TemplateUnraveler.ReaderState parent,
        InsertExpressionToken expression,
        TemplateContext context)
        : base(parent, (TemplateToken) expression, context)
      {
        this.Context.Memory.AddBytes(expression);
        this.Context.Memory.IncrementDepth();
      }

      public bool IsStart { get; private set; } = true;

      public bool IsEnd => !this.IsStart;

      public TemplateUnraveler.ReaderState Next(MappingToken value, int removeBytes = 0)
      {
        this.IsStart = false;
        return (TemplateUnraveler.ReaderState.CreateState((TemplateUnraveler.ReaderState) this, (TemplateToken) value, this.Context, removeBytes) as TemplateUnraveler.MappingState).Next();
      }

      public TemplateUnraveler.ReaderState End()
      {
        this.IsStart = false;
        return (TemplateUnraveler.ReaderState) this;
      }

      public TemplateUnraveler.ReaderState ToLiteral() => TemplateUnraveler.ReaderState.CreateState(this.Parent, (TemplateToken) new LiteralToken(this.Value.FileId, this.Value.Line, this.Value.Column, "${{ " + this.Value.Directive + " }}"), this.Context);

      public override void Remove()
      {
        this.Context.Memory.SubtractBytes(this.Value);
        this.Context.Memory.DecrementDepth();
      }

      public override string ToString()
      {
        StringBuilder stringBuilder = new StringBuilder();
        stringBuilder.AppendLine(this.GetType().Name + ":");
        stringBuilder.AppendLine(string.Format("  IsStart: {0}", (object) this.IsStart));
        return stringBuilder.ToString();
      }
    }

    private sealed class IfExpressionState : TemplateUnraveler.ReaderState<IfExpressionToken>
    {
      public IfExpressionState(
        TemplateUnraveler.ReaderState parent,
        IfExpressionToken expression,
        TemplateContext context)
        : base(parent, (TemplateToken) expression, context)
      {
        context.Memory.AddBytes(expression);
        context.Memory.IncrementDepth();
        if (!(parent is TemplateUnraveler.MappingState mappingState) || !mappingState.IsKey || !(mappingState.Value[mappingState.Index].Value is SequenceToken))
          return;
        this.IsSequenceInsertion = true;
      }

      public bool IsSequenceInsertion { get; }

      public bool IsStart { get; private set; } = true;

      public bool IsEnd => !this.IsStart;

      public bool EvaluationResult { get; set; }

      public TemplateUnraveler.ReaderState Next(SequenceToken value)
      {
        this.IsStart = false;
        return (TemplateUnraveler.ReaderState.CreateState((TemplateUnraveler.ReaderState) this, (TemplateToken) value, this.Context) as TemplateUnraveler.SequenceState).Next();
      }

      public TemplateUnraveler.ReaderState Next(MappingToken value, int removeBytes)
      {
        this.IsStart = false;
        return (TemplateUnraveler.ReaderState.CreateState((TemplateUnraveler.ReaderState) this, (TemplateToken) value, this.Context, removeBytes) as TemplateUnraveler.MappingState).Next();
      }

      public TemplateUnraveler.ReaderState End()
      {
        this.IsStart = false;
        return (TemplateUnraveler.ReaderState) this;
      }

      public TemplateUnraveler.ReaderState ToLiteral() => TemplateUnraveler.ReaderState.CreateState(this.Parent, (TemplateToken) new LiteralToken(this.Value.FileId, this.Value.Line, this.Value.Column, "${{ " + this.Value.Directive + " " + this.Value.Expression + " }}"), this.Context);

      public override void Remove()
      {
        this.Context.Memory.SubtractBytes(this.Value);
        this.Context.Memory.DecrementDepth();
      }

      public override string ToString()
      {
        StringBuilder stringBuilder = new StringBuilder();
        stringBuilder.AppendLine(this.GetType().Name + ":");
        stringBuilder.AppendLine(string.Format("  IsSequenceInsertion: {0}", (object) this.IsSequenceInsertion));
        stringBuilder.AppendLine(string.Format("  IsStart: {0}", (object) this.IsStart));
        return stringBuilder.ToString();
      }
    }

    private sealed class ElseIfExpressionState : TemplateUnraveler.ReaderState<ElseIfExpressionToken>
    {
      public ElseIfExpressionState(
        TemplateUnraveler.ReaderState parent,
        ElseIfExpressionToken expression,
        TemplateContext context)
        : base(parent, (TemplateToken) expression, context)
      {
        context.Memory.AddBytes((TemplateToken) expression);
        context.Memory.IncrementDepth();
        if (!(parent is TemplateUnraveler.MappingState mappingState) || !mappingState.IsKey || !(mappingState.Value[mappingState.Index].Value is SequenceToken))
          return;
        this.IsSequenceInsertion = true;
      }

      public bool IsSequenceInsertion { get; }

      public bool IsStart { get; private set; } = true;

      public bool IsEnd => !this.IsStart;

      public bool EvaluationResult { get; set; }

      public TemplateUnraveler.ReaderState Next(SequenceToken value)
      {
        this.IsStart = false;
        return (TemplateUnraveler.ReaderState.CreateState((TemplateUnraveler.ReaderState) this, (TemplateToken) value, this.Context) as TemplateUnraveler.SequenceState).Next();
      }

      public TemplateUnraveler.ReaderState Next(MappingToken value, int removeBytes)
      {
        this.IsStart = false;
        return (TemplateUnraveler.ReaderState.CreateState((TemplateUnraveler.ReaderState) this, (TemplateToken) value, this.Context, removeBytes) as TemplateUnraveler.MappingState).Next();
      }

      public TemplateUnraveler.ReaderState End()
      {
        this.IsStart = false;
        return (TemplateUnraveler.ReaderState) this;
      }

      public TemplateUnraveler.ReaderState ToLiteral() => TemplateUnraveler.ReaderState.CreateState(this.Parent, (TemplateToken) new LiteralToken(this.Value.FileId, this.Value.Line, this.Value.Column, "${{ " + this.Value.Directive + " " + this.Value.Expression + " }}"), this.Context);

      public override void Remove()
      {
        this.Context.Memory.SubtractBytes((TemplateToken) this.Value);
        this.Context.Memory.DecrementDepth();
      }

      public override string ToString()
      {
        StringBuilder stringBuilder = new StringBuilder();
        stringBuilder.AppendLine(this.GetType().Name + ":");
        stringBuilder.AppendLine(string.Format("  IsSequenceInsertion: {0}", (object) this.IsSequenceInsertion));
        stringBuilder.AppendLine(string.Format("  IsStart: {0}", (object) this.IsStart));
        return stringBuilder.ToString();
      }
    }

    private sealed class ElseExpressionState : TemplateUnraveler.ReaderState<ElseExpressionToken>
    {
      public ElseExpressionState(
        TemplateUnraveler.ReaderState parent,
        ElseExpressionToken expression,
        TemplateContext context)
        : base(parent, (TemplateToken) expression, context)
      {
        context.Memory.AddBytes((TemplateToken) expression);
        context.Memory.IncrementDepth();
        if (!(parent is TemplateUnraveler.MappingState mappingState) || !mappingState.IsKey || !(mappingState.Value[mappingState.Index].Value is SequenceToken))
          return;
        this.IsSequenceInsertion = true;
      }

      public bool IsSequenceInsertion { get; }

      public bool IsStart { get; private set; } = true;

      public bool IsEnd => !this.IsStart;

      public TemplateUnraveler.ReaderState Next(SequenceToken value)
      {
        this.IsStart = false;
        return (TemplateUnraveler.ReaderState.CreateState((TemplateUnraveler.ReaderState) this, (TemplateToken) value, this.Context) as TemplateUnraveler.SequenceState).Next();
      }

      public TemplateUnraveler.ReaderState Next(MappingToken value, int removeBytes)
      {
        this.IsStart = false;
        return (TemplateUnraveler.ReaderState.CreateState((TemplateUnraveler.ReaderState) this, (TemplateToken) value, this.Context, removeBytes) as TemplateUnraveler.MappingState).Next();
      }

      public TemplateUnraveler.ReaderState End()
      {
        this.IsStart = false;
        return (TemplateUnraveler.ReaderState) this;
      }

      public TemplateUnraveler.ReaderState ToLiteral() => TemplateUnraveler.ReaderState.CreateState(this.Parent, (TemplateToken) new LiteralToken(this.Value.FileId, this.Value.Line, this.Value.Column, "${{ " + this.Value.Directive + " }}"), this.Context);

      public override void Remove()
      {
        this.Context.Memory.SubtractBytes((TemplateToken) this.Value);
        this.Context.Memory.DecrementDepth();
      }

      public override string ToString()
      {
        StringBuilder stringBuilder = new StringBuilder();
        stringBuilder.AppendLine(this.GetType().Name + ":");
        stringBuilder.AppendLine(string.Format("  IsSequenceInsertion: {0}", (object) this.IsSequenceInsertion));
        stringBuilder.AppendLine(string.Format("  IsStart: {0}", (object) this.IsStart));
        return stringBuilder.ToString();
      }
    }

    private sealed class EachExpressionState : TemplateUnraveler.ReaderState<EachExpressionToken>
    {
      private int m_collectionBytes;
      private TemplateContext m_originalContext;

      public EachExpressionState(
        TemplateUnraveler.ReaderState parent,
        EachExpressionToken expression,
        TemplateContext context)
        : base(parent, (TemplateToken) expression, context)
      {
        context.Memory.AddBytes(expression);
        context.Memory.IncrementDepth();
        if (parent is TemplateUnraveler.MappingState mappingState && mappingState.IsKey && mappingState.Value[mappingState.Index].Value is SequenceToken)
          this.IsSequenceInsertion = true;
        this.m_originalContext = context;
      }

      public bool IsSequenceInsertion { get; }

      public bool IsStart { get; private set; } = true;

      public IList Collection { get; private set; }

      public int Index { get; private set; }

      public bool IsEnd
      {
        get
        {
          if (this.IsStart)
            return false;
          int index = this.Index;
          IList collection = this.Collection;
          int count = collection != null ? collection.Count : 0;
          return index >= count;
        }
      }

      public void SetCollection(IList collection, int bytes)
      {
        this.Collection = collection;
        this.m_collectionBytes = bytes;
      }

      public void SetContext(int index)
      {
        TemplateContext templateContext = this.m_originalContext.NewScope();
        this.Context = templateContext;
        templateContext.ExpressionValues[this.Value.Identifier] = this.Collection[index];
      }

      public TemplateUnraveler.ReaderState Next(SequenceToken value)
      {
        if (this.IsStart)
          this.IsStart = false;
        else
          ++this.Index;
        return (TemplateUnraveler.ReaderState.CreateState((TemplateUnraveler.ReaderState) this, (TemplateToken) value, this.Context) as TemplateUnraveler.SequenceState).Next();
      }

      public TemplateUnraveler.ReaderState Next(MappingToken value, int removeBytes = 0)
      {
        if (this.IsStart)
          this.IsStart = false;
        else
          ++this.Index;
        return (TemplateUnraveler.ReaderState.CreateState((TemplateUnraveler.ReaderState) this, (TemplateToken) value, this.Context, removeBytes) as TemplateUnraveler.MappingState).Next();
      }

      public TemplateUnraveler.ReaderState End()
      {
        this.IsStart = false;
        IList collection = this.Collection;
        this.Index = collection != null ? collection.Count : 0;
        return (TemplateUnraveler.ReaderState) this;
      }

      public TemplateUnraveler.ReaderState ToLiteral() => TemplateUnraveler.ReaderState.CreateState(this.Parent, (TemplateToken) new LiteralToken(this.Value.FileId, this.Value.Line, this.Value.Column, "${{ " + this.Value.Directive + " " + this.Value.Identifier + " in " + this.Value.Expression + " }}"), this.Context);

      public override void Remove()
      {
        this.Context.Memory.SubtractBytes(this.Value);
        if (this.m_collectionBytes > 0)
          this.Context.Memory.SubtractBytes(this.m_collectionBytes);
        this.Context.Memory.DecrementDepth();
      }

      public override string ToString()
      {
        StringBuilder stringBuilder = new StringBuilder();
        stringBuilder.AppendLine(this.GetType().Name + ":");
        stringBuilder.AppendLine(string.Format("  IsSequenceInsertion: {0}", (object) this.IsSequenceInsertion));
        stringBuilder.AppendLine(string.Format("  IsStart: {0}", (object) this.IsStart));
        stringBuilder.AppendLine(string.Format("  Index: {0}", (object) this.Index));
        stringBuilder.AppendLine(string.Format("  IsEnd: {0}", (object) this.IsEnd));
        return stringBuilder.ToString();
      }
    }
  }
}
