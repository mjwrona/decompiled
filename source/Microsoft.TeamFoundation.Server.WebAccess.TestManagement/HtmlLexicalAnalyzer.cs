// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.TestManagement.HtmlLexicalAnalyzer
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.TestManagement, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 2E4165D5-898A-42D9-B816-9FABF135E4DA
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.TestManagement.dll

using System;
using System.IO;
using System.Text;

namespace Microsoft.TeamFoundation.Server.WebAccess.TestManagement
{
  internal class HtmlLexicalAnalyzer
  {
    private StringReader _inputStringReader;
    private int _nextCharacterCode;
    private char _nextCharacter;
    private int _lookAheadCharacterCode;
    private char _lookAheadCharacter;
    private char _previousCharacter;
    private bool _isNextCharacterEntity;
    private StringBuilder _nextToken;
    private HtmlTokenType _nextTokenType;

    internal HtmlLexicalAnalyzer(string inputTextString)
    {
      this._inputStringReader = new StringReader(inputTextString);
      this._nextCharacterCode = 0;
      this._nextCharacter = ' ';
      this._lookAheadCharacterCode = this._inputStringReader.Read();
      this._lookAheadCharacter = (char) this._lookAheadCharacterCode;
      this._previousCharacter = ' ';
      this._nextToken = new StringBuilder(100);
      this._nextTokenType = HtmlTokenType.Text;
      this.GetNextCharacter();
    }

    internal void GetNextContentToken()
    {
      this._nextToken.Length = 0;
      if (this.IsAtEndOfStream)
        this._nextTokenType = HtmlTokenType.EOF;
      else if (this.IsAtTagStart)
      {
        this.GetNextCharacter();
        if (this.NextCharacter == '/')
        {
          this._nextToken.Append("</");
          this._nextTokenType = HtmlTokenType.ClosingTagStart;
          this.GetNextCharacter();
        }
        else
        {
          this._nextTokenType = HtmlTokenType.OpeningTagStart;
          this._nextToken.Append("<");
        }
      }
      else if (this.IsAtDirectiveStart)
      {
        this.GetNextCharacter();
        if (this._lookAheadCharacter == '[')
          this.ReadDynamicContent();
        else if (this._lookAheadCharacter == '-')
          this.ReadComment();
        else
          this.ReadUnknownDirective();
      }
      else
      {
        this._nextTokenType = HtmlTokenType.Text;
        while (!this.IsAtTagStart && !this.IsAtEndOfStream && !this.IsAtDirectiveStart)
        {
          if (this.NextCharacter == '<' && !this.IsNextCharacterEntity && this._lookAheadCharacter == '?')
          {
            this.SkipProcessingDirective();
          }
          else
          {
            this._nextToken.Append(this.NextCharacter);
            this.GetNextCharacter();
          }
        }
      }
    }

    internal void GetNextTagToken()
    {
      this._nextToken.Length = 0;
      if (this.IsAtEndOfStream)
      {
        this._nextTokenType = HtmlTokenType.EOF;
      }
      else
      {
        this.SkipWhiteSpace();
        if (this.NextCharacter == '>' && !this.IsNextCharacterEntity)
        {
          this._nextTokenType = HtmlTokenType.TagEnd;
          this._nextToken.Append('>');
          this.GetNextCharacter();
        }
        else if (this.NextCharacter == '/' && this._lookAheadCharacter == '>')
        {
          this._nextTokenType = HtmlTokenType.EmptyTagEnd;
          this._nextToken.Append("/>");
          this.GetNextCharacter();
          this.GetNextCharacter();
        }
        else if (this.IsGoodForNameStart(this.NextCharacter))
        {
          this._nextTokenType = HtmlTokenType.Name;
          while (this.IsGoodForName(this.NextCharacter) && !this.IsAtEndOfStream)
          {
            this._nextToken.Append(this.NextCharacter);
            this.GetNextCharacter();
          }
        }
        else
        {
          this._nextTokenType = HtmlTokenType.Atom;
          this._nextToken.Append(this.NextCharacter);
          this.GetNextCharacter();
        }
      }
    }

    internal void GetNextEqualSignToken()
    {
      this._nextToken.Length = 0;
      this._nextToken.Append('=');
      this._nextTokenType = HtmlTokenType.EqualSign;
      this.SkipWhiteSpace();
      if (this.NextCharacter != '=')
        return;
      this.GetNextCharacter();
    }

    internal void GetNextAtomToken()
    {
      this._nextToken.Length = 0;
      this.SkipWhiteSpace();
      this._nextTokenType = HtmlTokenType.Atom;
      if ((this.NextCharacter == '\'' || this.NextCharacter == '"') && !this.IsNextCharacterEntity)
      {
        char nextCharacter = this.NextCharacter;
        this.GetNextCharacter();
        while (((int) this.NextCharacter != (int) nextCharacter || this.IsNextCharacterEntity) && !this.IsAtEndOfStream)
        {
          this._nextToken.Append(this.NextCharacter);
          this.GetNextCharacter();
        }
        if ((int) this.NextCharacter != (int) nextCharacter)
          return;
        this.GetNextCharacter();
      }
      else
      {
        while (!this.IsAtEndOfStream && !char.IsWhiteSpace(this.NextCharacter) && this.NextCharacter != '>')
        {
          this._nextToken.Append(this.NextCharacter);
          this.GetNextCharacter();
        }
      }
    }

    internal HtmlTokenType NextTokenType => this._nextTokenType;

    internal string NextToken => this._nextToken.ToString();

    private void GetNextCharacter()
    {
      if (this._nextCharacterCode == -1)
        throw new InvalidOperationException("GetNextCharacter method called at the end of a stream");
      this._previousCharacter = this._nextCharacter;
      this._nextCharacter = this._lookAheadCharacter;
      this._nextCharacterCode = this._lookAheadCharacterCode;
      this._isNextCharacterEntity = false;
      this.ReadLookAheadCharacter();
      if (this._nextCharacter != '&')
        return;
      if (this._lookAheadCharacter == '#')
      {
        int num = 0;
        this.ReadLookAheadCharacter();
        for (int index = 0; index < 7 && char.IsDigit(this._lookAheadCharacter); ++index)
        {
          num = 10 * num + (this._lookAheadCharacterCode - 48);
          this.ReadLookAheadCharacter();
        }
        if (this._lookAheadCharacter == ';')
        {
          this.ReadLookAheadCharacter();
          this._nextCharacterCode = num;
          this._nextCharacter = (char) this._nextCharacterCode;
          this._isNextCharacterEntity = true;
        }
        else
        {
          this._nextCharacter = this._lookAheadCharacter;
          this._nextCharacterCode = this._lookAheadCharacterCode;
          this.ReadLookAheadCharacter();
          this._isNextCharacterEntity = false;
        }
      }
      else
      {
        if (!char.IsLetter(this._lookAheadCharacter))
          return;
        string entityName = "";
        for (int index = 0; index < 10 && (char.IsLetter(this._lookAheadCharacter) || char.IsDigit(this._lookAheadCharacter)); ++index)
        {
          entityName += this._lookAheadCharacter.ToString();
          this.ReadLookAheadCharacter();
        }
        if (this._lookAheadCharacter == ';')
        {
          this.ReadLookAheadCharacter();
          if (HtmlSchema.IsEntity(entityName))
          {
            this._nextCharacter = HtmlSchema.EntityCharacterValue(entityName);
            this._nextCharacterCode = (int) this._nextCharacter;
            this._isNextCharacterEntity = true;
          }
          else
          {
            this._nextCharacter = this._lookAheadCharacter;
            this._nextCharacterCode = this._lookAheadCharacterCode;
            this.ReadLookAheadCharacter();
            this._isNextCharacterEntity = false;
          }
        }
        else
        {
          this._nextCharacter = this._lookAheadCharacter;
          this.ReadLookAheadCharacter();
          this._isNextCharacterEntity = false;
        }
      }
    }

    private void ReadLookAheadCharacter()
    {
      if (this._lookAheadCharacterCode == -1)
        return;
      this._lookAheadCharacterCode = this._inputStringReader.Read();
      this._lookAheadCharacter = (char) this._lookAheadCharacterCode;
    }

    private void SkipWhiteSpace()
    {
      while (true)
      {
        if (this._nextCharacter == '<' && (this._lookAheadCharacter == '?' || this._lookAheadCharacter == '!'))
        {
          this.GetNextCharacter();
          if (this._lookAheadCharacter == '[')
          {
            while (!this.IsAtEndOfStream && (this._previousCharacter != ']' || this._nextCharacter != ']' || this._lookAheadCharacter != '>'))
              this.GetNextCharacter();
            if (this._nextCharacter == '>')
              this.GetNextCharacter();
          }
          else
          {
            while (!this.IsAtEndOfStream && this._nextCharacter != '>')
              this.GetNextCharacter();
            if (this._nextCharacter == '>')
              this.GetNextCharacter();
          }
        }
        if (char.IsWhiteSpace(this.NextCharacter))
          this.GetNextCharacter();
        else
          break;
      }
    }

    private bool IsGoodForNameStart(char character) => character == '_' || char.IsLetter(character);

    private bool IsGoodForName(char character) => this.IsGoodForNameStart(character) || character == '.' || character == '-' || character == ':' || char.IsDigit(character) || this.IsCombiningCharacter(character) || this.IsExtender(character);

    private bool IsCombiningCharacter(char character) => false;

    private bool IsExtender(char character) => false;

    private void ReadDynamicContent()
    {
      this._nextTokenType = HtmlTokenType.Text;
      this._nextToken.Length = 0;
      this.GetNextCharacter();
      this.GetNextCharacter();
      while ((this._nextCharacter != ']' || this._lookAheadCharacter != '>') && !this.IsAtEndOfStream)
        this.GetNextCharacter();
      if (this.IsAtEndOfStream)
        return;
      this.GetNextCharacter();
      this.GetNextCharacter();
    }

    private void ReadComment()
    {
      this._nextTokenType = HtmlTokenType.Comment;
      this._nextToken.Length = 0;
      this.GetNextCharacter();
      this.GetNextCharacter();
      this.GetNextCharacter();
      while (true)
      {
        while (this.IsAtEndOfStream || this._nextCharacter == '-' && this._lookAheadCharacter == '-' || this._nextCharacter == '!' && this._lookAheadCharacter == '>')
        {
          this.GetNextCharacter();
          if (this._previousCharacter == '-' && this._nextCharacter == '-' && this._lookAheadCharacter == '>')
            this.GetNextCharacter();
          else if (this._previousCharacter != '!' || this._nextCharacter != '>')
          {
            this._nextToken.Append(this._previousCharacter);
            continue;
          }
          if (this._nextCharacter != '>')
            return;
          this.GetNextCharacter();
          return;
        }
        this._nextToken.Append(this.NextCharacter);
        this.GetNextCharacter();
      }
    }

    private void ReadUnknownDirective()
    {
      this._nextTokenType = HtmlTokenType.Text;
      this._nextToken.Length = 0;
      this.GetNextCharacter();
      while ((this._nextCharacter != '>' || this.IsNextCharacterEntity) && !this.IsAtEndOfStream)
        this.GetNextCharacter();
      if (this.IsAtEndOfStream)
        return;
      this.GetNextCharacter();
    }

    private void SkipProcessingDirective()
    {
      this.GetNextCharacter();
      this.GetNextCharacter();
      while ((this._nextCharacter != '?' && this._nextCharacter != '/' || this._lookAheadCharacter != '>') && !this.IsAtEndOfStream)
        this.GetNextCharacter();
      if (this.IsAtEndOfStream)
        return;
      this.GetNextCharacter();
      this.GetNextCharacter();
    }

    private char NextCharacter => this._nextCharacter;

    private bool IsAtEndOfStream => this._nextCharacterCode == -1;

    private bool IsAtTagStart => this._nextCharacter == '<' && (this._lookAheadCharacter == '/' || this.IsGoodForNameStart(this._lookAheadCharacter)) && !this._isNextCharacterEntity;

    private bool IsAtTagEnd => (this._nextCharacter == '>' || this._nextCharacter == '/' && this._lookAheadCharacter == '>') && !this._isNextCharacterEntity;

    private bool IsAtDirectiveStart => this._nextCharacter == '<' && this._lookAheadCharacter == '!' && !this.IsNextCharacterEntity;

    private bool IsNextCharacterEntity => this._isNextCharacterEntity;
  }
}
