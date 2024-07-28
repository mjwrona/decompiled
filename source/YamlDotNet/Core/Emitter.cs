// Decompiled with JetBrains decompiler
// Type: YamlDotNet.Core.Emitter
// Assembly: YamlDotNet, Version=5.0.0.0, Culture=neutral, PublicKeyToken=ec19458f3c15af5e
// MVID: 5F9DD5C4-A41D-46B2-A793-8157A0D55AB5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\YamlDotNet.dll

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using YamlDotNet.Core.Events;
using YamlDotNet.Core.Tokens;

namespace YamlDotNet.Core
{
  public class Emitter : IEmitter
  {
    private const int MinBestIndent = 2;
    private const int MaxBestIndent = 9;
    private const int MaxAliasLength = 128;
    private static readonly Regex uriReplacer = new Regex("[^0-9A-Za-z_\\-;?@=$~\\\\\\)\\]/:&+,\\.\\*\\(\\[!]", RegexOptions.Compiled | RegexOptions.Singleline);
    private readonly TextWriter output;
    private readonly bool outputUsesUnicodeEncoding;
    private readonly bool isCanonical;
    private readonly int bestIndent;
    private readonly int bestWidth;
    private EmitterState state;
    private readonly Stack<EmitterState> states = new Stack<EmitterState>();
    private readonly Queue<ParsingEvent> events = new Queue<ParsingEvent>();
    private readonly Stack<int> indents = new Stack<int>();
    private readonly TagDirectiveCollection tagDirectives = new TagDirectiveCollection();
    private int indent;
    private int flowLevel;
    private bool isMappingContext;
    private bool isSimpleKeyContext;
    private bool isRootContext;
    private int column;
    private bool isWhitespace;
    private bool isIndentation;
    private bool isOpenEnded;
    private bool isDocumentEndWritten;
    private readonly Emitter.AnchorData anchorData = new Emitter.AnchorData();
    private readonly Emitter.TagData tagData = new Emitter.TagData();
    private readonly Emitter.ScalarData scalarData = new Emitter.ScalarData();

    public Emitter(TextWriter output)
      : this(output, 2)
    {
    }

    public Emitter(TextWriter output, int bestIndent)
      : this(output, bestIndent, int.MaxValue)
    {
    }

    public Emitter(TextWriter output, int bestIndent, int bestWidth)
      : this(output, bestIndent, bestWidth, false)
    {
    }

    public Emitter(TextWriter output, int bestIndent, int bestWidth, bool isCanonical)
    {
      this.bestIndent = bestIndent >= 2 && bestIndent <= 9 ? bestIndent : throw new ArgumentOutOfRangeException(nameof (bestIndent), string.Format((IFormatProvider) CultureInfo.InvariantCulture, "The bestIndent parameter must be between {0} and {1}.", new object[2]
      {
        (object) 2,
        (object) 9
      }));
      this.bestWidth = bestWidth > bestIndent * 2 ? bestWidth : throw new ArgumentOutOfRangeException(nameof (bestWidth), "The bestWidth parameter must be greater than bestIndent * 2.");
      this.isCanonical = isCanonical;
      this.output = output;
      this.outputUsesUnicodeEncoding = this.IsUnicode(output.Encoding);
    }

    public void Emit(ParsingEvent @event)
    {
      this.events.Enqueue(@event);
      while (!this.NeedMoreEvents())
      {
        ParsingEvent evt = this.events.Peek();
        try
        {
          this.AnalyzeEvent(evt);
          this.StateMachine(evt);
        }
        finally
        {
          this.events.Dequeue();
        }
      }
    }

    private bool NeedMoreEvents()
    {
      if (this.events.Count == 0)
        return true;
      int num1;
      switch (this.events.Peek().Type)
      {
        case EventType.DocumentStart:
          num1 = 1;
          break;
        case EventType.SequenceStart:
          num1 = 2;
          break;
        case EventType.MappingStart:
          num1 = 3;
          break;
        default:
          return false;
      }
      if (this.events.Count > num1)
        return false;
      int num2 = 0;
      foreach (ParsingEvent parsingEvent in this.events)
      {
        switch (parsingEvent.Type)
        {
          case EventType.DocumentStart:
          case EventType.SequenceStart:
          case EventType.MappingStart:
            ++num2;
            break;
          case EventType.DocumentEnd:
          case EventType.SequenceEnd:
          case EventType.MappingEnd:
            --num2;
            break;
        }
        if (num2 == 0)
          return false;
      }
      return true;
    }

    private void AnalyzeEvent(ParsingEvent evt)
    {
      this.anchorData.anchor = (string) null;
      this.tagData.handle = (string) null;
      this.tagData.suffix = (string) null;
      switch (evt)
      {
        case YamlDotNet.Core.Events.AnchorAlias anchorAlias:
          this.AnalyzeAnchor(anchorAlias.Value, true);
          break;
        case NodeEvent nodeEvent:
          if (evt is YamlDotNet.Core.Events.Scalar scalar)
            this.AnalyzeScalar(scalar);
          this.AnalyzeAnchor(nodeEvent.Anchor, false);
          if (string.IsNullOrEmpty(nodeEvent.Tag) || !this.isCanonical && !nodeEvent.IsCanonical)
            break;
          this.AnalyzeTag(nodeEvent.Tag);
          break;
      }
    }

    private void AnalyzeAnchor(string anchor, bool isAlias)
    {
      this.anchorData.anchor = anchor;
      this.anchorData.isAlias = isAlias;
    }

    private void AnalyzeScalar(YamlDotNet.Core.Events.Scalar scalar)
    {
      string str = scalar.Value;
      this.scalarData.value = str;
      if (str.Length == 0)
      {
        if (scalar.Tag == "tag:yaml.org,2002:null")
        {
          this.scalarData.isMultiline = false;
          this.scalarData.isFlowPlainAllowed = false;
          this.scalarData.isBlockPlainAllowed = true;
          this.scalarData.isSingleQuotedAllowed = false;
          this.scalarData.isBlockAllowed = false;
        }
        else
        {
          this.scalarData.isMultiline = false;
          this.scalarData.isFlowPlainAllowed = false;
          this.scalarData.isBlockPlainAllowed = false;
          this.scalarData.isSingleQuotedAllowed = true;
          this.scalarData.isBlockAllowed = false;
        }
      }
      else
      {
        bool flag1 = false;
        bool flag2 = false;
        if (str.StartsWith("---", StringComparison.Ordinal) || str.StartsWith("...", StringComparison.Ordinal))
        {
          flag1 = true;
          flag2 = true;
        }
        CharacterAnalyzer<StringLookAheadBuffer> characterAnalyzer = new CharacterAnalyzer<StringLookAheadBuffer>(new StringLookAheadBuffer(str));
        bool flag3 = true;
        bool flag4 = characterAnalyzer.IsWhiteBreakOrZero(1);
        bool flag5 = false;
        bool flag6 = false;
        bool flag7 = false;
        bool flag8 = false;
        bool flag9 = false;
        bool flag10 = false;
        bool flag11 = false;
        bool flag12 = false;
        bool flag13 = false;
        bool flag14 = false;
        bool flag15 = !this.ValueIsRepresentableInOutputEncoding(str);
        bool flag16 = false;
        bool flag17 = true;
        while (!characterAnalyzer.EndOfInput)
        {
          if (flag17)
          {
            if (characterAnalyzer.Check("#,[]{}&*!|>\\\"%@`'"))
            {
              flag1 = true;
              flag2 = true;
              flag9 = characterAnalyzer.Check('\'');
              flag16 |= characterAnalyzer.Check('\'');
            }
            if (characterAnalyzer.Check("?:"))
            {
              flag1 = true;
              if (flag4)
                flag2 = true;
            }
            if (characterAnalyzer.Check('-') & flag4)
            {
              flag1 = true;
              flag2 = true;
            }
          }
          else
          {
            if (characterAnalyzer.Check(",?[]{}"))
              flag1 = true;
            if (characterAnalyzer.Check(':'))
            {
              flag1 = true;
              if (flag4)
                flag2 = true;
            }
            if (characterAnalyzer.Check('#') & flag3)
            {
              flag1 = true;
              flag2 = true;
            }
            flag16 |= characterAnalyzer.Check('\'');
          }
          if (!flag15 && !characterAnalyzer.IsPrintable())
            flag15 = true;
          if (characterAnalyzer.IsBreak())
            flag14 = true;
          if (characterAnalyzer.IsSpace())
          {
            if (flag17)
              flag5 = true;
            if (characterAnalyzer.Buffer.Position >= characterAnalyzer.Buffer.Length - 1)
              flag7 = true;
            if (flag13)
              flag10 = true;
            flag12 = true;
            flag13 = false;
          }
          else if (characterAnalyzer.IsBreak())
          {
            if (flag17)
              flag6 = true;
            if (characterAnalyzer.Buffer.Position >= characterAnalyzer.Buffer.Length - 1)
              flag8 = true;
            if (flag12)
              flag11 = true;
            flag12 = false;
            flag13 = true;
          }
          else
          {
            flag12 = false;
            flag13 = false;
          }
          flag3 = characterAnalyzer.IsWhiteBreakOrZero();
          characterAnalyzer.Skip(1);
          if (!characterAnalyzer.EndOfInput)
            flag4 = characterAnalyzer.IsWhiteBreakOrZero(1);
          flag17 = false;
        }
        this.scalarData.isFlowPlainAllowed = true;
        this.scalarData.isBlockPlainAllowed = true;
        this.scalarData.isSingleQuotedAllowed = true;
        this.scalarData.isBlockAllowed = true;
        if (flag5 | flag6 | flag7 | flag8 | flag9)
        {
          this.scalarData.isFlowPlainAllowed = false;
          this.scalarData.isBlockPlainAllowed = false;
        }
        if (flag7)
          this.scalarData.isBlockAllowed = false;
        if (flag10)
        {
          this.scalarData.isFlowPlainAllowed = false;
          this.scalarData.isBlockPlainAllowed = false;
          this.scalarData.isSingleQuotedAllowed = false;
        }
        if (flag11 | flag15)
        {
          this.scalarData.isFlowPlainAllowed = false;
          this.scalarData.isBlockPlainAllowed = false;
          this.scalarData.isSingleQuotedAllowed = false;
          this.scalarData.isBlockAllowed = false;
        }
        this.scalarData.isMultiline = flag14;
        if (flag14)
        {
          this.scalarData.isFlowPlainAllowed = false;
          this.scalarData.isBlockPlainAllowed = false;
        }
        if (flag1)
          this.scalarData.isFlowPlainAllowed = false;
        if (flag2)
          this.scalarData.isBlockPlainAllowed = false;
        this.scalarData.hasSingleQuotes = flag16;
      }
    }

    private bool ValueIsRepresentableInOutputEncoding(string value)
    {
      if (this.outputUsesUnicodeEncoding)
        return true;
      try
      {
        byte[] bytes = this.output.Encoding.GetBytes(value);
        return this.output.Encoding.GetString(bytes, 0, bytes.Length).Equals(value);
      }
      catch (EncoderFallbackException ex)
      {
        return false;
      }
      catch (ArgumentOutOfRangeException ex)
      {
        return false;
      }
    }

    private bool IsUnicode(Encoding encoding)
    {
      switch (encoding)
      {
        case UTF8Encoding _:
        case UnicodeEncoding _:
          return true;
        default:
          return encoding is UTF7Encoding;
      }
    }

    private void AnalyzeTag(string tag)
    {
      this.tagData.handle = tag;
      foreach (TagDirective tagDirective in (Collection<TagDirective>) this.tagDirectives)
      {
        if (tag.StartsWith(tagDirective.Prefix, StringComparison.Ordinal))
        {
          this.tagData.handle = tagDirective.Handle;
          this.tagData.suffix = tag.Substring(tagDirective.Prefix.Length);
          break;
        }
      }
    }

    private void StateMachine(ParsingEvent evt)
    {
      if (evt is YamlDotNet.Core.Events.Comment comment)
      {
        this.EmitComment(comment);
      }
      else
      {
        switch (this.state)
        {
          case EmitterState.StreamStart:
            this.EmitStreamStart(evt);
            break;
          case EmitterState.StreamEnd:
            throw new YamlException("Expected nothing after STREAM-END");
          case EmitterState.FirstDocumentStart:
            this.EmitDocumentStart(evt, true);
            break;
          case EmitterState.DocumentStart:
            this.EmitDocumentStart(evt, false);
            break;
          case EmitterState.DocumentContent:
            this.EmitDocumentContent(evt);
            break;
          case EmitterState.DocumentEnd:
            this.EmitDocumentEnd(evt);
            break;
          case EmitterState.FlowSequenceFirstItem:
            this.EmitFlowSequenceItem(evt, true);
            break;
          case EmitterState.FlowSequenceItem:
            this.EmitFlowSequenceItem(evt, false);
            break;
          case EmitterState.FlowMappingFirstKey:
            this.EmitFlowMappingKey(evt, true);
            break;
          case EmitterState.FlowMappingKey:
            this.EmitFlowMappingKey(evt, false);
            break;
          case EmitterState.FlowMappingSimpleValue:
            this.EmitFlowMappingValue(evt, true);
            break;
          case EmitterState.FlowMappingValue:
            this.EmitFlowMappingValue(evt, false);
            break;
          case EmitterState.BlockSequenceFirstItem:
            this.EmitBlockSequenceItem(evt, true);
            break;
          case EmitterState.BlockSequenceItem:
            this.EmitBlockSequenceItem(evt, false);
            break;
          case EmitterState.BlockMappingFirstKey:
            this.EmitBlockMappingKey(evt, true);
            break;
          case EmitterState.BlockMappingKey:
            this.EmitBlockMappingKey(evt, false);
            break;
          case EmitterState.BlockMappingSimpleValue:
            this.EmitBlockMappingValue(evt, true);
            break;
          case EmitterState.BlockMappingValue:
            this.EmitBlockMappingValue(evt, false);
            break;
          default:
            throw new InvalidOperationException();
        }
      }
    }

    private void EmitComment(YamlDotNet.Core.Events.Comment comment)
    {
      if (comment.IsInline)
        this.Write(' ');
      else
        this.WriteIndent();
      this.Write("# ");
      this.Write(comment.Value);
      this.WriteBreak();
      this.isIndentation = true;
    }

    private void EmitStreamStart(ParsingEvent evt)
    {
      if (!(evt is YamlDotNet.Core.Events.StreamStart))
        throw new ArgumentException("Expected STREAM-START.", nameof (evt));
      this.indent = -1;
      this.column = 0;
      this.isWhitespace = true;
      this.isIndentation = true;
      this.state = EmitterState.FirstDocumentStart;
    }

    private void EmitDocumentStart(ParsingEvent evt, bool isFirst)
    {
      switch (evt)
      {
        case YamlDotNet.Core.Events.DocumentStart documentStart:
          bool flag = documentStart.IsImplicit & isFirst && !this.isCanonical;
          TagDirectiveCollection tagDirectives = this.NonDefaultTagsAmong((IEnumerable<TagDirective>) documentStart.Tags);
          if (!isFirst && !this.isDocumentEndWritten && (documentStart.Version != null || tagDirectives.Count > 0))
          {
            this.isDocumentEndWritten = false;
            this.WriteIndicator("...", true, false, false);
            this.WriteIndent();
          }
          if (documentStart.Version != null)
          {
            this.AnalyzeVersionDirective(documentStart.Version);
            flag = false;
            this.WriteIndicator("%YAML", true, false, false);
            this.WriteIndicator(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}.{1}", new object[2]
            {
              (object) 1,
              (object) 1
            }), true, false, false);
            this.WriteIndent();
          }
          foreach (TagDirective tagDirective in (Collection<TagDirective>) tagDirectives)
            Emitter.AppendTagDirectiveTo(tagDirective, false, this.tagDirectives);
          foreach (TagDirective defaultTagDirective in Constants.DefaultTagDirectives)
            Emitter.AppendTagDirectiveTo(defaultTagDirective, true, this.tagDirectives);
          if (tagDirectives.Count > 0)
          {
            flag = false;
            foreach (TagDirective defaultTagDirective in Constants.DefaultTagDirectives)
              Emitter.AppendTagDirectiveTo(defaultTagDirective, true, tagDirectives);
            foreach (TagDirective tagDirective in (Collection<TagDirective>) tagDirectives)
            {
              this.WriteIndicator("%TAG", true, false, false);
              this.WriteTagHandle(tagDirective.Handle);
              this.WriteTagContent(tagDirective.Prefix, true);
              this.WriteIndent();
            }
          }
          if (this.CheckEmptyDocument())
            flag = false;
          if (!flag)
          {
            this.WriteIndent();
            this.WriteIndicator("---", true, false, false);
            if (this.isCanonical)
              this.WriteIndent();
          }
          this.state = EmitterState.DocumentContent;
          break;
        case YamlDotNet.Core.Events.StreamEnd _:
          if (this.isOpenEnded)
          {
            this.WriteIndicator("...", true, false, false);
            this.WriteIndent();
          }
          this.state = EmitterState.StreamEnd;
          break;
        default:
          throw new YamlException("Expected DOCUMENT-START or STREAM-END");
      }
    }

    private TagDirectiveCollection NonDefaultTagsAmong(IEnumerable<TagDirective> tagCollection)
    {
      TagDirectiveCollection tagDirectives = new TagDirectiveCollection();
      if (tagCollection == null)
        return tagDirectives;
      foreach (TagDirective tag in tagCollection)
        Emitter.AppendTagDirectiveTo(tag, false, tagDirectives);
      foreach (TagDirective defaultTagDirective in Constants.DefaultTagDirectives)
        tagDirectives.Remove(defaultTagDirective);
      return tagDirectives;
    }

    private void AnalyzeVersionDirective(VersionDirective versionDirective)
    {
      if (versionDirective.Version.Major != 1 || versionDirective.Version.Minor != 1)
        throw new YamlException("Incompatible %YAML directive");
    }

    private static void AppendTagDirectiveTo(
      TagDirective value,
      bool allowDuplicates,
      TagDirectiveCollection tagDirectives)
    {
      if (tagDirectives.Contains(value))
      {
        if (!allowDuplicates)
          throw new YamlException("Duplicate %TAG directive.");
      }
      else
        tagDirectives.Add(value);
    }

    private void EmitDocumentContent(ParsingEvent evt)
    {
      this.states.Push(EmitterState.DocumentEnd);
      this.EmitNode(evt, true, false, false);
    }

    private void EmitNode(ParsingEvent evt, bool isRoot, bool isMapping, bool isSimpleKey)
    {
      this.isRootContext = isRoot;
      this.isMappingContext = isMapping;
      this.isSimpleKeyContext = isSimpleKey;
      switch (evt.Type)
      {
        case EventType.Alias:
          this.EmitAlias();
          break;
        case EventType.Scalar:
          this.EmitScalar(evt);
          break;
        case EventType.SequenceStart:
          this.EmitSequenceStart(evt);
          break;
        case EventType.MappingStart:
          this.EmitMappingStart(evt);
          break;
        default:
          throw new YamlException(string.Format("Expected SCALAR, SEQUENCE-START, MAPPING-START, or ALIAS, got {0}", (object) evt.Type));
      }
    }

    private void EmitAlias()
    {
      this.ProcessAnchor();
      this.state = this.states.Pop();
    }

    private void EmitScalar(ParsingEvent evt)
    {
      this.SelectScalarStyle(evt);
      this.ProcessAnchor();
      this.ProcessTag();
      this.IncreaseIndent(true, false);
      this.ProcessScalar();
      this.indent = this.indents.Pop();
      this.state = this.states.Pop();
    }

    private void SelectScalarStyle(ParsingEvent evt)
    {
      YamlDotNet.Core.Events.Scalar scalar = (YamlDotNet.Core.Events.Scalar) evt;
      ScalarStyle scalarStyle = scalar.Style;
      bool flag = this.tagData.handle == null && this.tagData.suffix == null;
      if (flag && !scalar.IsPlainImplicit && !scalar.IsQuotedImplicit)
        throw new YamlException("Neither tag nor isImplicit flags are specified.");
      if (scalarStyle == ScalarStyle.Any)
        scalarStyle = this.scalarData.isMultiline ? ScalarStyle.Folded : ScalarStyle.Plain;
      if (this.isCanonical)
        scalarStyle = ScalarStyle.DoubleQuoted;
      if (this.isSimpleKeyContext && this.scalarData.isMultiline)
        scalarStyle = ScalarStyle.DoubleQuoted;
      if (scalarStyle == ScalarStyle.Plain)
      {
        if (this.flowLevel != 0 && !this.scalarData.isFlowPlainAllowed || this.flowLevel == 0 && !this.scalarData.isBlockPlainAllowed)
          scalarStyle = !this.scalarData.isSingleQuotedAllowed || this.scalarData.hasSingleQuotes ? ScalarStyle.DoubleQuoted : ScalarStyle.SingleQuoted;
        if (string.IsNullOrEmpty(this.scalarData.value) && (this.flowLevel != 0 || this.isSimpleKeyContext))
          scalarStyle = ScalarStyle.SingleQuoted;
        if (flag && !scalar.IsPlainImplicit)
          scalarStyle = ScalarStyle.SingleQuoted;
      }
      if (scalarStyle == ScalarStyle.SingleQuoted && !this.scalarData.isSingleQuotedAllowed)
        scalarStyle = ScalarStyle.DoubleQuoted;
      if ((scalarStyle == ScalarStyle.Literal || scalarStyle == ScalarStyle.Folded) && (!this.scalarData.isBlockAllowed || this.flowLevel != 0 || this.isSimpleKeyContext))
        scalarStyle = ScalarStyle.DoubleQuoted;
      this.scalarData.style = scalarStyle;
    }

    private void ProcessScalar()
    {
      switch (this.scalarData.style)
      {
        case ScalarStyle.Plain:
          this.WritePlainScalar(this.scalarData.value, !this.isSimpleKeyContext);
          break;
        case ScalarStyle.SingleQuoted:
          this.WriteSingleQuotedScalar(this.scalarData.value, !this.isSimpleKeyContext);
          break;
        case ScalarStyle.DoubleQuoted:
          this.WriteDoubleQuotedScalar(this.scalarData.value, !this.isSimpleKeyContext);
          break;
        case ScalarStyle.Literal:
          this.WriteLiteralScalar(this.scalarData.value);
          break;
        case ScalarStyle.Folded:
          this.WriteFoldedScalar(this.scalarData.value);
          break;
        default:
          throw new InvalidOperationException();
      }
    }

    private void WritePlainScalar(string value, bool allowBreaks)
    {
      if (!this.isWhitespace)
        this.Write(' ');
      bool flag1 = false;
      bool flag2 = false;
      for (int index = 0; index < value.Length; ++index)
      {
        char character = value[index];
        if (Emitter.IsSpace(character))
        {
          if (allowBreaks && !flag1 && this.column > this.bestWidth && index + 1 < value.Length && value[index + 1] != ' ')
            this.WriteIndent();
          else
            this.Write(character);
          flag1 = true;
        }
        else
        {
          char breakChar;
          if (Emitter.IsBreak(character, out breakChar))
          {
            if (!flag2 && character == '\n')
              this.WriteBreak();
            this.WriteBreak(breakChar);
            this.isIndentation = true;
            flag2 = true;
          }
          else
          {
            if (flag2)
              this.WriteIndent();
            this.Write(character);
            this.isIndentation = false;
            flag1 = false;
            flag2 = false;
          }
        }
      }
      this.isWhitespace = false;
      this.isIndentation = false;
      if (!this.isRootContext)
        return;
      this.isOpenEnded = true;
    }

    private void WriteSingleQuotedScalar(string value, bool allowBreaks)
    {
      this.WriteIndicator("'", true, false, false);
      bool flag1 = false;
      bool flag2 = false;
      for (int index = 0; index < value.Length; ++index)
      {
        char character = value[index];
        if (character == ' ')
        {
          if (allowBreaks && !flag1 && this.column > this.bestWidth && index != 0 && index + 1 < value.Length && value[index + 1] != ' ')
            this.WriteIndent();
          else
            this.Write(character);
          flag1 = true;
        }
        else
        {
          char breakChar;
          if (Emitter.IsBreak(character, out breakChar))
          {
            if (!flag2 && character == '\n')
              this.WriteBreak();
            this.WriteBreak(breakChar);
            this.isIndentation = true;
            flag2 = true;
          }
          else
          {
            if (flag2)
              this.WriteIndent();
            if (character == '\'')
              this.Write(character);
            this.Write(character);
            this.isIndentation = false;
            flag1 = false;
            flag2 = false;
          }
        }
      }
      this.WriteIndicator("'", false, false, false);
      this.isWhitespace = false;
      this.isIndentation = false;
    }

    private void WriteDoubleQuotedScalar(string value, bool allowBreaks)
    {
      this.WriteIndicator("\"", true, false, false);
      bool flag = false;
      for (int index = 0; index < value.Length; ++index)
      {
        char ch = value[index];
        if (Emitter.IsPrintable(ch) && !Emitter.IsBreak(ch, out char _))
        {
          switch (ch)
          {
            case ' ':
              if (allowBreaks && !flag && this.column > this.bestWidth && index > 0 && index + 1 < value.Length)
              {
                this.WriteIndent();
                if (value[index + 1] == ' ')
                  this.Write('\\');
              }
              else
                this.Write(ch);
              flag = true;
              continue;
            case '"':
            case '\\':
              break;
            default:
              this.Write(ch);
              flag = false;
              continue;
          }
        }
        this.Write('\\');
        switch (ch)
        {
          case char.MinValue:
            this.Write('0');
            break;
          case '\a':
            this.Write('a');
            break;
          case '\b':
            this.Write('b');
            break;
          case '\t':
            this.Write('t');
            break;
          case '\n':
            this.Write('n');
            break;
          case '\v':
            this.Write('v');
            break;
          case '\f':
            this.Write('f');
            break;
          case '\r':
            this.Write('r');
            break;
          case '\u001B':
            this.Write('e');
            break;
          case '"':
            this.Write('"');
            break;
          case '\\':
            this.Write('\\');
            break;
          case '\u0085':
            this.Write('N');
            break;
          case ' ':
            this.Write('_');
            break;
          case '\u2028':
            this.Write('L');
            break;
          case '\u2029':
            this.Write('P');
            break;
          default:
            ushort num = (ushort) ch;
            if (num <= (ushort) byte.MaxValue)
            {
              this.Write('x');
              this.Write(num.ToString("X02", (IFormatProvider) CultureInfo.InvariantCulture));
              break;
            }
            if (Emitter.IsHighSurrogate(ch))
            {
              if (index + 1 >= value.Length || !Emitter.IsLowSurrogate(value[index + 1]))
                throw new SyntaxErrorException("While writing a quoted scalar, found an orphaned high surrogate.");
              this.Write('U');
              this.Write(char.ConvertToUtf32(ch, value[index + 1]).ToString("X08", (IFormatProvider) CultureInfo.InvariantCulture));
              ++index;
              break;
            }
            this.Write('u');
            this.Write(num.ToString("X04", (IFormatProvider) CultureInfo.InvariantCulture));
            break;
        }
        flag = false;
      }
      this.WriteIndicator("\"", false, false, false);
      this.isWhitespace = false;
      this.isIndentation = false;
    }

    private void WriteLiteralScalar(string value)
    {
      bool flag = true;
      this.WriteIndicator("|", true, false, false);
      this.WriteBlockScalarHints(value);
      this.WriteBreak();
      this.isIndentation = true;
      this.isWhitespace = true;
      for (int index = 0; index < value.Length; ++index)
      {
        char character = value[index];
        if (character != '\r' || index + 1 >= value.Length || value[index + 1] != '\n')
        {
          char breakChar;
          if (Emitter.IsBreak(character, out breakChar))
          {
            this.WriteBreak(breakChar);
            this.isIndentation = true;
            flag = true;
          }
          else
          {
            if (flag)
              this.WriteIndent();
            this.Write(character);
            this.isIndentation = false;
            flag = false;
          }
        }
      }
    }

    private void WriteFoldedScalar(string value)
    {
      bool flag1 = true;
      bool flag2 = true;
      this.WriteIndicator(">", true, false, false);
      this.WriteBlockScalarHints(value);
      this.WriteBreak();
      this.isIndentation = true;
      this.isWhitespace = true;
      for (int index = 0; index < value.Length; ++index)
      {
        char character = value[index];
        char breakChar1;
        if (Emitter.IsBreak(character, out breakChar1))
        {
          if (!flag1 && !flag2 && character == '\n')
          {
            int num = 0;
            char breakChar2;
            while (index + num < value.Length && Emitter.IsBreak(value[index + num], out breakChar2))
              ++num;
            if (index + num < value.Length && !Emitter.IsBlank(value[index + num]) && !Emitter.IsBreak(value[index + num], out breakChar2))
              this.WriteBreak();
          }
          this.WriteBreak(breakChar1);
          this.isIndentation = true;
          flag1 = true;
        }
        else
        {
          if (flag1)
          {
            this.WriteIndent();
            flag2 = Emitter.IsBlank(character);
          }
          if (!flag1 && character == ' ' && index + 1 < value.Length && value[index + 1] != ' ' && this.column > this.bestWidth)
            this.WriteIndent();
          else
            this.Write(character);
          this.isIndentation = false;
          flag1 = false;
        }
      }
    }

    private static bool IsSpace(char character) => character == ' ';

    private static bool IsBreak(char character, out char breakChar)
    {
      switch (character)
      {
        case '\n':
        case '\r':
        case '\u0085':
          breakChar = '\n';
          return true;
        case '\u2028':
        case '\u2029':
          breakChar = character;
          return true;
        default:
          breakChar = char.MinValue;
          return false;
      }
    }

    private static bool IsBlank(char character) => character == ' ' || character == '\t';

    private static bool IsPrintable(char character)
    {
      if (character == '\t' || character == '\n' || character == '\r' || character >= ' ' && character <= '~' || character == '\u0085' || character >= ' ' && character <= '\uD7FF')
        return true;
      return character >= '\uE000' && character <= '�';
    }

    private static bool IsHighSurrogate(char c) => '\uD800' <= c && c <= '\uDBFF';

    private static bool IsLowSurrogate(char c) => '\uDC00' <= c && c <= '\uDFFF';

    private void EmitSequenceStart(ParsingEvent evt)
    {
      this.ProcessAnchor();
      this.ProcessTag();
      SequenceStart sequenceStart = (SequenceStart) evt;
      if (this.flowLevel != 0 || this.isCanonical || sequenceStart.Style == SequenceStyle.Flow || this.CheckEmptySequence())
        this.state = EmitterState.FlowSequenceFirstItem;
      else
        this.state = EmitterState.BlockSequenceFirstItem;
    }

    private void EmitMappingStart(ParsingEvent evt)
    {
      this.ProcessAnchor();
      this.ProcessTag();
      MappingStart mappingStart = (MappingStart) evt;
      if (this.flowLevel != 0 || this.isCanonical || mappingStart.Style == MappingStyle.Flow || this.CheckEmptyMapping())
        this.state = EmitterState.FlowMappingFirstKey;
      else
        this.state = EmitterState.BlockMappingFirstKey;
    }

    private void ProcessAnchor()
    {
      if (this.anchorData.anchor == null)
        return;
      this.WriteIndicator(this.anchorData.isAlias ? "*" : "&", true, false, false);
      this.WriteAnchor(this.anchorData.anchor);
    }

    private void ProcessTag()
    {
      if (this.tagData.handle == null && this.tagData.suffix == null)
        return;
      if (this.tagData.handle != null)
      {
        this.WriteTagHandle(this.tagData.handle);
        if (this.tagData.suffix == null)
          return;
        this.WriteTagContent(this.tagData.suffix, false);
      }
      else
      {
        this.WriteIndicator("!<", true, false, false);
        this.WriteTagContent(this.tagData.suffix, false);
        this.WriteIndicator(">", false, false, false);
      }
    }

    private void EmitDocumentEnd(ParsingEvent evt)
    {
      if (!(evt is YamlDotNet.Core.Events.DocumentEnd documentEnd))
        throw new YamlException("Expected DOCUMENT-END.");
      this.WriteIndent();
      if (!documentEnd.IsImplicit)
      {
        this.WriteIndicator("...", true, false, false);
        this.WriteIndent();
        this.isDocumentEndWritten = true;
      }
      this.state = EmitterState.DocumentStart;
      this.tagDirectives.Clear();
    }

    private void EmitFlowSequenceItem(ParsingEvent evt, bool isFirst)
    {
      if (isFirst)
      {
        this.WriteIndicator("[", true, true, false);
        this.IncreaseIndent(true, false);
        ++this.flowLevel;
      }
      if (evt is SequenceEnd)
      {
        --this.flowLevel;
        this.indent = this.indents.Pop();
        if (this.isCanonical && !isFirst)
        {
          this.WriteIndicator(",", false, false, false);
          this.WriteIndent();
        }
        this.WriteIndicator("]", false, false, false);
        this.state = this.states.Pop();
      }
      else
      {
        if (!isFirst)
          this.WriteIndicator(",", false, false, false);
        if (this.isCanonical || this.column > this.bestWidth)
          this.WriteIndent();
        this.states.Push(EmitterState.FlowSequenceItem);
        this.EmitNode(evt, false, false, false);
      }
    }

    private void EmitFlowMappingKey(ParsingEvent evt, bool isFirst)
    {
      if (isFirst)
      {
        this.WriteIndicator("{", true, true, false);
        this.IncreaseIndent(true, false);
        ++this.flowLevel;
      }
      if (evt is MappingEnd)
      {
        --this.flowLevel;
        this.indent = this.indents.Pop();
        if (this.isCanonical && !isFirst)
        {
          this.WriteIndicator(",", false, false, false);
          this.WriteIndent();
        }
        this.WriteIndicator("}", false, false, false);
        this.state = this.states.Pop();
      }
      else
      {
        if (!isFirst)
          this.WriteIndicator(",", false, false, false);
        if (this.isCanonical || this.column > this.bestWidth)
          this.WriteIndent();
        if (!this.isCanonical && this.CheckSimpleKey())
        {
          this.states.Push(EmitterState.FlowMappingSimpleValue);
          this.EmitNode(evt, false, true, true);
        }
        else
        {
          this.WriteIndicator("?", true, false, false);
          this.states.Push(EmitterState.FlowMappingValue);
          this.EmitNode(evt, false, true, false);
        }
      }
    }

    private void EmitFlowMappingValue(ParsingEvent evt, bool isSimple)
    {
      if (isSimple)
      {
        this.WriteIndicator(":", false, false, false);
      }
      else
      {
        if (this.isCanonical || this.column > this.bestWidth)
          this.WriteIndent();
        this.WriteIndicator(":", true, false, false);
      }
      this.states.Push(EmitterState.FlowMappingKey);
      this.EmitNode(evt, false, true, false);
    }

    private void EmitBlockSequenceItem(ParsingEvent evt, bool isFirst)
    {
      if (isFirst)
        this.IncreaseIndent(false, this.isMappingContext && !this.isIndentation);
      if (evt is SequenceEnd)
      {
        this.indent = this.indents.Pop();
        this.state = this.states.Pop();
      }
      else
      {
        this.WriteIndent();
        this.WriteIndicator("-", true, false, true);
        this.states.Push(EmitterState.BlockSequenceItem);
        this.EmitNode(evt, false, false, false);
      }
    }

    private void EmitBlockMappingKey(ParsingEvent evt, bool isFirst)
    {
      if (isFirst)
        this.IncreaseIndent(false, false);
      if (evt is MappingEnd)
      {
        this.indent = this.indents.Pop();
        this.state = this.states.Pop();
      }
      else
      {
        this.WriteIndent();
        if (this.CheckSimpleKey())
        {
          this.states.Push(EmitterState.BlockMappingSimpleValue);
          this.EmitNode(evt, false, true, true);
        }
        else
        {
          this.WriteIndicator("?", true, false, true);
          this.states.Push(EmitterState.BlockMappingValue);
          this.EmitNode(evt, false, true, false);
        }
      }
    }

    private void EmitBlockMappingValue(ParsingEvent evt, bool isSimple)
    {
      if (isSimple)
      {
        this.WriteIndicator(":", false, false, false);
      }
      else
      {
        this.WriteIndent();
        this.WriteIndicator(":", true, false, true);
      }
      this.states.Push(EmitterState.BlockMappingKey);
      this.EmitNode(evt, false, true, false);
    }

    private void IncreaseIndent(bool isFlow, bool isIndentless)
    {
      this.indents.Push(this.indent);
      if (this.indent < 0)
      {
        this.indent = isFlow ? this.bestIndent : 0;
      }
      else
      {
        if (isIndentless)
          return;
        this.indent += this.bestIndent;
      }
    }

    private bool CheckEmptyDocument()
    {
      int num = 0;
      foreach (ParsingEvent parsingEvent in this.events)
      {
        ++num;
        if (num == 2)
        {
          if (parsingEvent is YamlDotNet.Core.Events.Scalar scalar)
            return string.IsNullOrEmpty(scalar.Value);
          break;
        }
      }
      return false;
    }

    private bool CheckSimpleKey()
    {
      if (this.events.Count < 1)
        return false;
      int num;
      switch (this.events.Peek().Type)
      {
        case EventType.Alias:
          num = this.SafeStringLength(this.anchorData.anchor);
          break;
        case EventType.Scalar:
          if (this.scalarData.isMultiline)
            return false;
          num = this.SafeStringLength(this.anchorData.anchor) + this.SafeStringLength(this.tagData.handle) + this.SafeStringLength(this.tagData.suffix) + this.SafeStringLength(this.scalarData.value);
          break;
        case EventType.SequenceStart:
          if (!this.CheckEmptySequence())
            return false;
          num = this.SafeStringLength(this.anchorData.anchor) + this.SafeStringLength(this.tagData.handle) + this.SafeStringLength(this.tagData.suffix);
          break;
        case EventType.MappingStart:
          if (!this.CheckEmptySequence())
            return false;
          num = this.SafeStringLength(this.anchorData.anchor) + this.SafeStringLength(this.tagData.handle) + this.SafeStringLength(this.tagData.suffix);
          break;
        default:
          return false;
      }
      return num <= 128;
    }

    private int SafeStringLength(string value) => value != null ? value.Length : 0;

    private bool CheckEmptySequence()
    {
      if (this.events.Count < 2)
        return false;
      FakeList<ParsingEvent> fakeList = new FakeList<ParsingEvent>((IEnumerable<ParsingEvent>) this.events);
      return fakeList[0] is SequenceStart && fakeList[1] is SequenceEnd;
    }

    private bool CheckEmptyMapping()
    {
      if (this.events.Count < 2)
        return false;
      FakeList<ParsingEvent> fakeList = new FakeList<ParsingEvent>((IEnumerable<ParsingEvent>) this.events);
      return fakeList[0] is MappingStart && fakeList[1] is MappingEnd;
    }

    private void WriteBlockScalarHints(string value)
    {
      CharacterAnalyzer<StringLookAheadBuffer> characterAnalyzer = new CharacterAnalyzer<StringLookAheadBuffer>(new StringLookAheadBuffer(value));
      if (characterAnalyzer.IsSpace() || characterAnalyzer.IsBreak())
        this.WriteIndicator(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}", new object[1]
        {
          (object) this.bestIndent
        }), false, false, false);
      this.isOpenEnded = false;
      string indicator = (string) null;
      if (value.Length == 0 || !characterAnalyzer.IsBreak(value.Length - 1))
        indicator = "-";
      else if (value.Length >= 2 && characterAnalyzer.IsBreak(value.Length - 2))
      {
        indicator = "+";
        this.isOpenEnded = true;
      }
      if (indicator == null)
        return;
      this.WriteIndicator(indicator, false, false, false);
    }

    private void WriteIndicator(
      string indicator,
      bool needWhitespace,
      bool whitespace,
      bool indentation)
    {
      if (needWhitespace && !this.isWhitespace)
        this.Write(' ');
      this.Write(indicator);
      this.isWhitespace = whitespace;
      this.isIndentation &= indentation;
      this.isOpenEnded = false;
    }

    private void WriteIndent()
    {
      int num = Math.Max(this.indent, 0);
      if (!this.isIndentation || this.column > num || this.column == num && !this.isWhitespace)
        this.WriteBreak();
      while (this.column < num)
        this.Write(' ');
      this.isWhitespace = true;
      this.isIndentation = true;
    }

    private void WriteAnchor(string value)
    {
      this.Write(value);
      this.isWhitespace = false;
      this.isIndentation = false;
    }

    private void WriteTagHandle(string value)
    {
      if (!this.isWhitespace)
        this.Write(' ');
      this.Write(value);
      this.isWhitespace = false;
      this.isIndentation = false;
    }

    private void WriteTagContent(string value, bool needsWhitespace)
    {
      if (needsWhitespace && !this.isWhitespace)
        this.Write(' ');
      this.Write(this.UrlEncode(value));
      this.isWhitespace = false;
      this.isIndentation = false;
    }

    private string UrlEncode(string text) => Emitter.uriReplacer.Replace(text, (MatchEvaluator) (match =>
    {
      StringBuilder stringBuilder = new StringBuilder();
      foreach (byte num in Encoding.UTF8.GetBytes(match.Value))
        stringBuilder.AppendFormat("%{0:X02}", (object) num);
      return stringBuilder.ToString();
    }));

    private void Write(char value)
    {
      this.output.Write(value);
      ++this.column;
    }

    private void Write(string value)
    {
      this.output.Write(value);
      this.column += value.Length;
    }

    private void WriteBreak(char breakCharacter = '\n')
    {
      if (breakCharacter == '\n')
        this.output.WriteLine();
      else
        this.output.Write(breakCharacter);
      this.column = 0;
    }

    private class AnchorData
    {
      public string anchor;
      public bool isAlias;
    }

    private class TagData
    {
      public string handle;
      public string suffix;
    }

    private class ScalarData
    {
      public string value;
      public bool isMultiline;
      public bool isFlowPlainAllowed;
      public bool isBlockPlainAllowed;
      public bool isSingleQuotedAllowed;
      public bool isBlockAllowed;
      public bool hasSingleQuotes;
      public ScalarStyle style;
    }
  }
}
