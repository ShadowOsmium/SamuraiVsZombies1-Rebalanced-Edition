using System;

namespace Glu.Burstly
{
	[Flags]
	public enum BannerAlignment
	{
		Bottom = 1,
		Top = 2,
		Left = 4,
		Right = 8,
		Center = 0xF,
		TopLeft = 6,
		TopRight = 0xA,
		BottomLeft = 5,
		BottomRight = 9
	}
}
