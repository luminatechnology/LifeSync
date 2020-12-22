using PX.Data;
using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace PX.Objects.GL
{
    public class BatchExt : PXCacheExtension<PX.Objects.GL.Batch>
    {
		public static string[] NumChar = { "兆", "亿", "万" };
		public static string[] NumChar1 = { "元", "万", "亿", "兆" };
		public static string[] NumChar2 = { "", "拾", "佰", "仟" };
		public static string[] NumChar3 = { "", "壹", "贰", "参", "肆", "伍", "陆", "柒", "捌", "玖" };
		public static string[] NumChar4 = { "角", "分" };

		#region UsrSwitchFromNumToCN
		[PXInt]
        [PXUIField(DisplayName = "UsrSwitchFromNumToCN", Visible = false, IsReadOnly = true)]
        public virtual string UsrSwitchFromNumToCN
        {
			

			[PXDependsOnFields(typeof(Batch.controlTotal))]
            get { return NumberToChineseNumber(Convert.ToDouble(Base.ControlTotal)); }
            set { }
        }
        public abstract class usrSwitchFromNumToCN : PX.Data.BQL.BqlString.Field<usrSwitchFromNumToCN> { }
        #endregion

		public static string NumberToChineseNumber(double num) //IsWithZero金額是否含零
		{
			bool IsWithZero = true; //預設補零

			string pattern = @"^[1-9][0-9]{0,20}[.]?[0-9]{0,3}$"; // 規則字串
			Regex regex = new Regex(pattern, RegexOptions.IgnoreCase); // 宣告 Regex 忽略大小寫
			//if (!regex.IsMatch(String.Format("{0:F3}", num)))
			//	throw new Exception("整數位數需小於等於21，小數為0到2位");
			num = Math.Round(num, 3, MidpointRounding.AwayFromZero);
			string numstr = string.Empty;
			string tempA = String.Format("{0:F3}", num).TrimEnd('.').Replace(".000", "");
			string tempB = string.Empty;
			string temp = tempA;
			if (tempA.IndexOf('.') > -1)
			{
				tempB = tempA.Substring(tempA.IndexOf('.') + 1);
				temp = tempA.Substring(0, tempA.IndexOf('.'));
			}
			//處理整數
			for (long i = temp.Length - 1; i >= 0; i--)
			{
				numstr = NumberToChar(temp.Substring((int)i, 1), temp.Length - 1 - i, IsWithZero) + numstr;
			}

			if (IsWithZero)//金額含零
				numstr = numstr.Replace("零零零", "零").Replace("零零", "零").Replace("零兆", "兆").Replace("零亿", "亿").Replace("零万", "万").Replace("兆亿", "兆").Replace("兆万", "兆").Replace("亿万", "亿").Replace("零元", "元");
			else //金額不含零
				numstr = numstr.Replace("兆亿", "兆").Replace("兆万", "兆").Replace("亿万", "亿");
			//處理小數
			if (!string.IsNullOrWhiteSpace(tempB))
			{
				for (long i = 0; i < tempB.Length; i++)
				{
					numstr += NumberToChineseChar_1(tempB.Substring((int)i, 1), i, IsWithZero);
				}
				if (IsWithZero)//金額含零
					return numstr.TrimEnd('零');
				else //金額不含零
					return numstr;
			}
			return numstr + "整";
		}
		//處理小數
		private static string NumberToChineseChar_1(string x, long LetterInx, bool IsWithZero)
		{
			if (Convert.ToInt32(x) == 0)
			{
				if (IsWithZero)//金額含零
					return "零";
				else //金額不含零
					return "";
			}
			int n = NumChar4.Length;
			if (LetterInx >= n || LetterInx < 0) return "";
			return NumberLetterToChar(Convert.ToInt32(x)) + NumChar4[LetterInx];
		}
		//處理整數
		private static string NumberToChar(string x, long LetterInx, bool IsWithZero)
		{
			if (LetterInx % 4 == 0)
			{
				Int32 temp = (int)(LetterInx / 4);

				int n = NumChar1.Length;
				if (temp >= n || temp < 0) return "";
				return NumberLetterToChar(Convert.ToInt32(x)) + NumChar1[temp];
			}
			else
			{
				if (Convert.ToInt32(x) == 0)
				{
					if (IsWithZero)//金額含零
						return "零";
					else //金額不含零
						return "";
				}
				Int32 temp = (int)(LetterInx % 4);

				int n = NumChar2.Length;
				if (temp >= n || temp < 0) return "";
				return NumberLetterToChar(Convert.ToInt32(x)) + NumChar2[temp];
			}
		}
		//數字轉大寫
		private static string NumberLetterToChar(Int32 x)
		{
			int n = NumChar3.Length;
			if (x >= n || x < 0) return "";
			return NumChar3[x];
		}
	}
}