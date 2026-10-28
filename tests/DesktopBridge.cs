using System;
using System.Diagnostics;
using System.Threading;
using System.Windows.Forms;
using FlowDesk;
class DesktopBridgeTest {
 [STAThread]static int Main(string[] args){if(args.Length==2&&args[0]=="--guard"){FlowDesk.Program.Main(args);return 0;}Application.EnableVisualStyles();if(args.Length>0&&args[0]=="--fixture"){using(var window=new Form{Text="Flow Desk disposable desktop move fixture"}){var timer=new System.Windows.Forms.Timer{Interval=20000};timer.Tick+=delegate{window.Close();};timer.Start();Application.Run(window);timer.Dispose();}return 0;}
  int count,current;string error;if(!DesktopBridge.Probe(out count,out current,out error))throw new Exception(error);if(count<2)throw new Exception("Need two existing desktops for cross-process integration test");
  using(var host=new Form{Text="Desktop transport regression"})using(var child=Process.Start(new ProcessStartInfo(Application.ExecutablePath,"--fixture"){UseShellExecute=false,CreateNoWindow=true})){
   host.Show();Application.DoEvents();IntPtr target=IntPtr.Zero;var clock=Stopwatch.StartNew();while(clock.ElapsedMilliseconds<5000&&target==IntPtr.Zero){Thread.Sleep(50);child.Refresh();target=child.MainWindowHandle;}if(target==IntPtr.Zero)throw new Exception("Disposable fixture did not expose its window");long started=child.StartTime.ToUniversalTime().Ticks;
   try{int other=(current+1)%count;if(!DesktopBridge.Restore(target,child.Id,started,other,out error))throw new Exception("Cross-process move away failed: "+error);var candidate=new Candidate{H=target,Title="Disposable desktop fixture",Path=Application.ExecutablePath,OtherDesktop=true};if(!DesktopAccess.BringCandidate(candidate,host.Handle,out error))throw new Exception("Cross-process import failed: "+error);if(candidate.OriginalDesktop!=other||string.IsNullOrEmpty(candidate.OriginalDesktopId))throw new Exception("Original desktop identity not captured");var surface=new Panel{Dock=DockStyle.Fill};host.Controls.Add(surface);var managed=new ManagedWindow(candidate,new TabModel{Mode="Aligned"},surface,host);managed.Place(surface,true);managed.Detach();int returnedFrom;if(!DesktopBridge.MoveHere(target,host.Handle,out returnedFrom,out error)||returnedFrom!=other)throw new Exception("Detach did not restore original desktop: "+error);int afterCount,afterCurrent;if(!DesktopBridge.Probe(out afterCount,out afterCurrent,out error)||afterCurrent!=current)throw new Exception("Current virtual desktop changed during move test");Console.WriteLine("PASS local DLL: "+count+" desktops; cross-process import + attach/detach + original-desktop GUID restoration; no desktop switch");}
   finally{string ignored;DesktopBridge.Restore(target,child.Id,started,current,out ignored);Native.PostMessage(target,0x10,IntPtr.Zero,IntPtr.Zero);if(!child.WaitForExit(1000))child.Kill();host.Close();}
  }return 0;
 }

}
