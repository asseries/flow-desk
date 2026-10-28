using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Threading;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using FlowDesk;
class CaptureScreenshots {
 [DllImport("user32.dll")]static extern bool PrintWindow(IntPtr window,IntPtr dc,uint flags);
 [STAThread]static void Main(string[] args){
  if(args.Length==2&&args[0]=="--guard"){Program.Main(args);return;}
  Application.EnableVisualStyles();
  if(args.Length==2&&args[0]=="--fixture"){
   using(var form=new Form{Text=args[1]+" — native demo app",Size=new Size(600,600),BackColor=Theme.Bg,ForeColor=Theme.Ink}){
    var editor=new TextBox{Multiline=true,Dock=DockStyle.Fill,BorderStyle=BorderStyle.None,BackColor=Color.FromArgb(30,33,41),ForeColor=Theme.Ink,Font=new Font("Segoe UI",12),ReadOnly=false,ScrollBars=ScrollBars.Vertical};
    editor.Text=args[1]=="Notes"?"Project notes\r\n\r\nA focused workspace for everyday work.\r\n\r\n• Keep related windows together\r\n• Switch between real running apps\r\n• Resize panels to fit your workflow\r\n\r\nThis is editable demo content in a separate\r\nWindows process. No personal files are open.":"Today\r\n\r\n☑ Open the workspace\r\n☑ Add two native windows\r\n☑ Adjust the divider\r\n☐ Save a named layout\r\n\r\nTip: detaching keeps the application open.\r\n\r\nSample data only.";
    var padding=new Panel{Dock=DockStyle.Fill,Padding=new Padding(24),BackColor=editor.BackColor};padding.Controls.Add(editor);form.Controls.Add(padding);
    var timeout=new System.Windows.Forms.Timer{Interval=30000};timeout.Tick+=delegate{form.Close();};timeout.Start();Application.Run(form);timeout.Dispose();
   }return;
  }
  string root=args.Length==2&&args[0]=="--output-root"?Path.GetFullPath(args[1]):Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory,".."));Store.Root=Path.Combine(root,"dist","screenshot-profile");Program.Context=new FlowDesk.AppContext();
  var processes=new List<Process>();var handles=new List<IntPtr>();
  var model=new WorkspaceModel{Name="Demo workspace",Split=true,Ratio=.5};var host=Program.Context.Open(model);host.TopMost=true;host.Size=new Size(1280,760);var area=Screen.PrimaryScreen.WorkingArea;host.Location=new Point(area.Left+(area.Width-host.Width)/2,area.Top+(area.Height-host.Height)/2);
  try{int index=0;foreach(string name in new[]{"Notes","Checklist"}){
   var process=Process.Start(new ProcessStartInfo(Application.ExecutablePath,"--fixture "+name){UseShellExecute=false});processes.Add(process);IntPtr handle=IntPtr.Zero;var wait=Stopwatch.StartNew();while(handle==IntPtr.Zero&&wait.ElapsedMilliseconds<5000){Application.DoEvents();Thread.Sleep(30);process.Refresh();handle=process.MainWindowHandle;}if(handle==IntPtr.Zero)throw new Exception("Demo app did not open");handles.Add(handle);
   host.Panes[index++].Attach(new Candidate{H=handle,ProcessId=process.Id,Started=process.StartTime.ToUniversalTime().Ticks,Title=name+" — native demo app",Path=Application.ExecutablePath,Icon=SystemIcons.Application},new TabModel{Name=name,Title=name+" — native demo app",Path=Application.ExecutablePath,Mode="Aligned"});
  }
  host.Activate();host.Sync(false);var paint=Stopwatch.StartNew();while(paint.ElapsedMilliseconds<1000){Application.DoEvents();Thread.Sleep(15);}
  string folder=Path.Combine(root,"docs","images");Directory.CreateDirectory(folder);using(var bitmap=new Bitmap(host.Width,host.Height)){host.DrawToBitmap(bitmap,host.ClientRectangle);Rect outer;Native.GetWindowRect(host.Handle,out outer);using(var canvas=Graphics.FromImage(bitmap)){for(int i=0;i<handles.Count;i++){IntPtr handle=handles[i];if(!Native.IsWindow(handle)||Native.Pid(handle)!=processes[i].Id)throw new Exception("Demo window identity changed");Rect rect;Native.GetWindowRect(handle,out rect);using(var appImage=new Bitmap(rect.Right-rect.Left,rect.Bottom-rect.Top)){using(var graphics=Graphics.FromImage(appImage)){IntPtr dc=graphics.GetHdc();try{if(!PrintWindow(handle,dc,2))throw new Exception("Cannot render disposable native window");}finally{graphics.ReleaseHdc(dc);}}canvas.DrawImageUnscaled(appImage,rect.Left-outer.Left,rect.Top-outer.Top);}}}bitmap.Save(Path.Combine(folder,"split-workspace.png"));}
  Console.WriteLine("Saved sanitized live-native demo screenshot.");
  }finally{host.TopMost=false;host.Close();foreach(var handle in handles)if(Native.IsWindow(handle))Native.PostMessage(handle,0x10,IntPtr.Zero,IntPtr.Zero);foreach(var process in processes){if(!process.WaitForExit(1000))process.Kill();process.Dispose();}}
 }
}
