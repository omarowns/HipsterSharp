using System;
using HipsterSharp;

namespace HipsterSharp
{
	public class Scanner
	{
		public enum State
		{
			START,INASSIGN,INCOMMENT,INNUM,INID,DONE
		}

		public enum Token
		{
			ENDFILE,ERROR,
			/* reserved words */
			IF,THEN,ELSE,END,READ,WRITE,
			/* multicharacter tokens */
			ID,NUM,
			/* special symbols */
			ASSIGN,EQ,LT,PLUS,MINUS,TIMES,OVER,LPAREN,RPAREN,SEMI
		}

		static int linepos = 0;
		static int bufsize = 0; /* current size of buffer string */
		static bool EOF_flag = false; /* corrects ungetNextChar behavior on EOF */

	}

}

