using System;
using System.Drawing;
using System.Windows.Forms;
class NativeFixture {
 [STAThread] static void Main(){Application.EnableVisualStyles();var f=new Form{Text="Flow Desk Native Fixture — disposable editor",Size=new Size(640,480)};var editor=new TextBox{Multiline=true,AcceptsTab=true,ScrollBars=ScrollBars.Both,Dock=DockStyle.Fill,Text="Disposable test document. No user files are opened or saved."};var menu=new MenuStrip();menu.Items.Add("Test modal",null,delegate{MessageBox.Show(f,"Modal remains accessible. No data changed.","Fixture modal",MessageBoxButtons.OK);});menu.Items.Add("File dialog",null,delegate{using(var d=new OpenFileDialog())d.ShowDialog(f);});f.Controls.Add(editor);f.Controls.Add(menu);f.MainMenuStrip=menu;Application.Run(f);}
}
