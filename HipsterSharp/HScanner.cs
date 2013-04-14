using Gtk;
using System;
using HipsterSharp;

namespace HipsterSharp
{
	public class HScanner
	{
		private HToken[] reserved_words = new HToken[12];
		int line 	= 0;
		int col		= 0;
		string _char;

		public HScanner (Gtk.TextView text)
		{
			initreservedwords();

		}

		private void initreservedwords()
		{
			reserved_words[0]	= new HToken(HTokenTypes.TKN_IF,"if");
			reserved_words[1]	= new HToken(HTokenTypes.TKN_THEN,"then");
			reserved_words[2]	= new HToken(HTokenTypes.TKN_ELSE,"else");
			reserved_words[3]	= new HToken(HTokenTypes.TKN_FI,"fi");
			reserved_words[4]	= new HToken(HTokenTypes.TKN_DO,"do");
			reserved_words[5]	= new HToken(HTokenTypes.TKN_UNTIL,"until");
			reserved_words[6]	= new HToken(HTokenTypes.TKN_WHILE,"while");
			reserved_words[7]	= new HToken(HTokenTypes.TKN_READ,"read");
			reserved_words[8]	= new HToken(HTokenTypes.TKN_WRITE,"write");
			reserved_words[9] 	= new HToken(HTokenTypes.TKN_FLOAT,"float");
			reserved_words[10]	= new HToken(HTokenTypes.TKN_INT,"int");
			reserved_words[11]	= new HToken(HTokenTypes.TKN_BOOL,"bool");
			reserved_words[12]	= new HToken(HTokenTypes.TKN_PROGRAM,"program");
		}
	}
}
