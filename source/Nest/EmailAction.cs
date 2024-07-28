// Decompiled with JetBrains decompiler
// Type: Nest.EmailAction
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System.Collections.Generic;

namespace Nest
{
  public class EmailAction : ActionBase, IEmailAction, IAction
  {
    public EmailAction(string name)
      : base(name)
    {
    }

    public string Account { get; set; }

    public override ActionType ActionType => ActionType.Email;

    public IEmailAttachments Attachments { get; set; }

    public IEnumerable<string> Bcc { get; set; }

    public IEmailBody Body { get; set; }

    public IEnumerable<string> Cc { get; set; }

    public string From { get; set; }

    public EmailPriority? Priority { get; set; }

    public IEnumerable<string> ReplyTo { get; set; }

    public string Subject { get; set; }

    public IEnumerable<string> To { get; set; }
  }
}
