// Decompiled with JetBrains decompiler
// Type: Microsoft.Cloud.Metrics.Client.Metrics.ValidateKqlmQueryResult
// Assembly: Microsoft.Cloud.Metrics.Client, Version=2.2023.705.2051, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 06B39E1C-7DF0-4BC1-AFBA-9AD635E73CB0
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Cloud.Metrics.Client.dll

using System;
using System.Collections.Generic;
using System.IO;

namespace Microsoft.Cloud.Metrics.Client.Metrics
{
  public sealed class ValidateKqlmQueryResult
  {
    public bool IsValidQuery { get; set; }

    public TimeSpan OutputResolution { get; private set; }

    public List<string> OutputSamplingTypes { get; private set; }

    public List<string> OutputDimensions { get; private set; }

    public List<StatementProcessingMessage> ValidationMessages { get; private set; }

    public List<PreAggregateUsage> DailyUsages { get; private set; }

    public void AddPreaggUsage(PreAggregateUsage preaggUsage)
    {
      if (preaggUsage == null)
        return;
      if (this.DailyUsages == null)
        this.DailyUsages = new List<PreAggregateUsage>();
      this.DailyUsages.Add(preaggUsage);
    }

    public void Deserialize(BinaryReader reader)
    {
      this.IsValidQuery = reader.ReadBoolean();
      this.OutputResolution = TimeSpan.FromTicks(reader.ReadInt64());
      int num1 = reader.ReadInt32();
      if (num1 == 0)
      {
        this.OutputSamplingTypes = (List<string>) null;
      }
      else
      {
        List<string> stringList = new List<string>();
        for (int index = 0; index < num1; ++index)
          stringList.Add(reader.ReadString());
        this.OutputSamplingTypes = stringList;
      }
      int num2 = reader.ReadInt32();
      if (num2 == 0)
      {
        this.OutputDimensions = (List<string>) null;
      }
      else
      {
        List<string> stringList = new List<string>();
        for (int index = 0; index < num2; ++index)
          stringList.Add(reader.ReadString());
        this.OutputDimensions = stringList;
      }
      int num3 = reader.ReadInt32();
      if (num3 == 0)
      {
        this.ValidationMessages = (List<StatementProcessingMessage>) null;
      }
      else
      {
        List<StatementProcessingMessage> processingMessageList = new List<StatementProcessingMessage>();
        for (int index = 0; index < num3; ++index)
        {
          string stringRepresentation = reader.ReadString();
          int severity = reader.ReadInt32();
          string text = reader.ReadString();
          int num4 = reader.ReadInt32();
          int num5 = reader.ReadInt32();
          int lineNumber = reader.ReadInt32();
          int num6 = reader.ReadInt32();
          StatementProcessingErrorCode codeRepresentation = StatementProcessingErrorCodeExtension.ToErrorCodeRepresentation(stringRepresentation);
          int charPositionInLine = num5;
          int charPositionAbsolute = num4;
          int errorSectionLength = num6;
          StatementContextInformation statementContext = new StatementContextInformation(lineNumber, charPositionInLine, charPositionAbsolute, errorSectionLength);
          processingMessageList.Add((StatementProcessingMessage) new ValidationMessage((MessageSeverity) severity, text, codeRepresentation, statementContext));
        }
        this.ValidationMessages = processingMessageList;
      }
      int num7 = reader.ReadInt32();
      if (num7 == 0)
      {
        this.DailyUsages = (List<PreAggregateUsage>) null;
      }
      else
      {
        for (int index = 0; index < num7; ++index)
        {
          PreAggregateUsage preaggUsage = new PreAggregateUsage();
          preaggUsage.Deserialize(reader);
          this.AddPreaggUsage(preaggUsage);
        }
      }
    }
  }
}
