using System;
using Gtk;

namespace HipsterSharp
{
	public class HSyntaxHighlighter
	{
		Gtk.TextView editorTextArea;
		private string[] reserved = {
			"if","then","else","fi","do","until","while",
			"read","write","program"
		};
		private string[] types = {
			"int","float","bool"
		};
		bool lineComment	= false;
		bool blockComment	= false;
		bool preendBlockComment = false;

		public HSyntaxHighlighter (Gtk.TextView eta)
		{
			this.editorTextArea = eta;
		}

		public void optimizedHighlight (Gdk.Key pressedkey)
		{
			int num_of_lines = editorTextArea.Buffer.LineCount;
			for (int i=0; i<num_of_lines; i++) {
				lineComment = false;
				TextIter startLine	= editorTextArea.Buffer.GetIterAtLine(i);
				TextIter endLine	= getLastLineIter(startLine);
				breakwords(startLine,endLine,i);
			}
			blockComment = false;
		}

		void breakwords (TextIter startLine, TextIter endLine,int currentLine)
		{
			TextIter s = startLine;
			string word = "";
			int iterStart = s.Offset;
			int iterEnd = s.Offset;
			bool commentStart 	= false;

			while (!s.EndsLine()) { //Loop until end of the line
				switch (s.Char) {
				case "(": case ")": case "{": case "}": case "[": case "]":
					if(lineComment || blockComment){break;}
					paintNormalWords(iterEnd,iterEnd+1);
					word = "";
					iterStart = ++iterEnd;//Skip the character
					s = editorTextArea.Buffer.GetIterAtOffset(iterEnd);//Next character
					continue;
				case " ": case "\t":
					if(lineComment || blockComment){break;}
					paintNormalWords(iterEnd,iterEnd+1);
					word = "";
					iterStart = ++iterEnd; //Skip the character
					s = editorTextArea.Buffer.GetIterAtOffset (iterEnd);//Next character
					continue;
				case "/":
					if(lineComment || blockComment){break;}
					if(commentStart){lineComment = true;break;}
					else{
						word = "";
						word += s.Char;
						++iterEnd; //Next character
						s = editorTextArea.Buffer.GetIterAtOffset (iterEnd);//Next character
						commentStart = true;
					}
					continue;
				case "*":
					if(lineComment || blockComment){break;}
					if(commentStart){
						lineComment=false;
						blockComment=true;
						word += s.Char;
						++iterEnd; //Next character
						s = editorTextArea.Buffer.GetIterAtOffset (iterEnd);//Next character
						paintWord(iterStart,iterEnd,word);
						continue;
					} else {
						break;
					}
				default:

					break;
				}
				if(blockComment){
					switch(s.Char){
					case "*":preendBlockComment = true;break;
					case "/":
						if(preendBlockComment){
							preendBlockComment=false;
							word += s.Char;
							++iterEnd; //Next character
							s = editorTextArea.Buffer.GetIterAtOffset (iterEnd);//Next character
							paintWord(iterStart,iterEnd,word);
							blockComment=false;
							word = "";
							continue;
						}else{break;}
					default:
						preendBlockComment = false;
						word += s.Char;
						++iterEnd; //Next character
						s = editorTextArea.Buffer.GetIterAtOffset (iterEnd);//Next character
						paintWord(iterStart,iterEnd,word);
						continue;
					}
				}
				word += s.Char;
				++iterEnd; //Next character
				s = editorTextArea.Buffer.GetIterAtOffset (iterEnd);//Next character
				paintWord(iterStart,iterEnd,word);
			}
		}

		TextIter getLastLineIter (TextIter startLine)
		{
			int offset = startLine.Offset;
			TextIter t = startLine;
			while (!t.EndsLine()) {
				offset++;
				t = editorTextArea.Buffer.GetIterAtOffset(offset);
			}
			return t;
		}

		public void highlight (Gdk.Key pressedkey)
		{
			for (int i = 0; i < editorTextArea.Buffer.LineCount; i++) {
				//Get Iterator at line 'i'
				TextIter s = editorTextArea.Buffer.GetIterAtLine (i);
				//Initialize iterators
				int iterStart = s.Offset;
				int iterEnd = s.Offset;
				string word = ""; //Define word
				
				while (!s.EndsLine ()) {//Loop until end of the line
					if(s.Char.Equals("(") || s.Char.Equals(")") || s.Char.Equals("{") || s.Char.Equals("}") 
					   || s.Char.Equals("[") || s.Char.Equals("]") ){
						word = "";
						iterStart = ++iterEnd; //Skip the character
						s = editorTextArea.Buffer.GetIterAtOffset (iterEnd); //Get next char
						continue;
					}else{
						word += s.Char;//Keep appending to the word
					}
					//word += s.Char;//Keep appending to the word
					if(s.Char.Equals(" ") || s.Char.Equals("\t")){
						word = "";
						iterStart = ++iterEnd; //Skip the character
						s = editorTextArea.Buffer.GetIterAtOffset (iterEnd); //Get next char
						continue;
					}
					if (isReserved (word)) {
						paintReservedWords (iterStart, iterEnd);//Paint if is reserved
						word = ""; //Reset the word
						iterStart = iterEnd + 1;//Set the start iter to the end iter and increment the end iter
					}else if( isType(word) ){
						paintTypedWords (iterStart, iterEnd);//Paint if is reserved
						word = ""; //Reset the word
						iterStart = iterEnd + 1;//Set the start iter to the end iter and increment the end iter
					}
					else{
						paintNormalWords(iterStart,iterEnd);
						if(pressedkey.ToString().Equals("BackSpace")){
							if( !isReserved(word) || !isType(word) ){
								paintNormalWords(iterStart,iterEnd);
							}else{
								if (isReserved (word)) {
									paintReservedWords (iterStart, iterEnd);//Paint if is reserved
									word = ""; //Reset the word
									iterStart = iterEnd + 1;//Set the start iter to the end iter and increment the end iter
								}else if( isType(word) ){
									paintTypedWords (iterStart, iterEnd);//Paint if is reserved
									word = ""; //Reset the word
									iterStart = iterEnd + 1;//Set the start iter to the end iter and increment the end iter
								}
							}
						}else{
							paintNormalWords(iterStart,iterEnd);
							if(s.Char.Equals(" ") || s.Char.Equals("\t")){
								word = "";
								iterStart = ++iterEnd; //Skip the character
								s = editorTextArea.Buffer.GetIterAtOffset (iterEnd); //Get next char
								continue;
							}
						}
					}
					iterEnd++;//Increment the end iterator
					s = editorTextArea.Buffer.GetIterAtOffset (iterEnd); //Get next char
				}
			}
		}

		private void paintWord (int iterStart, int iterEnd, string word)
		{
			paintNormalWords (iterStart, iterEnd);
			if (isReserved (word)) {
				paintReservedWords (iterStart, iterEnd);
			} else if (isType (word)) {
				paintTypedWords (iterStart, iterEnd);
			} else if (lineComment || blockComment) {
				paintCommentWords(iterStart, iterEnd);
			} else {
				paintNormalWords (iterStart, iterEnd);
			}
		}

		private void paintReservedWords (int start, int end)
		{
			TextIter iS = editorTextArea.Buffer.GetIterAtOffset(start);
			TextIter iE = editorTextArea.Buffer.GetIterAtOffset(end);
			editorTextArea.Buffer.ApplyTag("reserved",iS,iE);
		}
		private void paintTypedWords (int start, int end)
		{
			TextIter iS = editorTextArea.Buffer.GetIterAtOffset(start);
			TextIter iE = editorTextArea.Buffer.GetIterAtOffset(end);
			editorTextArea.Buffer.ApplyTag("types",iS,iE);
		}

		private void paintCommentWords (int start, int end)
		{
			TextIter iS = editorTextArea.Buffer.GetIterAtOffset(start);
			TextIter iE = editorTextArea.Buffer.GetIterAtOffset(end);
			editorTextArea.Buffer.ApplyTag("comment",iS,iE);
		}

		private void paintNormalWords (int start, int end)
		{
			TextIter iS = editorTextArea.Buffer.GetIterAtOffset(start);
			TextIter iE = editorTextArea.Buffer.GetIterAtOffset(end);
			editorTextArea.Buffer.RemoveTag("reserved",iS,iE);
			editorTextArea.Buffer.RemoveTag("types",iS,iE);
			editorTextArea.Buffer.RemoveTag("comment",iS,iE);
		}
		private bool isReserved (String word)
		{
			//Check if is reserved
			bool ret = false;
			foreach (string x in reserved) {
				if(word.Equals(x)){
					ret = true;
					break;
				}
			}
			return ret;
		}
		private bool isType (string word)
		{
			//Check if is reserved
			bool ret = false;
			foreach (string x in types) {
				if(word.Equals(x)){
					ret = true;
					break;
				}
			}
			return ret;
		}
	}
	class BlockComment{
		int start;
		int end;
		public BlockComment (int start, int end)
		{
			this.start	= start;
			this.end	= end;
		}
	}
}

