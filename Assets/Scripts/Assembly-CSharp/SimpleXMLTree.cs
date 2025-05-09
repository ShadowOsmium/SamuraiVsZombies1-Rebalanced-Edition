using System.IO;
using System.Xml;
using UnityEngine;

internal class SimpleXMLTree : SDFTreeNode
{
	public SimpleXMLTree(string filename)
	{
		TextAsset textAsset = (TextAsset)Resources.Load(filename, typeof(TextAsset));
		XmlReader xmlReader = XmlReader.Create(new StringReader(textAsset.text));
		while (xmlReader.Read())
		{
			if (xmlReader.NodeType == XmlNodeType.Element)
			{
				SDFTreeNode node = new SDFTreeNode();
				SetChild(xmlReader.Name, node);
				ProcessElement(xmlReader, node);
			}
		}
	}

	private string ProcessElement(XmlReader reader, SDFTreeNode node)
	{
		while (reader.MoveToNextAttribute())
		{
			node[reader.Name] = reader.Value;
		}
		if (reader.IsEmptyElement)
		{
			return string.Empty;
		}
		string result = string.Empty;
		while (reader.Read())
		{
			if (reader.NodeType == XmlNodeType.Element)
			{
				SDFTreeNode sDFTreeNode = new SDFTreeNode();
				string name = reader.Name;
				string text = ProcessElement(reader, sDFTreeNode);
				if (text == string.Empty || sDFTreeNode.childCount > 0 || sDFTreeNode.attributeCount > 0)
				{
					if (text != string.Empty)
					{
						sDFTreeNode[name] = text;
					}
					node.SetChild(name, sDFTreeNode);
				}
				else
				{
					node[name] = text;
				}
			}
			else if (reader.NodeType == XmlNodeType.Text)
			{
				result = reader.Value;
			}
			else if (reader.NodeType == XmlNodeType.EndElement)
			{
				break;
			}
		}
		return result;
	}
}
