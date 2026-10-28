using System;
using System.IO;
using System.Threading;
using System.Windows.Forms;
using FlowDesk;
class StartupTest {
 static void Command(Form form,int command){Native.PostMessage(form.Handle,0x112,new IntPtr(command),IntPtr.Zero);Application.DoEvents();if(!form.IsDisposed&&(Native.GetLong(form.Handle,-16).ToInt64()&0x00c40000)!=0)throw new Exception("Classic frame reappeared after system command");}
 [STAThread] static int Main(){
  Application.EnableVisualStyles();string previous=Store.Root;Store.Root=Path.Combine(AppDomain.CurrentDomain.BaseDirectory,"startup-test-settings");
  try{using(FlowDesk.Program.Activation=new EventWaitHandle(false,EventResetMode.AutoReset)){
   var context=new FlowDesk.AppContext();FlowDesk.Program.Context=context;context.Session=new Session();
   for(int i=0;i<10;i++)context.Session.Workspaces.Add(new WorkspaceModel{Name="Saved "+i,Saved=true});
   context.Session.LastWorkspaceId=context.Session.Workspaces[6].Id;
   context.Start();Application.DoEvents();
   try{
    if(context.Forms.Count!=1||context.Forms[0].Model.Name!="Saved 6")throw new Exception("Startup opened extra or wrong windows");
    var form=context.Forms[0];if(!form.Enabled)throw new Exception("Startup form disabled");
    if((Native.GetLong(form.Handle,-16).ToInt64()&0x00c40000)!=0)throw new Exception("Native caption or frame still attached");
    int refreshed=context.DesktopRefreshCount;
    Native.SetLong(form.Handle,-16,new IntPtr(Native.GetLong(form.Handle,-16).ToInt64()|0x00c40000));
    Native.NotifyWinEvent(0x8018,form.Handle,0,0);
    var clock=System.Diagnostics.Stopwatch.StartNew();
    while(clock.ElapsedMilliseconds<1000&&(context.DesktopRefreshCount==refreshed||(Native.GetLong(form.Handle,-16).ToInt64()&0x00c40000)!=0)){Application.DoEvents();Thread.Sleep(5);}
    if(context.DesktopRefreshCount==refreshed||(Native.GetLong(form.Handle,-16).ToInt64()&0x00c40000)!=0)throw new Exception("Uncloak event did not repair frame without tab click");
    if(context.Forms.Count!=1)throw new Exception("Desktop notification created extra workspace");
    var original=new System.Drawing.Rectangle(-300,50,1200,800);
    if(Workspace.DragBounds(original,new System.Drawing.Point(30,-20),2,form.MinimumSize).Location!=new System.Drawing.Point(-270,30))throw new Exception("Borderless move failed on negative monitor coordinates");
    var clamped=Workspace.DragBounds(original,new System.Drawing.Point(5000,5000),13,form.MinimumSize);
    if(clamped.Size!=form.MinimumSize||clamped.Right!=original.Right||clamped.Bottom!=original.Bottom)throw new Exception("Borderless resize minimum/anchor failed");
    context.Open(form.Model);if(context.Forms.Count!=1)throw new Exception("Duplicate saved workspace opened");
    Command(form,0xf020);if(form.WindowState!=FormWindowState.Minimized)throw new Exception("System minimize failed");
    Command(form,0xf120);if(form.WindowState!=FormWindowState.Normal)throw new Exception("System restore failed");
    Command(form,0xf030);if(form.WindowState!=FormWindowState.Maximized)throw new Exception("System maximize failed");
    Command(form,0xf120);if(form.WindowState!=FormWindowState.Normal)throw new Exception("Maximize restore failed");
    Command(form,0xf060);if(!form.IsDisposed||context.Forms.Count!=0)throw new Exception("System close failed");
    if(context.Session.Workspaces.Count!=10)throw new Exception("Saved layouts lost");
    var temporary=context.Open(new WorkspaceModel{Name="Temporary launch"});context.Save();
    var persisted=Store.Read<Session>(Path.Combine(Store.Root,"session.xml"));
    if(persisted.Workspaces.Count!=10||System.Linq.Enumerable.Any(persisted.Workspaces,w=>!w.Saved||w.Name=="Temporary launch"))throw new Exception("Temporary workspace persisted");
    Command(temporary,0xf060);if(context.Session.Workspaces.Count!=10)throw new Exception("Closed temporary workspace left in history");
   }finally{context.Shutdown();}
   Console.WriteLine("PASS 10 saved layouts -> 1 startup window; no duplicates; native minimize/restore/maximize/close");
  }}finally{Store.Root=previous;}return 0;
 }
}
