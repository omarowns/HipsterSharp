using System;

namespace HipsterSharp
{
	public class HToken
	{
		public HTokenTypes token_type {get; set;}
		public string lexemme {get; set;}

		public HToken (HTokenTypes tt, string lx)
		{
			this.token_type = tt;
			this.lexemme	= lx;
		}
		public HToken ()
		{
		}
	}
}

