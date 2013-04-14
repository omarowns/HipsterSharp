using System;
using Gtk;

namespace HipsterSharp
{
	public class HFile
	{
		private String fileName = null;
		private String fileCont = null;
		private int		lineNumber = 1;
		private Boolean change = false;
		private Boolean newfile = false;
		
		public HFile ()
		{
			this.fileName = "NewFile.txt*";
			this.fileCont = "";
			this.lineNumber = 1;
			//New file means needs to save
			this.change = true;
			this.newfile = true;
		}
		
		public void newFile (Gtk.Label filename, Gtk.HPaned pane)
		{
			Gtk.TextView ln = (Gtk.TextView)pane.Child1;
			Gtk.TextView cn = (Gtk.TextView)((Gtk.ScrolledWindow)pane.Child2).Child;

			filename.Text = "Untitled.txt*";
			this.lineNumber = 1;
			//New file means needs to save
			this.change = true;
			this.newfile = true;

			cn.Buffer.Text = "";
			ln.Buffer.Text = this.lineNumber + "\n";
		}
		
		public void openFile (Gtk.Label fn, Gtk.HPaned pane)
		{
			Gtk.FileChooserDialog fc = new Gtk.FileChooserDialog ("Open a new file", null,
			                                                      Gtk.FileChooserAction.Open,
			                                                      "Open", Gtk.ResponseType.Accept, "Cancel", Gtk.ResponseType.Cancel);
			Gtk.TextView ln = (Gtk.TextView)pane.Child1;
			Gtk.TextView cn = (Gtk.TextView)((Gtk.ScrolledWindow)pane.Child2).Child;

			if (fc.Run () == (int)ResponseType.Accept) {
				//Set up the filename
				String filename = fc.Filename;
				this.fileName = filename;
				fn.Text = filename;
				
				//Open the file in READ mode
				System.IO.FileStream file = System.IO.File.OpenRead (this.fileName);
				//Get the StreamReader for the content
				System.IO.StreamReader r = new System.IO.StreamReader (file);
				r.BaseStream.Seek (0, System.IO.SeekOrigin.Begin);
				Read(r,ln);
				
				//Append to the window component
				cn.Buffer.Text = this.fileCont;
				file.Close ();
				
				//The file hasnt changed
				this.change = false;
				
				//This isn't a new file, is an existing one
				this.newfile = false;
			}
			fc.Destroy ();
		}

		private void Read (System.IO.StreamReader r, Gtk.TextView lineNumber)
		{
			int lineCount = 1;
			this.fileCont = "";
			lineNumber.Buffer.Text = "";
			while (r.Peek() > -1) {
				this.fileCont += r.ReadLine ();
				this.fileCont += "\n";
				lineNumber.Buffer.Text += lineCount++ + "\n";
			}
			r.Close ();
		}

		public void save (Gtk.TextView editorTextArea)
		{
			System.IO.FileStream fs = new System.IO.FileStream (this.fileName, System.IO.FileMode.Truncate, System.IO.FileAccess.Write);
			System.IO.StreamWriter w = new System.IO.StreamWriter (fs);
			w.Write (editorTextArea.Buffer.Text);
			w.Close ();
			fs.Close ();
			this.change = false;
			this.newfile = false;
		}
		
		public void saveas (Gtk.TextView editorTextArea, Gtk.Label filelabel)
		{
			Gtk.FileChooserDialog fc = new Gtk.FileChooserDialog ("Save as..", null, Gtk.FileChooserAction.Save,
			                                                      "Save", Gtk.ResponseType.Accept,
			                                                      "Cancel", Gtk.ResponseType.Cancel);
			if (fc.Run () == (int)Gtk.ResponseType.Accept) {
				//Set the new filename
				String filename = fc.Filename;
				this.fileName = filename;
				filelabel.Text = filename;
				
				//Save the contents to the file
				System.IO.FileStream fs = new System.IO.FileStream (this.fileName, System.IO.FileMode.OpenOrCreate, System.IO.FileAccess.Write);
				System.IO.StreamWriter w = new System.IO.StreamWriter (fs);
				w.Write (editorTextArea.Buffer.Text);
				w.Close ();
				fs.Close ();
				
				//Presents no change nor is a new file
				this.change = false;
				this.newfile = false;
			}
			fc.Destroy ();
		}
		
		public Boolean hasChanged ()
		{
			return this.change;
		}
		
		public void setChange (Boolean status)
		{
			this.change = status;
		}
		
		public Boolean isNewFile ()
		{
			return this.newfile;
		}
		
		public void setNewFile (Boolean status)
		{
			this.newfile = status;
		}
		
		public String getContent ()
		{
			return this.fileCont;
		}
		
	}
}

