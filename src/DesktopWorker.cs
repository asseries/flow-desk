using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;
using System.Security.Cryptography;
class DesktopWorker {
 [ComImport,Guid("A5CD92FF-29BE-454C-8D04-D82879FB3F1B"),InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]interface DesktopIds {[PreserveSig]int IsCurrent(IntPtr h,[MarshalAs(UnmanagedType.Bool)]out bool current);[PreserveSig]int GetId(IntPtr h,out Guid id);[PreserveSig]int MoveTo(IntPtr h,ref Guid id);}
 static Guid PublicId(IntPtr window){object manager=Activator.CreateInstance(Type.GetTypeFromCLSID(new Guid("AA509086-5CA9-4C25-8F95-589D3C07B48A")));try{Guid id;int result=((DesktopIds)manager).GetId(window,out id);if(result!=0)Marshal.ThrowExceptionForHR(result);return id;}finally{Marshal.ReleaseComObject(manager);}}
 [DllImport("kernel32.dll",CharSet=CharSet.Unicode,SetLastError=true)] static extern IntPtr LoadLibraryEx(string path,IntPtr file,uint flags);
 [DllImport("kernel32.dll")] static extern IntPtr GetProcAddress(IntPtr module,string name);
 [DllImport("kernel32.dll")]static extern uint SetErrorMode(uint mode);
 [DllImport("user32.dll")]static extern bool IsWindow(IntPtr h);
 [DllImport("user32.dll")]static extern uint GetWindowThreadProcessId(IntPtr h,out uint pid);
 [UnmanagedFunctionPointer(CallingConvention.Cdecl)]delegate int Number();
 [UnmanagedFunctionPointer(CallingConvention.Cdecl)]delegate int WindowNumber(IntPtr window);
 [UnmanagedFunctionPointer(CallingConvention.Cdecl)]delegate int Move(IntPtr window,int desktop);
 [UnmanagedFunctionPointer(CallingConvention.Cdecl)]delegate Guid DesktopId(int desktop);
 static T Export<T>(IntPtr module,string name){var address=GetProcAddress(module,name);if(address==IntPtr.Zero)throw new Exception("Missing export: "+name);return (T)(object)Marshal.GetDelegateForFunctionPointer(address,typeof(T));}
 static bool Valid(IntPtr window,int pid,long ticks){try{uint actual;GetWindowThreadProcessId(window,out actual);return IsWindow(window)&&actual==pid&&Process.GetProcessById(pid).StartTime.ToUniversalTime().Ticks==ticks&&Process.GetProcessById(pid).SessionId==Process.GetCurrentProcess().SessionId;}catch{return false;}}
 [STAThread]static int Main(string[] args){try{
  SetErrorMode(3);
  string path=Path.Combine(AppDomain.CurrentDomain.BaseDirectory,"native","VirtualDesktopAccessor.dll");
  if(!File.Exists(path))throw new Exception("Local virtual desktop DLL is missing");
  using(var hash=SHA256.Create())using(var file=File.OpenRead(path)){string actual=BitConverter.ToString(hash.ComputeHash(file)).Replace("-","");if(actual!="8740C572A1C000E3B87FFEB1E4C397EAE9AF3BD4A2ABDC3BCFFACAB4493F8FF5")throw new Exception("Local desktop DLL hash does not match the bundled release");}
  var module=LoadLibraryEx(path,IntPtr.Zero,0x1100);if(module==IntPtr.Zero)throw new Exception("Cannot load local DLL: "+Marshal.GetLastWin32Error());
  var count=Export<Number>(module,"GetDesktopCount");var current=Export<Number>(module,"GetCurrentDesktopNumber");var number=Export<WindowNumber>(module,"GetWindowDesktopNumber");var move=Export<Move>(module,"MoveWindowToDesktopNumber");
  int total=count(),active=current();if(total<1||total>256||active<0||active>=total)throw new Exception("Virtual desktop interfaces are not compatible with this Windows build");
  if(args.Length==1&&args[0]=="--probe"){Console.WriteLine("OK|"+total+"|"+active);return 0;}
  if(args.Length!=7||args[0]!="--move")throw new Exception("Invalid command");
  var target=new IntPtr(long.Parse(args[1]));int pid=int.Parse(args[2]);long started=long.Parse(args[3]);if(!Valid(target,pid,started))throw new Exception("Window identity changed");
  int destination;
  if(args[4]=="desktop")destination=int.Parse(args[5]);
  else{var host=new IntPtr(long.Parse(args[4]));int hostPid=int.Parse(args[5]);long hostStarted=long.Parse(args[6]);if(!Valid(host,hostPid,hostStarted))throw new Exception("Workspace identity changed");destination=number(host);}
  var idByNumber=Export<DesktopId>(module,"GetDesktopIdByNumber");
  if(args[4]=="desktop"){Guid expected;if(Guid.TryParse(args[6],out expected)){destination=-1;for(int i=0;i<total;i++)if(idByNumber(i)==expected){destination=i;break;}}}
  int original=number(target);if(original<0||original>=total||destination<0||destination>=total)throw new Exception("Desktop cannot be identified or original desktop was removed");
  Guid originalId=PublicId(target);if(originalId==Guid.Empty||idByNumber(original)!=originalId)throw new Exception("Desktop identity validation failed for this Windows build");
  if(original!=destination){if(move(target,destination)!=1)throw new Exception("Windows internal move failed");var watch=Stopwatch.StartNew();while(watch.ElapsedMilliseconds<1200&&Valid(target,pid,started)&&number(target)!=destination)Thread.Sleep(20);if(!Valid(target,pid,started)||number(target)!=destination)throw new Exception("Move was not confirmed");}
  Console.WriteLine("OK|"+original+"|"+destination+"|"+originalId.ToString("D"));return 0;
 }catch(Exception e){Console.WriteLine("ERROR|"+e.Message.Replace('\r',' ').Replace('\n',' '));return 1;}}
}
