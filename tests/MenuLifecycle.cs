using System;
using System.Drawing;
using System.Windows.Forms;
using FlowDesk;
class TestMenu:ContextMenuStrip {
 public void DispatchItemClick(){OnItemClicked(new ToolStripItemClickedEventArgs(Items[0]));}
}
class MenuLifecycle {
 [STAThread] static int Main(){
  Application.EnableVisualStyles();
  using(var form=new Form{ShowInTaskbar=false,Size=new Size(240,120)}){
   var anchor=new Button{Text="Test menu"};form.Controls.Add(anchor);form.Show();
   for(int i=0;i<20;i++){
    var menu=new TestMenu();menu.Items.Add("Choose",null,delegate{});
    Theme.KeepMenu(anchor,menu);menu.Show(anchor,new Point(0,anchor.Height));
    menu.DispatchItemClick();menu.Close();
    if(menu.IsDisposed)throw new Exception("Menu disposed during click dispatch");
    menu.Show(anchor,new Point(0,anchor.Height));menu.Close();
   }
   var last=(ContextMenuStrip)anchor.Tag;form.Close();
   if(!last.IsDisposed)throw new Exception("Owner did not dispose its menu");
  }
  Console.WriteLine("PASS menu click/close/reopen lifecycle (20 cycles), owner cleanup");return 0;
 }
}
