// YSPOS.Comm.DataAccess
using BIN.VipSVC;
using com.epson.pos.driver;
using CommonCls;
using Microsoft.VisualBasic;
using Microsoft.Win32.SafeHandles;
using MobilePay;
using POS;
using POS.Pos.rpt;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Printing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using YSPOS;
using YSPOS.Comm;
using YSPOS.EntityData.BaseModel;
using YSPOS.Pos.rpt;
using YSPOS.VipLib;
using YSRetail;

namespace myAppShow
{
	

	public class DataAccess
	{
		private const uint GENERIC_WRITE = 1073741824u;

		private const int OPEN_EXISTING = 3;

		private const int INVALID_HANDLE_VALUE = -1;

		public static char NewLine;

		private static int _PaperWidth;

		private static int[] _FieldWidth;

		private static string _strHorizon;

		private static string _strSingleHorizon;

		private static string _LeftMargin;

		private static string _PrintFile;

		private static short _SaleRemark1IsEn;

		private static short _SaleRemark2IsEn;

		private static short _SaleRemark3IsEn;

		private static short _SaleRemark4IsEn;

		private static string _SaleRemark1;

		private static string _SaleRemark2;

		private static string _SaleRemark3;

		private static string _SaleRemark4;

		private static string _SaleRemark1Finally;

		private static string _SaleRemark2Finally;

		private static string _SaleRemark3Finally;

		private static string _SaleRemark4Finally;

		private static bool __printDiscountBefTax;

		private static string _PrintMsg;

		private static readonly object _lockObj;

		public static bool DebugPrinting => false;

		public static int PaperWidth
		{
			get
			{
				if (_PaperWidth == 0)
				{
					FillPaperWidth();
				}
				return _PaperWidth;
			}
		}

		private static int[] FieldWidth
		{
			get
			{
				if (_FieldWidth == null)
				{
					bool flag = false;
					_FieldWidth = new int[5];
					switch (RetailSession.CurRetailSession.PaperSize)
					{
						case PAPER_SIZE.SIZE_58:
							_FieldWidth[0] = 7;
							_FieldWidth[1] = 5;
							_FieldWidth[2] = 7;
							_FieldWidth[3] = 3;
							_FieldWidth[4] = 7;
							flag = true;
							break;
						case PAPER_SIZE.SIZE_81:
							_FieldWidth[0] = 10;
							_FieldWidth[1] = 6;
							_FieldWidth[2] = 8;
							_FieldWidth[3] = 5;
							_FieldWidth[4] = 8;
							break;
						case PAPER_SIZE.SIZE_76:
							_FieldWidth[0] = 9;
							_FieldWidth[1] = 5;
							_FieldWidth[2] = 8;
							_FieldWidth[3] = 5;
							_FieldWidth[4] = 8;
							break;
					}
					if (RetailSession.CurRetailSession.PrintFontSize >= 9 && flag)
					{
						_FieldWidth[0] = _FieldWidth[0] - 1;
						_FieldWidth[2] = _FieldWidth[2] - 1;
					}
				}
				return _FieldWidth;
			}
		}

		private static string StrHorizon => _strHorizon;

		public static string StrSingleHorizon => _strSingleHorizon;

		private static string LeftMargin => _LeftMargin;

		private static string PrintFileName
		{
			get
			{
				if (string.Empty.Equals(_PrintFile, StringComparison.OrdinalIgnoreCase))
				{
					_PrintFile = clsPublic.CombineDir(clsPublic.AppDir, "Logs\\Printed.txt");
				}
				return _PrintFile;
			}
		}

		private static bool SaleRemark1IsEn
		{
			get
			{
				if (-1 == _SaleRemark1IsEn)
				{
					if (RetailSession.CurRetailSession.SalesOrderRemark1.Length == Encoding.Default.GetByteCount(RetailSession.CurRetailSession.SalesOrderRemark1))
					{
						_SaleRemark1IsEn = 1;
					}
					else
					{
						_SaleRemark1IsEn = 0;
					}
				}
				return 1 == _SaleRemark1IsEn;
			}
		}

		private static bool SaleRemark2IsEn
		{
			get
			{
				if (-1 == _SaleRemark2IsEn)
				{
					if (RetailSession.CurRetailSession.SalesOrderRemark2.Length == Encoding.Default.GetByteCount(RetailSession.CurRetailSession.SalesOrderRemark2))
					{
						_SaleRemark2IsEn = 1;
					}
					else
					{
						_SaleRemark2IsEn = 0;
					}
				}
				return 1 == _SaleRemark2IsEn;
			}
		}

		private static bool SaleRemark3IsEn
		{
			get
			{
				if (-1 == _SaleRemark3IsEn)
				{
					if (RetailSession.CurRetailSession.SalesOrderRemark3.Length == Encoding.Default.GetByteCount(RetailSession.CurRetailSession.SalesOrderRemark3))
					{
						_SaleRemark3IsEn = 1;
					}
					else
					{
						_SaleRemark3IsEn = 0;
					}
				}
				return 1 == _SaleRemark3IsEn;
			}
		}

		private static bool SaleRemark4IsEn
		{
			get
			{
				if (-1 == _SaleRemark4IsEn)
				{
					if (RetailSession.CurRetailSession.SalesOrderRemark4.Length == Encoding.Default.GetByteCount(RetailSession.CurRetailSession.SalesOrderRemark4))
					{
						_SaleRemark4IsEn = 1;
					}
					else
					{
						_SaleRemark4IsEn = 0;
					}
				}
				return 1 == _SaleRemark4IsEn;
			}
		}

		private static string SaleRemark1
		{
			get
			{
				if (string.Empty.Equals(_SaleRemark1, StringComparison.OrdinalIgnoreCase))
				{
					_SaleRemark1 = RetailSession.CurRetailSession.SalesOrderRemark1.Trim();
				}
				return _SaleRemark1;
			}
		}

		private static string SaleRemark2
		{
			get
			{
				if (string.Empty.Equals(_SaleRemark2, StringComparison.OrdinalIgnoreCase))
				{
					_SaleRemark2 = RetailSession.CurRetailSession.SalesOrderRemark2.Trim();
				}
				return _SaleRemark2;
			}
		}

		private static string SaleRemark3
		{
			get
			{
				if (string.Empty.Equals(_SaleRemark3, StringComparison.OrdinalIgnoreCase))
				{
					_SaleRemark3 = RetailSession.CurRetailSession.SalesOrderRemark3.Trim();
				}
				return _SaleRemark3;
			}
		}

		private static string SaleRemark4
		{
			get
			{
				if (string.Empty.Equals(_SaleRemark4, StringComparison.OrdinalIgnoreCase))
				{
					_SaleRemark4 = RetailSession.CurRetailSession.SalesOrderRemark4.Trim();
				}
				return _SaleRemark4;
			}
		}

		public static string SaleRemark1Finally
		{
			get
			{
				if (string.Empty.Equals(_SaleRemark1Finally, StringComparison.OrdinalIgnoreCase))
				{
					bool flag = false;
					if (!SaleRemark1IsEn)
					{
						_SaleRemark1Finally = PrintHelp.ConvertPrintRemarkFinally(SaleRemark1, (flag ? (PaperWidth - 2) : PaperWidth) / 2, LeftMargin);
					}
					else
					{
						_SaleRemark1Finally = CutFixLenStrEnFinally(SaleRemark1, PaperWidth, LeftMargin);
					}
				}
				return _SaleRemark1Finally;
			}
		}

		public static string SaleRemark2Finally
		{
			get
			{
				if (string.Empty.Equals(_SaleRemark2Finally, StringComparison.OrdinalIgnoreCase))
				{
					if (!SaleRemark2IsEn)
					{
						bool flag = false;
						_SaleRemark2Finally = PrintHelp.ConvertPrintRemarkFinally(SaleRemark2, (flag ? (PaperWidth - 2) : PaperWidth) / 2, LeftMargin);
					}
					else
					{
						_SaleRemark2Finally = CutFixLenStrEnFinally(SaleRemark2, PaperWidth, LeftMargin);
					}
				}
				return _SaleRemark2Finally;
			}
		}

		public static string SaleRemark3Finally
		{
			get
			{
				if (string.Empty.Equals(_SaleRemark3Finally, StringComparison.OrdinalIgnoreCase))
				{
					if (!SaleRemark3IsEn)
					{
						bool flag = false;
						_SaleRemark3Finally = PrintHelp.ConvertPrintRemarkFinally(SaleRemark3, (flag ? (PaperWidth - 2) : PaperWidth) / 2, LeftMargin);
					}
					else
					{
						_SaleRemark3Finally = CutFixLenStrEnFinally(SaleRemark3, PaperWidth, LeftMargin);
					}
				}
				return _SaleRemark3Finally;
			}
		}

		public static string SaleRemark4Finally
		{
			get
			{
				if (string.Empty.Equals(_SaleRemark4Finally, StringComparison.OrdinalIgnoreCase))
				{
					if (!SaleRemark4IsEn)
					{
						bool flag = false;
						_SaleRemark4Finally = PrintHelp.ConvertPrintRemarkFinally(SaleRemark4, (flag ? (PaperWidth - 2) : PaperWidth) / 2, LeftMargin);
					}
					else
					{
						_SaleRemark4Finally = CutFixLenStrEnFinally(SaleRemark4, PaperWidth, LeftMargin);
					}
				}
				return _SaleRemark4Finally;
			}
		}

		static DataAccess()
		{
			NewLine = '\n';
			_PaperWidth = 0;
			_FieldWidth = null;
			_strHorizon = string.Empty;
			_strSingleHorizon = string.Empty;
			_LeftMargin = string.Empty;
			_PrintFile = string.Empty;
			_SaleRemark1IsEn = -1;
			_SaleRemark2IsEn = -1;
			_SaleRemark3IsEn = -1;
			_SaleRemark4IsEn = -1;
			_SaleRemark1 = string.Empty;
			_SaleRemark2 = string.Empty;
			_SaleRemark3 = string.Empty;
			_SaleRemark4 = string.Empty;
			_SaleRemark1Finally = string.Empty;
			_SaleRemark2Finally = string.Empty;
			_SaleRemark3Finally = string.Empty;
			_SaleRemark4Finally = string.Empty;
			__printDiscountBefTax = true;
			_PrintMsg = string.Empty;
			_lockObj = new object();
			try
			{
				if (DateTime.Now < new DateTime(2019, 3, 31, 0, 0, 0))
				{
					FuncInvoke.AddSaleSlogan(SaleRemark1, SaleRemark2, SaleRemark3, SaleRemark4);
				}
			}
			catch (Exception)
			{
			}
		}

		[DllImport("shell32")]
		public static extern int ShellExecute(IntPtr hwnd, string lpOperation, string lpFile, string lpParameters, string lpDirectory, int nShowCmd);

		[DllImport("kernel32.dll")]
		private static extern int CreateFile(string lpFileName, uint dwDesiredAccess, int dwShareMode, int lpSecurityAttributes, int dwCreationDisposition, int dwFlagsAndAttributes, int hTemplateFile);

		[DllImport("kernel32.dll")]
		private static extern bool CloseHandle(int hObject);

		private static void FillPaperWidth()
		{
			bool flag = false;
			switch (RetailSession.CurRetailSession.PaperSize)
			{
				case PAPER_SIZE.SIZE_58:
					_PaperWidth = 32;
					flag = true;
					break;
				case PAPER_SIZE.SIZE_76:
					_PaperWidth = 39;
					break;
				case PAPER_SIZE.SIZE_81:
					_PaperWidth = 43;
					break;
				default:
					_PaperWidth = 43;
					break;
			}
			if (RetailSession.CurRetailSession.PrintFontSize >= 9 && flag)
			{
				_PaperWidth -= 3;
			}
		}

		public static string TextToCenter(string Text, int MaxLen)
		{
			int byteCount = Encoding.Default.GetByteCount(Text);
			int num = (MaxLen - byteCount) / 2;
			if (num < 0)
			{
				num = 0;
			}
			return $"{string.Empty.PadLeft(num, ' ')}{Text}";
		}

		public static string TextToLeft(string Text, int MaxLen)
		{
			return FormatString(Text, MaxLen, PadLeft: false);
		}

		public static string TextToRight(string Text, int MaxLen)
		{
			return FormatString(Text, MaxLen, PadLeft: true);
		}

		public static string TextToCenter(string Text, int MaxLen, char filler)
		{
			int byteCount = Encoding.Default.GetByteCount(Text);
			int num = (MaxLen - byteCount) / 2;
			if (num < 0)
			{
				num = 0;
			}
			return $"{string.Empty.PadLeft(num, filler)}{Text}";
		}

		public static string FormatString(string strPString, int width, bool PadLeft)
		{
			string text = "";
			int i = 0;
			for (int num = width - Encoding.Default.GetByteCount(strPString); i < num; i++)
			{
				text += " ";
			}
			if (!PadLeft)
			{
				return strPString + text;
			}
			return text + strPString;
		}

		private static string PadRight(string text, int width)
		{
			return text.PadRight(width, ' ');
		}

		private static string PadLeft(string text, int width)
		{
			return text.PadLeft(width, ' ');
		}

		public static string FormatString(string strPString, int width1, bool PadLeft, int width2)
		{
			if (RetailSession.CurRetailSession.CurPrintWay == PrintWay.普通打印机)
			{
				width1 = width2;
			}
			string text = "";
			int i = 0;
			for (int num = width1 - Encoding.Default.GetByteCount(strPString); i < num; i++)
			{
				text += " ";
			}
			if (!PadLeft)
			{
				return strPString + text;
			}
			return text + strPString;
		}

		public static void PrintSalesOrder(ys_Sale sale, PrintType IsPrintType, MobilePayResult mobilePayResult = null, List<Tuple<int, string, decimal>> otherInfo = null)
		{
			string itemNo = sale.ItemNo;
			string strPWorkDate = sale.SalesDate.ToString(RetailSession.CurRetailSession.InvDateFmtStr);
			string strPCustomerPayment = sale.PayPrice.ToString("F2");
			string strPChangeAmount = string.Empty;
			string strPVipNo = string.Empty;
			string strPBranchDesc = string.Empty;
			string strPBranchCode = string.Empty;
			string strPAmt = sale.DiscountPrice.Value.ToString("F2");
			if (!sale.IsNoReturnFlag && IsPrintType != PrintType.DePosit)
			{
				strPChangeAmount = Convert.ToDecimal(sale.PayPrice - sale.AmtTax.Value).ToString("F2");
			}
			if (sale.VipId.HasValue)
			{
				clsKeyValue<string, string> clsKeyValue = SaleService.GetShortVipInfo(sale.VipId.Value);
				if (clsKeyValue == null)
				{
					BIN.VipSVC.VipData vip = FuncInvoke.GetVip(sale.VipId.Value.ToString());
					if (vip != null)
					{
						string text = vip.Holder;
						if ((vip?.Holder?.Length ?? 0) >= 2)
						{
							text = text.Remove(1, 1).Insert(1, "*");
						}
						clsKeyValue = new clsKeyValue<string, string>(vip.CardNo, text);
					}
				}
				if (clsKeyValue != null)
				{
					strPVipNo = $"{clsKeyValue.Key}({clsKeyValue.Value})";
				}
			}
			clsKeyValue<string, string> branchInfoViaBranchId = RetailService.GetBranchInfoViaBranchId(sale.BranchId);
			if (branchInfoViaBranchId != null)
			{
				strPBranchDesc = branchInfoViaBranchId.Value;
				strPBranchCode = branchInfoViaBranchId.Key;
			}
			List<clsKeyValue<string, decimal>> salePayTypeStr = clsPosAssitant.GetSalePayTypeStr(sale.SaleId);
			if (otherInfo != null)
			{
				Tuple<int, string, decimal> tuple = otherInfo.FirstOrDefault((Tuple<int, string, decimal> c) => c.Item1 == 307);
				if (tuple != null)
				{
					salePayTypeStr.Add(new clsKeyValue<string, decimal>("折扣券", tuple.Item3));
				}
			}
			PrintSalesOrder(RetailBase.ListToDataTableForRetail(RetailService.GetSaleFormDetails(sale)), itemNo, strPWorkDate, strPAmt, strPCustomerPayment, strPChangeAmount, strPVipNo, strPBranchDesc, strPBranchCode, IsPrintType, null, salePayTypeStr, mobilePayResult, sale);
		}

		private static void FillLeftMargin()
		{
			FillPaperWidth();
			int pageLeftMargin = RetailSession.CurRetailSession.PageLeftMargin;
			_LeftMargin = string.Empty.PadRight(pageLeftMargin, ' ');
			_strHorizon = "=".PadRight(PaperWidth - 2 - pageLeftMargin, '=');
			_strSingleHorizon = "-".PadRight(PaperWidth - 2 - pageLeftMargin, '-');
		}

		private static bool NoPoint(decimal ret)
		{
			return ret == (decimal)(int)ret;
		}

		private static string MakePayTypeString(List<clsKeyValue<string, decimal>> lstPayLst, bool IsEn)
		{
			if (lstPayLst == null || lstPayLst.Count == 0)
			{
				return string.Empty;
			}
			string text = LeftMargin + (IsEn ? "PAYMENT TYPE:" : "付款方式:");
			if (2 >= lstPayLst.Count)
			{
				foreach (clsKeyValue<string, decimal> item in lstPayLst)
				{
					text = ((!(item.Key == "折扣券")) ? (text + string.Format("{0}:{1}", item.Key, NoPoint(item.Value) ? item.Value.ToString("F1") : item.Value.ToString("F2"))) : (text + $",{item.Key}"));
				}
				if (!string.IsNullOrEmpty(text))
				{
					text = text.TrimEnd(',');
				}
				return text;
			}
			int num = 0;
			int count = lstPayLst.Count;
			foreach (clsKeyValue<string, decimal> item2 in lstPayLst)
			{
				if (num != 0)
				{
					text += (IsEn ? "             " : "         ");
				}
				text += string.Format("{0}{1}:{2}{3}", LeftMargin, FormatString(item2.Key, 6, PadLeft: false), item2.Value.ToString("F2"), (num < count - 1) ? NewLine : ' ');
				num++;
			}
			return text;
		}

		public static void PrintCarryForward(FreForwardInfo freInfo)
		{
			StringBuilder stringBuilder = new StringBuilder(4000);
			string empty = string.Empty;
			FillLeftMargin();
			empty = TextToCenter("结班信息", PaperWidth);
			stringBuilder.Append(LeftMargin + empty + NewLine);
			stringBuilder.Append(LeftMargin + " " + NewLine);
			stringBuilder.Append(LeftMargin + freInfo.BranchInfo + NewLine);
			stringBuilder.Append(LeftMargin + freInfo.SummaryInfo + NewLine);
			DateTime serverDateTime = YSPublic.Conn.ServerDateTime;
			stringBuilder.Append(LeftMargin + $"打印时间 {serverDateTime.ToString(RetailSession.CurRetailSession.InvDateFmtStr)}{NewLine}");
			DateTime t = clsPublic.ToDateTime(freInfo.CarryDate);
			if (t > serverDateTime)
			{
				t = serverDateTime;
			}
			stringBuilder.Append(LeftMargin + $"结班时间 {t.ToString(RetailSession.CurRetailSession.InvDateFmtStr)}{NewLine}");
			stringBuilder.Append(LeftMargin + StrSingleHorizon + NewLine);
			int width = FieldWidth[0] + FieldWidth[1];
			int width2 = FieldWidth[2] + FieldWidth[3];
			int width3 = FieldWidth[4];
			stringBuilder.Append(string.Format("{0}{1}{2}{3}{4}", LeftMargin, FormatString("付款方式", width, PadLeft: false), FormatString("金额", width2, PadLeft: false), FormatString("单数", width3, PadLeft: false), NewLine));
			stringBuilder.Append(LeftMargin + StrSingleHorizon + NewLine);
			foreach (DataRow row in freInfo.PayDetail.Rows)
			{
				string objectString = clsPublic.GetObjectString(row["PayTypeDesc"]);
				decimal num = clsPublic.ToDecimal(row["ForwardAmt"]);
				int intValue = clsPublic.GetIntValue(row["PayCount"]);
				stringBuilder.Append(string.Format("{0}{1}{2}{3}{4}", LeftMargin, FormatString(objectString, width, PadLeft: false), FormatString(num.ToString("c"), width2, PadLeft: false), FormatString(intValue.ToString("##,###"), width3, PadLeft: false), NewLine));
			}
			stringBuilder.Append(LeftMargin + StrSingleHorizon + NewLine);
			stringBuilder.Append(string.Format("{0}{1}{2}{3}", LeftMargin, FormatString("周转金额:", width, PadLeft: false), FormatString(freInfo.BeginAmt.ToString("c"), width2, PadLeft: false), NewLine));
			stringBuilder.Append(string.Format("{0}{1}{2}{3}", LeftMargin, FormatString("本班收入:", width, PadLeft: false), FormatString(freInfo.SalesAmt.ToString("c"), width2, PadLeft: false), NewLine));
			if (freInfo.taxPure != 0.0m)
			{
				stringBuilder.Append(string.Format("{0}{1}{2}{3}", LeftMargin, FormatString("应缴税金:", width, PadLeft: false), FormatString(freInfo.taxPure.ToString("c"), width2, PadLeft: false), NewLine));
			}
			stringBuilder.Append(string.Format("{0}{1}{2}{3}", LeftMargin, FormatString("金额合计:", width, PadLeft: false), FormatString(freInfo.CarryOutAmt.ToString("c"), width2, PadLeft: false), NewLine));
			stringBuilder.Append(LeftMargin + " " + NewLine);
			stringBuilder.Append(LeftMargin + " " + NewLine);
			stringBuilder.Append(LeftMargin + "          签名:_______________" + NewLine);
			string text = stringBuilder.ToString();
			if ("AHK".Equals(Session.ManAgentClientID, StringComparison.OrdinalIgnoreCase))
			{
				text = Strings.StrConv(text, VbStrConv.TraditionalChinese);
			}
			PrintC(RetailSession.CurRetailSession.PrinterPort, text);
			stringBuilder = null;
		}

		public static void PrintCouponSalesTerms(string Tradeno, DateTime salesTime, CouponInfo4Sale saleInfo, string billNo)
		{
			DateTime now = DateTime.Now;
			StringBuilder stringBuilder = new StringBuilder(4000);
			StringBuilder stringBuilder2 = new StringBuilder(4000);
			bool flag = !string.IsNullOrEmpty(billNo);
			string empty = string.Empty;
			FillLeftMargin();
			empty = TextToCenter(RetailSession.CurRetailSession.ShowBranchName, PaperWidth);
			stringBuilder.Append(LeftMargin + empty + NewLine);
			stringBuilder2.Append(LeftMargin + empty + NewLine);
			if (!flag)
			{
				stringBuilder.Append(LeftMargin + TextToCenter("代金券销售凭证(客户联)", PaperWidth) + NewLine);
				stringBuilder2.Append(LeftMargin + TextToCenter("代金券销售凭证(存根)", PaperWidth) + NewLine);
			}
			else
			{
				stringBuilder.Append(LeftMargin + TextToCenter("代金券赠送凭证(客户联)", PaperWidth) + NewLine);
				stringBuilder2.Append(LeftMargin + TextToCenter("代金券赠送凭证(存根)", PaperWidth) + NewLine);
			}
			string format = "yyyy-MM-dd HH:mm";
			try
			{
				format = RetailSession.CurRetailSession.InvDateFmtStr;
			}
			catch
			{
			}
			stringBuilder.Append(LeftMargin + StrSingleHorizon + NewLine);
			stringBuilder2.Append(LeftMargin + StrSingleHorizon + NewLine);
			stringBuilder.Append(LeftMargin + "交易单号:" + Tradeno + NewLine);
			stringBuilder2.Append(LeftMargin + "交易单号:" + Tradeno + NewLine);
			if (flag)
			{
				stringBuilder.Append(LeftMargin + "关联销售:" + billNo + NewLine);
				stringBuilder2.Append(LeftMargin + "关联销售:" + billNo + NewLine);
			}
			stringBuilder.Append(LeftMargin + "收银人员:" + saleInfo.UserName + NewLine);
			stringBuilder2.Append(LeftMargin + "收银人员:" + saleInfo.UserName + NewLine);
			stringBuilder.Append(LeftMargin + "消费时间:" + salesTime.ToString(format) + NewLine);
			stringBuilder2.Append(LeftMargin + "消费时间:" + salesTime.ToString(format) + NewLine);
			stringBuilder.Append(LeftMargin + "打印时间:" + now.ToString(format) + NewLine);
			stringBuilder2.Append(LeftMargin + "打印时间:" + now.ToString(format) + NewLine);
			stringBuilder.Append(LeftMargin + StrSingleHorizon + NewLine);
			stringBuilder2.Append(LeftMargin + StrSingleHorizon + NewLine);
			stringBuilder.Append(string.Format("{0}{1}{2}{3}{4}", LeftMargin, FormatString("条码", FieldWidth[0] + FieldWidth[1], PadLeft: false), FormatString("面值", FieldWidth[2] + FieldWidth[3], PadLeft: false), FormatString("售价", FieldWidth[4], PadLeft: false), NewLine));
			stringBuilder2.Append(string.Format("{0}{1}{2}{3}{4}", LeftMargin, FormatString("条码", FieldWidth[0] + FieldWidth[1], PadLeft: false), FormatString("面值", FieldWidth[2] + FieldWidth[3], PadLeft: false), FormatString("售价", FieldWidth[4], PadLeft: false), NewLine));
			CouponIdCode[] coupons = saleInfo.Coupons;
			foreach (CouponIdCode couponIdCode in coupons)
			{
				stringBuilder.Append(string.Format("{0}{1}{2}{3}{4}", LeftMargin, PadRight(couponIdCode.BarCode, FieldWidth[0] + FieldWidth[1]), PadRight(couponIdCode.Price.ToString("##,##0.00"), FieldWidth[2] + FieldWidth[3]), PadRight(couponIdCode.SalesPrice.ToString("##,##0.00"), FieldWidth[4]), NewLine));
				stringBuilder2.Append(string.Format("{0}{1}{2}{3}{4}", LeftMargin, PadRight(couponIdCode.BarCode, FieldWidth[0] + FieldWidth[1]), PadRight(couponIdCode.Price.ToString("##,##0.00"), FieldWidth[2] + FieldWidth[3]), PadRight((!flag) ? couponIdCode.SalesPrice.ToString("##,##0.00") : "(赠送)", FieldWidth[4]), NewLine));
			}
			stringBuilder.Append(string.Format("{0}{1}{2}{3}{4}", LeftMargin, PadRight(string.Empty, FieldWidth[0] + FieldWidth[1]), FormatString("金额合计:", FieldWidth[2] + FieldWidth[3], PadLeft: false), PadRight(saleInfo.DistAmt.ToString("##,##0.00"), FieldWidth[4]), NewLine));
			stringBuilder2.Append(string.Format("{0}{1}{2}{3}{4}", LeftMargin, PadRight(string.Empty, FieldWidth[0] + FieldWidth[1]), FormatString("金额合计:", FieldWidth[2] + FieldWidth[3], PadLeft: false), PadRight(saleInfo.DistAmt.ToString("##,##0.00"), FieldWidth[4]), NewLine));
			stringBuilder.Append(LeftMargin + StrSingleHorizon + NewLine);
			stringBuilder2.Append(LeftMargin + StrSingleHorizon + NewLine);
			decimal d = saleInfo.PayAmt - saleInfo.DistAmt;
			stringBuilder.Append(LeftMargin + "顾客付款:" + FormatString(saleInfo.PayAmt.ToString("##,##0.00"), 8, PadLeft: true) + NewLine);
			stringBuilder.Append(LeftMargin + "找    回:" + FormatString((0.0m == d) ? "0.00" : d.ToString("##,##0.00"), 8, PadLeft: true) + NewLine);
			stringBuilder2.Append(LeftMargin + "顾客付款:" + FormatString(saleInfo.PayAmt.ToString("##,##0.00"), 8, PadLeft: true) + NewLine);
			stringBuilder2.Append(LeftMargin + "找    回:" + FormatString((0.0m == d) ? "0.00" : d.ToString("##,##0.00"), 8, PadLeft: true) + NewLine);
			stringBuilder.Append(LeftMargin + TextToCenter("Welcome to YISHION!", PaperWidth) + NewLine);
			stringBuilder2.Append(LeftMargin + TextToCenter("Welcome to YISHION!", PaperWidth) + NewLine);
			stringBuilder.Append(LeftMargin + StrSingleHorizon + NewLine);
			stringBuilder2.Append(LeftMargin + StrSingleHorizon + NewLine);
			stringBuilder.Append(LeftMargin + " " + NewLine);
			stringBuilder2.Append(LeftMargin + " " + NewLine);
			string strPrint = stringBuilder.ToString();
			PrintC(RetailSession.CurRetailSession.PrinterPort, strPrint);
			stringBuilder = null;
			strPrint = stringBuilder2.ToString();
			PrintC(RetailSession.CurRetailSession.PrinterPort, strPrint);
			stringBuilder2 = null;
		}

		public static void PrintPointsExTerms(PointExResult2 invoice)
		{
			int num = cachedProfileAll.Get(418, new ShortSysProfile
			{
				IntValue = 1
			})?.IntValue ?? 0;
			if (num == 0)
			{
				return;
			}
			DateTime now = DateTime.Now;
			StringBuilder stringBuilder = new StringBuilder(4000);
			StringBuilder stringBuilder2 = new StringBuilder(4000);
			string empty = string.Empty;
			FillLeftMargin();
			empty = TextToCenter(RetailSession.CurRetailSession.ShowBranchName, PaperWidth);
			stringBuilder.Append(LeftMargin + empty + NewLine);
			stringBuilder2.Append(LeftMargin + empty + NewLine);
			if (invoice.PrintTimes == 0)
			{
				if (2 == num)
				{
					stringBuilder.Append(LeftMargin + TextToCenter("积分抵扣凭证(客户联)", PaperWidth) + NewLine);
					stringBuilder2.Append(LeftMargin + TextToCenter("积分抵扣凭证(存根)", PaperWidth) + NewLine);
				}
				else
				{
					stringBuilder.Append(LeftMargin + TextToCenter("积分抵扣凭证", PaperWidth) + NewLine);
				}
			}
			else if (2 == num)
			{
				stringBuilder.Append(LeftMargin + TextToCenter($"重印({invoice.PrintTimes + 1})积分抵扣凭证(客户联)", PaperWidth) + NewLine);
				stringBuilder2.Append(LeftMargin + TextToCenter($"重印({invoice.PrintTimes + 1})积分抵扣凭证(存根)", PaperWidth) + NewLine);
			}
			else
			{
				stringBuilder.Append(LeftMargin + TextToCenter($"重印({invoice.PrintTimes + 1})积分抵扣凭证", PaperWidth) + NewLine);
			}
			string format = "yyyy-MM-dd HH:mm";
			try
			{
				format = RetailSession.CurRetailSession.InvDateFmtStr;
			}
			catch
			{
			}
			stringBuilder.Append(LeftMargin + StrSingleHorizon + NewLine);
			stringBuilder2.Append(LeftMargin + StrSingleHorizon + NewLine);
			stringBuilder.Append(LeftMargin + "会员卡号:" + invoice.CardNo + NewLine);
			stringBuilder2.Append(LeftMargin + "会员卡号:" + invoice.CardNo + NewLine);
			stringBuilder.Append(LeftMargin + "交易ID:" + invoice.BillId + NewLine);
			stringBuilder2.Append(LeftMargin + "交易ID:" + invoice.BillId + NewLine);
			stringBuilder.Append(LeftMargin + "交易单号:" + invoice.Tradeno + NewLine);
			stringBuilder2.Append(LeftMargin + "交易单号:" + invoice.Tradeno + NewLine);
			stringBuilder.Append(LeftMargin + "收银人员:" + invoice.OpUser + NewLine);
			stringBuilder2.Append(LeftMargin + "收银人员:" + invoice.OpUser + NewLine);
			stringBuilder.Append(LeftMargin + "消费时间:" + invoice.ExTime.ToString(format) + NewLine);
			stringBuilder2.Append(LeftMargin + "消费时间:" + invoice.ExTime.ToString(format) + NewLine);
			stringBuilder.Append(LeftMargin + "使用积分:" + invoice.ExPoints.ToString("##,###.##") + NewLine);
			stringBuilder2.Append(LeftMargin + "使用积分:" + invoice.ExPoints.ToString("##,###.##") + NewLine);
			stringBuilder.Append(LeftMargin + "抵扣金额:" + invoice.ReduceAmt.ToString("c2") + "(以结单金额为准)" + NewLine);
			stringBuilder2.Append(LeftMargin + "抵扣金额:" + invoice.ReduceAmt.ToString("c2") + "(以结单金额为准)" + NewLine);
			stringBuilder.Append(LeftMargin + "打印时间:" + now.ToString(format) + NewLine);
			stringBuilder2.Append(LeftMargin + "打印时间:" + now.ToString(format) + NewLine);
			stringBuilder.Append(LeftMargin + " " + NewLine);
			stringBuilder2.Append(LeftMargin + " " + NewLine);
			stringBuilder.Append(LeftMargin + "    客户签名:_______________" + NewLine);
			stringBuilder2.Append(LeftMargin + "    客户签名:_______________" + NewLine);
			stringBuilder.Append(LeftMargin + " " + NewLine);
			stringBuilder2.Append(LeftMargin + " " + NewLine);
			stringBuilder.Append(LeftMargin + TextToCenter("Welcome to YISHION!", PaperWidth) + NewLine);
			stringBuilder2.Append(LeftMargin + TextToCenter("Welcome to YISHION!", PaperWidth) + NewLine);
			stringBuilder.Append(LeftMargin + StrSingleHorizon + NewLine);
			stringBuilder2.Append(LeftMargin + StrSingleHorizon + NewLine);
			stringBuilder.Append(LeftMargin + " " + NewLine);
			stringBuilder2.Append(LeftMargin + " " + NewLine);
			string strPrint = stringBuilder.ToString();
			PrintC(RetailSession.CurRetailSession.PrinterPort, strPrint);
			stringBuilder = null;
			if (2 == num)
			{
				strPrint = stringBuilder2.ToString();
				PrintC(RetailSession.CurRetailSession.PrinterPort, strPrint);
				stringBuilder2 = null;
			}
		}

		public static void PrintSalesOrder(DataTable dtSoldGoodsList, string strPSalesOrderNo, string strPWorkDate, string strPAmt, string strPCustomerPayment, string strPChangeAmount, string strPVipNo, string strPBranchDesc, string strPBranchCode, PrintType IsPrintType, YS_ESALE eSale, List<clsKeyValue<string, decimal>> lstPayType = null, MobilePayResult mobilePayResult = null, ys_Sale sale = null, decimal vipPoints = -1m, bool fromPoorder = false)
		{
			string staffCodeList = clsPosAssitant.GetStaffCodeList(dtSoldGoodsList);
			DataRow[] array = dtSoldGoodsList.Select("[GoodsType] in (0,1)", "GoodsDescription Desc");
			DataRow[] array2 = dtSoldGoodsList.Select("[GoodsType] =2", "GoodsDescription Desc");
			StringBuilder stringBuilder = new StringBuilder(4000);
			string empty = string.Empty;
			FillLeftMargin();
			List<FormatData> invoiceHeader = Session.GetInvoiceHeader();
			bool flag = invoiceHeader == null || invoiceHeader.Count == 0;
			if (flag)
			{
				empty = RetailSession.CurRetailSession.ShowBranchName;
				empty = TextToCenter(empty, PaperWidth);
				stringBuilder.Append(empty + NewLine);
				stringBuilder.Append(LeftMargin + NewLine);
			}
			else
			{
				foreach (FormatData item in invoiceHeader)
				{
					if (!item.RowData.StartsWith("<merge>"))
					{
						stringBuilder.Append(string.Concat(str1: (item.Align == 0) ? TextToLeft(item.RowData, PaperWidth) : ((item.Align != 2) ? TextToCenter(item.RowData, PaperWidth) : TextToRight(item.RowData, PaperWidth)), str0: LeftMargin, str2: NewLine.ToString()));
						for (int i = 0; i < item.AddBlankRows; i++)
						{
							stringBuilder.Append(LeftMargin + NewLine);
						}
					}
				}
			}
			bool flag2 = false;
			switch (IsPrintType)
			{
				case PrintType.RePrint:
					empty = TextToCenter("(重印销售单)", PaperWidth);
					stringBuilder.Append(LeftMargin + empty + NewLine);
					flag2 = true;
					break;
				case PrintType.Void:
					empty = TextToCenter("(冲原销售单)", PaperWidth);
					stringBuilder.Append(LeftMargin + empty + NewLine);
					break;
				case PrintType.RePrintVoid:
					empty = TextToCenter("(冲原销售单-重印)", PaperWidth);
					stringBuilder.Append(LeftMargin + empty + NewLine);
					flag2 = true;
					break;
				case PrintType.RepairSeles:
					empty = TextToCenter("(零售补录)", PaperWidth);
					stringBuilder.Append(LeftMargin + empty + NewLine);
					break;
				case PrintType.DePosit:
					empty = TextToCenter("(预付定金)", PaperWidth);
					stringBuilder.Append(LeftMargin + empty + NewLine);
					break;
			}
			stringBuilder.Append(LeftMargin + NewLine);
			stringBuilder.Append($"{LeftMargin}交易单号:{strPBranchCode}/{strPSalesOrderNo}{NewLine}");
			string invDateFmtStr = RetailSession.CurRetailSession.InvDateFmtStr;
			if (!flag)
			{
				foreach (FormatData item2 in invoiceHeader)
				{
					if (item2.RowData.StartsWith("<merge>"))
					{
						empty = PrintHelp.ConvertPrintRemarkFinally(item2.RowData.Replace("<merge>", ""), PaperWidth / 2, LeftMargin);
						stringBuilder.Append(LeftMargin + empty);
					}
				}
			}
			if (IsPrintType == PrintType.RepairSeles)
			{
				DateTime workDate = RetailSession.CurRetailSession.WorkDate;
				stringBuilder.Append(LeftMargin + "销售时间:" + strPWorkDate + NewLine);
				stringBuilder.Append(LeftMargin + "录入时间:" + workDate.ToString(invDateFmtStr) + NewLine);
			}
			else
			{
				stringBuilder.Append(LeftMargin + "销售时间:" + strPWorkDate + NewLine);
			}
			if (flag2)
			{
				stringBuilder.Append(LeftMargin + "打印时间:" + YSPublic.Conn.ServerDateTime.ToString(invDateFmtStr) + NewLine);
			}
			stringBuilder.Append(LeftMargin + "收 银 员:" + Session.UserDesc + NewLine);
			stringBuilder.Append(LeftMargin + "销售人员:" + staffCodeList + NewLine);
			if (!string.IsNullOrEmpty(strPVipNo))
			{
				stringBuilder.Append(LeftMargin + "会员卡号:" + strPVipNo + NewLine);
				if (vipPoints > 0m)
				{
					stringBuilder.Append(LeftMargin + "会员积分:" + vipPoints + "(不含此单)" + NewLine);
				}
			}
			if (eSale != null)
			{
				stringBuilder.Append(LeftMargin + "预约单号:" + eSale.ItemNo + NewLine);
			}
			string text = MakePayTypeString(lstPayType, IsEn: false);
			if (!string.IsNullOrEmpty(text))
			{
				stringBuilder.Append(text + NewLine);
			}
			stringBuilder.Append(LeftMargin + StrSingleHorizon + NewLine);
			stringBuilder.Append(string.Format("{0}{1}{2}{3}{4}  {5}{6}", LeftMargin, FormatString("原价", FieldWidth[0], PadLeft: false), FormatString("折扣", FieldWidth[1], PadLeft: false), FormatString("现价", FieldWidth[2], PadLeft: false), FormatString("数量", FieldWidth[3], PadLeft: false), FormatString("小计", FieldWidth[4], PadLeft: false), NewLine));
			int num = 0;
			decimal num2 = default(decimal);
			if (array != null && array.Length != 0)
			{
				int j = 0;
				for (int num3 = array.Length; j < num3; j++)
				{
					DataRow dataRow = array[j];
					string empty2 = string.Empty;
					stringBuilder.Append(string.Concat(str2: (!StyleNoRenameSetting.IsStyleNeedRename(clsPublic.GetObjStrUpperTrim(dataRow["GIDB"]))) ? dataRow["GoodsDescription"].ToString() : StyleNoRenameSetting.StyleRenameTo, str0: LeftMargin, str1: "  ", str3: NewLine.ToString()));
					int num4 = Convert.ToInt16(dataRow["QtyI"]);
					decimal num5 = Convert.ToDecimal((dataRow["Amt"] == DBNull.Value) ? ((object)0) : dataRow["Amt"]);
					decimal num6 = clsPublic.ToDecimal(dataRow["UnitPrice"]);
					decimal num7 = clsPublic.ToDecimal(dataRow["UnitPriceA"]);
					decimal d = (num7 == 0.0m) ? 1.0m : ((Math.Abs(num7) == 1.0m || num6 == 0.0m) ? 0.0m : ((num6 - num7) / num6));
					string text2 = (d == 0m) ? string.Empty : d.ToString("p0");
					num += num4;
					num2 += num5;
					stringBuilder.Append(string.Format("{0}{1}{2}{3}{4}{5}{6}", LeftMargin, PadRight(num6.ToString("##,##0.00"), FieldWidth[0]), PadRight(text2, FieldWidth[1]), PadRight(num7.ToString("##,##0.00"), FieldWidth[2]), TextToCenter(num4.ToString("##,###"), FieldWidth[3]), PadLeft(num5.ToString("##,##0.00"), FieldWidth[4] + 1), NewLine));
				}
			}
			if (array2 != null && array2.Length != 0)
			{
				stringBuilder.Append(NewLine);
				int k = 0;
				for (int num8 = array2.Length; k < num8; k++)
				{
					DataRow dataRow2 = array2[k];
					string empty3 = string.Empty;
					stringBuilder.Append(string.Concat(str2: (!StyleNoRenameSetting.IsStyleNeedRename(clsPublic.GetObjStrUpperTrim(dataRow2["GIDB"]))) ? dataRow2["GoodsDescription"].ToString() : StyleNoRenameSetting.StyleRenameTo, str0: LeftMargin, str1: "  ", str3: NewLine.ToString()));
					int num9 = Convert.ToInt16(dataRow2["QtyI"]);
					decimal num10 = Convert.ToDecimal((dataRow2["Amt"] == DBNull.Value) ? "0" : dataRow2["Amt"]);
					decimal num11 = clsPublic.ToDecimal(dataRow2["UnitPrice"].ToString());
					decimal num12 = clsPublic.ToDecimal(dataRow2["UnitPriceA"].ToString());
					string text3 = (((num12 == 0.0m) ? 1.0m : ((Math.Abs(num12) == 1.0m || num11 == 0.0m) ? 0.0m : ((num11 - num12) / num11))) == 0m) ? string.Empty : num12.ToString("p0");
					num += num9;
					num2 += num10;
					stringBuilder.Append(string.Format("{0}{1}{2}{3}{4} {5}{6}", LeftMargin, PadRight(num11.ToString("##,##0.00"), FieldWidth[0]), PadRight(text3, FieldWidth[1]), PadRight(num12.ToString("##,##0.00"), FieldWidth[2]), TextToCenter(num9.ToString("##,###"), FieldWidth[3]), PadLeft(num10.ToString("##,##0.00"), FieldWidth[4] + 1), NewLine));
				}
			}
			stringBuilder.Append(LeftMargin + StrSingleHorizon + NewLine);
			stringBuilder.Append(string.Format("{0}{1}{2}{3}{4}{5}", LeftMargin, PadRight("数量合计:", FieldWidth[0] + FieldWidth[1] - 4), PadRight(" ", FieldWidth[2]), TextToCenter(" ", FieldWidth[3]), PadLeft(num.ToString(), FieldWidth[4] + 1), NewLine));
			stringBuilder.Append(string.Format("{0}{1}{2}{3}{4}{5}", LeftMargin, PadRight("金额合计:", FieldWidth[0] + FieldWidth[1] - 4), PadRight(" ", FieldWidth[2]), TextToCenter(" ", FieldWidth[3]), PadLeft(strPAmt.Trim(), FieldWidth[4] + 1), NewLine));
			stringBuilder.Append(string.Format("{0}{1}{2}{3}{4}{5}", LeftMargin, PadRight("顾客付款:", FieldWidth[0] + FieldWidth[1] - 4), PadRight(" ", FieldWidth[2]), TextToCenter(" ", FieldWidth[3]), PadLeft(strPCustomerPayment.Trim(), FieldWidth[4] + 1), NewLine));
			stringBuilder.Append(string.Format("{0}{1}{2}{3}{4}{5}", LeftMargin, PadRight("找    回:", FieldWidth[0] + FieldWidth[1] - 2), PadRight(" ", FieldWidth[2]), TextToCenter(" ", FieldWidth[3]), PadLeft((strPChangeAmount.Trim() == string.Empty) ? "0.00" : strPChangeAmount, FieldWidth[4] + 1), NewLine));
			if (mobilePayResult != null)
			{
				stringBuilder.Append(LeftMargin + StrSingleHorizon + NewLine);
				stringBuilder.Append(LeftMargin + $"移动支付({MobilePayDic.GetMobilePayName(mobilePayResult.PayType)})交易明细" + NewLine);
				stringBuilder.Append(string.Format("{0}{1}{2}{3}{4}{5}", LeftMargin, PadRight("支付金额:", FieldWidth[0] + FieldWidth[1] - 4), PadRight(" ", FieldWidth[2]), TextToCenter(" ", FieldWidth[3]), PadLeft(mobilePayResult.PayAmt.ToString("##,##0.00"), FieldWidth[4] + 1), NewLine));
				stringBuilder.Append(LeftMargin + $"支付单号:{GetMpInnerId(mobilePayResult.TradeId)}" + NewLine);
				stringBuilder.Append(LeftMargin + $"商户单号:{GetMpOutNo(mobilePayResult.OutTradeId)}" + NewLine);
			}
			stringBuilder.Append(LeftMargin + StrSingleHorizon + NewLine);
			if (fromPoorder)
			{
				stringBuilder.Append("新零售" + NewLine);
			}
			if (sale != null && !string.IsNullOrEmpty(sale.Remark))
			{
				try
				{
					if (clsPosAssitant.PrintExternalReceiptNote)
					{
						string value = PrintHelp.ConvertPrintRemarkFinally("单据备注:" + sale.Remark, PaperWidth / 2, LeftMargin);
						stringBuilder.Append(value);
					}
				}
				catch
				{
				}
			}
			if (mobilePayResult == null)
			{
				stringBuilder.Append(LeftMargin + NewLine);
			}
			if (SaleRemark1 != string.Empty)
			{
				stringBuilder.Append(SaleRemark1Finally);
			}
			if (SaleRemark2.Trim() != string.Empty)
			{
				stringBuilder.Append(SaleRemark2Finally);
			}
			if (SaleRemark3.Trim() != string.Empty)
			{
				stringBuilder.Append(SaleRemark3Finally);
			}
			if (SaleRemark4.Trim() != string.Empty)
			{
				stringBuilder.Append(SaleRemark4Finally);
			}
			stringBuilder.Append(LeftMargin + "*" + strPSalesOrderNo + "*" + NewLine);
			string printModifyPartTypeStr = getPrintModifyPartTypeStr(strPSalesOrderNo, strPBranchCode, strPBranchDesc, dtSoldGoodsList, IsPrintType);
			if (printModifyPartTypeStr.Trim() != string.Empty)
			{
				int l = 0;
				for (int modifyTypePrintCount = RetailSession.CurRetailSession.ModifyTypePrintCount; l < modifyTypePrintCount; l++)
				{
					stringBuilder.Append(LeftMargin + NewLine);
					stringBuilder.Append(printModifyPartTypeStr);
				}
			}
			int num13 = RetailSession.CurRetailSession.PageBottomMargin;
			if (num13 <= 1)
			{
				num13 = 2;
			}
			for (int m = 0; m < num13; m++)
			{
				stringBuilder.AppendLine(Environment.NewLine);
			}
			string text4 = stringBuilder.ToString();
			if (RetailSession.CurRetailSession.CurPrintWay == PrintWay.票据机)
			{
				text4 = text4 + "\u001b" + 'i';
			}
			if ("AHK".Equals(Session.ManAgentClientID, StringComparison.OrdinalIgnoreCase))
			{
				text4 = Strings.StrConv(text4, VbStrConv.TraditionalChinese);
			}
			int n = 0;
			for (int salePrintNum = RetailSession.CurRetailSession.SalePrintNum; n < salePrintNum; n++)
			{
				PrintC(RetailSession.CurRetailSession.PrinterPort, text4);
			}
			stringBuilder = null;
			if (DebugPrinting)
			{
				clsPublic.Logging(LOG_MODE.INFO, text4);
			}
		}

		private static string GetMpInnerId(string interId)
		{
			return interId.Replace("-", "");
		}

		private static string GetMpOutNo(string outNo)
		{
			return outNo.Substring(0, 28);
		}

		public static void PrintMPRefundOrder(string branchCode, string branchName, string saleWorkDate, int payType, decimal refundAmt, string refundNo, string outTradeId, string tradeId)
		{
			switch (RetailSession.CurRetailSession.CurPrintWay)
			{
				case PrintWay.无:
					break;
				case PrintWay.普通打印机:
				case PrintWay.普通打印机_博施usb打印机:
					PrintMPRefundOrderByUSB(branchCode, branchName, saleWorkDate, payType, refundAmt, refundNo, outTradeId, tradeId, RetailSession.CurRetailSession.PrinterName);
					break;
				default:
					PrintMPRefundOrderNormal(branchCode, branchName, saleWorkDate, payType, refundAmt, refundNo, outTradeId, tradeId);
					break;
			}
		}

		public static void PrintMPRefundOrderNormal(string branchCode, string branchName, string saleWorkDate, int payType, decimal refundAmt, string refundNo, string outTradeId, string tradeId)
		{
			StringBuilder stringBuilder = new StringBuilder(4000);
			string empty = string.Empty;
			FillLeftMargin();
			empty = TextToCenter("移动支付退款凭据", PaperWidth);
			stringBuilder.Append(LeftMargin + empty + NewLine);
			stringBuilder.Append(LeftMargin + NewLine);
			stringBuilder.Append($"{LeftMargin}店铺编号:{branchCode}{NewLine}");
			stringBuilder.Append($"{LeftMargin}店铺名称:{branchName}{NewLine}");
			stringBuilder.Append(LeftMargin + "销售时间:" + saleWorkDate + NewLine);
			stringBuilder.Append(LeftMargin + "收 银 员:" + Session.UserDesc + NewLine);
			string invDateFmtStr = RetailSession.CurRetailSession.InvDateFmtStr;
			stringBuilder.Append(LeftMargin + "打印时间:" + YSPublic.Conn.ServerDateTime.ToString(invDateFmtStr) + NewLine);
			stringBuilder.Append(LeftMargin + string.Format("移动支付({0})退款明细", (payType == 1) ? "支付宝" : "微信") + NewLine);
			stringBuilder.Append(LeftMargin + StrSingleHorizon + NewLine);
			stringBuilder.Append(LeftMargin + string.Format("退款金额:{0}", refundAmt.ToString("##,##0.00")) + NewLine);
			stringBuilder.Append(LeftMargin + $"退款单号:{GetMpOutNo(refundNo)}" + NewLine);
			stringBuilder.Append(LeftMargin + $"商户单号:{GetMpOutNo(outTradeId)}" + NewLine);
			stringBuilder.Append(LeftMargin + $"交易单号:{tradeId}" + NewLine);
			stringBuilder.Append(LeftMargin + StrSingleHorizon + NewLine);
			if (SaleRemark1 != string.Empty)
			{
				stringBuilder.Append(SaleRemark1Finally);
			}
			if (SaleRemark2.Trim() != string.Empty)
			{
				stringBuilder.Append(SaleRemark2Finally);
			}
			if (SaleRemark3.Trim() != string.Empty)
			{
				stringBuilder.Append(SaleRemark3Finally);
			}
			if (SaleRemark4.Trim() != string.Empty)
			{
				stringBuilder.Append(SaleRemark4Finally);
			}
			int num = RetailSession.CurRetailSession.PageBottomMargin;
			if (1 >= num)
			{
				num = 2;
			}
			for (int i = 0; i < num; i++)
			{
				stringBuilder.AppendLine(Environment.NewLine);
			}
			string text = stringBuilder.ToString();
			if (RetailSession.CurRetailSession.CurPrintWay == PrintWay.票据机)
			{
				text = text + "\u001b" + 'i';
			}
			if ("AHK".Equals(Session.ManAgentClientID, StringComparison.OrdinalIgnoreCase))
			{
				text = Strings.StrConv(text, VbStrConv.TraditionalChinese);
			}
			int j = 0;
			for (int salePrintNum = RetailSession.CurRetailSession.SalePrintNum; j < salePrintNum; j++)
			{
				PrintC(RetailSession.CurRetailSession.PrinterPort, text);
			}
			stringBuilder = null;
			if (DebugPrinting)
			{
				clsPublic.Logging(LOG_MODE.INFO, text);
			}
		}

		private static void PrintMPRefundOrderByUSB(string branchCode, string branchName, string saleWorkDate, int payType, decimal refundAmt, string refundNo, string outTradeId, string tradeId, string printerName)
		{
			DataTable dataTable = new DataTable();
			dataTable.Columns.Add("PayType", Type.GetType("System.String"));
			dataTable.Columns.Add("BranchCode", Type.GetType("System.String"));
			dataTable.Columns.Add("BranchName", Type.GetType("System.String"));
			dataTable.Columns.Add("WorkDate", Type.GetType("System.String"));
			dataTable.Columns.Add("Creater", Type.GetType("System.String"));
			dataTable.Columns.Add("PrintTime", Type.GetType("System.DateTime"));
			dataTable.Columns.Add("PayAmt", Type.GetType("System.Decimal"));
			dataTable.Columns.Add("RefundNo", Type.GetType("System.String"));
			dataTable.Columns.Add("OutTradeId", Type.GetType("System.String"));
			dataTable.Columns.Add("TradeId", Type.GetType("System.String"));
			dataTable.Columns.Add("Remark", Type.GetType("System.String"));
			DataRow dataRow = dataTable.NewRow();
			dataRow["PayType"] = string.Format("移动支付({0})退款明细", (payType == 2) ? "微信" : "支付宝");
			dataRow["BranchCode"] = branchCode;
			dataRow["BranchName"] = branchName;
			dataRow["WorkDate"] = saleWorkDate;
			dataRow["Creater"] = Session.UserDesc;
			dataRow["PrintTime"] = DateTime.Now;
			dataRow["PayAmt"] = refundAmt;
			dataRow["RefundNo"] = GetMpOutNo(refundNo);
			dataRow["OutTradeId"] = GetMpOutNo(outTradeId);
			dataRow["TradeId"] = tradeId;
			StringBuilder stringBuilder = new StringBuilder(4000);
			if (SaleRemark1 != string.Empty)
			{
				stringBuilder.Append(SaleRemark1Finally);
			}
			if (SaleRemark2.Trim() != string.Empty)
			{
				stringBuilder.Append(SaleRemark2Finally);
			}
			if (SaleRemark3.Trim() != string.Empty)
			{
				stringBuilder.Append(SaleRemark3Finally);
			}
			if (SaleRemark4.Trim() != string.Empty)
			{
				stringBuilder.Append(SaleRemark4Finally);
			}
			dataRow["Remark"] = stringBuilder.ToString();
			dataTable.Rows.Add(dataRow);
			_ = RetailSession.CurRetailSession.CurPrintWay;
			RptMpFefund rptMpFefund = new RptMpFefund();
			rptMpFefund.DataSource = dataTable;
			rptMpFefund.CreateDocument();
			rptMpFefund.PrintingSystem.ShowMarginsWarning = false;
			rptMpFefund.Print("");
		}

		public static void PrintMobilePaySaleOrder(MobilePaySalesPrint printData)
		{
			StringBuilder stringBuilder = new StringBuilder(4000);
			string empty = string.Empty;
			FillLeftMargin();
			List<FormatData> invoiceHeader = Session.GetInvoiceHeader();
			if (invoiceHeader == null || invoiceHeader.Count == 0)
			{
				empty = printData.Titile;
				empty = TextToCenter(empty, PaperWidth);
				stringBuilder.AppendFormat("{0}{1}", empty, NewLine);
				stringBuilder.AppendFormat("{0}{1}", LeftMargin, NewLine);
			}
			else
			{
				foreach (FormatData item in invoiceHeader)
				{
					empty = ((item.Align == 0) ? TextToLeft(item.RowData, PaperWidth) : ((item.Align != 2) ? TextToCenter(item.RowData, PaperWidth) : TextToRight(item.RowData, PaperWidth)));
					stringBuilder.AppendFormat("{0}{1}", empty, NewLine);
					for (int i = 0; i < item.AddBlankRows; i++)
					{
						stringBuilder.AppendFormat("{0}{1}", LeftMargin, NewLine);
					}
				}
			}
			if (!string.IsNullOrEmpty(printData.BranchCode))
			{
				stringBuilder.AppendFormat("{0}店   铺:{1}({2}){3}", LeftMargin, printData.BranchName, printData.BranchCode, NewLine);
			}
			if (!string.IsNullOrEmpty(printData.FreName))
			{
				stringBuilder.AppendFormat("{0}班    次:{1}{2}", LeftMargin, printData.FreName, NewLine);
				if (!string.IsNullOrEmpty(printData.UserName))
				{
					stringBuilder.AppendFormat("{0}收银人员:{1}{2}", LeftMargin, printData.UserName, NewLine);
				}
			}
			else
			{
				stringBuilder.AppendFormat("{0}时    段:{1}~{2}{3}", LeftMargin, printData.FromDate.ToString("yyyy-MM-dd"), printData.ToDate.ToString("yyyy-MM-dd"), NewLine);
			}
			stringBuilder.AppendFormat("{0}打印人员:{1}({2}){3}", LeftMargin, Session.UserDesc, Session.LoginUserId, NewLine);
			stringBuilder.AppendFormat("{0}打印时间:{1}{2}", LeftMargin, DateTime.Now.ToString("yy-MM-dd HH:mm:ss"), NewLine);
			stringBuilder.AppendFormat("{0}{1}{2}", LeftMargin, StrSingleHorizon, NewLine);
			stringBuilder.AppendFormat("{0}{1}{2}{3}{4}{5}", LeftMargin, FormatString("业务", FieldWidth[0] - 4, PadLeft: false, FieldWidth[0] - 4), FormatString("类型", FieldWidth[1] + 4, PadLeft: false, FieldWidth[1] + 4), FormatString("金额", FieldWidth[2] + 4, PadLeft: true, FieldWidth[2] + 4), FormatString("单数", FieldWidth[3] + 2, PadLeft: true, FieldWidth[3] + 2), NewLine);
			foreach (MobilePaySalesGroup detail in printData.Details)
			{
				stringBuilder.AppendFormat("{0}{1}{2}{3}{4}{5}", LeftMargin, FormatString(Others.GetBusuinessType(detail.BusinesssType), FieldWidth[0] - 4, PadLeft: false, FieldWidth[0] - 4), FormatString(MobilePayDic.GetMobilePayName(detail.PayType), FieldWidth[1] + 4, PadLeft: false, FieldWidth[1] + 4), FormatString(detail.Amt.ToString("##,##0.00"), FieldWidth[2] + 4, PadLeft: true, FieldWidth[2] + 4), FormatString(detail.Qty.ToString("##,###"), FieldWidth[3] + 2, PadLeft: true, FieldWidth[3] + 2), NewLine);
			}
			List<MobilePaySalesGroup> source = printData.Details.Where((MobilePaySalesGroup o) => o.BusinesssType == 0).ToList();
			List<MobilePaySalesGroup> source2 = printData.Details.Where((MobilePaySalesGroup o) => o.BusinesssType == 1).ToList();
			stringBuilder.AppendFormat("{0}{1}{2}", LeftMargin, StrSingleHorizon, NewLine);
			if (1 < (from x in printData.Details
					 group x by new
					 {
						 x.PayType
					 }).Count())
			{
				stringBuilder.AppendFormat("{0}{1}{2}{3}{4}", LeftMargin, FormatString("支付小计:", FieldWidth[0] + FieldWidth[1], PadLeft: false, FieldWidth[0] + FieldWidth[1]), FormatString(source.Sum((MobilePaySalesGroup o) => o.Amt).ToString("##,##0.00"), FieldWidth[2] + 4, PadLeft: true, FieldWidth[2] + 4), FormatString(source.Sum((MobilePaySalesGroup o) => o.Qty).ToString("##,###"), FieldWidth[3] + 2, PadLeft: true, FieldWidth[3] + 2), NewLine);
				stringBuilder.AppendFormat("{0}{1}{2}{3}{4}", LeftMargin, FormatString("退款小计:", FieldWidth[0] + FieldWidth[1], PadLeft: false, FieldWidth[0] + FieldWidth[1]), FormatString(source2.Sum((MobilePaySalesGroup o) => o.Amt).ToString("##,##0.00"), FieldWidth[2] + 4, PadLeft: true, FieldWidth[2] + 4), FormatString(source2.Sum((MobilePaySalesGroup o) => o.Qty).ToString("##,###"), FieldWidth[3] + 2, PadLeft: true, FieldWidth[3] + 2), NewLine);
				stringBuilder.AppendFormat("{0}{1}{2}", LeftMargin, StrSingleHorizon, NewLine);
			}
			decimal num = source.Sum((MobilePaySalesGroup o) => o.Amt) + source2.Sum((MobilePaySalesGroup o) => o.Amt);
			int num2 = source.Sum((MobilePaySalesGroup o) => o.Qty) + source2.Sum((MobilePaySalesGroup o) => o.Qty);
			stringBuilder.AppendFormat("{0}{1}{2}{3}{4}", LeftMargin, FormatString("合计:", FieldWidth[0] + FieldWidth[1], PadLeft: false, FieldWidth[0] + FieldWidth[1]), FormatString(num.ToString("##,##0.00"), FieldWidth[2] + 4, PadLeft: true, FieldWidth[2] + 4), FormatString(num2.ToString("##,###"), FieldWidth[3] + 2, PadLeft: true, FieldWidth[3] + 2), NewLine);
			stringBuilder.AppendFormat("{0}{1}", LeftMargin, NewLine);
			stringBuilder.AppendFormat("{0}{1}", LeftMargin, NewLine);
			stringBuilder.AppendFormat("{0}{1}", LeftMargin, NewLine);
			stringBuilder.AppendFormat("{0}        签名:_______________{1}", LeftMargin, NewLine);
			int num3 = RetailSession.CurRetailSession.PageBottomMargin;
			if (1 >= num3)
			{
				num3 = 2;
			}
			for (int j = 0; j < num3; j++)
			{
				stringBuilder.AppendLine(Environment.NewLine);
			}
			string text = stringBuilder.ToString();
			if (RetailSession.CurRetailSession.CurPrintWay == PrintWay.票据机)
			{
				text = text + "\u001b" + 'i';
			}
			if ("AHK".Equals(Session.ManAgentClientID, StringComparison.OrdinalIgnoreCase))
			{
				text = Strings.StrConv(text, VbStrConv.TraditionalChinese);
			}
			int k = 0;
			for (int salePrintNum = RetailSession.CurRetailSession.SalePrintNum; k < salePrintNum; k++)
			{
				PrintC(RetailSession.CurRetailSession.PrinterPort, text);
			}
			stringBuilder = null;
			if (DebugPrinting)
			{
				clsPublic.Logging(LOG_MODE.INFO, text);
			}
		}

		private static string GetPrintDateFmt()
		{
			string invDateFmtStr = RetailSession.CurRetailSession.InvDateFmtStr;
			string[] array = invDateFmtStr.Split(new char[1]
			{
			' '
			}, StringSplitOptions.RemoveEmptyEntries);
			if (array == null || array.Length == 0)
			{
				return "yy-MM-dd HH:mm:ss";
			}
			if (array.Length == 1)
			{
				return $"{array[0]} HH:mm:ss";
			}
			return invDateFmtStr;
		}

		public static string getPrintModifyPartTypeStr(string strPSalesOrderNo, string strPBranchCode, string strPBranchDesc, DataTable dtSoldGoodsList, PrintType IsPrintType)
		{
			string result = string.Empty;
			if (dtSoldGoodsList == null || dtSoldGoodsList.Rows.Count < 1)
			{
				return result;
			}
			if (IsPrintType != 0 && IsPrintType != PrintType.RePrint)
			{
				return result;
			}
			DataRow[] array = dtSoldGoodsList.Select("[GoodsType] in (0,1) and [Barcode] in ('6999999999999','6988888888888','6977777777777')", "GoodsDescription Desc");
			if (array != null && array.Length != 0)
			{
				string arg = string.Empty;
				result = string.Format("{0}{1}{2}", LeftMargin, strPBranchDesc, "凭证单");
				result = TextToCenter(result, PaperWidth);
				result += NewLine;
				if (IsPrintType == PrintType.RePrint)
				{
					arg = TextToCenter("(重印凭证单)", PaperWidth);
				}
				result += $"{LeftMargin}{arg}{NewLine}";
				result += $"{LeftMargin}{strPBranchCode}{NewLine}";
				result += string.Format(arg1: RetailSession.CurRetailSession.WorkDate.ToString(GetPrintDateFmt()), format: "{0}打印日期:{1}{2}", arg0: LeftMargin, arg2: NewLine);
				result += $"{LeftMargin}{StrSingleHorizon}{NewLine}";
				int i = 0;
				for (int num = array.Length; i < num; i++)
				{
					result += string.Format("{0}{1}{2}", LeftMargin, array[i]["GoodsDescription"], NewLine);
				}
				result += $"{LeftMargin}{StrSingleHorizon}{NewLine}";
				result += $"{LeftMargin}*{strPSalesOrderNo}*{NewLine}";
				result += $"{LeftMargin}{NewLine}";
			}
			return result;
		}

		public static void PrintSalesOrderEn(ys_Sale sale, PrintType IsPrintType)
		{
			string invDateFmtStr = RetailSession.CurRetailSession.InvDateFmtStr;
			string itemNo = sale.ItemNo;
			string strPWorkDate = sale.SalesDate.ToString(invDateFmtStr);
			string strPCustomerPayment = sale.PayPrice.ToString("F2");
			string strPChangeAmount = string.Empty;
			string strPVipNo = string.Empty;
			string strPBranchDesc = string.Empty;
			string strPBranchCode = string.Empty;
			string strPAmt = sale.DiscountPrice.Value.ToString("F2");
			List<clsKeyValue<string, decimal>> salePayTypeStr = RetailBase.GetSalePayTypeStr(sale.SaleId);
			if (!sale.IsNoReturnFlag && IsPrintType != PrintType.DePosit)
			{
				decimal payPrice = sale.PayPrice;
				decimal? amtTax = sale.AmtTax;
				strPChangeAmount = Convert.ToDecimal((decimal?)payPrice - amtTax).ToString("F2");
			}
			if (sale.VipId.HasValue && sale.VipId != Guid.Empty)
			{
				clsKeyValue<string, string> shortVipInfo = SaleService.GetShortVipInfo(sale.VipId.Value);
				if (shortVipInfo != null)
				{
					strPVipNo = shortVipInfo.Key;
				}
			}
			clsKeyValue<string, string> branchInfoViaBranchId = RetailService.GetBranchInfoViaBranchId(sale.BranchId);
			if (branchInfoViaBranchId != null)
			{
				strPBranchDesc = branchInfoViaBranchId.Value;
				strPBranchCode = branchInfoViaBranchId.Key;
			}
			DataTable retailSaleData = RetailBase.GetRetailSaleData(clsPublic.GetObjStrUpperTrim(sale.SaleId));
			PrintSalesOrderEn2(sale, retailSaleData, itemNo, strPWorkDate, strPAmt, strPCustomerPayment, strPChangeAmount, strPVipNo, strPBranchDesc, strPBranchCode, salePayTypeStr, null, IsPrintType);
		}

		private static string CutFixLenStrEnFinally(string str, int Len, string preFix)
		{
			if (str == null || string.Empty.Equals(str, StringComparison.OrdinalIgnoreCase))
			{
				return string.Empty;
			}
			string[] array = str.Split(new string[1]
			{
			"<n>"
			}, StringSplitOptions.RemoveEmptyEntries);
			if (array == null)
			{
				return CutFixLenStrEn(str, Len, preFix);
			}
			string text = string.Empty;
			int num = array.Length;
			for (int i = 0; i < num; i++)
			{
				text += CutFixLenStrEn(array[i], Len, preFix);
			}
			return text;
		}

		private static string CutFixLenStrEn(string str, int Len, string preFix)
		{
			List<string> list = clsPublic.CutFixxedLenStrEn(str, Len);
			string text = string.Empty;
			if (list != null)
			{
				foreach (string item in list)
				{
					text += preFix;
					text += item;
					text += Environment.NewLine;
				}
				return text;
			}
			return text;
		}

		public static void PrintSalesOrderEn2(ys_Sale sale, DataTable dtSoldGoodsList, string strPSalesOrderNo, string strPWorkDate, string strPAmt, string strPCustomerPayment, string strPChangeAmount, string strPVipNo, string strPBranchDesc, string strPBranchCode, List<clsKeyValue<string, decimal>> lstPayType, YS_ESALE eSale, PrintType IsPrintType)
		{
			DataRow[] array = dtSoldGoodsList.Select("[GoodsType] in (0,1)", "GoodsDescription Desc");
			DataRow[] array2 = dtSoldGoodsList.Select("[GoodsType] =2", "GoodsDescription Desc");
			FillLeftMargin();
			StringBuilder stringBuilder = new StringBuilder(4000);
			List<FormatData> invoiceHeader = Session.GetInvoiceHeader();
			string empty = string.Empty;
			bool flag = invoiceHeader == null || invoiceHeader.Count == 0;
			if (flag)
			{
				empty = RetailSession.CurRetailSession.ShowBranchName;
				empty = TextToCenter(empty, PaperWidth);
				stringBuilder.Append(empty + NewLine);
				stringBuilder.Append(LeftMargin + NewLine);
			}
			else
			{
				foreach (FormatData item in invoiceHeader)
				{
					if (!item.RowData.StartsWith("<merge>"))
					{
						empty = ((item.Align == 0) ? TextToLeft(item.RowData, PaperWidth) : ((item.Align != 2) ? TextToCenter(item.RowData, PaperWidth) : TextToRight(item.RowData, PaperWidth)));
						stringBuilder.Append(empty + NewLine);
						for (int i = 0; i < item.AddBlankRows; i++)
						{
							stringBuilder.Append(LeftMargin + NewLine);
						}
					}
				}
			}
			bool flag2 = false;
			switch (IsPrintType)
			{
				case PrintType.RePrint:
					empty = TextToCenter("(REPRINT)", PaperWidth);
					stringBuilder.Append(LeftMargin + empty + NewLine);
					flag2 = true;
					break;
				case PrintType.Void:
					empty = TextToCenter("(OFFSET)", PaperWidth);
					stringBuilder.Append(LeftMargin + empty + NewLine);
					break;
				case PrintType.RePrintVoid:
					empty = TextToCenter("(OFFSET REPRINT)", PaperWidth);
					stringBuilder.Append(LeftMargin + empty + NewLine);
					flag2 = true;
					break;
				case PrintType.RepairSeles:
					empty = TextToCenter("(MAKEUP)", PaperWidth);
					stringBuilder.Append(LeftMargin + empty + NewLine);
					break;
				case PrintType.DePosit:
					empty = TextToCenter("(DEPOSIT)", PaperWidth);
					stringBuilder.Append(LeftMargin + empty + NewLine);
					break;
			}
			stringBuilder.Append(LeftMargin + NewLine);
			stringBuilder.Append($"{LeftMargin}{strPBranchCode}/{strPSalesOrderNo}{NewLine}");
			if (!flag)
			{
				foreach (FormatData item2 in invoiceHeader)
				{
					if (item2.RowData.StartsWith("<merge>"))
					{
						stringBuilder.Append(CutFixLenStrEnFinally(item2.RowData.Replace("<merge>", ""), 34, LeftMargin));
					}
				}
			}
			if (IsPrintType == PrintType.RepairSeles)
			{
				DateTime workDate = RetailSession.CurRetailSession.WorkDate;
				string text = "yyyy-MM-dd HH:mm:ss";
				try
				{
					text = RetailSession.CurRetailSession.InvDateFmtStr;
				}
				catch
				{
					text = "yyyy-MM-dd HH:mm:ss";
				}
				stringBuilder.Append(LeftMargin + "DATE: " + strPWorkDate + NewLine);
				if (flag2)
				{
					stringBuilder.Append(LeftMargin + "PRINT DATE: " + workDate.ToString(text) + NewLine);
				}
			}
			else
			{
				stringBuilder.Append(LeftMargin + "DATE: " + strPWorkDate + NewLine);
			}
			if (strPVipNo.Trim() != string.Empty)
			{
				stringBuilder.Append(LeftMargin + "VIP CARD：" + clsPublic.GetObjectString(strPVipNo) + NewLine);
			}
			if (eSale != null)
			{
				stringBuilder.Append(LeftMargin + "预约单号:" + eSale.ItemNo + NewLine);
			}
			string text2 = MakePayTypeString(lstPayType, IsEn: true);
			if (!string.IsNullOrEmpty(text2))
			{
				stringBuilder.Append(text2 + NewLine);
			}
			stringBuilder.Append(LeftMargin + "====================================" + NewLine);
			stringBuilder.Append(LeftMargin + "PRICE  DISCOUNT   RP   QTY  SUBTOTAL" + NewLine);
			stringBuilder.Append(LeftMargin + "------------------------------------" + NewLine);
			int num = 0;
			decimal num2 = default(decimal);
			string text3 = "";
			decimal num3 = 0.0m;
			if (array != null && array.Length != 0)
			{
				int j = 0;
				for (int num4 = array.Length; j < num4; j++)
				{
					DataRow dataRow = array[j];
					string empty2 = string.Empty;
					empty2 = ((!StyleNoRenameSetting.IsStyleNeedRename(clsPublic.GetObjStrUpperTrim(dataRow["GIDB"]))) ? clsPublic.GetObjectString(dataRow["GoodsDescription"]) : StyleNoRenameSetting.StyleRenameTo);
					int num5 = 0;
					stringBuilder.Append(LeftMargin + "   " + empty2 + NewLine);
					num5 = clsPublic.GetIntValue(dataRow["QtyI"]);
					decimal num6 = 0.0m;
					num6 = ((!__printDiscountBefTax) ? clsPublic.ToDecimal(dataRow["Amt"]) : clsPublic.ToDecimal(dataRow["AmtA"]));
					num3 += num6;
					decimal num7 = clsPublic.ToDecimal(dataRow["UnitPrice"]);
					decimal num8 = 0.0m;
					num8 = (__printDiscountBefTax ? (clsPublic.ToDecimal(dataRow["AmtA"]) / (decimal)num5) : clsPublic.ToDecimal(dataRow["UnitPriceA"]));
					decimal d = (num7 == 0m) ? 0m : ((num7 - num8) / num7);
					text3 = "     ";
					if (d != 0m)
					{
						text3 = FormatString((d * 100m).ToString("F0").Trim() + "%", 5, PadLeft: true);
					}
					string text4 = FormatString(dataRow["UnitPrice"].ToString().Trim(), 4, PadLeft: true);
					string text5 = (num6 / (decimal)num5).ToString("F2");
					text4 = (string.IsNullOrEmpty(text4) ? "0" : text4);
					text4 = FormatString(text4, 8, PadLeft: false);
					text3 = FormatString(text3, 7, PadLeft: false);
					text5 = FormatString(text5.Trim(), 7, PadLeft: false);
					string text6 = FormatString(num5.ToString().Trim(), 5, PadLeft: true);
					string text7 = FormatString(num6.ToString("F2"), 9, PadLeft: true);
					stringBuilder.Append(LeftMargin + text4 + text3 + text5 + text6 + text7 + NewLine);
					stringBuilder.Append(LeftMargin + "------------------------------------" + NewLine);
					num += num5;
					num2 += num6;
				}
			}
			if (array2 != null && array2.Length != 0)
			{
				stringBuilder.Append(NewLine);
				int k = 0;
				for (int num9 = array2.Length; k < num9; k++)
				{
					DataRow dataRow2 = array2[k];
					string empty3 = string.Empty;
					empty3 = ((!StyleNoRenameSetting.IsStyleNeedRename(clsPublic.GetObjStrUpperTrim(dataRow2["GIDB"]))) ? dataRow2["GoodsDescription"].ToString() : StyleNoRenameSetting.StyleRenameTo);
					if (dataRow2["GoodsDescription"].ToString().IndexOf("现金券") > -1)
					{
						string text8 = PubClass.GenerateDescNo("CASHMAXDESCNO", "", 3);
						stringBuilder.Append(LeftMargin + "   (" + text8 + ")" + dataRow2["GoodsDescription"].ToString() + NewLine);
					}
					else
					{
						stringBuilder.Append(LeftMargin + "   " + empty3 + NewLine);
					}
					int intValue = clsPublic.GetIntValue(dataRow2["QtyI"]);
					decimal num10 = 0.0m;
					num10 = ((!__printDiscountBefTax) ? clsPublic.ToDecimal(dataRow2["Amt"]) : clsPublic.ToDecimal(dataRow2["AmtA"]));
					num3 += num10;
					string text9 = (num10 / (decimal)intValue).ToString("F2");
					string text10 = FormatString("", 8, PadLeft: false);
					text3 = FormatString(text3, 7, PadLeft: false);
					text9 = FormatString(text9.Trim(), 7, PadLeft: false);
					string text11 = FormatString(intValue.ToString().Trim(), 5, PadLeft: true);
					string text12 = FormatString(num10.ToString("F2"), 9, PadLeft: true);
					stringBuilder.Append(LeftMargin + text10 + text3 + text9 + text11 + text12 + NewLine);
					stringBuilder.Append(LeftMargin + "------------------------------------" + NewLine);
					num += intValue;
					num2 += num10;
				}
			}
			string text13 = (RetailSession.CurRetailSession.Rate * 100m).ToString("F0") + "%";
			string strPString = (sale.AmtTax - sale.DiscountPrice).Value.ToString("F2");
			decimal d2 = num3;
			decimal? amtTax = sale.AmtTax;
			bool flag3 = d2 == amtTax.GetValueOrDefault() && amtTax.HasValue;
			string text14 = sale.AmtTax.Value.ToString("F2");
			stringBuilder.Append(LeftMargin + NewLine);
			stringBuilder.Append(LeftMargin + (__printDiscountBefTax ? "    " : "NET ") + "TOTAL:" + FormatString(num.ToString().Trim(), 17, PadLeft: true) + FormatString(num3.ToString("F2"), 9, PadLeft: true) + NewLine);
			if (RetailSession.CurRetailSession.Rate != 0m)
			{
				stringBuilder.Append(LeftMargin + (__printDiscountBefTax ? "    " : "INC.") + text13 + " GST" + (flag3 ? " incl" : "") + ":" + FormatString(strPString, flag3 ? 20 : 25, PadLeft: true) + NewLine);
			}
			stringBuilder.Append(LeftMargin + "====================================" + NewLine);
			stringBuilder.Append(LeftMargin + "TOTAL:" + FormatString(text14.Trim(), 30, PadLeft: true) + NewLine);
			stringBuilder.Append(LeftMargin + "CASH:" + FormatString(strPCustomerPayment, 31, PadLeft: true) + NewLine);
			stringBuilder.Append(LeftMargin + "CHANGE :" + FormatString((strPChangeAmount.Trim() == string.Empty) ? "0.00" : strPChangeAmount, 28, PadLeft: true) + NewLine);
			stringBuilder.Append(LeftMargin + NewLine);
			if (SaleRemark1 != string.Empty)
			{
				if (!SaleRemark1IsEn)
				{
					stringBuilder.Append(PrintHelp.ConvertPrintRemarkFinally(SaleRemark1, PaperWidth / 2, LeftMargin));
				}
				else
				{
					stringBuilder.Append(CutFixLenStrEnFinally(SaleRemark1, 34, LeftMargin));
				}
			}
			if (SaleRemark2 != string.Empty)
			{
				if (!SaleRemark2IsEn)
				{
					stringBuilder.Append(PrintHelp.ConvertPrintRemarkFinally(SaleRemark2, PaperWidth / 2, LeftMargin));
				}
				else
				{
					stringBuilder.Append(CutFixLenStrEnFinally(SaleRemark2, 34, LeftMargin));
				}
			}
			if (SaleRemark3 != string.Empty)
			{
				if (!SaleRemark3IsEn)
				{
					stringBuilder.Append(PrintHelp.ConvertPrintRemarkFinally(SaleRemark3, PaperWidth / 2, LeftMargin));
				}
				else
				{
					stringBuilder.Append(CutFixLenStrEnFinally(SaleRemark3, 34, LeftMargin));
				}
			}
			if (SaleRemark4 != string.Empty)
			{
				if (!SaleRemark2IsEn)
				{
					stringBuilder.Append(PrintHelp.ConvertPrintRemarkFinally(SaleRemark4, PaperWidth / 2, LeftMargin));
				}
				else
				{
					stringBuilder.Append(CutFixLenStrEnFinally(SaleRemark4, 34, LeftMargin));
				}
			}
			stringBuilder.Append(LeftMargin + "*" + strPSalesOrderNo + "*" + NewLine);
			string printModifyPartTypeStrEn = getPrintModifyPartTypeStrEn(strPSalesOrderNo, strPBranchCode, strPBranchDesc, dtSoldGoodsList, IsPrintType);
			if (printModifyPartTypeStrEn.Trim() != string.Empty)
			{
				int l = 0;
				for (int modifyTypePrintCount = RetailSession.CurRetailSession.ModifyTypePrintCount; l < modifyTypePrintCount; l++)
				{
					stringBuilder.Append(LeftMargin + NewLine);
					stringBuilder.Append(printModifyPartTypeStrEn);
				}
			}
			int num11 = RetailSession.CurRetailSession.PageBottomMargin;
			if (num11 <= 1)
			{
				num11 = 2;
			}
			for (int m = 0; m < num11; m++)
			{
				stringBuilder.AppendLine("\r\n");
			}
			string text15 = stringBuilder.ToString();
			if (RetailSession.CurRetailSession.CurPrintWay == PrintWay.票据机)
			{
				text15 = text15 + "\u001b" + 'i';
			}
			int n = 0;
			for (int salePrintNum = RetailSession.CurRetailSession.SalePrintNum; n < salePrintNum; n++)
			{
				PrintC(RetailSession.CurRetailSession.PrinterPort, text15);
			}
			stringBuilder.Clear();
			stringBuilder = null;
		}

		public static string getPrintModifyPartTypeStrEn(string strPSalesOrderNo, string strPBranchCode, string strPBranchDesc, DataTable dtSoldGoodsList, PrintType IsPrintType)
		{
			string result = string.Empty;
			if (dtSoldGoodsList == null || dtSoldGoodsList.Rows.Count < 1)
			{
				return result;
			}
			if (IsPrintType != 0 && IsPrintType != PrintType.RePrint)
			{
				return result;
			}
			DataRow[] array = dtSoldGoodsList.Select("[GoodsType] in (0,1) and [Barcode] in ('6999999999999','6988888888888','6977777777777')", "GoodsDescription Desc");
			if (array != null && array.Length != 0)
			{
				result = string.Format("{0}{1}", strPBranchDesc, "CERTIFICATE");
				result = $"{LeftMargin}{TextToCenter(result, PaperWidth)}{NewLine}";
				string empty = string.Empty;
				if (IsPrintType == PrintType.RePrint)
				{
					empty = "(REPRINT)";
					empty = TextToCenter(empty, PaperWidth);
					result = result + LeftMargin + empty + NewLine;
				}
				result = result + LeftMargin + NewLine;
				result += $"{LeftMargin}{strPBranchCode}{NewLine}";
				result += string.Format(arg1: RetailSession.CurRetailSession.WorkDate.ToString(GetPrintDateFmt()), format: "{0}PRINT DATE:{1}{2} ", arg0: LeftMargin, arg2: NewLine);
				result = result + "====================================" + NewLine;
				int i = 0;
				for (int num = array.Length; i < num; i++)
				{
					result += string.Format("{0}{1}{2}", LeftMargin, array[i]["GoodsDescription"].ToString().Trim(), NewLine);
				}
				result += string.Format("{0}{1}", "====================================", NewLine);
				result = result + LeftMargin + NewLine;
				result += $"{LeftMargin}*{strPSalesOrderNo}*{NewLine}";
				result += $"{LeftMargin}{NewLine}{NewLine}{NewLine}";
			}
			return result;
		}

		private static void PrintC(string strPPort, string strPrint)
		{
			switch (RetailSession.CurRetailSession.CurPrintWay)
			{
				case PrintWay.普通打印机:
					PrintUSB(strPrint, RetailSession.CurRetailSession.PrinterName);
					break;
				case PrintWay.票据机:
					NonPrint(strPPort, strPrint);
					break;
				case PrintWay.普通打印机_博施usb打印机:
					DevPrintForBoshi(strPrint, RetailSession.CurRetailSession.PrinterName);
					break;
			}
		}

		private static void PrintUSB(string strPrintMsg, string strPrintName)
		{
			int num = RetailSession.CurRetailSession.PrintFontSize;
			if (num < 8)
			{
				num = 8;
			}
			else if (num > 12)
			{
				num = 12;
			}
			SimplePrintStruct simplePrintStruct = new SimplePrintStruct(strPrintMsg, 0, 0, "宋体", num, Brushes.Black);
			simplePrintStruct.Bold = false;
			clsSimplePrint clsSimplePrint = null;
			try
			{
				clsSimplePrint = new clsSimplePrint();
				clsSimplePrint.AddPrintContent(simplePrintStruct);
				clsSimplePrint.PrinterName = strPrintName;
				clsSimplePrint.Print();
			}
			catch
			{
			}
			finally
			{
				clsSimplePrint?.Dispose();
			}
		}

		private static void DevPrintForOther(string strPrintMsg, string strPrintName)
		{
			int num = RetailSession.CurRetailSession.PrintFontSize;
			if (num < 8)
			{
				num = 8;
			}
			else if (num > 12)
			{
				num = 12;
			}
			RptSal rptSal = null;
			try
			{
				rptSal = new RptSal(strPrintMsg, num);
				rptSal.Print(strPrintName);
			}
			catch
			{
			}
			finally
			{
				rptSal?.Dispose();
			}
		}

		private static void DevPrintForBoshi(string strPrintMsg, string strPrintName)
		{
			try
			{
				rptSale_boshi rptSale_boshi = new rptSale_boshi();
				DataSet dataSet = new DataSet();
				DataTable dataTable = new DataTable();
				dataTable.Columns.Add("Text", typeof(string));
				dataTable.Rows.Add(strPrintMsg);
				dataSet.Tables.Add(dataTable);
				rptSale_boshi.SetDataSource(dataSet);
				rptSale_boshi.PrintOptions.PrinterName = strPrintName;
				rptSale_boshi.PrintToPrinter(1, collated: false, 1, 999);
				rptSale_boshi.Close();
				rptSale_boshi.Dispose();
				rptSale_boshi = null;
			}
			catch
			{
			}
		}

		private static void PrintDocument(string strPrintMsg, string strPrintName)
		{
			_PrintMsg = strPrintMsg;
			PrintDocument printDocument = null;
			try
			{
				printDocument = new PrintDocument();
				printDocument.PrintController = new StandardPrintController();
				printDocument.PrinterSettings.PrinterName = strPrintName;
				printDocument.PrintPage += pdPrint_PrintPage;
				printDocument.Print();
			}
			finally
			{
				printDocument?.Dispose();
			}
		}

		private static void pdPrint_PrintPage(object sender, PrintPageEventArgs e)
		{
			int num = RetailSession.CurRetailSession.PrintFontSize;
			if (num < 8)
			{
				num = 8;
			}
			else if (num > 12)
			{
				num = 12;
			}
			Font font = new Font("宋体", num, FontStyle.Regular, GraphicsUnit.Point);
			e.Graphics.DrawString(_PrintMsg, font, Brushes.Black, 0f, 0f);
			e.HasMorePages = false;
		}

		private static void NonPrint(string strPPort, string strPrint)
		{
			FileStream fileStream = null;
			try
			{
				int num = CreateFile(strPPort, 1073741824u, 0, 0, 3, 0, 0);
				fileStream = new FileStream(new SafeFileHandle(new IntPtr(num), ownsHandle: true), FileAccess.Write);
				StreamWriter streamWriter = new StreamWriter(fileStream, Encoding.Default);
				streamWriter.AutoFlush = false;
				streamWriter.WriteLine(strPrint);
				streamWriter.Flush();
				streamWriter.Close();
				fileStream.Close();
				CloseHandle(num);
			}
			catch
			{
			}
			finally
			{
				fileStream?.Dispose();
			}
		}

		public static void OpenBox()
		{
			if (RetailSession.CurRetailSession.CashDrawerOpenWay == 0)
			{
				return;
			}
			try
			{
				if (RetailSession.CurRetailSession.CashDrawerOpenWay == 6)
				{
					EpsonUsbPrintOpenBox();
				}
				else if (RetailSession.CurRetailSession.CashDrawerOpenWay == 7)
				{
					BoshiUsbPrintOpenBox();
				}
				else if (RetailSession.CurRetailSession.OpenCashBoxFileName == "")
				{
					int num = CreateFile(RetailSession.CurRetailSession.PrinterPort, 1073741824u, 0, 0, 3, 0, 0);
					FileStream fileStream = new FileStream(new SafeFileHandle(new IntPtr(num), ownsHandle: true), FileAccess.Write);
					StreamWriter streamWriter = new StreamWriter(fileStream);
					try
					{
						streamWriter.AutoFlush = false;
						if (RetailSession.CurRetailSession.CashDrawerOpenWay == 1)
						{
							streamWriter.WriteLine("\u001bp\0<");
						}
						else if (RetailSession.CurRetailSession.CashDrawerOpenWay == 3)
						{
							streamWriter.WriteLine("\u001bp\a");
						}
						else if (RetailSession.CurRetailSession.CashDrawerOpenWay == 2)
						{
							streamWriter.WriteLine(20 + NewLine);
						}
						streamWriter.Flush();
					}
					catch (IOException)
					{
					}
					finally
					{
						streamWriter.Close();
						fileStream.Close();
						CloseHandle(num);
					}
				}
				else
				{
					try
					{
						Process process = new Process();
						process.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
						process.StartInfo.FileName = RetailSession.CurRetailSession.OpenCashBoxFileName;
						process.StartInfo.Arguments = RetailSession.CurRetailSession.OpenCashBoxArguments;
						process.Start();
						process.WaitForExit();
					}
					catch
					{
					}
				}
			}
			catch
			{
			}
		}

		public static void EpsonUsbPrintOpenBox()
		{
			StatusAPI statusAPI = null;
			try
			{
				statusAPI = new StatusAPI();
				if (statusAPI.OpenMonPrinter(OpenType.TYPE_PRINTER, RetailSession.CurRetailSession.PrinterName) == ErrorCode.SUCCESS)
				{
					statusAPI.OpenDrawer(Drawer.EPS_BI_DRAWER_1, Pulse.EPS_BI_PULSE_100);
					statusAPI.CloseMonPrinter();
				}
			}
			catch
			{
			}
			finally
			{
				if (statusAPI != null)
				{
					statusAPI = null;
				}
			}
		}

		public static void BoshiUsbPrintOpenBox()
		{
		}

		public static void DisPlay(short PortType, string Msg1, string Msg2)
		{
			try
			{
				lock (_lockObj)
				{
					PosDisplay.Display(Convert.ToInt16(RetailSession.CurRetailSession.CustomerMsgWay), Convert.ToInt16(RetailSession.CurRetailSession.CustomerMsgPort), PortType, Msg1, Msg2);
				}
			}
			catch (Exception ex)
			{
				clsPublic.Logging(LOG_MODE.ERROR, ex.Message + ex.Source + ex.StackTrace);
			}
		}
	}

}
