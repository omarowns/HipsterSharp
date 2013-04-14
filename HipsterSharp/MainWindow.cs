using System;
using System.Diagnostics;
using Gtk;
using HipsterSharp;

public partial class MainWindow: Gtk.Window
{
	
	private HFile hfile = null;
	private HSyntaxHighlighter hsh = null;

	public MainWindow (): base (Gtk.WindowType.Toplevel)
	{
		Build ();
		hfile = new HFile ();
		hfile.newFile (fileNameLabel, editorPane);
		insertTagReserved ();
		editorTextArea.GrabFocus();
		hsh = new HSyntaxHighlighter(editorTextArea);
	}
	void insertTagReserved ()
	{
		//Define tags
		TextTag _tagRESERVED	= new TextTag ("reserved");
		TextTag _tagTYPES		= new TextTag ("types");
		TextTag _tagCOMMENT		= new TextTag ("comment");
		//Set tag properties
		_tagRESERVED.ForegroundGdk = new Gdk.Color (0, 0, 255);
		_tagRESERVED.Weight = Pango.Weight.Bold;
		_tagTYPES.ForegroundGdk = new Gdk.Color (0,0,255);
		_tagTYPES.Weight	= Pango.Weight.Bold;
		_tagCOMMENT.ForegroundGdk	= new Gdk.Color(0,255,150);
		_tagCOMMENT.Weight	= Pango.Weight.Normal;
		//Set tags to the buffer tag table
		editorTextArea.Buffer.TagTable.Add (_tagRESERVED);
		editorTextArea.Buffer.TagTable.Add (_tagTYPES);
		editorTextArea.Buffer.TagTable.Add (_tagCOMMENT);
	}

	protected void OnDeleteEvent (object sender, DeleteEventArgs a)
	{
		Application.Quit ();
		a.RetVal = true;
	}

	protected void OnNewActionActivated (object sender, EventArgs e)
	{
		hfile.newFile (fileNameLabel, editorPane);
	}

	protected void OnOpenActionActivated (object sender, EventArgs e)
	{
		hfile.openFile (fileNameLabel,editorPane);
		hsh.optimizedHighlight(Gdk.Key.A);
		setNumberOfLines(editorTextArea);
	}

	protected void OnEditorTextAreaKeyReleaseEvent (object o, KeyReleaseEventArgs args)
	{
		//  && (hfile.getChange() == false)
		// Check if our editor has a diferent text than our object HFile.content
		if (!(hfile.getContent ().Equals (editorTextArea.Buffer.Text, StringComparison.Ordinal)) && (!hfile.hasChanged ())) {
			fileNameLabel.Text = fileNameLabel.Text + "*";
			hfile.setChange (true);
		}
		Gdk.Key keyPressed = args.Event.Key;
		//hsh.highlight(keyPressed);
		hsh.optimizedHighlight(keyPressed);
		setNumberOfLines(editorTextArea);
	}

	void setNumberOfLines (TextView text)
	{
		int lines = text.Buffer.LineCount;
		editorLines.Buffer.Text = "";
		for (int i=1; i<=lines; i++) {
			editorLines.Buffer.Text += i + "\n";
		}
	}

	protected void OnSaveActionActivated (object sender, EventArgs e)
	{
		if (hfile.hasChanged ()) {
			//If file has a change save it
			if (hfile.isNewFile ()) {
				//Call save as... method
				hfile.saveas (editorTextArea, fileNameLabel);
				//Remove the star
				//fileNameLabel.Text.Remove(fileNameLabel.Text.LastIndexOf("*"));
				fileNameLabel.Text = fileNameLabel.Text.Remove (fileNameLabel.Text.Length - 1);
			} else {
				hfile.save (editorTextArea);
				//Remove the star
				//fileNameLabel.Text.Remove(fileNameLabel.Text.LastIndexOf("*"));
				fileNameLabel.Text = fileNameLabel.Text.Remove (fileNameLabel.Text.Length - 1);
			}
		} else {
			//If it hasn't a change, well, what for?
		}
	}

	protected void OnSaveAsActionActivated (object sender, EventArgs e)
	{
		hfile.saveas (editorTextArea, fileNameLabel);
	}

	protected void OnQuitActionActivated (object sender, EventArgs e)
	{
		Application.Quit ();
	}

	protected void OnCompileActivated (object sender, EventArgs e)
	{
		OnSaveActionActivated(null,null);
		Process lexeme_analyzer = new Process();
		lexeme_analyzer.StartInfo.FileName = @"AnalizadorLexico.exe";
		lexeme_analyzer.StartInfo.Arguments = "\"" + fileNameLabel.Text + "\"";
		lexeme_analyzer.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
		lexeme_analyzer.StartInfo.CreateNoWindow = true;
		lexeme_analyzer.StartInfo.UseShellExecute = false;
		lexeme_analyzer.StartInfo.RedirectStandardOutput = true;
		lexeme_analyzer.Start();
		lexeme_analyzer.WaitForExit();
		string cout = lexeme_analyzer.StandardOutput.ReadToEnd();
		lexemeOutput.Buffer.Text = "";
		lexemeOutput.Buffer.Text = cout;
	}
}
