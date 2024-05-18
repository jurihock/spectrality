using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace Spectrality.Serialization;

public partial class PropertyBag : IXmlSerializable
{
  private const string XmlRootName = nameof(PropertyBag);
  private const string XmlVersionName = "Version";
  private const string XmlVersionValue = "1";
  private const string XmlNodeName = "Property";
  private const string XmlKeyName = "Key";
  private const string XmlTypeName = "Type";

  private static readonly XmlReaderSettings DefaultXmlReaderSettings = new()
  {
    IgnoreComments = true,
    IgnoreWhitespace = true
  };

  private static readonly XmlWriterSettings DefaultXmlWriterSettings = new()
  {
    Encoding = Encoding.UTF8,
    Indent = true,
    IndentChars = new string(' ', 2)
  };

  public XmlSchema? GetSchema() => null;

  public void ReadXmlFile(string xmlFilePath)
  {
    var settings = DefaultXmlReaderSettings;
    using var reader = XmlReader.Create(xmlFilePath, settings);

    ReadXml(reader);
  }

  public void ReadXmlString(string xmlString)
  {
    var settings = DefaultXmlReaderSettings;
    using var stream = new StringReader(xmlString);
    using var reader = XmlReader.Create(stream, settings);

    ReadXml(reader);
  }

  public void ReadXml(XmlReader reader)
  {
    static Type DeserializeType(string name)
    {
      var type = Type.GetType(name);
      ArgumentNullException.ThrowIfNull(type);
      return type;
    }

    if (reader.IsEmptyElement)
      return;

    if (!reader.Read())
      throw new XmlException("Unable to read the XML data!");

    if (!reader.ReadToFollowing(XmlRootName))
      throw new XmlException("Unable to read the root XML node!");

    var version = reader.GetAttribute(XmlVersionName)!;

    if (version != XmlVersionValue)
      throw new XmlException("Unsupported XML schema version!");

    var serializers = new Dictionary<Type, XmlSerializer>();

    while (reader.ReadToFollowing(XmlNodeName))
    {
      var key = reader.GetAttribute(XmlKeyName)!;
      var type = DeserializeType(reader.GetAttribute(XmlTypeName)!);

      if (!reader.Read())
        continue;

      if (!serializers.ContainsKey(type))
        serializers.Add(type, new XmlSerializer(type));

      var value = serializers[type].Deserialize(reader)!;
      var property = new Property(key, type, value);

      Action? notify = null;

      Properties.AddOrUpdate(key,
        (_) =>
        {
          notify = () => OnPropertyAdded(key);
          return property;
        },
        (_, _) =>
        {
          OnPropertyChanging(key);
          notify = () => OnPropertyChanged(key);
          return property;
        });

      if (notify is not null)
      {
        notify();
      }
    }
  }

  public void WriteXmlFile(string xmlFilePath)
  {
    var settings = DefaultXmlWriterSettings;
    using var writer = XmlWriter.Create(xmlFilePath, settings);

    WriteXml(writer);
  }

  public string WriteXmlString()
  {
    var settings = DefaultXmlWriterSettings;
    var buffer = new StringBuilder();
    using var stream = new StringWriter(buffer);
    using var writer = XmlWriter.Create(stream, settings);

    WriteXml(writer);

    return buffer.ToString();
  }

  public void WriteXml(XmlWriter writer)
  {
    static string SerializeType(Type type)
    {
      ArgumentNullException.ThrowIfNull(type.FullName);
      ArgumentNullException.ThrowIfNull(type.AssemblyQualifiedName);

      if (Type.GetType(type.FullName) is not null)
      {
        return type.FullName;
      }

      return type.AssemblyQualifiedName;
    }

    var serializers = new Dictionary<Type, XmlSerializer>();

    writer.WriteStartElement(XmlRootName);
    writer.WriteAttributeString(XmlVersionName, XmlVersionValue);

    foreach (var property in this)
    {
      var key = property.Key;
      var value = property.Value;
      var type = property.Type;

      writer.WriteStartElement(XmlNodeName);
      writer.WriteAttributeString(XmlKeyName, key);
      writer.WriteAttributeString(XmlTypeName, SerializeType(type));

      if (!serializers.ContainsKey(type))
        serializers.Add(type, new XmlSerializer(type));

      serializers[type].Serialize(writer, value);
      writer.WriteEndElement();
    }

    writer.WriteEndElement();
    writer.Flush();
  }
}
