// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Client.UIConfig
// Assembly: Microsoft.TeamFoundation.Client, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 03892C75-AE2B-482B-8E0D-B14588A2C857
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Client.dll

using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Windows;
using System.Windows.Forms;
using System.Xml;

namespace Microsoft.TeamFoundation.Client
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  public class UIConfig
  {
    private string m_configFile;
    private Hashtable m_keyValueStore;
    private Hashtable m_origKeyValueStore;
    private const string m_attrTextWidth = "width";
    private const string m_attrTextHeight = "height";
    private const string m_attrTextX = "x";
    private const string m_attrTextY = "y";
    private const string m_attrTextValue = "Value";
    private const string m_srNodeName = "UIConfig";
    private const int DesktopEdgeFudgeX = 30;
    private const int DesktopEdgeFudgeY = 30;

    internal UIConfig(string configFile)
    {
      if (string.IsNullOrEmpty(configFile))
        throw new ArgumentNullException(nameof (configFile));
      this.m_keyValueStore = new Hashtable((IEqualityComparer) StringComparer.OrdinalIgnoreCase);
      this.m_origKeyValueStore = new Hashtable((IEqualityComparer) StringComparer.OrdinalIgnoreCase);
      this.m_configFile = configFile;
      this.LoadStore();
    }

    private void AddKeyValue(string key, string value)
    {
      if (this.m_keyValueStore.Contains((object) key))
        this.m_keyValueStore.Remove((object) key);
      this.m_keyValueStore.Add((object) key, (object) value);
    }

    public void Add(string key, string value) => this.AddKeyValue(key, value);

    public void Add(string key, int value) => this.AddKeyValue(key, value.ToString((IFormatProvider) CultureInfo.InvariantCulture));

    public void Add(string key, System.Drawing.Point xy)
    {
      string key1 = key + ".x";
      string key2 = key + ".y";
      this.AddKeyValue(key1, xy.X.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      this.AddKeyValue(key2, xy.Y.ToString((IFormatProvider) CultureInfo.InvariantCulture));
    }

    public void Add(string key, System.Drawing.Size wh)
    {
      string key1 = key + ".width";
      string key2 = key + ".height";
      this.AddKeyValue(key1, wh.Width.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      this.AddKeyValue(key2, wh.Height.ToString((IFormatProvider) CultureInfo.InvariantCulture));
    }

    public void Add(string key, System.Drawing.Point xy, System.Drawing.Size wh)
    {
      this.Add(key, xy);
      this.Add(key, wh);
    }

    public bool Retrieve(string key, out string value)
    {
      if (this.m_keyValueStore.ContainsKey((object) key))
      {
        value = (string) this.m_keyValueStore[(object) key];
        return true;
      }
      value = string.Empty;
      return false;
    }

    public bool Retrieve(string key, out int value)
    {
      value = 0;
      return this.m_keyValueStore.ContainsKey((object) key) && int.TryParse((string) this.m_keyValueStore[(object) key], NumberStyles.Integer, (IFormatProvider) CultureInfo.InvariantCulture, out value);
    }

    public bool Retrieve(string key, out System.Drawing.Point xy)
    {
      xy = new System.Drawing.Point(0, 0);
      int num1;
      int num2;
      if (!this.Retrieve(key + ".x", out num1) || !this.Retrieve(key + ".y", out num2))
        return false;
      xy.X = num1;
      xy.Y = num2;
      return true;
    }

    public bool Retrieve(string key, out System.Drawing.Size wh)
    {
      wh = new System.Drawing.Size(0, 0);
      int num1;
      int num2;
      if (!this.Retrieve(key + ".width", out num1) || !this.Retrieve(key + ".height", out num2))
        return false;
      wh.Width = num1;
      wh.Height = num2;
      return true;
    }

    public bool Retrieve(string key, out System.Drawing.Point xy, out System.Drawing.Size wh)
    {
      xy = new System.Drawing.Point(0, 0);
      wh = new System.Drawing.Size(0, 0);
      return this.Retrieve(key, out xy) && this.Retrieve(key, out wh);
    }

    public bool Retrieve(Form form, out System.Drawing.Point xy, out System.Drawing.Size wh)
    {
      bool flag;
      if (this.IsFormSizable(form))
      {
        flag = this.Retrieve(form.Name, out xy, out wh);
      }
      else
      {
        flag = this.Retrieve(form.Name, out xy);
        wh = System.Drawing.Size.Empty;
      }
      return flag;
    }

    private bool IsFormSizable(Form form) => form.FormBorderStyle == FormBorderStyle.Sizable || form.FormBorderStyle == FormBorderStyle.SizableToolWindow;

    public void LoadStore()
    {
      this.m_keyValueStore.Clear();
      this.m_origKeyValueStore.Clear();
      XmlDocument doc = new XmlDocument();
      if (!Directory.Exists(TFUtil.GetDirectoryName(this.m_configFile)))
        return;
      if (!File.Exists(this.m_configFile))
        return;
      try
      {
        XmlReaderSettings settings = new XmlReaderSettings()
        {
          DtdProcessing = DtdProcessing.Prohibit,
          XmlResolver = (XmlResolver) null
        };
        using (FileStream input = new FileStream(this.m_configFile, FileMode.Open, FileAccess.Read, FileShare.Read))
        {
          using (XmlReader reader = XmlReader.Create((Stream) input, settings))
            doc.Load(reader);
        }
      }
      catch (Exception ex)
      {
        return;
      }
      this.ConvertXmlToHash(doc);
    }

    public void SaveStore()
    {
      if (!this.ContentChanged())
        return;
      XmlDocument doc = new XmlDocument();
      this.ConvertHashToXml(doc);
      try
      {
        string directoryName = TFUtil.GetDirectoryName(this.m_configFile);
        if (!Directory.Exists(directoryName))
          Directory.CreateDirectory(directoryName);
        using (FileStream outStream = new FileStream(this.m_configFile, FileMode.OpenOrCreate, FileAccess.Write, FileShare.None))
        {
          outStream.SetLength(0L);
          outStream.Position = 0L;
          doc.Save((Stream) outStream);
          outStream.Flush();
        }
      }
      catch (Exception ex)
      {
      }
    }

    private XmlDocument ConvertHashToXml(XmlDocument doc)
    {
      ArrayList arrayList = new ArrayList(this.m_keyValueStore.Keys.Count);
      foreach (string key in (IEnumerable) this.m_keyValueStore.Keys)
        arrayList.Add((object) key);
      arrayList.Sort();
      XmlNode node = doc.CreateNode(XmlNodeType.Element, nameof (UIConfig), (string) null);
      doc.AppendChild(node);
      string str = (string) null;
      XmlNode xmlNode1 = (XmlNode) null;
      foreach (string key in arrayList)
      {
        int length = key.LastIndexOf('.');
        string name;
        string attrName;
        if (length >= 0)
        {
          name = key.Substring(0, length);
          if (length + 1 >= key.Length)
          {
            attrName = "Value";
          }
          else
          {
            attrName = key.Substring(length + 1);
            if (attrName.StartsWith("Value", StringComparison.OrdinalIgnoreCase))
              attrName = "Value" + attrName;
          }
        }
        else
        {
          name = key;
          attrName = "Value";
        }
        XmlNode xmlNode2;
        if (str == null || name != str)
        {
          xmlNode2 = doc.CreateNode(XmlNodeType.Element, name, (string) null);
          node.AppendChild(xmlNode2);
          str = name;
          xmlNode1 = xmlNode2;
        }
        else
          xmlNode2 = xmlNode1;
        TFUtil.AddXmlAttribute(xmlNode2, attrName, (string) this.m_keyValueStore[(object) key]);
      }
      return doc;
    }

    private void ConvertXmlToHash(XmlDocument doc)
    {
      foreach (XmlNode childNode in doc.FirstChild.ChildNodes)
      {
        foreach (XmlAttribute attribute in (XmlNamedNodeMap) childNode.Attributes)
        {
          string str = !attribute.Name.StartsWith("Value", StringComparison.OrdinalIgnoreCase) ? attribute.Name : attribute.Name.Substring(5);
          string key = str.Length <= 0 ? childNode.Name + str : childNode.Name + "." + str;
          this.m_keyValueStore.Add((object) key, (object) attribute.Value);
          this.m_origKeyValueStore.Add((object) key, (object) attribute.Value);
        }
      }
    }

    private bool ContentChanged()
    {
      if (this.m_keyValueStore.Count != this.m_origKeyValueStore.Count)
        return true;
      foreach (string key in (IEnumerable) this.m_origKeyValueStore.Keys)
      {
        if (!this.m_keyValueStore.Contains((object) key) || (string) this.m_origKeyValueStore[(object) key] != (string) this.m_keyValueStore[(object) key])
          return true;
      }
      return false;
    }

    public void SaveForm(Form form)
    {
      if (this.IsFormSizable(form))
        this.Add(form.Name, form.Location, form.Size);
      else
        this.Add(form.Name, form.Location);
      this.SaveStore();
    }

    public void SaveWindow(Window window)
    {
      if (window.ResizeMode != ResizeMode.NoResize)
        this.Add(window.Name, new System.Drawing.Point((int) window.Left, (int) window.Top), new System.Drawing.Size((int) window.ActualWidth, (int) window.ActualHeight));
      else
        this.Add(window.Name, new System.Drawing.Point((int) window.Left, (int) window.Top));
      this.SaveStore();
    }

    public void RestoreWindow(Window window)
    {
      System.Drawing.Point xy;
      System.Drawing.Size wh;
      if (this.Retrieve(window, out xy, out wh))
        this.RestoreOrCenterWindow(window, xy, wh);
      else
        window.WindowStartupLocation = WindowStartupLocation.CenterOwner;
    }

    public bool Retrieve(Window window, out System.Drawing.Point xy, out System.Drawing.Size wh)
    {
      bool flag;
      if (window.ResizeMode != ResizeMode.NoResize)
      {
        flag = this.Retrieve(window.Name, out xy, out wh);
      }
      else
      {
        flag = this.Retrieve(window.Name, out xy);
        wh = System.Drawing.Size.Empty;
      }
      return flag;
    }

    private void RestoreOrCenterWindow(Window window, System.Drawing.Point xy, System.Drawing.Size wh)
    {
      if (this.WpfNeedRelocation((double) xy.X, (double) xy.Y, (double) wh.Width, (double) wh.Height))
      {
        window.WindowStartupLocation = WindowStartupLocation.CenterOwner;
      }
      else
      {
        window.WindowStartupLocation = WindowStartupLocation.Manual;
        window.Left = (double) xy.X;
        window.Top = (double) xy.Y;
      }
      if (window.ResizeMode == ResizeMode.NoResize)
        return;
      window.Width = (double) wh.Width;
      window.Height = (double) wh.Height;
    }

    public void RestoreForm(
      Form form,
      int defaultX,
      int defaultY,
      int defaultWidth,
      int defaultHeight,
      FormStartPosition defaultStartPosition)
    {
      System.Drawing.Point xy;
      System.Drawing.Size wh;
      if (this.Retrieve(form.Name, out xy, out wh))
      {
        this.RestoreOrCenterForm(form, xy, wh);
      }
      else
      {
        form.StartPosition = defaultStartPosition;
        form.Location = new System.Drawing.Point(defaultX, defaultY);
        form.Size = new System.Drawing.Size(defaultWidth, defaultHeight);
      }
    }

    public void RestoreForm(Form form)
    {
      System.Drawing.Point xy;
      System.Drawing.Size wh;
      if (this.Retrieve(form, out xy, out wh))
        this.RestoreOrCenterForm(form, xy, wh);
      else
        form.StartPosition = FormStartPosition.CenterParent;
    }

    private void RestoreOrCenterForm(Form form, System.Drawing.Point xy, System.Drawing.Size wh)
    {
      bool flag = this.IsFormSizable(form);
      if (this.NeedRelocation(xy, wh))
      {
        form.StartPosition = FormStartPosition.CenterParent;
        if (!flag)
          return;
        form.Size = wh;
      }
      else
      {
        form.StartPosition = FormStartPosition.Manual;
        form.Location = xy;
        if (!flag)
          return;
        form.Size = wh;
      }
    }

    private bool NeedRelocation(System.Drawing.Point xy, System.Drawing.Size wh)
    {
      Rectangle virtualScreen = SystemInformation.VirtualScreen;
      return xy.X + 30 > virtualScreen.X + virtualScreen.Width || xy.Y + 30 > virtualScreen.Y + virtualScreen.Height || xy.X + wh.Width - 30 < virtualScreen.X || xy.Y + wh.Height - 30 < virtualScreen.Y;
    }

    public bool WpfNeedRelocation(double left, double top, double width, double height)
    {
      double virtualScreenLeft = SystemParameters.VirtualScreenLeft;
      double virtualScreenTop = SystemParameters.VirtualScreenTop;
      double virtualScreenWidth = SystemParameters.VirtualScreenWidth;
      double virtualScreenHeight = SystemParameters.VirtualScreenHeight;
      return left + 30.0 > virtualScreenLeft + virtualScreenWidth || top + 30.0 > virtualScreenTop + virtualScreenHeight || left + width - 30.0 < virtualScreenLeft || top < virtualScreenTop;
    }
  }
}
