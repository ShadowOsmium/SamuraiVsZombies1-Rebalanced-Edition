using UnityEngine;

public class SUILayoutCreator
{
	public static SUIProcess Create(SDFTreeNode data)
	{
		string text = data["type"];
		switch (text)
		{
		case "sprite":
			return CreateSprite(data);
		case "label":
			return CreateLabel(data);
		case "touchArea":
			return CreateTouchArea(data);
		case "button":
			return CreateButton(data);
		default:
			Debug.Log("SUILayout ERROR: Unknown class '" + text + "'");
			return null;
		}
	}

	private static SUIProcess CreateSprite(SDFTreeNode data)
	{
		SUISprite sUISprite = null;
		if (data["file"] != string.Empty)
		{
			sUISprite = new SUISprite(data["file"]);
		}
		else
		{
			if (data.to("dynamicFrame") == null)
			{
				Debug.Log("ERROR: XML Sprite definition missing a <file> or <dynamicFrame>.");
				return null;
			}
			SDFTreeNode sDFTreeNode = data.to("dynamicFrame");
			if (!(sDFTreeNode["color"] != string.Empty) || !(sDFTreeNode["size"] != string.Empty))
			{
				Debug.Log("ERROR: XML dynamicFrame block missing a <color> and/or a <size> data.");
				return null;
			}
			Color color = SUILayoutConv.GetColor(sDFTreeNode["color"]);
			Vector2 vector = SUILayoutConv.GetVector2(sDFTreeNode["size"]);
			sUISprite = new SUISprite(color, (int)vector.x, (int)vector.y);
			sUISprite.scale = new Vector2(1f, 1f);
		}
		if (data["hotspot"] != string.Empty)
		{
			sUISprite.hotspotPixels = SUILayoutConv.GetVector2(data["hotspot"]);
		}
		if (data["position"] != string.Empty)
		{
			sUISprite.position = SUILayoutConv.GetVector2(data["position"]);
		}
		if (data["priority"] != string.Empty)
		{
			sUISprite.priority = float.Parse(data["priority"]);
		}
		if (data["alpha"] != string.Empty)
		{
			sUISprite.alpha = float.Parse(data["alpha"]);
		}
		if (data["scale"] != string.Empty)
		{
			sUISprite.scale = SUILayoutConv.GetVector2(data["scale"]);
		}
		if (data["visible"] != string.Empty)
		{
			sUISprite.visible = SUILayoutConv.GetBool(data["visible"]);
		}
		if (data["keepAspectRatio"] != string.Empty)
		{
			sUISprite.autoscaleKeepAspectRatio = SUILayoutConv.GetBool(data["keepAspectRatio"]);
		}
		return sUISprite;
	}

	private static SUIProcess CreateButton(SDFTreeNode data)
	{
		SUIButton sUIButton = ((!(data["font"] != string.Empty)) ? new SUIButton() : new SUIButton(data["font"]));
		if (data["position"] != string.Empty)
		{
			sUIButton.position = SUILayoutConv.GetVector2(data["position"]);
		}
		if (data["priority"] != string.Empty)
		{
			sUIButton.priority = float.Parse(data["priority"]);
		}
		if (data["alpha"] != string.Empty)
		{
			sUIButton.alpha = float.Parse(data["alpha"]);
		}
		if (data["text"] != string.Empty)
		{
			sUIButton.text = SUILayoutConv.GetFormattedText(Singleton<Localizer>.instance.Parse(data["text"]));
		}
		if (data["textOffset"] != string.Empty)
		{
			sUIButton.labelOffset = SUILayoutConv.GetVector2(data["textOffset"]);
		}
		if (data["offsetWhenPressed"] != string.Empty)
		{
			sUIButton.offsetWhenPressed = SUILayoutConv.GetVector2(data["offsetWhenPressed"]);
		}
		if (data["alphaWhenDisabled"] != string.Empty)
		{
			sUIButton.alphaWhenDisabled = float.Parse(data["alphaWhenDisabled"]);
		}
		if (data["normalFrame"] != string.Empty)
		{
			sUIButton.frameNormal = data["normalFrame"];
		}
		if (data["pressedFrame"] != string.Empty)
		{
			sUIButton.framePressed = data["pressedFrame"];
		}
		if (data["disabledFrame"] != string.Empty)
		{
			sUIButton.frameDisabled = data["disabledFrame"];
		}
		if (data["pressedSound"] != string.Empty)
		{
			sUIButton.pressedSound = data["pressedSound"];
		}
		if (data["releasedSound"] != string.Empty)
		{
			sUIButton.releasedSound = data["releasedSound"];
		}
		if (data["visible"] != string.Empty)
		{
			sUIButton.visible = SUILayoutConv.GetBool(data["visible"]);
		}
		if (data["keepAspectRatio"] != string.Empty)
		{
			sUIButton.autoscaleKeepAspectRatio = SUILayoutConv.GetBool(data["keepAspectRatio"]);
		}
		return sUIButton;
	}

	private static SUIProcess CreateLabel(SDFTreeNode data)
	{
		SUILabel sUILabel = new SUILabel(data["font"]);
		if (data["shadowOffset"] != string.Empty)
		{
			sUILabel.shadowOffset = SUILayoutConv.GetVector2(data["shadowOffset"]);
		}
		if (data["shadowColor"] != string.Empty)
		{
			sUILabel.shadowColor = SUILayoutConv.GetColor(data["shadowColor"]);
		}
		if (data["text"] != string.Empty)
		{
			sUILabel.text = SUILayoutConv.GetFormattedText(Singleton<Localizer>.instance.Parse(data["text"]));
		}
		if (data["position"] != string.Empty)
		{
			sUILabel.position = SUILayoutConv.GetVector2(data["position"]);
		}
		if (data["priority"] != string.Empty)
		{
			sUILabel.priority = float.Parse(data["priority"]);
		}
		if (data["alpha"] != string.Empty)
		{
			sUILabel.alpha = float.Parse(data["alpha"]);
		}
		if (data["maxWidth"] != string.Empty)
		{
			sUILabel.maxWidth = int.Parse(data["maxWidth"]);
		}
		if (data["maxLines"] != string.Empty)
		{
			sUILabel.maxLines = int.Parse(data["maxLines"]);
		}
		if (data["fontColor"] != string.Empty)
		{
			sUILabel.fontColor = SUILayoutConv.GetColor(data["fontColor"]);
		}
		if (data["anchor"] != string.Empty)
		{
			sUILabel.anchor = SUILayoutConv.GetAnchor(data["anchor"]);
		}
		if (data["alignment"] != string.Empty)
		{
			sUILabel.alignment = SUILayoutConv.GetAlignment(data["alignment"]);
		}
		if (data["visible"] != string.Empty)
		{
			sUILabel.visible = SUILayoutConv.GetBool(data["visible"]);
		}
		return sUILabel;
	}

	private static SUIProcess CreateTouchArea(SDFTreeNode data)
	{
		SUITouchArea sUITouchArea = new SUITouchArea(SUILayoutConv.GetRect(data["rect"]));
		if (data["reverse"] != string.Empty)
		{
			sUITouchArea.reverse = SUILayoutConv.GetBool(data["reverse"]);
		}
		return sUITouchArea;
	}
}
