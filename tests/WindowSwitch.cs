using System;
using System.Drawing;
using System.Windows.Forms;
using FlowDesk;
class WindowSwitch {
 [STAThread] static int Main(string[] args){
  if(args.Length==2&&args[0]=="--guard"){FlowDesk.Program.Main(args);return 0;}
  Application.EnableVisualStyles();
  var filled=ManagedWindow.FillScreenEdges(new Rectangle(0,24,1920,1056),new Rectangle(0,0,1920,1080),new Rect{Left=100,Top=100,Right=1100,Bottom=900},new Rect{Left=108,Top=100,Right=1092,Bottom=892});
  if(filled!=new Rectangle(-8,24,1936,1064))throw new Exception("Invisible full-screen frame margins not compensated");
  var interior=new Rectangle(100,100,800,600);
  if(ManagedWindow.FillScreenEdges(interior,new Rectangle(0,0,1920,1080),new Rect{Left=100,Top=100,Right=1100,Bottom=900},new Rect{Left=108,Top=100,Right=1092,Bottom=892})!=interior)throw new Exception("Internal panel divider bounds changed");
  using(var host=new Form{Size=new Size(800,600)})
  using(var target=new Form{Text="Disposable switching regression",Size=new Size(400,300)}){
   var surface=new Panel{Dock=DockStyle.Fill};host.Controls.Add(surface);host.Show();target.Show();Application.DoEvents();
   var candidate=new Candidate{H=target.Handle,Path=Application.ExecutablePath,Title=target.Text};
   var window=new ManagedWindow(candidate,new TabModel{Mode="Aligned"},surface,host);
   try{
    int misplacedShows=0;target.VisibleChanged+=delegate{if(target.Visible){Rect r;Native.GetWindowRect(target.Handle,out r);Rectangle expected=surface.RectangleToScreen(surface.ClientRectangle);if(r.Left!=expected.Left||r.Top!=expected.Top)misplacedShows++;}};
    for(int i=0;i<20;i++){window.Place(surface,false);window.Place(surface,true);Application.DoEvents();}
    if(misplacedShows!=0)throw new Exception("Window was revealed at stale bounds");
    target.MinimumSize=new Size(950,700);window.Place(surface,true);Application.DoEvents();
    int repeatedResizes=0;target.SizeChanged+=delegate{repeatedResizes++;};
    for(int i=0;i<20;i++)window.Place(surface,true);
    if(repeatedResizes!=0)throw new Exception("Minimum-size constraint causes repeated resize");
   }finally{window.Detach();}
   Console.WriteLine("PASS 20 native hide/show cycles at prepared bounds, no repeated constrained resize");
  }
  return 0;
 }
}
