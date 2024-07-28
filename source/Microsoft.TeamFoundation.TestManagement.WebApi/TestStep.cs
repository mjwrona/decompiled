// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.WebApi.TestStep
// Assembly: Microsoft.TeamFoundation.TestManagement.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 10F0A812-3ECA-42B4-865D-429941F99EBE
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.TestManagement.WebApi.dll

using Microsoft.VisualStudio.Services.WebApi.Patch;
using Microsoft.VisualStudio.Services.WebApi.Patch.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Xml;

namespace Microsoft.TeamFoundation.TestManagement.WebApi
{
  internal class TestStep : TestAction, ITestStep, ITestAction
  {
    public const string ElementName = "step";
    public const string TypeAttributeName = "type";
    public const string DescriptionElementName = "description";
    public const string AttachedFile = "AttachedFile";
    public const string AttachmentRelationPathFormat = "/relations/-";
    public const string StepCommentFormattedString = "[TestStep={0}]:";
    public const string AttributeComment = "comment";
    public const string AttributeName = "name";
    private ParameterizedString m_title;
    private ParameterizedString m_expectedResult;
    private string m_description;
    private TestStepType m_stepType;
    private TestAttachmentCollection m_attachments;

    public TestStep(ITestActionOwner owner)
      : base(owner)
    {
      this.m_title = ParameterizedString.Empty;
      this.m_expectedResult = ParameterizedString.Empty;
      this.m_attachments = new TestAttachmentCollection();
    }

    public TestStep(ITestActionOwner owner, int id)
      : base(owner, id)
    {
      this.m_title = ParameterizedString.Empty;
      this.m_expectedResult = ParameterizedString.Empty;
      this.m_attachments = new TestAttachmentCollection();
    }

    public ParameterizedString Title
    {
      get => this.m_title;
      set => this.m_title = value != null ? value : throw new ArgumentNullException(nameof (value));
    }

    public ParameterizedString ExpectedResult
    {
      get => this.m_expectedResult;
      set => this.m_expectedResult = value != null ? value : throw new ArgumentNullException(nameof (value));
    }

    public string Description
    {
      get => this.m_description;
      set => this.m_description = value;
    }

    public TestStepType TestStepType
    {
      get => this.m_stepType;
      set => this.m_stepType = value;
    }

    public ITestStepAttachment CreateAttachment(string url, string attachmentName = null) => (ITestStepAttachment) new TestStepAttachment(url, attachmentName);

    public TestAttachmentCollection Attachments => this.m_attachments;

    public override void FromXml(XmlReader reader)
    {
      string str = reader["type"];
      try
      {
        this.m_stepType = (TestStepType) Enum.Parse(typeof (TestStepType), str);
      }
      catch (ArgumentException ex)
      {
        throw new XmlException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, "the following step type is not valid: {0}.", (object) str), (Exception) ex);
      }
      reader.Read();
      this.Title = new ParameterizedString(reader);
      this.ExpectedResult = new ParameterizedString(reader);
      this.Description = reader.ReadElementContentAsString();
      reader.ReadEndElement();
    }

    public override void ToXml(XmlWriter writer)
    {
      writer.WriteStartElement("step");
      this.WriteBaseAttributes(writer);
      writer.WriteAttributeString("type", this.m_stepType.ToString());
      this.m_title.ToXml(writer);
      this.m_expectedResult.ToXml(writer);
      writer.WriteStartElement("description");
      writer.WriteString(this.m_description);
      writer.WriteEndElement();
      writer.WriteEndElement();
    }

    public JsonPatchDocument UpdateAttachmentsInJson(JsonPatchDocument json)
    {
      foreach (ITestStepAttachment attachment in (Collection<ITestAttachment>) this.m_attachments)
      {
        if (!string.IsNullOrEmpty(attachment.Url))
        {
          JsonPatchOperation jsonPatchOperation = new JsonPatchOperation();
          jsonPatchOperation.Operation = Operation.Add;
          jsonPatchOperation.Path = "/relations/-";
          TestAttachmentLink testAttachmentLink = new TestAttachmentLink()
          {
            Rel = "AttachedFile",
            Url = attachment.Url,
            Attributes = (IDictionary<string, object>) new Dictionary<string, object>()
            {
              {
                "comment",
                (object) string.Format((IFormatProvider) CultureInfo.InvariantCulture, "[TestStep={0}]:", (object) this.Id)
              }
            }
          };
          jsonPatchOperation.Value = (object) testAttachmentLink;
          json.Add(jsonPatchOperation);
        }
      }
      return json;
    }

    public void LoadAttachementFromLinks(IList<TestAttachmentLink> links)
    {
      foreach (TestAttachmentLink link in (IEnumerable<TestAttachmentLink>) links)
      {
        if (link.Rel == "AttachedFile" && link.Attributes != null && link.Attributes.ContainsKey("comment") && (string) link.Attributes["comment"] == string.Format((IFormatProvider) CultureInfo.InvariantCulture, "[TestStep={0}]:", (object) this.Id))
        {
          string name = (string) null;
          if (link.Attributes.ContainsKey("name"))
            name = (string) link.Attributes["name"];
          this.m_attachments.Add((ITestAttachment) new TestStepAttachment(link.Url, name));
        }
      }
    }
  }
}
