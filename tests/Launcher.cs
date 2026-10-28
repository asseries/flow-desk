using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;
using FlowDesk;
class LauncherTest {
 [STAThread] static int Main(string[] args){
  Application.EnableVisualStyles();
  if(args.Length>0&&args[0]=="--fixture"){using(var f=new Form{Text="Flow Desk disposable launch test"}){var timer=new System.Windows.Forms.Timer{Interval=10000};timer.Tick+=delegate{f.Close();};timer.Start();Application.Run(f);timer.Dispose();}return 0;}
  var before=new HashSet<IntPtr>{new IntPtr(1)};
  var one=new Candidate{H=new IntPtr(1),Path="C:\\same.exe"};var two=new Candidate{H=new IntPtr(2),Path="C:\\same.exe"};var three=new Candidate{H=new IntPtr(3),Path="C:\\same.exe"};
  var batch=WindowSelection.Build(new[]{one,two,three},candidate=>candidate==two);if(batch.Allowed.Count!=1||batch.Allowed[0]!=two||batch.Skipped.Count!=2)throw new Exception("Blocked first window aborted whole batch");
  if(AppCatalog.UniqueNewWindow(new[]{one},before,"C:\\same.exe",0)!=null)throw new Exception("Existing window selected automatically");
  if(AppCatalog.UniqueNewWindow(new[]{one,two},before,"C:\\same.exe",0)!=two)throw new Exception("Unique executable window not matched");
  if(AppCatalog.UniqueNewWindow(new[]{two,three},before,"C:\\same.exe",0)!=null)throw new Exception("Ambiguous windows selected automatically");
  two.OtherDesktop=true;if(AppCatalog.UniqueNewWindow(new[]{two},before,"C:\\same.exe",0)!=null)throw new Exception("Other-desktop window auto-attached");
  two.OtherDesktop=false;if(AppCatalog.UniqueNewWindow(new[]{two},before,null,0)!=null)throw new Exception("Unidentified packaged app matched arbitrarily");
  string shortcut=Path.Combine(AppDomain.CurrentDomain.BaseDirectory,"launcher-test.lnk");
  object shell=Activator.CreateInstance(Type.GetTypeFromProgID("WScript.Shell")),link=null;Process child=null;Candidate found=null;
  try{dynamic automation=shell;link=automation.CreateShortcut(shortcut);dynamic lnk=link;lnk.TargetPath=Application.ExecutablePath;lnk.Arguments="--fixture";lnk.Save();
   var entry=InstalledApp.FromFile(shortcut);if(entry.ExpectedPath!=Application.ExecutablePath)throw new Exception("Shortcut target not resolved");
   var existing=new HashSet<IntPtr>(Candidate.All(true).Select(c=>c.H));child=entry.Launch();var clock=Stopwatch.StartNew();while(clock.ElapsedMilliseconds<7000&&found==null){Thread.Sleep(100);found=AppCatalog.UniqueNewWindow(Candidate.All(),existing,entry.ExpectedPath,0);}
   if(found==null)throw new Exception("Launched shortcut window not discovered");
   if(!Candidate.All(true).Any(c=>c.H==found.H))throw new Exception("All-desktop enumeration lost current-desktop window");
  }finally{if(found!=null)Native.PostMessage(found.H,0x10,IntPtr.Zero,IntPtr.Zero);if(child!=null)child.Dispose();if(link!=null)Marshal.ReleaseComObject(link);Marshal.ReleaseComObject(shell);if(File.Exists(shortcut))File.Delete(shortcut);}
  var catalog=AppCatalog.Read();if(catalog.Count==0)throw new Exception("Installed app catalog empty");Console.WriteLine("PASS safe launch matching, shortcut launch + real window discovery, all-desktop enumeration; catalog="+catalog.Count+", Windows apps="+catalog.Count(a=>a.Packaged));return 0;
 }
}
