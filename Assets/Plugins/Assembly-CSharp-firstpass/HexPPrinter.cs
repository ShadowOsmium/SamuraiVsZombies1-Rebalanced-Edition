using System.Text;

public static class HexPPrinter
{
	private const int HexLineLength = 16;

	private const string HexBlockSeparator = "  ";

	public static void ByteArrayToString(StringBuilder sb, byte[] b)
	{
		int num = b.Length;
		int num2 = 0;
		int num3 = num - num % 16;
		if (num2 < num3)
		{
			while (true)
			{
				int offset = num2;
				ByteToHex(sb, b, num2, 8);
				num2 += 8;
				sb.Append("  ");
				ByteToHex(sb, b, num2, 8);
				num2 += 8;
				sb.Append("  ");
				ByteToASCII(sb, b, offset, 16);
				if (num2 >= num3)
				{
					break;
				}
				sb.Append('\n');
			}
		}
		int num4 = num - num2;
		if (num4 > 0)
		{
			if (num2 > 0)
			{
				sb.Append('\n');
			}
			int num5 = num2;
			int num6 = num5 + 8;
			int num7 = 8;
			if (num2 + num7 > num)
			{
				num7 = num4;
			}
			ByteToHex(sb, b, num2, num7);
			for (num2 += num7; num2 < num6; num2++)
			{
				sb.Append("   ");
			}
			sb.Append("  ");
			int num8 = num5 + 16;
			if (num2 < num)
			{
				int num9 = num - num2;
				ByteToHex(sb, b, num2, num9);
				num2 += num9;
			}
			else
			{
				sb.Append("  ");
				num2++;
			}
			for (; num2 < num8; num2++)
			{
				sb.Append("   ");
			}
			sb.Append("  ");
			ByteToASCII(sb, b, num5, num4);
			for (int i = num4; i < 16; i++)
			{
				sb.Append(' ');
			}
		}
	}

	public static void ByteToHex(StringBuilder sb, byte[] b, int offset, int len)
	{
		int num = offset + len;
		int num2 = offset;
		sb.AppendFormat("{0:X2}", (int)b[num2]);
		for (num2++; num2 < num; num2++)
		{
			sb.AppendFormat(" {0:X2}", (int)b[num2]);
		}
	}

	public static void ByteToASCII(StringBuilder sb, byte[] b, int offset, int len)
	{
		int num = offset + len;
		for (int i = offset; i < num; i++)
		{
			byte b2 = b[i];
			if (b2 >= 32 && b2 < 127)
			{
				sb.Append((char)b2);
			}
			else
			{
				sb.Append('.');
			}
		}
	}
}
